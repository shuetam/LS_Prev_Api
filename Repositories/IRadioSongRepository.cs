using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Live.Core;
using Microsoft.AspNetCore.Mvc;

namespace Live.Repositories
{
    public interface IRadioSongRepository
    {
        Task <List<RadioSongDto>> GetAllAsync();
        Task <List<RadioSongDto>>  GetByRadioAsync(List<string> stations);
        Task UpdateAsync();
        Task SetYouTubeIdAsync(int from, int to);

        Task <List<Object>> GetTryAsync();
    }

}