CREATE TABLE [dbo].[LeaveTrans] (
    [LeaveTransId] BIGINT          IDENTITY (1, 1) NOT NULL,
    [EmpId]        INT             NOT NULL,
    [RequestId]    BIGINT          NULL,
    [Description]  NVARCHAR (4000) NULL,
    [FromDate]     DATETIME        NOT NULL,
    [ToDate]       DATETIME        NULL,
    [DeductDays]   NUMERIC (18, 1) NULL,
    [CreditDays]   NUMERIC (18, 1) NULL,
    [Deleted]      BIT             NULL,
    [CreatedDate]  DATETIME        NULL,
    [CreatedBy]    INT             NULL,
    [ModifiedDate] DATETIME        NULL,
    [ModifiedBy]   INT             NULL,
    CONSTRAINT [PK_LeaveTrans] PRIMARY KEY CLUSTERED ([LeaveTransId] ASC) ON [EmpRequest]
);


GO
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE TRIGGER [dbo].[LeaveTransBeforUpdate]
   ON  [dbo].[LeaveTrans]
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;
 
	INSERT INTO [dbo].[Log](
		 [LogActionID]
		  ,[LogDate]
		  ,[Description]
		  ,[TrackingValue]
		  ,[UserID]
		  ,EmpID
		  )
		SELECT 100,getdate() as [LogDate],lt.Description,
		'<From Date: ' + CONVERT(nvarchar(25),ISNULL(lt.FromDate,'-'),101)
		+',DeductDays:' + CONVERT(nvarchar(25),ISNULL(lt.DeductDays,0))
		+',CreditDays:' + CONVERT(nvarchar(25),ISNULL(lt.CreditDays,0))
		+',ModifiedDate: ' + CONVERT(nvarchar(25),ISNULL(lt.ModifiedDate,getdate()),101)
		+',ModifiedBy:' + CONVERT(nvarchar(25),ISNULL(lt.ModifiedBy,'-'))
		+'>' as [TrackingValue],
		lt.EmpID as [UserID],
		lt.EmpID as EmpID
	FROM  deleted lt

END
 