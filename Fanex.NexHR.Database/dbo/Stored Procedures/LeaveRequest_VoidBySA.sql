
CREATE PROCEDURE [dbo].[LeaveRequest_VoidBySA] @RequestId BIGINT
	,@ActorId INT
	,@Comment NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20151026@Quinn.Vo
Task: Void request by SA
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @StatusId INT
		,@ProcessedDate DATETIME = GETDATE()
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@EmpId INT
		-- request status
		,@ApprovedStatusId TINYINT = 4
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7

	SELECT @StatusId = r.CurrentStatus
		,@FromDate = lr.FromDate
		,@EmpId = r.EmpId
	FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
		INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON r.RequestId = lr.RequestId
		AND lr.RequestId = @RequestId

	EXEC dbo.LeaveRequest_UpdateDiscipline @RequestId = @RequestId
		,@ActorId = @ActorId
		,@RequestStatusId = @VoidStatusId
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT

	UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
	SET CurrentStatus = @VoidStatusId
		,HRId = @ActorId
		,HRDate = @ProcessedDate
		,ModifiedBy = @ActorId
		,ModifiedDate = @ProcessedDate
	WHERE RequestId = @RequestId

	IF @StatusId IN (@ApprovedStatusId, @ProcessedStatusId)
	BEGIN
		EXEC dbo.LeaveRequest_Reject_RollbackCalendar @RequestId
			,@EmpId
			,@ActorId

		EXEC dbo.AnnualLeave_DeleteTransactionByRequestId @RequestId
			,@ProcessedDate
			,@ActorId

		EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
			,@FromDate
			,@ToDate
			,1
	END

	EXEC dbo.LeaveRequest_UpdateTmpAnnualLeaveTrans @RequestId = @RequestId
		,@EmpId = @EmpId
		,@Status = 2

	EXEC dbo.LeaveRequest_UpdateTmpKIVLeaveTrans @RequestId = @RequestId
		,@EmpId = @EmpId
		,@Status = 2

	EXEC dbo.LeaveRequest_WriteLog @RequestId = @RequestId
		,@Comment = @Comment
END
