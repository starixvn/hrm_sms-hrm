/****** Object:  Stored Procedure dbo.WorkingTimeRequest_WriteHistory    Script Date: 7/20/2016 10:19:26 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_WriteHistory    Script Date: 7/19/2016 5:22:24 PM ******/
-- WorkingTimeRequest_WriteHistory 316

CREATE PROCEDURE [dbo].[WorkingTimeRequest_WriteHistory]	
	@RequestID bigint,
	@Comment NVARCHAR(500)=''
AS
/*
Created: Tom.Vo@2013-11-04
Task: Copy request to history 
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/

SET NOCOUNT ON

DECLARE

	@EmpId int ,
	@RequestType int ,
	@Remark nvarchar(2000) ,
	@CurrentStatus tinyint ,
	@VerifierId int ,
	@ApproverId int ,
	@HRId int ,
	@VerifiedDate datetime ,
	@ApprovedDate datetime ,
	@HRDate datetime ,
	@VerifiedStatus tinyint ,
	@ApprovedStatus tinyint ,
	@HRStatus tinyint ,
	@CreatedBy int ,
	@CreatedDate datetime ,
	@ModifiedBy int ,
	@ModifiedDate datetime ,
	@ReworkCount int 

SELECT
		@EmpId=	 EmpId
		,@RequestType=RequestType
		,@Remark=Remark
		,@CurrentStatus=CurrentStatus

		,@VerifierId=VerifierId
		,@ApproverId=ApproverId
		,@HRId=HRId
		,@VerifiedStatus=VerifiedStatus
		,@ApprovedStatus=ApprovedStatus
		,@HRStatus=HRStatus
		
		,@VerifiedDate=VerifiedDate
		,@ApprovedDate=ApprovedDate
		,@HRDate=HRDate

		,@CreatedBy=CreatedBy
		,@CreatedDate=CreatedDate

		,@ModifiedDate=ModifiedDate
		,@ModifiedBy=ModifiedBy

FROM dbo.WorkingTimeRequest WITH(NOLOCK) 
WHERE RequestID = @RequestID

	IF (@RequestType = 6)
	 BEGIN
		SELECT @ReworkCount  = MAX(lrh.ReworkCount)  FROM dbo.LeaveRequest lrh WITH(NOLOCK) WHERE  lrh.RequestId = @RequestID
	 END
	
	SET @ReworkCount = ISNULL(@ReworkCount,0)

IF @CurrentStatus NOT IN (1) 
	BEGIN
		IF @Comment <> ''
			SET @Remark = @Comment
		ELSE
			SET @Remark = ''
	END


INSERT dbo.WorkingTimeRequestHistory
		(
		EmpId,RequestID,RequestType,Remark,CurrentStatus
		,VerifierId,ApproverId,HRId,VerifiedDate,ApprovedDate,HRDate,VerifiedStatus,ApprovedStatus,HRStatus
		,CreatedDate,CreatedBy,ModifiedDate,ModifiedBy,ReworkCount,InsertedDate
		)
	VALUES(
		@EmpId,@RequestID,@RequestType,@Remark,@CurrentStatus
		,@VerifierId,@ApproverId,@HRId,@VerifiedDate,@ApprovedDate,@HRDate,@VerifiedStatus,@ApprovedStatus,@HRStatus
		,@CreatedDate,@CreatedBy,@ModifiedDate,@ModifiedBy,@ReworkCount,GETDATE()	
	)