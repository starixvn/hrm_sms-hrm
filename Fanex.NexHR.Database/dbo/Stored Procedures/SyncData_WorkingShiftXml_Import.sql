Create PROCEDURE [dbo].[SyncData_WorkingShiftXml_Import]
	@WorkingShiftXml XML
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20200318@Lilac.Nguyen
Task: Insert Shift, update EmployeeCalendar by XML
DB:
Original:

Revisions:
Param's Explanation:
- @WorkingShiftXml: "<WorkingShift EmployeeID={1} ShiftCode={2} ShiftDate={3} CheckInTime={4} CheckOutTime={5} ModifiedDate={6} IsInAfterDay={6} IsOutAfterDay={6} IsCrossDay={6} TotalHours={6}> </WorkingShift>"
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME = GETDATE()

	CREATE TABLE #tmpWorkingShift (
		EmployeeID VARCHAR(20) NOT NULL
		,ShiftCode NVARCHAR(100) NOT NULL
		,ShiftDate DATETIME NOT NULL
		,CheckInTime VARCHAR(20) NOT NULL
		,CheckOutTime VARCHAR(20) NOT NULL
		,ModifiedDate DATETIME NOT NULL
		,IsInAfterDay BIT NOT NULL
		,IsOutAfterDay BIT NOT NULL
		,IsCrossDay BIT NOT NULL
		,TotalHours DECIMAL(10 ,2) NOT NULL
		)

	INSERT INTO #tmpWorkingShift (
		EmployeeID
		,ShiftCode
		,ShiftDate
		,CheckInTime
		,CheckOutTime
		,ModifiedDate
		,IsInAfterDay
		,IsOutAfterDay
		,IsCrossDay
		,TotalHours
		)
	SELECT t.c.value('@EmployeeID', 'VARCHAR(20)') AS EmployeeID
		,t.c.value('@ShiftCode', 'NVARCHAR(100)') AS ShiftCode
		,t.c.value('@ShiftDate', 'DATETIME') AS ShiftDate
		,t.c.value('@CheckInTime', 'VARCHAR(20)') AS CheckOutTime
		,t.c.value('@CheckOutTime', 'VARCHAR(20)') AS CheckOutTime
		,t.c.value('@LastUpdated', 'DATETIME') AS ModifiedDate
		,t.c.value('@IsInAfterDay', 'BIT') AS IsInAfterDay
		,t.c.value('@IsOutAfterDay', 'BIT') AS IsOutAfterDay
		,t.c.value('@IsCrossDay', 'BIT') AS IsCrossDay
		,t.c.value('@TotalHours', 'DECIMAL(10 ,2)') AS TotalHours
	FROM @WorkingShiftXml.nodes('Root/WorkingShiftSyncModel') AS t(c)
	OPTION (OPTIMIZE FOR (@WorkingShiftXml = NULL))
	

	INSERT INTO dbo.[Shift](
		[Code]
		,[Name]
		,[BeginCheckInTime]
		,[EndCheckInTime]
		,[BeginCheckOutTime]
		,[EndCheckOutTime]
		,[BreakTimeMins]
		,[TotalHours]
		,[CrossDay]
		,[IsInAfterDay]
		,[IsOutAfterDay]
		,[IsInBeforeDay]
		,[SyncModifiedDate]
		,[SyncDate]
		,[CreatedDate]
		,[IsDisabled]
		)
	SELECT 
		ws.ShiftCode
		,ws.ShiftCode
		,CheckInTime
		,CheckInTime
		,CheckOutTime
		,CheckOutTime
		,0
		,ws.TotalHours
		,ws.IsCrossDay
		,ws.IsInAfterDay
		,IsOutAfterDay
		,NULL
		,(select TOP 1 ws1.ModifiedDate from #tmpWorkingShift ws1 where ws1.ShiftCode = ws.ShiftCode)
		,@SyncDate
		,GETDATE(),
		0
	FROM #tmpWorkingShift AS ws WITH (NOLOCK)
	WHERE NOT EXISTS (
						SELECT 1
						FROM dbo.[Shift] AS s WITH (NOLOCK)
						WHERE s.Code = ws.ShiftCode
						)
	Group BY ws.ShiftCode, ws.CheckInTime, ws.CheckOutTime ,ws.IsInAfterDay ,ws.IsOutAfterDay ,ws.IsCrossDay,ws.TotalHours


	UPDATE ec
		SET ec.ShiftId = CASE 
						WHEN CHARINDEX('OFF', tmpws.ShiftCode) > 0 THEN NULL
						ELSE s.ShiftID END 
		,ec.IsOffDay = CASE 
						WHEN CHARINDEX('OFF', tmpws.ShiftCode) > 0 THEN 1
						ELSE ec.IsOffDay END 
		,ec.SyncDate = @SyncDate
	FROM #tmpWorkingShift AS tmpws
	INNER JOIN Employee AS e  WITH (NOLOCK) ON e.Code = tmpws.EmployeeID
	INNER JOIN EmployeeCalendar AS ec WITH (NOLOCK) ON ec.WorkDate = CAST(CONVERT(VARCHAR(10), tmpws.ShiftDate, 111) AS DATETIME) AND ec.EmpId = e.EmpId
	INNER JOIN [Shift] AS s WITH (NOLOCK) On s.Code = tmpws.ShiftCode
	WHERE ec.ChangeShiftId IS NULL AND ec.IsLeaveDay != 1 AND ec.IsKeepInView != 1

	INSERT INTO dbo.EmployeeCalendar(
		WorkDate
		,EmpId
		,ShiftId
		,IsOffDay
		,SyncDate
		)
		SELECT 
		w.ShiftDate
		,e.EmpId
		,CASE 
			WHEN CHARINDEX('OFF', w.ShiftCode) > 0 THEN NULL
			ELSE s.ShiftID
		END
		,CASE 
			WHEN CHARINDEX('OFF', w.ShiftCode) > 0 THEN 1
			ELSE 0
	END
	,@SyncDate
	FROM #tmpWorkingShift AS w
	INNER JOIN Employee AS e WITH (NOLOCK) ON e.Code = w.EmployeeID
	INNER JOIN [Shift] AS s WITH (NOLOCK) ON s.Code = w.ShiftCode
	WHERE NOT EXISTS (
					SELECT 1
					FROM dbo.EmployeeCalendar AS ec WITH (NOLOCK)
					WHERE ec.EmpId = e.EmpId AND CONVERT(VARCHAR(10), w.ShiftDate, 111) =  CONVERT(VARCHAR(10), ec.WorkDate, 111)
					)
	

	DROP TABLE #tmpWorkingShift
END
