
CREATE PROCEDURE [dbo].[ShiftMng_Set_ShiftSchedule]
	@ModifiedDate datetime = NULL,
	@ModifiedBy int = NULL,

	@ScheduleList varchar(max) = '',
	@ShiftMultipleList varchar(max) = '',
	
	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(1000) = '' OUTPUT	
AS
/*
Created: Tom.Vo@2013-11-18
Task: Update Schedule Arrange
DB: HRM_SMS
Original: 

Revisions:
Param's Explanation:
	@ScheduleList=N'<ROOT>
		 <WorkingCalenndar
			EmpId="1" 
			ShiftID="1"
			WorkDate="20130101"
			IsHoliday="1"
			IsOffDay="1"
			IsPublic="1"
		  </WorkingCalenndar>;

	@ShiftMultipleList=N'<ROOT>
		 <ShiftMultiple
			EmpId="1" 
			ForDate="20130101"
			ShiftID="1"
		  </ShiftMultiple>;

*/
SET NOCOUNT ON;

CREATE TABLE #EmployeeCalendar (WorkDate DateTime,ShiftID	INT,EmpId	INT,IsHoliday BIT, IsOffDay BIT, IsPublic BIT)
CREATE TABLE #EmployeeShiftMultiple (EmpId INT, ForDate DateTime, ShiftID INT)

DECLARE @DocHandler INT

BEGIN TRY	
	EXEC sp_xml_preparedocument @DocHandler OUTPUT, @ScheduleList

	INSERT INTO #EmployeeCalendar (WorkDate,ShiftID,EmpId,IsHoliday,IsOffDay, IsPublic)
	SELECT WorkDate,CASE WHEN ShiftId='' THEN NULL ELSE CONVERT(INT, ShiftId) END,EmpId,ISNULL(IsHoliday,0),ISNULL(IsOffDay,0),ISNULL(IsPublic,0)
	FROM OPENXML (@DocHandler, '/ROOT/WorkingCalenndar',1) 
	WITH (	EmpId	INT , 
			ShiftId	NVARCHAR(100),
			WorkDate DateTime,
			IsHoliday	BIT,
			IsOffDay	BIT,
			IsPublic	BIT
			) AS Data 

	EXEC sp_xml_removedocument @DocHandler

	EXEC sp_xml_preparedocument @DocHandler OUTPUT, @ShiftMultipleList

	INSERT INTO #EmployeeShiftMultiple (EmpId, ForDate, ShiftID)
	SELECT EmpId, ForDate, ShiftID
	FROM OPENXML (@DocHandler, '/ROOT/ShiftMultiple',1) 
	WITH (	EmpId	INT , 
			ForDate DateTime,
			ShiftId	NVARCHAR(100)
		) AS Data 

	EXEC sp_xml_removedocument @DocHandler

END TRY
BEGIN CATCH
		SELECT @ErrCode = 100, @ErrMsg = N'Paramter invalid.'
		Return
END CATCH;

DELETE  tmp
	FROM #EmployeeCalendar tmp
		INNER JOIN dbo.EmployeeAttendance ea WITH (NOLOCK) 
			ON tmp.WorkDate = ea.AttendanceDate AND tmp.EmpId = ea.EmpId 
		INNER JOIN dbo.EmployeeCalendar ec WITH (NOLOCK)  
			ON ec.WorkDate = tmp.WorkDate 
				AND ec.EmpId = tmp.EmpId
				AND ISNULL(ec.ShiftId,-1) = ISNULL(tmp.ShiftID,-1)
				AND ec.IsOffDay = tmp.IsOffDay
				AND ec.IsHoliday= tmp.IsHoliday
				AND ec.IsPublic= tmp.IsPublic
	WHERE tmp.WorkDate <=CONVERT(NVARCHAR(100),GETDATE(),111)

SELECT tmp.*,'Attendance' Reason
	INTO #tblError
	FROM #EmployeeCalendar tmp
		INNER JOIN dbo.EmployeeAttendance ea WITH (NOLOCK) 
			ON tmp.WorkDate = ea.AttendanceDate AND tmp.EmpId = ea.EmpId 
	WHERE tmp.WorkDate <=CONVERT(NVARCHAR(100),GETDATE(),111)

INSERT INTO #tblError
SELECT tmp.*,'Overtime' Reason
	FROM #EmployeeCalendar tmp
		INNER JOIN dbo.OTDailyRequest ot WITH (NOLOCK) 
			ON tmp.WorkDate = ot.OvertimeDate AND tmp.EmpId = ot.EmpId 
	WHERE tmp.WorkDate <=CONVERT(NVARCHAR(100),GETDATE(),111)
		AND ot.Status in (2,4,6)

DELETE  tmp
FROM #EmployeeCalendar tmp
	INNER JOIN dbo.OTDailyRequest ot WITH (NOLOCK) 
		ON tmp.WorkDate = ot.OvertimeDate AND tmp.EmpId = ot.EmpId 
	WHERE tmp.WorkDate <=CONVERT(NVARCHAR(100),GETDATE(),111)
		AND ot.Status in (2,4,6)

