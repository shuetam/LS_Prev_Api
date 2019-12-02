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
    public class Empik: BookStore
    {
        public override async Task<List<Book>> GetBestsellersAsync()
        {
            var bookList = new List<Book>();
            string url = "https://www.empik.com/bestsellery/ksiazki";
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

            var bestBooks = htmlDoc.DocumentNode.SelectNodes("//div[@class='productWrapper']");

            foreach (var bestNode in bestBooks)
            {
                try
                {
                    var bookOut = bestNode.OuterHtml;
                    var doc = new HtmlDocument();
                    doc.LoadHtml(bookOut);

                    var title = doc.DocumentNode.SelectSingleNode("//strong[@class='ta-product-title']").InnerText.Trim();
                    var author = doc.DocumentNode.SelectSingleNode("//div[@class='smartAuthorWrapper ta-product-smartauthor']").InnerText.Trim();
                    var imgSrc = doc.DocumentNode.SelectSingleNode("//img[@class='lazy']").Attributes["lazy-img"].Value.Trim();

                    var rep = new Regex("-p-");
                    var img =  rep.Replace(imgSrc, "-w-");

                    var regTitle = new Regex("^[1-9]+[\n]{1}");
                    var regAuthor = new Regex("[\n]+");
                    var regAuthor1 = new Regex(" ,");
                    author = regAuthor.Replace(author, "");
                    author = regAuthor1.Replace(author, ", ");
                    title = regTitle.Replace(title, "");

                    var book = new Book(title, author, img, "Empik");
                    await book.SetSizeAsync();
                    bookList.Add(book);

                }
                catch (Exception e)
                {

                }
            }

            return bookList;
        }
    }
}
