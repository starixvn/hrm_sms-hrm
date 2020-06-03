/****** Object:  Stored Procedure dbo.EmployeeSupervisor_GetRequestByEmpID    Script Date: 7/20/2016 10:19:07 AM ******/
/****** Object:  Stored Procedure dbo.EmployeeSupervisor_GetRequestByEmpID    Script Date: 7/19/2016 5:22:15 PM ******/

CREATE PROCEDURE [dbo].[EmployeeSupervisor_GetRequestByEmpID] @EmpId INT
	,@Role TINYINT = NULL
AS
/*
Created: TOM.VO@2013/10/25
Task: Get list supervisor for Employee by EmpId.
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	IF @Role IS NULL
		SELECT su.OwnerId
			,ee.FullName
			,ee.NickName
			,su.ActionType
			,ee.Email
			,'' AS OrderCode
			,--OrderNo,
			MAX(CASE 
					WHEN RequestType IN (1, 2) THEN 1
					ELSE 0
					END) CheckInOuts
			,MAX(CASE 
					WHEN RequestType IN (3, 4) THEN 1
					ELSE 0
					END) LateInEarlyOuts
			,MAX(CASE 
					WHEN RequestType IN (5) THEN 1
					ELSE 0
					END) ChangeShifts
			,MAX(CASE 
					WHEN RequestType IN (6) THEN 1
					ELSE 0
					END) Leaves
			,MAX(CASE 
					WHEN RequestType IN (7) THEN 1
					ELSE 0
					END) KeepInViews
			,MAX(CASE 
					WHEN RequestType IN (8) THEN 1
					ELSE 0
					END) Overtimes
			,isnull(g.GroupID, - 1) AS [TeamID]
			,isnull(g.NAME, '') AS [TeamCode]
		FROM [dbo].[EmployeeSupervisor] su WITH (NOLOCK)
			INNER JOIN [dbo].Employee ee WITH (NOLOCK) ON su.OwnerId = ee.EmpId
			LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON ee.GroupId = g.GroupId
		WHERE su.EmpId = @EmpId
			AND ISNULL(su.IsDeleted, 0) = 0
		GROUP BY su.OwnerId
			,isnull(g.GroupID, - 1)
			,isnull(g.NAME, '')
			,ee.FullName
			,ee.NickName
			,su.ActionType
			,ee.Email --, OrderNo 
	ELSE IF @Role = 4
		SELECT su.OwnerId
			,ee.FullName
			,ee.NickName
			,su.ActionType
			,ee.Email
			,su.OrderCode
			,--OrderNo,
			0 CheckInOuts
			,0 LateInEarlyOuts
			,0 ChangeShifts
			,0 Leaves
			,0 KeepInViews
			,0 Overtimes
			,isnull(g.GroupID, - 1) AS [TeamID]
			,isnull(g.NAME, '') AS [TeamCode]
		FROM [dbo].[EmployeeSupervisor] su WITH (NOLOCK)
			INNER JOIN [dbo].Employee ee WITH (NOLOCK) ON su.OwnerId = ee.EmpId
			LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON ee.GroupId = g.GroupId
		WHERE su.EmpId = @EmpId
			AND su.ActionType = @Role
			AND ISNULL(su.IsDeleted, 0) = 0
		--GROUP BY su.OwnerId , ee.FullName, ee.NickName, su.ActionType,ee.Email--,OrderNo
		ORDER BY su.OrderCode
			,OrderNo
			,su.OwnerId
			,su.ActionType
	ELSE
		SELECT su.OwnerId
			,su.FullName
			,su.NickName
			,su.ActionType
			,su.Email
			,'' AS OrderCode
			,--OrderNo,
			su.CheckInOuts
			,su.LateInEarlyOuts
			,su.ChangeShifts
			,su.Leaves
			,su.KeepInViews
			,su.Overtimes
			,su.TeamId
			,su.[Team] AS [TeamCode]
			,ROW_NUMBER() OVER (
				ORDER BY MIN(ord.OrderNo)
				) OrderNo
		FROM (
			SELECT su.OwnerId
				,ee.FullName
				,ee.NickName
				,su.ActionType
				,ee.Email
				,--OrderNo,
				MAX(CASE 
						WHEN RequestType IN (1, 2) THEN 1
						ELSE 0
						END) CheckInOuts
				,MAX(CASE 
						WHEN RequestType IN (3, 4) THEN 1
						ELSE 0
						END) LateInEarlyOuts
				,MAX(CASE 
						WHEN RequestType IN (5) THEN 1
						ELSE 0
						END) ChangeShifts
				,MAX(CASE 
						WHEN RequestType IN (6) THEN 1
						ELSE 0
						END) Leaves
				,MAX(CASE 
						WHEN RequestType IN (7) THEN 1
						ELSE 0
						END) KeepInViews
				,MAX(CASE 
						WHEN RequestType IN (8) THEN 1
						ELSE 0
						END) Overtimes
				,isnull(g.GroupID, - 1) AS [TeamID]
				,isnull(g.NAME, '') AS [Team]
			FROM [dbo].[EmployeeSupervisor] su WITH (NOLOCK)
				INNER JOIN [dbo].Employee ee WITH (NOLOCK) ON su.OwnerId = ee.EmpId
				LEFT JOIN dbo.[Group] AS g WITH (NOLOCK) ON ee.GroupId = g.GroupId
			WHERE su.EmpId = @EmpId
				AND su.ActionType = @Role
				AND ISNULL(su.IsDeleted, 0) = 0
			GROUP BY su.OwnerId
				,isnull(g.GroupID, - 1)
				,isnull(g.NAME, '')
				,ee.FullName
				,ee.NickName
				,su.ActionType
				,ee.Email
			) AS Su
			LEFT JOIN [dbo].[EmployeeSupervisor] ord WITH (NOLOCK) ON Su.OwnerId = ord.OwnerId
			AND Su.ActionType = ord.ActionType
			AND ord.EmpId = @EmpId
			AND ISNULL(ord.IsDeleted, 0) = 0
		GROUP BY su.OwnerId
			,su.FullName
			,su.NickName
			,su.ActionType
			,su.Email
			,--OrderNo,
			su.CheckInOuts
			,su.LateInEarlyOuts
			,su.ChangeShifts
			,su.Leaves
			,su.KeepInViews
			,su.Overtimes
			,su.TeamID
			,su.[Team]
		ORDER BY OrderNo
END
