CREATE TABLE [dbo].[EmployeeSummary] (
    [EmpId]              INT             NOT NULL,
    [AnnualLeaveBalance] NUMERIC (18, 1) CONSTRAINT [DF_EmployeeSummary_AnnualLeaveBalance] DEFAULT ((0)) NULL,
    [KeepInViewBalance]  NUMERIC (18, 1) CONSTRAINT [DF_EmployeeSummary_KeepInViewBalance] DEFAULT ((0)) NULL,
    [CanOT]              BIT             NULL,
    [IsLocal]            BIT             CONSTRAINT [DF_EmployeeSummary_IsLocal] DEFAULT ((1)) NULL,
    [WorkingDays]        INT             NULL,
    [WorkingHours]       INT             NULL,
    [IsDefaultSchedule]  BIT             CONSTRAINT [DF_EmployeeSummary_IsDefaultSchedule] DEFAULT ((0)) NULL,
    [CompanyDate]        DATETIME        NULL,
    [CountryCode]        VARCHAR (5)     NULL,
    [IsTopManager]       BIT             CONSTRAINT [DF_EmployeeSummary_IsTopManager] DEFAULT ((0)) NULL,
    [OffDaysbyWeek]      SMALLINT        CONSTRAINT [DF_EmployeeSummary_OffDaybyWeek] DEFAULT ((0)) NULL,
    [Unit]               NVARCHAR (150)  NULL,
    [IsExpatUseHoliday]  BIT             CONSTRAINT [DF_EmployeeSummary_IsExpatUseHoliday] DEFAULT ((0)) NULL,
    [MilitaryFromDate]   DATETIME        NULL,
    [MilitaryToDate]     DATETIME        NULL,
    [Remark]             NVARCHAR (200)  NULL,
    CONSTRAINT [PK_EmployeeSummary] PRIMARY KEY CLUSTERED ([EmpId] ASC) ON [Employee]
) ON [Employee];



