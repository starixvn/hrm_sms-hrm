CREATE TABLE [dbo].[UserSite] (
    [UserId]        INT      NOT NULL,
    [SiteId]        SMALLINT NOT NULL,
    [IsActive]      BIT      CONSTRAINT [DF_UserSite_IsActive] DEFAULT ((1)) NOT NULL,
    [LastLoginDate] DATETIME NULL,
    [CreatedBy]     INT      NULL,
    [CreatedDate]   DATETIME NULL,
    [ModifiedBy]    INT      NULL,
    [ModifiedDate]  DATETIME NULL,
    CONSTRAINT [PK_UserSite] PRIMARY KEY CLUSTERED ([UserId] ASC, [SiteId] ASC)
);

