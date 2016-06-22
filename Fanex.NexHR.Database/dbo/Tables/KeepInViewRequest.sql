CREATE TABLE [dbo].[KeepInViewRequest] (
    [RequestId]      BIGINT          NOT NULL,
    [EmpId]          INT             NULL,
    [IsHalfDay]      BIT             CONSTRAINT [DF_KeepInViewRequest_IsHalfDay] DEFAULT ((0)) NULL,
    [ShiftId]        INT             NULL,
    [FromTime]       DATETIME        NULL,
    [ToTime]         DATETIME        NULL,
    [ExpExchangeDay] DECIMAL (18, 2) NULL,
    [RefNo]          NVARCHAR (200)  NULL,
    [FromDate]       SMALLDATETIME   NULL,
    [ToDate]         SMALLDATETIME   NULL,
    CONSTRAINT [PK_KeepInViewRequest] PRIMARY KEY CLUSTERED ([RequestId] ASC) ON [EmpRequest]
);


GO
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

 
CREATE TRIGGER [dbo].[KeepInViewRequestBeforUpdate]
   ON  [dbo].[KeepInViewRequest]
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
	
	INSERT INTO [KeepInViewRequestHistory] ( [RequestId]
      ,[EmpId]
      ,[IsHalfDay]
      ,[ShiftId]
      ,[FromTime]
      ,[ToTime]
      ,[ExpExchangeDay]
      ,[RefNo]
	  ,FromDate
	  ,ToDate
      ,[InsertedDate])
	SELECT [RequestId]
      ,[EmpId]
      ,[IsHalfDay]
      ,[ShiftId]
      ,[FromTime]
      ,[ToTime]
      ,[ExpExchangeDay]
      ,[RefNo]
	  ,FromDate
	  ,ToDate
		  ,GETDATE() [InsertedDate]	FROM  deleted lt
	 
END
 


