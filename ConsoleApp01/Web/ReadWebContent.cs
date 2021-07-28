using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp01.Web
{
    public class ReadWebContent
    {
        public static void ReadWebPageContent()
        {
            System.Net.WebClient webClient = new System.Net.WebClient
            {
                Encoding = System.Text.Encoding.UTF8
            };

            string url = string.Format("https://www.credly.com/users/singhkuldeep/badges");


            string result = webClient.DownloadString(url);

            var _doc = new HtmlDocument();
            _doc.LoadHtml(result);

            IEnumerable<HtmlNode> nodes = _doc.DocumentNode.Descendants().Where(n => n.HasClass("data-table-row-grid"));

            foreach (var item in nodes)
            {
                var item01 = item.Descendants().Where(n => n.HasClass("cr-standard-grid-item-content__title")).FirstOrDefault();
                var item02 = item.Descendants().Where(n => n.HasClass("cr-standard-grid-item-content__subtitle")).FirstOrDefault();
                var item03 = item.Descendants("img").FirstOrDefault();
                var item04 = item.Descendants("a").FirstOrDefault();

                Console.WriteLine(item01.InnerText.Trim());
                Console.WriteLine(item02.InnerText.Trim());
                Console.WriteLine(item03.Attributes["src"].Value);
                Console.WriteLine(item04.Attributes["href"].Value);
                Console.WriteLine();

            }
        }
    }
}
