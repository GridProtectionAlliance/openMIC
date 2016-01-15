/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.ViewDevice.Details_postRender = function (element, contentItem) {
    // Write code here.
    var name = contentItem.screen.Device.details.getModel()[':@SummaryProperty'].property.name;
    contentItem.dataBind("screen.Device." + name, function (value) {
        contentItem.screen.details.displayName = value;
    });
}

