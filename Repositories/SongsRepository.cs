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

namespace Live.Repositories
{
    public class SongsRepository : ISongsRepository
    {
        private readonly LiveContext _liveContext;
        private readonly IMapper _autoMapper;

           public SongsRepository(LiveContext liveContext, IMapper autoMapper )
        {
            this._liveContext = liveContext;
             this._autoMapper = autoMapper;
        }

/*          public async Task<Song> GetByNameFromActual(string name)
        {
          var actualSongs = await _liveContext.ActualSongs.ToListAsync();
          var song = actualSongs.SingleOrDefault(s => s.Name == name);
          return song;
        }
        public async Task<Song> GetByYouTubeFromActual(string id)
        {
          var actualSongs = await _liveContext.ActualSongs.ToListAsync();
          var song = actualSongs.SingleOrDefault(s => s.YouTube.VideoID == id);
          return song;
        }

        
         public async Task<Song> GetByNameFromArchive(string name)
        {
          var actualSongs = await _liveContext.ArchiveSongs.ToListAsync();
          var song = actualSongs.SingleOrDefault(s => s.Name == name);
          return song;
        }
        */

        public async Task<List<SongDto>> GetAllFromArchive()
        {
            var archiveSongs = await _liveContext.ArchiveSongs.Include(x=>x.YouTube)
            .Where(x => x.YouTube.top_.Length<9 && x.YouTube.left_.Length<9)
            .ToListAsync();
           return   archiveSongs.Select(s =>  _autoMapper.Map<SongDto>(s)).ToList();
        }

         public async Task<List<Song>> GetAllActuall()
        {
            var actuallSongs = await _liveContext.Songs.Include(x=>x.YouTube).ToListAsync();
            return actuallSongs;
        } 

        public async Task<ArchiveSong> GetByYouTubeFromArchive(string id)
        {
          var archiveSongs = await _liveContext.ArchiveSongs.Include(x=>x.YouTube).ToListAsync();
          var song = archiveSongs.SingleOrDefault(s => s.YouTube.VideoID == id);
          return song;
        } 


     
         public async Task<ArchiveSong> GetByNameFromArchive(string name)
        {
            var archiveSongs = await _liveContext.ArchiveSongs.Include(x=>x.YouTube).ToListAsync();

           // var actuallSongs = await _liveContext.Songs.Include(x=>x.YouTube).ToListAsync();
            
            var songArch = archiveSongs.SingleOrDefault(s => s.Name == name);
            
            return songArch;

            

        }








    /*1 - zet
    2 - rmf
    3 - eska
    4 - rmf maxx
    5 - antyradio
    6 - rmf classic
    40 - chillizet
    9 - zlote przeboje
    30 - vox fm
    8 - plus */




        public async Task<DateTime> GetLastDate ()
        {
            var actualSongs = await _liveContext.Songs.ToListAsync();
            var dates = actualSongs.Select(s => s.PlayAt).ToList();
            var LastDate = dates.Max();
            return LastDate;

        }

    public async Task UpdateAsync()
    {
          //  var stations = new Dictionary<int, string>(){{1,"zet"},{2,"rmf"},{3,"eska"},{4, "rmfmaxx"},
          //  {5,"antyradio"},{6, "rmfclassic"},{8, "plus"},{9, "zloteprzeboje"},{30, "vox"},{40, "chillizet"}};
            
             var stations = new Dictionary<int, string>(){{2,"rmf"}, {30, "vox"}};
            
            
          // var dateLast = await GetLastDate();

          Console.WriteLine("last date - " + await GetLastDate());

              DateTime dateLast = DateTime.ParseExact(
         "2019-01-05 16:06", "yyyy-MM-dd HH:mm", 
        System.Globalization.CultureInfo.InvariantCulture);

Console.WriteLine(dateLast);

                var dateNow = DateTime.Now;
                int hourNow = dateNow.Hour;
Console.WriteLine(dateNow );

                var hours = (dateNow - dateLast).TotalHours;

                int i = 0;
                int h = 50;
Console.WriteLine(hours);
                if (hours>=24)
                {
                    i = 24;
                
                }

                if(hourNow == dateLast.Hour && hours<24 )
                {
                    i = 0;
              
                }
                if(hourNow != dateLast.Hour && hours<24 )
                {

                    while(h != hourNow)
                    {
                    dateLast = dateLast.AddHours(1);
                    h =  dateLast.Hour;
                    Console.WriteLine(h);
                    i++;
            
                    } 
                }
                Console.WriteLine(i);

            var listOfInitialSongs = new List<Song>();

            for (int j = 0;j<i;j++)
                {
                    //var date = date24.ToString("dd-MM-yyyy");
                    var hourTo = dateNow.AddHours(-j).Hour;
                    var dateBase = dateNow.AddHours(-j);
                    var date = dateNow.AddHours(-j-1).ToString("dd-MM-yyyy");
                    var hourFrom = dateNow.AddHours(-j-1).Hour;
                foreach(var s in stations.Keys)
                {
                    string addres = "https://www.odsluchane.eu/szukaj.php?r="+s+"&date="+date+"&time_from="+hourFrom+"&time_to="+hourTo;
                  //  Console.WriteLine(dateBase);
                    Console.WriteLine(addres);
                  var names = get_names_from_url(addres);

                  foreach(var name in names)
                  {
                      listOfInitialSongs.Add(new Song(name, stations[s], dateBase ));
                  }
                }

                }


                foreach(var song in listOfInitialSongs)
                {
                    Console.WriteLine(song.Name);
                    var archiveSong = await GetByNameFromArchive(song.Name);

                if(archiveSong is null)
                    {
                    song.SetYoutube();
                    await CorrectNameOrUpdateArchive(song);
                    }
                    else
                    {
                    song.SetYoutube(archiveSong);
                    }

                await _liveContext.Songs.AddAsync(song);
                await _liveContext.SaveChangesAsync();
                }
           




        List<string> get_names_from_url(string url)
        {
        WebClient client = new WebClient();
        string htmlCode = client.DownloadString(url);

        string pattern = "class[=]{1}[\"]{1}title-link[\"]{1}[>]{1}([^\"]+)[<]{1}[/]{1}a[>]{1}";
        var reg1 = new Regex(pattern);
        List<string> names = reg1.Matches(htmlCode).Select(s => s.Groups[1].Value.Trim()).ToList();
        return names;
        }

    }

