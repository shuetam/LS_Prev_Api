using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using Live.Core;
using Live;
using Microsoft.Extensions.Configuration;
using Live.DataBase.DatabaseModels;

public class LiveContext : DbContext
{
    public DbSet<User> Users {get; set;}
    public DbSet<UserYoutube> UserYoutubes {get; set;}
    public DbSet<UserImage> UserImages {get; set;}
    public DbSet<UserSpotify> UserSpotify {get; set;}
    public DbSet<Folder> Folders {get; set;}
    public DbSet<RadioSong> RadioSongs {get; set;}
    public DbSet<Song> Songs {get; set;}
    public DbSet<ArchiveSong> ArchiveSongs {get; set;}
    public DbSet<ArchiveMovie> ArchiveMovies {get; set;}
    public DbSet<TVMovie> TVMovies {get; set;}
    public DbSet<YouTube> YouTubes {get; set;}
    public DbSet<Bestseller> Bestsellers { get; set; }

    private readonly SqlConnectingSettings _sqlSettings;
    public LiveContext(DbContextOptions<LiveContext> options, SqlConnectingSettings SqlSettings) : base(options)
    {
        _sqlSettings = SqlSettings;
    } 

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_sqlSettings.ConnectionString);
    }
        
 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var YouTubeBuilder = modelBuilder.Entity<YouTube>();
        YouTubeBuilder.HasKey(x => x.ID);

        var SongBuilder = modelBuilder.Entity<Song>();
        SongBuilder.HasKey(x => x.ID);

        var UserBuilder = modelBuilder.Entity<User>();
        UserBuilder.HasKey(x => x.ID);


    }
}