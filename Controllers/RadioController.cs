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
        private readonly  ISongsRepository _songRepository;
        
        public RadioController (IRadioSongRepository radioRepository, ISongsRepository songRepository)
        {
            this._radioRepository = radioRepository;
            this._songRepository = songRepository;
            
        }

        
        [HttpPost("update")]
        public async Task Post()
        {
           await  _songRepository.UpdateAsync();
        }

       /*  [HttpPost("updatearchive")]
        public async Task UpdateArchve()
        {
           await  _songRepository.UpdateArchiveAsync();
        } */

       /*   [HttpPut("setidfor={from}/{to}")]
        public async Task Post(int from, int to)
        {
           await  _radioRepository.SetYouTubeIdAsync(from, to);
        } */


        [HttpGet("allradiosongs")]
        
        public async Task <IActionResult> GetAllSongs()
        {
            var songs = await _radioRepository.GetAllAsync();
            return Json(songs);
        }

      //  [HttpGet("allradiosongs/{stations}")]
      //  public async Task <IActionResult> GetSongsByStations(string stations)
      //  {
           // var radio_list= stations.Split('_').ToList();
           // var songs = await _songRepository.GetByRadioAsync(radio_list);
          //  return Json(songs);
      //  }
        


        /* [HttpGet("try")]
        
        public async Task <IActionResult> GetTrySongs()
        {
            var songs = await _songRepository.GetTryAsync();
            return Json(songs);
        } */

 

    }

}