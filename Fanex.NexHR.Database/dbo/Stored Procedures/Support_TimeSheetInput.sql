---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[Support_TimeSheetInput]  
	-- Add the parameters for the stored procedure here
	@EmpId int,
	@RecordTime datetime,
	@InOutType bit,
	@InsertDate datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    INSERT INTO [dbo].[TimeRecord] (EmpId, RecordTime, InOutType, InsertDate)
    VALUES(@EmpId,@RecordTime,@InOutType,@InsertDate)

	DECLARE @FromDate DATETIME
	DECLARE @ToDate DATETIME

	SET @FromDate = convert(varchar(10),dateadd(d,-2,@RecordTime) ,111)
	SET @ToDate = convert(varchar(10), @RecordTime ,111)

	EXEC EmployeeAttendance_ProcessingAndInsert @EmpId, @FromDate, @ToDate, 0 
END