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

        public YouTube(string Name)
        {
            this.set_location();
            this.SetID(Name);
        }
        public YouTube(string Name, bool ID)
        {
            this.set_location();
            this.VideoID = "Error"+this.top;
            Console.WriteLine($"{Name} - to many reqests.");
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
            string json = "";
            try
            {
            this.VideoID = "Error" + this.top;
            WebRequest request = WebRequest.Create(query); 
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = request.GetResponse(); 
            Stream dataStream = response.GetResponseStream();   
            StreamReader reader = new StreamReader(dataStream);   
            json = reader.ReadToEnd();  
            reader.Close();
            response.Close();
            }
            catch(WebException e)
            {   Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++");
                Console.WriteLine(e.Message);
                Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++");
                this.VideoID = "FirstError" + this.top;
                Console.WriteLine(this.VideoID);
                //Console.WriteLine("+++++++++++++++++++++++++++++++++++++++++++++++");
            }

            string pattern = "[\"]{1}videoId[\"]{1}[:]{1}[\\s]{1}[\"]{1}([^\"]+)[\"]{1}"; 
           

        var reg = new Regex(pattern);
        //string ID  = "Error"+this.top_;
        //this.VideoID = "Error"+this.top_;
        if(reg.IsMatch(json))
        {
        string ID = reg.Matches(json).Select(s => s.Groups[1].Value).ToArray()[0];
        this.VideoID = ID;
     //   Console.WriteLine("--------------F R O M   A P I-----------------");
        Console.WriteLine(name);
      //  Console.WriteLine(ID);
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
            
        string pattern = "watch[?]{1}v[=]{1}([^\"]+)[\"]{1}";
        var reg = new Regex(pattern);
        string ID = "Error"+this.top_;
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

            if(reg.IsMatch(htmlCode))
            {
            ID = reg.Matches(htmlCode).Select(s => s.Groups[1].Value).ToArray()[0];
            }
            if(ID.Length > 30)
            {
                ID = "Error"+this.top_;
            }
            Random rnd = new Random();
            int sek = rnd.Next(2000, 5000);
        //  Console.WriteLine("----------------F R O M   H T T P-----------------------");
          Console.WriteLine(name + " - from youtube");
        //  Console.WriteLine(ID);
        //  Console.WriteLine("Sleep--- " + sek);
            System.Threading.Thread.Sleep(sek);
            this.VideoID = ID;
           }
           catch(Exception e)
           {
               Console.WriteLine(e.Message);
                this.VideoID = "ServerError"+this.top_+this.left_;
           }


        }
    }
}

  
