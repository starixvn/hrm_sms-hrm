CREATE PROCEDURE [dbo].[tmpEmployeeCalendar_UpdateOff] @fromDate DATE
	,@toDate DATE
	,@empId INT = NULL
AS
/*
Created: Teddy.le@2020-08-12
Task: update off for tmpEmployeeCalendar
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE  @TableTempDate DATE
			,@TempFromDate DATE
			,@TempToDate DATE
			,@SetDayOff DATE
			,@OffDaysbyWeek TINYINT
			,@CountDaysOff TINYINT
			,@CountWeeken TINYINT
			,@OffDayLeft TINYINT
			,@I TINYINT = 0
			,@CountDayDiff INT
			,@StartDay DATE
			,@CountDayWorked INT


	CREATE TABLE #updateOffDay (
		WorkDate DATE
		)
	CREATE TABLE #tempWeekDoNotHaveData (
		WorkDate DATE
		)
	CREATE TABLE #dayOffDefaultPerWeek (
		WorkDate DATE
		,DayName VARCHAR(9)
		)
	
	INSERT INTO #dayOffDefaultPerWeek( WorkDate , DayName)	
			SELECT  tmpEmp.WorkDate , DATENAME(DW, tmpEmp.WorkDate)
			FROM	tmpEmployeeCalendar tmpEmp
			WHERE   tmpEmp.EmpId = @empId AND tmpEmp.WorkDate BETWEEN @fromDate
			AND DATEADD(DAY, 6, @fromDate) AND tmpEmp.IsOffDay = 1 
			AND tmpEmp.ShiftId IS NULL
	
	SELECT @CountWeeken = SUM(CASE  WHEN DayName ='Sunday'	 THEN 2
									WHEN DayName ='Saturday' THEN 1
									ELSE 0
							  END)
	FROM #dayOffDefaultPerWeek

	SELECT @CountDaysOff = COUNT(WorkDate) FROM #dayOffDefaultPerWeek

	SELECT  @OffDaysbyWeek = empsum.OffDaysbyWeek
			FROM EmployeeSummary empsum  WITH (NOLOCK) 
			WHERE empsum.EmpId = @empId 
	SET @CountDayDiff = DATEDIFF(DAY,GETDATE(),@fromDate)
	IF( @CountDayDiff < 0)
	BEGIN 
		SET @StartDay = GETDATE()
	END
	ELSE
	BEGIN 
		SET @StartDay = @fromDate
	END
	IF ( @OffDaysbyWeek > @CountDaysOff )
	BEGIN
	SET @OffDayLeft = @OffDaysbyWeek - @CountDaysOff
		WHILE @StartDay < @toDate
		BEGIN
			SELECT @CountDayWorked = COUNT(empatt.EmpId)
			FROM  EmployeeAttendance AS empatt  WITH (NOLOCK)
			WHERE empatt.AttendanceDate BETWEEN @StartDay AND DATEADD(DAY, 6, @StartDay)
			AND empatt.EmpId = @EmpId
			AND empatt.ShiftId IS NOT NULL 
			AND empatt.InFixed IS NOT NULL 
			AND empatt.OutFixed IS NOT NULL 

			IF(@CountDayWorked = 0)
			BEGIN
				INSERT INTO #tempWeekDoNotHaveData(WorkDate)
				SELECT @StartDay
			END
			SET @StartDay = DATEADD(DAY, 7, @StartDay)
		END
		WHILE EXISTS (SELECT * FROM #tempWeekDoNotHaveData)
		BEGIN
			SELECT TOP 1 @TableTempDate = WorkDate
			FROM #tempWeekDoNotHaveData
			SET @TempFromDate = @TableTempDate
			IF(DATEDIFF(DAY, @TempFromDate, @toDate) >= 6 )
			BEGIN
				SET @TempToDate = DATEADD(DAY, 6, @TableTempDate)
			END				
			ELSE
			BEGIN
				SET @TempToDate = @toDate
			END
			WHILE @TempFromDate <= @TempToDate
			BEGIN	
				IF(DATEPART(dw, @TempFromDate) = 1)
				BEGIN
					WHILE @I < @OffDayLeft
					BEGIN	
						IF(@CountWeeken = 3)
						BEGIN
							SET @SetDayOff = DATEADD(DAY, - (@I +2), @TempFromDate )	
						END
						ELSE IF(@CountWeeken = 2)
						BEGIN	
							SET @SetDayOff = DATEADD(DAY, - (@I + 1), @TempFromDate )
						END
						ELSE IF(@CountWeeken = 1)
						BEGIN	
							 SET @SetDayOff = DATEADD(DAY, - (@I), @TempFromDate )
							 IF(@I >= 1)
							 BEGIN
							 SET @SetDayOff = DATEADD(DAY, - (@I +1), @TempFromDate )
							 END
						END
						ELSE
						BEGIN
							 SET @SetDayOff = DATEADD(DAY, - (@I), @TempFromDate )
						END
						SET @I = @I +1
						INSERT INTO #updateOffDay (WorkDate)
							   SELECT @SetDayOff
					END
					SET @I = 0
				END
				SET @TempFromDate = DATEADD(DAY, 1, @TempFromDate)
			END		

			DELETE #tempWeekDoNotHaveData
			WHERE WorkDate = @TableTempDate	
		END
		UPDATE dbo.tmpEmployeeCalendar WITH (ROWLOCK, UPDLOCK)
						SET IsOffDay = 1, ShiftId = NULL
						WHERE EmpId = @empId AND
							  WorkDate IN ( SELECT WorkDate  
											FROM #updateOffDay  )
		DROP TABLE #dayOffDefaultPerWeek
		DROP TABLE #updateOffDay
		DROP TABLE #tempWeekDoNotHaveData
		END
	END