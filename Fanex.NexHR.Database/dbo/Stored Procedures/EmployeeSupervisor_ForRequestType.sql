---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[EmployeeSupervisor_ForRequestType] 
	@EmpId  INT,
	@Role	TINYINT,
	@RequestType  TINYINT,
	@SupervisorIds VARCHAR(5000) = '',
	@ModifiedBy INT
AS
/*
Created: 2013/10/23@Quinn
Task: Update list supervisor for Employee with request type.
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
SET NOCOUNT ON
IF OBJECT_ID('tempdb..#SupervisorId_Type') IS NOT NULL
    DROP TABLE #SupervisorId_Type

CREATE TABLE #SupervisorId_Type (EmpId INT,OrderNo SmallINT IDENTITY(1, 1))
INSERT INTO #SupervisorId_Type (EmpId)
SELECT val FROM fnc_strSplit (@SupervisorIds, ',')

DELETE FROM [dbo].[EmployeeSupervisor] 
WHERE EmpId = @EmpId 
AND ActionType = @Role 
AND RequestType = @RequestType

INSERT INTO [dbo].[EmployeeSupervisor] (
	EmpId, 
	OwnerId, 
	ActionType, 
	RequestType, 
	ModifiedBy, 
	ModifiedDate,
	OrderNo)
SELECT @EmpId, EmpId, @Role, @RequestType, @ModifiedBy, GETDATE(),OrderNo
FROM #SupervisorId_Type
--GROUP BY EmpId,OrderNo

DROP TABLE #SupervisorId_Type


