
CREATE PROCEDURE [dbo].[KeepInViewRequest_InsertTrans] @RequestId BIGINT
AS
/*
Created: 20190522@Quinn.Vo
Task: Insert KIV transaction.
DB: HRM
Original:

Revisions:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @OnDate DATE
		,@IsHalfDay BIT
		,@TotalMinutes INT
		,@EmpId INT
		,@KeepInViewType TINYINT
		-- keep in view type
		,@KeepInViewByDay TINYINT = 1
		,@KeepInViewByHour TINYINT = 2

	SELECT @EmpId = EmpId
		,@OnDate = OnDate
		,@IsHalfDay = IsHalfDay
		,@TotalMinutes = DATEDIFF(MI, FromTime, ToTime)
		,@KeepInViewType = KeepInViewType
	FROM dbo.KeepInViewRequest WITH (NOLOCK)
	WHERE RequestId = @RequestId

	IF @KeepInViewType = @KeepInViewByDay
	BEGIN
		INSERT INTO dbo.KeepInViewByDayTrans (
			EmpId
			,RequestId
			,CreditDays
			,DeductDays
			,InYear
			,InsertedDate
			)
		VALUES (
			@EmpId
			,@RequestId
			,CASE 
				WHEN @IsHalfDay = 1 THEN 0.5
				ELSE 1
				END
			,0
			,YEAR(@OnDate)
			,GETDATE()
			)
	END
	ELSE
	BEGIN
		INSERT INTO dbo.KeepInViewByHourTrans (
			EmpId
			,WorkDate
			,RequestId
			,CreditMinutes
			,DeductMinutes
			,InsertedDate
			,MinutesBalance
			)
		VALUES (
			@EmpId
			,@OnDate
			,@RequestId
			,@TotalMinutes
			,0
			,GETDATE()
			,@TotalMinutes
			)
	END
END
