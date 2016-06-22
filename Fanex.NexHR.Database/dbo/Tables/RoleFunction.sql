CREATE TABLE [dbo].[RoleFunction] (
    [RoleId]       INT      NOT NULL,
    [FunctionId]   INT      NOT NULL,
    [IsRead]       BIT      NOT NULL,
    [IsEdit]       BIT      NOT NULL,
    [IsCreate]     BIT      NOT NULL,
    [IsDelete]     BIT      NOT NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_RoleFunction] PRIMARY KEY CLUSTERED ([RoleId] ASC, [FunctionId] ASC)
);

