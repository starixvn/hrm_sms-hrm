
CREATE PROCEDURE [dbo].[Holiday_Update] @Id INT
	,@TypeId TINYINT
	,@HolidayName NVARCHAR(50)
	,@FromDate DATETIME
	,@ToDate DATETIME
	,@ModifiedBy INT
	,@ModifiedDate DATETIME
	,@ErrCode INT = 0 OUTPUT
	,@ErrMsg VARCHAR(200) = '' OUTPUT
AS
/*
Created: Tom.Vo@2013-10-23
Task: update a holiday
DB: NexHR
Original:

Revisions:
Param's Explanation:

*/
BEGIN
	SET NOCOUNT ON;
	SET XACT_ABORT ON;

	DECLARE @OldFromdate DATETIME
		,@Oldtodate DATETIME

	SELECT @OldFromdate = fromdate
		,@Oldtodate = todate
	FROM dbo.Holiday WITH (NOLOCK)
	WHERE Id = @id

	BEGIN TRANSACTION;

	IF @OldFromdate = @FromDate
		AND @ToDate = @Oldtodate
	BEGIN
		UPDATE dbo.Holiday WITH (ROWLOCK, UPDLOCK)
		SET HolidayName = @HolidayName
			,TypeId = @TypeId
		WHERE Id = @Id
	END
	ELSE
	BEGIN
		UPDATE dbo.EmployeeCalendar WITH (ROWLOCK, UPDLOCK)
		SET IsHoliday = 0
		WHERE WorkDate BETWEEN @OldFromdate
				AND @Oldtodate

		UPDATE cl
		SET cl.IsHoliday = 1
			,cl.ModifiedBy = @ModifiedBy
			,cl.ModifiedDate = @ModifiedDate
		FROM dbo.EmployeeCalendar AS cl WITH (ROWLOCK, UPDLOCK)
		WHERE WorkDate BETWEEN @FromDate
				AND @ToDate
			AND EXISTS (
				SELECT 1
				FROM dbo.EmployeeSummary WITH (NOLOCK)
				WHERE EmpId = cl.EmpId
					AND (
						IsLocal = 1
						OR (
							IsLocal = 0
							AND IsExpatUseHoliday = 1
							)
						)
				)

		DECLARE @WorkDate DATETIME

		SELECT @WorkDate = CONVERT(VARCHAR(10), @FromDate, 111)

		WHILE DATEDIFF(d, @WorkDate, @ToDate) >= 0
		BEGIN
			IF @WorkDate < GETDATE()
			BEGIN
				INSERT INTO dbo.tmpEmployeeAttendace (
					EmpId
					,WorkDate
					,InsertedDate
					)
				SELECT EmpId
					,@WorkDate
					,GETDATE()
				FROM dbo.Employee AS e WITH (NOLOCK)
				WHERE [Enabled] = 1
			END

			INSERT INTO dbo.EmployeeCalendar (
				EmpId
				,WorkDate
				,IsHoliday
				,ModifiedDate
				)
			SELECT EmpId
				,@WorkDate
				,1
				,GETDATE()
			FROM dbo.Employee AS e WITH (NOLOCK)
			WHERE [Enabled] = 1
				AND EXISTS (
					SELECT 1
					FROM dbo.EmployeeSummary WITH (NOLOCK)
					WHERE EmpId = e.EmpId
						AND (
							IsLocal = 1
							OR (
								IsLocal = 0
								AND IsExpatUseHoliday = 1
								)
							)
					)
				AND NOT EXISTS (
					SELECT 1
					FROM dbo.EmployeeCalendar WITH (NOLOCK)
					WHERE EmpId = e.EmpId
						AND WorkDate = @WorkDate
					)

			SET @WorkDate = DATEADD(d, 1, @WorkDate)
		END

		UPDATE dbo.Holiday WITH (ROWLOCK, UPDLOCK)
		SET HolidayName = @HolidayName
			,TypeId = @TypeId
			,FromDate = @FromDate
			,ToDate = @ToDate
		WHERE Id = @Id

		IF @@ERROR <> 0
		BEGIN
			SELECT @ErrCode = 1
				,@ErrMsg = N'Update Holiday failed.'
		END
	END

	COMMIT TRANSACTION;
END



