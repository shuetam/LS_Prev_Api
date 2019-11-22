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

        public async Task<List<IconDto>> GetFromArchiveByIndex(int i, int j)
        {
            var archiveSongs = await _liveContext.ArchiveSongs.Include(x=>x.YouTube).ToListAsync();
            int archCount = archiveSongs.Count;
            if(j>archCount)
            {
                j=archCount-1;
            }

            var songs = archiveSongs.Where(s => archiveSongs.IndexOf(s)>=i && archiveSongs.IndexOf(s)<=j).ToList();
            return songs.Select(s =>  _autoMapper.Map<IconDto>(s)).ToList();
        }

      public async Task DeleteByYouTubeId(string id)
        {
            var archiveSong = await GetByYouTubeFromArchive(id);
            var actuallSong =  await GetByYouTubeFromActuall(id);

            if(archiveSong != null)
            {
                _liveContext.ArchiveSongs.Remove(archiveSong);
            }

            if(actuallSong != null)
            {
                _liveContext.Songs.Remove(actuallSong);
            }
           await _liveContext.SaveChangesAsync();
        }

/////////////CHANGE!! - FOR ALL IN ARCHIVE AND IN CURRENT SONGS!!------
       public async Task ChangeYouTubeId(string Id, string toId)
       {
            var archiveSong = await GetByYouTubeFromArchive(Id);
            var actuallSong =  await GetByYouTubeFromActuall(Id);

            if(actuallSong != null)
            {
                actuallSong.ChangeYouTubeId(toId);
                _liveContext.Update(actuallSong);
            }
            if(archiveSong != null)
            {
            archiveSong.ChangeYouTubeId(toId);

            _liveContext.Update(archiveSong);
            }
           await _liveContext.SaveChangesAsync();
       }


        public async Task ChangeName(string Id, string name)
       {
            var archiveSong = await GetByYouTubeFromArchive(Id);
            var actuallSong =  await GetByYouTubeFromActuall(Id);

            if(actuallSong != null)
            {
                actuallSong.ChangeName(name);
                _liveContext.Update(actuallSong);
            }
            if(archiveSong != null)
            {
            archiveSong.ChangeName(name);

            _liveContext.Update(archiveSong);
            }
           await _liveContext.SaveChangesAsync();
       }




        public async Task<List<IconDto>> GetAllFromArchive()
        {
            var archiveSongs = await _liveContext.ArchiveSongs.Include(x=>x.YouTube)
            .ToListAsync();
           return archiveSongs.Select(s =>  _autoMapper.Map<IconDto>(s)).ToList();
        }


        public async Task<List<IconDto>> GetActualByRadioAsync(List<string> stations)
        {  
            var date24 = DateTime.Now.AddHours(-24);
           var all_songs =  await _liveContext.Songs.Include(s => s.YouTube).Where(s => s.PlayAt>=date24).ToListAsync();
            //var all_songs =  await _liveContext.Songs.Include(s => s.YouTube).Where(s => s.PlayAt<date24).ToListAsync();
            
            foreach(var s in all_songs)
            {
                Console.WriteLine(s.Name);
            }

            var songs = new List<Song>();
            var ytFront = new List<FrontYouTube>();

          foreach(string radio in stations)
          {
              songs.AddRange(all_songs.Where(s => s.Station == radio).ToList());
          }
        
          while(songs.Count != 0)
          {
              var song = songs[0];
              var songCount = songs.Where(s => s.YouTube.VideoID == song.YouTube.VideoID).ToList().Count;
              songs.RemoveAll(s => s.YouTube.VideoID == song.YouTube.VideoID);
              ytFront.Add(new FrontYouTube(song, songCount));
              Console.WriteLine(song.Name);
          }

          return ytFront.Select(x => _autoMapper.Map<IconDto>(x)).ToList();

        }


        public async Task<List<IconDto>> GetActualRandomSongs()
        {  
            var date24 = DateTime.Now.AddHours(-24);
            var all_songs =  await _liveContext.Songs.Include(s => s.YouTube).Where(s => s.PlayAt>=date24).ToListAsync();
            var songs = new List<Song>();
            var songsDto = new List<FrontYouTube>();
            Random random = new Random();
           /*  var stations = new Dictionary<int, string>(){{1,"zet"},{2,"rmf"},{3,"eska"},{4, "rmfmaxx"},
           {5,"antyradio"},{6, "rmfclassic"},{8, "plus"},{9, "zloteprzeboje"},{30, "vox"},{40, "chillizet"}};
 */
        var stations = new Dictionary<int, string>(){{1,"zet"},{2,"rmf"},{3,"eska"},{4, "rmfmaxx"},{9, "zloteprzeboje"},{30, "vox"}};
            
          foreach(var radio in stations)
          {
            for(int i = 0;i<10;i++)
            {
            
            var randomSong = all_songs.Where(s => s.Station == radio.Value).ToList()[random.Next(all_songs.Where(s => s.Station == radio.Value).ToList().Count)];
            all_songs.RemoveAll(s=>s.YouTube.VideoID == randomSong.YouTube.VideoID);
            songs.Add(randomSong);

            }
          }
        
          foreach(var s in songs)
          {
              Console.WriteLine($"{s.Name}  --- {s.Station}");
          }
        
          while(songs.Count != 0)
          {
              var song = songs[0];
              var songCount = songs.Where(s => s.YouTube.VideoID == song.YouTube.VideoID).ToList().Count;

//if(!song.YouTube.VideoID.Contains("Error"))
              // {
                songs.RemoveAll(s => s.YouTube.VideoID == song.YouTube.VideoID);
              // }
              songsDto.Add(new FrontYouTube(song, songCount));
          }

          return songsDto.Select(x => _autoMapper.Map<IconDto>(x)).ToList();

        }


         public async Task<List<Song>> GetAllActuall()
        {
            var actuallSongs = await _liveContext.Songs.Include(x=>x.YouTube).ToListAsync();
            return actuallSongs;
        } 

        public async Task<ArchiveSong> GetByYouTubeFromArchive(string id)
        {
          var archiveSongs = await _liveContext.ArchiveSongs.Include(x=>x.YouTube).ToListAsync();
          var song = archiveSongs.FirstOrDefault(s => s.YouTube.VideoID == id);
          return song;
        }

        public async Task<Song> GetByYouTubeFromActuall(string id)
        {
          var archiveSongs = await _liveContext.Songs.Include(x=>x.YouTube).ToListAsync();
          var song = archiveSongs.FirstOrDefault(s => s.YouTube.VideoID == id);
          return song;
        } 


     
         public async Task<ArchiveSong> GetByNameFromArchive(string name)
        {
            var archiveSongs = await _liveContext.ArchiveSongs.Include(x=>x.YouTube).ToListAsync();

           // var actuallSongs = await _liveContext.Songs.Include(x=>x.YouTube).ToListAsync();
            
            var songArch = archiveSongs.SingleOrDefault(s => s.Name == name);
            
            return songArch;
        }

        public async Task<DateTime> GetLastDate ()
        {
            var actualSongs = await _liveContext.Songs.ToListAsync();
            var dates = actualSongs.Select(s => s.PlayAt).ToList();
            var LastDate = dates.Max();
            return LastDate;

        }

    public async Task UpdateAsync()
    {

         // var stations = new Dictionary<int, string>(){{1,"zet"},{2,"rmf"},{3,"eska"},{4, "rmfmaxx"},
          // {5,"antyradio"},{6, "rmfclassic"},{8, "plus"},{9, "zloteprzeboje"},{30, "vox"},{40, "chillizet"}};
            
             var stations = new Dictionary<int, string>(){{1,"zet"},{2,"rmf"},{3,"eska"},{4, "rmfmaxx"},{9, "zloteprzeboje"},{30, "vox"}};
            
    //  var stations = new Dictionary<int, string>(){{40, "chillizet"}, {30, "vox"},{9, "zloteprzeboje"}};
            
            
           var dateLast = await GetLastDate();

      //    Console.WriteLine("last date - " + await GetLastDate());

  //   DateTime dateLast = DateTime.ParseExact(
    //     "2019-03-18 21:06", "yyyy-MM-dd HH:mm", 
    //    System.Globalization.CultureInfo.InvariantCulture); 

                Console.WriteLine(dateLast);



                var dateNow = DateTime.Now;
                int hourNow = dateNow.Hour;
                Console.WriteLine(dateNow );

                var hours = (dateNow - dateLast).TotalHours;

        _liveContext.Songs.RemoveRange(_liveContext.Songs.Where(s => s.PlayAt<dateNow.AddHours(-25)));


                int i = 0;
                int h = 50;
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

                    i++;
            
                    } 
                }


            var listOfInitialSongs = new List<Song>();
            var songsCount = 0;

            for (int j = 0;j<i;j++)
                {

                    var hourTo = dateNow.AddHours(-j).Hour;
                    var dateBase = dateNow.AddHours(-j);
                    var date = dateNow.AddHours(-j-1).ToString("dd-MM-yyyy");
                    var hourFrom = dateNow.AddHours(-j-1).Hour;
                foreach(var s in stations.Keys)
                {
                    string addres = "https://www.odsluchane.eu/szukaj.php?r="+s+"&date="+date+"&time_from="+hourFrom+"&time_to="+hourTo;
                    Console.WriteLine(addres);
                  var names = get_names_from_url(addres);
                
                if(names.Count>0)
                {
                  foreach(var name in names)
                  {
                      listOfInitialSongs.Add(new Song(name, stations[s], dateBase ));
                  }
                }
                }

                }

            if(listOfInitialSongs.Count>0)
            {
                var toManyReq = false;
              songsCount = listOfInitialSongs.Count;
                foreach(var song in listOfInitialSongs)
                {   
                    
                    Console.WriteLine(songsCount);
                    
                    var archiveSong = await GetByNameFromArchive(song.Name);

                if(archiveSong is null)
                {
                    if(toManyReq == false)
                    {
                        song.SetYoutube();
                    }
                    else
                    {
                        song.SetWhileYoutube();
                    }

                    if (song.YouTube.VideoID.Contains("FirstError"))
                    {
                        toManyReq = true;
                    }
                        await AddToArchiveAsync(song);
                }
                else
                {
                    song.SetYoutube(archiveSong);
                }

                    await _liveContext.Songs.AddAsync(song);

                songsCount = songsCount-1;
                }
                await _liveContext.SaveChangesAsync();
            }
        Console.WriteLine("-----------------------FINISH UPDATE----------------------");
           
        List<string> get_names_from_url(string url)
        {
            WebClient client = new WebClient();
            string htmlCode = client.DownloadString(url);
            List<string> names = new List<string>();
            string pattern = "class[=]{1}[\"]{1}title-link[\"]{1}[>]{1}([^\"]+)[<]{1}[/]{1}a[>]{1}";
            var reg1 = new Regex(pattern);
        if(reg1.IsMatch(htmlCode))
        {
            names = reg1.Matches(htmlCode).Select(s => s.Groups[1].Value.Trim()).ToList();
        }
            return names;
        }
    }

    public async Task AddToArchiveAsync(Song song)
    {
            var toArchiveSong = new ArchiveSong(song);
            await _liveContext.ArchiveSongs.AddAsync(toArchiveSong);
            await _liveContext.SaveChangesAsync();

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
} 
}