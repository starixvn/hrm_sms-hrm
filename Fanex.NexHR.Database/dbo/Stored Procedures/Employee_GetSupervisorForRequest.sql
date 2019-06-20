/****** Object:  Stored Procedure dbo.Employee_GetSupervisorForRequest    Script Date: 7/20/2016 10:19:03 AM ******/
/****** Object:  Stored Procedure dbo.Employee_GetSupervisorForRequest    Script Date: 7/19/2016 5:22:12 PM ******/

CREATE PROCEDURE [dbo].[Employee_GetSupervisorForRequest] @EmpId INT
	,@Role TINYINT
	,@RequestType TINYINT
AS
/*
Created: 20140724@Quinn.Vo
Task: Get list supervisor for employee when create/edit request.
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @Supervisors TABLE (
		OwnerId INT
		)

	INSERT INTO @Supervisors (OwnerId)
	SELECT DISTINCT OwnerId
	FROM [dbo].[EmployeeSupervisor] AS su WITH (NOLOCK)
	WHERE su.EmpId = @EmpId
		AND (
			@RequestType IS NULL
			OR su.RequestType = @RequestType
			)
		AND (
			@Role IS NULL
			OR su.ActionType = @Role
			)
		AND ISNULL(su.IsDeleted, 0) = 0

	IF (
			NOT EXISTS (
				SELECT 1
				FROM @Supervisors
				)
			)
	BEGIN
		INSERT INTO @Supervisors (OwnerId)
		SELECT DISTINCT su.OwnerId
		FROM dbo.GroupSupervisor AS su WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON su.GroupId = e.GroupId
		WHERE e.EmpId = @EmpId
			AND (
				@RequestType IS NULL
				OR su.RequestType = @RequestType
				)
			AND (
				@Role IS NULL
				OR su.ActionType = @Role
				)
			AND ISNULL(su.IsDeleted, 0) = 0
	END

	SELECT OwnerId
		,ee.FullName
		,ee.NickName
		,ee.Email
	FROM @Supervisors AS su
		INNER JOIN [dbo].Employee AS ee WITH (NOLOCK) ON su.OwnerId = ee.EmpId
END
