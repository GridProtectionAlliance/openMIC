CREATE TABLE ConnectionProfile(
    ID NUMBER NOT NULL,
    Name VARCHAR2(200) NOT NULL,
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
    ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    ConnectionProfileID NUMBER DEFAULT 1 NOT NULL,
    Name VARCHAR2(200) NOT NULL,
    Settings VARCHAR2(4000) NULL,
    CreatedOn DATE NOT NULL,
    CreatedBy VARCHAR2(200) NOT NULL,
    UpdatedOn DATE NOT NULL,
    UpdatedBy VARCHAR2(200) NOT NULL
);

CREATE TABLE [dbo].[StatusLog](		
      [ID] [int] IDENTITY(1,1) NOT NULL,		
      [DeviceID] [int] NOT NULL,		
      [LastSuccess] [DateTime2] NULL,		
	  [LastFailure] [DateTime2] NULL,		
      [Message] [varchar](max) NULL,
	  [LastFile] [varchar](max) NULL		
);

CREATE UNIQUE INDEX IX_ConnectionProfileTask_ID ON ConnectionProfileTask (ID ASC) TABLESPACE openMIC_INDEX;

ALTER TABLE ConnectionProfileTask ADD CONSTRAINT PK_ConnectionProfileTask PRIMARY KEY (ID);

CREATE SEQUENCE SEQ_ConnectionProfileTask START WITH 1 INCREMENT BY 1;

CREATE TRIGGER AI_ConnectionProfileTask BEFORE INSERT ON ConnectionProfileTask
    FOR EACH ROW BEGIN SELECT SEQ_ConnectionProfileTask.nextval INTO :NEW.ID FROM dual;
END;
/

ALTER TABLE ConnectionProfileTask ADD CONSTRAINT FK_ConnectionProfileTask_ConnectionProfile FOREIGN KEY(ConnectionProfileID) REFERENCES ConnectionProfile (ID);