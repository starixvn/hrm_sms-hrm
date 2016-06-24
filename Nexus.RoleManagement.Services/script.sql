CREATE TABLE [dbo].[EmployeeRole]
([EmployeeId]   INT NOT NULL,
 [RoleId]       INT NOT NULL,
 [ModifiedBy]   INT NULL,
 [ModifiedDate] DATETIME NULL
);
GO
CREATE TABLE [dbo].[FunctionKey]
([FunctionId]       INT IDENTITY(1, 1)
                        NOT NULL,
 [FunctionKey]      VARCHAR(50) NOT NULL,
 [Name]             NVARCHAR(200) NOT NULL,
 [ParentFunctionId] INT NULL,
 [IsActive]         BIT NULL,
 CONSTRAINT [PK_FunctionKey] PRIMARY KEY CLUSTERED([FunctionId] ASC)
);
GO
CREATE TABLE [dbo].[Menu]
([MenuId]       INT IDENTITY(1, 1)
                    NOT NULL,
 [Name]         NVARCHAR(50) NOT NULL,
 [Text]         NVARCHAR(100) NULL,
 [Link]         NVARCHAR(100) NULL,
 [ParentId]     INT NULL,
 [FunctionId]   INT NULL,
 [DisplayOrder] INT NULL,
 [IsActive]     BIT NULL,
 CONSTRAINT [PK_Menu] PRIMARY KEY CLUSTERED([MenuId] ASC)
);
GO
CREATE TABLE [dbo].[RoleFunction]
([RoleId]       INT NOT NULL,
 [FunctionId]   INT NOT NULL,
 [IsRead]       BIT NOT NULL,
 [IsEdit]       BIT NOT NULL,
 [IsCreate]     BIT NOT NULL,
 [IsDelete]     BIT NOT NULL,
 [ModifiedBy]   INT NULL,
 [ModifiedDate] DATETIME NULL,
 CONSTRAINT [PK_RoleFunction] PRIMARY KEY CLUSTERED([RoleId] ASC, [FunctionId] ASC)
);
GO
IF NOT EXISTS
(
    SELECT *
    FROM sys.objects
    WHERE object_id = OBJECT_ID(N'[dbo].[Lookup]')
          AND type IN(N'U')
)
    CREATE TABLE [dbo].[Lookup]
    ([Id]   [INT] IDENTITY(1, 1)
                  NOT NULL,
     [Code] [VARCHAR](20) NULL,
     [Name] [VARCHAR](50) NULL,
     CONSTRAINT [PK_Lookup] PRIMARY KEY CLUSTERED([Id] ASC)
    );
GO
IF NOT EXISTS
(
    SELECT *
    FROM sys.objects
    WHERE object_id = OBJECT_ID(N'[dbo].[LookupData]')
          AND type IN(N'U')
)
    CREATE TABLE [dbo].[LookupData]
    ([Id]       [INT] IDENTITY(1, 1)
                      NOT NULL,
     [LookupId] [INT] NOT NULL,
     [Code]     [VARCHAR](20) NULL,
     [Name]     [NVARCHAR](100) NULL,
     [Value]    [VARCHAR](500) NULL,
     CONSTRAINT [PK_LookupData] PRIMARY KEY CLUSTERED([Id] ASC)
    );
GO
CREATE PROCEDURE [dbo].[Function_GetFunctions]
AS

/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:

*/

     BEGIN
         SET NOCOUNT ON;
         SELECT [FunctionId],
                [FunctionKey],
                [Name] AS Name,
                [ParentFunctionId],
                [IsActive]
         FROM dbo.[FunctionKey] WITH (NOLOCK)
         WHERE [IsActive] = 1;
     END;
GO
CREATE PROCEDURE [dbo].[Function_GetFunctionsByRoleId] @RoleId INT
AS

/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:

