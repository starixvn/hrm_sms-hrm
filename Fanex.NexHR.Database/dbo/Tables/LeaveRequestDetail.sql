CREATE TABLE [dbo].[LeaveRequestDetail] (
    [RequestId] BIGINT   NOT NULL,
    [EmpId]     INT      NOT NULL,
    [OnDate]    DATE     NOT NULL,
    [ShiftId]   INT      NULL,
    [LeaveType] INT      NULL,
    [IsHalfDay] BIT      NULL,
    [FromTime]  DATETIME NULL,
    [ToTime]    DATETIME NULL,
    [WorkType]  TINYINT  NULL
) ON [EmpRequest];






GO
CREATE CLUSTERED INDEX [IX_LeaveRequestDetail_IDX_Shift]
    ON [dbo].[LeaveRequestDetail]([OnDate] ASC, [EmpId] ASC, [RequestId] ASC, [WorkType] ASC)
    ON [EmpRequest];


GO



