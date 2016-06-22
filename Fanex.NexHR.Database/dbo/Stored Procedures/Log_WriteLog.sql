CREATE PROCEDURE [dbo].[Log_WriteLog]
	@LogActionID int,
	@LogDate datetime,
	@Description NVARCHAR(4000),
	@TrackingValue NVARCHAR(MAX),
	@UserID int,
	@EmpId INT=NULL
AS
/*
Created: Tom.Vo@2013-10-25
Task: write log
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/

INSERT INTO [dbo].[Log]
           ([LogActionID]
           ,[LogDate]
           ,[Description]
           ,[TrackingValue]
           ,[UserID]
		   ,EmpId)
     VALUES
           (
			@LogActionID
           ,@LogDate
           ,@Description
           ,@TrackingValue
           ,@UserID
		   ,@EmpId
			)