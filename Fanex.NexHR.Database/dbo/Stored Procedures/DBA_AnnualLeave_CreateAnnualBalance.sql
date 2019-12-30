
CREATE PROCEDURE [dbo].[DBA_AnnualLeave_CreateAnnualBalance] @Year INT
	,@EmpId INT = NULL
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013.10.29
Task: create annual balance leave.
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @Description NVARCHAR(100) = 'Create annual balance leave in ' + CONVERT(NVARCHAR(10), @Year) + '.'
	DECLARE @DeductDays_local NUMERIC(18, 1) = 0
		,@CreditDays_local NUMERIC(18, 1) = 12
		,@CreditDays_DriverAndHouseGuard NUMERIC(18, 1) = 6
	DECLARE @DeductDays_expat NUMERIC(18, 1) = 0
		,@CreditDays_expat NUMERIC(18, 1) = 42
	DECLARE @CreatedDate DATETIME = GETDATE()
		,@FromDate DATE = CONVERT(NVARCHAR(10), @Year) + '/01/01'
		,@ToDate DATE

	SELECT @ToDate = DATEADD(DD, - 1, DATEADD(YY, 1, @FromDate))

	UPDATE es
	SET es.AnnualLeaveBalance = es.AnnualLeaveBalance + (CASE 
			WHEN es.IsLocal = 1 THEN (CASE 
					WHEN es.WorkingDays = 15 THEN @CreditDays_DriverAndHouseGuard
					ELSE @CreditDays_local END
					)
			ELSE @CreditDays_expat END
			)
	FROM dbo.EmployeeSummary AS es WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN (
		SELECT MAX(FromDate) AS FromDate
			,EmpId
		FROM dbo.LeaveTrans AS lt WITH (NOLOCK)
		WHERE lt.FromDate >= @FromDate
			AND lt.CreditDays > 0
			AND ISNULL(Deleted, 0) = 0
		GROUP BY EmpId
		) lt ON e.EmpId = lt.EmpId
	WHERE ISNULL(isdeleted, 0) = 0
		AND e.[Enabled] = 1
		AND (
			(
				es.CompanyDate IS NOT NULL
				AND es.CompanyDate <= @FromDate
				)
			OR es.IsLocal = 0
			)
		AND e.ResignedDate IS NULL
		AND (
			@EmpId IS NULL
			OR e.EmpId = @EmpId
			)
		AND lt.EmpId IS NULL

	INSERT INTO dbo.LeaveTrans (
		EmpId
		,[Description]
		,FromDate
		,ToDate
		,DeductDays
		,CreditDays
		,CreatedDate
		,CreatedBy
		,Deleted
		)
	SELECT e.EmpId
		,@Description
		,@FromDate
		,@ToDate
		,0 AS DeductDays
		,(CASE 
			WHEN es.IsLocal = 1 THEN (CASE 
					WHEN es.WorkingDays = 15 THEN @CreditDays_DriverAndHouseGuard
					ELSE @CreditDays_local END
					)
			ELSE @CreditDays_expat END
			) AS CreditDays
		,@CreatedDate
		,NULL AS CreatedBy
		,0 AS Deleted
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		LEFT JOIN (
		SELECT MAX(FromDate) AS FromDate
			,EmpId
		FROM dbo.LeaveTrans AS lt WITH (NOLOCK)
		WHERE lt.FromDate >= @FromDate
			AND lt.CreditDays > 0
			AND ISNULL(Deleted, 0) = 0
		GROUP BY EmpId
		) lt ON e.EmpId = lt.EmpId
	WHERE ISNULL(isdeleted, 0) = 0
		AND e.[Enabled] = 1
		AND (
			(
				es.CompanyDate IS NOT NULL
				AND es.CompanyDate <= @FromDate
				)
			OR es.IsLocal = 0
			)
		AND e.ResignedDate IS NULL
		AND (
			@EmpId IS NULL
			OR e.EmpId = @EmpId
			)
		AND lt.EmpId IS NULL
END
