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
    public class Profit24: BookStore
    {
        public override async Task<List<Book>> GetBestsellersAsync()
        {
            var bookList = new List<Book>();
            string url = "https://www.profit24.pl/bestsellery-dzial-ogolny";
            WebClient client = new WebClient(){ Encoding = System.Text.Encoding.UTF8 };
            string htmlCode = "";
            client.Headers.Add("User-Agent: Other");

        try {

            await Task.Run(() =>
            {
                htmlCode = client.DownloadString(url);
            });
            }
            catch(Exception ex)
            {
                Log.Error($"Error in Profit24: {ex.Message}");
                Log.Error(ex.StackTrace);
            }

            var htmlDoc = new HtmlDocument();

            await Task.Run(() =>
            {
                htmlDoc.LoadHtml(htmlCode);
            });

            var bestBooks = htmlDoc.DocumentNode.SelectNodes("//div[@class='produktLista']");

        if(bestBooks != null)
        {
            foreach (var bestNode in bestBooks)
            {
                try
                {
                    var bookOut = bestNode.OuterHtml;
                    var doc = new HtmlDocument();
                    doc.LoadHtml(bookOut);

                    var title = doc.DocumentNode.SelectSingleNode("//div[@class='produktListaTytul']").InnerText.Trim();
                    var author = doc.DocumentNode.SelectSingleNode("//div[@class='produktListaAutorzy']").InnerText.Trim();
                    var imgSrcHtml = doc.DocumentNode.SelectSingleNode("//div[@class='produktListaZdjecie']").OuterHtml;
                    
                    var imgDoc = new HtmlDocument();

                    imgDoc.LoadHtml(imgSrcHtml);

                    var srcImg = imgDoc.DocumentNode.SelectSingleNode("//img").Attributes["src"].Value;

                    var regWhites = new Regex("\\s{2,}");
                    author = regWhites.Replace(author, "");
                    title = regWhites.Replace(title, "");
                    author = new Regex("Autor[:]{1}").Replace(author, "");

                    var src = "https://www.profit24.pl" + new Regex("[_]{2}t[_]{1}").Replace(srcImg, "__b_");
  
                    var book = new Book(title, author, src, "Profit24");
                    await book.SetSizeAsync();
                    bookList.Add(book);
                Console.WriteLine(book.Title);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                     Log.Error(e.StackTrace);
                }
            }
        }

            return bookList;
        }
    }
}
