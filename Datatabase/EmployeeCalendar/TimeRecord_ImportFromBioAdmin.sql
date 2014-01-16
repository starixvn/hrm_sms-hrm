USE [NexHRM]
GO
/****** Object:  StoredProcedure [dbo].[TimeRecord_ImportFromBioAdmin]    Script Date: 9/24/2012 8:56:54 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
 -- TimeRecord_ImportFromBioAdmin '2012/09/18', '2012/09/18'
--select *  from dbo.TimeRecord
ALTER PROCEDURE [dbo].[TimeRecord_ImportFromBioAdmin]
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
FROM [10.19.10.243].[bodb_finger].dbo.tb_reportslist 
WHERE dtDateTime>= @fromDate
	and dtDatetime<= dateadd(d,1,@toDate) -- not use datediff because it does not use index
	and nUserID > 0
ORDER BY dtDateTime

--DECLARE @RowCount int

-- import check-in/out data to TS_TimeRecord 
INSERT TimeRecord(EmpID, RecordTime, InOutType, InsertDate)
SELECT e.ID, dtDateTime, 
		CASE WHEN nTNAEvent=0 THEN 0
					WHEN nTNAEvent=1 THEN 1
					ELSE NULL END,
		getdate() 
FROM dbo.Employee e JOIN #tempFinger f ON e.Code is not null 
						AND	CAST(e.Code as decimal(18, 0)) = f.nUserID
WHERE NOT EXISTS (select * from dbo.TimeRecord t
		WHERE t.empID = e.ID 
		AND datediff (second, RecordTime, f.dtDateTime) = 0		
	)
ORDER BY dtDateTime			

EXEC EmployeeAttendance_ProcessingAndInsert null, @fromDate , @toDate

DROP TABLE #tempFinger
