


CREATE PROCEDURE [dbo].[EmployeeDiscipline_GetOtherDiscipline] @EmpId INT
	,@FromDate DATE
	,@ToDate DATE
AS
/*
Created: 20160414@Tim.Trinh
Task: Get other violations by empId.
DB: SMSHRM
Original: 

Revisions:  
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT d.WorkDate
		,d.EmpId AS EmployeeId
		,emp.NickName
		,dtype.DisciplineTypeId
		,dtype.[Name]
		,d.DisciplineScore AS Score
		,d.ApproverComment AS Comment
	FROM dbo.EmployeeDiscipline AS d WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON d.EmpId = emp.EmpId
		INNER JOIN dbo.DisciplineType AS dtype WITH (NOLOCK) ON d.DisciplineType = dtype.DisciplineTypeId
		AND dtype.DisciplineTypeId IN (- 1, - 2, -3) -- verbal, written warning
	WHERE emp.EmpId = @EmpId
		AND WorkDate BETWEEN @FromDate
			AND @ToDate
		AND ISNULL(d.IsDeleted, 0) = 0
END
