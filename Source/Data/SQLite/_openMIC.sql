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

CREATE TABLE [dbo].[StatusLog](		
      [ID] [int] IDENTITY(1,1) NOT NULL,		
      [DeviceID] [int] NOT NULL,		
      [LastSuccess] [DateTime2] NULL,		
	  [LastFailure] [DateTime2] NULL,		
      [Message] [varchar](max) NULL,
	  [LastFile] [varchar](max) NULL		
);


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
