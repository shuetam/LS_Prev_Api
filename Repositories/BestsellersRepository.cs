using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Live.Core.BookStores;
using Live.Core;
using Live.DataBase.DatabaseModels;
using Live.Live.Core.BookStores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<List<IconDto>> GetActuallBestsellersAsync()
        {
            var bestSellers = await _liveContext.Bestsellers.ToListAsync();
            
            var unknown = bestSellers.Where(x => x.Title.Contains('�'));

            foreach(var unk in unknown)
            {
                Console.WriteLine(unk.Title);
            }

            int maxIndex = bestSellers.Select(x => x.GroupNo).Max();
            var books = new List<IconDto>();

            for(int i=0;i<=maxIndex;i++)
            {
                var theSameBooks = bestSellers.Where(x => x.GroupNo==i).ToList();
                var count = theSameBooks.Count;
                var bestImg = theSameBooks.Select(x => x.Size).Max();
                var agent = theSameBooks.FirstOrDefault(x => x.Size == bestImg);
                var icon = new IconDto(agent, count);
                 books.Add(icon);
            }
    
            return books;
        }

        public async Task UpdateAsync()
        {
            var bestList  = new List<Book>();
            
            _liveContext.Bestsellers.RemoveRange(_liveContext.Bestsellers.Where(x => x.Store != ""));
            
            await _liveContext.SaveChangesAsync();
            
            bestList.AddRange( await new Bonito().GetBestsellersAsync());
            await _liveContext.SaveChangesAsync();
            bestList.AddRange( await new Aros().GetBestsellersAsync());
            await _liveContext.SaveChangesAsync();
            bestList.AddRange( await new Czytam().GetBestsellersAsync());
            await _liveContext.SaveChangesAsync();
            bestList.AddRange( await new Empik().GetBestsellersAsync());
            await _liveContext.SaveChangesAsync();
            bestList.AddRange( await new Gandalf().GetBestsellersAsync());
            await _liveContext.SaveChangesAsync();
            bestList.AddRange( await new Livro().GetBestsellersAsync());
            await _liveContext.SaveChangesAsync();
            bestList.AddRange( await new Profit24().GetBestsellersAsync());
            await _liveContext.SaveChangesAsync(); 

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
            Console.WriteLine("========Finish book update==========");

        }
    }
}
