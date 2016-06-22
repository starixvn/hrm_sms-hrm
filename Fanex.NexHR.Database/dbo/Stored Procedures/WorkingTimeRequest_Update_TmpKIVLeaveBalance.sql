
CREATE PROCEDURE [dbo].[WorkingTimeRequest_Update_TmpKIVLeaveBalance] 
	@RequestId BIGINT
	,@EmpID INT
	,@Status INT = 1
AS
/*
Created: Quinn.Vo@20150927
Task: Update KIV temp.
DB: HRM
Original: 

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @KIVLeave NUMERIC(18, 2)

	IF (@Status = 2)
	BEGIN
		SELECT @KIVLeave = SUM(TotalDays)
		FROM dbo.tmpKIVLeaveBalance WITH (NOLOCK)
		WHERE Requestid = @Requestid
			AND [Status] <> 2

		IF ISNULL(@KIVLeave, 0) > 0
		BEGIN
			UPDATE EmployeeSummary WITH (ROWLOCK, UPDLOCK)
			SET KeepInViewBalance = ISNULL(KeepInViewBalance, 0) - ISNULL(@KIVLeave, 0)
			WHERE EmpId = @EmpId
		END
	END

	UPDATE dbo.[tmpKIVLeaveBalance] WITH (ROWLOCK, UPDLOCK)
	SET [Status] = @Status
	WHERE Requestid = @Requestid
END
