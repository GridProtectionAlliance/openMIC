-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW [dbo].[LocalSchemaVersion] AS
SELECT 2 AS VersionNumber
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConnectionProfileTaskQueue](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [Name] [varchar](200) NOT NULL,
    [MaxThreadCount] [int] NOT NULL CONSTRAINT [DF_ConnectionProfileTaskQueue_MaxThreadCount]  DEFAULT ((0)),
    [UseBackgroundThreads] [bit] NOT NULL CONSTRAINT [DF_ConnectionProfileTaskQueue_UseBackgroundThreads]  DEFAULT ((0)),
    [Description] [varchar](max) NULL,
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_ConnectionProfileTaskQueue_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_ConnectionProfileTaskQueue_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_ConnectionProfileTaskQueue_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_ConnectionProfileTaskQueue_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_ConnectionProfileTaskQueue] PRIMARY KEY CLUSTERED
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConnectionProfile](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [Name] [varchar](200) NOT NULL,
    [DefaultTaskQueueID] [int] NULL,
    [Description] [varchar](max) NULL,
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_ConnectionProfile_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_ConnectionProfile_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_ConnectionProfile_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_ConnectionProfile_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_ConnectionProfile] PRIMARY KEY CLUSTERED
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConnectionProfileTask](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [ConnectionProfileID] [int] NOT NULL CONSTRAINT [DF_ConnectionProfileTask_ConnectionProfileID] DEFAULT (0),
    [Name] [varchar](200) NOT NULL,
    [Settings] [varchar](max) NULL,
    [LoadOrder] [int] NOT NULL CONSTRAINT [DF_ConnectionProfileTask_LoadOrder]  DEFAULT ((0)),
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_ConnectionProfileTask_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_ConnectionProfileTask_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_ConnectionProfileTask_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_ConnectionProfileTask_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_ConnectionProfileTask] PRIMARY KEY CLUSTERED
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[DownloadedFile](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [DeviceID] [int] NOT NULL,
    [FilePath] [nvarchar](200) NOT NULL,
    [Timestamp] [datetime2] NOT NULL,
    [CreationTime] [datetime] NOT NULL,
    [LastWriteTime] [datetime] NOT NULL,
    [LastAccessTime] [datetime] NOT NULL,
    [FileSize] [int] NOT NULL,
 CONSTRAINT [PK_DownloadedFile] PRIMARY KEY CLUSTERED
(
    [ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[StatusLog](
      [ID] [int] IDENTITY(1,1) NOT NULL,
      [DeviceID] [int] NOT NULL,
      [LastDownloadedFileID] [int] NULL,
      [LastOutcome] [nvarchar](50) NULL,
      [LastRun] [DateTime2] NULL,
      [LastFailure] [DateTime2] NULL,
      [LastErrorMessage] [nvarchar](max) NULL,
      [LastDownloadStartTime] [DateTime2] NULL,
      [LastDownloadEndTime] [DateTime2] NULL,
      [LastDownloadFileCount] [int] NULL,
    CONSTRAINT [PK_StatusLog] PRIMARY KEY CLUSTERED
    (
        [ID] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[SentEmail](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [DeviceID] [int] NOT NULL,
    [Message] [nvarchar](max) NOT NULL,
    [Timestamp] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_SentEmail] PRIMARY KEY CLUSTERED
(
    [ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ConnectionProfile]  WITH CHECK ADD  CONSTRAINT [FK_ConnectionProfile_ConnectionProfileTaskQueue] FOREIGN KEY([DefaultTaskQueueID])
REFERENCES [dbo].[ConnectionProfileTaskQueue] ([ID])
GO

ALTER TABLE [dbo].[ConnectionProfileTask]  WITH CHECK ADD  CONSTRAINT [FK_ConnectionProfileTask_ConnectionProfile] FOREIGN KEY([ConnectionProfileID])
REFERENCES [dbo].[ConnectionProfile] ([ID])
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_StatusLog_DeviceID] ON [dbo].[StatusLog]
(
    [DeviceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_DownloadedFile_DeviceID] ON [dbo].[DownloadedFile]
(
    [DeviceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_DownloadedFile_FilePath] ON [dbo].[DownloadedFile]
(
    [FilePath] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_SentEmail_DeviceID] ON [dbo].[SentEmail]
(
    [DeviceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_SentEmail_Timestamp] ON [dbo].[SentEmail]
(
    [Timestamp] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO



-- This trigger can be used to get email notifications from the database on status log update
-- make a semicolon separated list of recipients in the @recipents field and set the db email service name in the @profile_name field
-- update if expressions to ensure emails are sent when desired.

-- This trigger will need to be updated before it will work with LocalSchemaVersion 2.

--CREATE TRIGGER [dbo].[StatusLog_Email]
--   ON  [dbo].[StatusLog]
--   AFTER UPDATE
--AS
--BEGIN

--	SET NOCOUNT ON;

--	DECLARE @html nvarchar(MAX);

--	SELECT * INTO #inserted FROM inserted

--	EXEC spQueryToHtmlTable @html = @html OUTPUT,  @query = N'SELECT * FROM #inserted';
--	DECLARE @recipients nvarchar(max) = N'bernest@gridprotectionalliance.org' -- SemiColon separated list of users to send to
--	DECLARE @profile_name nvarchar(max) ='Emails'   -- DB email service to use

--	DECLARE @deviceID int =  (Select TOP 1 DeviceID from inserted)
--	DECLARE @enabled bit = (SELECT [Enabled] FROM Device WHERE ID = @deviceID)

--	DECLARE @message nvarchar(MAX) = (SELECT TOP 1 Message FROM inserted)
--	DECLARE @name nvarchar(max) = (SELECT Name FROM Device WHERE ID = @deviceID)
--	DECLARE @lastFile nvarchar(max) = (SELECT TOP 1 LastFile FROM inserted)
--	DECLARE @downloadDate nvarchar(max) = (SELECT TOP 1 FileDownloadTimestamp FROM inserted)
--	DECLARE @lastSuccess DateTime = (SELECT LastSuccess FROM StatusLog WHERE DeviceID = @deviceID)
--	DECLARE @lastFailure DateTime = (SELECT LastFailure FROM StatusLog WHERE DeviceID = @deviceID)
--	DECLARE @fileSize int = (SELECT FileSize FROM DownloadedFile WHERE DeviceID = @deviceID)
--	DECLARE @fileDate DateTime
--	IF @lastFile IS NOT NULL
--	BEGIN
--		BEGIN TRY
--			SET @fileDate = '20' + (Select TOP 1 SUBSTRING(@lastFile, 1,2)) + '-' +
--									(Select TOP 1 SUBSTRING(@LastFile, 3,2)) + '-' +
-- 									(Select TOP 1 SUBSTRING(@LastFile, 5,2)) + ' ' +
--									(Select TOP 1 SUBSTRING(@LastFile, 8,2)) + ':' +
-- 									(Select TOP 1 SUBSTRING(@LastFile, 10,2)) + ':' +
-- 									(Select TOP 1 SUBSTRING(@LastFile, 12,2))
--		END TRY
--		BEGIN CATCH
--		 SET @fileDate = (SELECT GETDATE())
--		END CATCH
--	END

--	DECLARE @downloadDateDiff int = (SELECT DATEDIFF(HOUR, @fileDate, @downloadDate))
--	DECLARE @successDateDiff int = (SELECT DATEDIFF(HOUR, @lastSuccess, @lastFailure))
--	DECLARE @emailFlag bit = 0;
--	DECLARE @intro nvarchar(max) = N''
--	DECLARE @emailCountToday int = (SELECT COUNT(*) FROM SentEmail WHERE DeviceID = @deviceID AND Timestamp > CAST(GETDATE() as DATE))


--	IF @message = 'Disabled due to excessive file production.'  AND @enabled = 0 AND @emailCountToday = 0
--	BEGIN
--		SET @intro = @intro + N'<div>'+ @Name+' has been disabled due to excessive downloads.</div><br>'
--		SET @emailFlag = 1
--	END

--	IF @fileDate > GETDATE() AND @enabled = 1 AND @emailCountToday = 0
--	BEGIN
--		SET @intro = @intro + N'<div>'+ @Name+' has produced a record in the future.</div><br>'
--		SET @emailFlag = 1
--	END

--	IF @downloadDateDiff > 12 AND @enabled = 1 AND @emailCountToday = 0
--	BEGIN
--		SET @intro = @intro + N'<div>'+ @Name+' has taken '+ CAST(@downloadDateDiff as nvarchar(100)) +' hours to download a file.  The meter may require attention.</div><br>'
--		SET @emailFlag = 1
--	END

--	IF @successDateDiff > 24 AND @enabled = 1 AND @emailCountToday = 0
--	BEGIN
--		SET @intro = @intro + N'<div>'+ @Name+' has not had a successful connection in  '+ CAST(@successDateDiff as nvarchar(100)) +' hours.  The meter may require attention.</div><br>'
--		SET @emailFlag = 1
--	END

--	IF @fileSize > 1028*50 AND @enabled = 1 AND @emailCountToday = 0 -- email on greater than 50 MB
--	BEGIN
--		SET @intro = @intro + N'<div>'+ @Name+' has produced a record that is too large.</div><br>'
--		SET @emailFlag = 1
--	END


--	IF @emailFlag = 1
--	BEGIN
--		SET @html = @intro + @html;
--		DECLARE @subject nvarchar(max) = N'OpenMIC ' +@Name + N' problems ...'
--		EXEC msdb.dbo.sp_send_dbmail
--			@profile_name =	@profile_name,
--			@recipients=@recipients,
--			@subject = @subject,
--			@body = @html,
--			@body_format = 'HTML';

--		INSERT INTO SentEmail (DeviceID, [Message],[Timestamp]) VALUES ( @deviceID, @html, GETDATE())

--	END

--	DROP Table #inserted

--END

--GO
