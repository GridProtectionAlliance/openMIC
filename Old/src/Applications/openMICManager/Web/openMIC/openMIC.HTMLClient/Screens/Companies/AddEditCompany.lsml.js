/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.AddEditCompany.created = function (screen) {
    screen.Company.MapAcronym = "";
    screen.Company.Name = "";
    screen.Company.LoadOrder = 0;
    screen.Company.CreatedOn = new Date();
    screen.Company.CreatedBy = "<CurrentUser>";
    screen.Company.UpdatedOn = new Date();
    screen.Company.UpdatedBy = "<CurrentUser>";
};

myapp.AddEditCompany.Acronym_postRender = function (element, contentItem) {
    $(element).change(function () {
        contentItem.value = contentItem.value.toString().toUpperCase();
    });
};