/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.ViewNodes.Details_postRender = function (element, contentItem) {
    // Write code here.
    var name = contentItem.screen.Node.details.getModel()[':@SummaryProperty'].property.name;
    contentItem.dataBind("screen.Node." + name, function (value) {
        contentItem.screen.details.displayName = value;
    });
}

