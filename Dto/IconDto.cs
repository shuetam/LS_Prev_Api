using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
namespace Live.Core
{
    public class IconDto
    {
        public string  Id {get; set;}
        public string title {get; set;}
        public string  videoId {get; set;}
        public string  top {get; set;}
        public string left {get; set;}
        public string  count {get; set;}
    }
}