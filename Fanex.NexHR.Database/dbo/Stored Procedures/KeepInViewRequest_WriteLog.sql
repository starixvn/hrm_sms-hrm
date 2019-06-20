
CREATE PROCEDURE [dbo].[KeepInViewRequest_WriteLog] @RequestId BIGINT
	,@Comment NVARCHAR(2000) = ''
AS
/*
Created: 20190522@Quinn.Vo
Task: write log keep in view.
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
		,@KeepInViewType TINYINT
		,@RequestType INT = 7
		,@OnDate DATE
		,@IsHalfDay BIT
		,@FromTime DATETIME
		,@ToTime DATETIME
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
		,@KeepInViewType = KeepInViewType
		,@OnDate = kiv.OnDate
		,@IsHalfDay = kiv.IsHalfDay
		,@FromTime = kiv.FromTime
		,@ToTime = kiv.ToTime
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
		INNER JOIN dbo.KeepInViewRequest AS kiv WITH (NOLOCK) ON wk.RequestId = kiv.RequestId
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

	INSERT INTO dbo.KeepInViewRequestHistory (
		[RequestId]
		,[EmpId]
		,[OnDate]
		,[KeepInViewType]
		,[IsHalfDay]
		,[FromTime]
		,[ToTime]
		,[HistoryId]
		)
	VALUES (
		@RequestId
		,@EmpId
		,@OnDate
		,@KeepInViewType
		,@IsHalfDay
		,@FromTime
		,@ToTime
		,@HistoryId
		)
END