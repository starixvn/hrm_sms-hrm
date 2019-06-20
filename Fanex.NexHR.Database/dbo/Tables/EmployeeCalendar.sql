CREATE TABLE [dbo].[EmployeeCalendar] (
    [WorkDate]              DATETIME        NOT NULL,
    [EmpId]                 INT             NOT NULL,
    [ShiftId]               INT             NULL,
    [IsHoliday]             BIT             CONSTRAINT [DF_EmployeeCalendar_IsHoliday] DEFAULT ((0)) NULL,
    [IsOffDay]              BIT             CONSTRAINT [DF_EmployeeCalendar_IsOffDay] DEFAULT ((0)) NULL,
    [IsLeaveDay]            BIT             CONSTRAINT [DF_EmployeeCalendar_IsLeaveDay] DEFAULT ((0)) NULL,
    [LeaveTypeId]           INT             NULL,
    [LateIn]                INT             NULL,
    [EarlyOut]              INT             NULL,
    [RequestedCheckinTime]  DATETIME        NULL,
    [RequestedCheckoutTime] DATETIME        NULL,
    [ChangeShiftId]         BIGINT          NULL,
    [LeaveDay]              DECIMAL (18, 1) CONSTRAINT [DF_EmployeeCalendar_LeaveDay] DEFAULT ((0)) NULL,
    [ModifiedDate]          DATETIME        CONSTRAINT [DF_EmployeeCalendar_ModifiedDate] DEFAULT (getdate()) NULL,
    [ModifiedBy]            INT             NULL,
    [IsKeepInView]          BIT             CONSTRAINT [DF_EmployeeCalendar_IsKeepInView] DEFAULT ((0)) NULL,
    [LeaveTypeId2]          INT             NULL,
    [IsPublic]              BIT             CONSTRAINT [DF_EmployeeCalendar_IsPublicDay] DEFAULT ((1)) NULL,
    CONSTRAINT [PK_EmployeeCalendar] PRIMARY KEY CLUSTERED ([WorkDate] ASC, [EmpId] ASC) ON [EmpAttendance]
) ON [EmpAttendance];



