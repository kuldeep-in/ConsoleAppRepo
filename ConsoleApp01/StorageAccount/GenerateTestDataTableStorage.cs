using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;

namespace ConsoleApp01.StorageAccount
{
    public static class GenerateTestDataTableStorage
    {
        static string storageconn = "DefaultEndpointsProtocol=https;AccountName=;AccountKey=;EndpointSuffix=core.windows.net";

        static string tablename = "powerbipoc01";

        public static void GenerateTestData()
        {
            CloudStorageAccount storageAcc = CloudStorageAccount.Parse(storageconn);
            CloudTableClient tblclient = storageAcc.CreateCloudTableClient(new TableClientConfiguration());
            CloudTable table = tblclient.GetTableReference(tablename);

            for (int i = 1; i <= 12; i++)
            {
                string partitionkey = "2016" + i.ToString("00");
                for (int j = 1; j <= 2000; j++)
                {
                    Console.WriteLine(i.ToString() + ":" + j.ToString());
                    int rowkey = 2016 * i * j;
                    string rowKeyString = rowkey.ToString();

                    Task.Run(async () =>
                    {
                        Random random = new Random();

                        Website website = new Website()
                        {
                            RowKey = rowKeyString,
                            PartitionKey = partitionkey,

                            SiteURL = "www.SiteURL" + partitionkey + rowKeyString + ".com",
                            OwnerName = "Owner" + rowKeyString,
                            IsDeleted = j / 3 == 1 ? false : true,
                            LastDate = DateTime.Now,
                            FileCount = random.Next(40, 100),
                            ActiveFileCount = random.Next(20, 100),
                            PageViewCount = random.Next(100, 10000),
                            VisitedPageCount = random.Next(100, 10000),
                            StorageUsed = random.Next(10000, 100000),
                            StorageAllocated = random.Next(10000, 100000),
                            OwnerPrincipalName = "ownerP" + rowKeyString,
                            RootTemplate = "Roottemp" + rowKeyString
                        };
                        await InsertTableEntity(table, website);
                    }).GetAwaiter().GetResult();
                }
            }

            Console.ReadKey();

        }

        public static async Task InsertTableEntity(CloudTable p_tbl, Website website)
        {
            TableOperation insertOperation = TableOperation.InsertOrMerge(website);
            TableResult result = await p_tbl.ExecuteAsync(insertOperation);
            //Console.WriteLine(website.SiteURL);
        }

        public class Website : TableEntity
        {
            public string SiteURL { get; set; }
            public string OwnerName { get; set; }
            public bool IsDeleted { get; set; }
            public DateTime LastDate { get; set; }
            public int FileCount { get; set; }
            public int ActiveFileCount { get; set; }
            public int PageViewCount { get; set; }
            public int VisitedPageCount { get; set; }
            public int StorageUsed { get; set; }
            public int StorageAllocated { get; set; }
            public string OwnerPrincipalName { get; set; }
            public string RootTemplate { get; set; }
        }
    }
}
