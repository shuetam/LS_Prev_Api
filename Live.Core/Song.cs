using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;


namespace Live.Core
{
    public class Song : Entity
    {
        public string Name {get; protected set;}
        public YouTube YouTube {get; protected set;}
        public string Station {get; protected set;}
        
        public DateTime PlayAt {get; protected set;}
/* 
        public Song(string station, Song archive_song)
        {
            this.PlayAt = DateTime.Now;
            this.Station = station;
            this.Name= archive_song.Name;
            this.YouTube = archive_song.YouTube;
        } */

       protected Song()
        {}
        public Song(string name, string station, DateTime date)
        {
            this.PlayAt = date; 

            if(Regex.IsMatch(name , @"&#039;"))
            {
            name = Regex.Replace(name , @"&#039;", "'");
            }

            if(Regex.IsMatch(name , @"&amp;"))
            {
            name = Regex.Replace(name , @"&amp;", "&");
            }

            this.Name = name;


            this.Station = station;
        }

            public void  SetYoutube()
            {
                this.YouTube = new YouTube(this.Name);
            }

            public void  SetYoutube(ArchiveSong songFromDatabse)
            {
                this.YouTube = songFromDatabse.YouTube;
                Console.WriteLine("----------------F R O M   ARCHIVE-----------------------");
                Console.WriteLine(songFromDatabse.Name);
                Console.WriteLine(songFromDatabse.YouTube.VideoID);
            }

            public void  CorrectName(ArchiveSong songFromDatabse)
            {
                Console.WriteLine("---------------------CORRECT  NAME------------------------");
                Console.WriteLine($"FROM  ///{this.Name}///  TO  ///{songFromDatabse.Name}///");
                this.Name = songFromDatabse.Name;
            }

        public void ChangeYouTubeId(string id)
        {
            this.YouTube.VideoID = id;
        }

        public void ChangeName(string name)
        {
            this.Name = name;
        }

        public void  ReplaceBush(string name)
            {
                this.Name = name;
            }


    }
}