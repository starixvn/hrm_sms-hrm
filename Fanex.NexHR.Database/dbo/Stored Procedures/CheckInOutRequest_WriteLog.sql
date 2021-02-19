
CREATE PROCEDURE [dbo].[CheckInOutRequest_WriteLog] @RequestId BIGINT
	,@Comment NVARCHAR(2000) = ''
	,@HrApproverId INT = NULL
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
		,@OnDate DATE
		,@CheckTime DATETIME
		,@ReasonId TINYINT
		,@RefNo NVARCHAR(200)
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
		,@OnDate = mis.OnDate
		,@CheckTime = mis.RequestTime
		,@ReasonId = mis.ReasonId
		,@RefNo = mis.RefNo
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
		INNER JOIN dbo.CheckInOutRequest AS mis WITH (NOLOCK) ON wk.RequestId = mis.RequestId
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
		,ISNULL(@HrApproverId, @ApproverId)
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

	INSERT INTO dbo.CheckInOutRequestHistory (
		[RequestId]
		,[EmpId]
		,[OnDate]
		,[RequestTime]
		,[RefNo]
		,[ReasonId]
		,[HistoryId]
		)
	VALUES (
		@RequestId
		,@EmpId
		,@OnDate
		,@CheckTime
		,@RefNo
		,@ReasonId
		,@HistoryId
		)
END