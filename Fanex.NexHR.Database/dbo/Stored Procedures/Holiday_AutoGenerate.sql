CREATE PROCEDURE [dbo].[Holiday_AutoGenerate] @Holidays NVARCHAR(MAX)
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20190705@Oliver.Dang
Task: Auto insert holidays at new year
DB: HRM
Original:

Revisions:
Param's Explanation:
@holidays: 
    N'<ROOT>
	   <Holiday HolidayName="New Year" FromDate="2019-01-01" ToDate="2019-01-01" />
    </ROOT>'
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	CREATE TABLE #tblHoliday (
		HolidayName NVARCHAR(50)
		,FromDate DATETIME
		,ToDate DATETIME
		)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@Holidays

		INSERT INTO #tblHoliday (
			HolidayName
			,FromDate
			,ToDate
			)
		SELECT HolidayName
			,FromDate
			,ToDate
		FROM OPENXML(@DocHandler, '/Root/Row', 1) WITH (
				HolidayName NVARCHAR(MAX)
				,FromDate DATETIME
				,ToDate DATETIME
				) AS data

		EXEC sp_xml_removedocument @DocHandler
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Paramter invalid.'

		RETURN
	END CATCH

	BEGIN TRY
		INSERT INTO dbo.Holiday (
			HolidayName
			,FromDate
			,ToDate
			)
		SELECT holiday.HolidayName
			,holiday.Fromdate
			,holiday.ToDate
		FROM #tblHoliday AS holiday WITH (NOLOCK)

		DROP TABLE #tblHoliday
	END TRY

	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Insert holiday failed'

		RETURN
	END CATCH
END