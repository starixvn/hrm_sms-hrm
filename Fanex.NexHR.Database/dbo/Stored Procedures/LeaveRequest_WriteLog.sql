
CREATE PROCEDURE [dbo].[LeaveRequest_WriteLog] @RequestId BIGINT
	,@Comment NVARCHAR(2000) = ''
AS
/*
Created: 20190620@Quinn.Vo
Task: write log leave request.
DB: HRM
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmpId INT
		,@Remark NVARCHAR(2000)
		,@CurrentStatus TINYINT
		,@RequestType INT = 6
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@LeaveType INT
		,@TotalDays DECIMAL(18, 2)

		,@ApproverComment NVARCHAR(2000)
		,@VerifierId INT
		,@ApproverId INT
		,@HRId INT
		,@VerifiedDate DATETIME
		,@ApprovedDate DATETIME
		,@HRDate DATETIME
		,@VerifiedStatus TINYINT
		,@ApprovedStatus TINYINT
		,@HRStatus TINYINT
		,@CreatedBy INT
		,@CreatedDate DATETIME
		,@ModifiedBy INT
		,@ModifiedDate DATETIME
		,@HistoryId BIGINT

	SELECT @EmpId = wk.EmpId
		,@Remark = Remark
		,@ApproverComment = wk.ApproverComment
		,@CurrentStatus = CurrentStatus
		,@VerifierId = VerifierId
		,@ApproverId = ApproverId
		,@HRId = HRId
		,@VerifiedStatus = VerifiedStatus
		,@ApprovedStatus = ApprovedStatus
		,@HRStatus = HRStatus
		,@VerifiedDate = VerifiedDate
		,@ApprovedDate = ApprovedDate
		,@HRDate = HRDate
		,@CreatedBy = CreatedBy
		,@CreatedDate = CreatedDate
		,@ModifiedDate = ModifiedDate
		,@ModifiedBy = ModifiedBy
	FROM dbo.WorkingTimeRequest AS wk WITH (NOLOCK)
		INNER JOIN dbo.LeaveRequest AS lr WITH (NOLOCK) ON wk.RequestId = lr.RequestId
	WHERE wk.RequestID = @RequestId

	IF @CurrentStatus <> 1
	BEGIN
		IF @Comment <> ''
			SET @Remark = @Comment
		ELSE
			SET @Remark = ''
	END

	INSERT INTO dbo.WorkingTimeRequestHistory (
		[RequestId]
		,[EmpId]
		,[RequestType]
		,[Remark]
		,[ApproverComment]
		,[CurrentStatus]
		,[VerifierId]
		,[ApproverId]
		,[HRId]
		,[VerifiedDate]
		,[ApprovedDate]
		,[HRDate]
		,[VerifiedStatus]
		,[ApprovedStatus]
		,[HRStatus]
		,[ModifiedDate]
		,[ModifiedBy]
		,[CreatedBy]
		,[CreatedDate]
		,[LogDate]
		)
	VALUES (
		@RequestId
		,@EmpId
		,@RequestType
		,@Remark
		,@ApproverComment
		,@CurrentStatus
		,@VerifierId
		,@ApproverId
		,@HRId
		,@VerifiedDate
		,@ApprovedDate
		,@HRDate
		,@VerifiedStatus
		,@ApprovedStatus
		,@HRStatus
		,@ModifiedDate
		,@ModifiedBy
		,@CreatedBy
		,@CreatedDate
		,GETDATE()
		)

	SELECT @HistoryId = CAST(SCOPE_IDENTITY() AS BIGINT)

	INSERT INTO dbo.LeaveRequestHistory (
		[RequestId]
		,[EmpId]
		,[FromDate]
		,[ToDate]
		,[LeaveType]
		,[TotalDays]
		,[HistoryId]
		)
	SELECT [RequestId]
		,[EmpId]
		,[FromDate]
		,[ToDate]
		,[LeaveType]
		,[TotalDays]
		,@HistoryId
	FROM dbo.LeaveRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	INSERT INTO dbo.LeaveRequestDetailHistory (
		[RequestId]
		,[EmpId]
		,[OnDate]
		,[ShiftId]
		,[LeaveType]
		,[IsHalfDay]
		,[FromTime]
		,[ToTime]
		,[WorkType]
		,[HistoryId]
		)
	SELECT [RequestId]
		,[EmpId]
		,[OnDate]
		,[ShiftId]
		,[LeaveType]
		,[IsHalfDay]
		,[FromTime]
		,[ToTime]
		,[WorkType]
		,@HistoryId
	FROM dbo.LeaveRequestDetail WITH (NOLOCK)
	WHERE RequestId = @RequestId
END