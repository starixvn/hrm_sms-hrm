
CREATE PROCEDURE [dbo].[WorkingTimeRequest_RejectRequest_ResetIgnoreDiscipline] @RequestId BIGINT
	,@ModifiedBy INT
	,@RoleId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160602@Tim.Trinh
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
		,IsHalfDay BIT
		,FromTime DATETIME
		,ToTime DATETIME
		,WorkType INT
		,IsOffOrHoliday BIT
		,ScheduleStatus NVARCHAR(MAX)
		,IsFromAfterday BIT
		,IsToAfterday BIT
		,IsUnpaidLeaveAbsent BIT
		,IsAbsent BIT
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
		INSERT INTO #LeaveRequestDetail
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

		IF @RoleId IN (2, 3, 6) -- verifier/approver/HR
		BEGIN
			DELETE dis
			FROM dbo.EmployeeDiscipline AS dis
				INNER JOIN #RequestDetail AS r ON dis.WorkDate = r.WorkDate
				AND dis.EmpId = r.EmployeeId
				AND dis.DisciplineType = 1
		END

		COMMIT TRANSACTION
	END TRY

	BEGIN CATCH
		ROLLBACK TRANSACTION

		SELECT @ErrCode = 1
			,@ErrMsg = N'Ignored discipline score unsuccessfully.'
	END CATCH

	DROP TABLE #LeaveRequestDetail

	DROP TABLE #RequestDetail
END
