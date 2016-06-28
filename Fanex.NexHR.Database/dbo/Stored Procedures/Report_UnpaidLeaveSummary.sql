CREATE PROCEDURE [dbo].[Report_UnpaidLeaveSummary] @Year INT
	,@EmpId INT = NULL
	,@GroupIds VARCHAR(MAX) = ''
	,@IsLocal BIT = NULL
	,@SupervisorId INT = NULL
AS
/*
Created: 20151028@Quinn.Vo
Task: Get unpaid leave report.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @UnpaidLeaveType INT

	SELECT @UnpaidLeaveType = LeaveTypeId
	FROM dbo.LeaveType WITH (NOLOCK)
	WHERE Code = 'UPL'

	CREATE TABLE #Groups (GroupId INT)

	CREATE TABLE #EmployeeId (EmpId INT)

	IF ISNULL(@GroupIds, '') <> ''
	BEGIN
		INSERT INTO #Groups (GroupId)
		SELECT val
		FROM fnc_strSplit(@GroupIds, ',')
	END
	ELSE
	BEGIN
		INSERT INTO #Groups (GroupId)
		SELECT GroupId
		FROM dbo.[Group] AS g WITH (NOLOCK)

		IF EXISTS (
				SELECT 1
				FROM dbo.Employee
				WHERE EmpId = @SupervisorId
					AND (
						Roles LIKE '%6%'
						OR Roles LIKE '%7%'
						OR Roles LIKE '%2%'
						OR Roles LIKE '%3%'
						OR Roles LIKE '%4%'
						OR Roles LIKE '%5%'
						)
				)
		BEGIN
			INSERT INTO #Groups (GroupId)
			SELECT GroupId
			FROM dbo.[Group] AS g WITH (NOLOCK)
		END
		ELSE
		BEGIN
			SET @EmpId = @SupervisorId
		END
	END

	IF (@EmpId IS NULL)
	BEGIN
		INSERT INTO #EmployeeId (EmpId)
		SELECT EmpId
		FROM [dbo].[fnc_GetEmployeeByActorId](@SupervisorId, 1, 0)
	END
	ELSE
	BEGIN
		INSERT INTO #EmployeeId (EmpId)
		VALUES (@EmpId)
	END

	SELECT SUM((CASE 
				WHEN MONTH(cl.WorkDate) = 1
					AND cl.IsOffday = 0
					AND cl.IsHoliday = 0
					AND cl.IsLeaveDay = 1
					AND (
						cl.LeaveTypeId = @UnpaidLeaveType
						OR cl.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(cl.LeaveTypeId2, 0) <> 0
							AND ISNULL(cl.LeaveTypeId, 0) <> ISNULL(cl.LeaveTypeId2, 0) THEN 0.5
						ELSE cl.LeaveDay END
						)
				ELSE 0 END
				)) AS Jan
		,SUM((CASE 
				WHEN MONTH(cl.WorkDate) = 2
					AND cl.IsOffday = 0
					AND cl.IsHoliday = 0
					AND cl.IsLeaveDay = 1
					AND (
						cl.LeaveTypeId = @UnpaidLeaveType
						OR cl.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(cl.LeaveTypeId2, 0) <> 0
							AND ISNULL(cl.LeaveTypeId, 0) <> ISNULL(cl.LeaveTypeId2, 0) THEN 0.5
						ELSE cl.LeaveDay END
						)
				ELSE 0 END
				)) AS Feb
		,SUM((CASE 
				WHEN MONTH(cl.WorkDate) = 3
					AND cl.IsOffday = 0
					AND cl.IsHoliday = 0
					AND cl.IsLeaveDay = 1
					AND (
						cl.LeaveTypeId = @UnpaidLeaveType
						OR cl.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(cl.LeaveTypeId2, 0) <> 0
							AND ISNULL(cl.LeaveTypeId, 0) <> ISNULL(cl.LeaveTypeId2, 0) THEN 0.5
						ELSE cl.LeaveDay END
						)
				ELSE 0 END
				)) AS Mar
		,SUM((CASE 
				WHEN MONTH(cl.WorkDate) = 4
					AND cl.IsOffday = 0
					AND cl.IsHoliday = 0
					AND cl.IsLeaveDay = 1
					AND (
						cl.LeaveTypeId = @UnpaidLeaveType
						OR cl.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(cl.LeaveTypeId2, 0) <> 0
							AND ISNULL(cl.LeaveTypeId, 0) <> ISNULL(cl.LeaveTypeId2, 0) THEN 0.5
						ELSE cl.LeaveDay END
						)
				ELSE 0 END
				)) AS Apr
		,SUM((CASE 
				WHEN MONTH(cl.WorkDate) = 5
					AND cl.IsOffday = 0
					AND cl.IsHoliday = 0
					AND cl.IsLeaveDay = 1
					AND (
						cl.LeaveTypeId = @UnpaidLeaveType
						OR cl.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(cl.LeaveTypeId2, 0) <> 0
							AND ISNULL(cl.LeaveTypeId, 0) <> ISNULL(cl.LeaveTypeId2, 0) THEN 0.5
						ELSE cl.LeaveDay END
						)
				ELSE 0 END
				)) AS May
		,SUM((CASE 
				WHEN MONTH(cl.WorkDate) = 6
					AND cl.IsOffday = 0
					AND cl.IsHoliday = 0
					AND cl.IsLeaveDay = 1
					AND (
						cl.LeaveTypeId = @UnpaidLeaveType
						OR cl.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(cl.LeaveTypeId2, 0) <> 0
							AND ISNULL(cl.LeaveTypeId, 0) <> ISNULL(cl.LeaveTypeId2, 0) THEN 0.5
						ELSE cl.LeaveDay END
						)
				ELSE 0 END
				)) AS Jun
		,SUM((CASE 
				WHEN MONTH(cl.WorkDate) = 7
					AND cl.IsOffday = 0
					AND cl.IsHoliday = 0
					AND cl.IsLeaveDay = 1
					AND (
						cl.LeaveTypeId = @UnpaidLeaveType
						OR cl.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(cl.LeaveTypeId2, 0) <> 0
							AND ISNULL(cl.LeaveTypeId, 0) <> ISNULL(cl.LeaveTypeId2, 0) THEN 0.5
						ELSE cl.LeaveDay END
						)
				ELSE 0 END
				)) AS Jul
		,SUM((CASE 
				WHEN MONTH(cl.WorkDate) = 8
					AND cl.IsOffday = 0
					AND cl.IsHoliday = 0
					AND cl.IsLeaveDay = 1
					AND (
						cl.LeaveTypeId = @UnpaidLeaveType
						OR cl.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(cl.LeaveTypeId2, 0) <> 0
							AND ISNULL(cl.LeaveTypeId, 0) <> ISNULL(cl.LeaveTypeId2, 0) THEN 0.5
						ELSE cl.LeaveDay END
						)
				ELSE 0 END
				)) AS Aug
		,SUM((CASE 
				WHEN MONTH(cl.WorkDate) = 9
					AND cl.IsOffday = 0
					AND cl.IsHoliday = 0
					AND cl.IsLeaveDay = 1
					AND (
						cl.LeaveTypeId = @UnpaidLeaveType
						OR cl.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(cl.LeaveTypeId2, 0) <> 0
							AND ISNULL(cl.LeaveTypeId, 0) <> ISNULL(cl.LeaveTypeId2, 0) THEN 0.5
						ELSE cl.LeaveDay END
						)
				ELSE 0 END
				)) AS Sep
		,SUM((CASE 
				WHEN MONTH(cl.WorkDate) = 10
					AND cl.IsOffday = 0
					AND cl.IsHoliday = 0
					AND cl.IsLeaveDay = 1
					AND (
						cl.LeaveTypeId = @UnpaidLeaveType
						OR cl.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(cl.LeaveTypeId2, 0) <> 0
							AND ISNULL(cl.LeaveTypeId, 0) <> ISNULL(cl.LeaveTypeId2, 0) THEN 0.5
						ELSE cl.LeaveDay END
						)
				ELSE 0 END
				)) AS Oct
		,SUM((CASE 
				WHEN MONTH(cl.WorkDate) = 11
					AND cl.IsOffday = 0
					AND cl.IsHoliday = 0
					AND cl.IsLeaveDay = 1
					AND (
						cl.LeaveTypeId = @UnpaidLeaveType
						OR cl.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(cl.LeaveTypeId2, 0) <> 0
							AND ISNULL(cl.LeaveTypeId, 0) <> ISNULL(cl.LeaveTypeId2, 0) THEN 0.5
						ELSE cl.LeaveDay END
						)
				ELSE 0 END
				)) AS Nov
		,SUM((CASE 
				WHEN MONTH(cl.WorkDate) = 12
					AND cl.IsOffday = 0
					AND cl.IsHoliday = 0
					AND cl.IsLeaveDay = 1
					AND (
						cl.LeaveTypeId = @UnpaidLeaveType
						OR cl.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(cl.LeaveTypeId2, 0) <> 0
							AND ISNULL(cl.LeaveTypeId, 0) <> ISNULL(cl.LeaveTypeId2, 0) THEN 0.5
						ELSE cl.LeaveDay END
						)
				ELSE 0 END
				)) AS [Dec]
		,SUM((CASE 
				WHEN cl.IsOffday = 0
					AND cl.IsHoliday = 0
					AND cl.IsLeaveDay = 1
					AND (
						cl.LeaveTypeId = @UnpaidLeaveType
						OR cl.LeaveTypeId2 = @UnpaidLeaveType
						) THEN (CASE 
						WHEN ISNULL(cl.LeaveTypeId2, 0) <> 0
							AND ISNULL(cl.LeaveTypeId, 0) <> ISNULL(cl.LeaveTypeId2, 0) THEN 0.5
						ELSE cl.LeaveDay END
						)
				ELSE 0 END
				)) AS [LeaveBalance]
		,e.EmpId
		,e.NickName
		,e.FullName
		,g.GroupId
		,g.Code
		,g.[Name]
	FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON cl.EmpId = e.EmpId
		INNER JOIN #EmployeeId AS empId ON e.EmpId = empId.EmpId
		INNER JOIN EmployeeSummary es WITH (NOLOCK) ON e.EmpId = es.EmpId
		INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON g.GroupId = e.GroupId
		INNER JOIN #Groups AS gr ON g.GroupId = gr.GroupId
	WHERE YEAR(cl.WorkDate) = @Year
		AND ISNULL(e.isDeleted, 0) = 0
		AND ISNULL(e.[Enabled], 0) = 1
		AND (
			@IsLocal IS NULL
			OR es.IsLocal = @IsLocal
			)
	GROUP BY e.EmpId
		,e.NickName
		,e.FullName
		,g.GroupId
		,g.Code
		,g.[Name]
		,g.[Rank]
	ORDER BY [Rank]
		,g.[Name]
		,Code

	DROP TABLE #EmployeeId

	DROP TABLE #Groups
END