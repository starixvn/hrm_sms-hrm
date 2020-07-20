CREATE TABLE [dbo].[GroupManager] (
    [GroupId] INT     NOT NULL,
    [EmpId]   INT     NOT NULL,
    [RoleId]  TINYINT NOT NULL,
    CONSTRAINT [PK_GroupManager] PRIMARY KEY CLUSTERED ([GroupId] ASC, [EmpId] ASC, [RoleId] ASC)
);


