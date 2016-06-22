CREATE PROCEDURE [dbo].[EmployeeCalendar_Insert]
	@empid int,
	@fromdate datetime, 
	@todate datetime	,

	@ErrCode int = 0 OUTPUT,
	@ErrMsg varchar(200) = '' OUTPUT	
AS	

/*
Created: Tom.Vo@2013-10-21
Task: Insert default working calendar for employee
DB: HRM_SMS
Original: 

Revisions:  
Param's Explanation:

*/
SET NOCOUNT ON
-- cut off time
set @fromdate = convert(varchar(10),@fromdate,111)
set @todate = convert(varchar(10),@todate,111)

declare @dw int, @isHoliday bit

while @fromdate<=@todate
begin 
	set @isHoliday=0
	set @dw=datepart(dw, @fromdate)
	--print @dw
		
	-- kiem tra @fromdate co phai la ngay nghi
	if (exists(select TOP 1 1 
				from dbo.Holiday WITH(NOLOCK)
				where @fromdate between FromDate and ToDate))
		set @isHoliday=1

	-- se xu ly leave request o day neu can

	-- insert vao working calendar
	insert into dbo.EmployeeCalendar (EmpID, WorkDate, ShiftID, isOffDay, IsHoliday, ModifiedDate)
	select s.empid,
			@fromdate as WorkDate,
			case when @dw=1 then SunShift
				when @dw=2 then MonShift
				when @dw=3 then TueShift
				when @dw=4 then WedShift
				when @dw=5 then ThuShift
				when @dw=6 then FriShift
				when @dw=7 then SatShift
			end as ShiftID,
			case when es.IsLocal =1 AND @isHoliday = 1 then 0 
				when @dw=1 and SunShift is null then 1 
				when @dw=2 and MonShift is null then 1
				when @dw=3 and TueShift is null then 1
				when @dw=4 and WedShift is null then 1
				when @dw=5 and ThuShift is null then 1
				when @dw=6 and FriShift is null then 1
				when @dw=7 and SatShift is null then 1
				else 0
			end as isOffDay,
			case when es.IsLocal =1 AND @isHoliday = 1 then 1 else 0 end as isHoliday,
			GETDATE()
		
	from dbo.EmployeeDefaultShift s WITH(NOLOCK) 
		join dbo.Employee e WITH(NOLOCK) on e.empid=s.empid
		left join dbo.EmployeeSummary es WITH(NOLOCK) on es.empid=s.empid
	where isnull(IsDeleted,0)=0 
		and(@empid is null or s.empid=@empid)
		and es.IsDefaultSchedule = 1
		and isnull(ResignedDate,'2079/01/01' )>@fromdate
		and  not exists (SELECT TOP 1 1 FROM EmployeeCalendar c WITH(NOLOCK)
				WHERE WorkDate = @fromdate AND s.empid=c.empid)
			
	set @fromdate=dateadd(d,1,@fromdate)
end

 IF @@ERROR <> 0 
	SELECT @ErrCode=100,@ErrMsg=N'Create Default Calendar for employee failed'
ELSE 
	SELECT @ErrCode=0,@ErrMsg=''