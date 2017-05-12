-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW LocalSchemaVersion AS
SELECT 1 AS VersionNumber;

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

CREATE TABLE StatusLog(		
    ID SERIAL NOT NULL PRIMARY KEY,
    DeviceID INTEGER NOT NULL,
    LastSuccess TIMESTAMP NULL,
    LastFailure TIMESTAMP NULL,
    Message TEXT NULL,
    LastFile TEXT NULL,
    CONSTRAINT IX_StatusLog_DeviceID UNIQUE (DeviceID),
    FileDownloadTimestamp TIMESTAMP NULL
);

CREATE TABLE DownloadedFile(
    ID SERIAL NOT NULL PRIMARY KEY,
    DeviceID INTEGER NOT NULL,
    File VARCHAR(200) NOT NULL,
    Timestamp TIMESTAMP NOT NULL,
    FileSize INTEGER NOT NULL,
    CreationTime TIMESTAMP NOT NULL
 );

 CREATE TABLE SentEmail(
    ID SERIAL NOT NULL PRIMARY KEY,
    DeviceID INTEGER NOT NULL,
    Message TEXT NOT NULL,
    Timestamp TIMESTAMP NOT NULL
 );

CREATE INDEX IX_DownloadedFile_DeviceID ON DownloadedFile (DeviceID);
CREATE INDEX IX_SentEmail_DeviceID ON SentEmail (DeviceID);
CREATE INDEX IX_SentEmail_Timestamp ON SentEmail (Timestamp);

