CREATE TABLE [dbo].[Manpower_AnnualRequestFormDetail_Log] (
    [RequestId]     INT           NOT NULL,
    [SiteId]        SMALLINT      NOT NULL,
    [SectionId]     INT           NOT NULL,
    [JobLevel]      NVARCHAR (50) NOT NULL,
    [SectionName]   NVARCHAR (50) NULL,
    [LocalAddition] SMALLINT      NOT NULL,
    [ExpatAddition] SMALLINT      NOT NULL,
    [ModifiedBy]    INT           NOT NULL,
    [ModifiedDate]  DATETIME      NOT NULL
);


GO
CREATE NONCLUSTERED INDEX [IX_tbl_AMRF_Detail_Log]
    ON [dbo].[Manpower_AnnualRequestFormDetail_Log]([RequestId] ASC, [SiteId] ASC, [SectionId] ASC, [JobLevel] ASC);

