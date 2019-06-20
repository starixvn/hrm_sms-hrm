
CREATE PROCEDURE [dbo].[WorkingTimeRequest_ResetRequest_ResetIgnoreDiscipline] @RequestId BIGINT
	,@ModifiedBy INT
	,@RoleId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160510@Tim.Trinh
Task: Reset ignore dicipline core
DB: SMSHRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @RequestType INT
		,@EmpId INT

	SELECT @EmpId = EmpId
		,@RequestType = RequestType
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	CREATE TABLE #LeaveRequestDetail (
		WorkDate DATE
		,LeaveTypeId INT
		,LeaveTypeName NVARCHAR(200)
		,IsHalfDay BIT
		,FromTime DATETIME
		,ToTime DATETIME
		,IsOffOrHoliday BIT
		,ScheduleStatus NVARCHAR(MAX)
		,IsFromAfterday BIT
		,IsToAfterday BIT
		,IsUnpaidLeaveAbsent BIT
		,IsAbsent BIT
		,IsUnplannedLeave BIT
		)

	CREATE TABLE #RequestDetail (
		WorkDate DATE
		,EmployeeId INT
		,DisciplineTypeId INT
		)

	IF @RequestType IN (1, 2) --check in/out
	BEGIN
		INSERT INTO #RequestDetail
		SELECT inout.OnDate
			,@EmpId
			,2 -- missing check in/out
		FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
			INNER JOIN dbo.InoutMissRequest AS inout WITH (NOLOCK) ON r.RequestId = inout.RequestId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON r.EmpId = emp.EmpId
			AND emp.isDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.EmployeeSummary AS emps WITH (NOLOCK) ON emp.EmpId = emps.EmpId
			INNER JOIN dbo.DisciplineSectionRule AS drule WITH (NOLOCK) ON emp.GroupId = drule.GroupId
			AND emps.IsLocal = drule.IsLocal
			AND drule.DisciplineTypeId = 2
		WHERE r.RequestId = @RequestId
	END
	ELSE IF @RequestType IN (3, 4) -- late in/ early out
	BEGIN
		INSERT INTO #RequestDetail
		SELECT inout.FromDate
			,@EmpId
			,CASE 
				WHEN @RequestType = 3 THEN 3 -- late in
				ELSE 4 -- early out
				END
		FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
			INNER JOIN dbo.InoutRequest AS inout WITH (NOLOCK) ON r.RequestId = inout.RequestId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = r.EmpId
			AND emp.isDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.EmployeeSummary AS emps WITH (NOLOCK) ON emp.EmpId = emps.EmpId
			INNER JOIN dbo.DisciplineSectionRule AS drule WITH (NOLOCK) ON emp.GroupId = drule.GroupId
			AND emps.IsLocal = drule.IsLocal
			AND (
				(
					@RequestType = 3
					AND drule.DisciplineTypeId = 3
					)
				OR (
					@RequestType = 4
					AND drule.DisciplineTypeId = 4
					)
				)
		WHERE r.RequestId = @RequestId
	END
	ELSE IF @RequestType = 5 -- change shift
	BEGIN
		INSERT INTO #RequestDetail
		SELECT d.OnDate
			,@EmpId
			,1 -- change shift
		FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
			INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON r.RequestId = cs.RequestId
			INNER JOIN dbo.ChangeShiftRequestDetail AS d WITH (NOLOCK) ON r.RequestId = d.RequestId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON r.EmpId = emp.EmpId
			AND emp.isDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.EmployeeSummary AS emps WITH (NOLOCK) ON emp.EmpId = emps.EmpId
			INNER JOIN dbo.DisciplineSectionRule AS drule WITH (NOLOCK) ON emp.GroupId = drule.GroupId
			AND emps.IsLocal = drule.IsLocal
			AND drule.DisciplineTypeId = 1
		WHERE r.RequestId = @RequestId
	END
	ELSE IF @RequestType = 6 -- leave
	BEGIN
		INSERT INTO #LeaveRequestDetail (
			WorkDate
			,LeaveTypeId
			,LeaveTypeName
			,IsHalfDay
			,FromTime
			,ToTime
			,IsOffOrHoliday
			,ScheduleStatus
			,IsFromAfterday
			,IsToAfterday
			,IsUnpaidLeaveAbsent
			,IsAbsent
			,IsUnplannedLeave
			)
		EXEC dbo.WorkingTimeRequest_GetLeaveRequestDetailToIgnoreById @RequestId = @RequestId

		INSERT INTO #RequestDetail
		SELECT WorkDate
			,@EmpId
			,CASE 
				WHEN IsUnpaidLeaveAbsent = 1 THEN 8
				WHEN IsAbsent = 1 THEN 5
				WHEN LeaveTypeId = 25 THEN 6 -- unpaid leave
				END
		FROM #LeaveRequestDetail
		WHERE IsUnpaidLeaveAbsent = 1
			OR IsAbsent = 1
			OR LeaveTypeId = 25 -- unpaid leave
	END

	BEGIN TRY
		BEGIN TRANSACTION

		IF @RoleId IN (2, 3, 6) -- verifier/approver/hr
		BEGIN
			UPDATE dis
			SET dis.IsIgnoredChangeShift = (CASE 
					WHEN r.DisciplineTypeId = 1 THEN 0
					ELSE dis.IsIgnoredChangeShift END
					)
				,dis.IsConfirmedChangeShift = (CASE 
					WHEN r.DisciplineTypeId = 1 THEN 0
					ELSE dis.IsConfirmedChangeShift END
					)
				,dis.IsIgnoredMissingCheckInOut = (CASE 
					WHEN r.DisciplineTypeId = 2 THEN 0
					ELSE dis.IsIgnoredMissingCheckInOut END
					)
				,dis.IsConfirmedMissingCheckInOut = (CASE 
					WHEN r.DisciplineTypeId = 2 THEN 0
					ELSE dis.IsConfirmedMissingCheckInOut END
					)
				,dis.IsIgnoredLateIn = (CASE 
					WHEN r.DisciplineTypeId = 3 THEN 0
					ELSE dis.IsIgnoredLateIn END
					)
				,dis.IsConfirmedLateIn = (CASE 
					WHEN r.DisciplineTypeId = 3 THEN 0
					ELSE dis.IsConfirmedLateIn END
					)
				,dis.IsIgnoredEarlyOut = (CASE 
					WHEN r.DisciplineTypeId = 4 THEN 0
					ELSE dis.IsIgnoredEarlyOut END
					)
				,dis.IsConfirmedEarlyOut = (CASE 
					WHEN r.DisciplineTypeId = 4 THEN 0
					ELSE dis.IsConfirmedEarlyOut END
					)
				,dis.IsIgnoredAbsent = (CASE 
					WHEN r.DisciplineTypeId = 5 THEN 0
					ELSE dis.IsIgnoredAbsent END
					)
				,dis.IsConfirmedAbsent = (CASE 
					WHEN r.DisciplineTypeId = 5 THEN 0
					ELSE dis.IsConfirmedAbsent END
					)
				,dis.IsIgnoredUnpaidLeave = (CASE 
					WHEN r.DisciplineTypeId = 6 THEN 0
					ELSE dis.IsIgnoredUnpaidLeave END
					)
				,dis.IsConfirmedUnpaidLeave = (CASE 
					WHEN r.DisciplineTypeId = 6 THEN 0
					ELSE dis.IsConfirmedUnpaidLeave END
					)
				,dis.IsIgnoredWorkingUnderTime = (CASE 
					WHEN r.DisciplineTypeId = 7 THEN 0
					ELSE dis.IsIgnoredWorkingUnderTime END
					)
				,dis.IsConfirmedWorkingUnderTime = (CASE 
					WHEN r.DisciplineTypeId = 7 THEN 0
					ELSE dis.IsConfirmedWorkingUnderTime END
					)
				,dis.IsIgnoredUnpaidLeaveAbsent = (CASE 
					WHEN r.DisciplineTypeId = 8 THEN 0
					ELSE dis.IsIgnoredUnpaidLeaveAbsent END
					)
				,dis.IsConfirmedUnpaidLeaveAbsent = (CASE 
					WHEN r.DisciplineTypeId = 8 THEN 0
					ELSE dis.IsConfirmedUnpaidLeaveAbsent END
					)
				,dis.IsIgnoredUnplannedLeave = (CASE 
					WHEN r.DisciplineTypeId = 9 THEN 0
					ELSE dis.IsIgnoredUnplannedLeave END
					)
				,dis.IsConfirmedUnplannedLeave = (CASE 
					WHEN r.DisciplineTypeId = 9 THEN 0
					ELSE dis.IsConfirmedUnplannedLeave END
					)
				,dis.IsIgnoreWeekendLeave = (CASE 
					WHEN r.DisciplineTypeId = 10 THEN 0
					ELSE dis.IsIgnoreWeekendLeave END
					)
				,dis.IsConfirmedWeekendLeave = (CASE 
					WHEN r.DisciplineTypeId = 10 THEN 0
					ELSE dis.IsConfirmedWeekendLeave END
					)
				,ModifiedBy = @ModifiedBy
				,ModifiedDate = GETDATE()
			FROM dbo.EmployeeDiscipline AS dis WITH (ROWLOCK, UPDLOCK)
				INNER JOIN #RequestDetail AS r ON dis.WorkDate = r.WorkDate
				AND dis.EmpId = r.EmployeeId
				AND dis.DisciplineType = 1
		END

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Ignore discipline score unsuccessfully.'
	END CATCH

	DROP TABLE #LeaveRequestDetail

	DROP TABLE #RequestDetail
END