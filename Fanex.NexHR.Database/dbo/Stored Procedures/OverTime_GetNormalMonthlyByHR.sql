---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[OverTime_GetNormalMonthlyByHR]
	--@SupervisorID INT=NULL,
	@empid int = null,
	@groupids varchar(max) = '',
	@fromdate datetime,
	@todate datetime,
	@status varchar(max) = null,
	@excludedEmpID int= null,
	--@role varchar(max) = '',

	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT,
	@isDebug BIT=0
AS
/*
Created: Tom.Vo@2013-11-04
Task:Get  normal calendar  By Date For Overtime
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
SET NOCOUNT ON;

SELECT @fromdate = CONVERT(VARCHAR(10),@fromDate,111) + ' 00:00:00',
		@todate = CONVERT(VARCHAR(10),@toDate,111) + ' 23:59:59'

CREATE TABLE #status (Currentstatus TINYINT)
IF LEN(ISNULL(@status,'')) = 0
	SET @status =  '4,6'

IF ISNULL(@status,'') <>''
	INSERT INTO #status  (Currentstatus)
	SELECT val FROM fnc_strSplit (@status, ',')


IF (EXISTS(SELECT TOP 1 1 FROM #status WHERE Currentstatus = 1))
	BEGIN
		DELETE #status WHERE Currentstatus IN (2,3)
		INSERT #status (Currentstatus)
			VALUES (2),
					(3)
	END

CREATE TABLE #GroupsList (GroupId INT)
SET @groupids = ISNULL(@groupids,'')

IF @groupids <>''
	INSERT INTO #GroupsList (GroupId)
	SELECT val FROM fnc_strSplit (@groupids, ',')
ELSE
	BEGIN
		INSERT INTO #GroupsList (GroupId)
		SELECT GRoupID FROM [Group] g WITH (NOLOCK)
	END


DECLARE @SQL VARCHAR(8000)
SET @SQL = 'SELECT 
				r.empID,
				r.Month AS RequestDate,

				r.[NormalHours],
			    r.[OffHours],
			    r.[PublicHours],
				(r.[NormalHours]+r.[OffHours]+r.[PublicHours]) ToTalHours,
				
				e.EmpId AS RequesterID,
				e.NickName AS Requester,
				e.FullName AS FullNameRequester,
				
				g.GroupId As GroupId,
				g.Name AS Team,
				g.Code AS TeamCode
				
 FROM  [dbo].[OTMonthlySum] r WITH (NOLOCK)
				LEFT JOIN Employee e WITH (NOLOCK) ON e.empID = r.empID	
				
				LEFT JOIN [Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID

		WHERE (e.isDeleted is null or e.isDeleted = 0)'+CHAR(13)
	+'   AND	r.Month >= ''' + CONVERT(NVARCHAR(20),@fromdate)+ ''' 
				and r.Month  <= ''' + CONVERT(NVARCHAR(20),@todate)+ ''''+CHAR(13)
	+ CASE WHEN @empid is null THEN '' ELSE '   and r.empid = ' + CONVERT(NVARCHAR(13),@empid)+ '  ' +CHAR(13) END
	+ CASE WHEN @excludedEmpID is null THEN '' ELSE '   and r.empid <> ' + CONVERT(NVARCHAR(13),@excludedEmpID)+ '  ' +CHAR(13) END
	--+ CASE WHEN @status is null or @status ='' THEN '' ELSE '			and r.currentstatus IN (SELECT Currentstatus FROM #status ) ' +CHAR(13) END
	+ CASE WHEN @groupids ='' THEN '' ELSE '		and e.GroupId IN (SELECT GroupId FROM #GroupsList ) ' +CHAR(13) END
	+'			ORDER BY r.Month DESC'
	
	IF @isDebug = 1 PRINT @SQL 
	EXEC (@SQL )

SELECT @ErrCode=0,@ErrMsg=''
	
DROP TABLE #GroupsList
DROP TABLE #status


