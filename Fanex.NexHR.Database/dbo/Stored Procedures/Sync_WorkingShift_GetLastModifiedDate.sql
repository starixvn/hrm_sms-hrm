
CREATE PROCEDURE [dbo].[Sync_WorkingShift_GetLastModifiedDate]
AS
/*
Created: 20210317@Lilac.Nguyen
Task: Get last modified date
DB: bodb_HRM
Original: 
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @SyncDate DATETIME = NULL

	SELECT @SyncDate = MAX(OWPModifiedDate)
	FROM dbo.EmployeeCalendar WITH (NOLOCK)

	SET @SyncDate = ISNULL(@SyncDate, '2022-01-01')

	SELECT @SyncDate
END
