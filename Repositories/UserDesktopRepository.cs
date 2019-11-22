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


        public async Task AddYouTubeAsync(EntitySetter addYoutube)
        {
            var exist =_liveContext.UserYoutubes.FirstOrDefault(x => x.UserId.ToString() == addYoutube.UserId && x.VideoId == addYoutube.Id);

        
            if(exist == null)
            {
                //var top = GetLocationAsync(addYoutube.UserId).Result;
                //var existYT = _liveContext.YouTubes.FirstOrDefault(x => x.VideoID == addYoutube.Id);
                var newYoutube = new UserYoutube(addYoutube.UserId, addYoutube.Id, addYoutube.Title, addYoutube.Left, addYoutube.Top, addYoutube.FolderId);
                //var idu = new Guid( addYoutube.UserId);
                //var user = _liveContext.Users.FirstOrDefault(x => x.ID == idu);
                //user.UserYoutubes.Add(newYoutube);
                //_liveContext.Update(user);
            
                //Console.WriteLine($"user has YT: {user.UserYoutubes.Count}");
              
            
                _liveContext.UserYoutubes.Add(newYoutube);
                await _liveContext.SaveChangesAsync();
            }
        }

        private async Task<string>  GetLocationAsync(string userId)
        {
            var entities = await GetAllIconsForUserAsync(userId,"");
            var location = "30px";
            if(entities.Count>0)
            {
                var reg = new Regex("px");
                var locationsTop = entities.Select(x => double.Parse(reg.Replace(x.top, "")));
                location = (locationsTop.Max() + 50) +  "px";
            }
            return location;

        }

        public async Task<List<IconDto>> GetAllIconsForUserAsync(string userId, string folderId)
        {
            Console.WriteLine(userId + "      -       " + folderId);
            var yotubes = 
            string.IsNullOrEmpty(folderId)?       
            await _liveContext.UserYoutubes.Where(x => x.UserId.ToString() == userId && x.FolderId==null ).ToListAsync()
            :
            await _liveContext.UserYoutubes.Where(x => x.UserId.ToString() == userId && x.FolderId.ToString()==folderId ).ToListAsync();
            
            var icons = yotubes.Select(x => _autoMapper.Map<IconDto>(x)).ToList();
         
            Console.WriteLine("Getting icons");
            return icons;
        }

        public async Task<List<IconDto>> GetAllImagesForUserAsync(string userId, string folderId)
        {
            Console.WriteLine(userId + "      -       " + folderId);
            var images = 
            string.IsNullOrEmpty(folderId)?       
            await _liveContext.UserImages.Where(x => x.UserId.ToString() == userId && x.FolderId==null ).ToListAsync()
            :
            await _liveContext.UserImages.Where(x => x.UserId.ToString() == userId && x.FolderId.ToString()==folderId ).ToListAsync();
            
            var icons = images.Select(x => _autoMapper.Map<IconDto>(x)).ToList();
         
            Console.WriteLine("Getting images");
            return icons;
        }




        public async Task<List<string>> GetAllIconsIdAsync(string userId)
        {
            var iconsIds = await _liveContext.UserYoutubes.Where(x => x.UserId.ToString() == userId).Select(x => x.VideoId).ToListAsync();
            return iconsIds;
        }


        public async Task<List<FolderDto>> GetAllFoldersForUserAsync(string userId)
        {
            
            //var folers = await _liveContext.Folders.Where(x => x.UserId.ToString() == userId ).ToListAsync();
            var folders = await _liveContext.Folders
            .Include(x => x.UserYouTubes)
            .Include(x => x.UserImages)
            .Where(x => x.UserId.ToString() == userId).ToListAsync();

            foreach(var folder in folders)
            {
                folder.SetFourIcons();
            }
            var icons = folders.Select(x => _autoMapper.Map<FolderDto>(x)).ToList();
            //icons.AddRange(folders.Select(x => _autoMapper.Map<IconDto>(x)).ToList());
            Console.WriteLine("Getting folders");
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

        if(entityType == "IMG")
            {
                Console.WriteLine("i am removing IMAGE");
               var entity = _liveContext.UserImages.FirstOrDefault(x => x.UserId == userId && x.UrlAddress == entityId);
                Console.WriteLine(entity.ID);
                _liveContext.Remove(entity);
               await _liveContext.SaveChangesAsync();
            }

        if(entityType == "FOLDER")
            {
                Console.WriteLine("i am removing folder");
               var folder = _liveContext.Folders.Include(x => x.UserYouTubes).FirstOrDefault(x => x.UserId == userId && x.ID.ToString() == entityId);
                Console.WriteLine(folder.ID);
                _liveContext.RemoveRange(folder.UserYouTubes);
                _liveContext.Remove(folder);
               await _liveContext.SaveChangesAsync();
            }
        }

        public async Task MoveEntityFromFolder(Guid userId, string entityId, string entityType)
        {
              //Console.WriteLine(entityType);
            if(entityType == "YT")
            {
                //Console.WriteLine("i am removing entity");
               var entity = _liveContext.UserYoutubes.FirstOrDefault(x => x.UserId == userId && x.VideoId == entityId);
                Console.WriteLine(entity.ID + " removing from folder");
               entity.RemoveFromFolder();
            }

        if(entityType == "IMG")
            {
                //Console.WriteLine("i am removing entity");
               var entity = _liveContext.UserImages.FirstOrDefault(x => x.UserId == userId && x.UrlAddress == entityId);
                Console.WriteLine(entity.ID + " removing from folder");
               entity.RemoveFromFolder();
            }
               await _liveContext.SaveChangesAsync();
        }

        public async Task<object> AddEntityToFolder(Guid userId, string folderId, string entityId, string entityType)
        {
                Console.WriteLine("FOLDER ID!:  "+ folderId);

            Folder folder = null;
            if(entityType == "ICON")
            {
                
                var entity = _liveContext.UserYoutubes.FirstOrDefault(x => x.UserId == userId && x.VideoId == entityId);
                if(entity != null)
                {
                    entity.SetFolder(new Guid(folderId));
                    _liveContext.Update(entity);
                }
                else 
                {
                    var entityImg = _liveContext.UserImages.FirstOrDefault(x => x.UserId == userId && x.UrlAddress == entityId);
                    entityImg.SetFolder(new Guid(folderId));
                    _liveContext.Update(entityImg);
                }
                
                
                await _liveContext.SaveChangesAsync();

                folder = _liveContext.Folders
                .Include(x => x.UserYouTubes)
                .Include(x => x.UserImages)
                .FirstOrDefault(x => x.ID.ToString() == folderId);
                
                folder.SetFourIcons();
                //Console.WriteLine($"Folder has youtbes: {folder.UserYouTubes.Count}");
            }

            return new {folder = _autoMapper.Map<FolderDto>(folder), entityId = entityId };
        }

        public async Task<FolderDto> CreateFolderAsync(Guid userId, string Title)
        {
            var folder = new Folder(userId, Title);
            await _liveContext.Folders.AddAsync(folder);
            await _liveContext.SaveChangesAsync();
            return _autoMapper.Map<FolderDto>(folder);
        }

          public async Task SaveIconsLocations(string userId, List<EntitySetter> icons)
          {
              var user = _liveContext.Users
              .Include(x => x.UserYoutubes)
              .Include(x => x.UserImages)
              .FirstOrDefault(x => x.ID.ToString()==userId);
  
              foreach(var icon in icons.Where(x => x.Type == "ICON"))
              {

                var yt = user.UserYoutubes.FirstOrDefault(x => x.VideoId == icon.Id);
                if(yt != null)
                {
                   yt.ChangeLocation(icon.Left, icon.Top);
                }

                var im = user.UserImages.FirstOrDefault(x => x.UrlAddress == icon.Id);
                if(im != null)
                {
                   im.ChangeLocation(icon.Left, icon.Top);
                }

              }
                await _liveContext.SaveChangesAsync();

                foreach(var folder in icons.Where(x=> x.Type=="FOLDER"))
                {
                    var fol = _liveContext.Folders.FirstOrDefault(x => x.ID.ToString() == folder.Id);

                    if(fol != null)
                    {
                        fol.ChangeLocation(folder.Left, folder.Top);
                    }
                }

                await _liveContext.SaveChangesAsync();

          }


          public async Task<List<IconDto>> GetNewIcons(Guid userId, string url) 
          {

              var user = await _liveContext.Users.FirstOrDefaultAsync(x => x.ID == userId);

              var icons = new List<IconDto>();

                if(user != null)
                {
                    //var iconsFromUrl = new IconsUrl(url);
                    if(!url.Contains("http"))
                    {
                        url = "http://" + url;
                    }
                    
                    var getIcons = await IconsUrl.GetIdsFromUrl(url);

                    //icons.AddRange(iconsFromUrl.IDS);
                    icons.AddRange(getIcons);

            icons =  icons
            .Where(x => !_liveContext.UserImages.Any(f => f.UrlAddress == x.id))
            .Where(x => !_liveContext.UserYoutubes.Any(f => f.VideoId == x.id))
            .ToList();
            
              
              
                    foreach(var icon in icons)
                    {
                        //Console.WriteLine(icon.id);
                    }
                    
                }

            if(icons.Count==0)
            {
                icons.Add(new IconDto("noFound","noFound","noFound"));
            }
            return icons;
          }

        public async Task AddImageAsync(EntitySetter addImage)
        {

        var exist =_liveContext.UserImages.FirstOrDefault(x => x.UserId.ToString() == addImage.UserId && x.Source == addImage.Id);
            if(exist == null)
            {
                var newImage = new UserImage(addImage.UserId, addImage.Source, addImage.Id, addImage.Title, addImage.Left, addImage.Top, addImage.FolderId);
                _liveContext.UserImages.Add(newImage);
                await _liveContext.SaveChangesAsync();
            }
        }
    }

}