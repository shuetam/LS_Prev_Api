using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Live.Mapper;
using Serilog;

namespace Live
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                
                .WriteTo.File(Path.GetFullPath("Live_Logs/Livesearch_Log.txt").ToString())
                .CreateLogger(); 

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
           //.ConfigureLogging((hostingContext, builder) =>
           // {

                /* var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false)
                .Build(); */


                //builder.AddFile("Logs/myapp-{Date}.txt");
               
            //})
            
            .UseStartup<Startup>()
            .Build();
    }

}