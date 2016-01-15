/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.ViewProtocol.Details_postRender = function (element, contentItem) {
    // Write code here.
    var name = contentItem.screen.Protocol.details.getModel()[':@SummaryProperty'].property.name;
    contentItem.dataBind("screen.Protocol." + name, function (value) {
        contentItem.screen.details.displayName = value;
    });
}

