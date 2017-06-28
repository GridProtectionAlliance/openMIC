-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW LocalSchemaVersion AS
SELECT 2 AS VersionNumber;

CREATE TABLE ConnectionProfileTaskQueue(
    ID INT AUTO_INCREMENT NOT NULL,
    Name VARCHAR(200) NOT NULL,
    MaxThreadCount INT NOT NULL DEFAULT 0,
    UseBackgroundThreads INT NOT NULL DEFAULT 0,
    Description TEXT NULL,
    CreatedOn DATETIME NOT NULL,
    CreatedBy VARCHAR(200) NOT NULL,
    UpdatedOn DATETIME NOT NULL,
    UpdatedBy VARCHAR(200) NOT NULL,
    CONSTRAINT PK_ConnectionProfileTaskQueue PRIMARY KEY (ID ASC)
);

CREATE TABLE ConnectionProfile(
    ID INT AUTO_INCREMENT NOT NULL,
    Name VARCHAR(200) NOT NULL,
    DefaultTaskQueueID INT NULL,
    Description TEXT NULL,
    CreatedOn DATETIME NOT NULL,
    CreatedBy VARCHAR(200) NOT NULL,
    UpdatedOn DATETIME NOT NULL,
    UpdatedBy VARCHAR(200) NOT NULL,
    CONSTRAINT PK_ConnectionProfile PRIMARY KEY (ID ASC)
);

CREATE TABLE ConnectionProfileTask(
    ID INT AUTO_INCREMENT NOT NULL,
    ConnectionProfileID INT NOT NULL DEFAULT 1,
    Name VARCHAR(200) NOT NULL,
    Settings TEXT NULL,
    LoadOrder INT NOT NULL DEFAULT 0,
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_ConnectionProfileTask PRIMARY KEY (ID ASC)
);

CREATE TABLE DownloadedFile(
    ID int AUTO_INCREMENT NOT NULL,
    DeviceID INT NOT NULL,
    FilePath VARCHAR(200) NOT NULL,
    Timestamp DATETIME NOT NULL,
    CreationTime DATETIME NOT NULL,
    LastWriteTime DATETIME NOT NULL,
    LastAccessTime DATETIME NOT NULL,
    FileSize INT NOT NULL,
    CONSTRAINT PK_DownloadedFile PRIMARY KEY CLUSTERED (ID ASC)
 );

CREATE TABLE StatusLog(
    ID INT AUTO_INCREMENT NOT NULL,
    DeviceID INT NOT NULL,
    LastDownloadedFileID INT NULL,
    LastOutcome VARCHAR(50) NULL,
    LastRun DATETIME NULL,
    LastFailure DATETIME NULL,
    LastErrorMessage TEXT NULL,
    LastDownloadStartTime DATETIME NULL,
    LastDownloadEndTime DATETIME NULL,
    LastDownloadFileCount INT NULL,
    CONSTRAINT PK_StatusLog PRIMARY KEY (ID ASC),
    CONSTRAINT IX_StatusLog_DeviceID UNIQUE KEY (DeviceID ASC)
);

CREATE TABLE SentEmail(
    ID INT AUTO_INCREMENT NOT NULL,
    DeviceID INT NOT NULL,
    Message TEXT NOT NULL,
    Timestamp DATETIME NOT NULL,
    FileSize INT NOT NULL,
    CONSTRAINT PK_SentEMail PRIMARY KEY CLUSTERED (ID ASC) 
 );

ALTER TABLE ConnectionProfile ADD CONSTRAINT FK_ConnectionProfile_ConnectionProfileTaskQueue FOREIGN KEY(DefaultTaskQueueID) REFERENCES ConnectionProfileTaskQueue (ID);
ALTER TABLE ConnectionProfileTask ADD CONSTRAINT FK_ConnectionProfileTask_ConnectionProfile FOREIGN KEY(ConnectionProfileID) REFERENCES ConnectionProfile (ID);

CREATE TRIGGER ConnectionProfileTaskQueue_InsertDefault BEFORE INSERT ON ConnectionProfileTaskQueue
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER ConnectionProfile_InsertDefault BEFORE INSERT ON ConnectionProfile
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER ConnectionProfileTask_InsertDefault BEFORE INSERT ON ConnectionProfileTask
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE INDEX IX_DownloadedFile_DeviceID ON DownloadedFile (DeviceID);
CREATE INDEX IX_DownloadedFile_FilePath ON DownloadedFile (FilePath);
CREATE INDEX IX_SentEmail_DeviceID ON SentEmail (DeviceID);
CREATE INDEX IX_SentEmail_Timestamp ON SentEmail (Timestamp);
