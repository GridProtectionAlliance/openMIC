--  ----------------------------------------------------------------------------------------------------
--  openMIC Data Structures for SQL Server - Gbtc
--
--  Copyright © 2015, Grid Protection Alliance.  All Rights Reserved.
--
--  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
--  the NOTICE file distributed with this work for additional information regarding copyright ownership.
--  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
--  not use this file except in compliance with the License. You may obtain a copy of the License at:
--
--      http://www.opensource.org/licenses/MIT
--
--  Unless agreed to in writing, the subject software distributed under the License is distributed on an
--  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
--  License for the specific language governing permissions and limitations.
--
--  Schema Modification History:
--  ----------------------------------------------------------------------------------------------------
--  09/15/2015 - J. Ritchie Carroll
--       Derived original version of schema from GSFSchema.
--  ----------------------------------------------------------------------------------------------------

-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
--
-- Search for:
--     CREATE VIEW [dbo].[SchemaVersion] AS
--     SELECT 1 AS VersionNumber
--     GO
-- *******************************************************************************************

USE [master]
GO
CREATE DATABASE [openMIC];
GO
ALTER DATABASE [openMIC] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [openMIC] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [openMIC] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [openMIC] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [openMIC] SET ARITHABORT OFF 
GO
ALTER DATABASE [openMIC] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [openMIC] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [openMIC] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [openMIC] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [openMIC] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [openMIC] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [openMIC] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [openMIC] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [openMIC] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [openMIC] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [openMIC] SET  ENABLE_BROKER 
GO
ALTER DATABASE [openMIC] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [openMIC] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [openMIC] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [openMIC] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [openMIC] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [openMIC] SET  READ_WRITE 
GO
ALTER DATABASE [openMIC] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [openMIC] SET  MULTI_USER 
GO
ALTER DATABASE [openMIC] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [openMIC] SET DB_CHAINING OFF 
GO
-- The next three commented statements are used to create a user with access to the openMIC database.
-- Be sure to change the username and password.
-- Replace-all from NewUser to the desired username is the preferred method of changing the username.

--IF  NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = N'NewUser')
--CREATE LOGIN [NewUser] WITH PASSWORD=N'MyPassword', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
--GO
USE [openMIC]
GO
--CREATE USER [NewUser] FOR LOGIN [NewUser]
--GO
--CREATE ROLE [openMICManagerRole] AUTHORIZATION [dbo]
--GO
--EXEC sp_addrolemember N'openMICManagerRole', N'NewUser'
--GO
--EXEC sp_addrolemember N'db_datareader', N'openMICManagerRole'
--GO
--EXEC sp_addrolemember N'db_datawriter', N'openMICManagerRole'
--GO

