
CREATE PROCEDURE [dbo].[EmployeeSupervisor_UpdateOrderShiftArrange]
	@SupervisorId  INT,
	@MemberOrderXml VARCHAR(MAX) = '',
	
	@ErrCode INT = 0 OUTPUT,
	@ErrMsg VARCHAR(200) = '' OUTPUT	
AS
/*
Created: 20151226@Quinn.Vo
Task: Update order for shift arrange
DB: HRM
Original:

Revisions:
Param's Explanation:
@MemberOrderXml=N'<ROOT>
		 <Employee
			EmployeeId="1"
			OrderCode="a">
		  </Employee>
		  </ROOT>'
*/
BEGIN
	SET NOCOUNT ON 

	DECLARE @EmployeeIds TABLE(
		EmployeeId INT, OrderCode VARCHAR(10), OrderNo SMALLINT IDENTITY(1, 1)
	)

	BEGIN TRY
		DECLARE @DocHandler INT

		EXEC sp_xml_preparedocument @DocHandler OUTPUT
			,@MemberOrderXml

		INSERT INTO @EmployeeIds (
			EmployeeId
			,OrderCode
			)
		SELECT EmployeeId 
			,OrderCode
		FROM OPENXML(@DocHandler, '/ROOT/Employee', 1) WITH (EmployeeId INT, OrderCode VARCHAR(10)) AS Data

		EXEC sp_xml_removedocument @DocHandler
	END TRY
	BEGIN CATCH
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update arrange shift employee order failed. Paramter invalid.'

		RETURN
	END CATCH

	UPDATE sd
	SET sd.OwnerId = ee.EmployeeId,
		sd.EmpId = @SupervisorId,
		sd.OrderCode = ee.OrderCode,
		sd.OrderNo = ee.OrderNo,
		sd.ModifiedDate = GETDATE()
	FROM [dbo].[ShiftArrangementOrder] AS sd WITH (ROWLOCK, UPDLOCK) 
	INNER JOIN @EmployeeIds AS ee ON sd.OwnerId = ee.EmployeeId
	WHERE sd.EmpId = @SupervisorId

	DELETE sd
	FROM [dbo].[ShiftArrangementOrder] AS sd 
	LEFT JOIN @EmployeeIds AS ee ON sd.OwnerId = ee.EmployeeId
	WHERE sd.EmpId = @SupervisorId
		AND ee.EmployeeId IS NULL

	--SELECT @SupervisorId
	--	,ee.EmployeeId
	--	,ee.OrderCode
	--	,ee.OrderNo
	--	,GETDATE()
	--FROM @EmployeeIds AS ee
	--LEFT JOIN [dbo].[ShiftArrangementOrder] AS sd ON sd.OwnerId = ee.EmployeeId AND sd.EmpId = @SupervisorId
	--WHERE sd.OwnerId IS NULL

	INSERT INTO [dbo].[ShiftArrangementOrder]
	(
		EmpId
		,OwnerId
		,OrderCode
		,OrderNo
		,ModifiedDate
	)
	SELECT @SupervisorId
		,ee.EmployeeId
		,ee.OrderCode
		,ee.OrderNo
		,GETDATE()
	FROM @EmployeeIds AS ee
	LEFT JOIN [dbo].[ShiftArrangementOrder] AS sd ON sd.OwnerId = ee.EmployeeId AND sd.EmpId = @SupervisorId
	WHERE sd.OwnerId IS NULL


	IF @@ERROR <> 0
	BEGIN
		SELECT @ErrCode = 1
			,@ErrMsg = N'Update arrange shift employee order failed.'
	END
	ELSE
	BEGIN
		SELECT @ErrCode = 0
			,@ErrMsg = ''
	END
END