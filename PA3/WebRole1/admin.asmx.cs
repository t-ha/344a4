using System.Collections.Generic;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using Microsoft.WindowsAzure.Storage.Queue;
using System.Threading;
using System;
using Microsoft.WindowsAzure.Storage.Table;
using System.Text;
using System.Diagnostics;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WebRole1
{
    /// <summary>
    /// Summary description for admin
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 

    [System.Web.Script.Services.ScriptService]
    public class admin : System.Web.Services.WebService
    {
        private PerformanceCounter memProcess = new PerformanceCounter("Memory", "Available MBytes");
        private static InitClass ic = new InitClass();
        private List<string> stats = new List<string>();
        private static string status = null;
        private static string[] errorMessage = new string[2];
        private static string wikiFileStream { get; set; }
        private static Trie wikiTrie;
        private static string lastTitle = "NaN";

        [WebMethod]
        public void LoadCrawler()
        {
            ic.statusQueue.AddMessage(new CloudQueueMessage("...Loading sitemaps..."));
            ic.adminQueue.AddMessage(new CloudQueueMessage("load"));
        }

        [WebMethod]
        public void StartCrawling()
        {
            ic.statusQueue.AddMessage(new CloudQueueMessage("Crawling"));
            ic.adminQueue.AddMessage(new CloudQueueMessage("start"));
            
        }

        [WebMethod]
        public void StopCrawling()
        {
            ic.statusQueue.AddMessage(new CloudQueueMessage("Idle"));
            ic.adminQueue.AddMessage(new CloudQueueMessage("stop"));
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string ClearIndex()
        {
            ic.statusQueue.AddMessage(new CloudQueueMessage("Idle and cleared"));
            ic.adminQueue.AddMessage(new CloudQueueMessage("clear"));
            ic.urlQueue.Clear();
            ic.errorQueue.Clear();
            ic.statsQueue.Clear();
            TableQuery<PageTitle> query = new TableQuery<PageTitle>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.NotEqual, "00"));
            foreach (PageTitle entity in ic.titlesTable.ExecuteQuery(query))
            {
                ic.titlesTable.Execute(TableOperation.Delete(entity));
            }
            ic.adminQueue.Clear();
            ic.statusQueue.Clear();
            return new JavaScriptSerializer().Serialize("All queues and tables cleared. Workers stopped.");
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetUrls(string query)
        {
            HashSet<Tuple<string, string, string>> urls = new HashSet<Tuple<string, string, string>>();
            string[] tokens = query.Trim().Split(' ');
            foreach (string token in tokens)
            {
                TableQuery<UrlEntity> querys = new TableQuery<UrlEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, token.ToLower()));
                foreach (UrlEntity entity in ic.titlesTable.ExecuteQuery(querys))
                {
                    var page = Tuple.Create(entity.Title, entity.Url, entity.Date.Date.ToString());
                    if (!urls.Contains(page))
                    {
                        urls.Add(page);
                    }
                }
            }
            
            return new JavaScriptSerializer().Serialize(urls);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetPageTitle(string url)
        {
            string pageTitle = "";
            string encodedUrl = Encode64(url);
            TableQuery<PageTitle> query = new TableQuery<PageTitle>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, encodedUrl));
            foreach (PageTitle entity in ic.titlesTable.ExecuteQuery(query))
            {
                pageTitle = entity.Title;
            }
            return new JavaScriptSerializer().Serialize(pageTitle);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string CheckStatus()
        {
            CloudQueueMessage msg = ic.statusQueue.GetMessage();
            if (msg != null)
            {
                status = msg.AsString;
                ic.statusQueue.DeleteMessage(msg);
            }
            return new JavaScriptSerializer().Serialize(status);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetStats()
        {
            CloudQueueMessage msg = ic.statsQueue.GetMessage();
            if (msg != null)
            {
                stats = new List<string>();
                string messageString = msg.AsString;
                foreach (string stat in messageString.Split(','))
                {
                    stats.Add(stat);
                }
                ic.statsQueue.DeleteMessage(msg);
            }
            return new JavaScriptSerializer().Serialize(stats);
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string GetErrors()
        {
            CloudQueueMessage msg = ic.errorQueue.GetMessage();
            if (msg != null)
            {
                errorMessage = msg.AsString.Split('|');
                ic.errorQueue.DeleteMessage(msg);
            }
            return new JavaScriptSerializer().Serialize(errorMessage);
        }


        private string Encode64(string plaintText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plaintText));
        }

        [WebMethod]
        public string DownloadWiki()
        {
            CloudBlockBlob blockBlob = ic.container.GetBlockBlobReference("wikiTitles.txt");
            using (var fileStream = System.IO.File.OpenWrite(System.IO.Path.GetTempFileName()))
            {
                wikiFileStream = fileStream.Name;
                blockBlob.DownloadToStream(fileStream);
            }

            return "Download complete.";
        }

        private float GetAvailableMBytes()
        {
            float memUsage = memProcess.NextValue();
            return memUsage;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string TrieStats()
        {
            List<string> tStats = new List<string>();
            if (wikiTrie != null)
            {
                tStats.Add(wikiTrie.Count.ToString());
            } else
            {
                tStats.Add("0");
            }
            tStats.Add(lastTitle);
            return new JavaScriptSerializer().Serialize(tStats);
        }

        [WebMethod]
        public string BuildTrie()
        {
            wikiTrie = new Trie();
            string line = "";
            using (StreamReader sr = new StreamReader(wikiFileStream))
            {
                float availableMBytes = GetAvailableMBytes();
                int iter = 0;
                while (sr.EndOfStream == false && availableMBytes > 20)
                {
                    line = sr.ReadLine();
                    wikiTrie.AddTitle(line);
                    if (iter % 10000 == 0)
                    {
                        availableMBytes = GetAvailableMBytes();
                    }
                    iter++;
                }
            }
            lastTitle = line;
            return "Trie built. Last title in trie: [" + lastTitle + "]";
        }


        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string SearchTrie(string prefix)
        {
            return new JavaScriptSerializer().Serialize(wikiTrie.SearchForPrefix(prefix));
        }
    }
}