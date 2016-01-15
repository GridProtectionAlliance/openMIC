USE [openMIC]
GO

CREATE PROCEDURE [dbo].[InsertIntoAuditLog] (@tableName VARCHAR(128), @primaryKeyColumn VARCHAR(128), @primaryKeyValue NVARCHAR(MAX), @deleted BIT = '0', @inserted BIT = '0') AS	
BEGIN

	DECLARE @columnName varchar(100) 
	DECLARE @cursorColumnNames CURSOR 
	
	SET @cursorColumnNames = CURSOR FOR 
	SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @tableName AND TABLE_CATALOG = db_name()

	OPEN @cursorColumnNames 

	FETCH NEXT FROM @cursorColumnNames INTO @columnName 
	WHILE @@FETCH_STATUS = 0 
	BEGIN 

		DECLARE @sql VARCHAR(MAX)
		
		IF @deleted = '0' AND @inserted = '1'
			SET @sql = 'INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) ' +
					'SELECT ''' + @tableName + ''', ''' + @primaryKeyColumn + ''', ''' + @primaryKeyValue + ''', ''' + @columnName + ''', ' +
					'NULL, CONVERT(NVARCHAR(MAX), #inserted.' + @columnName + '), ''0'', #inserted.UpdatedBy FROM #inserted'
		ELSE IF @deleted = '1' AND @inserted = '0'
			BEGIN
				DECLARE @context VARCHAR(128)
				SELECT @context = CASE WHEN CONTEXT_INFO() IS NULL THEN SUSER_NAME() ELSE CAST(CONTEXT_INFO() AS VARCHAR(128)) END
				SET @sql = 'INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) ' +
						'SELECT ''' + @tableName + ''', ''' + @primaryKeyColumn + ''', ''' + @primaryKeyValue + ''', ''' + @columnName + ''', ' +
						'CONVERT(NVARCHAR(MAX), #deleted.' + @columnName + '), NULL, ''1'', ''' + @context + ''' FROM #deleted'
			END
		ELSE
			SET @sql = 'DECLARE @oldVal NVARCHAR(MAX) ' +
					'DECLARE @newVal NVARCHAR(MAX) ' +
					'SELECT @oldVal = CONVERT(NVARCHAR(MAX), #deleted.' + @columnName + '), @newVal = CONVERT(NVARCHAR(MAX), #inserted.' + @columnName + ') FROM #deleted, #inserted ' +
					'IF @oldVal <> @newVal BEGIN ' +			
					'INSERT INTO AuditLog (TableName, PrimaryKeyColumn, PrimaryKeyValue, ColumnName, OriginalValue, NewValue, Deleted, UpdatedBy) ' +
					'SELECT ''' + @tableName + ''', ''' + @primaryKeyColumn + ''', ''' + @primaryKeyValue + ''', ''' + @columnName + ''', ' +
					'CONVERT(NVARCHAR(MAX), #deleted.' + @columnName + '), CONVERT(NVARCHAR(MAX), #inserted.' + @columnName + '), ''0'', #inserted.UpdatedBy ' +
					'FROM #inserted, #deleted ' +
					'END'

		EXECUTE (@sql)
		
		FETCH NEXT FROM @cursorColumnNames INTO @columnName 
	END 

	CLOSE @cursorColumnNames 
	DEALLOCATE @cursorColumnNames
	
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER UserAccount_AuditInsert 
   ON  UserAccount
   AFTER INSERT
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #inserted FROM inserted
	
	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #inserted	

	EXEC InsertIntoAuditLog 'UserAccount', 'ID', @id, '0', '1'
	
	DROP TABLE #inserted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER UserAccount_AuditUpdate 
   ON  UserAccount
   AFTER UPDATE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted  FROM deleted
	SELECT * INTO #inserted FROM inserted
	
	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	

	EXEC InsertIntoAuditLog 'UserAccount', 'ID', @id
	
	DROP TABLE #inserted
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER UserAccount_AuditDelete
   ON  UserAccount
   AFTER DELETE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted FROM deleted
		
	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	

	EXEC InsertIntoAuditLog 'UserAccount', 'ID', @id, '1'
		
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER SecurityGroup_AuditInsert 
   ON  SecurityGroup
   AFTER INSERT
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #inserted
	
	EXEC InsertIntoAuditLog 'SecurityGroup', 'ID', @id, '0', '1'
	
	DROP TABLE #inserted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER SecurityGroup_AuditUpdate 
   ON  SecurityGroup
   AFTER UPDATE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted  FROM deleted
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	
	
	EXEC InsertIntoAuditLog 'SecurityGroup', 'ID', @id
	
	DROP TABLE #inserted
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER SecurityGroup_AuditDelete
   ON  SecurityGroup
   AFTER DELETE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted FROM deleted
		
	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	

	EXEC InsertIntoAuditLog 'SecurityGroup', 'ID', @id, '1'
		
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER ApplicationRole_AuditInsert 
   ON  ApplicationRole
   AFTER INSERT
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #inserted
	
	EXEC InsertIntoAuditLog 'ApplicationRole', 'ID', @id, '0', '1'
	
	DROP TABLE #inserted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER ApplicationRole_AuditUpdate 
   ON  ApplicationRole
   AFTER UPDATE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted  FROM deleted
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	
	
	EXEC InsertIntoAuditLog 'ApplicationRole', 'ID', @id
	
	DROP TABLE #inserted
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER ApplicationRole_AuditDelete
   ON  ApplicationRole
   AFTER DELETE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted FROM deleted
		
	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	

	EXEC InsertIntoAuditLog 'ApplicationRole', 'ID', @id, '1'
		
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Company_AuditInsert
   ON  Company
   AFTER INSERT
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #inserted
	
	EXEC InsertIntoAuditLog 'Company', 'ID', @id, '0', '1'
	
	DROP TABLE #inserted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Company_AuditUpdate 
   ON  Company
   AFTER UPDATE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted  FROM deleted
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	
	
	EXEC InsertIntoAuditLog 'Company', 'ID', @id
	
	DROP TABLE #inserted
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Company_AuditDelete
   ON  Company
   AFTER DELETE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted FROM deleted
		
	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	

	EXEC InsertIntoAuditLog 'Company', 'ID', @id, '1'
		
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER CustomActionAdapter_AuditInsert
   ON  CustomActionAdapter
   AFTER INSERT
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #inserted
	
	EXEC InsertIntoAuditLog 'CustomActionAdapter', 'ID', @id, '0', '1'
	
	DROP TABLE #inserted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER CustomActionAdapter_AuditUpdate 
   ON  CustomActionAdapter
   AFTER UPDATE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted  FROM deleted
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	
	
	EXEC InsertIntoAuditLog 'CustomActionAdapter', 'ID', @id
	
	DROP TABLE #inserted
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER CustomActionAdapter_AuditDelete
   ON  CustomActionAdapter
   AFTER DELETE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted FROM deleted
		
	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	

	EXEC InsertIntoAuditLog 'CustomActionAdapter', 'ID', @id, '1'
		
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER CustomInputAdapter_AuditInsert
   ON  CustomInputAdapter
   AFTER INSERT
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #inserted
	
	EXEC InsertIntoAuditLog 'CustomInputAdapter', 'ID', @id, '0', '1'
	
	DROP TABLE #inserted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER CustomInputAdapter_AuditUpdate 
   ON  CustomInputAdapter
   AFTER UPDATE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted  FROM deleted
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	
	
	EXEC InsertIntoAuditLog 'CustomInputAdapter', 'ID', @id
	
	DROP TABLE #inserted
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER CustomInputAdapter_AuditDelete
   ON  CustomInputAdapter
   AFTER DELETE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted FROM deleted
		
	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	

	EXEC InsertIntoAuditLog 'CustomInputAdapter', 'ID', @id, '1'
		
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER CustomOutputAdapter_AuditInsert
   ON  CustomOutputAdapter
   AFTER INSERT
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #inserted
	
	EXEC InsertIntoAuditLog 'CustomOutputAdapter', 'ID', @id, '0', '1'
	
	DROP TABLE #inserted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER CustomOutputAdapter_AuditUpdate 
   ON  CustomOutputAdapter
   AFTER UPDATE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted  FROM deleted
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	
	
	EXEC InsertIntoAuditLog 'CustomOutputAdapter', 'ID', @id
	
	DROP TABLE #inserted
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER CustomOutputAdapter_AuditDelete
   ON  CustomOutputAdapter
   AFTER DELETE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted FROM deleted
		
	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	

	EXEC InsertIntoAuditLog 'CustomOutputAdapter', 'ID', @id, '1'
		
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Device_AuditInsert
   ON  Device
   AFTER INSERT
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #inserted
	
	EXEC InsertIntoAuditLog 'Device', 'ID', @id, '0', '1'
	
	DROP TABLE #inserted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Device_AuditUpdate 
   ON  Device
   AFTER UPDATE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted  FROM deleted
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	
	
	EXEC InsertIntoAuditLog 'Device', 'ID', @id
	
	DROP TABLE #inserted
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Device_AuditDelete
   ON  Device
   AFTER DELETE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted FROM deleted
		
	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	

	EXEC InsertIntoAuditLog 'Device', 'ID', @id, '1'
		
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Historian_AuditInsert
   ON  Historian
   AFTER INSERT
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #inserted
	
	EXEC InsertIntoAuditLog 'Historian', 'ID', @id, '0', '1'
	
	DROP TABLE #inserted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Historian_AuditUpdate 
   ON  Historian
   AFTER UPDATE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted  FROM deleted
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	
	
	EXEC InsertIntoAuditLog 'Historian', 'ID', @id
	
	DROP TABLE #inserted
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Historian_AuditDelete
   ON  Historian
   AFTER DELETE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted FROM deleted
		
	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	

	EXEC InsertIntoAuditLog 'Historian', 'ID', @id, '1'
		
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Measurement_AuditInsert
   ON  Measurement
   AFTER INSERT
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), SignalID) FROM #inserted
	
	EXEC InsertIntoAuditLog 'Measurement', 'SignalID', @id, '0', '1'
	
	DROP TABLE #inserted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Measurement_AuditUpdate 
   ON  Measurement
   AFTER UPDATE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted  FROM deleted
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), SignalID) FROM #deleted	
	
	EXEC InsertIntoAuditLog 'Measurement', 'SignalID', @id
	
	DROP TABLE #inserted
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Measurement_AuditDelete
   ON  Measurement
   AFTER DELETE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted FROM deleted
		
	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), SignalID) FROM #deleted	

	EXEC InsertIntoAuditLog 'Measurement', 'SignalID', @id, '1'
		
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Node_AuditInsert
   ON  Node
   AFTER INSERT
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #inserted
	
	EXEC InsertIntoAuditLog 'Node', 'ID', @id, '0', '1'
	
	DROP TABLE #inserted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Node_AuditUpdate 
   ON  Node
   AFTER UPDATE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted  FROM deleted
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	
	
	EXEC InsertIntoAuditLog 'Node', 'ID', @id
	
	DROP TABLE #inserted
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Node_AuditDelete
   ON  Node
   AFTER DELETE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted FROM deleted
		
	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	

	EXEC InsertIntoAuditLog 'Node', 'ID', @id, '1'
		
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Alarm_AuditInsert
   ON  Alarm
   AFTER INSERT
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #inserted
	
	EXEC InsertIntoAuditLog 'Alarm', 'ID', @id, '0', '1'
	
	DROP TABLE #inserted

END
GO

CREATE TRIGGER Alarm_AuditUpdate 
   ON  Alarm
   AFTER UPDATE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted  FROM deleted
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	
	
	EXEC InsertIntoAuditLog 'Alarm', 'ID', @id
	
	DROP TABLE #inserted
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Alarm_AuditDelete
   ON  Alarm
   AFTER DELETE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted FROM deleted
		
	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	

	EXEC InsertIntoAuditLog 'Alarm', 'ID', @id, '1'
		
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Vendor_AuditInsert
   ON  Vendor
   AFTER INSERT
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #inserted
	
	EXEC InsertIntoAuditLog 'Vendor', 'ID', @id, '0', '1'
	
	DROP TABLE #inserted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Vendor_AuditUpdate 
   ON  Vendor
   AFTER UPDATE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted  FROM deleted
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	
	
	EXEC InsertIntoAuditLog 'Vendor', 'ID', @id
	
	DROP TABLE #inserted
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER Vendor_AuditDelete
   ON  Vendor
   AFTER DELETE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted FROM deleted
		
	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	

	EXEC InsertIntoAuditLog 'Vendor', 'ID', @id, '1'
		
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER VendorDevice_AuditInsert
   ON  VendorDevice
   AFTER INSERT
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #inserted
	
	EXEC InsertIntoAuditLog 'VendorDevice', 'ID', @id, '0', '1'
	
	DROP TABLE #inserted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER VendorDevice_AuditUpdate 
   ON  VendorDevice
   AFTER UPDATE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted  FROM deleted
	SELECT * INTO #inserted FROM inserted

	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	
	
	EXEC InsertIntoAuditLog 'VendorDevice', 'ID', @id
	
	DROP TABLE #inserted
	DROP TABLE #deleted

END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER VendorDevice_AuditDelete
   ON  VendorDevice
   AFTER DELETE
AS 
BEGIN
	
	SET NOCOUNT ON;
	
	SELECT * INTO #deleted FROM deleted
		
	DECLARE @id NVARCHAR(MAX)		
	SELECT @id = CONVERT(NVARCHAR(MAX), ID) FROM #deleted	

	EXEC InsertIntoAuditLog 'VendorDevice', 'ID', @id, '1'
		
	DROP TABLE #deleted

END
GO
