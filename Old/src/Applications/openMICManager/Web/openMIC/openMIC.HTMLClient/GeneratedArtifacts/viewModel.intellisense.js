/// <reference path="viewModel.js" />

(function (lightSwitchApplication) {

    var $element = document.createElement("div");

    lightSwitchApplication.AddEditAlarm.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditAlarm
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.AddEditAlarm,
            value: lightSwitchApplication.AddEditAlarm
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.AddEditAlarm,
            value: lightSwitchApplication.Alarm
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: lightSwitchApplication.Alarm
        },
        TagName: {
            _$class: msls.ContentItem,
            _$name: "TagName",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: String
        },
        AssociatedMeasurementID: {
            _$class: msls.ContentItem,
            _$name: "AssociatedMeasurementID",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: String
        },
        Description: {
            _$class: msls.ContentItem,
            _$name: "Description",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: String
        },
        Severity: {
            _$class: msls.ContentItem,
            _$name: "Severity",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: Number
        },
        Operation: {
            _$class: msls.ContentItem,
            _$name: "Operation",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: Number
        },
        SetPoint: {
            _$class: msls.ContentItem,
            _$name: "SetPoint",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: Number
        },
        Tolerance: {
            _$class: msls.ContentItem,
            _$name: "Tolerance",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: Number
        },
        Delay: {
            _$class: msls.ContentItem,
            _$name: "Delay",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: Number
        },
        Hysteresis: {
            _$class: msls.ContentItem,
            _$name: "Hysteresis",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: Number
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: lightSwitchApplication.Alarm
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: Number
        },
        Enabled: {
            _$class: msls.ContentItem,
            _$name: "Enabled",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: Boolean
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: Date
        },
        CreatedBy: {
            _$class: msls.ContentItem,
            _$name: "CreatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: String
        },
        UpdatedOn: {
            _$class: msls.ContentItem,
            _$name: "UpdatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: Date
        },
        UpdatedBy: {
            _$class: msls.ContentItem,
            _$name: "UpdatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: String
        },
        Measurement: {
            _$class: msls.ContentItem,
            _$name: "Measurement",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: lightSwitchApplication.Measurement
        },
        RowTemplate: {
            _$class: msls.ContentItem,
            _$name: "RowTemplate",
            _$parentName: "Measurement",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Measurement,
            value: lightSwitchApplication.Measurement
        },
        Node: {
            _$class: msls.ContentItem,
            _$name: "Node",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Alarm,
            value: lightSwitchApplication.Node
        },
        RowTemplate1: {
            _$class: msls.ContentItem,
            _$name: "RowTemplate1",
            _$parentName: "Node",
            screen: lightSwitchApplication.AddEditAlarm,
            data: lightSwitchApplication.Node,
            value: lightSwitchApplication.Node
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditAlarm
        }
    };

    msls._addEntryPoints(lightSwitchApplication.AddEditAlarm, {
        /// <field>
        /// Called when a new AddEditAlarm screen is created.
        /// <br/>created(msls.application.AddEditAlarm screen)
        /// </field>
        created: [lightSwitchApplication.AddEditAlarm],
        /// <field>
        /// Called before changes on an active AddEditAlarm screen are applied.
        /// <br/>beforeApplyChanges(msls.application.AddEditAlarm screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.AddEditAlarm],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("left"); }],
        /// <field>
        /// Called after the TagName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        TagName_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("TagName"); }],
        /// <field>
        /// Called after the AssociatedMeasurementID content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AssociatedMeasurementID_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("AssociatedMeasurementID"); }],
        /// <field>
        /// Called after the Description content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Description_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("Description"); }],
        /// <field>
        /// Called after the Severity content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Severity_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("Severity"); }],
        /// <field>
        /// Called after the Operation content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Operation_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("Operation"); }],
        /// <field>
        /// Called after the SetPoint content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        SetPoint_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("SetPoint"); }],
        /// <field>
        /// Called after the Tolerance content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Tolerance_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("Tolerance"); }],
        /// <field>
        /// Called after the Delay content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Delay_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("Delay"); }],
        /// <field>
        /// Called after the Hysteresis content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Hysteresis_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("Hysteresis"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("right"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("LoadOrder"); }],
        /// <field>
        /// Called after the Enabled content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Enabled_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("Enabled"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("CreatedOn"); }],
        /// <field>
        /// Called after the CreatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedBy_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("CreatedBy"); }],
        /// <field>
        /// Called after the UpdatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedOn_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("UpdatedOn"); }],
        /// <field>
        /// Called after the UpdatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedBy_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("UpdatedBy"); }],
        /// <field>
        /// Called after the Measurement content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Measurement_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("Measurement"); }],
        /// <field>
        /// Called after the RowTemplate content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        RowTemplate_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("RowTemplate"); }],
        /// <field>
        /// Called after the Node content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Node_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("Node"); }],
        /// <field>
        /// Called after the RowTemplate1 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        RowTemplate1_postRender: [$element, function () { return new lightSwitchApplication.AddEditAlarm().findContentItem("RowTemplate1"); }]
    });

    lightSwitchApplication.BrowseAlarms.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseAlarms
        },
        AlarmList: {
            _$class: msls.ContentItem,
            _$name: "AlarmList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseAlarms,
            data: lightSwitchApplication.BrowseAlarms,
            value: lightSwitchApplication.BrowseAlarms
        },
        Alarms: {
            _$class: msls.ContentItem,
            _$name: "Alarms",
            _$parentName: "AlarmList",
            screen: lightSwitchApplication.BrowseAlarms,
            data: lightSwitchApplication.BrowseAlarms,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseAlarms,
                _$entry: {
                    elementType: lightSwitchApplication.Alarm
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "Alarms",
            screen: lightSwitchApplication.BrowseAlarms,
            data: lightSwitchApplication.Alarm,
            value: lightSwitchApplication.Alarm
        },
        TagName: {
            _$class: msls.ContentItem,
            _$name: "TagName",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseAlarms,
            data: lightSwitchApplication.Alarm,
            value: String
        },
        AssociatedMeasurementID: {
            _$class: msls.ContentItem,
            _$name: "AssociatedMeasurementID",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseAlarms,
            data: lightSwitchApplication.Alarm,
            value: String
        },
        Description: {
            _$class: msls.ContentItem,
            _$name: "Description",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseAlarms,
            data: lightSwitchApplication.Alarm,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseAlarms
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseAlarms, {
        /// <field>
        /// Called when a new BrowseAlarms screen is created.
        /// <br/>created(msls.application.BrowseAlarms screen)
        /// </field>
        created: [lightSwitchApplication.BrowseAlarms],
        /// <field>
        /// Called before changes on an active BrowseAlarms screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseAlarms screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseAlarms],
        /// <field>
        /// Called after the AlarmList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AlarmList_postRender: [$element, function () { return new lightSwitchApplication.BrowseAlarms().findContentItem("AlarmList"); }],
        /// <field>
        /// Called after the Alarms content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Alarms_postRender: [$element, function () { return new lightSwitchApplication.BrowseAlarms().findContentItem("Alarms"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseAlarms().findContentItem("rows"); }],
        /// <field>
        /// Called after the TagName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        TagName_postRender: [$element, function () { return new lightSwitchApplication.BrowseAlarms().findContentItem("TagName"); }],
        /// <field>
        /// Called after the AssociatedMeasurementID content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AssociatedMeasurementID_postRender: [$element, function () { return new lightSwitchApplication.BrowseAlarms().findContentItem("AssociatedMeasurementID"); }],
        /// <field>
        /// Called after the Description content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Description_postRender: [$element, function () { return new lightSwitchApplication.BrowseAlarms().findContentItem("Description"); }]
    });

    lightSwitchApplication.ViewAlarm.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewAlarm
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.ViewAlarm,
            value: lightSwitchApplication.ViewAlarm
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.ViewAlarm,
            value: lightSwitchApplication.Alarm
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: lightSwitchApplication.Alarm
        },
        TagName: {
            _$class: msls.ContentItem,
            _$name: "TagName",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: String
        },
        AssociatedMeasurementID: {
            _$class: msls.ContentItem,
            _$name: "AssociatedMeasurementID",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: String
        },
        Description: {
            _$class: msls.ContentItem,
            _$name: "Description",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: String
        },
        Severity: {
            _$class: msls.ContentItem,
            _$name: "Severity",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: Number
        },
        Operation: {
            _$class: msls.ContentItem,
            _$name: "Operation",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: Number
        },
        SetPoint: {
            _$class: msls.ContentItem,
            _$name: "SetPoint",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: Number
        },
        Tolerance: {
            _$class: msls.ContentItem,
            _$name: "Tolerance",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: Number
        },
        Delay: {
            _$class: msls.ContentItem,
            _$name: "Delay",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: Number
        },
        Hysteresis: {
            _$class: msls.ContentItem,
            _$name: "Hysteresis",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: Number
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: lightSwitchApplication.Alarm
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: Number
        },
        Enabled: {
            _$class: msls.ContentItem,
            _$name: "Enabled",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: Boolean
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: Date
        },
        CreatedBy: {
            _$class: msls.ContentItem,
            _$name: "CreatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: String
        },
        UpdatedOn: {
            _$class: msls.ContentItem,
            _$name: "UpdatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: Date
        },
        UpdatedBy: {
            _$class: msls.ContentItem,
            _$name: "UpdatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: String
        },
        Measurement: {
            _$class: msls.ContentItem,
            _$name: "Measurement",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: lightSwitchApplication.Measurement
        },
        Node: {
            _$class: msls.ContentItem,
            _$name: "Node",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewAlarm,
            data: lightSwitchApplication.Alarm,
            value: lightSwitchApplication.Node
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewAlarm
        }
    };

    msls._addEntryPoints(lightSwitchApplication.ViewAlarm, {
        /// <field>
        /// Called when a new ViewAlarm screen is created.
        /// <br/>created(msls.application.ViewAlarm screen)
        /// </field>
        created: [lightSwitchApplication.ViewAlarm],
        /// <field>
        /// Called before changes on an active ViewAlarm screen are applied.
        /// <br/>beforeApplyChanges(msls.application.ViewAlarm screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.ViewAlarm],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("left"); }],
        /// <field>
        /// Called after the TagName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        TagName_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("TagName"); }],
        /// <field>
        /// Called after the AssociatedMeasurementID content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AssociatedMeasurementID_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("AssociatedMeasurementID"); }],
        /// <field>
        /// Called after the Description content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Description_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("Description"); }],
        /// <field>
        /// Called after the Severity content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Severity_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("Severity"); }],
        /// <field>
        /// Called after the Operation content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Operation_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("Operation"); }],
        /// <field>
        /// Called after the SetPoint content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        SetPoint_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("SetPoint"); }],
        /// <field>
        /// Called after the Tolerance content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Tolerance_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("Tolerance"); }],
        /// <field>
        /// Called after the Delay content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Delay_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("Delay"); }],
        /// <field>
        /// Called after the Hysteresis content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Hysteresis_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("Hysteresis"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("right"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("LoadOrder"); }],
        /// <field>
        /// Called after the Enabled content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Enabled_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("Enabled"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("CreatedOn"); }],
        /// <field>
        /// Called after the CreatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedBy_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("CreatedBy"); }],
        /// <field>
        /// Called after the UpdatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedOn_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("UpdatedOn"); }],
        /// <field>
        /// Called after the UpdatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedBy_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("UpdatedBy"); }],
        /// <field>
        /// Called after the Measurement content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Measurement_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("Measurement"); }],
        /// <field>
        /// Called after the Node content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Node_postRender: [$element, function () { return new lightSwitchApplication.ViewAlarm().findContentItem("Node"); }]
    });

    lightSwitchApplication.AddEditCompany.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditCompany
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.AddEditCompany,
            data: lightSwitchApplication.AddEditCompany,
            value: lightSwitchApplication.AddEditCompany
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.AddEditCompany,
            data: lightSwitchApplication.AddEditCompany,
            value: lightSwitchApplication.Company
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditCompany,
            data: lightSwitchApplication.Company,
            value: lightSwitchApplication.Company
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditCompany,
            data: lightSwitchApplication.Company,
            value: String
        },
        MapAcronym: {
            _$class: msls.ContentItem,
            _$name: "MapAcronym",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditCompany,
            data: lightSwitchApplication.Company,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditCompany,
            data: lightSwitchApplication.Company,
            value: String
        },
        URL: {
            _$class: msls.ContentItem,
            _$name: "URL",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditCompany,
            data: lightSwitchApplication.Company,
            value: String
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditCompany,
            data: lightSwitchApplication.Company,
            value: Number
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditCompany,
            data: lightSwitchApplication.Company,
            value: lightSwitchApplication.Company
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditCompany,
            data: lightSwitchApplication.Company,
            value: Date
        },
        CreatedBy: {
            _$class: msls.ContentItem,
            _$name: "CreatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditCompany,
            data: lightSwitchApplication.Company,
            value: String
        },
        UpdatedOn: {
            _$class: msls.ContentItem,
            _$name: "UpdatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditCompany,
            data: lightSwitchApplication.Company,
            value: Date
        },
        UpdatedBy: {
            _$class: msls.ContentItem,
            _$name: "UpdatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditCompany,
            data: lightSwitchApplication.Company,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditCompany
        }
    };

    msls._addEntryPoints(lightSwitchApplication.AddEditCompany, {
        /// <field>
        /// Called when a new AddEditCompany screen is created.
        /// <br/>created(msls.application.AddEditCompany screen)
        /// </field>
        created: [lightSwitchApplication.AddEditCompany],
        /// <field>
        /// Called before changes on an active AddEditCompany screen are applied.
        /// <br/>beforeApplyChanges(msls.application.AddEditCompany screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.AddEditCompany],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.AddEditCompany().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.AddEditCompany().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.AddEditCompany().findContentItem("left"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.AddEditCompany().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the MapAcronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        MapAcronym_postRender: [$element, function () { return new lightSwitchApplication.AddEditCompany().findContentItem("MapAcronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.AddEditCompany().findContentItem("Name"); }],
        /// <field>
        /// Called after the URL content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        URL_postRender: [$element, function () { return new lightSwitchApplication.AddEditCompany().findContentItem("URL"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.AddEditCompany().findContentItem("LoadOrder"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.AddEditCompany().findContentItem("right"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.AddEditCompany().findContentItem("CreatedOn"); }],
        /// <field>
        /// Called after the CreatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedBy_postRender: [$element, function () { return new lightSwitchApplication.AddEditCompany().findContentItem("CreatedBy"); }],
        /// <field>
        /// Called after the UpdatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedOn_postRender: [$element, function () { return new lightSwitchApplication.AddEditCompany().findContentItem("UpdatedOn"); }],
        /// <field>
        /// Called after the UpdatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedBy_postRender: [$element, function () { return new lightSwitchApplication.AddEditCompany().findContentItem("UpdatedBy"); }]
    });

    lightSwitchApplication.BrowseCompanies.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseCompanies
        },
        CompanyList: {
            _$class: msls.ContentItem,
            _$name: "CompanyList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseCompanies,
            data: lightSwitchApplication.BrowseCompanies,
            value: lightSwitchApplication.BrowseCompanies
        },
        Companies: {
            _$class: msls.ContentItem,
            _$name: "Companies",
            _$parentName: "CompanyList",
            screen: lightSwitchApplication.BrowseCompanies,
            data: lightSwitchApplication.BrowseCompanies,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseCompanies,
                _$entry: {
                    elementType: lightSwitchApplication.Company
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "Companies",
            screen: lightSwitchApplication.BrowseCompanies,
            data: lightSwitchApplication.Company,
            value: lightSwitchApplication.Company
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseCompanies,
            data: lightSwitchApplication.Company,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseCompanies,
            data: lightSwitchApplication.Company,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseCompanies
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseCompanies, {
        /// <field>
        /// Called when a new BrowseCompanies screen is created.
        /// <br/>created(msls.application.BrowseCompanies screen)
        /// </field>
        created: [lightSwitchApplication.BrowseCompanies],
        /// <field>
        /// Called before changes on an active BrowseCompanies screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseCompanies screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseCompanies],
        /// <field>
        /// Called after the CompanyList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CompanyList_postRender: [$element, function () { return new lightSwitchApplication.BrowseCompanies().findContentItem("CompanyList"); }],
        /// <field>
        /// Called after the Companies content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Companies_postRender: [$element, function () { return new lightSwitchApplication.BrowseCompanies().findContentItem("Companies"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseCompanies().findContentItem("rows"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.BrowseCompanies().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.BrowseCompanies().findContentItem("Name"); }]
    });

    lightSwitchApplication.ViewCompany.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewCompany
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.ViewCompany,
            data: lightSwitchApplication.ViewCompany,
            value: lightSwitchApplication.ViewCompany
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.ViewCompany,
            data: lightSwitchApplication.ViewCompany,
            value: lightSwitchApplication.Company
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewCompany,
            data: lightSwitchApplication.Company,
            value: lightSwitchApplication.Company
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewCompany,
            data: lightSwitchApplication.Company,
            value: String
        },
        MapAcronym: {
            _$class: msls.ContentItem,
            _$name: "MapAcronym",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewCompany,
            data: lightSwitchApplication.Company,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewCompany,
            data: lightSwitchApplication.Company,
            value: String
        },
        URL: {
            _$class: msls.ContentItem,
            _$name: "URL",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewCompany,
            data: lightSwitchApplication.Company,
            value: String
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewCompany,
            data: lightSwitchApplication.Company,
            value: Number
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewCompany,
            data: lightSwitchApplication.Company,
            value: lightSwitchApplication.Company
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewCompany,
            data: lightSwitchApplication.Company,
            value: Date
        },
        CreatedBy: {
            _$class: msls.ContentItem,
            _$name: "CreatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewCompany,
            data: lightSwitchApplication.Company,
            value: String
        },
        UpdatedOn: {
            _$class: msls.ContentItem,
            _$name: "UpdatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewCompany,
            data: lightSwitchApplication.Company,
            value: Date
        },
        UpdatedBy: {
            _$class: msls.ContentItem,
            _$name: "UpdatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewCompany,
            data: lightSwitchApplication.Company,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewCompany
        }
    };

    msls._addEntryPoints(lightSwitchApplication.ViewCompany, {
        /// <field>
        /// Called when a new ViewCompany screen is created.
        /// <br/>created(msls.application.ViewCompany screen)
        /// </field>
        created: [lightSwitchApplication.ViewCompany],
        /// <field>
        /// Called before changes on an active ViewCompany screen are applied.
        /// <br/>beforeApplyChanges(msls.application.ViewCompany screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.ViewCompany],
        /// <field>
        /// Called to determine if the Delete method can be executed.
        /// <br/>canExecute(msls.application.ViewCompany screen)
        /// </field>
        Delete_canExecute: [lightSwitchApplication.ViewCompany],
        /// <field>
        /// Called to execute the Delete method.
        /// <br/>execute(msls.application.ViewCompany screen)
        /// </field>
        Delete_execute: [lightSwitchApplication.ViewCompany],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.ViewCompany().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.ViewCompany().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.ViewCompany().findContentItem("left"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.ViewCompany().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the MapAcronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        MapAcronym_postRender: [$element, function () { return new lightSwitchApplication.ViewCompany().findContentItem("MapAcronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.ViewCompany().findContentItem("Name"); }],
        /// <field>
        /// Called after the URL content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        URL_postRender: [$element, function () { return new lightSwitchApplication.ViewCompany().findContentItem("URL"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.ViewCompany().findContentItem("LoadOrder"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.ViewCompany().findContentItem("right"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.ViewCompany().findContentItem("CreatedOn"); }],
        /// <field>
        /// Called after the CreatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedBy_postRender: [$element, function () { return new lightSwitchApplication.ViewCompany().findContentItem("CreatedBy"); }],
        /// <field>
        /// Called after the UpdatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedOn_postRender: [$element, function () { return new lightSwitchApplication.ViewCompany().findContentItem("UpdatedOn"); }],
        /// <field>
        /// Called after the UpdatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedBy_postRender: [$element, function () { return new lightSwitchApplication.ViewCompany().findContentItem("UpdatedBy"); }]
    });

    lightSwitchApplication.AddEditDefaultValue.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditDefaultValue
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.AddEditDefaultValue,
            data: lightSwitchApplication.AddEditDefaultValue,
            value: lightSwitchApplication.AddEditDefaultValue
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "Details",
            screen: lightSwitchApplication.AddEditDefaultValue,
            data: lightSwitchApplication.AddEditDefaultValue,
            value: lightSwitchApplication.AddEditDefaultValue
        },
        Node: {
            _$class: msls.ContentItem,
            _$name: "Node",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditDefaultValue,
            data: lightSwitchApplication.AddEditDefaultValue,
            value: lightSwitchApplication.Node
        },
        RowTemplate3: {
            _$class: msls.ContentItem,
            _$name: "RowTemplate3",
            _$parentName: "Node",
            screen: lightSwitchApplication.AddEditDefaultValue,
            data: lightSwitchApplication.Node,
            value: lightSwitchApplication.Node
        },
        Company: {
            _$class: msls.ContentItem,
            _$name: "Company",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditDefaultValue,
            data: lightSwitchApplication.AddEditDefaultValue,
            value: lightSwitchApplication.Company
        },
        RowTemplate: {
            _$class: msls.ContentItem,
            _$name: "RowTemplate",
            _$parentName: "Company",
            screen: lightSwitchApplication.AddEditDefaultValue,
            data: lightSwitchApplication.Company,
            value: lightSwitchApplication.Company
        },
        Interconnection: {
            _$class: msls.ContentItem,
            _$name: "Interconnection",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditDefaultValue,
            data: lightSwitchApplication.AddEditDefaultValue,
            value: lightSwitchApplication.Interconnection
        },
        RowTemplate2: {
            _$class: msls.ContentItem,
            _$name: "RowTemplate2",
            _$parentName: "Interconnection",
            screen: lightSwitchApplication.AddEditDefaultValue,
            data: lightSwitchApplication.Interconnection,
            value: lightSwitchApplication.Interconnection
        },
        Historian: {
            _$class: msls.ContentItem,
            _$name: "Historian",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditDefaultValue,
            data: lightSwitchApplication.AddEditDefaultValue,
            value: lightSwitchApplication.Historian
        },
        RowTemplate1: {
            _$class: msls.ContentItem,
            _$name: "RowTemplate1",
            _$parentName: "Historian",
            screen: lightSwitchApplication.AddEditDefaultValue,
            data: lightSwitchApplication.Historian,
            value: lightSwitchApplication.Historian
        },
        ContactList: {
            _$class: msls.ContentItem,
            _$name: "ContactList",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditDefaultValue,
            data: lightSwitchApplication.AddEditDefaultValue,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditDefaultValue
        }
    };

    msls._addEntryPoints(lightSwitchApplication.AddEditDefaultValue, {
        /// <field>
        /// Called when a new AddEditDefaultValue screen is created.
        /// <br/>created(msls.application.AddEditDefaultValue screen)
        /// </field>
        created: [lightSwitchApplication.AddEditDefaultValue],
        /// <field>
        /// Called before changes on an active AddEditDefaultValue screen are applied.
        /// <br/>beforeApplyChanges(msls.application.AddEditDefaultValue screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.AddEditDefaultValue],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.AddEditDefaultValue().findContentItem("Details"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.AddEditDefaultValue().findContentItem("left"); }],
        /// <field>
        /// Called after the Node content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Node_postRender: [$element, function () { return new lightSwitchApplication.AddEditDefaultValue().findContentItem("Node"); }],
        /// <field>
        /// Called after the RowTemplate3 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        RowTemplate3_postRender: [$element, function () { return new lightSwitchApplication.AddEditDefaultValue().findContentItem("RowTemplate3"); }],
        /// <field>
        /// Called after the Company content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Company_postRender: [$element, function () { return new lightSwitchApplication.AddEditDefaultValue().findContentItem("Company"); }],
        /// <field>
        /// Called after the RowTemplate content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        RowTemplate_postRender: [$element, function () { return new lightSwitchApplication.AddEditDefaultValue().findContentItem("RowTemplate"); }],
        /// <field>
        /// Called after the Interconnection content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Interconnection_postRender: [$element, function () { return new lightSwitchApplication.AddEditDefaultValue().findContentItem("Interconnection"); }],
        /// <field>
        /// Called after the RowTemplate2 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        RowTemplate2_postRender: [$element, function () { return new lightSwitchApplication.AddEditDefaultValue().findContentItem("RowTemplate2"); }],
        /// <field>
        /// Called after the Historian content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Historian_postRender: [$element, function () { return new lightSwitchApplication.AddEditDefaultValue().findContentItem("Historian"); }],
        /// <field>
        /// Called after the RowTemplate1 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        RowTemplate1_postRender: [$element, function () { return new lightSwitchApplication.AddEditDefaultValue().findContentItem("RowTemplate1"); }],
        /// <field>
        /// Called after the ContactList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ContactList_postRender: [$element, function () { return new lightSwitchApplication.AddEditDefaultValue().findContentItem("ContactList"); }]
    });

    lightSwitchApplication.BrowseDefaultValues.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseDefaultValues
        },
        DefaultValueList: {
            _$class: msls.ContentItem,
            _$name: "DefaultValueList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseDefaultValues,
            data: lightSwitchApplication.BrowseDefaultValues,
            value: lightSwitchApplication.BrowseDefaultValues
        },
        DefaultValues: {
            _$class: msls.ContentItem,
            _$name: "DefaultValues",
            _$parentName: "DefaultValueList",
            screen: lightSwitchApplication.BrowseDefaultValues,
            data: lightSwitchApplication.BrowseDefaultValues,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseDefaultValues,
                _$entry: {
                    elementType: lightSwitchApplication.DefaultValue
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "DefaultValues",
            screen: lightSwitchApplication.BrowseDefaultValues,
            data: lightSwitchApplication.DefaultValue,
            value: lightSwitchApplication.DefaultValue
        },
        Node: {
            _$class: msls.ContentItem,
            _$name: "Node",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseDefaultValues,
            data: lightSwitchApplication.DefaultValue,
            value: lightSwitchApplication.Node
        },
        Company: {
            _$class: msls.ContentItem,
            _$name: "Company",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseDefaultValues,
            data: lightSwitchApplication.DefaultValue,
            value: lightSwitchApplication.Company
        },
        Interconnection: {
            _$class: msls.ContentItem,
            _$name: "Interconnection",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseDefaultValues,
            data: lightSwitchApplication.DefaultValue,
            value: lightSwitchApplication.Interconnection
        },
        Historian: {
            _$class: msls.ContentItem,
            _$name: "Historian",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseDefaultValues,
            data: lightSwitchApplication.DefaultValue,
            value: lightSwitchApplication.Historian
        },
        ContactList: {
            _$class: msls.ContentItem,
            _$name: "ContactList",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseDefaultValues,
            data: lightSwitchApplication.DefaultValue,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseDefaultValues
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseDefaultValues, {
        /// <field>
        /// Called when a new BrowseDefaultValues screen is created.
        /// <br/>created(msls.application.BrowseDefaultValues screen)
        /// </field>
        created: [lightSwitchApplication.BrowseDefaultValues],
        /// <field>
        /// Called before changes on an active BrowseDefaultValues screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseDefaultValues screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseDefaultValues],
        /// <field>
        /// Called after the DefaultValueList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        DefaultValueList_postRender: [$element, function () { return new lightSwitchApplication.BrowseDefaultValues().findContentItem("DefaultValueList"); }],
        /// <field>
        /// Called after the DefaultValues content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        DefaultValues_postRender: [$element, function () { return new lightSwitchApplication.BrowseDefaultValues().findContentItem("DefaultValues"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseDefaultValues().findContentItem("rows"); }],
        /// <field>
        /// Called after the Node content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Node_postRender: [$element, function () { return new lightSwitchApplication.BrowseDefaultValues().findContentItem("Node"); }],
        /// <field>
        /// Called after the Company content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Company_postRender: [$element, function () { return new lightSwitchApplication.BrowseDefaultValues().findContentItem("Company"); }],
        /// <field>
        /// Called after the Interconnection content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Interconnection_postRender: [$element, function () { return new lightSwitchApplication.BrowseDefaultValues().findContentItem("Interconnection"); }],
        /// <field>
        /// Called after the Historian content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Historian_postRender: [$element, function () { return new lightSwitchApplication.BrowseDefaultValues().findContentItem("Historian"); }],
        /// <field>
        /// Called after the ContactList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ContactList_postRender: [$element, function () { return new lightSwitchApplication.BrowseDefaultValues().findContentItem("ContactList"); }]
    });

    lightSwitchApplication.AddEditDevice.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditDevice
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.AddEditDevice,
            value: lightSwitchApplication.AddEditDevice
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.AddEditDevice,
            value: lightSwitchApplication.Device
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Device
        },
        Node: {
            _$class: msls.ContentItem,
            _$name: "Node",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Node
        },
        RowTemplate2: {
            _$class: msls.ContentItem,
            _$name: "RowTemplate2",
            _$parentName: "Node",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Node,
            value: lightSwitchApplication.Node
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        VendorDevice: {
            _$class: msls.ContentItem,
            _$name: "VendorDevice",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.VendorDevice
        },
        RowTemplate4: {
            _$class: msls.ContentItem,
            _$name: "RowTemplate4",
            _$parentName: "VendorDevice",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.VendorDevice,
            value: lightSwitchApplication.VendorDevice
        },
        Protocol: {
            _$class: msls.ContentItem,
            _$name: "Protocol",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Protocol
        },
        RowTemplate3: {
            _$class: msls.ContentItem,
            _$name: "RowTemplate3",
            _$parentName: "Protocol",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Protocol,
            value: lightSwitchApplication.Protocol
        },
        ConnectionString: {
            _$class: msls.ContentItem,
            _$name: "ConnectionString",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        AccessID: {
            _$class: msls.ContentItem,
            _$name: "AccessID",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: Number
        },
        Company: {
            _$class: msls.ContentItem,
            _$name: "Company",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Company
        },
        Company1: {
            _$class: msls.ContentItem,
            _$name: "Company1",
            _$parentName: "Company",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Company,
            value: lightSwitchApplication.Company
        },
        Interconnection: {
            _$class: msls.ContentItem,
            _$name: "Interconnection",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Interconnection
        },
        RowTemplate1: {
            _$class: msls.ContentItem,
            _$name: "RowTemplate1",
            _$parentName: "Interconnection",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Interconnection,
            value: lightSwitchApplication.Interconnection
        },
        Historian: {
            _$class: msls.ContentItem,
            _$name: "Historian",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Historian
        },
        Historian1: {
            _$class: msls.ContentItem,
            _$name: "Historian1",
            _$parentName: "Historian",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Historian,
            value: lightSwitchApplication.Historian
        },
        Enabled: {
            _$class: msls.ContentItem,
            _$name: "Enabled",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: Boolean
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Device
        },
        ContactList: {
            _$class: msls.ContentItem,
            _$name: "ContactList",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        TimeZone: {
            _$class: msls.ContentItem,
            _$name: "TimeZone",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        TimeAdjustmentTicks: {
            _$class: msls.ContentItem,
            _$name: "TimeAdjustmentTicks",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        MeasuredLines: {
            _$class: msls.ContentItem,
            _$name: "MeasuredLines",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: Number
        },
        TypicalDataFrequency: {
            _$class: msls.ContentItem,
            _$name: "TypicalDataFrequency",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: Number
        },
        DataFrequencyUnits: {
            _$class: msls.ContentItem,
            _$name: "DataFrequencyUnits",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        Longitude: {
            _$class: msls.ContentItem,
            _$name: "Longitude",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        Latitude: {
            _$class: msls.ContentItem,
            _$name: "Latitude",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: Number
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: Date
        },
        CreatedBy: {
            _$class: msls.ContentItem,
            _$name: "CreatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        UpdatedOn: {
            _$class: msls.ContentItem,
            _$name: "UpdatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: Date
        },
        UpdatedBy: {
            _$class: msls.ContentItem,
            _$name: "UpdatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditDevice
        }
    };

    msls._addEntryPoints(lightSwitchApplication.AddEditDevice, {
        /// <field>
        /// Called when a new AddEditDevice screen is created.
        /// <br/>created(msls.application.AddEditDevice screen)
        /// </field>
        created: [lightSwitchApplication.AddEditDevice],
        /// <field>
        /// Called before changes on an active AddEditDevice screen are applied.
        /// <br/>beforeApplyChanges(msls.application.AddEditDevice screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.AddEditDevice],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("left"); }],
        /// <field>
        /// Called after the Node content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Node_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("Node"); }],
        /// <field>
        /// Called after the RowTemplate2 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        RowTemplate2_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("RowTemplate2"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("Name"); }],
        /// <field>
        /// Called after the VendorDevice content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        VendorDevice_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("VendorDevice"); }],
        /// <field>
        /// Called after the RowTemplate4 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        RowTemplate4_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("RowTemplate4"); }],
        /// <field>
        /// Called after the Protocol content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Protocol_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("Protocol"); }],
        /// <field>
        /// Called after the RowTemplate3 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        RowTemplate3_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("RowTemplate3"); }],
        /// <field>
        /// Called after the ConnectionString content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ConnectionString_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("ConnectionString"); }],
        /// <field>
        /// Called after the AccessID content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AccessID_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("AccessID"); }],
        /// <field>
        /// Called after the Company content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Company_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("Company"); }],
        /// <field>
        /// Called after the Company1 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Company1_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("Company1"); }],
        /// <field>
        /// Called after the Interconnection content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Interconnection_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("Interconnection"); }],
        /// <field>
        /// Called after the RowTemplate1 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        RowTemplate1_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("RowTemplate1"); }],
        /// <field>
        /// Called after the Historian content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Historian_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("Historian"); }],
        /// <field>
        /// Called after the Historian1 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Historian1_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("Historian1"); }],
        /// <field>
        /// Called after the Enabled content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Enabled_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("Enabled"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("right"); }],
        /// <field>
        /// Called after the ContactList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ContactList_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("ContactList"); }],
        /// <field>
        /// Called after the TimeZone content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        TimeZone_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("TimeZone"); }],
        /// <field>
        /// Called after the TimeAdjustmentTicks content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        TimeAdjustmentTicks_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("TimeAdjustmentTicks"); }],
        /// <field>
        /// Called after the MeasuredLines content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        MeasuredLines_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("MeasuredLines"); }],
        /// <field>
        /// Called after the TypicalDataFrequency content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        TypicalDataFrequency_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("TypicalDataFrequency"); }],
        /// <field>
        /// Called after the DataFrequencyUnits content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        DataFrequencyUnits_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("DataFrequencyUnits"); }],
        /// <field>
        /// Called after the Longitude content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Longitude_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("Longitude"); }],
        /// <field>
        /// Called after the Latitude content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Latitude_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("Latitude"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("LoadOrder"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("CreatedOn"); }],
        /// <field>
        /// Called after the CreatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedBy_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("CreatedBy"); }],
        /// <field>
        /// Called after the UpdatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedOn_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("UpdatedOn"); }],
        /// <field>
        /// Called after the UpdatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedBy_postRender: [$element, function () { return new lightSwitchApplication.AddEditDevice().findContentItem("UpdatedBy"); }]
    });

    lightSwitchApplication.BrowseDevices.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseDevices
        },
        DeviceList: {
            _$class: msls.ContentItem,
            _$name: "DeviceList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseDevices,
            data: lightSwitchApplication.BrowseDevices,
            value: lightSwitchApplication.BrowseDevices
        },
        Devices: {
            _$class: msls.ContentItem,
            _$name: "Devices",
            _$parentName: "DeviceList",
            screen: lightSwitchApplication.BrowseDevices,
            data: lightSwitchApplication.BrowseDevices,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseDevices,
                _$entry: {
                    elementType: lightSwitchApplication.Device
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "Devices",
            screen: lightSwitchApplication.BrowseDevices,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Device
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseDevices,
            data: lightSwitchApplication.Device,
            value: String
        },
        State: {
            _$class: msls.ContentItem,
            _$name: "State",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseDevices,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Device
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "State",
            screen: lightSwitchApplication.BrowseDevices,
            data: lightSwitchApplication.Device,
            value: String
        },
        Protocol: {
            _$class: msls.ContentItem,
            _$name: "Protocol",
            _$parentName: "State",
            screen: lightSwitchApplication.BrowseDevices,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Protocol
        },
        Company: {
            _$class: msls.ContentItem,
            _$name: "Company",
            _$parentName: "State",
            screen: lightSwitchApplication.BrowseDevices,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Company
        },
        ContactList: {
            _$class: msls.ContentItem,
            _$name: "ContactList",
            _$parentName: "State",
            screen: lightSwitchApplication.BrowseDevices,
            data: lightSwitchApplication.Device,
            value: String
        },
        Group: {
            _$class: msls.ContentItem,
            _$name: "Group",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseDevices,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Device
        },
        Enabled: {
            _$class: msls.ContentItem,
            _$name: "Enabled",
            _$parentName: "Group",
            screen: lightSwitchApplication.BrowseDevices,
            data: lightSwitchApplication.Device,
            value: Boolean
        },
        ShowLog: {
            _$class: msls.ContentItem,
            _$name: "ShowLog",
            _$parentName: "Group",
            screen: lightSwitchApplication.BrowseDevices
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseDevices
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseDevices, {
        /// <field>
        /// Called when a new BrowseDevices screen is created.
        /// <br/>created(msls.application.BrowseDevices screen)
        /// </field>
        created: [lightSwitchApplication.BrowseDevices],
        /// <field>
        /// Called before changes on an active BrowseDevices screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseDevices screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseDevices],
        /// <field>
        /// Called after the DeviceList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        DeviceList_postRender: [$element, function () { return new lightSwitchApplication.BrowseDevices().findContentItem("DeviceList"); }],
        /// <field>
        /// Called after the Devices content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Devices_postRender: [$element, function () { return new lightSwitchApplication.BrowseDevices().findContentItem("Devices"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseDevices().findContentItem("rows"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.BrowseDevices().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the State content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        State_postRender: [$element, function () { return new lightSwitchApplication.BrowseDevices().findContentItem("State"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.BrowseDevices().findContentItem("Name"); }],
        /// <field>
        /// Called after the Protocol content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Protocol_postRender: [$element, function () { return new lightSwitchApplication.BrowseDevices().findContentItem("Protocol"); }],
        /// <field>
        /// Called after the Company content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Company_postRender: [$element, function () { return new lightSwitchApplication.BrowseDevices().findContentItem("Company"); }],
        /// <field>
        /// Called after the ContactList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ContactList_postRender: [$element, function () { return new lightSwitchApplication.BrowseDevices().findContentItem("ContactList"); }],
        /// <field>
        /// Called after the Group content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Group_postRender: [$element, function () { return new lightSwitchApplication.BrowseDevices().findContentItem("Group"); }],
        /// <field>
        /// Called after the Enabled content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Enabled_postRender: [$element, function () { return new lightSwitchApplication.BrowseDevices().findContentItem("Enabled"); }],
        /// <field>
        /// Called after the ShowLog content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ShowLog_postRender: [$element, function () { return new lightSwitchApplication.BrowseDevices().findContentItem("ShowLog"); }]
    });

    lightSwitchApplication.ViewDevice.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewDevice
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.ViewDevice,
            value: lightSwitchApplication.ViewDevice
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.ViewDevice,
            value: lightSwitchApplication.Device
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Device
        },
        Node1: {
            _$class: msls.ContentItem,
            _$name: "Node1",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Node
        },
        Protocol1: {
            _$class: msls.ContentItem,
            _$name: "Protocol1",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Protocol
        },
        VendorDevice1: {
            _$class: msls.ContentItem,
            _$name: "VendorDevice1",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.VendorDevice
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        Company: {
            _$class: msls.ContentItem,
            _$name: "Company",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Company
        },
        Interconnection: {
            _$class: msls.ContentItem,
            _$name: "Interconnection",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Interconnection
        },
        Historian: {
            _$class: msls.ContentItem,
            _$name: "Historian",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Historian
        },
        ConnectionString: {
            _$class: msls.ContentItem,
            _$name: "ConnectionString",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        AccessID: {
            _$class: msls.ContentItem,
            _$name: "AccessID",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: Number
        },
        Longitude: {
            _$class: msls.ContentItem,
            _$name: "Longitude",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        Latitude: {
            _$class: msls.ContentItem,
            _$name: "Latitude",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: Date
        },
        CreatedBy: {
            _$class: msls.ContentItem,
            _$name: "CreatedBy",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Device
        },
        ContactList: {
            _$class: msls.ContentItem,
            _$name: "ContactList",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        TimeZone: {
            _$class: msls.ContentItem,
            _$name: "TimeZone",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        TimeAdjustmentTicks: {
            _$class: msls.ContentItem,
            _$name: "TimeAdjustmentTicks",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        TypicalDataFrequency: {
            _$class: msls.ContentItem,
            _$name: "TypicalDataFrequency",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: Number
        },
        DataFrequencyUnits: {
            _$class: msls.ContentItem,
            _$name: "DataFrequencyUnits",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        MeasurementReportingInterval: {
            _$class: msls.ContentItem,
            _$name: "MeasurementReportingInterval",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: Number
        },
        MeasuredLines: {
            _$class: msls.ContentItem,
            _$name: "MeasuredLines",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: Number
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: Number
        },
        Enabled: {
            _$class: msls.ContentItem,
            _$name: "Enabled",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: Boolean
        },
        UniqueID: {
            _$class: msls.ContentItem,
            _$name: "UniqueID",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        UpdatedOn: {
            _$class: msls.ContentItem,
            _$name: "UpdatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: Date
        },
        UpdatedBy: {
            _$class: msls.ContentItem,
            _$name: "UpdatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewDevice,
            data: lightSwitchApplication.Device,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewDevice
        }
    };

    msls._addEntryPoints(lightSwitchApplication.ViewDevice, {
        /// <field>
        /// Called when a new ViewDevice screen is created.
        /// <br/>created(msls.application.ViewDevice screen)
        /// </field>
        created: [lightSwitchApplication.ViewDevice],
        /// <field>
        /// Called before changes on an active ViewDevice screen are applied.
        /// <br/>beforeApplyChanges(msls.application.ViewDevice screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.ViewDevice],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("left"); }],
        /// <field>
        /// Called after the Node1 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Node1_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("Node1"); }],
        /// <field>
        /// Called after the Protocol1 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Protocol1_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("Protocol1"); }],
        /// <field>
        /// Called after the VendorDevice1 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        VendorDevice1_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("VendorDevice1"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("Name"); }],
        /// <field>
        /// Called after the Company content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Company_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("Company"); }],
        /// <field>
        /// Called after the Interconnection content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Interconnection_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("Interconnection"); }],
        /// <field>
        /// Called after the Historian content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Historian_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("Historian"); }],
        /// <field>
        /// Called after the ConnectionString content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ConnectionString_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("ConnectionString"); }],
        /// <field>
        /// Called after the AccessID content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AccessID_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("AccessID"); }],
        /// <field>
        /// Called after the Longitude content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Longitude_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("Longitude"); }],
        /// <field>
        /// Called after the Latitude content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Latitude_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("Latitude"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("CreatedOn"); }],
        /// <field>
        /// Called after the CreatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedBy_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("CreatedBy"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("right"); }],
        /// <field>
        /// Called after the ContactList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ContactList_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("ContactList"); }],
        /// <field>
        /// Called after the TimeZone content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        TimeZone_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("TimeZone"); }],
        /// <field>
        /// Called after the TimeAdjustmentTicks content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        TimeAdjustmentTicks_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("TimeAdjustmentTicks"); }],
        /// <field>
        /// Called after the TypicalDataFrequency content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        TypicalDataFrequency_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("TypicalDataFrequency"); }],
        /// <field>
        /// Called after the DataFrequencyUnits content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        DataFrequencyUnits_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("DataFrequencyUnits"); }],
        /// <field>
        /// Called after the MeasurementReportingInterval content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        MeasurementReportingInterval_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("MeasurementReportingInterval"); }],
        /// <field>
        /// Called after the MeasuredLines content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        MeasuredLines_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("MeasuredLines"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("LoadOrder"); }],
        /// <field>
        /// Called after the Enabled content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Enabled_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("Enabled"); }],
        /// <field>
        /// Called after the UniqueID content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UniqueID_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("UniqueID"); }],
        /// <field>
        /// Called after the UpdatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedOn_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("UpdatedOn"); }],
        /// <field>
        /// Called after the UpdatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedBy_postRender: [$element, function () { return new lightSwitchApplication.ViewDevice().findContentItem("UpdatedBy"); }]
    });

    lightSwitchApplication.AddEditHistorian.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditHistorian
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.AddEditHistorian,
            value: lightSwitchApplication.AddEditHistorian
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.AddEditHistorian,
            value: lightSwitchApplication.Historian
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: lightSwitchApplication.Historian
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: String
        },
        AssemblyName: {
            _$class: msls.ContentItem,
            _$name: "AssemblyName",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: String
        },
        TypeName: {
            _$class: msls.ContentItem,
            _$name: "TypeName",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: String
        },
        ConnectionString: {
            _$class: msls.ContentItem,
            _$name: "ConnectionString",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: String
        },
        IsLocal: {
            _$class: msls.ContentItem,
            _$name: "IsLocal",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: Boolean
        },
        MeasurementReportingInterval: {
            _$class: msls.ContentItem,
            _$name: "MeasurementReportingInterval",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: Number
        },
        Description: {
            _$class: msls.ContentItem,
            _$name: "Description",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: String
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: lightSwitchApplication.Historian
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: Number
        },
        Enabled: {
            _$class: msls.ContentItem,
            _$name: "Enabled",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: Boolean
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: Date
        },
        CreatedBy: {
            _$class: msls.ContentItem,
            _$name: "CreatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: String
        },
        UpdatedOn: {
            _$class: msls.ContentItem,
            _$name: "UpdatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: Date
        },
        UpdatedBy: {
            _$class: msls.ContentItem,
            _$name: "UpdatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: String
        },
        Node: {
            _$class: msls.ContentItem,
            _$name: "Node",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Historian,
            value: lightSwitchApplication.Node
        },
        RowTemplate: {
            _$class: msls.ContentItem,
            _$name: "RowTemplate",
            _$parentName: "Node",
            screen: lightSwitchApplication.AddEditHistorian,
            data: lightSwitchApplication.Node,
            value: lightSwitchApplication.Node
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditHistorian
        }
    };

    msls._addEntryPoints(lightSwitchApplication.AddEditHistorian, {
        /// <field>
        /// Called when a new AddEditHistorian screen is created.
        /// <br/>created(msls.application.AddEditHistorian screen)
        /// </field>
        created: [lightSwitchApplication.AddEditHistorian],
        /// <field>
        /// Called before changes on an active AddEditHistorian screen are applied.
        /// <br/>beforeApplyChanges(msls.application.AddEditHistorian screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.AddEditHistorian],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("left"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("Name"); }],
        /// <field>
        /// Called after the AssemblyName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AssemblyName_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("AssemblyName"); }],
        /// <field>
        /// Called after the TypeName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        TypeName_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("TypeName"); }],
        /// <field>
        /// Called after the ConnectionString content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ConnectionString_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("ConnectionString"); }],
        /// <field>
        /// Called after the IsLocal content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        IsLocal_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("IsLocal"); }],
        /// <field>
        /// Called after the MeasurementReportingInterval content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        MeasurementReportingInterval_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("MeasurementReportingInterval"); }],
        /// <field>
        /// Called after the Description content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Description_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("Description"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("right"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("LoadOrder"); }],
        /// <field>
        /// Called after the Enabled content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Enabled_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("Enabled"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("CreatedOn"); }],
        /// <field>
        /// Called after the CreatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedBy_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("CreatedBy"); }],
        /// <field>
        /// Called after the UpdatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedOn_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("UpdatedOn"); }],
        /// <field>
        /// Called after the UpdatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedBy_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("UpdatedBy"); }],
        /// <field>
        /// Called after the Node content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Node_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("Node"); }],
        /// <field>
        /// Called after the RowTemplate content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        RowTemplate_postRender: [$element, function () { return new lightSwitchApplication.AddEditHistorian().findContentItem("RowTemplate"); }]
    });

    lightSwitchApplication.BrowseHistorians.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseHistorians
        },
        HistorianList: {
            _$class: msls.ContentItem,
            _$name: "HistorianList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseHistorians,
            data: lightSwitchApplication.BrowseHistorians,
            value: lightSwitchApplication.BrowseHistorians
        },
        Historians: {
            _$class: msls.ContentItem,
            _$name: "Historians",
            _$parentName: "HistorianList",
            screen: lightSwitchApplication.BrowseHistorians,
            data: lightSwitchApplication.BrowseHistorians,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseHistorians,
                _$entry: {
                    elementType: lightSwitchApplication.Historian
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "Historians",
            screen: lightSwitchApplication.BrowseHistorians,
            data: lightSwitchApplication.Historian,
            value: lightSwitchApplication.Historian
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseHistorians,
            data: lightSwitchApplication.Historian,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseHistorians,
            data: lightSwitchApplication.Historian,
            value: String
        },
        AssemblyName: {
            _$class: msls.ContentItem,
            _$name: "AssemblyName",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseHistorians,
            data: lightSwitchApplication.Historian,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseHistorians
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseHistorians, {
        /// <field>
        /// Called when a new BrowseHistorians screen is created.
        /// <br/>created(msls.application.BrowseHistorians screen)
        /// </field>
        created: [lightSwitchApplication.BrowseHistorians],
        /// <field>
        /// Called before changes on an active BrowseHistorians screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseHistorians screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseHistorians],
        /// <field>
        /// Called after the HistorianList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        HistorianList_postRender: [$element, function () { return new lightSwitchApplication.BrowseHistorians().findContentItem("HistorianList"); }],
        /// <field>
        /// Called after the Historians content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Historians_postRender: [$element, function () { return new lightSwitchApplication.BrowseHistorians().findContentItem("Historians"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseHistorians().findContentItem("rows"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.BrowseHistorians().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.BrowseHistorians().findContentItem("Name"); }],
        /// <field>
        /// Called after the AssemblyName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AssemblyName_postRender: [$element, function () { return new lightSwitchApplication.BrowseHistorians().findContentItem("AssemblyName"); }]
    });

    lightSwitchApplication.ViewHistorian.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewHistorian
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.ViewHistorian,
            value: lightSwitchApplication.ViewHistorian
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.ViewHistorian,
            value: lightSwitchApplication.Historian
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: lightSwitchApplication.Historian
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: String
        },
        AssemblyName: {
            _$class: msls.ContentItem,
            _$name: "AssemblyName",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: String
        },
        TypeName: {
            _$class: msls.ContentItem,
            _$name: "TypeName",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: String
        },
        ConnectionString: {
            _$class: msls.ContentItem,
            _$name: "ConnectionString",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: String
        },
        IsLocal: {
            _$class: msls.ContentItem,
            _$name: "IsLocal",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: Boolean
        },
        MeasurementReportingInterval: {
            _$class: msls.ContentItem,
            _$name: "MeasurementReportingInterval",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: Number
        },
        Description: {
            _$class: msls.ContentItem,
            _$name: "Description",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: String
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: lightSwitchApplication.Historian
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: Number
        },
        Enabled: {
            _$class: msls.ContentItem,
            _$name: "Enabled",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: Boolean
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: Date
        },
        CreatedBy: {
            _$class: msls.ContentItem,
            _$name: "CreatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: String
        },
        UpdatedOn: {
            _$class: msls.ContentItem,
            _$name: "UpdatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: Date
        },
        UpdatedBy: {
            _$class: msls.ContentItem,
            _$name: "UpdatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: String
        },
        Node: {
            _$class: msls.ContentItem,
            _$name: "Node",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewHistorian,
            data: lightSwitchApplication.Historian,
            value: lightSwitchApplication.Node
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewHistorian
        }
    };

    msls._addEntryPoints(lightSwitchApplication.ViewHistorian, {
        /// <field>
        /// Called when a new ViewHistorian screen is created.
        /// <br/>created(msls.application.ViewHistorian screen)
        /// </field>
        created: [lightSwitchApplication.ViewHistorian],
        /// <field>
        /// Called before changes on an active ViewHistorian screen are applied.
        /// <br/>beforeApplyChanges(msls.application.ViewHistorian screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.ViewHistorian],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("left"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("Name"); }],
        /// <field>
        /// Called after the AssemblyName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AssemblyName_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("AssemblyName"); }],
        /// <field>
        /// Called after the TypeName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        TypeName_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("TypeName"); }],
        /// <field>
        /// Called after the ConnectionString content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ConnectionString_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("ConnectionString"); }],
        /// <field>
        /// Called after the IsLocal content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        IsLocal_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("IsLocal"); }],
        /// <field>
        /// Called after the MeasurementReportingInterval content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        MeasurementReportingInterval_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("MeasurementReportingInterval"); }],
        /// <field>
        /// Called after the Description content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Description_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("Description"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("right"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("LoadOrder"); }],
        /// <field>
        /// Called after the Enabled content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Enabled_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("Enabled"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("CreatedOn"); }],
        /// <field>
        /// Called after the CreatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedBy_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("CreatedBy"); }],
        /// <field>
        /// Called after the UpdatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedOn_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("UpdatedOn"); }],
        /// <field>
        /// Called after the UpdatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedBy_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("UpdatedBy"); }],
        /// <field>
        /// Called after the Node content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Node_postRender: [$element, function () { return new lightSwitchApplication.ViewHistorian().findContentItem("Node"); }]
    });

    lightSwitchApplication.AddEditInterconnection.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditInterconnection
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.AddEditInterconnection,
            data: lightSwitchApplication.AddEditInterconnection,
            value: lightSwitchApplication.AddEditInterconnection
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.AddEditInterconnection,
            data: lightSwitchApplication.AddEditInterconnection,
            value: lightSwitchApplication.Interconnection
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditInterconnection,
            data: lightSwitchApplication.Interconnection,
            value: lightSwitchApplication.Interconnection
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditInterconnection,
            data: lightSwitchApplication.Interconnection,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditInterconnection,
            data: lightSwitchApplication.Interconnection,
            value: String
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditInterconnection,
            data: lightSwitchApplication.Interconnection,
            value: lightSwitchApplication.Interconnection
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditInterconnection,
            data: lightSwitchApplication.Interconnection,
            value: Number
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditInterconnection
        }
    };

    msls._addEntryPoints(lightSwitchApplication.AddEditInterconnection, {
        /// <field>
        /// Called when a new AddEditInterconnection screen is created.
        /// <br/>created(msls.application.AddEditInterconnection screen)
        /// </field>
        created: [lightSwitchApplication.AddEditInterconnection],
        /// <field>
        /// Called before changes on an active AddEditInterconnection screen are applied.
        /// <br/>beforeApplyChanges(msls.application.AddEditInterconnection screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.AddEditInterconnection],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.AddEditInterconnection().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.AddEditInterconnection().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.AddEditInterconnection().findContentItem("left"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.AddEditInterconnection().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.AddEditInterconnection().findContentItem("Name"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.AddEditInterconnection().findContentItem("right"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.AddEditInterconnection().findContentItem("LoadOrder"); }]
    });

    lightSwitchApplication.BrowseInterconnections.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseInterconnections
        },
        InterconnectionList: {
            _$class: msls.ContentItem,
            _$name: "InterconnectionList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseInterconnections,
            data: lightSwitchApplication.BrowseInterconnections,
            value: lightSwitchApplication.BrowseInterconnections
        },
        Interconnections: {
            _$class: msls.ContentItem,
            _$name: "Interconnections",
            _$parentName: "InterconnectionList",
            screen: lightSwitchApplication.BrowseInterconnections,
            data: lightSwitchApplication.BrowseInterconnections,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseInterconnections,
                _$entry: {
                    elementType: lightSwitchApplication.Interconnection
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "Interconnections",
            screen: lightSwitchApplication.BrowseInterconnections,
            data: lightSwitchApplication.Interconnection,
            value: lightSwitchApplication.Interconnection
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseInterconnections,
            data: lightSwitchApplication.Interconnection,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseInterconnections,
            data: lightSwitchApplication.Interconnection,
            value: String
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseInterconnections,
            data: lightSwitchApplication.Interconnection,
            value: Number
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseInterconnections
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseInterconnections, {
        /// <field>
        /// Called when a new BrowseInterconnections screen is created.
        /// <br/>created(msls.application.BrowseInterconnections screen)
        /// </field>
        created: [lightSwitchApplication.BrowseInterconnections],
        /// <field>
        /// Called before changes on an active BrowseInterconnections screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseInterconnections screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseInterconnections],
        /// <field>
        /// Called after the InterconnectionList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        InterconnectionList_postRender: [$element, function () { return new lightSwitchApplication.BrowseInterconnections().findContentItem("InterconnectionList"); }],
        /// <field>
        /// Called after the Interconnections content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Interconnections_postRender: [$element, function () { return new lightSwitchApplication.BrowseInterconnections().findContentItem("Interconnections"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseInterconnections().findContentItem("rows"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.BrowseInterconnections().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.BrowseInterconnections().findContentItem("Name"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.BrowseInterconnections().findContentItem("LoadOrder"); }]
    });

    lightSwitchApplication.ViewInterconnection.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewInterconnection
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.ViewInterconnection,
            data: lightSwitchApplication.ViewInterconnection,
            value: lightSwitchApplication.ViewInterconnection
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.ViewInterconnection,
            data: lightSwitchApplication.ViewInterconnection,
            value: lightSwitchApplication.Interconnection
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewInterconnection,
            data: lightSwitchApplication.Interconnection,
            value: lightSwitchApplication.Interconnection
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewInterconnection,
            data: lightSwitchApplication.Interconnection,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewInterconnection,
            data: lightSwitchApplication.Interconnection,
            value: String
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewInterconnection,
            data: lightSwitchApplication.Interconnection,
            value: lightSwitchApplication.Interconnection
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewInterconnection,
            data: lightSwitchApplication.Interconnection,
            value: Number
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewInterconnection
        }
    };

    msls._addEntryPoints(lightSwitchApplication.ViewInterconnection, {
        /// <field>
        /// Called when a new ViewInterconnection screen is created.
        /// <br/>created(msls.application.ViewInterconnection screen)
        /// </field>
        created: [lightSwitchApplication.ViewInterconnection],
        /// <field>
        /// Called before changes on an active ViewInterconnection screen are applied.
        /// <br/>beforeApplyChanges(msls.application.ViewInterconnection screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.ViewInterconnection],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.ViewInterconnection().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.ViewInterconnection().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.ViewInterconnection().findContentItem("left"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.ViewInterconnection().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.ViewInterconnection().findContentItem("Name"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.ViewInterconnection().findContentItem("right"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.ViewInterconnection().findContentItem("LoadOrder"); }]
    });

    lightSwitchApplication.AddEditMeasurement.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditMeasurement
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.AddEditMeasurement,
            value: lightSwitchApplication.AddEditMeasurement
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.AddEditMeasurement,
            value: lightSwitchApplication.Measurement
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: lightSwitchApplication.Measurement
        },
        SignalID: {
            _$class: msls.ContentItem,
            _$name: "SignalID",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: String
        },
        HistorianID: {
            _$class: msls.ContentItem,
            _$name: "HistorianID",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: Number
        },
        PointTag: {
            _$class: msls.ContentItem,
            _$name: "PointTag",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: String
        },
        AlternateTag: {
            _$class: msls.ContentItem,
            _$name: "AlternateTag",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: String
        },
        SignalReference: {
            _$class: msls.ContentItem,
            _$name: "SignalReference",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: String
        },
        Adder: {
            _$class: msls.ContentItem,
            _$name: "Adder",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: Number
        },
        Multiplier: {
            _$class: msls.ContentItem,
            _$name: "Multiplier",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: Number
        },
        Description: {
            _$class: msls.ContentItem,
            _$name: "Description",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: String
        },
        Internal: {
            _$class: msls.ContentItem,
            _$name: "Internal",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: Boolean
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: lightSwitchApplication.Measurement
        },
        Subscribed: {
            _$class: msls.ContentItem,
            _$name: "Subscribed",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: Boolean
        },
        Enabled: {
            _$class: msls.ContentItem,
            _$name: "Enabled",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: Boolean
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: Date
        },
        CreatedBy: {
            _$class: msls.ContentItem,
            _$name: "CreatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: String
        },
        UpdatedOn: {
            _$class: msls.ContentItem,
            _$name: "UpdatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: Date
        },
        UpdatedBy: {
            _$class: msls.ContentItem,
            _$name: "UpdatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: String
        },
        Device: {
            _$class: msls.ContentItem,
            _$name: "Device",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: lightSwitchApplication.Device
        },
        RowTemplate: {
            _$class: msls.ContentItem,
            _$name: "RowTemplate",
            _$parentName: "Device",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Device,
            value: lightSwitchApplication.Device
        },
        SignalType: {
            _$class: msls.ContentItem,
            _$name: "SignalType",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.Measurement,
            value: lightSwitchApplication.SignalType
        },
        RowTemplate1: {
            _$class: msls.ContentItem,
            _$name: "RowTemplate1",
            _$parentName: "SignalType",
            screen: lightSwitchApplication.AddEditMeasurement,
            data: lightSwitchApplication.SignalType,
            value: lightSwitchApplication.SignalType
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditMeasurement
        }
    };

    msls._addEntryPoints(lightSwitchApplication.AddEditMeasurement, {
        /// <field>
        /// Called when a new AddEditMeasurement screen is created.
        /// <br/>created(msls.application.AddEditMeasurement screen)
        /// </field>
        created: [lightSwitchApplication.AddEditMeasurement],
        /// <field>
        /// Called before changes on an active AddEditMeasurement screen are applied.
        /// <br/>beforeApplyChanges(msls.application.AddEditMeasurement screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.AddEditMeasurement],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("left"); }],
        /// <field>
        /// Called after the SignalID content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        SignalID_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("SignalID"); }],
        /// <field>
        /// Called after the HistorianID content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        HistorianID_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("HistorianID"); }],
        /// <field>
        /// Called after the PointTag content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        PointTag_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("PointTag"); }],
        /// <field>
        /// Called after the AlternateTag content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AlternateTag_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("AlternateTag"); }],
        /// <field>
        /// Called after the SignalReference content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        SignalReference_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("SignalReference"); }],
        /// <field>
        /// Called after the Adder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Adder_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("Adder"); }],
        /// <field>
        /// Called after the Multiplier content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Multiplier_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("Multiplier"); }],
        /// <field>
        /// Called after the Description content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Description_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("Description"); }],
        /// <field>
        /// Called after the Internal content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Internal_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("Internal"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("right"); }],
        /// <field>
        /// Called after the Subscribed content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Subscribed_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("Subscribed"); }],
        /// <field>
        /// Called after the Enabled content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Enabled_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("Enabled"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("CreatedOn"); }],
        /// <field>
        /// Called after the CreatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedBy_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("CreatedBy"); }],
        /// <field>
        /// Called after the UpdatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedOn_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("UpdatedOn"); }],
        /// <field>
        /// Called after the UpdatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedBy_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("UpdatedBy"); }],
        /// <field>
        /// Called after the Device content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Device_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("Device"); }],
        /// <field>
        /// Called after the RowTemplate content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        RowTemplate_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("RowTemplate"); }],
        /// <field>
        /// Called after the SignalType content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        SignalType_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("SignalType"); }],
        /// <field>
        /// Called after the RowTemplate1 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        RowTemplate1_postRender: [$element, function () { return new lightSwitchApplication.AddEditMeasurement().findContentItem("RowTemplate1"); }]
    });

    lightSwitchApplication.BrowseMeasurements.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseMeasurements
        },
        MeasurementList: {
            _$class: msls.ContentItem,
            _$name: "MeasurementList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseMeasurements,
            data: lightSwitchApplication.BrowseMeasurements,
            value: lightSwitchApplication.BrowseMeasurements
        },
        Measurements: {
            _$class: msls.ContentItem,
            _$name: "Measurements",
            _$parentName: "MeasurementList",
            screen: lightSwitchApplication.BrowseMeasurements,
            data: lightSwitchApplication.BrowseMeasurements,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseMeasurements,
                _$entry: {
                    elementType: lightSwitchApplication.Measurement
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "Measurements",
            screen: lightSwitchApplication.BrowseMeasurements,
            data: lightSwitchApplication.Measurement,
            value: lightSwitchApplication.Measurement
        },
        PointTag: {
            _$class: msls.ContentItem,
            _$name: "PointTag",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseMeasurements,
            data: lightSwitchApplication.Measurement,
            value: String
        },
        SignalID: {
            _$class: msls.ContentItem,
            _$name: "SignalID",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseMeasurements,
            data: lightSwitchApplication.Measurement,
            value: String
        },
        HistorianID: {
            _$class: msls.ContentItem,
            _$name: "HistorianID",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseMeasurements,
            data: lightSwitchApplication.Measurement,
            value: Number
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseMeasurements
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseMeasurements, {
        /// <field>
        /// Called when a new BrowseMeasurements screen is created.
        /// <br/>created(msls.application.BrowseMeasurements screen)
        /// </field>
        created: [lightSwitchApplication.BrowseMeasurements],
        /// <field>
        /// Called before changes on an active BrowseMeasurements screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseMeasurements screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseMeasurements],
        /// <field>
        /// Called after the MeasurementList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        MeasurementList_postRender: [$element, function () { return new lightSwitchApplication.BrowseMeasurements().findContentItem("MeasurementList"); }],
        /// <field>
        /// Called after the Measurements content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Measurements_postRender: [$element, function () { return new lightSwitchApplication.BrowseMeasurements().findContentItem("Measurements"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseMeasurements().findContentItem("rows"); }],
        /// <field>
        /// Called after the PointTag content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        PointTag_postRender: [$element, function () { return new lightSwitchApplication.BrowseMeasurements().findContentItem("PointTag"); }],
        /// <field>
        /// Called after the SignalID content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        SignalID_postRender: [$element, function () { return new lightSwitchApplication.BrowseMeasurements().findContentItem("SignalID"); }],
        /// <field>
        /// Called after the HistorianID content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        HistorianID_postRender: [$element, function () { return new lightSwitchApplication.BrowseMeasurements().findContentItem("HistorianID"); }]
    });

    lightSwitchApplication.ViewMeasurement.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewMeasurement
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.ViewMeasurement,
            value: lightSwitchApplication.ViewMeasurement
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.ViewMeasurement,
            value: lightSwitchApplication.Measurement
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: lightSwitchApplication.Measurement
        },
        SignalID: {
            _$class: msls.ContentItem,
            _$name: "SignalID",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: String
        },
        HistorianID: {
            _$class: msls.ContentItem,
            _$name: "HistorianID",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: Number
        },
        PointTag: {
            _$class: msls.ContentItem,
            _$name: "PointTag",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: String
        },
        AlternateTag: {
            _$class: msls.ContentItem,
            _$name: "AlternateTag",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: String
        },
        SignalReference: {
            _$class: msls.ContentItem,
            _$name: "SignalReference",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: String
        },
        Adder: {
            _$class: msls.ContentItem,
            _$name: "Adder",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: Number
        },
        Multiplier: {
            _$class: msls.ContentItem,
            _$name: "Multiplier",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: Number
        },
        Description: {
            _$class: msls.ContentItem,
            _$name: "Description",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: String
        },
        Internal: {
            _$class: msls.ContentItem,
            _$name: "Internal",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: Boolean
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: lightSwitchApplication.Measurement
        },
        Subscribed: {
            _$class: msls.ContentItem,
            _$name: "Subscribed",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: Boolean
        },
        Enabled: {
            _$class: msls.ContentItem,
            _$name: "Enabled",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: Boolean
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: Date
        },
        CreatedBy: {
            _$class: msls.ContentItem,
            _$name: "CreatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: String
        },
        UpdatedOn: {
            _$class: msls.ContentItem,
            _$name: "UpdatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: Date
        },
        UpdatedBy: {
            _$class: msls.ContentItem,
            _$name: "UpdatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: String
        },
        Device: {
            _$class: msls.ContentItem,
            _$name: "Device",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: lightSwitchApplication.Device
        },
        SignalType: {
            _$class: msls.ContentItem,
            _$name: "SignalType",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewMeasurement,
            data: lightSwitchApplication.Measurement,
            value: lightSwitchApplication.SignalType
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewMeasurement
        }
    };

    msls._addEntryPoints(lightSwitchApplication.ViewMeasurement, {
        /// <field>
        /// Called when a new ViewMeasurement screen is created.
        /// <br/>created(msls.application.ViewMeasurement screen)
        /// </field>
        created: [lightSwitchApplication.ViewMeasurement],
        /// <field>
        /// Called before changes on an active ViewMeasurement screen are applied.
        /// <br/>beforeApplyChanges(msls.application.ViewMeasurement screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.ViewMeasurement],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("left"); }],
        /// <field>
        /// Called after the SignalID content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        SignalID_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("SignalID"); }],
        /// <field>
        /// Called after the HistorianID content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        HistorianID_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("HistorianID"); }],
        /// <field>
        /// Called after the PointTag content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        PointTag_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("PointTag"); }],
        /// <field>
        /// Called after the AlternateTag content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AlternateTag_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("AlternateTag"); }],
        /// <field>
        /// Called after the SignalReference content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        SignalReference_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("SignalReference"); }],
        /// <field>
        /// Called after the Adder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Adder_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("Adder"); }],
        /// <field>
        /// Called after the Multiplier content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Multiplier_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("Multiplier"); }],
        /// <field>
        /// Called after the Description content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Description_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("Description"); }],
        /// <field>
        /// Called after the Internal content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Internal_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("Internal"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("right"); }],
        /// <field>
        /// Called after the Subscribed content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Subscribed_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("Subscribed"); }],
        /// <field>
        /// Called after the Enabled content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Enabled_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("Enabled"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("CreatedOn"); }],
        /// <field>
        /// Called after the CreatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedBy_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("CreatedBy"); }],
        /// <field>
        /// Called after the UpdatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedOn_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("UpdatedOn"); }],
        /// <field>
        /// Called after the UpdatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedBy_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("UpdatedBy"); }],
        /// <field>
        /// Called after the Device content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Device_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("Device"); }],
        /// <field>
        /// Called after the SignalType content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        SignalType_postRender: [$element, function () { return new lightSwitchApplication.ViewMeasurement().findContentItem("SignalType"); }]
    });

    lightSwitchApplication.AddEditNodes.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditNodes
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.AddEditNodes,
            value: lightSwitchApplication.AddEditNodes
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.AddEditNodes,
            value: lightSwitchApplication.Node
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.Node,
            value: lightSwitchApplication.Node
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        Description: {
            _$class: msls.ContentItem,
            _$name: "Description",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        ImagePath: {
            _$class: msls.ContentItem,
            _$name: "ImagePath",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        Settings: {
            _$class: msls.ContentItem,
            _$name: "Settings",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        Enabled: {
            _$class: msls.ContentItem,
            _$name: "Enabled",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.Node,
            value: Boolean
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.Node,
            value: lightSwitchApplication.Node
        },
        Master: {
            _$class: msls.ContentItem,
            _$name: "Master",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.Node,
            value: Boolean
        },
        Longitude: {
            _$class: msls.ContentItem,
            _$name: "Longitude",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        Latitude: {
            _$class: msls.ContentItem,
            _$name: "Latitude",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.Node,
            value: Number
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.Node,
            value: Date
        },
        CreatedBy: {
            _$class: msls.ContentItem,
            _$name: "CreatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        UpdatedOn: {
            _$class: msls.ContentItem,
            _$name: "UpdatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.Node,
            value: Date
        },
        UpdatedBy: {
            _$class: msls.ContentItem,
            _$name: "UpdatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditNodes
        }
    };

    msls._addEntryPoints(lightSwitchApplication.AddEditNodes, {
        /// <field>
        /// Called when a new AddEditNodes screen is created.
        /// <br/>created(msls.application.AddEditNodes screen)
        /// </field>
        created: [lightSwitchApplication.AddEditNodes],
        /// <field>
        /// Called before changes on an active AddEditNodes screen are applied.
        /// <br/>beforeApplyChanges(msls.application.AddEditNodes screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.AddEditNodes],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("left"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("Name"); }],
        /// <field>
        /// Called after the Description content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Description_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("Description"); }],
        /// <field>
        /// Called after the ImagePath content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ImagePath_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("ImagePath"); }],
        /// <field>
        /// Called after the Settings content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Settings_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("Settings"); }],
        /// <field>
        /// Called after the Enabled content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Enabled_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("Enabled"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("right"); }],
        /// <field>
        /// Called after the Master content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Master_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("Master"); }],
        /// <field>
        /// Called after the Longitude content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Longitude_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("Longitude"); }],
        /// <field>
        /// Called after the Latitude content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Latitude_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("Latitude"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("LoadOrder"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("CreatedOn"); }],
        /// <field>
        /// Called after the CreatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedBy_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("CreatedBy"); }],
        /// <field>
        /// Called after the UpdatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedOn_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("UpdatedOn"); }],
        /// <field>
        /// Called after the UpdatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedBy_postRender: [$element, function () { return new lightSwitchApplication.AddEditNodes().findContentItem("UpdatedBy"); }]
    });

    lightSwitchApplication.BrowseNodesSet.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseNodesSet
        },
        NodesList: {
            _$class: msls.ContentItem,
            _$name: "NodesList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseNodesSet,
            data: lightSwitchApplication.BrowseNodesSet,
            value: lightSwitchApplication.BrowseNodesSet
        },
        Nodes: {
            _$class: msls.ContentItem,
            _$name: "Nodes",
            _$parentName: "NodesList",
            screen: lightSwitchApplication.BrowseNodesSet,
            data: lightSwitchApplication.BrowseNodesSet,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseNodesSet,
                _$entry: {
                    elementType: lightSwitchApplication.Node
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "Nodes",
            screen: lightSwitchApplication.BrowseNodesSet,
            data: lightSwitchApplication.Node,
            value: lightSwitchApplication.Node
        },
        ID: {
            _$class: msls.ContentItem,
            _$name: "ID",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseNodesSet,
            data: lightSwitchApplication.Node,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseNodesSet,
            data: lightSwitchApplication.Node,
            value: String
        },
        Description: {
            _$class: msls.ContentItem,
            _$name: "Description",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseNodesSet,
            data: lightSwitchApplication.Node,
            value: String
        },
        Enabled: {
            _$class: msls.ContentItem,
            _$name: "Enabled",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseNodesSet,
            data: lightSwitchApplication.Node,
            value: Boolean
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseNodesSet
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseNodesSet, {
        /// <field>
        /// Called when a new BrowseNodesSet screen is created.
        /// <br/>created(msls.application.BrowseNodesSet screen)
        /// </field>
        created: [lightSwitchApplication.BrowseNodesSet],
        /// <field>
        /// Called before changes on an active BrowseNodesSet screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseNodesSet screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseNodesSet],
        /// <field>
        /// Called after the NodesList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        NodesList_postRender: [$element, function () { return new lightSwitchApplication.BrowseNodesSet().findContentItem("NodesList"); }],
        /// <field>
        /// Called after the Nodes content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Nodes_postRender: [$element, function () { return new lightSwitchApplication.BrowseNodesSet().findContentItem("Nodes"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseNodesSet().findContentItem("rows"); }],
        /// <field>
        /// Called after the ID content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ID_postRender: [$element, function () { return new lightSwitchApplication.BrowseNodesSet().findContentItem("ID"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.BrowseNodesSet().findContentItem("Name"); }],
        /// <field>
        /// Called after the Description content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Description_postRender: [$element, function () { return new lightSwitchApplication.BrowseNodesSet().findContentItem("Description"); }],
        /// <field>
        /// Called after the Enabled content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Enabled_postRender: [$element, function () { return new lightSwitchApplication.BrowseNodesSet().findContentItem("Enabled"); }]
    });

    lightSwitchApplication.ViewNodes.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewNodes
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.ViewNodes,
            value: lightSwitchApplication.ViewNodes
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.ViewNodes,
            value: lightSwitchApplication.Node
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: lightSwitchApplication.Node
        },
        ID: {
            _$class: msls.ContentItem,
            _$name: "ID",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        Longitude: {
            _$class: msls.ContentItem,
            _$name: "Longitude",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        Latitude: {
            _$class: msls.ContentItem,
            _$name: "Latitude",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        Description: {
            _$class: msls.ContentItem,
            _$name: "Description",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        ImagePath: {
            _$class: msls.ContentItem,
            _$name: "ImagePath",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        Settings: {
            _$class: msls.ContentItem,
            _$name: "Settings",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        MenuType: {
            _$class: msls.ContentItem,
            _$name: "MenuType",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        MenuData: {
            _$class: msls.ContentItem,
            _$name: "MenuData",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: lightSwitchApplication.Node
        },
        Master: {
            _$class: msls.ContentItem,
            _$name: "Master",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: Boolean
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: Number
        },
        Enabled: {
            _$class: msls.ContentItem,
            _$name: "Enabled",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: Boolean
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: Date
        },
        CreatedBy: {
            _$class: msls.ContentItem,
            _$name: "CreatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        UpdatedOn: {
            _$class: msls.ContentItem,
            _$name: "UpdatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: Date
        },
        UpdatedBy: {
            _$class: msls.ContentItem,
            _$name: "UpdatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewNodes,
            data: lightSwitchApplication.Node,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewNodes
        }
    };

    msls._addEntryPoints(lightSwitchApplication.ViewNodes, {
        /// <field>
        /// Called when a new ViewNodes screen is created.
        /// <br/>created(msls.application.ViewNodes screen)
        /// </field>
        created: [lightSwitchApplication.ViewNodes],
        /// <field>
        /// Called before changes on an active ViewNodes screen are applied.
        /// <br/>beforeApplyChanges(msls.application.ViewNodes screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.ViewNodes],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("left"); }],
        /// <field>
        /// Called after the ID content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ID_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("ID"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("Name"); }],
        /// <field>
        /// Called after the Longitude content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Longitude_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("Longitude"); }],
        /// <field>
        /// Called after the Latitude content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Latitude_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("Latitude"); }],
        /// <field>
        /// Called after the Description content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Description_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("Description"); }],
        /// <field>
        /// Called after the ImagePath content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ImagePath_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("ImagePath"); }],
        /// <field>
        /// Called after the Settings content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Settings_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("Settings"); }],
        /// <field>
        /// Called after the MenuType content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        MenuType_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("MenuType"); }],
        /// <field>
        /// Called after the MenuData content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        MenuData_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("MenuData"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("right"); }],
        /// <field>
        /// Called after the Master content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Master_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("Master"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("LoadOrder"); }],
        /// <field>
        /// Called after the Enabled content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Enabled_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("Enabled"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("CreatedOn"); }],
        /// <field>
        /// Called after the CreatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedBy_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("CreatedBy"); }],
        /// <field>
        /// Called after the UpdatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedOn_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("UpdatedOn"); }],
        /// <field>
        /// Called after the UpdatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedBy_postRender: [$element, function () { return new lightSwitchApplication.ViewNodes().findContentItem("UpdatedBy"); }]
    });

    lightSwitchApplication.AddEditProtocol.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditProtocol
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.AddEditProtocol,
            data: lightSwitchApplication.AddEditProtocol,
            value: lightSwitchApplication.AddEditProtocol
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.AddEditProtocol,
            data: lightSwitchApplication.AddEditProtocol,
            value: lightSwitchApplication.Protocol
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditProtocol,
            data: lightSwitchApplication.Protocol,
            value: lightSwitchApplication.Protocol
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditProtocol,
            data: lightSwitchApplication.Protocol,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditProtocol,
            data: lightSwitchApplication.Protocol,
            value: String
        },
        Type: {
            _$class: msls.ContentItem,
            _$name: "Type",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditProtocol,
            data: lightSwitchApplication.Protocol,
            value: String
        },
        Category: {
            _$class: msls.ContentItem,
            _$name: "Category",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditProtocol,
            data: lightSwitchApplication.Protocol,
            value: String
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditProtocol,
            data: lightSwitchApplication.Protocol,
            value: lightSwitchApplication.Protocol
        },
        AssemblyName: {
            _$class: msls.ContentItem,
            _$name: "AssemblyName",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditProtocol,
            data: lightSwitchApplication.Protocol,
            value: String
        },
        TypeName: {
            _$class: msls.ContentItem,
            _$name: "TypeName",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditProtocol,
            data: lightSwitchApplication.Protocol,
            value: String
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditProtocol,
            data: lightSwitchApplication.Protocol,
            value: Number
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditProtocol
        }
    };

    msls._addEntryPoints(lightSwitchApplication.AddEditProtocol, {
        /// <field>
        /// Called when a new AddEditProtocol screen is created.
        /// <br/>created(msls.application.AddEditProtocol screen)
        /// </field>
        created: [lightSwitchApplication.AddEditProtocol],
        /// <field>
        /// Called before changes on an active AddEditProtocol screen are applied.
        /// <br/>beforeApplyChanges(msls.application.AddEditProtocol screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.AddEditProtocol],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.AddEditProtocol().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.AddEditProtocol().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.AddEditProtocol().findContentItem("left"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.AddEditProtocol().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.AddEditProtocol().findContentItem("Name"); }],
        /// <field>
        /// Called after the Type content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Type_postRender: [$element, function () { return new lightSwitchApplication.AddEditProtocol().findContentItem("Type"); }],
        /// <field>
        /// Called after the Category content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Category_postRender: [$element, function () { return new lightSwitchApplication.AddEditProtocol().findContentItem("Category"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.AddEditProtocol().findContentItem("right"); }],
        /// <field>
        /// Called after the AssemblyName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AssemblyName_postRender: [$element, function () { return new lightSwitchApplication.AddEditProtocol().findContentItem("AssemblyName"); }],
        /// <field>
        /// Called after the TypeName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        TypeName_postRender: [$element, function () { return new lightSwitchApplication.AddEditProtocol().findContentItem("TypeName"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.AddEditProtocol().findContentItem("LoadOrder"); }]
    });

    lightSwitchApplication.BrowseProtocols.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseProtocols
        },
        ProtocolList: {
            _$class: msls.ContentItem,
            _$name: "ProtocolList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseProtocols,
            data: lightSwitchApplication.BrowseProtocols,
            value: lightSwitchApplication.BrowseProtocols
        },
        Protocols: {
            _$class: msls.ContentItem,
            _$name: "Protocols",
            _$parentName: "ProtocolList",
            screen: lightSwitchApplication.BrowseProtocols,
            data: lightSwitchApplication.BrowseProtocols,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseProtocols,
                _$entry: {
                    elementType: lightSwitchApplication.Protocol
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "Protocols",
            screen: lightSwitchApplication.BrowseProtocols,
            data: lightSwitchApplication.Protocol,
            value: lightSwitchApplication.Protocol
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseProtocols,
            data: lightSwitchApplication.Protocol,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseProtocols,
            data: lightSwitchApplication.Protocol,
            value: String
        },
        Type: {
            _$class: msls.ContentItem,
            _$name: "Type",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseProtocols,
            data: lightSwitchApplication.Protocol,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseProtocols
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseProtocols, {
        /// <field>
        /// Called when a new BrowseProtocols screen is created.
        /// <br/>created(msls.application.BrowseProtocols screen)
        /// </field>
        created: [lightSwitchApplication.BrowseProtocols],
        /// <field>
        /// Called before changes on an active BrowseProtocols screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseProtocols screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseProtocols],
        /// <field>
        /// Called after the ProtocolList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ProtocolList_postRender: [$element, function () { return new lightSwitchApplication.BrowseProtocols().findContentItem("ProtocolList"); }],
        /// <field>
        /// Called after the Protocols content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Protocols_postRender: [$element, function () { return new lightSwitchApplication.BrowseProtocols().findContentItem("Protocols"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseProtocols().findContentItem("rows"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.BrowseProtocols().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.BrowseProtocols().findContentItem("Name"); }],
        /// <field>
        /// Called after the Type content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Type_postRender: [$element, function () { return new lightSwitchApplication.BrowseProtocols().findContentItem("Type"); }]
    });

    lightSwitchApplication.ViewProtocol.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewProtocol
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.ViewProtocol,
            data: lightSwitchApplication.ViewProtocol,
            value: lightSwitchApplication.ViewProtocol
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.ViewProtocol,
            data: lightSwitchApplication.ViewProtocol,
            value: lightSwitchApplication.Protocol
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewProtocol,
            data: lightSwitchApplication.Protocol,
            value: lightSwitchApplication.Protocol
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewProtocol,
            data: lightSwitchApplication.Protocol,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewProtocol,
            data: lightSwitchApplication.Protocol,
            value: String
        },
        Type: {
            _$class: msls.ContentItem,
            _$name: "Type",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewProtocol,
            data: lightSwitchApplication.Protocol,
            value: String
        },
        Category: {
            _$class: msls.ContentItem,
            _$name: "Category",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewProtocol,
            data: lightSwitchApplication.Protocol,
            value: String
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewProtocol,
            data: lightSwitchApplication.Protocol,
            value: lightSwitchApplication.Protocol
        },
        AssemblyName: {
            _$class: msls.ContentItem,
            _$name: "AssemblyName",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewProtocol,
            data: lightSwitchApplication.Protocol,
            value: String
        },
        TypeName: {
            _$class: msls.ContentItem,
            _$name: "TypeName",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewProtocol,
            data: lightSwitchApplication.Protocol,
            value: String
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewProtocol,
            data: lightSwitchApplication.Protocol,
            value: Number
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewProtocol
        }
    };

    msls._addEntryPoints(lightSwitchApplication.ViewProtocol, {
        /// <field>
        /// Called when a new ViewProtocol screen is created.
        /// <br/>created(msls.application.ViewProtocol screen)
        /// </field>
        created: [lightSwitchApplication.ViewProtocol],
        /// <field>
        /// Called before changes on an active ViewProtocol screen are applied.
        /// <br/>beforeApplyChanges(msls.application.ViewProtocol screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.ViewProtocol],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.ViewProtocol().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.ViewProtocol().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.ViewProtocol().findContentItem("left"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.ViewProtocol().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.ViewProtocol().findContentItem("Name"); }],
        /// <field>
        /// Called after the Type content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Type_postRender: [$element, function () { return new lightSwitchApplication.ViewProtocol().findContentItem("Type"); }],
        /// <field>
        /// Called after the Category content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Category_postRender: [$element, function () { return new lightSwitchApplication.ViewProtocol().findContentItem("Category"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.ViewProtocol().findContentItem("right"); }],
        /// <field>
        /// Called after the AssemblyName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AssemblyName_postRender: [$element, function () { return new lightSwitchApplication.ViewProtocol().findContentItem("AssemblyName"); }],
        /// <field>
        /// Called after the TypeName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        TypeName_postRender: [$element, function () { return new lightSwitchApplication.ViewProtocol().findContentItem("TypeName"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.ViewProtocol().findContentItem("LoadOrder"); }]
    });

    lightSwitchApplication.AddEditSignalType.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditSignalType
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.AddEditSignalType,
            data: lightSwitchApplication.AddEditSignalType,
            value: lightSwitchApplication.AddEditSignalType
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.AddEditSignalType,
            data: lightSwitchApplication.AddEditSignalType,
            value: lightSwitchApplication.SignalType
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditSignalType,
            data: lightSwitchApplication.SignalType,
            value: lightSwitchApplication.SignalType
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditSignalType,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditSignalType,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        Suffix: {
            _$class: msls.ContentItem,
            _$name: "Suffix",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditSignalType,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        Abbreviation: {
            _$class: msls.ContentItem,
            _$name: "Abbreviation",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditSignalType,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditSignalType,
            data: lightSwitchApplication.SignalType,
            value: lightSwitchApplication.SignalType
        },
        LongAcronym: {
            _$class: msls.ContentItem,
            _$name: "LongAcronym",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditSignalType,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        Source: {
            _$class: msls.ContentItem,
            _$name: "Source",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditSignalType,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        EngineeringUnits: {
            _$class: msls.ContentItem,
            _$name: "EngineeringUnits",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditSignalType,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditSignalType
        }
    };

    msls._addEntryPoints(lightSwitchApplication.AddEditSignalType, {
        /// <field>
        /// Called when a new AddEditSignalType screen is created.
        /// <br/>created(msls.application.AddEditSignalType screen)
        /// </field>
        created: [lightSwitchApplication.AddEditSignalType],
        /// <field>
        /// Called before changes on an active AddEditSignalType screen are applied.
        /// <br/>beforeApplyChanges(msls.application.AddEditSignalType screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.AddEditSignalType],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.AddEditSignalType().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.AddEditSignalType().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.AddEditSignalType().findContentItem("left"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.AddEditSignalType().findContentItem("Name"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.AddEditSignalType().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Suffix content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Suffix_postRender: [$element, function () { return new lightSwitchApplication.AddEditSignalType().findContentItem("Suffix"); }],
        /// <field>
        /// Called after the Abbreviation content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Abbreviation_postRender: [$element, function () { return new lightSwitchApplication.AddEditSignalType().findContentItem("Abbreviation"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.AddEditSignalType().findContentItem("right"); }],
        /// <field>
        /// Called after the LongAcronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LongAcronym_postRender: [$element, function () { return new lightSwitchApplication.AddEditSignalType().findContentItem("LongAcronym"); }],
        /// <field>
        /// Called after the Source content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Source_postRender: [$element, function () { return new lightSwitchApplication.AddEditSignalType().findContentItem("Source"); }],
        /// <field>
        /// Called after the EngineeringUnits content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        EngineeringUnits_postRender: [$element, function () { return new lightSwitchApplication.AddEditSignalType().findContentItem("EngineeringUnits"); }]
    });

    lightSwitchApplication.BrowseSignalTypes.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseSignalTypes
        },
        SignalTypeList: {
            _$class: msls.ContentItem,
            _$name: "SignalTypeList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseSignalTypes,
            data: lightSwitchApplication.BrowseSignalTypes,
            value: lightSwitchApplication.BrowseSignalTypes
        },
        SignalTypes: {
            _$class: msls.ContentItem,
            _$name: "SignalTypes",
            _$parentName: "SignalTypeList",
            screen: lightSwitchApplication.BrowseSignalTypes,
            data: lightSwitchApplication.BrowseSignalTypes,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseSignalTypes,
                _$entry: {
                    elementType: lightSwitchApplication.SignalType
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "SignalTypes",
            screen: lightSwitchApplication.BrowseSignalTypes,
            data: lightSwitchApplication.SignalType,
            value: lightSwitchApplication.SignalType
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseSignalTypes,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseSignalTypes,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        Suffix: {
            _$class: msls.ContentItem,
            _$name: "Suffix",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseSignalTypes,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseSignalTypes
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseSignalTypes, {
        /// <field>
        /// Called when a new BrowseSignalTypes screen is created.
        /// <br/>created(msls.application.BrowseSignalTypes screen)
        /// </field>
        created: [lightSwitchApplication.BrowseSignalTypes],
        /// <field>
        /// Called before changes on an active BrowseSignalTypes screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseSignalTypes screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseSignalTypes],
        /// <field>
        /// Called after the SignalTypeList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        SignalTypeList_postRender: [$element, function () { return new lightSwitchApplication.BrowseSignalTypes().findContentItem("SignalTypeList"); }],
        /// <field>
        /// Called after the SignalTypes content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        SignalTypes_postRender: [$element, function () { return new lightSwitchApplication.BrowseSignalTypes().findContentItem("SignalTypes"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseSignalTypes().findContentItem("rows"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.BrowseSignalTypes().findContentItem("Name"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.BrowseSignalTypes().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Suffix content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Suffix_postRender: [$element, function () { return new lightSwitchApplication.BrowseSignalTypes().findContentItem("Suffix"); }]
    });

    lightSwitchApplication.ViewSignalType.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewSignalType
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.ViewSignalType,
            data: lightSwitchApplication.ViewSignalType,
            value: lightSwitchApplication.ViewSignalType
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.ViewSignalType,
            data: lightSwitchApplication.ViewSignalType,
            value: lightSwitchApplication.SignalType
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewSignalType,
            data: lightSwitchApplication.SignalType,
            value: lightSwitchApplication.SignalType
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewSignalType,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewSignalType,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        Suffix: {
            _$class: msls.ContentItem,
            _$name: "Suffix",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewSignalType,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        Abbreviation: {
            _$class: msls.ContentItem,
            _$name: "Abbreviation",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewSignalType,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewSignalType,
            data: lightSwitchApplication.SignalType,
            value: lightSwitchApplication.SignalType
        },
        LongAcronym: {
            _$class: msls.ContentItem,
            _$name: "LongAcronym",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewSignalType,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        Source: {
            _$class: msls.ContentItem,
            _$name: "Source",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewSignalType,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        EngineeringUnits: {
            _$class: msls.ContentItem,
            _$name: "EngineeringUnits",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewSignalType,
            data: lightSwitchApplication.SignalType,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewSignalType
        }
    };

    msls._addEntryPoints(lightSwitchApplication.ViewSignalType, {
        /// <field>
        /// Called when a new ViewSignalType screen is created.
        /// <br/>created(msls.application.ViewSignalType screen)
        /// </field>
        created: [lightSwitchApplication.ViewSignalType],
        /// <field>
        /// Called before changes on an active ViewSignalType screen are applied.
        /// <br/>beforeApplyChanges(msls.application.ViewSignalType screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.ViewSignalType],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.ViewSignalType().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.ViewSignalType().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.ViewSignalType().findContentItem("left"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.ViewSignalType().findContentItem("Name"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.ViewSignalType().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Suffix content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Suffix_postRender: [$element, function () { return new lightSwitchApplication.ViewSignalType().findContentItem("Suffix"); }],
        /// <field>
        /// Called after the Abbreviation content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Abbreviation_postRender: [$element, function () { return new lightSwitchApplication.ViewSignalType().findContentItem("Abbreviation"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.ViewSignalType().findContentItem("right"); }],
        /// <field>
        /// Called after the LongAcronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LongAcronym_postRender: [$element, function () { return new lightSwitchApplication.ViewSignalType().findContentItem("LongAcronym"); }],
        /// <field>
        /// Called after the Source content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Source_postRender: [$element, function () { return new lightSwitchApplication.ViewSignalType().findContentItem("Source"); }],
        /// <field>
        /// Called after the EngineeringUnits content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        EngineeringUnits_postRender: [$element, function () { return new lightSwitchApplication.ViewSignalType().findContentItem("EngineeringUnits"); }]
    });

    lightSwitchApplication.AddEditStatistic.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditStatistic
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.AddEditStatistic,
            value: lightSwitchApplication.AddEditStatistic
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.AddEditStatistic,
            value: lightSwitchApplication.Statistic
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.Statistic,
            value: lightSwitchApplication.Statistic
        },
        Source: {
            _$class: msls.ContentItem,
            _$name: "Source",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        SignalIndex: {
            _$class: msls.ContentItem,
            _$name: "SignalIndex",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.Statistic,
            value: Number
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        Description: {
            _$class: msls.ContentItem,
            _$name: "Description",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        AssemblyName: {
            _$class: msls.ContentItem,
            _$name: "AssemblyName",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        TypeName: {
            _$class: msls.ContentItem,
            _$name: "TypeName",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        MethodName: {
            _$class: msls.ContentItem,
            _$name: "MethodName",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.Statistic,
            value: lightSwitchApplication.Statistic
        },
        Arguments: {
            _$class: msls.ContentItem,
            _$name: "Arguments",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        Enabled: {
            _$class: msls.ContentItem,
            _$name: "Enabled",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.Statistic,
            value: Boolean
        },
        DataType: {
            _$class: msls.ContentItem,
            _$name: "DataType",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        DisplayFormat: {
            _$class: msls.ContentItem,
            _$name: "DisplayFormat",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        IsConnectedState: {
            _$class: msls.ContentItem,
            _$name: "IsConnectedState",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.Statistic,
            value: Boolean
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditStatistic,
            data: lightSwitchApplication.Statistic,
            value: Number
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditStatistic
        }
    };

    msls._addEntryPoints(lightSwitchApplication.AddEditStatistic, {
        /// <field>
        /// Called when a new AddEditStatistic screen is created.
        /// <br/>created(msls.application.AddEditStatistic screen)
        /// </field>
        created: [lightSwitchApplication.AddEditStatistic],
        /// <field>
        /// Called before changes on an active AddEditStatistic screen are applied.
        /// <br/>beforeApplyChanges(msls.application.AddEditStatistic screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.AddEditStatistic],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("left"); }],
        /// <field>
        /// Called after the Source content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Source_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("Source"); }],
        /// <field>
        /// Called after the SignalIndex content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        SignalIndex_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("SignalIndex"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("Name"); }],
        /// <field>
        /// Called after the Description content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Description_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("Description"); }],
        /// <field>
        /// Called after the AssemblyName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AssemblyName_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("AssemblyName"); }],
        /// <field>
        /// Called after the TypeName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        TypeName_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("TypeName"); }],
        /// <field>
        /// Called after the MethodName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        MethodName_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("MethodName"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("right"); }],
        /// <field>
        /// Called after the Arguments content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Arguments_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("Arguments"); }],
        /// <field>
        /// Called after the Enabled content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Enabled_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("Enabled"); }],
        /// <field>
        /// Called after the DataType content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        DataType_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("DataType"); }],
        /// <field>
        /// Called after the DisplayFormat content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        DisplayFormat_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("DisplayFormat"); }],
        /// <field>
        /// Called after the IsConnectedState content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        IsConnectedState_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("IsConnectedState"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.AddEditStatistic().findContentItem("LoadOrder"); }]
    });

    lightSwitchApplication.BrowseStatistics.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseStatistics
        },
        StatisticList: {
            _$class: msls.ContentItem,
            _$name: "StatisticList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseStatistics,
            data: lightSwitchApplication.BrowseStatistics,
            value: lightSwitchApplication.BrowseStatistics
        },
        Statistics: {
            _$class: msls.ContentItem,
            _$name: "Statistics",
            _$parentName: "StatisticList",
            screen: lightSwitchApplication.BrowseStatistics,
            data: lightSwitchApplication.BrowseStatistics,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseStatistics,
                _$entry: {
                    elementType: lightSwitchApplication.Statistic
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "Statistics",
            screen: lightSwitchApplication.BrowseStatistics,
            data: lightSwitchApplication.Statistic,
            value: lightSwitchApplication.Statistic
        },
        Source: {
            _$class: msls.ContentItem,
            _$name: "Source",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseStatistics,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        SignalIndex: {
            _$class: msls.ContentItem,
            _$name: "SignalIndex",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseStatistics,
            data: lightSwitchApplication.Statistic,
            value: Number
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseStatistics,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseStatistics
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseStatistics, {
        /// <field>
        /// Called when a new BrowseStatistics screen is created.
        /// <br/>created(msls.application.BrowseStatistics screen)
        /// </field>
        created: [lightSwitchApplication.BrowseStatistics],
        /// <field>
        /// Called before changes on an active BrowseStatistics screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseStatistics screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseStatistics],
        /// <field>
        /// Called after the StatisticList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        StatisticList_postRender: [$element, function () { return new lightSwitchApplication.BrowseStatistics().findContentItem("StatisticList"); }],
        /// <field>
        /// Called after the Statistics content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Statistics_postRender: [$element, function () { return new lightSwitchApplication.BrowseStatistics().findContentItem("Statistics"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseStatistics().findContentItem("rows"); }],
        /// <field>
        /// Called after the Source content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Source_postRender: [$element, function () { return new lightSwitchApplication.BrowseStatistics().findContentItem("Source"); }],
        /// <field>
        /// Called after the SignalIndex content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        SignalIndex_postRender: [$element, function () { return new lightSwitchApplication.BrowseStatistics().findContentItem("SignalIndex"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.BrowseStatistics().findContentItem("Name"); }]
    });

    lightSwitchApplication.ViewStatistic.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewStatistic
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.ViewStatistic,
            value: lightSwitchApplication.ViewStatistic
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.ViewStatistic,
            value: lightSwitchApplication.Statistic
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.Statistic,
            value: lightSwitchApplication.Statistic
        },
        Source: {
            _$class: msls.ContentItem,
            _$name: "Source",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        SignalIndex: {
            _$class: msls.ContentItem,
            _$name: "SignalIndex",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.Statistic,
            value: Number
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        Description: {
            _$class: msls.ContentItem,
            _$name: "Description",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        AssemblyName: {
            _$class: msls.ContentItem,
            _$name: "AssemblyName",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        TypeName: {
            _$class: msls.ContentItem,
            _$name: "TypeName",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        MethodName: {
            _$class: msls.ContentItem,
            _$name: "MethodName",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.Statistic,
            value: lightSwitchApplication.Statistic
        },
        Arguments: {
            _$class: msls.ContentItem,
            _$name: "Arguments",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        Enabled: {
            _$class: msls.ContentItem,
            _$name: "Enabled",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.Statistic,
            value: Boolean
        },
        DataType: {
            _$class: msls.ContentItem,
            _$name: "DataType",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        DisplayFormat: {
            _$class: msls.ContentItem,
            _$name: "DisplayFormat",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.Statistic,
            value: String
        },
        IsConnectedState: {
            _$class: msls.ContentItem,
            _$name: "IsConnectedState",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.Statistic,
            value: Boolean
        },
        LoadOrder: {
            _$class: msls.ContentItem,
            _$name: "LoadOrder",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewStatistic,
            data: lightSwitchApplication.Statistic,
            value: Number
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewStatistic
        }
    };

    msls._addEntryPoints(lightSwitchApplication.ViewStatistic, {
        /// <field>
        /// Called when a new ViewStatistic screen is created.
        /// <br/>created(msls.application.ViewStatistic screen)
        /// </field>
        created: [lightSwitchApplication.ViewStatistic],
        /// <field>
        /// Called before changes on an active ViewStatistic screen are applied.
        /// <br/>beforeApplyChanges(msls.application.ViewStatistic screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.ViewStatistic],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("left"); }],
        /// <field>
        /// Called after the Source content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Source_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("Source"); }],
        /// <field>
        /// Called after the SignalIndex content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        SignalIndex_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("SignalIndex"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("Name"); }],
        /// <field>
        /// Called after the Description content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Description_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("Description"); }],
        /// <field>
        /// Called after the AssemblyName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AssemblyName_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("AssemblyName"); }],
        /// <field>
        /// Called after the TypeName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        TypeName_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("TypeName"); }],
        /// <field>
        /// Called after the MethodName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        MethodName_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("MethodName"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("right"); }],
        /// <field>
        /// Called after the Arguments content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Arguments_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("Arguments"); }],
        /// <field>
        /// Called after the Enabled content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Enabled_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("Enabled"); }],
        /// <field>
        /// Called after the DataType content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        DataType_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("DataType"); }],
        /// <field>
        /// Called after the DisplayFormat content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        DisplayFormat_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("DisplayFormat"); }],
        /// <field>
        /// Called after the IsConnectedState content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        IsConnectedState_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("IsConnectedState"); }],
        /// <field>
        /// Called after the LoadOrder content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        LoadOrder_postRender: [$element, function () { return new lightSwitchApplication.ViewStatistic().findContentItem("LoadOrder"); }]
    });

    lightSwitchApplication.AddEditVendorDevice.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditVendorDevice
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.AddEditVendorDevice,
            data: lightSwitchApplication.AddEditVendorDevice,
            value: lightSwitchApplication.AddEditVendorDevice
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.AddEditVendorDevice,
            data: lightSwitchApplication.AddEditVendorDevice,
            value: lightSwitchApplication.VendorDevice
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: lightSwitchApplication.VendorDevice
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: String
        },
        Description: {
            _$class: msls.ContentItem,
            _$name: "Description",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: String
        },
        URL: {
            _$class: msls.ContentItem,
            _$name: "URL",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: String
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: Date
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: lightSwitchApplication.VendorDevice
        },
        CreatedBy: {
            _$class: msls.ContentItem,
            _$name: "CreatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: String
        },
        UpdatedOn: {
            _$class: msls.ContentItem,
            _$name: "UpdatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: Date
        },
        UpdatedBy: {
            _$class: msls.ContentItem,
            _$name: "UpdatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: String
        },
        Vendor: {
            _$class: msls.ContentItem,
            _$name: "Vendor",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: lightSwitchApplication.Vendor
        },
        RowTemplate: {
            _$class: msls.ContentItem,
            _$name: "RowTemplate",
            _$parentName: "Vendor",
            screen: lightSwitchApplication.AddEditVendorDevice,
            data: lightSwitchApplication.Vendor,
            value: lightSwitchApplication.Vendor
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditVendorDevice
        }
    };

    msls._addEntryPoints(lightSwitchApplication.AddEditVendorDevice, {
        /// <field>
        /// Called when a new AddEditVendorDevice screen is created.
        /// <br/>created(msls.application.AddEditVendorDevice screen)
        /// </field>
        created: [lightSwitchApplication.AddEditVendorDevice],
        /// <field>
        /// Called before changes on an active AddEditVendorDevice screen are applied.
        /// <br/>beforeApplyChanges(msls.application.AddEditVendorDevice screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.AddEditVendorDevice],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendorDevice().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendorDevice().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendorDevice().findContentItem("left"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendorDevice().findContentItem("Name"); }],
        /// <field>
        /// Called after the Description content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Description_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendorDevice().findContentItem("Description"); }],
        /// <field>
        /// Called after the URL content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        URL_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendorDevice().findContentItem("URL"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendorDevice().findContentItem("CreatedOn"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendorDevice().findContentItem("right"); }],
        /// <field>
        /// Called after the CreatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedBy_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendorDevice().findContentItem("CreatedBy"); }],
        /// <field>
        /// Called after the UpdatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedOn_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendorDevice().findContentItem("UpdatedOn"); }],
        /// <field>
        /// Called after the UpdatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedBy_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendorDevice().findContentItem("UpdatedBy"); }],
        /// <field>
        /// Called after the Vendor content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Vendor_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendorDevice().findContentItem("Vendor"); }],
        /// <field>
        /// Called after the RowTemplate content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        RowTemplate_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendorDevice().findContentItem("RowTemplate"); }]
    });

    lightSwitchApplication.BrowseVendorDevices.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseVendorDevices
        },
        VendorDeviceList: {
            _$class: msls.ContentItem,
            _$name: "VendorDeviceList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseVendorDevices,
            data: lightSwitchApplication.BrowseVendorDevices,
            value: lightSwitchApplication.BrowseVendorDevices
        },
        VendorDevices: {
            _$class: msls.ContentItem,
            _$name: "VendorDevices",
            _$parentName: "VendorDeviceList",
            screen: lightSwitchApplication.BrowseVendorDevices,
            data: lightSwitchApplication.BrowseVendorDevices,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseVendorDevices,
                _$entry: {
                    elementType: lightSwitchApplication.VendorDevice
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "VendorDevices",
            screen: lightSwitchApplication.BrowseVendorDevices,
            data: lightSwitchApplication.VendorDevice,
            value: lightSwitchApplication.VendorDevice
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseVendorDevices,
            data: lightSwitchApplication.VendorDevice,
            value: String
        },
        Description: {
            _$class: msls.ContentItem,
            _$name: "Description",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseVendorDevices,
            data: lightSwitchApplication.VendorDevice,
            value: String
        },
        URL: {
            _$class: msls.ContentItem,
            _$name: "URL",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseVendorDevices,
            data: lightSwitchApplication.VendorDevice,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseVendorDevices
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseVendorDevices, {
        /// <field>
        /// Called when a new BrowseVendorDevices screen is created.
        /// <br/>created(msls.application.BrowseVendorDevices screen)
        /// </field>
        created: [lightSwitchApplication.BrowseVendorDevices],
        /// <field>
        /// Called before changes on an active BrowseVendorDevices screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseVendorDevices screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseVendorDevices],
        /// <field>
        /// Called after the VendorDeviceList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        VendorDeviceList_postRender: [$element, function () { return new lightSwitchApplication.BrowseVendorDevices().findContentItem("VendorDeviceList"); }],
        /// <field>
        /// Called after the VendorDevices content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        VendorDevices_postRender: [$element, function () { return new lightSwitchApplication.BrowseVendorDevices().findContentItem("VendorDevices"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseVendorDevices().findContentItem("rows"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.BrowseVendorDevices().findContentItem("Name"); }],
        /// <field>
        /// Called after the Description content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Description_postRender: [$element, function () { return new lightSwitchApplication.BrowseVendorDevices().findContentItem("Description"); }],
        /// <field>
        /// Called after the URL content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        URL_postRender: [$element, function () { return new lightSwitchApplication.BrowseVendorDevices().findContentItem("URL"); }]
    });

    lightSwitchApplication.ViewVendorDevice.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewVendorDevice
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.ViewVendorDevice,
            data: lightSwitchApplication.ViewVendorDevice,
            value: lightSwitchApplication.ViewVendorDevice
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.ViewVendorDevice,
            data: lightSwitchApplication.ViewVendorDevice,
            value: lightSwitchApplication.VendorDevice
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: lightSwitchApplication.VendorDevice
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: String
        },
        Description: {
            _$class: msls.ContentItem,
            _$name: "Description",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: String
        },
        URL: {
            _$class: msls.ContentItem,
            _$name: "URL",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: String
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: Date
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: lightSwitchApplication.VendorDevice
        },
        CreatedBy: {
            _$class: msls.ContentItem,
            _$name: "CreatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: String
        },
        UpdatedOn: {
            _$class: msls.ContentItem,
            _$name: "UpdatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: Date
        },
        UpdatedBy: {
            _$class: msls.ContentItem,
            _$name: "UpdatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: String
        },
        Vendor: {
            _$class: msls.ContentItem,
            _$name: "Vendor",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewVendorDevice,
            data: lightSwitchApplication.VendorDevice,
            value: lightSwitchApplication.Vendor
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewVendorDevice
        }
    };

    msls._addEntryPoints(lightSwitchApplication.ViewVendorDevice, {
        /// <field>
        /// Called when a new ViewVendorDevice screen is created.
        /// <br/>created(msls.application.ViewVendorDevice screen)
        /// </field>
        created: [lightSwitchApplication.ViewVendorDevice],
        /// <field>
        /// Called before changes on an active ViewVendorDevice screen are applied.
        /// <br/>beforeApplyChanges(msls.application.ViewVendorDevice screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.ViewVendorDevice],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.ViewVendorDevice().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.ViewVendorDevice().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.ViewVendorDevice().findContentItem("left"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.ViewVendorDevice().findContentItem("Name"); }],
        /// <field>
        /// Called after the Description content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Description_postRender: [$element, function () { return new lightSwitchApplication.ViewVendorDevice().findContentItem("Description"); }],
        /// <field>
        /// Called after the URL content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        URL_postRender: [$element, function () { return new lightSwitchApplication.ViewVendorDevice().findContentItem("URL"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.ViewVendorDevice().findContentItem("CreatedOn"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.ViewVendorDevice().findContentItem("right"); }],
        /// <field>
        /// Called after the CreatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedBy_postRender: [$element, function () { return new lightSwitchApplication.ViewVendorDevice().findContentItem("CreatedBy"); }],
        /// <field>
        /// Called after the UpdatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedOn_postRender: [$element, function () { return new lightSwitchApplication.ViewVendorDevice().findContentItem("UpdatedOn"); }],
        /// <field>
        /// Called after the UpdatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedBy_postRender: [$element, function () { return new lightSwitchApplication.ViewVendorDevice().findContentItem("UpdatedBy"); }],
        /// <field>
        /// Called after the Vendor content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Vendor_postRender: [$element, function () { return new lightSwitchApplication.ViewVendorDevice().findContentItem("Vendor"); }]
    });

    lightSwitchApplication.AddEditVendor.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditVendor
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.AddEditVendor,
            data: lightSwitchApplication.AddEditVendor,
            value: lightSwitchApplication.AddEditVendor
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.AddEditVendor,
            data: lightSwitchApplication.AddEditVendor,
            value: lightSwitchApplication.Vendor
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditVendor,
            data: lightSwitchApplication.Vendor,
            value: lightSwitchApplication.Vendor
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditVendor,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditVendor,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        PhoneNumber: {
            _$class: msls.ContentItem,
            _$name: "PhoneNumber",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditVendor,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        ContactEmail: {
            _$class: msls.ContentItem,
            _$name: "ContactEmail",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditVendor,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        URL: {
            _$class: msls.ContentItem,
            _$name: "URL",
            _$parentName: "left",
            screen: lightSwitchApplication.AddEditVendor,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.AddEditVendor,
            data: lightSwitchApplication.Vendor,
            value: lightSwitchApplication.Vendor
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditVendor,
            data: lightSwitchApplication.Vendor,
            value: Date
        },
        CreatedBy: {
            _$class: msls.ContentItem,
            _$name: "CreatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditVendor,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        UpdatedOn: {
            _$class: msls.ContentItem,
            _$name: "UpdatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditVendor,
            data: lightSwitchApplication.Vendor,
            value: Date
        },
        UpdatedBy: {
            _$class: msls.ContentItem,
            _$name: "UpdatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.AddEditVendor,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.AddEditVendor
        }
    };

    msls._addEntryPoints(lightSwitchApplication.AddEditVendor, {
        /// <field>
        /// Called when a new AddEditVendor screen is created.
        /// <br/>created(msls.application.AddEditVendor screen)
        /// </field>
        created: [lightSwitchApplication.AddEditVendor],
        /// <field>
        /// Called before changes on an active AddEditVendor screen are applied.
        /// <br/>beforeApplyChanges(msls.application.AddEditVendor screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.AddEditVendor],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendor().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendor().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendor().findContentItem("left"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendor().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendor().findContentItem("Name"); }],
        /// <field>
        /// Called after the PhoneNumber content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        PhoneNumber_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendor().findContentItem("PhoneNumber"); }],
        /// <field>
        /// Called after the ContactEmail content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ContactEmail_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendor().findContentItem("ContactEmail"); }],
        /// <field>
        /// Called after the URL content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        URL_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendor().findContentItem("URL"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendor().findContentItem("right"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendor().findContentItem("CreatedOn"); }],
        /// <field>
        /// Called after the CreatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedBy_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendor().findContentItem("CreatedBy"); }],
        /// <field>
        /// Called after the UpdatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedOn_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendor().findContentItem("UpdatedOn"); }],
        /// <field>
        /// Called after the UpdatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedBy_postRender: [$element, function () { return new lightSwitchApplication.AddEditVendor().findContentItem("UpdatedBy"); }]
    });

    lightSwitchApplication.BrowseVendors.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseVendors
        },
        VendorList: {
            _$class: msls.ContentItem,
            _$name: "VendorList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseVendors,
            data: lightSwitchApplication.BrowseVendors,
            value: lightSwitchApplication.BrowseVendors
        },
        Vendors: {
            _$class: msls.ContentItem,
            _$name: "Vendors",
            _$parentName: "VendorList",
            screen: lightSwitchApplication.BrowseVendors,
            data: lightSwitchApplication.BrowseVendors,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseVendors,
                _$entry: {
                    elementType: lightSwitchApplication.Vendor
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "Vendors",
            screen: lightSwitchApplication.BrowseVendors,
            data: lightSwitchApplication.Vendor,
            value: lightSwitchApplication.Vendor
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseVendors,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseVendors,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        PhoneNumber: {
            _$class: msls.ContentItem,
            _$name: "PhoneNumber",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseVendors,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseVendors
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseVendors, {
        /// <field>
        /// Called when a new BrowseVendors screen is created.
        /// <br/>created(msls.application.BrowseVendors screen)
        /// </field>
        created: [lightSwitchApplication.BrowseVendors],
        /// <field>
        /// Called before changes on an active BrowseVendors screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseVendors screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseVendors],
        /// <field>
        /// Called after the VendorList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        VendorList_postRender: [$element, function () { return new lightSwitchApplication.BrowseVendors().findContentItem("VendorList"); }],
        /// <field>
        /// Called after the Vendors content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Vendors_postRender: [$element, function () { return new lightSwitchApplication.BrowseVendors().findContentItem("Vendors"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseVendors().findContentItem("rows"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.BrowseVendors().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.BrowseVendors().findContentItem("Name"); }],
        /// <field>
        /// Called after the PhoneNumber content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        PhoneNumber_postRender: [$element, function () { return new lightSwitchApplication.BrowseVendors().findContentItem("PhoneNumber"); }]
    });

    lightSwitchApplication.ViewVendor.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewVendor
        },
        Details: {
            _$class: msls.ContentItem,
            _$name: "Details",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.ViewVendor,
            value: lightSwitchApplication.ViewVendor
        },
        columns: {
            _$class: msls.ContentItem,
            _$name: "columns",
            _$parentName: "Details",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.ViewVendor,
            value: lightSwitchApplication.Vendor
        },
        left: {
            _$class: msls.ContentItem,
            _$name: "left",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.Vendor,
            value: lightSwitchApplication.Vendor
        },
        Acronym: {
            _$class: msls.ContentItem,
            _$name: "Acronym",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        Name: {
            _$class: msls.ContentItem,
            _$name: "Name",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        PhoneNumber: {
            _$class: msls.ContentItem,
            _$name: "PhoneNumber",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        ContactEmail: {
            _$class: msls.ContentItem,
            _$name: "ContactEmail",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        URL: {
            _$class: msls.ContentItem,
            _$name: "URL",
            _$parentName: "left",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        right: {
            _$class: msls.ContentItem,
            _$name: "right",
            _$parentName: "columns",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.Vendor,
            value: lightSwitchApplication.Vendor
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.Vendor,
            value: Date
        },
        CreatedBy: {
            _$class: msls.ContentItem,
            _$name: "CreatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        UpdatedOn: {
            _$class: msls.ContentItem,
            _$name: "UpdatedOn",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.Vendor,
            value: Date
        },
        UpdatedBy: {
            _$class: msls.ContentItem,
            _$name: "UpdatedBy",
            _$parentName: "right",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.Vendor,
            value: String
        },
        VendorDevices: {
            _$class: msls.ContentItem,
            _$name: "VendorDevices",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.ViewVendor,
            value: lightSwitchApplication.ViewVendor
        },
        VendorDevices1: {
            _$class: msls.ContentItem,
            _$name: "VendorDevices1",
            _$parentName: "VendorDevices",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.ViewVendor,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.ViewVendor,
                _$entry: {
                    elementType: lightSwitchApplication.VendorDevice
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "VendorDevices1",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.VendorDevice,
            value: lightSwitchApplication.VendorDevice
        },
        Name1: {
            _$class: msls.ContentItem,
            _$name: "Name1",
            _$parentName: "rows",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.VendorDevice,
            value: String
        },
        Description: {
            _$class: msls.ContentItem,
            _$name: "Description",
            _$parentName: "rows",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.VendorDevice,
            value: String
        },
        URL1: {
            _$class: msls.ContentItem,
            _$name: "URL1",
            _$parentName: "rows",
            screen: lightSwitchApplication.ViewVendor,
            data: lightSwitchApplication.VendorDevice,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.ViewVendor
        }
    };

    msls._addEntryPoints(lightSwitchApplication.ViewVendor, {
        /// <field>
        /// Called when a new ViewVendor screen is created.
        /// <br/>created(msls.application.ViewVendor screen)
        /// </field>
        created: [lightSwitchApplication.ViewVendor],
        /// <field>
        /// Called before changes on an active ViewVendor screen are applied.
        /// <br/>beforeApplyChanges(msls.application.ViewVendor screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.ViewVendor],
        /// <field>
        /// Called after the Details content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Details_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("Details"); }],
        /// <field>
        /// Called after the columns content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        columns_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("columns"); }],
        /// <field>
        /// Called after the left content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        left_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("left"); }],
        /// <field>
        /// Called after the Acronym content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Acronym_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("Acronym"); }],
        /// <field>
        /// Called after the Name content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("Name"); }],
        /// <field>
        /// Called after the PhoneNumber content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        PhoneNumber_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("PhoneNumber"); }],
        /// <field>
        /// Called after the ContactEmail content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ContactEmail_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("ContactEmail"); }],
        /// <field>
        /// Called after the URL content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        URL_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("URL"); }],
        /// <field>
        /// Called after the right content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        right_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("right"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("CreatedOn"); }],
        /// <field>
        /// Called after the CreatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedBy_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("CreatedBy"); }],
        /// <field>
        /// Called after the UpdatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedOn_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("UpdatedOn"); }],
        /// <field>
        /// Called after the UpdatedBy content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UpdatedBy_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("UpdatedBy"); }],
        /// <field>
        /// Called after the VendorDevices content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        VendorDevices_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("VendorDevices"); }],
        /// <field>
        /// Called after the VendorDevices1 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        VendorDevices1_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("VendorDevices1"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("rows"); }],
        /// <field>
        /// Called after the Name1 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Name1_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("Name1"); }],
        /// <field>
        /// Called after the Description content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Description_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("Description"); }],
        /// <field>
        /// Called after the URL1 content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        URL1_postRender: [$element, function () { return new lightSwitchApplication.ViewVendor().findContentItem("URL1"); }]
    });

    lightSwitchApplication.BrowseAccessLogs.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseAccessLogs
        },
        AccessLogList: {
            _$class: msls.ContentItem,
            _$name: "AccessLogList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseAccessLogs,
            data: lightSwitchApplication.BrowseAccessLogs,
            value: lightSwitchApplication.BrowseAccessLogs
        },
        AccessLogs: {
            _$class: msls.ContentItem,
            _$name: "AccessLogs",
            _$parentName: "AccessLogList",
            screen: lightSwitchApplication.BrowseAccessLogs,
            data: lightSwitchApplication.BrowseAccessLogs,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseAccessLogs,
                _$entry: {
                    elementType: lightSwitchApplication.AccessLog
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "AccessLogs",
            screen: lightSwitchApplication.BrowseAccessLogs,
            data: lightSwitchApplication.AccessLog,
            value: lightSwitchApplication.AccessLog
        },
        UserName: {
            _$class: msls.ContentItem,
            _$name: "UserName",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseAccessLogs,
            data: lightSwitchApplication.AccessLog,
            value: String
        },
        AccessGranted: {
            _$class: msls.ContentItem,
            _$name: "AccessGranted",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseAccessLogs,
            data: lightSwitchApplication.AccessLog,
            value: Boolean
        },
        CreatedOn: {
            _$class: msls.ContentItem,
            _$name: "CreatedOn",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseAccessLogs,
            data: lightSwitchApplication.AccessLog,
            value: Date
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseAccessLogs
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseAccessLogs, {
        /// <field>
        /// Called when a new BrowseAccessLogs screen is created.
        /// <br/>created(msls.application.BrowseAccessLogs screen)
        /// </field>
        created: [lightSwitchApplication.BrowseAccessLogs],
        /// <field>
        /// Called before changes on an active BrowseAccessLogs screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseAccessLogs screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseAccessLogs],
        /// <field>
        /// Called after the AccessLogList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AccessLogList_postRender: [$element, function () { return new lightSwitchApplication.BrowseAccessLogs().findContentItem("AccessLogList"); }],
        /// <field>
        /// Called after the AccessLogs content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AccessLogs_postRender: [$element, function () { return new lightSwitchApplication.BrowseAccessLogs().findContentItem("AccessLogs"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseAccessLogs().findContentItem("rows"); }],
        /// <field>
        /// Called after the UserName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        UserName_postRender: [$element, function () { return new lightSwitchApplication.BrowseAccessLogs().findContentItem("UserName"); }],
        /// <field>
        /// Called after the AccessGranted content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AccessGranted_postRender: [$element, function () { return new lightSwitchApplication.BrowseAccessLogs().findContentItem("AccessGranted"); }],
        /// <field>
        /// Called after the CreatedOn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        CreatedOn_postRender: [$element, function () { return new lightSwitchApplication.BrowseAccessLogs().findContentItem("CreatedOn"); }]
    });

    lightSwitchApplication.BrowseAlarmLogs.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseAlarmLogs
        },
        AlarmLogList: {
            _$class: msls.ContentItem,
            _$name: "AlarmLogList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseAlarmLogs,
            data: lightSwitchApplication.BrowseAlarmLogs,
            value: lightSwitchApplication.BrowseAlarmLogs
        },
        AlarmLogs: {
            _$class: msls.ContentItem,
            _$name: "AlarmLogs",
            _$parentName: "AlarmLogList",
            screen: lightSwitchApplication.BrowseAlarmLogs,
            data: lightSwitchApplication.BrowseAlarmLogs,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseAlarmLogs,
                _$entry: {
                    elementType: lightSwitchApplication.AlarmLog
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "AlarmLogs",
            screen: lightSwitchApplication.BrowseAlarmLogs,
            data: lightSwitchApplication.AlarmLog,
            value: lightSwitchApplication.AlarmLog
        },
        ID: {
            _$class: msls.ContentItem,
            _$name: "ID",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseAlarmLogs,
            data: lightSwitchApplication.AlarmLog,
            value: Number
        },
        Ticks: {
            _$class: msls.ContentItem,
            _$name: "Ticks",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseAlarmLogs,
            data: lightSwitchApplication.AlarmLog,
            value: String
        },
        Timestamp: {
            _$class: msls.ContentItem,
            _$name: "Timestamp",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseAlarmLogs,
            data: lightSwitchApplication.AlarmLog,
            value: Date
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseAlarmLogs
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseAlarmLogs, {
        /// <field>
        /// Called when a new BrowseAlarmLogs screen is created.
        /// <br/>created(msls.application.BrowseAlarmLogs screen)
        /// </field>
        created: [lightSwitchApplication.BrowseAlarmLogs],
        /// <field>
        /// Called before changes on an active BrowseAlarmLogs screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseAlarmLogs screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseAlarmLogs],
        /// <field>
        /// Called after the AlarmLogList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AlarmLogList_postRender: [$element, function () { return new lightSwitchApplication.BrowseAlarmLogs().findContentItem("AlarmLogList"); }],
        /// <field>
        /// Called after the AlarmLogs content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AlarmLogs_postRender: [$element, function () { return new lightSwitchApplication.BrowseAlarmLogs().findContentItem("AlarmLogs"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseAlarmLogs().findContentItem("rows"); }],
        /// <field>
        /// Called after the ID content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ID_postRender: [$element, function () { return new lightSwitchApplication.BrowseAlarmLogs().findContentItem("ID"); }],
        /// <field>
        /// Called after the Ticks content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Ticks_postRender: [$element, function () { return new lightSwitchApplication.BrowseAlarmLogs().findContentItem("Ticks"); }],
        /// <field>
        /// Called after the Timestamp content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Timestamp_postRender: [$element, function () { return new lightSwitchApplication.BrowseAlarmLogs().findContentItem("Timestamp"); }]
    });

    lightSwitchApplication.BrowseAuditLogs.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseAuditLogs
        },
        AuditLogList: {
            _$class: msls.ContentItem,
            _$name: "AuditLogList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseAuditLogs,
            data: lightSwitchApplication.BrowseAuditLogs,
            value: lightSwitchApplication.BrowseAuditLogs
        },
        AuditLogs: {
            _$class: msls.ContentItem,
            _$name: "AuditLogs",
            _$parentName: "AuditLogList",
            screen: lightSwitchApplication.BrowseAuditLogs,
            data: lightSwitchApplication.BrowseAuditLogs,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseAuditLogs,
                _$entry: {
                    elementType: lightSwitchApplication.AuditLog
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "AuditLogs",
            screen: lightSwitchApplication.BrowseAuditLogs,
            data: lightSwitchApplication.AuditLog,
            value: lightSwitchApplication.AuditLog
        },
        TableName: {
            _$class: msls.ContentItem,
            _$name: "TableName",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseAuditLogs,
            data: lightSwitchApplication.AuditLog,
            value: String
        },
        PrimaryKeyColumn: {
            _$class: msls.ContentItem,
            _$name: "PrimaryKeyColumn",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseAuditLogs,
            data: lightSwitchApplication.AuditLog,
            value: String
        },
        PrimaryKeyValue: {
            _$class: msls.ContentItem,
            _$name: "PrimaryKeyValue",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseAuditLogs,
            data: lightSwitchApplication.AuditLog,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseAuditLogs
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseAuditLogs, {
        /// <field>
        /// Called when a new BrowseAuditLogs screen is created.
        /// <br/>created(msls.application.BrowseAuditLogs screen)
        /// </field>
        created: [lightSwitchApplication.BrowseAuditLogs],
        /// <field>
        /// Called before changes on an active BrowseAuditLogs screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseAuditLogs screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseAuditLogs],
        /// <field>
        /// Called after the AuditLogList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AuditLogList_postRender: [$element, function () { return new lightSwitchApplication.BrowseAuditLogs().findContentItem("AuditLogList"); }],
        /// <field>
        /// Called after the AuditLogs content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        AuditLogs_postRender: [$element, function () { return new lightSwitchApplication.BrowseAuditLogs().findContentItem("AuditLogs"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseAuditLogs().findContentItem("rows"); }],
        /// <field>
        /// Called after the TableName content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        TableName_postRender: [$element, function () { return new lightSwitchApplication.BrowseAuditLogs().findContentItem("TableName"); }],
        /// <field>
        /// Called after the PrimaryKeyColumn content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        PrimaryKeyColumn_postRender: [$element, function () { return new lightSwitchApplication.BrowseAuditLogs().findContentItem("PrimaryKeyColumn"); }],
        /// <field>
        /// Called after the PrimaryKeyValue content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        PrimaryKeyValue_postRender: [$element, function () { return new lightSwitchApplication.BrowseAuditLogs().findContentItem("PrimaryKeyValue"); }]
    });

    lightSwitchApplication.BrowseErrorLogs.prototype._$contentItems = {
        Tabs: {
            _$class: msls.ContentItem,
            _$name: "Tabs",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseErrorLogs
        },
        ErrorLogList: {
            _$class: msls.ContentItem,
            _$name: "ErrorLogList",
            _$parentName: "Tabs",
            screen: lightSwitchApplication.BrowseErrorLogs,
            data: lightSwitchApplication.BrowseErrorLogs,
            value: lightSwitchApplication.BrowseErrorLogs
        },
        ErrorLogs: {
            _$class: msls.ContentItem,
            _$name: "ErrorLogs",
            _$parentName: "ErrorLogList",
            screen: lightSwitchApplication.BrowseErrorLogs,
            data: lightSwitchApplication.BrowseErrorLogs,
            value: {
                _$class: msls.VisualCollection,
                screen: lightSwitchApplication.BrowseErrorLogs,
                _$entry: {
                    elementType: lightSwitchApplication.ErrorLog
                }
            }
        },
        rows: {
            _$class: msls.ContentItem,
            _$name: "rows",
            _$parentName: "ErrorLogs",
            screen: lightSwitchApplication.BrowseErrorLogs,
            data: lightSwitchApplication.ErrorLog,
            value: lightSwitchApplication.ErrorLog
        },
        Source: {
            _$class: msls.ContentItem,
            _$name: "Source",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseErrorLogs,
            data: lightSwitchApplication.ErrorLog,
            value: String
        },
        Type: {
            _$class: msls.ContentItem,
            _$name: "Type",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseErrorLogs,
            data: lightSwitchApplication.ErrorLog,
            value: String
        },
        Message: {
            _$class: msls.ContentItem,
            _$name: "Message",
            _$parentName: "rows",
            screen: lightSwitchApplication.BrowseErrorLogs,
            data: lightSwitchApplication.ErrorLog,
            value: String
        },
        Popups: {
            _$class: msls.ContentItem,
            _$name: "Popups",
            _$parentName: "RootContentItem",
            screen: lightSwitchApplication.BrowseErrorLogs
        }
    };

    msls._addEntryPoints(lightSwitchApplication.BrowseErrorLogs, {
        /// <field>
        /// Called when a new BrowseErrorLogs screen is created.
        /// <br/>created(msls.application.BrowseErrorLogs screen)
        /// </field>
        created: [lightSwitchApplication.BrowseErrorLogs],
        /// <field>
        /// Called before changes on an active BrowseErrorLogs screen are applied.
        /// <br/>beforeApplyChanges(msls.application.BrowseErrorLogs screen)
        /// </field>
        beforeApplyChanges: [lightSwitchApplication.BrowseErrorLogs],
        /// <field>
        /// Called after the ErrorLogList content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ErrorLogList_postRender: [$element, function () { return new lightSwitchApplication.BrowseErrorLogs().findContentItem("ErrorLogList"); }],
        /// <field>
        /// Called after the ErrorLogs content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        ErrorLogs_postRender: [$element, function () { return new lightSwitchApplication.BrowseErrorLogs().findContentItem("ErrorLogs"); }],
        /// <field>
        /// Called after the rows content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        rows_postRender: [$element, function () { return new lightSwitchApplication.BrowseErrorLogs().findContentItem("rows"); }],
        /// <field>
        /// Called after the Source content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Source_postRender: [$element, function () { return new lightSwitchApplication.BrowseErrorLogs().findContentItem("Source"); }],
        /// <field>
        /// Called after the Type content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Type_postRender: [$element, function () { return new lightSwitchApplication.BrowseErrorLogs().findContentItem("Type"); }],
        /// <field>
        /// Called after the Message content item has been rendered.
        /// <br/>postRender(HTMLElement element, msls.ContentItem contentItem)
        /// </field>
        Message_postRender: [$element, function () { return new lightSwitchApplication.BrowseErrorLogs().findContentItem("Message"); }]
    });

}(msls.application));