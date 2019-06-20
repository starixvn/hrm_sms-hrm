 -- TimeRecord_ImportFromBioAdmin '2012/09/18', '2012/09/18'
--select *  from dbo.TimeRecord
CREATE PROCEDURE [dbo].[TimeRecord_ImportFromBioAdmin] --'2014-05-25', '2014-05-30'
		@fromDate datetime,
		@toDate datetime,
		@FromTime DATE OUTPUT, 
		@ToTime DATE OUTPUT
AS
/*
Created: 2012/03/13@Dan
Task: Import data from BioAdmin to HRSmart system
DB: NexHR
Original: 

Revisions:    
Param's Explanation:
• @importdate: the date to import data
*/

SET NOCOUNT ON

CREATE TABLE #tempTimeRecord (
		EmpID INT
		,RecordTime DATETIME
		,InOutType INT
		)

-- get data from BioAdmin
SELECT dtDateTime, nDeviceID, nEvent, nTNAEvent, nUserID 
INTO #tempFinger
FROM BIOADMIN.[BioStation].dbo.tb_reportslist
WHERE dtDateTime>= @fromDate
	and dtDatetime<= dateadd(d,1,@toDate) -- not use datediff because it does not use index
	and nUserID > 0
ORDER BY dtDateTime


--DECLARE @FromTime DATE, @ToTime DATE

INSERT #tempTimeRecord (
		EmpID
		,RecordTime
		,InOutType
		)
SELECT e.EmpId, dtDateTime, 
			CASE WHEN nTNAEvent=0 THEN 0
						WHEN nTNAEvent=1 THEN 1
						ELSE NULL END
FROM dbo.Employee e JOIN #tempFinger f ON e.Code is not null 
						AND ISNUMERIC(e.Code) = 1
						AND	CAST(e.Code as decimal(18, 0)) = f.nUserID
WHERE NOT EXISTS (SELECT 1 from dbo.TimeRecord t
		WHERE t.empID = e.EmpId 
		AND datediff (second, RecordTime, f.dtDateTime) = 0		
	)

SELECT @FromTime = MIN(RecordTime),
	@ToTime = MAX(RecordTime)
FROM #tempTimeRecord

IF(@FromTime IS NOT NULL)
BEGIN
	--DECLARE @RowCount int
	IF(DATEDIFF(DD, @FromTime, GETDATE()) = 0)
	BEGIN
		SET @FromTime = DATEADD(DD, -1, @FromTime)
	END
	-- import check-in/out data to TS_TimeRecord 
	INSERT TimeRecord(EmpID, RecordTime, InOutType, InsertDate)
	SELECT EmpID
		,RecordTime
		,InOutType
		,getdate() 
	FROM #tempTimeRecord
	ORDER BY RecordTime
	
	SELECT @FromTime = CAST(@FromTime AS DATE), @ToTime = CAST(@ToTime AS DATE)

	INSERT INTO dbo.tmpEmployeeAttendace (
			[EmpId]
			,[WorkDate]
			,[InsertedDate]
		)

	SELECT EmpId
		,CAST(DATEADD(DD, -1, RecordTime) AS DATE)
		,GETDATE()
	FROM #tempTimeRecord
	UNION
	SELECT EmpId
		,CAST(RecordTime AS DATE)
		,GETDATE()
	FROM #tempTimeRecord
	--EXEC dbo.EmployeeAttendance_ProcessingAndInsert null, @FromTime, @ToTime
END

DROP TABLE #tempFinger
DROP TABLE #tempTimeRecord