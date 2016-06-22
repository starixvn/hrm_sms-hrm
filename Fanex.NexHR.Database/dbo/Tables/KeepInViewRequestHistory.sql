CREATE TABLE [dbo].[KeepInViewRequestHistory] (
    [RequestId]      BIGINT          NOT NULL,
    [EmpId]          INT             NULL,
    [OnDate]         DATETIME        NULL,
    [IsHalfDay]      BIT             NULL,
    [ShiftId]        INT             NULL,
    [FromTime]       DATETIME        NULL,
    [ToTime]         DATETIME        NULL,
    [ExpExchangeDay] DECIMAL (18, 2) NULL,
    [RefNo]          NVARCHAR (200)  NULL,
    [InsertedDate]   DATETIME        NULL,
    [ToDate]         SMALLDATETIME   NULL,
    [FromDate]       SMALLDATETIME   NULL
) ON [EmpHistory];

