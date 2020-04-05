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


        public async Task<bool> AddYouTubeAsync(EntitySetter addYoutube, Guid userId)
        {
            var exist =_liveContext.UserYoutubes.FirstOrDefault(x => x.UserId == userId && x.VideoId == addYoutube.Id);

        
            if(exist == null)
            {
                var newYoutube = new UserYoutube(userId, addYoutube.Id, addYoutube.Title, addYoutube.Left, addYoutube.Top, addYoutube.FolderId);
           
                _liveContext.UserYoutubes.Add(newYoutube);
                await _liveContext.SaveChangesAsync();
                return true;
            }
            else 
            {
                return false;
            }
        }

        private async Task<string>  GetLocationAsync(Guid userId)
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

        public async Task<List<IconDto>> GetAllIconsForUserAsync(Guid userId, string folderId)
        {
            Console.WriteLine(userId + "      -       " + folderId);
            var yotubes = 
            string.IsNullOrEmpty(folderId)?       
            await _liveContext.UserYoutubes.Where(x => x.UserId == userId && x.FolderId==null ).ToListAsync()
            :
            await _liveContext.UserYoutubes.Where(x => x.UserId == userId && x.FolderId.ToString()==folderId ).ToListAsync();
            
            var icons = yotubes.Select(x => _autoMapper.Map<IconDto>(x)).ToList();
         
            Console.WriteLine("Getting icons");
            return icons;
        }

        public async Task<List<IconDto>> GetAllImagesForUserAsync(Guid userId, string folderId)
        {
            Console.WriteLine(userId + "      -       " + folderId);
            var images = 
            string.IsNullOrEmpty(folderId)?       
            await _liveContext.UserImages.Where(x => x.UserId == userId && x.FolderId==null ).ToListAsync()
            :
            await _liveContext.UserImages.Where(x => x.UserId == userId && x.FolderId.ToString()==folderId ).ToListAsync();
            
            var icons = images.Select(x => _autoMapper.Map<IconDto>(x)).ToList();
         
            Console.WriteLine("Getting images");
            return icons;
        }

        public async Task<List<IconDto>> GetAllSpotifyForUserAsync(Guid userId, string folderId)
        {
            Console.WriteLine(userId + "      -       " + folderId);
            var spotifies = 
            string.IsNullOrEmpty(folderId)?       
            await _liveContext.UserSpotify.Where(x => x.UserId == userId && x.FolderId==null ).ToListAsync()
            :
            await _liveContext.UserSpotify.Where(x => x.UserId == userId && x.FolderId.ToString()==folderId ).ToListAsync();
            
            var icons = spotifies.Select(x => _autoMapper.Map<IconDto>(x)).ToList();
         
            Console.WriteLine("Getting spotifies");
            return icons;
        }




        public async Task<List<string>> GetAllIconsIdAsync(Guid userId)
        {
            var iconsIds = await _liveContext.UserYoutubes.Where(x => x.UserId == userId).Select(x => x.VideoId).ToListAsync();
            return iconsIds;
        }


        public async Task<List<FolderDto>> GetAllFoldersForUserAsync(Guid userId)
        {
            
            //var folers = await _liveContext.Folders.Where(x => x.UserId.ToString() == userId ).ToListAsync();
            var folders = await _liveContext.Folders
            .Include(x => x.UserYouTubes)
            .Include(x => x.UserImages)
            .Include(x => x.UserSpotify)
            .Where(x => x.UserId == userId).ToListAsync();

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

            if(entityType == "SPOTIFY")
            {
                Console.WriteLine("i am removing entity");
               var entity = _liveContext.UserSpotify.FirstOrDefault(x => x.UserId == userId && x.SpotifyId == entityId);
                Console.WriteLine(entity.ID);
                _liveContext.Remove(entity);
               await _liveContext.SaveChangesAsync();
            }

        if(entityType == "IMG" || entityType == "BOOK" )
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
               var folder = _liveContext.Folders
               .Include(x => x.UserYouTubes)
               .Include(x => x.UserImages)
               .Include(x => x.UserSpotify)
               .FirstOrDefault(x => x.UserId == userId && x.ID.ToString() == entityId);
                //Console.WriteLine(folder.ID);
                _liveContext.RemoveRange(folder.UserYouTubes);
                _liveContext.RemoveRange(folder.UserImages);
                _liveContext.RemoveRange(folder.UserSpotify);
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
               // Console.WriteLine(entity.ID + " removing from folder");
               entity.RemoveFromFolder();
            }

            if(entityType == "SPOTIFY")
            {
               var entity = _liveContext.UserSpotify.FirstOrDefault(x => x.UserId == userId && x.SpotifyId== entityId);
               entity.RemoveFromFolder();
            }

        if(entityType == "IMG" || entityType == "BOOK" )
            {
                //Console.WriteLine("i am removing entity");
               var entity = _liveContext.UserImages.FirstOrDefault(x => x.UserId == userId && x.UrlAddress == entityId);
               // Console.WriteLine(entity.ID + " removing from folder");
               entity.RemoveFromFolder();
            }
               await _liveContext.SaveChangesAsync();
        }

        public async Task<object> AddEntityToFolder(Guid userId, string folderId, string entityId, string entityType)
        {
                Console.WriteLine("FOLDER ID!:  "+ folderId);

            Folder folder = null;
            
                if(entityType == "YT")
                {
                    var entity = _liveContext.UserYoutubes.FirstOrDefault(x => x.UserId == userId && x.VideoId == entityId);
                    if(entity != null)
                    {
                        entity.SetFolder(new Guid(folderId));
                        _liveContext.Update(entity);
                    }
                }
                if(entityType == "IMG" || entityType == "BOOK") 
                {
                    var entityImg = _liveContext.UserImages.FirstOrDefault(x => x.UserId == userId && x.UrlAddress == entityId);
                    entityImg.SetFolder(new Guid(folderId));
                    _liveContext.Update(entityImg);
                }
                if(entityType == "SPOTIFY") 
                {
                    var entityImg = _liveContext.UserSpotify.FirstOrDefault(x => x.UserId == userId && x.SpotifyId == entityId);
                    entityImg.SetFolder(new Guid(folderId));
                    _liveContext.Update(entityImg);
                }
                
                
                await _liveContext.SaveChangesAsync();

                folder = _liveContext.Folders
                .Include(x => x.UserYouTubes)
                .Include(x => x.UserImages)
                .Include(x => x.UserSpotify)
                .FirstOrDefault(x => x.ID.ToString() == folderId);
                
                folder.SetFourIcons();
                //Console.WriteLine($"Folder has youtbes: {folder.UserYouTubes.Count}");
            

            return new {folder = _autoMapper.Map<FolderDto>(folder), entityId = entityId };
        }

        public async Task<FolderDto> CreateFolderAsync(Guid userId, string Title)
        {
            var folder = new Folder(userId, Title);
            await _liveContext.Folders.AddAsync(folder);
            await _liveContext.SaveChangesAsync();
            return _autoMapper.Map<FolderDto>(folder);
        }

          public async Task SaveIconsLocations(Guid userId, List<EntitySetter> icons)
          {
              var user = _liveContext.Users
              .Include(x => x.UserYoutubes)
              .Include(x => x.UserImages)
              .Include(x => x.UserSpotify)
              .FirstOrDefault(x => x.ID == userId);
  
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

                var sp = user.UserSpotify.FirstOrDefault(x => x.SpotifyId == icon.Id);
                if(sp != null)
                {
                   sp.ChangeLocation(icon.Left, icon.Top);
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

                }

            if(icons.Count==0)
            {
                icons.Add(new IconDto("noFound","noFound","noFound"));
            }
            return icons;
          }

        public async Task<bool> AddImageAsync(EntitySetter addImage, Guid userId)
        {

        var exist =_liveContext.UserImages.FirstOrDefault(x => x.UserId == userId && x.UrlAddress == addImage.Id);
            if(exist == null)
            {
                var newImage = new UserImage(userId, addImage.Source, addImage.Id, 
                addImage.Title, addImage.Left, addImage.Top, addImage.FolderId, addImage.Type);
                _liveContext.UserImages.Add(newImage);
                await _liveContext.SaveChangesAsync();
                return true;
            }
            else {
                return false;
            }
        }

        public async Task<bool> AddSpotifyAsync(EntitySetter addSpotify, Guid userId)
        {

        var exist =_liveContext.UserSpotify.FirstOrDefault(x => x.UserId == userId && x.SpotifyId == addSpotify.Id);
            if(exist == null)
            {
                var newSpot= new UserSpotify(userId, addSpotify.Id, addSpotify.Source, 
                addSpotify.Title, addSpotify.Left, addSpotify.Top, addSpotify.FolderId);
                _liveContext.UserSpotify.Add(newSpot);
                await _liveContext.SaveChangesAsync();
                return true;
            }
            else {
                return false;
            }
        }


        
        public async Task<IconDto> ChangeEntityTitleAsync(EntitySetter newTitle, Guid userId)
        {
            var enType = newTitle.Type;
      
      switch (enType)
      {
          case "IMG":
          case "BOOK":
              var img = _liveContext.UserImages.FirstOrDefault(x => x.UrlAddress == newTitle.Id && x.UserId==userId);
                if(img != null)
                {
                    var title = newTitle.Title;
                    img.ChangeTitle(title);
                    _liveContext.Update(img);
                    await _liveContext.SaveChangesAsync();
                    return _autoMapper.Map<IconDto>(img);
                    //return new IconDto(img.UrlAddress, title);
                }
                break;
           
          case "YT":
               var yt = _liveContext.UserYoutubes.FirstOrDefault(x => x.VideoId == newTitle.Id && x.UserId==userId);
                if(yt != null)
                {
                    var titleYT = newTitle.Title;
                    yt.ChangeTitle(titleYT);
                     _liveContext.Update(yt);
                     await _liveContext.SaveChangesAsync();
                     return _autoMapper.Map<IconDto>(yt);
                    //return new IconDto(yt.VideoId, titleYT);
                }
                break;

            case "SPOTIFY":
               var sp = _liveContext.UserSpotify.FirstOrDefault(x => x.SpotifyId == newTitle.Id && x.UserId==userId);
                if(sp != null)
                {
                    var titleSP = newTitle.Title;
                    sp.ChangeTitle(titleSP);
                     _liveContext.Update(sp);
                     await _liveContext.SaveChangesAsync();
                     return _autoMapper.Map<IconDto>(sp);
                   //return new IconDto(sp.SpotifyId, titleSP);
                }
                break;


            case "FOLDER":
               var fol = _liveContext.Folders.FirstOrDefault(x => x.ID.ToString() == newTitle.Id && x.UserId==userId);
                if(fol != null)
                {
                    var titleF = newTitle.Title;
                    fol.ChangeTitle(titleF);
                    _liveContext.Update(fol);
                    await _liveContext.SaveChangesAsync();
                    return _autoMapper.Map<IconDto>(fol);
                    //return new IconDto(fol.ID.ToString(), titleF);
                }
                break;

      }
            


            return null;

        }

    }

}