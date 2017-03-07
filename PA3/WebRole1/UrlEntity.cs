using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WebRole1
{
    public class UrlEntity : TableEntity
    {
        public string Url { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }

        public UrlEntity(string title, string pagetitleToken, string url, DateTime date)
        {
            this.PartitionKey = pagetitleToken.ToLower();
            this.RowKey = Guid.NewGuid().ToString();

            this.Url = url;
            this.Date = date;
            this.Title = title;
        }

        public UrlEntity() { }
    }
}