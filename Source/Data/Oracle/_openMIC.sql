-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW LocalSchemaVersion AS
SELECT 2 AS VersionNumber
FROM dual;

CREATE TABLE ConnectionProfileTaskQueue(
    ID NUMBER NOT NULL,
    Name VARCHAR2(200) NOT NULL,
    MaxThreadCount NUMBER DEFAULT 0 NOT NULL,
    UseBackgroundThreads NUMBER DEFAULT 0 NOT NULL,
    Description VARCHAR2(MAX) NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_ConnectionProfTaskQueue_ID ON ConnectionProfileTaskQueue (ID ASC) TABLESPACE openMIC_INDEX;

ALTER TABLE ErrorLog ADD CONSTRAINT PK_ConnectionProfileTaskQueue PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_ConnectionProfileTaskQueue START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_ConnectionProfileTaskQueue BEFORE INSERT ON ConnectionProfileTaskQueue
    FOR EACH ROW BEGIN SELECT SEQ_ConnectionProfileTaskQueue.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE ConnectionProfile(
    ID NUMBER NOT NULL,
    Name VARCHAR2(200) NOT NULL,
    DefaultTaskQueueID NUMBER NULL,
    Description VARCHAR2(4000) NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_ConnectionProfile_ID ON ConnectionProfile (ID ASC) TABLESPACE openMIC_INDEX;

ALTER TABLE ConnectionProfile ADD CONSTRAINT PK_ConnectionProfile PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_ConnectionProfile START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_ConnectionProfile BEFORE INSERT ON ConnectionProfile
    FOR EACH ROW BEGIN SELECT SEQ_ConnectionProfile.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE ConnectionProfileTask(
    ID NUMBER NOT NULL,
    ConnectionProfileID NUMBER DEFAULT 1 NOT NULL,
    Name VARCHAR2(200) NOT NULL,
    Settings VARCHAR2(4000) NULL,
    LoadOrder NUMBER DEFAULT 0 NOT NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE UNIQUE INDEX IX_ConnectionProfileTask_ID ON ConnectionProfileTask (ID ASC) TABLESPACE openMIC_INDEX;

ALTER TABLE ConnectionProfileTask ADD CONSTRAINT PK_ConnectionProfileTask PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_ConnectionProfileTask START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_ConnectionProfileTask BEFORE INSERT ON ConnectionProfileTask
    FOR EACH ROW BEGIN SELECT SEQ_ConnectionProfileTask.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE DownloadedFile(
    ID NUMBER NOT NULL,
    DeviceID NUMBER NOT NULL,
    FilePath VARCHAR2(200) NOT NULL,
    Timestamp DATE NOT NULL,
    CreationTime DATE NOT NULL,
    LastWriteTime DATE NOT NULL,
    LastAccessTime DATE NOT NULL,
    FileSize NUMBER NOT NULL
 );

CREATE UNIQUE INDEX IX_DownloadedFile_ID ON DownloadedFile (ID ASC) TABLESPACE openMIC_INDEX;
CREATE INDEX IX_DownloadedFile_DeviceID ON DownloadedFile (DeviceID)  TABLESPACE openMIC_INDEX;
CREATE INDEX IX_DownloadedFile_FilePath ON DownloadedFile (FilePath)  TABLESPACE openMIC_INDEX;

ALTER TABLE DownloadedFile ADD CONSTRAINT PK_DownloadedFile PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_DownloadedFile START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_DownloadedFile BEFORE INSERT ON DownloadedFile
    FOR EACH ROW BEGIN SELECT SEQ_DownloadedFile.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE StatusLog(
    ID NUMBER NOT NULL,
    DeviceID NUMBER NOT NULL,
    LastDownloadedFileID NUMBER NULL,
    LastOutcome VARCHAR2(50) NULL,
    LastRun DATE NULL,
    LastFailure DATE NULL,
    LastErrorMessage VARCHAR2(4000) NULL,
    LastDownloadStartTime DATE NULL,
    LastDownloadEndTime DATE NULL,
    LastDownloadFileCount NUMBER NULL
);

CREATE UNIQUE INDEX IX_StatusLog_ID ON StatusLog (ID ASC) TABLESPACE openMIC_INDEX;
CREATE UNIQUE INDEX IX_StatusLog_ID ON StatusLog (DeviceID ASC) TABLESPACE openMIC_INDEX;

ALTER TABLE StatusLog ADD CONSTRAINT PK_StatusLog PRIMARY KEY (DeviceID);

CREATE SEQUENCE SEQ_StatusLog START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_StatusLog BEFORE INSERT ON StatusLog
    FOR EACH ROW BEGIN SELECT SEQ_StatusLog.nextval INTO :NEW.ID FROM dual;
END;
/

 CREATE TABLE SentEmail(
    ID NUMBER NOT NULL,
    DeviceID NUMBER NOT NULL,
    Message VARCHAR2(MAX) NOT NULL,
    Timestamp DATE NOT NULL,
    FileSize NUMBER NOT NULL
 );

CREATE UNIQUE INDEX IX_SentEmail_ID ON SentEmail (ID ASC) TABLESPACE openMIC_INDEX;
CREATE INDEX IX_SentEmail_DeviceID ON SentEmail (DeviceID)  TABLESPACE openMIC_INDEX;
CREATE INDEX IX_SentEmail_Timestamp ON SentEmail (Timestamp)  TABLESPACE openMIC_INDEX;

ALTER TABLE SentEmail ADD CONSTRAINT PK_SentEmail PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_SentEmail START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_SentEmail BEFORE INSERT ON SentEmail
    FOR EACH ROW BEGIN SELECT SEQ_SentEmail.nextval INTO :NEW.ID FROM dual;
END;
/

ALTER TABLE ConnectionProfile ADD CONSTRAINT FK_ConnProf_ConnProfTaskQueue FOREIGN KEY(DefaultTaskQueueID) REFERENCES ConnectionProfileTaskQueue (ID);
ALTER TABLE ConnectionProfileTask ADD CONSTRAINT FK_ConnectionProfileTask_ConnectionProfile FOREIGN KEY(ConnectionProfileID) REFERENCES ConnectionProfile (ID);
