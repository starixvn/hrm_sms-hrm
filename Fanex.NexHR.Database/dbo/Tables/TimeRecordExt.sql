CREATE TABLE [dbo].[TimeRecordExt] (
    [ID]        INT           NOT NULL,
    [IPAddress] VARCHAR (50)  NULL,
    [Platform]  VARCHAR (255) NULL,
    CONSTRAINT [PK_TimeRecordExt] PRIMARY KEY CLUSTERED ([ID] ASC)
);



