CREATE TABLE [dbo].[InoutMissRequest] (
    [RequestId]   BIGINT         NOT NULL,
    [EmpId]       INT            NOT NULL,
    [OnDate]      DATETIME       NOT NULL,
    [CheckTime]   DATETIME       NOT NULL,
    [RefNo]       NVARCHAR (200) NULL,
    [ReworkCount] INT            CONSTRAINT [DF_InoutMissRequest_ReworkCount] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_InoutMissRequest] PRIMARY KEY CLUSTERED ([RequestId] ASC) ON [EmpRequest]
);


GO
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

 
CREATE TRIGGER [dbo].[InoutMissRequestBeforUpdate]
   ON  [dbo].[InoutMissRequest]
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	
	INSERT INTO [InoutMissRequestHistory] (
		[RequestId],
		[EmpId] ,
		[OnDate] ,
		[CheckTime] ,
		[RefNo] ,
		[ReworkCount],
		InsertedDate)
		SELECT [RequestId],
			[EmpId] ,
			[OnDate] ,
			[CheckTime] ,
			[RefNo] ,
			[ReworkCount]
		  ,GETDATE() [InsertedDate]	FROM  deleted lt
	 
END
 


