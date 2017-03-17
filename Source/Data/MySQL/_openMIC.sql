CREATE TABLE ConnectionProfile(
    ID INT AUTO_INCREMENT NOT NULL,
    Name VARCHAR(200) NOT NULL,
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
    CreatedOn DATETIME NULL,
    CreatedBy VARCHAR(200) NULL,
    UpdatedOn DATETIME NULL,
    UpdatedBy VARCHAR(200) NULL,
    CONSTRAINT PK_ConnectionProfileTask PRIMARY KEY (ID ASC)
);

CREATE TABLE StatusLog(
    ID INT AUTO_INCREMENT NOT NULL,
    DeviceID INT NOT NULL,
    LastSuccess DateTime NULL,		
	LastFailure DateTime NULL,		
    Message TEXT NULL,
    LastFile TEXT NULL,
    CONSTRAINT PK_StatusLog PRIMARY KEY (ID ASC),
    CONSTRAINT IX_StatusLog_DeviceID UNIQUE KEY (DeviceID ASC)
);

ALTER TABLE ConnectionProfileTask ADD CONSTRAINT FK_ConnectionProfileTask_ConnectionProfile FOREIGN KEY(ConnectionProfileID) REFERENCES ConnectionProfile (ID);

CREATE TRIGGER ConnectionProfile_InsertDefault BEFORE INSERT ON ConnectionProfile
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());

CREATE TRIGGER ConnectionProfileTask_InsertDefault BEFORE INSERT ON ConnectionProfileTask
FOR EACH ROW SET NEW.CreatedBy = COALESCE(NEW.CreatedBy, USER()), NEW.CreatedOn = COALESCE(NEW.CreatedOn, UTC_TIMESTAMP()), NEW.UpdatedBy = COALESCE(NEW.UpdatedBy, USER()), NEW.UpdatedOn = COALESCE(NEW.UpdatedOn, UTC_TIMESTAMP());
