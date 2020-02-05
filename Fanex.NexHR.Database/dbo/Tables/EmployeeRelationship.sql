CREATE TABLE [dbo].[EmployeeRelationship] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [EmpId]        INT            NOT NULL,
    [FullName]     NVARCHAR (200) NOT NULL,
    [Phone]        NVARCHAR (50)  NULL,
    [Relationship] NVARCHAR (50)  NULL,
    [DateOfBirth]  DATE           NULL,
    [Gender]       BIT            NOT NULL,
    [IsEmergency]  BIT            NOT NULL		DEFAULT 0,
    [CreatedDate]  DATETIME       NOT NULL,
    [CreatedBy]    INT            NOT NULL,
    [ModifiedDate] DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    CONSTRAINT [PK_EmployeeRelationship] PRIMARY KEY NONCLUSTERED ([Id] ASC)
);