INSERT INTO #tblError
SELECT tmp.*,CASE 
	WHEN (ec.IsLeaveDay = 1 AND ec.LeaveDay>=1)  THEN 'Leave day' 
	WHEN ec.ChangeShiftId IS NOT NULL  THEN 'Change Shift day'
	WHEN  ec.IsKeepInView =1   THEN 'Keep In View Day'
	END Reason
FROM #EmployeeCalendar tmp
	INNER JOIN dbo.EmployeeCalendar ec WITH (NOLOCK)  
		ON tmp.WorkDate = ec.WorkDate AND tmp.EmpId = ec.EmpId
WHERE (ec.IsLeaveDay = 1 AND ec.LeaveDay=1) 
		OR ec.ChangeShiftId IS NOT NULL 
		OR (ec.IsKeepInView =1  AND tmp.ShiftId IS NULL)
		OR (ec.IsKeepInView =1  AND tmp.IsHoliday = 0 AND tmp.IsOffDay= 0 )

DELETE  tmp
FROM #EmployeeCalendar tmp
	INNER JOIN dbo.EmployeeCalendar ec WITH (NOLOCK)  
		ON tmp.WorkDate = ec.WorkDate AND tmp.EmpId = ec.EmpId
WHERE (ec.IsLeaveDay = 1 AND ec.LeaveDay=1) 
		OR ec.ChangeShiftId IS NOT NULL 
		OR (ec.IsKeepInView =1  AND tmp.ShiftId IS NULL)
		OR (ec.IsKeepInView =1  AND tmp.IsHoliday = 0 AND tmp.IsOffDay= 0 )

BEGIN TRY
	INSERT INTO dbo.tmpEmployeeAttendace(EmpId, WorkDate, InsertedDate)
	SELECT tmp.EmpId, tmp.WorkDate, GETDATE()
	FROM #EmployeeCalendar tmp  WITH (NOLOCK)
	LEFT JOIN dbo.tmpEmployeeAttendace AS att WITH (NOLOCK) ON tmp.EmpId = att.EmpId AND tmp.WorkDate = att.WorkDate
	WHERE att.EmpId IS NULL
	AND DATEDIFF(DD, tmp.WorkDate, GETDATE()) > 0

	UPDATE ec
	SET  
		ec.ShiftId = tmp.ShiftID,
		ec.ModifiedDate = @ModifiedDate,
		ec.ModifiedBy = @ModifiedBy,
		ec.IsOffDay = tmp.IsOffDay,
		ec.IsHoliday= tmp.IsHoliday,
		ec.IsPublic = tmp.IsPublic
	FROM dbo.EmployeeCalendar ec WITH(ROWLOCK,UPDLOCK)
		INNER JOIN #EmployeeCalendar tmp  WITH (NOLOCK)  
			ON tmp.WorkDate = ec.WorkDate AND tmp.EmpId = ec.EmpId

	DELETE  tmp
	FROM #EmployeeCalendar tmp
		INNER JOIN dbo.EmployeeCalendar ec
			ON tmp.WorkDate = ec.WorkDate AND tmp.EmpId = ec.EmpId

	INSERT INTO [dbo].[EmployeeCalendar]
           ([WorkDate]
           ,[EmpId]
           ,[ShiftId]
		   ,IsOffDay
		   ,IsHoliday
		   ,IsPublic
           ,[ModifiedDate]
           ,[ModifiedBy]
			)
     SELECT tmp.WorkDate,tmp.EmpId,tmp.ShiftID,IsOffDay,IsHoliday,IsPublic,@ModifiedDate [ModifiedDate],@ModifiedBy [ModifiedBy]
	 FROM #EmployeeCalendar tmp

	UPDATE sm
	SET  
		sm.ShiftId = tmp.ShiftID
	FROM dbo.EmployeeScheduleShiftMultiple sm WITH(ROWLOCK,UPDLOCK)
		INNER JOIN #EmployeeShiftMultiple tmp  WITH (NOLOCK)  
			ON tmp.ForDate = sm.ForDate AND tmp.EmpId = sm.EmpId

	DELETE  tmp
	FROM #EmployeeShiftMultiple tmp
		INNER JOIN dbo.EmployeeScheduleShiftMultiple sm
			ON tmp.ForDate = sm.ForDate AND tmp.EmpId = sm.EmpId

	 INSERT INTO [dbo].[EmployeeScheduleShiftMultiple](EmpId, ForDate, ShiftId)
     SELECT tmp.EmpId, tmp.ForDate, tmp.ShiftID
	 FROM #EmployeeShiftMultiple tmp
	 WHERE tmp.ShiftID>0

END TRY
BEGIN CATCH
	SELECT @ErrCode=100,@ErrMsg = 'Update Schedule Arrange'
	RETURN
END CATCH;

DROP TABLE #EmployeeCalendar
DROP TABLE #EmployeeShiftMultiple
DROP TABLE #tblError
