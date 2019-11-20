
CREATE PROCEDURE [dbo].[Auto_EmployeeCalendar_UpdateIsPublic] @EmployeeId INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-06-26
Task: Update Is Public(from 60 days ago to current week)
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromdate DATETIME
		,@todate DATETIME

	SET @todate = DATEADD(wk, DATEDIFF(wk, 0, GETDATE()), 6)
	SET @fromdate = @todate - 60

	UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
	SET IsPublic = 1
	WHERE WorkDate BETWEEN @fromdate
			AND @todate
		AND ISNULL(IsPublic, 0) = 0
		AND (
			@EmployeeId IS NULL
			OR EmpId = @EmployeeId
			)
END
