using HtmlAgilityPack;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
            WebClient client = new WebClient(){ Encoding = System.Text.Encoding.UTF8 };
            string htmlCode = "";

        try{
            await Task.Run(() =>
            {
                htmlCode = client.DownloadString(url);
            });
        }
        catch(Exception ex)
        {
            Log.Error($"Eror in Aros: {ex.Message}");
            Log.Error(ex.StackTrace);
        }

            var htmlDoc = new HtmlDocument();

if(!string.IsNullOrEmpty(htmlCode))
{
            await Task.Run(() =>
            {
                htmlDoc.LoadHtml(htmlCode);
            });

            var bestSel = htmlDoc.DocumentNode.SelectNodes("//b").FirstOrDefault(x => x.InnerText == "Bestsellery");

            var bestListhtml = bestSel.ParentNode.ParentNode.InnerHtml;

            htmlDoc.LoadHtml(bestListhtml);

            var bestList = htmlDoc.DocumentNode.SelectNodes("//tr");

    if(bestList != null)
    {
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

                        WebClient clientBook = new WebClient(){ Encoding = System.Text.Encoding.UTF8 };
                        string htmlBook = "";

                    using (WebClient client1 = new WebClient())
                    {
                        var htmlData = client1.DownloadData(urlAddress);
                        htmlBook = Encoding.UTF8.GetString(htmlData);
                    }


                    //Console.WriteLine(htmlBook);
                    //Console.WriteLine("===============================================================");
                        var htmlDocBook = new HtmlDocument();
                      
                        htmlDocBook.LoadHtml(htmlBook);

                        var titleNode = htmlDocBook.DocumentNode.SelectSingleNode("//h1");

                        //titleNode.Attributes
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
                    //Console.WriteLine(book.Title);

                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Exception in Aros: {e.Message}");
                    Log.Error(e.StackTrace);

                }
            }
    }
}

            return bookList;
        }
    }
}
