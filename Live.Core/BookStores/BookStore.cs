using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Live.Core.BookStores
{
    public abstract class BookStore
    {
        public abstract Task<List<Book>> GetBestsellersAsync();
    }
}
