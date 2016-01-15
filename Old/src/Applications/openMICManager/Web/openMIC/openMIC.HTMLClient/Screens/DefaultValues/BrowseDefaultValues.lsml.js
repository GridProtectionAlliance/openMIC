/// <reference path="~/GeneratedArtifacts/viewModel.js" />

myapp.BrowseDefaultValues.AddDefaultValue_postRender = function (element, contentItem) {
    // Only allow a single record to be added
    msls.application.activeDataWorkspace.openMICData.DefaultValueCount().execute().then(
            function (results) {
                contentItem.isEnabled = (results.results.length === 0);
            },
            function (error) {
                alert(error);
                contentItem.isEnabled = true;
            }
        );
};

myapp.BrowseDefaultValues.Node_postRender = function (element, contentItem) {
    // When any new row gets added - this means first record exists, so disable add button
    contentItem.dataBind("value", function (newValue) {
        contentItem.screen.findContentItem("AddDefaultValue").isEnabled = false;
    });
};