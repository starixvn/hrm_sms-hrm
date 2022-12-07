CREATE TABLE [dbo].[UserRole] (
    [UserId]      INT      NOT NULL,
    [RoleId]      SMALLINT NOT NULL,
    [SiteId]      SMALLINT NOT NULL,
    [CreatedBy]   INT      NULL,
    [CreatedDate] DATETIME NULL,
    CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC, [SiteId] ASC)
);

