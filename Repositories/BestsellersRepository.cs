using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Live.Business.BookStores;
using Live.Core;
using Live.DataBase.DatabaseModels;

namespace Live.Repositories
{
    public class BestsellersRepository : IBestsellersRepository
    {

        private readonly LiveContext _liveContext;

        private readonly IMapper _autoMapper;
        public BestsellersRepository(LiveContext liveContext, IMapper autoMapper)
        {
            this._liveContext = liveContext;
            this._autoMapper = autoMapper;
        }
        Task<List<IconDto>> IBestsellersRepository.GetActuallBestsellers()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync()
        {
            var livro = new Livro();
            var bestList = await livro.GetBestsellersAsync();
            var i = 0;
            while (bestList.Count > 0)
            {


                var book = bestList.FirstOrDefault();
                if (book != null)
                {

                    var books = bestList.Where(x => x.TheSame(book)).ToList();
                    foreach (var b in books)
                    {
                        var bestseller = new Bestseller(b, i);
                        await _liveContext.Bestsellers.AddAsync(bestseller);
                    }
                    bestList.RemoveAll(x => books.Contains(x));
                    i++;
                }


            }
            await _liveContext.SaveChangesAsync();

        }
    }
}
