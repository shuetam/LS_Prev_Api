using System;
using System.Collections.Generic;
using System.Linq;
using Live.Core;

public class Folder : Live.Core.Entity
{
    public Guid? ParentId {get; protected set;}
    public Guid UserId {get; protected set;}
    public string LocLeft {get; protected set;}
    public string LocTop {get; protected set;}
    public string Title {get; protected set;}
    public DateTime CreatedAt {get; protected set;}
    public List<UserYoutube> UserYouTubes {get; protected set;}
    public List<UserImage> UserImages {get; protected set;}

        public string icon0 {get => four[0];}
        public string icon1 {get => four[1];}
        public string icon2 {get => four[2];}
        public string icon3 {get => four[3];}

    private List<string> GetLastFour() 
    {

        if(UserYouTubes.Count()>5)
        {
            return this.UserYouTubes.OrderBy(x=> x.AddedToFolder).Select(x => x.VideoId).ToList()
            .Skip(Math.Max(0, UserYouTubes.Count() - 4)).ToList();
        }
        else
        {
            return this.UserYouTubes.OrderBy(x=> x.AddedToFolder).Select(x => x.VideoId).ToList();
        }
    }

    string [] four = {"","","",""};


        private string getImgSrc(string videoId)
        {
            return "https://i.ytimg.com/vi/" + videoId+ "/hqdefault.jpg";
        }


    public void SetFourIcons()
    {
        var listYT = this.UserYouTubes.OrderByDescending(x=> x.AddedToFolder)
        .Select(x => new FolderDto(x.AddedToFolder, getImgSrc(x.VideoId)))
        .ToList().Take(4);

        
        var listImg = this.UserImages.OrderByDescending(x=> x.AddedToFolder)
        .Select(x => new FolderDto(x.AddedToFolder, x.UrlAddress)).ToList()
        .Take(4);
        
        var list = new List<FolderDto>();
        list.AddRange(listYT);
        list.AddRange(listImg);
        
       list = list.OrderByDescending(x => x.DateCreated).ToList();

        //var listofId = this.UserYouTubes.Select(x => x.VideoId).ToList();
        //listofId.Reverse();
        //var listImg = this.UserImages.ToList();



        for(int i =0 ;i<4;i++)
        {
            try
            {
                four[i] = list[i].id;
               
            }
            catch(Exception e)
            {

            }

        }

    }

        protected Folder()
        {
        }
        public Folder(Guid userId, string title)
        {
            UserId = userId;
            Title = title;
            LocLeft = "90vw";
            LocTop = "80vh";
            CreatedAt = DateTime.Now;
            UserYouTubes = new List<UserYoutube>();
            UserImages = new List<UserImage>();
            
        }

        public void ChangeLocation(string left, string top)
        {
            this.LocLeft = left;
            this.LocTop = top;
        }

}
