
CREATE PROCEDURE [dbo].[EmployeeAttendanceViolation_Insert] @EmpId INT = NULL
	,@WorkDate DATETIME = NULL
	,@SyncFromDate DATETIME
AS
/*
Created: 2015-08-26@Josh.Phan
Task: Insert employee attendance violation.
DB: Redmine
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF(DATEDIFF(DD, @SyncFromDate, '2016-01-01') > 0)
	BEGIN
		SELECT @SyncFromDate = '2016-01-01'
	END

	DECLARE @TotalWorkHoursWeekend DECIMAL(18, 2)
		,@TotalWorkHours DECIMAL(18, 2)
		,@EarlyOutMins INT

	SET @TotalWorkHoursWeekend = 9
	SET @TotalWorkHours = 7
	SET @EarlyOutMins = 15

	--IF ISNULL(@EmpId, '') <> ''
	--	AND ISNULL(@WorkDate, '') <> ''
	--BEGIN
	--	DELETE
	--	FROM [dbo].[EmployeeAttendanceViolation]
	--	WHERE [EmpId] = @EmpId
	--		AND [WorkDate] = @WorkDate
	--END
	CREATE TABLE #EmployeeViolation1 (
		[WorkDate] DATETIME
		,[EmpId] INT
		,[MissingCheckInOut] INT
		,[LateIn] INT
		,[EarlyOut] INT
		,[WorkingBelowTime] INT
		,[ChangeShift] INT
		)

	INSERT INTO #EmployeeViolation1 (
		[WorkDate]
		,[EmpId]
		,[MissingCheckInOut]
		,[LateIn]
		,[EarlyOut]
		,[WorkingBelowTime]
		,[ChangeShift]
		)
	SELECT [AttendanceDate]
		,empA.[EmpId]
		,(CASE 
			WHEN ISNULL(empA.[ShiftId], '') <> ''
				AND empC.[IsOffDay] = 0 THEN CASE 
					WHEN ISNULL([InFixed], '') = ''
						AND ISNULL([OutFixed], '') <> '' 
						AND GETDATE() > empA.[ExpectedInTime] THEN 1
					WHEN ISNULL([InFixed], '') <> ''
						AND ISNULL([OutFixed], '') = ''
						AND GETDATE() > empA.[ExpectedOutTime] THEN 1
					WHEN EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[InoutMissRequest] AS missR WITH (NOLOCK)
								INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON missR.[RequestId] = workR.[RequestId]
								AND workR.[CurrentStatus] IN (4, 6)
								AND workR.[RequestType] IN (1, 2) -- Check In / Check Out
							WHERE empA.[AttendanceDate] = missR.[OnDate]
								AND empA.[EmpId] = missR.[EmpId]
							) THEN 1
					ELSE 0
					END
			ELSE 0 END
			)
		,(CASE 
			WHEN ISNULL(empA.[ShiftId], '') <> ''
				AND empC.[IsOffDay] = 0 THEN CASE 
					WHEN empA.[LateIn] > 0
						OR EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
								INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
								AND workR.[CurrentStatus] IN (4, 6)
								AND workR.[RequestType] = 3
							WHERE empA.[AttendanceDate] = inR.[FromDate]
								AND empA.[EmpId] = inR.[EmpId]
							) THEN 1
					ELSE 0
					END
			ELSE 0 END
			)
		,(CASE 
			WHEN ISNULL(empA.[ShiftId], '') <> ''
				AND empC.[IsOffDay] = 0 THEN CASE 
					WHEN empA.[EarlyOut] > @EarlyOutMins
						OR EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
								INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
								AND workR.[CurrentStatus] IN (4, 6)
								AND workR.[RequestType] = 4
							WHERE empA.[AttendanceDate] = inR.[FromDate]
								AND empA.[EmpId] = inR.[EmpId]
								AND inR.[EarlyOutMins] > @EarlyOutMins
							) THEN 1
					ELSE 0
					END
			ELSE 0 END
			)
		,(CASE 
			WHEN ISNULL(empA.[ShiftId], '') <> ''
				AND empC.[IsOffDay] = 0 THEN CASE 
					WHEN EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[Employee] AS emp WITH (NOLOCK)
								INNER JOIN [dbo].[EmployeeSummary] AS empS WITH (NOLOCK) ON emp.[EmpId] = empS.[EmpId]
							WHERE emp.[EmpId] = empA.[EmpId]
								AND emp.[GroupId] IN (4, 8) -- LG, PH
								AND empS.[IsLocal] = 0 -- Expat							
							) THEN CASE 
							WHEN EXISTS (
									SELECT TOP 1 1
									WHERE DATEPART(DW, empA.[AttendanceDate]) IN (1, 7) -- Sunday or Saturday
									) THEN CASE 
									WHEN [TotalWorkHours] < @TotalWorkHoursWeekend THEN 1
									ELSE 0
									END
							ELSE CASE 
									WHEN [TotalWorkHours] < @TotalWorkHours THEN 1
									ELSE 0
									END
							END
					ELSE CASE 
							WHEN [TotalWorkHours] < @TotalWorkHours THEN 1
							ELSE 0
							END
					END
			ELSE 0 END
			)
		,(CASE 
			WHEN EXISTS (
					SELECT TOP 1 1
					FROM [dbo].[ChangeShiftRequest] AS cg WITH (NOLOCK)
						INNER JOIN [dbo].[WorkingTimeRequest] AS wtc WITH (NOLOCK) ON cg.[RequestId] = wtc.[RequestId]
					WHERE cg.[EmpId] = empC.[EmpId]
						AND cg.[FromDate] = empC.[WorkDate]
						AND (
							wtc.[CurrentStatus] = 4
							OR wtc.[CurrentStatus] = 6
							)
					) THEN 1
			ELSE 0 END
			)
	FROM [dbo].[EmployeeAttendance] AS empA WITH (NOLOCK)
		INNER JOIN [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK) ON empA.[EmpId] = empC.[EmpId]
		AND empA.[AttendanceDate] = empC.[WorkDate]
	WHERE [AttendanceDate] >= @SyncFromDate
		AND (
			ISNULL(@EmpId, '') = ''
			OR empA.[EmpId] = @EmpId
			)
		AND (
			ISNULL(@WorkDate, '') = ''
			OR empA.[AttendanceDate] = @WorkDate
			)

	CREATE TABLE #EmployeeViolation2 (
		[WorkDate] DATETIME
		,[EmpId] INT
		,[Absent] INT
		,[UnpaidLeaveAbsent] INT
		,[UnpaidLeave] FLOAT
		)

	INSERT INTO #EmployeeViolation2 (
		[WorkDate]
		,[EmpId]
		,[Absent]
		,[UnpaidLeaveAbsent]
		,[UnpaidLeave]
		)
	SELECT empC.[WorkDate]
		,empC.[EmpId]
		,(CASE 
			WHEN ISNULL(leaRD.[RequestId], '') <> '' THEN CASE 
					WHEN empC.[WorkDate] = leaRD.[OnDate]
						AND leaRD.[IsHalfDay] = 1
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[EmployeeAttendance] WITH (NOLOCK)
							WHERE [AttendanceDate] = empC.[WorkDate]
								AND [EmpId] = empC.[EmpId]
								AND ISNULL([InFixed], '') = ''
								AND ISNULL([OutFixed], '') = ''
								AND GETDATE() > [ExpectedOutTime]
							) THEN CASE 
							WHEN woT.[CreatedDate] < leaRD.[OnDate] THEN 1
							WHEN woT.[CreatedDate] >= leaRD.[OnDate]
								AND leaRD.[OnDate] <> (
									SELECT MIN(OnDate)
									FROM [dbo].[LeaveRequestDetail] WITH (NOLOCK)
									WHERE [RequestId] = leaRD.[RequestId]
									GROUP BY [RequestId]
									) THEN 1
							ELSE 0
							END
					WHEN leaRD.[LeaveType] = 2 -- Annual Leave
						AND woT.[CreatedDate] >= leaRD.[OnDate]
						AND leaRD.[OnDate] = (
							SELECT MIN(OnDate)
							FROM [dbo].[LeaveRequestDetail] WITH (NOLOCK)
							WHERE [RequestId] = leaRD.[RequestId]
							GROUP BY [RequestId]
							) THEN CASE 
							WHEN leaRD.[IsHalfDay] <> 1 THEN 1
							ELSE CASE 
									WHEN NOT EXISTS (
											SELECT TOP 1 1
											FROM [dbo].[LeaveRequestDetail] AS leaDt WITH (NOLOCK)
												INNER JOIN [dbo].[WorkingTimeRequest] AS woTt WITH (NOLOCK) ON leaDt.[RequestId] = woTt.[RequestId]
											WHERE leaDt.[EmpId] = leaRD.[EmpId]
												AND leaDt.[OnDate] = leaRD.[OnDate]
												AND (
													[LeaveType] <> leaRD.[LeaveType]
													OR leaDt.[RequestId] <> leaRD.[RequestId]
													)
												AND (
													woTt.[CurrentStatus] = 4
													OR woTt.[CurrentStatus] = 6
													)
											) THEN 1
									ELSE CASE 
											WHEN EXISTS (
													SELECT TOP 1 1
													FROM [dbo].[LeaveRequestDetail] AS leaDt WITH (NOLOCK)
														INNER JOIN [dbo].[WorkingTimeRequest] AS woTt WITH (NOLOCK) ON leaDt.[RequestId] = woTt.[RequestId]
													WHERE leaDt.[EmpId] = leaRD.[EmpId]
														AND leaDt.[OnDate] = leaRD.[OnDate]
														AND (
															[LeaveType] <> leaRD.[LeaveType]
															OR leaDt.[RequestId] <> leaRD.[RequestId]
															)
														AND [FromTime] > leaRD.[FromTime]
														AND (
															woTt.[CurrentStatus] = 4
															OR woTt.[CurrentStatus] = 6
															)
													) THEN 1
											ELSE 0
											END
									END
							END
					ELSE 0
					END
			WHEN ISNULL(lea.[RequestId], '') = '' THEN CASE 
					WHEN ISNULL(empC.[ShiftId], '') <> ''
						AND empC.[IsOffDay] <> 1
						AND empC.[IsHoliday] <> 1
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[EmployeeAttendance] WITH (NOLOCK)
							WHERE [AttendanceDate] = empC.[WorkDate]
								AND [EmpId] = empC.[EmpId]
								AND ISNULL([InFixed], '') = ''
								AND ISNULL([OutFixed], '') = ''
								AND GETDATE() > [ExpectedOutTime]
							) THEN 1
					ELSE 0
					END
			ELSE CASE 
					WHEN empC.[WorkDate] = lea.[FromDate]
						AND lea.[TotalDays] = 0.5
						AND EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[EmployeeAttendance] WITH (NOLOCK)
							WHERE [AttendanceDate] = empC.[WorkDate]
								AND [EmpId] = empC.[EmpId]
								AND ISNULL([InFixed], '') = ''
								AND ISNULL([OutFixed], '') = ''
								AND GETDATE() > [ExpectedOutTime]
							) THEN 1
					WHEN lea.[LeaveType] = 2
						AND woT.[CreatedDate] >= lea.[FromDate]
						AND empC.[WorkDate] = lea.[FromDate] THEN 1
					ELSE 0
					END END
			) -- Absent
		,(CASE 
			WHEN ISNULL(leaRD.[RequestId], '') <> '' THEN CASE 
					WHEN leaRD.[LeaveType] = 25 -- Unpaid Leave
						AND woT.[CreatedDate] >= leaRD.[OnDate]
						AND leaRD.[OnDate] = (
							SELECT MIN(OnDate)
							FROM [dbo].[LeaveRequestDetail] WITH (NOLOCK)
							WHERE [RequestId] = leaRD.[RequestId]
							GROUP BY [RequestId]
							) THEN CASE 
							WHEN leaRD.[IsHalfDay] <> 1 THEN 1
							ELSE CASE 
									WHEN NOT EXISTS (
											SELECT TOP 1 1
											FROM [dbo].[LeaveRequestDetail] AS leaDt WITH (NOLOCK)
												INNER JOIN [dbo].[WorkingTimeRequest] AS woTt WITH (NOLOCK) ON leaDt.[RequestId] = woTt.[RequestId]
											WHERE leaDt.[EmpId] = leaRD.[EmpId]
												AND leaDt.[OnDate] = leaRD.[OnDate]
												AND (
													[LeaveType] <> leaRD.[LeaveType]
													OR leaDt.[RequestId] <> leaRD.[RequestId]
													)
												AND (
													woTt.[CurrentStatus] = 4
													OR woTt.[CurrentStatus] = 6
													)
											) THEN 1
									ELSE CASE 
											WHEN EXISTS (
													SELECT TOP 1 1
													FROM [dbo].[LeaveRequestDetail] AS leaDt WITH (NOLOCK)
														INNER JOIN [dbo].[WorkingTimeRequest] AS woTt WITH (NOLOCK) ON leaDt.[RequestId] = woTt.[RequestId]
													WHERE leaDt.[EmpId] = leaRD.[EmpId]
														AND leaDt.[OnDate] = leaRD.[OnDate]
														AND (
															[LeaveType] <> leaRD.[LeaveType]
															OR leaDt.[RequestId] <> leaRD.[RequestId]
															)
														AND [FromTime] > leaRD.[FromTime]
														AND (
															woTt.[CurrentStatus] = 4
															OR woTt.[CurrentStatus] = 6
															)
													) THEN 1
											ELSE 0
											END
									END
							END
					ELSE 0
					END
			ELSE CASE 
					WHEN lea.[LeaveType] = 25
						AND woT.[CreatedDate] >= lea.[FromDate]
						AND empC.[WorkDate] = lea.[FromDate] THEN 1
					ELSE 0
					END END
			) -- Unpaid Leave Absent
		,(CASE 
			WHEN ISNULL(leaRD.[RequestId], '') <> '' THEN CASE 
					WHEN leaRD.[LeaveType] = 25 THEN CASE 
							WHEN woT.[CreatedDate] < leaRD.[OnDate] THEN CASE 
									WHEN leaRD.[IsHalfDay] = 1 THEN 0.5
									ELSE 1
									END
							ELSE CASE 
									WHEN leaRD.[OnDate] > lea.[FromDate]
										AND leaRD.[OnDate] <= lea.[ToDate] THEN CASE 
											WHEN leaRD.[IsHalfDay] = 1 THEN 0.5
											ELSE 1
											END
									WHEN leaRD.[OnDate] = (
											SELECT MIN(OnDate)
											FROM [dbo].[LeaveRequestDetail] WITH (NOLOCK)
											WHERE [RequestId] = leaRD.[RequestId]
											GROUP BY [RequestId]
											)
										AND leaRD.[IsHalfDay] = 1 THEN CASE 
											WHEN EXISTS (
													SELECT TOP 1 1
													FROM [dbo].[LeaveRequestDetail] AS leaDt WITH (NOLOCK)
														INNER JOIN [dbo].[WorkingTimeRequest] AS woTt WITH (NOLOCK) ON leaDt.[RequestId] = woTt.[RequestId]
													WHERE leaDt.[EmpId] = leaRD.[EmpId]
														AND leaDt.[OnDate] = leaRD.[OnDate]
														AND (
															leaDt.[LeaveType] <> leaRD.[LeaveType]
															OR leaDt.[RequestId] <> leaRD.[RequestId]
															)
														AND leaDt.[FromTime] < leaRD.[FromTime]
														AND (
															woTt.[CurrentStatus] = 4
															OR woTt.[CurrentStatus] = 6
															)
													) THEN 0.5
											ELSE 0
											END
									ELSE 0
									END
							END
					ELSE 0
					END
			ELSE CASE 
					WHEN lea.[LeaveType] = 25
						AND (
							(
								woT.[CreatedDate] >= lea.[FromDate]
								AND empC.[WorkDate] > lea.[FromDate]
								)
							OR woT.[CreatedDate] < lea.[FromDate]
							) THEN 1
					ELSE 0
					END END
			) -- Unpaid Leave
	FROM [dbo].[EmployeeCalendar] AS empC WITH (NOLOCK)
		LEFT JOIN [dbo].[LeaveRequest] AS lea WITH (NOLOCK) ON empC.[EmpId] = lea.[EmpId]
		AND CAST(empC.[WorkDate] AS DATE) >= CAST(lea.[FromDate] AS DATE)
		AND CAST(empC.[WorkDate] AS DATE) <= CAST(lea.[ToDate] AS DATE)
		LEFT JOIN [dbo].[WorkingTimeRequest] AS woT WITH (NOLOCK) ON lea.[RequestId] = woT.[RequestId]
		LEFT JOIN [dbo].[LeaveRequestDetail] AS leaRD WITH (NOLOCK) ON leaRD.[OnDate] = empC.[WorkDate]
		AND leaRD.[RequestId] = lea.[RequestId]
	WHERE CAST(empC.[WorkDate] AS DATE) <= CAST(GETDATE() AS DATE)
		AND empC.[WorkDate] >= @SyncFromDate
		AND (
			ISNULL(woT.[RequestId], '') = ''
			OR woT.[CurrentStatus] = 4
			OR woT.[CurrentStatus] = 6
			)
		AND (
			ISNULL(@EmpId, '') = ''
			OR empC.[EmpId] = @EmpId
			)
		AND (
			ISNULL(@WorkDate, '') = ''
			OR empC.[WorkDate] = @WorkDate
			)
		AND empC.[IsOffDay] = 0
	ORDER BY empC.WorkDate

	CREATE TABLE #EmployeeViolation3 (
		[WorkDate] DATETIME
		,[EmpId] INT
		,[Absent] INT
		,[UnpaidLeaveAbsent] INT
		,[UnpaidLeave] FLOAT
		)

	INSERT INTO #EmployeeViolation3 (
		[WorkDate]
		,[EmpId]
		,[Absent]
		,[UnpaidLeaveAbsent]
		,[UnpaidLeave]
		)
	SELECT [WorkDate]
		,[EmpId]
		,MAX([Absent])
		,MAX([UnpaidLeaveAbsent])
		,MAX([UnpaidLeave])
	FROM #EmployeeViolation2
	GROUP BY [EmpId]
		,[WorkDate]

	------- MISSING CHECK IN/OUT COMMENT ---------
	CREATE TABLE #MissingAllInOutTemp (
		EmpId INT
		,OnDate DATETIME
		,[CurrentStatus] INT
		,[Remark] NVARCHAR(500)
		)

	INSERT INTO #MissingAllInOutTemp (
		EmpId
		,OnDate
		,[CurrentStatus]
		,[Remark]
		)
	SELECT missA.[EmpId]
		,missA.[OnDate]
		,missA.[CurrentStatus]
		,missA.[Remark]
	FROM (
		SELECT missR.[EmpId]
			,missR.[OnDate]
			,wr.[Remark]
			,wr.[CurrentStatus]
		FROM (
			SELECT miss.[RequestId]
				,miss.[EmpId]
				,miss.[OnDate]
			FROM [dbo].[InoutMissRequest] AS miss WITH (NOLOCK)
				INNER JOIN [dbo].[WorkingTimeRequest] AS woT WITH (NOLOCK) ON miss.[RequestId] = woT.[RequestId]
				AND (
					woT.[CurrentStatus] = 4
					OR woT.[CurrentStatus] = 6
					)
				AND woT.[RequestType] IN (1, 2)
			) AS missR
			INNER JOIN (
			SELECT [RequestId]
				,[Remark]
				,[CurrentStatus]
			FROM [dbo].[WorkingTimeRequestHistory] AS wr1
			WHERE (
					[CurrentStatus] = 1 -- Employee
					OR [CurrentStatus] = 4 -- Approver
					)
				AND [RequestType] IN (1, 2)
				AND EXISTS (
					SELECT TOP 1 1
					FROM dbo.[WorkingTimeRequestHistory] WITH (NOLOCK)
					WHERE [RequestId] = wr1.[RequestId]
					GROUP BY [RequestId]
						,[CurrentStatus]
					HAVING wr1.[InsertedDate] = MAX(InsertedDate)
					)
			) AS wR ON missR.[RequestId] = wR.[RequestId]
		) AS missA
	WHERE missA.[OnDate] >= @SyncFromDate

	CREATE TABLE #EmployeeMissingInOut (
		EmpId INT
		,OnDate DATETIME
		,[CurrentStatus] INT
		,[InOutMissApproverComment] NVARCHAR(500)
		,[InOutMissEmployeeComment] NVARCHAR(500)
		)

	INSERT INTO #EmployeeMissingInOut (
		EmpId
		,OnDate
		,[CurrentStatus]
		,[InOutMissApproverComment]
		,[InOutMissEmployeeComment]
		)
	SELECT missR.[EmpId]
		,missR.[OnDate]
		,missR.[CurrentStatus]
		,(CASE 
			WHEN missR.[CurrentStatus] = 4 THEN missR.[Remark]
			ELSE '' END
			)
		,(CASE 
			WHEN missR.[CurrentStatus] = 1 THEN missR.[Remark]
			ELSE '' END
			)
	FROM (
		SELECT [EmpId]
			,[OnDate]
			,[CurrentStatus]
			,Remark = STUFF((
					SELECT ', ' + Remark
					FROM #MissingAllInOutTemp
					WHERE [EmpId] = missA.[EmpId]
						AND OnDate = missA.[OnDate]
						AND CurrentStatus = missA.[CurrentStatus]
					FOR XML PATH('')
						,TYPE
					).value('.[1]', 'nvarchar(max)'), 1, 2, '')
		FROM #MissingAllInOutTemp AS missA
		GROUP BY missA.[EmpId]
			,missA.[OnDate]
			,missA.[CurrentStatus]
		) AS missR

	------- LATE IN COMMENT ---------
	CREATE TABLE #EmployeeInComment (
		EmpId INT
		,FromDate DATETIME
		,ToDate DATETIME
		,[CurrentStatus] INT
		,[LateInApproverComment] NVARCHAR(500)
		,[LateInEmployeeComment] NVARCHAR(500)
		)

	INSERT INTO #EmployeeInComment (
		EmpId
		,FromDate
		,ToDate
		,[CurrentStatus]
		,[LateInApproverComment]
		,[LateInEmployeeComment]
		)
	SELECT inR.[EmpId]
		,inR.[FromDate]
		,inR.[ToDate]
		,wR.[CurrentStatus]
		,(CASE 
			WHEN wR.[CurrentStatus] = 4 THEN wR.[Remark]
			ELSE '' END
			)
		,(CASE 
			WHEN wR.[CurrentStatus] = 1 THEN wR.[Remark]
			ELSE '' END
			)
	FROM (
		SELECT inD.[RequestId]
			,inD.[EmpId]
			,inD.[LateInMins]
			,inD.[FromDate]
			,inD.[ToDate]
		FROM [dbo].[InOutRequest] AS inD WITH (NOLOCK)
			INNER JOIN [dbo].[WorkingTimeRequest] AS wo WITH (NOLOCK) ON inD.[RequestId] = wo.[RequestId]
			AND (
				[CurrentStatus] = 4
				OR [CurrentStatus] = 6
				)
			AND [RequestType] = 3 -- Late In
		) AS inR
		INNER JOIN (
		SELECT [RequestId]
			,[Remark]
			,[CurrentStatus]
		FROM [dbo].[WorkingTimeRequestHistory] AS wr1
		WHERE (
				[CurrentStatus] = 1 -- Employee
				OR [CurrentStatus] = 4 -- Approver
				)
			AND [RequestType] = 3 -- Late In
			AND EXISTS (
				SELECT TOP 1 1
				FROM dbo.[WorkingTimeRequestHistory] WITH (NOLOCK)
				WHERE [RequestId] = wr1.[RequestId]
				GROUP BY [RequestId]
					,[CurrentStatus]
				HAVING wr1.[InsertedDate] = MAX(InsertedDate)
				)
		) AS wR ON inR.[RequestId] = wR.[RequestId]
	WHERE inR.[LateInMins] > 0
		AND inR.[FromDate] >= @SyncFromDate
		AND (
			ISNULL(@EmpId, '') = ''
			OR inR.[EmpId] = @EmpId
			)
		AND (
			ISNULL(@WorkDate, '') = ''
			OR inR.[FromDate] = @WorkDate
			)

	------- EARLY OUT COMMENT ---------
	CREATE TABLE #EmployeeOutComment (
		EmpId INT
		,FromDate DATETIME
		,ToDate DATETIME
		,[CurrentStatus] INT
		,[EarlyOutApproverComment] NVARCHAR(500)
		,[EarlyOutEmployeeComment] NVARCHAR(500)
		)

	INSERT INTO #EmployeeOutComment (
		EmpId
		,FromDate
		,ToDate
		,[CurrentStatus]
		,[EarlyOutApproverComment]
		,[EarlyOutEmployeeComment]
		)
	SELECT outR.[EmpId]
		,outR.[FromDate]
		,outR.[ToDate]
		,wr.[CurrentStatus]
		,(CASE 
			WHEN wR.[CurrentStatus] = 4 THEN wR.[Remark]
			ELSE '' END
			)
		,(CASE 
			WHEN wR.[CurrentStatus] = 1 THEN wR.[Remark]
			ELSE '' END
			)
	FROM (
		SELECT inD.[RequestId]
			,inD.[EmpId]
			,inD.[EarlyOutMins]
			,inD.[FromDate]
			,inD.[ToDate]
		FROM [dbo].[InOutRequest] AS inD WITH (NOLOCK)
			INNER JOIN [dbo].[WorkingTimeRequest] AS wo WITH (NOLOCK) ON inD.[RequestId] = wo.[RequestId]
			AND (
				[CurrentStatus] = 4
				OR [CurrentStatus] = 6
				)
			AND [RequestType] = 4 -- Early Out
		) AS outR
		INNER JOIN (
		SELECT [RequestId]
			,[Remark]
			,[CurrentStatus]
		FROM [dbo].[WorkingTimeRequestHistory] AS wr1
		WHERE (
				[CurrentStatus] = 1 -- Employee
				OR [CurrentStatus] = 4 -- Approver
				)
			AND [RequestType] = 4 -- Early Out
			AND EXISTS (
				SELECT TOP 1 1
				FROM dbo.[WorkingTimeRequestHistory] WITH (NOLOCK)
				WHERE [RequestId] = wr1.[RequestId]
				GROUP BY [RequestId]
					,[CurrentStatus]
				HAVING wr1.[InsertedDate] = MAX(InsertedDate)
				)
		) AS wR ON outR.[RequestId] = wR.[RequestId]
	WHERE outR.[EarlyOutMins] > 0
		AND outR.[FromDate] >= @SyncFromDate
		AND (
			ISNULL(@EmpId, '') = ''
			OR outR.[EmpId] = @EmpId
			)
		AND (
			ISNULL(@WorkDate, '') = ''
			OR outR.[FromDate] = @WorkDate
			)

	------- CHANGE SHIFT COMMENT ---------
	CREATE TABLE #EmployeeChangeShiftComment (
		EmpId INT
		,FromDate DATETIME
		,ToDate DATETIME
		,[CurrentStatus] INT
		,[ChangeShiftApproverComment] NVARCHAR(500)
		,[ChangeShiftEmployeeComment] NVARCHAR(500)
		)

	INSERT INTO #EmployeeChangeShiftComment (
		EmpId
		,FromDate
		,ToDate
		,[CurrentStatus]
		,[ChangeShiftApproverComment]
		,[ChangeShiftEmployeeComment]
		)
	SELECT chR.[EmpId]
		,chR.[FromDate]
		,chR.[ToDate]
		,wr.[CurrentStatus]
		,(CASE 
			WHEN wR.[CurrentStatus] = 4 THEN wR.[Remark]
			ELSE '' END
			)
		,(CASE 
			WHEN wR.[CurrentStatus] = 1 THEN wR.[Remark]
			ELSE '' END
			)
	FROM (
		SELECT ch.[RequestId]
			,ch.[EmpId]
			,ch.[FromDate]
			,ch.[ToDate]
		FROM [dbo].[ChangeShiftRequest] AS ch WITH (NOLOCK)
			INNER JOIN [dbo].[WorkingTimeRequest] AS wo WITH (NOLOCK) ON ch.[RequestId] = wo.[RequestId]
			AND (
				[CurrentStatus] = 4
				OR [CurrentStatus] = 6
				)
			AND [RequestType] = 5 -- Change Shift
		) AS chR
		INNER JOIN (
		SELECT [RequestId]
			,[Remark]
			,[CurrentStatus]
		FROM [dbo].[WorkingTimeRequestHistory] AS wr1
		WHERE (
				[CurrentStatus] = 1 -- Employee
				OR [CurrentStatus] = 4 -- Approver
				)
			AND [RequestType] = 5 -- Change Shift
			AND EXISTS (
				SELECT TOP 1 1
				FROM dbo.[WorkingTimeRequestHistory] WITH (NOLOCK)
				WHERE [RequestId] = wr1.[RequestId]
				GROUP BY [RequestId]
					,[CurrentStatus]
				HAVING wr1.[InsertedDate] = MAX(InsertedDate)
				)
		) AS wR ON chR.[RequestId] = wR.[RequestId]
	WHERE chR.[FromDate] >= @SyncFromDate
		AND (
			ISNULL(@EmpId, '') = ''
			OR chR.[EmpId] = @EmpId
			)
		AND (
			ISNULL(@WorkDate, '') = ''
			OR chR.[FromDate] = @WorkDate
			)

	------- LEAVE REQUEST COMMENT ---------
	CREATE TABLE #EmployeeLeaveComment (
		EmpId INT
		,FromDate DATETIME
		,ToDate DATETIME
		,[CurrentStatus] INT
		,[LeaveApproverComment] NVARCHAR(500)
		,[LeaveEmployeeComment] NVARCHAR(500)
		)

	INSERT INTO #EmployeeLeaveComment (
		[EmpId]
		,[FromDate]
		,[ToDate]
		,[CurrentStatus]
		,[LeaveApproverComment]
		,[LeaveEmployeeComment]
		)
	SELECT leR.[EmpId]
		,leR.[FromDate]
		,leR.[ToDate]
		,wr.[CurrentStatus]
		,(CASE 
			WHEN wR.[CurrentStatus] = 4 THEN wR.[Remark]
			ELSE '' END
			)
		,(CASE 
			WHEN wR.[CurrentStatus] = 1 THEN wR.[Remark]
			ELSE '' END
			)
	FROM (
		SELECT leD.[RequestId]
			,leD.[EmpId]
			,leD.[FromDate]
			,leD.[ToDate]
		FROM (
			SELECT DISTINCT [RequestId]
				,[EmpId]
				,[FromDate]
				,[ToDate]
			FROM [dbo].[LeaveRequest] WITH (NOLOCK)
			) AS leD
			INNER JOIN [dbo].[WorkingTimeRequest] AS wo WITH (NOLOCK) ON leD.[RequestId] = wo.[RequestId]
			AND (
				[CurrentStatus] = 4
				OR [CurrentStatus] = 6
				)
			AND wo.[RequestType] = 6 -- Leave request
		) AS leR
		INNER JOIN (
		SELECT [RequestId]
			,[Remark]
			,[CurrentStatus]
		FROM [dbo].[WorkingTimeRequestHistory] AS wr1
		WHERE (
				[CurrentStatus] = 1 -- Employee
				OR [CurrentStatus] = 4 -- Approver
				)
			AND [RequestType] = 6 -- Leave request
			AND EXISTS (
				SELECT TOP 1 1
				FROM dbo.[WorkingTimeRequestHistory] WITH (NOLOCK)
				WHERE [RequestId] = wr1.[RequestId]
				GROUP BY [RequestId]
					,[CurrentStatus]
				HAVING wr1.[InsertedDate] = MAX(InsertedDate)
				)
		) AS wR ON leR.[RequestId] = wR.[RequestId]
	WHERE leR.FromDate >= @SyncFromDate
		AND (
			ISNULL(@EmpId, '') = ''
			OR leR.[EmpId] = @EmpId
			)
		AND (
			ISNULL(@WorkDate, '') = ''
			OR (
				DATEDIFF(DD, leR.FromDate, @WorkDate) >= 0
				AND DATEDIFF(DD, @WorkDate, leR.ToDate) >= 0
			)
			)

	------- INSERT INTO VIOLATION TABLE ---------
	INSERT INTO [dbo].[EmployeeAttendanceViolation] (
		[WorkDate]
		,[EmpId]
		,[MissingCheckInOut]
		,[LateIn]
		,[EarlyOut]
		,[WorkingBelowTime]
		,[Absent]
		,[UnpaidLeaveAbsent]
		,[UnpaidLeave]
		,[ChangeShift]
		,[ShiftName]
		,[InFixed]
		,[OutFixed]
		,[TotalWorkHours]
		,[LateInValue]
		,[EarlyOutValue]
		,[LateInApproverComment]
		,[LateInEmployeeComment]
		,[EarlyOutApproverComment]
		,[EarlyOutEmployeeComment]
		,[ChangeShiftApproverComment]
		,[ChangeShiftEmployeeComment]
		,[LeaveApproverComment]
		,[LeaveEmployeeComment]
		,[InOutMissApproverComment]
		,[InOutMissEmployeeComment]
		,[ModifiedDate]
		,[IsDeleted]
		)
	SELECT empV1.[WorkDate]
		,empV1.[EmpId]
		,empV1.[MissingCheckInOut]
		,CASE 
			WHEN ISNULL(empV3.[Absent], '') <> '' THEN 0
			ELSE empV1.[LateIn]
			END
		,CASE 
			WHEN ISNULL(empV3.[Absent], '') <> '' THEN 0
			ELSE empV1.[EarlyOut]
			END
		,empV1.[WorkingBelowTime]
		,(CASE 
			WHEN ISNULL(empV3.[Absent], '') <> '' THEN empV3.[Absent]
			ELSE 0 END
			)
		,(CASE 
			WHEN ISNULL(empV3.[UnpaidLeaveAbsent], '') <> '' THEN empV3.[UnpaidLeaveAbsent]
			ELSE 0 END
			)
		,(CASE 
			WHEN ISNULL(empV3.[UnpaidLeave], '') <> '' THEN empV3.[UnpaidLeave]
			ELSE 0 END
			)
		,(CASE 
			WHEN ISNULL(empV1.[ChangeShift], '') <> '' THEN empV1.[ChangeShift]
			ELSE 0 END
			)
		,CASE 
			WHEN ISNULL(empA.[ShiftId], '') <> '' THEN sh.[Name]
			ELSE NULL
			END
		,CASE 
			WHEN ISNULL(empA.[ShiftId], '') <> '' THEN empA.[InFixed]
			ELSE NULL
			END
		,CASE 
			WHEN ISNULL(empA.[ShiftId], '') <> '' THEN empA.[OutFixed]
			ELSE NULL
			END
		,CASE 
			WHEN ISNULL(empA.[ShiftId], '') <> '' THEN empA.[TotalWorkHours]
			ELSE NULL
			END
		,CASE 
			WHEN ISNULL(empA.[ShiftId], '') <> '' THEN CASE 
					WHEN EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
								INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
								AND workR.[CurrentStatus] IN (4, 6)
								AND workR.[RequestType] = 3
							WHERE empA.[AttendanceDate] = inR.[FromDate]
								AND empA.[EmpId] = inR.[EmpId]
							) THEN (
							SELECT inR.[LateInMins]
							FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
								INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
								AND workR.[CurrentStatus] IN (4, 6)
								AND workR.[RequestType] = 3
							WHERE empA.[AttendanceDate] = inR.[FromDate]
								AND empA.[EmpId] = inR.[EmpId]
							)
					ELSE empA.[LateIn]
					END
			ELSE NULL
			END
		,CASE 
			WHEN ISNULL(empA.[ShiftId], '') <> '' THEN CASE 
					WHEN EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
								INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
								AND workR.[CurrentStatus] IN (4, 6)
								AND workR.[RequestType] = 4
							WHERE empA.[AttendanceDate] = inR.[FromDate]
								AND empA.[EmpId] = inR.[EmpId]
							) THEN (
							SELECT inR.[EarlyOutMins]
							FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
								INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
								AND workR.[CurrentStatus] IN (4, 6)
								AND workR.[RequestType] = 4
							WHERE empA.[AttendanceDate] = inR.[FromDate]
								AND empA.[EmpId] = inR.[EmpId]
							)
					ELSE empA.[EarlyOut]
					END
			ELSE NULL
			END
		,(
			SELECT [LateInApproverComment]
			FROM #EmployeeInComment
			WHERE empV1.[EmpId] = [EmpId]
				AND empV1.[WorkDate] >= [FromDate]
				AND empV1.[WorkDate] <= [ToDate]
				AND [CurrentStatus] = 4
			)
		,(
			SELECT [LateInEmployeeComment]
			FROM #EmployeeInComment
			WHERE empV1.[EmpId] = [EmpId]
				AND empV1.[WorkDate] >= [FromDate]
				AND empV1.[WorkDate] <= [ToDate]
				AND [CurrentStatus] = 1
			)
		,(
			SELECT [EarlyOutApproverComment]
			FROM #EmployeeOutComment
			WHERE empV1.[WorkDate] >= [FromDate]
				AND empV1.[WorkDate] <= [ToDate]
				AND [CurrentStatus] = 4
				AND empV1.[EmpId] = [EmpId]
			)
		,(
			SELECT [EarlyOutEmployeeComment]
			FROM #EmployeeOutComment
			WHERE empV1.[WorkDate] >= [FromDate]
				AND empV1.[WorkDate] <= [ToDate]
				AND [CurrentStatus] = 1
				AND empV1.[EmpId] = [EmpId]
			)
		,(
			SELECT [ChangeShiftApproverComment]
			FROM #EmployeeChangeShiftComment
			WHERE empV1.[WorkDate] >= [FromDate]
				AND empV1.[WorkDate] <= [ToDate]
				AND [CurrentStatus] = 4
				AND empV1.[EmpId] = [EmpId]
			)
		,(
			SELECT [ChangeShiftEmployeeComment]
			FROM #EmployeeChangeShiftComment
			WHERE empV1.[WorkDate] >= [FromDate]
				AND empV1.[WorkDate] <= [ToDate]
				AND [CurrentStatus] = 1
				AND empV1.[EmpId] = [EmpId]
			)
		,(
			SELECT [LeaveApproverComment]
			FROM #EmployeeLeaveComment
			WHERE --empV1.[WorkDate] = [OnDate]
				DATEDIFF(DD, FromDate, empV1.[WorkDate]) >= 0
				AND DATEDIFF(DD, empV1.[WorkDate], ToDate) >= 0
				AND [CurrentStatus] = 4
				AND empV1.[EmpId] = [EmpId]
			)
		,(
			SELECT [LeaveEmployeeComment]
			FROM #EmployeeLeaveComment
			WHERE -- empV1.[WorkDate] = [OnDate]
				DATEDIFF(DD, FromDate, empV1.[WorkDate]) >= 0
				AND DATEDIFF(DD, empV1.[WorkDate], ToDate) >= 0
				AND [CurrentStatus] = 1
				AND empV1.[EmpId] = [EmpId]
			)
		,(
			SELECT [InOutMissApproverComment]
			FROM #EmployeeMissingInOut
			WHERE empV1.[WorkDate] = [OnDate]
				AND [CurrentStatus] = 4
				AND empV1.[EmpId] = [EmpId]
			)
		,(
			SELECT [InOutMissEmployeeComment]
			FROM #EmployeeMissingInOut
			WHERE empV1.[WorkDate] = [OnDate]
				AND [CurrentStatus] = 1
				AND empV1.[EmpId] = [EmpId]
			)
		,GETDATE()
		,NULL
	FROM #EmployeeViolation1 AS empV1
		LEFT JOIN [dbo].[EmployeeAttendanceViolation] AS empAV WITH (NOLOCK) ON empV1.[EmpId] = empAV.[EmpId]
		AND empV1.[WorkDate] = empAV.[WorkDate]
		LEFT JOIN #EmployeeViolation3 AS empV3 ON empV1.[WorkDate] = empV3.[WorkDate]
		AND empV1.[EmpId] = empV3.[EmpId]
		LEFT JOIN [dbo].[EmployeeAttendance] AS empA WITH (NOLOCK) ON empV1.[WorkDate] = empA.[AttendanceDate]
		AND empV1.[EmpId] = empA.[EmpId]
		LEFT JOIN [dbo].[Shift] AS sh WITH (NOLOCK) ON empA.[ShiftId] = sh.[ShiftID]
	WHERE empV1.[WorkDate] >= @SyncFromDate
		AND (
			ISNULL(@EmpId, '') = ''
			OR empV1.[EmpId] = @EmpId
			)
		AND (
			ISNULL(@WorkDate, '') = ''
			OR empV1.[WorkDate] = @WorkDate
			)
		AND ISNULL(empAV.[EmpId], '') = ''

	INSERT INTO [dbo].[EmployeeAttendanceViolation] (
		[WorkDate]
		,[EmpId]
		,[MissingCheckInOut]
		,[LateIn]
		,[EarlyOut]
		,[WorkingBelowTime]
		,[Absent]
		,[UnpaidLeaveAbsent]
		,[UnpaidLeave]
		,[ChangeShift]
		,[ShiftName]
		,[InFixed]
		,[OutFixed]
		,[TotalWorkHours]
		,[LateInValue]
		,[EarlyOutValue]
		,[LateInApproverComment]
		,[LateInEmployeeComment]
		,[EarlyOutApproverComment]
		,[EarlyOutEmployeeComment]
		,[ChangeShiftApproverComment]
		,[ChangeShiftEmployeeComment]
		,[LeaveApproverComment]
		,[LeaveEmployeeComment]
		,[InOutMissApproverComment]
		,[InOutMissEmployeeComment]
		,[ModifiedDate]
		,[IsDeleted]
		)
	SELECT empV3.[WorkDate]
		,empV3.[EmpId]
		,0
		,0
		,0
		,0
		,empV3.[Absent]
		,empV3.[UnpaidLeaveAbsent]
		,empV3.[UnpaidLeave]
		,0
		,CASE 
			WHEN ISNULL(empA.[ShiftId], '') <> '' THEN sh.[Name]
			ELSE NULL
			END
		,CASE 
			WHEN ISNULL(empA.[ShiftId], '') <> '' THEN empA.[InFixed]
			ELSE NULL
			END
		,CASE 
			WHEN ISNULL(empA.[ShiftId], '') <> '' THEN empA.[OutFixed]
			ELSE NULL
			END
		,CASE 
			WHEN ISNULL(empA.[ShiftId], '') <> '' THEN empA.[TotalWorkHours]
			ELSE NULL
			END
		,CASE 
			WHEN ISNULL(empA.[ShiftId], '') <> '' THEN CASE 
					WHEN EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
								INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
								AND workR.[CurrentStatus] = 6
								AND workR.[RequestType] = 3
							WHERE empA.[AttendanceDate] = inR.[FromDate]
								AND empA.[EmpId] = inR.[EmpId]
							) THEN (
							SELECT inR.[LateInMins]
							FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
								INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
								AND workR.[CurrentStatus] IN (4, 6)
								AND workR.[RequestType] = 3
							WHERE empA.[AttendanceDate] = inR.[FromDate]
								AND empA.[EmpId] = inR.[EmpId]
							)
					ELSE empA.[LateIn]
					END
			ELSE NULL
			END
		,CASE 
			WHEN ISNULL(empA.[ShiftId], '') <> '' THEN CASE 
					WHEN EXISTS (
							SELECT TOP 1 1
							FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
								INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
								AND workR.[CurrentStatus] = 6
								AND workR.[RequestType] = 4
							WHERE empA.[AttendanceDate] = inR.[FromDate]
							) THEN (
							SELECT inR.[EarlyOutMins]
							FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
								INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
								AND workR.[CurrentStatus] IN (4, 6)
								AND workR.[RequestType] = 4
							WHERE empA.[AttendanceDate] = inR.[FromDate]
								AND empA.[EmpId] = inR.[EmpId]
							)
					ELSE empA.[EarlyOut]
					END
			ELSE NULL
			END
		,(
			SELECT [LateInApproverComment]
			FROM #EmployeeInComment
			WHERE empV3.[EmpId] = [EmpId]
				AND empV3.[WorkDate] >= [FromDate]
				AND empV3.[WorkDate] <= [ToDate]
				AND [CurrentStatus] = 4
			)
		,(
			SELECT [LateInEmployeeComment]
			FROM #EmployeeInComment
			WHERE empV3.[EmpId] = [EmpId]
				AND empV3.[WorkDate] >= [FromDate]
				AND empV3.[WorkDate] <= [ToDate]
				AND [CurrentStatus] = 1
			)
		,(
			SELECT [EarlyOutApproverComment]
			FROM #EmployeeOutComment
			WHERE empV3.[WorkDate] >= [FromDate]
				AND empV3.[WorkDate] <= [ToDate]
				AND [CurrentStatus] = 4
				AND empV3.[EmpId] = [EmpId]
			)
		,(
			SELECT [EarlyOutEmployeeComment]
			FROM #EmployeeOutComment
			WHERE empV3.[WorkDate] >= [FromDate]
				AND empV3.[WorkDate] <= [ToDate]
				AND [CurrentStatus] = 1
				AND empV3.[EmpId] = [EmpId]
			)
		,(
			SELECT [ChangeShiftApproverComment]
			FROM #EmployeeChangeShiftComment
			WHERE empV3.[WorkDate] >= [FromDate]
				AND empV3.[WorkDate] <= [ToDate]
				AND [CurrentStatus] = 4
				AND empV3.[EmpId] = [EmpId]
			)
		,(
			SELECT [ChangeShiftEmployeeComment]
			FROM #EmployeeChangeShiftComment
			WHERE empV3.[WorkDate] >= [FromDate]
				AND empV3.[WorkDate] <= [ToDate]
				AND [CurrentStatus] = 1
				AND empV3.[EmpId] = [EmpId]
			)
		,(
			SELECT [LeaveApproverComment]
			FROM #EmployeeLeaveComment
			WHERE -- empV3.[WorkDate] = [OnDate]
				DATEDIFF(DD, FromDate, empV3.[WorkDate]) >= 0
				AND DATEDIFF(DD, empV3.[WorkDate], ToDate) >= 0
				AND [CurrentStatus] = 4
				AND empV3.[EmpId] = [EmpId]
			)
		,(
			SELECT [LeaveEmployeeComment]
			FROM #EmployeeLeaveComment
			WHERE -- empV3.[WorkDate] = [OnDate]
				DATEDIFF(DD, FromDate, empV3.[WorkDate]) >= 0
				AND DATEDIFF(DD, empV3.[WorkDate], ToDate) >= 0
				AND [CurrentStatus] = 1
				AND empV3.[EmpId] = [EmpId]
			)
		,(
			SELECT [InOutMissApproverComment]
			FROM #EmployeeMissingInOut
			WHERE empV3.[WorkDate] = [OnDate]
				AND [CurrentStatus] = 4
				AND empV3.[EmpId] = [EmpId]
			)
		,(
			SELECT [InOutMissEmployeeComment]
			FROM #EmployeeMissingInOut
			WHERE empV3.[WorkDate] = [OnDate]
				AND [CurrentStatus] = 1
				AND empV3.[EmpId] = [EmpId]
			)
		,GETDATE()
		,NULL
	FROM #EmployeeViolation3 AS empV3
		LEFT JOIN [dbo].[EmployeeAttendanceViolation] AS empAV WITH (NOLOCK) ON empV3.[EmpId] = empAV.[EmpId]
		AND empV3.[WorkDate] = empAV.[WorkDate]
		LEFT JOIN [dbo].[EmployeeAttendance] AS empA WITH (NOLOCK) ON empV3.[WorkDate] = empA.[AttendanceDate]
		AND empV3.[EmpId] = empA.[EmpId]
		LEFT JOIN [dbo].[Shift] AS sh WITH (NOLOCK) ON empA.[ShiftId] = sh.[ShiftID]
	WHERE NOT EXISTS (
			SELECT TOP 1 1
			FROM #EmployeeViolation1
			WHERE [EmpId] = empV3.[EmpId]
				AND [WorkDate] = empV3.[WorkDate]
			)
		AND empV3.[WorkDate] >= @SyncFromDate
		AND (
			ISNULL(@EmpId, '') = ''
			OR empV3.[EmpId] = @EmpId
			)
		AND (
			ISNULL(@WorkDate, '') = ''
			OR empV3.[WorkDate] = @WorkDate
			)
		AND ISNULL(empAV.[EmpId], '') = ''

	--IF ISNULL(@EmpId, '') <> ''
	--	AND ISNULL(@WorkDate, '') <> ''
	--BEGIN
		UPDATE empAV
		SET [WorkDate] = empV1.[WorkDate]
			,[EmpId] = empV1.[EmpId]
			,[MissingCheckInOut] = empV1.[MissingCheckInOut]
			,[LateIn] = CASE 
				WHEN ISNULL(empV3.[Absent], '') <> '' THEN 0
				ELSE empV1.[LateIn]
				END
			,[EarlyOut] = CASE 
				WHEN ISNULL(empV3.[Absent], '') <> '' THEN 0
				ELSE empV1.[EarlyOut]
				END
			,[WorkingBelowTime] = empV1.[WorkingBelowTime]
			,[Absent] = CASE 
				WHEN ISNULL(empV3.[Absent], '') <> '' THEN empV3.[Absent]
				ELSE 0
				END
			,[UnpaidLeaveAbsent] = CASE 
				WHEN ISNULL(empV3.[UnpaidLeaveAbsent], '') <> '' THEN empV3.[UnpaidLeaveAbsent]
				ELSE 0
				END
			,[UnpaidLeave] = CASE 
				WHEN ISNULL(empV3.[UnpaidLeave], '') <> '' THEN empV3.[UnpaidLeave]
				ELSE 0
				END
			,[ChangeShift] = CASE 
				WHEN ISNULL(empV1.[ChangeShift], '') <> '' THEN empV1.[ChangeShift]
				ELSE 0
				END
			,[ShiftName] = CASE 
				WHEN ISNULL(empA.[ShiftId], '') <> '' THEN sh.[Name]
				ELSE NULL
				END
			,[InFixed] = CASE 
				WHEN ISNULL(empA.[ShiftId], '') <> '' THEN empA.[InFixed]
				ELSE NULL
				END
			,[OutFixed] = CASE 
				WHEN ISNULL(empA.[ShiftId], '') <> '' THEN empA.[OutFixed]
				ELSE NULL
				END
			,[TotalWorkHours] = CASE 
				WHEN ISNULL(empA.[ShiftId], '') <> '' THEN empA.[TotalWorkHours]
				ELSE NULL
				END
			,[LateInValue] = CASE 
				WHEN ISNULL(empA.[ShiftId], '') <> '' THEN CASE 
						WHEN EXISTS (
								SELECT TOP 1 1
								FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
									INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
									AND workR.[CurrentStatus] IN (4, 6)
									AND workR.[RequestType] = 3
								WHERE empA.[AttendanceDate] = inR.[FromDate]
									AND empA.[EmpId] = inR.[EmpId]
								) THEN (
								SELECT inR.[LateInMins]
								FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
									INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
									AND workR.[CurrentStatus] IN (4, 6)
									AND workR.[RequestType] = 3
								WHERE empA.[AttendanceDate] = inR.[FromDate]
									AND empA.[EmpId] = inR.[EmpId]
								)
						ELSE empA.[LateIn]
						END
				ELSE NULL
				END
			,[EarlyOutValue] = CASE 
				WHEN ISNULL(empA.[ShiftId], '') <> '' THEN CASE 
						WHEN EXISTS (
								SELECT TOP 1 1
								FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
									INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
									AND workR.[CurrentStatus] IN (4, 6)
									AND workR.[RequestType] = 4
								WHERE empA.[AttendanceDate] = inR.[FromDate]
									AND empA.[EmpId] = inR.[EmpId]
								) THEN (
								SELECT inR.[EarlyOutMins]
								FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
									INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
									AND workR.[CurrentStatus] IN (4, 6)
									AND workR.[RequestType] = 4
								WHERE empA.[AttendanceDate] = inR.[FromDate]
									AND empA.[EmpId] = inR.[EmpId]
								)
						ELSE empA.[EarlyOut]
						END
				ELSE NULL
				END
			,[LateInApproverComment] = (
				SELECT [LateInApproverComment]
				FROM #EmployeeInComment
				WHERE empV1.[EmpId] = [EmpId]
					AND empV1.[WorkDate] >= [FromDate]
					AND empV1.[WorkDate] <= [ToDate]
					AND [CurrentStatus] = 4
				)
			,[LateInEmployeeComment] = (
				SELECT [LateInEmployeeComment]
				FROM #EmployeeInComment
				WHERE empV1.[EmpId] = [EmpId]
					AND empV1.[WorkDate] >= [FromDate]
					AND empV1.[WorkDate] <= [ToDate]
					AND [CurrentStatus] = 1
				)
			,[EarlyOutApproverComment] = (
				SELECT [EarlyOutApproverComment]
				FROM #EmployeeOutComment
				WHERE empV1.[WorkDate] >= [FromDate]
					AND empV1.[WorkDate] <= [ToDate]
					AND [CurrentStatus] = 4
					AND empV1.[EmpId] = [EmpId]
				)
			,[EarlyOutEmployeeComment] = (
				SELECT [EarlyOutEmployeeComment]
				FROM #EmployeeOutComment
				WHERE empV1.[WorkDate] >= [FromDate]
					AND empV1.[WorkDate] <= [ToDate]
					AND [CurrentStatus] = 1
					AND empV1.[EmpId] = [EmpId]
				)
			,[ChangeShiftApproverComment] = (
				SELECT [ChangeShiftApproverComment]
				FROM #EmployeeChangeShiftComment
				WHERE empV1.[WorkDate] >= [FromDate]
					AND empV1.[WorkDate] <= [ToDate]
					AND [CurrentStatus] = 4
					AND empV1.[EmpId] = [EmpId]
				)
			,[ChangeShiftEmployeeComment] = (
				SELECT [ChangeShiftEmployeeComment]
				FROM #EmployeeChangeShiftComment
				WHERE empV1.[WorkDate] >= [FromDate]
					AND empV1.[WorkDate] <= [ToDate]
					AND [CurrentStatus] = 1
					AND empV1.[EmpId] = [EmpId]
				)
			,[LeaveApproverComment] = (
				SELECT TOP 1 [LeaveApproverComment]
				FROM #EmployeeLeaveComment
				WHERE -- empV1.[WorkDate] = [OnDate]
					DATEDIFF(DD, FromDate, empV1.[WorkDate]) >= 0
					AND DATEDIFF(DD, empV1.[WorkDate], ToDate) >= 0
					AND [CurrentStatus] = 4
					AND empV1.[EmpId] = [EmpId]
				)
			,[LeaveEmployeeComment] = (
				SELECT TOP 1 [LeaveEmployeeComment]
				FROM #EmployeeLeaveComment
				WHERE -- empV1.[WorkDate] = [OnDate]
					DATEDIFF(DD, FromDate, empV1.[WorkDate]) >= 0
					AND DATEDIFF(DD, empV1.[WorkDate], ToDate) >= 0
					AND [CurrentStatus] = 1
					AND empV1.[EmpId] = [EmpId]
				)
			,[InOutMissApproverComment] = (
				SELECT [InOutMissApproverComment]
				FROM #EmployeeMissingInOut
				WHERE empV1.[WorkDate] = [OnDate]
					AND [CurrentStatus] = 4
					AND empV1.[EmpId] = [EmpId]
				)
			,[InOutMissEmployeeComment] = (
				SELECT [InOutMissEmployeeComment]
				FROM #EmployeeMissingInOut
				WHERE empV1.[WorkDate] = [OnDate]
					AND [CurrentStatus] = 1
					AND empV1.[EmpId] = [EmpId]
				)
			,[ModifiedDate] = GETDATE()
			,[IsDeleted] = NULL
		FROM [dbo].[EmployeeAttendanceViolation] AS empAV WITH (NOLOCK)
			INNER JOIN #EmployeeViolation1 AS empV1 ON empV1.[EmpId] = empAV.[EmpId]
			AND empV1.[WorkDate] = empAV.[WorkDate]
			LEFT JOIN #EmployeeViolation3 AS empV3 ON empV1.[WorkDate] = empV3.[WorkDate]
			AND empV1.[EmpId] = empV3.[EmpId]
			LEFT JOIN [dbo].[EmployeeAttendance] AS empA WITH (NOLOCK) ON empV1.[WorkDate] = empA.[AttendanceDate]
			AND empV1.[EmpId] = empA.[EmpId]
			LEFT JOIN [dbo].[Shift] AS sh WITH (NOLOCK) ON empA.[ShiftId] = sh.[ShiftID]
		WHERE empV1.[WorkDate] >= @SyncFromDate
			AND (
				ISNULL(@EmpId, '') = ''
				OR empV1.[EmpId] = @EmpId
				)
			AND (
				ISNULL(@WorkDate, '') = ''
				OR empV1.[WorkDate] = @WorkDate
				)

		UPDATE empAV
		SET [WorkDate] = empV3.[WorkDate]
			,[EmpId] = empV3.[EmpId]
			,[MissingCheckInOut] = 0
			,[LateIn] = 0
			,[EarlyOut] = 0
			,[WorkingBelowTime] = 0
			,[Absent] = CASE 
				WHEN ISNULL(empV3.[Absent], '') <> '' THEN empV3.[Absent]
				ELSE 0
				END
			,[UnpaidLeaveAbsent] = CASE 
				WHEN ISNULL(empV3.[UnpaidLeaveAbsent], '') <> '' THEN empV3.[UnpaidLeaveAbsent]
				ELSE 0
				END
			,[UnpaidLeave] = CASE 
				WHEN ISNULL(empV3.[UnpaidLeave], '') <> '' THEN empV3.[UnpaidLeave]
				ELSE 0
				END
			,[ChangeShift] = 0
			,[ShiftName] = CASE 
				WHEN ISNULL(empA.[ShiftId], '') <> '' THEN sh.[Name]
				ELSE NULL
				END
			,[InFixed] = CASE 
				WHEN ISNULL(empA.[ShiftId], '') <> '' THEN empA.[InFixed]
				ELSE NULL
				END
			,[OutFixed] = CASE 
				WHEN ISNULL(empA.[ShiftId], '') <> '' THEN empA.[OutFixed]
				ELSE NULL
				END
			,[TotalWorkHours] = CASE 
				WHEN ISNULL(empA.[ShiftId], '') <> '' THEN empA.[TotalWorkHours]
				ELSE NULL
				END
			,[LateInValue] = CASE 
				WHEN ISNULL(empA.[ShiftId], '') <> '' THEN CASE 
						WHEN EXISTS (
								SELECT TOP 1 1
								FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
									INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
									AND workR.[CurrentStatus] = 6
									AND workR.[RequestType] = 3
								WHERE empA.[AttendanceDate] = inR.[FromDate]
									AND empA.[EmpId] = inR.[EmpId]
								) THEN (
								SELECT inR.[LateInMins]
								FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
									INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
									AND workR.[CurrentStatus] IN (4, 6)
									AND workR.[RequestType] = 3
								WHERE empA.[AttendanceDate] = inR.[FromDate]
									AND empA.[EmpId] = inR.[EmpId]
								)
						ELSE empA.[LateIn]
						END
				ELSE NULL
				END
			,[EarlyOutValue] = CASE 
				WHEN ISNULL(empA.[ShiftId], '') <> '' THEN CASE 
						WHEN EXISTS (
								SELECT TOP 1 1
								FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
									INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
									AND workR.[CurrentStatus] = 6
									AND workR.[RequestType] = 4
								WHERE empA.[AttendanceDate] = inR.[FromDate]
								) THEN (
								SELECT inR.[EarlyOutMins]
								FROM [dbo].[InOutRequest] AS inR WITH (NOLOCK)
									INNER JOIN [dbo].[WorkingTimeRequest] AS workR WITH (NOLOCK) ON inR.[RequestId] = workR.[RequestId]
									AND workR.[CurrentStatus] IN (4, 6)
									AND workR.[RequestType] = 4
								WHERE empA.[AttendanceDate] = inR.[FromDate]
									AND empA.[EmpId] = inR.[EmpId]
								)
						ELSE empA.[EarlyOut]
						END
				ELSE NULL
				END
			,[LateInApproverComment] = (
				SELECT [LateInApproverComment]
				FROM #EmployeeInComment
				WHERE empV3.[EmpId] = [EmpId]
					AND empV3.[WorkDate] >= [FromDate]
					AND empV3.[WorkDate] <= [ToDate]
					AND [CurrentStatus] = 4
				)
			,[LateInEmployeeComment] = (
				SELECT [LateInEmployeeComment]
				FROM #EmployeeInComment
				WHERE empV3.[EmpId] = [EmpId]
					AND empV3.[WorkDate] >= [FromDate]
					AND empV3.[WorkDate] <= [ToDate]
					AND [CurrentStatus] = 1
				)
			,[EarlyOutApproverComment] = (
				SELECT [EarlyOutApproverComment]
				FROM #EmployeeOutComment
				WHERE empV3.[WorkDate] >= [FromDate]
					AND empV3.[WorkDate] <= [ToDate]
					AND [CurrentStatus] = 4
					AND empV3.[EmpId] = [EmpId]
				)
			,[EarlyOutEmployeeComment] = (
				SELECT [EarlyOutEmployeeComment]
				FROM #EmployeeOutComment
				WHERE empV3.[WorkDate] >= [FromDate]
					AND empV3.[WorkDate] <= [ToDate]
					AND [CurrentStatus] = 1
					AND empV3.[EmpId] = [EmpId]
				)
			,[ChangeShiftApproverComment] = (
				SELECT [ChangeShiftApproverComment]
				FROM #EmployeeChangeShiftComment
				WHERE empV3.[WorkDate] >= [FromDate]
					AND empV3.[WorkDate] <= [ToDate]
					AND [CurrentStatus] = 4
					AND empV3.[EmpId] = [EmpId]
				)
			,[ChangeShiftEmployeeComment] = (
				SELECT [ChangeShiftEmployeeComment]
				FROM #EmployeeChangeShiftComment
				WHERE empV3.[WorkDate] >= [FromDate]
					AND empV3.[WorkDate] <= [ToDate]
					AND [CurrentStatus] = 1
					AND empV3.[EmpId] = [EmpId]
				)
			,[LeaveApproverComment] = (
				SELECT [LeaveApproverComment]
				FROM #EmployeeLeaveComment
				WHERE -- empV3.[WorkDate] = [OnDate]
					DATEDIFF(DD, FromDate, empV3.[WorkDate]) >= 0
					AND DATEDIFF(DD, empV3.[WorkDate], ToDate) >= 0
					AND [CurrentStatus] = 4
					AND empV3.[EmpId] = [EmpId]
				)
			,[LeaveEmployeeComment] = (
				SELECT [LeaveEmployeeComment]
				FROM #EmployeeLeaveComment
				WHERE -- empV3.[WorkDate] = [OnDate]
					DATEDIFF(DD, FromDate, empV3.[WorkDate]) >= 0
					AND DATEDIFF(DD, empV3.[WorkDate], ToDate) >= 0
					AND [CurrentStatus] = 1
					AND empV3.[EmpId] = [EmpId]
				)
			,[InOutMissApproverComment] = (
				SELECT [InOutMissApproverComment]
				FROM #EmployeeMissingInOut
				WHERE empV3.[WorkDate] = [OnDate]
					AND [CurrentStatus] = 4
					AND empV3.[EmpId] = [EmpId]
				)
			,[InOutMissEmployeeComment] = (
				SELECT [InOutMissEmployeeComment]
				FROM #EmployeeMissingInOut
				WHERE empV3.[WorkDate] = [OnDate]
					AND [CurrentStatus] = 1
					AND empV3.[EmpId] = [EmpId]
				)
			,[ModifiedDate] = GETDATE()
			,[IsDeleted] = NULL
		FROM #EmployeeViolation3 AS empV3
			INNER JOIN [dbo].[EmployeeAttendanceViolation] AS empAV WITH (NOLOCK) ON empV3.[EmpId] = empAV.[EmpId]
			AND empV3.[WorkDate] = empAV.[WorkDate]
			LEFT JOIN [dbo].[EmployeeAttendance] AS empA WITH (NOLOCK) ON empV3.[WorkDate] = empA.[AttendanceDate]
			AND empV3.[EmpId] = empA.[EmpId]
			LEFT JOIN [dbo].[Shift] AS sh WITH (NOLOCK) ON empA.[ShiftId] = sh.[ShiftID]
		WHERE NOT EXISTS (
				SELECT TOP 1 1
				FROM #EmployeeViolation1
				WHERE [EmpId] = empV3.[EmpId]
					AND [WorkDate] = empV3.[WorkDate]
				)
			AND empV3.[WorkDate] >= @SyncFromDate
			AND (
				ISNULL(@EmpId, '') = ''
				OR empV3.[EmpId] = @EmpId
				)
			AND (
				ISNULL(@WorkDate, '') = ''
				OR empV3.[WorkDate] = @WorkDate
				)

		IF NOT EXISTS (
				SELECT TOP 1 1
				FROM #EmployeeViolation1
				)
			AND NOT EXISTS (
				SELECT TOP 1 1
				FROM #EmployeeViolation3
				)
		BEGIN
			UPDATE [dbo].[EmployeeAttendanceViolation] WITH (ROWLOCK, UPDLOCK)
			SET [MissingCheckInOut] = 0
				,[LateIn] = 0
				,[EarlyOut] = 0
				,[WorkingBelowTime] = 0
				,[Absent] = 0
				,[UnpaidLeaveAbsent] = 0
				,[UnpaidLeave] = 0
				,[ChangeShift] = 0
				,[ShiftName] = 0
				,[InFixed] = 0
				,[OutFixed] = 0
				,[TotalWorkHours] = 0
				,[LateInValue] = 0
				,[EarlyOutValue] = 0
				,[LateInApproverComment] = NULL
				,[LateInEmployeeComment] = NULL
				,[EarlyOutApproverComment] = NULL
				,[EarlyOutEmployeeComment] = NULL
				,[ChangeShiftApproverComment] = NULL
				,[ChangeShiftEmployeeComment] = NULL
				,[LeaveApproverComment] = NULL
				,[LeaveEmployeeComment] = NULL
				,[InOutMissApproverComment] = NULL
				,[InOutMissEmployeeComment] = NULL
				,[ModifiedDate] = GETDATE()
			WHERE [EmpId] = @EmpId
				AND [WorkDate] = @WorkDate

			UPDATE [dbo].[EmployeeAttendanceViolation] WITH (ROWLOCK, UPDLOCK)
			SET [IsDeleted] = 1
				,[ModifiedDate] = GETDATE()
			WHERE [MissingCheckInOut] = 0
				AND [LateIn] = 0
				AND [EarlyOut] = 0
				AND [ChangeShift] = 0
				AND [WorkingBelowTime] = 0
				AND [Absent] = 0
				AND [UnpaidLeave] = 0
				AND [UnpaidLeaveAbsent] = 0
		END
	--END

	IF ISNULL(@EmpId, '') = ''
		AND ISNULL(@WorkDate, '') = ''
	BEGIN
		DELETE
		FROM [dbo].[EmployeeAttendanceViolation]
		WHERE [MissingCheckInOut] = 0
			AND [LateIn] = 0
			AND [EarlyOut] = 0
			AND [ChangeShift] = 0
			AND [WorkingBelowTime] = 0
			AND [Absent] = 0
			AND [UnpaidLeave] = 0
			AND [UnpaidLeaveAbsent] = 0
	END

	DROP TABLE #EmployeeViolation1

	DROP TABLE #EmployeeViolation2

	DROP TABLE #EmployeeViolation3

	DROP TABLE #EmployeeInComment

	DROP TABLE #EmployeeOutComment

	DROP TABLE #EmployeeChangeShiftComment

	DROP TABLE #EmployeeLeaveComment

	DROP TABLE #MissingAllInOutTemp

	DROP TABLE #EmployeeMissingInOut
END