-- *******************************************************************************************
-- IMPORTANT NOTE: When making updates to this schema, please increment the version number!
-- *******************************************************************************************
CREATE VIEW [dbo].[SchemaVersion] AS
SELECT 1 AS VersionNumber
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ErrorLog](
      [ID] [int] IDENTITY(1,1) NOT NULL,
      [Source] [varchar](200) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
      [Type] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
      [Message] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
      [Detail] [varchar](max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
      [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_ErrorLog_CreatedOn]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED 
(
      [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Runtime](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [SourceID] [int] NOT NULL,
    [SourceTable] [varchar](200) NOT NULL,
 CONSTRAINT [PK_Runtime] PRIMARY KEY CLUSTERED 
(
    [SourceID] ASC,
    [SourceTable] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Runtime] ON [dbo].[Runtime] 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AuditLog](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [TableName] [varchar](200) NOT NULL,
    [PrimaryKeyColumn] [varchar](200) NOT NULL,
    [PrimaryKeyValue] [varchar](max) NOT NULL,
    [ColumnName] [varchar](200) NOT NULL,
    [OriginalValue] [varchar](max) NULL,
    [NewValue] [varchar](max) NULL,
    [Deleted] [bit] NOT NULL CONSTRAINT [DF_AuditLog_Deleted]  DEFAULT ((0)),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_AuditLog_UpdatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_AuditLog_UpdatedOn]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_AuditLog] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Company](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [Acronym] [varchar](200) NOT NULL,
    [MapAcronym] [nchar](3) NOT NULL,
    [Name] [varchar](200) NOT NULL,
    [URL] [varchar](max) NULL,
    [LoadOrder] [int] NOT NULL CONSTRAINT [DF_Company_LoadOrder]  DEFAULT ((0)),
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_Company_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_Company_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_Company_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_Company_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TrackedChange](
    [ID] [bigint] IDENTITY(1,1) NOT NULL,
    [TableName] [varchar](200) NOT NULL,
    [PrimaryKeyColumn] [varchar](200) NOT NULL,
    [PrimaryKeyValue] [varchar](max) NULL,
 CONSTRAINT [PK_TrackedChange] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConfigurationEntity](
    [SourceName] [varchar](200) NOT NULL,
    [RuntimeName] [varchar](200) NOT NULL,
    [Description] [varchar](max) NULL,
    [LoadOrder] [int] NOT NULL CONSTRAINT [DF_ConfigurationEntity_LoadOrder]  DEFAULT ((0)),
    [Enabled] [bit] NOT NULL CONSTRAINT [DF_ConfigurationEntity_Enabled]  DEFAULT ((0))
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vendor](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [Acronym] [varchar](200) NULL,
    [Name] [varchar](200) NOT NULL,
    [PhoneNumber] [varchar](200) NULL,
    [ContactEmail] [varchar](200) NULL,
    [URL] [varchar](max) NULL,
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_Vendor_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_Vendor_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_Vendor_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_Vendor_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_Vendor] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Protocol](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [Acronym] [varchar](200) NOT NULL,
    [Name] [varchar](200) NOT NULL,
    [Type] [varchar](200) NOT NULL CONSTRAINT [DF_Protocol_Type] DEFAULT (N'File'),
    [Category] [varchar](200) NOT NULL CONSTRAINT [DF_Protocol_Category] DEFAULT (N'Power'),
    [AssemblyName] [varchar] (1024) NOT NULL CONSTRAINT [DF_Protocol_AssemblyName] DEFAULT (N'TestingAdapters.dll'),
    [TypeName] [varchar] (200) NOT NULL CONSTRAINT [DF_Protocol_TypeName] DEFAULT (N'TestingAdapters.VirtualInputAdapter'),
    [LoadOrder] [int] NOT NULL CONSTRAINT [DF_Protocol_LoadOrder] DEFAULT ((0)),
 CONSTRAINT [PK_Protocol] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SignalType](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [Name] [varchar](200) NOT NULL,
    [Acronym] [varchar](4) NOT NULL,
    [Suffix] [varchar](2) NOT NULL,
    [Abbreviation] [varchar](2) NOT NULL,
    [LongAcronym] [varchar](200) NOT NULL DEFAULT 'Undefined',
    [Source] [varchar](10) NOT NULL,
    [EngineeringUnits] [varchar](10) NULL,
 CONSTRAINT [PK_SignalType] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Interconnection](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [Acronym] [varchar](50) NOT NULL,
    [Name] [varchar](100) NOT NULL,
    [LoadOrder] [int] NULL CONSTRAINT [DF_Interconnection_LoadOrder]  DEFAULT ((0)),
 CONSTRAINT [PK_Interconnection] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Node](
    [ID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Node_ID]  DEFAULT (newid()),
    [Name] [varchar](200) NOT NULL,
    [CompanyID] [int] NULL,
    [Longitude] [decimal](9, 6) NULL,
    [Latitude] [decimal](9, 6) NULL,
    [Description] [varchar](max) NULL,
    [ImagePath] [varchar](max) NULL,
    [Settings] [varchar] (max) NULL,
    [MenuType] [varchar] (200) NOT NULL CONSTRAINT [DF_Node_MenuType] DEFAULT (N'File'),
    [MenuData] [varchar](max) NOT NULL CONSTRAINT [DF_Node_MenuData] DEFAULT (N'Menu.xml'),
    [Master] [bit] NOT NULL CONSTRAINT [DF_Node_Master]  DEFAULT ((0)),
    [LoadOrder] [int] NOT NULL CONSTRAINT [DF_Node_LoadOrder]  DEFAULT ((0)),
    [Enabled] [bit] NOT NULL CONSTRAINT [DF_Node_Enabled]  DEFAULT ((0)),
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_Node_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_Node_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_Node_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_Node_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_Node] PRIMARY KEY NONCLUSTERED 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Node_Name] ON [dbo].[Node]
(
    [Name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DataOperation](
    [NodeID] [uniqueidentifier] NULL,
    [Description] [varchar](max) NULL,
    [AssemblyName] [varchar](max) NOT NULL,
    [TypeName] [varchar](max) NOT NULL,
    [MethodName] [varchar](200) NOT NULL,
    [Arguments] [varchar](max) NULL,
    [LoadOrder] [int] NOT NULL CONSTRAINT [DF_DataOperation_LoadOrder]  DEFAULT ((0)),
    [Enabled] [bit] NOT NULL CONSTRAINT [DF_DataOperation_Enabled]  DEFAULT ((0))
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Device](
    [NodeID] [uniqueidentifier] NOT NULL,
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [UniqueID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Device_UniqueID]  DEFAULT (newid()),
    [Acronym] [varchar](200) NOT NULL,
    [Name] [varchar](200) NULL,
    [CompanyID] [int] NULL,
    [HistorianID] [int] NULL,
    [AccessID] [int] NOT NULL CONSTRAINT [DF_Device_AccessID]  DEFAULT ((0)),
    [VendorDeviceID] [int] NULL,
    [ProtocolID] [int] NULL,
    [Longitude] [decimal](9, 6) NULL,
    [Latitude] [decimal](9, 6) NULL,
    [InterconnectionID] [int] NULL,
    [ConnectionString] [varchar](max) NULL,
    [TimeZone] [varchar](200) NULL,
    [TimeAdjustmentTicks] [bigint] NOT NULL CONSTRAINT [DF_Device_TimeAdjustmentTicks]  DEFAULT ((0)),
    [TypicalDataFrequency] [int] NULL DEFAULT ((1)),
    [DataFrequencyUnits] [varchar](200) NULL DEFAULT (N'PerHour'),
    [MeasurementReportingInterval] [int] NOT NULL CONSTRAINT [DF_Device_MeasurementReportingInterval]  DEFAULT ((100000)),
    [ContactList] [nvarchar](max) NULL,
    [MeasuredLines] [int] NULL,
    [LoadOrder] [int] NOT NULL CONSTRAINT [DF_Device_LoadOrder]  DEFAULT ((0)),
    [Enabled] [bit] NOT NULL CONSTRAINT [DF_Device_Enabled]  DEFAULT ((0)),	
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_Device_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](50) NOT NULL CONSTRAINT [DF_Device_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_Device_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](50) NOT NULL CONSTRAINT [DF_Device_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_Device] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Device_Acronym] ON [dbo].[Device]
(
    [NodeID] ASC,
    [Acronym] ASC
)
WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DefaultValue](
    [NodeID] [uniqueidentifier] NOT NULL,
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [CompanyID] [int] NULL,
    [HistorianID] [int] NULL,
    [InterconnectionID] [int] NULL,
    [ContactList] [nvarchar](max) NULL,
    CONSTRAINT [PK_DefaultValue] PRIMARY KEY CLUSTERED ([ID] ASC) WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/*
    This triggers after INSERT and DELETE on [dbo].[Device_RuntimeSync] performs INSERT and DELETE on [dbo].[Runtime] table.	
*/

CREATE TRIGGER [dbo].[Device_RuntimeSync] 
   ON  [dbo].[Device]
   AFTER INSERT, DELETE
AS 
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- If a new record has been added
    IF EXISTS(SELECT * FROM INSERTED)
        BEGIN
            INSERT INTO Runtime (SourceID, SourceTable)
            SELECT ID, 'Device' FROM INSERTED
        END

    -- If a record has been deleted
    IF EXISTS(SELECT * FROM DELETED)
        BEGIN
            DELETE FROM Runtime WHERE SourceID IN (SELECT ID FROM DELETED) AND SourceTable = 'Device'
        END

END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VendorDevice](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [VendorID] [int] NOT NULL CONSTRAINT [DF_VendorDevice_VendorID]  DEFAULT ((10)),
    [Name] [varchar](200) NOT NULL,
    [Description] [varchar](max) NULL,
    [URL] [varchar](max) NULL,
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_VendorDevice_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_VendorDevice_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_VendorDevice_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_VendorDevice_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_VendorDevice] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Measurement](
    [PointID] [int] IDENTITY(1,1) NOT NULL,
    [SignalID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_Measurement_SignalID]  DEFAULT (newid()),
    [HistorianID] [int] NULL,	
    [DeviceID] [int] NULL,
    [PointTag] [varchar](200) NOT NULL,
    [AlternateTag] [varchar](max) NULL,
    [SignalTypeID] [int] NOT NULL,
    [SignalReference] [varchar](200) NOT NULL,
    [Adder] [float] NOT NULL CONSTRAINT [DF_Measurement_Adder]  DEFAULT ((0.0)),
    [Multiplier] [float] NOT NULL CONSTRAINT [DF_Measurement_Multiplier]  DEFAULT ((1.0)),
    [Description] [varchar](max) NULL,
    [Internal] [bit] NOT NULL CONSTRAINT [DF_Measurement_Internal]  DEFAULT ((1)),
    [Subscribed] [bit] NOT NULL CONSTRAINT [DF_Measurement_Subscribed]  DEFAULT ((0)),
    [Enabled] [bit] NOT NULL CONSTRAINT [DF_Measurement_Enabled]  DEFAULT ((0)),
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_Measurement_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_Measurement_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_Measurement_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_Measurement_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_Measurement] PRIMARY KEY CLUSTERED 
(
    [SignalID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Measurement_PointID] ON [dbo].[Measurement] 
(
    [PointID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Statistic](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [Source] [varchar](20) NOT NULL,
    [SignalIndex] [int] NOT NULL,
    [Name] [varchar](200) NOT NULL,
    [Description] [varchar](max) NULL,
    [AssemblyName] [varchar](max) NOT NULL,
    [TypeName] [varchar](max) NOT NULL,
    [MethodName] [varchar](200) NOT NULL,
    [Arguments] [varchar](max) NULL,
    [Enabled] [bit] NOT NULL CONSTRAINT [DF_Statistic_Enabled]  DEFAULT ((0)),
    [DataType] [varchar](200) NULL,
    [DisplayFormat] [varchar](200) NULL,
    [IsConnectedState] [bit] NOT NULL CONSTRAINT [DF_Statistic_IsConnectedState]  DEFAULT ((0)),
    [LoadOrder] [int] NOT NULL CONSTRAINT [DF_Statistic_LoadOrder]  DEFAULT ((0)),
 CONSTRAINT [PK_Statistic] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Statistic_Source_SignalIndex] ON [dbo].[Statistic] 
(
    [Source] ASC,
    [SignalIndex] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomActionAdapter](
    [NodeID] [uniqueidentifier] NOT NULL,
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [AdapterName] [varchar](200) NOT NULL,
    [AssemblyName] [varchar](max) NOT NULL,
    [TypeName] [varchar](max) NOT NULL,
    [ConnectionString] [varchar](max) NULL,
    [LoadOrder] [int] NOT NULL CONSTRAINT [DF_CustomActionAdapter_LoadOrder]  DEFAULT ((0)),
    [Enabled] [bit] NOT NULL CONSTRAINT [DF_CustomActionAdapter_Enabled]  DEFAULT ((0)),
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_CustomActionAdapter_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_CustomActionAdapter_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_CustomActionAdapter_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_CustomActionAdapter_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_CustomActionAdapter] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER [dbo].[CustomActionAdapter_RuntimeSync] 
   ON  [dbo].[CustomActionAdapter]
   AFTER INSERT, DELETE
AS 
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- If a new record has been added
    IF EXISTS(SELECT * FROM INSERTED)
        BEGIN
            INSERT INTO Runtime (SourceID, SourceTable)
            SELECT ID, 'CustomActionAdapter' FROM INSERTED
        END

    -- If a record has been deleted
    IF EXISTS(SELECT * FROM DELETED)
        BEGIN
            DELETE FROM Runtime WHERE SourceID IN (SELECT ID FROM DELETED) AND SourceTable = 'CustomActionAdapter'
        END

END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Historian](
    [NodeID] [uniqueidentifier] NOT NULL,
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [Acronym] [varchar](200) NOT NULL,
    [Name] [varchar](200) NULL,
    [AssemblyName] [varchar](max) NULL,
    [TypeName] [varchar](max) NULL,
    [ConnectionString] [varchar](max) NULL,
    [IsLocal] [bit] NOT NULL CONSTRAINT [DF_Historian_IsLocal]  DEFAULT ((1)),
    [MeasurementReportingInterval] [int] NOT NULL CONSTRAINT [DF_Historian_MeasurementReportingInterval]  DEFAULT ((100000)),
    [Description] [varchar](max) NULL,
    [LoadOrder] [int] NOT NULL CONSTRAINT [DF_Historian_LoadOrder]  DEFAULT ((0)),
    [Enabled] [bit] NOT NULL CONSTRAINT [DF_Historian_Enabled]  DEFAULT ((0)),
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_Historian_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_Historian_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_Historian_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_Historian_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_Historian] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE TRIGGER [dbo].[Historian_RuntimeSync] 
   ON  [dbo].[Historian]
   AFTER INSERT, DELETE
AS 
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- If a new record has been added
    IF EXISTS(SELECT * FROM INSERTED)
        BEGIN
            INSERT INTO Runtime (SourceID, SourceTable)
            SELECT ID, 'Historian' FROM INSERTED
        END

    -- If a record has been deleted
    IF EXISTS(SELECT * FROM DELETED)
        BEGIN
            DELETE FROM Runtime WHERE SourceID IN (SELECT ID FROM DELETED) AND SourceTable = 'Historian'
        END

END


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomInputAdapter](
    [NodeID] [uniqueidentifier] NOT NULL,
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [AdapterName] [varchar](200) NOT NULL,
    [AssemblyName] [varchar](max) NOT NULL,
    [TypeName] [varchar](max) NOT NULL,
    [ConnectionString] [varchar](max) NULL,
    [LoadOrder] [int] NOT NULL CONSTRAINT [DF_CustomInputAdapter_LoadOrder]  DEFAULT ((0)),
    [Enabled] [bit] NOT NULL CONSTRAINT [DF_CustomInputAdapter_Enabled]  DEFAULT ((0)),
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_CustomInputAdapter_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_CustomInputAdapter_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_CustomInputAdapter_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_CustomInputAdapter_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_CustomInputAdapter] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[CustomInputAdapter_RuntimeSync] 
   ON  [dbo].[CustomInputAdapter]
   AFTER INSERT, DELETE
