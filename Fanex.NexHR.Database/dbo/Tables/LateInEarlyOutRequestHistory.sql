CREATE TABLE [dbo].[LateInEarlyOutRequestHistory] (
    [RequestId]    BIGINT         NOT NULL,
    [EmpId]        INT            NULL,
    [OnDate]       DATE           NULL,
    [LateInMins]   INT            NULL,
    [EarlyOutMins] INT            NULL,
    [ReasonId]     TINYINT        NULL,
    [RefNo]        NVARCHAR (200) NULL,
    [HistoryId]    BIGINT         NULL
) ON [EmpHistory];



