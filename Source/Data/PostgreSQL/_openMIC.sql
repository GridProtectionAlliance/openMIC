-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW LocalSchemaVersion AS
SELECT 9 AS VersionNumber;

CREATE TABLE Setting(
    ID SERIAL NOT NULL PRIMARY KEY,
    Name VARCHAR(200) NULL,
    Value TEXT NULL,
    DefaultValue TEXT NULL,
	Description TEXT NULL
);

CREATE TABLE ConnectionProfileTaskQueue(
    ID SERIAL NOT NULL PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    MaxThreadCount INTEGER NOT NULL DEFAULT 0,
    UseBackgroundThreads INTEGER NOT NULL DEFAULT 0,
    Description TEXT NULL,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT ''
);

CREATE TABLE ConnectionProfile(
    ID SERIAL NOT NULL PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    DefaultTaskQueueID INTEGER NULL,
    Description TEXT NULL,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_ConnectionProfile_ConnectionProfileTaskQueue FOREIGN KEY(DefaultTaskQueueID) REFERENCES ConnectionProfileTaskQueue (ID)
);

CREATE TABLE ConnectionProfileTask(
    ID SERIAL NOT NULL PRIMARY KEY,
    ConnectionProfileID INTEGER NOT NULL DEFAULT 1,
    Name VARCHAR(200) NOT NULL,
    Settings TEXT NULL,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_ConnectionProfileTask_ConnectionProfile FOREIGN KEY(ConnectionProfileID) REFERENCES ConnectionProfile (ID)
);

CREATE TABLE OutputMirror(
    ID SERIAL NOT NULL PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Source TEXT NOT NULL,
    ConnectionType VARCHAR(200) NOT NULL,
    Settings TEXT NULL,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT ''
);

CREATE TABLE DownloadedFile(
    ID SERIAL NOT NULL PRIMARY KEY,
    DeviceID INTEGER NOT NULL,
    FilePath VARCHAR(200) NOT NULL,
    Timestamp TIMESTAMP NOT NULL,
    CreationTime TIMESTAMP NOT NULL,
    LastWriteTime TIMESTAMP NOT NULL,
    LastAccessTime TIMESTAMP NOT NULL,
    FileSize INTEGER NOT NULL
 );

CREATE TABLE StatusLog(		
    ID SERIAL NOT NULL PRIMARY KEY,
    DeviceID INTEGER NOT NULL,
    LastDownloadedFileID INTEGER NULL,
    LastOutcome VARCHAR(50) NULL,
    LastRun TIMESTAMP NULL,
    LastFailure TIMESTAMP NULL,
    LastErrorMessage TEXT NULL,
    LastDownloadStartTime TIMESTAMP NULL,
    LastDownloadEndTime TIMESTAMP NULL,
    LastDownloadFileCount INTEGER NULL,
    CONSTRAINT IX_StatusLog_DeviceID UNIQUE (DeviceID)
);

 CREATE TABLE SentEmail(
    ID SERIAL NOT NULL PRIMARY KEY,
    DeviceID INTEGER NOT NULL,
    Message TEXT NOT NULL,
    Timestamp TIMESTAMP NOT NULL
 );

CREATE TABLE NodeCheckin(
    ID SERIAL NOT NULL PRIMARY KEY,
    URL VARCHAR(200) NOT NULL,
    Task VARCHAR(500) NOT NULL,
    LastCheckin TIMESTAMP NOT NULL,
    FailureReason VARCHAR(MAX) NULL,
    TasksQueued INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT IX_NodeCheckin_URL_Task UNIQUE (URL ASC, Task ASC)
);

CREATE TABLE DranetzWaveformCheckpoint(
    ID SERIAL NOT NULL PRIMARY KEY,
    Device VARCHAR(200) NOT NULL,
    TimeRecorded TIMESTAMP NOT NULL,
    CONSTRAINT IX_DranetzWaveformCheckpoint_Device UNIQUE (Device ASC)
);

CREATE TABLE DranetzTrendingCheckpoint(
    ID SERIAL NOT NULL PRIMARY KEY,
    Device VARCHAR(200) NOT NULL,
    TimeRecorded TIMESTAMP NOT NULL,
    CONSTRAINT IX_DranetzTrendingCheckpoint_Device UNIQUE (Device ASC)
);

CREATE TABLE DailyStatisticsRecord(
    ID SERIAL NOT NULL PRIMARY KEY,
    Timestamp TIMESTAMP NOT NULL,
    BadDays INTEGER NOT NULL DEFAULT 0,
    Meter VARCHAR(200) NOT NULL,
    LastSuccessfulConnection TIMESTAMP NULL,
    LastUnsuccessfulConnection TIMESTAMP NULL,
    LastUnsuccessfulConnectionExplanation TEXT NULL,
    TotalSuccessfulConnections INTEGER NOT NULL DEFAULT 0,
    TotalUnsuccessfulConnections INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT IX_DailyStatisticsRecord_Meter UNIQUE (Meter ASC, [Timestamp] DESC)
);

CREATE TABLE PollingTask
(
    ID SERIAL NOT NULL PRIMARY KEY,
    RuntimeID NCHAR(36) NOT NULL,
    NodeID VARCHAR(200) NOT NULL,
    DownloaderName VARCHAR(200) NOT NULL,
    Task VARCHAR(200) NOT NULL,
    CONSTRAINT IX_PollingTask_RuntimeID UNIQUE (RuntimeID ASC)
);

CREATE INDEX IX_PollingTask_DownloaderName_Task
ON PollingTask(DownloaderName, Task);

CREATE TABLE DownloaderGroupLock
(
    ID SERIAL NOT NULL PRIMARY KEY,
    GroupID VARCHAR(50) NOT NULL,
    NodeID VARCHAR(200) NULL,
    LockedAt TIMESTAMP NULL,
    CONSTRAINT IX_DownloaderGroupLock_GroupID UNIQUE (GroupID ASC)
);

DROP VIEW TrackedTable;

CREATE VIEW TrackedTable AS
SELECT 'Measurement' AS Name  WHERE 1 < 0;

ALTER TABLE TrackedChange RENAME TO TrackedChange_dummy;

CREATE VIEW TrackedChange AS
SELECT * FROM TrackedChange_dummy;

CREATE FUNCTION TrackedChange_ClearTableFn() RETURNS TRIGGER
AS $TrackedChange_ClearTableFn$
BEGIN
    RETURN NULL;
END;
$TrackedChange_ClearTableFn$ LANGUAGE plpgsql;

CREATE TRIGGER TrackedChange_ClearTable INSTEAD OF INSERT ON TrackedChange
FOR EACH ROW EXECUTE PROCEDURE TrackedChange_ClearTableFn();

CREATE INDEX IX_DownloadedFile_DeviceID ON DownloadedFile (DeviceID);
CREATE INDEX IX_DownloadedFile_FilePath ON DownloadedFile (FilePath);
CREATE INDEX IX_SentEmail_DeviceID ON SentEmail (DeviceID);
CREATE INDEX IX_SentEmail_Timestamp ON SentEmail (Timestamp);

