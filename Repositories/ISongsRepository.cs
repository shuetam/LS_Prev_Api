using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Live.Core;
using Microsoft.AspNetCore.Mvc;

namespace Live.Repositories
{
    public interface ISongsRepository
    {   
        
    
          Task<List<Song>> GetAllActuall();
         Task<ArchiveSong> GetByYouTubeFromArchive(string id);
  

          Task<ArchiveSong> GetByNameFromArchive(string name);



   

         Task<DateTime> GetLastDate();
   
     Task UpdateAsync();
  

     Task CorrectNameOrUpdateArchive(Song song);




     Task UpdateArchiveAsync(Song actualSong);
     Task<List<SongDto>> GetAllFromArchive();
   



    }

}