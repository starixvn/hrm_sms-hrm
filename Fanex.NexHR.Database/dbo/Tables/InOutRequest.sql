CREATE TABLE [dbo].[InOutRequest] (
    [RequestId]    BIGINT         NOT NULL,
    [EmpId]        INT            NULL,
    [LateInMins]   INT            NULL,
    [EarlyOutMins] INT            NULL,
    [FromDate]     DATETIME       NULL,
    [ToDate]       DATETIME       NULL,
    [RefNo]        NVARCHAR (200) NULL,
    CONSTRAINT [PK_InOutRequest] PRIMARY KEY CLUSTERED ([RequestId] ASC) ON [EmpRequest]
);


GO
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

 
CREATE TRIGGER [dbo].[InOutRequestBeforUpdate]
   ON  [dbo].InOutRequest
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	
	INSERT INTO [InOutRequestHistory] (
		[RequestId]
      ,[EmpId]
      ,[LateInMins]
      ,[EarlyOutMins]
      ,[FromDate]
      ,[ToDate]
      ,[RefNo]
      ,[InsertedDate])
		SELECT [RequestId]
		  ,[EmpId]
		  ,[LateInMins]
		  ,[EarlyOutMins]
		  ,[FromDate]
		  ,[ToDate]
			,[RefNo]
		  ,GETDATE() [InsertedDate]	FROM  deleted lt
	 
END
 


