
CREATE PROCEDURE [dbo].[Manpower_Period_Get] @IsActive BIT = NULL
	,@PageIndex SMALLINT = 0
	,@PageSize SMALLINT = 0
	,@TotalRows INT = 0 OUTPUT
AS
/*
Created: 2023-06-02@Brian.Nguyen
Task: get manpower period
DB: bodb_IUC
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @RowStart INT
		,@RowEnd INT
		,@UnitHeadProcessId TINYINT = 1

	SELECT @PageIndex = @PageIndex - 1
		,@RowStart = @PageIndex * @PageSize + 1
		,@RowEnd = @RowStart + @PageSize - 1

	SELECT ROW_NUMBER() OVER (
			ORDER BY ps.FromDate DESC
			) AS RowNum
		,p.PeriodId
		,p.[Name]
		,p.IsActive
		,p.[Year]
	INTO #tmpPeriod
	FROM dbo.Manpower_Period AS p WITH (NOLOCK)
		INNER JOIN dbo.Manpower_PeriodProcessSetting AS ps WITH (NOLOCK) ON p.PeriodId = ps.PeriodId
	WHERE ps.ProcessId = 1
		AND (
			@IsActive IS NULL
			OR @IsActive = p.IsActive
			)

	SELECT @TotalRows = COUNT(1)
	FROM #tmpPeriod WITH (NOLOCK)

	IF @PageIndex >= 0
		AND @PageSize > 0
	BEGIN
		DELETE
		FROM #tmpPeriod WITH (ROWLOCK, UPDLOCK)
		WHERE RowNum < @RowStart
			OR RowNum > @RowEnd
	END

	SELECT p.PeriodId
		,p.[Name]
		,p.IsActive
		,p.[Year]
	FROM #tmpPeriod AS p WITH (NOLOCK)
		INNER JOIN dbo.Manpower_PeriodProcessSetting AS ps WITH (NOLOCK) ON p.PeriodId = ps.PeriodId
		AND ps.ProcessId = @UnitHeadProcessId
	ORDER BY ps.FromDate DESC

	SELECT PeriodId
		,ProcessId
		,FromDate
		,ToDate
	FROM dbo.Manpower_PeriodProcessSetting WITH (NOLOCK)
	ORDER BY PeriodId
		,ProcessId

	DROP TABLE #tmpPeriod
END