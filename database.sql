

---------------LIVESEARCH-------------

use shuetam_livesearch

------------------------LIVESEARCH-------------------------

-----------------------NEW DATABASE-----------------------


CREATE TABLE Users
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    UserSocialId NVARCHAR (100),
    UserName NVARCHAR (100) NOT NULL,
    UserEmail NVARCHAR (50) NOT NULL,
    CreatedAt DATETIME NULL,
    LastLogin DATETIME NULL,
    LoginsCount INT,
    IsActive BIT,
    AuthType NVARCHAR (10) NOT NULL
)

ALTER TABLE Users
ADD UserRole NVARCHAR (10) NULL;

SELECT * FROM Users

DROP TABLE Users

SELECT * FROM UserYoutubes

SELECT * FROM UserImages

delete from UserYoutubes
WHERE LocLeft like '%20%'

UPDATE Users
SET UserRole = 'USER'
WHERE AuthType like 'Google' 

delete from UserYoutubes
WHERE FolderId is NULL

ALTER TABLE UserImages
ADD ImgType NVARCHAR (20) NULL;

alter table UserYoutubes
add AddedToFolder DATETIME NULL

alter table UserImages
add UrlAddress NVARCHAR (MAX)  NULL

CREATE TABLE UserYoutubes
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    FolderId UNIQUEIDENTIFIER NULL,
    VideoId NVARCHAR (300) NOT NULL,
    LocLeft NVARCHAR (50) NOT NULL,
    LocTop NVARCHAR (50) NOT NULL,
    Title NVARCHAR (300) NOT NULL,
    CreatedAt DATETIME,
)


CREATE TABLE UserImages
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    FolderId UNIQUEIDENTIFIER NULL,
    Source NVARCHAR (MAX) NOT NULL,
    LocLeft NVARCHAR (50) NOT NULL,
    LocTop NVARCHAR (50) NOT NULL,
    Title NVARCHAR (300) NOT NULL,
    CreatedAt DATETIME,
    AddedToFolder DATETIME NULL,
    UrlAddress NVARCHAR (MAX) NOT  NULL
)

select * from UserImages

DROP TABLE UserImages

ALTER TABLE UserImages ADD CONSTRAINT 
FK_ImageUserID FOREIGN KEY (UserId) 
REFERENCES Users(ID)

ALTER TABLE UserImages ADD CONSTRAINT 
FK_ImageFolderID FOREIGN KEY (FolderId) 
REFERENCES Folders(ID)
-------------------------------------------------------------
    public Guid UserId {get; protected set;}
    public Guid? FolderId {get; protected set;}
    public string SpotifyId {get; protected set;}
    public string ImgSource {get; protected set;}
    public string LocLeft {get; protected set;}
    public string LocTop {get; protected set;}
    public string Title {get; protected set;}
    public DateTime CreatedAt {get; protected set;}
    public DateTime? AddedToFolder {get; protected set;}
-------------------------------------------------------------


CREATE TABLE UserSpotify
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    FolderId UNIQUEIDENTIFIER NULL,
    SpotifyId NVARCHAR (300)  NULL,
    ImgSource NVARCHAR (MAX)  NULL,
    LocLeft NVARCHAR (50) NOT NULL,
    LocTop NVARCHAR (50) NOT NULL,
    Title NVARCHAR (300) NOT NULL,
    CreatedAt DATETIME,
    AddedToFolder DATETIME NULL,
)

select * from UserSpotify

ALTER TABLE UserSpotify ADD CONSTRAINT 
FK_SpotifyUserID FOREIGN KEY (UserId) 
REFERENCES Users(ID)

ALTER TABLE UserSpotify ADD CONSTRAINT 
FK_SpotifyFolderID FOREIGN KEY (FolderId) 
REFERENCES Folders(ID)



drop table UserYoutubes

UPDATE Folders
SET LocLeft = '50vw'
WHERE ParentId is NULL;  

UPDATE UserYoutubes
SET LocLeft = '50vw', LocTop = '10vh'
WHERE LocLeft like '%110%';


