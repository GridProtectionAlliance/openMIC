/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.ViewCompany.Details_postRender = function (element, contentItem) {
    var name = contentItem.screen.Company.details.getModel()[':@SummaryProperty'].property.name;
    contentItem.dataBind("screen.Company." + name, function (value) {
        contentItem.screen.details.displayName = value;
    });
}


myapp.ViewCompany.Delete_execute = function (screen) {
    msls.showMessageBox("Are you sure you want to delete '" + screen.details.displayName + "'?", {
        title: "Confirm Deletion",
        buttons: msls.MessageBoxButtons.yesNo
    }).then(function (result) {
        screen.Company.deleteEntity();

        myapp.commitChanges().then(null, function fail(e) {
            myapp.cancelChanges();
            throw e;
        });
    });
};