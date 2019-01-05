using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using AutoMapper;
using Live.Core;

namespace Live.Mapper
{
    public static class AutoMapperConfig
    {

        public static IMapper Initialize()
        => new MapperConfiguration(config => 
        {
            config.CreateMap<RadioSong, RadioSongDto>()
                .ForMember(d => d.title, s => s.MapFrom(x => x.Name))
                .ForMember(d => d.videoId  , s => s.MapFrom(x => x.YouTubeId))
                .ForMember(d => d.count  , s => s.MapFrom(x => x.Count))
                .ForMember(d => d.top  , s => s.MapFrom(x => Regex.Replace(x.top_, @"\,+", "."))) 
                .ForMember(d => d.left  , s => s.MapFrom(x => Regex.Replace(x.left_, @"\,+", ".")));

            config.CreateMap<ArchiveSong, SongDto>()
                .ForMember(d => d.title, s => s.MapFrom(x => x.Name))
                .ForMember(d => d.videoId  , s => s.MapFrom(x => x.YouTube.VideoID))
                .ForMember(d => d.count  , opt => opt.MapFrom(src => "1"))
                .ForMember(d => d.top  , s => s.MapFrom(x => x.YouTube.top_))
                .ForMember(d => d.left  , s => s.MapFrom(x => x.YouTube.left_));
        }
        ).CreateMapper();

    }
}
