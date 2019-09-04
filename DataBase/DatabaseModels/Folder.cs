using System;
using System.Collections.Generic;

public class Folder : Live.Core.Entity
{
    public Guid? ParentId {get; protected set;}
    public Guid UserId {get; protected set;}
    public string LocLeft {get; protected set;}
    public string LocTop {get; protected set;}
    public string Title {get; protected set;}
    public DateTime CreatedAt {get; protected set;}
    public List<UserYoutube> UserYouTubes {get; protected set;}

protected Folder()
{
}
public Folder(Guid userId, string title)
{
    UserId = userId;
    Title = title;
    LocLeft = "30vw";
    LocTop = "10vh";
    CreatedAt = DateTime.Now;
}

}
