

----------LIVESEARCh------------

use shuetam_coukkas

CREATE TABLE RadioSongs
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR (300) NOT NULL,
    YouTubeId NVARCHAR (100) NOT NULL,
    Count INT  NULL,
    Size NVARCHAR (200) NOT NULL,
    top_ NVARCHAR (200) NOT NULL,
    left_ NVARCHAR (200) NOT NULL,
    CountRmf INT  NULL,
    CountZet INT  NULL,
    CountEska INT  NULL,
    CountRmfMaxx INT  NULL,
    CountAntyRadio INT  NULL,
    CountRmfClassic INT  NULL,
    CountChilliZet INT  NULL,
    CountZlotePrzeboje INT  NULL,
    CountVox INT  NULL,
    CountPlus INT  NULL,
)




---------------NEW DATABASE------------------------

CREATE TABLE YouTubes
(
    ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    top_ NVARCHAR (50) NOT NULL,
    left_ NVARCHAR (50) NOT NULL,
    VideoID NVARCHAR (50) NOT NULL,
)



CREATE TABLE Songs
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR (300) NOT NULL,
    Station NVARCHAR (20) NULL,
    PlayAt DATETIME NULL,
    YouTubeID  INT  NULL,
)




CREATE TABLE ArchiveSongs
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR (300) NOT NULL,
    YouTubeID  INT  NULL,
)


ALTER TABLE Songs ADD CONSTRAINT 
FK_YouTube FOREIGN KEY (YouTubeID) 
REFERENCES YouTubes(ID)

ALTER TABLE ArchiveSongs ADD CONSTRAINT 
FK_YouTubeArchive FOREIGN KEY (YouTubeID) 
REFERENCES YouTubes(ID)

select * from Songs

select * from  ArchiveSongs  left join YouTubes on ArchiveSongs.YouTubeID=Youtubes.ID

select *  from  Songs  left join YouTubes on Songs.YouTubeID=Youtubes.ID

drop TABLE ActualSongs



-------------------------------------------------------------



SELECT * FROM RadioSongs

SELECT Sum(Count), YouTubeId, Name
from RadioSongs
GROUP By  Name



   
      
