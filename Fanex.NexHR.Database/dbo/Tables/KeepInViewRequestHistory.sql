CREATE TABLE [dbo].[KeepInViewRequestHistory] (
    [RequestId]      BIGINT   NOT NULL,
    [EmpId]          INT      NULL,
    [OnDate]         DATETIME NULL,
    [KeepInViewType] TINYINT  NULL,
    [IsHalfDay]      BIT      NULL,
    [FromTime]       DATETIME NULL,
    [ToTime]         DATETIME NULL,
    [HistoryId]      BIGINT   NULL
) ON [EmpHistory];



