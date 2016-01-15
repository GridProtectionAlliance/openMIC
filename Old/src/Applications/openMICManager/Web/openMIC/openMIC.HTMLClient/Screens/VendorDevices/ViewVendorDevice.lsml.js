/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.ViewVendorDevice.Details_postRender = function (element, contentItem) {
    // Write code here.
    var name = contentItem.screen.VendorDevice.details.getModel()[':@SummaryProperty'].property.name;
    contentItem.dataBind("screen.VendorDevice." + name, function (value) {
        contentItem.screen.details.displayName = value;
    });
}

