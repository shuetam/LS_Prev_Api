using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Live.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using Live.Mapper;
using HtmlAgilityPack;

namespace Live.Repositories
{
    public class TVMovieRepository : ITVMovieRepository
    {

        private readonly LiveContext _liveContext;
        private readonly IMapper _autoMapper;

        public TVMovieRepository(LiveContext liveContext, IMapper autoMapper)
        {
            this._liveContext = liveContext;
            this._autoMapper = autoMapper;
        }

        private bool GetExists(TVMovie tvMovie)
        {
            var actuall = _liveContext.TVMovies.FirstOrDefault(x => x.Title == tvMovie.Title && x.PlayAt == tvMovie.PlayAt);
            return actuall == null;
        }


        public async Task<List<IconDto>> GetActuallMovies()
        {
            var movies = await _liveContext.TVMovies.Include(x => x.YouTube)
            .Where(x => x.PlayAt >= DateTime.Now.AddHours(-1) && x.PlayAt <= DateTime.Now.AddHours(48))
            .ToListAsync();

            var frontMovies = new List<FrontYouTube>();
            foreach (var movie in movies)
            {
                var dates = movies.Where(x => x.Title == movie.Title && x.PlayAt != movie.PlayAt).Select(x => x.PlayAt).ToList();

                frontMovies.Add(new FrontYouTube(movie, dates));

            }
            var frontMoviesToReturn = new List<FrontYouTube>();
            while (frontMovies.Count != 0)
            {
                var movie = frontMovies[0];
                //var songCount = frontMovies.Where(s => s.YouTube.VideoID == song.YouTube.VideoID).ToList().Count;
                frontMoviesToReturn.Add(movie);

                frontMovies.RemoveAll(m => m.videoId == movie.videoId);


            }
            return frontMoviesToReturn.Select(m => _autoMapper.Map<IconDto>(m)).ToList();
        }

        private DateTime GetLastDate()
        {
            var dates = _liveContext.TVMovies.Select(x => x.PlayAt).ToList();
            return dates.Max();
        }

        public async Task UpdateAsync()
        {

            var toManyReq = false;
            var lastDate = GetLastDate();

            var future = (lastDate - DateTime.Now).Days;

            if (DateTime.Now > lastDate || future == 0)
            {
            }
            future = 2;

            Console.WriteLine(future);

            var allMovies = new List<TVMovie>();


            for (int i = 0; i < future; i++)
            {

                var date = System.DateTime.Now.AddDays(i);
                var day = date.ToString("yyyy-MM-dd");
                Console.WriteLine(day);

                //https://www.telemagazyn.pl/?dzien=2019-10-09&gatunek=film&od=tv_puls#program

                string urlTele = $"https://www.telemagazyn.pl/?dzien={day}&gatunek=film";//#program";

                string urlTele1 = $"https://www.telemagazyn.pl/?dzien={day}&gatunek=film&od=tv_puls";//#program";

                Console.WriteLine(urlTele);
                Console.WriteLine(urlTele1);
                var movies = GetMoviesInfoFromUrl(urlTele, day);
                var movies1 = GetMoviesInfoFromUrl(urlTele1, day);

                allMovies.AddRange(movies);
                allMovies.AddRange(movies1);
            }

            foreach (var movie in allMovies)
            {
                if (toManyReq == false)
                {

                    var actMovie = allMovies.FirstOrDefault(x => x.Title == movie.Title && x.PlayAt != movie.PlayAt && x.YouTube != null);

                    if (actMovie != null)
                    {
                        movie.SetYoutube(actMovie);
                    }
                    else
                    {
                        movie.SetYoutube();

                    }
                }
                else
                {
                    movie.SetWhileYoutube();
                }

                if (movie.YouTube.VideoID.Contains("FirstError"))
                {
                    toManyReq = true;
                }

                var rating = movie.getFilwebRating();
                Console.WriteLine(movie.Title + "  " + movie.YouTube.VideoID);
                await _liveContext.TVMovies.AddAsync(movie);
                _liveContext.TVMovies.RemoveRange(_liveContext.TVMovies.Where(x => x.PlayAt < DateTime.Now));
                await _liveContext.SaveChangesAsync();
            }


            List<TVMovie> GetMoviesInfoFromUrl(string url, string day)
            {
                WebClient client = new WebClient();
                //string url1 = "https://www.telemagazyn.pl/?dzien=2019-11-25&gatunek=film";
                string htmlCode = client.DownloadString(url);

                List<string> names = new List<string>();
                //string pattern = "class[=]{1}[\"]{1}mainCell[\\s]{1}filmy[\"]{1}[>]{1}(.+)[<]{1}[/]{1}a[>]{1}[<]{1}[/]{1}div[>]{1}";
                // string pattern = "class[=]{1}[\"]{1}programInfo[\"]{1}.+[>]{1}([^\"]+)[<]{1}[/]{1}a[>]{1}";

                var mainHTML = new HtmlDocument();
                mainHTML.LoadHtml(htmlCode);

                var pTags = mainHTML.DocumentNode.Descendants("a");

                var tableClass = "tabelaProg hyph";
                var programClass = "programInfo";
                var tables = mainHTML.DocumentNode.SelectNodes("//table[@class='" + tableClass + "']");


                //var date = System.DateTime.Now.AddDays(1);

                var tvMovies = new List<TVMovie>();
                foreach (var table in tables)
                {
                    string emisionDate = day;
                    var tableHtml = new HtmlDocument();
                    tableHtml.LoadHtml(table.OuterHtml);
                    //Console.WriteLine(table.OuterHtml);
                    var movieslist = tableHtml.DocumentNode.SelectNodes("//a[@class='" + programClass + "']");


                    foreach (var movie in movieslist)
                    {


                        if (!movie.InnerText.Contains("Film dokumentalny") && !System.Web.HttpUtility.HtmlDecode(movie.InnerText).Contains("Film krótkometrażowy"))
                        {

                            var tvMovie = new TVMovie(movie.OuterHtml, emisionDate);

                            var ATM = tvMovie.Station != "ATM Rozrywka";
                            var HBO = tvMovie.Station != "HBO";
                            var TvpKultura = tvMovie.Station != "TVP Kultura";
                            var TvpABC = tvMovie.Station != "TVP ABC";
                            var Tvp3 = !tvMovie.Station.Contains("TVP3");

                            //var actuall = _liveContext.TVMovies.FirstOrDefaultAsync(x => x.Title==tvMovie.Title && x.PlayAt == tvMovie.PlayAt);

                            var UNIQ = GetExists(tvMovie);

                            if (ATM && HBO && TvpKultura && UNIQ && TvpABC && Tvp3)
                            {
                                //tvMovie.SetYoutube();
                                tvMovies.Add(tvMovie);
                            }
                        }


                        /*tring title = movieHTML.DocumentNode.Descendants("p").FirstOrDefault().InnerText;
                        Console.WriteLine(title);
                        Console.WriteLine(href); */
                        //Console.WriteLine("--------------------------------------------------------------");
                    }
                    day = DateTime.Parse(day).AddDays(1).ToString("yyyy-MM-dd");
                    // Console.WriteLine("==============================================================");
                }

                return tvMovies;

            }

        }


    }
}