AS 
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- If a new record has been added
    IF EXISTS(SELECT * FROM INSERTED)
        BEGIN
            INSERT INTO Runtime (SourceID, SourceTable)
            SELECT ID, 'CustomInputAdapter' FROM INSERTED
        END

    -- If a record has been deleted
    IF EXISTS(SELECT * FROM DELETED)
        BEGIN
            DELETE FROM Runtime WHERE SourceID IN (SELECT ID FROM DELETED) AND SourceTable = 'CustomInputAdapter'
        END

END

GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Alarm](
    [NodeID] [uniqueidentifier] NOT NULL,
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [TagName] [varchar](200) NOT NULL,
    [SignalID] [uniqueidentifier] NOT NULL,
    [AssociatedMeasurementID] [uniqueidentifier] NULL,
    [Description] [varchar](max) NULL,
    [Severity] [int] NOT NULL,
    [Operation] [int] NOT NULL,
    [SetPoint] [float] NULL,
    [Tolerance] [float] NULL,
    [Delay] [float] NULL,
    [Hysteresis] [float] NULL,
    [LoadOrder] [int] NOT NULL CONSTRAINT [DF_Alarm_LoadOrder]  DEFAULT ((0)),
    [Enabled] [bit] NOT NULL CONSTRAINT [DF_Alarm_Enabled]  DEFAULT ((0)),
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_Alarm_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_Alarm_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_Alarm_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_Alarm_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_Alarm] PRIMARY KEY NONCLUSTERED
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IX_Alarm_TagName] ON [dbo].[Alarm] 
(
    [TagName] ASC
) ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AlarmLog](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [SignalID] [uniqueidentifier] NOT NULL,
    [PreviousState] [int] NULL,
    [NewState] [int] NULL,
    [Ticks] [bigint] NOT NULL,
    [Timestamp] [DATETIME2] NOT NULL,
    [Value] [FLOAT] NOT NULL,
 CONSTRAINT [PK_AlarmLog] PRIMARY KEY NONCLUSTERED
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

CREATE NONCLUSTERED INDEX [IX_AlarmLog_SignalID] ON [dbo].[AlarmLog]
(
    [SignalID] ASC
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_AlarmLog_PreviousState] ON [dbo].[AlarmLog]
(
    [PreviousState] ASC
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_AlarmLog_NewState] ON [dbo].[AlarmLog]
(
    [NewState] ASC
) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [IX_AlarmLog_Timestamp] ON [dbo].[AlarmLog]
(
    [Timestamp] ASC
) ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomOutputAdapter](
    [NodeID] [uniqueidentifier] NOT NULL,
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [AdapterName] [varchar](200) NOT NULL,
    [AssemblyName] [varchar](max) NOT NULL,
    [TypeName] [varchar](max) NOT NULL,
    [ConnectionString] [varchar](max) NULL,
    [LoadOrder] [int] NOT NULL CONSTRAINT [DF_CustomOutputAdapter_LoadOrder]  DEFAULT ((0)),
    [Enabled] [bit] NOT NULL CONSTRAINT [DF_CustomOutputAdapter_Enabled]  DEFAULT ((0)),
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_CustomOutputAdapter_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_CustomOutputAdapter_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_CustomOutputAdapter_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_CustomOutputAdapter_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_CustomOutputAdapter] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TRIGGER [dbo].[CustomOutputAdapter_RuntimeSync] 
   ON  [dbo].[CustomOutputAdapter]
   AFTER INSERT, DELETE
AS 
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;

    -- If a new record has been added
    IF EXISTS(SELECT * FROM INSERTED)
        BEGIN
            INSERT INTO Runtime (SourceID, SourceTable)
            SELECT ID, 'CustomOutputAdapter' FROM INSERTED
        END

    -- If a record has been deleted
    IF EXISTS(SELECT * FROM DELETED)
        BEGIN
            DELETE FROM Runtime WHERE SourceID IN (SELECT ID FROM DELETED) AND SourceTable = 'CustomOutputAdapter'
        END

END
GO

-- Application Security Related Tables and Views

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[UserAccount](
    [ID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_UserAccount_ID]  DEFAULT (newid()),
    [Name] [varchar](200) NOT NULL,
    [Password] [varchar](200) NULL,
    [FirstName] [varchar](200) NULL,
    [LastName] [varchar](200) NULL,
    [DefaultNodeID] [uniqueidentifier] NOT NULL,
    [Phone] [varchar](200) NULL,
    [Email] [varchar](200) NULL,
    [LockedOut] [bit] NOT NULL CONSTRAINT [DF_UserAccount_LockedOut]  DEFAULT ((0)),
    [UseADAuthentication] [bit] NOT NULL CONSTRAINT [DF_UserAccount_UseADAuthentication] DEFAULT ((1)), 
    [ChangePasswordOn] [datetime] NULL CONSTRAINT [DF_UserAccount_ChangePasswordOn] DEFAULT (dateadd(day,(90),getutcdate())),
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_UserAccount_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](50) NOT NULL CONSTRAINT [DF_UserAccount_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_UserAccount_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](50) NOT NULL CONSTRAINT [DF_UserAccount_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_UserAccount] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_UserAccount] ON [dbo].[UserAccount]
(
    [Name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[UserAccount]  WITH CHECK ADD  CONSTRAINT [FK_UserAccount_Node] FOREIGN KEY([DefaultNodeID])
REFERENCES [dbo].[Node] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UserAccount] CHECK CONSTRAINT [FK_UserAccount_Node]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[ApplicationRole](
    [ID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_ApplicationRole_ID]  DEFAULT (newid()),
    [Name] [varchar](200) NOT NULL,
    [Description] [varchar](max) NULL,
    [NodeID] [uniqueidentifier] NOT NULL,
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_ApplicationRole_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_ApplicationRole_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_ApplicationRole_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_ApplicationRole_UpdatedBy]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_ApplicationRole] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_ApplicationRole] ON [dbo].[ApplicationRole]
(
    [NodeID] ASC,
    [Name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[ApplicationRole]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationRole_Node] FOREIGN KEY([NodeID])
REFERENCES [dbo].[Node] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ApplicationRole] CHECK CONSTRAINT [FK_ApplicationRole_Node]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[SecurityGroup](
    [ID] [uniqueidentifier] NOT NULL CONSTRAINT [DF_SecurityGroup_ID]  DEFAULT (newid()),
    [Name] [varchar](200) NOT NULL,
    [Description] [varchar](max) NULL,
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_SecurityGroup_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_SecurityGroup_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_SecurityGroup_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_SecurityGroup_UpdatedBy]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_SecurityGroup] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_SecurityGroup] ON [dbo].[SecurityGroup]
(
    [Name] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
GO

SET ANSI_PADDING OFF
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SecurityGroupUserAccount](
    [SecurityGroupID] [uniqueidentifier] NOT NULL,
    [UserAccountID] [uniqueidentifier] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[SecurityGroupUserAccount]  WITH CHECK ADD  CONSTRAINT [FK_SecurityGroupUserAccount_SecurityGroup] FOREIGN KEY([SecurityGroupID])
REFERENCES [dbo].[SecurityGroup] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SecurityGroupUserAccount] CHECK CONSTRAINT [FK_SecurityGroupUserAccount_SecurityGroup]
GO

