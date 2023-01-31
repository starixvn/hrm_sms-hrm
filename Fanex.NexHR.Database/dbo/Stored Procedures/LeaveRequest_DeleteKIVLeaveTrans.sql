
CREATE PROCEDURE [dbo].[LeaveRequest_DeleteKIVLeaveTrans] @RequestId BIGINT
AS
/*
Created: Quinn.Vo@20190613
Task: delete kiv temp leave balance.
DB: HRM
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF EXISTS (
			SELECT 1
			FROM dbo.KeepInViewByDayTrans WITH (NOLOCK)
			WHERE Requestid = @RequestId
			)
	BEGIN
		DELETE
		FROM dbo.KeepInViewByDayTrans WITH (ROWLOCK, UPDLOCK)
		WHERE Requestid = @RequestId
	END

	IF EXISTS (
			SELECT 1
			FROM dbo.KeepInViewByHourTrans WITH (NOLOCK)
			WHERE Requestid = @RequestId
			)
	BEGIN
		DECLARE @deductMinutes INT
			,@empId INT
			,@onDate DATE
			,@RequestTransId BIGINT

		SELECT @empId = EmpId
			,@onDate = WorkDate
			,@deductMinutes = DeductMinutes
		FROM dbo.KeepInViewByHourTrans WITH (NOLOCK)
		WHERE RequestId = @RequestId

		WHILE @deductMinutes > 0
		BEGIN
			SELECT TOP 1 @deductMinutes = @deductMinutes - CreditMinutes
				,@RequestTransId = RequestId
			FROM dbo.KeepInViewByHourTrans WITH (NOLOCK)
			WHERE EmpId = @empId
				AND WorkDate > DATEADD(DAY, - 1, DATEADD(MONTH, - 1, @onDate))
				AND CreditMinutes > 0
				AND CreditMinutes > MinutesBalance
			ORDER BY WorkDate ASC

			IF @deductMinutes >= 0
			BEGIN
				UPDATE dbo.KeepInViewByHourTrans WITH (ROWLOCK, UPDLOCK)
				SET MinutesBalance = CreditMinutes
				WHERE RequestId = @RequestTransId
			END
			ELSE
			BEGIN
				UPDATE dbo.KeepInViewByHourTrans WITH (ROWLOCK, UPDLOCK)
				SET MinutesBalance = MinutesBalance + CreditMinutes + @deductMinutes
				WHERE RequestId = @RequestTransId
			END
		END

		DELETE
		FROM dbo.KeepInViewByHourTrans WITH (ROWLOCK, UPDLOCK)
		WHERE RequestId = @RequestId
	END
END
