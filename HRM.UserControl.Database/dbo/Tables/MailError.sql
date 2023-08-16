CREATE TABLE [dbo].[MailError] (
    [ErrorId]    BIGINT         IDENTITY (1, 1) NOT NULL,
    [MailId]     BIGINT         NULL,
    [Message]    NVARCHAR (MAX) NULL,
    [StackTrace] NVARCHAR (MAX) NULL,
    [ErrorTime]  DATETIME       NULL,
    CONSTRAINT [PK_MailError] PRIMARY KEY CLUSTERED ([ErrorId] ASC)
);

