using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Live.Controllers;
using Live.Core;


namespace Live.Repositories
{
    public interface IUserDesktopRepository
    {   
        Task<List<IconDto>> GetAllIconsForUserAsync(Guid userId, string folderId);
        Task<List<IconDto>> GetAllImagesForUserAsync(Guid userId, string folderId);

        Task<List<IconDto>> GetAllSpotifyForUserAsync(Guid userId, string folderId);
         Task<IconDto> ChangeEntityTitleAsync(EntitySetter newTitle, Guid userId);
        
        Task<bool> AddYouTubeAsync(EntitySetter addYoutube, Guid userId);
        Task<bool> AddImageAsync(EntitySetter addYoutube, Guid userId);
       Task<bool> AddSpotifyAsync(EntitySetter addSpotify, Guid userId);
        Task<FolderDto> CreateFolderAsync(Guid userId, string Title);
        Task<object> AddEntityToFolder(Guid userId, string folderId, string entityId, string entityType);
    
        Task RemoveEntity(Guid userId, string entityId, string entityType);
        Task MoveEntityFromFolder(Guid userId, string entityId, string entityType);
        Task<List<FolderDto>> GetAllFoldersForUserAsync(Guid userId);
        Task<List<string>> GetAllIconsIdAsync(Guid userId);

        Task SaveIconsLocations(Guid userId, List<EntitySetter> icons);
        Task<List<IconDto>> GetNewIcons( Guid userId, string url);

        
    }

}
