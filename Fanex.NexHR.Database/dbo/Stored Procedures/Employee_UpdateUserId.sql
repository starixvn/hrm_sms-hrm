
CREATE PROCEDURE [dbo].[Employee_UpdateUserId] @EmpId INT
	,@UserId INT
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20220111@Tim.Trinh
Task: update userid
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @xml XML
		,@logData NVARCHAR(MAX)
		,@logOldData NVARCHAR(MAX)

	SET @xml = (
			SELECT EmpId AS '@EmpId'
				,UserId AS '@UserId'
			FROM dbo.Employee WITH (NOLOCK)
			WHERE EmpId = @EmpId
			FOR XML PATH('Row')
				,TYPE
			)
	SET @logOldData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	UPDATE dbo.Employee WITH (ROWLOCK)
	SET UserId = @UserId
		,ModifiedDate = GETDATE()
		,ModifiedBy = @ModifiedBy
	WHERE EmpId = @EmpId

	SET @xml = (
			SELECT EmpId AS '@EmpId'
				,UserId AS '@UserId'
			FROM dbo.Employee WITH (NOLOCK)
			WHERE EmpId = @EmpId
			FOR XML PATH('Row')
				,TYPE
			)
	SET @logData = REPLACE(CONVERT(NVARCHAR(MAX), @xml), '" />', '"/"')

	EXEC dbo.ActivityLog_Insert @UserId = @ModifiedBy
		,@ObjectTypeId = 1
		,@ObjectId = @EmpId
		,@OldData = @logOldData
		,@Data = @logData
		,@ErrCode = @ErrCode OUTPUT
		,@ErrMsg = @ErrMsg OUTPUT
END