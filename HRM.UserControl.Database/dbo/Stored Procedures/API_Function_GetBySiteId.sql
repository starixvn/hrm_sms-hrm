
CREATE PROCEDURE [dbo].[API_Function_GetBySiteId] @SiteId SmallInt
	,@IsActive BIT = NULL
AS
/*
Created: 20211027@Tim.Trinh
Task: get function by siteid
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT fnc.FunctionId
		,fnc.FunctionKey
		,fnc.FunctionName
		,fnc.SiteId
		,fnc.[Description]
		,fnc.ParentId
		,fnc.Policy
		,fnc.IsActive
		,fnc.DisplayOrder
		,fnc.ModifiedDate
		,u.Nickname as ModifiedBy
	FROM dbo.[Function] AS fnc WITH (NOLOCK)
	LEFT JOIN [dbo].[User] AS u WITH (NOLOCK) ON fnc.ModifiedBy = u.UserId
	WHERE fnc.SiteId = @SiteId
		AND (
			@IsActive IS NULL
			OR fnc.IsActive = @IsActive
			)
END