using Live.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Live.Repositories
{
    public interface IBestsellersRepository
    {
        Task<List<IconDto>> GetActuallBestsellers();
        Task UpdateAsync();
    }
}
