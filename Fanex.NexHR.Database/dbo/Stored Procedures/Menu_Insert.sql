
CREATE PROCEDURE [dbo].[Menu_Insert] @Name NVARCHAR(50)
	,@Text NVARCHAR(100) = NULL
	,@Link NVARCHAR(100) = NULL
	,@ParentId INT = NULL
	,@FunctionId INT = NULL
	,@Policy INT = NULL
	,@DisplayOrder INT = NULL
	,@ErrorCode INT = 0 OUTPUT
	,@ErrorMessage VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @IsActive BIT

	SET @IsActive = 1

	INSERT INTO [dbo].[Menu]
	VALUES (
		@Name
		,@Text
		,@Link
		,@ParentId
		,@FunctionId
		,@DisplayOrder
		,@IsActive
		,@Policy
		);
END;