*/

     BEGIN
         SET NOCOUNT ON;
         SELECT fk.[FunctionId],
                fk.[FunctionKey],
                fk.[Name] AS FunctionName,
                fk.[ParentFunctionId],
                rf.[IsRead],
                rf.[IsEdit],
                rf.[IsCreate],
                rf.[IsDelete]
         FROM dbo.[FunctionKey] AS fk WITH (NOLOCK)
              LEFT JOIN
         (
             SELECT [FunctionId],
                    [IsRead],
                    [IsEdit],
                    [IsCreate],
                    [IsDelete]
             FROM dbo.[RoleFunction] WITH (NOLOCK)
             WHERE [RoleId] = @RoleId
         ) AS rf ON fk.[FunctionId] = rf.[FunctionId]
         WHERE fk.[IsActive] = 1;
     END;
GO
CREATE PROCEDURE [dbo].[Function_AssignFunctionToRole] @RoleId       INT,
                                                       @Functions    VARCHAR(5000) = '',
                                                       @ModifiedBy   INT           = NULL,
                                                       @ErrorCode    INT           = 0 OUTPUT,
                                                       @ErrorMessage VARCHAR(200)  = '' OUTPUT
AS

/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:
	@Functions = <ROOT><Function FunctionId='{0}' IsRead='{1}' IsEdit='{2}' IsCreate='{3}' IsDelete='{4}'></Function></ROOT>
*/

     BEGIN
         SET NOCOUNT ON;
         CREATE TABLE #Functions
         (FunctionId INT,
          IsRead     BIT,
          IsEdit     BIT,
          IsCreate   BIT,
          IsDelete   BIT
         );
         BEGIN TRY
             DECLARE @DocHandler INT;
             EXEC sp_xml_preparedocument
                  @DocHandler OUTPUT,
                  @Functions;
             INSERT INTO #Functions
             (FunctionId,
              IsRead,
              IsEdit,
              IsCreate,
              IsDelete
             )
                    SELECT FunctionId,
                           IsRead,
                           IsEdit,
                           IsCreate,
                           IsDelete
                    FROM OPENXML(@DocHandler, '/ROOT/Function', 1) WITH(FunctionId INT, IsRead BIT, IsEdit BIT, IsCreate BIT, IsDelete BIT) AS Data;
             EXEC sp_xml_removedocument
                  @DocHandler;
         END TRY
         BEGIN CATCH
             SELECT @ErrorCode = 1,
                    @ErrorMessage = N'Evaluate failed. Paramter invalid.';
             RETURN;
         END CATCH;
         DECLARE @ModifiedDate DATETIME;
         SET @ModifiedDate = GETDATE();
         BEGIN TRY
             BEGIN TRANSACTION;
             UPDATE rf
               SET
                   rf.IsRead = f.IsRead,
                   rf.IsEdit = f.IsEdit,
                   rf.IsCreate = f.IsCreate,
                   rf.IsDelete = f.IsDelete
             FROM dbo.[RoleFunction] AS rf WITH (ROWLOCK, UPDLOCK)
                  INNER JOIN #Functions AS f WITH (NOLOCK) ON rf.FunctionId = f.FunctionId
             WHERE rf.RoleId = @RoleId;
             DELETE f
             FROM #Functions AS f
             WHERE EXISTS
             (
                 SELECT 1
                 FROM dbo.RoleFunction AS rf WITH (NOLOCK)
                 WHERE rf.RoleId = @RoleId
                       AND rf.FunctionId = f.FunctionId
             );
             INSERT INTO dbo.[RoleFunction]
             ([RoleId],
              [FunctionId],
              [IsRead],
              [IsEdit],
              [IsCreate],
              [IsDelete],
              [ModifiedBy],
              [ModifiedDate]
             )
                    SELECT @RoleId,
                           FunctionId,
                           IsRead,
                           IsEdit,
                           IsCreate,
                           IsDelete,
                           @ModifiedBy,
                           @ModifiedDate
                    FROM #Functions;
             COMMIT TRANSACTION;
         END TRY
         BEGIN CATCH
             ROLLBACK TRANSACTION;
             SELECT @ErrorCode = 10,
                    @ErrorMessage = N'Assign function to role unsuccessfully.';
         END CATCH;
     END;
