CREATE TABLE [dbo].[UserMapping] (
    [UserId]       INT      NOT NULL,
    [SiteId]       SMALLINT NOT NULL,
    [RefId]        INT      NULL,
    [NewUserId]    INT      NULL,
    [CreatedBy]    INT      NULL,
    [CreatedDate]  DATETIME NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_UserMapping] PRIMARY KEY CLUSTERED ([UserId] ASC, [SiteId] ASC)
);

