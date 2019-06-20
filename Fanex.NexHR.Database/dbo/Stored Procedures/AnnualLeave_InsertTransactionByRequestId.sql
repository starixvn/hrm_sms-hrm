/****** Object:  Stored Procedure dbo.AnnualLeave_InsertTransactionByRequestId    Script Date: 7/20/2016 10:18:59 AM ******/
/****** Object:  Stored Procedure dbo.AnnualLeave_InsertTransactionByRequestId    Script Date: 7/19/2016 5:22:10 PM ******/

CREATE PROCEDURE [dbo].[AnnualLeave_InsertTransactionByRequestId] @RequestId INT
	,@EmpId INT
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@CreatedBy INT
AS
/*
Created: Quinn.Vo@20150927
Task: insert leave transaction and calculate annual leave balance for employee
DB: HRM
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	DECLARE @CreatedDate DATETIME
		,@tmpDate DATETIME
		,@tmpStartDate DATETIME
		,@tmpEndDate DATETIME
		,@LeaveDay NUMERIC(5, 1)

	SET @CreatedDate = GETDATE()
	SET @tmpDate = @FromDate

	WHILE (DATEDIFF(MM, @tmpDate, @ToDate) >= 0)
	BEGIN
		SELECT @tmpStartDate = CASE 
				WHEN DATEDIFF(MM, @tmpDate, @FromDate) = 0 THEN @FromDate
				ELSE DATEADD(MM, DATEDIFF(MM, 0, @tmpDate), 0)
				END
			,@tmpEndDate = CASE 
				WHEN DATEDIFF(MM, @tmpDate, @ToDate) = 0 THEN @ToDate
				ELSE EOMONTH(@tmpDate)
				END
			,@LeaveDay = 0

		SELECT @LeaveDay = TotalDays
		FROM dbo.tmpLeaveBalance WITH (NOLOCK)
		WHERE RequestID = @RequestID
			AND YEAR(@tmpDate) = InYear
			AND MONTH(@tmpDate) = InMonth

		IF (ISNULL(@LeaveDay, 0) > 0)
		BEGIN
			EXEC AnnualLeave_InsertTransaction @EmpId
				,@RequestID
				,'Leave Request'
				,@tmpStartDate
				,@tmpEndDate
				,@LeaveDay
				,NULL
				,@CreatedDate
				,@CreatedBy
		END

		SET @tmpDate = DATEADD(MM, 1, @tmpDate)
	END
END