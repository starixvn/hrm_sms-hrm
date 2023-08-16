
CREATE PROCEDURE [dbo].[Manpower_AnnualRequestForm_GetByPeriodId] @PeriodId INT
AS
/*  
Created: 20230705@Brian.Nguyen
Task: get annual manpower forecast request by period
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @VerifyProcessId TINYINT = 2
		,@ApproveProcessId TINYINT = 3
		,@PendingStatusId TINYINT = 1
		,@VerifiedStatusId TINYINT = 2
		,@ApprovedStatusId TINYINT = 3

	CREATE TABLE #tmpUnit (
		UnitName NVARCHAR(50)
		,VerifierId INT
		,ApproverId INT
		)

	INSERT INTO #tmpUnit (UnitName)
	SELECT DISTINCT unit.[Name]
	FROM dbo.Manpower_Flow AS flow WITH (NOLOCK)
		INNER JOIN dbo.HRM_Unit AS unit WITH (NOLOCK) ON flow.UnitName = unit.[Name]
		AND unit.IsActive = 1
		AND unit.IsDeleted = 0
	WHERE flow.ProcessId IN (@VerifyProcessId, @ApproveProcessId)
		AND EXISTS (
			SELECT 1
			FROM dbo.HRM_Employee AS emp WITH (NOLOCK)
			WHERE emp.SiteId = flow.SiteId
				AND emp.EmpId = flow.EmpId
			)

	UPDATE tmp
	SET tmp.VerifierId = emp.UserId
	FROM #tmpUnit AS tmp WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Manpower_Flow AS flow WITH (NOLOCK) ON flow.UnitName = tmp.UnitName
		INNER JOIN dbo.HRM_Employee AS emp WITH (NOLOCK) ON emp.SiteId = flow.SiteId
		AND emp.EmpId = flow.EmpId
	WHERE flow.ProcessId = @VerifyProcessId

	UPDATE tmp
	SET tmp.ApproverId = emp.UserId
	FROM #tmpUnit AS tmp WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Manpower_Flow AS flow WITH (NOLOCK) ON flow.UnitName = tmp.UnitName
		INNER JOIN dbo.HRM_Employee AS emp WITH (NOLOCK) ON emp.SiteId = flow.SiteId
		AND emp.EmpId = flow.EmpId
	WHERE flow.ProcessId = @ApproveProcessId

	SELECT rq.RequestId
		,rq.UnitName
		,rq.StatusId
		,creator.UserId AS CreatorId
		,creator.Code
		,creator.Nickname
		,creator.FullName
		,verifier.UserId AS VerifierId
		,verifier.Code
		,verifier.Nickname
		,verifier.FullName
		,approver.UserId AS ApproverId
		,approver.Code
		,approver.Nickname
		,approver.FullName
	FROM dbo.Manpower_AnnualRequestForm AS rq WITH (NOLOCK)
		INNER JOIN dbo.Manpower_Period AS p WITH (NOLOCK) ON rq.PeriodId = p.PeriodId
		AND p.IsActive = 1
		INNER JOIN #tmpUnit AS unit WITH (NOLOCK) ON rq.UnitName = unit.UnitName
		INNER JOIN dbo.[User] AS creator WITH (NOLOCK) ON rq.CreatedBy = creator.UserId
		LEFT JOIN dbo.[User] AS verifier WITH (NOLOCK) ON (
			rq.StatusId IN (@VerifiedStatusId, @ApprovedStatusId)
			AND rq.VerifiedBy = verifier.UserId
			)
		OR (
			rq.StatusId = @PendingStatusId
			AND unit.VerifierId = verifier.UserId
			)
		LEFT JOIN dbo.[User] AS approver WITH (NOLOCK) ON (
			rq.StatusId = @ApprovedStatusId
			AND rq.ApprovedBy = approver.UserId
			)
		OR (
			rq.StatusId IN (@PendingStatusId, @VerifiedStatusId)
			AND unit.ApproverId = approver.UserId
			)
	WHERE p.PeriodId = @PeriodId

	DROP TABLE #tmpUnit
END