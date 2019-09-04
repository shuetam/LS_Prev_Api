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

namespace Live.Controllers
{
    [EnableCors(origins: "http://localhost:3000", headers: "*", methods: "*")]
    
	[Route("api/[controller]")]
    public class UserDesktopController : Controller
    {

        private readonly  IUserDesktopRepository _desktopRepository;
        
        public UserDesktopController (IUserDesktopRepository desktopRepository)
        {
            this._desktopRepository = desktopRepository;
        }


        [HttpPost("addyoutube")]
        public async Task AddYoutube([FromBody] AddEntity youtube)
        {
            Console.Write(youtube.Title);
            await _desktopRepository.AddYouTubeAsync(youtube);
        }

        [HttpPost("createfolder")]
        public async Task CreateFolder([FromBody] AddEntity folder)
        {
            Console.Write(folder.Title);
            await _desktopRepository.CreateFolderAsync(new Guid(folder.UserId), folder.Title);
        }


        [HttpPost("geticons")]
        public async Task<IActionResult> GetIcons([FromBody] AuthUser user)
        {
          var icons = await _desktopRepository.GetAllIconsForUserAsync(user.userId);
          return Json(icons);
        }


        [HttpPost("getfolders")]
        public async Task<IActionResult> GetFolders([FromBody] AuthUser user)
        {
          var icons = await _desktopRepository.GetAllFoldersForUserAsync(user.userId);
          return Json(icons);
        }


        [HttpPost("removeentity")]
        public async Task RemoveEntity([FromBody] EntitySetter entity)
        {
           await _desktopRepository.RemoveEntity(new Guid(entity.UserId), entity.Id, entity.Type);
           
          //return Json(icons);
        }
    }

}
