
CREATE PROCEDURE [dbo].[Report_WorkingHoursSummary] @FromDate DATE
	,@ToDate DATE
	,@EmployeeId INT = NULL
	,@GroupIds VARCHAR(MAX) = NULL
	,@isLocal BIT = NULL
AS
/*
Created: Tom.Vo@2013-11-21
Task: Working Hours Summary Report
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	DECLARE @AnnualLeaveTypeId SMALLINT = 2
		,@UnpaidLeaveTypeId SMALLINT = 25
		,@UnpaidLeaveInsuranceTypeId SMALLINT = 41
		,@PaternityLeaveTypeId SMALLINT = 14
		,@MaternityLeaveTypeId SMALLINT = 12
		,@BusinessTripTypeId SMALLINT = 9
		,@BereavementLeaveTypeId SMALLINT = 21
		,@CompensationLeaveTypeId SMALLINT = 27
		,@WeddingLeaveTypeId SMALLINT = 6
		-- request status
		,@ApprovedStatusId TINYINT = 6

	CREATE TABLE #tblGroupIds (GroupId INT)

	INSERT INTO #tblGroupIds (GroupId)
	SELECT val
	FROM dbo.fnc_strSplit(@GroupIds, ',')

	SELECT emp.EmpId AS EmployeeId
		,emp.Code
		,emp.FullName
		,emp.NickName
		,gr.[Name] AS GroupName
		,empsum.WorkingDays
		,empsum.WorkingHours
		,SUM(ISNULL(empatt.TotalWorkHours, 0)) AS TotalWorkingHours
		--,SUM(CASE 
		--		WHEN ISNULL(empatt.TotalWorkHours, 0) = 0
		--			OR (
		--				empca.IsLeaveDay = 1
		--				AND empca.LeaveDay = 1
		--				) THEN 0
		--		WHEN empca.IsLeaveDay = 0 THEN 1
		--		WHEN empca.LeaveDay < 1 THEN 0.5
		--		ELSE 0
		--		END) AS ActualWorkingDays
		,SUM(CASE 
				WHEN empca.IsHoliday = 1
					OR empca.IsOffDay = 1
					--OR ISNULL(empatt.TotalWorkHours, 0) = 0
					OR (
						empca.IsLeaveDay = 1
						AND empca.LeaveDay = 1
						) THEN 0
				WHEN empca.IsLeaveDay = 0 THEN 1
				WHEN empca.LeaveDay < 1 THEN 0.5
				ELSE 0
				END) AS PaymentWorkingDays
		,SUM(CASE 
				WHEN empca.IsOffDay = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @AnnualLeaveTypeId
						OR empca.LeaveTypeId2 = @AnnualLeaveTypeId
						) THEN CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay
						END
				ELSE 0
				END) AS AnnualLeave
		,SUM(CASE 
				WHEN empca.IsOffDay = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @BusinessTripTypeId
						OR empca.LeaveTypeId2 = @BusinessTripTypeId
						) THEN CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay
						END
				ELSE 0
				END) AS BusinessTrip
		,SUM(CASE 
				WHEN empca.IsOffDay = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @BereavementLeaveTypeId
						OR empca.LeaveTypeId2 = @BereavementLeaveTypeId
						) THEN CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay
						END
				ELSE 0
				END) AS BereavementLeave
		,SUM(CASE 
				WHEN empca.IsOffDay = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @CompensationLeaveTypeId
						OR empca.LeaveTypeId2 = @CompensationLeaveTypeId
						) THEN CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay
						END
				ELSE 0
				END) AS CompensationLeave
		,SUM(CASE 
				WHEN empca.IsOffDay = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @WeddingLeaveTypeId
						OR empca.LeaveTypeId2 = @WeddingLeaveTypeId
						) THEN CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay
						END
				ELSE 0
				END) AS WeddingLeave
		,SUM(CASE 
				WHEN empca.IsOffDay = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @UnpaidLeaveTypeId
						OR empca.LeaveTypeId2 = @UnpaidLeaveTypeId
						) THEN CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay
						END
				ELSE 0
				END) AS UnpaidLeave
		,SUM(CASE 
				WHEN empca.IsOffDay = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @UnpaidLeaveInsuranceTypeId
						OR empca.LeaveTypeId2 = @UnpaidLeaveInsuranceTypeId
						) THEN CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay
						END
				ELSE 0
				END) AS UnpaidLeaveInsurance
		,SUM(CASE 
				WHEN empca.IsOffDay = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @PaternityLeaveTypeId
						OR empca.LeaveTypeId2 = @PaternityLeaveTypeId
						) THEN CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay
						END
				ELSE 0
				END) AS PaternityLeave
		,SUM(CASE 
				WHEN empca.IsOffDay = 0
					AND empca.IsHoliday = 0
					AND empca.IsLeaveDay = 1
					AND (
						empca.LeaveTypeId = @MaternityLeaveTypeId
						OR empca.LeaveTypeId2 = @MaternityLeaveTypeId
						) THEN CASE 
						WHEN ISNULL(empca.LeaveTypeId2, 0) <> 0
							AND ISNULL(empca.LeaveTypeId, 0) <> ISNULL(empca.LeaveTypeId2, 0) THEN 0.5
						ELSE empca.LeaveDay
						END
				ELSE 0
				END) AS MaternityLeave
		,SUM(ISNULL(ot.NormalHours, 0)) AS NormalOvertimeHours
		,SUM(ISNULL(ot.OffHours, 0)) AS OffOvertimeHours
		,SUM(ISNULL(ot.PublicHours, 0)) AS PublicOvertimeHours
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
		INNER JOIN dbo.EmployeeAttendance AS empatt WITH (NOLOCK) ON emp.EmpId = empatt.EmpId
		AND empatt.AttendanceDate BETWEEN @FromDate
			AND @ToDate
		LEFT JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON emp.EmpId = empca.EmpId
		AND empca.WorkDate = empatt.AttendanceDate
		LEFT JOIN dbo.OTDailyRequest AS ot WITH (NOLOCK) ON ot.EmpId = emp.EmpId
		AND empca.WorkDate = ot.OvertimeDate
		AND ot.[Status] = @ApprovedStatusId
	WHERE (
			@EmployeeId IS NULL
			OR emp.EmpId = @EmployeeId
			)
		AND emp.isDeleted = 0
		AND (
			@isLocal IS NULL
			OR empsum.IsLocal = @isLocal
			)
		AND (
			ISNULL(@GroupIds, '') = ''
			OR EXISTS (
				SELECT 1
				FROM #tblGroupIds AS tbl
				WHERE emp.GroupId = tbl.GroupId
				)
			)
	GROUP BY emp.EmpId
		,emp.Code
		,emp.NickName
		,emp.FullName
		,empsum.WorkingDays
		,empsum.WorkingHours
		,gr.[Name]
	ORDER BY emp.NickName

	DROP TABLE #tblGroupIds
END
