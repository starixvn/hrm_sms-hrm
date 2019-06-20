/****** Object:  Stored Procedure dbo.WorkingTimeRequest_CheckNumberRequest    Script Date: 7/20/2016 10:19:20 AM ******/
/****** Object:  Stored Procedure dbo.WorkingTimeRequest_CheckNumberRequest    Script Date: 7/19/2016 5:22:21 PM ******/
---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[WorkingTimeRequest_CheckNumberRequest]
	@EmpId int, 
	@RequestType smallint,		
	@date datetime,
	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT	
AS
/*
Created: Tom.Vo@2013-11-05
Task: check the maximum request in month
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
IF (@RequestType IN (5,6)
	and (select count(1) 
			from dbo.WorkingTimeRequest r WITH (NOLOCK)
				inner join LeaveRequest l  WITH (NOLOCK)
					on r.requestid = l.requestid
		where r.empid=@empid 
			and RequestType IN (5,6)
			and Currentstatus in (1,2)
			and month(fromdate) =  month(@date)
			and year(fromdate) =  year(@date)
		) >= 2
	)
BEGIN
	SET @ErrCode =100
	SET @ErrMsg = 'The maximum reuqest in a month has reached'
	RETURN;
END

set @ErrCode=0


