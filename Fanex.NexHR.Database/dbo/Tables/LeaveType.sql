CREATE TABLE [dbo].[LeaveType] (
    [LeaveTypeId]          INT             IDENTITY (1, 1) NOT NULL,
    [Code]                 VARCHAR (10)    NOT NULL,
    [Name]                 NVARCHAR (100)  NOT NULL,
    [IsAnnualQuota]        SMALLINT        NULL,
    [IsDefault]            BIT             NOT NULL,
    [IsSalaryDeduction]    BIT             NULL,
    [IsALDeduction]        BIT             NULL,
    [IsHoliday]            BIT             NULL,
    [isShown]              BIT             NULL,
    [IsLeaveDuringWorking] BIT             NULL,
    [IsCalculateWoking]    BIT             NULL,
    [Color]                NVARCHAR (50)   NULL,
    [OrderNo]              INT             NULL,
    [Description]          NVARCHAR (2000) NULL,
    CONSTRAINT [PK_LeaveType] PRIMARY KEY CLUSTERED ([LeaveTypeId] ASC) ON [EmpRequest]
) ON [EmpRequest];



