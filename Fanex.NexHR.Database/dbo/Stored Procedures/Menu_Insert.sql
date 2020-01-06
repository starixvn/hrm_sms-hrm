
CREATE PROCEDURE [dbo].[Menu_Insert] @Name NVARCHAR(50)
	,@Text NVARCHAR(100) = NULL
	,@Link NVARCHAR(100) = NULL
	,@ParentId INT = NULL
	,@FunctionId INT = NULL
	,@DisplayOrder INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160609@Razor.Nguyen
Task: insert menu
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsActive BIT = 1

	INSERT INTO [dbo].[Menu]
	VALUES (
		@Name
		,@Text
		,@Link
		,@ParentId
		,@FunctionId
		,@DisplayOrder
		,@IsActive
		)
END