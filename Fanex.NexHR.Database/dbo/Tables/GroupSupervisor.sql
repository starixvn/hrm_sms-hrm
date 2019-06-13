CREATE TABLE [dbo].[GroupSupervisor] (
    [OwnerId]      INT      NOT NULL,
    [GroupId]      INT      NOT NULL,
    [ActionType]   TINYINT  NOT NULL,
    [RequestType]  TINYINT  NOT NULL,
    [ModifiedBy]   INT      NULL,
    [ModifiedDate] DATETIME NULL,
    [IsDeleted]    BIT      NULL,
    [DisplayOrder] SMALLINT NULL, 
    CONSTRAINT [PK_GroupSupervisor] PRIMARY KEY CLUSTERED ([OwnerId] ASC, [GroupId] ASC, [ActionType] ASC, [RequestType] ASC) ON [Employee]
);

