CREATE TABLE [dbo].[DisciplineOutOfControl] (
    [EmpId]                           INT  NOT NULL,
    [WorkDate]                        DATE NOT NULL,
    [IsOutOfControlMissingCheckInOut] BIT  CONSTRAINT [DF_DisciplineOutOfControl_IsOutOfControlMissingCheckInOut] DEFAULT ((0)) NOT NULL,
    [IsOutOfControlLateIn]            BIT  CONSTRAINT [DF_DisciplineOutOfControl_IsOutOfControlLateIn] DEFAULT ((0)) NOT NULL,
    [IsOutOfControlEarlyOut]          BIT  CONSTRAINT [DF_DisciplineOutOfControl_IsOutOfControlEarlyOut] DEFAULT ((0)) NOT NULL,
    [IsOutOfControlWorkingUnderTime]  BIT  CONSTRAINT [DF_DisciplineOutOfControl_IsOutOfControlWorkingUnderTime] DEFAULT ((0)) NOT NULL,
    [IsOutOfControlChangeShift]       BIT  CONSTRAINT [DF_DisciplineOutOfControl_IsOutOfControlChangeShift] DEFAULT ((0)) NOT NULL,
    [IsOutOfControlAbsent]            BIT  CONSTRAINT [DF_DisciplineOutOfControl_IsOutOfControlAbsent] DEFAULT ((0)) NOT NULL,
    [IsOutOfControlUnpaidLeave]       BIT  CONSTRAINT [DF_DisciplineOutOfControl_IsOutOfControlUnpaidLeave] DEFAULT ((0)) NOT NULL,
    [IsOutOfControlUnpaidLeaveAbsent] BIT  CONSTRAINT [DF_DisciplineOutOfControl_IsOutOfControlUnpaidLeaveAbsent] DEFAULT ((0)) NOT NULL,
    [IsOutOfControlUnplannedLeave]    BIT  CONSTRAINT [DF_DisciplineOutOfControl_IsOutOfControlUnplannedLeave] DEFAULT ((0)) NOT NULL,
    [IsOutOfControlWeekendLeave]      BIT  CONSTRAINT [DF_DisciplineOutOfControl_IsOutOfControlWeekendLeave] DEFAULT ((0)) NOT NULL,
    [IsOutOfControlSevenDaysLeave]    BIT  CONSTRAINT [DF_DisciplineOutOfControl_IsOutOfControlSevenDaysLeave] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_DisciplineOutOfControl] PRIMARY KEY CLUSTERED ([EmpId] ASC, [WorkDate] ASC)
);





