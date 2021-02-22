
CREATE PROCEDURE [dbo].[ChangeShiftRequest_WriteLog] @RequestId BIGINT
	,@Comment NVARCHAR(2000) = ''
AS
/*
Created: Quinn.Vo@20190624
Task: write log check in out request.
DB: HRM
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @EmpId INT
		,@RequestType INT
		,@FromDate DATETIME
		,@ToDate DATETIME
		,@TypeId TINYINT
		,@Remark NVARCHAR(2000)
		,@CurrentStatus TINYINT
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
		,@FromDate = cs.FromDate
		,@ToDate = cs.ToDate
		,@TypeId = cs.TypeId
		,@RequestType = wk.RequestType
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
		INNER JOIN dbo.ChangeShiftRequest AS cs WITH (NOLOCK) ON wk.RequestId = cs.RequestId
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

	INSERT INTO dbo.ChangeShiftRequestHistory (
		[RequestId]
		,[EmpId]
		,[FromDate]
		,[ToDate]
		,TypeId
		,[HistoryId]
		)
	VALUES (
		@RequestId
		,@EmpId
		,@FromDate
		,@ToDate
		,@TypeId
		,@HistoryId
		)

	INSERT INTO dbo.ChangeShiftRequestDetailHistory (
		[OnDate]
		,[EmpId]
		,[RequestId]
		,[FromShift]
		,[ToShift]
		,[Remark]
		,[ToDate]
		,[HistoryId]
		)
	SELECT OnDate
		,EmpId
		,RequestId
		,FromShift
		,ToShift
		,Remark
		,ToDate
		,@HistoryId
	FROM dbo.ChangeShiftRequestDetail WITH (NOLOCK)
	WHERE RequestId = @RequestId
END
