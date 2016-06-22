---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[DBA_Job_EmployeeCalendar_Insert_weekly]
	@empid int=NULL,
	@ErrCode int=0 OUTPUT,
	@ErrMsg varchar(200)='' OUTPUT
AS
SET NOCOUNT ON

DECLARE	
	@fromdate datetime, 
	@todate datetime	

SELECT @fromdate = GETDATE(),@todate= DATEADD(DD, 120, GETDATE())

set @fromdate = convert(varchar(10),@fromdate,111)
set @todate = convert(varchar(10),@todate,111)

DECLARE @tblCalendare TABLE(WorkDate SmallDateTime,IsEnabled BIT) 

while @fromdate<=@todate
begin
	INSERT INTO @tblCalendare (WorkDate,IsEnabled)
	VALUES (@fromdate,1)

	set @fromdate=dateadd(d,1,@fromdate)

end 
	-- insert vao working calendar
insert into dbo.EmployeeCalendar (EmpID, WorkDate, ShiftID, isOffDay, IsHoliday, IsPublic)
select distinct s.empid,
		tblCalendare.WorkDate as WorkDate,
		case when datepart(dw, tblCalendare.WorkDate)=1 then SunShift
			when datepart(dw, tblCalendare.WorkDate)=2 then MonShift
			when datepart(dw, tblCalendare.WorkDate)=3 then TueShift
			when datepart(dw, tblCalendare.WorkDate)=4 then WedShift
			when datepart(dw, tblCalendare.WorkDate)=5 then ThuShift
			when datepart(dw, tblCalendare.WorkDate)=6 then FriShift
			when datepart(dw, tblCalendare.WorkDate)=7 then SatShift
		end as ShiftID,
		case when es.IsLocal =1 AND hld.Id IS NOT NULL then 0 
			when datepart(dw, tblCalendare.WorkDate)=1 and SunShift is null then 1 
			when datepart(dw, tblCalendare.WorkDate)=2 and MonShift is null then 1
			when datepart(dw, tblCalendare.WorkDate)=3 and TueShift is null then 1
			when datepart(dw, tblCalendare.WorkDate)=4 and WedShift is null then 1
			when datepart(dw, tblCalendare.WorkDate)=5 and ThuShift is null then 1
			when datepart(dw, tblCalendare.WorkDate)=6 and FriShift is null then 1
			when datepart(dw, tblCalendare.WorkDate)=7 and SatShift is null then 1
			else 0
		end as isOffDay,
		case when es.IsLocal =1 AND  hld.Id IS NOT NULL then 1 else 0 end  as isHoliday,
		1
from dbo.EmployeeDefaultShift s WITH(NOLOCK) 
	inner join dbo.Employee e WITH(NOLOCK) on e.empid=s.empid
	inner join @tblCalendare tblCalendare on tblCalendare.IsEnabled = e.Enabled
	inner join dbo.EmployeeSummary es WITH(NOLOCK) on es.empid=s.empid
	LEFT JOIN EmployeeCalendar c ON e.EmpId = c.EmpId AND c.WorkDate = tblCalendare.WorkDate 
	LEFT JOIN dbo.Holiday hld WITH(NOLOCK)  ON  tblCalendare.WorkDate between hld.FromDate and hld.ToDate	
where es.IsLocal = 1
	and es.IsDefaultSchedule = 1
	and isnull(IsDeleted,0)=0 
	and(@empid is null or s.empid=@empid)
	and c.EmpId is null
	and (s.SunShift IS NOT NULL
		or s.MonShift IS NOT NULL
		or s.TueShift IS NOT NULL
		or s.WedShift IS NOT NULL
		or s.ThuShift IS NOT NULL
		or s.FriShift IS NOT NULL
		or s.SatShift IS NOT NULL
	)

 IF @@ERROR <> 0 
	SELECT @ErrCode=100,@ErrMsg=N'Create Default Calendar for employee failed'
ELSE 
	SELECT @ErrCode=0,@ErrMsg=''