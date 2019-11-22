using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Web;
using System.Globalization;

namespace Live.Core
{
    public class FrontYouTube
    {
    public string title {get; set;}
    public string  videoId {get; set;}
    public string  top {get; set;}
    public string left {get; set;}
    public string  count {get; set;}

    public FrontYouTube(Song song, int count)
    {
        this.title = song.Name;
        this.videoId = song.YouTube.VideoID;
        this.top = song.YouTube.top_;
        this.left = song.YouTube.left_;
        this.count = count.ToString();
    }

    public FrontYouTube(TVMovie movie,  List<DateTime> dates)
    {
        //var hour = movie.PlayAt.Hour;
        var dateTimeFormats = new CultureInfo("pl-PL").DateTimeFormat;
        var day = movie.PlayAt.ToString("dddd", dateTimeFormats);
        if(day == DateTime.Now.ToString("dddd", dateTimeFormats))
        {
            day = "dzisiaj";
        }
        string another = "";

        string rating = movie.Rating.Replace(",","");
        //int frontCount = Int32.Parse(rating);

        if(rating == "00")
        {
            rating = "50";
        }

        foreach(var date in dates)
        {
            var daya = date.ToString("dddd", dateTimeFormats);

        if(daya == DateTime.Now.ToString("dddd", dateTimeFormats))
        {
            daya = "dzisiaj";
        }
            var houra = date.ToString("HH:mm");

            another += $"{daya} godz. {houra}||";
        }

        var hour = movie.PlayAt.ToString("HH:mm");
        var title = $"\"{movie.Title}\"||{day} godz. {hour}||{another}{movie.Station}";

        this.title = title;
        this.videoId = movie.YouTube.VideoID;
        this.top = movie.YouTube.top_;
        this.left = movie.YouTube.left_;
        this.count = rating;
    }

    }

}