using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Live.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Live.Mapper;
using HtmlAgilityPack;

namespace Live.Repositories
{
    public class MovieCinemaRepository : IMovieCinemaRepository
    {
        public async Task<List<FrontYouTube>> GetActuallMovies()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync()
        {
            throw new NotImplementedException();
        }
    }
}