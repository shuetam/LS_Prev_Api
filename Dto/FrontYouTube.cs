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
    public class FrontSong
    {
    public string title {get; set;}
    public string  videoId {get; set;}
    public string  top {get; set;}
    public string left {get; set;}
    public string  count {get; set;}

    public FrontSong(Song song, int count)
    {
        this.title = song.Name;
        this.videoId = song.YouTube.VideoID;
        this.top = song.YouTube.top_;
        this.left = song.YouTube.left_;
        this.count = count.ToString();
    }

    }

}