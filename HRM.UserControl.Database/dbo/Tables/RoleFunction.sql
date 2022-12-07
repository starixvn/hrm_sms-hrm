CREATE TABLE [dbo].[RoleFunction] (
    [SiteId]       SMALLINT NOT NULL,
    [RoleId]       SMALLINT NOT NULL,
    [FunctionId]   INT      NOT NULL,
    [IsRead]       BIT      CONSTRAINT [DF_RoleFunction_IsRead] DEFAULT ((0)) NULL,
    [IsHost]       BIT      NULL,
    [CreatedBy]    INT      NULL,
    [CreatedDate]  DATETIME NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_RoleFunction] PRIMARY KEY CLUSTERED ([SiteId] ASC, [RoleId] ASC, [FunctionId] ASC)
);

