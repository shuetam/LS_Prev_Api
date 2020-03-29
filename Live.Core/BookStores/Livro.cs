using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Live.Core.BookStores
{
    public class Livro : BookStore
    {
        public async override Task<List<Book>> GetBestsellersAsync()
        {
            var bookList = new List<Book>();
            string url = "https://livro.pl/bestsellery.html";
            WebClient client = new WebClient(){ Encoding = System.Text.Encoding.UTF8 };
            string htmlCode = "";

        try {
            await Task.Run(() =>
            {
                htmlCode = client.DownloadString(url);
            });
        }
        catch {
            
        }

            var htmlDoc = new HtmlDocument();

            await Task.Run(() =>
            {
                htmlDoc.LoadHtml(htmlCode);
            });



            //Console.WriteLine(table.OuterHtml);
            // li class="item last"
            var bestList = htmlDoc.DocumentNode.SelectNodes("//li[@class='item last']");
            //var bestList = htmlDoc.DocumentNode.SelectNodes("//div[@class='product-info']");

        if(bestList != null)
        {
            foreach (var book in bestList)
            {
                try
                {
                    var outBook = book.OuterHtml;
                    var bookHtml = new HtmlDocument();
                    bookHtml.LoadHtml(outBook);
                    var imgSrc = bookHtml.DocumentNode.SelectSingleNode("//img").Attributes["src"].Value.Trim();

                    var small = "cache/1/small_image/210x/9df78eab33525d08d6e5fb8d27136e95";
                    var large = "cache/1/image/555x555/602f0fa2c1f0d1ba5e241f914e856ff9";

                    var repl = new Regex(small);
                    imgSrc = repl.Replace(imgSrc, large);

                    var bookInfo = bookHtml.DocumentNode.SelectSingleNode("//div[@class='product-info']").OuterHtml;
                    var bookHtmlInfo = new HtmlDocument();
                    bookHtmlInfo.LoadHtml(bookInfo);
                    var nodes = bookHtmlInfo.DocumentNode.SelectNodes("//a[@href]");
                    var values = nodes.Select(n => n.InnerHtml).ToList();
                    var bookLivro = new Book(values[0].Trim(), values[1].Trim(), imgSrc.Trim(), "Livro");
                    await bookLivro.SetSizeAsync();
                    bookList.Add(bookLivro);
                    Console.WriteLine(bookLivro.Title);
                }
                catch (Exception e)
                {

                }
            }
        }
            //var images = bookList.Select(x => x.ImageSrc);

            //string imagess = "";
            //foreach (var img in images)
            //{
            //    imagess += (img + Environment.NewLine);
            //}

            return bookList;

        }
    }
}
