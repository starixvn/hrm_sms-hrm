CREATE TABLE [dbo].[DisciplineIgnore] (
    [EmpId]                      INT  NOT NULL,
    [WorkDate]                   DATE NOT NULL,
    [IsIgnoredMissingCheckInOut] BIT  NULL,
    [IsIgnoredLateIn]            BIT  NULL,
    [IsIgnoredEarlyOut]          BIT  NULL,
    [IsIgnoredWorkingUnderTime]  BIT  NULL,
    [IsIgnoredChangeShift]       BIT  NULL,
    [IsIgnoredAbsent]            BIT  NULL,
    [IsIgnoredUnpaidLeave]       BIT  NULL,
    [IsIgnoredUnpaidLeaveAbsent] BIT  NULL,
    [IsIgnoredUnplannedLeave]    BIT  NULL,
    [IsIgnoredWeekendLeave]      BIT  NULL,
    CONSTRAINT [PK_DisciplineIgnore] PRIMARY KEY CLUSTERED ([EmpId] ASC, [WorkDate] ASC)
);

