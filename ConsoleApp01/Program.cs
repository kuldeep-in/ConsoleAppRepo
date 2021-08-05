using ConsoleApp01.AzDevOps;
using ConsoleApp01.CosmosDB;
using ConsoleApp01.DataStructure;
using ConsoleApp01.StorageAccount;
using System;
using System.Threading.Tasks;

namespace ConsoleApp01
{
    class Program
    {
        //static void Main(string[] args)
        static async Task Main(string[] args)
        {

            //CopyDashboard.CopyDashboards();
            //DSQuestions.comparestring("ABACAB", "");
            //DSQuestions.ReverseString("abcddca");
            //Console.WriteLine("Completed. Press enter to Exit");
            //GenerateTestDataTableStorage.GenerateTestData();
            //ConsoleApp01.Web.ReadWebContent.ReadWebPageContent();

            CosmosOperation demo = new CosmosOperation();

            await demo.ExecuteSP();

            Console.ReadLine();
        }



    }
}
