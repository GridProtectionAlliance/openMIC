-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW [dbo].[LocalSchemaVersion] AS
SELECT 6 AS VersionNumber
GO

CREATE TABLE Setting
(
    ID INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
    Name VARCHAR(200) NULL,
    Value VARCHAR(MAX) NULL,
    DefaultValue VARCHAR(MAX) NULL,
	Description VARCHAR(MAX) NULL
)
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

CREATE TABLE [dbo].[OutputMirror](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [Name] [varchar](200) NOT NULL,
	[Source] [varchar](max) NOT NULL,
	[ConnectionType] [varchar](200) NOT NULL,
    [Settings] [varchar](max) NULL,
    [LoadOrder] [int] NOT NULL CONSTRAINT [DF_OutputMirror_LoadOrder] DEFAULT ((0)),
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_OutputMirror_CreatedOn] DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_OutputMirror_CreatedBy] DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_OutputMirror_UpdatedOn] DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_OutputMirror_UpdatedBy] DEFAULT (suser_name()),
 CONSTRAINT [PK_OutputMirror] PRIMARY KEY CLUSTERED
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

CREATE TABLE IONWaveformCheckpoint
(
    ID INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
    Device VARCHAR(200) NOT NULL,
    TimeRecorded DATETIME2 NOT NULL,
    LogPositions VARCHAR(MAX) NOT NULL DEFAULT '[]',
    CONSTRAINT IX_IONWaveformCheckpoint_Device UNIQUE (Device ASC, TimeRecorded ASC)
)
GO

CREATE TABLE IONTrendingCheckpoint
(
    ID INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
    Device VARCHAR(200) NOT NULL,
    TimeRecorded DATETIME NOT NULL,
    LogPositions VARCHAR(MAX) NOT NULL DEFAULT '[]',
    CONSTRAINT IX_IONTrendingCheckpoint_Device UNIQUE (Device ASC, TimeRecorded ASC)
)
GO


CREATE TABLE DranetzWaveformCheckpoint
(
    ID INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
    Device VARCHAR(200) NOT NULL UNIQUE,
    TimeRecorded DATETIME2 NOT NULL
)
GO

CREATE TABLE DranetzTrendingCheckpoint
(
    ID INT IDENTITY(1, 1) NOT NULL PRIMARY KEY,
    Device VARCHAR(200) NOT NULL UNIQUE,
    TimeRecorded DATETIME NOT NULL
)
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
--	--SELECT * FROM #inserted

--	DECLARE @date DATETIME = GETUTCDATE()
--	DECLARE @deviceID int =  (Select TOP 1 DeviceID from #inserted)
--	DECLARE @enabled bit = (SELECT [Enabled] FROM Device WHERE ID = @deviceID)

--	EXEC spQueryToHtmlTable @html = @html OUTPUT,  @query = N'SELECT * FROM #inserted';
--	DECLARE @recipients nvarchar(max) = (SELECT Value FROM Setting WHERE Name = 'EmailRecipients')
--	DECLARE @profile_name nvarchar(max) = (SELECT Value FROM Setting WHERE Name = 'SqlServerEmailProfile')
--	DECLARE @downloadThreshholdWindow int = (SELECT Value FROM Setting WHERE Name = 'MaxDownloadThresholdTimeWindow')
--	DECLARE @downloadThreshhold int = (SELECT Value FROM Setting WHERE Name = 'MaxDownloadThreshold')
--	DECLARE @downloadCount int = (SELECT COUNT(*) FROM DownloadedFile WHERE Timestamp BETWEEN DATEADD(HOUR, -@downloadThreshholdWindow, @date) AND @date AND DeviceID = @deviceID)

--	DECLARE @message nvarchar(MAX) = (SELECT TOP 1 Message FROM inserted)
--	DECLARE @name nvarchar(max) = (SELECT Name FROM Device WHERE ID = @deviceID)
--	DECLARE @lastFile nvarchar(max) = (SELECT TOP 1 LastFile FROM inserted)
--	DECLARE @downloadDate nvarchar(max) = (SELECT TOP 1 FileDownloadTimestamp FROM inserted)
--	DECLARE @lastSuccess DateTime = (SELECT LastSuccess FROM StatusLog WHERE DeviceID = @deviceID)
--	DECLARE @lastFailure DateTime = (SELECT LastFailure FROM StatusLog WHERE DeviceID = @deviceID)
--	DECLARE @fileSize int = (SELECT TOP 1 FileSize FROM DownloadedFile WHERE DeviceID = @deviceID ORDER BY ID DESC)
--	DECLARE @fileDate DateTime = (SELECT TOP 1 FileDownloadTimeStamp FROM #inserted)


--	DECLARE @downloadDateDiff int = (SELECT DATEDIFF(HOUR, @fileDate, @downloadDate))
--	DECLARE @successDateDiff int = (SELECT DATEDIFF(HOUR, @lastSuccess, @lastFailure))
--	DECLARE @emailFlag bit = 0;
--	DECLARE @intro nvarchar(max) = N''
--	DECLARE @emailCountToday int = (SELECT COUNT(*) FROM SentEmail WHERE DeviceID = @deviceID AND Timestamp > CAST(@date as DATE))

--	IF @downloadThreshhold > 0 AND @downloadCount > @downloadThreshhold AND @enabled = 1
--	BEGIN
--		SET @intro = @intro + N'<div>'+ @Name+' has been disabled due to excessive downloads.</div><br>'
--		SET @emailFlag = 1
--		UPDATE Device set [Enabled] = 0 WHERE ID = @deviceID
--	END

--	IF @fileDate > @date AND @enabled = 1 AND @emailCountToday = 0
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
--		DECLARE @subject nvarchar(max) = N'OpenMIC '+ @Name +' problems ...'
--		EXEC msdb.dbo.sp_send_dbmail
--			@profile_name =	@profile_name,
--			@recipients= @recipients,
--			@subject = @subject,
--			@body = @html,
--			@body_format = 'HTML';

--		INSERT INTO SentEmail (DeviceID, [Message],[Timestamp]) VALUES ( @deviceID, @html, @date)

--	END

--	DROP Table #inserted

--END

--GO
