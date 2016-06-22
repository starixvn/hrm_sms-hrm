CREATE TABLE [dbo].[LeaveRequest] (
    [RequestId]   BIGINT          NOT NULL,
    [EmpId]       INT             NOT NULL,
    [FromDate]    DATETIME        NOT NULL,
    [ToDate]      DATETIME        NOT NULL,
    [LeaveType]   INT             NULL,
    [TotalDays]   DECIMAL (18, 2) NULL,
    [RefNo]       NVARCHAR (200)  NULL,
    [ReworkCount] INT             CONSTRAINT [DF_LeaveRequest_ReworkCount] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_LeaveRequest] PRIMARY KEY CLUSTERED ([RequestId] ASC) ON [EmpRequest]
);


GO
CREATE NONCLUSTERED INDEX [IX_LeaveRequest_FromDate_ToDate]
    ON [dbo].[LeaveRequest]([FromDate] ASC, [ToDate] ASC, [EmpId] ASC)
    ON [EmpRequest];


GO
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

 
CREATE TRIGGER [dbo].[LeaveRequestBeforUpdate]
   ON  LeaveRequest
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	
	INSERT INTO [LeaveRequestHistory] ( 
			[RequestId]
		  ,[EmpId]
		  ,[FromDate]
		  ,[ToDate]
		  ,[LeaveType]
		  ,[TotalDays]
		  ,[RefNo]
		  ,[ReworkCount]
		  ,[InsertedDate]
		)
	SELECT [RequestId]
		  ,[EmpId]
		  ,[FromDate]
		  ,[ToDate]
		  ,[LeaveType]
		  ,[TotalDays]
		  ,[RefNo]
		  ,[ReworkCount]
		  ,GETDATE() [InsertedDate]	FROM  deleted lt
	 
END
 


