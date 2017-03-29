CREATE TABLE ConnectionProfile(
    ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    Name VARCHAR(200) NOT NULL,
    Description TEXT NULL,
    CreatedOn DATETIME NOT NULL DEFAULT '',
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn DATETIME NOT NULL DEFAULT '',
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT ''
);

CREATE TABLE ConnectionProfileTask(
    ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    ConnectionProfileID INTEGER NOT NULL DEFAULT 1,
    Name VARCHAR(200) NOT NULL,
    Settings TEXT NULL,
    CreatedOn DATETIME NOT NULL DEFAULT '',
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn DATETIME NOT NULL DEFAULT '',
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT '',
    CONSTRAINT FK_ConnectionProfileTask_ConnectionProfile FOREIGN KEY(ConnectionProfileID) REFERENCES ConnectionProfile (ID)
);

CREATE TABLE StatusLog(
    ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
    DeviceID INTEGER NOT NULL,
    LastSuccess DATETIME NULL,		
	LastFailure DATETIME NULL,		
    Message TEXT NULL,
	LastFile TEXT NULL,
	FileDownloadTimestamp DATETIME NULL,
    CONSTRAINT IX_StatusLog_DeviceID UNIQUE (DeviceID ASC)
);

CREATE TABLE DownloadedFile(
	ID INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL ,
	DeviceID INTEGER NOT NULL,
	File VARCHAR(200) NOT NULL,
	Timestamp DATETIME NOT NULL,
	CreationTime DATETIME NOT NULL
);

 CREATE TABLE SentEmail(
	ID  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
	DeviceID int NOT NULL,
	Message VARCHAR(MAX) NOT NULL,
	Timestamp DATETIME NOT NULL
);

CREATE INDEX IX_DownloadedFile_DeviceID ON DownloadedFile (DeviceID);
CREATE INDEX IX_SentEmail_DeviceID ON SentEmail (DeviceID);
CREATE INDEX IX_SentEmail_Timestamp ON SentEmail (Timestamp);


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
