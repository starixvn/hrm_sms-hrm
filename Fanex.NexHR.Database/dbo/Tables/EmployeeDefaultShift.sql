CREATE TABLE [dbo].[EmployeeDefaultShift] (
    [EmpID]    INT NOT NULL,
    [MonShift] INT NULL,
    [TueShift] INT NULL,
    [WedShift] INT NULL,
    [ThuShift] INT NULL,
    [FriShift] INT NULL,
    [SatShift] INT NULL,
    [SunShift] INT NULL,
    CONSTRAINT [PK_NexHR_EmployeeShift] PRIMARY KEY CLUSTERED ([EmpID] ASC) ON [Employee]
);

