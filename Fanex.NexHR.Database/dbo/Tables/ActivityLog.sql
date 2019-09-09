CREATE TABLE [dbo].[ActivityLog]
(
	[Id] BIGINT IDENTITY (1, 1) NOT NULL PRIMARY KEY, 
    [LogDate] DATETIME NOT NULL, 
    [UserId] INT NOT NULL, 
    [ActionId] SMALLINT NULL, 
    [ObjectTypeId] TINYINT NULL, 
    [ObjectId] INT NULL, 
    [OldData] NVARCHAR(MAX) NULL, 
    [Data] NVARCHAR(MAX) NULL
)
