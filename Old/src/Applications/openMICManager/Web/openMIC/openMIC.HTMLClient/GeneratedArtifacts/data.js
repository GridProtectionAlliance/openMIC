/// <reference path="../Scripts/msls.js" />

window.myapp = msls.application;

(function (lightSwitchApplication) {

    var $Entity = msls.Entity,
        $DataService = msls.DataService,
        $DataWorkspace = msls.DataWorkspace,
        $defineEntity = msls._defineEntity,
        $defineDataService = msls._defineDataService,
        $defineDataWorkspace = msls._defineDataWorkspace,
        $DataServiceQuery = msls.DataServiceQuery,
        $toODataString = msls._toODataString;

    function AccessLog(entitySet) {
        /// <summary>
        /// Represents the AccessLog entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this accessLog.
        /// </param>
        /// <field name="ID" type="Number">
        /// Gets or sets the iD for this accessLog.
        /// </field>
        /// <field name="UserName" type="String">
        /// Gets or sets the userName for this accessLog.
        /// </field>
        /// <field name="AccessGranted" type="Boolean">
        /// Gets or sets the accessGranted for this accessLog.
        /// </field>
        /// <field name="CreatedOn" type="Date">
        /// Gets or sets the createdOn for this accessLog.
        /// </field>
        /// <field name="details" type="msls.application.AccessLog.Details">
        /// Gets the details for this accessLog.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function AlarmLog(entitySet) {
        /// <summary>
        /// Represents the AlarmLog entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this alarmLog.
        /// </param>
        /// <field name="ID" type="Number">
        /// Gets or sets the iD for this alarmLog.
        /// </field>
        /// <field name="Ticks" type="String">
        /// Gets or sets the ticks for this alarmLog.
        /// </field>
        /// <field name="Timestamp" type="Date">
        /// Gets or sets the timestamp for this alarmLog.
        /// </field>
        /// <field name="Value" type="Number">
        /// Gets or sets the value for this alarmLog.
        /// </field>
        /// <field name="Alarm" type="msls.application.Alarm">
        /// Gets or sets the alarm for this alarmLog.
        /// </field>
        /// <field name="Alarm1" type="msls.application.Alarm">
        /// Gets or sets the alarm1 for this alarmLog.
        /// </field>
        /// <field name="Measurement" type="msls.application.Measurement">
        /// Gets or sets the measurement for this alarmLog.
        /// </field>
        /// <field name="details" type="msls.application.AlarmLog.Details">
        /// Gets the details for this alarmLog.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function Alarm(entitySet) {
        /// <summary>
        /// Represents the Alarm entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this alarm.
        /// </param>
        /// <field name="ID" type="Number">
        /// Gets or sets the iD for this alarm.
        /// </field>
        /// <field name="TagName" type="String">
        /// Gets or sets the tagName for this alarm.
        /// </field>
        /// <field name="AssociatedMeasurementID" type="String">
        /// Gets or sets the associatedMeasurementID for this alarm.
        /// </field>
        /// <field name="Description" type="String">
        /// Gets or sets the description for this alarm.
        /// </field>
        /// <field name="Severity" type="Number">
        /// Gets or sets the severity for this alarm.
        /// </field>
        /// <field name="Operation" type="Number">
        /// Gets or sets the operation for this alarm.
        /// </field>
        /// <field name="SetPoint" type="Number">
        /// Gets or sets the setPoint for this alarm.
        /// </field>
        /// <field name="Tolerance" type="Number">
        /// Gets or sets the tolerance for this alarm.
        /// </field>
        /// <field name="Delay" type="Number">
        /// Gets or sets the delay for this alarm.
        /// </field>
        /// <field name="Hysteresis" type="Number">
        /// Gets or sets the hysteresis for this alarm.
        /// </field>
        /// <field name="LoadOrder" type="Number">
        /// Gets or sets the loadOrder for this alarm.
        /// </field>
        /// <field name="Enabled" type="Boolean">
        /// Gets or sets the enabled for this alarm.
        /// </field>
        /// <field name="CreatedOn" type="Date">
        /// Gets or sets the createdOn for this alarm.
        /// </field>
        /// <field name="CreatedBy" type="String">
        /// Gets or sets the createdBy for this alarm.
        /// </field>
        /// <field name="UpdatedOn" type="Date">
        /// Gets or sets the updatedOn for this alarm.
        /// </field>
        /// <field name="UpdatedBy" type="String">
        /// Gets or sets the updatedBy for this alarm.
        /// </field>
        /// <field name="Measurement" type="msls.application.Measurement">
        /// Gets or sets the measurement for this alarm.
        /// </field>
        /// <field name="Node" type="msls.application.Node">
        /// Gets or sets the node for this alarm.
        /// </field>
        /// <field name="AlarmLogs" type="msls.EntityCollection" elementType="msls.application.AlarmLog">
        /// Gets the alarmLogs for this alarm.
        /// </field>
        /// <field name="AlarmLogs1" type="msls.EntityCollection" elementType="msls.application.AlarmLog">
        /// Gets the alarmLogs1 for this alarm.
        /// </field>
        /// <field name="details" type="msls.application.Alarm.Details">
        /// Gets the details for this alarm.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function AuditLog(entitySet) {
        /// <summary>
        /// Represents the AuditLog entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this auditLog.
        /// </param>
        /// <field name="ID" type="Number">
        /// Gets or sets the iD for this auditLog.
        /// </field>
        /// <field name="TableName" type="String">
        /// Gets or sets the tableName for this auditLog.
        /// </field>
        /// <field name="PrimaryKeyColumn" type="String">
        /// Gets or sets the primaryKeyColumn for this auditLog.
        /// </field>
        /// <field name="PrimaryKeyValue" type="String">
        /// Gets or sets the primaryKeyValue for this auditLog.
        /// </field>
        /// <field name="ColumnName" type="String">
        /// Gets or sets the columnName for this auditLog.
        /// </field>
        /// <field name="OriginalValue" type="String">
        /// Gets or sets the originalValue for this auditLog.
        /// </field>
        /// <field name="NewValue" type="String">
        /// Gets or sets the newValue for this auditLog.
        /// </field>
        /// <field name="Deleted" type="Boolean">
        /// Gets or sets the deleted for this auditLog.
        /// </field>
        /// <field name="UpdatedBy" type="String">
        /// Gets or sets the updatedBy for this auditLog.
        /// </field>
        /// <field name="UpdatedOn" type="Date">
        /// Gets or sets the updatedOn for this auditLog.
        /// </field>
        /// <field name="details" type="msls.application.AuditLog.Details">
        /// Gets the details for this auditLog.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function Company(entitySet) {
        /// <summary>
        /// Represents the Company entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this company.
        /// </param>
        /// <field name="ID" type="Number">
        /// Gets or sets the iD for this company.
        /// </field>
        /// <field name="Acronym" type="String">
        /// Gets or sets the acronym for this company.
        /// </field>
        /// <field name="MapAcronym" type="String">
        /// Gets or sets the mapAcronym for this company.
        /// </field>
        /// <field name="Name" type="String">
        /// Gets or sets the name for this company.
        /// </field>
        /// <field name="URL" type="String">
        /// Gets or sets the uRL for this company.
        /// </field>
        /// <field name="LoadOrder" type="Number">
        /// Gets or sets the loadOrder for this company.
        /// </field>
        /// <field name="CreatedOn" type="Date">
        /// Gets or sets the createdOn for this company.
        /// </field>
        /// <field name="CreatedBy" type="String">
        /// Gets or sets the createdBy for this company.
        /// </field>
        /// <field name="UpdatedOn" type="Date">
        /// Gets or sets the updatedOn for this company.
        /// </field>
        /// <field name="UpdatedBy" type="String">
        /// Gets or sets the updatedBy for this company.
        /// </field>
        /// <field name="Devices" type="msls.EntityCollection" elementType="msls.application.Device">
        /// Gets the devices for this company.
        /// </field>
        /// <field name="Nodes" type="msls.EntityCollection" elementType="msls.application.Node">
        /// Gets the nodes for this company.
        /// </field>
        /// <field name="DefaultValues" type="msls.EntityCollection" elementType="msls.application.DefaultValue">
        /// Gets the defaultValues for this company.
        /// </field>
        /// <field name="details" type="msls.application.Company.Details">
        /// Gets the details for this company.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function DefaultValue(entitySet) {
        /// <summary>
        /// Represents the DefaultValue entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this defaultValue.
        /// </param>
        /// <field name="ID" type="Number">
        /// Gets or sets the iD for this defaultValue.
        /// </field>
        /// <field name="ContactList" type="String">
        /// Gets or sets the contactList for this defaultValue.
        /// </field>
        /// <field name="Company" type="msls.application.Company">
        /// Gets or sets the company for this defaultValue.
        /// </field>
        /// <field name="Historian" type="msls.application.Historian">
        /// Gets or sets the historian for this defaultValue.
        /// </field>
        /// <field name="Interconnection" type="msls.application.Interconnection">
        /// Gets or sets the interconnection for this defaultValue.
        /// </field>
        /// <field name="Node" type="msls.application.Node">
        /// Gets or sets the node for this defaultValue.
        /// </field>
        /// <field name="details" type="msls.application.DefaultValue.Details">
        /// Gets the details for this defaultValue.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function Device(entitySet) {
        /// <summary>
        /// Source Devices
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this device.
        /// </param>
        /// <field name="ID" type="Number">
        /// Gets or sets the iD for this device.
        /// </field>
        /// <field name="UniqueID" type="String">
        /// Gets or sets the uniqueID for this device.
        /// </field>
        /// <field name="Acronym" type="String">
        /// Gets or sets the acronym for this device.
        /// </field>
        /// <field name="Name" type="String">
        /// Gets or sets the name for this device.
        /// </field>
        /// <field name="HistorianID" type="Number">
        /// Gets or sets the historianID for this device.
        /// </field>
        /// <field name="AccessID" type="Number">
        /// Gets or sets the accessID for this device.
        /// </field>
        /// <field name="Longitude" type="String">
        /// Gets or sets the longitude for this device.
        /// </field>
        /// <field name="Latitude" type="String">
        /// Gets or sets the latitude for this device.
        /// </field>
        /// <field name="ConnectionString" type="String">
        /// Gets or sets the connectionString for this device.
        /// </field>
        /// <field name="TimeZone" type="String">
        /// Gets or sets the timeZone for this device.
        /// </field>
        /// <field name="TimeAdjustmentTicks" type="String">
        /// Gets or sets the timeAdjustmentTicks for this device.
        /// </field>
        /// <field name="TypicalDataFrequency" type="Number">
        /// Gets or sets the typicalDataFrequency for this device.
        /// </field>
        /// <field name="DataFrequencyUnits" type="String">
        /// Gets or sets the dataFrequencyUnits for this device.
        /// </field>
        /// <field name="MeasurementReportingInterval" type="Number">
        /// Gets or sets the measurementReportingInterval for this device.
        /// </field>
        /// <field name="ContactList" type="String">
        /// Gets or sets the contactList for this device.
        /// </field>
        /// <field name="MeasuredLines" type="Number">
        /// Gets or sets the measuredLines for this device.
        /// </field>
        /// <field name="LoadOrder" type="Number">
        /// Gets or sets the loadOrder for this device.
        /// </field>
        /// <field name="Enabled" type="Boolean">
        /// Gets or sets the enabled for this device.
        /// </field>
        /// <field name="CreatedOn" type="Date">
        /// Gets or sets the createdOn for this device.
        /// </field>
        /// <field name="CreatedBy" type="String">
        /// Gets or sets the createdBy for this device.
        /// </field>
        /// <field name="UpdatedOn" type="Date">
        /// Gets or sets the updatedOn for this device.
        /// </field>
        /// <field name="UpdatedBy" type="String">
        /// Gets or sets the updatedBy for this device.
        /// </field>
        /// <field name="Interconnection" type="msls.application.Interconnection">
        /// Gets or sets the interconnection for this device.
        /// </field>
        /// <field name="Node" type="msls.application.Node">
        /// Gets or sets the node for this device.
        /// </field>
        /// <field name="Protocol" type="msls.application.Protocol">
        /// Gets or sets the protocol for this device.
        /// </field>
        /// <field name="VendorDevice" type="msls.application.VendorDevice">
        /// Gets or sets the vendorDevice for this device.
        /// </field>
        /// <field name="Measurements" type="msls.EntityCollection" elementType="msls.application.Measurement">
        /// Gets the measurements for this device.
        /// </field>
        /// <field name="Historian" type="msls.application.Historian">
        /// Gets or sets the historian for this device.
        /// </field>
        /// <field name="Company" type="msls.application.Company">
        /// Gets or sets the company for this device.
        /// </field>
        /// <field name="details" type="msls.application.Device.Details">
        /// Gets the details for this device.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function ErrorLog(entitySet) {
        /// <summary>
        /// Represents the ErrorLog entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this errorLog.
        /// </param>
        /// <field name="ID" type="Number">
        /// Gets or sets the iD for this errorLog.
        /// </field>
        /// <field name="Source" type="String">
        /// Gets or sets the source for this errorLog.
        /// </field>
        /// <field name="Type" type="String">
        /// Gets or sets the type for this errorLog.
        /// </field>
        /// <field name="Message" type="String">
        /// Gets or sets the message for this errorLog.
        /// </field>
        /// <field name="Detail" type="String">
        /// Gets or sets the detail for this errorLog.
        /// </field>
        /// <field name="CreatedOn" type="Date">
        /// Gets or sets the createdOn for this errorLog.
        /// </field>
        /// <field name="details" type="msls.application.ErrorLog.Details">
        /// Gets the details for this errorLog.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function Historian(entitySet) {
        /// <summary>
        /// Represents the Historian entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this historian.
        /// </param>
        /// <field name="ID" type="Number">
        /// Gets or sets the iD for this historian.
        /// </field>
        /// <field name="Acronym" type="String">
        /// Gets or sets the acronym for this historian.
        /// </field>
        /// <field name="Name" type="String">
        /// Gets or sets the name for this historian.
        /// </field>
        /// <field name="AssemblyName" type="String">
        /// Gets or sets the assemblyName for this historian.
        /// </field>
        /// <field name="TypeName" type="String">
        /// Gets or sets the typeName for this historian.
        /// </field>
        /// <field name="ConnectionString" type="String">
        /// Gets or sets the connectionString for this historian.
        /// </field>
        /// <field name="IsLocal" type="Boolean">
        /// Gets or sets the isLocal for this historian.
        /// </field>
        /// <field name="MeasurementReportingInterval" type="Number">
        /// Gets or sets the measurementReportingInterval for this historian.
        /// </field>
        /// <field name="Description" type="String">
        /// Gets or sets the description for this historian.
        /// </field>
        /// <field name="LoadOrder" type="Number">
        /// Gets or sets the loadOrder for this historian.
        /// </field>
        /// <field name="Enabled" type="Boolean">
        /// Gets or sets the enabled for this historian.
        /// </field>
        /// <field name="CreatedOn" type="Date">
        /// Gets or sets the createdOn for this historian.
        /// </field>
        /// <field name="CreatedBy" type="String">
        /// Gets or sets the createdBy for this historian.
        /// </field>
        /// <field name="UpdatedOn" type="Date">
        /// Gets or sets the updatedOn for this historian.
        /// </field>
        /// <field name="UpdatedBy" type="String">
        /// Gets or sets the updatedBy for this historian.
        /// </field>
        /// <field name="Node" type="msls.application.Node">
        /// Gets or sets the node for this historian.
        /// </field>
        /// <field name="Devices" type="msls.EntityCollection" elementType="msls.application.Device">
        /// Gets the devices for this historian.
        /// </field>
        /// <field name="DefaultValues" type="msls.EntityCollection" elementType="msls.application.DefaultValue">
        /// Gets the defaultValues for this historian.
        /// </field>
        /// <field name="details" type="msls.application.Historian.Details">
        /// Gets the details for this historian.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function Interconnection(entitySet) {
        /// <summary>
        /// Represents the Interconnection entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this interconnection.
        /// </param>
        /// <field name="ID" type="Number">
        /// Gets or sets the iD for this interconnection.
        /// </field>
        /// <field name="Acronym" type="String">
        /// Gets or sets the acronym for this interconnection.
        /// </field>
        /// <field name="Name" type="String">
        /// Gets or sets the name for this interconnection.
        /// </field>
        /// <field name="LoadOrder" type="Number">
        /// Gets or sets the loadOrder for this interconnection.
        /// </field>
        /// <field name="Devices" type="msls.EntityCollection" elementType="msls.application.Device">
        /// Gets the devices for this interconnection.
        /// </field>
        /// <field name="DefaultValues" type="msls.EntityCollection" elementType="msls.application.DefaultValue">
        /// Gets the defaultValues for this interconnection.
        /// </field>
        /// <field name="details" type="msls.application.Interconnection.Details">
        /// Gets the details for this interconnection.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function Measurement(entitySet) {
        /// <summary>
        /// Represents the Measurement entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this measurement.
        /// </param>
        /// <field name="SignalID" type="String">
        /// Gets or sets the signalID for this measurement.
        /// </field>
        /// <field name="HistorianID" type="Number">
        /// Gets or sets the historianID for this measurement.
        /// </field>
        /// <field name="PointTag" type="String">
        /// Gets or sets the pointTag for this measurement.
        /// </field>
        /// <field name="AlternateTag" type="String">
        /// Gets or sets the alternateTag for this measurement.
        /// </field>
        /// <field name="SignalReference" type="String">
        /// Gets or sets the signalReference for this measurement.
        /// </field>
        /// <field name="Adder" type="Number">
        /// Gets or sets the adder for this measurement.
        /// </field>
        /// <field name="Multiplier" type="Number">
        /// Gets or sets the multiplier for this measurement.
        /// </field>
        /// <field name="Description" type="String">
        /// Gets or sets the description for this measurement.
        /// </field>
        /// <field name="Internal" type="Boolean">
        /// Gets or sets the internal for this measurement.
        /// </field>
        /// <field name="Subscribed" type="Boolean">
        /// Gets or sets the subscribed for this measurement.
        /// </field>
        /// <field name="Enabled" type="Boolean">
        /// Gets or sets the enabled for this measurement.
        /// </field>
        /// <field name="CreatedOn" type="Date">
        /// Gets or sets the createdOn for this measurement.
        /// </field>
        /// <field name="CreatedBy" type="String">
        /// Gets or sets the createdBy for this measurement.
        /// </field>
        /// <field name="UpdatedOn" type="Date">
        /// Gets or sets the updatedOn for this measurement.
        /// </field>
        /// <field name="UpdatedBy" type="String">
        /// Gets or sets the updatedBy for this measurement.
        /// </field>
        /// <field name="PointID" type="Number">
        /// Gets or sets the pointID for this measurement.
        /// </field>
        /// <field name="Alarms" type="msls.EntityCollection" elementType="msls.application.Alarm">
        /// Gets the alarms for this measurement.
        /// </field>
        /// <field name="AlarmLogs" type="msls.EntityCollection" elementType="msls.application.AlarmLog">
        /// Gets the alarmLogs for this measurement.
        /// </field>
        /// <field name="Device" type="msls.application.Device">
        /// Gets or sets the device for this measurement.
        /// </field>
        /// <field name="SignalType" type="msls.application.SignalType">
        /// Gets or sets the signalType for this measurement.
        /// </field>
        /// <field name="details" type="msls.application.Measurement.Details">
        /// Gets the details for this measurement.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function Node(entitySet) {
        /// <summary>
        /// Represents the Node entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this node.
        /// </param>
        /// <field name="ID" type="String">
        /// Gets or sets the iD for this node.
        /// </field>
        /// <field name="Name" type="String">
        /// Gets or sets the name for this node.
        /// </field>
        /// <field name="Longitude" type="String">
        /// Gets or sets the longitude for this node.
        /// </field>
        /// <field name="Latitude" type="String">
        /// Gets or sets the latitude for this node.
        /// </field>
        /// <field name="Description" type="String">
        /// Gets or sets the description for this node.
        /// </field>
        /// <field name="ImagePath" type="String">
        /// Gets or sets the imagePath for this node.
        /// </field>
        /// <field name="Settings" type="String">
        /// Gets or sets the settings for this node.
        /// </field>
        /// <field name="MenuType" type="String">
        /// Gets or sets the menuType for this node.
        /// </field>
        /// <field name="MenuData" type="String">
        /// Gets or sets the menuData for this node.
        /// </field>
        /// <field name="Master" type="Boolean">
        /// Gets or sets the master for this node.
        /// </field>
        /// <field name="LoadOrder" type="Number">
        /// Gets or sets the loadOrder for this node.
        /// </field>
        /// <field name="Enabled" type="Boolean">
        /// Gets or sets the enabled for this node.
        /// </field>
        /// <field name="CreatedOn" type="Date">
        /// Gets or sets the createdOn for this node.
        /// </field>
        /// <field name="CreatedBy" type="String">
        /// Gets or sets the createdBy for this node.
        /// </field>
        /// <field name="UpdatedOn" type="Date">
        /// Gets or sets the updatedOn for this node.
        /// </field>
        /// <field name="UpdatedBy" type="String">
        /// Gets or sets the updatedBy for this node.
        /// </field>
        /// <field name="Alarms" type="msls.EntityCollection" elementType="msls.application.Alarm">
        /// Gets the alarms for this node.
        /// </field>
        /// <field name="Devices" type="msls.EntityCollection" elementType="msls.application.Device">
        /// Gets the devices for this node.
        /// </field>
        /// <field name="Historians" type="msls.EntityCollection" elementType="msls.application.Historian">
        /// Gets the historians for this node.
        /// </field>
        /// <field name="Company" type="msls.application.Company">
        /// Gets or sets the company for this node.
        /// </field>
        /// <field name="DefaultValues" type="msls.EntityCollection" elementType="msls.application.DefaultValue">
        /// Gets the defaultValues for this node.
        /// </field>
        /// <field name="details" type="msls.application.Node.Details">
        /// Gets the details for this node.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function Protocol(entitySet) {
        /// <summary>
        /// Represents the Protocol entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this protocol.
        /// </param>
        /// <field name="ID" type="Number">
        /// Gets or sets the iD for this protocol.
        /// </field>
        /// <field name="Acronym" type="String">
        /// Gets or sets the acronym for this protocol.
        /// </field>
        /// <field name="Name" type="String">
        /// Gets or sets the name for this protocol.
        /// </field>
        /// <field name="Type" type="String">
        /// Gets or sets the type for this protocol.
        /// </field>
        /// <field name="Category" type="String">
        /// Gets or sets the category for this protocol.
        /// </field>
        /// <field name="AssemblyName" type="String">
        /// Gets or sets the assemblyName for this protocol.
        /// </field>
        /// <field name="TypeName" type="String">
        /// Gets or sets the typeName for this protocol.
        /// </field>
        /// <field name="LoadOrder" type="Number">
        /// Gets or sets the loadOrder for this protocol.
        /// </field>
        /// <field name="Devices" type="msls.EntityCollection" elementType="msls.application.Device">
        /// Gets the devices for this protocol.
        /// </field>
        /// <field name="details" type="msls.application.Protocol.Details">
        /// Gets the details for this protocol.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function SignalType(entitySet) {
        /// <summary>
        /// Represents the SignalType entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this signalType.
        /// </param>
        /// <field name="ID" type="Number">
        /// Gets or sets the iD for this signalType.
        /// </field>
        /// <field name="Name" type="String">
        /// Gets or sets the name for this signalType.
        /// </field>
        /// <field name="Acronym" type="String">
        /// Gets or sets the acronym for this signalType.
        /// </field>
        /// <field name="Suffix" type="String">
        /// Gets or sets the suffix for this signalType.
        /// </field>
        /// <field name="Abbreviation" type="String">
        /// Gets or sets the abbreviation for this signalType.
        /// </field>
        /// <field name="LongAcronym" type="String">
        /// Gets or sets the longAcronym for this signalType.
        /// </field>
        /// <field name="Source" type="String">
        /// Gets or sets the source for this signalType.
        /// </field>
        /// <field name="EngineeringUnits" type="String">
        /// Gets or sets the engineeringUnits for this signalType.
        /// </field>
        /// <field name="Measurements" type="msls.EntityCollection" elementType="msls.application.Measurement">
        /// Gets the measurements for this signalType.
        /// </field>
        /// <field name="details" type="msls.application.SignalType.Details">
        /// Gets the details for this signalType.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function Statistic(entitySet) {
        /// <summary>
        /// Represents the Statistic entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this statistic.
        /// </param>
        /// <field name="ID" type="Number">
        /// Gets or sets the iD for this statistic.
        /// </field>
        /// <field name="Source" type="String">
        /// Gets or sets the source for this statistic.
        /// </field>
        /// <field name="SignalIndex" type="Number">
        /// Gets or sets the signalIndex for this statistic.
        /// </field>
        /// <field name="Name" type="String">
        /// Gets or sets the name for this statistic.
        /// </field>
        /// <field name="Description" type="String">
        /// Gets or sets the description for this statistic.
        /// </field>
        /// <field name="AssemblyName" type="String">
        /// Gets or sets the assemblyName for this statistic.
        /// </field>
        /// <field name="TypeName" type="String">
        /// Gets or sets the typeName for this statistic.
        /// </field>
        /// <field name="MethodName" type="String">
        /// Gets or sets the methodName for this statistic.
        /// </field>
        /// <field name="Arguments" type="String">
        /// Gets or sets the arguments for this statistic.
        /// </field>
        /// <field name="Enabled" type="Boolean">
        /// Gets or sets the enabled for this statistic.
        /// </field>
        /// <field name="DataType" type="String">
        /// Gets or sets the dataType for this statistic.
        /// </field>
        /// <field name="DisplayFormat" type="String">
        /// Gets or sets the displayFormat for this statistic.
        /// </field>
        /// <field name="IsConnectedState" type="Boolean">
        /// Gets or sets the isConnectedState for this statistic.
        /// </field>
        /// <field name="LoadOrder" type="Number">
        /// Gets or sets the loadOrder for this statistic.
        /// </field>
        /// <field name="details" type="msls.application.Statistic.Details">
        /// Gets the details for this statistic.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function VendorDevice(entitySet) {
        /// <summary>
        /// Represents the VendorDevice entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this vendorDevice.
        /// </param>
        /// <field name="ID" type="Number">
        /// Gets or sets the iD for this vendorDevice.
        /// </field>
        /// <field name="Name" type="String">
        /// Gets or sets the name for this vendorDevice.
        /// </field>
        /// <field name="Description" type="String">
        /// Gets or sets the description for this vendorDevice.
        /// </field>
        /// <field name="URL" type="String">
        /// Gets or sets the uRL for this vendorDevice.
        /// </field>
        /// <field name="CreatedOn" type="Date">
        /// Gets or sets the createdOn for this vendorDevice.
        /// </field>
        /// <field name="CreatedBy" type="String">
        /// Gets or sets the createdBy for this vendorDevice.
        /// </field>
        /// <field name="UpdatedOn" type="Date">
        /// Gets or sets the updatedOn for this vendorDevice.
        /// </field>
        /// <field name="UpdatedBy" type="String">
        /// Gets or sets the updatedBy for this vendorDevice.
        /// </field>
        /// <field name="Devices" type="msls.EntityCollection" elementType="msls.application.Device">
        /// Gets the devices for this vendorDevice.
        /// </field>
        /// <field name="Vendor" type="msls.application.Vendor">
        /// Gets or sets the vendor for this vendorDevice.
        /// </field>
        /// <field name="details" type="msls.application.VendorDevice.Details">
        /// Gets the details for this vendorDevice.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function Vendor(entitySet) {
        /// <summary>
        /// Represents the Vendor entity type.
        /// </summary>
        /// <param name="entitySet" type="msls.EntitySet" optional="true">
        /// The entity set that should contain this vendor.
        /// </param>
        /// <field name="ID" type="Number">
        /// Gets or sets the iD for this vendor.
        /// </field>
        /// <field name="Acronym" type="String">
        /// Gets or sets the acronym for this vendor.
        /// </field>
        /// <field name="Name" type="String">
        /// Gets or sets the name for this vendor.
        /// </field>
        /// <field name="PhoneNumber" type="String">
        /// Gets or sets the phoneNumber for this vendor.
        /// </field>
        /// <field name="ContactEmail" type="String">
        /// Gets or sets the contactEmail for this vendor.
        /// </field>
        /// <field name="URL" type="String">
        /// Gets or sets the uRL for this vendor.
        /// </field>
        /// <field name="CreatedOn" type="Date">
        /// Gets or sets the createdOn for this vendor.
        /// </field>
        /// <field name="CreatedBy" type="String">
        /// Gets or sets the createdBy for this vendor.
        /// </field>
        /// <field name="UpdatedOn" type="Date">
        /// Gets or sets the updatedOn for this vendor.
        /// </field>
        /// <field name="UpdatedBy" type="String">
        /// Gets or sets the updatedBy for this vendor.
        /// </field>
        /// <field name="VendorDevices" type="msls.EntityCollection" elementType="msls.application.VendorDevice">
        /// Gets the vendorDevices for this vendor.
        /// </field>
        /// <field name="details" type="msls.application.Vendor.Details">
        /// Gets the details for this vendor.
        /// </field>
        $Entity.call(this, entitySet);
    }

    function openMICData(dataWorkspace) {
        /// <summary>
        /// Represents the openMICData data service.
        /// </summary>
        /// <param name="dataWorkspace" type="msls.DataWorkspace">
        /// The data workspace that created this data service.
        /// </param>
        /// <field name="AccessLogs" type="msls.EntitySet">
        /// Gets the AccessLogs entity set.
        /// </field>
        /// <field name="AlarmLogs" type="msls.EntitySet">
        /// Gets the AlarmLogs entity set.
        /// </field>
        /// <field name="Alarms" type="msls.EntitySet">
        /// Gets the Alarms entity set.
        /// </field>
        /// <field name="AuditLogs" type="msls.EntitySet">
        /// Gets the AuditLogs entity set.
        /// </field>
        /// <field name="Companies" type="msls.EntitySet">
        /// Gets the Companies entity set.
        /// </field>
        /// <field name="DefaultValues" type="msls.EntitySet">
        /// Gets the DefaultValues entity set.
        /// </field>
        /// <field name="Devices" type="msls.EntitySet">
        /// Gets the Devices entity set.
        /// </field>
        /// <field name="ErrorLogs" type="msls.EntitySet">
        /// Gets the ErrorLogs entity set.
        /// </field>
        /// <field name="Historians" type="msls.EntitySet">
        /// Gets the Historians entity set.
        /// </field>
        /// <field name="Interconnections" type="msls.EntitySet">
        /// Gets the Interconnections entity set.
        /// </field>
        /// <field name="Measurements" type="msls.EntitySet">
        /// Gets the Measurements entity set.
        /// </field>
        /// <field name="Nodes" type="msls.EntitySet">
        /// Gets the Nodes entity set.
        /// </field>
        /// <field name="Protocols" type="msls.EntitySet">
        /// Gets the Protocols entity set.
        /// </field>
        /// <field name="SignalTypes" type="msls.EntitySet">
        /// Gets the SignalTypes entity set.
        /// </field>
        /// <field name="Statistics" type="msls.EntitySet">
        /// Gets the Statistics entity set.
        /// </field>
        /// <field name="VendorDevices" type="msls.EntitySet">
        /// Gets the VendorDevices entity set.
        /// </field>
        /// <field name="Vendors" type="msls.EntitySet">
        /// Gets the Vendors entity set.
        /// </field>
        /// <field name="details" type="msls.application.openMICData.Details">
        /// Gets the details for this data service.
        /// </field>
        $DataService.call(this, dataWorkspace);
    };
    function DataWorkspace() {
        /// <summary>
        /// Represents the data workspace.
        /// </summary>
        /// <field name="openMICData" type="msls.application.openMICData">
        /// Gets the openMICData data service.
        /// </field>
        /// <field name="details" type="msls.application.DataWorkspace.Details">
        /// Gets the details for this data workspace.
        /// </field>
        $DataWorkspace.call(this);
    };

    msls._addToNamespace("msls.application", {

        AccessLog: $defineEntity(AccessLog, [
            { name: "ID", type: Number, isReadOnly: true },
            { name: "UserName", type: String },
            { name: "AccessGranted", type: Boolean },
            { name: "CreatedOn", type: Date }
        ]),

        AlarmLog: $defineEntity(AlarmLog, [
            { name: "ID", type: Number, isReadOnly: true },
            { name: "Ticks", type: String },
            { name: "Timestamp", type: Date },
            { name: "Value", type: Number },
            { name: "Alarm", kind: "reference", type: Alarm },
            { name: "Alarm1", kind: "reference", type: Alarm },
            { name: "Measurement", kind: "reference", type: Measurement }
        ]),

        Alarm: $defineEntity(Alarm, [
            { name: "ID", type: Number, isReadOnly: true },
            { name: "TagName", type: String },
            { name: "AssociatedMeasurementID", type: String },
            { name: "Description", type: String },
            { name: "Severity", type: Number },
            { name: "Operation", type: Number },
            { name: "SetPoint", type: Number },
            { name: "Tolerance", type: Number },
            { name: "Delay", type: Number },
            { name: "Hysteresis", type: Number },
            { name: "LoadOrder", type: Number },
            { name: "Enabled", type: Boolean },
            { name: "CreatedOn", type: Date },
            { name: "CreatedBy", type: String },
            { name: "UpdatedOn", type: Date },
            { name: "UpdatedBy", type: String },
            { name: "Measurement", kind: "reference", type: Measurement },
            { name: "Node", kind: "reference", type: Node },
            { name: "AlarmLogs", kind: "collection", elementType: AlarmLog },
            { name: "AlarmLogs1", kind: "collection", elementType: AlarmLog }
        ]),

        AuditLog: $defineEntity(AuditLog, [
            { name: "ID", type: Number, isReadOnly: true },
            { name: "TableName", type: String },
            { name: "PrimaryKeyColumn", type: String },
            { name: "PrimaryKeyValue", type: String },
            { name: "ColumnName", type: String },
            { name: "OriginalValue", type: String },
            { name: "NewValue", type: String },
            { name: "Deleted", type: Boolean },
            { name: "UpdatedBy", type: String },
            { name: "UpdatedOn", type: Date }
        ]),

        Company: $defineEntity(Company, [
            { name: "ID", type: Number, isReadOnly: true },
            { name: "Acronym", type: String },
            { name: "MapAcronym", type: String },
            { name: "Name", type: String },
            { name: "URL", type: String },
            { name: "LoadOrder", type: Number },
            { name: "CreatedOn", type: Date },
            { name: "CreatedBy", type: String },
            { name: "UpdatedOn", type: Date },
            { name: "UpdatedBy", type: String },
            { name: "Devices", kind: "collection", elementType: Device },
            { name: "Nodes", kind: "collection", elementType: Node },
            { name: "DefaultValues", kind: "collection", elementType: DefaultValue }
        ]),

        DefaultValue: $defineEntity(DefaultValue, [
            { name: "ID", type: Number, isReadOnly: true },
            { name: "ContactList", type: String },
            { name: "Company", kind: "reference", type: Company },
            { name: "Historian", kind: "reference", type: Historian },
            { name: "Interconnection", kind: "reference", type: Interconnection },
            { name: "Node", kind: "reference", type: Node }
        ]),

        Device: $defineEntity(Device, [
            { name: "ID", type: Number, isReadOnly: true },
            { name: "UniqueID", type: String },
            { name: "Acronym", type: String },
            { name: "Name", type: String },
            { name: "HistorianID", type: Number },
            { name: "AccessID", type: Number },
            { name: "Longitude", type: String },
            { name: "Latitude", type: String },
            { name: "ConnectionString", type: String },
            { name: "TimeZone", type: String },
            { name: "TimeAdjustmentTicks", type: String },
            { name: "TypicalDataFrequency", type: Number },
            { name: "DataFrequencyUnits", type: String },
            { name: "MeasurementReportingInterval", type: Number },
            { name: "ContactList", type: String },
            { name: "MeasuredLines", type: Number },
            { name: "LoadOrder", type: Number },
            { name: "Enabled", type: Boolean },
            { name: "CreatedOn", type: Date },
            { name: "CreatedBy", type: String },
            { name: "UpdatedOn", type: Date },
            { name: "UpdatedBy", type: String },
            { name: "Interconnection", kind: "reference", type: Interconnection },
            { name: "Node", kind: "reference", type: Node },
            { name: "Protocol", kind: "reference", type: Protocol },
            { name: "VendorDevice", kind: "reference", type: VendorDevice },
            { name: "Measurements", kind: "collection", elementType: Measurement },
            { name: "Historian", kind: "reference", type: Historian },
            { name: "Company", kind: "reference", type: Company }
        ]),

        ErrorLog: $defineEntity(ErrorLog, [
            { name: "ID", type: Number, isReadOnly: true },
            { name: "Source", type: String },
            { name: "Type", type: String },
            { name: "Message", type: String },
            { name: "Detail", type: String },
            { name: "CreatedOn", type: Date }
        ]),

        Historian: $defineEntity(Historian, [
            { name: "ID", type: Number, isReadOnly: true },
            { name: "Acronym", type: String },
            { name: "Name", type: String },
            { name: "AssemblyName", type: String },
            { name: "TypeName", type: String },
            { name: "ConnectionString", type: String },
            { name: "IsLocal", type: Boolean },
            { name: "MeasurementReportingInterval", type: Number },
            { name: "Description", type: String },
            { name: "LoadOrder", type: Number },
            { name: "Enabled", type: Boolean },
            { name: "CreatedOn", type: Date },
            { name: "CreatedBy", type: String },
            { name: "UpdatedOn", type: Date },
            { name: "UpdatedBy", type: String },
            { name: "Node", kind: "reference", type: Node },
            { name: "Devices", kind: "collection", elementType: Device },
            { name: "DefaultValues", kind: "collection", elementType: DefaultValue }
        ]),

        Interconnection: $defineEntity(Interconnection, [
            { name: "ID", type: Number, isReadOnly: true },
            { name: "Acronym", type: String },
            { name: "Name", type: String },
            { name: "LoadOrder", type: Number },
            { name: "Devices", kind: "collection", elementType: Device },
            { name: "DefaultValues", kind: "collection", elementType: DefaultValue }
        ]),

        Measurement: $defineEntity(Measurement, [
            { name: "SignalID", type: String },
            { name: "HistorianID", type: Number },
            { name: "PointTag", type: String },
            { name: "AlternateTag", type: String },
            { name: "SignalReference", type: String },
            { name: "Adder", type: Number },
            { name: "Multiplier", type: Number },
            { name: "Description", type: String },
            { name: "Internal", type: Boolean },
            { name: "Subscribed", type: Boolean },
            { name: "Enabled", type: Boolean },
            { name: "CreatedOn", type: Date },
            { name: "CreatedBy", type: String },
            { name: "UpdatedOn", type: Date },
            { name: "UpdatedBy", type: String },
            { name: "PointID", type: Number, isReadOnly: true },
            { name: "Alarms", kind: "collection", elementType: Alarm },
            { name: "AlarmLogs", kind: "collection", elementType: AlarmLog },
            { name: "Device", kind: "reference", type: Device },
            { name: "SignalType", kind: "reference", type: SignalType }
        ]),

        Node: $defineEntity(Node, [
            { name: "ID", type: String },
            { name: "Name", type: String },
            { name: "Longitude", type: String },
            { name: "Latitude", type: String },
            { name: "Description", type: String },
            { name: "ImagePath", type: String },
            { name: "Settings", type: String },
            { name: "MenuType", type: String },
            { name: "MenuData", type: String },
            { name: "Master", type: Boolean },
            { name: "LoadOrder", type: Number },
            { name: "Enabled", type: Boolean },
            { name: "CreatedOn", type: Date },
            { name: "CreatedBy", type: String },
            { name: "UpdatedOn", type: Date },
            { name: "UpdatedBy", type: String },
            { name: "Alarms", kind: "collection", elementType: Alarm },
            { name: "Devices", kind: "collection", elementType: Device },
            { name: "Historians", kind: "collection", elementType: Historian },
            { name: "Company", kind: "reference", type: Company },
            { name: "DefaultValues", kind: "collection", elementType: DefaultValue }
        ]),

        Protocol: $defineEntity(Protocol, [
            { name: "ID", type: Number, isReadOnly: true },
            { name: "Acronym", type: String },
            { name: "Name", type: String },
            { name: "Type", type: String },
            { name: "Category", type: String },
            { name: "AssemblyName", type: String },
            { name: "TypeName", type: String },
            { name: "LoadOrder", type: Number },
            { name: "Devices", kind: "collection", elementType: Device }
        ]),

        SignalType: $defineEntity(SignalType, [
            { name: "ID", type: Number, isReadOnly: true },
            { name: "Name", type: String },
            { name: "Acronym", type: String },
            { name: "Suffix", type: String },
            { name: "Abbreviation", type: String },
            { name: "LongAcronym", type: String },
            { name: "Source", type: String },
            { name: "EngineeringUnits", type: String },
            { name: "Measurements", kind: "collection", elementType: Measurement }
        ]),

        Statistic: $defineEntity(Statistic, [
            { name: "ID", type: Number, isReadOnly: true },
            { name: "Source", type: String },
            { name: "SignalIndex", type: Number },
            { name: "Name", type: String },
            { name: "Description", type: String },
            { name: "AssemblyName", type: String },
            { name: "TypeName", type: String },
            { name: "MethodName", type: String },
            { name: "Arguments", type: String },
            { name: "Enabled", type: Boolean },
            { name: "DataType", type: String },
            { name: "DisplayFormat", type: String },
            { name: "IsConnectedState", type: Boolean },
            { name: "LoadOrder", type: Number }
        ]),

        VendorDevice: $defineEntity(VendorDevice, [
            { name: "ID", type: Number, isReadOnly: true },
            { name: "Name", type: String },
            { name: "Description", type: String },
            { name: "URL", type: String },
            { name: "CreatedOn", type: Date },
            { name: "CreatedBy", type: String },
            { name: "UpdatedOn", type: Date },
            { name: "UpdatedBy", type: String },
            { name: "Devices", kind: "collection", elementType: Device },
            { name: "Vendor", kind: "reference", type: Vendor }
        ]),

        Vendor: $defineEntity(Vendor, [
            { name: "ID", type: Number, isReadOnly: true },
            { name: "Acronym", type: String },
            { name: "Name", type: String },
            { name: "PhoneNumber", type: String },
            { name: "ContactEmail", type: String },
            { name: "URL", type: String },
            { name: "CreatedOn", type: Date },
            { name: "CreatedBy", type: String },
            { name: "UpdatedOn", type: Date },
            { name: "UpdatedBy", type: String },
            { name: "VendorDevices", kind: "collection", elementType: VendorDevice }
        ]),

        openMICData: $defineDataService(openMICData, lightSwitchApplication.rootUri + "/openMICData.svc", [
            { name: "AccessLogs", elementType: AccessLog },
            { name: "AlarmLogs", elementType: AlarmLog },
            { name: "Alarms", elementType: Alarm },
            { name: "AuditLogs", elementType: AuditLog },
            { name: "Companies", elementType: Company },
            { name: "DefaultValues", elementType: DefaultValue },
            { name: "Devices", elementType: Device },
            { name: "ErrorLogs", elementType: ErrorLog },
            { name: "Historians", elementType: Historian },
            { name: "Interconnections", elementType: Interconnection },
            { name: "Measurements", elementType: Measurement },
            { name: "Nodes", elementType: Node },
            { name: "Protocols", elementType: Protocol },
            { name: "SignalTypes", elementType: SignalType },
            { name: "Statistics", elementType: Statistic },
            { name: "VendorDevices", elementType: VendorDevice },
            { name: "Vendors", elementType: Vendor }
        ], [
            {
                name: "AccessLogs_SingleOrDefault", value: function (ID) {
                    return new $DataServiceQuery({ _entitySet: this.AccessLogs },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/AccessLogs(" + "ID=" + $toODataString(ID, "Int32?") + ")"
                    );
                }
            },
            {
                name: "AlarmLogs_SingleOrDefault", value: function (ID) {
                    return new $DataServiceQuery({ _entitySet: this.AlarmLogs },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/AlarmLogs(" + "ID=" + $toODataString(ID, "Int32?") + ")"
                    );
                }
            },
            {
                name: "Alarms_SingleOrDefault", value: function (ID) {
                    return new $DataServiceQuery({ _entitySet: this.Alarms },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/Alarms(" + "ID=" + $toODataString(ID, "Int32?") + ")"
                    );
                }
            },
            {
                name: "AuditLogs_SingleOrDefault", value: function (ID) {
                    return new $DataServiceQuery({ _entitySet: this.AuditLogs },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/AuditLogs(" + "ID=" + $toODataString(ID, "Int32?") + ")"
                    );
                }
            },
            {
                name: "Companies_SingleOrDefault", value: function (ID) {
                    return new $DataServiceQuery({ _entitySet: this.Companies },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/Companies(" + "ID=" + $toODataString(ID, "Int32?") + ")"
                    );
                }
            },
            {
                name: "DefaultValues_SingleOrDefault", value: function (ID) {
                    return new $DataServiceQuery({ _entitySet: this.DefaultValues },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/DefaultValues(" + "ID=" + $toODataString(ID, "Int32?") + ")"
                    );
                }
            },
            {
                name: "DefaultValueCount", value: function () {
                    return new $DataServiceQuery({ _entitySet: this.DefaultValues },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/DefaultValueCount",
                        {
                        });
                }
            },
            {
                name: "Devices_SingleOrDefault", value: function (ID) {
                    return new $DataServiceQuery({ _entitySet: this.Devices },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/Devices(" + "ID=" + $toODataString(ID, "Int32?") + ")"
                    );
                }
            },
            {
                name: "ErrorLogs_SingleOrDefault", value: function (ID) {
                    return new $DataServiceQuery({ _entitySet: this.ErrorLogs },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/ErrorLogs(" + "ID=" + $toODataString(ID, "Int32?") + ")"
                    );
                }
            },
            {
                name: "Historians_SingleOrDefault", value: function (ID) {
                    return new $DataServiceQuery({ _entitySet: this.Historians },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/Historians(" + "ID=" + $toODataString(ID, "Int32?") + ")"
                    );
                }
            },
            {
                name: "Interconnections_SingleOrDefault", value: function (ID) {
                    return new $DataServiceQuery({ _entitySet: this.Interconnections },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/Interconnections(" + "ID=" + $toODataString(ID, "Int32?") + ")"
                    );
                }
            },
            {
                name: "Measurements_SingleOrDefault", value: function (SignalID) {
                    return new $DataServiceQuery({ _entitySet: this.Measurements },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/Measurements(" + "SignalID=" + $toODataString(SignalID, "Guid?") + ")"
                    );
                }
            },
            {
                name: "Nodes_SingleOrDefault", value: function (ID) {
                    return new $DataServiceQuery({ _entitySet: this.Nodes },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/Nodes(" + "ID=" + $toODataString(ID, "Guid?") + ")"
                    );
                }
            },
            {
                name: "Protocols_SingleOrDefault", value: function (ID) {
                    return new $DataServiceQuery({ _entitySet: this.Protocols },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/Protocols(" + "ID=" + $toODataString(ID, "Int32?") + ")"
                    );
                }
            },
            {
                name: "SignalTypes_SingleOrDefault", value: function (ID) {
                    return new $DataServiceQuery({ _entitySet: this.SignalTypes },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/SignalTypes(" + "ID=" + $toODataString(ID, "Int32?") + ")"
                    );
                }
            },
            {
                name: "Statistics_SingleOrDefault", value: function (ID) {
                    return new $DataServiceQuery({ _entitySet: this.Statistics },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/Statistics(" + "ID=" + $toODataString(ID, "Int32?") + ")"
                    );
                }
            },
            {
                name: "VendorDevices_SingleOrDefault", value: function (ID) {
                    return new $DataServiceQuery({ _entitySet: this.VendorDevices },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/VendorDevices(" + "ID=" + $toODataString(ID, "Int32?") + ")"
                    );
                }
            },
            {
                name: "Vendors_SingleOrDefault", value: function (ID) {
                    return new $DataServiceQuery({ _entitySet: this.Vendors },
                        lightSwitchApplication.rootUri + "/openMICData.svc" + "/Vendors(" + "ID=" + $toODataString(ID, "Int32?") + ")"
                    );
                }
            }
        ]),

        DataWorkspace: $defineDataWorkspace(DataWorkspace, [
            { name: "openMICData", type: openMICData }
        ])

    });

}(msls.application));
