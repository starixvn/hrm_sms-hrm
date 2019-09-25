CREATE TABLE [dbo].[ChangeShiftRequestHistory]
(
	[RequestId] [bigint] NOT NULL,
	[EmpId] [int] NOT NULL,
	[FromDate] [date] NOT NULL,
	[ToDate] [date] NOT NULL,
	[TypeId] [tinyint] NOT NULL,
	[HistoryId] [bigint] NOT NULL
) ON [EmpHistory]
