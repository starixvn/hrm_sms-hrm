
CREATE PROCEDURE [dbo].[Manpower_Period_GetById] @PeriodId SMALLINT
AS
/*
Created: 2023-06-02@Brian.Nguyen
Task: get period by id
DB: bodb_IUC
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT PeriodId
		,[Name]
		,IsActive
	FROM dbo.Manpower_Period WITH (NOLOCK)
	WHERE PeriodId = @PeriodId

	SELECT PeriodId
		,ProcessId
		,FromDate
		,ToDate
	FROM dbo.Manpower_PeriodProcessSetting WITH (NOLOCK)
	WHERE PeriodId = @PeriodId
	ORDER BY PeriodId
		,ProcessId
END