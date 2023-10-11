CREATE TABLE [dbo].[UserOTP] (
    [UserId]       INT           NOT NULL,
    [SiteId]       SMALLINT      NOT NULL,
    [Enabled]      BIT           NOT NULL,
    [SecretKey]    VARCHAR (250) NULL,
    [IsRegistered] BIT           NULL,
    CONSTRAINT [PK_UserOTP] PRIMARY KEY CLUSTERED ([UserId] ASC, [SiteId] ASC)
);

