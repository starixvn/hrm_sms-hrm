CREATE TABLE [dbo].[InOutRequestHistory] (
    [RequestId]    BIGINT         NOT NULL,
    [EmpId]        INT            NULL,
    [LateInMins]   INT            NULL,
    [EarlyOutMins] INT            NULL,
    [FromDate]     DATETIME       NULL,
    [ToDate]       DATETIME       NULL,
    [RefNo]        NVARCHAR (200) NULL,
    [InsertedDate] DATETIME       NULL
) ON [EmpHistory];

