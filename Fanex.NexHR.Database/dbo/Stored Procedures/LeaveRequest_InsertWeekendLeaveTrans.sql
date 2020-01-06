
CREATE PROCEDURE [dbo].[LeaveRequest_InsertWeekendLeaveTrans] @RequestId BIGINT
AS
/*
Created: 20190411@Quinn.Vo
Task: Insert weekend leave transaction.
DB: HRM
Original: 

Revisions: 
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @AnnualLeaveType INT = 2
		,@UpaidLeaveType INT = 25
		,@KIVLeaveType INT = 10
		,@GeneralLeaveType INT = 0
		,@LeaveWorkType TINYINT = 2
		,@DisciplineReachedTo INT = 16
	DECLARE @EmpId INT
		,@LeaveTypeId INT
		,@TotalDays NUMERIC(5, 1)
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@tmpDate DATETIME
		,@TotalWeekendLeaveDay NUMERIC(5, 1)
		,@WeekendLeaveDayTaken NUMERIC(5, 1)
		,@Index INT
		,@TmpWeekendLeaveDayTaken NUMERIC(5, 1)

	CREATE TABLE #WorkingCalendar (WorkDate DATETIME)

	CREATE TABLE #WeekendLeave (
		Id INT PRIMARY KEY IDENTITY(1, 1)
		,WorkDate DATETIME
		,LeaveTypeId TINYINT
		,LeaveDay NUMERIC(5, 1)
		,IsDiscipline BIT
		)

	SELECT @EmpId = rq.EmpId
		,@FromDate = lr.FromDate
		,@ToDate = lr.ToDate
		,@LeaveTypeId = lr.LeaveType
		,@TotalDays = lr.TotalDays
	FROM dbo.WorkingTimeRequest rq WITH (NOLOCK)
		INNER JOIN dbo.LeaveRequest lr WITH (NOLOCK) ON rq.RequestId = lr.RequestId
	WHERE rq.RequestID = @RequestId

	IF EXISTS (
			SELECT 1
			FROM dbo.WeekendLeaveTrans WITH (NOLOCK)
			WHERE Requestid = @RequestId
			)
	BEGIN
		DELETE
		FROM dbo.WeekendLeaveTrans
		WHERE Requestid = @RequestId
	END

	IF @LeaveTypeId = @GeneralLeaveType
		OR @TotalDays = 0.5
	BEGIN
		SET @tmpDate = @FromDate

		INSERT INTO #WeekendLeave (
			WorkDate
			,LeaveTypeId
			,LeaveDay
			)
		SELECT OnDate
			,LeaveType
			,CASE 
				WHEN IsHalfDay = 1 THEN 0.5
				ELSE 1
				END
		FROM dbo.LeaveRequestDetail WITH (NOLOCK)
		WHERE RequestId = @Requestid
			AND WorkType = @LeaveWorkType
			AND (
				LeaveType = @AnnualLeaveType
				OR LeaveType = @UpaidLeaveType
				OR LeaveType = @KIVLeaveType
				)
			AND (
				DATENAME(DW, OnDate) = 'Saturday'
				OR DATENAME(DW, OnDate) = 'Sunday'
				)
		ORDER BY OnDate
	END
	ELSE IF @LeaveTypeId = @AnnualLeaveType
		OR @LeaveTypeId = @UpaidLeaveType
		OR @LeaveTypeId = @KIVLeaveType
	BEGIN
		SET @tmpDate = @FromDate

		WHILE DATEDIFF(DD, @tmpDate, @ToDate) >= 0
		BEGIN
			IF DATENAME(DW, @tmpDate) = 'Saturday'
				OR DATENAME(DW, @tmpDate) = 'Sunday'
			BEGIN
				INSERT INTO #WorkingCalendar (WorkDate)
				VALUES (@tmpDate)
			END

			SET @tmpDate = DATEADD(DD, 1, @tmpDate)
		END

		INSERT INTO #WeekendLeave (
			WorkDate
			,LeaveTypeId
			,LeaveDay
			)
		SELECT cl.WorkDate
			,@LeaveTypeId
			,1
		FROM #WorkingCalendar AS tcl
			LEFT JOIN dbo.EmployeeCalendar AS cl WITH (NOLOCK) ON EmpId = @EmpId
			AND tcl.WorkDate = cl.WorkDate
		WHERE cl.EmpId IS NULL
			OR (
				cl.EmpId IS NOT NULL
				AND IsHoliday = 0
				AND IsOffDay = 0
				)
		ORDER BY tcl.WorkDate
	END

	IF EXISTS (
			SELECT 1
			FROM #WeekendLeave
			)
	BEGIN
		SELECT @TotalWeekendLeaveDay = SUM(LeaveDay)
		FROM #WeekendLeave

		SET @WeekendLeaveDayTaken = dbo.fnc_GetWeekendLeaveTaken(@EmpId, @FromDate, NULL)

		IF @WeekendLeaveDayTaken >= @DisciplineReachedTo
		BEGIN
			UPDATE #WeekendLeave
			SET IsDiscipline = 1
		END
		ELSE IF @WeekendLeaveDayTaken + @TotalWeekendLeaveDay > @DisciplineReachedTo
		BEGIN
			SELECT @Index = 1
				,@TmpWeekendLeaveDayTaken = @WeekendLeaveDayTaken

			WHILE EXISTS (
					SELECT 1
					FROM #WeekendLeave
					WHERE Id = @Index
					)
			BEGIN
				SELECT @TmpWeekendLeaveDayTaken = @TmpWeekendLeaveDayTaken + LeaveDay
				FROM #WeekendLeave
				WHERE Id = @Index

				IF @TmpWeekendLeaveDayTaken > @DisciplineReachedTo
				BEGIN
					UPDATE #WeekendLeave
					SET IsDiscipline = 1
					WHERE Id = @Index
				END
				ELSE
				BEGIN
					UPDATE #WeekendLeave
					SET IsDiscipline = 0
					WHERE Id = @Index
				END

				SET @Index = @Index + 1
			END
		END
		ELSE
		BEGIN
			UPDATE #WeekendLeave
			SET IsDiscipline = 0
		END

		INSERT INTO dbo.WeekendLeaveTrans (
			EmpId
			,WorkDate
			,LeaveTypeId
			,LeaveDay
			,RequestId
			,IsDiscipline
			,InsertedDate
			)
		SELECT @EmpId
			,WorkDate
			,LeaveTypeId
			,LeaveDay
			,@RequestId
			,IsDiscipline
			,GETDATE()
		FROM #WeekendLeave
	END

	DROP TABLE #WorkingCalendar

	DROP TABLE #WeekendLeave
END