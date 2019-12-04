
CREATE PROCEDURE [dbo].[OtherDiscipline_Get] @EmpId INT
	,@FromDate DATE
	,@ToDate DATE
AS
/*
Created: 20191204@Tim.Trinh
Task: get other discipline.
DB: SMSHRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT d.WorkDate
		,d.EmpId AS EmployeeId
		,emp.FullName
		,emp.NickName
		,dtype.DisciplineTypeId
		,dtype.[Name]
		,d.Score
		,d.Comment
	FROM dbo.OtherDiscipline AS d WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON d.EmpId = emp.EmpId
		AND emp.EmpId = @EmpId
		INNER JOIN dbo.DisciplineType AS dtype WITH (NOLOCK) ON d.DisciplineTypeId = dtype.DisciplineTypeId
	WHERE d.WorkDate BETWEEN @FromDate
			AND @ToDate
		AND d.IsDeleted = 0
END
