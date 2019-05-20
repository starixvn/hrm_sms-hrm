/****** Object:  Stored Procedure dbo.Employee_Get    Script Date: 7/20/2016 10:19:02 AM ******/
/****** Object:  Stored Procedure dbo.Employee_Get    Script Date: 7/19/2016 5:22:12 PM ******/



CREATE PROCEDURE [dbo].[Employee_GetUploadedFile] 
	@empId INT
AS
/*
Created: 20190520@Oliver.Dang
Task: Get all employee from change date
DB: NexEPA
Original: 


*/
BEGIN
	SET NOCOUNT ON;

	SELECT e.EmpId
		  ,e.Code
		  ,euf.[FileName]
		  ,euf.UploadedDate
	FROM dbo.EmployeeUploadFile AS euf WITH (NOLOCK)
	INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON euf.EmpId = e.EmpId
	WHERE euf.EmpId = @empId
	
END