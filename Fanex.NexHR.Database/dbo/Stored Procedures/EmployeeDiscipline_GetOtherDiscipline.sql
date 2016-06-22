
CREATE PROCEDURE [dbo].[EmployeeDiscipline_GetOtherDiscipline] @EmpId INT
	,@FromDate DATETIME
	,@ToDate DATETIME
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
		,dtype.Score
		,d.ApproverComment AS Comment
	FROM dbo.EmployeeDiscipline AS d WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON d.EmpId = emp.EmpId
		INNER JOIN dbo.DisciplineType AS dtype WITH (NOLOCK) ON d.DisciplineType = dtype.DisciplineTypeId
		AND dtype.DisciplineTypeId IN (- 1, - 2) -- verbal, written warning
	WHERE emp.EmpId = @EmpId
		AND CAST(WorkDate AS DATE) BETWEEN CAST(@FromDate AS DATE)
			AND CAST(@ToDate AS DATE)
END
