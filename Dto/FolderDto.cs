using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;

namespace Live.Core
{
    public class FolderDto: IconDto
    {
        public DateTime? DateCreated {get; set;}
        public FolderDto(string _id, string _type) : base(_id, "", _type)
        {
        }

        protected FolderDto()
        {
          
        }
    
        public FolderDto(DateTime? date, string ID)
        {
          DateCreated = date;
          id = ID;
        }

        public string icon0 {get; set;}
        public string icon1 {get; set;}
        public string icon2 {get; set;}
        public string icon3 {get; set;}
    }
}