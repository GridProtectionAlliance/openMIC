//******************************************************************************************************
//  RemoteConsole.js - Gbtc
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

$(function() {
    var overRemoteConsole = false;
    var defaultRemoteConsoleEntries = 100;
    var minimumRemoteConsoleEntries = 10;
    var totalRemoteConsoleEntries = Cookies.get("totalRemoteConsoleEntries");

    // Initialize default setting for total remote console entries
    if (!totalRemoteConsoleEntries)
        totalRemoteConsoleEntries = defaultRemoteConsoleEntries;

    // Set initial text input values
    $("#remoteConsoleTotalEntriesTextInput").val(totalRemoteConsoleEntries.toString());
    $("#remoteConsoleTotalEntriesTextInput").attr("min", minimumRemoteConsoleEntries.toString());

    $("#remoteConsoleSettingsShowButton").click(function() {
        $("#remoteConsoleSettingsForm").toggle();

        if ($("#remoteConsoleSettingsForm").is(":visible"))
            $("#remoteConsoleTotalEntriesTextInput").focus();
    });

    $("#setTotalRemoteConsoleEntriesButton").click(function() {
        totalRemoteConsoleEntries = parseInt($("#remoteConsoleTotalEntriesTextInput").val());

        if (totalRemoteConsoleEntries < minimumRemoteConsoleEntries)
            totalRemoteConsoleEntries = minimumRemoteConsoleEntries;

        Cookies.set("totalRemoteConsoleEntries", totalRemoteConsoleEntries, { expires: 365 });
        $("#remoteConsoleTotalEntriesTextInput").val(totalRemoteConsoleEntries.toString());
        $("#remoteConsoleSettingsForm").hide();
    });

    // Prevent default form submission when user presses enter
    $("#remoteConsoleSettingsForm").submit(function() {
        return false;
    });

    $("#remoteConsoleTotalEntriesTextInput").keyup(function(event) {
        if (event.keyCode === 13)
            $("#setTotalRemoteConsoleEntriesButton").click();
    });

    // Auto-hide pop-up form when user clicks outside form area
    $("#remoteConsoleSettingsForm").focusout(function() {
        if (!$("#remoteConsoleSettingsForm").is(":hover") && !$("#remoteConsoleSettingsShowButton").is(":hover"))
            $("#remoteConsoleSettingsForm").hide();
    });

    $("#remoteConsoleSettingsCloseButton").click(function() {
        $("#remoteConsoleSettingsForm").hide();
    });

    function scrollRemoteConsoleToBottom() {
        var remoteConsole = $("#remoteConsoleWindow");
        remoteConsole.scrollTop(remoteConsole[0].scrollHeight);
    }

    // Create a function that the hub can call to broadcast new remote console messages
    serviceHubClient.broadcastMessage = function(message, color) {
        // Html encode message
        var encodedMessage = $("<div />").text(message).html();
        var remoteConsole = $("#remoteConsoleWindow");

        remoteConsole.append('<span style="color: ' + color + '">' + encodedMessage + "</span>");

        if (remoteConsole[0].childElementCount > totalRemoteConsoleEntries)
            remoteConsole.find(":first-child").remove();

        if (!overRemoteConsole)
            scrollRemoteConsoleToBottom();
    }

    $("#remoteConsoleWindow").mouseenter(function() {
        overRemoteConsole = true;
    });

    $("#remoteConsoleWindow").mouseleave(function() {
        overRemoteConsole = false;
        scrollRemoteConsoleToBottom();
    });

    $(window).resize(function() {
        $("#remoteConsoleFontSizeLabel").html("Font Size: " + $("#remoteConsoleWindow").css("font-size") + ", Window Size: " + $(window).width().toString());
        scrollRemoteConsoleToBottom();
    });

    $("#sendCommandButton").click(function() {
        // Call the send command method on the hub
        if (hubIsConnected)
            serviceHub.sendCommand($("#commandTextInput").val());

        // Clear text box and reset focus for next command
        $("#commandTextInput").val("").focus();
    });

    $("#commandTextInput").keyup(function(event) {
        if (event.keyCode === 13)
            $("#sendCommandButton").click();
    });

    $('#commandTextInput').focus();
});
