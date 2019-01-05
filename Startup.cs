using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Live.Repositories;
using System.Xml;
using AutoMapper;
using Live.Mapper;
using Newtonsoft.Json;

namespace Live
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            var connectionString = Configuration.GetConnectionString("LiveDatabase");
            var sql_connection = new SqlConnectingSettings(connectionString);
            
            
            services.AddMvc().AddJsonOptions(j => j.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented);
            services.AddScoped<IRadioSongRepository, RadioSongRepository>();
            services.AddScoped<ISongsRepository, SongsRepository>();
            services.AddSingleton(AutoMapperConfig.Initialize());
            services.AddSingleton(sql_connection);
            services.AddCors();
            
           // var connectionString = Configuration.GetSection("SqlConnecting").Get<SqlConnectingSettings>().ConnectionString; 
         
            services.AddDbContext<LiveContext>(options => options.UseSqlServer(connectionString) );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(options => options.WithOrigins("http://localhost:3000").AllowAnyMethod()); // allow all methods on my api port

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }



 



    }
}
