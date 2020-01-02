
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
	DECLARE @CreatedDate DATETIME = GETDATE()
		,@FromDate DATE = CONVERT(NVARCHAR(10), @Year) + '/01/01'
		,@ToDate DATE

	SELECT @ToDate = DATEADD(DD, - 1, DATEADD(YY, 1, @FromDate))

	UPDATE es
	SET es.AnnualLeaveBalance = es.AnnualLeaveBalance + ISNULL(gr.BasicAnnualLeaveBalance, 0)
	FROM dbo.EmployeeSummary AS es WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON e.EmpId = es.EmpId
		AND ISNULL(e.isdeleted, 0) = 0
		AND e.[Enabled] = 1
		AND e.ResignedDate IS NULL
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON e.GroupId = gr.GroupId
	WHERE (
			(
				es.CompanyDate IS NOT NULL
				AND es.CompanyDate <= @FromDate
				)
			OR es.IsLocal = 0
			)
		AND (
			@EmpId IS NULL
			OR e.EmpId = @EmpId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.LeaveTrans AS lt WITH (NOLOCK)
			WHERE e.EmpId = lt.EmpId
				AND lt.FromDate >= @FromDate
				AND lt.CreditDays > 0
				AND ISNULL(Deleted, 0) = 0
			)

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
		,ISNULL(gr.BasicAnnualLeaveBalance, 0)
		,@CreatedDate
		,NULL AS CreatedBy
		,0 AS Deleted
	FROM dbo.Employee AS e WITH (NOLOCK)
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON e.GroupId = gr.GroupId
		INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
		AND (
			(
				es.CompanyDate IS NOT NULL
				AND es.CompanyDate <= @FromDate
				)
			OR es.IsLocal = 0
			)
	WHERE ISNULL(e.isdeleted, 0) = 0
		AND e.[Enabled] = 1
		AND e.ResignedDate IS NULL
		AND (
			@EmpId IS NULL
			OR e.EmpId = @EmpId
			)
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.LeaveTrans AS lt WITH (NOLOCK)
			WHERE e.EmpId = lt.EmpId
				AND lt.FromDate >= @FromDate
				AND lt.CreditDays > 0
				AND ISNULL(Deleted, 0) = 0
			)
END
