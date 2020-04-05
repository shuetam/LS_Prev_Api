using Live.Repositories;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Live.Mapper;
using System.Diagnostics;
using System.Web.Http.Cors;
using Newtonsoft.Json.Linq;
using Live.Core;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using System;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog;

namespace Live.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
	[Route("api/[controller]")]
    public class MovieController : LiveController
    {
        private readonly  ITVMovieRepository _movieRepository;
        
        public MovieController (ITVMovieRepository movieRepository)
        {
            this._movieRepository = movieRepository;
        }

        
        [HttpGet("takemovies")]
        public async Task<IActionResult> TakeMovies()
        {
           // Log.Information("Hello from movies");
             var movies = await _movieRepository.GetActuallMovies();

             return Json(movies);
        }

        [HttpPost("update")]
        public async Task UpdateMovies()
        {
            await _movieRepository.UpdateAsync();
        }


    }
}
