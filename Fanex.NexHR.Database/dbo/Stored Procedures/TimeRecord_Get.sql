
CREATE PROCEDURE [dbo].[TimeRecord_Get] --NULL, NULL, '2015-09-15', '2015-09-15'
	@EmpID INT = NULL,	
	@isLocal BIT = NULL,
	@fromDate DATETIME, 
	@toDate DATETIME, 
	@groupIds VARCHAR(MAX) = ''
AS
/*
Created: Quinn.Vo@2014-01-24
Task: Get Time Record
DB: HRM_SMS
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET ARITHABORT ON;

	DECLARE @GroupsList TABLE (GroupId TINYINT)
	INSERT INTO @GroupsList (GroupId)
	SELECT val FROM fnc_strSplit (@groupids, ',')

	SELECT t.ID AS RecordTimeId, t.RecordTime, t.InOutType, e.EmpId, e.Code, e.NickName, e.FullName, g.GroupId, g.Code AS GroupCode, g.Name
	FROM dbo.TimeRecord t WITH (NOLOCK) 
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON t.EmpID = e.EmpId
		INNER JOIN dbo.[Group] g WITH (NOLOCK) ON e.GroupId = g.GroupId
		LEFT JOIN dbo.EmployeeSummary e_s WITH (NOLOCK)  ON e_s.EmpId = e.EmpId 
	WHERE 
		CONVERT(VARCHAR(10), RecordTime, 111) BETWEEN CONVERT(VARCHAR(10), @fromDate, 111) AND CONVERT(VARCHAR(10), @toDate, 111)
		AND (@EmpID IS NULL OR e.EmpId = @EmpID) 
		AND (@groupIds = '' OR e.GroupId IN (SELECT GroupId FROM @GroupsList))
		AND (@isLocal IS NULL OR e_s.IsLocal = @isLocal )
	ORDER BY e.EmpId, t.RecordTime
END