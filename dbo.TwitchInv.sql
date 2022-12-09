CREATE TABLE [dbo].[TwitchInv] (
    [NickName]    NVARCHAR (50) NULL,
    [DataTimeSet] DATETIME2 (7) NULL,
    [inForest]    BIT           NULL,
    [Leaves]      FLOAT (53)    NULL,
    [Sticks]      INT           NULL,
    [Clay]        INT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

