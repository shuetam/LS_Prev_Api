using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;


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
        string p = @"\n";
        var r = new Regex(p);
        string q = r.Replace(name,"+");
        if( Regex.IsMatch(q , @"&"))
        {
            q = Regex.Replace(q , @"&", "%26");
        }
        string query = "https://www.youtube.com/results?search_query=" + q;
            WebRequest request = WebRequest.Create(query); 
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse(); 
            Stream dataStream = response.GetResponseStream();   
            StreamReader reader = new StreamReader(dataStream);   
            string htmlCode = reader.ReadToEnd();  
            reader.Close();
            response.Close();
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
        int sek = rnd.Next(5000, 20000);
        Console.WriteLine("----------------F R O M   H T T P-----------------------");
        Console.WriteLine(name);
        Console.WriteLine(ID);
        System.Threading.Thread.Sleep(sek);
        this.VideoID = ID;
        }
    }
}

  
