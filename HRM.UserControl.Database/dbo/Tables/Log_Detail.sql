CREATE TABLE [dbo].[Log_Detail] (
    [LogId]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [SiteId]   SMALLINT       NULL,
    [UserId]   INT            NULL,
    [SpName]   VARCHAR (100)  NULL,
    [ObjectId] INT            NULL,
    [LogDate]  DATETIME       NULL,
    [Duration] INT            NULL,
    [Data]     NVARCHAR (MAX) NULL,
    [OldData]  NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Log_Detail] PRIMARY KEY CLUSTERED ([LogId] ASC)
);

