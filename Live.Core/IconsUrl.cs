using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using HtmlAgilityPack;

namespace Live.Core
{
    public class IconsUrl
    {

        //public List<IconDto> IDS {get; private set;}
        public IconsUrl(string url)
        {
              //this.SetIDFromUrl(url);
        }

        private static bool isSrc(string url, List<string> list)
        {
            foreach(var form in list)
            {
                var reg = new Regex(".*[.]{1}"+form+"$");
                if(reg.IsMatch(url))
                {
                    return true;
                }
            }
            return false;
        }

        public static async Task<List<IconDto>> GetIdsFromUrl(string url)
        {
            var icons = new List<IconDto>();

                string patternYT1 = "watch[?]{1}v[=]{1}([^&]+)";
                string patternYT2 =  "youtu.be[/]{1}([^&]+)";

                var imagesF = new List<string>(){
                    "apng", "bmp","pjpeg","pjp","png","svg","tif",
                    "gif", "ico", "cur","jpg","jpeg", "jfif","tiff", "webp"        
                    };
                
                var reg1 = new Regex(patternYT1);
                var reg2 = new Regex(patternYT2);
                bool yt1 = reg1.IsMatch(url);
                bool yt2 = reg2.IsMatch(url);
                bool insta = url.Contains("instagram");
                bool is_Src = isSrc(url, imagesF);


                if(is_Src)
                {
                    icons.Add(new IconDto(url, url, "IMG"));
                }


                if(yt1)
                {
                    string src = reg1.Matches(url).Select(s => s.Groups[1].Value).ToArray()[0];
                    icons.Add(new IconDto(src, url, "YT"));
                    //Console.WriteLine(reg1.Matches(url).Select(s => s.Groups[1].Value).ToArray()[0]);
                }
                if(yt2)
                {
                    string src = reg2.Matches(url).Select(s => s.Groups[1].Value).ToArray()[0];
                    icons.Add( new IconDto(src, url, "YT"));
                    //Console.WriteLine(reg2.Matches(url).Select(s => s.Groups[1].Value).ToArray()[0]);
                }
                if(insta)
                {
                            
                    WebClient client = new WebClient();
                    //var uri = new Uri(url);
                    string htmlCode = "";

            await Task.Run(() => {
                    htmlCode = client.DownloadString(url);
            });
             //client.DownloadStringAsync(uri);

                /* client.DownloadStringCompleted += (sender, e) =>
                {
                    htmlCode = e.Result;
                }; */

                    string instaPattern =  "[\"]display_url[\"]{1}[:]{1}[\"]{1}([^\"]+)";
                    //string instaPattern =  "display_url";
                    var instareq = new Regex(instaPattern);
                    //this.IDS.Add(new IconDto( instareq.Matches(htmlCode).Select(s => s.Groups[1].Value).ToArray()[0], "IMG"));
                    Console.WriteLine("MACHES:");
                    var maches = instareq.Matches(htmlCode).Select(s => s.Groups[1].Value).ToArray();
                    if(maches.Count()>0)
                    {
                        foreach(var mach in maches)
                        {
                            //Console.WriteLine(mm);
                           var src = (maches[0].Split('\\').Count()>0)?  mach.Split('\\')[0] : "";
                           icons.Add(new IconDto(src, url, "IMG"));
                        }
                    }
                }



                if(!yt1 && !yt2 && !insta && !is_Src)
                {
                    var mainHTML = new HtmlDocument();
                    string host = "";
                    try
                    {

                    
                    WebClient client = new WebClient();
                    string htmlCode = "";

            await Task.Run(() => {
                    htmlCode = client.DownloadString(url);
            });
           
                var uri = new Uri(url);

                 host = uri.Host;

                List<string> ids = new List<string>();

                ///Console.WriteLine(htmlCode);
     
                //mainHTML = new HtmlDocument();
            await Task.Run(() => {
                   mainHTML.LoadHtml(htmlCode);
            });

                 
                var list = new List<string>(); 

                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }



        if(mainHTML.DocumentNode.SelectNodes("//img") != null)
        {

var src = "";
        try
        {
           
            var httpReg = new Regex("^http[s]?[:]{1}[/]{1}[/]{1}");

            //var wwwReg = new Regex("^www[.]{1}");

            var hashReg = new Regex("^[/]+");

            foreach(HtmlNode node in mainHTML.DocumentNode.SelectNodes("//img"))
            {
                    //list.Add(node.InnerText);
                     src = node.Attributes["src"].Value.Trim();
                    src = hashReg.Replace(src, "");

           if(!httpReg.IsMatch(src))
            {
                src = "http://" + src;
            }
                    
            //Console.WriteLine(src); 
                  
            try 
            {
                //await Task.Run(() => {
                   byte[] response = await new System.Net.WebClient().DownloadDataTaskAsync(src);
            //});
                
            }  
            catch(Exception e) 
            {
                 Console.WriteLine("---------------------");
                Console.WriteLine(src);
                Console.WriteLine("---------------------");   
                src =  httpReg.Replace(src, "");  
                src = "http://" + host + "/" + src;              
            }

   

           // Console.WriteLine(response);

                   // if(response.Length==0)
                   // {
                       // src = "http://"  + src;
                    //}
                    //if(!httpReg.IsMatch(src))
                    //{
                        //src = "http://" + src;
                    //}

                   // if(!httpReg.IsMatch(src))
                   // {
                       // src = "http://" + host + "/" + src;
                   // }
                    
        // await Task.Run(() => {
             byte[] response1 = await new System.Net.WebClient().DownloadDataTaskAsync(src);
            //});
                    
                    if(!icons.Any(x => x.id == src))
                    {
                        icons.Add(new IconDto(src,url, "IMG"));
                        
                    }
                } 
            }
            catch (Exception e) 
            {
                Console.WriteLine("---------------------");
                 Console.WriteLine(src);
                Console.WriteLine("---------------------");
            }
                
        }
                }
                return icons;

        }

    }

}