
/****** Object:  Stored Procedure dbo.Group_Insert    Script Date: 7/20/2016 10:19:09 AM ******/
/****** Object:  Stored Procedure dbo.Group_Insert    Script Date: 7/19/2016 5:22:16 PM ******/
CREATE PROCEDURE [dbo].[Group_Insert] @Code VARCHAR(20)
	,@Name VARCHAR(50)
	,@DepartmentId INT
	,@MailGroup VARCHAR(50) = ''
	,@LeaderIds VARCHAR(50)
	,@AssistantLeaderIds VARCHAR(50) = ''
	,@CreatedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190515@Oliver.Dang
Task: INSERT group 
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.[Group] WITH (NOLOCK)
			WHERE Code = @Code
				AND IsDeleted = 0
			)
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Code "' + @code + '" exists.'

		RETURN
	END

	INSERT INTO dbo.[Group] (
		Code
		,[Name]
		,DepartmentId
		,MailGroup
		,CreatedDate
		,ModifiedDate
		,CreatedBy
		)
	VALUES (
		@Code
		,@Name
		,@DepartmentId
		,@MailGroup
		,GETDATE()
		,GETDATE()
		,@CreatedBy
		)

	DECLARE @id INT

	IF @@ERROR <> 0
	BEGIN
		SET @ID = 0

		SELECT @ErrCode = 1
			,@ErrMsg = N'Create Team failed.';
	END
	ELSE
	BEGIN
		SET @id = cast(SCOPE_IDENTITY() AS INT)

		INSERT INTO dbo.GroupLeader (
			GroupId
			,EmployeeId
			,IsLeader
			)
		SELECT @id
			,val
			,1
		FROM fnc_strSplit(@LeaderIds, ',')

		INSERT INTO dbo.GroupLeader (
			GroupId
			,EmployeeId
			,IsLeader
			)
		SELECT @id
			,val
			,NULL
		FROM fnc_strSplit(@AssistantLeaderIds, ',')

		IF @@ERROR <> 0
		BEGIN
			SET @ID = 0

			SELECT @ErrCode = 1
				,@ErrMsg = N'Create Team failed.';
		END
	END

	SELECT @ErrCode = 0
		,@ErrMsg = ''
END
