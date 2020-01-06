CREATE TABLE [dbo].[DisciplineIgnore] (
    [EmpId]                      INT  NOT NULL,
    [WorkDate]                   DATE NOT NULL,
    [IsIgnoredMissingCheckInOut] BIT  CONSTRAINT [DF_DisciplineIgnore_IsIgnoredMissingCheckInOut] DEFAULT ((0)) NOT NULL,
    [IsIgnoredLateIn]            BIT  CONSTRAINT [DF_DisciplineIgnore_IsIgnoredLateIn] DEFAULT ((0)) NOT NULL,
    [IsIgnoredEarlyOut]          BIT  CONSTRAINT [DF_DisciplineIgnore_IsIgnoredEarlyOut] DEFAULT ((0)) NOT NULL,
    [IsIgnoredWorkingUnderTime]  BIT  CONSTRAINT [DF_DisciplineIgnore_IsIgnoredWorkingUnderTime] DEFAULT ((0)) NOT NULL,
    [IsIgnoredChangeShift]       BIT  CONSTRAINT [DF_DisciplineIgnore_IsIgnoredChangeShift] DEFAULT ((0)) NOT NULL,
    [IsIgnoredAbsent]            BIT  CONSTRAINT [DF_DisciplineIgnore_IsIgnoredAbsent] DEFAULT ((0)) NOT NULL,
    [IsIgnoredUnpaidLeave]       BIT  CONSTRAINT [DF_DisciplineIgnore_IsIgnoredUnpaidLeave] DEFAULT ((0)) NOT NULL,
    [IsIgnoredUnpaidLeaveAbsent] BIT  CONSTRAINT [DF_DisciplineIgnore_IsIgnoredUnpaidLeaveAbsent] DEFAULT ((0)) NOT NULL,
    [IsIgnoredUnplannedLeave]    BIT  CONSTRAINT [DF_DisciplineIgnore_IsIgnoredUnplannedLeave] DEFAULT ((0)) NOT NULL,
    [IsIgnoredWeekendLeave]      BIT  CONSTRAINT [DF_DisciplineIgnore_IsIgnoredWeekendLeave] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_DisciplineIgnore] PRIMARY KEY CLUSTERED ([EmpId] ASC, [WorkDate] ASC)
);



