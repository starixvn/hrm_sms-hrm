
CREATE PROCEDURE [dbo].[ScheduleArrangement_UpdateOffDayReplacement] @ActorId INT
	,@FromDate DATE
	,@ToDate DATE
	,@EmployeeIds VARCHAR(MAX) = NULL
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

	IF EXISTS (
			SELECT 1
			FROM dbo.RefResource WITH (NOLOCK)
			WHERE RefName = 'EnableOffdayReplacement'
				AND Value = '0'
				AND IsActive = 1
			)
	BEGIN
		RETURN
	END

	DECLARE
		-- change shift type
		@ChangeShiftTypeId TINYINT = 1
		,@MoveOfDayTypeId TINYINT = 2
		--
		,@EmployeeId INT
		,@RowIndex INT = 1
		,@TotalRows INT
		,@tmpFromDate DATE
		,@tmpToDate DATE
		,@WorkDate DATE
		,@PHDate DATE
		,@OvertimeDate DATE
		,@ReplacedDate DATE
		,@TotalOffDaysByWeek SMALLINT
		,@TotalClaimOffDays SMALLINT
		,@IsFixedOffDay BIT
		,@DefaultDateFirst TINYINT = @@datefirst
	DECLARE @tblEmployeeIds TABLE (EmployeeId INT)
	DECLARE @tblEmployeeWeekly TABLE (
		EmployeeId INT
		,FromDate DATE
		,ToDate DATE
		,OffDaysByWeek SMALLINT
		,TotalOffDaysUsed SMALLINT
		)
	DECLARE @tblDateRange TABLE (WorkDate DATE)
	DECLARE @tblResetOffDay TABLE (
		EmployeeId INT
		,WorkDate DATE
		)
	DECLARE @tblOffDayReplacement TABLE (
		EmployeeId INT
		,WorkDate DATE
		,ReplaceForDate DATE
		)

	CREATE TABLE #tblEmployee (
		RowIndex INT IDENTITY(1, 1)
		,EmployeeId INT
		,IsLocal BIT
		,OffDaysByWeek SMALLINT
		)

	CREATE TABLE #tblWeeklyHasHoliday (
		RowIndex INT IDENTITY(1, 1)
		,FromDate DATE
		,ToDate DATE
		)

	SET DATEFIRST 1 -- monday
	SET @FromDate = DATEADD(DAY, - 1 * (6 + DATEPART(dw, @FromDate)) % 7, @FromDate)
	SET @ToDate = DATEADD(DAY, 7 - DATEPART(dw, @ToDate), @ToDate)

	INSERT INTO #tblWeeklyHasHoliday (FromDate)
	SELECT DISTINCT DATEADD(DAY, - 1 * (6 + DATEPART(dw, FromDate)) % 7, FromDate)
	FROM dbo.Holiday WITH (NOLOCK)
	WHERE FromDate BETWEEN DATEADD(DAY, - 14, @FromDate)
			AND @ToDate

	INSERT INTO #tblWeeklyHasHoliday (FromDate)
	SELECT DISTINCT DATEADD(DAY, - 1 * (6 + DATEPART(dw, ToDate)) % 7, ToDate)
	FROM dbo.Holiday WITH (NOLOCK)
	WHERE ToDate BETWEEN DATEADD(DAY, - 14, @FromDate)
			AND @ToDate
		AND DATEADD(DAY, - 1 * (6 + DATEPART(dw, ToDate)) % 7, ToDate) > FromDate

	UPDATE #tblWeeklyHasHoliday
	SET ToDate = DATEADD(DAY, 6, FromDate)

	SET DATEFIRST @DefaultDateFirst

	SELECT @RowIndex = 1
		,@TotalRows = COUNT(1)
	FROM #tblWeeklyHasHoliday

	IF @TotalRows = 0 RETURN

	WHILE @RowIndex <= @TotalRows
	BEGIN
		SELECT @tmpFromDate = FromDate
			,@tmpToDate = DATEADD(DAY, 14, ToDate)
		FROM #tblWeeklyHasHoliday
		WHERE RowIndex = @RowIndex

		WHILE (@tmpFromDate <= @tmpToDate)
		BEGIN
			IF NOT EXISTS (
					SELECT 1
					FROM @tblDateRange
					WHERE WorkDate = @tmpFromDate
					)
			BEGIN
				INSERT INTO @tblDateRange (WorkDate)
				VALUES (@tmpFromDate)
			END

			SET @tmpFromDate = DATEADD(DAY, 1, @tmpFromDate)
		END

		SET @RowIndex = @RowIndex + 1
	END

	INSERT INTO @tblEmployeeIds (EmployeeId)
	SELECT val
	FROM dbo.fnc_strSplit(@EmployeeIds, ',')

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
		LEFT JOIN @tblEmployeeIds AS tblempid ON emp.EmpId = tblempid.EmployeeId
	WHERE (
			@EmployeeIds IS NULL
			OR tblempid.EmployeeId IS NOT NULL
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

	INSERT INTO @tblEmployeeWeekly (
		EmployeeId
		,FromDate
		,ToDate
		,OffDaysByWeek
		,TotalOffDaysUsed
		)
	SELECT emp.EmployeeId
		,holiday.FromDate
		,holiday.ToDate
		,emp.OffDaysByWeek
		,0
	FROM #tblEmployee AS emp
		,#tblWeeklyHasHoliday AS holiday

	-- get calendar
	SELECT @tmpFromDate = MIN(WorkDate)
		,@tmpToDate = MAX(WorkDate)
	FROM @tblDateRange

	EXEC dbo.tmpEmployeeCalendar_Insert @fromDate = @tmpFromDate
		,@toDate = @tmpToDate
		,@empIds = @EmployeeIds
		,@isEnable = 1
		,@isDelete = 0

	SELECT emp.EmployeeId
		,tmpca.WorkDate
		,empca.ShiftId
		,tmpca.IsOvertime
		,tmpca.IsHoliday
		,tmpca.IsOffDay
		,ISNULL(empca.IsFixedOffDay, 0) AS IsFixedOffDay
		,tmpca.IsKeepInView
		,tmpca.IsLeaveDay
		,tmpca.IsLeaveOffday
		,tmpca.LeaveDay
		,tmpca.LeaveTypeId
		,tmpca.LeaveTypeId2
		,empca.ModifiedBy AS ModifierId
		,tmpca.ChangeShiftTypeId
		,tmpca.MoveOffFromDate
		,tmpca.MoveOffToDate
	INTO #tblEmployeeCalendar
	FROM #tblEmployee AS emp WITH (NOLOCK)
		INNER JOIN dbo.tmpEmployeeCalendar AS tmpca WITH (NOLOCK) ON emp.EmployeeId = tmpca.EmpId
		AND tmpca.WorkDate BETWEEN @tmpFromDate
			AND @tmpToDate
		LEFT JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON emp.EmployeeId = empca.EmpId
		AND tmpca.WorkDate = empca.WorkDate
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.ScheduleArrangementOffDayReplacement AS offrep WITH (NOLOCK)
			WHERE emp.EmployeeId = offrep.EmployeeId
				AND tmpca.WorkDate = offrep.WorkDate
				AND offrep.ReplaceForDate < DATEADD(DAY, - 14, @FromDate)
			)

	SELECT @RowIndex = 1
		,@TotalRows = COUNT(1)
	FROM #tblWeeklyHasHoliday WITH (NOLOCK)

	WHILE @RowIndex <= @TotalRows
	BEGIN
		SELECT @FromDate = FromDate
			,@ToDate = ToDate
		FROM #tblWeeklyHasHoliday WITH (NOLOCK)
		WHERE RowIndex = @RowIndex

		UPDATE emp
		SET emp.TotalOffDaysUsed = empca.TotalOffDaysUsed
		FROM @tblEmployeeWeekly AS emp
			CROSS APPLY (
			SELECT COUNT(1) AS TotalOffDaysUsed
			FROM #tblEmployeeCalendar AS empca WITH (NOLOCK)
			WHERE empca.EmployeeId = emp.EmployeeId
				AND empca.WorkDate BETWEEN emp.FromDate AND emp.ToDate
				AND empca.IsHoliday = 0
				AND (
					empca.IsLeaveOffDay = 1
					OR (
						empca.IsOffDay = 0
						AND empca.ChangeShiftTypeId = @MoveOfDayTypeId
						AND empca.MoveOffToDate IS NOT NULL
						)
					OR (
						(
						empca.IsOffDay = 1
						AND (
							empca.IsFixedOffDay = 1
							OR empca.ShiftId IS NULL
							OR (
								empca.ShiftId IS NOT NULL
								AND (
									empca.IsOvertime = 1
									OR DATEPART(DW, empca.WorkDate) IN (1, 7) -- sat, sun
									)
								)
							)
						)
						AND NOT EXISTS (
							SELECT 1
							FROM dbo.ScheduleArrangementOffDayReplacement AS offrep WITH (NOLOCK)
							WHERE emp.EmployeeId = offrep.EmployeeId
								AND empca.WorkDate = offrep.WorkDate
								--AND empca.ModifierId IS NULL
								AND empca.IsOvertime = 0
							)
						AND NOT EXISTS (
							SELECT 1
							FROM dbo.ScheduleArrangementOffDayReplacement AS offrep WITH (NOLOCK)
							WHERE emp.EmployeeId = offrep.EmployeeId
								AND empca.WorkDate = offrep.WorkDate
								AND offrep.ReplaceForDate NOT BETWEEN emp.FromDate AND emp.ToDate
							)
						)
					)
			) AS empca
		WHERE emp.FromDate = @FromDate
			AND emp.ToDate = @ToDate

		UPDATE empca
		SET empca.IsOffDay = 0
		FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
			INNER JOIN @tblEmployeeWeekly AS emp ON empca.EmpId = emp.EmployeeId
			AND emp.FromDate = @FromDate
			AND emp.ToDate = @ToDate
			INNER JOIN #tblEmployeeCalendar AS tmpempca WITH (NOLOCK) ON empca.EmpId = tmpempca.EmployeeId
			AND empca.WorkDate = tmpempca.WorkDate
		WHERE empca.IsOffDay = 1
			AND ISNULL(empca.IsFixedOffDay, 0) = 0
			AND tmpempca.IsOvertime = 0
			AND EXISTS (
				SELECT 1
				FROM dbo.ScheduleArrangementOffDayReplacement AS offrep WITH (NOLOCK)
				WHERE empca.EmpId = offrep.EmployeeId
					--AND (
					--	(
					--		emp.TotalOffDaysUsed < emp.OffDaysByWeek
					--		AND empca.ModifiedBy IS NULL
					--		)
					--	OR emp.TotalOffDaysUsed >= emp.OffDaysByWeek
					--	)
					AND empca.WorkDate = offrep.WorkDate
					AND offrep.ReplaceForDate BETWEEN emp.FromDate AND emp.ToDate
				)

		UPDATE empca
		SET empca.IsOffDay = 0
		FROM #tblEmployeeCalendar AS empca WITH (NOLOCK)
			INNER JOIN @tblEmployeeWeekly AS emp ON empca.EmployeeId = emp.EmployeeId
			AND emp.FromDate = @FromDate
			AND emp.ToDate = @ToDate
		WHERE empca.IsOffDay = 1
			AND empca.IsFixedOffDay = 0
			AND empca.IsOvertime = 0
			AND EXISTS (
				SELECT 1
				FROM dbo.ScheduleArrangementOffDayReplacement AS offrep WITH (NOLOCK)
				WHERE empca.EmployeeId = offrep.EmployeeId
					--AND (
					--	(
					--		emp.TotalOffDaysUsed < emp.OffDaysByWeek
					--		AND empca.ModifierId IS NULL
					--		)
					--	OR emp.TotalOffDaysUsed >= emp.OffDaysByWeek
					--	)
					AND empca.WorkDate = offrep.WorkDate
					AND offrep.ReplaceForDate BETWEEN emp.FromDate
						AND emp.ToDate
				)

		DELETE offr
		FROM dbo.ScheduleArrangementOffDayReplacement AS offr WITH (ROWLOCK, UPDLOCK)
			INNER JOIN @tblEmployeeWeekly AS tbl ON offr.EmployeeId = tbl.EmployeeId
			AND offr.ReplaceForDate BETWEEN tbl.FromDate AND tbl.ToDate
			AND tbl.TotalOffDaysUsed >= tbl.OffDaysByWeek

		DELETE
		FROM @tblEmployeeWeekly
		WHERE TotalOffDaysUsed >= OffDaysByWeek

		WHILE EXISTS (
				SELECT 1
				FROM @tblEmployeeWeekly
				WHERE FromDate = @FromDate AND ToDate = @ToDate
				)
		BEGIN
			SELECT TOP 1 @EmployeeId = EmployeeId
				,@TotalOffDaysByWeek = OffDaysByWeek
				,@TotalClaimOffDays = OffDaysByWeek - TotalOffDaysUsed
			FROM @tblEmployeeWeekly
			WHERE FromDate = @FromDate AND ToDate = @ToDate

			WHILE @TotalClaimOffDays > 0
			BEGIN
				SELECT @PHDate = MIN(daterange.WorkDate)
				FROM @tblDateRange AS daterange
				WHERE daterange.WorkDate BETWEEN @FromDate AND @ToDate
					AND EXISTS (
						SELECT 1
						FROM dbo.Holiday WITH (NOLOCK)
						WHERE daterange.WorkDate BETWEEN FromDate AND ToDate
						)
					AND NOT EXISTS (
						SELECT 1
						FROM @tblOffDayReplacement
						WHERE EmployeeId = @EmployeeId AND ReplaceForDate = daterange.WorkDate
						)

				IF @PHDate IS NULL
					BREAK

				-- off day next to PH
				SELECT @WorkDate = MIN(WorkDate)
				FROM #tblEmployeeCalendar WITH (NOLOCK)
				WHERE EmployeeId = @EmployeeId
					AND WorkDate > @PHDate
					AND WorkDate <= @ToDate
					AND IsHoliday = 0
					AND ShiftId IS NULL
					AND ChangeShiftTypeId IS NULL
					AND IsKeepInView = 0
					AND IsOvertime = 0
					AND IsLeaveDay = 0
					AND (IsLeaveOffDay = 1 OR IsOffDay = 1)
					AND DATEPART(DW, WorkDate) NOT IN (1, 7) -- sat, sun

				IF @WorkDate IS NOT NULL
					AND NOT EXISTS (
						SELECT 1
						FROM #tblEmployeeCalendar WITH (NOLOCK)
						WHERE EmployeeId = @EmployeeId
							AND WorkDate > @PHDate
							AND WorkDate < @WorkDate
							AND IsHoliday = 0
							AND IsLeaveOffDay = 0
							AND ChangeShiftTypeId IS NULL
							AND (
								IsOffDay = 0
								OR (
									IsOffDay = 1
									AND ShiftId IS NOT NULL
									)
								)
						)
				BEGIN
					INSERT INTO @tblOffDayReplacement (EmployeeId, WorkDate, ReplaceForDate)
					VALUES (@EmployeeId, @WorkDate, @PHDate)

					DELETE
					FROM #tblEmployeeCalendar WITH (ROWLOCK, UPDLOCK)
					WHERE EmployeeId = @EmployeeId
						AND WorkDate = @WorkDate

					SET @TotalClaimOffDays = @TotalClaimOffDays - 1

					CONTINUE
				END

				-- off day OT
				SELECT @WorkDate = MIN(WorkDate)
					,@IsFixedOffDay = MAX(CAST(IsFixedOffDay AS TINYINT))
				FROM #tblEmployeeCalendar WITH (NOLOCK)
				WHERE EmployeeId = @EmployeeId
					AND WorkDate BETWEEN @FromDate
						AND @ToDate
					AND IsHoliday = 0
					AND ShiftId IS NOT NULL
					AND ChangeShiftTypeId IS NULL
					AND IsKeepInView = 0
					--AND IsOvertime = 0
					AND IsLeaveDay = 0
					AND IsLeaveOffDay = 0
					AND IsOffDay = 1
					AND DATEPART(DW, WorkDate) NOT IN (1, 7) -- sat, sun

				IF @WorkDate IS NOT NULL
				BEGIN
					IF @IsFixedOffDay = 0
					BEGIN
						SELECT @ReplacedDate = MIN(WorkDate)
						FROM #tblEmployeeCalendar WITH (NOLOCK)
						WHERE EmployeeId = @EmployeeId
							AND WorkDate > @PHDate
							AND WorkDate <= @ToDate
							AND IsHoliday = 0
							AND ChangeShiftTypeId IS NULL
							AND IsKeepInView = 0
							--AND IsOvertime = 0
							AND IsLeaveDay = 0
							AND IsLeaveOffDay = 0
							AND (IsOffDay = 0 OR WorkDate = @WorkDate)

						INSERT INTO @tblResetOffDay (EmployeeId, WorkDate)
						VALUES (@EmployeeId, @WorkDate)

						UPDATE #tblEmployeeCalendar WITH (ROWLOCK, UPDLOCK)
						SET IsOffDay = 0
						WHERE EmployeeId = @EmployeeId
							AND WorkDate = @WorkDate

						IF @ReplacedDate IS NOT NULL
						BEGIN
							INSERT INTO @tblOffDayReplacement (EmployeeId, WorkDate, ReplaceForDate)
							VALUES (@EmployeeId, @ReplacedDate, @PHDate)

							DELETE
							FROM #tblEmployeeCalendar WITH (ROWLOCK, UPDLOCK)
							WHERE EmployeeId = @EmployeeId
								AND WorkDate = @ReplacedDate

							SET @TotalClaimOffDays = @TotalClaimOffDays - 1

							CONTINUE
						END
					END
					ELSE
					BEGIN
						INSERT INTO @tblOffDayReplacement (EmployeeId, WorkDate, ReplaceForDate)
						VALUES (@EmployeeId, @WorkDate, @PHDate)

						DELETE
						FROM #tblEmployeeCalendar WITH (ROWLOCK, UPDLOCK)
						WHERE EmployeeId = @EmployeeId
							AND WorkDate = @WorkDate

						SET @TotalClaimOffDays = @TotalClaimOffDays - 1

						CONTINUE
					END
				END

				-- normal day
				SELECT @ReplacedDate = MIN(WorkDate)
				FROM #tblEmployeeCalendar WITH (NOLOCK)
				WHERE EmployeeId = @EmployeeId
					AND WorkDate > @PHDate
					AND WorkDate <= @ToDate
					AND IsHoliday = 0
					AND ChangeShiftTypeId IS NULL
					AND IsKeepInView = 0
					AND IsOvertime = 0
					AND IsLeaveDay = 0
					AND IsLeaveOffDay = 0
					AND IsOffDay = 0

				IF @ReplacedDate IS NOT NULL
				BEGIN
					INSERT INTO @tblOffDayReplacement (EmployeeId, WorkDate, ReplaceForDate)
					VALUES (@EmployeeId, @ReplacedDate, @PHDate)

					DELETE
					FROM #tblEmployeeCalendar WITH (ROWLOCK, UPDLOCK)
					WHERE EmployeeId = @EmployeeId
						AND WorkDate = @ReplacedDate

					SET @TotalClaimOffDays = @TotalClaimOffDays - 1

					CONTINUE
				END

				-- next week, next week + 1
				SELECT @tmpFromDate = DATEADD(DAY, 1, @ToDate)

				WHILE @tmpFromDate < DATEADD(DAY, 14, @ToDate)
				BEGIN
					SET @tmpToDate = DATEADD(DAY, 6, @tmpFromDate)

					-- off day next to PH
					SELECT @WorkDate = MIN(WorkDate)
					FROM #tblEmployeeCalendar WITH (NOLOCK)
					WHERE EmployeeId = @EmployeeId
						AND WorkDate BETWEEN @tmpFromDate
							AND @tmpToDate
						AND IsHoliday = 0
						AND ShiftId IS NULL
						AND ChangeShiftTypeId IS NULL
						AND IsKeepInView = 0
						AND IsOvertime = 0
						AND IsLeaveDay = 0
						AND (IsLeaveOffDay = 1 OR IsOffDay = 1)

					IF @WorkDate IS NOT NULL
						AND NOT EXISTS (
							SELECT 1
							FROM #tblEmployeeCalendar WITH (NOLOCK)
							WHERE EmployeeId = @EmployeeId
								AND WorkDate BETWEEN @tmpFromDate AND @WorkDate
								AND IsHoliday = 0
								AND IsLeaveOffDay = 0
								AND ChangeShiftTypeId IS NULL
								AND (
									IsOffDay = 0
									OR (
										IsOffDay = 1
										AND ShiftId IS NOT NULL
										)
									)
							)
					BEGIN
						INSERT INTO @tblOffDayReplacement (EmployeeId, WorkDate, ReplaceForDate)
						VALUES (@EmployeeId, @WorkDate, @PHDate)

						DELETE
						FROM #tblEmployeeCalendar WITH (ROWLOCK, UPDLOCK)
						WHERE EmployeeId = @EmployeeId
							AND WorkDate = @WorkDate

						BREAK
					END

					-- off day OT
					SELECT @WorkDate = MIN(WorkDate)
						,@IsFixedOffDay = MAX(CAST(IsFixedOffDay AS TINYINT))
					FROM #tblEmployeeCalendar WITH (NOLOCK)
					WHERE EmployeeId = @EmployeeId
						AND WorkDate BETWEEN @tmpFromDate AND @tmpToDate
						AND IsHoliday = 0
						AND ShiftId IS NOT NULL
						AND ChangeShiftTypeId IS NULL
						AND IsKeepInView = 0
						--AND IsOvertime = 0
						AND IsLeaveDay = 0
						AND IsLeaveOffDay = 0
						AND IsOffDay = 1

					IF @WorkDate IS NOT NULL
					BEGIN
						IF @IsFixedOffDay = 0
						BEGIN
							SELECT @ReplacedDate = MIN(WorkDate)
							FROM #tblEmployeeCalendar WITH (NOLOCK)
							WHERE EmployeeId = @EmployeeId
								AND WorkDate BETWEEN @tmpFromDate AND @tmpToDate
								AND IsHoliday = 0
								AND ChangeShiftTypeId IS NULL
								AND IsKeepInView = 0
								--AND IsOvertime = 0
								AND IsLeaveDay = 0
								AND IsLeaveOffDay = 0
								AND (IsOffDay = 0 OR WorkDate = @WorkDate)

							INSERT INTO @tblOffDayReplacement (EmployeeId, WorkDate, ReplaceForDate)
							VALUES (@EmployeeId, @ReplacedDate, @PHDate)

							INSERT INTO @tblResetOffDay (EmployeeId, WorkDate)
							VALUES (@EmployeeId, @WorkDate)

							UPDATE #tblEmployeeCalendar WITH (ROWLOCK, UPDLOCK)
							SET IsOffDay = 0
							WHERE EmployeeId = @EmployeeId
								AND WorkDate = @WorkDate

							DELETE
							FROM #tblEmployeeCalendar WITH (ROWLOCK, UPDLOCK)
							WHERE EmployeeId = @EmployeeId
								AND WorkDate = @ReplacedDate
						END
						ELSE
						BEGIN
							INSERT INTO @tblOffDayReplacement (EmployeeId, WorkDate, ReplaceForDate)
							VALUES (@EmployeeId, @WorkDate, @PHDate)

							DELETE
							FROM #tblEmployeeCalendar WITH (ROWLOCK, UPDLOCK)
							WHERE EmployeeId = @EmployeeId
								AND WorkDate = @WorkDate
						END

						BREAK
					END

					-- TotalOffDays > TotalOffDaysByWeek
					IF EXISTS (
							SELECT 1
							FROM #tblEmployeeCalendar WITH (NOLOCK)
							WHERE EmployeeId = @EmployeeId
								AND WorkDate BETWEEN @tmpFromDate AND @tmpToDate
								AND IsHoliday = 0
								AND ShiftId IS NULL
								AND ChangeShiftTypeId IS NULL
								AND IsKeepInView = 0
								AND IsOvertime = 0
								AND IsLeaveDay = 0
								AND (IsLeaveOffDay = 1 OR IsOffDay = 1)
							GROUP BY EmployeeId
							HAVING COUNT(1) > @TotalOffDaysByWeek
							)
					BEGIN
						SELECT @ReplacedDate = MIN(WorkDate)
						FROM #tblEmployeeCalendar WITH (NOLOCK)
						WHERE EmployeeId = @EmployeeId
							AND WorkDate BETWEEN @tmpFromDate AND @tmpToDate
							AND IsHoliday = 0
							AND ShiftId IS NULL
							AND ChangeShiftTypeId IS NULL
							AND IsKeepInView = 0
							AND IsOvertime = 0
							AND IsLeaveDay = 0
							AND (IsLeaveOffDay = 1 OR IsOffDay = 1)

						INSERT INTO @tblOffDayReplacement (EmployeeId, WorkDate, ReplaceForDate)
						VALUES (@EmployeeId, @ReplacedDate, @PHDate)

						DELETE
						FROM #tblEmployeeCalendar WITH (ROWLOCK, UPDLOCK)
						WHERE EmployeeId = @EmployeeId
							AND WorkDate = @ReplacedDate

						BREAK
					END

					-- normal day
					SELECT @ReplacedDate = MIN(WorkDate)
					FROM #tblEmployeeCalendar WITH (NOLOCK)
					WHERE EmployeeId = @EmployeeId
						AND WorkDate BETWEEN @tmpFromDate AND @tmpToDate
						AND IsHoliday = 0
						AND ChangeShiftTypeId IS NULL
						AND IsKeepInView = 0
						AND IsOvertime = 0
						AND IsLeaveDay = 0
						AND IsLeaveOffDay = 0
						AND IsOffDay = 0

					IF @ReplacedDate IS NOT NULL
					BEGIN
						INSERT INTO @tblOffDayReplacement (EmployeeId, WorkDate, ReplaceForDate)
						VALUES (@EmployeeId, @ReplacedDate, @PHDate)

						DELETE
						FROM #tblEmployeeCalendar WITH (ROWLOCK, UPDLOCK)
						WHERE EmployeeId = @EmployeeId
							AND WorkDate = @ReplacedDate

						BREAK
					END

					SET @tmpFromDate = DATEADD(DAY, 7, @tmpFromDate)
				END

				SET @TotalClaimOffDays = @TotalClaimOffDays - 1
			END

			DELETE
			FROM @tblEmployeeWeekly
			WHERE EmployeeId = @EmployeeId
				AND FromDate = @FromDate
				AND ToDate = @ToDate
		END

		SET @RowIndex = @RowIndex + 1
	END

	UPDATE empca
	SET empca.IsOffDay = 0
	FROM dbo.EmployeeCalendar AS empca WITH (ROWLOCK, UPDLOCK)
		INNER JOIN @tblResetOffDay AS tbl ON empca.EmpId = tbl.EmployeeId
		AND empca.WorkDate = tbl.WorkDate

	MERGE dbo.EmployeeCalendar AS [target]
	USING @tblOffDayReplacement AS [source] ON (
				[target].EmpId = [source].EmployeeId
				AND [target].WorkDate = [source].WorkDate
				)
	WHEN MATCHED THEN
			UPDATE
			SET [target].IsOffDay = 1
	WHEN NOT MATCHED BY TARGET THEN
			INSERT (EmpId, WorkDate, IsOffDay)
			VALUES ([source].EmployeeId, [source].WorkDate, 1);

	DELETE offrep
	FROM dbo.ScheduleArrangementOffDayReplacement AS offrep
		LEFT JOIN @tblEmployeeIds AS tblid ON offrep.EmployeeId = tblid.EmployeeId
	WHERE (
			@EmployeeIds IS NULL
			OR tblid.EmployeeId IS NOT NULL
			)
		AND EXISTS (
			SELECT 1
			FROM #tblWeeklyHasHoliday AS tbl
			WHERE offrep.ReplaceForDate BETWEEN tbl.FromDate AND tbl.ToDate
			)

	UPDATE offr
	SET offr.ReplaceForDate = tbl.ReplaceForDate
	FROM dbo.ScheduleArrangementOffDayReplacement AS offr WITH (ROWLOCK, UPDLOCK)
		INNER JOIN @tblOffDayReplacement AS tbl ON offr.EmployeeId = tbl.EmployeeId
		AND offr.WorkDate = tbl.WorkDate

	INSERT INTO dbo.ScheduleArrangementOffDayReplacement (
		EmployeeId
		,WorkDate
		,ReplaceForDate
		)
	SELECT EmployeeId
		,WorkDate
		,ReplaceForDate
	FROM @tblOffDayReplacement AS tbl
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.ScheduleArrangementOffDayReplacement AS offr WITH (NOLOCK)
			WHERE tbl.EmployeeId = offr.EmployeeId
				AND tbl.WorkDate = offr.WorkDate
			)

	DROP TABLE #tblEmployee

	DROP TABLE #tblWeeklyHasHoliday

	DROP TABLE #tblEmployeeCalendar
END
