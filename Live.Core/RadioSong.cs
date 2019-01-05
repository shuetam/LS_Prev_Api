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
    public class RadioSong
    {
        public Guid Id {get; protected set;}
        public string Name {get; protected set;}
        public string YouTubeId {get; protected set;}
        public int Count {get; protected set;}
        public string Size {get; protected set;}
        public string top_ {get; protected set;}
        public string left_ {get; protected set;}

        public int CountRmf {get; protected set;}
        public int CountZet {get; protected set;}
        public int CountEska {get; protected set;}
        public int CountRmfMaxx {get; protected set;}
        public int CountAntyRadio {get; protected set;}
        public int CountRmfClassic {get; protected set;}
        public int CountChilliZet {get; protected set;}
        public int CountZlotePrzeboje {get; protected set;}
        public int CountVox {get; protected set;}
        public int CountPlus {get; protected set;}



    /*1 - zet
    2 - rmf
    3 - eska
    4 - rmf maxx
    5 - antyradio
    6 - rmf classic
    40 - chillizet
    9 - zlote przeboje
    30 - vox fm
    8 - plus */



        protected RadioSong()
        {}
        public RadioSong(string name)
        {
            this.Id = new Guid();
            this.Name = name;
            this.Count=0;
            this.YouTubeId = "oMktsOtN9uc";
            this.set_location();
            this.Size = "20px"; 
        }

        public void setTotalCount(List<string> stations)
        {
            if(stations.Contains("rmf"))
        {  
           this.Count+=this.CountRmf;
        }
        if(stations.Contains("rmfmaxx"))
        {
           this.Count+=this.CountRmfMaxx; 
           
        }
        if(stations.Contains("zet"))
        {
        this.Count+=this.CountZet;
        
        }
        if(stations.Contains("plus"))
        {
           this.Count+=this.CountPlus;
         
        }
        if(stations.Contains("eska"))
        {
           this.Count+=this.CountEska;
            
        }
        if(stations.Contains("chillizet"))
        {
         this.Count+=this.CountChilliZet;
        }
        if(stations.Contains("antyradio"))
        {
          this.Count+=this.CountAntyRadio;
        }
        if(stations.Contains("zloteprzeboje"))
        {
          this.Count+=this.CountZlotePrzeboje;
        }
        if(stations.Contains("vox"))
        {
          this.Count+=this.CountVox;
        }
        if(stations.Contains("rmfclassic"))
        {
           this.Count+=this.CountRmfClassic;
        }
        }

        public void setCount(int r, int c)
        {
           
                if(r==1)
                    this.CountZet = c;
                   
                if(r==2)
                    this.CountRmf = c;
                    
                if(r==3)
                    this.CountEska = c;
                   
                if(r==4)
                    this.CountRmfMaxx = c;
                    
                if(r==5)
                    this.CountAntyRadio = c;
                    
                if(r==6)
                    this.CountRmfClassic = c;
                    
                if(r==40)
                    this.CountChilliZet = c;
                    
                if(r==9)
                    this.CountZlotePrzeboje = c;
                    
                if(r==30)
                    this.CountVox = c;
                   
                if(r==8)
                    this.CountPlus = c;
                   
        
        }

        public void setSongID(string name)
        {
        string p = @"\n";
        var r = new Regex(p);
        string q = r.Replace(name,"+");
        string query = "https://www.youtube.com/results?search_query=" + q;

      //  WebClient client = new WebClient();

     //   string htmlCode =  client.DownloadString(query);


            // Create a request for the URL.   
            WebRequest request = WebRequest.Create(query);  
            // If required by the server, set the credentials.  
            request.Credentials = CredentialCache.DefaultCredentials;  
            // Get the response.  
            WebResponse response = request.GetResponse();  
            // Display the status.  
            // Console.WriteLine (((HttpWebResponse)response).StatusDescription);  
            // Get the stream containing content returned by the server.  
            Stream dataStream = response.GetResponseStream();  
            // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(dataStream);  
            // Read the content.  
            string htmlCode = reader.ReadToEnd();  
            // Display the content.  
           // Console.WriteLine(htmlCode);  
            // Clean up the streams and the response.  
            reader.Close();
            response.Close();

         string pattern = "watch[?]{1}v[=]{1}([^\"]+)[\"]{1}";
         var reg = new Regex(pattern);
         string ID = reg.Matches(htmlCode).Select(s => s.Groups[1].Value).ToArray()[0];
         Console.WriteLine(ID);
         this.YouTubeId = ID;
        }



        public void SetSize(int max)
        {
            if (this.Count == 1)
            {
                this.Size =  "20px";
            }
            if (this.Count == max) 
            {
                this.Size = "80px";
            }
            else 
            {
                double result = ((60*(this.Count-max))/(max-1)) + 80;
                this.Size = Math.Round(result).ToString() + "px";
            }
        }

        private void set_location () 
        {
            Random random = new Random();
            int region = random.Next(1, 4);
            Random random_d = new Random();

            if (region == 1)
            {
               this.left_ =  (random_d.NextDouble() * (22 - 0) + 0).ToString() + "vw";
               this.top_ =  (random_d.NextDouble() * (85 - 6) + 6).ToString() + "vh";
            }

            if (region == 2)
            {
               this.left_ =  (random_d.NextDouble() * (72 - 22) + 22).ToString() + "vw";
               this.top_ =  (random_d.NextDouble() * (85 - 54) + 54).ToString() + "vh";
            }

             if (region == 3)
            {
                this.left_ =  (random_d.NextDouble() * (95 - 72) + 72).ToString() + "vw";
                this.top_ =  (random_d.NextDouble() * (85 - 6) + 6).ToString() + "vh";   
            }

        }
    }
}