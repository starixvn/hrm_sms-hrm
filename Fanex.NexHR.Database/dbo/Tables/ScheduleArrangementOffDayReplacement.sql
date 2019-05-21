CREATE TABLE [dbo].[ScheduleArrangementOffDayReplacement](
	[EmployeeId] [int] NOT NULL,
	[WorkDate] [date] NOT NULL,
	[ReplaceForDate] [date] NULL,
 CONSTRAINT [PK_ScheduleArrangementOffDayReplacement] PRIMARY KEY CLUSTERED ([EmployeeId] ASC, [WorkDate] ASC)
);
