
CREATE PROCEDURE [dbo].[LeaveRequest_Approve] @RequestID BIGINT
	,@RoleID INT
	,@IgnoredDetails NVARCHAR(MAX) = NULL
	,@Comment NVARCHAR(500) = ''
	,@ProcessedBy INT
	,@ProcessedDate DATETIME
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Quinn.Vo@20150925
Task: approve leave request.
DB: HRM_SMS
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @FromDate DATETIME
		,@ToDate DATETIME
		,@maxDate DATETIME
		,@minDate DATETIME
		,@result BIT
	DECLARE @EmpId INT
		,@RequestType SMALLINT
		,@Status TINYINT
		,@TotalDay INT
		,@VerifierId INT
		,@ApproverId INT
		,@HRId INT
		,@VerifiedStatus INT
		,@ApprovedStatus INT
		,@HRStatus INT

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

	SELECT @FromDate = FromDate
		,@ToDate = ToDate
	FROM dbo.LeaveRequest lr WITH (NOLOCK)
	WHERE lr.RequestID = @RequestID

	IF (
			@Status NOT IN (1, 2)
				OR @ApproverId <> @ProcessedBy
			)
	BEGIN
		SET @ErrCode = 103
		SET @ErrMsg = 'You can not approve this request. Please contact Administrator.'

		RETURN
	END

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

	SET XACT_ABORT ON;

	BEGIN TRY
		BEGIN TRANSACTION;
		
		IF @RoleID = 2 --Verifier
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 2
				,VerifiedStatus = 2
				,VerifierId = @ProcessedBy
				,VerifiedDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID
		END
		ELSE IF @RoleID = 3 --Approver
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 4
				,ApprovedStatus = 2
				,ApproverComment = @Comment
				,ApproverId = @ProcessedBy
				,ApprovedDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID
			
			EXECUTE dbo.DisciplineRequestTrans_Insert @RequestId = @RequestId
				,@ModifiedBy = @ProcessedBy
				,@IgnoredDetails = @IgnoredDetails
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT

			EXECUTE dbo.LeaveRequest_Approve_UpdateCalendar @RequestId
				,@EmpId
				,@ProcessedBy

			EXECUTE dbo.AnnualLeave_InsertTransactionByRequestId @RequestId
				,@EmpId
				,@FromDate
				,@ToDate
				,@ProcessedBy

			EXECUTE dbo.LeaveRequest_UpdateTmpAnnualLeaveTrans @Requestid = @Requestid
				,@EmpID = @EmpID
				,@Status = 1
				
			EXECUTE [dbo].EmployeeAttendance_ProcessingAndInsert @EmpId
				,@FromDate
				,@ToDate
				,1
		END
		ELSE IF @RoleID = 6 --HR
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = 6
				,HRStatus = 2
				,HRId = @ProcessedBy
				,HRDate = @ProcessedDate
				,ModifiedBy = @ProcessedBy
				,ModifiedDate = @ProcessedDate
			WHERE RequestID = @RequestID
		END

		EXECUTE dbo.LeaveRequest_WriteLog @RequestID
				,@Comment

		COMMIT TRANSACTION;

		SELECT @ErrCode = 0;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 100
			,@ErrMsg = 'Approve process failed'

		ROLLBACK TRANSACTION
	END CATCH

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