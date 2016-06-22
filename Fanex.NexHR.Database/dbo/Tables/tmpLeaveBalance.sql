CREATE TABLE [dbo].[tmpLeaveBalance] (
    [EmpId]     INT             NOT NULL,
    [RequestID] BIGINT          NOT NULL,
    [TotalDays] DECIMAL (18, 2) NULL,
    [InYear]    INT             NULL,
    [Status]    INT             CONSTRAINT [DF_tmpLeaveBalance_Status] DEFAULT ((0)) NULL,
    [InMonth]   INT             NULL
) ON [EmpRequest];

