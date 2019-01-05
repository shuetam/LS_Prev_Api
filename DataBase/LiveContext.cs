using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using Live.Core;
using Live;
using Microsoft.Extensions.Configuration;

public class LiveContext : DbContext
{
    public DbSet<RadioSong> RadioSongs {get; set;}
    public DbSet<Song> Songs {get; set;}
    public DbSet<ArchiveSong> ArchiveSongs {get; set;}
    public DbSet<YouTube> YouTubes {get; set;}

     private readonly SqlConnectingSettings _sqlSettings;
    public LiveContext(DbContextOptions<LiveContext> options, SqlConnectingSettings SqlSettings) : base(options)
    {
        _sqlSettings = SqlSettings;
    } 


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        optionsBuilder.UseSqlServer(_sqlSettings.ConnectionString);
 
    }

    protected override void OnModelCreating(ModelBuilder modelBuider)
    {
        var YouTubeBuilder = modelBuider.Entity<YouTube>();
        YouTubeBuilder.HasKey(x => x.ID);

        

        var SongBuilder = modelBuider.Entity<Song>();
        SongBuilder.HasKey(x => x.ID);

    }

}