CREATE TABLE [dbo].[ScheduleArrangement] (
    [EmployeeId]   INT            NOT NULL,
    [Remark]       NVARCHAR (500) NULL,
    [DisplayOrder] INT            NULL,
    [ModifiedBy]   INT            NULL,
    [ModifiedDate] DATETIME       NULL,
    CONSTRAINT [PK_ScheduleArrangement] PRIMARY KEY CLUSTERED ([EmployeeId] ASC)
);

