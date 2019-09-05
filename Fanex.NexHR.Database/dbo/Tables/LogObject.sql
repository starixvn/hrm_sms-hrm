CREATE TABLE [dbo].[LogObject]
(
	[Id] BIGINT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [LogDate] DATETIME NOT NULL, 
    [UserId] INT NOT NULL, 
    [ObjectTypeId] TINYINT NOT NULL, 
    [ObjectId] INT NOT NULL, 
    [OldData] NVARCHAR(MAX) NULL, 
    [Data] NVARCHAR(MAX) NULL
)
