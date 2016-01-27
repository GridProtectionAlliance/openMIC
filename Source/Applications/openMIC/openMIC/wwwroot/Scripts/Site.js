//******************************************************************************************************
//  Site.js - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may not use this
//  file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  01/15/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

// Declare page scoped SignalR variables
var dataHub, dataHubClient, serviceHub, serviceHubClient;
var hubIsConnecting = false;
var hubIsConnected = false;
var textMetrics;

function hideErrorMessage() {
    $("#error-msg-block").hide();
}

function hideInfoMessage() {
    $("#info-msg-block").hide();
}

function showErrorMessage(message, timeout) {
    $("#error-msg-text").html(message);
    $("#error-msg-block").show();

    if (timeout != undefined && timeout > 0)
        setTimeout(hideErrorMessage, timeout);
}

function showInfoMessage(message, timeout) {
    $("#info-msg-text").html(message);
    $("#info-msg-block").show();

    if (timeout === undefined)
        timeout = 3000;

    if (timeout > 0)
        setTimeout(hideInfoMessage, timeout);
}

$.fn.paddingHeight = function () {
    return this.outerHeight(true) - this.height();
}

$.fn.paddingWidth = function () {
    return this.outerWidth(true) - this.width();
}

$.fn.truncateToWidth = function (text, rows) {
    if (isEmpty(text))
        return "";

    if (!rows)
        rows = 1;

    textMetrics.font = this.css("font");

    var targetWidth = this.innerWidth();
    var textWidth = textMetrics.measureText(text).width;

    if (rows > 1) {
        targetWidth *= (0.65 * rows);
        //this.height(40);
    }

    var limit = Math.min(text.length, Math.ceil(targetWidth / (textWidth / text.length)));
    
    while (textWidth > targetWidth && limit > 1) {
        limit--;
        text = truncateString(text, limit);
        textWidth = textMetrics.measureText(text).width;
    }

    return text;
}

var getTextHeight = function (font) {

    var text = $('<span>H</span>').css({ fontFamily: font });
    var block = $('<div style="display: inline-block; width: 1px; height: 0px;"></div>');

    var div = $('<div></div>');
    div.append(text, block);

    var body = $('body');
    body.append(div);

    var result = {};

    try {
        block.css({ verticalAlign: 'baseline' });
        result.ascent = block.offset().top - text.offset().top;

        block.css({ verticalAlign: 'bottom' });
        result.height = block.offset().top - text.offset().top;

        result.descent = result.height - result.ascent;

    } finally {
        div.remove();
    }

    return result;
};

function truncateString(text, limit) {
    if (!text)
        return "";

    if (typeof text != "string")
        text = text.toString();

    text = text.trim();

    if (!limit)
        limit = 65;

    if (text.length > limit)
        return text.substr(0, limit - 3) + "...";

    return text;
}

function truncateNumber(value) {
    if (typeof Math.trunc != "function")
        return parseInt(value.toString());

    return Math.trunc(value);
}

function isEmpty(str) {
    return !str || 0 === str.length;
}

function notNull(value, nonNullValue) {
    return value || (nonNullValue || "");
}

function detectIE() {
    var ua = window.navigator.userAgent;
    var msie = ua.indexOf('MSIE ');

    if (msie > 0) {
        // IE 10 or older => return version number
        return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
    }

    var trident = ua.indexOf('Trident/');

    if (trident > 0) {
        // IE 11 => return version number
        var rv = ua.indexOf('rv:');
        return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
    }

    var edge = ua.indexOf('Edge/');

    if (edge > 0) {
        // Edge (IE 12+) => return version number
        return parseInt(ua.substring(edge + 5, ua.indexOf('.', edge)), 10);
    }

    // Other browser
    return false;
}

function calculateRemainingBodyHeight() {

    // Calculation based on content in Layout.cshtml
    return $(window).height() -
        $("#menuBar").outerHeight(true) -
        $("#bodyContainer").paddingHeight() -
        $("#pageHeader").outerHeight(true) -
        ($(window).width() < 768 ? 30 : 5);
}

function hubConnected() {
    hideErrorMessage();

    if (hubIsConnecting)
        showInfoMessage("Reconnected to service.");

    hubIsConnecting = false;
    hubIsConnected = true;

    // Re-enable hub dependent controls
    updateHubDependentControlState(true);

    // Call "onHubConnected" function, if page has defined one
    if (typeof onHubConnected == "function")
        onHubConnected();
}

function updateHubDependentControlState(enabled) {
    if (enabled)
        $("[hub-dependent]").removeClass("disabled");
    else
        $("[hub-dependent]").addClass("disabled");
}

// Useful to call when dynamic data-binding adds new controls
function refreshHubDependentControlState() {
    updateHubDependentControlState(hubIsConnected);
}

$(function () {
    $(".page-header").css("margin-bottom", "-5px");

    // Get text metrics canvas
    textMetrics = document.getElementById("textMetricsCanvas").getContext("2d");

    // Set initial state of hub dependent controls
    updateHubDependentControlState(false);

    // Initialize proxy references to the SignalR hubs
    dataHub = $.connection.dataHub.server;
    dataHubClient = $.connection.dataHub.client;
    serviceHub = $.connection.serviceHub.server;
    serviceHubClient = $.connection.serviceHub.client;

    $.connection.hub.reconnecting(function () {
        hubIsConnecting = true;
        showInfoMessage("Attempting to reconnect to service&nbsp;&nbsp;<span class='glyphicon glyphicon-refresh glyphicon-spin'></span>", -1);

        // Disable hub dependent controls
        updateHubDependentControlState(false);
    });

    $.connection.hub.reconnected(function () {
        hubConnected();
    });

    $.connection.hub.disconnected(function () {
        hubIsConnected = false;

        if (hubIsConnecting)
            showErrorMessage("Disconnected from server");

        // Disable hub dependent controls
        updateHubDependentControlState(false);

        // Call "onHubDisconnected" function, if page has defined one
        if (typeof onHubDisconnected == "function")
            onHubDisconnected();

        setTimeout(function () {
            $.connection.hub.start().done(function () {
                hubConnected();
            });
        }, 5000); // Restart connection after 5 seconds
    });

    // Start the connection
    $.connection.hub.start().done(function () {
        hubConnected();
    });

    $(window).on("beforeunload", function () {
        if (!hubIsConnected || hubIsConnecting)
            return "Service is disconnected, web pages are currently unavailable.";

        return undefined;
    });

    // Enable tool-tips on the page
    $("[data-toggle='tooltip']").tooltip();
});
