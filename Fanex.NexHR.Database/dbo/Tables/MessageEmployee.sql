CREATE TABLE [dbo].[MessageEmployee] (
    [MessageId]  INT          NOT NULL,
    [EmployeeId] INT          NOT NULL,
    [Email]      VARCHAR (50) NULL,
    [IsDisabled] BIT          NULL,
    [IsRead]     BIT          NULL,
    CONSTRAINT [PK_MessageEmployee] PRIMARY KEY CLUSTERED ([MessageId] ASC, [EmployeeId] ASC)
);

