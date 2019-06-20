/****** Object:  Stored Procedure dbo.WorkingTimeRequest_GetCheckInOutRequestToIgnoreById    Script Date: 7/20/2016 10:19:21 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_GetCheckInOutRequestToIgnoreById    Script Date: 7/19/2016 5:22:22 PM ******/

CREATE PROCEDURE [dbo].[WorkingTimeRequest_GetCheckInOutRequestToIgnoreById] @RequestId BIGINT
AS
/*
Created: 20160518@Tim.Trinh
Task: Get check in/out request to ignore
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

	IF @RequestType IN (1, 2)
	BEGIN
		SELECT r.RequestId
			,lr.OnDate AS FromDate
			,lr.OnDate AS ToDate
			,lr.CheckTime AS RequestedDate
			,r.Remark
			,r.CreatedDate
			,shift.ShiftId
			,shift.[Name]
			,emp.EmpId AS RequesterId
			,emp.NickName
			,emp.FullName
			,r.RequestType
		FROM dbo.WorkingTimeRequest AS r WITH (NOLOCK)
			INNER JOIN dbo.InoutMissRequest AS lr WITH (NOLOCK) ON r.RequestId = lr.RequestId
			INNER JOIN dbo.EmployeeCalendar AS ca WITH (NOLOCK) ON ca.EmpId = r.EmpId
			AND CAST(ca.WorkDate AS DATE) = CAST(lr.OnDate AS DATE)
			LEFT JOIN dbo.Shift AS shift WITH (NOLOCK) ON shift.ShiftID = ca.ShiftId
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = r.EmpId
			AND emp.isDeleted = 0
			AND emp.[Enabled] = 1
			INNER JOIN dbo.EmployeeSummary AS emps WITH (NOLOCK) ON emp.EmpId = emps.EmpId
			INNER JOIN dbo.DisciplineSectionRule AS drule WITH (NOLOCK) ON emp.GroupId = drule.GroupId
			AND emps.IsLocal = drule.IsLocal
			AND drule.DisciplineTypeId = 2
		WHERE r.RequestId = @RequestId
	END
END
