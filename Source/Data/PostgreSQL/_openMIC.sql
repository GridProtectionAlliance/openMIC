CREATE TABLE ConnectionProfile(
    ID SERIAL NOT NULL PRIMARY KEY,
    Name VARCHAR(200) NOT NULL,
    Description TEXT NULL,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedBy VARCHAR(200) NOT NULL DEFAULT ''
);

CREATE TABLE ConnectionProfileTask(
    ID SERIAL NOT NULL PRIMARY KEY,
    ConnectionProfileID INTEGER NOT NULL DEFAULT 1,
    Name VARCHAR(200) NOT NULL,
    Settings TEXT NULL,
    CreatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    CreatedBy VARCHAR(200) NOT NULL DEFAULT '',
    UpdatedOn TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
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
