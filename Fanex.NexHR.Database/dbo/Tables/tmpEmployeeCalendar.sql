CREATE TABLE [dbo].[tmpEmployeeCalendar] (
    [EmpId]             INT            NOT NULL,
    [WorkDate]          DATE           NOT NULL,
    [RequestId]         BIGINT         NULL,
    [RequestTypeId]     SMALLINT       NULL,
    [ShiftId]           INT            NULL,
    [IsOffDay]          BIT            CONSTRAINT [DF_tmpEmployeeCalendar_IsOffDay] DEFAULT ((0)) NOT NULL,
    [IsHoliday]         BIT            CONSTRAINT [DF_tmpEmployeeCalendar_IsHoliday] DEFAULT ((0)) NOT NULL,
    [IsOvertime]        BIT            CONSTRAINT [DF_tmpEmployeeCalendar_IsOvertime] DEFAULT ((0)) NOT NULL,
    [IsKeepInView]      BIT            CONSTRAINT [DF_tmpEmployeeCalendar_IsKeepInView] DEFAULT ((0)) NOT NULL,
    [IsLeaveDay]        BIT            CONSTRAINT [DF_tmpEmployeeCalendar_IsLeaveDay] DEFAULT ((0)) NOT NULL,
    [IsLeaveOffDay]     BIT            CONSTRAINT [DF_tmpEmployeeCalendar_IsLeaveOffday] DEFAULT ((0)) NOT NULL,
    [LeaveDay]          DECIMAL (2, 1) CONSTRAINT [DF_tmpEmployeeCalendar_LeaveDay] DEFAULT ((0)) NOT NULL,
    [LeaveTypeId]       INT            NULL,
    [FromTime]          DATETIME       NULL,
    [ToTime]            DATETIME       NULL,
    [LeaveTypeId2]      INT            NULL,
    [FromTime2]         DATETIME       NULL,
    [ToTime2]           DATETIME       NULL,
    [ChangeShiftTypeId] TINYINT        NULL,
    [MoveOffFromDate]   DATE           NULL,
    [MoveOffToDate]     DATE           NULL,
    CONSTRAINT [PK_tmpEmployeeCalendar] PRIMARY KEY CLUSTERED ([EmpId] ASC, [WorkDate] ASC)
);


