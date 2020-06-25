
CREATE PROCEDURE [dbo].[TimeRecord_ImportFromBioStar]
	@fromDate DATETIME
	,@toDate DATETIME
AS
/*
Created: 20170914@QuinnVo
Task: Import data from BioStar to HR system
DB: HRM
Original: 

Revisions:    
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON

	DECLARE @SQLString NVARCHAR(4000)
		,@CurrentMonthTableName VARCHAR(50)
		,@LastMonthTableName VARCHAR(50)
		,@fromDateString VARCHAR(10)
		,@toDateString VARCHAR(10)
		,@StartDateConnectBitoStar VARCHAR(10)

	CREATE TABLE #tempFinger (
		nUserID VARCHAR(32)
		,dtDateTime DATETIME
		,nTNAEvent INT
		)

	CREATE TABLE #tempTimeRecord (
		EmpID INT
		,RecordTime DATETIME
		,InOutType INT
		)

	SELECT @CurrentMonthTableName = 'T_LG' + LEFT(CONVERT(VARCHAR, GETDATE(), 112), 6)
		,@LastMonthTableName = 'T_LG' + LEFT(CONVERT(VARCHAR, DATEADD(MM, - 1, GETDATE()), 112), 6)
		,@fromDateString = CONVERT(VARCHAR, @fromDate, 112)
		,@toDateString = CONVERT(VARCHAR, @toDate, 112)
		,@StartDateConnectBitoStar = '2017-09-15'

	IF EXISTS (
			SELECT 1
			FROM BIOSTAR.[biostar2_ac].[dbo].[T_LGTBIDX]
			WHERE [LGTABLENM] = @LastMonthTableName
			)
	BEGIN
		SET @SQLString = N'INSERT INTO #tempFinger(nUserID, dtDateTime, nTNAEvent) 
						SELECT 
						USRID AS nUserID
						  ,DATEADD(HH, 7, DATEADD(s, [DEVDT], ''1970-01-01'')) AS dtDateTime
						  ,[TNAKEY] AS nTNAEvent

					FROM BIOSTAR.[biostar2_ac].[dbo].' + @LastMonthTableName + ' WHERE USRID <> '''' AND EVT < 5400
					AND DATEADD(HH, 7, DATEADD(s, [DEVDT], ''1970-01-01'')) > ''' + @fromDateString + '''
					AND DATEADD(HH, 7, DATEADD(s, [DEVDT], ''1970-01-01'')) < DATEADD(d, 1, ''' + @toDateString + ''')
					AND DATEADD(HH, 7, DATEADD(s, [DEVDT], ''1970-01-01'')) > ''' + @StartDateConnectBitoStar + ''''

		EXECUTE sp_executesql @SQLString

	END

	IF EXISTS (
			SELECT 1
			FROM BIOSTAR.[biostar2_ac].[dbo].[T_LGTBIDX]
			WHERE [LGTABLENM] = @CurrentMonthTableName
			)
	BEGIN
		SET @SQLString = N'INSERT INTO #tempFinger(nUserID, dtDateTime, nTNAEvent) 
						SELECT 
					 USRID AS nUserID
						  ,DATEADD(HH, 7, DATEADD(s, [DEVDT], ''1970-01-01'')) AS dtDateTime
						  ,[TNAKEY] AS nTNAEvent
	  
					FROM BIOSTAR.[biostar2_ac].[dbo].' + @CurrentMonthTableName + ' WHERE USRID <> '''' AND EVT < 5400
					AND DATEADD(HH, 7, DATEADD(s, [DEVDT], ''1970-01-01'')) > ''' + @fromDateString + '''
					AND DATEADD(HH, 7, DATEADD(s, [DEVDT], ''1970-01-01'')) < DATEADD(d, 1, ''' + @toDateString + ''')
					AND DATEADD(HH, 7, DATEADD(s, [DEVDT], ''1970-01-01'')) > ''' + @StartDateConnectBitoStar + ''''

		EXECUTE sp_executesql @SQLString

	END
	
	INSERT #tempTimeRecord (
		EmpID
		,RecordTime
		,InOutType
		)
	SELECT e.EmpId
		,dtDateTime
		,CASE 
			WHEN nTNAEvent = 1 THEN 0
			WHEN nTNAEvent = 2 THEN 1
			ELSE NULL
			END
	FROM dbo.Employee e WITH (NOLOCK)
		INNER JOIN #tempFinger f ON ISNULL(TRY_PARSE(e.Code AS INT), 0) = ISNULL(TRY_PARSE(f.nUserID AS INT), -1)
	WHERE NOT EXISTS (
			SELECT 1
			FROM dbo.TimeRecord t WITH (NOLOCK)
			WHERE t.empID = e.EmpId
				AND DATEDIFF(SECOND, RecordTime, f.dtDateTime) = 0
			)

	INSERT TimeRecord (
		EmpID
		,RecordTime
		,InOutType
		,InsertDate
		)
	SELECT EmpID
		,RecordTime
		,InOutType
		,GETDATE()
	FROM #tempTimeRecord
	ORDER BY RecordTime

	INSERT INTO dbo.tmpEmployeeAttendace (
			[EmpId]
				,[WorkDate]
				,[InsertedDate]
		)

	SELECT EmpId
		,CAST(DATEADD(DD, -1, RecordTime) AS DATE)
		,GETDATE()
	FROM #tempTimeRecord t
	WHERE NOT EXISTS (
		SELECT 1 
		FROM dbo.tmpEmployeeAttendace WITH (NOLOCK) 
		WHERE EmpId = t.EmpID 
		AND WorkDate = CAST(DATEADD(DD, -1, t.RecordTime) AS DATE))
	UNION
	SELECT EmpId
		,CAST(RecordTime AS DATE)
		,GETDATE()
	FROM #tempTimeRecord t
	WHERE NOT EXISTS (
		SELECT 1 
		FROM dbo.tmpEmployeeAttendace WITH (NOLOCK) 
		WHERE EmpId = t.EmpID 
		AND WorkDate = CAST(t.RecordTime AS DATE))
	
	DROP TABLE #tempFinger
	DROP TABLE #tempTimeRecord
END