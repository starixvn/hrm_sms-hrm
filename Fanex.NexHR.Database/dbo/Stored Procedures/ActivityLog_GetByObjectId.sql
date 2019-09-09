
CREATE PROCEDURE [dbo].[ActivityLog_GetByObjectId] @ObjectTypeId TINYINT
	,@ObjectId INT
AS
/*
Created: 20190904@Tim.Trinh
Task: get log
DB: HRM
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT lo.UserId
		,emp.NickName
		,emp.FullName
		,lo.LogDate
		,lo.OldData
		,lo.Data
	FROM dbo.ActivityLog AS lo WITH (NOLOCK)
		LEFT JOIN dbo.Employee AS emp WITH (NOLOCK) ON lo.UserId = emp.EmpId
	WHERE lo.ObjectTypeId = @ObjectTypeId
		AND lo.ObjectId = @ObjectId
		AND ISNULL(lo.OldData, '') <> ISNULL(lo.Data, '')
END
