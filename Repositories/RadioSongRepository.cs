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
    public class RadioSongRepository : IRadioSongRepository
    {
        
        private readonly LiveContext _liveContext;

        private readonly IMapper _autoMapper;

        public RadioSongRepository(LiveContext liveContext, IMapper autoMapper )
        {
            this._liveContext = liveContext;
            this._autoMapper = autoMapper;
        
        }

      public async  Task <List<object>> GetTryAsync()
      {
          List<object> l1 = new List<object>();
          var songs =  await _liveContext.RadioSongs.ToListAsync();
          var trySongs =  songs.Select(o => new { o.Count , o}).ToList();
          foreach(var i in trySongs)
          {
              l1.Add(i);
          }
          return l1;
      }

        
        public async Task<List<RadioSongDto>> GetAllAsync()
        {
          var songs =  await _liveContext.RadioSongs.ToListAsync();
          return  songs.Select(s =>  _autoMapper.Map<RadioSongDto>(s)).ToList();
        } 



        public async Task<List<RadioSongDto>> GetByRadioAsync(List<string> stations)
        {
          var songs =  await _liveContext.RadioSongs.ToListAsync();
          var radio_songs = new List<RadioSong>();

        if(stations.Contains("rmf"))
        {  
           radio_songs.AddRange(songs.Where(s=> s.CountRmf>0));
        }
        if(stations.Contains("rmfmaxx"))
        {
           radio_songs.AddRange(songs.Where(s=> s.CountRmfMaxx>0)); 
           
        }
        if(stations.Contains("zet"))
        {
           radio_songs.AddRange(songs.Where(s=> s.CountZet>0));
        
        }
        if(stations.Contains("plus"))
        {
           radio_songs.AddRange(songs.Where(s=> s.CountPlus>0));
         
        }
        if(stations.Contains("eska"))
        {
           radio_songs.AddRange(songs.Where(s=> s.CountEska>0));
            
        }
        if(stations.Contains("chillizet"))
        {
           radio_songs.AddRange(songs.Where(s=> s.CountChilliZet>0));
         
        }
        if(stations.Contains("antyradio"))
        {
           radio_songs.AddRange(songs.Where(s=> s.CountAntyRadio>0)); 
          
        }
        if(stations.Contains("zloteprzeboje"))
        {
           radio_songs.AddRange(songs.Where(s=> s.CountZlotePrzeboje>0));
          
        }
        if(stations.Contains("vox"))
        {
           radio_songs.AddRange(songs.Where(s=> s.CountVox>0));
          
        }
        if(stations.Contains("rmfclassic"))
        {
           radio_songs.AddRange(songs.Where(s=> s.CountRmfClassic>0));
           
        }


        var hash_radio_songs = new HashSet<RadioSong>(radio_songs).ToList();

        foreach(var h_s in hash_radio_songs)
        {
            h_s.setTotalCount(stations);
        }

        return  hash_radio_songs.Select(s =>  _autoMapper.Map<RadioSongDto>(s)).ToList();

        } 


        public async Task SetYouTubeIdAsync(int from, int to)
        {
            var all_songs =  await _liveContext.RadioSongs.ToListAsync();
            Console.WriteLine(all_songs.Count);
            var songs = all_songs.Where(x => all_songs.IndexOf(x)> from && all_songs.IndexOf(x)<to);

            foreach(var s in songs)
            {
            s.setSongID(s.Name);
            await _liveContext.SaveChangesAsync();
            System.Threading.Thread.Sleep(3000);

            }
        }      
public async Task UpdateAsync()
{
            var addresses = GetAddresses();

            var songs_names =  new List<Tuple<string, int>>();

            foreach (var ad in addresses)
            {
            
                songs_names.AddRange( get_names_from_url(ad.Key, ad.Value).ToList());
            }

            songs_names.Sort();
       

          //  var list_of_songs = new List<RadioSong>();
            var hash_songs = new HashSet<string>();

            foreach(var tu in songs_names)
            {
                hash_songs.Add(tu.Item1);
            }

            var hash_songs_list = hash_songs.ToList();

            /* foreach(var name in hash_songs.ToList())
            {
                await  _liveContext.RadioSongs.AddAsync(new RadioSong(name));
                await _liveContext.SaveChangesAsync();
            }  */ 

            var r_stations = new List<int>(){1,2,3,4,5,6,40,9,30,8};

           // var list_from_database = await _liveContext.RadioSongs.ToListAsync();

              foreach(var song in hash_songs_list)   
              {
                var song_to_data = new RadioSong(song);

                  foreach(var station in r_stations)
                  {
                      int count = songs_names.Where(t => t.Item1 == song && t.Item2==station).ToList().Count;
                    
                    if(count>0)
                    {
                        Console.WriteLine(song+"------"+station+"-----"+count);
                        song_to_data.setCount(station,count);
                    }

                       /*  int count = 100;
                        song.setCount(station, count);      
                        _liveContext.Update(song);
                       await _liveContext.SaveChangesAsync(); */

                   //    Console.WriteLine(song.Name +"---"+ station +"------"+count);
                  }
                   await  _liveContext.RadioSongs.AddAsync(song_to_data);
                   await _liveContext.SaveChangesAsync();
              }
}




        List<Tuple<string, int>> get_names_from_url(string url, int radio)
        {
        WebClient client = new WebClient();
        string htmlCode = client.DownloadString(url);

        string pattern = "class[=]{1}[\"]{1}title-link[\"]{1}[>]{1}([^\"]+)[<]{1}[/]{1}a[>]{1}";
        var reg1 = new Regex(pattern);
        List<string> names = reg1.Matches(htmlCode).Select(s => s.Groups[1].Value.Trim()).ToList();
        names.Sort();
        var tuples = new List<Tuple<string, int>>();
         foreach(var n in names)
        {
            tuples.Add(new Tuple<string, int>(n,radio));
        } 
        return tuples.ToList();
        }


        Dictionary<string,int> GetAddresses()
    {
        var addresses = new Dictionary<string, int>();


        var stations = new List<int>(){1,2,3,4,5,6,40,9,30,8};

    /* 
    1 - zet
    2 - rmf
    3 - eska
    4 - rmf maxx
    5 - antyradio
    6 - rmf classic
    40 - chillizet
    9 - zlote przeboje
    30 - vox fm
    8 - plus
     */
            

       //     https://www.odsluchane.eu/szukaj.php?r=2&date=07-11-2018&time_from=14&time_to=16

    foreach(var v in stations)
    {
            var dateNow = DateTime.Now;
            var date24 = dateNow.AddHours(-24);
            if(int.Parse(date24.Hour.ToString())%2!=0)
           {
               date24 = date24.AddHours(1);
           }

        for (int j = 0;j<12;j++)
          {
            var date = date24.ToString("dd-MM-yyyy");
            var hour1 = date24.Hour;
            date24 = date24.AddHours(2);
            var hour2 = date24.Hour;
            var station = v.ToString();
            string addres = "https://www.odsluchane.eu/szukaj.php?r="+station+"&date="+date+"&time_from="+hour1+"&time_to="+hour2;
            addresses.Add(addres, v); 
          }
    }
    return addresses;
       
        }




    }

}