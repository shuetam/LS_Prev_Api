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

        public UrlAddres(DateTime maxDate) 
        { 
            this.SetAddresses(maxDate);
        }

        private  void SetAddresses(DateTime maxDate)
        {
            var stations = new List<int>(){1,2,3,4,5,6,8,9,30,40};
            
            var dateNow = DateTime.Now;

            
  /*   var hours = (DateTime.Now - myDate).TotalHours;
    Console.WriteLine(hours); */


              for (int j = 0;j<24;j++)
                {
                    //var date = date24.ToString("dd-MM-yyyy");
                    var hourTo = dateNow.AddHours(-j).Hour;
                    var dateBase = dateNow.AddHours(-j-1);
                    var date = dateNow.AddHours(-j-1).ToString("dd-MM-yyyy");
                    var hourFrom = dateNow.AddHours(-j-1).Hour;
                foreach(var s in stations)
                {
                    string addres = "https://www.odsluchane.eu/szukaj.php?r="+s+"&date="+date+"&time_from="+hourFrom+"&time_to="+hourTo;
                    Console.WriteLine(dateBase);
                    Console.WriteLine(addres);
                }

                }
                
            


        }
    }
}