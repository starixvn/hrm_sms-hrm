
CREATE FUNCTION [dbo].[fncCheckPermissionIgnoreDisciplineInReport] (
	@OwnerId INT
	,@GroupId INT
	,@EmpId INT
	,@ActionType INT
	,@RequestType INT = NULL
	)
RETURNS BIT
AS
/*
Created: 20160615@Tim.Trinh
Task: Get permission ignore request.
DB: SMSHRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	DECLARE @HasIgnore BIT

	SET @HasIgnore = 0

	IF (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
				WHERE empS.OwnerId = @OwnerId
					AND empS.EmpId = @EmpId
					AND empS.ActionType = 8 -- edit discipline
					AND empS.EmpId <> @OwnerId
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
				WHERE grS.GroupId = @GroupId
					AND grS.OwnerId = @OwnerId
					AND grS.ActionType = 8 -- edit discipline
					AND @EmpId <> @OwnerId
					AND NOT EXISTS (
						SELECT 1
						FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
						WHERE empS.OwnerId <> @OwnerId
							AND empS.EmpId = @EmpId
							AND empS.ActionType = 8
						)
				)
			)
		AND (
			EXISTS (
				SELECT 1
				FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
				WHERE empS.OwnerId = @OwnerId
					AND empS.EmpId = @EmpId
					AND empS.ActionType = @ActionType
					AND (
						ISNULL(@RequestType, '') = ''
						OR empS.RequestType = @RequestType
						)
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
				WHERE grS.GroupId = @GroupId
					AND grS.OwnerId = @OwnerId
					AND grS.ActionType = @ActionType
					AND (
						ISNULL(@RequestType, '') = ''
						OR grS.RequestType = @RequestType
						)
					AND @EmpId <> @OwnerId
					AND NOT EXISTS (
						SELECT 1
						FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
						WHERE empS.OwnerId <> @OwnerId
							AND empS.EmpId = @EmpId
							AND empS.ActionType = @ActionType
							AND (
								ISNULL(@RequestType, '') = ''
								OR empS.RequestType = @RequestType
								)
						)
				)
			)
	BEGIN
		SET @HasIgnore = 1
	END

	RETURN @HasIgnore
END
