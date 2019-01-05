using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Live.Repositories;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Live.Mapper;

namespace Live.Controllers
{
    [Route("api/[controller]")]
    public class SongController : Controller
    {
    
        private readonly  ISongsRepository _songRepository;
        
        public SongController (ISongsRepository songRepository)
        {
            this._songRepository = songRepository;
        }


        [HttpGet("allarchive")]
        public async Task <IActionResult> GetAllSongs()
        {
            var songs = await _songRepository.GetAllFromArchive();
            return Json(songs);
        }
    }

}