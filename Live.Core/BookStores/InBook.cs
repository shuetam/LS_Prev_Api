using HtmlAgilityPack;
using Live.Core;
using Live.Core.BookStores;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Live.Live.Core.BookStores
{
    public class InBook: BookStore
    {
        public override async Task<List<Book>> GetBestsellersAsync()
        {
            var bookList = new List<Book>();
            string url = "https://www.inbook.pl/bestsellers/list/2";
            WebClient client = new WebClient(){ Encoding = System.Text.Encoding.UTF8 };
            string htmlCode = "";
            client.Headers.Add("User-Agent: Other");

            try{
            await Task.Run(() =>
            {
                htmlCode = client.DownloadString(url);
            });
            }
            catch (Exception ex){
                Log.Error($"Error in InBook: {ex.Message}");
                Log.Error(ex.StackTrace);
            }

 if(!string.IsNullOrEmpty(htmlCode))
    {
            var htmlDoc = new HtmlDocument();

            await Task.Run(() =>
            {
                htmlDoc.LoadHtml(htmlCode);
            });

            var bestBooks = htmlDoc.DocumentNode.SelectNodes("//div[@class='product-box text-center product-box-default']");

if(bestBooks != null)
{
            foreach (var bestNode in bestBooks)
            {
                try
                {
                    var bookOut = bestNode.OuterHtml;
                    var doc = new HtmlDocument();
                    doc.LoadHtml(bookOut);

                    var titleNode = doc.DocumentNode.SelectSingleNode("//a");
                    var titleWithAuthor = titleNode.Attributes["title"].Value;

                    var docHtml = new HtmlDocument();
                    docHtml.LoadHtml(bookOut);

                    var imgSrc = docHtml.DocumentNode.SelectSingleNode("//img").Attributes["src"].Value;
                    

                }
                catch (Exception e)
                {
                    //Console.WriteLine(e.Message);
                    Log.Error(e.StackTrace);
                }
            }
        }
    }

            return bookList;
        }
    }
}
