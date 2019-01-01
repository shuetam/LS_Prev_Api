




CREATE DATABASE CoukkasDatabase

----------DROP DATABASE CoukkasDatabase

use master

USE CoukkasDatabase



CREATE TABLE Users
(
ID UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
Name NVARCHAR(20) NOT NULL,
Email NVARCHAR(20) NOT NULL,
Password NVARCHAR(20) NOT NULL,
Role NVARCHAR(20) NOT NULL,
LocationID  INT  NULL,
CreatedAt DATE NOT NULL,
);




/* 
sudo docker run -e 'ACCEPT_EULA=Y' -e 'MSSQL_SA_PASSWORD=sqlpassword' \
   -p 1433:1433 --name sql2 \
   -d microsoft/mssql-server-linux:2017-latest


765d456bc8a863c2be119daf6d183720f1b65f9b7e66e841fc37bd4e16d0735d */

CREATE TABLE Regions
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
Latitude FLOAT  NULL,
Longitude FLOAT  NULL,
)

use shuetam_coukkas

SELECT * FROM Regions

DELETE from Regions
WHERE ID<100

INSERT Into Regions (Latitude, Longitude)
VALUES
(70,20),
(52,34),
(5,34),
(59,50),
(50.061509, 19.944035)





CREATE TABLE Locations
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
Latitude FLOAT  NULL,
Longitude FLOAT  NULL,
)

ALTER TABLE Users ADD CONSTRAINT 
FK_LocationID FOREIGN KEY (LocationID) 
REFERENCES Locations(ID)


use CoukkasDatabase

CREATE TABLE FactTryCatchCoupons
(
ID INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
Category NVARCHAR (20) NOT NULL,
LocationID  INT  NULL,
TryDate DATE NOT NULL,
)

ALTER TABLE FactTryCatchCoupons ADD CONSTRAINT 
FKFs_LocationID FOREIGN KEY (LocationID) 
REFERENCES Locations(ID)

drop TABLE FactTryCatchCoupon



SELECT * from FactTryCatchCoupons // taka sama nazwa musi byc koniecznie!!

CREATE TABLE Fences
(
ID UNIQUEIDENTIFIER PRIMARY KEY,
Name NVARCHAR (20) NOT NULL,
Category NVARCHAR (20) NOT NULL,
Description  NVARCHAR(max),
OwnerID UNIQUEIDENTIFIER NOT NULL,
LocationID  INT  NULL,
Radius FLOAT NOT NULL,
CreatedAt DATE NOT NULL,
EndDate DATE NULL,
)

ALTER TABLE Fences ADD CONSTRAINT 
FK_Location FOREIGN KEY (LocationID) 
REFERENCES Locations(ID)

ALTER TABLE Fences ADD CONSTRAINT 
FK_OwnerID FOREIGN KEY (OwnerID) 
REFERENCES Users(ID)

use CoukkasDatabase

drop TABLE Coupons

CREATE TABLE Coupons
(
ID UNIQUEIDENTIFIER PRIMARY KEY,
FenceID UNIQUEIDENTIFIER NOT NULL,
UserID UNIQUEIDENTIFIER NULL,
LocationID  INT  NULL,
)

ALTER TABLE Coupons ADD CONSTRAINT 
FK_LocationCou FOREIGN KEY (LocationID) 
REFERENCES Locations(ID)

ALTER TABLE Coupons ADD CONSTRAINT 
FK_User FOREIGN KEY (UserID) 
REFERENCES Users(ID)


ALTER TABLE Coupons ADD CONSTRAINT 
FK_Fence FOREIGN KEY (FenceID) 
REFERENCES Fences(ID)





----------------------------------------------------------------
USE CoukkasDatabase

SELECT * from FactTryCatchCoupons

SELECT * FROM Fences

SELECT * FROM Users

select * from locations

SELECT * FROM Coupons

select * from Fences join Users on Fences.OwnerID = Users.ID

DELETE  from Fences
where Name = 'Biedronka';
-----------------------------------------------------------------------

DELETE FROM FactTryCatchCoupons
WHERE ID > 0

DELETE FROM Fences;
DELETE from Coupons;
delete from Locations;

min: 19.768210
max: 20.116805

min: 49.980660
max: 50.120083


center: 50.061509 , 19.944035

radius: 12000



-----------------------------------------------------------------------
select Fences.ID, Radius, Latitude, Longitude from Fences Join Locations 
on Fences.LocationID = Locations.ID


select Coupons.ID, FenceID, Locations.ID as LocationID, Latitude, Longitude from Coupons
join Locations on Coupons.LocationID = Locations.ID WHERE COUPONS.UserID is NULL


-----------------------------------------------------------------------


update Locations
set Latitude=34, Longitude=43 where id>70

delete  from Coupons
where UserID is NULL




Docker:
# Backup
docker exec CONTAINER /usr/bin/mysqldump -u root --password=root DATABASE > backup.sql
# Restore
cat backup.sql | docker exec -i CONTAINER /usr/bin/mysql -u root --password=root DATABASE


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





SELECT * FROM RadioSongs

SELECT Sum(Count), YouTubeId, Name
from RadioSongs
GROUP By  Name





   
      
