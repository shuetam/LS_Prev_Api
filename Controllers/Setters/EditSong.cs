using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;


namespace Live.Controllers
{
    public class EditSong
    {
        public string name {get;  set;}
        public string youTubeId {get;  set;}
        public string newYouTubeId {get; set;}
        public string newName {get;  set;}
    }
}
