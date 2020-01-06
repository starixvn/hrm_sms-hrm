CREATE TABLE [dbo].[LateInEarlyOutRequest] (
    [RequestId]    BIGINT         NOT NULL,
    [EmpId]        INT            NULL,
    [OnDate]       DATE           NULL,
    [LateInMins]   INT            NULL,
    [EarlyOutMins] INT            NULL,
    [ReasonId]     TINYINT        NULL,
    [RefNo]        NVARCHAR (200) NULL,
    CONSTRAINT [PK_LateInEarlyOutRequest] PRIMARY KEY CLUSTERED ([RequestId] ASC) ON [EmpRequest]
) ON [EmpRequest];



