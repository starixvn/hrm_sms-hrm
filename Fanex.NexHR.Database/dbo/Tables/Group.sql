CREATE TABLE [dbo].[Group] (
    [GroupId]           INT            IDENTITY (1, 1) NOT NULL,
    [Code]              VARCHAR (20)   NOT NULL,
    [Name]              NVARCHAR (150) NULL,
    [DepartmentId]      INT            NULL,
    [Rank]              INT            NULL,
    [CreatedDate]       DATETIME       NULL,
    [CreatedBy]         INT            NULL,
    [ModifiedDate]      DATETIME       NULL,
    [ModifiedBy]        INT            NULL,
    [IsDeleted]         BIT            CONSTRAINT [DF_Group_IsDeleted] DEFAULT ((0)) NULL,
    [MailGroup]         VARCHAR (50)   NULL,
    [BasicAnnualLeaveBalance] TINYINT NULL, 
    CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED ([GroupId] ASC) ON [Employee]
);

