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

if (!String.prototype.replaceAll) {
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
            text = text.replaceAll(`{${i - 1}}`, arguments[i]);

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

    // Property types that can display a zero value, i.e., are subject to show zero as blank
    const zeroFieldTypes = [PropDefType.NUMERIC, PropDefType.FLOAT, PropDefType.TEXT];

    // Defines the maximum number of alias parameters to check
    self.maxAliasParams = 5;

    // Assign function to determine if section element is enabled
    self.elementIsEnabled = (element) => element;

    // Assign function expression (as string) that represents notification that model has changed (isDirty = true)
    self.modelChangedExpr = "";

    // Option to show loaded zero values as blank
    self.showZeroValuesAsBlank = true;

    // Internal fields
    self._configRoot = undefined;
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
        if (!zeroFieldTypes.includes(type))
            return;

        // Show zero values from config as blank
        if (parseFloat(target.val()) === 0.0)
            target.val("");
    };

    self.readValue = (element, readOperation, enabled, type) => {
        const target = $(element);
        const selections = self.getBankSelections(target.attr("bank"), true);
        const isCheckBox = type === PropDefType.CHECKBOX;

        if (selections) {
            const values = [];

            selections.forEach(selection => {
                self.execOperationAtBankIndex(selection, readOperation);
                values.push(isCheckBox ? target.prop("checked") : target.val());
            });

            // If any selected items have different values, clear target
            if (values.some(value => value !== values[0]))
                self.clearValue(target, isCheckBox);
            else if (self.showZeroValuesAsBlank)
                self.clearZeroValue(target, type);
        }
        else {
            if (enabled) {
                readOperation();

                if (self.showZeroValuesAsBlank)
                    self.clearZeroValue(target, type);
            } else {
                self.clearValue(target, isCheckBox, true);
            }
        }
    };

    self.writeValue = (element, writeOperation) => {
        const target = $(element);
        const selections = self.getBankSelections(target.attr("bank"), true);

        if (selections) {
            // Assign changed value to all selected items
            selections.forEach(selection => {
                self.execOperationAtBankIndex(selection, writeOperation);
            });
        }
        else {
            writeOperation();
        }
    };

    self.getValue = (targetID, targetType) => {
        const target = $(`#${targetID}`);

        if (target.length === 0)
            return undefined;

        if (self.targetIsBankList(target)) {
            // Get selected bank index
            if (self._bankIndex === undefined) {
                const values = target.val() || [];
                return values.length > 0 ? parseInt(values[0], 10) : 0;
            } else {
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

    self.targetIsBankList = (target) => {
        if (typeof target === "string")
            target = $(`#${target}`);

        if (target instanceof jQuery)
            target = target.length ? target[0] : undefined;

        return target ? target.hasAttribute("bank-list") : false;
    };

    self.getBankSelections = (bank, multipleOnly) => {
        const target = $(`#${bank}`);

        if (self.targetIsBankList(target)) {
            const values = target.val() || [];

            if (Array.isArray(values) && !(multipleOnly && values.length < 2)) {
                const selections = [];
                values.forEach(value => selections.push(parseInt(value, 10)));
                return selections;
            }
        }

        return multipleOnly ? undefined : [];
    };

    self.selectBankItem = (bankTargetID) => {
        $(`[bank="${bankTargetID}"]`).each(function () {
            try {
                eval(this.getAttribute("update-expr"));
            } catch (e) {
                console.error(e);
            }
        });
    };

    self.getProperty = (element, propertyName, defaultValue) => {
        if (element && element.hasOwnProperty(propertyName)) {
            const value = element[propertyName].trim();

            if (value.length)
                return value;
        }

        return defaultValue;
    };

    self.getAlias = (element) => {
        let alias = self.getProperty(element, "@ALIAS");

        if (!alias)
            return self.getProperty(element, "@NAME");

        for (let i = 0; i < self.maxAliasParams; i++) {
            const aliasParam = self.getProperty(element, `@ALIAS${i}`);

            if (aliasParam === undefined)
                break;

            alias = alias.replaceAll(`{${i}}`, eval(aliasParam));
        }

        return alias;
    };

    self.getType = (element, defaultValue) => {
        const typeValue = self.getProperty(element, "@TYPE", defaultValue);

        if (!typeValue)
            throw new Error("@TYPE attribute not found");

        const type = parseInt(typeValue, 10);

        if (!isNaN(type))
            return type;

        const typeName = typeValue.toString().toUpperCase().trim();

        if (PropDefType.hasOwnProperty(typeName))
            return PropDefType[typeName];

        throw new Error(`@TYPE attribute "${typeName}" is undefined`);
    };

    self.buildListElements = (list, type, defaultVal) => {
        const html = [];

        let index = 0;
        let totalOptions = 0;
        let items = [];

        if (Array.isArray(list.ITEM))
            items = list.ITEM;
        else
            items.push(list.ITEM);

        if (type === PropDefType.NUMERIC)
            defaultVal = defaultVal ? parseInt(defaultVal, 10) : -1;
        else
            defaultVal = defaultVal || null;

        for (let i = 0; i < items.length; i++) {
            const value = items[i];
            const keyVal = value["@KEYVALUE"] ? ` key-value="${value["@KEYVALUE"]}"` : "";
            const enabled = self.elementIsEnabled(value);
            const hidden = enabled ? "" : " hidden";

            let optVal;
            
            if (type === PropDefType.NUMERIC)
                optVal = parseInt(self.getProperty(value, "@VALUE", index), 10);
            else if (type === PropDefType.TEXT)
                optVal = self.getProperty(value, "@VALUE", "");
            else
                optVal = self.getProperty(value, "@VALUE", index);

            const selected = optVal === defaultVal || !defaultVal && enabled && totalOptions === 0 ? " selected" : "";

            html.push(`<option value="${optVal}"${keyVal}${selected}${hidden}>${self.getAlias(value)}</option>`);

            if (enabled)
                totalOptions++;

            index++;
        }

        return [html.join(""), totalOptions];
    };

    self.buildPropDefElement = (propDef, mapRoot, bankTarget) => {
        const type = self.getType(propDef);
        const name = self.getProperty(propDef, "@NAME");
        const enabled = self.elementIsEnabled(propDef);
        const hidden = enabled ? "" : " hidden";
        const bank = bankTarget && bankTarget.length ? ` bank="${bankTarget}"` : "";
        const getPropVal = type === PropDefType.CHECKBOX ? `$('#${name}').prop('checked')` : `$('#${name}').val()`;
        const setPropVal = type === PropDefType.CHECKBOX ? `$('#${name}').prop('checked', {0})` : `$('#${name}').val({0})`;
        const readOnly = parseInt(self.getProperty(propDef, "@RO", "0"), 10) > 0;
        const map = self.getProperty(propDef, "@MAP") || `${instanceName}._unmapped`;

        const applyMappingSubstitutions = value => value
            .replaceAll("{configRoot}", self._configRoot, true)
            .replaceAll("{bankTarget}", bankTarget, true)
            .replaceAll("#bankIndex", `${instanceName}.getValue('${bankTarget}')`, true)
            .replaceAll("#val(", `${instanceName}.getValue(`, true)
            .replaceAll("#keyVal(", `${instanceName}.getKeyValue(`, true);

        const mapExpr = applyMappingSubstitutions(map.startsWith(".") || map.startsWith("[") ? `${mapRoot}${map}` : map);
        const sectionName = self.getProperty(propDef, "@SECTIONNAME");
        const rootName = `${sectionName}${bankTarget && bankTarget.length ? `_${bankTarget}` : ""}`;

        const applyExpressionSubstitutions = value => applyMappingSubstitutions(value
            .replaceAll("{name}", name, true)                   // Current element name, e.g., inputSection_bankInput_channelName
            .replaceAll("{sectionName}", sectionName, true)     // Current section name, e.g., inputSection (from <SECTION NAME="input"/>)
            .replaceAll("{rootName}", rootName, true)           // Root name of current element, e.g., inputSection_bankInput
            .replaceAll("{mapExpr}", mapExpr, true)             // Fully qualified mapping expression, e.g., viewModel.deviceConfig().inputSection.channelName
            .replaceAll("{mapRoot}", mapRoot, true)             // Map expression root, e.g., viewModel.deviceConfig().inputSection
            .replaceAll("{value}", getPropVal, true)            // Javascript expression that results in DOM element value
            .replaceAll("{keyValue}",                           // Gets selected key-value attribute from drop-down (from <ITEM KEYVALUE="altValue"/>)
                `${instanceName}.getKeyValue('${name}')`, true)
        );

        const readExpr = propDef["@READ"] ? applyExpressionSubstitutions(propDef["@READ"]) : undefined;
        const writeExpr = propDef["@WRITE"] ? applyExpressionSubstitutions(propDef["@WRITE"]) : undefined;
        const preReadExpr = propDef["@PREREAD"] ? `${applyExpressionSubstitutions(propDef["@PREREAD"])}; ` : "";
        const postReadExpr = propDef["@POSTREAD"] ? ` ${applyExpressionSubstitutions(propDef["@POSTREAD"])};` : "";
        const preWriteExpr = propDef["@PREWRITE"] ? `${applyExpressionSubstitutions(propDef["@PREWRITE"])}; ` : "";
        const postWriteExpr = propDef["@POSTWRITE"] ? ` ${applyExpressionSubstitutions(propDef["@POSTWRITE"])};` : "";
        const updatesConfigExpr = enabled && parseInt(self.getProperty(propDef, "@UPDATESCONFIG", "1"), 10) !== 0 && self.modelChangedExpr ? ` ${self.modelChangedExpr};` : "";

        const updateFunction = `const update_{0} = new Function("${readOnly ? "" : `${preReadExpr}{1};${postReadExpr}`}");`;
        const changedFunction = `const {0}_changed = new Function("${preWriteExpr}{1};${postWriteExpr}${updatesConfigExpr}");`;

        const placeHolderText = propDef["@PLACEHOLDER"] ? ` placeholder="${propDef["@PLACEHOLDER"]}"` : "";
        const trueValue = propDef["@TRUE"] || "1";
        const falseValue = propDef["@FALSE"] || "0";
        const precision = propDef["@PRECISION"] || "6";
        const min = self.getProperty(propDef.PARAM, "@MIN");
        const max = self.getProperty(propDef.PARAM, "@MAX");
        const minMaxText = `${min ? ` min="${min}"` : ``}${max ? ` max="${max}"` : ``}`;
        const maxLen = self.getProperty(propDef.PARAM, "@LEN");
        const maxLenText = `${maxLen ? ` maxlength="${maxLen}"` : ``}`;

        const inputAttributes =
            `id="${name}"${minMaxText}${maxLenText}${placeHolderText} ` +
            `onchange="${instanceName}.writeValue(this, ${name}_changed)" ` +
            `update-expr="${instanceName}.readValue(this, update_${name}, ${enabled}, ${type})" ` +
            `data-toggle="tooltip" map-expr="${mapExpr}"${bank}`;

        const html = [];
        const script = [];

        html.push(`<tr id="row-${name}"${hidden}><th class="smb-pad-right" width="30%">${self.getAlias(propDef)}:</th><td>`);

        switch (type) {
            case PropDefType.CHECKBOX:
                script.push(String.format(updateFunction, name, String.format(setPropVal, readExpr ? readExpr : `${mapExpr} !== '${falseValue}'`)));
                script.push(String.format(changedFunction, name, `${mapExpr} = ${writeExpr ? `(${writeExpr}).toString()` : `${getPropVal} ? '${trueValue}' : '${falseValue}'`}`));
                html.push(`<input type="checkbox" class="smb-input" ${inputAttributes}/>`);
                break;
            case PropDefType.NUMERIC:
                script.push(String.format(updateFunction, name, String.format(setPropVal, readExpr || mapExpr)));
                script.push(String.format(changedFunction, name, `${mapExpr} = ${writeExpr ? `(${writeExpr}).toString()` : getPropVal}`));
                html.push(`<input type="number" class="form-control smb-input" ${inputAttributes}/>`);
                break;
            case PropDefType.FLOAT:
                script.push(String.format(updateFunction, name, String.format(setPropVal, readExpr ? readExpr : `parseFloat(${mapExpr}).toFixed(${precision})`)));
                script.push(String.format(changedFunction, name, `${mapExpr} = ${writeExpr ? `(${writeExpr}).toString()` : getPropVal}`));
                html.push(`<input type="number" class="form-control smb-input" ${inputAttributes}/>`);
                break;
            case PropDefType.TEXT:
            case PropDefType.PASSWORD:
                script.push(String.format(updateFunction, name, String.format(setPropVal, readExpr || mapExpr)));
                script.push(String.format(changedFunction, name, `${mapExpr} = ${writeExpr ? `(${writeExpr}).toString()` : getPropVal}`));
                html.push(`<input type="${type === PropDefType.TEXT ? "text" : "password"}" class="form-control smb-input" ${inputAttributes}/>`);
                break;
            case PropDefType.SELECT:
                script.push(String.format(updateFunction, name, String.format(setPropVal, readExpr || mapExpr)));
                script.push(String.format(changedFunction, name, `${mapExpr} = ${writeExpr ? `(${writeExpr}).toString()` : getPropVal}`));
                html.push(`<select class="form-control smb-input" ${inputAttributes}>`);
                html.push(self.buildListElements(propDef.PARAM.LIST, self.getType(propDef.PARAM, PropDefType.NUMERIC), propDef["@DEFAULT"])[0]);
                html.push(`</select>`);
                break;
        }

        html.push(`</td></tr>`);

        return [html.join(""), script.join("\r\n")];
    };

    self.buildBankElement = (definition, mapRoot) => {
        const bankName = self.getProperty(definition, "@NAME");
        const enabled = self.elementIsEnabled(definition);
        const hidden = enabled ? "" : " hidden";
        const rows = self.getProperty(definition, "@ROWS");
        const sizeToken = "[_SIZE-TOKEN_]";
        const html = [];
        const script = [];

        let sizeAttribute = "";
        let propDefsStarted = false;
        let bankMapRoot = mapRoot;
        let groupMapRoot = undefined;

        const mapRootValue = self.getProperty(definition, "@MAPROOT");

        if (mapRootValue)
            bankMapRoot = mapRootValue.startsWith(".") || mapRootValue.startsWith("[") ? `${mapRoot}${mapRootValue}` : `${self._configRoot}.${mapRootValue}`;

        html.push(`<tr${hidden}><th class="smb-header${definition.order > 0 ? "-section-row" : ""} smb-bank-header"><div class="smb-header">${self.getAlias(definition)}</div></th></tr>`);
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
            if (self.getType(propDef) === PropDefType.HEADER) {
                if (self.elementIsEnabled(propDef)) {
                    const headerMapRoot = self.getProperty(propDef, "@MAPROOT");

                    groupMapRoot = headerMapRoot ?
                        (headerMapRoot.startsWith(".") || headerMapRoot.startsWith("[") ? `${bankMapRoot}${headerMapRoot}` : `${self._configRoot}.${headerMapRoot}`) :
                        undefined;

                    const name = self.getProperty(propDef, "@NAME");
                    const rowID = name ? ` id="row-${bankName}_${name}"` : "";

                    html.push(`<tr${rowID}><th class="smb-bank-header-row" colspan="2"><div class="smb-bank-header-row">${self.getAlias(propDef) || ""}</div></th></tr>`);
                }
            }
            else {
                const name = self.getProperty(propDef, "@NAME");

                if (name)
                    propDef["@NAME"] = `${bankName}_${name}`;

                const [propDefHtml, propDefScript] = self.buildPropDefElement(propDef, groupMapRoot || bankMapRoot, bankName);

                html.push(propDefHtml);

                if (propDefScript.length > 0)
                    script.push(propDefScript);
            }
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
        const mapRoot = self.getProperty(section, "@MAPROOT");
        const sectionMapRoot = `${configRoot}${mapRoot ? `.${mapRoot}` : ""}`;
        const sectionName = self.getProperty(section, "@NAME");
        const html = [];
        const script = [];
        const definitions = [];

        let naturalOrder = 100000;
        let propDefsStarted = false;
        let groupMapRoot = undefined;
        self._configRoot = configRoot;

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
            const elementName = self.getProperty(definition, "@NAME");

            if (elementName && self.getType(definition, -1) !== PropDefType.HEADER) {
                definition["@SECTIONNAME"] = `${sectionName}Section`;
                definition["@NAME"] = `${definition["@SECTIONNAME"]}_${elementName}`;
            }

            const order = parseInt(self.getProperty(definition, "@ORDER"), 10);

            definition.order = isNaN(order) ?
                definitions.length > 0 ? naturalOrder++ : 0 :
                order;

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
                        const [bankHtml, bankScript] = self.buildBankElement(definition, sectionMapRoot);

                        closePropDefs();
                        html.push(bankHtml);

                        if (bankScript.length > 0)
                            script.push(bankScript);
                    }
                    break;
                case "PROPDEF":
                    {
                        if (self.getType(definition) === PropDefType.HEADER) {
                            if (self.elementIsEnabled(definition)) {
                                closePropDefs();

                                const headerMapRoot = self.getProperty(definition, "@MAPROOT");

                                groupMapRoot = headerMapRoot ?
                                    (headerMapRoot.startsWith(".") || headerMapRoot.startsWith("[") ? `${sectionMapRoot}${headerMapRoot}` : `${self._configRoot}.${headerMapRoot}`) :
                                    undefined;

                                html.push(`<tr><th class="smb-header${definition.order > 0 ? "-section-row" : ""}"><div class="smb-header">${self.getAlias(definition) || ""}</div></th></tr>`);
                            }
                        }
                        else {
                            const [propDefHtml, propDefScript] = self.buildPropDefElement(definition, groupMapRoot || sectionMapRoot);

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
