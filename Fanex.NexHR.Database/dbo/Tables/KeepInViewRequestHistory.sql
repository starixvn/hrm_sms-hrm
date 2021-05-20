CREATE TABLE [dbo].[KeepInViewRequestHistory] (
    [RequestId]      BIGINT   NOT NULL,
    [EmpId]          INT      NULL,
    [OnDate]         DATE     NULL,
    [IsHalfDay]      BIT      NULL,
    [FromTime]       DATETIME NULL,
    [ToTime]         DATETIME NULL,
    [HistoryId]      BIGINT   NULL,
    [KeepInViewType] TINYINT  NULL
) ON [EmpHistory];







