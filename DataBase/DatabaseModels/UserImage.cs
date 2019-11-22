using System;

public class UserImage : Live.Core.Entity
{
    public Guid UserId {get; protected set;}
    public Guid? FolderId {get; protected set;}
    public string UrlAddress {get; protected set;}
    public string Source {get; protected set;}
    public string LocLeft {get; protected set;}
    public string LocTop {get; protected set;}
    public string Title {get; protected set;}
    //public User User {get; protected set;}
    public DateTime CreatedAt {get; protected set;}
    public DateTime? AddedToFolder {get; protected set;}
    

        public UserImage()
        {
        }

        public UserImage(string userId, string source, string url, string title, string left, string top, string folderId) 
        {
            UserId = new Guid(userId);
            Title = title;
            UrlAddress = url;

                if(string.IsNullOrEmpty(folderId))
                {
                    FolderId = null;
                }
                else 
                {
                    FolderId =  new Guid(folderId);
                }

            Source = source;
            LocLeft = left;
            LocTop = top;
            CreatedAt = DateTime.Now;
        }

          public UserImage( string source, string title, string left, string top) 
        {
            Title = title;
            FolderId = null;
            Source = source;
            LocLeft = left;
            LocTop = top;
            CreatedAt = DateTime.Now;
        }

        public void SetFolder(Guid folderId)
        {
            if(this.FolderId == null)
            {
                this.FolderId = folderId;
                this.AddedToFolder = DateTime.Now;
            }
        }

        public void RemoveFromFolder()
        {
            if(this.FolderId != null)
            {
                this.FolderId = null;
                this.AddedToFolder = null;
            }
        }

        public void ChangeLocation(string left, string top)
        {
            this.LocLeft = left;
            this.LocTop = top;
        }

        
}