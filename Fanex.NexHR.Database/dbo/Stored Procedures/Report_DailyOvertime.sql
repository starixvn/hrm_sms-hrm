CREATE PROCEDURE [dbo].[Report_DailyOvertime] @ActorId INT
	,@fromDate DATE
	,@toDate DATE
	,@TeamIds VARCHAR(MAX) = NULL
	,@isLocal BIT = NULL
	,@Employee NVARCHAR(200) = NULL
	,@EmployeeStatusId TINYINT = NULL
AS
/*
Created: Tim.Trinh@2019-07-10
Task: Get daily overtime details report 
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE
		-- employee status
		@Enable TINYINT = 1
		,@Disabled TINYINT = 2
		,@Deleted TINYINT = 3

	CREATE TABLE #tblGroupIds (GroupId INT)

	INSERT INTO #tblGroupIds (GroupId)
	SELECT val
	FROM dbo.fnc_strSplit(@TeamIds, ',')

	SELECT emp.EmpId
		,emp.NickName
		,emp.FullName
		,emp.Code
		,gr.GroupId
		,gr.[Name] AS Team
		,ot.OvertimeDate AS WorkDate
		,s.ShiftId
		,s.Code AS ShiftCode
		,CONVERT(VARCHAR(5), ot.TimeIn, 108) AS TimeIn
		,CONVERT(VARCHAR(5), ot.[TimeOut], 108) AS [TimeOut]
		,ot.TotalHours AS ActualWorkingHours
		,ot.NormalHours
		,ot.OffHours
		,ot.PublicHours
		,ot.SpecialHours
		,ot.Remark
		,empca.IsHoliday
		,empca.IsOffDay
		,ot.[Status] AS StatusId
		,dept.Code as DepartmentCode
	    ,sec.Code as Section
	FROM dbo.OTDailyRequest AS ot WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = ot.EmpId
		INNER JOIN (
		SELECT EmpId
		FROM dbo.fnc_GetEmployeeByActorId(@ActorId, 0, 1)
		) AS member ON emp.EmpId = member.EmpId
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON empsum.EmpId = emp.EmpId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.Section AS sec WITH (NOLOCK) ON gr.SectionId = sec.SectionId
		INNER JOIN dbo.Department AS dept WITH (NOLOCK) ON sec.DepartmentId = dept.DepartmentId
		LEFT JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON emp.EmpId = empca.EmpId
		AND ot.OvertimeDate = empca.WorkDate
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.ShiftId = ot.[Schedule]
	WHERE ot.OvertimeDate BETWEEN @fromDate
			AND @toDate
		AND (
			@EmployeeStatusId IS NULL
			OR (
				@EmployeeStatusId = @Enable
				AND emp.[Enabled] = 1
				AND emp.isDeleted = 0
				)
			OR (
				@EmployeeStatusId = @Disabled
				AND emp.[Enabled] = 0
				AND emp.isDeleted = 0
				)
			OR (
				@EmployeeStatusId = @Deleted
				AND emp.isDeleted = 1
				)
			)
		AND (
			emp.NickName LIKE '%' + ISNULL(@Employee, '') + '%'
			OR emp.FullName LIKE '%' + ISNULL(@Employee, '') + '%'
			)
		AND (
			@isLocal IS NULL
			OR empsum.IsLocal = @isLocal
			)
		AND (
			ISNULL(@TeamIds, '') = ''
			OR EXISTS (
				SELECT 1
				FROM #tblGroupIds AS tbl
				WHERE emp.GroupId = tbl.GroupId
				)
			)
	ORDER BY ot.OvertimeDate
		,emp.NickName

	DROP TABLE #tblGroupIds
END
