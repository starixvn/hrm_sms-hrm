
CREATE PROCEDURE [dbo].[Employee_GetVerifierAndApprover] @EmpId INT
	,@RequestType TINYINT
AS
/*
Created: 20160609@Quinn.Vo
Task: Get list verifier and approver for employee when create/edit request.
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @Verifiers TABLE (OwnerId INT)
	DECLARE @Approvers TABLE (OwnerId INT)

	INSERT INTO @Verifiers (OwnerId)
	SELECT DISTINCT OwnerId
	FROM dbo.EmployeeSupervisor AS su WITH (NOLOCK)
	WHERE su.EmpId = @EmpId
		AND (
			@RequestType IS NULL
			OR su.RequestType = @RequestType
			)
		AND su.ActionType = 2
		AND su.IsDeleted = 0

	IF NOT EXISTS (
			SELECT 1
			FROM @Verifiers
			)
	BEGIN
		INSERT INTO @Verifiers (OwnerId)
		SELECT DISTINCT su.OwnerId
		FROM dbo.GroupSupervisor AS su WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON su.GroupId = e.GroupId
		WHERE e.EmpId = @EmpId
			AND (
				@RequestType IS NULL
				OR su.RequestType = @RequestType
				)
			AND su.ActionType = 2
			AND su.IsDeleted = 0
	END

	INSERT INTO @Approvers (OwnerId)
	SELECT DISTINCT OwnerId
	FROM dbo.EmployeeSupervisor AS su WITH (NOLOCK)
	WHERE su.EmpId = @EmpId
		AND (
			@RequestType IS NULL
			OR su.RequestType = @RequestType
			)
		AND su.ActionType = 3
		AND su.IsDeleted = 0

	IF NOT EXISTS (
			SELECT 1
			FROM @Approvers
			)
	BEGIN
		INSERT INTO @Approvers (OwnerId)
		SELECT DISTINCT su.OwnerId
		FROM dbo.GroupSupervisor AS su WITH (NOLOCK)
			INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON su.GroupId = e.GroupId
		WHERE e.EmpId = @EmpId
			AND (
				@RequestType IS NULL
				OR su.RequestType = @RequestType
				)
			AND su.ActionType = 3
			AND su.IsDeleted = 0
	END

	SELECT OwnerId
		,ee.FullName
		,ee.NickName
		,ee.Email
	FROM @Verifiers AS su
		INNER JOIN dbo.Employee AS ee WITH (NOLOCK) ON su.OwnerId = ee.EmpId

	SELECT OwnerId
		,ee.FullName
		,ee.NickName
		,ee.Email
	FROM @Approvers AS su
		INNER JOIN dbo.Employee AS ee WITH (NOLOCK) ON su.OwnerId = ee.EmpId
END
