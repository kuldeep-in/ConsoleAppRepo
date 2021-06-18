
namespace ConsoleApp01.AzDevOps
{
    using System;
    using System.Linq;
    using Microsoft.VisualStudio.Services.Common;
    using Microsoft.VisualStudio.Services.WebApi;
    using Microsoft.TeamFoundation.Core.WebApi;
    using Microsoft.TeamFoundation.Core.WebApi.Types;
    using Microsoft.TeamFoundation.Dashboards.WebApi;

    public static class CopyDashboard
    {
        public static void CopyDashboards()
        {
            //if (args.Length != 7)
            //{
            //    Console.WriteLine("Usage: copydashboard {orgUrl} {personalAccessToken} {projectName} {sourceTeamName} {sourceDashboardName} {targetTeamName} {targetDashboardName}");
            //    return;
            //}
            string[] args1 = { "https://dev.azure.com/xxx",
                                "{PAT Token}",
                                "{Source Proj}",
                                "{Source Team}",
                                "Overview",
                                "p02 Team",
                                "Overview01"
                             };
            Uri orgUrl = new Uri(args1[0]);         // Organization URL, for example: https://dev.azure.com/fabrikam               
            string personalAccessToken = args1[1];  // See https://docs.microsoft.com/azure/devops/integrate/get-started/authentication/pats
            string projectName = args1[2];          // Project Name
            string sourceTeamName = args1[3];       // Source Team Name
            string sourceDashboardName = args1[4];  // Source Dashboard to copy
            string targetTeamName = args1[5];       // Target Team Name
            string targetDashboardName = args1[6];  // Target Dashboard name

            string targetProjectName = "p02";
            Uri targetOrg = new Uri("https://dev.azure.com/adminorg");

            // Create a connection
            using VssConnection connection = new VssConnection(orgUrl, new VssBasicCredential(string.Empty, personalAccessToken));
            try
            {
                // Make sure we can connect before running the copy
                connection.ConnectAsync().SyncResult();
            }
            catch (VssServiceResponseException)
            {
                Console.Error.WriteLine("ERROR: Could not connect to {0}", orgUrl);
                return;
            }

            // Get WebApi client
            using DashboardHttpClient dashboardClient = connection.GetClient<DashboardHttpClient>();

            // Set source team context
            TeamContext sourceTeamContext = new TeamContext(projectName, sourceTeamName);

            // Get dashboard entries
            DashboardGroup dashboards;
            try
            {
                dashboards = dashboardClient.GetDashboardsAsync(sourceTeamContext).SyncResult();
            }
            catch (VssUnauthorizedException)
            {
                Console.Error.WriteLine("ERROR: Not authorized. Invalid token or invalid scope");
                return;
            }
            catch (ProjectDoesNotExistWithNameException)
            {
                Console.Error.WriteLine("ERROR: Project not found: {0}", projectName);
                return;
            }
            catch (TeamNotFoundException)
            {
                Console.Error.WriteLine("ERROR: Team not found: {0}", sourceTeamName);
                return;
            }
            // Get dashboard by name
            Dashboard sourceDashboardEntry = dashboards.DashboardEntries.Single(d => d.Name == sourceDashboardName);
            if (sourceDashboardEntry == null)
            {
                Console.Error.WriteLine("ERROR: Dashboard not found: {0}", sourceDashboardName);
                return;
            }
            Dashboard sourceDashboard = dashboardClient.GetDashboardAsync(sourceTeamContext, (Guid)sourceDashboardEntry.Id).SyncResult();

            // get target team

            using VssConnection targetconnection = new VssConnection(targetOrg, new VssBasicCredential(string.Empty, personalAccessToken));
            try
            {
                // Make sure we can connect before running the copy
                targetconnection.ConnectAsync().SyncResult();
            }
            catch (VssServiceResponseException)
            {
                Console.Error.WriteLine("ERROR: Could not connect to {0}", orgUrl);
                return;
            }

            // Get WebApi client
            using DashboardHttpClient targetdashboardClient = targetconnection.GetClient<DashboardHttpClient>();


            WebApiTeam targetTeam;
            using TeamHttpClient teamClient = targetconnection.GetClient<TeamHttpClient>();
            try
            {
                targetTeam = teamClient.GetTeamAsync(targetProjectName, targetTeamName).SyncResult();
            }
            catch (TeamNotFoundException)
            {
                Console.Error.WriteLine("ERROR: Team not found: {0}", targetTeamName);
                return;
            }

            // replace source team id with target team id  
            // for widgets like Burndown or Backlog where team id is a parameter
            foreach (Widget w in sourceDashboard.Widgets)
            {
                if (w.Settings != null)
                {
                    w.Settings = w.Settings.Replace(sourceDashboard.OwnerId.ToString(), targetTeam.Id.ToString());
                }
            }

            // Set target team context
            TeamContext targetTeamContext = new TeamContext(targetProjectName, targetTeamName);

            // Create target dashboard
            Dashboard targetObj = new Dashboard
            {
                Name = targetDashboardName,
                Description = sourceDashboard.Description,
                Widgets = sourceDashboard.Widgets
            };
            try
            {
                targetdashboardClient.CreateDashboardAsync(targetObj, targetTeamContext).SyncResult();
            }
            catch (DuplicateDashboardNameException)
            {
                Console.Error.WriteLine("ERROR: Dashboard {0} already exists in {1} team", targetDashboardName, targetTeamName);
                return;
            }
        }
    }
}