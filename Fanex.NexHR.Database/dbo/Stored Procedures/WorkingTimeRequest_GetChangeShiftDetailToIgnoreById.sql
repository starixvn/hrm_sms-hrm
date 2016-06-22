
CREATE PROCEDURE [dbo].[WorkingTimeRequest_GetChangeShiftDetailToIgnoreById] @RequestId BIGINT
AS
/*
Created: 20160520@Tim.Trinh
Task: Get change shift detail to ignore
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

	IF @RequestType = 5
	BEGIN
		SELECT rd.OnDate
			,rd.FromShift AS FromShiftId
			,s1.Code AS FromShift
			,rd.ToShift AS ToShiftId
			,s2.Code AS ToShift
			,rd.Remark
		FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
			INNER JOIN dbo.ChangeShiftRequestDetail AS rd WITH (NOLOCK) ON r.RequestId = rd.RequestId
			LEFT JOIN dbo.Shift AS s1 WITH (NOLOCK) ON s1.ShiftID = rd.FromShift
			LEFT JOIN dbo.Shift AS s2 WITH (NOLOCK) ON s2.ShiftID = rd.ToShift
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON r.EmpId = emp.EmpId
			AND emp.isDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.EmployeeSummary AS emps WITH (NOLOCK) ON emp.EmpId = emps.EmpId
			INNER JOIN dbo.DisciplineSectionRule AS drule WITH (NOLOCK) ON emp.GroupId = drule.GroupId
			AND emps.IsLocal = drule.IsLocal
			AND drule.DisciplineTypeId = 1
		WHERE r.RequestId = @RequestId
		ORDER BY rd.OnDate
	END
END
