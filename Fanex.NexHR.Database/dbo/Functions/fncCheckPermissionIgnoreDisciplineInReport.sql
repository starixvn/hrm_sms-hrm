CREATE FUNCTION [dbo].[fncCheckPermissionIgnoreDisciplineInReport] (
	@OwnerId INT
	,@TeamId INT
	,@EmpId INT
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
					AND empS.IsDeleted = 0
				)
			OR EXISTS (
				SELECT 1
				FROM dbo.GroupSupervisor AS grS WITH (NOLOCK)
				WHERE grS.GroupId = @TeamId
					AND grS.OwnerId = @OwnerId
					AND grS.ActionType = 8 -- edit discipline
					AND grS.IsDeleted = 0
					AND NOT EXISTS (
						SELECT 1
						FROM dbo.EmployeeSupervisor AS empS WITH (NOLOCK)
						WHERE empS.OwnerId <> @OwnerId
							AND empS.EmpId = @EmpId
							AND empS.ActionType = 8
							AND empS.IsDeleted = 0
						)
				)
			)
	BEGIN
		SET @HasIgnore = 1
	END

	RETURN @HasIgnore
END