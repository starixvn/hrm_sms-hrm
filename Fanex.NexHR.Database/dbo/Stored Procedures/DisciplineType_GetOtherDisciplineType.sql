/****** Object:  Stored Procedure dbo.DisciplineType_GetOtherDisciplineType    Script Date: 7/20/2016 10:19:01 AM ******/
/****** Object:  Stored Procedure dbo.DisciplineType_GetOtherDisciplineType    Script Date: 7/19/2016 5:22:11 PM ******/

CREATE PROCEDURE [dbo].[DisciplineType_GetOtherDisciplineType]
AS
/*
Created: 20160415@Tim.Trinh
Task: Get other violation type.
DB: SMSHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT DisciplineTypeId
		,[Name]
		,Score
	FROM dbo.DisciplineType WITH (NOLOCK)
	WHERE DisciplineTypeId < 0
END
