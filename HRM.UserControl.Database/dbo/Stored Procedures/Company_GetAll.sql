
CREATE PROCEDURE [dbo].[Company_GetAll] @IsActive BIT = NULL
AS
/*
Created: 20211116@Tim.Trinh
Task: get all company
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	SELECT CompanyId
		,[Name]
		,IsActive
	FROM dbo.Company WITH (NOLOCK)
	WHERE @IsActive IS NULL
		OR IsActive = @IsActive
END