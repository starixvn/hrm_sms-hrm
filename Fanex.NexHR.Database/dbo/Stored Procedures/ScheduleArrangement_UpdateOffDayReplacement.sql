
CREATE PROCEDURE [dbo].[ScheduleArrangement_UpdateOffDayReplacement] @ActorId INT
	,@FromDate DATE
	,@ToDate DATE
	,@EmployeeId INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-05-14
Task: update off day replacement
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @GeneralLeaveId TINYINT = 0
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
		--
		,@RowIndex INT = 1
		,@TotalRows INT
		,@FromDate1 DATE
		,@ToDate1 DATE
		,@WorkDate DATE
		,@PHDate DATE
		,@OvertimeDate DATE
		,@ReplacedDate DATE
		,@TotalOffDaysByWeek SMALLINT
		,@TotalClaimOffDays SMALLINT
		,@DefaultDateFirst TINYINT = @@datefirst

	CREATE TABLE #tblEmployee (
		RowIndex INT IDENTITY(1, 1)
		,EmployeeId INT
		,IsLocal BIT
		,OffDaysByWeek SMALLINT
		)

	CREATE TABLE #tblEmployeeWeekly (
		EmployeeId INT
		,FromDate DATE
		,ToDate DATE
		,OffDaysByWeek SMALLINT
		,TotalOffDaysUsed SMALLINT
		)

	CREATE TABLE #tblWeeklyHasHoliday (
		RowIndex INT IDENTITY(1, 1)
		,FromDate DATE
		,ToDate DATE
		)

	CREATE TABLE #tblDateRange (WorkDate DATE)

	CREATE TABLE #tblLeaveRequest (
		EmployeeId INT
		,WorkDate DATE
		,WorkType TINYINT
		,LeaveTypeId INT
		,TotalDays SMALLINT
		)

	CREATE TABLE #tblCalendar (
		EmployeeId INT
		,WorkDate DATE
		,ShiftId INT
		,IsChangeShift BIT
		,IsLeaveOffDay BIT
		,IsOvertime BIT
		,IsHoliday BIT
		,IsOffDay BIT
		,IsKeepInView BIT
		,IsLeaveDay BIT
		,LeaveTypeId INT
		,LeaveTypeId2 INT
		,LeaveDay DECIMAL(18, 1)
		)

	CREATE TABLE #tblResetOffDay (
		EmployeeId INT
		,WorkDate DATE
		)

	CREATE TABLE #tblOffDayReplacement (
		EmployeeId INT
		,WorkDate DATE
		,ReplaceForDate DATE
		)

	SET DATEFIRST 1 -- monday

	INSERT INTO #tblWeeklyHasHoliday (FromDate)
	SELECT DISTINCT DATEADD(DAY, - 1 * (6 + DATEPART(dw, FromDate)) % 7, FromDate)
	FROM dbo.Holiday WITH (NOLOCK)
	WHERE FromDate BETWEEN DATEADD(DAY, - 14, @FromDate)
			AND @ToDate

	UPDATE #tblWeeklyHasHoliday
	SET ToDate = DATEADD(DAY, 6, FromDate)

	SET DATEFIRST @DefaultDateFirst

	SELECT @RowIndex = 1
		,@TotalRows = COUNT(1)
	FROM #tblWeeklyHasHoliday

	IF @TotalRows = 0 RETURN

	WHILE @RowIndex <= @TotalRows
	BEGIN
		SELECT @FromDate1 = FromDate
			,@ToDate1 = DATEADD(DAY, 14, ToDate)
		FROM #tblWeeklyHasHoliday
		WHERE RowIndex = @RowIndex

		WHILE (@FromDate1 <= @ToDate1)
		BEGIN
			IF NOT EXISTS (
					SELECT 1
					FROM #tblDateRange
					WHERE WorkDate = @FromDate1
					)
			BEGIN
				INSERT INTO #tblDateRange (WorkDate)
				VALUES (@FromDate1)
			END

			SET @FromDate1 = DATEADD(DAY, 1, @FromDate1)
		END

		SET @RowIndex = @RowIndex + 1
	END

	INSERT INTO #tblEmployee (
		EmployeeId
		,IsLocal
		,OffDaysByWeek
		)
	SELECT DISTINCT emp.EmpId
		,empsum.IsLocal
		,empsum.OffDaysbyWeek
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		AND empsum.OffDaysbyWeek > 0
	WHERE (
			@EmployeeId IS NULL
			OR emp.EmpId = @EmployeeId
			)
		AND emp.[Enabled] = 1
		AND emp.isDeleted = 0
		AND (
			empsum.IsLocal = 1
			OR (
				empsum.IsLocal = 0
				AND empsum.IsExpatUseHoliday = 1
				)
			)

	INSERT INTO #tblEmployeeWeekly (
		EmployeeId
		,FromDate
		,ToDate
		,OffDaysByWeek
		)
	SELECT emp.EmployeeId
		,holiday.FromDate
		,holiday.ToDate
		,emp.OffDaysByWeek
	FROM #tblEmployee AS emp
		,#tblWeeklyHasHoliday AS holiday

	INSERT INTO #tblLeaveRequest (
		EmployeeId
		,WorkDate
		,WorkType
		,LeaveTypeId
		,TotalDays
		)
	SELECT emp.EmployeeId
		,daterange.WorkDate
		,lrd.LeaveType
		,lrd.WorkType
		,(CASE lrd.IsHalfDay
			WHEN 1 THEN 0.5
			ELSE 1 END
			) AS TotalDays
	FROM #tblEmployee AS emp
		INNER JOIN dbo.WorkingTimeRequest AS wkr WITH (NOLOCK) ON emp.EmployeeId = wkr.EmpId
		AND wkr.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
		INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON wkr.RequestId = lrd.RequestId
		AND lrd.WorkType IN (@OffDay, @LeaveDay)
		INNER JOIN #tblDateRange AS daterange ON lrd.OnDate = daterange.WorkDate
		INNER JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON emp.EmployeeId = empca.EmpId
		AND lrd.OnDate = empca.WorkDate
	
	UNION
	SELECT emp.EmployeeId
		,daterange.WorkDate
		,lr.LeaveType
		,@LeaveDay AS WorkType
		,1 AS TotalDays
	FROM #tblEmployee AS emp
		INNER JOIN dbo.WorkingTimeRequest AS wkr WITH (NOLOCK) ON emp.EmployeeId = wkr.EmpId
		AND wkr.CurrentStatus NOT IN (@NotVerifiedStatusId, @RejectedStatusId, @VoidStatusId)
		INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON wkr.RequestId = lr.RequestId
		AND lr.LeaveType <> @GeneralLeaveId
		INNER JOIN #tblDateRange AS daterange ON daterange.WorkDate BETWEEN lr.FromDate
			AND lr.ToDate
		INNER JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON emp.EmployeeId = empca.EmpId
		AND daterange.WorkDate = empca.WorkDate
		AND empca.IsOffDay <> 1
		AND empca.IsHoliday <> 1
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.LeaveRequestDetail AS lrd WITH (NOLOCK)
			WHERE lr.RequestId = lrd.RequestId
			)

	INSERT INTO #tblCalendar (
		EmployeeId
		,WorkDate
		,ShiftId
		,IsChangeShift
		,IsLeaveOffDay
		,IsOvertime
		,IsHoliday
		,IsOffDay
		,IsKeepInView
		,IsLeaveDay
		,LeaveTypeId
		,LeaveTypeId2
		,LeaveDay
		)
	SELECT emp.EmployeeId
		,daterange.WorkDate
		,empca.ShiftId
		,(CASE 
			WHEN changeshift.EmpId IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsChangeShift
		,(CASE 
			WHEN lr.WorkDate IS NOT NULL
				AND lr.WorkType = @OffDay THEN 1
			ELSE 0 END
			) AS IsLeaveOffday
		,(CASE 
			WHEN ot.[Status] IN (@VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId) THEN 1
			ELSE 0 END
			) AS IsOvertime
		,(CASE 
			WHEN holiday.Id IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsHoliday
		,(CASE 
			WHEN empca.IsOffDay IS NULL THEN 0
			ELSE empca.IsOffDay END
			) AS IsOffDay
		,(CASE 
			WHEN empca.IsKeepInView IS NULL THEN 0
			ELSE empca.IsKeepInView END
			) AS IsKeepInView
		,(CASE 
			WHEN lr.WorkDate IS NOT NULL THEN 1
			ELSE 0 END
			) AS IsLeaveDay
		,lr.LeaveTypeId
		,empca.LeaveTypeId2
		,(CASE 
			WHEN ISNULL(empca.LeaveDay, 0) = 0 THEN ISNULL(lr.TotalDays, 0)
			ELSE empca.LeaveDay END
			) AS LeaveDay
	FROM #tblEmployee AS emp
		CROSS JOIN #tblDateRange AS daterange
		LEFT JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON emp.EmployeeId = empca.EmpId
		AND daterange.WorkDate = empca.WorkDate
		LEFT JOIN dbo.OTDailyRequest AS ot WITH (NOLOCK) ON emp.EmployeeId = ot.EmpId
		AND daterange.WorkDate = ot.OvertimeDate
		AND emp.IsLocal = 1
		LEFT JOIN dbo.Holiday AS holiday WITH (NOLOCK) ON daterange.WorkDate BETWEEN holiday.FromDate
			AND holiday.ToDate
		LEFT JOIN #tblLeaveRequest AS lr ON emp.EmployeeId = lr.EmployeeId
		AND lr.WorkDate = daterange.WorkDate
		OUTER APPLY (
		SELECT csr.EmpId
		FROM dbo.ChangeShiftRequest AS csr WITH (NOLOCK)
			INNER JOIN dbo.ChangeShiftRequestDetail AS csrdt WITH (NOLOCK) ON csr.RequestId = csrdt.RequestId
			AND daterange.WorkDate = csrdt.OnDate
			INNER JOIN dbo.WorkingTimeRequest AS wkr WITH (NOLOCK) ON csr.RequestId = wkr.RequestId
			AND wkr.CurrentStatus IN (@VerifiedStatusId, @ApprovedStatusId, @ProcessedStatusId)
		WHERE emp.EmployeeId = csr.EmpId
		) AS changeshift
	ORDER BY emp.EmployeeId
		,daterange.WorkDate

	SELECT @RowIndex = 1
		,@TotalRows = COUNT(1)
	FROM #tblWeeklyHasHoliday

	WHILE @RowIndex <= @TotalRows
	BEGIN
		SELECT @FromDate = FromDate
			,@ToDate = ToDate
		FROM #tblWeeklyHasHoliday
		WHERE RowIndex = @RowIndex

		UPDATE emp
		SET emp.TotalOffDaysUsed = empca.TotalOffDaysUsed
		FROM #tblEmployeeWeekly AS emp
			CROSS APPLY (
			SELECT COUNT(1) AS TotalOffDaysUsed
			FROM #tblCalendar AS empca
			WHERE empca.EmployeeId = emp.EmployeeId
				AND empca.WorkDate BETWEEN emp.FromDate
					AND emp.ToDate
				AND empca.IsHoliday = 0
				AND empca.IsChangeShift = 0
				AND empca.IsKeepInView = 0
				AND (
					empca.IsLeaveOffDay = 1
					OR (
						empca.IsOffDay = 1
						AND (
							(
								empca.IsOvertime = 0
								AND empca.ShiftId IS NULL
								)
							OR empca.IsOvertime = 1
							)
						AND NOT EXISTS (
							SELECT 1
							FROM #tblOffDayReplacement AS offrep
							WHERE emp.EmployeeId = offrep.EmployeeId
								AND empca.WorkDate = offrep.WorkDate
							)
						AND NOT EXISTS (
							SELECT 1
							FROM dbo.ScheduleArrangementOffDayReplacement AS offrep
							WHERE emp.EmployeeId = offrep.EmployeeId
								AND empca.WorkDate = offrep.WorkDate
							)
						)
					)
			) AS empca
		WHERE emp.FromDate = @FromDate
			AND emp.ToDate = @ToDate

		UPDATE empca
		SET empca.IsOffDay = 0
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
			INNER JOIN #tblEmployeeWeekly AS emp ON empca.EmpId = emp.EmployeeId
			AND emp.FromDate = @FromDate
			AND emp.ToDate = @ToDate
		WHERE empca.IsOffDay = 1
			AND EXISTS (
				SELECT 1
				FROM dbo.ScheduleArrangementOffDayReplacement AS offrep WITH (NOLOCK)
				WHERE empca.EmpId = offrep.EmployeeId
					AND empca.WorkDate = offrep.WorkDate
					AND offrep.ReplaceForDate BETWEEN emp.FromDate
						AND emp.ToDate
				)

		UPDATE empca
		SET empca.IsOffDay = 0
		FROM #tblCalendar AS empca
			INNER JOIN #tblEmployeeWeekly AS emp ON empca.EmployeeId = emp.EmployeeId
			AND emp.FromDate = @FromDate
			AND emp.ToDate = @ToDate
		WHERE empca.IsOffDay = 1
			AND EXISTS (
				SELECT 1
				FROM dbo.ScheduleArrangementOffDayReplacement AS offrep WITH (NOLOCK)
				WHERE empca.EmployeeId = offrep.EmployeeId
					AND empca.WorkDate = offrep.WorkDate
					AND offrep.ReplaceForDate BETWEEN emp.FromDate
						AND emp.ToDate
				)

		DELETE
		FROM #tblEmployeeWeekly
		WHERE TotalOffDaysUsed >= OffDaysByWeek

		WHILE EXISTS (
				SELECT 1
				FROM #tblEmployeeWeekly
				WHERE FromDate = @FromDate
					AND ToDate = @ToDate
				)
		BEGIN
			SELECT TOP 1 @EmployeeId = EmployeeId
				,@TotalOffDaysByWeek = OffDaysByWeek
				,@TotalClaimOffDays = OffDaysByWeek - TotalOffDaysUsed
			FROM #tblEmployeeWeekly
			WHERE FromDate = @FromDate
				AND ToDate = @ToDate

			WHILE @TotalClaimOffDays > 0
			BEGIN
				SELECT @PHDate = MIN(daterange.WorkDate)
				FROM #tblDateRange AS daterange
				WHERE daterange.WorkDate BETWEEN @FromDate
						AND @ToDate
					AND EXISTS (
						SELECT 1
						FROM dbo.Holiday WITH (NOLOCK)
						WHERE daterange.WorkDate BETWEEN FromDate
								AND ToDate
						)
					AND NOT EXISTS (
						SELECT 1
						FROM #tblOffDayReplacement
						WHERE EmployeeId = @EmployeeId
							AND ReplaceForDate = daterange.WorkDate
						)

				IF @PHDate IS NULL
					BREAK

				-- off day OT
				SELECT @WorkDate = MIN(WorkDate)
				FROM #tblCalendar AS empca
				WHERE EmployeeId = @EmployeeId
					AND WorkDate BETWEEN @FromDate
						AND @ToDate
					AND IsHoliday = 0
					AND ShiftId IS NOT NULL
					AND IsChangeShift = 0
					AND IsKeepInView = 0
					AND IsOvertime = 0
					AND IsLeaveDay = 0
					AND IsLeaveOffDay = 0
					AND IsOffDay = 1
					AND NOT EXISTS (
						SELECT 1
						FROM #tblResetOffDay
						WHERE empca.EmployeeId = EmployeeId
							AND empca.WorkDate = WorkDate
						)
					AND NOT EXISTS (
						SELECT 1
						FROM #tblOffDayReplacement AS offr
						WHERE empca.EmployeeId = offr.EmployeeId
							AND empca.WorkDate = offr.WorkDate
							AND offr.ReplaceForDate < @FromDate
						)

				IF @WorkDate IS NOT NULL
				BEGIN
					SELECT @ReplacedDate = MIN(WorkDate)
					FROM #tblCalendar
					WHERE EmployeeId = @EmployeeId
						AND WorkDate > @PHDate
						AND WorkDate <= @ToDate
						AND IsHoliday = 0
						AND IsChangeShift = 0
						AND IsKeepInView = 0
						AND IsOvertime = 0
						AND IsLeaveDay = 0
						AND IsLeaveOffDay = 0
						AND (
							IsOffDay = 0
							OR WorkDate = @WorkDate
							)

					IF @ReplacedDate IS NOT NULL
					BEGIN
						INSERT INTO #tblOffDayReplacement (
							EmployeeId
							,WorkDate
							,ReplaceForDate
							)
						VALUES (
							@EmployeeId
							,@ReplacedDate
							,@PHDate
							)

						INSERT INTO #tblResetOffDay (
							EmployeeId
							,WorkDate
							)
						VALUES (
							@EmployeeId
							,@WorkDate
							)

						DELETE
						FROM #tblCalendar
						WHERE EmployeeId = @EmployeeId
							AND WorkDate = @ReplacedDate
					END

					SET @TotalClaimOffDays = @TotalClaimOffDays - 1

					CONTINUE
				END

				--
				SELECT @ReplacedDate = MIN(WorkDate)
				FROM #tblCalendar
				WHERE EmployeeId = @EmployeeId
					AND WorkDate > @PHDate
					AND WorkDate <= @ToDate
					AND IsHoliday = 0
					AND IsChangeShift = 0
					AND IsKeepInView = 0
					AND IsOvertime = 0
					AND IsLeaveDay = 0
					AND IsLeaveOffDay = 0
					AND IsOffDay = 0

				IF @ReplacedDate IS NOT NULL
				BEGIN
					INSERT INTO #tblOffDayReplacement (
						EmployeeId
						,WorkDate
						,ReplaceForDate
						)
					VALUES (
						@EmployeeId
						,@ReplacedDate
						,@PHDate
						)

					DELETE
					FROM #tblCalendar
					WHERE EmployeeId = @EmployeeId
						AND WorkDate = @ReplacedDate

					SET @TotalClaimOffDays = @TotalClaimOffDays - 1

					CONTINUE
				END

				-- next week, next week + 1
				SELECT @FromDate1 = DATEADD(DAY, 1, @ToDate)

				WHILE @FromDate1 < DATEADD(DAY, 14, @ToDate)
				BEGIN
					SET @ToDate1 = DATEADD(DAY, 6, @FromDate1)

					-- off day next to PH
					SELECT @WorkDate = MIN(WorkDate)
					FROM #tblCalendar AS empca
					WHERE EmployeeId = @EmployeeId
						AND WorkDate BETWEEN @FromDate1
							AND @ToDate1
						AND IsHoliday = 0
						AND ShiftId IS NULL
						AND IsChangeShift = 0
						AND IsKeepInView = 0
						AND IsOvertime = 0
						AND IsLeaveDay = 0
						AND (
							IsLeaveOffDay = 1
							OR IsOffDay = 1
							)
						AND NOT EXISTS (
							SELECT 1
							FROM #tblOffDayReplacement AS offr
							WHERE empca.EmployeeId = offr.EmployeeId
								AND empca.WorkDate = offr.WorkDate
								AND offr.ReplaceForDate < @FromDate1
							)

					IF @WorkDate IS NOT NULL
						AND NOT EXISTS (
							SELECT 1
							FROM #tblCalendar AS empca
							WHERE EmployeeId = @EmployeeId
								AND WorkDate BETWEEN @FromDate1
									AND @WorkDate
								AND IsHoliday = 0
								AND IsChangeShift = 0
								AND IsKeepInView = 0
								AND IsOvertime = 0
								AND IsLeaveDay = 0
								AND IsLeaveOffDay = 0
								AND IsOffDay = 0
								AND NOT EXISTS (
									SELECT 1
									FROM #tblOffDayReplacement AS offr
									WHERE empca.EmployeeId = offr.EmployeeId
										AND empca.WorkDate = offr.WorkDate
										AND offr.ReplaceForDate < @FromDate1
									)
							)
					BEGIN
						INSERT INTO #tblOffDayReplacement (
							EmployeeId
							,WorkDate
							,ReplaceForDate
							)
						VALUES (
							@EmployeeId
							,@WorkDate
							,@PHDate
							)

						DELETE
						FROM #tblCalendar
						WHERE EmployeeId = @EmployeeId
							AND WorkDate = @WorkDate

						BREAK
					END

					-- off day OT
					SELECT @WorkDate = MIN(WorkDate)
					FROM #tblCalendar AS empca
					WHERE EmployeeId = @EmployeeId
						AND WorkDate BETWEEN @FromDate1
							AND @ToDate1
						AND IsHoliday = 0
						AND ShiftId IS NOT NULL
						AND IsChangeShift = 0
						AND IsKeepInView = 0
						AND IsOvertime = 0
						AND IsLeaveDay = 0
						AND IsLeaveOffDay = 0
						AND IsOffDay = 1
						AND NOT EXISTS (
							SELECT 1
							FROM #tblResetOffDay
							WHERE empca.EmployeeId = EmployeeId
								AND empca.WorkDate = WorkDate
							)
						AND NOT EXISTS (
							SELECT 1
							FROM #tblOffDayReplacement AS offr
							WHERE empca.EmployeeId = offr.EmployeeId
								AND empca.WorkDate = offr.WorkDate
								AND offr.ReplaceForDate < @FromDate1
							)

					IF @WorkDate IS NOT NULL
					BEGIN
						SELECT @ReplacedDate = MIN(WorkDate)
						FROM #tblCalendar
						WHERE EmployeeId = @EmployeeId
							AND WorkDate BETWEEN @FromDate1
								AND @ToDate1
							AND IsHoliday = 0
							AND IsChangeShift = 0
							AND IsKeepInView = 0
							AND IsOvertime = 0
							AND IsLeaveDay = 0
							AND IsLeaveOffDay = 0
							AND (
								IsOffDay = 0
								OR WorkDate = @WorkDate
								)

						INSERT INTO #tblOffDayReplacement (
							EmployeeId
							,WorkDate
							,ReplaceForDate
							)
						VALUES (
							@EmployeeId
							,@ReplacedDate
							,@PHDate
							)

						INSERT INTO #tblResetOffDay (
							EmployeeId
							,WorkDate
							)
						VALUES (
							@EmployeeId
							,@WorkDate
							)

						DELETE
						FROM #tblCalendar
						WHERE EmployeeId = @EmployeeId
							AND WorkDate = @ReplacedDate

						BREAK
					END

					--
					IF EXISTS (
							SELECT 1
							FROM #tblCalendar
							WHERE EmployeeId = @EmployeeId
								AND WorkDate BETWEEN @FromDate1
									AND @ToDate1
								AND IsHoliday = 0
								AND ShiftId IS NULL
								AND IsChangeShift = 0
								AND IsKeepInView = 0
								AND IsOvertime = 0
								AND IsLeaveDay = 0
								AND (
									IsLeaveOffDay = 1
									OR IsOffDay = 1
									)
							GROUP BY EmployeeId
							HAVING COUNT(1) > @TotalOffDaysByWeek
							)
					BEGIN
						SELECT @ReplacedDate = MIN(WorkDate)
						FROM #tblCalendar
						WHERE EmployeeId = @EmployeeId
							AND WorkDate BETWEEN @FromDate1
								AND @ToDate1
							AND IsHoliday = 0
							AND ShiftId IS NULL
							AND IsChangeShift = 0
							AND IsKeepInView = 0
							AND IsOvertime = 0
							AND IsLeaveDay = 0
							AND (
								IsLeaveOffDay = 1
								OR IsOffDay = 1
								)

						INSERT INTO #tblOffDayReplacement (
							EmployeeId
							,WorkDate
							,ReplaceForDate
							)
						VALUES (
							@EmployeeId
							,@ReplacedDate
							,@PHDate
							)

						DELETE
						FROM #tblCalendar
						WHERE EmployeeId = @EmployeeId
							AND WorkDate = @ReplacedDate

						BREAK
					END

					--
					SELECT @ReplacedDate = MIN(WorkDate)
					FROM #tblCalendar
					WHERE EmployeeId = @EmployeeId
						AND WorkDate BETWEEN @FromDate1
							AND @ToDate1
						AND IsHoliday = 0
						AND IsChangeShift = 0
						AND IsKeepInView = 0
						AND IsOvertime = 0
						AND IsLeaveDay = 0
						AND IsLeaveOffDay = 0
						AND IsOffDay = 0

					IF @ReplacedDate IS NOT NULL
					BEGIN
						INSERT INTO #tblOffDayReplacement (
							EmployeeId
							,WorkDate
							,ReplaceForDate
							)
						VALUES (
							@EmployeeId
							,@ReplacedDate
							,@PHDate
							)

						DELETE
						FROM #tblCalendar
						WHERE EmployeeId = @EmployeeId
							AND WorkDate = @ReplacedDate

						BREAK
					END

					SET @FromDate1 = DATEADD(DAY, 7, @FromDate1)
				END

				SET @TotalClaimOffDays = @TotalClaimOffDays - 1
			END

			DELETE
			FROM #tblEmployeeWeekly
			WHERE EmployeeId = @EmployeeId
				AND FromDate = @FromDate
				AND ToDate = @ToDate
		END

		SET @RowIndex = @RowIndex + 1
	END

	UPDATE empca
	SET IsOffDay = 0
	FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblResetOffDay AS tbl ON empca.EmpId = tbl.EmployeeId
		AND empca.WorkDate = tbl.WorkDate

	MERGE dbo.EmployeeCalendar AS [target]
	USING #tblOffDayReplacement AS [source] ON (
				[target].EmpId = [source].EmployeeId
				AND [target].WorkDate = [source].WorkDate
				)
	WHEN MATCHED THEN
			UPDATE
			SET [target].IsOffDay = 1
	WHEN NOT MATCHED BY TARGET THEN
			INSERT (
				EmpId
				,WorkDate
				,IsOffDay
				)
			VALUES (
				[source].EmployeeId
				,[source].WorkDate
				,1
				);

	DELETE offrep
	FROM dbo.ScheduleArrangementOffDayReplacement AS offrep
	WHERE EXISTS (
			SELECT 1
			FROM #tblWeeklyHasHoliday AS tbl
			WHERE offrep.ReplaceForDate BETWEEN tbl.FromDate
					AND tbl.ToDate
			)

	INSERT INTO dbo.ScheduleArrangementOffDayReplacement (
		EmployeeId
		,WorkDate
		,ReplaceForDate
		)
	SELECT EmployeeId
		,WorkDate
		,ReplaceForDate
	FROM #tblOffDayReplacement

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 101
			,@ErrMsg = N'Update off replacement unsuccessfully!'
	END

	DROP TABLE #tblEmployee

	DROP TABLE #tblWeeklyHasHoliday

	DROP TABLE #tblDateRange

	DROP TABLE #tblLeaveRequest

	DROP TABLE #tblCalendar

	DROP TABLE #tblOffDayReplacement
END
