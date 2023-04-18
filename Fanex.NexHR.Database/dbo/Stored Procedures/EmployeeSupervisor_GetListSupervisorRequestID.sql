
CREATE PROCEDURE [dbo].[EmployeeSupervisor_GetListSupervisorRequestID] @RequestId BIGINT
	,@Action INT = NULL
AS
/*
Created: TOM.VO@2013/10/25
Task: Get list supervisor for Employee by RequestID.
DB: HRM_SMS
Original:

Revisions:
	2023-03-20@Teddy.Le: get Letstalk
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	DECLARE @ApproverId INT
		,@VerifierId INT
		,@EmpId INT
		,@HRId INT
		,@TeamId INT
		,@IsExistSetting BIT

	SELECT @ApproverId = wr.ApproverId
		,@VerifierId = wr.VerifierId
		,@EmpId = wr.EmpId
		,@HRId = wr.HRId
	FROM WorkingTimeRequest AS wr WITH (NOLOCK)
	WHERE wr.RequestId = @RequestId

	SELECT 0 AS RoleId
		,'Member' AS [Role]
		,EmpId
		,FullName
		,NickName
		,Email
		,LetsTalk
	FROM dbo.Employee WITH (NOLOCK)
	WHERE EmpId = @EmpId
		AND [Enabled] = 1
		AND isDeleted = 0

	SELECT 2 AS RoleId
		,'Verifier' AS [Role]
		,EmpId
		,FullName
		,NickName
		,Email
		,LetsTalk
	FROM dbo.Employee WITH (NOLOCK)
	WHERE EmpId = @VerifierId
		AND [Enabled] = 1
		AND isDeleted = 0

	SELECT 3 AS RoleId
		,'Approver' AS [Role]
		,EmpId
		,FullName
		,NickName
		,Email
		,LetsTalk
	FROM dbo.Employee WITH (NOLOCK)
	WHERE EmpId = @ApproverId
		AND [Enabled] = 1
		AND isDeleted = 0

	SELECT 6 AS RoleId
		,'HR' AS [Role]
		,emp.EmpId
		,emp.FullName
		,emp.NickName
		,emp.Email
		,emp.LetsTalk
	FROM dbo.Employee AS emp WITH (NOLOCK)
		INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON emp.EmpId = er.EmployeeId
	WHERE emp.[Enabled] = 1
		AND emp.isDeleted = 0
		AND er.RoleId = 6
		AND NOT EXISTS (
			SELECT 1
			FROM dbo.EmployeeRole WITH (NOLOCK)
			WHERE emp.EmpId = EmployeeId
				AND RoleId = 7
			)

	SELECT @TeamId = GroupId
	FROM dbo.Employee WITH (NOLOCK)
	WHERE EmpId = @EmpId

	IF EXISTS (
			SELECT 1
			FROM dbo.MailSettings WITH (NOLOCK)
			WHERE GroupId = @TeamId
				AND [Action] = @Action
			)
	BEGIN
		SELECT GroupId
			,[Action]
			,AllowSendUser
			,AllowSendVerifier
			,AllowSendApprover
			,AllowSendHR
			,AllowSendMailGroup
		FROM dbo.MailSettings WITH (NOLOCK)
		WHERE GroupId = @TeamId
			AND [Action] = @Action
	END
	ELSE
	BEGIN
		SELECT GroupId
			,[Action]
			,AllowSendUser
			,AllowSendVerifier
			,AllowSendApprover
			,AllowSendHR
			,AllowSendMailGroup
		FROM dbo.MailSettings WITH (NOLOCK)
		WHERE GroupId = 0
			AND [Action] = @Action
	END
END

