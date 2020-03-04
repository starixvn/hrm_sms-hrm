CREATE TABLE [dbo].[EmployeePaternity] (
    [PaternityId]   INT            IDENTITY (1, 1) NOT NULL,
    [EmpId]         INT            NOT NULL,
    [ChildBornDate] DATE       NOT NULL,
    [CreatedBy]     INT            NOT NULL,
    [CreatedDate]   DATETIME       NOT NULL,
    [ModifiedBy]    INT            NULL,
    [ModifiedDate]  DATETIME       NULL,
    [LeaveDay]      INT            NOT NULL,
    [Remark]        NVARCHAR (MAX) NULL,
    [BalanceDay] INT NOT NULL, 
    CONSTRAINT [PK_EmployeePaternity] PRIMARY KEY CLUSTERED ([PaternityId] ASC)
);

