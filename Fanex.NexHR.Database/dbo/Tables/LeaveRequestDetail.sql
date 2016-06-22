CREATE TABLE [dbo].[LeaveRequestDetail] (
    [RequestId] BIGINT   NOT NULL,
    [EmpId]     INT      NOT NULL,
    [OnDate]    DATETIME NOT NULL,
    [ShiftId]   INT      NULL,
    [LeaveType] INT      NULL,
    [IsHalfDay] BIT      NULL,
    [FromTime]  DATETIME NULL,
    [ToTime]    DATETIME NULL,
    [WorkType]  TINYINT  NULL
);


GO
CREATE CLUSTERED INDEX [IX_LeaveRequestDetail_IDX_Shift]
    ON [dbo].[LeaveRequestDetail]([OnDate] ASC, [EmpId] ASC, [RequestId] ASC, [WorkType] ASC)
    ON [EmpRequest];


GO
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


CREATE TRIGGER [dbo].[LeaveRequestDetailBeforUpdate]
   ON  [dbo].[LeaveRequestDetail] 
   AFTER UPDATE,DELETE
AS 
BEGIN
	SET NOCOUNT ON;

	DECLARE @ReworkCount INT 

	SELECT @ReworkCount  = MAX(lrh.ReworkCount) 
		FROM dbo.LeaveRequestHistory lrh WITH(NOLOCK)
			INNER JOIN deleted d
				ON lrh.RequestId = d.RequestId

	SET @ReworkCount = ISNULL(@ReworkCount,0)

	INSERT INTO LeaveRequestDetailHistory(
		[RequestId]
		  ,[EmpId]
		  ,[OnDate]
		  ,[ShiftId]
		  ,[LeaveType]
		  ,[IsHalfDay]
		  ,[FromTime]
		  ,[ToTime]
		  ,[InsertedDate]
		  ,ReworkCount 
		  ,WorkType
		)
	SELECT [RequestId]
		  ,[EmpId]
		  ,[OnDate]
		  ,[ShiftId]
		  ,[LeaveType]
		  ,[IsHalfDay]
		  ,[FromTime]
		  ,[ToTime]
		  ,GETDATE()
		  ,@ReworkCount
		  ,WorkType
	  FROM  deleted lt

END


