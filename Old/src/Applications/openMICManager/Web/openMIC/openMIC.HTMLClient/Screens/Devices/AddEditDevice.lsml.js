/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.AddEditDevice.created = function (screen) {
    screen.Device.UniqueID = Math.uuid();
    screen.Device.AccessID = 0;
    screen.Device.TimeAdjustmentTicks = 0;
    screen.Device.MeasurementReportingInterval = 10;
    screen.Device.LoadOrder = 0;
    screen.Device.Enabled = true;
    screen.Device.CreatedOn = new Date();
    screen.Device.CreatedBy = "<CurrentUser>";
    screen.Device.UpdatedOn = new Date();
    screen.Device.UpdatedBy = "<CurrentUser>";
};

myapp.AddEditDevice.Acronym_postRender = function (element, contentItem) {
    $(element).change(function () {
        contentItem.value = contentItem.value.toString().toUpperCase();
    });
};
