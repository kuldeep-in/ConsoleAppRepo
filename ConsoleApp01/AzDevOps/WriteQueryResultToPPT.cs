
namespace ConsoleApp01.AzDevOps
{
    using Microsoft.Office.Core;
    using Microsoft.Office.Interop.PowerPoint;
    using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
    using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
    using Microsoft.VisualStudio.Services.Common;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class WriteQueryResultToPPT
    {
        static Uri uri = new Uri("https://dev.azure.com/xxx");
        static string personalAccessToken = "";

        public static void ResultToPPT()
        {
            string project = "PlanningDemo";

            Task.Run(async () =>
            {
                var workItems = await QueryOpenBugs(project);
                Console.WriteLine("Query Results: {0} items found", workItems.Count);

                foreach (var workItem in workItems)
                {
                    Console.WriteLine(
                        "{0}\t{1}\t{2}",
                        workItem.Id,
                        workItem.Fields["System.Title"],
                        workItem.Fields["System.State"]);

                    Application pptApp = new Application();
                    Presentation ppt = pptApp.Presentations.Add(Microsoft.Office.Core.MsoTriState.msoCTrue);
                    Slides slides;
                    _Slide slide;
                    TextRange objText;
                    TextRange objText1;

                    CustomLayout customLayout = ppt.SlideMaster.CustomLayouts[PpSlideLayout.ppLayoutText];

                    slides = ppt.Slides;
                    slide = slides.AddSlide(1, customLayout);

                    objText = slide.Shapes[1].TextFrame.TextRange;
                    objText.Text = workItem.Id.ToString() + " - " + workItem.Fields["System.Title"].ToString();

                    objText1 = slide.Shapes[2].TextFrame.TextRange;
                    objText1.Text = "State - " + workItem.Fields["System.State"].ToString();

                    ppt.SaveAs(@"D:\" + workItem.Id.ToString() + ".pptx", PpSaveAsFileType.ppSaveAsDefault, MsoTriState.msoCTrue);
                    ppt.Close();
                }
            }).GetAwaiter().GetResult();

            //Console.Read();
        }

        public static async Task<IList<WorkItem>> QueryOpenBugs(string project)
        {
            var credentials = new VssBasicCredential(string.Empty, personalAccessToken);

            // create a wiql object and build our query
            var wiql = new Wiql()
            {
                // NOTE: Even if other columns are specified, only the ID & URL will be available in the WorkItemReference
                Query = "Select [Id] " +
                        "From WorkItems " +
                        "Where [Work Item Type] = 'Bug' " +
                        "And [System.TeamProject] = '" + project + "' " +
                        "And [System.State] <> 'Closed' " +
                        "Order By [State] Asc, [Changed Date] Desc",
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
