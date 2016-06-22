CREATE TABLE [dbo].[LeaveRequestDetailHistory] (
    [RequestId]    BIGINT   NOT NULL,
    [EmpId]        INT      NOT NULL,
    [OnDate]       DATETIME NOT NULL,
    [ShiftId]      INT      NULL,
    [LeaveType]    INT      NULL,
    [IsHalfDay]    BIT      NULL,
    [FromTime]     DATETIME NULL,
    [ToTime]       DATETIME NULL,
    [InsertedDate] DATETIME NULL,
    [ReworkCount]  INT      NULL,
    [WorkType]     TINYINT  NULL
) ON [EmpHistory];

