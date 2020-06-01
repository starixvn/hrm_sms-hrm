
CREATE PROCEDURE [dbo].[DisciplineRule_GetByGroupId] @TeamId INT
AS
/*
Created: Tim.Trinh@2019-11-05
Task: get discipline rule by group id
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @tblDisciplineRule TABLE (
		DisciplineTypeId SMALLINT
		,[Name] NVARCHAR(100)
		,IsApplyForLocal BIT
		,IsApplyForExpat BIT
		)

	INSERT INTO @tblDisciplineRule (
		DisciplineTypeId
		,[Name]
		,IsApplyForLocal
		)
	SELECT dtype.DisciplineTypeId
		,dtype.[Name]
		,ISNULL(dsr.Value, 0)
	FROM dbo.DisciplineType AS dtype WITH (NOLOCK)
		LEFT JOIN dbo.DisciplineGroupRule AS dsr WITH (NOLOCK) ON dsr.DisciplineTypeId = dtype.DisciplineTypeId
		AND dsr.GroupId = @TeamId
		AND dsr.IsLocal = 1
	WHERE dtype.DisciplineTypeId > 0

	UPDATE dtype
	SET dtype.IsApplyForExpat = ISNULL(dsr.Value, 0)
	FROM @tblDisciplineRule AS dtype
		LEFT JOIN dbo.DisciplineGroupRule AS dsr WITH (NOLOCK) ON dsr.DisciplineTypeId = dtype.DisciplineTypeId
		AND dsr.GroupId = @TeamId
		AND dsr.IsLocal = 0

	SELECT *
	FROM @tblDisciplineRule
END
