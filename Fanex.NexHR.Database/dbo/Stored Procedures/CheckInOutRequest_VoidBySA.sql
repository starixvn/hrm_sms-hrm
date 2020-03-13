
CREATE PROCEDURE [dbo].[CheckInOutRequest_VoidBySA] @RequestId BIGINT
	,@ActorId INT
	,@Comment NVARCHAR(500) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190624@Quinn.Vo
Task: void check in/out request by SA.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @StatusId TINYINT
		,@ProcessedDate DATETIME = GETDATE()
		,@OnDate DATE
		,@EmpId INT
		,@RequestTypeId SMALLINT
		-- request type
		,@CheckInRequestTypeId TINYINT = 1
		,@CheckOutRequestTypeId TINYINT = 2
		-- request status
		,@ApprovedStatusId TINYINT = 4
		,@ProcessedStatusId TINYINT = 6
		,@VoidStatusId TINYINT = 7

	SELECT @EmpId = cio.EmpId
		,@OnDate = cio.OnDate
		,@StatusId = r.CurrentStatus
		,@RequestTypeId = r.RequestType
	FROM dbo.CheckInOutRequest AS cio WITH (NOLOCK)
		INNER JOIN dbo.WorkingTimeRequest AS r WITH (NOLOCK) ON cio.RequestId = r.RequestId
		AND r.RequestId = @RequestId

	EXEC dbo.CheckInOutRequest_UpdateDiscipline @RequestId = @RequestId
		,@ActorId = @ActorId
		,@RequestStatusId = @VoidStatusId
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT

	UPDATE dbo.WorkingTimeRequest WITH (ROWLOCK, UPDLOCK)
	SET CurrentStatus = @VoidStatusId
		,ModifiedBy = @ActorId
		,ModifiedDate = @ProcessedDate
	WHERE RequestId = @RequestId

	IF @StatusId IN (@ApprovedStatusId, @ProcessedStatusId)
	BEGIN
		UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
		SET RequestedCheckinTime = CASE 
				WHEN @RequestTypeId = @CheckInRequestTypeId THEN NULL
				ELSE RequestedCheckinTime
				END
			,RequestedCheckoutTime = CASE 
				WHEN @RequestTypeId = @CheckOutRequestTypeId THEN NULL
				ELSE RequestedCheckoutTime
				END
			,ModifiedBy = @ActorId
			,ModifiedDate = @ProcessedDate
		WHERE WorkDate = @OnDate
			AND EmpId = @EmpId

		EXEC dbo.EmployeeAttendance_ProcessingAndInsert @EmpId
			,@OnDate
			,@OnDate
			,1
	END

	EXEC dbo.CheckInOutRequest_WriteLog @RequestId = @RequestId
		,@Comment = @Comment
END
