using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Live.Core.BookStores
{
    public class Aros : BookStore
    {
        public override async Task<List<Book>> GetBestsellersAsync()
        {
            var bookList = new List<Book>();
            string url = "https://aros.pl/";
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

            var bestSel = htmlDoc.DocumentNode.SelectNodes("//b").FirstOrDefault(x => x.InnerText == "Bestsellery");

            var bestListhtml = bestSel.ParentNode.ParentNode.InnerHtml;

            htmlDoc.LoadHtml(bestListhtml);

            var bestList = htmlDoc.DocumentNode.SelectNodes("//tr");

            foreach (var bestNode in bestList)
            {
                try
                {
                    var htmlDocSingle = new HtmlDocument();

                    htmlDocSingle.LoadHtml(bestNode.InnerHtml);
                    var href = htmlDocSingle.DocumentNode.SelectSingleNode("//a");

                    var urlAddress = href.Attributes["href"].Value;
                    urlAddress = "https://www.aros.pl" + urlAddress;

                    if (bestNode.InnerHtml.Contains("autor"))
                    {

                        WebClient clientBook = new WebClient();
                        string htmlBook = "";

                        await Task.Run(() =>
                        {
                            htmlBook = clientBook.DownloadString(urlAddress);
                        });

                        var htmlDocBook = new HtmlDocument();
                        htmlDocBook.LoadHtml(htmlBook);

                        var titleNode = htmlDocBook.DocumentNode.SelectSingleNode("//h1");
                        var title = titleNode.InnerHtml.Trim();
                        var mainNode = titleNode.ParentNode.ParentNode.ParentNode;
                        var authorNode = mainNode.InnerHtml;
                        var authorDoc = new HtmlDocument();
                            authorDoc.LoadHtml(authorNode);
                        var author = authorDoc.DocumentNode.SelectSingleNode("//b").InnerText;
                        var imgSrc = authorDoc.DocumentNode.SelectNodes("//img")
                            .FirstOrDefault(x => x.Attributes["alt"].Value == title).Attributes["src"].Value;
                            
                        //Attributes["alt"].Value;
                        imgSrc = "https:" + imgSrc;
                       

                        var book = new Book(title, author, imgSrc, "Aros");
                        await book.SetSizeAsync();
                        bookList.Add(book);


                    }

                    //  var parent = bestNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.ParentNode.InnerHtml.Trim();
                    //        var htmlParent = new HtmlDocument();
                    //        htmlParent.LoadHtml(parent);
                    //        var img = "https:" + htmlParent.DocumentNode.SelectSingleNode("//img").Attributes["src"].Value;
                    //        var author = htmlParent.DocumentNode.SelectNodes("//a").FirstOrDefault(x => x.Attributes["title"].Value.Contains("autora")).InnerText.Trim();

                    //        bookList.Add(new Book(title, author, img, "Bonito"));
                    //        var tt = "";
                }
                catch (Exception e)
                {

                }
            }

            return bookList;
        }
    }
}
