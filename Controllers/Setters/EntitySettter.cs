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
        public string ParentId {get; set;}
        public string Source {get; set;}
        public string UserId {get; set;}
        public string Type {get; set;}
         public string Title {get; set;}
        public string Left {get; set;}
        public string Top {get; set;}
        public string FolderId {get; set;}
    }

}