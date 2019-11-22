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
using System.Web.Http;

namespace Live
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            var connectionString = Configuration.GetConnectionString("LiveSearchDatabase");
            var sql_connection = new SqlConnectingSettings(connectionString);



            services.AddMvc().AddJsonOptions(j => j.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented);
            services.AddScoped<IRadioSongRepository, RadioSongRepository>();
            services.AddScoped<ISongsRepository, SongsRepository>();
			services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserDesktopRepository, UserDesktopRepository>();
            services.AddScoped<ITVMovieRepository, TVMovieRepository>();
            services.AddSingleton(AutoMapperConfig.Initialize());
            services.AddSingleton(sql_connection);
    /*         services.AddCors(options => options.AddPolicy(MyAllowSpecificOrigins, builder =>
            {
                builder.WithOrigins("http://localhost:3000",
                                    "http://localhost:3001");
            })); */

            // var connectionString = Configuration.GetSection("SqlConnecting").Get<SqlConnectingSettings>().ConnectionString; 
   services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    }));


            services.AddDbContext<LiveContext>(options => options.UseSqlServer(connectionString));
            services.AddAuthentication()
                .AddGoogle(options =>
                {
                    options.ClientId = Configuration["auth:google:clientid"];
                    options.ClientSecret = Configuration["auth:google:clientsecret"];
                });
       
       
       
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //app.UseCors(options => options.WithOrigins("http://localhost:3000/songs").AllowAnyMethod()); // allow all methods on my api port
            //app.UseCors(options => options.WithOrigins("http://localhost:3001").AllowAnyMethod());
          /*   if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
             else */
      
        //app.UseCors(MyAllowSpecificOrigins);
        app.UseCors("MyPolicy");
            app.UseMvc();
            app.UseAuthentication();
        }

    }
}
