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
    public class ArchiveSong : Entity
    {
        public string Name {get; protected set;}
        public YouTube YouTube {get; protected set;}
/* 
        public Song(string station, Song archive_song)
        {
            this.PlayAt = DateTime.Now;
            this.Station = station;
            this.Name= archive_song.Name;
            this.YouTube = archive_song.YouTube;
        } */

       protected ArchiveSong()
        {}
       

        public ArchiveSong(string name, RadioSong song)
        {

            this.Name = name;
            this.YouTube = new YouTube(song);
        }

        public ArchiveSong(Song song)
        {

            this.Name = song.Name;
            this.YouTube = song.YouTube;
        }

        public void ChangeYouTubeId(string id)
        {
            this.YouTube.VideoID = id;
        }

        public void ChangeName(string name)
        {
            this.Name = name;
        }


    }
}