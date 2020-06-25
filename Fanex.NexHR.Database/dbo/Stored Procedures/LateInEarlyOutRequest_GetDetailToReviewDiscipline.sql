
CREATE PROCEDURE [dbo].[LateInEarlyOutRequest_GetDetailToReviewDiscipline] @RequestId BIGINT
AS
/*
Created: 20190930@Tim.Trinh
Task: get late in/early out request to review discipline
DB: SMS_HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @LateInRequestTypeId TINYINT = 3
		,@EarlyOutRequestTypeId TINYINT = 4

	SELECT lio.RequestId
		,lio.OnDate AS FromDate
		,lio.OnDate AS ToDate
		,lio.OnDate AS RequestedDate
		,lio.ReasonId
		,wkr.Remark
		,wkr.CreatedDate
		,(CASE 
			WHEN wkr.RequestType = @LateInRequestTypeId THEN lio.LateInMins
			ELSE lio.EarlyOutMins END
			) AS TotalMins
		,shift.ShiftId
		,shift.[Name]
		,emp.EmpId AS RequesterId
		,emp.NickName
		,emp.FullName
		,wkr.RequestType
	FROM dbo.WorkingTimeRequest AS wkr WITH (NOLOCK)
		INNER JOIN dbo.LateInEarlyOutRequest AS lio WITH (NOLOCK) ON wkr.RequestId = lio.RequestId
		AND lio.RequestId = @RequestId
		INNER JOIN dbo.EmployeeCalendar AS ca WITH (NOLOCK) ON ca.EmpId = wkr.EmpId
		AND CAST(ca.WorkDate AS DATE) = CAST(lio.OnDate AS DATE)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = wkr.EmpId
		INNER JOIN dbo.EmployeeSummary AS emps WITH (NOLOCK) ON emp.EmpId = emps.EmpId
		INNER JOIN dbo.DisciplineGroupRule AS drule WITH (NOLOCK) ON emp.GroupId = drule.GroupId
		AND emps.IsLocal = drule.IsLocal
		AND drule.Value = 1
		AND (
			wkr.RequestType = @LateInRequestTypeId
			OR (
				wkr.RequestType = @EarlyOutRequestTypeId
				AND ISNULL(ca.IsOffDay, 0) = 0
				AND ISNULL(ca.IsHoliday, 0) = 0
				)
			)
		AND (
			(
				wkr.RequestType = @LateInRequestTypeId
				AND drule.DisciplineTypeId = 3
				)
			OR (
				wkr.RequestType = @EarlyOutRequestTypeId
				AND drule.DisciplineTypeId = 4
				)
			)
		LEFT JOIN dbo.Shift AS shift WITH (NOLOCK) ON shift.ShiftId = ca.ShiftId
END
