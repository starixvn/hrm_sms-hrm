
CREATE PROCEDURE [dbo].[Manpower_Flow_Delete] @UnitName NVARCHAR(200)
	,@ModifiedBy INT
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg NVARCHAR(200) = '' OUTPUT
AS
/*
Created: 20230607@Tim.Trinh
Task: delete flow
DB:
Original:

Revisions:
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DELETE flow
	FROM dbo.Manpower_Flow AS flow WITH (ROWLOCK, UPDLOCK)
	WHERE flow.UnitName = @UnitName
END