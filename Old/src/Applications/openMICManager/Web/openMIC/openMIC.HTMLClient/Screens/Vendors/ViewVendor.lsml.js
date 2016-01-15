/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.ViewVendor.Details_postRender = function (element, contentItem) {
    // Write code here.
    var name = contentItem.screen.Vendor.details.getModel()[':@SummaryProperty'].property.name;
    contentItem.dataBind("screen.Vendor." + name, function (value) {
        contentItem.screen.details.displayName = value;
    });
}

