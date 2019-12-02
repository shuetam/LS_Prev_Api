using Live.Core;
using Live.DataBase.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Live.Repositories
{
    public interface IBestsellersRepository
    {
        Task<List<IconDto>> GetActuallBestsellersAsync();
        Task UpdateAsync();
    }
}
