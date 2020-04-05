using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using HtmlAgilityPack;
using System.Globalization;
using Serilog;

namespace Live.Core
{
    public class TVMovie : Entity
    {
        public string Title {get; protected set;}
        public string TrailerSearch {get; protected set;}
        public YouTube YouTube {get; protected set;}
        public string Station {get; protected set;}
        public DateTime PlayAt {get; protected set;}
        public string Rating {get; protected set;}

       protected TVMovie()
        {}

        public TVMovie(string outHtml, string emisionDay)
        {
            var movieHTML = new HtmlDocument();

            movieHTML.LoadHtml(outHtml);
            var hrefNode = movieHTML.DocumentNode.SelectNodes("//a[@href]").FirstOrDefault();
            string href =   "https://www.telemagazyn.pl" + hrefNode.GetAttributeValue( "href", string.Empty);
            
            string title = movieHTML.DocumentNode.Descendants("p").FirstOrDefault().InnerText;
            title = System.Web.HttpUtility.HtmlDecode(title);
            this.Title = title;

            string hourClass = "left";
            var hour = movieHTML.DocumentNode.SelectNodes("//span[@class='" + hourClass + "']").FirstOrDefault().InnerText;
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime playDate = DateTime.Parse(emisionDay + " "+hour, provider);
            this.PlayAt = playDate;
            this.SetDataFromHref(href);

            //Console.WriteLine(emisionDay);
           // Console.WriteLine(playDate.ToString("dd-MM-yyyy HH:mm:ss"));

        }


        public TVMovie(string inHtml, string movHref, string emisionDay, bool nextDay)
        {
            var movieHTML = new HtmlDocument();

            movieHTML.LoadHtml(inHtml);
            var hrefNode = movieHTML.DocumentNode.SelectNodes("//a[@href]").FirstOrDefault();
            //string href =   "https://www.telemagazyn.pl" + hrefNode.GetAttributeValue( "href", string.Empty);
            string href =   "https://www.telemagazyn.pl" + movHref;
            
            string title = movieHTML.DocumentNode.Descendants("span").FirstOrDefault().InnerText;
            title = System.Web.HttpUtility.HtmlDecode(title.Trim());
            this.Title = title;
           // Console.WriteLine(title);
            //string hourClass = "left";
            //var hour = movieHTML.DocumentNode.SelectNodes("//span[@class='" + hourClass + "']").FirstOrDefault().InnerText;
            string hour = movieHTML.DocumentNode.Descendants("em").FirstOrDefault().InnerText;
            var addedHours = nextDay? 24 : 0 ;
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime playDate = DateTime.Parse(emisionDay + " "+hour, provider);
            this.PlayAt = playDate.AddHours(addedHours);
            this.SetDataFromHref(href);

            //Console.WriteLine(emisionDay);
           // Console.WriteLine(playDate.ToString("dd-MM-yyyy HH:mm:ss"));

        }

            public void  SetYoutube()
            {
                this.YouTube = new YouTube(this.TrailerSearch +" "+ "trailer");
              
            }

            public void SetYoutube(TVMovie movie)
            {
                this.YouTube = movie.YouTube;
              
            }

              public void SetYoutubeFromArchive(ArchiveMovie movie)
            {
                this.YouTube = movie.YouTube;
              
            }

            public void SetWhileYoutube()
            {
                  this.YouTube = new YouTube(this.Title, false);
            }

            public string getFilwebRating()
            {
                WebClient client = new WebClient();
                string htmlCode = "";

                try 
                {
                htmlCode = client.DownloadString("https://www.filmweb.pl/search?q=" + this.TrailerSearch);
                }
                catch(Exception ex)
                {
                    Log.Error($"Error with DownloadString in getFilwebRating: {this.TrailerSearch}");
                    Log.Error(ex.Message);
                }

           
                List<string> names = new List<string>();

                var mainHTML = new HtmlDocument();
                mainHTML.LoadHtml(htmlCode);

                var RatClass = "rateBox__rate";
                //var movieNameClass =  "filmPreview__titleDetails vod-box-margin";
                var dontFind = mainHTML.DocumentNode.InnerText.Contains("Niestety, nie znaleźliśmy");
                var rates = mainHTML.DocumentNode.SelectNodes("//span[@class='" + RatClass + "']");
                //var namesMovies = mainHTML.DocumentNode.SelectNodes("//div[@class='" + movieNameClass + "']");
                Random rnd = new Random();
                int sek = rnd.Next(500, 3000);
                System.Threading.Thread.Sleep(sek);
                if(rates != null && !dontFind)
                {
                    if(rates.Count>0)
                    {
                        var rat = rates[0].InnerText;
                        //var name = namesMovies[0].InnerText;
                        //Console.WriteLine(movieSearch);
                        //Console.WriteLine(name);
                        //Console.WriteLine(rat);
                        this.Rating = rat;
                        return rat;
                    }
                }
                this.Rating = "0,0";
                return "0,0";
                
            } 

        private void SetDataFromHref(string href)
        {
            WebClient client = new WebClient(){ Encoding = System.Text.Encoding.UTF8 };
            string htmlCode = client.DownloadString(href);
            var mainHTML = new HtmlDocument();
            mainHTML.LoadHtml(htmlCode);
            var info = "belkaInfo";
            var movieInfo = mainHTML.DocumentNode.SelectNodes("//div[@class='" + info + "']").FirstOrDefault().InnerText;
            
            var emisionInfo =  "emisjaSzczegoly";

            var emision = mainHTML.DocumentNode.SelectNodes("//div[@class='" + emisionInfo + "']").FirstOrDefault().InnerHtml;
            
            var emisionHTML = new HtmlDocument();
            emisionHTML.LoadHtml(emision);
            var station = emisionHTML.DocumentNode.Descendants("a").FirstOrDefault().InnerText;
            this.Station = station;

        //var ATM =  System.Web.HttpUtility.HtmlDecode(station)=="ATM Rozrywka";
        //var HBO =  System.Web.HttpUtility.HtmlDecode(station)=="HBO";

            movieInfo = movieInfo.Trim().Replace("\n"," ");

            var reg = new Regex("[0-9]{4}$");

            var year = "";

            if(reg.IsMatch(movieInfo))
            {
                year = reg.Matches(movieInfo).FirstOrDefault().Value;
            }


            var regTitle = new Regex("^.+[,]{1}");

            var originalTitle = this.Title;

            if(regTitle.IsMatch(movieInfo))
            {
             originalTitle = regTitle.Matches(movieInfo).FirstOrDefault().Value;
            }


            originalTitle = System.Web.HttpUtility.HtmlDecode(originalTitle);

            this.TrailerSearch = (originalTitle +" "+year).Trim();

            //Console.WriteLine(this.PlayAt.ToString("dd-MM-yyyy HH:mm:ss"));
            //Console.WriteLine(this.TrailerSearch);
            //Console.WriteLine(station);
      
        }

        public void ChangeYouTubeId(string id)
        {
            this.YouTube.VideoID = id;
        }

        public void ChangeLocation(string left, string top)
        {
            this.YouTube.ChangeLocation(left, top);
        }
        public void ChangeName(string name)
        {
            this.Title = name;
        }


    }
}