GO
CREATE PROCEDURE [dbo].[Menu_GetMenusByUserId] @UserId INT
AS

/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:
*/

     BEGIN
         SET NOCOUNT ON;
         SELECT DISTINCT
                me.MenuId,
                me.[Name],
                me.[Text],
                me.Link,
                me.ParentId,
                me.FunctionId,
                me.DisplayOrder,
                me.IsActive
         FROM dbo.EmployeeRole AS empRo WITH (NOLOCK)
              INNER JOIN dbo.RoleFunction AS roFunc WITH (NOLOCK) ON empRo.RoleId = roFunc.RoleId
                                                                     AND roFunc.IsRead = 1
              INNER JOIN dbo.Menu AS me WITH (NOLOCK) ON roFunc.FunctionId = me.FunctionId
                                                         AND me.IsActive = 1
         WHERE empRo.EmployeeId = @UserId;
     END;
GO
CREATE PROCEDURE [dbo].[Menu_GetMenusById] @MenuId INT
AS

/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:
*/

     BEGIN
         SET NOCOUNT ON;
         SELECT DISTINCT
                me.MenuId,
                me.[Name],
                me.[Text],
                me.Link,
                me.ParentId,
                me.FunctionId,
                me.DisplayOrder,
                me.IsActive
         FROM dbo.Menu AS me WITH (NOLOCK)
         WHERE me.MenuId = @MenuId;
     END;
GO
CREATE PROCEDURE [dbo].[Menu_GetMenus]
AS

/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:
*/

     BEGIN
         SET NOCOUNT ON;
         SELECT DISTINCT
                me.MenuId,
                me.[Name],
                me.[Text],
                me.Link,
                me.ParentId,
                me.FunctionId,
                me.DisplayOrder,
                me.IsActive
         FROM dbo.Menu AS me WITH (NOLOCK);
     END;
GO
CREATE PROCEDURE [dbo].[Menu_Delete] @MenuId INT
AS

/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:
*/

     BEGIN
         SET NOCOUNT ON;
         DELETE FROM [dbo].[Menu] WHERE [MenuId] = @MenuId
     END;
GO
CREATE PROCEDURE [dbo].[Menu_Update] @MenuId       INT,
                                     @Name         NVARCHAR(50),
                                     @Text         NVARCHAR(100),
                                     @Link         NVARCHAR(100),
                                     @ParentId     INT,
                                     @FunctionId   INT,
                                     @DisplayOrder INT,
                                     @IsActive     BIT
AS

/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:
*/

     BEGIN
         SET NOCOUNT ON;
         UPDATE [dbo].[Menu]
           SET
               [Name] = @Name,
               [Text] = @Text,
               [Link] = @Link,
               [ParentId] = @ParentId,
               [FunctionId] = @FunctionId,
               [DisplayOrder] = @DisplayOrder,
               [IsActive] = @IsActive
         WHERE MenuId = @MenuId;
     END;
GO
CREATE PROCEDURE [dbo].[Menu_Insert] @Name         NVARCHAR(50),
                                     @Text         NVARCHAR(100),
                                     @Link         NVARCHAR(100),
                                     @ParentId     INT,
                                     @FunctionId   INT,
                                     @DisplayOrder INT
AS

/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:
*/

     BEGIN
         SET NOCOUNT ON;
         INSERT INTO [dbo].[Menu]
         VALUES
         (@Name,
          @Text,
          @Link,
          @ParentId,
          @FunctionId,
          @DisplayOrder,
          1
         );
     END;
GO
CREATE PROCEDURE [dbo].[Role_GetRolesByUserId] @UserId INT
AS

/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:
*/

     BEGIN
         SET NOCOUNT ON;
         SELECT emRo.RoleId,
                loDa.Code AS RoleCode,
                loDa.Name AS RoleName
         FROM dbo.EmployeeRole AS emRo WITH (NOLOCK)
              INNER JOIN dbo.LookupData loDa WITH (NOLOCK) ON emRo.RoleId = loDa.Value
              INNER JOIN dbo.[Lookup] look WITH (NOLOCK) ON look.Id = loDa.LookupId
         WHERE emRO.EmployeeId = @UserId
               AND look.Code = 'SR';-- System Role
     END;
