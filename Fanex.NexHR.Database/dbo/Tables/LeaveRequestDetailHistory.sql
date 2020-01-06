CREATE TABLE [dbo].[LeaveRequestDetailHistory] (
    [RequestId] BIGINT   NOT NULL,
    [EmpId]     INT      NOT NULL,
    [OnDate]    DATE     NOT NULL,
    [ShiftId]   INT      NULL,
    [LeaveType] INT      NULL,
    [IsHalfDay] BIT      NULL,
    [FromTime]  DATETIME NULL,
    [ToTime]    DATETIME NULL,
    [WorkType]  TINYINT  NULL,
    [HistoryId] BIGINT   NULL
) ON [EmpHistory];





