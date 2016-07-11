CREATE PROCEDURE [dbo].[Employee_GetNewEmployeeCode] AS

/*
Created: Tom.Vo@2013-10-21
Task: get employee by code
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Code VARCHAR(10)

	SELECT @Code = MAX(CASE 
				WHEN ISNUMERIC(Right(Code, 4)) = 1 THEN Right(Code, 4)
				ELSE - 1
				END + 1)
	FROM dbo.Employee WITH (NOLOCK)
	WHERE [Enabled] = 1
		AND CASE 
			WHEN ISNUMERIC(Right(Code, 4)) = 1 THEN Right(Code, 4)
			ELSE - 1
			END < 9000

	IF @Code IS NOT NULL
		SELECT REPLICATE('0', 4 - LEN(@Code)) + @Code AS Code
	ELSE
		SELECT '0001' Code
END
