using HtmlAgilityPack;
using Live.Core;
using Live.Core.BookStores;
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
            WebClient client = new WebClient();
            string htmlCode = "";
            client.Headers.Add("User-Agent: Other");

            await Task.Run(() =>
            {
                htmlCode = client.DownloadString(url);
            });

            var htmlDoc = new HtmlDocument();

            await Task.Run(() =>
            {
                htmlDoc.LoadHtml(htmlCode);
            });

            var bestBooks = htmlDoc.DocumentNode.SelectNodes("//div[@class='product-box text-center product-box-default']");

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
                    
                    //Console.WriteLine(titleWithAuthor);
                    //Console.WriteLine(imgSrc);
                    //Console.WriteLine(src);

                    //var book = new Book(title, author, src, "Profit24");
                    //await book.SetSizeAsync();
                    //bookList.Add(book);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return bookList;
        }
    }
}
