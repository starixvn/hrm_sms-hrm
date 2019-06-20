/****** Object:  Stored Procedure dbo.AnnualLeave_GetByID    Script Date: 7/20/2016 10:18:59 AM ******/
/****** Object:  Stored Procedure dbo.AnnualLeave_GetByID    Script Date: 7/19/2016 5:22:10 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


CREATE PROCEDURE [dbo].[AnnualLeave_GetByID]
	@LeaveTransId int
AS
/*
Created: Tom.Vo@2013-10-23
Task: retrieve data on shift
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/

SET NOCOUNT ON;

SELECT 
		LeaveTransId
		,[EmpId]
		,[RequestId]
		,[Description]
		,[FromDate] as AffectedFrom
		,[ToDate]
		,[DeductDays]
		,[CreditDays]
		, ISNULL(CreditDays,0) - ISNULL(DeductDays,0) AS [ToTalDays]
		,[Deleted]
		,[CreatedDate]
		,[CreatedBy]
		,[ModifiedDate]
		,[ModifiedBy]
FROM [dbo].[LeaveTrans] WITH(NOLOCK)
WHERE LeaveTransId = @LeaveTransId 
	AND RequestId IS NULL
	AND ISNULL(Deleted,0)=0