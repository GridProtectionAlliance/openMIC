/// <reference path="~/GeneratedArtifacts/viewModel.js" />
/// <reference path="../Scripts/ls-utils.js" />

var initialized = false;

myapp.BrowseDevices.created = function (screen) {
    if (initialized)
        return;

    initialized = true;

    // hook up template events once template is loaded 
    // this is a callbackfunction so set it up before loading template
    $(screen).on('templateLoaded', function (path) {
        //// -- Example Tabs --
        //$('#tabCustomers').bind("click", function () {
        //    screen.showTab("CustomerTab");
        //});
        //$('#tabSuppliers').bind("click", function () {
        //    screen.showTab("SupplierTab");
        //});
        //$('#tabShippers').bind("click", function () {
        //    screen.showTab("ShipperTab");
        //});
        //$('#tabTerritory').bind("click", function () {
        //    screen.showTab("TerritoryTab");
        //});
        //$('#tabEmployees').bind("click", function () {
        //    screen.showTab("EmployeeTab");
        //});
        //$('#tabOrders').bind("click", function () {
        //    screen.showTab("OrderTab");
        //});

        // -- Toolbar --
        $('#toolbarUser').bind("click", function () {
            alert('you clicked User');
        });
        $('#toolbarReports').bind("click", function () {
            alert('you clicked Reports');
        });
        $('#toolbarAdministration').bind("click", function () {
            alert('you clicked Administration');
        });
        $('#toolbarSystem').bind("click", function () {
            alert('you clicked System');
        });
        $('#toolbarLogout').bind("click", function () {
            alert('you clicked Logout');
        });

    });

    // use a timeout function (to give slight pause after the created event) to
    // load the template into the header
    setTimeout(function () {
        // load and replace header titles bar with our new header template
        var $headerTitle = $('html').find(".titles-bar");
        LightSwitchUtils.loadTemplate(screen, $headerTitle, "PartialViews/header.htm", false);
    });
};

myapp.BrowseDevices.updateProgressBar = function (id, value) {
    $(id).val(value);
    $(id).slider("refresh");
}

// TEST: Remove this testing function once progress bar is wired to SignalR
myapp.BrowseDevices.testProgressUpdate = function(id)  {
    var i = 1;

    var interval = setInterval(function () {
        myapp.BrowseDevices.updateProgressBar(id, i++);
        if (i > 100)
            clearInterval(interval);
    }, 100);
}

myapp.BrowseDevices.updateEnabledState = function (deviceFrame, deviceID, enabled) {
    
    var id = "#deviceProgress" + deviceID;
    $(id).deviceEnabled = enabled;

    if (enabled) {
        deviceFrame.css({ 'color': '', 'background-color': '', 'background-image': '' });
        myapp.BrowseDevices.testProgressUpdate(id);
    }
    else {
        deviceFrame.css({ 'color': '#000000', 'background-color': '#c0c0c0', 'background-image': 'none' });
    }
}

myapp.BrowseDevices.rows_postRender = function (element, contentItem) {
    var id = 'deviceProgress' + contentItem.data.ID;
    var progressControl = $('<div>Current Download Progress:</div>');

    $(progressControl).appendTo($(element).parent());

    $('<input>').appendTo($(progressControl)).attr({ 'name': id, 'id': id, 'data-highlight': 'true', 'min': '0', 'max': '100', 'value': '0', 'type': 'range', 'data-role': 'none' }).slider({
        create: function (event, ui) {
            $(this).parent().find('input').hide();
            $(this).parent().find('input').css('margin-left', '-9999px');
            $(this).parent().find('.ui-slider-track').css('margin', '0 1px 0 1px');
            $(this).parent().find('.ui-slider-handle').hide();
        }
    }).slider("refresh");

    myapp.BrowseDevices.updateEnabledState($(element).parent(), contentItem.data.ID, contentItem.data.Enabled);
};

myapp.BrowseDevices.Enabled_postRender = function (element, contentItem) {
    contentItem.dataBind("value", function (newValue) {
        if (newValue !== contentItem.data.Enabled) {
            myapp.applyChanges();
            myapp.BrowseDevices.updateEnabledState($(element).parent().parent().parent(), contentItem.data.ID, newValue);
        }
    });
};
