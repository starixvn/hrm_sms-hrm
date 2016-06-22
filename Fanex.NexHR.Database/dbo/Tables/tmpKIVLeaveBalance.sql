CREATE TABLE [dbo].[tmpKIVLeaveBalance] (
    [EmpId]     INT             NOT NULL,
    [RequestID] BIGINT          NOT NULL,
    [TotalDays] DECIMAL (18, 2) NULL,
    [InYear]    INT             NULL,
    [Status]    INT             CONSTRAINT [DF_tmpKIVLeaveBalance_Status] DEFAULT ((0)) NULL
) ON [EmpRequest];

