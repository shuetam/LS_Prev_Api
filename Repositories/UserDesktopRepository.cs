using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AutoMapper;
using Live.Controllers;
using Live.Core;
using Microsoft.EntityFrameworkCore;

namespace Live.Repositories
{
    public class UserDesktopRepository : IUserDesktopRepository
    {

        private readonly LiveContext _liveContext;
        private readonly IMapper _autoMapper;

        public UserDesktopRepository(LiveContext liveContext, IMapper autoMapper )
        {
            this._liveContext = liveContext;
            this._autoMapper = autoMapper;
        }


        public async Task AddYouTubeAsync(AddEntity addYoutube)
        {
            var exist =_liveContext.UserYoutubes.FirstOrDefault(x => x.UserId.ToString() == addYoutube.UserId && x.VideoId == addYoutube.Id);

            Console.Write("im in repostiry");
            if(exist == null)
            {
                //var top = GetLocationAsync(addYoutube.UserId).Result;
                var newYoutube = new UserYoutube(addYoutube.UserId, addYoutube.Id, addYoutube.Title, addYoutube.Left, addYoutube.Top);
                _liveContext.UserYoutubes.Add(newYoutube);
                await _liveContext.SaveChangesAsync();
            }
        }

        private async Task<string>  GetLocationAsync(string userId)
        {
            var entities = await GetAllIconsForUserAsync(userId);
            var location = "30px";
            if(entities.Count>0)
            {
                var reg = new Regex("px");
                var locationsTop = entities.Select(x => double.Parse(reg.Replace(x.top, "")));
                location = (locationsTop.Max() + 50) +  "px";
            }
            return location;

        }

        public async Task<List<IconDto>> GetAllIconsForUserAsync(string userId)
        {
            
            var yotubes = await _liveContext.UserYoutubes.Where(x => x.UserId.ToString() == userId ).ToListAsync();
            //var folders = await _liveContext.Folders.Where(x => x.UserId.ToString() == userId).ToListAsync();
            
        

            var icons = yotubes.Select(x => _autoMapper.Map<IconDto>(x)).ToList();
            //icons.AddRange(folders.Select(x => _autoMapper.Map<IconDto>(x)).ToList());
            return icons;
        }


        public async Task<List<IconDto>> GetAllFoldersForUserAsync(string userId)
        {
            
            //var folers = await _liveContext.Folders.Where(x => x.UserId.ToString() == userId ).ToListAsync();
            var folders = await _liveContext.Folders.Where(x => x.UserId.ToString() == userId).ToListAsync();
            var icons = folders.Select(x => _autoMapper.Map<IconDto>(x)).ToList();
            //icons.AddRange(folders.Select(x => _autoMapper.Map<IconDto>(x)).ToList());
            return icons;
        }

        public async Task RemoveEntity(Guid userId, string entityId, string entityType)
        {
              Console.WriteLine(entityType);
            if(entityType == "YT")
            {
                Console.WriteLine("i am removing entity");
               var entity = _liveContext.UserYoutubes.FirstOrDefault(x => x.UserId == userId && x.VideoId == entityId);
                Console.WriteLine(entity.ID);
                _liveContext.Remove(entity);
               await _liveContext.SaveChangesAsync();
            }
        }



        public Task AddEntityToFolder(Guid userId)
        {
            throw new NotImplementedException();
        }


        public async Task CreateFolderAsync(Guid userId, string Title)
        {
            var folder = new Folder(userId, Title);
            await _liveContext.Folders.AddAsync(folder);
            await _liveContext.SaveChangesAsync();
        }

    }

}