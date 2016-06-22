CREATE TABLE [dbo].[EmployeeGroup] (
    [GroupId]     INT NOT NULL,
    [EmpID]       INT NOT NULL,
    [isMainGroup] BIT NULL,
    CONSTRAINT [PK_EmployeeGroup_1] PRIMARY KEY CLUSTERED ([GroupId] ASC, [EmpID] ASC) ON [Employee]
);

