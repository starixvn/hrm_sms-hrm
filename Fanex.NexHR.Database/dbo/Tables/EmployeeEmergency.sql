CREATE TABLE [dbo].[EmployeeEmergency] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [EmpId]        INT            NOT NULL,
    [FullName]     NVARCHAR (200) NOT NULL,
    [Phone]        NVARCHAR (50)  NOT NULL,
    [Relationship] NVARCHAR (50)  NULL,
    [DateOfBirth]  DATE           NULL,
    [Gender]       BIT            NOT NULL,
    [CreatedDate]  DATETIME       NOT NULL,
    [CreatedBy]    INT            NOT NULL,
    [ModifiedDate] DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    CONSTRAINT [PK_EmployeeEmergency] PRIMARY KEY NONCLUSTERED ([Id] ASC)
);

