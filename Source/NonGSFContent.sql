SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConnectionProfile](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [Name] [varchar](200) NOT NULL,
    [Description] [varchar](max) NULL,
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_ConnectionProfile_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_ConnectionProfile_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_ConnectionProfile_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_ConnectionProfile_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_ConnectionProfile] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ConnectionProfileTask](
    [ID] [int] IDENTITY(1,1) NOT NULL,
    [ConnectionProfileID] [int] NOT NULL CONSTRAINT [DF_ConnectionProfileTask_ConnectionProfileID] DEFAULT (0),
    [Name] [varchar](200) NOT NULL,
    [Settings] [varchar](max) NULL,
    [CreatedOn] [datetime] NOT NULL CONSTRAINT [DF_ConnectionProfileTask_CreatedOn]  DEFAULT (getutcdate()),
    [CreatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_ConnectionProfileTask_CreatedBy]  DEFAULT (suser_name()),
    [UpdatedOn] [datetime] NOT NULL CONSTRAINT [DF_ConnectionProfileTask_UpdatedOn]  DEFAULT (getutcdate()),
    [UpdatedBy] [varchar](200) NOT NULL CONSTRAINT [DF_ConnectionProfileTask_UpdatedBy]  DEFAULT (suser_name()),
 CONSTRAINT [PK_ConnectionProfileTask] PRIMARY KEY CLUSTERED 
(
    [ID] ASC
)WITH (IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[ConnectionProfileTask]  WITH CHECK ADD  CONSTRAINT [FK_ConnectionProfileTask_ConnectionProfile] FOREIGN KEY([ConnectionProfileID])
REFERENCES [dbo].[ConnectionProfile] ([ID])
GO

INSERT INTO Vendor(Acronym, Name, PhoneNumber, ContactEmail, URL) VALUES('OTHER', 'Other / Unspecified', '', '', '')
GO

INSERT INTO Vendor(Acronym, Name, PhoneNumber, ContactEmail, URL) VALUES('APP', 'APP Engineering Inc.', '', '', 'http://appengineering.com/')
GO

INSERT INTO Vendor(Acronym, Name, PhoneNumber, ContactEmail, URL) VALUES('EMAX', 'E-MAX Instruments, Inc.', '', '', 'http://www.e-maxinstruments.com/')
GO

INSERT INTO Vendor(Acronym, Name, PhoneNumber, ContactEmail, URL) VALUES('QUAL', 'Qualitrol LLC', '', '', 'http://www.qualitrolcorp.com/')
GO

INSERT INTO VendorDevice(VendorID, Name, Description, URL) VALUES(2, 'APP-501', 'APP-501 Multifunction Recorder', 'http://appengineering.com/products/601_Sales_Literature_Rev_4.pdf')
GO

INSERT INTO VendorDevice(VendorID, Name, Description, URL) VALUES(2, 'APP-601', 'APP-601 Multifunction Recorder', 'http://appengineering.com/products/601_Sales_Literature_Rev_4.pdf')
GO

INSERT INTO VendorDevice(VendorID, Name, Description, URL) VALUES(3, 'EMAX-DII', 'E-MAX Director DII DFR', 'http://www.e-maxinstruments.com/images/pdf/product-catalog/E-MAX-Digital-Fault-Recorder-DII-2014.pdf')
GO

INSERT INTO VendorDevice(VendorID, Name, Description, URL) VALUES(3, 'EMAX-SUR', 'E-MAX Surveyor Distributed DFR', 'http://www.e-maxinstruments.com/images/pdf/product-catalog/E-MAX-Surveyor-Distributed-DFR-2015.pdf')
GO

INSERT INTO VendorDevice(VendorID, Name, Description, URL) VALUES(3, 'EMAX-WAV', 'E-MAX Wave DFR', 'http://www.e-maxinstruments.com/images/pdf/product-catalog/E-MAX-DFR-Wave-2014%20.pdf')
GO

INSERT INTO VendorDevice(VendorID, Name, Description, URL) VALUES(4, 'BEN-5000', 'BEN 5000 Portable DFR', 'http://www.qualitrolcorp.com/Products/BEN_5000_to_BEN_6000_Upgrade/')
GO

INSERT INTO VendorDevice(VendorID, Name, Description, URL) VALUES(4, 'BEN-6000', 'BEN 6000 Portable DFR', 'http://www.qualitrolcorp.com/Products/BEN_6000_Portable_digital_fault_recorder_(with_options_for_power_quality_and_PMU)/')
GO