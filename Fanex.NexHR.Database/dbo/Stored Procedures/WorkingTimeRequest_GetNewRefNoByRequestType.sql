
CREATE PROCEDURE [dbo].[WorkingTimeRequest_GetNewRefNoByRequestType] @RefNo NVARCHAR(100) = NULL OUTPUT
	,@RequestType INT = NULL
AS
/*
Created: Tom.Vo@2013-11-05
Task: Get New RefNo From Request Type
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
	
*/
BEGIN
	SET NOCOUNT ON;

	IF @RequestType = 6
		SELECT @RefNo = Max(RefNo)
		FROM dbo.LeaveRequest r WITH (NOLOCK)
			INNER JOIN dbo.WorkingTimeRequest wr WITH (NOLOCK) ON r.RequestId = wr.RequestId
	ELSE IF @RequestType = 7
		SELECT @RefNo = Max(RefNo)
		FROM dbo.KeepInViewRequest r WITH (NOLOCK)
			INNER JOIN [dbo].WorkingTimeRequest wr WITH (NOLOCK) ON r.RequestId = wr.RequestId

	DECLARE @Num INT

	--Ref-000001
	IF ISNULL(@RefNo, '') = ''
		SET @RefNo = CASE 
				WHEN @RequestType = 6 THEN 'Ref-000001'
				WHEN @RequestType = 7 THEN 'KIV-000001'
				ELSE 'Ref-000001'
				END
	ELSE
	BEGIN
		SET @Num = CONVERT(INT, RIGHT(@RefNo, LEN(@RefNo) - 4)) + 1
		SET @RefNo = CASE 
				WHEN @RequestType = 6 THEN 'Ref-'
				WHEN @RequestType = 7 THEN 'KIV-'
				ELSE 'Ref-'
				END + REPLICATE('0', 6 - LEN(@Num)) + CONVERT(NVARCHAR(100), @Num)
	END
END


