﻿//******************************************************************************************************
//  gsf.web.client.js - Gbtc
//
//  Copyright © 2016, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the MIT License (MIT), the "License"; you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://opensource.org/licenses/MIT
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  01/26/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

// Grid Solutions Framework Core Web Client Script Functions
"use strict";

const isIE = detectIE();
var textMetrics;

$(function () {
    // Create a canvas object that will be used for text metrics calculations
    $("<canvas id=\"textMetricsCanvas\" height=\"1px\" width=\"1px\" style=\"visibility: hidden\"></canvas>").appendTo("body");

    // Get text metrics canvas context
    textMetrics = document.getElementById("textMetricsCanvas").getContext("2d");
});

// Miscellaneous functions
function notNull(value, nonNullValue) {
    return value || (nonNullValue || "");
}

function detectIE() {
    const ua = window.navigator.userAgent;
    const msie = ua.indexOf('MSIE ');

    if (msie > 0) {
        // IE 10 or older => return version number
        return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
    }

    const trident = ua.indexOf('Trident/');

    if (trident > 0) {
        // IE 11 => return version number
        const rv = ua.indexOf('rv:');
        return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
    }

    const edge = ua.indexOf('Edge/');

    if (edge > 0) {
        // Edge (IE 12+) => return version number
        return parseInt(ua.substring(edge + 5, ua.indexOf('.', edge)), 10);
    }

    // Other browser
    return false;
}

// Number functions
Number.prototype.truncate = function() {
    if (typeof Math.trunc != "function")
        return parseInt(this.toString());

    return Math.trunc(this);
}

Number.prototype.padLeft = function (totalWidth, paddingChar) {
    return this.truncate().toString().padLeft(totalWidth, paddingChar || "0");
}

Number.prototype.padRight = function (totalWidth, paddingChar) {
    return this.truncate().toString().padRight(totalWidth, paddingChar || "0");
}

// Array functions

// Combines a dictionary of key-value pairs into a string. Values will be escaped within startValueDelimiter and endValueDelimiter
// to contain nested key/value pair expressions like the following: "normalKVP=-1; nestedKVP={p1=true; p2=0.001}" when either the
// parameterDelimiter or the keyValueDelimiter are detected in the value of the key/value pair.
function joinKeyValuePairs (source, parameterDelimiter, keyValueDelimiter, startValueDelimiter, endValueDelimiter) {
    if (!parameterDelimiter)
        parameterDelimiter = ";";

    if (!keyValueDelimiter)
        keyValueDelimiter = "=";

    if (!startValueDelimiter)
        startValueDelimiter = "{";

    if (!endValueDelimiter)
        endValueDelimiter = "}";

    const values = [];

    for (let key in source) {
        if (source.hasOwnProperty(key)) {
            let value = source[key];

            if (typeof value === "boolean")
                value = value.toString();
            else
                value = value ? value.toString() : "";

            if (value.indexOf(parameterDelimiter) >= 0 || value.indexOf(keyValueDelimiter) >= 0)
                value = startValueDelimiter + value + endValueDelimiter;

            values.push(key + keyValueDelimiter + value);
        }
    }

    return values.join(parameterDelimiter + " ");
};

Array.prototype.joinKeyValuePairs = function (parameterDelimiter, keyValueDelimiter, startValueDelimiter, endValueDelimiter) {
    return joinKeyValuePairs(this, parameterDelimiter, keyValueDelimiter, startValueDelimiter, endValueDelimiter);
};

