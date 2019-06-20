/****** Object:  User Defined Function dbo.fnc_getLeaveTypeDefault    Script Date: 7/20/2016 10:18:58 AM ******/
/****** Object:  User Defined Function dbo.fnc_getLeaveTypeDefault    Script Date: 7/19/2016 5:22:10 PM ******/
CREATE FUNCTION [dbo].[fnc_getLeaveTypeDefault] () 
returns int
as
begin
	declare @value int
	select top 1 @value = LeaveTypeId
	from dbo.LeaveType WITH(NOLOCK)
	where IsDefault = 1

	--if @value is null select @value = dbo.fnc_GetLeaveDayCode()
	return @value
end


