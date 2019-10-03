
CREATE PROCEDURE [dbo].[LeaveRequest_Approve] @RequestId BIGINT
	,@RoleId TINYINT
	,@DisciplineXml NVARCHAR(MAX) = NULL
	,@Comment NVARCHAR(500) = ''
	,@ActorId INT
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
    @DisciplineXml: 
        <Root>
            <Row WorkDate='' EmployeeId='' DisciplineTypeId='' IsIgnored='' IsConfirmed='' />
        </Root>
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @FromDate DATETIME
		,@ToDate DATETIME
		,@maxDate DATETIME
		,@minDate DATETIME
		,@result BIT
		,@ProcessedDate DATETIME = GETDATE()
		-- system role
		,@VerifierRoleId TINYINT = 2
		,@ApproverRoleId TINYINT = 3
		,@HRRoleId TINYINT = 6
		,@SystemAdminRoleId TINYINT = 7
		-- request status
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@NotVerifiedStatusId TINYINT = 3
		,@ApprovedStatusId TINYINT = 4
		,@RejectedStatusId TINYINT = 5
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7
	DECLARE @EmpId INT
		,@RequestType SMALLINT
		,@StatusId TINYINT
		,@TotalDay INT
		,@VerifierId INT
		,@ApproverId INT
		,@HRId INT

	SELECT @EmpId = EmpId
		,@StatusId = CurrentStatus
		,@VerifierId = VerifierId
		,@ApproverId = ApproverId
		,@HRId = HRId
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	SELECT @FromDate = FromDate
		,@ToDate = ToDate
	FROM dbo.LeaveRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	IF @RoleId = @VerifierRoleId
		AND (
			@StatusId <> @PendingStatusId
			OR @VerifierId <> @ActorId
			)
	BEGIN
		SET @ErrCode = 100
		SET @ErrMsg = 'You can not verify this request. Please contact Administrator.'

		RETURN
	END

	IF @RoleId = @ApproverRoleId
		AND (
			@StatusId NOT IN (@PendingStatusId, @VerifiedStatusId)
			OR @ApproverId <> @ActorId
			)
	BEGIN
		SET @ErrCode = 101
		SET @ErrMsg = 'You can not approve this request. Please contact Administrator.'

		RETURN
	END

	IF @RoleId = @HRRoleId
		AND (
			@StatusId <> @ApprovedStatusId
			OR NOT EXISTS (
				SELECT 1
				FROM dbo.EmployeeRole WITH (NOLOCK)
				WHERE EmployeeId = @ActorId
					AND RoleId IN (@HRRoleId, @SystemAdminRoleId)
				)
			)
	BEGIN
		SET @ErrCode = 102
		SET @ErrMsg = 'You can not process this request. Please contact Administrator.'

		RETURN
	END

	SELECT @maxDate = MAX(WorkDate)
		,@minDate = MIN(WorkDate)
	FROM dbo.EmployeeCalendar WITH (NOLOCK)
	WHERE EmpId = @empid
		AND WorkDate BETWEEN @fromdate
			AND @todate

	IF @minDate IS NULL
		OR @maxDate IS NULL
		OR @fromdate <> @minDate
		OR @todate <> @maxDate
	BEGIN
		EXEC dbo.EmployeeCalendar_Insert @empid = @empid
			,@fromdate = @FromDate
			,@todate = @todate
			,@ErrCode = @ErrCode OUTPUT
			,@ErrMsg = @ErrMsg OUTPUT
	END

	DECLARE @TotalDays_tmp NUMERIC(5, 1)
		,@TotalDays_tmpbal NUMERIC(5, 1)
		,@isRquestDetail BIT = 1

	IF EXISTS (
			SELECT 1
			FROM dbo.LeaveRequestDetail WITH (NOLOCK)
			WHERE RequestId = @RequestId
			)
	BEGIN
		SELECT @TotalDays_tmp = SUM(CASE lrd.IsHalfDay
					WHEN 1 THEN 0.5
					ELSE 1
					END)
		FROM dbo.EmployeeCalendar AS empca WITH (NOLOCK)
			INNER JOIN dbo.LeaveRequestDetail AS lrd WITH (NOLOCK) ON lrd.OnDate = empca.WorkDate
			AND lrd.EmpId = empca.EmpId
			AND lrd.RequestId = @RequestId
			AND lrd.WorkType = 2
			INNER JOIN dbo.LeaveType AS ltype WITH (NOLOCK) ON lrd.LeaveType = ltype.LeaveTypeId
			AND ltype.IsALDeduction = 1
		WHERE empca.IsHoliday = 0
			AND empca.IsOffDay = 0
	END
	ELSE
	BEGIN
		SET @isRquestDetail = 0

		SELECT @TotalDays_tmp = SUM(1)
		FROM dbo.EmployeeCalendar AS empca WITH (NOLOCK)
			INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON lr.RequestId = @RequestId
			AND lr.EmpId = empca.EmpId
			AND empca.WorkDate BETWEEN CONVERT(SMALLDATETIME, CONVERT(NVARCHAR(20), lr.FromDate, 111))
				AND CONVERT(SMALLDATETIME, CONVERT(NVARCHAR(20), lr.ToDate, 111))
			INNER JOIN dbo.LeaveType AS ltype WITH (NOLOCK) ON lr.LeaveType = ltype.LeaveTypeId
			AND ltype.IsALDeduction = 1
		WHERE empca.IsHoliday = 0
			AND empca.IsOffDay = 0
	END

	SET @TotalDays_tmp = ISNULL(@TotalDays_tmp, 0)

	SELECT @TotalDays_tmpbal = SUM(TotalDays)
	FROM dbo.tmpLeaveBalance WITH (NOLOCK)
	WHERE RequestId = @RequestId

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
		SET @ErrCode = 103
		SET @ErrMsg = 'Could not approved this request because this employee''s calendar have been changed.'

		RETURN
	END

	BEGIN TRY
		BEGIN TRANSACTION;

		IF @RoleId = @VerifiedStatusId
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = @VerifiedStatusId
				,VerifiedStatus = @VerifiedStatusId
				,VerifierId = @ActorId
				,VerifiedDate = @ProcessedDate
				,ModifiedBy = @ActorId
				,ModifiedDate = @ProcessedDate
			WHERE RequestId = @RequestId
		END
		ELSE IF @RoleId = @ApprovedStatusId
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = @ApprovedStatusId
				,ApprovedStatus = @VerifiedStatusId
				,ApproverComment = @Comment
				,ApproverId = @ActorId
				,ApprovedDate = @ProcessedDate
				,ModifiedBy = @ActorId
				,ModifiedDate = @ProcessedDate
			WHERE RequestId = @RequestId

			EXEC dbo.DisciplineRequestTrans_Insert @RequestId = @RequestId
				,@ModifiedBy = @ActorId
				,@IgnoredDetails = @DisciplineXml
				,@ErrCode = @ErrCode OUTPUT
				,@ErrMsg = @ErrMsg OUTPUT

			EXEC dbo.LeaveRequest_Approve_UpdateCalendar @RequestId
				,@EmpId
				,@ActorId

			EXEC dbo.AnnualLeave_InsertTransactionByRequestId @RequestId
				,@EmpId
				,@FromDate
				,@ToDate
				,@ActorId

			EXEC dbo.LeaveRequest_UpdateTmpAnnualLeaveTrans @Requestid = @Requestid
				,@EmpId = @EmpId
				,@Status = 1

			EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
				,@FromDate
				,@ToDate
				,1
		END
		ELSE IF @RoleId = @HRRoleId
		BEGIN
			UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
			SET CurrentStatus = @ProcessedStatusId
				,HRStatus = @VerifiedStatusId
				,HRId = @ActorId
				,HRDate = @ProcessedDate
				,ModifiedBy = @ActorId
				,ModifiedDate = @ProcessedDate
			WHERE RequestId = @RequestId
		END

		EXEC dbo.LeaveRequest_WriteLog @RequestId
			,@Comment

		COMMIT TRANSACTION;
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 104
			,@ErrMsg = 'Aprrove unsuccessfully.'

		ROLLBACK TRANSACTION
	END CATCH
END
