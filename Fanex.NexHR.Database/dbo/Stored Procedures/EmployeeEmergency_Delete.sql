
/****** Object:  Stored Procedure dbo.EmployeeRole_Insert    Script Date: 7/20/2016 10:19:06 AM ******/
/****** Object:  Stored Procedure dbo.EmployeeRole_Insert    Script Date: 7/19/2016 5:22:14 PM ******/
CREATE PROCEDURE [dbo].[EmployeeEmergency_Delete] @EmergencyId INT
	,@ActorId INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190613@Oliver.Dang
Task: Delete Emergency
DB: SPM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	BEGIN
		IF (
				NOT EXISTS (
					SELECT 1
					FROM dbo.EmployeeEmergency AS r WITH (NOLOCK)
					WHERE r.Id = @EmergencyId
					)
				)
		BEGIN
			SELECT @ErrCode = 1
				,@ErrMsg = N'The emergency does not existed.';

			RETURN
		END

		IF (
				NOT EXISTS (
					SELECT 1
					FROM dbo.EmployeeEmergency AS r WITH (NOLOCK)
					WHERE r.Id = @EmergencyId
						AND r.EmpId = @ActorId
					)
				)
		BEGIN
			SELECT @ErrCode = 1
				,@ErrMsg = N'Do not have permission.';

			RETURN
		END

		DELETE
		FROM [dbo].[EmployeeEmergency]
		WHERE Id = @EmergencyId

		IF @@ERROR <> 0
		BEGIN
			SELECT @ErrCode = 1
				,@ErrMsg = N'Delete emergency failed.';
		END
	END
END