ALTER TABLE [dbo].[SecurityGroupUserAccount]  WITH CHECK ADD  CONSTRAINT [FK_SecurityGroupUserAccount_UserAccount] FOREIGN KEY([UserAccountID])
REFERENCES [dbo].[UserAccount] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[SecurityGroupUserAccount] CHECK CONSTRAINT [FK_SecurityGroupUserAccount_UserAccount]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ApplicationRoleUserAccount](
    [ApplicationRoleID] [uniqueidentifier] NOT NULL,
    [UserAccountID] [uniqueidentifier] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ApplicationRoleUserAccount]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationRoleUserAccount_ApplicationRole] FOREIGN KEY([ApplicationRoleID])
REFERENCES [dbo].[ApplicationRole] ([ID])
GO

ALTER TABLE [dbo].[ApplicationRoleUserAccount] CHECK CONSTRAINT [FK_ApplicationRoleUserAccount_ApplicationRole]
GO

ALTER TABLE [dbo].[ApplicationRoleUserAccount]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationRoleUserAccount_UserAccount] FOREIGN KEY([UserAccountID])
REFERENCES [dbo].[UserAccount] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ApplicationRoleUserAccount] CHECK CONSTRAINT [FK_ApplicationRoleUserAccount_UserAccount]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ApplicationRoleSecurityGroup](
    [ApplicationRoleID] [uniqueidentifier] NOT NULL,
    [SecurityGroupID] [uniqueidentifier] NOT NULL
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ApplicationRoleSecurityGroup]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationRoleSecurityGroup_SecurityGroup] FOREIGN KEY([SecurityGroupID])
REFERENCES [dbo].[SecurityGroup] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ApplicationRoleSecurityGroup] CHECK CONSTRAINT [FK_ApplicationRoleSecurityGroup_SecurityGroup]
GO

ALTER TABLE [dbo].[ApplicationRoleSecurityGroup]  WITH CHECK ADD  CONSTRAINT [FK_ApplicationRoleSecurityGroup_ApplicationRole] FOREIGN KEY([ApplicationRoleID])
REFERENCES [dbo].[ApplicationRole] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ApplicationRoleSecurityGroup] CHECK CONSTRAINT [FK_ApplicationRoleSecurityGroup_ApplicationRole]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[AccessLog](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [UserName] [varchar](200) NOT NULL,
    [AccessGranted] [bit] NOT NULL,
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_AccessLog_Timestamp]  DEFAULT (getutcdate()),
 CONSTRAINT [PK_AccessLog] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[NodeInfo]
AS
SELECT     dbo.Node.ID AS NodeID, dbo.Node.Name, dbo.Company.Name AS CompanyName, dbo.Node.Longitude, dbo.Node.Latitude, dbo.Node.Description, 
                      dbo.Node.ImagePath, dbo.Node.Settings, dbo.Node.MenuType, dbo.Node.MenuData, dbo.Node.Master, dbo.Node.Enabled
FROM         dbo.Node WITH (NOLOCK) LEFT OUTER JOIN dbo.Company WITH (NOLOCK) ON dbo.Node.CompanyID = dbo.Company.ID
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[SecurityGroupUserAccountDetail]
AS
SELECT     dbo.SecurityGroupUserAccount.SecurityGroupID, dbo.SecurityGroupUserAccount.UserAccountID, UserAccount.Name AS UserName, UserAccount.FirstName, UserAccount.LastName, UserAccount.Email, 
                      dbo.SecurityGroup.Name AS SecurityGroupName, dbo.SecurityGroup.Description AS SecurityGroupDescription
FROM         dbo.SecurityGroupUserAccount WITH (NOLOCK) INNER JOIN
                      dbo.SecurityGroup WITH (NOLOCK) ON dbo.SecurityGroupUserAccount.SecurityGroupID = dbo.SecurityGroup.ID INNER JOIN
                      UserAccount WITH (NOLOCK) ON dbo.SecurityGroupUserAccount.UserAccountID = UserAccount.ID
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[AppRoleSecurityGroupDetail] 
AS
SELECT ApplicationRoleSecurityGroup.ApplicationRoleID, ApplicationRoleSecurityGroup.SecurityGroupID, ApplicationRole.Name AS ApplicationRoleName, ApplicationRole.Description AS ApplicationRoleDescription, SecurityGroup.Name AS SecurityGroupName, SecurityGroup.Description AS SecurityGroupDescription
FROM ApplicationRoleSecurityGroup WITH (NOLOCK), ApplicationRole WITH (NOLOCK), SecurityGroup WITH (NOLOCK)
WHERE ApplicationRoleSecurityGroup.ApplicationRoleID = ApplicationRole.ID AND ApplicationRoleSecurityGroup.SecurityGroupID = SecurityGroup.ID
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[AppRoleUserAccountDetail]
AS
SELECT     dbo.ApplicationRoleUserAccount.ApplicationRoleID, dbo.ApplicationRoleUserAccount.UserAccountID, UserAccount.Name AS UserName, UserAccount.FirstName, UserAccount.LastName, UserAccount.Email, 
                      dbo.ApplicationRole.Name AS ApplicationRoleName, dbo.ApplicationRole.Description AS ApplicationRoleDescription
FROM         dbo.ApplicationRoleUserAccount WITH (NOLOCK) INNER JOIN
                      dbo.ApplicationRole WITH (NOLOCK) ON dbo.ApplicationRoleUserAccount.ApplicationRoleID = dbo.ApplicationRole.ID INNER JOIN
                      UserAccount WITH (NOLOCK) ON dbo.ApplicationRoleUserAccount.UserAccountID = UserAccount.ID
GO

-- End of Application Security related tables and views definitions.

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[RuntimeHistorian]
AS
SELECT     TOP (100) PERCENT dbo.Historian.NodeID, dbo.Runtime.ID, dbo.Historian.Acronym AS AdapterName, COALESCE (NULLIF(LTRIM(RTRIM(dbo.Historian.AssemblyName)), ''), 
                      N'HistorianAdapters.dll') AS AssemblyName, COALESCE (NULLIF(LTRIM(RTRIM(dbo.Historian.TypeName)), ''), 
                      CASE dbo.Historian.IsLocal WHEN 1 THEN N'HistorianAdapters.LocalOutputAdapter' ELSE N'HistorianAdapters.RemoteOutputAdapter' END) 
                      AS TypeName, CASE WHEN Historian.ConnectionString IS NULL THEN N'' ELSE Historian.ConnectionString + N'; ' END + 
                      N'instanceName=' + dbo.Historian.Acronym + N'; sourceids=' + dbo.Historian.Acronym + N'; measurementReportingInterval=' +
                      CONVERT(NVARCHAR(10), dbo.Historian.MeasurementReportingInterval) AS ConnectionString
