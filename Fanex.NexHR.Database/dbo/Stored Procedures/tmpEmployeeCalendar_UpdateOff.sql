CREATE PROCEDURE [dbo].[tmpEmployeeCalendar_UpdateOff] @fromDate DATE
	,@toDate DATE
	,@empIds VARCHAR(MAX) = NULL
AS
/*
Created: Teddy.le@2020-08-12
Task: update off for tmpEmployeeCalenda
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
			,@CountdataInWeeek TINYINT
			,@OffDaysbyWeek TINYINT
			,@CountDaysOff TINYINT
			,@CountWeeken TINYINT
			,@OffDayLeft TINYINT
			,@I TINYINT = 0

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
			WHERE   tmpEmp.EmpId = @empIds AND tmpEmp.WorkDate BETWEEN @fromDate
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
			WHERE empsum.EmpId = @empIds 

	IF ( @OffDaysbyWeek > @CountDaysOff )
	BEGIN
	SET @OffDayLeft = @OffDaysbyWeek - @CountDaysOff
		WHILE @fromDate < @toDate
		BEGIN
			SELECT @CountdataInWeeek = COUNT(empca.EmpId)
			FROM  EmployeeCalendar empca
			WHERE empca.EmpId = @empIds
			AND empca.WorkDate BETWEEN @fromDate AND DATEADD(DAY, 6, @fromDate)

			IF(@CountdataInWeeek = 0)
			BEGIN
				INSERT INTO #tempWeekDoNotHaveData(WorkDate)
				SELECT @fromDate
			END
			SET @fromDate = DATEADD(DAY, 7, @fromDate)
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

			UPDATE dbo.tmpEmployeeCalendar WITH (ROWLOCK, UPDLOCK)
						SET IsOffDay = 1, ShiftId = NULL
						WHERE EmpId = @empIds AND
							  WorkDate IN ( SELECT WorkDate  
											FROM #updateOffDay  )

			DELETE #tempWeekDoNotHaveData
			WHERE WorkDate = @TableTempDate	
		END
		DROP TABLE #dayOffDefaultPerWeek
		DROP TABLE #updateOffDay
		DROP TABLE #tempWeekDoNotHaveData
	END
END