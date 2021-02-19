
CREATE PROCEDURE [dbo].[LateInEarlyOutRequest_WriteLog] @RequestId BIGINT
	,@Comment NVARCHAR(2000) = ''
	,@HrApproverId INT = NULL
AS
/*
Created: Quinn.Vo@20190625
Task: write log late in/early out request.
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
		,@LateInMins INT
		,@EarlyOutMins INT
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
		,@OnDate = li.OnDate
		,@LateInMins = li.LateInMins
		,@EarlyOutMins = li.EarlyOutMins
		,@ReasonId = li.ReasonId
		,@RefNo = li.RefNo
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
		INNER JOIN dbo.LateInEarlyOutRequest AS li WITH (NOLOCK) ON wk.RequestId = li.RequestId
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

	INSERT INTO dbo.LateInEarlyOutRequestHistory (
	 [RequestId]
      ,[EmpId]
      ,[OnDate]
      ,[LateInMins]
      ,[EarlyOutMins]
      ,[ReasonId]
      ,[RefNo]
      ,[HistoryId]
		)
	VALUES (
		@RequestId
		,@EmpId
		,@OnDate
		,@LateInMins
		,@EarlyOutMins
		,@ReasonId
		,@RefNo
		,@HistoryId
		)
END