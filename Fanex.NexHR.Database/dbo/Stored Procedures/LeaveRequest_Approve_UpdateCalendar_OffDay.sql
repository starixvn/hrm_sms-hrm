CREATE PROCEDURE [dbo].[LeaveRequest_Approve_UpdateCalendar_OffDay] @RequestId BIGINT
	,@EmpId INT
	,@ProcessedBy INT
AS
/*
Created: Teddy.Le 2020-08-06
Task: update calendar when approved request greater than 7 days (UPL,UPLI,  Maternity Leave, Paternity Leave).
DB: HRM
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE 
		 @FromDate DATE
		,@ToDate DATE
		,@TableTempDate DATE
		,@FromDateNotModify DATE
		,@ToDateNotModify DATE
		,@ProcessedDate DATETIME = GETDATE()
		,@DayName VARCHAR(9)
		,@CountDaysOff TINYINT
		,@CountDaysOffNotModify TINYINT
		,@CountWeeken TINYINT 
		,@OffDaysbyWeek TINYINT
		,@OffDayLeft TINYINT
		,@I TINYINT = 0
		,@SetDayOff DATE;
	
	CREATE TABLE #updateOffDay (
		WorkDate DATE
		)
	CREATE TABLE #dayOfWeekNotModify (
		WorkDate DATE
		)

	CREATE TABLE #dayOffDefaultPerWeek (
		WorkDate DATE
		,DayName VARCHAR(9)
		)				

	SELECT @FromDate = FromDate
		  ,@ToDate = ToDate
	FROM dbo.LeaveRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.LeaveRequestDetail WITH (NOLOCK)
			WHERE RequestId = @RequestId
			) 
	BEGIN
		INSERT INTO #dayOffDefaultPerWeek( WorkDate , DayName)	
		SELECT  empca.WorkDate , DATENAME(DW, empca.WorkDate)
		FROM	EmployeeCalendar empca
		WHERE   empca.EmpId = @EmpId AND empca.WorkDate BETWEEN @FromDate
		AND DATEADD(DAY, 6, @fromdate) AND empca.IsOffDay = 1 
		AND empca.ShiftId IS NULL and empca.ModifiedBy IS NULL

		SELECT @OffDaysbyWeek = empsum.OffDaysbyWeek
				FROM EmployeeSummary empsum
				WHERE empsum.EmpId = @EmpId 
					
		SELECT @CountWeeken = SUM(CASE  WHEN DayName ='Sunday'	 THEN 2
										WHEN DayName ='Saturday' THEN 1
										ELSE 0
								  END)
								FROM #dayOffDefaultPerWeek

		SELECT @CountDaysOff = COUNT(WorkDate) FROM #dayOffDefaultPerWeek

		IF ( @OffDaysbyWeek > @CountDaysOff )
		BEGIN
			SET @OffDayLeft = @OffDaysbyWeek - @CountDaysOff
			WHILE @FromDate < @ToDate
			BEGIN
				SELECT @CountDaysOffNotModify = COUNT(empca.WorkDate)
				FROM EmployeeCalendar empca
				WHERE empca.EmpId = @EmpId AND empca.WorkDate BETWEEN @FromDate AND DATEADD(DAY, 6, @FromDate)
				AND empca.ModifiedBy IS NOT NULL	
					
				IF(@CountDaysOffNotModify = 0)
				BEGIN
					INSERT INTO #dayOfWeekNotModify(WorkDate)
					select @FromDate
				END
				SET @FromDate = DATEADD(DAY, 7, @FromDate)
			END
			WHILE EXISTS (SELECT * FROM #dayOfWeekNotModify)
				  BEGIN
					SELECT TOP 1 @TableTempDate = WorkDate
					FROM #dayOfWeekNotModify
						
					SET @FromDateNotModify = @TableTempDate
					IF(DATEDIFF(DAY, @FromDateNotModify, @ToDate) >= 6 )
						BEGIN
							SET @ToDateNotModify = DATEADD(DAY, 6, @TableTempDate)
						END
						ELSE
						BEGIN
							SET @ToDateNotModify = @ToDate
						END
						
					WHILE @FromDateNotModify <= @ToDateNotModify
						BEGIN
							SET @DayName = DATENAME(DW, @FromDateNotModify);
							IF( @DayName = 'Sunday')
							BEGIN
								WHILE @I < @OffDayLeft
								BEGIN	
								IF(@CountWeeken = 3)
								BEGIN
								   SET @SetDayOff = DATEADD(DAY, - (@I +2), @FromDateNotModify)	
								END
								ELSE IF(@CountWeeken = 2)
								BEGIN	
								   SET @SetDayOff = DATEADD(DAY, - (@I + 1), @FromDateNotModify)
								END
								ELSE IF(@CountWeeken = 1)
								BEGIN	
								   SET @SetDayOff = DATEADD(DAY, - (@I), @FromDateNotModify)
								   IF(@I >= 1)
								   BEGIN
									  SET @SetDayOff = DATEADD(DAY, - (@I +1), @FromDateNotModify)
								   END
								END
								ELSE
								   BEGIN
									  SET @SetDayOff = DATEADD(DAY, - (@I), @FromDateNotModify)
								   END
							   SET @I = @I +1
							   INSERT INTO #updateOffDay (WorkDate)
							   SELECT @SetDayOff		
						END
						SET @I = 0
					END				
				    SET @FromDateNotModify = DATEADD(DAY, 1, @FromDateNotModify)
			    END

				UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
						SET IsOffDay = 1, ShiftId = NULL, ModifiedDate = @ProcessedDate
						WHERE EmpId = @EmpId AND
							  WorkDate IN ( SELECT WorkDate  
											FROM #updateOffDay  ) 

				DELETE #dayOfWeekNotModify
				WHERE WorkDate = @TableTempDate	
				END
			END
			DROP TABLE #dayOffDefaultPerWeek
			DROP TABLE #updateOffDay
			DROP TABLE #dayOfWeekNotModify
	END
END