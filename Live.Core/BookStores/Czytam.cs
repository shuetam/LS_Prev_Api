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
    public class Czytam : BookStore
    {
        public override async Task<List<Book>> GetBestsellersAsync()
        {
            var bookList = new List<Book>();
            string url = "https://czytam.pl/bestsellery.html";
            WebClient client = new WebClient(){ Encoding = System.Text.Encoding.UTF8 };
            string htmlCode = "";

        try {
            await Task.Run(() =>
            {
                htmlCode = client.DownloadString(url);
            });
        }
        catch(Exception ex) {
            Log.Error($"Error in Czytam: {ex.Message}");
            Log.Error(ex.StackTrace);
        }

if(!string.IsNullOrEmpty(htmlCode))
{
            var htmlDoc = new HtmlDocument();

            await Task.Run(() =>
            {
                htmlDoc.LoadHtml(htmlCode);
            });

            var bestBooks = htmlDoc.DocumentNode.SelectNodes("//div[@class='product']");

    if(bestBooks != null)
    {
            foreach (var bestNode in bestBooks)
            {
                
                    var bookOut = bestNode.OuterHtml;
                    var doc = new HtmlDocument();
                    doc.LoadHtml(bookOut);

                    var title = doc.DocumentNode.SelectSingleNode("//span[@itemprop='name']").InnerText.Trim();
                    var author = doc.DocumentNode.SelectSingleNode("//span[@class='product-author']").InnerText.Trim();
                    var imgSrc = doc.DocumentNode.SelectSingleNode("//img[@itemprop='image']").Attributes["src"].Value.Trim();

                    var imgToChange = imgSrc.Split("/").LastOrDefault();
                    var imgChanged = "";

                    if(imgToChange[0] == 'm')
                    {
                       imgChanged = "d" + imgToChange.Remove(0, 1);
                    }
                    else
                    {
                        imgChanged = "d" + imgToChange;
                    }
                   var reg = new Regex(imgToChange);


                   var img =  reg.Replace(imgSrc, imgChanged);

                    var book = new Book(title, author, img, "Czytam");
                    await book.SetSizeAsync();
                    bookList.Add(book);
                //Console.WriteLine(book.Title);
            }
            }
    }

            return bookList;
        }
    }
}
