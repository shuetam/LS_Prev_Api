using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Live.Core
{
    public class YouTube
    {
        public int ID {get;set;} 
        public string VideoID {get; set;}
        public string top_ {get; protected set;}
        public string left_ {get; protected set;}

        private double top;
        private double left;

        public IConfiguration Configuration { get; }
 
       protected YouTube()
        {}

        public YouTube(string songName)
        {
            this.set_location();
            this.SetID(songName);
        }

         public YouTube(RadioSong radio_song)
        {
            this.VideoID = radio_song.YouTubeId;
            this.set_location();
        }



    private void set_location () 
        {
            Random random = new Random();
            int region = random.Next(1, 4);
            Random random_d = new Random();

            if (region == 1)
            {
               this.left = Math.Round((random_d.NextDouble() * (22 - 0) + 0), 3);
               this.top =  Math.Round((random_d.NextDouble() * (85 - 6) + 6), 3);
            }

            if (region == 2)
            {
               this.left =  Math.Round((random_d.NextDouble() * (72 - 22) + 22), 3);
               this.top=  Math.Round((random_d.NextDouble() * (85 - 54) + 54), 3);
            }

             if (region == 3)
            {
                this.left = Math.Round( (random_d.NextDouble() * (95 - 72) + 72), 3);
                this.top =  Math.Round((random_d.NextDouble() * (85 - 6) + 6), 3);   
            }

            this.left_ = Regex.Replace((this.left) + "vw", @"\,+", ".");
            this.top_ = Regex.Replace((this.top) + "vh" , @"\,+", ".");

        }

        private void SetID(string name)
        {
            string googleKey = new GoogleKey().googleKey;

            string query = $"https://www.googleapis.com/youtube/v3/search/?part=snippet%20&maxResults=1&q={name}&key={googleKey}";
            string json = "Error";
            try
            {
            WebRequest request = WebRequest.Create(query); 
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse(); 
            Stream dataStream = response.GetResponseStream();   
            StreamReader reader = new StreamReader(dataStream);   
            json = reader.ReadToEnd();  
            reader.Close();
            response.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            string pattern = "[\"]{1}videoId[\"]{1}[:]{1}[\\s]{1}[\"]{1}([^\"]+)[\"]{1}"; 
           

        var reg = new Regex(pattern);
        string ID = ID = "!!ID!!";
        if(reg.IsMatch(json))
        {
        ID = reg.Matches(json).Select(s => s.Groups[1].Value).ToArray()[0];
        this.VideoID = ID;
        Console.WriteLine("--------------F R O M   A P I-----------------");
        Console.WriteLine(name);
        Console.WriteLine(ID);
        }
       else
        {
           SetIDFromYouTube(name);
        }

        }


         private void SetIDFromYouTube(string name)
        {
        string p = @"\n";
        var r = new Regex(p);
        string q = r.Replace(name,"+");
        if( Regex.IsMatch(q , @"&"))
        {
            q = Regex.Replace(q , @"&", "%26");
        }

        string query = "https://www.youtube.com/results?search_query=" + q;
        string htmlCode = "Error";
           try 
           {
            WebRequest request = WebRequest.Create(query); 
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse(); 
            Stream dataStream = response.GetResponseStream();   
            StreamReader reader = new StreamReader(dataStream);   
            htmlCode = reader.ReadToEnd();  
            reader.Close();
            response.Close();
           }
           catch(Exception e)
           {
               Console.WriteLine(e.Message);
           }
        string pattern = "watch[?]{1}v[=]{1}([^\"]+)[\"]{1}";
        var reg = new Regex(pattern);
        string ID = ID = "!!ID!!"+this.top_+this.left_;
        if(reg.IsMatch(htmlCode))
        {
        ID = reg.Matches(htmlCode).Select(s => s.Groups[1].Value).ToArray()[0];
        }
        if(ID.Length > 30)
        {
            ID = "!!ID!!"+this.top_+this.left_;
        }
        Random rnd = new Random();
        int sek = rnd.Next(500, 1500);
        Console.WriteLine("----------------F R O M   H T T P-----------------------");
        Console.WriteLine(name);
        Console.WriteLine(ID);
        System.Threading.Thread.Sleep(sek);
        this.VideoID = ID;
        }
    }
}

  
