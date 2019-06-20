
CREATE PROCEDURE [dbo].[EmployeeDiscipline_GetSummaryByPeriod] @LastModifiedDate DATETIME
AS
/*
Created: 20160914@Quinn.Vo
Task: Get all employee discipline summary.
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF(@LastModifiedDate IS NOT NULL)
	BEGIN
		SELECT @LastModifiedDate = DATEADD(HH, -1, @LastModifiedDate)
	END

	DECLARE @Date DATETIME
		,@PreviousFromDate DATETIME
		,@PreviousToDate DATETIME
		,@CurrentFromDate DATETIME
		,@CurrentToDate DATETIME
		,@MinWorkDate DATETIME

	SELECT @MinWorkDate = MIN(WorkDate)
	FROM dbo.[EmployeeDiscipline] WITH (NOLOCK)
	WHERE ModifiedDate > @LastModifiedDate

	SELECT @Date = GETDATE()

	IF (MONTH(@Date) < 7)
	BEGIN
		SELECT @PreviousFromDate = CONVERT(NVARCHAR(10), (YEAR(@Date) - 1)) + '/07/01'
			,@PreviousToDate = CONVERT(NVARCHAR(10), (YEAR(@Date) - 1)) + '/12/31'
			,@CurrentFromDate = CONVERT(NVARCHAR(10), YEAR(@Date)) + '/01/01'
			,@CurrentToDate = CONVERT(NVARCHAR(10), YEAR(@Date)) + '/06/30'
	END
	ELSE
	BEGIN
		SELECT @PreviousFromDate = CONVERT(NVARCHAR(10), YEAR(@Date)) + '/01/01'
			,@PreviousToDate = CONVERT(NVARCHAR(10), YEAR(@Date)) + '/06/30'
			,@CurrentFromDate = CONVERT(NVARCHAR(10), YEAR(@Date)) + '/07/01'
			,@CurrentToDate = CONVERT(NVARCHAR(10), YEAR(@Date)) + '/12/31'
	END

	SELECT *
	FROM [dbo].[fnc_GetDisciplineSummary](@LastModifiedDate, @PreviousFromDate, @PreviousToDate)
	WHERE @MinWorkDate <= @PreviousToDate
	UNION
	SELECT *
	FROM [dbo].[fnc_GetDisciplineSummary](@LastModifiedDate, @CurrentFromDate, @CurrentToDate)
END