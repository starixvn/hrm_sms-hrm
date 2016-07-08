CREATE PROCEDURE [dbo].[EmployeeSupervisor_Get_zo] @EmployeeId INT           = NULL,
                                                  @ActionType TINYINT       = NULL,
                                                  @GroupIds   NVARCHAR(MAX) = '',
                                                  @IsLocal    BIT           = NULL
AS

/*
Created: 20140717@Quinn.Vo
Task: Get list supervisors.
DB: SMS-HRM
Original: 

Revisions:  
Param's Explanation:
*/

     BEGIN
         SET NOCOUNT ON;
         CREATE TABLE #Group(GroupId INT);
         CREATE TABLE #EmployeeSupervisor(EmpId INT);
         CREATE TABLE #Employee
         (EmpId      INT,
          OwnerId    INT,
          ActionType TINYINT
         );
         IF ISNULL(@GroupIds, '') <> ''
             BEGIN
                 INSERT INTO #Group(GroupId)
                        SELECT val
                        FROM fnc_strSplit(@GroupIds, ',');
             END;
         INSERT INTO #Employee
                SELECT DISTINCT
                       su.EmpId,
                       su.OwnerId,
                       su.ActionType
                FROM dbo.EmployeeSupervisor AS su WITH (NOLOCK)
                     INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON su.OwnerId = e.EmpId
                                                                   AND ISNULL(e.isDeleted, 0) = 0
                                                                   AND e.[Enabled] = 1
                     INNER JOIN dbo.Employee AS e1 WITH (NOLOCK) ON su.EmpId = e1.EmpId
                                                                    AND ISNULL(e1.isDeleted, 0) = 0
                                                                    AND e1.[Enabled] = 1
                     INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e1.EmpId = es.EmpId
                     INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON e1.GroupId = gr.GroupId
                WHERE ISNULL(su.IsDeleted, 0) = 0
                      AND su.ActionType IN(2, 3, 8)
                     AND (@ActionType IS NULL
                          OR su.ActionType = @ActionType)
                     AND (@EmployeeId IS NULL
                          OR e1.EmpId = @EmployeeId)
                     AND (@IsLocal IS NULL
                          OR es.IsLocal = @IsLocal)
                     AND (ISNULL(@GroupIds, '') = ''
                          OR e1.GroupId IN
                         (
                             SELECT GroupId
                             FROM #Group
                         ));
         INSERT INTO #Employee
                SELECT DISTINCT
                       su.OwnerId,
                       su.EmpId,
                       su.ActionType
                FROM dbo.EmployeeSupervisor AS su WITH (NOLOCK)
                     INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON su.EmpId = e.EmpId
                                                                   AND ISNULL(e.isDeleted, 0) = 0
                                                                   AND e.[Enabled] = 1
                     INNER JOIN dbo.Employee AS e1 WITH (NOLOCK) ON su.OwnerId = e1.EmpId
                                                                    AND ISNULL(e1.isDeleted, 0) = 0
                                                                    AND e1.[Enabled] = 1
                     INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e1.EmpId = es.EmpId
                     INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON e1.GroupId = gr.GroupId
                WHERE ISNULL(su.IsDeleted, 0) = 0
                      AND su.ActionType IN(4, 5)
                     AND (@ActionType IS NULL
                          OR su.ActionType = @ActionType)
                     AND (@EmployeeId IS NULL
                          OR e1.EmpId = @EmployeeId)
                     AND (@IsLocal IS NULL
                          OR es.IsLocal = @IsLocal)
                     AND (ISNULL(@GroupIds, '') = ''
                          OR e1.GroupId IN
                         (
                             SELECT GroupId
                             FROM #Group
                         ));
         INSERT INTO #EmployeeSupervisor
                SELECT DISTINCT
                       e.EmpId
                FROM dbo.Employee AS e WITH (NOLOCK)
                     INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e.EmpId = es.EmpId
                     INNER JOIN dbo.EmployeeSupervisor AS su WITH (NOLOCK) ON e.EmpId = su.EmpId
                WHERE(@ActionType IS NULL
                      OR su.ActionType = @ActionType)
                     AND (@EmployeeId IS NULL
                          OR e.EmpId = @EmployeeId)
                     AND (@IsLocal IS NULL
                          OR es.IsLocal = @IsLocal)
                     AND (ISNULL(@GroupIds, '') = ''
                          OR e.GroupId IN
                         (
                             SELECT GroupId
                             FROM #Group
                         ))
                     AND e.[Enabled] = 1
                     AND ISNULL(e.IsDeleted, 0) = 0
                     AND ISNULL(su.IsDeleted, 0) = 0
                ORDER BY e.EmpId;
         SELECT DISTINCT
                e.EmpId,
                e.Code,
                e.NickName,
                e.FullName,
                g.GroupId AS SectionId,
                g.Code,
                g.[Name]
         FROM dbo.Employee AS e WITH (NOLOCK)
              INNER JOIN dbo.[Group] AS g WITH (NOLOCK) ON e.GroupId = g.GroupId
              INNER JOIN #EmployeeSupervisor AS es WITH (NOLOCK) ON es.EmpId = e.EmpId
         ORDER BY e.Code;
         SELECT emRo.EmployeeId,
                emRo.RoleId,
                loDa.Code AS RoleCode,
                loDa.Name AS RoleName
         FROM dbo.EmployeeRole AS emRo WITH (NOLOCK)
              INNER JOIN dbo.LookupData loDa WITH (NOLOCK) ON emRo.RoleId = loDa.Value
              INNER JOIN dbo.[Lookup] look WITH (NOLOCK) ON look.Id = loDa.LookupId
                                                            AND look.Code = 'SR' -- System Role
              INNER JOIN #EmployeeSupervisor e ON e.EmpId = emRo.EmployeeId;
         SELECT DISTINCT
                su.OwnerId,
                e.NickName,
                e.FullName,
                su.ActionType,
                su.EmpId,
                e1.NickName,
                e1.FullName,
                e1.Code
         FROM #Employee AS su
              INNER JOIN dbo.Employee AS e WITH (NOLOCK) ON su.OwnerId = e.EmpId
                                                            AND ISNULL(e.isDeleted, 0) = 0
                                                            AND e.[Enabled] = 1
              INNER JOIN dbo.Employee AS e1 WITH (NOLOCK) ON su.EmpId = e1.EmpId
                                                             AND ISNULL(e1.isDeleted, 0) = 0
                                                             AND e1.[Enabled] = 1
              INNER JOIN dbo.EmployeeSummary AS es WITH (NOLOCK) ON e1.EmpId = es.EmpId
              INNER JOIN dbo.[Group] AS gr WITH (NOLOCK) ON e1.GroupId = gr.GroupId
         ORDER BY su.ActionType,
                  e1.Code,
                  e.NickName;
         DROP TABLE #Employee;
         DROP TABLE #Group;
     END;