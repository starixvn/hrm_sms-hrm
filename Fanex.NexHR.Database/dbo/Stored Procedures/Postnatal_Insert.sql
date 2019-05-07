/****** Object:  Stored Procedure dbo.Department_Insert    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.Department_Insert    Script Date: 7/19/2016 5:22:11 PM ******/

CREATE PROCEDURE [dbo].[Postnatal_Insert] 
	@EmpId INT
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Insert Postnatal.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:
• @ErrCode:
	= 0: Transaction recorded successfully.
	> 0: Specific Error.
• @ErrMsg: Error description.

*/
BEGIN
	SET NOCOUNT ON;

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeePostnatal WITH (NOLOCK)
				WHERE EmpId = @EmpId AND FromDate = @FromDate AND ToDate = @ToDate
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The postnatal of this employee from' + CONVERT(varchar(10), @FromDate, 103) + ' and ' + CONVERT(varchar(10), @ToDate, 103) + ' has already exists'

		RETURN
	END

	INSERT INTO dbo.EmployeePostnatal(
		EmpId
		,FromDate
		,ToDate
		,CreatedBy
		,CreatedDate
		)
	VALUES (
		@EmpId
		,@FromDate
		,@ToDate
		,@CreatedBy
		,GETDATE()
		)

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert postnatal failed.'
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END