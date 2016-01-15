/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.ViewSignalType.Details_postRender = function (element, contentItem) {
    // Write code here.
    var name = contentItem.screen.SignalType.details.getModel()[':@SummaryProperty'].property.name;
    contentItem.dataBind("screen.SignalType." + name, function (value) {
        contentItem.screen.details.displayName = value;
    });
}

