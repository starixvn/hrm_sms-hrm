CREATE TABLE [dbo].[DisciplineOutOfControl] (
    [EmpId]                           INT  NOT NULL,
    [WorkDate]                        DATE NOT NULL,
    [IsOutOfControlMissingCheckInOut] BIT  NULL,
    [IsOutOfControlLateIn]            BIT  NULL,
    [IsOutOfControlEarlyOut]          BIT  NULL,
    [IsOutOfControlWorkingUnderTime]  BIT  NULL,
    [IsOutOfControlChangeShift]       BIT  NULL,
    [IsOutOfControlAbsent]            BIT  NULL,
    [IsOutOfControlUnpaidLeave]       BIT  NULL,
    [IsOutOfControlUnpaidLeaveAbsent] BIT  NULL,
    [IsOutOfControlUnplannedLeave]    BIT  NULL,
    [IsOutOfControlWeekendLeave]      BIT  NULL,
    CONSTRAINT [PK_DisciplineOutOfControl] PRIMARY KEY CLUSTERED ([EmpId] ASC, [WorkDate] ASC)
);

