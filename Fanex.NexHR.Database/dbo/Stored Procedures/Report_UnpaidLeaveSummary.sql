
CREATE PROCEDURE [dbo].[Report_UnpaidLeaveSummary] @Year INT
	,@EmpId INT = NULL
	,@TeamIds VARCHAR(MAX) = ''
	,@IsLocal BIT = NULL
	,@SupervisorId INT = NULL
	,@LeaveType SMALLINT = 1
AS
/*
Created: 20151028@Quinn.Vo
Task: Get unpaid leave report.
DB: SMS-HRM
Original:

Revisions: by Oliver.Dang on 20190531
Param's Explanation:
LeaveType:
- 1: UPL
- 2: UPL Insurance
- 3: Paternity
- 4: Maternity

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @UnpaidLeaveType INT
		,@fromDate DATE = CAST(@year AS VARCHAR(5)) + '-01-01'
		,@toDate DATE = CAST(@year AS VARCHAR(5)) + '-12-31'
	DECLARE @tblGroupIds TABLE (GroupId INT)
	DECLARE @tblEmployeeIds TABLE (EmpId INT)

	SELECT @UnpaidLeaveType = LeaveTypeId
	FROM dbo.LeaveType WITH (NOLOCK)
	WHERE Code = CASE @LeaveType
			WHEN 1 THEN 'UPL'
			WHEN 2 THEN 'UPLI'
			WHEN 3 THEN 'PL'
			WHEN 4 THEN 'ML'
			ELSE 'UPL'
			END

	IF ISNULL(@TeamIds, '') <> ''
	BEGIN
		INSERT INTO @tblGroupIds (GroupId)
		SELECT DISTINCT val
		FROM fnc_strSplit(@TeamIds, ',')
	END
	ELSE
	BEGIN
		INSERT INTO @tblGroupIds (GroupId)
		SELECT DISTINCT GroupId
		FROM dbo.[Group] AS g WITH (NOLOCK)
		WHERE ISNULL(g.IsDeleted, 0) = 0

		IF NOT EXISTS (
				SELECT 1
				FROM dbo.Employee AS e WITH (NOLOCK)
					INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON e.EmpId = er.EmployeeId
				WHERE EmpId = @SupervisorId
					AND er.RoleId IN (2, 3, 4, 5, 6, 7)
				)
		BEGIN
			SET @EmpId = @SupervisorId
		END
	END

	IF @EmpId IS NULL
	BEGIN
		INSERT INTO @tblEmployeeIds (EmpId)
		SELECT DISTINCT EmpId
		FROM dbo.fnc_GetEmployeeByActorId(@SupervisorId, 1, 0)
	END
	ELSE
	BEGIN
		INSERT INTO @tblEmployeeIds (EmpId)
		VALUES (@EmpId)
	END

	SELECT SUM((CASE 
				WHEN MONTH(empca.WorkDate) = 1
					AND empca.IsOffday = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @UnpaidLeaveType
						OR empca.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay END
						)
				ELSE 0 END
				)) AS Jan
		,SUM((CASE 
				WHEN MONTH(empca.WorkDate) = 2
					AND empca.IsOffday = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @UnpaidLeaveType
						OR empca.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay END
						)
				ELSE 0 END
				)) AS Feb
		,SUM((CASE 
				WHEN MONTH(empca.WorkDate) = 3
					AND empca.IsOffday = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @UnpaidLeaveType
						OR empca.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay END
						)
				ELSE 0 END
				)) AS Mar
		,SUM((CASE 
				WHEN MONTH(empca.WorkDate) = 4
					AND empca.IsOffday = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @UnpaidLeaveType
						OR empca.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay END
						)
				ELSE 0 END
				)) AS Apr
		,SUM((CASE 
				WHEN MONTH(empca.WorkDate) = 5
					AND empca.IsOffday = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @UnpaidLeaveType
						OR empca.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay END
						)
				ELSE 0 END
				)) AS May
		,SUM((CASE 
				WHEN MONTH(empca.WorkDate) = 6
					AND empca.IsOffday = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @UnpaidLeaveType
						OR empca.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay END
						)
				ELSE 0 END
				)) AS Jun
		,SUM((CASE 
				WHEN MONTH(empca.WorkDate) = 7
					AND empca.IsOffday = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @UnpaidLeaveType
						OR empca.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay END
						)
				ELSE 0 END
				)) AS Jul
		,SUM((CASE 
				WHEN MONTH(empca.WorkDate) = 8
					AND empca.IsOffday = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @UnpaidLeaveType
						OR empca.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay END
						)
				ELSE 0 END
				)) AS Aug
		,SUM((CASE 
				WHEN MONTH(empca.WorkDate) = 9
					AND empca.IsOffday = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @UnpaidLeaveType
						OR empca.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay END
						)
				ELSE 0 END
				)) AS Sep
		,SUM((CASE 
				WHEN MONTH(empca.WorkDate) = 10
					AND empca.IsOffday = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @UnpaidLeaveType
						OR empca.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay END
						)
				ELSE 0 END
				)) AS Oct
		,SUM((CASE 
				WHEN MONTH(empca.WorkDate) = 11
					AND empca.IsOffday = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @UnpaidLeaveType
						OR empca.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay END
						)
				ELSE 0 END
				)) AS Nov
		,SUM((CASE 
				WHEN MONTH(empca.WorkDate) = 12
					AND empca.IsOffday = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @UnpaidLeaveType
						OR empca.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay END
						)
				ELSE 0 END
				)) AS [Dec]
		,SUM((CASE 
				WHEN empca.IsOffday = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @UnpaidLeaveType
						OR empca.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay END
						)
				ELSE 0 END
				)) AS [LeaveBalance]
		,emp.EmpId
		,emp.NickName
		,emp.FullName
		,g.GroupId
		,g.[Name] AS Team
	FROM dbo.EmployeeCalendar AS empca WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON empca.EmpId = emp.EmpId
		AND ISNULL(emp.isDeleted, 0) = 0
		AND ISNULL(emp.[Enabled], 0) = 1
		INNER JOIN EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = emp.GroupId
	WHERE empca.WorkDate BETWEEN @fromDate
			AND @toDate
		AND ISNULL(g.IsDeleted, 0) = 0
		AND (
			@IsLocal IS NULL
			OR empsum.IsLocal = @IsLocal
			)
		AND EXISTS (
			SELECT 1
			FROM @tblGroupIds AS tbl
			WHERE emp.GroupId = tbl.GroupId
			)
		AND EXISTS (
			SELECT 1
			FROM @tblEmployeeIds AS tbl
			WHERE emp.EmpId = tbl.EmpId
			)
	GROUP BY emp.EmpId
		,emp.NickName
		,emp.FullName
		,g.GroupId
		,g.[Name]
	HAVING SUM((CASE 
				WHEN empca.IsOffday = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @UnpaidLeaveType
						OR empca.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay END
						)
				ELSE 0 END
				)) > 0
	ORDER BY g.[Name]
END
