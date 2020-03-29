using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Live.Core;
using Microsoft.AspNetCore.Mvc;

namespace Live.Repositories
{
    public interface ITVMovieRepository
    { 
        Task<List<IconDto>> GetActuallMovies();
        Task UpdateAsync();
        Task ChangeYouTubeId(string Id, string toId);
        Task ChangeName(string Id, string name);
        Task ChangeLocation(string Id, string left, string top);
        Task DeleteByYouTubeId(string id);
        Task<List<IconDto>> GetAllErrorsFromArchive();
     }
}