FROM         dbo.Historian WITH (NOLOCK) LEFT OUTER JOIN
                      dbo.Runtime WITH (NOLOCK) ON dbo.Historian.ID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'Historian'
WHERE     (dbo.Historian.Enabled <> 0)
ORDER BY dbo.Historian.LoadOrder

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[RuntimeDevice]
AS
SELECT     TOP (100) PERCENT dbo.Device.NodeID, dbo.Runtime.ID, dbo.Device.Acronym AS AdapterName, dbo.Protocol.AssemblyName, 
                      dbo.Protocol.TypeName, dbo.Device.ConnectionString AS ConnectionString
FROM         dbo.Device WITH (NOLOCK) LEFT OUTER JOIN
                      dbo.Protocol WITH (NOLOCK) ON dbo.Device.ProtocolID = dbo.Protocol.ID LEFT OUTER JOIN
                      dbo.Runtime WITH (NOLOCK) ON dbo.Device.ID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'Device'
WHERE     (dbo.Device.Enabled <> 0)
ORDER BY dbo.Device.LoadOrder

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[RuntimeCustomOutputAdapter]
AS
SELECT     TOP (100) PERCENT dbo.CustomOutputAdapter.NodeID, dbo.Runtime.ID, dbo.CustomOutputAdapter.AdapterName, 
                      LTRIM(RTRIM(dbo.CustomOutputAdapter.AssemblyName)) AS AssemblyName, LTRIM(RTRIM(dbo.CustomOutputAdapter.TypeName)) AS TypeName, dbo.CustomOutputAdapter.ConnectionString
FROM         dbo.CustomOutputAdapter WITH (NOLOCK) LEFT OUTER JOIN
                      dbo.Runtime WITH (NOLOCK) ON dbo.CustomOutputAdapter.ID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'CustomOutputAdapter'
WHERE     (dbo.CustomOutputAdapter.Enabled <> 0)
ORDER BY dbo.CustomOutputAdapter.LoadOrder

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[RuntimeCustomInputAdapter]
AS
SELECT     TOP (100) PERCENT dbo.CustomInputAdapter.NodeID, dbo.Runtime.ID, dbo.CustomInputAdapter.AdapterName, 
                      LTRIM(RTRIM(dbo.CustomInputAdapter.AssemblyName)) AS AssemblyName, LTRIM(RTRIM(dbo.CustomInputAdapter.TypeName)) AS TypeName, dbo.CustomInputAdapter.ConnectionString
FROM         dbo.CustomInputAdapter WITH (NOLOCK) LEFT OUTER JOIN
                      dbo.Runtime WITH (NOLOCK) ON dbo.CustomInputAdapter.ID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'CustomInputAdapter'
WHERE     (dbo.CustomInputAdapter.Enabled <> 0)
ORDER BY dbo.CustomInputAdapter.LoadOrder

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[RuntimeCustomActionAdapter]
AS
SELECT     TOP (100) PERCENT dbo.CustomActionAdapter.NodeID, dbo.Runtime.ID, dbo.CustomActionAdapter.AdapterName, 
                      LTRIM(RTRIM(dbo.CustomActionAdapter.AssemblyName)) AS AssemblyName, LTRIM(RTRIM(dbo.CustomActionAdapter.TypeName)) AS TypeName, dbo.CustomActionAdapter.ConnectionString
FROM         dbo.CustomActionAdapter WITH (NOLOCK) LEFT OUTER JOIN
                      dbo.Runtime WITH (NOLOCK) ON dbo.CustomActionAdapter.ID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'CustomActionAdapter'
WHERE     (dbo.CustomActionAdapter.Enabled <> 0)
ORDER BY dbo.CustomActionAdapter.LoadOrder

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[MeasurementDetail]
AS
SELECT     dbo.Device.CompanyID, dbo.Company.Acronym AS CompanyAcronym, dbo.Company.Name AS CompanyName, dbo.Measurement.SignalID, 
                      dbo.Measurement.HistorianID, dbo.Historian.Acronym AS HistorianAcronym, dbo.Historian.ConnectionString AS HistorianConnectionString, 
                      dbo.Measurement.PointID, dbo.Measurement.PointTag, dbo.Measurement.AlternateTag, dbo.Measurement.DeviceID,  COALESCE (dbo.Device.NodeID, dbo.Historian.NodeID) AS NodeID, 
                      dbo.Device.Acronym AS DeviceAcronym, dbo.Device.Name AS DeviceName, COALESCE(dbo.Device.TypicalDataFrequency, 1) AS TypicalDataFrequency, dbo.Device.DataFrequencyUnits AS DataFrequencyUnits,
                      dbo.Device.Enabled AS DeviceEnabled, dbo.Device.ContactList, dbo.Device.VendorDeviceID, dbo.VendorDevice.Name AS VendorDeviceName, dbo.VendorDevice.Description AS VendorDeviceDescription, 
                      dbo.Device.ProtocolID, dbo.Protocol.Acronym AS ProtocolAcronym, dbo.Protocol.Name AS ProtocolName, dbo.Measurement.SignalTypeID, 
                      dbo.Measurement.SignalReference, dbo.Measurement.Adder, dbo.Measurement.Multiplier, dbo.Measurement.Description, dbo.Measurement.Subscribed,
                      dbo.Measurement.Internal, dbo.Measurement.Enabled, COALESCE (dbo.SignalType.EngineeringUnits, N'') AS EngineeringUnits, dbo.SignalType.Source, dbo.SignalType.Acronym AS SignalAcronym, 
                      dbo.SignalType.Name AS SignalName, dbo.SignalType.Suffix AS SignalTypeSuffix, dbo.Device.Longitude, dbo.Device.Latitude,
                      COALESCE(Historian.Acronym, Device.Acronym, '__') + ':' + CONVERT(NVARCHAR(10), Measurement.PointID) AS ID, Measurement.UpdatedOn
FROM         dbo.Company WITH (NOLOCK) RIGHT OUTER JOIN
                      dbo.Device WITH (NOLOCK) ON dbo.Company.ID = dbo.Device.CompanyID RIGHT OUTER JOIN
                      dbo.Measurement WITH (NOLOCK) LEFT OUTER JOIN
                      dbo.SignalType WITH (NOLOCK) ON dbo.Measurement.SignalTypeID = dbo.SignalType.ID ON dbo.Device.ID = dbo.Measurement.DeviceID LEFT OUTER JOIN
                      dbo.VendorDevice WITH (NOLOCK) ON dbo.Device.VendorDeviceID = dbo.VendorDevice.ID LEFT OUTER JOIN
                      dbo.Protocol WITH (NOLOCK) ON dbo.Device.ProtocolID = dbo.Protocol.ID LEFT OUTER JOIN
                      dbo.Historian WITH (NOLOCK) ON dbo.Measurement.HistorianID = dbo.Historian.ID


GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ActiveMeasurement]
AS
SELECT    dbo.Node.ID AS NodeID, COALESCE(dbo.Device.NodeID, dbo.Historian.NodeID) AS SourceNodeID, COALESCE(dbo.Historian.Acronym, dbo.Device.Acronym, '__') + ':' + CONVERT(NVARCHAR(10), dbo.Measurement.PointID) AS ID, dbo.Measurement.SignalID, 
                      dbo.Measurement.PointTag, dbo.Measurement.AlternateTag, dbo.Measurement.SignalReference, dbo.Measurement.Internal, dbo.Measurement.Subscribed, dbo.Device.Acronym AS Device, dbo.Runtime.ID AS DeviceID,
                      COALESCE(dbo.Device.TypicalDataFrequency, 1) AS TypicalDataFrequency, dbo.Device.DataFrequencyUnits AS DataFrequencyUnits, dbo.Protocol.Acronym AS Protocol, dbo.Protocol.Type AS ProtocolType, dbo.SignalType.Acronym AS SignalType, dbo.SignalType.EngineeringUnits,
                      dbo.Measurement.Adder, dbo.Measurement.Multiplier, dbo.Company.Acronym AS Company, dbo.Device.Longitude, 
                      dbo.Device.Latitude, dbo.Measurement.Description, dbo.Measurement.UpdatedOn
