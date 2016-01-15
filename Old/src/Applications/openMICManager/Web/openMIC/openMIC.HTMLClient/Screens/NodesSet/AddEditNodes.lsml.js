/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.AddEditNodes.created = function (screen) {
    screen.Node.ID = Math.uuid();
    screen.Node.Enabled = true;
    screen.Node.MenuType = "XML";
    screen.Node.MenuData = "Menu.xml";
    screen.Node.LoadOrder = 0;
    screen.Node.CreatedOn = new Date();
    screen.Node.CreatedBy = "<CurrentUser>";
    screen.Node.UpdatedOn = new Date();
    screen.Node.UpdatedBy = "<CurrentUser>";
};