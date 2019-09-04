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
    public class EntitySetter 
    {
        public string Id {get; set;}
        public string UserId {get; set;}
        public string Type {get; set;}
    }

}