using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Live.Core.BookStores
{
    public class Bonito : BookStore
    {
        public override async Task<List<Book>> GetBestsellersAsync()
        {
            var bookList = new List<Book>();
            string url = "https://bonito.pl/bestsellery";
            WebClient client = new WebClient();
            string htmlCode = "";

            await Task.Run(() =>
            {
                htmlCode = client.DownloadString(url);
            });

            var htmlDoc = new HtmlDocument();

            await Task.Run(() =>
            {
                htmlDoc.LoadHtml(htmlCode);
            });

            var bestListParents = htmlDoc.DocumentNode.SelectNodes("//tr").Where(x => x.OuterHtml.Contains("<h2")).ToList();
            var bestList = htmlDoc.DocumentNode.SelectNodes("//h2");

            foreach (var bestNode in bestList)
            {
                try
                {
                    var title = bestNode.InnerText.Trim();

                    var parent = bestNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.InnerHtml.Trim();
                    var htmlParent = new HtmlDocument();
                    htmlParent.LoadHtml(parent);
                    var img = "https:" + htmlParent.DocumentNode.SelectSingleNode("//img").Attributes["src"].Value;
                    var author = htmlParent.DocumentNode.SelectNodes("//a").FirstOrDefault(x => x.Attributes["title"].Value.Contains("autora")).InnerText.Trim();
                    var book = new Book(title, author, img, "Bonito");
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
