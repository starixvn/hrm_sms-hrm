
CREATE PROCEDURE [dbo].[Menu_Delete] @MenuId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20160609@Razor.Nguyen
Task: delete menu
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DELETE
	FROM [dbo].[Menu]
	WHERE [MenuId] = @MenuId
END