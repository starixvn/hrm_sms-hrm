CREATE TABLE [dbo].[Log_UserLogin] (
    [Id]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [UserId]   INT            NOT NULL,
    [SiteId]   SMALLINT       NOT NULL,
    [LogDate]  DATETIME       NOT NULL,
    [IP]       VARCHAR (50)   NULL,
    [Browser]  NVARCHAR (250) NULL,
    [Platform] NVARCHAR (250) NULL,
    [Domain]   NVARCHAR (250) NULL,
    CONSTRAINT [PK_Log_UserLogin] PRIMARY KEY CLUSTERED ([Id] ASC)
);

