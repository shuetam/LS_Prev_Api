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
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Live.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = "ADMIN")]
    public class AdminController : LiveController
    {
        private readonly  ISongsRepository _songRepository;
        private readonly  ITVMovieRepository _movieRepository;
        public AdminController (ISongsRepository songRepository, ITVMovieRepository movieRepository)
        {
            this._songRepository = songRepository;
            this._movieRepository = movieRepository;
        }

        [HttpPost("edityoutube")]
        public async Task <IActionResult> EditYoutube([FromBody]EditYoutube editSong)
        {
            //Debug.Print("EDITSONG");
            if(editSong.newYouTubeId != editSong.youTubeId)
            {
                await _songRepository.ChangeYouTubeId(editSong.name, editSong.youTubeId, editSong.newYouTubeId);
                 await _movieRepository.ChangeYouTubeId(editSong.youTubeId, editSong.newYouTubeId);
            }
            if(editSong.newName != editSong.name)
            {
                await _songRepository.ChangeName(editSong.youTubeId, editSong.newName);
                await _movieRepository.ChangeName(editSong.youTubeId, editSong.newName);
            }
           return Json(editSong);
        }

        [HttpPost("changeyoutubelocation")]
        public async Task <IActionResult> ChangeLocation([FromBody]EntitySetter editYoutube)
        {
            
            await _songRepository.ChangeLocation(editYoutube.Id, editYoutube.Left, editYoutube.Top);
            await _movieRepository.ChangeLocation(editYoutube.Id, editYoutube.Left, editYoutube.Top);
                  
           
           return Json(editYoutube);
        }



        [HttpPost("update")]
        public async Task Post()
        {
           await  _songRepository.UpdateAsync();
        }

        [HttpPost("allarchive")]
        public async Task <IActionResult> GetAllSongs()
        {
            var songs = await _songRepository.GetAllFromArchive();
            return Json(songs);
        }



        [HttpPost("getallerrors")]
        public async Task <IActionResult> GetAllErrors()
        {
            var songs = await _songRepository.GetAllErrorsFromArchive();
            var movies = await _movieRepository.GetAllErrorsFromArchive();
            songs.AddRange(movies);
            return Json(songs);
        }

        [HttpPost("archive/{i}/{j}")]
        public async Task <IActionResult> GetArchiveSongs(int i, int j)
        {
            var songs = await _songRepository.GetFromArchiveByIndex(i, j);
            return Json(songs);
        }

        [HttpPost("deleteyoutube")]
        public async Task <IActionResult> DeleteByYouTubeId([FromBody]EditYoutube youTube)
        {
            //Console.WriteLine("youTube.youTubeId");
            //Console.WriteLine(youTube.youTubeId);
             await _songRepository.DeleteByYouTubeId(youTube.youTubeId);
             await _movieRepository.DeleteByYouTubeId(youTube.youTubeId);
             return Json(youTube.youTubeId);
        }

        [HttpPost("changename/{Id}")]
        public async Task <IActionResult> ChangeName(string Id, [FromBody] NameSetter Name)
        {
             await _songRepository.ChangeName(Id, Name.name);
             return NoContent();
        }

        [HttpPost("allradiosongs/{stations}")]
        public async Task <IActionResult> GetAllActualSongs(string stations)
        {
            var radio_list= stations.Split('_').ToList();
            var songs = await _songRepository.GetActualByRadioAsync(radio_list);
            return Json(songs);
        }

        [HttpPost("radiorandom")]
        public async Task <IActionResult> GetAllRandomSongs()
        {
            var songs = await _songRepository.GetActualRandomSongs();
            return Json(songs);
        }
    }
}
