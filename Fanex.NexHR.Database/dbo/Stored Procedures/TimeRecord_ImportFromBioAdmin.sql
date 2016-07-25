---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[TimeRecord_ImportFromBioAdmin] -- '2015-11-30', '2015-11-30'
		@fromDate datetime,
		@toDate datetime
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

-- get data from BioAdmin
SELECT dtDateTime, nDeviceID, nEvent, nTNAEvent, nUserID 
INTO #tempFinger
FROM BIOADMIN.[BioStation].dbo.tb_reportslist 
WHERE dtDateTime>= @fromDate
	and dtDatetime<= dateadd(d,1,@toDate) -- not use datediff because it does not use index
	and nUserID > 0
ORDER BY dtDateTime

--DECLARE @RowCount int

-- import check-in/out data to TS_TimeRecord 
INSERT TimeRecord(EmpID, RecordTime, InOutType, InsertDate)
SELECT e.EmpId, dtDateTime, 
		CASE WHEN nTNAEvent=0 THEN 0
					WHEN nTNAEvent=1 THEN 1
					ELSE NULL END InOutType,
		getdate() InsertDate
FROM dbo.Employee e WITH(NOLOCK) JOIN #tempFinger f ON e.Code is not null 
						AND	CAST(e.Code as decimal(18, 0)) = f.nUserID
						-- CAST(RIGHT(code, 4) as decimal(18, 0)) = f.nUserID
WHERE NOT EXISTS (select TOP 1 1 from dbo.TimeRecord t
		WHERE t.empID = e.EmpId 
		AND datediff (second, RecordTime, f.dtDateTime) = 0		
	)
ORDER BY dtDateTime			

DROP TABLE #tempFinger

--While @iDay >0 
--BEGIN
--	SELECT @iDay,@fromDate,@toDate

--	EXEC EmployeeAttendance_ProcessingAndInsert null, @fromDate, @toDate
	
--	Set @fromDate=DATEADD(DD,1,@fromDate)	

--	Set @iDay=@iDay-1
--END

EXEC EmployeeAttendance_ProcessingAndInsert null, @fromDate, @toDate