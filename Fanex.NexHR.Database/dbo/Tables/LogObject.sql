CREATE TABLE [dbo].[LogObject] (
    [LogId]      BIGINT        IDENTITY (1, 1) NOT NULL,
    [ObjectType] VARCHAR (100) NOT NULL,
    [ObjectId]   INT           NOT NULL,
    CONSTRAINT [PK_LogObject] PRIMARY KEY CLUSTERED ([LogId] ASC)
);

