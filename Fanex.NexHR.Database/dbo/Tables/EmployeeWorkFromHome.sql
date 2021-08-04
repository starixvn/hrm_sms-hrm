CREATE TABLE [dbo].[EmployeeWorkFromHome] (
    [EmployeeWorkFromHomeId] INT            IDENTITY (1, 1) NOT NULL,
    [EmpId]                  INT            NOT NULL,
    [FromDate]               DATE           NOT NULL,
    [ToDate]                 DATE           NOT NULL,
    [Remark]                 NVARCHAR (500) NULL,
    [ModifiedBy]             INT            NULL,
    [ModifiedDate]           DATETIME       NULL,
    CONSTRAINT [PK_EmployeeWorkFromHome] PRIMARY KEY CLUSTERED ([EmployeeWorkFromHomeId] ASC)
);