ALTER TABLE UserYoutubes ADD CONSTRAINT 
FK_YoutubeUserID FOREIGN KEY (UserId) 
REFERENCES Users(ID)


DELETE FROM UserSpotify WHERE
SpotifyId like '%http%'

SELECT * FROM UserSpotify

DELETE FROM Folders WHERE
Title like '%www%'


CREATE TABLE Folders
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    UserId UNIQUEIDENTIFIER NOT NULL,
    ParentId UNIQUEIDENTIFIER NULL,
    LocLeft NVARCHAR (50) NOT NULL,
    LocTop NVARCHAR (50) NOT NULL,
    Title NVARCHAR (300) NOT NULL,
    CreatedAt DATETIME,
)

ALTER TABLE UserYoutubes ADD CONSTRAINT 
FK_YoutubeFolderID FOREIGN KEY (FolderId) 
REFERENCES Folders(ID)


ALTER TABLE Folders ADD CONSTRAINT 
FK_FoldersUserID FOREIGN KEY (UserId) 
REFERENCES Users(ID)



CREATE TABLE Bestsellers
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    Title NVARCHAR (200) NOT NULL,
    Author NVARCHAR (200) NOT NULL,
    ImageSrc NVARCHAR (300) NOT NULL,
    Store NVARCHAR (50) NOT NULL,
    Size  INT  NULL,
    GroupNo  INT  NULL,
)

select * from Bestsellers where Title like '%�%'

Po�o�na z Auschwitz

select * from YouTubes where Name like '%Race%'


CREATE TABLE YouTubes
(
    ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
    top_ NVARCHAR (50) NOT NULL,
    left_ NVARCHAR (50) NOT NULL,
    VideoID NVARCHAR (300) NOT NULL,
)


CREATE TABLE Songs
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR (300) NOT NULL,
    Station NVARCHAR (20) NULL,
    PlayAt DATETIME NULL,
    YouTubeID  INT  NULL,
)

CREATE TABLE TVMovies
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    Title NVARCHAR (300) NOT NULL,
    TrailerSearch NVARCHAR (300) NOT NULL,
    Station NVARCHAR (30) NULL,
    Rating NVARCHAR (20) NULL,
    PlayAt DATETIME NULL,
    YouTubeID  INT  NULL,
)

ALTER TABLE TVMovies ADD CONSTRAINT 
FK_Movie_YouTube FOREIGN KEY (YouTubeID) 
REFERENCES YouTubes(ID)


DELETE FROM TVMovies WHERE
Station like 'TVP ABC'

SELECT * FROM TVMovies where Title like '%Logan%'


CREATE TABLE ArchiveSongs
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR (300) NOT NULL,
    YouTubeID  INT  NULL,
)

CREATE TABLE ArchiveMovies
(
    ID UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR (300) NOT NULL,
    YouTubeID  INT  NULL,
)
select * from  ArchiveMovies

select * from TVMovies

ALTER TABLE ArchiveMovies ADD CONSTRAINT 
FK_YouTubeMovieArchive FOREIGN KEY (YouTubeID) 
REFERENCES YouTubes(ID)


ALTER TABLE Songs ADD CONSTRAINT 
FK_YouTube FOREIGN KEY (YouTubeID) 
REFERENCES YouTubes(ID)

ALTER TABLE ArchiveSongs ADD CONSTRAINT 
FK_YouTubeArchive FOREIGN KEY (YouTubeID) 
REFERENCES YouTubes(ID)

select * from Songs

select * from  ArchiveSongs  left join YouTubes on ArchiveSongs.YouTubeID=Youtubes.ID
where VideoID like '%Error%'

select * from  TVMovies  left join YouTubes on TVMovies.YouTubeID=Youtubes.ID
where VideoID like '%Error%'

select *  from  Songs  left join YouTubes on Songs.YouTubeID=Youtubes.ID

drop TABLE ActualSongs



-------------------------------------------------------------



SELECT * FROM RadioSongs

SELECT Sum(Count), YouTubeId, Name
from RadioSongs
GROUP By  Name

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



   
      
