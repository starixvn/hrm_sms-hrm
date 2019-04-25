
CREATE PROCEDURE [dbo].[ScheduleArrangement_UpdateRemark] @ActorId INT
	,@EmployeeId INT
	,@Remark NVARCHAR(500) = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-04-12
Task: update remark
DB: HRM
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @ModifiedDate DATETIME = GETDATE()

	IF NOT EXISTS (
			SELECT 1
			FROM dbo.ScheduleArrangement WITH (NOLOCK)
			WHERE EmployeeId = @EmployeeId
			)
	BEGIN
		INSERT INTO dbo.ScheduleArrangement (
			EmployeeId
			,Remark
			,ModifiedBy
			,ModifiedDate
			)
		VALUES (
			@EmployeeId
			,@Remark
			,@ActorId
			,@ModifiedDate
			)
	END
	ELSE
	BEGIN
		UPDATE dbo.ScheduleArrangement WITH (ROWLOCK, UPDLOCK)
		SET Remark = @Remark
			,ModifiedBy = @ActorId
			,ModifiedDate = @ModifiedDate
		WHERE EmployeeId = @EmployeeId
	END
END