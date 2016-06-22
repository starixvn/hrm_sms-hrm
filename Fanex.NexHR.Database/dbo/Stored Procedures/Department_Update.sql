
CREATE PROCEDURE [dbo].[Department_Update] 
	@DepartmentId INT
	,@Code VARCHAR(20)
	,@Name VARCHAR(100)
	,@ParentId INT = NULL
	,@ManagerId INT = NULL
	,@IsActive BIT = NULL
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20151222@Quinn.Vo
Task: Update Department.
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
				FROM dbo.Department WITH (NOLOCK)
				WHERE Code = @Code
				AND DepartmentId <> @DepartmentId
				)
			)
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Department code' + @Code + ' has already exists'

		RETURN
	END

	UPDATE dbo.Department WITH (ROWLOCK, UPDLOCK)
	SET 
		[Code] = @Code,
		[Name] = @Name,
		[ParentId] = @ParentId,
		[ManagerId] = @ManagerId,
		[IsActive] = @IsActive,
		[ModifiedBy] = @ModifiedBy,
		[ModifiedDate] = GETDATE()
	WHERE DepartmentId = @DepartmentId

	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update department failed.'
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END
