
namespace ConsoleApp01.AzDevOps
{
    using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
    using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
    using Microsoft.VisualStudio.Services.Common;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    public static class ReadWriteWorkItems
    {
        static Uri uri = new Uri("https://dev.azure.com/adminorg");
        static string personalAccessToken = "5ny2ekc6nvfazaeaia3urkhaabt3ebn6h5uyg3jq5t5awgyhyazq";

        public static void GetWorkItems()
        {
            string scrProject = "p01";

            Task.Run(async () =>
            {
                var workItems = await QueryWorkItems(scrProject);
                Console.WriteLine("Query Results: {0} items found", workItems.Count);

                for (int i = 0; i < workItems.Count(); i++)
                {
                    Console.WriteLine(
                        "{0}\t{1}\t{2}",
                        workItems[i].Id,
                        workItems[i].Fields["System.Title"],
                        workItems[i].Fields["System.State"]);
                }

            }).GetAwaiter().GetResult();

            //Console.Read();
        }

        public static void CreateWorkItems()
        {
            Console.WriteLine("Creating workitem...");
            string type = "Epic";
            string _UrlServiceCreate = $"https://dev.azure.com/adminorg/p02/_apis/wit/workitems/${type}?api-version=5.0";
            dynamic WorkItem = new List<dynamic>() {
                new
                {
                    op = "add",
                    path = "/fields/System.Title",
                    value = "Epic from API"
                }
            };

            var WorkItemValue = new StringContent(JsonConvert.SerializeObject(WorkItem), Encoding.UTF8, "application/json-patch+json");
            var JsonResultWorkItemCreated = HttpPost(_UrlServiceCreate, personalAccessToken, WorkItemValue);
            Console.WriteLine("Create workitem completed.");

        }

        public static string HttpPost(string urlService, string token, StringContent postValue)
        {
            try
            {
                string request = string.Empty;
                using (HttpClient httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", token))));
                    using (HttpRequestMessage httpRequestMessage = new HttpRequestMessage(new HttpMethod("POST"), urlService) { Content = postValue })
                    {
                        var httpResponseMessage = httpClient.SendAsync(httpRequestMessage).Result;
                        if (httpResponseMessage.IsSuccessStatusCode)
                            request = httpResponseMessage.Content.ReadAsStringAsync().Result;
                    }
                }
                return request;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<IList<WorkItem>> QueryWorkItems(string project)
        {
            var credentials = new VssBasicCredential(string.Empty, personalAccessToken);

            // create a wiql object and build our query
            var wiql = new Wiql()
            {
                // NOTE: Even if other columns are specified, only the ID & URL will be available in the WorkItemReference
                Query = "Select [Id] " +
                        "From WorkItems " +
                        "Where [Work Item Type] = 'Epic' " +
                        "And [System.TeamProject] = '" + project + "' ",
            };

            // create instance of work item tracking http client
            using (var httpClient = new WorkItemTrackingHttpClient(uri, credentials))
            {
                // execute the query to get the list of work items in the results
                var result = await httpClient.QueryByWiqlAsync(wiql).ConfigureAwait(false);
                var ids = result.WorkItems.Select(item => item.Id).ToArray();

                // some error handling
                if (ids.Length == 0)
                {
                    return Array.Empty<WorkItem>();
                }

                // build a list of the fields we want to see
                var fields = new[] { "System.Id", "System.Title", "System.State" };

                // get work items for the ids found in query
                return await httpClient.GetWorkItemsAsync(ids, fields, result.AsOf).ConfigureAwait(false);
            }
        }
    }
}
