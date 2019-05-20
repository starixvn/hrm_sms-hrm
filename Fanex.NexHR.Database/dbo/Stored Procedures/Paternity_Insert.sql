/****** Object:  Stored Procedure dbo.Department_Insert    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.Department_Insert    Script Date: 7/19/2016 5:22:11 PM ******/

CREATE PROCEDURE [dbo].[Paternity_Insert] 
	@EmpId INT
	,@ChildBornDate DATETIME
	,@LeaveDay INT
	,@CreatedBy INT
	,@Remark NVARCHAR(MAX) = ''
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190429@Oliver.Dang
Task: Insert Paternity.
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
				FROM dbo.EmployeePaternity WITH (NOLOCK)
				WHERE EmpId = @EmpId AND ChildBornDate = @ChildBornDate
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'The paternity of this employee has children born on' + CONVERT(varchar(10), @ChildBornDate, 103) + ' has already exists'

		RETURN
	END

	INSERT INTO dbo.EmployeePaternity(
		EmpId
		,ChildBornDate
		,LeaveDay
		,CreatedBy
		,CreatedDate
		,Remark
		)
	VALUES (
		@EmpId
		,@ChildBornDate
		,@LeaveDay
		,@CreatedBy
		,GETDATE()
		,@Remark
		)

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert paternity failed.'
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END