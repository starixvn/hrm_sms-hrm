Create PROCEDURE [dbo].[SyncData_GetWorkingShiftLastSyncDate]
AS
/*
Created: 20210317@Lilac.Nguyen
Task: Get last modified date of Shift
DB: bodb_HRM
Original: 
*/
BEGIN
	SET NOCOUNT ON;
	DECLARE @DefaultDate DATETIME = '01-01-1900'
	
	SELECT @DefaultDate = ISNULL(MAX(ec.SyncDate), @DefaultDate)
	FROM dbo.[EmployeeCalendar] ec WITH (NOLOCK)

	SELECT CONVERT(DATETIME, @DefaultDate)

END