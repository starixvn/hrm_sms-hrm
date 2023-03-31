CREATE TABLE [dbo].[EmployeeSupervisor] (
    [OwnerId]      INT          NOT NULL,
    [EmpId]        INT          NOT NULL,
    [ActionType]   TINYINT      NOT NULL,
    [RequestType]  TINYINT      NOT NULL,
    [ModifiedBy]   INT          NULL,
    [ModifiedDate] DATETIME     NULL,
    [OrderNo]      SMALLINT     CONSTRAINT [DF_EmployeeSupervisor_OrderNo] DEFAULT ((0)) NULL,
    [OrderCode]    VARCHAR (10) NULL,
    [IsDeleted]    BIT          NULL,
    CONSTRAINT [PK_EmployeeSupervisor] PRIMARY KEY CLUSTERED ([OwnerId] ASC, [EmpId] ASC, [ActionType] ASC, [RequestType] ASC) ON [Employee]
) ON [Employee];




GO
CREATE NONCLUSTERED INDEX [IDX_EmpId_ActionType_IsDeleted]
    ON [dbo].[EmployeeSupervisor]([EmpId] ASC, [ActionType] ASC, [IsDeleted] ASC)
    ON [Employee];

