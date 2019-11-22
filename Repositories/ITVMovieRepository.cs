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

     }
}
