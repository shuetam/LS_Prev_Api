using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using HtmlAgilityPack;
using Serilog;

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
            foreach (var form in list)
            {
                var reg = new Regex(".*[.]{1}" + form + "$");
                if (reg.IsMatch(url))
                {
                    return true;
                }
            }
            return false;
        }

        public static async Task<List<IconDto>> GetIdsFromUrl(string url)
        {
            var icons = new List<IconDto>();

        try 
        {
            string patternYT1 = "watch[?]{1}v[=]{1}([^&]+)";
            string patternYT2 = "youtu.be[/]{1}([^&]+)";

            var imagesF = new List<string>(){
                    "apng", "bmp","pjpeg","pjp","png","svg","tif",
                    "gif", "ico", "cur","jpg","jpeg", "jfif","tiff", "webp"
                    };

            var reg1 = new Regex(patternYT1);
            var reg2 = new Regex(patternYT2);
            bool yt1 = reg1.IsMatch(url);
            bool yt2 = reg2.IsMatch(url);
            bool insta = url.Contains("instagram.com/p/");
            bool is_Src = isSrc(url, imagesF);
            bool isSpotify = url.Contains("iframe") && url.Contains("spotify");


            if (is_Src)
            {
                icons.Add(new IconDto(url, url, "IMG"));
            }


            if (yt1)
            {
                string src = reg1.Matches(url).Select(s => s.Groups[1].Value).ToArray()[0];
                icons.Add(new IconDto(src, url, "YT"));
                //Console.WriteLine(reg1.Matches(url).Select(s => s.Groups[1].Value).ToArray()[0]);
            }
            if (yt2)
            {
                string src = reg2.Matches(url).Select(s => s.Groups[1].Value).ToArray()[0];
                icons.Add(new IconDto(src, url, "YT"));
                //Console.WriteLine(reg2.Matches(url).Select(s => s.Groups[1].Value).ToArray()[0]);
            }
            if (insta)
            {
                url = url.Replace("?utm_source=ig_web_copy_link", "");

                if(url[url.Length-1] != '/')
                {
                    url += '/';
                }
                string src = url + "media/?size=l";
                icons.Add(new IconDto(src, url, "IMG"));

            }
            if(isSpotify) 
            {
                    var doc = new HtmlDocument();
                    doc.LoadHtml(url);
                    var srcNode = doc.DocumentNode.SelectSingleNode("//iframe");
                    var spotifyId = srcNode.Attributes["src"].Value.Trim();
                    var spotPattern = "[%]{1}.*";
                    var spotReg = new Regex(spotPattern);
                    spotifyId = spotReg.Replace(spotifyId, "");
                    var imgAddress = spotifyId.Replace("embed/", "");

                    var spotIcon = new IconDto(spotifyId, "", "SPOTIFY");

                    WebClient client = new WebClient(){ Encoding = System.Text.Encoding.UTF8 };
                    string htmlCode = "";

                    await Task.Run(() =>
                    {
                        htmlCode = client.DownloadString(imgAddress);
                    });
                   // Console.WriteLine(htmlCode);
                    
                var imgHTML = new HtmlDocument();
                imgHTML.LoadHtml(htmlCode);
                var imgNode = imgHTML.DocumentNode.SelectSingleNode("//div[@class='cover-art-image']");
                spotIcon.source = "https://developer.spotify.com/assets/branding-guidelines/icon4@2x.png";

                if(imgNode == null)
                {
                    imgNode = imgHTML.DocumentNode.SelectSingleNode("//div[@class='bg lazy-image']");
                }

            if(imgNode != null)
            {
                var imgSrcAttribute = imgNode.Attributes["style"];
                var imgDataAttribute = imgNode.Attributes["data-src"];

                if(imgSrcAttribute != null)
                {
                    var imgSrcPattern =  new Regex("url[(]{1}([^)]+)[)]{1}");
                    //Console.WriteLine(imgSrcAttribute);

                    if(imgSrcPattern.IsMatch(imgSrcAttribute.Value)) 
                    {
                        var imgSrc =  imgSrcPattern.Matches(imgSrcAttribute.Value).Select(s => s.Groups[1].Value.Trim()).ToList();
                        if(imgSrc.Count>0)
                        {
                            spotIcon.source = "https:"+imgSrc[0];
                        }
                        //Console.WriteLine(imgSrc[0]);
                    }
                }
                 if(imgDataAttribute != null) 
                {
                    var imgSrcAttributeValue = imgDataAttribute.Value;
                    if(!string.IsNullOrEmpty(imgSrcAttributeValue))
                    {
                    spotIcon.source = "https:" + imgSrcAttributeValue;
                    }

                }

            }
         
            //Console.WriteLine(spotIcon.source);
                 icons.Add(spotIcon);

            }

            if (!yt1 && !yt2 && !insta && !is_Src && !isSpotify)
            {
                var mainHTML = new HtmlDocument();
                string host = "";

                    WebClient client = new WebClient(){ Encoding = System.Text.Encoding.UTF8 };
                    string htmlCode = "";

                    await Task.Run(() =>
                    {
                        htmlCode = client.DownloadString(url);
                        
                    });

                    var uri = new Uri(url);

                    host = uri.Host;

                    List<string> ids = new List<string>();

                    ///Console.WriteLine(htmlCode);

                    //mainHTML = new HtmlDocument();
                    await Task.Run(() =>
                    {
                        mainHTML.LoadHtml(htmlCode);
                    });


                    var list = new List<string>();

                
                if (mainHTML.DocumentNode.SelectNodes("//img") != null)
                {

                    var src = "";
                    var httpReg = new Regex("^http[s]?[:]{1}[/]{1}[/]{1}");

                        //var wwwReg = new Regex("^www[.]{1}");

                        var hashReg = new Regex("^[/]+");

                        foreach (HtmlNode node in mainHTML.DocumentNode.SelectNodes("//img"))
                        {
                            //list.Add(node.InnerText);
                            src = node.Attributes["src"].Value.Trim();
                            src = hashReg.Replace(src, "");

                            if (!httpReg.IsMatch(src))
                            {
                                src = "http://" + src;
                            }


                            if (!icons.Any(x => x.id == src))
                            {
                                icons.Add(new IconDto(src, url, "IMG"));

                            }
                        }
                    
                   

                }
            }
        }
        catch(Exception ex)
        {
            Log.Error($"Error in adding icon: {ex.Message}");
            Log.Error(ex.StackTrace);
        }
            return icons;

        }

    }

}