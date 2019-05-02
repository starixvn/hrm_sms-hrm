
CREATE PROCEDURE [dbo].[EmployeeCalendar_Update_byDefaultShift] @EmpId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-23
Task: Update Shift of Employee Calendar
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @fromdate DATE = GETDATE(), @ModifiedDate DATETIME = GETDATE()

	UPDATE ec
	SET ec.ShiftId = CASE 
			WHEN datepart(dw, ec.WorkDate) = 1 THEN SunShift
			WHEN datepart(dw, ec.WorkDate) = 2 THEN MonShift
			WHEN datepart(dw, ec.WorkDate) = 3 THEN TueShift
			WHEN datepart(dw, ec.WorkDate) = 4 THEN WedShift
			WHEN datepart(dw, ec.WorkDate) = 5 THEN ThuShift
			WHEN datepart(dw, ec.WorkDate) = 6 THEN FriShift
			WHEN datepart(dw, ec.WorkDate) = 7 THEN SatShift
			END
		,ec.IsOffDay = CASE 
			WHEN es.IsLocal = 1
				AND hld.Id IS NOT NULL THEN 0
			WHEN datepart(dw, ec.WorkDate) = 1
				AND SunShift IS NULL THEN 1
			WHEN datepart(dw, ec.WorkDate) = 2
				AND MonShift IS NULL THEN 1
			WHEN datepart(dw, ec.WorkDate) = 3
				AND TueShift IS NULL THEN 1
			WHEN datepart(dw, ec.WorkDate) = 4
				AND WedShift IS NULL THEN 1
			WHEN datepart(dw, ec.WorkDate) = 5
				AND ThuShift IS NULL THEN 1
			WHEN datepart(dw, ec.WorkDate) = 6
				AND FriShift IS NULL THEN 1
			WHEN datepart(dw, ec.WorkDate) = 7
				AND SatShift IS NULL THEN 1
			ELSE 0
			END
		,ec.IsHoliday = CASE 
			WHEN es.IsLocal = 1
				AND hld.Id IS NOT NULL THEN 1
			ELSE 0
			END
		,ec.ModifiedDate = @ModifiedDate
	FROM dbo.EmployeeCalendar AS ec WITH (ROWLOCK, UPDLOCK)
		INNER JOIN EmployeeSummary AS es WITH (NOLOCK) ON ec.EmpId = es.EmpId
		INNER JOIN Employee AS ee WITH (NOLOCK) ON ee.EmpId = es.EmpId
		LEFT JOIN dbo.EmployeeDefaultShift AS s WITH (NOLOCK) ON ec.EmpId = s.EmpID
		LEFT JOIN dbo.Holiday AS hld WITH (NOLOCK) ON ec.WorkDate BETWEEN hld.FromDate
			AND hld.ToDate
	WHERE es.IsDefaultSchedule = 1
		AND ISNULL(ee.IsDeleted, 0) = 0
		AND ec.EmpId = @EmpId
		AND ec.WorkDate >= @fromdate

	EXECUTE dbo.DBA_Job_EmployeeCalendar_Insert_weekly @EmpId = @EmpId

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Create Default Calendar for employee failed'
	END
END
