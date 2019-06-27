CREATE TABLE [dbo].[CheckInOutRequestHistory] (
    [RequestId]   BIGINT         NOT NULL,
    [EmpId]       INT            NOT NULL,
    [OnDate]      DATETIME       NOT NULL,
    [RequestTime] DATETIME       NOT NULL,
    [ReasonId]    TINYINT        NULL,
    [RefNo]       NVARCHAR (200) NULL,
    [HistoryId]   BIGINT         NULL
) ON [EmpHistory];

