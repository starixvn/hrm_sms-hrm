CREATE TABLE [dbo].[EmployeeScheduleShiftMultiple] (
    [EmpId]   INT      NULL,
    [ForDate] DATETIME NULL,
    [ShiftId] INT      NULL
);


GO
CREATE CLUSTERED INDEX [IX_EmployeeScheduleShiftMultiple]
    ON [dbo].[EmployeeScheduleShiftMultiple]([ForDate] ASC, [EmpId] ASC);

