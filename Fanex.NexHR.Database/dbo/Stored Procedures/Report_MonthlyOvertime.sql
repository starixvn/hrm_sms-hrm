
CREATE PROCEDURE [dbo].[Report_MonthlyOvertime] @FromDate DATE
	,@ToDate DATE
	,@EmployeeId INT = NULL
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
		-- request status
		,@VerifyStatusId TINYINT = 2
		,@ApproveStatusId TINYINT = 4
		,@ProcessStatusId TINYINT = 6

	CREATE TABLE #tblDate (
		WorkDate DATE
		,IsHoliday BIT
		)

	WHILE @fromDatetmp <= @ToDate
	BEGIN
		INSERT INTO #tblDate (
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
	FROM #tblDate
	ORDER BY WorkDate

	SELECT emp.EmpId AS EmployeeId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,gr.GroupId
		,gr.Code AS GroupCode
		,gr.[Name] AS GroupName
		,tmp.WorkDate
		,s.ShiftId
		,s.Code AS ShiftCode
		,CONVERT(VARCHAR(5), ot.TimeIn, 108) AS TimeIn
		,CONVERT(VARCHAR(5), ot.[TimeOut], 108) AS [TimeOut]
		,ot.TotalHours AS ActualWorkingHours
		,ot.NormalHours
		,ot.OffHours
		,ot.PublicHours
		,ot.Remark
		,empca.IsHoliday
		,empca.IsOffDay
		,ot.[Status] AS StatusId
	FROM #tblDate AS tmp WITH (NOLOCK)
		INNER JOIN dbo.OTDailyRequest AS ot WITH (NOLOCK) ON tmp.WorkDate = ot.OvertimeDate
		AND ot.[Status] IN (@VerifyStatusId, @ApproveStatusId, @ProcessStatusId)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON ot.EmpId = emp.EmpId
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		AND empsum.IsLocal = 1
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		LEFT JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON emp.EmpId = empca.EmpId
		AND ot.OvertimeDate = empca.WorkDate
		LEFT JOIN dbo.[Shift] AS s WITH (NOLOCK) ON s.ShiftId = ot.[Schedule]
	WHERE @EmployeeId IS NULL
		OR emp.EmpId = @EmployeeId
	ORDER BY tmp.WorkDate
		,emp.NickName

	DROP TABLE #tblDate
END
