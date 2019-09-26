CREATE TABLE [dbo].[ScheduleArrangementOffDayReplacement]
(
	[EmployeeId] INT NOT NULL,
	[WorkDate] DATE NOT NULL,
	[ReplaceForDate] DATE NULL,
	CONSTRAINT [PK_ScheduleArrangementOffDayReplacement] PRIMARY KEY CLUSTERED
	(
		[EmployeeId] ASC,
		[WorkDate] ASC
	)
)
