/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.BrowseCompanies.DisplayCompanyCount = function (screen) {
    if (screen.Companies != undefined && screen.Companies != null) {
        screen.details.displayName = "Companies: " + screen.Companies.count;
    }
}

myapp.BrowseCompanies.Companies_postRender = function (element, contentItem) {
    contentItem.dataBind("screen.Companies.count", function (newValue) {
        myapp.BrowseCompanies.DisplayCompanyCount(contentItem.screen);
    });
};

myapp.BrowseCompanies.Delete_execute = function (screen) {

    screen.Company.deleteEntity();

    return myapp.commitChanges().then(null, function fail(e) {
        myapp.cancelChanges();
        throw e;
    });
};
