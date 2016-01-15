/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.ViewHistorian.Details_postRender = function (element, contentItem) {
    // Write code here.
    var name = contentItem.screen.Historian.details.getModel()[':@SummaryProperty'].property.name;
    contentItem.dataBind("screen.Historian." + name, function (value) {
        contentItem.screen.details.displayName = value;
    });
}

