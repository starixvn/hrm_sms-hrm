CREATE TABLE [dbo].[tmpHospitalizationLeaveBalance] (
    [EmpId]     INT             NOT NULL,
    [RequestId] BIGINT          NOT NULL,
    [TotalDays] DECIMAL (18, 2) NULL,
    [Month]     TINYINT         NULL,
    [Year]      INT             NULL,
    [Status]    TINYINT         CONSTRAINT [DF_tmpHospitalizationLeaveBalance_Status] DEFAULT ((0)) NULL
) ON [EmpRequest];

