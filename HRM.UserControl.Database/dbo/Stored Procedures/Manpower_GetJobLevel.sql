
CREATE PROCEDURE [dbo].[Manpower_GetJobLevel]
AS
/*  
Created: 20230905@Brian.Nguyen
Task: get job level
DB:
Original:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT jl.[Name] AS JobLevel
	FROM dbo.HRM_JobLevel AS jl WITH (NOLOCK)
	WHERE jl.IsDeleted = 0
	ORDER BY jl.[Name] DESC
END