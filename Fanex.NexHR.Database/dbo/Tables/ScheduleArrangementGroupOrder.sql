CREATE TABLE [dbo].[ScheduleArrangementGroupOrder] (
    [GroupId]      INT      NOT NULL,
    [DisplayOrder] SMALLINT NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    CONSTRAINT [PK_ScheduleArrangementGroupOrder] PRIMARY KEY CLUSTERED ([GroupId] ASC)
);

