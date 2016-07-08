---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[OverTime_GetNormalYearlyByHR]
	--@SupervisorID INT,
	@empid int = null,
	@groupids varchar(max) = '',
	@Year int = null,
	@status varchar(max) = '4,6',
	@excludedEmpID int= null,
	--@role varchar(max) = '6',

	@ErrCode int = 0 OUTPUT,
	@ErrMsg nvarchar(200) = '' OUTPUT,
	@isDebug BIT=0
AS
/*
Created: Tom.Vo@2013-11-04
Task:Get normal calendar By year For Overtime
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
SET NOCOUNT ON;

DECLARE @fromdate VARCHAR(25),	@todate VARCHAR(25)

SELECT @fromdate = CONVERT(VARCHAR(10),@Year) + '/01/01 00:00:00',
		@todate  = CONVERT(VARCHAR(10),@Year) + '/12/31 23:59:59'

IF (ISDATE(@fromdate) = 0 OR ISDATE(@todate) = 0)
BEGIN
	SELECT @ErrCode=100,@ErrMsg='Parameter invalid.'
	PRINT @ErrMsg
	RETURN
END

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

CREATE  TABLE #Calendar(_MONTH INT,isDeleted BIT)
DECLARE @FromMonth INT=1,@ToMonth INT=12

WHILE (@FromMonth <= @ToMonth )
BEGIN
	INSERT INTO #Calendar(_MONTH)
	VALUES (@FromMonth)
	SET @FromMonth =@FromMonth +1
END

CREATE TABLE #OTSumaryMonthly (
	[Month] DATETIME,
	EmpId INT,
	NormalHours DECIMAL(18, 2),
	OffHours DECIMAL(18, 2),
	PublicHours DECIMAL(18, 2)
)

INSERT INTO #OTSumaryMonthly([Month], EmpId, NormalHours, OffHours, PublicHours)
SELECT  CAST(CAST(@Year AS VARCHAR(4)) + '/' + CAST(MONTH(OvertimeDate) AS VARCHAR(2)) + '/01' AS DATETIME), dl.EmpId, SUM(NormalHours), SUM(OffHours), SUM(PublicHours)
FROM [dbo].OTDailyRequest AS dl WITH (NOLOCK) 
WHERE 
	[Status] IN (4, 6)
	AND OvertimeDate BETWEEN @fromdate AND @todate
GROUP BY EmpId, MONTH(OvertimeDate)


DECLARE @SQL VARCHAR(8000)
SET @SQL = 
'		SELECT empID,Code,NickName,FullName,Team,TeamCode,
			[1] Jan,[2] Feb,[3] Mar,[4] Apr,[5] May,[6] Jun,[7] Jul,[8] Aug,[9] Sep,[10] Oct,[11] Nov,[12] Dec,
			(ISNULL([1],0)+ISNULL([2],0)+ISNULL([3],0)+ISNULL([4],0)
			+ISNULL([5],0)+ISNULL([6],0)+ISNULL([7],0)+ISNULL([8],0)
			+ISNULL([9],0)+ISNULL([10],0)+ISNULL([11],0)+ISNULL([12],0)) AS Total
		FROM
		(
		SELECT 
						e.empID,
						e.Code AS Code,
						e.NickName AS NickName,
						e.FullName AS FullName,
						g.Name AS Team,
						g.Code AS TeamCode,
						cl._MONTH AS [Month],				
						(ot.[NormalHours]+ot.[OffHours]+ot.[PublicHours])ToTalHours

 FROM #Calendar cl 
	LEFT JOIN #OTSumaryMonthly ot WITH (NOLOCK) ON cl._MONTH = Month(Ot.Month)
	LEFT JOIN Employee e WITH (NOLOCK) ON e.empID = ot.empID	
	LEFT JOIN [Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID
		WHERE (ot.Month >= ''' + @FromDate + ''' AND	ot.Month  <= ''' + @todate + ''') '+CHAR(13)
	+ CASE WHEN @empid is null THEN '' ELSE '   and ot.empid = ' + CONVERT(NVARCHAR(13),@empid)+ '  ' +CHAR(13) END
	+ CASE WHEN @excludedEmpID is null THEN '' ELSE '   and ot.empid <> ' + CONVERT(NVARCHAR(13),@excludedEmpID)+ '  ' +CHAR(13) END
	--+ CASE WHEN @status is null or @status ='' THEN '' ELSE '			and ot.currentstatus IN (SELECT Currentstatus FROM #status ) ' +CHAR(13) END
	+ CASE WHEN @groupids ='' THEN '' ELSE '		and e.GroupId IN (SELECT GroupId FROM #GroupsList ) ' +CHAR(13) END
	+'	) AS tmp
 Pivot
 (
	MAX(ToTalHours) FOR [Month] IN ([1],[2],[3],[4],[5],[6],[7],[8],[9],[10],[11],[12])
 )AS tmp2 
 WHERE 
	empID IS NOT NULL '
	
	IF @isDebug = 1 PRINT @SQL 
	EXEC (@SQL )

SELECT @ErrCode=0,@ErrMsg=''

DROP TABLE #GroupsList
DROP TABLE #status
DROP TABLE #Calendar
DROP TABLE #OTSumaryMonthly


