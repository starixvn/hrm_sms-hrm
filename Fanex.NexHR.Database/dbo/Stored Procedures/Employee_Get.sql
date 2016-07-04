


CREATE PROCEDURE [dbo].[Employee_Get] --'2014-12-04'
	@FromDate DATETIME = NULL
AS
/*
Created: 20140320@Quinn.Vo
Task: Get all employee from change date
DB: NexEPA
Original: 

Revisions:  10:12:18
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	--IF(EXISTS(SELECT 1 FROM dbo.Employee WITH (NOLOCK) WHERE DATEDIFF(SECOND, @FromDate, ModifiedDate) = 0 HAVING COUNT(1) > 1))
	--BEGIN
	--	DECLARE @Employee TABLE (EmpId INT, ModifiedDate DATETIME)

	--	INSERT INTO @Employee (EmpId, ModifiedDate)
	--	SELECT e.EmpId, DATEADD(SECOND, - ROW_NUMBER() OVER (ORDER BY e.ModifiedDate) + 1, e.ModifiedDate)
	--	FROM dbo.Employee AS e WITH (ROWLOCK, UPDLOCK)
	--	WHERE DATEDIFF(SECOND, @FromDate, e.ModifiedDate) = 0
	--	ORDER BY ModifiedDate

	--	SELECT @FromDate = MIN (ModifiedDate) FROM @Employee

	--	UPDATE e 
	--	SET e.ModifiedDate = ee.ModifiedDate
	--	FROM dbo.Employee AS e WITH (ROWLOCK, UPDLOCK)
	--	INNER JOIN @Employee AS ee ON e.EmpId = ee.EmpId
	--	WHERE DATEDIFF(SECOND, @FromDate, e.ModifiedDate) = 0
	--END

	SELECT
	e.EmpId AS UserId,
	e.[Password],
	e.Code,
	e.NickName,
	e.FullName,
	e.EmployeeCard,
	e.DomainUsername AS DomainName,
	e.Phone,
	e.Email,
	e.JobTitleId,
	e.[Enabled] AS IsEnabled,
	ISNULL(e.isDeleted, 0) AS IsDeleted,
	e.JoinedDate,
	e.ConfirmedDate,
	es.IsLocal,
	e.GroupId AS TeamId,
	CASE WHEN e.ModifiedDate IS NULL THEN e.CreatedDate ELSE e.ModifiedDate END AS ModifiedDate
	FROM dbo.Employee AS e WITH (NOLOCK)
	INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
	WHERE @FromDate IS NULL
	OR e.ModifiedDate > @FromDate
	OR e.CreatedDate > @FromDate
	ORDER BY e.ModifiedDate
	
END