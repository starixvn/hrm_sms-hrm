CREATE TABLE [dbo].[Department] (
    [DepartmentId] INT            IDENTITY (1, 1) NOT NULL,
    [Code]         VARCHAR (50)   NOT NULL,
    [Name]         NVARCHAR (200) NOT NULL,
    [UnitId]       INT            NULL,
    [IsActive]     BIT            NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    [IsDeleted]    BIT            NULL,
    CONSTRAINT [PK_Department] PRIMARY KEY CLUSTERED ([DepartmentId] ASC)
);


GO