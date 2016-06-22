CREATE function [dbo].[fnc_getLeaveTypeDefault] () 
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