FROM         dbo.Company WITH (NOLOCK) RIGHT OUTER JOIN
                      dbo.Device WITH (NOLOCK) ON dbo.Company.ID = dbo.Device.CompanyID RIGHT OUTER JOIN
                      dbo.Measurement WITH (NOLOCK) LEFT OUTER JOIN
                      dbo.SignalType WITH (NOLOCK) ON dbo.Measurement.SignalTypeID = dbo.SignalType.ID ON dbo.Device.ID = dbo.Measurement.DeviceID LEFT OUTER JOIN
                      dbo.Protocol WITH (NOLOCK) ON dbo.Device.ProtocolID = dbo.Protocol.ID LEFT OUTER JOIN
                      dbo.Historian WITH (NOLOCK) ON dbo.Measurement.HistorianID = dbo.Historian.ID LEFT OUTER JOIN
                      dbo.Runtime WITH (NOLOCK) ON dbo.Device.ID = dbo.Runtime.SourceID AND dbo.Runtime.SourceTable = N'Device' CROSS JOIN
                      dbo.Node WITH (NOLOCK)
WHERE     (dbo.Device.Enabled <> 0 OR dbo.Device.Enabled IS NULL) AND (dbo.Measurement.Enabled <> 0)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[RuntimeStatistic]
AS
SELECT		dbo.Node.ID AS NodeID, dbo.Statistic.ID AS ID, dbo.Statistic.Source, dbo.Statistic.SignalIndex, dbo.Statistic.Name, dbo.Statistic.Description,
                    dbo.Statistic.AssemblyName, dbo.Statistic.TypeName, dbo.Statistic.MethodName, dbo.Statistic.Arguments, dbo.Statistic.IsConnectedState, dbo.Statistic.DataType, 
                      dbo.Statistic.DisplayFormat, dbo.Statistic.Enabled
FROM dbo.Statistic WITH (NOLOCK), dbo.Node WITH (NOLOCK)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[IaonOutputAdapter]
AS
SELECT     NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM         dbo.RuntimeHistorian WITH (NOLOCK)
UNION
SELECT     NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM         dbo.RuntimeCustomOutputAdapter WITH (NOLOCK)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[IaonInputAdapter]
AS
SELECT     NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM         dbo.RuntimeDevice WITH (NOLOCK)
UNION
SELECT     NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM         dbo.RuntimeCustomInputAdapter WITH (NOLOCK)

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[IaonActionAdapter]
AS
SELECT     NodeID, ID, AdapterName, AssemblyName, TypeName, ConnectionString
FROM         dbo.RuntimeCustomActionAdapter WITH (NOLOCK)

GO
CREATE VIEW [dbo].[HistorianMetadata]
AS
SELECT 
    HistorianID             = PointID,
    DataType                = CASE SignalAcronym WHEN 'DIGI' THEN 1 ELSE 0 END,
    [Name]                  = PointTag,
    Synonym1                = SignalReference,
    Synonym2                = SignalAcronym,
    Synonym3                = AlternateTag,
    Description             = Description,
    HardwareInfo            = VendorDeviceDescription,    
    Remarks                 = '',
    PlantCode               = HistorianAcronym,
    UnitNumber              = 1,
    SystemName              = DeviceAcronym,
    SourceID                = ProtocolID,
    Enabled                 = Enabled,
    ScanRate                = 1.0 / TypicalDataFrequency,
    CompressionMinTime      = 0,
    CompressionMaxTime      = 0,
    EngineeringUnits        = EngineeringUnits,
    LowWarning              = 0,
    HighWarning             = 0,
    LowAlarm                = 0,
    HighAlarm               = 0,
    LowRange                = 0,
    HighRange               = 0,
    CompressionLimit        = 0.0,
    ExceptionLimit          = 0.0,
    DisplayDigits           = CASE SignalAcronym WHEN 'DIGI' THEN 0 ELSE 7 END,
    SetDescription          = '',
    ClearDescription        = '',
    AlarmState              = 0,
    ChangeSecurity          = 5,
    AccessSecurity          = 0,
    StepCheck               = 0,
    AlarmEnabled            = 0,
    AlarmFlags              = 0,
    AlarmDelay              = 0,
    AlarmToFile             = 0,
    AlarmByEmail            = 0,
    AlarmByPager            = 0,
    AlarmByPhone            = 0,
    AlarmEmails             = MeasurementDetail.ContactList,
    AlarmPagers             = '',
    AlarmPhones             = ''
FROM [dbo].[MeasurementDetail] WITH (NOLOCK)

GO
CREATE VIEW [dbo].[CurrentAlarmState] AS
SELECT
    SignalsWithAlarms.SignalID,
    CurrentState.NewState AS State,
    CurrentState.Timestamp,
    CurrentState.Value
FROM
    (
        SELECT DISTINCT Measurement.SignalID
        FROM Measurement JOIN Alarm ON Measurement.SignalID = Alarm.SignalID
        WHERE Alarm.Enabled <> 0
    ) AS SignalsWithAlarms
    LEFT OUTER JOIN
    (
        SELECT
            Log1.SignalID,
            Log1.NewState,
            Log1.Timestamp,
            Log1.Value
        FROM
            AlarmLog AS Log1 LEFT OUTER JOIN
            AlarmLog AS Log2 ON Log1.SignalID = Log2.SignalID AND Log1.Ticks < Log2.Ticks
        WHERE
            Log2.ID IS NULL
    ) AS CurrentState
    ON SignalsWithAlarms.SignalID = CurrentState.SignalID

GO
CREATE VIEW [dbo].[VendorDeviceDetail]
AS
SELECT     VD.ID, VD.VendorID, VD.Name, ISNULL(VD.Description, '') AS Description, ISNULL(VD.URL, '') AS URL, V.Name AS VendorName, 
                      V.Acronym AS VendorAcronym
FROM         dbo.VendorDevice AS VD WITH (NOLOCK) INNER JOIN
                      dbo.Vendor AS V WITH (NOLOCK) ON VD.VendorID = V.ID
GO
CREATE VIEW [dbo].[DeviceDetail]
AS
SELECT     D.NodeID, D.ID, D.UniqueID, D.Acronym, ISNULL(D.Name, '') AS Name, D.CompanyID, D.HistorianID, D.AccessID, D.VendorDeviceID, 
                      D.ProtocolID, D.Longitude, D.Latitude, D.InterconnectionID, ISNULL(D.ConnectionString, '') AS ConnectionString, ISNULL(D.TimeZone, '') AS TimeZone, 
                      ISNULL(D.TypicalDataFrequency, 30) AS TypicalDataFrequency, D.DataFrequencyUnits ASDataFrequencyUnits, D.TimeAdjustmentTicks,
                      ISNULL(D.ContactList, '') AS ContactList, D.MeasuredLines, D.LoadOrder, D.Enabled, ISNULL(C.Name, '') AS CompanyName, 
                      ISNULL(C.Acronym, '') AS CompanyAcronym, ISNULL(C.MapAcronym, '') AS CompanyMapAcronym, ISNULL(H.Acronym, '') AS HistorianAcronym, 
                      ISNULL(VD.VendorAcronym, '') AS VendorAcronym, ISNULL(VD.Name, '') AS VendorDeviceName, ISNULL(P.Name, '') AS ProtocolName, 
                      P.Type AS ProtocolType, P.Category, ISNULL(I.Name, '') AS InterconnectionName, N.Name AS NodeName, D.CreatedOn, D.UpdatedOn
FROM         dbo.Device AS D WITH (NOLOCK) LEFT OUTER JOIN
                      dbo.Company AS C WITH (NOLOCK) ON C.ID = D.CompanyID LEFT OUTER JOIN
                      dbo.Historian AS H WITH (NOLOCK) ON H.ID = D.HistorianID LEFT OUTER JOIN
                      dbo.VendorDeviceDetail AS VD WITH (NOLOCK) ON VD.ID = D.VendorDeviceID LEFT OUTER JOIN
                      dbo.Protocol AS P WITH (NOLOCK) ON P.ID = D.ProtocolID LEFT OUTER JOIN
                      dbo.Interconnection AS I WITH (NOLOCK) ON I.ID = D.InterconnectionID LEFT OUTER JOIN
                      dbo.Node AS N WITH (NOLOCK) ON N.ID = D.NodeID
