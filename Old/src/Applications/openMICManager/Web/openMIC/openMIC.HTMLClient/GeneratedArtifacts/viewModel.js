/// <reference path="data.js" />

(function (lightSwitchApplication) {

    var $Screen = msls.Screen,
        $defineScreen = msls._defineScreen,
        $DataServiceQuery = msls.DataServiceQuery,
        $toODataString = msls._toODataString,
        $defineShowScreen = msls._defineShowScreen;

    function AddEditAlarm(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the AddEditAlarm screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Alarm" type="msls.application.Alarm">
        /// Gets or sets the alarm for this screen.
        /// </field>
        /// <field name="details" type="msls.application.AddEditAlarm.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "AddEditAlarm", parameters);
    }

    function BrowseAlarms(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseAlarms screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Alarms" type="msls.VisualCollection" elementType="msls.application.Alarm">
        /// Gets the alarms for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseAlarms.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseAlarms", parameters);
    }

    function ViewAlarm(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the ViewAlarm screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Alarm" type="msls.application.Alarm">
        /// Gets or sets the alarm for this screen.
        /// </field>
        /// <field name="details" type="msls.application.ViewAlarm.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "ViewAlarm", parameters);
    }

    function AddEditCompany(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the AddEditCompany screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Company" type="msls.application.Company">
        /// Gets or sets the company for this screen.
        /// </field>
        /// <field name="details" type="msls.application.AddEditCompany.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "AddEditCompany", parameters);
    }

    function BrowseCompanies(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseCompanies screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Companies" type="msls.VisualCollection" elementType="msls.application.Company">
        /// Gets the companies for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseCompanies.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseCompanies", parameters);
    }

    function ViewCompany(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the ViewCompany screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Company" type="msls.application.Company">
        /// Gets or sets the company for this screen.
        /// </field>
        /// <field name="details" type="msls.application.ViewCompany.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "ViewCompany", parameters);
    }

    function AddEditDefaultValue(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the AddEditDefaultValue screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="DefaultValue" type="msls.application.DefaultValue">
        /// Gets or sets the defaultValue for this screen.
        /// </field>
        /// <field name="details" type="msls.application.AddEditDefaultValue.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "AddEditDefaultValue", parameters);
    }

    function BrowseDefaultValues(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseDefaultValues screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="DefaultValues" type="msls.VisualCollection" elementType="msls.application.DefaultValue">
        /// Gets the defaultValues for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseDefaultValues.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseDefaultValues", parameters);
    }

    function AddEditDevice(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the AddEditDevice screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Device" type="msls.application.Device">
        /// Gets or sets the device for this screen.
        /// </field>
        /// <field name="details" type="msls.application.AddEditDevice.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "AddEditDevice", parameters);
    }

    function BrowseDevices(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseDevices screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Devices" type="msls.VisualCollection" elementType="msls.application.Device">
        /// Gets the devices for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseDevices.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseDevices", parameters);
    }

    function ViewDevice(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the ViewDevice screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Device" type="msls.application.Device">
        /// Gets or sets the device for this screen.
        /// </field>
        /// <field name="details" type="msls.application.ViewDevice.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "ViewDevice", parameters);
    }

    function AddEditHistorian(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the AddEditHistorian screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Historian" type="msls.application.Historian">
        /// Gets or sets the historian for this screen.
        /// </field>
        /// <field name="details" type="msls.application.AddEditHistorian.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "AddEditHistorian", parameters);
    }

    function BrowseHistorians(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseHistorians screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Historians" type="msls.VisualCollection" elementType="msls.application.Historian">
        /// Gets the historians for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseHistorians.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseHistorians", parameters);
    }

    function ViewHistorian(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the ViewHistorian screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Historian" type="msls.application.Historian">
        /// Gets or sets the historian for this screen.
        /// </field>
        /// <field name="details" type="msls.application.ViewHistorian.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "ViewHistorian", parameters);
    }

    function AddEditInterconnection(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the AddEditInterconnection screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Interconnection" type="msls.application.Interconnection">
        /// Gets or sets the interconnection for this screen.
        /// </field>
        /// <field name="details" type="msls.application.AddEditInterconnection.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "AddEditInterconnection", parameters);
    }

    function BrowseInterconnections(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseInterconnections screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Interconnections" type="msls.VisualCollection" elementType="msls.application.Interconnection">
        /// Gets the interconnections for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseInterconnections.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseInterconnections", parameters);
    }

    function ViewInterconnection(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the ViewInterconnection screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Interconnection" type="msls.application.Interconnection">
        /// Gets or sets the interconnection for this screen.
        /// </field>
        /// <field name="details" type="msls.application.ViewInterconnection.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "ViewInterconnection", parameters);
    }

    function AddEditMeasurement(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the AddEditMeasurement screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Measurement" type="msls.application.Measurement">
        /// Gets or sets the measurement for this screen.
        /// </field>
        /// <field name="details" type="msls.application.AddEditMeasurement.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "AddEditMeasurement", parameters);
    }

    function BrowseMeasurements(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseMeasurements screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Measurements" type="msls.VisualCollection" elementType="msls.application.Measurement">
        /// Gets the measurements for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseMeasurements.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseMeasurements", parameters);
    }

    function ViewMeasurement(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the ViewMeasurement screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Measurement" type="msls.application.Measurement">
        /// Gets or sets the measurement for this screen.
        /// </field>
        /// <field name="details" type="msls.application.ViewMeasurement.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "ViewMeasurement", parameters);
    }

    function AddEditNodes(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the AddEditNodes screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Node" type="msls.application.Node">
        /// Gets or sets the node for this screen.
        /// </field>
        /// <field name="details" type="msls.application.AddEditNodes.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "AddEditNodes", parameters);
    }

    function BrowseNodesSet(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseNodesSet screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Nodes" type="msls.VisualCollection" elementType="msls.application.Node">
        /// Gets the nodes for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseNodesSet.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseNodesSet", parameters);
    }

    function ViewNodes(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the ViewNodes screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Node" type="msls.application.Node">
        /// Gets or sets the node for this screen.
        /// </field>
        /// <field name="details" type="msls.application.ViewNodes.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "ViewNodes", parameters);
    }

    function AddEditProtocol(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the AddEditProtocol screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Protocol" type="msls.application.Protocol">
        /// Gets or sets the protocol for this screen.
        /// </field>
        /// <field name="details" type="msls.application.AddEditProtocol.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "AddEditProtocol", parameters);
    }

    function BrowseProtocols(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseProtocols screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Protocols" type="msls.VisualCollection" elementType="msls.application.Protocol">
        /// Gets the protocols for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseProtocols.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseProtocols", parameters);
    }

    function ViewProtocol(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the ViewProtocol screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Protocol" type="msls.application.Protocol">
        /// Gets or sets the protocol for this screen.
        /// </field>
        /// <field name="details" type="msls.application.ViewProtocol.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "ViewProtocol", parameters);
    }

    function AddEditSignalType(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the AddEditSignalType screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="SignalType" type="msls.application.SignalType">
        /// Gets or sets the signalType for this screen.
        /// </field>
        /// <field name="details" type="msls.application.AddEditSignalType.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "AddEditSignalType", parameters);
    }

    function BrowseSignalTypes(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseSignalTypes screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="SignalTypes" type="msls.VisualCollection" elementType="msls.application.SignalType">
        /// Gets the signalTypes for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseSignalTypes.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseSignalTypes", parameters);
    }

    function ViewSignalType(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the ViewSignalType screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="SignalType" type="msls.application.SignalType">
        /// Gets or sets the signalType for this screen.
        /// </field>
        /// <field name="details" type="msls.application.ViewSignalType.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "ViewSignalType", parameters);
    }

    function AddEditStatistic(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the AddEditStatistic screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Statistic" type="msls.application.Statistic">
        /// Gets or sets the statistic for this screen.
        /// </field>
        /// <field name="details" type="msls.application.AddEditStatistic.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "AddEditStatistic", parameters);
    }

    function BrowseStatistics(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseStatistics screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Statistics" type="msls.VisualCollection" elementType="msls.application.Statistic">
        /// Gets the statistics for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseStatistics.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseStatistics", parameters);
    }

    function ViewStatistic(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the ViewStatistic screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Statistic" type="msls.application.Statistic">
        /// Gets or sets the statistic for this screen.
        /// </field>
        /// <field name="details" type="msls.application.ViewStatistic.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "ViewStatistic", parameters);
    }

    function AddEditVendorDevice(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the AddEditVendorDevice screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="VendorDevice" type="msls.application.VendorDevice">
        /// Gets or sets the vendorDevice for this screen.
        /// </field>
        /// <field name="details" type="msls.application.AddEditVendorDevice.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "AddEditVendorDevice", parameters);
    }

    function BrowseVendorDevices(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseVendorDevices screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="VendorDevices" type="msls.VisualCollection" elementType="msls.application.VendorDevice">
        /// Gets the vendorDevices for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseVendorDevices.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseVendorDevices", parameters);
    }

    function ViewVendorDevice(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the ViewVendorDevice screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="VendorDevice" type="msls.application.VendorDevice">
        /// Gets or sets the vendorDevice for this screen.
        /// </field>
        /// <field name="details" type="msls.application.ViewVendorDevice.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "ViewVendorDevice", parameters);
    }

    function AddEditVendor(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the AddEditVendor screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Vendor" type="msls.application.Vendor">
        /// Gets or sets the vendor for this screen.
        /// </field>
        /// <field name="details" type="msls.application.AddEditVendor.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "AddEditVendor", parameters);
    }

    function BrowseVendors(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseVendors screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Vendors" type="msls.VisualCollection" elementType="msls.application.Vendor">
        /// Gets the vendors for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseVendors.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseVendors", parameters);
    }

    function ViewVendor(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the ViewVendor screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="Vendor" type="msls.application.Vendor">
        /// Gets or sets the vendor for this screen.
        /// </field>
        /// <field name="VendorDevices" type="msls.VisualCollection" elementType="msls.application.VendorDevice">
        /// Gets the vendorDevices for this screen.
        /// </field>
        /// <field name="details" type="msls.application.ViewVendor.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "ViewVendor", parameters);
    }

    function BrowseAccessLogs(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseAccessLogs screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="AccessLogs" type="msls.VisualCollection" elementType="msls.application.AccessLog">
        /// Gets the accessLogs for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseAccessLogs.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseAccessLogs", parameters);
    }

    function BrowseAlarmLogs(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseAlarmLogs screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="AlarmLogs" type="msls.VisualCollection" elementType="msls.application.AlarmLog">
        /// Gets the alarmLogs for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseAlarmLogs.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseAlarmLogs", parameters);
    }

    function BrowseAuditLogs(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseAuditLogs screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="AuditLogs" type="msls.VisualCollection" elementType="msls.application.AuditLog">
        /// Gets the auditLogs for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseAuditLogs.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseAuditLogs", parameters);
    }

    function BrowseErrorLogs(parameters, dataWorkspace) {
        /// <summary>
        /// Represents the BrowseErrorLogs screen.
        /// </summary>
        /// <param name="parameters" type="Array">
        /// An array of screen parameter values.
        /// </param>
        /// <param name="dataWorkspace" type="msls.application.DataWorkspace" optional="true">
        /// An existing data workspace for this screen to use. By default, a new data workspace is created.
        /// </param>
        /// <field name="ErrorLogs" type="msls.VisualCollection" elementType="msls.application.ErrorLog">
        /// Gets the errorLogs for this screen.
        /// </field>
        /// <field name="details" type="msls.application.BrowseErrorLogs.Details">
        /// Gets the details for this screen.
        /// </field>
        if (!dataWorkspace) {
            dataWorkspace = new lightSwitchApplication.DataWorkspace();
        }
        $Screen.call(this, dataWorkspace, "BrowseErrorLogs", parameters);
    }

    msls._addToNamespace("msls.application", {

        AddEditAlarm: $defineScreen(AddEditAlarm, [
            { name: "Alarm", kind: "local", type: lightSwitchApplication.Alarm }
        ], [
        ]),

        BrowseAlarms: $defineScreen(BrowseAlarms, [
            {
                name: "Alarms", kind: "collection", elementType: lightSwitchApplication.Alarm,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.Alarms;
                }
            }
        ], [
        ]),

        ViewAlarm: $defineScreen(ViewAlarm, [
            { name: "Alarm", kind: "local", type: lightSwitchApplication.Alarm }
        ], [
        ]),

        AddEditCompany: $defineScreen(AddEditCompany, [
            { name: "Company", kind: "local", type: lightSwitchApplication.Company }
        ], [
        ]),

        BrowseCompanies: $defineScreen(BrowseCompanies, [
            {
                name: "Companies", kind: "collection", elementType: lightSwitchApplication.Company,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.Companies;
                }
            }
        ], [
        ]),

        ViewCompany: $defineScreen(ViewCompany, [
            { name: "Company", kind: "local", type: lightSwitchApplication.Company }
        ], [
            { name: "Delete" }
        ]),

        AddEditDefaultValue: $defineScreen(AddEditDefaultValue, [
            { name: "DefaultValue", kind: "local", type: lightSwitchApplication.DefaultValue }
        ], [
        ]),

        BrowseDefaultValues: $defineScreen(BrowseDefaultValues, [
            {
                name: "DefaultValues", kind: "collection", elementType: lightSwitchApplication.DefaultValue,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.DefaultValues.expand("Node").expand("Company").expand("Interconnection").expand("Historian");
                }
            }
        ], [
        ]),

        AddEditDevice: $defineScreen(AddEditDevice, [
            { name: "Device", kind: "local", type: lightSwitchApplication.Device }
        ], [
        ]),

        BrowseDevices: $defineScreen(BrowseDevices, [
            {
                name: "Devices", kind: "collection", elementType: lightSwitchApplication.Device,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.Devices.expand("Protocol").expand("Company");
                }
            }
        ], [
        ]),

        ViewDevice: $defineScreen(ViewDevice, [
            { name: "Device", kind: "local", type: lightSwitchApplication.Device }
        ], [
        ]),

        AddEditHistorian: $defineScreen(AddEditHistorian, [
            { name: "Historian", kind: "local", type: lightSwitchApplication.Historian }
        ], [
        ]),

        BrowseHistorians: $defineScreen(BrowseHistorians, [
            {
                name: "Historians", kind: "collection", elementType: lightSwitchApplication.Historian,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.Historians;
                }
            }
        ], [
        ]),

        ViewHistorian: $defineScreen(ViewHistorian, [
            { name: "Historian", kind: "local", type: lightSwitchApplication.Historian }
        ], [
        ]),

        AddEditInterconnection: $defineScreen(AddEditInterconnection, [
            { name: "Interconnection", kind: "local", type: lightSwitchApplication.Interconnection }
        ], [
        ]),

        BrowseInterconnections: $defineScreen(BrowseInterconnections, [
            {
                name: "Interconnections", kind: "collection", elementType: lightSwitchApplication.Interconnection,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.Interconnections;
                }
            }
        ], [
        ]),

        ViewInterconnection: $defineScreen(ViewInterconnection, [
            { name: "Interconnection", kind: "local", type: lightSwitchApplication.Interconnection }
        ], [
        ]),

        AddEditMeasurement: $defineScreen(AddEditMeasurement, [
            { name: "Measurement", kind: "local", type: lightSwitchApplication.Measurement }
        ], [
        ]),

        BrowseMeasurements: $defineScreen(BrowseMeasurements, [
            {
                name: "Measurements", kind: "collection", elementType: lightSwitchApplication.Measurement,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.Measurements;
                }
            }
        ], [
        ]),

        ViewMeasurement: $defineScreen(ViewMeasurement, [
            { name: "Measurement", kind: "local", type: lightSwitchApplication.Measurement }
        ], [
        ]),

        AddEditNodes: $defineScreen(AddEditNodes, [
            { name: "Node", kind: "local", type: lightSwitchApplication.Node }
        ], [
        ]),

        BrowseNodesSet: $defineScreen(BrowseNodesSet, [
            {
                name: "Nodes", kind: "collection", elementType: lightSwitchApplication.Node,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.Nodes;
                }
            }
        ], [
        ]),

        ViewNodes: $defineScreen(ViewNodes, [
            { name: "Node", kind: "local", type: lightSwitchApplication.Node }
        ], [
        ]),

        AddEditProtocol: $defineScreen(AddEditProtocol, [
            { name: "Protocol", kind: "local", type: lightSwitchApplication.Protocol }
        ], [
        ]),

        BrowseProtocols: $defineScreen(BrowseProtocols, [
            {
                name: "Protocols", kind: "collection", elementType: lightSwitchApplication.Protocol,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.Protocols;
                }
            }
        ], [
        ]),

        ViewProtocol: $defineScreen(ViewProtocol, [
            { name: "Protocol", kind: "local", type: lightSwitchApplication.Protocol }
        ], [
        ]),

        AddEditSignalType: $defineScreen(AddEditSignalType, [
            { name: "SignalType", kind: "local", type: lightSwitchApplication.SignalType }
        ], [
        ]),

        BrowseSignalTypes: $defineScreen(BrowseSignalTypes, [
            {
                name: "SignalTypes", kind: "collection", elementType: lightSwitchApplication.SignalType,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.SignalTypes;
                }
            }
        ], [
        ]),

        ViewSignalType: $defineScreen(ViewSignalType, [
            { name: "SignalType", kind: "local", type: lightSwitchApplication.SignalType }
        ], [
        ]),

        AddEditStatistic: $defineScreen(AddEditStatistic, [
            { name: "Statistic", kind: "local", type: lightSwitchApplication.Statistic }
        ], [
        ]),

        BrowseStatistics: $defineScreen(BrowseStatistics, [
            {
                name: "Statistics", kind: "collection", elementType: lightSwitchApplication.Statistic,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.Statistics;
                }
            }
        ], [
        ]),

        ViewStatistic: $defineScreen(ViewStatistic, [
            { name: "Statistic", kind: "local", type: lightSwitchApplication.Statistic }
        ], [
        ]),

        AddEditVendorDevice: $defineScreen(AddEditVendorDevice, [
            { name: "VendorDevice", kind: "local", type: lightSwitchApplication.VendorDevice }
        ], [
        ]),

        BrowseVendorDevices: $defineScreen(BrowseVendorDevices, [
            {
                name: "VendorDevices", kind: "collection", elementType: lightSwitchApplication.VendorDevice,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.VendorDevices;
                }
            }
        ], [
        ]),

        ViewVendorDevice: $defineScreen(ViewVendorDevice, [
            { name: "VendorDevice", kind: "local", type: lightSwitchApplication.VendorDevice }
        ], [
        ]),

        AddEditVendor: $defineScreen(AddEditVendor, [
            { name: "Vendor", kind: "local", type: lightSwitchApplication.Vendor }
        ], [
        ]),

        BrowseVendors: $defineScreen(BrowseVendors, [
            {
                name: "Vendors", kind: "collection", elementType: lightSwitchApplication.Vendor,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.Vendors;
                }
            }
        ], [
        ]),

        ViewVendor: $defineScreen(ViewVendor, [
            { name: "Vendor", kind: "local", type: lightSwitchApplication.Vendor },
            {
                name: "VendorDevices", kind: "collection", elementType: lightSwitchApplication.VendorDevice,
                getNavigationProperty: function () {
                    if (this.owner.Vendor) {
                        return this.owner.Vendor.details.properties.VendorDevices;
                    }
                    return null;
                },
                appendQuery: function () {
                    return this;
                }
            }
        ], [
        ]),

        BrowseAccessLogs: $defineScreen(BrowseAccessLogs, [
            {
                name: "AccessLogs", kind: "collection", elementType: lightSwitchApplication.AccessLog,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.AccessLogs;
                }
            }
        ], [
        ]),

        BrowseAlarmLogs: $defineScreen(BrowseAlarmLogs, [
            {
                name: "AlarmLogs", kind: "collection", elementType: lightSwitchApplication.AlarmLog,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.AlarmLogs;
                }
            }
        ], [
        ]),

        BrowseAuditLogs: $defineScreen(BrowseAuditLogs, [
            {
                name: "AuditLogs", kind: "collection", elementType: lightSwitchApplication.AuditLog,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.AuditLogs;
                }
            }
        ], [
        ]),

        BrowseErrorLogs: $defineScreen(BrowseErrorLogs, [
            {
                name: "ErrorLogs", kind: "collection", elementType: lightSwitchApplication.ErrorLog,
                createQuery: function () {
                    return this.dataWorkspace.openMICData.ErrorLogs;
                }
            }
        ], [
        ]),

        showAddEditAlarm: $defineShowScreen(function showAddEditAlarm(Alarm, options) {
            /// <summary>
            /// Asynchronously navigates forward to the AddEditAlarm screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("AddEditAlarm", parameters, options);
        }),

        showBrowseAlarms: $defineShowScreen(function showBrowseAlarms(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseAlarms screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseAlarms", parameters, options);
        }),

        showViewAlarm: $defineShowScreen(function showViewAlarm(Alarm, options) {
            /// <summary>
            /// Asynchronously navigates forward to the ViewAlarm screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("ViewAlarm", parameters, options);
        }),

        showAddEditCompany: $defineShowScreen(function showAddEditCompany(Company, options) {
            /// <summary>
            /// Asynchronously navigates forward to the AddEditCompany screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("AddEditCompany", parameters, options);
        }),

        showBrowseCompanies: $defineShowScreen(function showBrowseCompanies(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseCompanies screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseCompanies", parameters, options);
        }),

        showViewCompany: $defineShowScreen(function showViewCompany(Company, options) {
            /// <summary>
            /// Asynchronously navigates forward to the ViewCompany screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("ViewCompany", parameters, options);
        }),

        showAddEditDefaultValue: $defineShowScreen(function showAddEditDefaultValue(DefaultValue, options) {
            /// <summary>
            /// Asynchronously navigates forward to the AddEditDefaultValue screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("AddEditDefaultValue", parameters, options);
        }),

        showBrowseDefaultValues: $defineShowScreen(function showBrowseDefaultValues(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseDefaultValues screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseDefaultValues", parameters, options);
        }),

        showAddEditDevice: $defineShowScreen(function showAddEditDevice(Device, options) {
            /// <summary>
            /// Asynchronously navigates forward to the AddEditDevice screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("AddEditDevice", parameters, options);
        }),

        showBrowseDevices: $defineShowScreen(function showBrowseDevices(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseDevices screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseDevices", parameters, options);
        }),

        showViewDevice: $defineShowScreen(function showViewDevice(Device, options) {
            /// <summary>
            /// Asynchronously navigates forward to the ViewDevice screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("ViewDevice", parameters, options);
        }),

        showAddEditHistorian: $defineShowScreen(function showAddEditHistorian(Historian, options) {
            /// <summary>
            /// Asynchronously navigates forward to the AddEditHistorian screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("AddEditHistorian", parameters, options);
        }),

        showBrowseHistorians: $defineShowScreen(function showBrowseHistorians(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseHistorians screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseHistorians", parameters, options);
        }),

        showViewHistorian: $defineShowScreen(function showViewHistorian(Historian, options) {
            /// <summary>
            /// Asynchronously navigates forward to the ViewHistorian screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("ViewHistorian", parameters, options);
        }),

        showAddEditInterconnection: $defineShowScreen(function showAddEditInterconnection(Interconnection, options) {
            /// <summary>
            /// Asynchronously navigates forward to the AddEditInterconnection screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("AddEditInterconnection", parameters, options);
        }),

        showBrowseInterconnections: $defineShowScreen(function showBrowseInterconnections(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseInterconnections screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseInterconnections", parameters, options);
        }),

        showViewInterconnection: $defineShowScreen(function showViewInterconnection(Interconnection, options) {
            /// <summary>
            /// Asynchronously navigates forward to the ViewInterconnection screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("ViewInterconnection", parameters, options);
        }),

        showAddEditMeasurement: $defineShowScreen(function showAddEditMeasurement(Measurement, options) {
            /// <summary>
            /// Asynchronously navigates forward to the AddEditMeasurement screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("AddEditMeasurement", parameters, options);
        }),

        showBrowseMeasurements: $defineShowScreen(function showBrowseMeasurements(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseMeasurements screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseMeasurements", parameters, options);
        }),

        showViewMeasurement: $defineShowScreen(function showViewMeasurement(Measurement, options) {
            /// <summary>
            /// Asynchronously navigates forward to the ViewMeasurement screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("ViewMeasurement", parameters, options);
        }),

        showAddEditNodes: $defineShowScreen(function showAddEditNodes(Node, options) {
            /// <summary>
            /// Asynchronously navigates forward to the AddEditNodes screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("AddEditNodes", parameters, options);
        }),

        showBrowseNodesSet: $defineShowScreen(function showBrowseNodesSet(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseNodesSet screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseNodesSet", parameters, options);
        }),

        showViewNodes: $defineShowScreen(function showViewNodes(Node, options) {
            /// <summary>
            /// Asynchronously navigates forward to the ViewNodes screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("ViewNodes", parameters, options);
        }),

        showAddEditProtocol: $defineShowScreen(function showAddEditProtocol(Protocol, options) {
            /// <summary>
            /// Asynchronously navigates forward to the AddEditProtocol screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("AddEditProtocol", parameters, options);
        }),

        showBrowseProtocols: $defineShowScreen(function showBrowseProtocols(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseProtocols screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseProtocols", parameters, options);
        }),

        showViewProtocol: $defineShowScreen(function showViewProtocol(Protocol, options) {
            /// <summary>
            /// Asynchronously navigates forward to the ViewProtocol screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("ViewProtocol", parameters, options);
        }),

        showAddEditSignalType: $defineShowScreen(function showAddEditSignalType(SignalType, options) {
            /// <summary>
            /// Asynchronously navigates forward to the AddEditSignalType screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("AddEditSignalType", parameters, options);
        }),

        showBrowseSignalTypes: $defineShowScreen(function showBrowseSignalTypes(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseSignalTypes screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseSignalTypes", parameters, options);
        }),

        showViewSignalType: $defineShowScreen(function showViewSignalType(SignalType, options) {
            /// <summary>
            /// Asynchronously navigates forward to the ViewSignalType screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("ViewSignalType", parameters, options);
        }),

        showAddEditStatistic: $defineShowScreen(function showAddEditStatistic(Statistic, options) {
            /// <summary>
            /// Asynchronously navigates forward to the AddEditStatistic screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("AddEditStatistic", parameters, options);
        }),

        showBrowseStatistics: $defineShowScreen(function showBrowseStatistics(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseStatistics screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseStatistics", parameters, options);
        }),

        showViewStatistic: $defineShowScreen(function showViewStatistic(Statistic, options) {
            /// <summary>
            /// Asynchronously navigates forward to the ViewStatistic screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("ViewStatistic", parameters, options);
        }),

        showAddEditVendorDevice: $defineShowScreen(function showAddEditVendorDevice(VendorDevice, options) {
            /// <summary>
            /// Asynchronously navigates forward to the AddEditVendorDevice screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("AddEditVendorDevice", parameters, options);
        }),

        showBrowseVendorDevices: $defineShowScreen(function showBrowseVendorDevices(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseVendorDevices screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseVendorDevices", parameters, options);
        }),

        showViewVendorDevice: $defineShowScreen(function showViewVendorDevice(VendorDevice, options) {
            /// <summary>
            /// Asynchronously navigates forward to the ViewVendorDevice screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("ViewVendorDevice", parameters, options);
        }),

        showAddEditVendor: $defineShowScreen(function showAddEditVendor(Vendor, options) {
            /// <summary>
            /// Asynchronously navigates forward to the AddEditVendor screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("AddEditVendor", parameters, options);
        }),

        showBrowseVendors: $defineShowScreen(function showBrowseVendors(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseVendors screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseVendors", parameters, options);
        }),

        showViewVendor: $defineShowScreen(function showViewVendor(Vendor, options) {
            /// <summary>
            /// Asynchronously navigates forward to the ViewVendor screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 1);
            return lightSwitchApplication.showScreen("ViewVendor", parameters, options);
        }),

        showBrowseAccessLogs: $defineShowScreen(function showBrowseAccessLogs(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseAccessLogs screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseAccessLogs", parameters, options);
        }),

        showBrowseAlarmLogs: $defineShowScreen(function showBrowseAlarmLogs(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseAlarmLogs screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseAlarmLogs", parameters, options);
        }),

        showBrowseAuditLogs: $defineShowScreen(function showBrowseAuditLogs(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseAuditLogs screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseAuditLogs", parameters, options);
        }),

        showBrowseErrorLogs: $defineShowScreen(function showBrowseErrorLogs(options) {
            /// <summary>
            /// Asynchronously navigates forward to the BrowseErrorLogs screen.
            /// </summary>
            /// <param name="options" optional="true">
            /// An object that provides one or more of the following options:<br/>- beforeShown: a function that is called after boundary behavior has been applied but before the screen is shown.<br/>+ Signature: beforeShown(screen)<br/>- afterClosed: a function that is called after boundary behavior has been applied and the screen has been closed.<br/>+ Signature: afterClosed(screen, action : msls.NavigateBackAction)
            /// </param>
            /// <returns type="WinJS.Promise" />
            var parameters = Array.prototype.slice.call(arguments, 0, 0);
            return lightSwitchApplication.showScreen("BrowseErrorLogs", parameters, options);
        })

    });

}(msls.application));
