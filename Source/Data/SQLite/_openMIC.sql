-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW LocalSchemaVersion AS
SELECT 1 AS VersionNumber;

CREATE TABLE ConnectionProfileTaskQueue(
    ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    Name VARCHAR(200) NOT NULL,
    MaxThreadCount INTEGER NOT NULL DEFAULT 0,
    UseBackgroundThreads INTEGER NOT NULL DEFAULT 0,
    Description TEXT NULL,
    CreatedOn DATETIME NOT NULL DEFAULT '',
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn DATETIME NOT NULL DEFAULT '',
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT ''
);

CREATE TABLE ConnectionProfile(
    ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    Name VARCHAR(200) NOT NULL,
    DefaultTaskQueueID INTEGER NULL,
    Description TEXT NULL,
    CreatedOn DATETIME NOT NULL DEFAULT '',
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn DATETIME NOT NULL DEFAULT '',
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_ConnectionProfile_ConnectionProfileTaskQueue FOREIGN KEY(DefaultTaskQueueID) REFERENCES ConnectionProfileTaskQueue (ID)
);

CREATE TABLE ConnectionProfileTask(
    ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    ConnectionProfileID INTEGER NOT NULL DEFAULT 1,
    Name VARCHAR(200) NOT NULL,
    Settings TEXT NULL,
    LoadOrder INTEGER NOT NULL DEFAULT 0,
    CreatedOn DATETIME NOT NULL DEFAULT '',
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn DATETIME NOT NULL DEFAULT '',
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_ConnectionProfileTask_ConnectionProfile FOREIGN KEY(ConnectionProfileID) REFERENCES ConnectionProfile (ID)
);

CREATE TABLE DownloadedFile(
    ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    DeviceID INTEGER NOT NULL,
    FilepATH VARCHAR(200) NOT NULL,
    Timestamp DATETIME NOT NULL,
    CreationTime DATETIME NOT NULL,
    LastWriteTime DATETIME NOT NULL,
    LastAccessTime DATETIME NOT NULL,
    FileSize INTEGER NOT NULL
);

CREATE TABLE StatusLog(
    ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    DeviceID INTEGER NOT NULL,
    LastDownloadedFileID INTEGER NULL,
    LastOutcome VARCHAR(50) NULL,
    LastRun DATETIME NULL,
    LastFailure DATETIME NULL,
    LastErrorMessage TEXT NULL,
    LastDownloadStartTime DATETIME NULL,
    LastDownloadEndTime DATETIME NULL,
    LastDownloadFileCount INTEGER NULL,
    CONSTRAINT IX_StatusLog_DeviceID UNIQUE (DeviceID ASC)
);

 CREATE TABLE SentEmail(
    ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    DeviceID INTEGER NOT NULL,
    Message TEXT NOT NULL,
    Timestamp DATETIME NOT NULL
);

CREATE INDEX IX_DownloadedFile_DeviceID ON DownloadedFile (DeviceID);
CREATE INDEX IX_DownloadedFile_FilePath ON DownloadedFile (FilePath);
CREATE INDEX IX_SentEmail_DeviceID ON SentEmail (DeviceID);
CREATE INDEX IX_SentEmail_Timestamp ON SentEmail (Timestamp);

CREATE TRIGGER ConnectionProfileTaskQueue_InsertDefault AFTER INSERT ON ConnectionProfileTaskQueue
FOR EACH ROW
BEGIN
    UPDATE ConnectionProfileTaskQueue SET CreatedOn = strftime('%Y-%m-%d %H:%M:%f') WHERE ROWID = NEW.ROWID AND CreatedOn = '';
    UPDATE ConnectionProfileTaskQueue SET UpdatedOn = strftime('%Y-%m-%d %H:%M:%f') WHERE ROWID = NEW.ROWID AND UpdatedOn = '';
END;

CREATE TRIGGER ConnectionProfile_InsertDefault AFTER INSERT ON ConnectionProfile
FOR EACH ROW
BEGIN
    UPDATE ConnectionProfile SET CreatedOn = strftime('%Y-%m-%d %H:%M:%f') WHERE ROWID = NEW.ROWID AND CreatedOn = '';
    UPDATE ConnectionProfile SET UpdatedOn = strftime('%Y-%m-%d %H:%M:%f') WHERE ROWID = NEW.ROWID AND UpdatedOn = '';
END;

CREATE TRIGGER ConnectionProfileTask_InsertDefault AFTER INSERT ON ConnectionProfileTask
FOR EACH ROW
BEGIN
    UPDATE ConnectionProfileTask SET CreatedOn = strftime('%Y-%m-%d %H:%M:%f') WHERE ROWID = NEW.ROWID AND CreatedOn = '';
    UPDATE ConnectionProfileTask SET UpdatedOn = strftime('%Y-%m-%d %H:%M:%f') WHERE ROWID = NEW.ROWID AND UpdatedOn = '';
END;
