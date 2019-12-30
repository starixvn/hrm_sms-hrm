CREATE TABLE [dbo].[EmployeePostnatal] (
    [PostnatalId]  INT            IDENTITY (1, 1) NOT NULL,
    [EmpId]        INT            NOT NULL,
    [FromDate]     DATE       NOT NULL,
    [ToDate]       DATE       NOT NULL,
    [WorkingHours] TINYINT NULL, 
    [NormalWorkingHours] TINYINT NULL, 
    [Remark]       NVARCHAR (500) NULL,
    [CreatedBy]    INT            NOT NULL,
    [CreatedDate]  DATETIME       NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_EmployeePostnatal] PRIMARY KEY CLUSTERED ([PostnatalId] ASC)
);



