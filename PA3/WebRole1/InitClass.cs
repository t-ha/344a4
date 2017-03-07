﻿using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WebRole1
{
    public class InitClass
    {
        public CloudQueue urlQueue { get; set; }
        public CloudQueue adminQueue { get; set; }
        public CloudQueue statsQueue { get; set; }
        public CloudQueue errorQueue { get; set; }
        public CloudQueue statusQueue { get; set; }
        public CloudTable titlesTable { get; set; }
        public CloudBlobContainer container { get; set; }

        public InitClass()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            urlQueue = queueClient.GetQueueReference("urlqueue");
            urlQueue.CreateIfNotExists();
            adminQueue = queueClient.GetQueueReference("adminqueue");
            adminQueue.CreateIfNotExists();
            statsQueue = queueClient.GetQueueReference("statsqueue");
            statsQueue.CreateIfNotExists();
            errorQueue = queueClient.GetQueueReference("errorqueue");
            errorQueue.CreateIfNotExists();
            statusQueue = queueClient.GetQueueReference("statusqueue");
            statusQueue.CreateIfNotExists();

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            container = blobClient.GetContainerReference("pa2container");
            

            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            titlesTable = tableClient.GetTableReference("titlestable");
            titlesTable.CreateIfNotExists();
        }
    }
}