
CREATE PROCEDURE [dbo].[Auto_AnnualLeaveBalance_Update] @Year INT
	,@EmpId INT = NULL
	,@IsDelete BIT = 0
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tim.Trinh@2019-12-30
Task: Update annual leave balance
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

	SELECT @ToDate = DATEADD(DAY, - 1, DATEADD(YEAR, 1, @FromDate))

	CREATE TABLE #tblEmployeeBalance (
		EmpId INT
		,Amount DECIMAL(5, 2)
		)

	CREATE TABLE #tblLengthOfServiceBalance (
		EmpId INT
		,JoinedDate DATE
		,WorkingDays INT
		,LengthOfService INT
		,Amount DECIMAL(5, 2)
		)

	INSERT INTO #tblEmployeeBalance (
		EmpId
		,Amount
		)
	SELECT emp.EmpId
		,CASE 
			WHEN empsum.IsLocal = 1 THEN ISNULL(gr.LocalBasicAnnualLeaveBalance, 0)
			ELSE ISNULL(gr.ExpatBasicAnnualLeaveBalance, 0)
			END
	FROM dbo.EmployeeSummary AS empsum WITH (ROWLOCK, UPDLOCK)
		INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
		AND ISNULL(emp.isDeleted, 0) = 0
		AND emp.[Enabled] = 1
		AND emp.ResignedDate IS NULL
		INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON emp.GroupId = gr.GroupId
	WHERE (
			(
				emp.JoinedDate IS NOT NULL
				AND emp.JoinedDate <= @FromDate
				)
			OR empsum.IsLocal = 0
			)
		AND (
			@EmpId IS NULL
			OR emp.EmpId = @EmpId
			)

	IF EXISTS (
			SELECT 1
			FROM dbo.RefResource WITH (NOLOCK)
			WHERE RefName = 'EnableAdditionalAnnualLeaveBalance'
				AND IsActive = 1
			)
	BEGIN
		INSERT INTO #tblLengthOfServiceBalance (
			EmpId
			,JoinedDate
			,WorkingDays
			,LengthOfService
			)
		SELECT emp.EmpId
			,emp.JoinedDate
			,empsum.WorkingDays
			,FLOOR((
					DATEDIFF(MONTH, emp.JoinedDate, @FromDate) - 1 - CASE 
					WHEN empsum.MilitaryFromDate IS NOT NULL
						AND empsum.MilitaryToDate IS NOT NULL THEN DATEDIFF(MONTH, empsum.MilitaryFromDate, empsum.MilitaryToDate)
					ELSE 0 END
					) / 12.0)
		FROM dbo.EmployeeSummary AS empsum WITH (NOLOCK)
			INNER JOIN dbo.Employee AS emp WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
			AND ISNULL(emp.isDeleted, 0) = 0
			AND emp.[Enabled] = 1
			AND emp.ResignedDate IS NULL
		WHERE emp.JoinedDate IS NOT NULL
			AND emp.JoinedDate <= @FromDate
			AND (
				@EmpId IS NULL
				OR emp.EmpId = @EmpId
				)
			AND empsum.IsLocal = 1

		UPDATE lsb
		SET lsb.Amount = CASE 
				WHEN lsb.WorkingDays = 15 THEN (
						(lsb.LengthOfService / 5) + CASE 
						WHEN lsb.LengthOfService % 5 = 4
							AND MONTH(lsb.JoinedDate) < 7 THEN 1
						ELSE 0 END
						) / 2.0
				ELSE (lsb.LengthOfService / 5) + CASE 
						WHEN lsb.LengthOfService % 5 = 4
							AND MONTH(lsb.JoinedDate) < 7 THEN 1
						ELSE 0
						END
				END
		FROM #tblLengthOfServiceBalance AS lsb WITH (ROWLOCK, UPDLOCK)
	END

	MERGE #tblEmployeeBalance AS [target]
	USING #tblLengthOfServiceBalance AS [source] ON ([target].EmpId = [source].EmpId)
	WHEN MATCHED THEN
			UPDATE
			SET [target].Amount = [target].Amount + [source].Amount
	WHEN NOT MATCHED BY TARGET THEN
			INSERT (
				EmpId
				,Amount
				)
			VALUES (
				[source].EmpId
				,[source].Amount
				);

	DELETE
	FROM #tblEmployeeBalance
	WHERE Amount = 0

	IF @IsDelete = 1
	BEGIN
		UPDATE empsum
		SET AnnualLeaveBalance = empsum.AnnualLeaveBalance - lt.CreditDays
		FROM dbo.EmployeeSummary AS empsum WITH (ROWLOCK, UPDLOCK)
			INNER JOIN dbo.LeaveTrans AS lt WITH (NOLOCK) ON empsum.EmpId = lt.EmpId
			AND lt.FromDate = @FromDate
			AND ISNULL(lt.Deleted, 0) = 0
			AND lt.RequestId IS NULL
		WHERE EXISTS (
				SELECT 1
				FROM #tblEmployeeBalance AS tbl WITH (NOLOCK)
				WHERE empsum.EmpId = tbl.EmpId
				)

		DELETE lt
		FROM dbo.LeaveTrans AS lt
		WHERE lt.FromDate = @FromDate
			AND lt.CreditDays > 0
			AND lt.RequestId IS NULL
			AND EXISTS (
				SELECT 1
				FROM #tblEmployeeBalance AS tbl WITH (NOLOCK)
				WHERE lt.EmpId = tbl.EmpId
				)
	END

	UPDATE empsum
	SET empsum.AnnualLeaveBalance = empsum.AnnualLeaveBalance + emp.Amount
	FROM dbo.EmployeeSummary AS empsum WITH (ROWLOCK, UPDLOCK)
		INNER JOIN #tblEmployeeBalance AS emp WITH (NOLOCK) ON emp.EmpId = empsum.EmpId
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.LeaveTrans AS ltrans WITH (NOLOCK)
			WHERE emp.EmpId = ltrans.EmpId
				AND YEAR(ltrans.FromDate) = @Year
				AND ltrans.DeductDays = 0
				AND ltrans.CreditDays > 0
				AND ltrans.RequestId IS NULL
				AND ltrans.Deleted = 0
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
	SELECT EmpId
		,@Description
		,@FromDate
		,@ToDate
		,0 AS DeductDays
		,Amount AS CreditDays
		,@CreatedDate
		,NULL AS CreatedBy
		,0 AS Deleted
	FROM #tblEmployeeBalance AS tbl WITH (NOLOCK)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.LeaveTrans AS ltrans WITH (NOLOCK)
			WHERE tbl.EmpId = ltrans.EmpId
				AND YEAR(ltrans.FromDate) = @Year
				AND ltrans.DeductDays = 0
				AND ltrans.CreditDays > 0
				AND ltrans.RequestId IS NULL
				AND ltrans.Deleted = 0
			)

	DROP TABLE #tblEmployeeBalance

	DROP TABLE #tblLengthOfServiceBalance
END
