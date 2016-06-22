---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Employee_Shift_Get]
	@EmpID int
AS
	/*
	Created: Tom.Vo@2013-10-23
	Task: Get Shift of Employee
	DB: HRM_SMS
	Original: 

	Revisions:    
	Param's Explanation:

	*/
BEGIN
SET NOCOUNT ON;

SELECT [EmpID]
      ,[MonShift] AS MonShiftId
	  ,ISNULL(s1.Name, 'OFF') AS MonShiftName
      ,[TueShift] AS TueShiftId
	  ,ISNULL(s2.Name, 'OFF') AS TueShiftName
      ,[WedShift] AS WedShiftId
	  ,ISNULL(s3.Name, 'OFF') AS WedShiftName
      ,[ThuShift] AS ThuShiftId
	  ,ISNULL(s4.Name, 'OFF') AS ThuShiftName
      ,[FriShift] AS FriShiftId
	  ,ISNULL(s5.Name, 'OFF') AS FriShiftName
      ,[SatShift] AS SatShiftId
	  ,ISNULL(s6.Name, 'OFF') AS SatShiftName
      ,[SunShift] AS SunShiftId
	  ,ISNULL(s7.Name, 'OFF') AS SunShiftName
FROM [dbo].[EmployeeDefaultShift] AS es WITH(NOLOCK)
LEFT JOIN dbo.[Shift] AS s1 WITH (NOLOCK) ON es.MonShift = s1.ShiftID
LEFT JOIN dbo.[Shift] AS s2 WITH (NOLOCK) ON es.TueShift = s2.ShiftID
LEFT JOIN dbo.[Shift] AS s3 WITH (NOLOCK) ON es.WedShift = s3.ShiftID
LEFT JOIN dbo.[Shift] AS s4 WITH (NOLOCK) ON es.ThuShift = s4.ShiftID
LEFT JOIN dbo.[Shift] AS s5 WITH (NOLOCK) ON es.FriShift = s5.ShiftID
LEFT JOIN dbo.[Shift] AS s6 WITH (NOLOCK) ON es.SatShift = s6.ShiftID
LEFT JOIN dbo.[Shift] AS s7 WITH (NOLOCK) ON es.SunShift = s7.ShiftID
WHERE  EmpID = @EmpID

END