GO
CREATE PROCEDURE [dbo].[Role_GetRoles]
AS

/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:
*/

     BEGIN
         SET NOCOUNT ON;
         SELECT loDa.Value AS RoleId,
                loDa.NAME AS RoleName,
                loDa.Code AS RoleCode
         FROM dbo.[Lookup] AS look WITH (NOLOCK)
              INNER JOIN dbo.LookupData loDa WITH (NOLOCK) ON loDa.LookupId = look.Id
         WHERE look.Code = 'SR'; -- System Role
     END;
GO
CREATE PROCEDURE [dbo].[Function_GetFunctionsByUserId] @UserId INT
AS

/*
Created: 20160609@Razor.Nguyen
Task: Create Role Management Services
DB: SPM
Original: 

Revisions:  
Param's Explanation:
*/

     BEGIN
         SET NOCOUNT ON;
         SELECT f.FunctionId,
                f.FunctionKey,
                CAST(MAX(IsRead) AS BIT) AS IsRead,
                CAST(MAX(IsEdit) AS BIT) AS IsEdit,
                CAST(MAX(IsCreate) AS BIT) AS IsCreate,
                CAST(MAX(IsDelete) AS BIT) AS IsDelete
         FROM
         (
             SELECT RoleId,
                    FunctionId,
                    CAST(IsRead AS INT) AS IsRead,
                    CAST(IsEdit AS INT) AS IsEdit,
                    CAST(IsCreate AS INT) AS IsCreate,
                    CAST(IsDelete AS INT) AS IsDelete
             FROM dbo.RoleFunction WITH (NOLOCK)
         ) AS rf
         INNER JOIN dbo.FunctionKey AS f WITH (NOLOCK) ON rf.FunctionId = f.FunctionId
         INNER JOIN dbo.EmployeeRole AS er WITH (NOLOCK) ON rf.RoleId = er.RoleId
         WHERE er.EmployeeId = @UserId
         GROUP BY f.FunctionId,
                  FunctionKey;
     END;
