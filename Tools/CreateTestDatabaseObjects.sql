SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[ErrorRaisingForExecuteNonQuery]
  @rowId uniqueidentifier,
  @maxCountToRaiseErrors int,
  @error int
AS
BEGIN
  Declare @rowC int
  Set @rowC = 1
  Declare @retryCount int
  Set @retryCount = 0

  Select  @rowC = count(*)  From RetryCountTracker Where rowId = @rowId
  if (@rowC <= 0)
  BEGIN
    Insert Into RetryCountTracker (rowId, retryCount) Values (@rowId, 1);
  END 
  ELSE
  BEGIN
    Select @retryCount = retryCount From RetryCountTracker Where rowId = @rowId
    Set @retryCount = @retryCount + 1
    Update retryCountTracker Set retryCount = @retryCount Where rowId = @rowId;
  END
  if (@retryCount < @maxCountToRaiseErrors)
  BEGIN
    RAISERROR(@error, 10, 1, N'abcde');
  END
  ELSE
  BEGIN
    Select * From (Values (1),(2),(3),(4),(5),(6),(7),(8),(9),(10)) As StubTable(StubColumn);
  END
END
GO


CREATE PROCEDURE [dbo].[ErrorRaisingReader]
  @rowId uniqueidentifier,
  @maxCountToRaiseErrors int,
  @error int
AS
BEGIN
  Declare @rowC int
  Set @rowC = 1
  Declare @retryCount int
  Set @retryCount = 0

  Select  @rowC = count(*)  From RetryCountTracker Where rowId = @rowId
  if (@rowC <= 0)
  BEGIN
    Insert Into RetryCountTracker (rowId, retryCount) Values (@rowId, 1);
  END 
  ELSE
  BEGIN
    Select @retryCount = retryCount From RetryCountTracker Where rowId = @rowId
    Set @retryCount = @retryCount + 1
    Update retryCountTracker Set retryCount = @retryCount Where rowId = @rowId;
  END
  if (@retryCount < @maxCountToRaiseErrors)
  BEGIN
    RAISERROR(@error, 10, 1, N'abcde');
  END
  ELSE
  BEGIN
    Select * From (Values (1),(2),(3),(4),(5),(6),(7),(8),(9),(10)) As StubTable(StubColumn);
  END
END
GO


CREATE PROCEDURE [dbo].[ErrorRaisingScalar]
  @rowId uniqueidentifier,
  @maxCountToRaiseErrors int,
  @error int
AS
BEGIN
  Declare @rowC int
  Set @rowC = 1
  Declare @retryCount int
  Set @retryCount = 0

  Select  @rowC = count(*)  From RetryCountTracker Where rowId = @rowId
  if (@rowC <= 0)
  BEGIN
    Insert Into RetryCountTracker (rowId, retryCount) Values (@rowId, 1);
  END 
  ELSE
  BEGIN
    Select @retryCount = retryCount From RetryCountTracker Where rowId = @rowId
    Set @retryCount = @retryCount + 1
    Update retryCountTracker Set retryCount = @retryCount Where rowId = @rowId;
  END
  if (@retryCount < @maxCountToRaiseErrors)
  BEGIN
    RAISERROR(@error, 10, 1, N'abcde');
  END
  ELSE
  BEGIN
    Select Count(*) From (Values (1),(2),(3),(4),(5),(6),(7),(8),(9),(10)) As StubTable(StubColumn);
  END
END
GO


CREATE PROCEDURE [dbo].[ErrorRaisingXMLReader]
  @rowId uniqueidentifier,
  @maxCountToRaiseErrors int,
  @error int
AS
BEGIN
  Declare @rowC int
  Set @rowC = 1
  Declare @retryCount int
  Set @retryCount = 0

  Select  @rowC = count(*)  From RetryCountTracker Where rowId = @rowId
  if (@rowC <= 0)
  BEGIN
    Insert Into RetryCountTracker (rowId, retryCount) Values (@rowId, 1);
  END 
  ELSE
  BEGIN
    Select @retryCount = retryCount From RetryCountTracker Where rowId = @rowId
    Set @retryCount = @retryCount + 1
    Update retryCountTracker Set retryCount = @retryCount Where rowId = @rowId;
  END
  if (@retryCount < @maxCountToRaiseErrors)
  BEGIN
    RAISERROR(@error, 10, 1, N'abcde');
  END
  ELSE
  BEGIN
    Select * From (Values (1),(2),(3),(4),(5),(6),(7),(8),(9),(10)) As StubTable(StubColumn) FOR XML Auto;
  END
END
GO


CREATE TABLE [dbo].[ExecuteNonQuerytracker](
	[rowId] [uniqueidentifier] NOT NULL,
	[retryCount] [int] NOT NULL,
 CONSTRAINT [PK_ExecuteNonQuerytracker] PRIMARY KEY CLUSTERED 
(
	[rowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO


CREATE TABLE [dbo].[RetryCountTracker](
	[rowId] [uniqueidentifier] NOT NULL,
	[retryCount] [int] NOT NULL,
 CONSTRAINT [PK_RetryCountTracker] PRIMARY KEY CLUSTERED 
(
	[rowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO


CREATE TABLE [dbo].[TranscationScopeTestTable](
	[rowId] [uniqueidentifier] NOT NULL
 CONSTRAINT [PK_TranscationScopeTestTable] PRIMARY KEY CLUSTERED 
(
	[rowId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO