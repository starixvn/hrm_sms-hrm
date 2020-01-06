CREATE TABLE [dbo].[InoutMissRequestHistory] (
    [RequestId] BIGINT         NOT NULL,
    [EmpId]     INT            NOT NULL,
    [OnDate]    DATETIME       NOT NULL,
    [CheckTime] DATETIME       NOT NULL,
    [RefNo]     NVARCHAR (200) NULL,
    [ReasonId]  TINYINT        NULL,
    [HistoryId] BIGINT         NULL
) ON [EmpHistory];



