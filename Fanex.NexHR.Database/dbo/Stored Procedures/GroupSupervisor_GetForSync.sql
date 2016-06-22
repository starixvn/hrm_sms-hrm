CREATE PROCEDURE [dbo].[GroupSupervisor_GetForSync] @FromDate DATETIME = NULL
AS
/*
Created: 20150921@Quinn.Vo
Task: Get all group supervisor from change date
DB: NexEPA
Original: 

Revisions:  
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;

	SELECT DISTINCT r.[OwnerId]
		,r.[GroupId]
		,m.[ModifiedDate]
		,ISNULL(r.[IsDeleted], 0) AS IsDeleted
	FROM (
		SELECT OwnerId
			,GroupId
			,MAX(ModifiedDate) AS ModifiedDate
		FROM dbo.GroupSupervisor WITH (NOLOCK)
		WHERE @FromDate IS NULL
			OR ModifiedDate > @FromDate
		GROUP BY OwnerId
			,GroupId
		) AS m
		INNER JOIN dbo.[GroupSupervisor] r WITH (NOLOCK) ON r.OwnerId = m.OwnerId
		AND r.GroupId = m.GroupId
		AND r.ModifiedDate = m.ModifiedDate
END


