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
using Microsoft.Extensions.Logging;
using Serilog;

namespace Live.Repositories
{
    public class SongsRepository : ISongsRepository
    {
        private readonly LiveContext _liveContext;
        private readonly IMapper _autoMapper;

     // private readonly ILogger _logger;


        public SongsRepository(LiveContext liveContext, IMapper autoMapper)
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
            var archiveSongs = await GetByYouTubeFromArchive(id);
            var actuallSongs =  await GetByYouTubeFromActuall(id);

            if(archiveSongs != null)
            {
                _liveContext.ArchiveSongs.RemoveRange(archiveSongs);
            }

            if(actuallSongs.Count>0)
            {
                _liveContext.Songs.RemoveRange(actuallSongs);
            }
           await _liveContext.SaveChangesAsync();
        }

/////////////CHANGE!! - FOR ALL IN ARCHIVE AND IN CURRENT SONGS!!------
       public async Task ChangeYouTubeId(string name, string Id, string toId)
       {
            var actuallSongs =  await GetByNameFromActuall(name);
            var archiveSongs = await GetByYouTubeFromArchive(Id);

            if(actuallSongs.Count>0)
            {
                foreach(var actuallSong in actuallSongs)
                {
                actuallSong.ChangeYouTubeId(toId);
                _liveContext.Update(actuallSong);

                }
            }

            if(archiveSongs.Count>0)
            {
                foreach(var achiveSong in archiveSongs)
                {
                achiveSong.ChangeYouTubeId(toId);
                _liveContext.Update(achiveSong);

                }
            }
           /*  if(archiveSong != null)
            {
            archiveSong.ChangeYouTubeId(toId);

            _liveContext.Update(archiveSong);
            } */
           await _liveContext.SaveChangesAsync();
       }


    public async Task ChangeName(string Id, string name)
       {
            //var archiveSong = await GetByYouTubeFromArchive(Id);
            var actuallSongs =  await GetByYouTubeFromActuall(Id);

            if(actuallSongs.Count>0)
            {
                foreach(var actuallSong in  actuallSongs)
                {
                    actuallSong.ChangeName(name);
                    _liveContext.Update(actuallSong);
                }
            }
         /*    if(archiveSong != null)
            {
            archiveSong.ChangeName(name);
           _liveContext.Update(archiveSong);
            } */
           await _liveContext.SaveChangesAsync();
       }


    public async Task ChangeLocation(string Id, string left, string top)
       {
            var actuallSongs =  await GetByYouTubeFromActuall(Id);
            var archiveSongs = await GetByYouTubeFromArchive(Id);

            if(actuallSongs.Count>0)
            {
                foreach(var actuallSong in actuallSongs )
                {
                    actuallSong.ChangeLocation(left, top);
                    _liveContext.Update(actuallSong);

                }
            }
            if(archiveSongs.Count>0)
            {
                foreach(var archiveSong in archiveSongs )
                {
                    archiveSong.ChangeLocation(left, top);
                    _liveContext.Update(archiveSong);
                }
            }
 
           await _liveContext.SaveChangesAsync();
       }



        public async Task<List<IconDto>> GetAllFromArchive()
        {
            var archiveSongs = await _liveContext.ArchiveSongs.Include(x=>x.YouTube)
            .ToListAsync();
           return archiveSongs.Select(s =>  _autoMapper.Map<IconDto>(s)).ToList();
        }


        public async Task<List<IconDto>> GetAllErrorsFromArchive()
        {
            var archiveSongs = await _liveContext.ArchiveSongs.Include(x=>x.YouTube)
            .ToListAsync();
            var errors = archiveSongs.Where(x => x.YouTube.VideoID.Contains("Error") || x.YouTube.VideoID==x.Name);
           return errors.Select(s =>  _autoMapper.Map<IconDto>(s)).ToList();
        }



        public async Task<List<IconDto>> GetActualByRadioAsync(List<string> stations)
        {  
            var date24 = DateTime.Now.AddHours(-12);
           var all_songs =  await _liveContext.Songs.Include(s => s.YouTube).Where(s => s.PlayAt>=date24).ToListAsync();
            //var all_songs =  await _liveContext.Songs.Include(s => s.YouTube).Where(s => s.PlayAt<date24).ToListAsync();
            


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
              //Console.WriteLine(song.Name);
          }

