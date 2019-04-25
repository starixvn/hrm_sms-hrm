CREATE TABLE [dbo].[ScheduleArrangementDefaultShift] (
    [EmployeeId]   INT      NOT NULL,
    [FromDate]     DATE     NOT NULL,
    [ShiftId]      INT      NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_ScheduleArrangementDefaultShift] PRIMARY KEY CLUSTERED ([EmployeeId] ASC, [FromDate] ASC)
);

