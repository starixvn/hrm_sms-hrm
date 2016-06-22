CREATE TABLE [dbo].[tmpEmployeeAttendace] (
    [EmpId]        INT      NOT NULL,
    [WorkDate]     DATE     NOT NULL,
    [InsertedDate] DATETIME NULL,
    CONSTRAINT [PK_tmpEmployeeAttendace] PRIMARY KEY CLUSTERED ([EmpId] ASC, [WorkDate] ASC)
);

