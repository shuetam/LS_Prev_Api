using System;

public class UserYoutube : Live.Core.Entity
{
    public Guid UserId {get; protected set;}
    public Guid? FolderId {get; protected set;}
    public string VideoId {get; protected set;}
    public string LocLeft {get; protected set;}
    public string LocTop {get; protected set;}
    public string Title {get; protected set;}
    public DateTime CreatedAt {get; protected set;}

        protected UserYoutube()
        {

        }
        public UserYoutube(string userId, string videoId, string title, string left, string top) 
        {
            UserId = new Guid(userId);
            Title = title;
            FolderId = null;
            VideoId = videoId;
            LocLeft = left;
            LocTop = top;
            CreatedAt = DateTime.Now;
        }
}