---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[Overtime_GetRequestByRequestID] 
	@RequestID bigint
AS
/*
Created: Tom.Vo@2013-11-05
Task:Get Request By RequestID
DB: HRM_SMS
Original: 

Revisions:    
Param's Explanation:
*/
BEGIN
	SET NOCOUNT ON;

	SELECT 
				r.RequestId,
				r.empID,
				r.Month AS RequestDate,		
				r.CreatedDate,
				r.VerifiedDate,
				r.VerifiedStatus,
				r.ApprovedDate,
				r.ApprovedStatus,
				r.HRDate,
				r.HRStatus,
				r.[NormalHours],
			    r.[OffHours],
			    r.[PublicHours],
				(r.[NormalHours]+r.[OffHours]+r.[PublicHours]) ToTalHours,
				NULL Remark,				

				r.ModifiedBy ModifiedByID,
				e_m.NickName as NickNameModified,
				e_m.FullName as FullNameModified,
				r.ModifiedDate,
					
				e.EmpId AS RequesterID,
				e.NickName AS Requester,
				e.FullName AS FullNameRequester,
				
				g.GroupId as GroupId,
				g.Name AS Team,
				g.Code AS TeamCode,

				ap_V.EmpId AS VerifierID,
				ap_V.NickName as NickNameVerifier,
				ap_V.FullName as FullNameVerifierBy,

				ap_A.EmpId AS ApproverID,
				ap_A.NickName as NickNameApprover,
				ap_A.FullName as FullNameApproverBy,

				ap_H.EmpId AS HRID,
				ap_H.NickName as HRBy,
				ap_H.FullName as FullNameHRBy,	
				
				8 RequestTypeId,
				
				r.currentstatus AS StatusId		

	FROM  [dbo].[OTMonthlyRequest] r WITH (NOLOCK)
				LEFT JOIN [dbo].Employee ap_V WITH (NOLOCK) ON ap_V.empid = r.VerifierId
				LEFT JOIN [dbo].Employee ap_A WITH (NOLOCK) ON ap_A.empid = r.ApproverId
				LEFT JOIN [dbo].Employee ap_H WITH (NOLOCK) ON ap_H.empid = r.HRId
				LEFT JOIN Employee e_m WITH (NOLOCK) ON e_m.empID = r.ModifiedBy

				LEFT JOIN Employee e WITH (NOLOCK) ON e.empID = r.empID	
				
				LEFT JOIN [Group] g WITH (NOLOCK) ON e.GroupID = g.GroupID

	WHERE r.RequestId = @RequestID
END


