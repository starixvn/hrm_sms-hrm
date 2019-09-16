
CREATE PROCEDURE [dbo].[ScheduleArrangement_UpdateWeeklyDefaultShift] @ActorId INT
	,@EmployeeId INT
	,@FromDate DATE
	,@MonShiftId INT = NULL
	,@TueShiftId INT = NULL
	,@WedShiftId INT = NULL
	,@ThuShiftId INT = NULL
	,@FriShiftId INT = NULL
	,@SatShiftId INT = NULL
	,@SunShiftId INT = NULL
	,@IsUsedWeeklyDefaultShift BIT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-05-07
Task: update weekly default shift
DB: HRM
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @Today DATE = GETDATE()
		,@ModifiedDate DATETIME = GETDATE()
		,@FromDate1 DATE = @FromDate
		,@ToDate1 DATE = DATEADD(MONTH, 4, @FromDate)
		-- work type
		,@OffDay TINYINT = 1
		,@LeaveDay TINYINT = 2
		,@WorkDay TINYINT = 3
		-- request status
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@NotVerifiedStatusId TINYINT = 3
		,@ApprovedStatusId TINYINT = 4
		,@RejectedStatusId TINYINT = 5
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7

	CREATE TABLE #tblDate (WorkDate DATE)

	UPDATE dbo.EmployeeDefaultShift WITH (ROWLOCK, UPDLOCK)
	SET MonShift = @MonShiftId
		,TueShift = @TueShiftId
		,WedShift = @WedShiftId
		,ThuShift = @ThuShiftId
		,FriShift = @FriShiftId
		,SatShift = @SatShiftId
		,SunShift = @SunShiftId
	WHERE EmpID = @EmployeeId

	UPDATE dbo.EmployeeSummary WITH (ROWLOCK, UPDLOCK)
	SET IsDefaultSchedule = @IsUsedWeeklyDefaultShift
	WHERE EmpId = @EmployeeId

	IF @IsUsedWeeklyDefaultShift = 1
	BEGIN
		UPDATE empca
		SET empca.ShiftId = CASE 
				WHEN empsum.IsLocal = 0
					OR (
						empsum.IsLocal = 1
						AND holiday.Id IS NULL
						) THEN CASE 
						WHEN DATEPART(dw, empca.WorkDate) = 1 THEN SunShift
						WHEN DATEPART(dw, empca.WorkDate) = 2 THEN MonShift
						WHEN DATEPART(dw, empca.WorkDate) = 3 THEN TueShift
						WHEN DATEPART(dw, empca.WorkDate) = 4 THEN WedShift
						WHEN DATEPART(dw, empca.WorkDate) = 5 THEN ThuShift
						WHEN DATEPART(dw, empca.WorkDate) = 6 THEN FriShift
						WHEN DATEPART(dw, empca.WorkDate) = 7 THEN SatShift
						END
				ELSE empca.ShiftId
				END
			,empca.IsOffDay = CASE 
				WHEN (
						empsum.IsLocal = 1
						OR empsum.IsExpatUseHoliday = 1
						)
					AND holiday.Id IS NOT NULL THEN 0
				WHEN DATEPART(dw, empca.WorkDate) = 1
					AND SunShift IS NULL THEN 1
				WHEN DATEPART(dw, empca.WorkDate) = 2
					AND MonShift IS NULL THEN 1
				WHEN DATEPART(dw, empca.WorkDate) = 3
					AND TueShift IS NULL THEN 1
				WHEN DATEPART(dw, empca.WorkDate) = 4
					AND WedShift IS NULL THEN 1
				WHEN DATEPART(dw, empca.WorkDate) = 5
					AND ThuShift IS NULL THEN 1
				WHEN DATEPART(dw, empca.WorkDate) = 6
					AND FriShift IS NULL THEN 1
				WHEN DATEPART(dw, empca.WorkDate) = 7
					AND SatShift IS NULL THEN 1
				ELSE 0
				END
			,empca.IsHoliday = CASE 
				WHEN (
						empsum.IsLocal = 1
						OR empsum.IsExpatUseHoliday = 1
						)
					AND holiday.Id IS NOT NULL THEN 1
				ELSE 0
				END
			,empca.ModifiedBy = @ActorId
			,empca.ModifiedDate = @ModifiedDate
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON empca.EmpId = empsum.EmpId
			AND empsum.IsDefaultSchedule = 1
			LEFT JOIN dbo.EmployeeDefaultShift AS shift WITH (NOLOCK) ON empca.EmpId = shift.EmpID
			LEFT JOIN dbo.Holiday AS holiday WITH (NOLOCK) ON empca.WorkDate BETWEEN holiday.FromDate
				AND holiday.ToDate
		WHERE empca.EmpId = @EmployeeId
			AND empca.WorkDate >= @FromDate
			AND empca.IsLeaveDay = 0
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.ChangeShiftRequest AS csr WITH (NOLOCK)
					INNER JOIN dbo.ChangeShiftRequestDetail AS csrdt WITH (NOLOCK) ON csr.RequestId = csrdt.RequestId
					AND csrdt.OnDate = empca.WorkDate
					INNER JOIN dbo.WorkingTimeRequest AS wkr WITH (NOLOCK) ON csr.RequestId = wkr.RequestId
					AND wkr.CurrentStatus IN (@VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId)
				WHERE empca.EmpId = csr.EmpId
				)
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
					INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON wkr.RequestId = lrd.RequestId
					AND empca.EmpId = lrd.EmpId
					AND empca.WorkDate = lrd.OnDate
					AND lrd.WorkType = @OffDay
				WHERE wkr.CurrentStatus IN (@VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId)
				)

		UPDATE empca
		SET empca.IsFixedOffDay = 0
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		WHERE empca.IsOffDay = 0
			AND empca.IsFixedOffDay = 1

		WHILE @FromDate1 <= @ToDate1
		BEGIN
			INSERT INTO #tblDate (WorkDate)
			VALUES (@FromDate1)

			SET @FromDate1 = DATEADD(DAY, 1, @FromDate1)
		END

		INSERT INTO dbo.EmployeeCalendar (
			EmpId
			,WorkDate
			,ShiftId
			,IsOffDay
			,IsFixedOffDay
			,IsHoliday
			,IsPublic
			,ModifiedBy
			,ModifiedDate
			)
		SELECT DISTINCT emp.EmpId
			,tblc.WorkDate
			,(CASE 
				WHEN (
						empsum.IsLocal = 1
						OR empsum.IsExpatUseHoliday = 1
						)
					AND holiday.Id IS NOT NULL THEN NULL
				WHEN DATEPART(dw, tblc.WorkDate) = 1 THEN shift.SunShift
				WHEN DATEPART(dw, tblc.WorkDate) = 2 THEN shift.MonShift
				WHEN DATEPART(dw, tblc.WorkDate) = 3 THEN shift.TueShift
				WHEN DATEPART(dw, tblc.WorkDate) = 4 THEN shift.WedShift
				WHEN DATEPART(dw, tblc.WorkDate) = 5 THEN shift.ThuShift
				WHEN DATEPART(dw, tblc.WorkDate) = 6 THEN shift.FriShift
				WHEN DATEPART(dw, tblc.WorkDate) = 7 THEN shift.SatShift END
				) AS ShiftId
			,(CASE 
				WHEN (
						empsum.IsLocal = 1
						OR empsum.IsExpatUseHoliday = 1
						)
					AND holiday.Id IS NOT NULL THEN 0
				WHEN DATEPART(dw, tblc.WorkDate) = 1
					AND shift.SunShift IS NULL THEN 1
				WHEN DATEPART(dw, tblc.WorkDate) = 2
					AND shift.MonShift IS NULL THEN 1
				WHEN DATEPART(dw, tblc.WorkDate) = 3
					AND shift.TueShift IS NULL THEN 1
				WHEN DATEPART(dw, tblc.WorkDate) = 4
					AND shift.WedShift IS NULL THEN 1
				WHEN DATEPART(dw, tblc.WorkDate) = 5
					AND shift.ThuShift IS NULL THEN 1
				WHEN DATEPART(dw, tblc.WorkDate) = 6
					AND shift.FriShift IS NULL THEN 1
				WHEN DATEPART(dw, tblc.WorkDate) = 7
					AND shift.SatShift IS NULL THEN 1
				ELSE 0 END
				) AS IsOffDay
			,0 AS IsFixedOffDay
			,(CASE 
				WHEN (
						empsum.IsLocal = 1
						OR empsum.IsExpatUseHoliday = 1
						)
					AND holiday.Id IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsHoliday
			,1 AS IsPublic
			,@ActorId
			,@ModifiedDate
		FROM dbo.EmployeeDefaultShift AS shift WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = shift.EmpID
			AND emp.EmpId = @EmployeeId
			INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON empsum.EmpId = emp.EmpId
			AND empsum.IsDefaultSchedule = 1
			,#tblDate AS tblc
			LEFT JOIN dbo.Holiday AS holiday WITH (NOLOCK) ON tblc.WorkDate BETWEEN holiday.FromDate
				AND holiday.ToDate
		WHERE (
				shift.SunShift IS NOT NULL
				OR shift.MonShift IS NOT NULL
				OR shift.TueShift IS NOT NULL
				OR shift.WedShift IS NOT NULL
				OR shift.ThuShift IS NOT NULL
				OR shift.FriShift IS NOT NULL
				OR shift.SatShift IS NOT NULL
				)
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeCalendar AS empca WITH (NOLOCK)
				WHERE emp.EmpId = empca.EmpId
					AND tblc.WorkDate = empca.WorkDate
				)
	END
	ELSE
	BEGIN
		UPDATE empca
		SET empca.ShiftId = (CASE 
				WHEN (
						empsum.IsLocal = 1
						OR empsum.IsExpatUseHoliday = 1
						)
					AND holiday.Id IS NOT NULL THEN NULL
				ELSE shift.ShiftId END
				)
			,empca.IsOffDay = 0
			,empca.IsFixedOffDay = 0
			,empca.IsHoliday = CASE 
				WHEN (
						empsum.IsLocal = 1
						OR empsum.IsExpatUseHoliday = 1
						)
					AND holiday.Id IS NOT NULL THEN 1
				ELSE 0
				END
			,empca.ModifiedBy = @ActorId
			,empca.ModifiedDate = @ModifiedDate
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON empca.EmpId = empsum.EmpId
			AND empsum.IsDefaultSchedule = 0
			CROSS APPLY (
			SELECT TOP 1 shift.ShiftId
			FROM dbo.ScheduleArrangementDefaultShift AS shift WITH (NOLOCK)
			WHERE shift.EmployeeId = empca.EmpId
				AND shift.ShiftId > 0
				AND shift.FromDate <= @FromDate
			ORDER BY shift.FromDate DESC
			) AS shift
			LEFT JOIN dbo.Holiday AS holiday WITH (NOLOCK) ON empca.WorkDate BETWEEN holiday.FromDate
				AND holiday.ToDate
		WHERE empca.EmpId = @EmployeeId
			AND empca.WorkDate >= @FromDate
			AND empca.IsLeaveDay = 0
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.ChangeShiftRequest AS csr WITH (NOLOCK)
					INNER JOIN dbo.ChangeShiftRequestDetail AS csrdt WITH (NOLOCK) ON csr.RequestId = csrdt.RequestId
					AND csrdt.OnDate = empca.WorkDate
					INNER JOIN dbo.WorkingTimeRequest AS wkr WITH (NOLOCK) ON csr.RequestId = wkr.RequestId
					AND wkr.CurrentStatus IN (@VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId)
				WHERE empca.EmpId = csr.EmpId
				)
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
					INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON wkr.RequestId = lrd.RequestId
					AND empca.EmpId = lrd.EmpId
					AND empca.WorkDate = lrd.OnDate
					AND lrd.WorkType = @OffDay
				WHERE wkr.CurrentStatus IN (@VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId)
				)
	END

	EXEC dbo.ScheduleArrangement_UpdateOffDayReplacement @ActorId = - 1
		,@EmployeeIds = @EmployeeId
		,@FromDate = @FromDate
		,@ToDate = @ToDate1
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Update Unsuccessfully!'
	END

	DROP TABLE #tblDate
END
