CREATE TABLE [dbo].[LeaveRequestHistory] (
    [RequestId]    BIGINT          NOT NULL,
    [EmpId]        INT             NOT NULL,
    [FromDate]     DATETIME        NOT NULL,
    [ToDate]       DATETIME        NOT NULL,
    [LeaveType]    INT             NULL,
    [TotalDays]    DECIMAL (18, 2) NULL,
    [RefNo]        NVARCHAR (200)  NULL,
    [ReworkCount]  INT             NULL,
    [InsertedDate] DATETIME        NOT NULL
) ON [EmpHistory];

