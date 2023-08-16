CREATE TABLE [dbo].[MailRecord] (
    [MailId]       BIGINT         IDENTITY (1, 1) NOT NULL,
    [MailTo]       NVARCHAR (50)  NULL,
    [Subject]      NVARCHAR (250) NULL,
    [Body]         NVARCHAR (MAX) NULL,
    [SentSuccess]  SMALLINT       NULL,
    [SentFail]     SMALLINT       NULL,
    [CreatedDate]  DATETIME       NULL,
    [LastSentTime] DATETIME       NULL,
    CONSTRAINT [PK_MailRecord] PRIMARY KEY CLUSTERED ([MailId] ASC)
);

