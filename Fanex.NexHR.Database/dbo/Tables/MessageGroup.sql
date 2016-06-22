CREATE TABLE [dbo].[MessageGroup] (
    [MessageId] INT          NOT NULL,
    [GroupId]   INT          NOT NULL,
    [Email]     VARCHAR (50) NULL,
    CONSTRAINT [PK_MessageGroup] PRIMARY KEY CLUSTERED ([MessageId] ASC, [GroupId] ASC)
);

