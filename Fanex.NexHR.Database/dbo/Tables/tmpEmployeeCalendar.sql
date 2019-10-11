CREATE TABLE [dbo].[tmpEmployeeCalendar]
(
	[EmpId] INT NOT NULL PRIMARY KEY, 
    [WorkDate] DATE NOT NULL PRIMARY KEY, 
    [RequestId] BIGINT NULL, 
    [RequestTypeId] SMALLDATETIME NULL, 
    [ShiftId] INT NULL, 
    [IsOffDay] BIT NOT NULL DEFAULT 0, 
    [IsHoliday] BIT NOT NULL DEFAULT 0, 
    [IsOvertime] BIT NOT NULL DEFAULT 0, 
    [IsKeepInView] BIT NOT NULL DEFAULT 0, 
    [IsLeaveDay] BIT NOT NULL DEFAULT 0, 
    [IsLeaveOffDay] BIT NOT NULL DEFAULT 0, 
    [LeaveDay] DECIMAL(2, 1) NOT NULL DEFAULT 0, 
    [LeaveTypeId] INT NULL, 
    [FromTime] DATETIME NULL, 
    [ToTime] DATETIME NULL, 
    [LeaveTypeId2] INT NULL, 
    [FromTime2] DATETIME NULL, 
    [ToTime2] DATETIME NULL, 
    [ChangeShiftTypeId] TINYINT NULL, 
    [MoveOffFromDate] DATE NULL, 
    [MoveOffToDate] DATE NULL
)
