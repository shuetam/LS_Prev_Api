using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Live.Business.BookStores
{
    public class Gandalf : BookStore
    {
        public override async Task<List<Book>> GetBestsellersAsync()
        {
            throw new NotImplementedException();
        }
    }
}
