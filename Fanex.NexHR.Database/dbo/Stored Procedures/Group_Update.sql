
/****** Object:  Stored Procedure dbo.Group_Update    Script Date: 7/20/2016 10:19:09 AM ******/
/****** Object:  Stored Procedure dbo.Group_Update    Script Date: 7/19/2016 5:22:16 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Group_Update] @GroupId INT
	,@Code VARCHAR(20)
	,@Name NVARCHAR(50)
	,@DepartmentId INT
	,@MailGroup VARCHAR(50) = ''
	,@LeaderIds VARCHAR(50)
	,@AssistantLeaderIds VARCHAR(50) = ''
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-23
Task: Update group 
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @GroupId_other INT

	SELECT TOP 1 @GroupId_other = GroupId
	FROM dbo.[Group] WITH (NOLOCK)
	WHERE Code = @code
		AND IsDeleted = 0

	IF @GroupId <> @GroupId_other
	BEGIN
		SELECT @ErrCode = 100
			,@ErrMsg = N'Code "' + @code + '" have been used by other Group.'

		RETURN
	END

	DELETE
	FROM [dbo].[GroupLeader] WITH (
			ROWLOCK
			,UPDLOCK
			)
	WHERE GroupId = @GroupId

	INSERT INTO [dbo].[GroupLeader] (
		GroupId
		,EmployeeId
		,IsLeader
		)
	SELECT @GroupId
		,val
		,1
	FROM fnc_strSplit(@LeaderIds, ',')

	IF (
			@AssistantLeaderIds IS NOT NULL
			AND @AssistantLeaderIds <> ''
			)
	BEGIN
		INSERT INTO [dbo].[GroupLeader] (
			GroupId
			,EmployeeId
			,IsLeader
			)
		SELECT @GroupId
			,val
			,NULL
		FROM fnc_strSplit(@AssistantLeaderIds, ',')
	END

	UPDATE [dbo].[Group]
	WITH (
			ROWLOCK
			,UPDLOCK
			)

	SET Code = @Code
		,[Name] = @Name
		,DepartmentId = @DepartmentId
		,[MailGroup] = @MailGroup
		,ModifiedDate = GETDATE()
		,ModifiedBy = @ModifiedBy
	WHERE GroupId = @GroupId

	SELECT @ErrCode = 0
		,@ErrMsg = ''
END


