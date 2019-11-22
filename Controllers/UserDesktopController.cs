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


        [HttpPost("addicon")]
        public async Task AddIcon([FromBody] EntitySetter icon)
        {
           if(icon.Type == "YT")
           {
            await _desktopRepository.AddYouTubeAsync(icon);
           }
           if(icon.Type == "IMG")
           {
               Console.WriteLine("I am in repository images!!!!!!!!");
            await _desktopRepository.AddImageAsync(icon);
           }
        }

        [HttpPost("createfolder")]
        public async Task<IActionResult> CreateFolder([FromBody] EntitySetter folder)
        {
           // Console.Write(folder.Title);
            var newFolder = await _desktopRepository.CreateFolderAsync(new Guid(folder.UserId), folder.Title);
            return Json(newFolder);
        }

        [HttpPost("findiconsfromurl")]
        public async Task<IActionResult> FindIconsFromUrl([FromBody] EntitySetter data)
        {
            //Console.Write(data.Title);
            var newIcons = await _desktopRepository.GetNewIcons(new Guid(data.UserId), data.Title);
            return Json(newIcons);
        }



        [HttpPost("geticons")]
        public async Task<IActionResult> GetIcons([FromBody] AuthUser user)
        {
          var icons = await _desktopRepository.GetAllIconsForUserAsync(user.userId, user.folderId);
          return Json(icons);
        }

        [HttpPost("getimages")]
        public async Task<IActionResult> GetImages([FromBody] AuthUser user)
        {
          var icons = await _desktopRepository.GetAllImagesForUserAsync(user.userId, user.folderId);
          return Json(icons);
        }

        [HttpPost("getfolders")]
        public async Task<IActionResult> GetFolders([FromBody] AuthUser user)
        {
          var icons = await _desktopRepository.GetAllFoldersForUserAsync(user.userId);
          return Json(icons);
        }

        [HttpPost("geticonsid")]
        public async Task<IActionResult> GetIconsId([FromBody] AuthUser user)
        {
          var iconsIds = await _desktopRepository.GetAllIconsIdAsync(user.userId);
          return Json(iconsIds);
        }

        [HttpPost("addtofolder")]
        public async Task<IActionResult> AddToFolder([FromBody] EntitySetter en)
        {
          var data = await _desktopRepository.AddEntityToFolder(new Guid(en.UserId), en.ParentId, en.Id, en.Type);
          return Json(data);
        }



        [HttpPost("removeentity")]
        public async Task RemoveEntity([FromBody] EntitySetter entity)
        {
            Console.WriteLine("REMOVING  "+ entity.Id);
           await _desktopRepository.RemoveEntity(new Guid(entity.UserId), entity.Id, entity.Type);
           
          //return Json(icons);
        }


        [HttpPost("movefromfolder")]
        public async Task MoveEntityFromFolder([FromBody] EntitySetter entity)
        {
           await _desktopRepository.MoveEntityFromFolder(new Guid(entity.UserId), entity.Id, entity.Type);
        }

        [HttpPost("savelocations")]
        public async Task SaveLocations([FromBody] List<EntitySetter> entities)
        {
            Console.WriteLine("I am in savelocations");
            var userId =  entities[0].UserId;
            Console.WriteLine(userId);
            Console.WriteLine(entities[0].Top);
           await _desktopRepository.SaveIconsLocations(userId, entities);
           
          //return Json(icons);
        }
    }

}