          return ytFront.Select(x => _autoMapper.Map<IconDto>(x)).ToList();

        }


        public async Task<List<IconDto>> GetActualRandomSongs()
        {  
            var date12 = DateTime.Now.AddHours(-12);
            var allSongs =  await _liveContext.Songs.Include(s => s.YouTube).Where(s => s.PlayAt>=date12).ToListAsync();
            var songs = new List<Song>();
            var songsDto = new List<FrontYouTube>();
            Random random = new Random();
 
        var stations = new Dictionary<int, string>(){{1,"zet"},{2,"rmf"},{3,"eska"},{4, "rmfmaxx"},{9, "zloteprzeboje"},{30, "vox"},{48, "trojka"}};

        var arrayLog  = songsDto.ToArray();
       

        if(allSongs.Count>0) 
        {

          foreach(var radio in stations)
          {
            var radioSongs = allSongs.Where(s => s.Station == radio.Value).ToList();

            if(radioSongs.Count>11)
            {
                while(radioSongs.Count > 11)
                {
                    var index = random.Next(0, radioSongs.Count-1);
                    radioSongs.RemoveAt(index);
                }
            }
            songs.AddRange(radioSongs);   
          }
        }  

        if(songs.Count>0)
        {

          while(songs.Count != 0)
          {
              var song = songs[0];
              var songCount = songs.Where(s => s.YouTube.VideoID == song.YouTube.VideoID).ToList().Count;
            songs.RemoveAll(s => s.YouTube.VideoID == song.YouTube.VideoID);
              songsDto.Add(new FrontYouTube(song, songCount));
          }
        }

          return songsDto.Select(x => _autoMapper.Map<IconDto>(x)).ToList();

        }


         public async Task<List<Song>> GetAllActuall()
        {
            var actuallSongs = await _liveContext.Songs.Include(x=>x.YouTube).ToListAsync();
            return actuallSongs;
        } 

        public async Task<List<ArchiveSong>> GetByYouTubeFromArchive(string id)
        {
          var archiveSongs = await _liveContext.ArchiveSongs.Include(x=>x.YouTube).ToListAsync();
          var songs = archiveSongs.Where(s => s.YouTube.VideoID == id).ToList();
          return songs;
        }

        public async Task<List<Song>> GetByYouTubeFromActuall(string id)
        {
          var actualSongs = await _liveContext.Songs.Include(x=>x.YouTube).ToListAsync();
          var songs = actualSongs.Where(s => s.YouTube.VideoID == id).ToList();
          return songs;
        } 


   public async Task<List<Song>> GetByNameFromActuall(string name)
        {
            var actuallSongs = await _liveContext.Songs.Include(x=>x.YouTube).ToListAsync();
            var songs = actuallSongs.Where(s => s.Name == name).ToList();
            return songs;
        }
     
         public async Task<ArchiveSong> GetByNameFromArchive(string name)
        {
            var archiveSongs = await _liveContext.ArchiveSongs.Include(x=>x.YouTube).ToListAsync();

           // var actuallSongs = await _liveContext.Songs.Include(x=>x.YouTube).ToListAsync();
            
            var songArch = archiveSongs.FirstOrDefault(s => s.Name == name);
            
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

            var stations = new Dictionary<int, string>(){{1,"zet"},{2,"rmf"},{3,"eska"},{4, "rmfmaxx"},{9, "zloteprzeboje"},{30, "vox"},{48, "trojka"}};
           var dateLast = await GetLastDate();
                //Console.WriteLine(dateLast);
                var dateNow = DateTime.Now;
                int hourNow = dateNow.Hour;
                //Console.WriteLine(dateNow );

                var hours = (dateNow - dateLast).TotalHours;

        _liveContext.Songs.RemoveRange(_liveContext.Songs.Where(s => s.PlayAt<dateNow.AddHours(-25)));


                int i = 0;
                int h = 50;
                if (hours>=12)
                {
                    i = 12;
                
                }

                if(hourNow == dateLast.Hour && hours<12 )
                {
                    i = 0;
              
                }
                if(hourNow != dateLast.Hour && hours<12 )
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
                    //Console.WriteLine(addres);
                  var names = getNamesFromUrl(addres);
                
                if(names.Count>0)
                {
                  foreach(var name in names)
                  {
                      listOfInitialSongs.Add(new Song(name, stations[s], dateBase ));
                  }
                }
                }

                }

 Log.Information($"Radio Songs UPDATED with {listOfInitialSongs.Count} songs");

            if(listOfInitialSongs.Count>0)
            {
                var toManyReq = false;
              songsCount = listOfInitialSongs.Count;
                foreach(var song in listOfInitialSongs)
                {   
                    
                    //Console.WriteLine(songsCount);
                    
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
                        Log.Warning("First ERROR from Song Update");
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

        var errors = listOfInitialSongs.Where(x => x.YouTube.VideoID.Contains("Error")).ToList();
        Log.Information($"Finish radio songs update with {errors.Count} youtube errors");
        
        Console.WriteLine("-----------------------FINISH UPDATE----------------------");
           
        List<string> getNamesFromUrl(string url)
        {
            WebClient client = new WebClient() { Encoding = System.Text.Encoding.UTF8 };
            string htmlCode = "";
            try 
            {
            htmlCode = client.DownloadString(url);
            }
            catch(Exception ex)
            {
                Log.Error($"Exception DownloadString: {url}");
                Log.Error(ex.Message);
            }

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