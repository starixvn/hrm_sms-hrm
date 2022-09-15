CREATE TABLE [dbo].[Group] (
    [GroupId]           INT            IDENTITY (1, 1) NOT NULL,
    [Code]              VARCHAR (20)   NOT NULL,
    [Name]              NVARCHAR (150) NULL,
    [SectionId]      INT            NULL,
    [Rank]              INT            NULL,
    [CreatedDate]       DATETIME       NULL,
    [CreatedBy]         INT            NULL,
    [ModifiedDate]      DATETIME       NULL,
    [ModifiedBy]        INT            NULL,
    [IsDeleted]         BIT            CONSTRAINT [DF_Group_IsDeleted] DEFAULT ((0)) NULL,
    [MailGroup]         VARCHAR (50)   NULL,
    [LocalBasicAnnualLeaveBalance] TINYINT NULL, 
    [LocalWorkingDays] TINYINT NULL, 
    [LocalWorkingHours] TINYINT NULL, 
    [LocalOffDaysbyWeek] TINYINT NULL, 
    [ExpatBasicAnnualLeaveBalance] TINYINT NULL, 
    [ExpatWorkingDays] TINYINT NULL, 
    [ExpatWorkingHours] TINYINT NULL, 
    [ExpatOffDaysbyWeek] TINYINT NULL,
	[LocalWorkingHoursbyWeek] TINYINT NULL,
	[ExpatWorkingHoursbyWeek] TINYINT NULL
    CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED ([GroupId] ASC) ON [Employee]
);

