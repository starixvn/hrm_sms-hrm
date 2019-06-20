CREATE TABLE [dbo].[WeekendLeaveTrans] (
    [EmpId]        INT             NOT NULL,
    [WorkDate]     DATE            NOT NULL,
    [LeaveTypeId]  INT             NULL,
    [LeaveDay]     DECIMAL (18, 1) NULL,
    [RequestId]    BIGINT          NULL,
    [IsDiscipline] BIT             NULL,
    [InsertedDate] DATETIME        NULL
);

