using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System;
using Live.DataBase.DatabaseModels;

namespace Live.Core
{
    public class IconDto
    {

        protected IconDto()
        {
          
        }
        public IconDto(string _id, string source, string _type)
        {
            this.id = _id;
            this.type = _type;
            this.title = source;
            this.setLocation(true);
            this.count = "1";
        }

        public IconDto(Bestseller agent,  int count)
        {
            this.id = agent.ImageSrc;
            this.type = "BOOK";
            this.title = agent.Title + "||" + agent.Author;
            this.setLocation(false);
            this.count = count.ToString();
        }


        public string  id {get; set;}
        public string title {get; set;}
        public string  top {get; set;}
        public string left {get; set;}
        public string  count {get; set;}
        public string  type {get; set;}
        public string  source {get; set;}

        private void setAddingLocation () 
        {
            Random random = new Random();
            int region = random.Next(1, 4);
            Random random_d = new Random();
            var tleft = new double();
            var ttop = new double();

            if (region == 1)
            {
               tleft = Math.Round((random_d.NextDouble() * (22 - 0) + 0), 3);
               ttop =  Math.Round((random_d.NextDouble() * (85 - 6) + 6), 3);
            }

            if (region == 2)
            {
               tleft =  Math.Round((random_d.NextDouble() * (72 - 22) + 22), 3);
               ttop=  Math.Round((random_d.NextDouble() * (85 - 74) + 74), 3);
            }

             if (region == 3)
            {
                tleft = Math.Round( (random_d.NextDouble() * (95 - 72) + 72), 3);
                ttop =  Math.Round((random_d.NextDouble() * (85 - 6) + 6), 3);   
            }

            this.left = Regex.Replace((tleft) + "vw", @"\,+", ".");
            this.top = Regex.Replace((ttop) + "vh" , @"\,+", ".");

        }



    private void setLocation (bool adding) 
        {
            Random random = new Random();
            int region = random.Next(1, 4);
            Random random_d = new Random();
            var tleft = new double();
            var ttop = new double();

            if (region == 1)
            {
               tleft = Math.Round((random_d.NextDouble() * (22 - 0) + 0), 3);
               ttop =  Math.Round((random_d.NextDouble() * (85 - 6) + 6), 3);
            }

            if (region == 2)
            {
               tleft =  Math.Round((random_d.NextDouble() * (72 - 22) + 22), 3);

            if(adding)
            {
               ttop=  Math.Round((random_d.NextDouble() * (85 - 74) + 74), 3);
            }
            else 
            {
               ttop=  Math.Round((random_d.NextDouble() * (85 - 54) + 54), 3);
            }


            }

             if (region == 3)
            {
                tleft = Math.Round( (random_d.NextDouble() * (95 - 72) + 72), 3);
                ttop =  Math.Round((random_d.NextDouble() * (85 - 6) + 6), 3);   
            }

            this.left = Regex.Replace((tleft) + "vw", @"\,+", ".");
            this.top = Regex.Replace((ttop) + "vh" , @"\,+", ".");

        }

    }
}