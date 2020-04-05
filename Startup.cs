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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Live.Settings;
using Live.Services;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Logging.Debug;

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

            services.AddMvcCore();

            services.AddLogging(builder =>
            {
                //builder.AddConfiguration(Configuration.GetSection("Logging"));
                
                   /*  builder.AddFile("Logs/myapp-{Date}.txt")
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("Live.Controllers", LogLevel.Information); */
               
                   /*  builder.AddFilter("System", LogLevel.Information);
                    builder.AddFilter("Microsoft", LogLevel.Information);
                    builder.AddFilter<DebugLoggerProvider>("System", LogLevel.Information);
                    builder.AddFilter<DebugLoggerProvider>("Microsoft", LogLevel.Information); */

                    
            });

            services.AddMvc().AddJsonOptions(j => j.SerializerSettings.ReferenceLoopHandling
            = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            //services.AddMvc().AddJsonOptions(j => j.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented);
            services.AddScoped<IRadioSongRepository, RadioSongRepository>();
            services.AddScoped<ISongsRepository, SongsRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserDesktopRepository, UserDesktopRepository>();
            services.AddScoped<ITVMovieRepository, TVMovieRepository>();
            services.AddScoped<IBestsellersRepository, BestsellersRepository>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddSingleton(Configuration.GetSection("Jwt").Get<TokenParameters>());
            services.AddSingleton(AutoMapperConfig.Initialize());
            services.AddSingleton(sql_connection);
            services.AddAuthorization();
            
            //        services.AddMvc(config =>
            //{
            //    var policy = new AuthorizationPolicyBuilder()
            //        .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            //        .RequireAuthenticatedUser()
            //        .Build();
            //    config.Filters.Add(new AuthorizeFilter(policy));
            //});


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


            services.AddAuthentication(options =>
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.RequireHttpsMetadata = false;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = false,
                            ValidateLifetime = true,
                            ValidateIssuerSigningKey = true,
                            ValidIssuer = Configuration["Jwt:Issuer"],
                            ValidAudience = Configuration["Jwt:Issuer"],
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:SigningKey"]))
                        };
                    });





            /* var tokenParameters = Configuration.GetSection("Jwt").Get<TokenParameters>();
                  services.AddAuthentication(options =>
                  {
                     options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                  }). AddJwtBearer(options =>
                     {
                     options.RequireHttpsMetadata = false;
                     options.TokenValidationParameters = new TokenValidationParameters()
                     {
                         ValidIssuer = tokenParameters.Issuer,
                         ValidateAudience = false,
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenParameters.SigningKey))  
                     };
                     }); */

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors("MyPolicy");
            app.UseMvc();
            app.UseAuthentication();

        }

    }
}
