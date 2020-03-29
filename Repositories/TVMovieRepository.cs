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
            .Where(x => x.PlayAt >= DateTime.Now.AddHours(-1) && x.PlayAt <= DateTime.Now.AddHours(24))
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

        var tvPrograms = new List<string>() {
            "https://www.telemagazyn.pl/tvp_1/",
            "https://www.telemagazyn.pl/tvp_2/",
            "https://www.telemagazyn.pl/polsat/",
            "https://www.telemagazyn.pl/tvn/",
            "https://www.telemagazyn.pl/tvn_7/",
            "https://www.telemagazyn.pl/tv_4/",
            "https://www.telemagazyn.pl/tv_puls/",
            "https://www.telemagazyn.pl/stopklatka/",
            "https://www.telemagazyn.pl/fokus_tv/",
            "https://www.telemagazyn.pl/nowa_tv/",
            "https://www.telemagazyn.pl/metro/",
            "https://www.telemagazyn.pl/wp1/",
            "https://www.telemagazyn.pl/zoom_tv/",
        };

            var toManyReq = false;
            var lastDate = GetLastDate();

            var future = (lastDate - DateTime.Now).Days;

            if (DateTime.Now > lastDate || future == 0)
            {
            }
            future = 1;

            //Console.WriteLine(future);

            var allMovies = new List<TVMovie>();

                // var date = System.DateTime.Now.AddDays(i);
               // var day = date.ToString("yyyy-MM-dd");
                //Console.WriteLine(day);

                //https://www.telemagazyn.pl/?dzien=2019-10-09&gatunek=film&od=tv_puls#program

               // string urlTele = $"https://www.telemagazyn.pl/?dzien={day}&gatunek=film";//#program";

               // string urlTele1 = $"https://www.telemagazyn.pl/?dzien={day}&gatunek=film&od=tv_puls";//#program";

               // Console.WriteLine(urlTele);
               // Console.WriteLine(urlTele1);
               // var movies = GetMoviesInfoFromUrl(urlTele, day);
               // var movies1 = GetMoviesInfoFromUrl(urlTele1, day);

               for(int i=0;i<2;i++)
               {

               ////--0 or 1 day for dev version, 2 days to future for realese version--///
                var date = System.DateTime.Now.AddDays(i);
                var day = date.ToString("yyyy-MM-dd");
                
                foreach(var stat in tvPrograms)
                {
                    var progMovies = await GetMoviesForTvStationAsync(stat, day);
                    allMovies.AddRange(progMovies);
                }
               }

                //allMovies.AddRange(movies);
               // allMovies.AddRange(movies1);
            
 var moviesCount = allMovies.Count;
            foreach (var movie in allMovies)
            {

            var exists = await  GetTheSameFromActual(movie);

if(exists is null)
{
            Console.WriteLine(moviesCount);

            var archiveMovie = await GetByNameFromArchive(movie.TrailerSearch);
                if(archiveMovie is null)
                {
                    if(toManyReq == false)
                    {
                        movie.SetYoutube();
                    }
                    else
                    {
                        movie.SetWhileYoutube();
                    }

                    if (movie.YouTube.VideoID.Contains("FirstError"))
                    {
                        toManyReq = true;
                    }
                    await AddToArchiveAsync(movie);
                }
                else
                {
                    movie.SetYoutubeFromArchive(archiveMovie);
                }

                    //await _liveContext.Songs.AddAsync(movie);

                var rating = movie.getFilwebRating();
                Console.WriteLine(movie.Title + "  " + movie.YouTube.VideoID);
                await _liveContext.TVMovies.AddAsync(movie);
                await _liveContext.SaveChangesAsync();
                moviesCount = moviesCount-1;
            }
           // _liveContext.TVMovies.RemoveRange(_liveContext.TVMovies.Where(x => x.PlayAt < DateTime.Now.AddHours(-2)));
        }
              _liveContext.TVMovies.RemoveRange(_liveContext.TVMovies.Where(x => x.PlayAt < DateTime.Now.AddHours(-2)));
            await _liveContext.SaveChangesAsync();
             Console.WriteLine("-----------------------FINISH UPDATE----------------------");
    }

            async Task<List<TVMovie>> GetMoviesForTvStationAsync(string url, string day)
            {
                    var programDay = $"?dzien={day}";
                     WebClient client = new WebClient{ Encoding = System.Text.Encoding.UTF8 };
                     var movieList = new List<TVMovie>();
                try 
                {
                    var htmlCode = "";
                    var movieUrl = url + programDay;
                    await Task.Run(() =>
                    {
                        htmlCode = client.DownloadString(movieUrl); 
                    });

                    var mainHTML = new HtmlDocument();
                     await Task.Run(() =>
                    {
                        mainHTML.LoadHtml(htmlCode);
                    });

                    //var moviesLi = mainHTML.DocumentNode.SelectNodes("//li[@class='filmy']");
                    
                    var programList = mainHTML.DocumentNode.SelectSingleNode("//div[@class='lista']").InnerHtml;
                 


                    var statHTML = new HtmlDocument();
                        await Task.Run(() =>
                        {
                            statHTML.LoadHtml(programList); 
                        });
                        
                    var hours = statHTML.DocumentNode.SelectNodes("//em")
                                    .Select(x => double.Parse(x.InnerText.Trim().Replace(':',','))).ToList();

                    var moviesLi = statHTML.DocumentNode.SelectNodes("//li");
                    
                    if(moviesLi != null)
                    {
                        var moviesProgList = moviesLi.Where(x => x.OuterHtml.Contains("programInfo")).ToList();
                        int i = 0;
                        foreach(var movie in moviesProgList)
                        {
                            var restHours = hours.Skip(i).ToList();
                            
                            var nextDay = true;
                            if(restHours.Count>0)
                            {
                               Console.WriteLine(restHours[0]);
                                 nextDay = restHours.All(x => x<700);
                            }

                            i++;
                            if(movie.Attributes["class"].Value == "filmy")
                            {
                                var html = movie.InnerHtml;
                                var doc = new HtmlDocument();
                                    await Task.Run(() =>
                                    {
                                        doc.LoadHtml(html);
                                    });
                                var docHref = doc.DocumentNode.SelectSingleNode("//a[@class='programInfo']");
                                if(docHref != null)
                                {
                                    var href = docHref.Attributes["href"].Value;
                                    var movieObj = new TVMovie(html, href, day, nextDay);
                                    movieList.Add(movieObj);
                                }
                            }
                        }
                    }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine("Something wrong in program --> " + url);
                        Console.WriteLine(ex.Message);
                    }

                    return movieList;
            }
        

                /////deprecated///////
            private List<TVMovie> GetMoviesInfoFromUrl(string url, string day)
            {
                WebClient client = new WebClient(){ Encoding = System.Text.Encoding.UTF8 };
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


    public async Task ChangeYouTubeId(string Id, string toId)
       {
            var actuallMovies =  await GetByYouTubeFromActuall(Id);
            var archiveMovies = await GetByYouTubeFromArchive(Id);

            if(actuallMovies.Count>0 )
            {
                foreach(var actuallMovie in actuallMovies)
                {
                    actuallMovie.ChangeYouTubeId(toId);
                    _liveContext.Update(actuallMovie);
                }
            }

             if(archiveMovies.Count>0 )
            {
                foreach(var archiveMovie in archiveMovies)
                {
                    archiveMovie.ChangeYouTubeId(toId);
                    _liveContext.Update(archiveMovie);
                }
            }


      /*   if(archiveSong != null)
            {
            archiveSong.ChangeYouTubeId(toId);

            _liveContext.Update(archiveSong);
            } */

           await _liveContext.SaveChangesAsync();
       }


        public async Task ChangeName(string Id, string name)
       {
           // var archiveMovies = await GetByYouTubeFromArchive(Id);
            var actuallMovies =  await GetByYouTubeFromActuall(Id);

            if(actuallMovies.Count>0)
            {
                foreach(var actuallMovie in actuallMovies)
                {
                    actuallMovie.ChangeName(name);
                    _liveContext.Update(actuallMovie);
                }

            }

/*             if(archiveMovie != null)
            {
            archiveMovie.ChangeName(name);

            _liveContext.Update(archiveMovie);
            } */

           await _liveContext.SaveChangesAsync();
       }


       public async Task ChangeLocation(string Id, string left, string top)
       {
            var archiveMovies = await GetByYouTubeFromArchive(Id);
            var actuallMovies =  await GetByYouTubeFromActuall(Id);

            if(actuallMovies.Count>0)
            {
                foreach(var actuallMovie in actuallMovies)
                {
                    actuallMovie.ChangeLocation(left, top);
                    _liveContext.Update(actuallMovie);

                }
            }

           if(archiveMovies.Count>0)
            {
                foreach(var archiveMovie in archiveMovies )
                {
                archiveMovie.ChangeLocation(left, top);
                _liveContext.Update(archiveMovie);

                }
            } 

           await _liveContext.SaveChangesAsync();
       }
       
        public async Task<List<TVMovie>> GetByYouTubeFromActuall(string id)
        {
          var movies = await _liveContext.TVMovies.Include(x=>x.YouTube).ToListAsync();
          var movie = movies.Where(s => s.YouTube.VideoID == id).ToList();
          return movie;
        } 


   public async Task<List<ArchiveMovie>> GetByYouTubeFromArchive(string id)
        {
          var movies = await _liveContext.ArchiveMovies.Include(x=>x.YouTube).ToListAsync();
          var movie = movies.Where(s => s.YouTube.VideoID == id).ToList();
          return movie;
        } 


        public async Task<List<IconDto>> GetAllErrorsFromArchive()
        {
            var archiveSongs = await _liveContext.ArchiveMovies.Include(x=>x.YouTube)
            .ToListAsync();
            var errors = archiveSongs.Where(x => x.YouTube.VideoID.Contains("Error") || x.YouTube.VideoID==x.Name);
           return errors.Select(s =>  _autoMapper.Map<IconDto>(s)).ToList();
        }

        public async Task DeleteByYouTubeId(string id)
        {
            var archiveMovies = await GetByYouTubeFromArchive(id);
            var actuallMovies =  await GetByYouTubeFromActuall(id);

          if(archiveMovies.Count >0)
            {
                _liveContext.ArchiveMovies.RemoveRange(archiveMovies);
            } 

            if(actuallMovies.Count > 0)
            {
                _liveContext.TVMovies.RemoveRange(actuallMovies);
            }
           await _liveContext.SaveChangesAsync();
        }


    public async Task AddToArchiveAsync(TVMovie movie)
    {
            var toArchiveMovie = new ArchiveMovie(movie);
            await _liveContext.ArchiveMovies.AddAsync(toArchiveMovie);
            await _liveContext.SaveChangesAsync();

    }

        public async Task<ArchiveMovie> GetByNameFromArchive(string name)
        {
            var archiveMovies = await _liveContext.ArchiveMovies.Include(x=>x.YouTube).ToListAsync();
            var movie = archiveMovies.FirstOrDefault(s => s.Name == name);
            return movie;
        }

        public async Task<TVMovie> GetTheSameFromActual(TVMovie movie)
        {
            var movies = await _liveContext.TVMovies.Include(x=>x.YouTube).ToListAsync();
            var exMovie = movies.FirstOrDefault(s => s.TrailerSearch == movie.TrailerSearch && s.PlayAt==movie.PlayAt);
            return exMovie;
        }
    }

}
