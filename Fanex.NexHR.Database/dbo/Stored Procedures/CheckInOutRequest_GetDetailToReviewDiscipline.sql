
CREATE PROCEDURE [dbo].[CheckInOutRequest_GetDetailToReviewDiscipline] @RequestId BIGINT
AS
/*
Created: 20191001@Tim.Trinh
Task: Get check in/out request to review discipline
DB: SMSHRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT wkr.RequestId
		,cio.OnDate AS FromDate
		,cio.OnDate AS ToDate
		,cio.RequestTime AS RequestedDate
		,wkr.Remark
		,wkr.CreatedDate
		,cio.ReasonId
		,shift.ShiftId
		,shift.[Name]
		,emp.EmpId AS RequesterId
		,emp.NickName
		,emp.FullName
		,wkr.RequestType
	FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
		INNER JOIN dbo.CheckInOutRequest AS cio WITH (NOLOCK) ON wkr.RequestId = cio.RequestId
		INNER JOIN dbo.EmployeeCalendar AS empca WITH (NOLOCK) ON empca.EmpId = wkr.EmpId
		AND CAST(empca.WorkDate AS DATE) = cio.OnDate
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = wkr.EmpId
		INNER JOIN dbo.EmployeeSummary AS empsum WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		INNER JOIN dbo.DisciplineGroupRule AS drule WITH (NOLOCK) ON emp.GroupId = drule.GroupId
		AND empsum.IsLocal = drule.IsLocal
		AND drule.DisciplineTypeId = 2
		LEFT JOIN dbo.Shift AS shift WITH (NOLOCK) ON shift.ShiftID = empca.ShiftId
	WHERE wkr.RequestId = @RequestId
END
