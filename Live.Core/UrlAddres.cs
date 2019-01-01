using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Live.Repositories;


namespace Live.Core
{
    public class UrlAddres
    {

        public List<string> addresses {get; private set;}

        public UrlAddres() 
        { 
            this.SetAddresses();
        }

        private  void SetAddresses()
        {
            var dateNow = DateTime.Now;

           var date24 = dateNow.AddHours(-24);

           if(int.Parse(date24.Hour.ToString())%2!=0)
           {
               date24 = date24.AddHours(1);
           }
            var stations = new List<int>(){1,2,3,4,5,6,8,9,30,40};
            

       //     https://www.odsluchane.eu/szukaj.php?r=2&date=07-11-2018&time_from=14&time_to=16

    foreach(var v in stations)
    {
        for (int j = 0;j<12;j++)
          {
            var date = date24.ToString("dd-MM-yyyy");
            var hour1 = date24.Hour;
            date24 = date24.AddHours(2);
            var hour2 = date24.Hour;
            var station = v.ToString();
            string addres = "https://www.odsluchane.eu/szukaj.php?r="+station+"&date="+date+"&time_from="+hour1+"&time_to="+hour2;
            this.addresses.Add(addres);
          } 
    }
       
        }
    }
}