GO
DECLARE @id INT;
INSERT INTO [dbo].[Lookup]
VALUES
('SR',
 'System Role'
);
SELECT @id = CAST(SCOPE_IDENTITY() AS INT);
INSERT INTO [dbo].[LookupData]
VALUES
(@id,
 'Member',
 'Member',
 1
);
INSERT INTO [dbo].[LookupData]
VALUES
(@id,
 'Verifier',
 'Verifier',
 2
);
INSERT INTO [dbo].[LookupData]
VALUES
(@id,
 'Approver',
 'Approver',
 3
);
INSERT INTO [dbo].[LookupData]
VALUES
(@id,
 'ShiftManager',
 'Shift Manager',
 4
);
INSERT INTO [dbo].[LookupData]
VALUES
(@id,
 'ActingSupervisor',
 'On Behalf Function',
 5
);
INSERT INTO [dbo].[LookupData]
VALUES
(@id,
 'HR',
 'HR',
 6
);
INSERT INTO [dbo].[LookupData]
VALUES
(@id,
 'SystemAdmin',
 'System Admin',
 7
);
INSERT INTO [dbo].[LookupData]
VALUES
(@id,
 'DisciplineManager',
 'Discipline Manager',
 8
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('Memo',
 'Memo',
 NULL,
 1
);

INSERT INTO [dbo].[FunctionKey]
VALUES
('Discipline',
 'Discipline',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('Message',
 'Message',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('JobTitleManagement',
 'Job Title Management',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('SectionManagement',
 'Section Management',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('EmployeeManagement',
 'Employee Management',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('DivisionManagement',
 'Division Management',
 NULL,
 1
);

INSERT INTO [dbo].[FunctionKey]
VALUES
('MenuManagement',
 'Menu Management',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('FunctionManagement',
 'Function Management',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('Holiday',
 'Holiday',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('ExtensionNumber',
 'Extension Number',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('TimeRecord',
 'Time Record',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('AnnualLeaveBalance',
 'Annual Leave Balance',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('ShiftManagement',
 'Shift Management',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('ReportHome',
 'Report Home',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('FilterUnpaidLeaveSummary',
 'Filter Unpaid Leave Summary',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('CanOnBehalfOthers',
 'Can On Behalf Others',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('VerifyRequestOfMember',
 'Verify Request Of Member',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('ApproveRequestOfMember',
 'Approve Request Of Member',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('VerifyRequestOfEmployee',
 'Verify Request Of Employee',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('ApproveRequestOfEmployee',
 'Approve Request Of Employee',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('VerifyOTMember',
 'Verify OT Member',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('ApproveOTMember',
 'Approve OT Member',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('VerifyOTEmployee',
 'Verify OT Employee',
 NULL,
 1
);
INSERT INTO [dbo].[FunctionKey]
VALUES
('ApproveOTEmployee',
 'Approve OT Employee',
 NULL,
 1
);

INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 1,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 2,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 3,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 4,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 5,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 6,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 7,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 8,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 9,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 10,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 11,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 12,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 13,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 14,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 15,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 16,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 17,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 18,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 19,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 20,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 21,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 22,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 23,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 24,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);
INSERT INTO [dbo].[RoleFunction]
VALUES
(7,
 25,
 1,
 1,
 1,
 1,
 NULL,
 GETDATE()
);

INSERT INTO [dbo].[Menu]
VALUES
('Personal',
 'Personal',
 NULL,
 NULL,
 NULL,
 1,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('HrSpace',
 'Hr Space',
 NULL,
 NULL,
 NULL,
 2,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('CompanySpace',
 'Company Space',
 NULL,
 NULL,
 NULL,
 3,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('SaSpace',
 'Sa Space',
 NULL,
 NULL,
 NULL,
 4,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('Mydashboard',
 'My dashboard',
 '/',
 1,
 NULL,
 1,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('LeaveRequest',
 'Leave request',
 '/leave/employeeview',
 1,
 NULL,
 5,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('ChangeShift',
 'Change shift',
 '/changeshift/employeeview',
 1,
 NULL,
 10,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('LateInEarlyOut',
 'Late in - Early out',
 '/lateinearlyout/employeeview',
 1,
 NULL,
 15,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('CheckInCheckOut',
 'Check in - Check out',
 '/checkinout/employeeview',
 1,
 NULL,
 20,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('KeepInView',
 'Keep in view',
 '/keepinview/employeeview',
 1,
 NULL,
 25,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('MyOvertime',
 'My overtime',
 '/overtime/employeeview',
 1,
 NULL,
 30,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('MyTimesheet',
 'My timesheet',
 '/report/timesheet',
 1,
 NULL,
 35,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('MyCalendar',
 'My calendar',
 '/calendar/myworkingschedule',
 1,
 NULL,
 40,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('MembersRequest',
 'Members''s request',
 '/request/member',
 1,
 18,
 45,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('MembersOvertime',
 'Members''s overtime',
 '/overtime/memberovertime',
 1,
 22,
 50,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('DisciplineSummary',
 'Discipline summary',
 '/discipline/disciplinesummary',
 1,
 2,
 55,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('RequestsStatistic',
 'Requests statistic',
 '/report/leaderrequestmonitoring',
 1,
 18,
 60,
 1
);



INSERT INTO [dbo].[Menu]
VALUES
('MembersRequest',
 'Members''s request',
 '/request/member',
 1,
 19,
 45,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('MembersOvertime',
 'Members''s overtime',
 '/overtime/memberovertime',
 1,
 23,
 50,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('RequestsStatistic',
 'Requests statistic',
 '/report/leaderrequestmonitoring',
 1,
 19,
 60,
 1
);


INSERT INTO [dbo].[Menu]
VALUES
('ScheduleArrange',
 'Schedule arrange',
 '/calendar/arrangeschedule',
 1,
 14,
 65,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('ShiftStatistic',
 'Shift statistic',
 '/calendar/shiftstatistic',
 1,
 14,
 70,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('MyProfile',
 'My profile',
 '/account/index',
 1,
 NULL,
 75,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('MyMessage',
 'My message',
 '/message/viewmessageofemployee',
 1,
 NULL,
 80,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('EmployeeRequest',
 'Employee''s request',
 '/request/hrview',
 2,
 20,
 1,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('EmployeeOvertime',
 'Employee''s overtime',
 '/overtime/employeeovertime',
 2,
 24,
 5,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('RequestStatistic',
 'Request statistic',
 '/report/hrrequestmonitoring',
 2,
 24,
 15,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('UserManagement',
 'User management',
 '/employee/usermanagement',
 2,
 6,
 20,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('SectionManagement',
 'Section management',
 '/team/index',
 2,
 5,
 25,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('JobTitleManagement',
 'Job title management',
 '/jobtitle/jobtitlemanagement',
 2,
 4,
 30,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('TimeRecord',
 'Time record',
 '/report/timerecord',
 2,
 12,
 35,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('Reports',
 'Reports',
 '/report/index',
 2,
 15,
 40,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('Message',
 'Message',
 '/message/index',
 2,
 3,
 45,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('ContactList',
 'Contact list',
 '/employee/index',
 3,
 NULL,
 1,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('WorkingCalendars',
 'Working calendars',
 '/calendar/workingcalendars',
 3,
 NULL,
 5,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('DailyRequest',
 'Daily request',
 '/request/dailyview',
 3,
 NULL,
 15,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('AttendanceReport',
 'Attendance report',
 '/report/attendance',
 3,
 NULL,
 20,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('AnnualLeaveBalance',
 'Annual leave balance',
 '/annualleavebalance/index',
 3,
 NULL,
 25,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('UnpaidLeave',
 'Unpaid leave',
 '/report/unpaidleavesummary',
 3,
 NULL,
 30,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('ExtensionNumber',
 'Extension number',
 '/extensionnumber/index',
 3,
 NULL,
 35,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('CompanyMemo',
 'Company memo',
 '/memo/index',
 3,
 NULL,
 40,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('CreateAccount',
 'Create account',
 '/employee/create',
 4,
 6,
 1,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('VoidRequest',
 'Void request',
 '/request/voidrequest',
 4,
 20,
 5,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('Division',
 'Division',
 '/department/index',
 4,
 7,
 10,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('FunctionManagement',
 'Function management',
 '/function/index',
 4,
 9,
 15,
 1
);
INSERT INTO [dbo].[Menu]
VALUES
('MenuManagement',
 'Menu management',
 '/menu/index',
 4,
 8,
 20,
 1
);
--INSERT INTO employeerole from employee table
CREATE TABLE #Employee
(EmpId        INT,
 Roles        VARCHAR(20),
 ModifiedBy   INT,
 ModifiedDate DATETIME
);
INSERT INTO #Employee
       SELECT EmpId,
              Roles,
              ModifiedBy,
              ModifiedDate
       FROM dbo.Employee WITH (NOLOCK);
DECLARE @EmpId INT, @ModifiedBy INT, @ModifiedDate DATETIME, @RoleIds VARCHAR(20);
WHILE(EXISTS
     (
         SELECT *
         FROM #Employee
     ))
    BEGIN
        SELECT TOP 1 @EmpId = EmpId,
                     @ModifiedBy = ModifiedBy,
                     @ModifiedDate = ModifiedDate,
                     @RoleIds = Roles
        FROM #Employee;
        INSERT INTO [dbo].[EmployeeRole]
        ([EmployeeId],
         [RoleId],
         [ModifiedBy],
         [ModifiedDate]
        )
               SELECT DISTINCT
                      @EmpId,
                      val,
                      @ModifiedBy,
                      @ModifiedDate
               FROM fnc_strSplit(@RoleIds, ',');
        DELETE FROM #Employee
        WHERE EmpId = @EmpId;
    END;
DROP TABLE #Employee;