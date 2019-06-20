CREATE TABLE [dbo].[EmployeeRole] (
    [EmployeeId]   INT      NOT NULL,
    [RoleId]       INT      NOT NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_EmployeeRole] PRIMARY KEY CLUSTERED ([EmployeeId] ASC, [RoleId] ASC)
);



