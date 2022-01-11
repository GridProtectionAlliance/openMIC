-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW LocalSchemaVersion AS
SELECT 5 AS VersionNumber
FROM dual;

CREATE TABLE Setting(
    ID NUMBER NOT NULL,
    Name VARCHAR2(200) NULL,
    Value VARCHAR2(MAX) NULL,
    DefaultValue VARCHAR2(MAX) NULL,
	Description VARCHAR2(MAX) NULL
);

CREATE UNIQUE INDEX IX_Setting_ID ON Setting (ID ASC) TABLESPACE openMIC_INDEX;

ALTER TABLE Setting ADD CONSTRAINT PK_Setting PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_Setting START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_Setting BEFORE INSERT ON Setting
    FOR EACH ROW BEGIN SELECT SEQ_Setting.nextval INTO :NEW.ID FROM dual;
END;
/

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

ALTER TABLE ConnectionProfileTaskQueue ADD CONSTRAINT PK_ConnectionProfileTaskQueue PRIMARY KEY (ID);

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

CREATE TABLE IONWaveformCheckpoint(
    ID NUMBER NOT NULL,
    Device VARCHAR2(200) NOT NULL,
    TimeRecorded DATE NOT NULL,
    LogPositions VARCHAR2(MAX) DEFAULT '[]' NOT NULL
);

CREATE UNIQUE INDEX IX_IONWaveformCheckpoint_ID ON IONWaveformCheckpoint (ID ASC) TABLESPACE openMIC_INDEX;
CREATE UNIQUE INDEX IX_IONWaveformCheckpoint_Dev ON IONWaveformCheckpoint (Device ASC, TimeRecorded ASC)  TABLESPACE openMIC_INDEX;

ALTER TABLE IONWaveformCheckpoint ADD CONSTRAINT PK_IONWaveformCheckpoint PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_IONWaveformCheckpoint START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_IONWaveformCheckpoint BEFORE INSERT ON IONWaveformCheckpoint
    FOR EACH ROW BEGIN SELECT SEQ_IONWaveformCheckpoint.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE IONTrendingCheckpoint(
    ID NUMBER NOT NULL,
    Device VARCHAR2(200) NOT NULL,
    TimeRecorded DATE NOT NULL,
    LogPositions VARCHAR2(MAX) DEFAULT '[]' NOT NULL
);

CREATE UNIQUE INDEX IX_IONTrendingCheckpoint_ID ON IONTrendingCheckpoint (ID ASC) TABLESPACE openMIC_INDEX;
CREATE UNIQUE INDEX IX_IONTrendingCheckpoint_Dev ON IONTrendingCheckpoint (Device ASC, TimeRecorded ASC)  TABLESPACE openMIC_INDEX;

ALTER TABLE IONTrendingCheckpoint ADD CONSTRAINT PK_IONTrendingCheckpoint PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_IONTrendingCheckpoint START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_IONTrendingCheckpoint BEFORE INSERT ON IONTrendingCheckpoint
    FOR EACH ROW BEGIN SELECT SEQ_IONTrendingCheckpoint.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE DranetzWaveformCheckpoint(
    ID NUMBER NOT NULL,
    Device VARCHAR2(200) NOT NULL,
    TimeRecorded DATE NOT NULL,
    CONSTRAINT IX_DranetzWaveformCheckpoint_Device UNIQUE (Device ASC)
);

CREATE UNIQUE INDEX IX_DranetzWaveformCheckpoint_ID ON DranetzWaveformCheckpoint (ID ASC) TABLESPACE openMIC_INDEX;
CREATE UNIQUE INDEX IX_DranetzWaveformCheckpoint_Dev ON DranetzWaveformCheckpoint (Device ASC)  TABLESPACE openMIC_INDEX;

ALTER TABLE DranetzWaveformCheckpoint ADD CONSTRAINT PK_DranetzWaveformCheckpoint PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_DranetzWaveformCheckpoint START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_DranetzWaveformCheckpoint BEFORE INSERT ON DranetzWaveformCheckpoint
    FOR EACH ROW BEGIN SELECT SEQ_DranetzWaveformCheckpoint.nextval INTO :NEW.ID FROM dual;
END;
/

CREATE TABLE DranetzTrendingCheckpoint(
    ID NUMBER NOT NULL,
    Device VARCHAR2(200) NOT NULL,
    TimeRecorded DATE NOT NULL,
    CONSTRAINT IX_DranetzTrendingCheckpoint_Device UNIQUE (Device ASC)
);

CREATE UNIQUE INDEX IX_DranetzTrendingCheckpoint_ID ON DranetzTrendingCheckpoint (ID ASC) TABLESPACE openMIC_INDEX;
CREATE UNIQUE INDEX IX_DranetzTrendingCheckpoint_Dev ON DranetzTrendingCheckpoint (Device ASC)  TABLESPACE openMIC_INDEX;

ALTER TABLE DranetzTrendingCheckpoint ADD CONSTRAINT PK_DranetzTrendingCheckpoint PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_DranetzTrendingCheckpoint START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_DranetzTrendingCheckpoint BEFORE INSERT ON DranetzTrendingCheckpoint
    FOR EACH ROW BEGIN SELECT SEQ_DranetzTrendingCheckpoint.nextval INTO :NEW.ID FROM dual;
END;
/

ALTER TABLE ConnectionProfile ADD CONSTRAINT FK_ConnProf_ConnProfTaskQueue FOREIGN KEY(DefaultTaskQueueID) REFERENCES ConnectionProfileTaskQueue (ID);
ALTER TABLE ConnectionProfileTask ADD CONSTRAINT FK_ConnectionProfileTask_ConnectionProfile FOREIGN KEY(ConnectionProfileID) REFERENCES ConnectionProfile (ID);