GO
CREATE VIEW [dbo].[HistorianDetail] AS
SELECT     H.NodeID, H.ID, H.Acronym, ISNULL(H.Name, '') AS Name, ISNULL(H.AssemblyName, '') AS AssemblyName, ISNULL(H.TypeName, '') AS TypeName, 
                      ISNULL(H.ConnectionString, '') AS ConnectionString, H.IsLocal, ISNULL(H.Description, '') AS Description, H.LoadOrder, H.Enabled, 
                      N.Name AS NodeName, H.MeasurementReportingInterval
FROM         dbo.Historian AS H WITH (NOLOCK) INNER JOIN
                      dbo.Node AS N WITH (NOLOCK) ON H.NodeID = N.ID
GO
CREATE VIEW [dbo].[NodeDetail] AS
SELECT N.ID, N.Name, ISNULL(N.CompanyID, 0) AS CompanyID, ISNULL(N.Longitude, 0) AS Longitude, ISNULL(N.Latitude, 0) AS Latitude, 
        ISNULL(N.Description, '') AS Description, ISNULL(N.ImagePath, '') AS ImagePath, ISNULL(N.Settings, '') AS Settings, N.MenuType, N.MenuData,
        N.Master, N.LoadOrder, N.Enabled, ISNULL(C.Name, '') AS CompanyName
FROM Node N WITH (NOLOCK) LEFT JOIN Company C WITH (NOLOCK)
ON N.CompanyID = C.ID
GO
CREATE VIEW [dbo].[VendorDetail] AS
Select ID, ISNULL(Acronym, '') AS Acronym, Name, ISNULL(PhoneNumber, '') AS PhoneNumber, ISNULL(ContactEmail, '') AS ContactEmail, ISNULL(URL, '') AS URL 
FROM Vendor WITH (NOLOCK)
GO
CREATE VIEW [dbo].[CustomActionAdapterDetail] AS
SELECT     CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, ISNULL(CA.ConnectionString, '') AS ConnectionString, CA.LoadOrder, 
                      CA.Enabled, N.Name AS NodeName
FROM         dbo.CustomActionAdapter AS CA WITH (NOLOCK) INNER JOIN
                      dbo.Node AS N WITH (NOLOCK) ON CA.NodeID = N.ID
GO
CREATE VIEW [dbo].[CustomInputAdapterDetail] AS
SELECT     CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, ISNULL(CA.ConnectionString, '') AS ConnectionString, CA.LoadOrder, 
                      CA.Enabled, N.Name AS NodeName
FROM         dbo.CustomInputAdapter AS CA WITH (NOLOCK) INNER JOIN
                      dbo.Node AS N WITH (NOLOCK) ON CA.NodeID = N.ID
GO
CREATE VIEW [dbo].[CustomOutputAdapterDetail] AS
SELECT     CA.NodeID, CA.ID, CA.AdapterName, CA.AssemblyName, CA.TypeName, ISNULL(CA.ConnectionString, '') AS ConnectionString, CA.LoadOrder, 
                      CA.Enabled, N.Name AS NodeName
FROM         dbo.CustomOutputAdapter AS CA WITH (NOLOCK) INNER JOIN
                      dbo.Node AS N WITH (NOLOCK) ON CA.NodeID = N.ID
GO
CREATE VIEW [dbo].[IaonTreeView] AS
SELECT     'Action Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, ISNULL(ConnectionString, '') AS ConnectionString
FROM         dbo.IaonActionAdapter WITH (NOLOCK)
UNION ALL
SELECT     'Input Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, ISNULL(ConnectionString, '') AS ConnectionString
FROM         dbo.IaonInputAdapter WITH (NOLOCK)
UNION ALL
SELECT     'Output Adapters' AS AdapterType, NodeID, ID, AdapterName, AssemblyName, TypeName, ISNULL(ConnectionString, '') AS ConnectionString
FROM         dbo.IaonOutputAdapter WITH (NOLOCK)
GO
CREATE VIEW [dbo].[VendorDeviceDistribution] AS
SELECT     dbo.Device.NodeID, dbo.Vendor.Name AS VendorName, COUNT(*) AS DeviceCount
FROM       dbo.Device WITH (NOLOCK) LEFT OUTER JOIN
           dbo.VendorDevice WITH (NOLOCK) ON dbo.Device.VendorDeviceID = dbo.VendorDevice.ID INNER JOIN
           dbo.Vendor WITH (NOLOCK) ON dbo.VendorDevice.VendorID = dbo.Vendor.ID
GROUP BY dbo.Device.NodeID, dbo.Vendor.Name
GO

CREATE VIEW [dbo].[StatisticMeasurement] AS
SELECT *
FROM MeasurementDetail WITH (NOLOCK) 
WHERE SignalAcronym = 'STAT'
GO

CREATE VIEW [dbo].[TrackedTable] AS
SELECT 'Measurement' AS Name
UNION
SELECT 'ActiveMeasurement' AS Name
UNION
SELECT 'Device' AS Name
GO

