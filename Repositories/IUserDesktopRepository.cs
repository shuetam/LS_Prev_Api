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
        Task<List<IconDto>> GetAllIconsForUserAsync(string userId, string folderId);
        Task<List<IconDto>> GetAllImagesForUserAsync(string userId, string folderId);
        
        Task AddYouTubeAsync(EntitySetter addYoutube);
        Task AddImageAsync(EntitySetter addYoutube);
        Task<FolderDto> CreateFolderAsync(Guid userId, string Title);
        Task<object> AddEntityToFolder(Guid userId, string folderId, string entityId, string entityType);
    
        Task RemoveEntity(Guid userId, string entityId, string entityType);
        Task MoveEntityFromFolder(Guid userId, string entityId, string entityType);
        Task<List<FolderDto>> GetAllFoldersForUserAsync(string userId);
        Task<List<string>> GetAllIconsIdAsync(string userId);

        Task SaveIconsLocations(string userId, List<EntitySetter> icons);
        Task<List<IconDto>> GetNewIcons( Guid userId, string url);

        
    }

}
