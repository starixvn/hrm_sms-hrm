
CREATE PROCEDURE [dbo].[LeaveRequest_WeekendLeave_InsertAll] 
	@EmpId INT = NULL
	,@Year INT = NULL
AS
/*
Created: 20190419@Quinn.Vo
Task: Insert all weekend leave in year.
DB: HRM
Original: 

Revisions:  

*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @Index INT
		,@Index2 INT
		,@TmpEmpId INT
		,@TmpRequestId BIGINT
		,@FromDate DATE

	IF (@Year IS NULL)
	BEGIN
		SET @Year = YEAR(GETDATE())
	END

	SET @FromDate = CONVERT(DATE, CONVERT(NVARCHAR(10), CAST(@Year AS VARCHAR(4))) + '/01/01')

	CREATE TABLE #Employee(
		Id INT PRIMARY KEY IDENTITY(1,1)
		,EmpId INT
	)

	CREATE TABLE #LeaveRequest(
		Id INT PRIMARY KEY IDENTITY(1,1)
		,RequestId INT
	)
	
	IF (@EmpId IS NULL)
	BEGIN
		INSERT #Employee(EmpId)
		SELECT EmpId 
		FROM dbo.Employee WITH (NOLOCK) 
		WHERE [Enabled] = 1
	END
	ELSE
	BEGIN
		INSERT INTO #Employee(EmpId) VALUES (@EmpId)
	END

	SELECT @Index = 1

	-- loop employee
	WHILE EXISTS (SELECT 1 FROM #Employee WHERE Id = @Index)
	BEGIN
		SELECT @TmpEmpId = EmpId FROM #Employee WHERE Id = @Index

		TRUNCATE TABLE #LeaveRequest
		INSERT INTO #LeaveRequest(RequestId)
		SELECT lr.RequestId FROM dbo.LeaveRequest AS lr WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest AS wkr WITH (NOLOCK) ON lr.RequestId = wkr.RequestId
		WHERE lr.EmpId = @TmpEmpId
			AND FromDate >= @FromDate
			AND FromDate < DATEADD(YY, 1, @FromDate)
			AND wkr.CurrentStatus IN (1, 2, 4, 6)

		SELECT @Index2 = 1
		-- loop request
		WHILE EXISTS (SELECT 1 FROM #LeaveRequest WHERE Id = @Index2)
		BEGIN
			SELECT @TmpRequestId = RequestId FROM #LeaveRequest WHERE Id = @Index2

			EXEC dbo.LeaveRequest_InsertWeekendLeaveTrans @RequestId = @TmpRequestId

			SET @Index2 = @Index2 + 1
		END

		SET @Index = @Index + 1
	END
END