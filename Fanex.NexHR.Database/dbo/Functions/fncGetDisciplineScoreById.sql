/****** Object:  User Defined Function dbo.fncGetDisciplineScoreById    Script Date: 7/20/2016 10:18:58 AM ******/
/****** Object:  User Defined Function dbo.fncGetDisciplineScoreById    Script Date: 7/19/2016 5:22:10 PM ******/

CREATE FUNCTION [dbo].[fncGetDisciplineScoreById] (@DisciplineTypeId INT)
RETURNS DECIMAL(5, 2)
AS
/*
Created: 20160421@Tim.Trinh
Task: Get discipline score by id.
DB: SMSHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	DECLARE @Score DECIMAL(5, 2)

	SELECT @Score = Score
	FROM dbo.DisciplineType WITH (NOLOCK)
	WHERE DisciplineTypeId = @DisciplineTypeId

	RETURN ISNULL(@Score, 0)
END