// Represents a dictionary style class with case-insensitive keys
function Dictionary(source) {
    const self = this;

    if (!source)
        source = [];

    self._keys = [];
    self._values = [];

    self.count = function () {
        var size = 0;

        for (let property in self._values) {
            if (self._values.hasOwnProperty(property))
                size++;
        }

        return size;
    }

    self.keys = function () {
        const keys = [];

        for (let property in self._keys) {
            if (self._keys.hasOwnProperty(property))
                keys.push(self._keys[property]);
        }

        return keys;
    }

    self.values = function () {
        const values = [];

        for (let property in self._values) {
            if (self._keys.hasOwnProperty(property))
                values.push(self._values[property]);
        }

        return values;
    }

    self.get = function (key) {
        return self._values[String(key).toLowerCase()];
    }

    self.set = function (key, value) {
        const lkey = String(key).toLowerCase();

        if (!self._keys[lkey] || self._keys[lkey].toLowerCase() !== key)
            self._keys[lkey] = key;

        self._values[lkey] = value;
    }

    self.remove = function (key) {
        const lkey = String(key).toLowerCase();
        delete self._keys[lkey];
        delete self._values[lkey];
    }

    self.containsKey = function (key) {
        const lkey = String(key).toLowerCase();

        for (let property in self._values) {
            if (self._values.hasOwnProperty(property) && property === lkey)
                return true;
        }

        return false;
    }

    self.containsValue = function (value) {
        for (let property in self._values) {
            if (self._values.hasOwnProperty(property) && self._values[property] === value)
                return true;
        }

        return false;
    }

    self.clear = function () {
        self._keys = [];
        self._values = [];
    }

    self.joinKeyValuePairs = function (parameterDelimiter, keyValueDelimiter, startValueDelimiter, endValueDelimiter) {
        const keyValuePairs = [];

        for (let property in self._values) {
            if (self._values.hasOwnProperty(property))
                keyValuePairs[self._keys[property]] = self._values[property];
        }

        return keyValuePairs.joinKeyValuePairs(parameterDelimiter, keyValueDelimiter, startValueDelimiter, endValueDelimiter);
    }

    self.pushAll = function (source) {
        for (let property in source)
            if (source.hasOwnProperty(property))
                self.set(property, source[property]);
    }

    self.toObservableDictionary = function (useLowerKeys) {
        // See ko.observableDictionary.js
        const observableDictionary = new ko.observableDictionary();

        for (let property in self._values) {
            if (self._values.hasOwnProperty(property))
                observableDictionary.push(useLowerKeys ? property : self._keys[property], self._values[property]);
        }

        return observableDictionary;
    }

    self.updateObservableDictionary = function (observableDictionary, useLowerKeys) {
        for (let property in self._values) {
            if (self._values.hasOwnProperty(property))
                observableDictionary.set(useLowerKeys ? property : self._keys[property], self._values[property]);
        }
    }

    // Construction
    if (source instanceof Dictionary) {
        for (let property in source._values)
            if (source._values.hasOwnProperty(property))
                self.set(source._keys[property], source._values[property]);
    }
    else {
        for (let property in source) {
            if (source.hasOwnProperty(property))
                self.set(property, source[property]);
        }
    }
}

Dictionary.fromObservableDictionary = function (observableDictionary) {
    var dictionary = new Dictionary();
    dictionary.pushAll(observableDictionary.toJSON());
    return dictionary;
}

// String functions
function isEmpty(str) {
    return !str || String(str).length === 0;
}

String.prototype.truncate = function (limit) {
    const text = this.trim();

    if (text.length > limit)
        return text.substr(0, limit - 3) + "...";

    return text;
}

