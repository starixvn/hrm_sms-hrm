
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

	DECLARE @ModifiedDate DATETIME = GETDATE()

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
	END

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 102
			,@ErrMsg = N'Update Unsuccessfully!'
	END
END
