using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Drawing;




namespace Live.Business
{
    public class Book
    {
        public string Title;
        public string Author;
        public string ImageSrc;
        public int Size;
        public string Store;


        public Book(string title, string author, string img, string store)
        {
            this.Title = title;
            this.Author = author;
            this.ImageSrc = img;
            this.Store = store;
        }

        public async Task SetSizeAsync()
        {
            await Task.Run(() =>
            {
                byte[] imageData = new WebClient().DownloadData(this.ImageSrc);
                this.Size = imageData.Length;
            });
        }

        public bool TheSame(Book another)
        {
            if (another.Title == this.Title && another.Author == this.Author)
            {
                return true;
            }
            var reg = new Regex("[ ]+");
            var title1 = reg.Split(this.Title);
            var title2 = reg.Split(another.Title);
            var author1 = reg.Split(this.Author);
            var author2 = reg.Split(another.Author);
            var titleEq = title1.All(x => another.Title.Contains(x)) || title2.All(x => this.Title.Contains(x));
            var authorEq = author1.Any(x => another.Author.Contains(x)) || author2.Any(x => this.Author.Contains(x));

            if (titleEq && authorEq)
            {
                return true;
            }
            return false;
        }
    }
}