String.prototype.replaceAll = function (findText, replaceWith, ignoreCase) {
    return this.replace(
        new RegExp(findText.replace(/([\/\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g, "\\$&"), (ignoreCase ? "gi" : "g")),
        (typeof replaceWith == "string") ? replaceWith.replace(/\$/g, "$$$$") : replaceWith);
}

String.prototype.padLeft = function (totalWidth, paddingChar) {
    if (totalWidth > this.length)
        return Array(totalWidth - this.length + 1).join(paddingChar || " ") + this;

    return this;
}

String.prototype.padRight = function (totalWidth, paddingChar) {
    if (totalWidth > this.length)
        return this + Array(totalWidth - this.length + 1).join(paddingChar || " ");

    return this;
}

String.prototype.regexEncode = function () {
    return "\\u" + this.charCodeAt(0).toString(16).padLeft(4, "0");
}

// Returns a Dictionary of the parsed key/value pair expressions from a string. Parameter pairs are delimited by keyValueDelimiter
// and multiple pairs separated by parameterDelimiter. Supports encapsulated nested expressions.
String.prototype.parseKeyValuePairs = function (parameterDelimiter, keyValueDelimiter, startValueDelimiter, endValueDelimiter, ignoreDuplicateKeys) {
    if (!parameterDelimiter)
        parameterDelimiter = ";";

    if (!keyValueDelimiter)
        keyValueDelimiter = "=";

    if (!startValueDelimiter)
        startValueDelimiter = "{";

    if (!endValueDelimiter)
        endValueDelimiter = "}";

    if (ignoreDuplicateKeys === undefined)
        ignoreDuplicateKeys = true;

    if (parameterDelimiter === keyValueDelimiter ||
        parameterDelimiter === startValueDelimiter ||
        parameterDelimiter === endValueDelimiter ||
        keyValueDelimiter === startValueDelimiter ||
        keyValueDelimiter === endValueDelimiter ||
        startValueDelimiter === endValueDelimiter)
            throw "All delimiters must be unique";

    const escapedParameterDelimiter = parameterDelimiter.regexEncode();
    const escapedKeyValueDelimiter = keyValueDelimiter.regexEncode();
    const escapedStartValueDelimiter = startValueDelimiter.regexEncode();
    const escapedEndValueDelimiter = endValueDelimiter.regexEncode();
    const backslashDelimiter = "\\".regexEncode();

    var keyValuePairs = new Dictionary();
    var escapedValue = [];
    var valueEscaped = false;
    var delimiterDepth = 0;

    // Escape any parameter or key/value delimiters within tagged value sequences
    //      For example, the following string:
    //          "normalKVP=-1; nestedKVP={p1=true; p2=false}")
    //      would be encoded as:
    //          "normalKVP=-1; nestedKVP=p1\\u003dtrue\\u003b p2\\u003dfalse")
    for (let i = 0; i < this.length; i++) {
        let character = this[i];

        if (character === startValueDelimiter) {
            if (!valueEscaped) {
                valueEscaped = true;
                continue;   // Don't add tag start delimiter to final value
            }

            // Handle nested delimiters
            delimiterDepth++;
        }

        if (character === endValueDelimiter) {
            if (valueEscaped) {
                if (delimiterDepth > 0) {
                    // Handle nested delimiters
                    delimiterDepth--;
                }
                else {
                    valueEscaped = false;
                    continue;   // Don't add tag stop delimiter to final value
                }
            }
            else {
                throw "Failed to parse key/value pairs: invalid delimiter mismatch. Encountered end value delimiter \"" + endValueDelimiter + "\" before start value delimiter \"" + startValueDelimiter + "\".";
            }
        }

        if (valueEscaped) {
            // Escape any delimiter characters inside nested key/value pair
            if (character === parameterDelimiter)
                escapedValue.push(escapedParameterDelimiter);
            else if (character === keyValueDelimiter)
                escapedValue.push(escapedKeyValueDelimiter);
            else if (character === startValueDelimiter)
                escapedValue.push(escapedStartValueDelimiter);
            else if (character === endValueDelimiter)
                escapedValue.push(escapedEndValueDelimiter);
            else if (character === "\\")
                escapedValue.push(backslashDelimiter);
            else
                escapedValue.push(character);
        }
        else {
            if (character === "\\")
                escapedValue.push(backslashDelimiter);
            else
                escapedValue.push(character);
        }
    }

    if (delimiterDepth !== 0 || valueEscaped) {
        // If value is still escaped, tagged expression was not terminated
        if (valueEscaped)
            delimiterDepth = 1;

        throw "Failed to parse key/value pairs: invalid delimiter mismatch. Encountered more " + 
            (delimiterDepth > 0 ? "start value delimiters \"" + startValueDelimiter + "\"" : "end value delimiters \"" + endValueDelimiter + "\"") + " than " + 
            (delimiterDepth < 0 ? "start value delimiters \"" + startValueDelimiter + "\"" : "end value delimiters \"" + endValueDelimiter + "\"") + ".";
    }

    // Parse key/value pairs from escaped value
    let pairs = escapedValue.join("").split(parameterDelimiter);

    for (let i = 0; i < pairs.length; i++) {
        // Separate key from value
        let elements = pairs[i].split(keyValueDelimiter);

        if (elements.length === 2) {
            // Get key
            let key = elements[0].trim();

            // Get unescaped value
            let unescapedValue = elements[1].trim().
                replaceAll(escapedParameterDelimiter, parameterDelimiter).
                replaceAll(escapedKeyValueDelimiter, keyValueDelimiter).
                replaceAll(escapedStartValueDelimiter, startValueDelimiter).
                replaceAll(escapedEndValueDelimiter, endValueDelimiter).
                replaceAll(backslashDelimiter, "\\");

            // Add key/value pair to dictionary
            if (ignoreDuplicateKeys) {
                // Add or replace key elements with unescaped value
                keyValuePairs.set(key, unescapedValue);
            }
            else {
                // Add key elements with unescaped value throwing an exception for encountered duplicate keys
                if (keyValuePairs.containsKey(key))
                    throw "Failed to parse key/value pairs: duplicate key encountered. Key \"" + key + "\" is not unique within the string: \"" + this + "\"";

                keyValuePairs.set(key, unescapedValue);
            }
        }
    }

    return keyValuePairs;
}

// Date Functions
String.prototype.toDate = function () {
    return new Date(Date.parse(this));
};

String.prototype.formatDate = function (format, utc) {
    return formatDate(this.toDate(), format, utc);
};

Date.prototype.formatDate = function (format, utc) {
    return formatDate(this, format, utc);
};

function formatDate(date, format, utc) {
    if (typeof date === 'string')
        return formatDate(date.toDate(), format, utc);

    if (format === undefined && DateTimeFormat != undefined)
        format = DateTimeFormat;

    if (utc === undefined)
        utc = true;

    var MMMM = ["\x00", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    var MMM = ["\x01", "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var dddd = ["\x02", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    var ddd = ["\x03", "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

    function ii(i, len) {
        var ss = i + "";
        len = len || 2;
        while (ss.length < len) ss = "0" + ss;
        return ss;
    }

    var y = utc ? date.getUTCFullYear() : date.getFullYear();
    format = format.replace(/(^|[^\\])yyyy+/g, "$1" + y);
    format = format.replace(/(^|[^\\])yy/g, "$1" + y.toString().substr(2, 2));
    format = format.replace(/(^|[^\\])y/g, "$1" + y);

    var M = (utc ? date.getUTCMonth() : date.getMonth()) + 1;
    format = format.replace(/(^|[^\\])MMMM+/g, "$1" + MMMM[0]);
    format = format.replace(/(^|[^\\])MMM/g, "$1" + MMM[0]);
    format = format.replace(/(^|[^\\])MM/g, "$1" + ii(M));
    format = format.replace(/(^|[^\\])M/g, "$1" + M);

    var d = utc ? date.getUTCDate() : date.getDate();
    format = format.replace(/(^|[^\\])dddd+/g, "$1" + dddd[0]);
    format = format.replace(/(^|[^\\])ddd/g, "$1" + ddd[0]);
    format = format.replace(/(^|[^\\])dd/g, "$1" + ii(d));
    format = format.replace(/(^|[^\\])d/g, "$1" + d);

    var H = utc ? date.getUTCHours() : date.getHours();
    format = format.replace(/(^|[^\\])HH+/g, "$1" + ii(H));
    format = format.replace(/(^|[^\\])H/g, "$1" + H);

    var h = H > 12 ? H - 12 : H === 0 ? 12 : H;
    format = format.replace(/(^|[^\\])hh+/g, "$1" + ii(h));
    format = format.replace(/(^|[^\\])h/g, "$1" + h);

    var m = utc ? date.getUTCMinutes() : date.getMinutes();
    format = format.replace(/(^|[^\\])mm+/g, "$1" + ii(m));
    format = format.replace(/(^|[^\\])m/g, "$1" + m);

    var s = utc ? date.getUTCSeconds() : date.getSeconds();
    format = format.replace(/(^|[^\\])ss+/g, "$1" + ii(s));
    format = format.replace(/(^|[^\\])s/g, "$1" + s);

    var f = utc ? date.getUTCMilliseconds() : date.getMilliseconds();
    format = format.replace(/(^|[^\\])fff+/g, "$1" + ii(f, 3));
    f = Math.round(f / 10);
    format = format.replace(/(^|[^\\])ff/g, "$1" + ii(f));
    f = Math.round(f / 10);
    format = format.replace(/(^|[^\\])f/g, "$1" + f);

    var T = H < 12 ? "AM" : "PM";
    format = format.replace(/(^|[^\\])TT+/g, "$1" + T);
    format = format.replace(/(^|[^\\])T/g, "$1" + T.charAt(0));

    var t = T.toLowerCase();
    format = format.replace(/(^|[^\\])tt+/g, "$1" + t);
    format = format.replace(/(^|[^\\])t/g, "$1" + t.charAt(0));

    var tz = -date.getTimezoneOffset();
    var K = utc || !tz ? "Z" : tz > 0 ? "+" : "-";
    if (!utc) {
        tz = Math.abs(tz);
        var tzHrs = Math.floor(tz / 60);
        var tzMin = tz % 60;
        K += ii(tzHrs) + ":" + ii(tzMin);
    }

    format = format.replace(/(^|[^\\])K/g, "$1" + K);
    var day = (utc ? date.getUTCDay() : date.getDay()) + 1;

    format = format.replace(new RegExp(dddd[0], "g"), dddd[day]);
    format = format.replace(new RegExp(ddd[0], "g"), ddd[day]);

    format = format.replace(new RegExp(MMMM[0], "g"), MMMM[M]);
    format = format.replace(new RegExp(MMM[0], "g"), MMM[M]);

    format = format.replace(/\\(.)/g, "$1");

    return format;
};

// jQuery extensions
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

    if (rows > 1)
        targetWidth *= ((isIE ? 0.65 : 0.90) * rows);

    var limit = Math.min(text.length, Math.ceil(targetWidth / (textWidth / text.length)));

    while (textWidth > targetWidth && limit > 1) {
        limit--;
        text = text.truncate(limit);
        textWidth = textMetrics.measureText(text).width;
    }

    return text;
}
