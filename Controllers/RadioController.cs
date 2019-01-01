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
    public class RadioController : Controller
    {

        private readonly  IRadioSongRepository _radioRepository;
        
        public RadioController (IRadioSongRepository radioRepository)
        {
            this._radioRepository = radioRepository;
            
        }

        
        [HttpPost("update")]
        public async Task Post()
        {
           await  _radioRepository.UpdateAsync();
        }

         [HttpPut("setidfor={from}/{to}")]
        public async Task Post(int from, int to)
        {
           await  _radioRepository.SetYouTubeIdAsync(from, to);
        }


        [HttpGet("allradiosongs")]
        
        public async Task <IActionResult> GetAllSongs()
        {
            var songs = await _radioRepository.GetAllAsync();
            return Json(songs);
        }

        [HttpGet("allradiosongs/{stations}")]
        public async Task <IActionResult> GetSongsByStations(string stations)
        {
            var radio_list= stations.Split('_').ToList();
            var songs = await _radioRepository.GetByRadioAsync(radio_list);
            return Json(songs);
        }
        


        [HttpGet("try")]
        
        public async Task <IActionResult> GetTrySongs()
        {
            var songs = await _radioRepository.GetTryAsync();
            return Json(songs);
        }

    }

}