ALTER TABLE [dbo].[Node]  WITH CHECK ADD  CONSTRAINT [FK_Node_Company] FOREIGN KEY([CompanyID])
REFERENCES [dbo].[Company] ([ID])
GO
ALTER TABLE [dbo].[DataOperation]  WITH CHECK ADD  CONSTRAINT [FK_DataOperation_Node] FOREIGN KEY([NodeID])
REFERENCES [dbo].[Node] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_Company] FOREIGN KEY([CompanyID])
REFERENCES [dbo].[Company] ([ID])
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_Interconnection] FOREIGN KEY([InterconnectionID])
REFERENCES [dbo].[Interconnection] ([ID])
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_Node] FOREIGN KEY([NodeID])
REFERENCES [dbo].[Node] ([ID])
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_Protocol] FOREIGN KEY([ProtocolID])
REFERENCES [dbo].[Protocol] ([ID])
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_VendorDevice] FOREIGN KEY([VendorDeviceID])
REFERENCES [dbo].[VendorDevice] ([ID])
GO
ALTER TABLE [dbo].[DefaultValue]  WITH CHECK ADD  CONSTRAINT [FK_DefaultValue_Node] FOREIGN KEY([NodeID])
REFERENCES [dbo].[Node] ([ID])
GO
ALTER TABLE [dbo].[DefaultValue]  WITH CHECK ADD  CONSTRAINT [FK_DefaultValue_Company] FOREIGN KEY([CompanyID])
REFERENCES [dbo].[Company] ([ID])
GO
ALTER TABLE [dbo].[DefaultValue]  WITH CHECK ADD  CONSTRAINT [FK_DefaultValue_Historian] FOREIGN KEY([HistorianID])
REFERENCES [dbo].[Historian] ([ID])
GO
ALTER TABLE [dbo].[DefaultValue]  WITH CHECK ADD  CONSTRAINT [FK_DefaultValue_Interconnection] FOREIGN KEY([InterconnectionID])
REFERENCES [dbo].[Interconnection] ([ID])
GO
ALTER TABLE [dbo].[VendorDevice]  WITH CHECK ADD  CONSTRAINT [FK_VendorDevice_Vendor] FOREIGN KEY([VendorID])
REFERENCES [dbo].[Vendor] ([ID])
GO
ALTER TABLE [dbo].[Measurement]  WITH CHECK ADD  CONSTRAINT [FK_Measurement_Device] FOREIGN KEY([DeviceID])
REFERENCES [dbo].[Device] ([ID])
GO
ALTER TABLE [dbo].[Measurement]  WITH CHECK ADD  CONSTRAINT [FK_Measurement_SignalType] FOREIGN KEY([SignalTypeID])
REFERENCES [dbo].[SignalType] ([ID])
GO
ALTER TABLE [dbo].[CustomActionAdapter]  WITH CHECK ADD  CONSTRAINT [FK_CustomActionAdapter_Node] FOREIGN KEY([NodeID])
REFERENCES [dbo].[Node] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Historian]  WITH CHECK ADD  CONSTRAINT [FK_Historian_Node] FOREIGN KEY([NodeID])
REFERENCES [dbo].[Node] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CustomInputAdapter]  WITH CHECK ADD  CONSTRAINT [FK_CustomInputAdapter_Node] FOREIGN KEY([NodeID])
REFERENCES [dbo].[Node] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CustomOutputAdapter]  WITH CHECK ADD  CONSTRAINT [FK_CustomOutputAdapter_Node] FOREIGN KEY([NodeID])
REFERENCES [dbo].[Node] ([ID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Alarm]  WITH CHECK ADD  CONSTRAINT [FK_Alarm_Node] FOREIGN KEY([NodeID])
REFERENCES [dbo].[Node] ([ID])
GO
ALTER TABLE [dbo].[Alarm]  WITH CHECK ADD  CONSTRAINT [FK_Alarm_Measurement_SignalID] FOREIGN KEY([SignalID])
REFERENCES [dbo].[Measurement] ([SignalID])
GO
ALTER TABLE [dbo].[Alarm]  WITH CHECK ADD  CONSTRAINT [FK_Alarm_Measurement_AssociatedMeasurementID] FOREIGN KEY([SignalID])
REFERENCES [dbo].[Measurement] ([SignalID])
GO
ALTER TABLE [dbo].[AlarmLog]  WITH CHECK ADD  CONSTRAINT [FK_AlarmLog_Measurement] FOREIGN KEY([SignalID])
REFERENCES [dbo].[Measurement] ([SignalID])
GO
ALTER TABLE [dbo].[AlarmLog]  WITH CHECK ADD  CONSTRAINT [FK_AlarmLog_Alarm_PreviousState] FOREIGN KEY([PreviousState])
REFERENCES [dbo].[Alarm] ([ID])
GO
ALTER TABLE [dbo].[AlarmLog]  WITH CHECK ADD  CONSTRAINT [FK_AlarmLog_Alarm_NewState] FOREIGN KEY([NewState])
REFERENCES [dbo].[Alarm] ([ID])
GO

-- Triggers to clear references on delete.
-- SQL Server is strangely picky about cyclic cascade paths and multiple
-- cascade paths so triggers must be used instead of ON DELETE CASCADE.
CREATE TRIGGER [dbo].[Node_ClearReferences]
    ON [dbo].[Node]
    INSTEAD OF DELETE
AS
BEGIN
    DELETE FROM Alarm WHERE NodeID IN (SELECT ID FROM deleted)
    DELETE FROM Node WHERE ID IN (SELECT ID FROM deleted)
END
GO

CREATE TRIGGER [dbo].[Device_ClearReferences]
    ON [dbo].[Device]
    INSTEAD OF DELETE
AS
BEGIN
    DELETE FROM Measurement WHERE DeviceID IN (SELECT ID FROM deleted)
    DELETE FROM Device WHERE ID IN (SELECT ID FROM deleted)
END
GO

CREATE TRIGGER [dbo].[Measurement_ClearReferences]
    ON [dbo].[Measurement]
    INSTEAD OF DELETE
AS
BEGIN
    DELETE FROM AlarmLog WHERE SignalID IN (SELECT SignalID FROM deleted)
    DELETE FROM Alarm WHERE SignalID IN (SELECT SignalID FROM deleted) OR AssociatedMeasurementID IN (SELECT SignalID FROM deleted)
    DELETE FROM Measurement WHERE SignalID IN (SELECT SignalID FROM deleted)
END
GO

CREATE TRIGGER [dbo].[Alarm_ClearReferences]
    ON [dbo].[Alarm]
    INSTEAD OF DELETE
AS
BEGIN
    DELETE FROM AlarmLog WHERE PreviousState IN (SELECT ID FROM deleted) OR NewState IN (SELECT ID FROM deleted)
    DELETE FROM Alarm WHERE ID IN (SELECT ID FROM deleted)
END
GO

-- ***********************
-- Company Change Tracking
-- ***********************

CREATE TRIGGER [dbo].[Company_UpdateTracker] 
   ON  [dbo].[Company]
   AFTER UPDATE
AS 
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;
    
    SELECT inserted.Acronym INTO #acronym
    FROM inserted INNER JOIN deleted ON inserted.ID = deleted.ID
    WHERE inserted.Acronym <> deleted.Acronym
    
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue)
    SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM ActiveMeasurement INNER JOIN #acronym a ON ActiveMeasurement.Company = a.Acronym
    
    DROP TABLE #acronym
END
GO

-- **********************
-- Device Change Tracking
-- **********************

CREATE TRIGGER [dbo].[Device_UpdateTracker] 
   ON  [dbo].[Device]
   AFTER INSERT, UPDATE, DELETE
AS 
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;
    
    -- Track changes made to the Device table
    SELECT ID INTO #deviceUpdate FROM inserted
    UNION
    SELECT ID FROM deleted
    
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue)
    SELECT 'Device', 'ID', ID FROM #deviceUpdate
    
    DROP TABLE #deviceUpdate
    
    -- Track changes made to the ActiveMeasurement view
    SELECT inserted.ID INTO #activeMeasurementUpdate
    FROM inserted INNER JOIN deleted ON inserted.ID = deleted.ID
    WHERE inserted.NodeID <> deleted.NodeID
       OR inserted.Acronym <> deleted.Acronym
       OR inserted.TypicalDataFrequency <> deleted.TypicalDataFrequency
       OR inserted.Longitude <> deleted.Longitude
       OR inserted.Latitude <> deleted.Latitude
       OR inserted.CompanyID <> deleted.CompanyID
       OR inserted.ProtocolID <> deleted.ProtocolID
       OR inserted.Enabled <> deleted.Enabled
    
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue)
    SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM Measurement INNER JOIN #activeMeasurementUpdate am ON Measurement.DeviceID = am.ID
    
    DROP TABLE #activeMeasurementUpdate
END
GO

-- *************************
-- Historian Change Tracking
-- *************************

CREATE TRIGGER [dbo].[Historian_UpdateTracker] 
   ON  [dbo].[Historian]
   AFTER UPDATE
AS 
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;
    
    SELECT inserted.ID INTO #historianID
    FROM inserted INNER JOIN deleted ON inserted.ID = deleted.ID
    WHERE inserted.NodeID <> deleted.NodeID
       OR inserted.Acronym <> deleted.Acronym
    
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue)
    SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM Measurement INNER JOIN #historianID h ON Measurement.HistorianID = h.ID
    
    DROP TABLE #historianID
END
GO

-- ***************************
-- Measurement Change Tracking
-- ***************************

CREATE TRIGGER [dbo].[Measurement_UpdateTracker] 
   ON  [dbo].[Measurement]
   AFTER INSERT, UPDATE, DELETE
AS 
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;
    
    SELECT PointID INTO #pointID FROM inserted
    UNION
    SELECT PointID FROM deleted
    
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue)
    SELECT 'Measurement', 'PointID', PointID FROM #pointID
    
    DROP TABLE #pointID
    
    SELECT SignalID INTO #signalID FROM inserted
    UNION
    SELECT SignalID FROM deleted
    
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue)
    SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM #signalID
    
    DROP TABLE #signalID
END
GO

-- ************************
-- Protocol Change Tracking
-- ************************

CREATE TRIGGER [dbo].[Protocol_UpdateTracker] 
   ON  [dbo].[Protocol]
   AFTER UPDATE
AS 
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;
    
    SELECT inserted.Acronym INTO #acronym
    FROM inserted INNER JOIN deleted ON inserted.ID = deleted.ID
    WHERE inserted.Acronym <> deleted.Acronym
       OR inserted.Type <> deleted.Type
    
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue)
    SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM ActiveMeasurement INNER JOIN #acronym a ON ActiveMeasurement.Protocol = a.Acronym
    
    DROP TABLE #acronym
END
GO

-- **************************
-- SignalType Change Tracking
-- **************************

CREATE TRIGGER [dbo].[SignalType_UpdateTracker] 
   ON  [dbo].[SignalType]
   AFTER UPDATE
AS 
BEGIN
    -- SET NOCOUNT ON added to prevent extra result sets from
    -- interfering with SELECT statements.
    SET NOCOUNT ON;
    
    SELECT inserted.ID INTO #signalTypeID
    FROM inserted INNER JOIN deleted ON inserted.ID = deleted.ID
    WHERE inserted.Acronym <> deleted.Acronym
    
    INSERT INTO TrackedChange(TableName, PrimaryKeyColumn, PrimaryKeyValue)
    SELECT 'ActiveMeasurement', 'SignalID', SignalID FROM Measurement INNER JOIN #signalTypeID s ON Measurement.SignalTypeID = s.ID
    
    DROP TABLE #signalTypeID
END
GO
