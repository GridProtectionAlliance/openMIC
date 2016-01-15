/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.ViewMeasurement.Details_postRender = function (element, contentItem) {
    // Write code here.
    var name = contentItem.screen.Measurement.details.getModel()[':@SummaryProperty'].property.name;
    contentItem.dataBind("screen.Measurement." + name, function (value) {
        contentItem.screen.details.displayName = value;
    });
}

