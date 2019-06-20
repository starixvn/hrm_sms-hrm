/****** Object:  Stored Procedure dbo.EmployeeGroup_Get    Script Date: 7/20/2016 10:19:06 AM ******/
/****** Object:  Stored Procedure dbo.EmployeeGroup_Get    Script Date: 7/19/2016 5:22:14 PM ******/

CREATE PROCEDURE [dbo].[EmployeeGroup_Get] 
	@FromDate DATETIME = NULL
AS
/*
Created: 20140320@Quinn.Vo
Task: Get all groups from change date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	IF(@FromDate IS NOT NULL)
	BEGIN
		SELECT @FromDate = DATEADD(HH, -1, @FromDate)
	END

	SELECT
	g.GroupId AS TeamId,
	g.DepartmentId,
	g.Code,
	g.Name,
	g.LeaderId,
	g.AssistantLeaderId,
	g.[Rank],
	ISNULL(g.IsDeleted, 0) AS IsDeleted,
	CASE WHEN g.ModifiedDate IS NULL THEN g.CreatedDate ELSE g.ModifiedDate END AS ModifiedDate
	FROM dbo.[Group] AS g WITH (NOLOCK)
	WHERE @FromDate IS NULL
	OR g.ModifiedDate > @FromDate

	ORDER BY g.ModifiedDate
END
