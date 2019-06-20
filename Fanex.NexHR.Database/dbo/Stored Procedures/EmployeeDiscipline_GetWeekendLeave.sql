

CREATE PROCEDURE [dbo].[EmployeeDiscipline_GetWeekendLeave] @EmpId INT
	,@FromDate DATE
	,@ToDate DATE
AS
/*
Created: 20171102@Tim.Trinh
Task: Get weekend leave by empId.
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
		,WeekendLeave AS Score
	FROM dbo.EmployeeDiscipline AS d WITH (NOLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON d.EmpId = emp.EmpId
	WHERE emp.EmpId = @EmpId
		AND WorkDate BETWEEN @FromDate
			AND @ToDate
		AND ISNULL(d.IsDeleted, 0) = 0
		AND d.WeekendLeave > 0
END