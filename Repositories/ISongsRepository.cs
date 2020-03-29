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
    Task<List<IconDto>> GetActualByRadioAsync(List<string> stations);
    Task<List<ArchiveSong>> GetByYouTubeFromArchive(string id);
    Task<ArchiveSong> GetByNameFromArchive(string name);
    Task<DateTime> GetLastDate();
    Task UpdateAsync();
    Task AddToArchiveAsync(Song song);
    Task UpdateArchiveAsync(Song actualSong);
    Task<List<IconDto>> GetAllFromArchive();
    Task<List<IconDto>> GetAllErrorsFromArchive();
    Task<List<IconDto>> GetFromArchiveByIndex(int i, int j);
    Task DeleteByYouTubeId(string id);
    Task  ChangeYouTubeId(string name, string Id, string toId);
    Task ChangeName(string Id, string name);
    Task<List<IconDto>> GetActualRandomSongs();
    Task ChangeLocation(string Id, string left, string top);

    }

}