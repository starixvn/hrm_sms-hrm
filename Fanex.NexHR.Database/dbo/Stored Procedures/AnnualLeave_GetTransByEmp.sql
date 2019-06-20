
/****** Object:  Stored Procedure dbo.AnnualLeave_GetTransByEmp    Script Date: 7/20/2016 10:18:59 AM ******/
/****** Object:  Stored Procedure dbo.AnnualLeave_GetTransByEmp    Script Date: 7/19/2016 5:22:10 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


CREATE PROCEDURE [dbo].[AnnualLeave_GetTransByEmp] 
	@EmpID int,
	@Year int,
	@excludedRequestTrans bit
AS
/*
Created: Tom.Vo@2013.10.22
Task: show annual leave tranasaction by employee
DB: NexHR
Original: 

Revisions:    
Param's Explanation:

*/

BEGIN
	SET NOCOUNT ON;
	declare @fromdate datetime, @todate datetime
	set @fromdate = cast(@year as varchar(4)) + '/01/01'
	set @todate=dateadd(yy,1,@fromdate)		
    
	SELECT 
		t.LeaveTransId,
		RequestID,
		FromDate AS AffectedFrom,
		ISNULL(CreditDays,0) - ISNULL(DeductDays,0) AS [ToTalDays],
		Description,
		ISNULL(t.ModifiedDate, t.CreatedDate) AS ModifiedDate
	FROM dbo.LeaveTrans t WITH (NOLOCK) 
		INNER JOIN dbo.Employee e WITH (NOLOCK) ON e.EmpID = t.EmpID	
	WHERE fromdate>=@fromdate and fromdate<@todate AND
		t.EmpId=@EmpID AND
		ISNULL(Deleted,0)=0 AND
		(@excludedRequestTrans=0 OR RequestId IS NULL) 		
		
END



