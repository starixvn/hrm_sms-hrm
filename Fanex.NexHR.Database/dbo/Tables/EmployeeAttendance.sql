CREATE TABLE [dbo].[EmployeeAttendance] (
    [AttendanceDate]  DATETIME        NOT NULL,
    [EmpId]           INT             NOT NULL,
    [ShiftId]         INT             NULL,
    [InFixed]         DATETIME        NULL,
    [OutFixed]        DATETIME        NULL,
    [ExpectedInTime]  DATETIME        NULL,
    [ExpectedOutTime] DATETIME        NULL,
    [LateIn]          INT             NULL,
    [EarlyOut]        INT             NULL,
    [LeaveTypeId]     INT             NULL,
    [Absence]         BIT             NULL,
    [Notes]           NVARCHAR (2000) NULL,
    [TotalWorkHours]  DECIMAL (18, 2) NULL,
    [IsRightShift]    BIT             NULL,
    [IsLocked]        BIT             NULL,
    [LeaveDay]        DECIMAL (18, 1) NULL,
    [InsertDate]      DATETIME        NULL,
    [LeaveTypeId2]    INT             NULL,
    CONSTRAINT [PK_EmployeeAttendance] PRIMARY KEY CLUSTERED ([AttendanceDate] ASC, [EmpId] ASC) ON [EmpAttendance]
);

