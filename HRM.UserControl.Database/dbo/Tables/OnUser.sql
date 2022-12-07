CREATE TABLE [dbo].[OnUser] (
    [Id]           BIGINT       IDENTITY (1, 1) NOT NULL,
    [UserId]       INT          NOT NULL,
    [SiteId]       SMALLINT     NOT NULL,
    [ReloadTime]   DATETIME     NULL,
    [IsKickedOut]  BIT          NULL,
    [IP]           VARCHAR (50) NULL,
    [NetSessionId] VARCHAR (50) NULL,
    CONSTRAINT [PK_OnUser] PRIMARY KEY CLUSTERED ([Id] ASC)
);

