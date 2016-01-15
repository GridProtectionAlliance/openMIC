/// <reference path="data.js" />

(function (lightSwitchApplication) {

    msls._addEntryPoints(lightSwitchApplication.AccessLog, {
        /// <field>
        /// Called when a new accessLog is created.
        /// <br/>created(msls.application.AccessLog entity)
        /// </field>
        created: [lightSwitchApplication.AccessLog]
    });

    msls._addEntryPoints(lightSwitchApplication.AlarmLog, {
        /// <field>
        /// Called when a new alarmLog is created.
        /// <br/>created(msls.application.AlarmLog entity)
        /// </field>
        created: [lightSwitchApplication.AlarmLog]
    });

    msls._addEntryPoints(lightSwitchApplication.Alarm, {
        /// <field>
        /// Called when a new alarm is created.
        /// <br/>created(msls.application.Alarm entity)
        /// </field>
        created: [lightSwitchApplication.Alarm]
    });

    msls._addEntryPoints(lightSwitchApplication.AuditLog, {
        /// <field>
        /// Called when a new auditLog is created.
        /// <br/>created(msls.application.AuditLog entity)
        /// </field>
        created: [lightSwitchApplication.AuditLog]
    });

    msls._addEntryPoints(lightSwitchApplication.Company, {
        /// <field>
        /// Called when a new company is created.
        /// <br/>created(msls.application.Company entity)
        /// </field>
        created: [lightSwitchApplication.Company]
    });

    msls._addEntryPoints(lightSwitchApplication.DefaultValue, {
        /// <field>
        /// Called when a new defaultValue is created.
        /// <br/>created(msls.application.DefaultValue entity)
        /// </field>
        created: [lightSwitchApplication.DefaultValue]
    });

    msls._addEntryPoints(lightSwitchApplication.Device, {
        /// <field>
        /// Called when a new device is created.
        /// <br/>created(msls.application.Device entity)
        /// </field>
        created: [lightSwitchApplication.Device]
    });

    msls._addEntryPoints(lightSwitchApplication.ErrorLog, {
        /// <field>
        /// Called when a new errorLog is created.
        /// <br/>created(msls.application.ErrorLog entity)
        /// </field>
        created: [lightSwitchApplication.ErrorLog]
    });

    msls._addEntryPoints(lightSwitchApplication.Historian, {
        /// <field>
        /// Called when a new historian is created.
        /// <br/>created(msls.application.Historian entity)
        /// </field>
        created: [lightSwitchApplication.Historian]
    });

    msls._addEntryPoints(lightSwitchApplication.Interconnection, {
        /// <field>
        /// Called when a new interconnection is created.
        /// <br/>created(msls.application.Interconnection entity)
        /// </field>
        created: [lightSwitchApplication.Interconnection]
    });

    msls._addEntryPoints(lightSwitchApplication.Measurement, {
        /// <field>
        /// Called when a new measurement is created.
        /// <br/>created(msls.application.Measurement entity)
        /// </field>
        created: [lightSwitchApplication.Measurement]
    });

    msls._addEntryPoints(lightSwitchApplication.Node, {
        /// <field>
        /// Called when a new node is created.
        /// <br/>created(msls.application.Node entity)
        /// </field>
        created: [lightSwitchApplication.Node]
    });

    msls._addEntryPoints(lightSwitchApplication.Protocol, {
        /// <field>
        /// Called when a new protocol is created.
        /// <br/>created(msls.application.Protocol entity)
        /// </field>
        created: [lightSwitchApplication.Protocol]
    });

    msls._addEntryPoints(lightSwitchApplication.SignalType, {
        /// <field>
        /// Called when a new signalType is created.
        /// <br/>created(msls.application.SignalType entity)
        /// </field>
        created: [lightSwitchApplication.SignalType]
    });

    msls._addEntryPoints(lightSwitchApplication.Statistic, {
        /// <field>
        /// Called when a new statistic is created.
        /// <br/>created(msls.application.Statistic entity)
        /// </field>
        created: [lightSwitchApplication.Statistic]
    });

    msls._addEntryPoints(lightSwitchApplication.VendorDevice, {
        /// <field>
        /// Called when a new vendorDevice is created.
        /// <br/>created(msls.application.VendorDevice entity)
        /// </field>
        created: [lightSwitchApplication.VendorDevice]
    });

    msls._addEntryPoints(lightSwitchApplication.Vendor, {
        /// <field>
        /// Called when a new vendor is created.
        /// <br/>created(msls.application.Vendor entity)
        /// </field>
        created: [lightSwitchApplication.Vendor]
    });

}(msls.application));