    public async Task CorrectNameOrUpdateArchive(Song song)
    {

        var archiveSong = await GetByYouTubeFromArchive(song.YouTube.VideoID);

        if(archiveSong is null)
        {
            var toArchiveSong = new ArchiveSong(song);
            Console.WriteLine($"Add to archive song with name |{song.Name}| - {song.YouTube.VideoID}");
            await _liveContext.ArchiveSongs.AddAsync(toArchiveSong);
            await _liveContext.SaveChangesAsync();
        }
        else 
        {
            song.CorrectName(archiveSong);
        }
    }


    //public async Task GetByRadioAsync

    public async Task UpdateArchiveAsync(Song actualSong)
    {
       // GetByYouTubeFromArchive

        var actuallSongs = await GetAllActuall();

        foreach(var song in actuallSongs)
        {
            var archiveSong = await GetByYouTubeFromArchive(song.YouTube.VideoID);

            if (archiveSong is null)
            {
                var toArchiveSong = new ArchiveSong(song);
                await _liveContext.ArchiveSongs.AddAsync(toArchiveSong);
                await _liveContext.SaveChangesAsync();
            }  
           // Console.WriteLine(archiveSong.Name);
        }
    }









/*     public async Task Migrate()
    {
       // var old_radio_songs =  await _liveContext.RadioSongs.Where(r => r.YouTubeId != "oMktsOtN9uc").ToListAsync();

         var archSongs =  await _liveContext.ArchiveSongs.Where(s => Regex.IsMatch(s.Name , @"&amp;")).ToListAsync();

         foreach(var s in archSongs)
         {
            Console.WriteLine(s.Name);
            
             s.ReplaceBush(Regex.Replace(s.Name , @"&#039;", "'"));
             _liveContext.ArchiveSongs.Update(s);
             await _liveContext.SaveChangesAsync();
         }  */ 

       /* foreach(var old in old_radio_songs)
        {
           var archive = await _liveContext.ArchiveSongs.Include(x=> x.YouTube).ToListAsync();
           var list = archive.Where(s => s.YouTube.VideoID == old.YouTubeId && s.Name != old.Name)
           .ToList();
           var iscontain = list.Count;

           if(iscontain>0)
           {
               var name = list[0].Name;
               var song = new ArchiveSong(name,old);
               await _liveContext.ArchiveSongs.AddAsync(song);
               await _liveContext.SaveChangesAsync();
           }
           else
           {
                var name = old.Name;
                var song = new ArchiveSong(name,old);
                await _liveContext.ArchiveSongs.AddAsync(song);
                await _liveContext.SaveChangesAsync();
           }
            
        }  */
    } 

   /*  public async Task Migrate()
    {
        var archive = await _liveContext.ArchiveSongs.ToListAsync();

        foreach(var s in archive)
        {
            var theSame = archive.Where(x => x.Name == s.Name).ToList();
            Console.WriteLine(theSame.Count);
            if (theSame.Count>1)
            {
                Console.WriteLine($"Usuwam {s.Name}");
                _liveContext.ArchiveSongs.Remove(s);
               await _liveContext.SaveChangesAsync();
            }
        }

    } */






       
    

}




