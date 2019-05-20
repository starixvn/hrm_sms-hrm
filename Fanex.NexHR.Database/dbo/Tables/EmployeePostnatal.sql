CREATE TABLE [dbo].[EmployeePostnatal] (
    [PostnatalId]  INT            IDENTITY (1, 1) NOT NULL,
    [EmpId]        INT            NOT NULL,
    [FromDate]     DATETIME       NOT NULL,
    [ToDate]       DATETIME       NOT NULL,
    [CreatedBy]    INT            NOT NULL,
    [CreatedDate]  DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    [Remark]       NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_EmployeePostnatal] PRIMARY KEY CLUSTERED ([PostnatalId] ASC)
);



