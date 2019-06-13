
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
		-- request status
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@NotVerifiedStatusId TINYINT = 3
		,@ApprovedStatusId TINYINT = 4
		,@RejectedStatusId TINYINT = 5
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7

	CREATE TABLE #tblCalendar (WorkDate DATE)

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
		UPDATE ec
		SET ec.ShiftId = CASE 
				WHEN es.IsLocal = 0
					OR (
						es.IsLocal = 1
						AND hld.Id IS NULL
						) THEN CASE 
						WHEN DATEPART(dw, ec.WorkDate) = 1 THEN SunShift
						WHEN DATEPART(dw, ec.WorkDate) = 2 THEN MonShift
						WHEN DATEPART(dw, ec.WorkDate) = 3 THEN TueShift
						WHEN DATEPART(dw, ec.WorkDate) = 4 THEN WedShift
						WHEN DATEPART(dw, ec.WorkDate) = 5 THEN ThuShift
						WHEN DATEPART(dw, ec.WorkDate) = 6 THEN FriShift
						WHEN DATEPART(dw, ec.WorkDate) = 7 THEN SatShift
						END
				ELSE ec.ShiftId
				END
			,ec.IsOffDay = CASE 
				WHEN es.IsLocal = 1
					AND hld.Id IS NOT NULL THEN 0
				WHEN DATEPART(dw, ec.WorkDate) = 1
					AND SunShift IS NULL THEN 1
				WHEN DATEPART(dw, ec.WorkDate) = 2
					AND MonShift IS NULL THEN 1
				WHEN DATEPART(dw, ec.WorkDate) = 3
					AND TueShift IS NULL THEN 1
				WHEN DATEPART(dw, ec.WorkDate) = 4
					AND WedShift IS NULL THEN 1
				WHEN DATEPART(dw, ec.WorkDate) = 5
					AND ThuShift IS NULL THEN 1
				WHEN DATEPART(dw, ec.WorkDate) = 6
					AND FriShift IS NULL THEN 1
				WHEN DATEPART(dw, ec.WorkDate) = 7
					AND SatShift IS NULL THEN 1
				ELSE 0
				END
			,ec.IsHoliday = CASE 
				WHEN es.IsLocal = 1
					AND hld.Id IS NOT NULL THEN 1
				ELSE 0
				END
			,ec.ModifiedBy = @ActorId
			,ec.ModifiedDate = @ModifiedDate
		FROM dbo.EmployeeCalendar AS ec WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON ec.EmpId = es.EmpId
			AND es.IsDefaultSchedule = 1
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmpId = es.EmpId
			AND e.EmpId = @EmployeeId
			AND e.[Enabled] = 1
			AND e.isDeleted = 0
			LEFT JOIN dbo.EmployeeDefaultShift AS s WITH (NOLOCK) ON ec.EmpId = s.EmpID
			LEFT JOIN dbo.Holiday AS hld WITH (NOLOCK) ON ec.WorkDate BETWEEN hld.FromDate
				AND hld.ToDate
		WHERE ec.WorkDate >= @FromDate
			AND ec.IsLeaveDay = 0
			AND NOT EXISTS (
				SELECT 1
				FROM dbo.ChangeShiftRequest AS csr WITH (NOLOCK)
					INNER JOIN dbo.ChangeShiftRequestDetail AS csrdt WITH (NOLOCK) ON csr.RequestId = csrdt.RequestId
					AND csrdt.OnDate = ec.WorkDate
					INNER JOIN dbo.WorkingTimeRequest AS wkr WITH (NOLOCK) ON csr.RequestId = wkr.RequestId
					AND wkr.CurrentStatus IN (@VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId)
				WHERE ec.EmpId = csr.EmpId
				)

		WHILE @FromDate1 <= @ToDate1
		BEGIN
			INSERT INTO #tblCalendar (WorkDate)
			VALUES (@FromDate1)

			SET @FromDate1 = DATEADD(DAY, 1, @FromDate1)
		END

		INSERT INTO dbo.EmployeeCalendar (
			EmpId
			,WorkDate
			,ShiftId
			,IsOffDay
			,IsHoliday
			,IsPublic
			)
		SELECT DISTINCT emp.EmpId
			,tblc.WorkDate
			,(CASE 
				WHEN (
						empsum.IsLocal = 1
						OR (
							empsum.IsLocal = 0
							AND empsum.IsExpatUseHoliday = 1
							)
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
						OR (
							empsum.IsLocal = 0
							AND empsum.IsExpatUseHoliday = 1
							)
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
			,(CASE 
				WHEN (
						empsum.IsLocal = 1
						OR (
							empsum.IsLocal = 0
							AND empsum.IsExpatUseHoliday = 1
							)
						)
					AND holiday.Id IS NOT NULL THEN 1
				ELSE 0 END
				) AS IsHoliday
			,1
		FROM dbo.EmployeeDefaultShift AS shift WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = @EmployeeId
			AND emp.EmpId = shift.EmpID
			INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON empsum.EmpId = emp.EmpId
			AND empsum.IsDefaultSchedule = 1
			,#tblCalendar AS tblc
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

		EXEC dbo.ScheduleArrangement_UpdateOffDayReplacement @ActorId = - 1
			,@EmployeeId = @EmployeeId
			,@FromDate = @FromDate
			,@ToDate = @ToDate1
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT
	END

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Update Unsuccessfully!'
	END

	DROP TABLE #tblCalendar
END
