//******************************************************************************************************
//  SectionMapBuilder.js - Gbtc
//
//  Copyright © 2021, Grid Protection Alliance.  All Rights Reserved.
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
//  08/02/2021 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

"use strict";

if (typeof jQuery === "undefined") {
    throw new Error("SectionMapBuilder script requires jQuery - make sure jquery.js is loaded first");
}

if (!String.prototype) {
    String.prototype.replaceAll = function (findText, replaceWith, ignoreCase) {
        const findVal = typeof findText === "string" ? findText : String(findText);
        const replaceVal = typeof replaceWith === "string" ? replaceWith.replace(/\$/g, "$$$$") : replaceWith;

        return this.replace(
            new RegExp(findVal.replace(/([\/\,\!\\\^\$\{\}\[\]\(\)\.\*\+\?\|\<\>\-\&])/g, "\\$&"), ignoreCase ? "gi" : "g"), replaceVal);
    };
}

if (!String.format) {
    String.format = function (format) {
        let text = format;

        for (let i = 1; i < arguments.length; i++)
            text = text.replaceAll("{" + (i - 1) + "}", arguments[i]);

        return text;
    };
}

function SectionMapBuilder(instanceName) {
    const self = this;

    // Property definition types
    const PropDefType = {
        CHECKBOX:      0,
        NUMERIC:       1,
        FLOAT:         2,
        HEADER:        3,
        TEXT:          4,
        DATE:          5,
        TIME:          6,
        SELECT:        8,
        DATETIMERANGE: 11,
        PASSWORD:      12,
        MULTILINETEXT: 13
    }

    const zeroFieldTypes = [PropDefType.FLOAT, PropDefType.NUMERIC, PropDefType.TEXT];

    // Defines the maximum number of alias parameters to check for
    self.maxAliasParams = 5;

    // Assign function to determine if section element is enabled
    self.elementIsEnabled = (element) => element;

    // Assign function expression (as string) that represents notification that model has changed (isDirty = true)
    self.modelChangedExpr = "";

    self._bankIndex = undefined;
    self._unmapped = 0;

    self.execOperationAtBankIndex = (index, operation) => {
        try {
            self._bankIndex = index;
            operation();
        }
        finally {
            self._bankIndex = undefined;
        }
    };

    self.clearValue = (target, isCheckBox, clearConfig) => {
        if (isCheckBox)
            target.prop("checked", false);
        else
            target.val("");

        if (clearConfig && !target[0].hasAttribute("config-cleared")) {
            target.attr("config-cleared", "");
            target.trigger("change");
        }
    };

    self.clearZeroValue = (target, type) => {
        if (!zeroFieldTypes.includes(type) || target[0].hasAttribute("zero-cleared"))
            return;

        target.attr("zero-cleared", "");

        // Show zero values from config as blank
        if (parseFloat(target.val()) === 0.0)
            target.val("");
    };

    self.readValue = (element, readOperation, enabled, type) => {
        const target = $(element);
        const bank = target.attr("bank");
        const isCheckBox = type === PropDefType.CHECKBOX;

        // Handle multi-select bank list read operation
        if (bank && bank.length) {
            const selections = self.getValue(bank);

            if (Array.isArray(selections) && selections.length > 1) {
                const values = [];

                selections.forEach(selection => {
                    self.execOperationAtBankIndex(parseInt(selection, 10), readOperation);
                    values.push(isCheckBox ? target.prop("checked") : target.val());
                });

                // If any selected items have different values, clear target
                if (values.some(value => value !== values[0]))
                    self.clearValue(target, isCheckBox);
            }
            else {
                if (enabled) {
                    readOperation();
                    self.clearZeroValue(target, type);
                } else {
                    self.clearValue(target, isCheckBox, true);
                }
            }
        }
        else {
            if (enabled) {
                readOperation();
                self.clearZeroValue(target, type);
            } else {
                self.clearValue(target, isCheckBox, true);
            }
        }
    };

    self.writeValue = (element, writeOperation) => {
        const target = $(element);
        const bank = target.attr("bank");

        // Handle multi-select bank list write operation
        if (bank && bank.length) {
            const selections = self.getValue(bank);

            if (Array.isArray(selections) && selections.length > 1) {
                // Assign changed value to all selected items
                selections.forEach(selection => {
                    self.execOperationAtBankIndex(parseInt(selection, 10), writeOperation);
                });
            }
            else {
                writeOperation();
            }
        }
        else {
            writeOperation();
        }
    };

    self.getValue = (targetID, targetType) => {
        const target = $(`#${targetID}`);

        // Check if target is a bank list
        if (target[0].hasAttribute("bank-list")) {
            if (self._bankIndex === undefined) {
                const values = target.val() || [];

                if (values.length > 0)
                    return values.length === 1 ? parseInt(values[0], 10) : values;

                return 0;
            }
            else {
                return self._bankIndex;
            }
        }
        else {
            const value = target.val();

            switch (targetType) {
                case undefined:
                case "float":
                    return parseFloat(value);
                case "int":
                    return parseInt(value, 10);
                case "bool":
                    return parseInt(value, 10) !== 0;
                default:
                    return value.toString();
            }
        }
    };

    self.getKeyValue = (targetSelectID, targetType) => {
        const selectedOption = $(`#${targetSelectID} option:selected`);

        if (selectedOption.length === 0) {
            switch (targetType) {
                case undefined:
                case "float":
                    return 0.0;
                case "int":
                    return 0;
                case "bool":
                    return false;
                default:
                    return "";
            }
        }

        const value = selectedOption.attr("key-value");

        switch (targetType) {
            case undefined:
            case "float":
                return parseFloat(value);
            case "int":
                return parseInt(value, 10);
            case "bool":
                return parseInt(value, 10) !== 0;
            default:
                return value.toString();
        }
    };

    self.selectBankItem = (bankTargetID) => {
        $(`[bank="${bankTargetID}"]`).each(function () {
            try {
                this.removeAttribute("zero-cleared");
                eval(this.getAttribute("update-expr"));
            } catch (e) {
                console.error(e);
            }
        });
    };

    self.buildListElements = (list, defaultVal) => {
        const html = [];

        let index = 0;
        let totalOptions = 0;

        for (let i = 0; i < list.ITEM.length; i++) {
            const value = list.ITEM[i];
            const optVal = value["@VALUE"] || index;
            const keyVal = value["@KEYVALUE"] ? ` key-value="${value["@KEYVALUE"]}"` : "";
            const enabled = self.elementIsEnabled(value);
            const hidden = enabled ? "" : " hidden";
            const selected = optVal === defaultVal || !defaultVal && enabled && totalOptions === 0 ? " selected" : "";
            let alias = value["@ALIAS"];

            if (alias) {
                for (let j = 0; j < self.maxAliasParams; j++) {
                    const aliasParam = value[`@ALIAS${j}`];

                    if (aliasParam)
                        alias = alias.replaceAll(`{${j}}`, eval(aliasParam));
                    else
                        break;
                }
            }

            html.push(`<option value="${optVal}"${keyVal}${selected}${hidden}>${alias || value["@NAME"]}</option>`);

            if (enabled)
                totalOptions++;

            index++;
        }

        return [html.join(""), totalOptions];
    };

    self.buildPropDefElement = (propDef, mapRoot, bankTarget) => {
        const type = parseInt(propDef["@TYPE"], 10);
        const name = propDef["@NAME"];
        let alias = propDef["@ALIAS"];

        if (alias) {
            for (let i = 0; i < self.maxAliasParams; i++) {
                const aliasParam = propDef[`@ALIAS${i}`];

                if (aliasParam)
                    alias = alias.replaceAll(`{${i}}`, eval(aliasParam));
                else
                    break;
            }
        }

        const enabled = self.elementIsEnabled(propDef);
        const hidden = enabled ? "" : " hidden";
        const bank = bankTarget && bankTarget.length ? ` bank="${bankTarget}"` : "";
        const getPropVal = type === PropDefType.CHECKBOX ? `$('#${name}').prop('checked')` : `$('#${name}').val()`;
        const setPropVal = type === PropDefType.CHECKBOX ? `$('#${name}').prop('checked', {0})` : `$('#${name}').val({0})`;
        const readOnly = parseInt(propDef["@RO"] || "1", 10) > 0;
        const map = propDef["@MAP"] || `${instanceName}._unmapped`;

        const applyInstanceGetValue = value => value.replaceAll("#val(", `${instanceName}.getValue(`);
        const mapExpr = applyInstanceGetValue(String.format(map.startsWith(".") ? `${mapRoot}${map}` : map, bankTarget));

        const applySubstitutions = value => applyInstanceGetValue(value
            .replaceAll("{name}", name)
            .replaceAll("{value}", getPropVal)
            .replaceAll("{mapExpr}", mapExpr)
            .replaceAll("{mapRoot}", mapRoot)
            .replaceAll("{bankTarget}", bankTarget));

        const readExpr = propDef["@READ"] ? applySubstitutions(propDef["@READ"]) : undefined;
        const writeExpr = propDef["@WRITE"] ? applySubstitutions(propDef["@WRITE"]) : undefined;
        const preReadExpr = propDef["@PREREAD"] ? `${applySubstitutions(propDef["@PREREAD"])}; ` : "";
        const postReadExpr = propDef["@POSTREAD"] ? ` ${applySubstitutions(propDef["@POSTREAD"])};` : "";
        const preWriteExpr = propDef["@PREWRITE"] ? `${applySubstitutions(propDef["@PREWRITE"])}; ` : "";
        const postWriteExpr = propDef["@POSTWRITE"] ? ` ${applySubstitutions(propDef["@POSTWRITE"])};` : "";
        const updatesConfigExpr = enabled && parseInt(propDef["@UPDATESCONFIG"] || "1", 10) !== 0 && self.modelChangedExpr ? ` ${self.modelChangedExpr};` : "";

        const updateFunction = `const update_{0} = new Function("${readOnly ? "" : `${preReadExpr}{1};${postReadExpr}`}");`;
        const changedFunction = `const {0}_changed = new Function("${preWriteExpr}{1};${postWriteExpr}${updatesConfigExpr}");`;

        const placeHolderText = propDef["@PLACEHOLDER"] ? ` placeholder="${propDef["@PLACEHOLDER"]}"` : "";
        const hasParam = propDef.hasOwnProperty("PARAM");
        const min = hasParam ? propDef.PARAM["@MIN"] : undefined;
        const max = hasParam ? propDef.PARAM["@MAX"] : undefined;
        const minMaxText = `${min ? ` min="${min}"` : ``}${max ? ` max="${max}"` : ``}`;
        const maxLen = hasParam ? propDef.PARAM["@LEN"] : undefined;
        const maxLenText = `${maxLen ? ` maxlength="${maxLen}"` : ``}`;

        const inputAttributes =
            `id="${name}"${minMaxText}${maxLenText}${placeHolderText} ` +
            `onchange="${instanceName}.writeValue(this, ${name}_changed)" ` +
            `update-expr="${instanceName}.readValue(this, update_${name}, ${enabled}, ${type})" ` +
            `data-toggle="tooltip" map-expr="${mapExpr}"${bank}`;

        const html = [];
        const script = [];

        html.push(`<tr${hidden}><th class="smb-pad-right" width="30%">${alias || name}:</th><td>`);

        switch (type) {
            case PropDefType.CHECKBOX:
                script.push(String.format(updateFunction, name, `${String.format(setPropVal, `${readExpr ? String.format(readExpr, mapExpr) : `parseInt(${mapExpr}, 10) !== 0`}`)}`));
                script.push(String.format(changedFunction, name, `${mapExpr} = ${writeExpr ? String.format(writeExpr, getPropVal) : `${getPropVal} ? '1' : '0'`}`));
                html.push(`<input type="checkbox" class="smb-input" ${inputAttributes}/>`);
                break;
            case PropDefType.NUMERIC:
                script.push(String.format(updateFunction, name, `${String.format(setPropVal, `${readExpr ? String.format(readExpr, mapExpr) : mapExpr}`)}`));
                script.push(String.format(changedFunction, name, `${mapExpr} = ${writeExpr ? `(${String.format(writeExpr, getPropVal)}).toString()` : getPropVal}`));
                html.push(`<input type="number" class="form-control smb-input" ${inputAttributes}/>`);
                break;
            case PropDefType.FLOAT:
                script.push(String.format(updateFunction, name, `${String.format(setPropVal, `${readExpr ? `(${String.format(readExpr, mapExpr)})` : `parseFloat(${mapExpr})`}.toFixed(4)`)}`));
                script.push(String.format(changedFunction, name, `${mapExpr} = ${writeExpr ? `(${String.format(writeExpr, getPropVal)}).toString()` : getPropVal}`));
                html.push(`<input type="number" class="form-control smb-input" ${inputAttributes}/>`);
                break;
            case PropDefType.TEXT:
            case PropDefType.PASSWORD:
                script.push(String.format(updateFunction, name, `${String.format(setPropVal, `${readExpr ? String.format(readExpr, mapExpr) : mapExpr}`)}`));
                script.push(String.format(changedFunction, name, `${mapExpr} = ${writeExpr ? `(${String.format(writeExpr, getPropVal)}).toString()` : getPropVal}`));
                html.push(`<input type="${type === PropDefType.TEXT ? "text" : "password"}" class="form-control smb-input" ${inputAttributes}/>`);
                break;
            case PropDefType.SELECT:
                script.push(String.format(updateFunction, name, `${String.format(setPropVal, `${readExpr ? String.format(readExpr, mapExpr) : mapExpr}`)}`));
                script.push(String.format(changedFunction, name, `${mapExpr} = ${writeExpr ? `(${String.format(writeExpr, getPropVal)}).toString()` : getPropVal}`));
                html.push(`<select class="form-control smb-input" ${inputAttributes}>`);
                html.push(self.buildListElements(propDef.PARAM.LIST, propDef["@DEFAULT"])[0]);
                html.push(`</select>`);
                break;
        }

        html.push(`</td></tr>`);

        return [html.join(""), script.join("\r\n")];
    };

    self.buildBankElement = (definition, mapRoot, sectionName) => {
        const bankName = definition["@NAME"];
        const bankAlias = definition["@ALIAS"];
        const enabled = self.elementIsEnabled(definition);
        const hidden = enabled ? "" : " hidden";
        const rows = definition["@ROWS"];
        const sizeToken = "[_SIZE-TOKEN_]";
        const html = [];
        const script = [];

        let sizeAttribute = "";
        let propDefsStarted = false;

        html.push(`<tr${hidden}><th class="smb-header${definition.order > 0 ? "-section-row" : ""} smb-bank-header"><div class="smb-header">${bankAlias}</div></th></tr>`);
        html.push(`<tr class="smb-bank-group"${hidden}><td class="smb-bank-group">`);

        const startPropDefs = () => {
            if (propDefsStarted)
                return;

            propDefsStarted = true;
            html.push(`<table class="smb-section-elements">`);
        };

        const closePropDefs = () => {
            if (!propDefsStarted)
                return;

            propDefsStarted = false;
            html.push(`</table>`);
        };

        const addPropDef = (propDef) => {
            if (parseInt(propDef["@TYPE"], 10) === PropDefType.HEADER) {
                console.warn(`Unexpected header property definition encountered in section "${sectionName}" bank "${bankName}"`);
                return;
            }

            if (propDef.hasOwnProperty("@NAME"))
                propDef["@NAME"] = `${bankName}_${propDef["@NAME"]}`;

            const [propDefHtml, propDefScript] = self.buildPropDefElement(propDef, mapRoot, bankName);

            html.push(propDefHtml);

            if (propDefScript.length > 0)
                script.push(propDefScript);
        }

        for (let key in definition) {
            if (definition.hasOwnProperty(key) && !key.startsWith("@")) {
                const value = definition[key];

                switch (key) {
                    case "LIST":
                        {
                            closePropDefs();
                            html.push(`<select multiple class="form-control smb-bank-list" id="${bankName}"${sizeToken} onchange="${instanceName}.selectBankItem('${bankName}')" update-expr="${instanceName}.selectBankItem('${bankName}')" bank-list>`);

                            const [listHtml, totalOptions] = self.buildListElements(value);
                            html.push(listHtml);

                            if (rows) {
                                let selectSize = parseInt(rows, 10);

                                if (selectSize > totalOptions && totalOptions > 0)
                                    selectSize = totalOptions;

                                sizeAttribute = ` size="${selectSize}"`;
                            }

                            html.push(`</select>`);
                        }
                        break;
                    case "PROPDEF":
                        {
                            startPropDefs();

                            if (Array.isArray(value)) {
                                for (let i = 0; i < value.length; i++)
                                    addPropDef(value[i]);
                            }
                            else {
                                addPropDef(value);
                            }
                        }
                        break;
                }
            }
        }

        closePropDefs();

        html.push(`</td></tr>`);
        html.push(`<tr${hidden}><th class="smb-bank-footer"><div class="smb-bank-footer"></div></th></tr>`);

        return [html.join("").replace(sizeToken, sizeAttribute), script.join("\r\n")];
    };

    self.buildSection = (section, configRoot) => {
        const mapRoot = `${configRoot}${section["@MAPROOT"] ? `.${section["@MAPROOT"]}` : ""}`;
        const sectionName = section["@NAME"];
        const html = [];
        const script = [];
        const definitions = [];

        let naturalOrder = 100000;
        let propDefsStarted = false;

        html.push(`<table class="smb-section">`);

        const startPropDefs = () => {
            if (propDefsStarted)
                return;

            propDefsStarted = true;
            html.push(`<tr><td><table class="smb-section-elements">`);
        };

        const closePropDefs = () => {
            if (!propDefsStarted)
                return;

            propDefsStarted = false;
            html.push(`</table></td></tr>`);
        };

        const addSectionElementDefinition = (definition, elementType) => {
            if (definition.hasOwnProperty("@NAME") && parseInt(definition["@TYPE"], 10) !== PropDefType.HEADER)
                definition["@NAME"] = `${sectionName}Section_${definition["@NAME"]}`;

            definition.order = definition.hasOwnProperty("@ORDER") ?
                parseInt(definition["@ORDER"], 10) :
                definitions.length > 0 ? naturalOrder++ : 0;

            definition.elementType = elementType;

            definitions.push(definition);
        }

        const addSectionElementDefinitions = (elementType) => {
            const definition = section[elementType];

            if (!definition)
                return;

            if (Array.isArray(definition)) {
                for (let i = 0; i < definition.length; i++) {
                    addSectionElementDefinition(definition[i], elementType);
                }
            }
            else {
                addSectionElementDefinition(definition, elementType);
            }
        };

        addSectionElementDefinitions("BANK");
        addSectionElementDefinitions("PROPDEF");

        definitions.sort((a, b) => a.order > b.order ? 1 : -1);

        for (let i = 0; i < definitions.length; i++) {
            const definition = definitions[i];

            switch (definition.elementType) {
                case "BANK":
                    {
                        const [bankHtml, bankScript] = self.buildBankElement(definition, mapRoot, sectionName);

                        closePropDefs();
                        html.push(bankHtml);

                        if (bankScript.length > 0)
                            script.push(bankScript);
                    }
                    break;
                case "PROPDEF":
                    {
                        if (parseInt(definition["@TYPE"], 10) === PropDefType.HEADER) {
                            if (self.elementIsEnabled(definition)) {
                                closePropDefs();
                                html.push(`<tr><th class="smb-header${definition.order > 0 ? "-section-row" : ""}"><div class="header">${definition["@ALIAS"] || definition["@NAME"]}</div></th></tr>`);
                            }
                        }
                        else {
                            const [propDefHtml, propDefScript] = self.buildPropDefElement(definition, mapRoot);

                            startPropDefs();
                            html.push(propDefHtml);

                            if (propDefScript.length > 0)
                                script.push(propDefScript);
                        }
                    }
                    break;
            }
        }

        closePropDefs();
        html.push(`</table>`);

        return [html.join(""), script.join("\r\n")];
    };
}
