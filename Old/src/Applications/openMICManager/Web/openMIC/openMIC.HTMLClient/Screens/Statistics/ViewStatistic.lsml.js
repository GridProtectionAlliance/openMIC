/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.ViewStatistic.Details_postRender = function (element, contentItem) {
    // Write code here.
    var name = contentItem.screen.Statistic.details.getModel()[':@SummaryProperty'].property.name;
    contentItem.dataBind("screen.Statistic." + name, function (value) {
        contentItem.screen.details.displayName = value;
    });
}

