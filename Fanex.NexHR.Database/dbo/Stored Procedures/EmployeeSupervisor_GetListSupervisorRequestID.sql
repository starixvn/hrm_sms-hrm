
CREATE PROCEDURE [dbo].[EmployeeSupervisor_GetListSupervisorRequestID]
	@RequestId  BIGINT,
	@Action INT = NULL
AS
/*
Created: TOM.VO@2013/10/25
Task: Get list supervisor for Employee by RequestID.
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
SET NOCOUNT ON
	DECLARE @ApproverId  INT, @VerifierId INT, @EmpId INT, @HRId INT,  @TeamId INT, @IsExistSetting BIT

	SELECT @ApproverId =  wr.ApproverId  , @VerifierId = wr.VerifierId ,@EmpId= wr.EmpId,@HRId = wr.HRId
	FROM WorkingTimeRequest wr WITH(NOLOCK)
	WHERE wr.RequestId = @RequestId 

	IF @EmpId IS NULL
		SELECT TOP 0  0 RoleID,'Member'  [Role] ,''  FullName, '' NickName, '' Email  
	ELSE
		SELECT TOP 1  0 RoleID,'Member'  [Role] ,  FullName, NickName, Email  
		FROM [dbo].Employee ee  WITH(NOLOCK) 
		WHERE ee.EmpId = @EmpId
			AND ISNULL(ResignedDate,GETDATE()+30)>GETDATE()

	IF @VerifierId IS NULL
		SELECT TOP 0  0 RoleID,'Verifier'  [Role] ,''  FullName, '' NickName, '' Email  
	ELSE
		SELECT TOP 1  2 RoleID,'Verifier'  [Role] ,  FullName, NickName, Email  
		FROM [dbo].Employee ee  WITH(NOLOCK) 
		WHERE ee.EmpId = @VerifierId
			AND ISNULL(ResignedDate,GETDATE()+30)>GETDATE()

	IF @ApproverId IS NULL
		SELECT TOP 0  0 RoleID,'Approver'  [Role] ,''  FullName, '' NickName, '' Email  
	ELSE
		SELECT TOP 1  3 RoleID,'Approver'  [Role] ,  FullName, NickName, Email  
		FROM [dbo].Employee ee  WITH(NOLOCK) 
		WHERE ee.EmpId = @ApproverId
			AND ISNULL(ResignedDate,GETDATE()+30)>GETDATE()
	
	IF @HRId IS NOT NULL
		SELECT TOP 1  6 RoleID,'HR'  [Role] ,  FullName, NickName, Email  
			FROM [dbo].Employee ee  WITH(NOLOCK) 
			WHERE ee.EmpId = @HRId
	ELSE
		SELECT TOP 10  6 RoleID,'HR'  [Role],  FullName, NickName, Email  
			FROM [dbo].Employee ee  WITH(NOLOCK) 
			INNER JOIN dbo.EmployeeRole AS er WITH(NOLOCK) ON ee.EmpId=er.EmployeeId
			WHERE (ee.isDeleted IS NULL OR ee.isDeleted = 0)
				AND ee.Enabled = 1 AND  er.RoleId = 6
				AND NOT EXISTS (SELECT 1 FROM  dbo.EmployeeRole WHERE ee.EmpId = EmployeeId
					AND RoleId = 7
					)
	
	SELECT  @TeamId = GroupId FROM dbo.Employee WHERE EmpId = @EmpId

	IF EXISTS (SELECT 1 FROM dbo.MailSettings WHERE GroupId =  @TeamId AND Action = @Action)
	BEGIN
		SELECT GroupId, Action, AllowSendUser, AllowSendVerifier, AllowSendApprover, AllowSendHR, AllowSendMailGroup
		FROM dbo.MailSettings WHERE GroupId =  @TeamId AND Action = @Action
	END
	ELSE
	BEGIN
		SELECT GroupId, Action, AllowSendUser, AllowSendVerifier, AllowSendApprover, AllowSendHR, AllowSendMailGroup
		FROM dbo.MailSettings WHERE GroupId = 0 AND Action = @Action
	END

