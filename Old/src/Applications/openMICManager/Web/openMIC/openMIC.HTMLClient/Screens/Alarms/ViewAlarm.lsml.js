/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.ViewAlarm.Details_postRender = function (element, contentItem) {
    // Write code here.
    var name = contentItem.screen.Alarm.details.getModel()[':@SummaryProperty'].property.name;
    contentItem.dataBind("screen.Alarm." + name, function (value) {
        contentItem.screen.details.displayName = value;
    });
}

