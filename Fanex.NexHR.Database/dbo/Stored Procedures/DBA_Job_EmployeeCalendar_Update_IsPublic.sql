CREATE PROCEDURE [dbo].[DBA_Job_EmployeeCalendar_Update_IsPublic] @empid INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2014-03-15
Task: Update Is Public on Calendare (Hide only Public Calendare on next week.)
DB: HRM_SMS
Original: 

Revisions:    
	
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON

	DECLARE @fromdate DATETIME
		,@todate DATETIME

	SELECT @todate = DATEADD(wk, DATEDIFF(wk, 0, GETDATE()), 6)

	SELECT @fromdate = @todate - 60

	IF @empid IS NOT NULL
		UPDATE EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
		SET IsPublic = 1
		WHERE WorkDate BETWEEN @fromdate
				AND @todate
			AND EmpId = @empid
			AND ISNULL(IsPublic, 0) = 0
	ELSE
		UPDATE EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
		SET IsPublic = 1
		WHERE WorkDate BETWEEN @fromdate
				AND @todate
			AND ISNULL(IsPublic, 0) = 0
END