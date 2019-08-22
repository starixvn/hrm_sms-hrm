
CREATE PROCEDURE [dbo].[Overtime_GetMonthlyRequestByEmpId] @EmployeeId INT
	,@Year INT
AS
/*
Created: Tim.Trinh@2019-07-02
Task: get monthly overtime
DB: HRM_SMS
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @fromdate DATE
		,@todate DATE
		,@Month DATE
		,@Today DATE = GETDATE()

	IF YEAR(@Today) = @Year
	BEGIN
		SELECT @Month = CONVERT(VARCHAR(5), @Year) + '/' + CONVERT(VARCHAR(2), MONTH(@Today)) + + '/' + CONVERT(VARCHAR(2), DAY(@Today))
	END
	ELSE
	BEGIN
		SELECT @Month = CONVERT(VARCHAR(5), @Year) + '/12/01'
	END

	SELECT @Month = CASE 
			WHEN DAY(@Today) BETWEEN 10
					AND 31 THEN DATEADD(MONTH, 1, @Month)
			ELSE @Month
			END

	IF YEAR(@Month) > @Year
	BEGIN
		SELECT @Month = CONVERT(VARCHAR(5), @Year) + '/12/01'
	END

	SELECT @fromDate = CONVERT(VARCHAR(5), @Year) + '/01/01'
		,@toDate = @Month

	CREATE TABLE #tblMonth ([Month] DATE)

	WHILE @fromDate <= @toDate
	BEGIN
		INSERT INTO #tblMonth ([Month])
		VALUES (@fromDate)

		SET @fromDate = DATEADD(MONTH, 1, @fromDate)
	END

	SELECT @fromDate = CONVERT(VARCHAR(5), @Year) + '/01/01'

	SELECT request.RequestId
		,cl.[Month]
		,request.VerifiedDate
		,request.ApprovedDate
		,request.NormalHours
		,request.OffHours
		,request.PublicHours
		,request.SpecialHours
		,request.CurrentStatus AS StatusId
		,verifier.EmpId AS VerifierId
		,verifier.NickName
		,verifier.FullName
		,approver.EmpId AS ApproverId
		,approver.NickName
		,approver.FullName
	FROM #tblMonth AS cl
		LEFT JOIN dbo.OTMonthlyRequest AS request WITH (NOLOCK) ON request.[Month] = cl.[Month]
		AND request.[Month] BETWEEN @fromdate
			AND @todate
		AND request.EmpId = @EmployeeId
		LEFT JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = request.EmpId
		LEFT JOIN dbo.Employee AS verifier WITH (NOLOCK) ON verifier.empid = request.VerifierId
		LEFT JOIN dbo.Employee AS approver WITH (NOLOCK) ON approver.empid = request.ApproverId
	WHERE EXISTS (
			SELECT 1
			FROM dbo.EmployeeSummary WITH (NOLOCK)
			WHERE EmpId = @EmployeeId
				AND CanOT = 1
			)
	ORDER BY cl.[Month] DESC

	DROP TABLE #tblMonth
END
