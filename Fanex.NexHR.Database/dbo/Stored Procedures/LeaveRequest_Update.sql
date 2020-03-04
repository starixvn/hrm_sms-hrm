
CREATE PROCEDURE [dbo].[LeaveRequest_Update] @RequestId BIGINT
	,@EmpId INT
	,@Remark NVARCHAR(2000) = NULL
	,@FromDate DATETIME = NULL
	,@ToDate DATETIME = NULL
	,@LeaveTypeId INT = NULL
	,@TotalDays NUMERIC(5, 1) = NULL
	,@ModifiedBy INT = NULL
	,@VerifierId INT = NULL
	,@ApproverId INT = NULL
	,@HRId INT = NULL
	,@RefNo NVARCHAR(200) = ''
	,@ListDetail VARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
	,@PaternityLeaveType INT = 14
AS
/*
Created: Tom.Vo@2013-11-11
Task: update leave request
DB: HRM
Original:

Revisions:    
Param's Explanation:
	- @ListDetail: '<ROOT><Row OnDate='' ShiftID='' LeaveType='' IsHalfDay='' WorkType='' FromTime='' ToTime='' /></ROOT>'
	- @WorkType :
			1. off
			2. leave
			3. work
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @RequestType SMALLINT = 6
		,@AnnualLeaveType INT = 2
		,@KIVLeaveType INT = 10
		,@KIVByHourLeaveType INT = 48
		,@PublicHolidayLeaveType INT = 13
		,@UnpaidLeaveType INT = 25
		,@BusinessTripLeaveType INT = 9
		,@BusinessDuringWorkingLeaveType INT = 26
		,@OnbehalfActionType TINYINT = 5
		,@VerifyActionType TINYINT = 2
		,@ApproveActionType TINYINT = 3
		,@LeaveWorkType TINYINT = 2
		,@TotalDays_tmp NUMERIC(5, 1)
		,@AnnualLeaveDay DECIMAL(18, 2)
		,@AnnualLeavePending DECIMAL(18, 2)
		,@AnnualLeaveBalance DECIMAL(18, 2)
		,@KIVLeaveDay DECIMAL(18, 2)
		,@KIVLeaveBalance DECIMAL(18, 2)
		,@isHalfDay BIT
		,@ModifiedDate DATETIME
		,@IsTopManager BIT
		,@TmpFromDate DATE
		,@TmpToDate DATE
		,@DocHandler INT

	CREATE TABLE #LeaveRequestDetail (
		OnDate DATETIME
		,ShiftId INT
		,LeaveType INT
		,WorkType TINYINT
		,IsHalfDay BIT
		,FromTime DATETIME
		,ToTime DATETIME
		)

	SELECT @IsHalfDay = CASE 
			WHEN @TotalDays = 0.5 THEN 1
			ELSE 0
			END
		,@TotalDays_tmp = @TotalDays
		,@ModifiedDate = GETDATE()
		,@TmpFromDate = CAST(@FromDate AS DATE)
		,@TmpToDate = CAST(@ToDate AS DATE)

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.WorkingTimeRequest WITH (NOLOCK)
			WHERE RequestId = @RequestId
			)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'Request does not exists.'

		RETURN;
	END

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.WorkingTimeRequest WITH (NOLOCK)
			WHERE RequestId = @RequestId
				AND CurrentStatus = 1
			)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'You can not change this request beacause current status is not pending.'

		RETURN;
	END

	IF @FromDate IS NULL
		OR @ToDate IS NULL
		OR @LeaveTypeId IS NULL
		OR ISNULL(@TotalDays, 0) < 0
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'Invalid parameter'

		RETURN;
	END

	IF ISNULL(@RefNo, '') NOT LIKE 'REF-%'
	BEGIN
		SET @ErrCode = 101
		SET @ErrMsg = 'Ref No incorrect format'

		RETURN;
	END

	IF @FromDate IS NOT NULL
		AND @ToDate IS NOT NULL
		AND YEAR(@FromDate) <> YEAR(@ToDate)
	BEGIN
		SET @ErrCode = 102
		SET @ErrMsg = 'You cannot submit a cross-year leave request. Please split them into 2 requests.'

		RETURN;
	END

	IF @EmpId <> @ModifiedBy
		AND NOT EXISTS (
			SELECT 1
			FROM [dbo].[EmployeeSupervisor] sp WITH (NOLOCK)
			WHERE sp.OwnerId = @EmpId
				AND sp.EmpId = @ModifiedBy
				AND sp.ActionType = @OnbehalfActionType
				AND sp.RequestType = @RequestType
				AND ISNULL(sp.IsDeleted, 0) = 0
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.GroupSupervisor AS g WITH (NOLOCK)
				INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON g.GroupId = e.GroupId
			WHERE e.EmpId = @EmpId
				AND g.OwnerId = @ModifiedBy
				AND g.RequestType = @RequestType
				AND g.ActionType = @OnbehalfActionType
				AND ISNULL(g.IsDeleted, 0) = 0
			)
	BEGIN
		SET @ErrCode = 106
		SET @ErrMsg = 'You have not on behalf permision on this employee.'

		RETURN;
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.EmployeeSummary WITH (NOLOCK)
			WHERE EmpId = @EmpId
				AND IsTopManager = 1
			)
	BEGIN
		SET @IsTopManager = 1
	END
	ELSE
	BEGIN
		IF @VerifierId IS NULL
			OR (
				NOT EXISTS (
					SELECT 1
					FROM [dbo].[EmployeeSupervisor] sp WITH (NOLOCK)
					WHERE sp.OwnerId = @VerifierId
						AND sp.EmpId = @EmpId
						AND sp.ActionType = @VerifyActionType
						AND sp.RequestType = @RequestType
						AND ISNULL(sp.IsDeleted, 0) = 0
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS g WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON g.GroupId = e.GroupId
					WHERE e.EmpId = @EmpId
						AND g.OwnerId = @VerifierId
						AND g.RequestType = @RequestType
						AND g.ActionType = @VerifyActionType
						AND ISNULL(g.IsDeleted, 0) = 0
					)
				)
		BEGIN
			SET @ErrCode = 108
			SET @ErrMsg = 'Invalid Verifier.'

			RETURN;
		END

		IF @ApproverId IS NULL
			OR (
				NOT EXISTS (
					SELECT 1
					FROM [dbo].[EmployeeSupervisor] sp WITH (NOLOCK)
					WHERE sp.OwnerId = @ApproverId
						AND sp.EmpId = @EmpId
						AND sp.ActionType = @ApproveActionType
						AND sp.RequestType = @RequestType
						AND ISNULL(sp.IsDeleted, 0) = 0
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.GroupSupervisor AS g WITH (NOLOCK)
						INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON g.GroupId = e.GroupId
					WHERE e.EmpId = @EmpId
						AND g.OwnerId = @ApproverId
						AND g.RequestType = @RequestType
						AND g.ActionType = @ApproveActionType
						AND ISNULL(g.IsDeleted, 0) = 0
					)
				)
		BEGIN
			SET @ErrCode = 109
			SET @ErrMsg = 'Invalid Approver.'

			RETURN;
		END
	END

	IF @LeaveTypeID <> @PublicHolidayLeaveType
		AND @TotalDays = 0.5
		AND EXISTS (
			SELECT 1
			FROM dbo.EmployeeCalendar WITH (NOLOCK)
			WHERE EmpID = @EmpId
				AND WorkDate = @FromDate
				AND (
					IsHoliday = 1
					OR IsOffDay = 1
					)
			)
	BEGIN
		SET @ErrCode = 105
		SET @ErrMsg = 'Your can not request on off day or holiday.'

		RETURN;
	END

	BEGIN TRY
		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@ListDetail

		IF DATEDIFF(DD, @TmpFromDate, @TmpToDate) + 1 <> (
				SELECT COUNT(1)
				FROM dbo.EmployeeCalendar WITH (NOLOCK)
				WHERE EmpID = @EmpID
					AND WorkDate BETWEEN @TmpFromDate
						AND @TmpToDate
				)
		BEGIN
			EXEC dbo.EmployeeCalendar_Insert @EmpId = @EmpId
				,@fromdate = @TmpFromDate
				,@todate = @TmpToDate
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		END

		INSERT INTO #LeaveRequestDetail (
			OnDate
			,ShiftID
			,LeaveType
			,WorkType
			,IsHalfDay
			,FromTime
			,ToTime
			)
		SELECT Data.OnDate
			,ISNULL(Data.ShiftID, ec.ShiftId) ShiftID
			,Data.LeaveType
			,Data.WorkType
			,Data.IsHalfDay
			,Data.FromTime
			,Data.ToTime
		FROM OPENXML(@DocHandler, '/ROOT/Row', 1) WITH (OnDate DATETIME, ShiftID INT, LeaveType INT, IsHalfDay BIT, WorkType TINYINT, FromTime DATETIME, ToTime DATETIME) AS Data
			LEFT JOIN dbo.EmployeeCalendar ec WITH (NOLOCK) ON ec.WorkDate = data.OnDate
			AND ec.EmpId = @EmpId

		IF @@ROWCOUNT <> 0
		BEGIN
			SELECT @TotalDays_tmp = SUM(CASE lrd.IsHalfDay
						WHEN 1 THEN 0.5
						ELSE 1
						END)
			FROM dbo.EmployeeCalendar cl WITH (NOLOCK)
				INNER JOIN #LeaveRequestDetail lrd ON cl.WorkDate = lrd.OnDate
			WHERE EmpID = @EmpID
				AND WorkDate BETWEEN @TmpFromDate
					AND @TmpToDate
				AND IsHoliday = 0
				AND IsOffDay = 0 --AND IsLeaveDay = 0
				AND lrd.WorkType = @LeaveWorkType
		END

		IF @TotalDays_tmp IS NULL
			SELECT @TotalDays = SUM(CASE lrd.IsHalfDay
						WHEN 1 THEN 0.5
						ELSE 1
						END)
			FROM #LeaveRequestDetail lrd
			WHERE lrd.WorkType = @LeaveWorkType

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 103
			,@ErrMsg = N'Edit leave request for an employee failed.'

		RETURN
	END CATCH;

	IF EXISTS (
			SELECT 1
			FROM #LeaveRequestDetail
			WHERE IsHalfDay = 1
			GROUP BY OnDate
			HAVING COUNT(DISTINCT LeaveType) = 1
				AND COUNT(OnDate) > 1
			)
	BEGIN
		SELECT @ErrCode = 101
			,@ErrMsg = N'Please check Leave Type on this request.'

		RETURN
	END

	IF EXISTS (
			SELECT 1
			FROM #LeaveRequestDetail
			)
	BEGIN
		SELECT @TotalDays_tmp = SUM(CASE lrd.IsHalfDay
					WHEN 1 THEN 0.5
					ELSE 1
					END)
		FROM dbo.EmployeeCalendar cl WITH (NOLOCK)
			INNER JOIN #LeaveRequestDetail lrd ON cl.WorkDate = lrd.OnDate
		WHERE EmpID = @EmpID
			AND WorkDate BETWEEN @TmpFromDate
				AND @TmpToDate
			AND IsHoliday = 0
			AND IsOffDay = 0
			AND IsLeaveDay = 0
			AND lrd.WorkType = @LeaveWorkType

		IF @TotalDays_tmp IS NULL
		BEGIN
			SELECT @TotalDays_tmp = SUM(CASE lrd.IsHalfDay
						WHEN 1 THEN 0.5
						ELSE 1
						END)
			FROM #LeaveRequestDetail lrd
			WHERE lrd.WorkType = @LeaveWorkType
		END

		SELECT @AnnualLeaveDay = SUM(CASE cl.IsHalfDay
					WHEN 1 THEN 0.5
					ELSE 1
					END)
		FROM #LeaveRequestDetail cl WITH (NOLOCK)
			INNER JOIN dbo.LeaveType l WITH (NOLOCK) ON cl.LeaveType = l.LeaveTypeId
		WHERE IsALDeduction = 1

		SELECT @KIVLeaveDay = SUM(CASE IsHalfDay
					WHEN 1 THEN 0.5
					ELSE 1
					END)
		FROM #LeaveRequestDetail WITH (NOLOCK)
		WHERE LeaveType = @KIVLeaveType
	END
	ELSE
	BEGIN
		SELECT @TotalDays_tmp = SUM(1)
		FROM dbo.EmployeeCalendar cl WITH (NOLOCK)
		WHERE EmpID = @EmpID
			AND WorkDate BETWEEN @TmpFromDate
				AND @TmpToDate
			AND IsHoliday = 0
			AND IsOffDay = 0

		IF @isHalfDay = 1
			AND ISNULL(@TotalDays_tmp, 1) = 1
		BEGIN
			SET @TotalDays_tmp = 0.5
		END

		IF @LeaveTypeId = @AnnualLeaveType
		BEGIN
			SELECT @AnnualLeaveDay = @TotalDays
		END
		ELSE IF @LeaveTypeID = @KIVLeaveType
		BEGIN
			SELECT @KIVLeaveDay = @TotalDays
		END
	END

	IF ISNULL(@TotalDays_tmp, 0) <= 0
		AND @LeaveTypeID <> @PublicHolidayLeaveType
	BEGIN
		SET @ErrCode = 105
		SET @ErrMsg = 'Your leave days invalid.'

		RETURN;
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
				INNER JOIN dbo.LeaveRequest l WITH (NOLOCK) ON r.RequestId = l.RequestId
				LEFT JOIN dbo.EmployeeCalendar c WITH (NOLOCK) ON c.WorkDate BETWEEN l.fromdate
					AND l.ToDate
				AND c.empid = l.EmpId
			WHERE r.empid = @empid
				AND r.RequestType = @RequestType
				AND r.RequestId <> @RequestId
				AND CurrentStatus IN (1, 2, 4, 6)
				AND (
					(
						@TotalDays >= 1
						AND (
							CAST(l.FromDate AS DATE) BETWEEN @TmpFromDate
								AND @TmpToDate
							OR CAST(l.ToDate AS DATE) BETWEEN @TmpFromDate
								AND @TmpToDate
							OR @TmpFromDate BETWEEN CAST(l.FromDate AS DATE)
								AND CAST(l.ToDate AS DATE)
							OR @TmpToDate BETWEEN CAST(l.FromDate AS DATE)
								AND CAST(l.ToDate AS DATE)
							)
						)
					OR (
						@TotalDays < 1
						AND TotalDays >= 1
						AND (
							CAST(l.FromDate AS DATE) BETWEEN @TmpFromDate
								AND @TmpToDate
							OR CAST(l.ToDate AS DATE) BETWEEN @TmpFromDate
								AND @TmpToDate
							OR @TmpFromDate BETWEEN CAST(l.FromDate AS DATE)
								AND CAST(l.ToDate AS DATE)
							OR @TmpToDate BETWEEN CAST(l.FromDate AS DATE)
								AND CAST(l.ToDate AS DATE)
							)
						)
					)
			)
		OR EXISTS (
			SELECT 1
			FROM dbo.WorkingTimeRequest r WITH (NOLOCK)
				INNER JOIN dbo.LeaveRequest l WITH (NOLOCK) ON r.RequestId = l.RequestId
				INNER JOIN dbo.LeaveRequestDetail ldt WITH (NOLOCK) ON r.RequestId = ldt.RequestId
				INNER JOIN #LeaveRequestDetail ldtt WITH (NOLOCK) ON ldtt.OnDate = ldt.OnDate
			WHERE r.empid = @empid
				AND r.RequestType = @RequestType
				AND r.RequestId <> @RequestId
				AND CurrentStatus IN (1, 2, 4, 6)
				AND @TotalDays = 0.5
				AND l.TotalDays = 0.5
				AND (
					(
						ldt.FromTime BETWEEN ldtt.FromTime
							AND ldtt.ToTime
						)
					OR (
						ldt.ToTime BETWEEN ldtt.FromTime
							AND ldtt.ToTime
						)
					OR (
						ldtt.FromTime BETWEEN ldt.FromTime
							AND ldt.ToTime
						)
					OR (
						ldtt.ToTime BETWEEN ldt.FromTime
							AND ldt.ToTime
						)
					)
				AND ldt.FromTime <> ldtt.ToTime
				AND ldt.ToTime <> ldtt.FromTime
			)
	BEGIN
		SET @ErrCode = 104
		SET @ErrMsg = 'There is another request in the same range'

		RETURN;
	END

	IF @TotalDays_tmp IS NOT NULL
		OR @TotalDays_tmp > 0
		SET @TotalDays = @TotalDays_tmp

	SELECT @AnnualLeaveDay = ISNULL(@AnnualLeaveDay, 0)

	IF @AnnualLeaveDay > 0
	BEGIN
		SELECT @AnnualLeaveBalance = dbo.fnc_GetAnnualLeaveBalanceByDate(@EmpId, @ToDate, @RequestId)

		IF @AnnualLeaveDay > @AnnualLeaveBalance
		BEGIN
			SET @ErrCode = 108
			SET @ErrMsg = 'You have not enough annual leave days to request'

			RETURN;
		END
	END

	SELECT @KIVLeaveDay = ISNULL(@KIVLeaveDay, 0)

	IF @KIVLeaveDay > 0
	BEGIN
		SELECT @KIVLeaveBalance = dbo.fnc_GetKIVByDayBalance(@EmpId, @ToDate, @RequestId)

		IF @KIVLeaveDay > @KIVLeaveBalance
		BEGIN
			SET @ErrCode = 111
			SET @ErrMsg = 'You have not enough Keep In View leave days to request'

			RETURN;
		END
	END

	IF EXISTS (
			SELECT 1
			FROM #LeaveRequestDetail
			)
	BEGIN
		IF EXISTS (
				SELECT 1
				FROM #LeaveRequestDetail
				WHERE IsHalfDay = 1
					AND LeaveType NOT IN (@AnnualLeaveType, @UnpaidLeaveType, @KIVLeaveType, @KIVByHourLeaveType, @BusinessTripLeaveType, @BusinessDuringWorkingLeaveType)
				)
		BEGIN
			SET @ErrCode = 112
			SET @ErrMsg = 'Leave type does not support half day.'

			RETURN;
		END

		IF EXISTS (
				SELECT 1
				FROM #LeaveRequestDetail
				WHERE IsHalfDay = 1
					AND LeaveType IN (@AnnualLeaveType, @UnpaidLeaveType, @KIVLeaveType)
					AND DATEDIFF(MINUTE, FromTime, ToTime) > 300
				)
		BEGIN
			SET @ErrCode = 113
			SET @ErrMsg = 'You can not request leave half day over 5 hours.'

			RETURN;
		END
	END

	-- check rule for leave during working hours
	IF @LeaveTypeId = @BusinessDuringWorkingLeaveType
		AND EXISTS (
			SELECT 1
			FROM #LeaveRequestDetail
			WHERE IsHalfDay = 1
			)
	BEGIN
		IF EXISTS (
				SELECT 1
				FROM dbo.LeaveRequestDetail AS ldt WITH (NOLOCK)
					INNER JOIN dbo.LeaveRequest AS lv WITH (NOLOCK) ON lv.RequestId = ldt.RequestId
					AND lv.LeaveType = @BusinessDuringWorkingLeaveType
					AND DATEDIFF(MONTH, lv.FromDate, @FromDate) = 0
					INNER JOIN dbo.WorkingTimeRequest AS wk WITH (NOLOCK) ON wk.RequestId = ldt.RequestId
					AND wk.CurrentStatus IN (1, 2, 4, 6)
				WHERE ldt.RequestId <> @RequestId
					AND ldt.IsHalfDay = 1
				)
		BEGIN
			SET @ErrCode = 114
			SET @ErrMsg = 'You had been reached maximum leave during working hours in month.'

			RETURN
		END

		IF DATEDIFF(DD, @FromDate, @ToDate) > 0
			OR EXISTS (
				SELECT 1
				FROM #LeaveRequestDetail
				WHERE IsHalfDay = 1
					AND LeaveType = @BusinessDuringWorkingLeaveType
					AND DATEDIFF(MI, FromTime, ToTime) > 120
				)
		BEGIN
			SET @ErrCode = 115
			SET @ErrMsg = 'You can not request leave during working hours > 120 minutes.'

			RETURN
		END

		EXEC dbo.tmpEmployeeCalendar_Insert @fromDate = @FromDate
			,@toDate = @FromDate
			,@empIds = @EmpId

		IF NOT EXISTS (
				SELECT 1
				FROM dbo.tmpEmployeeCalendar WITH (NOLOCK)
				WHERE EmpId = @EmpId
					AND DATEDIFF(DD, @FromDate, WorkDate) = 0
					AND ISNULL(ShiftId, 0) > 0
				)
		BEGIN
			SET @ErrCode = 116
			SET @ErrMsg = 'You can not request because this date has not arranged shift yet.'

			RETURN
		END

		IF EXISTS (
				SELECT 1
				FROM dbo.tmpEmployeeCalendar AS empca WITH (NOLOCK)
					INNER JOIN dbo.Shift AS shift WITH (NOLOCK) ON empca.ShiftId = shift.ShiftId
					INNER JOIN #LeaveRequestDetail AS detail ON DATEDIFF(DD, detail.OnDate, empca.WorkDate) = 0
					AND CONVERT(VARCHAR(5), detail.FromTime, 108) = shift.BeginCheckInTime
				WHERE empca.EmpId = @EmpId
					AND DATEDIFF(DD, @FromDate, empca.WorkDate) = 0
				)
		BEGIN
			SET @ErrCode = 117
			SET @ErrMsg = 'You can not request at the beginning of the shift.'

			RETURN
		END
	END

	-- check leave request paternity by Lilac

	IF @LeaveTypeId = @PaternityLeaveType
	BEGIN
		IF EXISTS (SELECT 1
					FROM dbo.EmployeePaternity
					WHERE DATEADD(M, 1, ChildBornDate) >= @ToDate AND @FromDate >= ChildBornDate AND EmpId = @EmpId AND BalanceDay + (SELECT TotalDays FROM LeaveRequest WHERE RefNo = @RefNo) >= @TotalDays)
		BEGIN
			UPDATE EmployeePaternity
			SET BalanceDay = BalanceDay - @TotalDays + (SELECT TotalDays FROM LeaveRequest WHERE RefNo = @RefNo)
			WHERE EmpId = @EmpId AND DATEADD(M, 1, ChildBornDate) >= @ToDate
		END
		ELSE
		BEGIN
			SET @ErrCode = 118
			SET @ErrMsg = 'You can not request because request within 1 month from child born date or leave day incorrect.'

			RETURN
		END
	END

	BEGIN TRY
		BEGIN TRANSACTION Update_LeaveMultiRequest

		UPDATE WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
		SET Remark = @Remark
			,[CurrentStatus] = CASE 
				WHEN @IsTopManager = 1
					AND @VerifierId IS NOT NULL THEN 1
				WHEN @IsTopManager = 1
					AND @ApproverId IS NOT NULL THEN 2
				WHEN @IsTopManager = 1
					AND @ApproverId IS NULL THEN 4
				ELSE 1
				END
			,VerifierId = @VerifierId
			,[VerifiedStatus] = CASE 
				WHEN @IsTopManager = 1
					AND @VerifierId IS NULL THEN 2
				ELSE 1
				END
			,ApproverId = @ApproverId
			,[ApprovedStatus] = CASE 
				WHEN @IsTopManager = 1
					AND @ApproverId IS NULL THEN 2
				ELSE 1
				END
			,HRId = @HRId
			,ModifiedBy = @ModifiedBy
			,ModifiedDate = @ModifiedDate
		WHERE RequestId = @RequestId

		UPDATE dbo.LeaveRequest WITH (ROWLOCK, UPDLOCK)
		SET FromDate = @FromDate
			,ToDate = @ToDate
			,LeaveType = @LeaveTypeId
			,TotalDays = @TotalDays
		WHERE RequestId = @RequestId

		DELETE
		FROM dbo.LeaveRequestDetail
		WHERE RequestId = @RequestId

		INSERT INTO dbo.LeaveRequestDetail (
			RequestId
			,EmpId
			,OnDate
			,ShiftId
			,LeaveType
			,IsHalfDay
			,FromTime
			,ToTime
			,WorkType
			)
		SELECT @RequestId
			,@EmpId
			,OnDate
			,ShiftId
			,LeaveType
			,IsHalfDay
			,FromTime
			,ToTime
			,WorkType
		FROM #LeaveRequestDetail

		EXEC dbo.LeaveRequest_InsertTmpAnnualLeaveTrans @RequestId = @RequestId
			,@EmpId = @EmpId

		EXEC dbo.LeaveRequest_InsertKIVLeaveTrans @RequestId = @RequestId

		EXEC dbo.LeaveRequest_InsertWeekendLeaveTrans @RequestId = @RequestId

		IF @ApproverId IS NULL
			AND @VerifierId IS NULL
			AND @IsTopManager = 1
		BEGIN
			EXEC dbo.LeaveRequest_Approve @RequestId = @RequestId
				,@ActorId = @EmpId
				,@RoleID = 3
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		END

		EXEC dbo.LeaveRequest_WriteLog @RequestId

		COMMIT TRANSACTION Update_LeaveMultiRequest;
	END TRY

	BEGIN CATCH
		SELECT ERROR_MESSAGE()

		SELECT @ErrCode = 999
			,@ErrMsg = N'Update request failed.'

		ROLLBACK TRANSACTION Update_LeaveMultiRequest;

		RETURN
	END CATCH;

	DROP TABLE #LeaveRequestDetail
END
