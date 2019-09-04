using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;


namespace Live.Controllers
{
    public class SocialLogin 
    {
        public string Email {get; set;}
        public string Token {get; set;}
    }

}