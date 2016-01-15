/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.ViewInterconnection.Details_postRender = function (element, contentItem) {
    // Write code here.
    var name = contentItem.screen.Interconnection.details.getModel()[':@SummaryProperty'].property.name;
    contentItem.dataBind("screen.Interconnection." + name, function (value) {
        contentItem.screen.details.displayName = value;
    });
}

