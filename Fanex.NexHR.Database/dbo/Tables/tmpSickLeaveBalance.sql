CREATE TABLE [dbo].[tmpSickLeaveBalance] (
    [EmpId]     INT             NOT NULL,
    [RequestId] BIGINT          NOT NULL,
    [TotalDays] DECIMAL (18, 2) NULL,
    [Month]     TINYINT         NULL,
    [Year]      INT             NULL,
    [Status]    TINYINT         CONSTRAINT [DF_tmpSickLeaveBalance_Status] DEFAULT ((0)) NULL
) ON [EmpRequest];

