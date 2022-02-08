using System;
using System.Collections.Generic;
using System.Text;
using ConsoleApp01.AzDevOps;
using ConsoleApp01.CosmosDB;
using ConsoleApp01.DataStructure;
using ConsoleApp01.StorageAccount;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.ApplicationInsights.WindowsServer.TelemetryChannel;
using Microsoft.ApplicationInsights.Extensibility;
using System.Threading;

namespace ConsoleApp01
{
    public static class AppInsights
    {
        public static void GenerateAppInsightsData()
        {
            int AppId = 0;
            try
            {
                //var telemetryClient = new TelemetryClient() { InstrumentationKey = "e229d237-a3f9-4b20-bca5-9140be2b3817" };

                //ServiceCollection services = new ServiceCollection();
                //services.AddSingleton(x => telemetryClient);

                //var provider = services.BuildServiceProvider();

                //var loggerFactory = new LoggerFactory();
                //loggerFactory.AddApplicationInsights(provider, LogLevel.Information);

                //var logger = loggerFactory.CreateLogger<Program>();
                //logger.LogInformation($"{0}a test from 0911 again...", i);
                //logger.LogError("a error test from 0911 again...");


                //Console.WriteLine("aaa");

                //telemetryClient.Flush();
                //System.Threading.Thread.Sleep(5000);


                IServiceCollection services = new ServiceCollection();
                var channel1 = new ServerTelemetryChannel();
                services.Configure<TelemetryConfiguration>(
                    (config) =>
                    {
                        config.TelemetryChannel = channel1;
                    }
                );

                services.AddLogging(builder =>
                {
                    builder.AddApplicationInsights("e229d237-a3f9-4b20-bca5-9140be2b3817");
                });

                var provider = services.BuildServiceProvider();
                var logger = provider.GetService<ILogger<Program>>();
                //int i = 0;
                System.Random rnd = new System.Random();
                bool status = false;

                while (AppId < 10)
                {
                    //status = NextBoolean(new System.Random());
                    string AppName = $"App{AppId}";
                    if (rnd.Next(3) == 0)
                        status = false;
                    else
                        status = true;

                    Console.WriteLine($"App: {++AppId} status: {status}");
                    logger.LogInformation("Status of application: {AppName} is {status}", AppName, status);
                    Thread.Sleep(1000);

                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
