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
        Task<List<IconDto>> GetAllIconsForUserAsync(string userId);
        Task AddYouTubeAsync(AddEntity addYoutube);
        Task CreateFolderAsync(Guid userId, string Title);
        Task AddEntityToFolder(Guid userId);
        Task RemoveEntity(Guid userId, string entityId, string entityType);
         Task<List<IconDto>> GetAllFoldersForUserAsync(string userId);
        
    }

}
