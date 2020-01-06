CREATE TABLE [dbo].[InOutRequest] (
    [RequestId]    BIGINT         NOT NULL,
    [EmpId]        INT            NULL,
    [LateInMins]   INT            NULL,
    [EarlyOutMins] INT            NULL,
    [FromDate]     DATETIME       NULL,
    [ToDate]       DATETIME       NULL,
    [RefNo]        NVARCHAR (200) NULL,
    CONSTRAINT [PK_InOutRequest] PRIMARY KEY CLUSTERED ([RequestId] ASC) ON [EmpRequest]
) ON [EmpRequest];




GO

 


