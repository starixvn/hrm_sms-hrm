
CREATE PROCEDURE [dbo].[WorkingTimeRequest_GetLateInEarlyOutRequestToIgnoreById] @RequestId BIGINT
AS
/*
Created: 20160518@Tim.Trinh
Task: Get late in/early out request to ignore
DB: SMSHRM
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @RequestType INT

	SELECT @RequestType = RequestType
	FROM dbo.WorkingTimeRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	IF @RequestType IN (3, 4)
	BEGIN
		SELECT r.RequestId
			,inout.FromDate
			,inout.ToDate
			,inout.FromDate AS RequestedDate
			,r.Remark
			,r.CreatedDate
			,(CASE 
				WHEN r.RequestType = 3 THEN inout.LateInMins
				ELSE inout.EarlyOutMins END
				) AS TotalMins
			,shift.ShiftId
			,shift.[Name]
			,emp.EmpId AS RequesterId
			,emp.NickName
			,emp.FullName
			,r.RequestType
		FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
			INNER JOIN dbo.InOutRequest AS inout WITH (NOLOCK) ON r.RequestId = inout.RequestId
			INNER JOIN dbo.EmployeeCalendar AS ca WITH (NOLOCK) ON ca.EmpId = r.EmpId
			AND CAST(ca.WorkDate AS DATE) = CAST(inout.FromDate AS DATE)
			LEFT JOIN dbo.Shift AS shift WITH (NOLOCK) ON shift.ShiftID = ca.ShiftId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = r.EmpId
			AND emp.isDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.EmployeeSummary AS emps WITH (NOLOCK) ON emp.EmpId = emps.EmpId
			INNER JOIN dbo.DisciplineSectionRule AS drule WITH (NOLOCK) ON emp.GroupId = drule.GroupId
			AND emps.IsLocal = drule.IsLocal
			AND (
				(
					@RequestType = 3
					)
				OR (
					@RequestType = 4
					AND ISNULL(ca.IsOffDay, 0) = 0
					AND ISNULL(ca.IsHoliday, 0) = 0
					)
			)
			AND (
				(
					@RequestType = 3
					AND drule.DisciplineTypeId = 3
					)
				OR (
					@RequestType = 4
					AND drule.DisciplineTypeId = 4
					)
				)
		WHERE r.RequestId = @RequestId
	END
END
