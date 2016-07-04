CREATE TABLE [dbo].[RoleFunction] (
    [RoleId]       INT      NOT NULL,
    [FunctionId]   INT      NOT NULL,
    [IsRead]       BIT      NOT NULL,
    [IsHost]       BIT      NOT NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_RoleFunction] PRIMARY KEY CLUSTERED ([RoleId] ASC, [FunctionId] ASC)
);



