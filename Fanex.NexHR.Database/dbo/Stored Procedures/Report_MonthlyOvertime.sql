
CREATE PROCEDURE [dbo].[Report_MonthlyOvertime] @ActorId INT
	,@FromDate DATE
	,@ToDate DATE
	,@Employee NVARCHAR(200) = NULL
	,@EmployeeStatusId TINYINT = NULL
	,@TeamIds VARCHAR(MAX) = ''
AS
/*
Created: Tim.Trinh@2019-07-09
Task: monthly overtime
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromDatetmp DATE = @FromDate
		-- employee status
		,@Enable TINYINT = 1
		,@Disabled TINYINT = 2
		,@Deleted TINYINT = 3
		-- request status
		,@VerifyStatusId TINYINT = 2
		,@ApproveStatusId TINYINT = 4
		,@ProcessStatusId TINYINT = 6
	DECLARE @tblEmployeeId TABLE (EmpId INT)
	DECLARE @tblDate TABLE (
		WorkDate DATE
		,IsHoliday BIT
		)
	DECLARE @tblGroupIds TABLE (GroupId INT)

	IF ISNULL(@TeamIds, '') <> ''
	BEGIN
		INSERT INTO @tblGroupIds (GroupId)
		SELECT DISTINCT val
		FROM fnc_strSplit(@TeamIds, ',')
	END

	WHILE @fromDatetmp <= @ToDate
	BEGIN
		INSERT INTO @tblDate (
			WorkDate
			,IsHoliday
			)
		VALUES (
			@fromDatetmp
			,CASE 
				WHEN EXISTS (
						SELECT 1
						FROM dbo.Holiday WITH (NOLOCK)
						WHERE @fromDatetmp BETWEEN FromDate
								AND ToDate
						) THEN 1
				ELSE 0
				END
			)

		SET @fromDatetmp = DATEADD(DAY, 1, @fromDatetmp)
	END

	SELECT WorkDate AS [Date]
		,IsHoliday
	FROM @tblDate
	ORDER BY WorkDate

	INSERT INTO @tblEmployeeId (EmpId)
	SELECT emp.EmpId
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		AND empsum.IsLocal = 1
		INNER JOIN (
		SELECT EmpId
		FROM dbo.fnc_GetEmployeeByActorId(@ActorId, 0, 1)
		) AS member ON emp.EmpId = member.EmpId
	WHERE (
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
		AND EXISTS (
			SELECT 1
			FROM @tblGroupIds AS tbl
			WHERE tbl.GroupId = emp.GroupId
			)

	SELECT emp.EmpId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,gr.[Name] AS Team
		,tmp.WorkDate
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
	FROM @tblDate AS tmp
		INNER JOIN dbo.OTDailyRequest AS ot WITH (NOLOCK) ON tmp.WorkDate = ot.OvertimeDate
		AND ot.[Status] IN (@VerifyStatusId, @ApproveStatusId, @ProcessStatusId)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON ot.EmpId = emp.EmpId
		INNER JOIN @tblEmployeeId AS empid ON emp.EmpId = empid.EmpId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON emp.EmpId = empca.EmpId
		AND ot.OvertimeDate = empca.WorkDate
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.ShiftId = ot.[Schedule]
	ORDER BY tmp.WorkDate
		,emp.NickName
END
