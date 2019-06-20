CREATE TABLE [dbo].[LogObjectDetail] (
    [LogId]        BIGINT         NOT NULL,
    [ModifiedDate] DATETIME       NOT NULL,
    [ActorId]      INT            NOT NULL,
    [Data]         NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_LogObjectDetail] PRIMARY KEY CLUSTERED ([LogId] ASC, [ModifiedDate] ASC)
);

