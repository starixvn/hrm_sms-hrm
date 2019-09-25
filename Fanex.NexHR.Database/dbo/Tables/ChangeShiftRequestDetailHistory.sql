CREATE TABLE [dbo].[ChangeShiftRequestDetailHistory]
(
	[RequestId] [bigint] NOT NULL,
	[EmpId] [int] NOT NULL,
	[OnDate] [date] NOT NULL,
	[FromShift] [int] NULL,
	[ToShift] [int] NULL,
	[Remark] [nvarchar](2000) NULL,
	[ToDate] [date] NULL,
	[HistoryId] [bigint] NOT NULL
) ON [EmpHistory]
