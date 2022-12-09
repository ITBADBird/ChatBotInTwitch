CREATE TABLE [dbo].[Table]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY, 
    [DataTimeSET] DATETIME2 NULL, 
    [InForest] BIT NULL, 
    [Name] NCHAR(10) NULL, 
    [MoneyLeaves] FLOAT NULL, 
    [Sticks ] INT NULL
)
