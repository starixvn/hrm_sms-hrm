
CREATE PROCEDURE [dbo].[WorkingTimeRequest_ApproveRequest] @RequestId BIGINT
	,@ProcessedBy INT
	,@RoleId INT
	,@Comment NVARCHAR(500) = ''
	,@IgnoredDetails NVARCHAR(MAX) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-11-06
Task: approve request form (main SP)
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
    @IgnoredDetails:
        <ROOT>
            <Discipline WorkDate='' EmployeeId='' DisciplineTypeId='' IsIgnored='' IsConfirmed=''></Discipline>
        </ROOT>
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmpId INT
		,@RequestType SMALLINT
		,@Status TINYINT
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@TotalDay INT
		,@VerifierId INT
		,@ApproverId INT
		,@HRId INT
		,@VerifiedStatus INT
		,@ApprovedStatus INT
		,@HRStatus INT
		,@ProcessedDate DATETIME

	-- get employee and requestype of the request
	SELECT TOP 1 @EmpId = EmpId
		,@Status = [CurrentStatus]
		,@VerifierId = VerifierId
		,@VerifiedStatus = VerifiedStatus
		,@ApproverId = ApproverId
		,@ApprovedStatus = ApprovedStatus
		,@HRId = HRId
		,@HRStatus = HRStatus
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE RequestID = @RequestId

	IF @RoleId = 2
		AND (
			@Status <> 1
			OR @VerifierId <> @ProcessedBy
			)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'You can not verify this request. Please contact Administrator.'

		RETURN
	END

	IF @RoleId = 3
		AND (
			@Status NOT IN (1, 2)
			OR @ApproverId <> @ProcessedBy
			)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'You can not approve this request. Please contact Administrator.'

		RETURN
	END

	IF @RoleId = 6
		AND (
			@Status <> 4
			OR NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole
				WHERE EmployeeId = @ProcessedBy
					AND RoleId IN (6, 7)
				)
			)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'You can not process this request. Please contact Administrator.'

		RETURN
	END

	EXEC dbo.WorkingTimeRequest_GetRangeDateByRequestID @RequestId = @RequestId
		,@FromDate = @FromDate OUTPUT
		,@ToDate = @ToDate OUTPUT
		,@TotalDay = @TotalDay OUTPUT
		,@RequestType = @RequestType OUTPUT

	SELECT @FromDate = CONVERT(VARCHAR(10), @FromDate, 111)
		,@ToDate = CONVERT(VARCHAR(10), @ToDate, 111)

	IF @RequestType IN (1, 2)
	BEGIN
		IF EXISTS (
				SELECT 1
				FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
					INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON r.RequestId = lr.RequestId
				WHERE r.empid = @EmpId
					AND r.RequestType = @RequestType
					AND r.CurrentStatus = 6
					AND lr.FromDate = @FromDate
				)
		BEGIN
			SET @ErrCode = 104
			SET @ErrMsg = 'Another approved request in the same date has existed'

			RETURN
		END
	END

	-- if not exist calendar
	IF @RequestType IN (1, 2, 3, 4, 5)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeCalendar WITH (NOLOCK)
			WHERE empid = @empid
				AND workdate = @fromdate
			)
	BEGIN
		EXEC [dbo].[EmployeeCalendar_Insert] @empid = @empid
			,@fromdate = @FromDate
			,@todate = @fromdate
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT

		IF @ErrCode <> 0
		BEGIN
			ROLLBACK TRANSACTION;

			RETURN
		END

		SET @ErrCode = 107
		SET @ErrMsg = 'The working calendar does not exist. Please contact HR for more info.'

		RETURN
	END
	ELSE IF (@RequestType = 6)
	BEGIN
		DECLARE @maxDate DATETIME
			,@minDate DATETIME
			,@result BIT

		SELECT @maxDate = MAX(workdate)
			,@minDate = MIN(workdate)
		FROM dbo.EmployeeCalendar WITH (NOLOCK)
		WHERE empid = @empid
			AND workdate BETWEEN @fromdate
				AND @todate

		IF @minDate IS NULL
			OR @maxDate IS NULL
			OR @fromdate <> @minDate
			OR @todate <> @maxDate
		BEGIN
			EXEC [dbo].[EmployeeCalendar_Insert] @empid = @empid
				,@fromdate = @FromDate
				,@todate = @todate
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT

			SET @ErrCode = 108
			SET @ErrMsg = 'The working calendar does not exist. Please contact HR for more info.'

			RETURN
		END

		DECLARE @TotalDays_tmp NUMERIC(5, 1)
			,@TotalDays_tmpbal NUMERIC(5, 1)
			,@isRquestDetail BIT = 1

		IF EXISTS (
				SELECT 1
				FROM dbo.LeaveRequestDetail WITH (NOLOCK)
				WHERE RequestID = @RequestId
				)
		BEGIN
			SELECT @TotalDays_tmp = SUM(CASE lrd.IsHalfDay
						WHEN 1 THEN 0.5
						ELSE 1
						END)
			FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
				INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON lrd.OnDate = cl.WorkDate
				AND lrd.EmpId = cl.EmpId
				INNER JOIN dbo.LeaveType AS l WITH (NOLOCK) ON lrd.LeaveType = l.LeaveTypeId
			WHERE lrd.RequestID = @RequestId
				AND cl.IsHoliday = 0
				AND cl.IsOffDay = 0
				AND lrd.WorkType = 2
				AND l.IsALDeduction = 1
		END
		ELSE
		BEGIN
			SET @isRquestDetail = 0

			SELECT @TotalDays_tmp = SUM(1)
			FROM dbo.EmployeeCalendar AS cl WITH (NOLOCK)
				INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON (
					cl.WorkDate BETWEEN CONVERT(SMALLDATETIME, CONVERT(NVARCHAR(20), lr.FromDate, 111))
						AND CONVERT(SMALLDATETIME, CONVERT(NVARCHAR(20), lr.ToDate, 111))
					)
				AND lr.EmpId = cl.EmpId
				INNER JOIN dbo.LeaveType AS l WITH (NOLOCK) ON lr.LeaveType = l.LeaveTypeId
			WHERE lr.RequestId = @RequestId
				AND cl.IsHoliday = 0
				AND cl.IsOffDay = 0
				AND l.IsALDeduction = 1
		END

		SET @TotalDays_tmp = ISNULL(@TotalDays_tmp, 0)

		SELECT @TotalDays_tmpbal = SUM(tmp.TotalDays)
		FROM dbo.tmpLeaveBalance AS tmp WITH (NOLOCK)
		WHERE tmp.RequestID = @RequestId

		IF (
				@TotalDays_tmp <> @TotalDays_tmpbal
				AND @isRquestDetail = 1
				)
			OR (
				@isRquestDetail = 0
				AND @TotalDays_tmp <> @TotalDays_tmpbal
				AND @TotalDays_tmpbal <> 0.5
				)
		BEGIN
			SET @ErrCode = 108
			SET @ErrMsg = 'Could not approved this request because this employee''s calendar have been changed.'

			RETURN
		END
	END

	--ignore
	EXEC dbo.WorkingTimeRequest_ApproveRequest_IgnoreDiscipline @RequestId = @RequestId
		,@RoleId = @RoleId
		,@ModifiedBy = @ProcessedBy
		,@IgnoredDetails = @IgnoredDetails
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT

	IF @ErrCode = 0
	BEGIN
		SET @ProcessedDate = GETDATE()

		-- call appropriate SP for each request form
		IF (@RequestType = 1) -- check in
		BEGIN
			EXEC dbo.WorkingTimeRequest_ApproveRequest_1 @RequestId = @RequestId
				,@EmpId = @EmpId
				,@RoleId = @RoleId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@Comment = @Comment
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		END
		ELSE IF (@RequestType = 2) -- check out
		BEGIN
			EXEC dbo.WorkingTimeRequest_ApproveRequest_2 @RequestId = @RequestId
				,@EmpId = @EmpId
				,@RoleId = @RoleId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@Comment = @Comment
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		END
		ELSE IF (@RequestType = 6) -- leave
		BEGIN
			EXEC dbo.WorkingTimeRequest_ApproveRequest_6 @RequestId = @RequestId
				,@EmpId = @EmpId
				,@RoleId = @RoleId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@Comment = @Comment
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		END
		ELSE IF (@RequestType = 7) -- keep in view
		BEGIN
			EXEC dbo.WorkingTimeRequest_ApproveRequest_7 @RequestId = @RequestId
				,@EmpId = @EmpId
				,@RoleId = @RoleId
				,@ProcessedBy = @ProcessedBy
				,@ProcessedDate = @ProcessedDate
				,@Comment = @Comment
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT
		END
	END
	ELSE
	BEGIN
		RETURN
	END

	IF @ErrCode = 0
	BEGIN
		-- write log
		DECLARE @LogSPData VARCHAR(4000)
			,@now DATETIME
			,@Description VARCHAR(200)

		SET @now = GETDATE()

		--Log general info
		SELECT @LogSPData = '<WorkingTimeRequest_ApproveRequest RequestID="' + COALESCE(CAST(@RequestId AS VARCHAR(50)), SPACE(1)) + '" ' + 'ApprovedBy="' + COALESCE(CAST(@ProcessedBy AS VARCHAR(100)), SPACE(1)) + '" ' + 'ApprovedDate="' + COALESCE(CONVERT(VARCHAR(10), @ProcessedDate, 111), SPACE(1)) + '" ' + 'RoleID="' + COALESCE(CAST(@RoleId AS VARCHAR(100)), SPACE(1)) + '" ' + 'ErrCode="' + COALESCE(CAST(@ErrCode AS VARCHAR(50)), SPACE(1)) + '" ' + 'ErrMsg="' + COALESCE(CAST(@ErrMsg AS VARCHAR(4000)), SPACE(1)) + '" />'

		SELECT @Description = 'Approve Request:' + COALESCE(CAST(@RequestId AS VARCHAR(50)), SPACE(1))

		EXEC dbo.Log_WriteLog @LogActionID = 1
			,@LogDate = @now
			,@Description = @Description
			,@TrackingValue = @LogSPData
			,@UserID = @ProcessedBy
			,@EmpID = @EmpID
	END
END
