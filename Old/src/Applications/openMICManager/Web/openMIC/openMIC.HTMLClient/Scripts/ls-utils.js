/* 
LightSwitch sample general utils javascript library.
*/

var LightSwitchUtils = (function () {

    function LightSwitchUtils() {
    }

    //Method: loadTemplate
    //Params: (screen) screen: the screen object into which the template is loaded (required for templateLoaded() callback)
    //Params: ($element) element: the element which needs to be appended to or of wich the content needs to be replaced
    //Params: (string) path: the relative path to a file that contains template definition(s)
    //Params: (boolean) replace: replace the contents instead of appending (default to false)
    LightSwitchUtils.loadTemplate = function (screen, element, path, replace) {
        replace = (typeof replace === "undefined") ? false : replace;
        //Use jQuery Ajax to fetch the template file
        var templateLoader = $.get(path)
            .success(function (result) {
                //On success, Add templates to DOM (assumes file only has template definitions)
                if (replace) {
                    $(element).html($(result));
                } else {
                    $(result).appendTo($(element));
                }
            })
            .error(function (result) {
                alert("Error Loading Template: " + path);
            });

        templateLoader.complete(function () {
            $(screen).trigger("templateLoaded", path);
        });
    }

    return LightSwitchUtils;
})();




