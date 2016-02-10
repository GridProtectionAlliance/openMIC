//******************************************************************************************************
//  pagedViewModel.js - Gbtc
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
//  01/23/2016 - J. Ritchie Carroll
//       Generated original version of source code.
//
//******************************************************************************************************

// Paged view model base class scripts
"use strict";

var RecordMode = {
    View: 0,
    Edit: 1,
    AddNew: 2
}

// Define paged view model class
function PagedViewModel() {
    const self = this;

    // Configuration fields
    self.labelField = "{name}";                             // Field name that best represents row for delete confirmation
    self.primaryKeyFields = ["{id}"];                       // Primary key field names array
    self.defaultSortField = "{id}";                         // Default sort field
    self.initialFocusField = "";                            // Initial add/edit field with focus
    self.modelName = "{name}";                              // Name of model used for cookie names, defaults to page title

    // Observable fields
    self.pageRecords = ko.observableArray();                // Records queried for current page
    self.recordCount = ko.observable(0);                    // Queried record count
    self.sortField = ko.observable(self.defaultSortField);  // Current sort field, persisted per model
    self.sortAscending = ko.observable(true);               // Current sort ascending flag, persisted per model
    self.unassignedFieldCount = ko.observable(0);           // Number of bound fields with missing data
    self.dataHubIsConnected = ko.observable(false);         // Data hub connected observable flag - externally managed
    self.errors = ko.validation.group([ko.observable(0)]);  // Validation errors array

    // Internal fields
    self._currentPageSize = ko.observable(1);
    self._currentPage = ko.observable(0);
    self._currentRecord = ko.observable();
    self._recordMode = ko.observable(RecordMode.View);
    self._isDirty = false;
    self._columnWidths = [];

    // Properties

    // Gets or sets the number records to display on the page
    self.currentPageSize = ko.pureComputed({
        read: self._currentPageSize,
        write: function (value) {
            if (value < 1)
                value = 1;

            if (value !== self._currentPageSize()) {
                self._currentPageSize(value);

                // Validate current page after page size change
                if (self.currentPage())
                    self.currentPage(self.currentPage());
            }
        },
        owner: self
    });

    // Gets or sets the current page number
    self.currentPage = ko.pureComputed({
        read: self._currentPage,
        write: function (value) {
            if (value < 1)
                value = 1;
            else if (value > self.totalPages())
                value = self.totalPages();

            // Requery records for page when current page changes
            if (value !== self._currentPage()) {
                self._currentPage(value);
                self.queryPageRecords();
            }
        },
        owner: self
    });

    // Gets or sets current record on page
    self.currentRecord = ko.pureComputed({
        read: self._currentRecord,
        write: function(value) {
            self._currentRecord(value);
            self.setDirtyFlag(false);
            $(self).trigger("currentRecordChanged");
            self.applyValidationParameters();

            // Watch for changes to fields in current record
            ko.watch(self._currentRecord(), function (parents, child, item) {
                self.setDirtyFlag(true, child);
            });
        },
        owner: self
    });

    // Gets or sets the record mode, i.e., view, edit or add new
    self.recordMode = ko.pureComputed({
        read: self._recordMode,
        write: function (newMode) {
            const oldMode = self._recordMode();

            if (newMode !== oldMode) {
                self._recordMode(newMode);

                if (oldMode === RecordMode.View && newMode === RecordMode.Edit)
                    self.setFocusOnInitialField();

                $(self).trigger("recordModeChanged", [oldMode, newMode]);
            }
        },
        owner: self
    });

    // Gets total number pages based on total record count and current page size
    self.totalPages = ko.pureComputed(function () {
        return Math.max(Math.ceil(self.recordCount() / self.currentPageSize()), 1);
    });

    // Gets flag that determines if current page is first page
    self.onFirstPage = ko.pureComputed(function () {
        return self.currentPage() <= 1;
    });

    // Gets flag that determines if current page is last page
    self.onLastPage = ko.pureComputed(function () {
        return self.currentPage() >= self.totalPages();
    });

    // Return validation errors, i.e., non-required field errors
    self.validationErrors = ko.pureComputed(function() {
        return self.errors().length - self.unassignedFieldCount();
    }).extend({ notify: "always" });

    // Delegates
    self.queryRecordCount = function () { };
    self.queryRecords = function (/* sortField, ascending, page, pageSize */) { };
    self.deleteRecord = function (/* keyValues[] */) { };
    self.newRecord = function () { };
    self.addNewRecord = function (/* record */) { };
    self.updateRecord = function (/* record */) { };
    self.applyValidationParameters = function () { };

    // Setters needed to assign delegate properties, 'cause Javascript
    self.setQueryRecordCount = function (queryRecordCountFunction) {
        self.queryRecordCount = queryRecordCountFunction;
    }

    self.setQueryRecords = function (queryRecordsFunction) {
        self.queryRecords = queryRecordsFunction;
    }

    self.setDeleteRecord = function (deleteRecordFunction) {
        self.deleteRecord = deleteRecordFunction;
    }

    self.setNewRecord = function (newRecordFunction) {
        self.newRecord = newRecordFunction;
    }

    self.setAddNewRecord = function (addNewRecordFunction) {
        self.addNewRecord = addNewRecordFunction;
    }

    self.setUpdateRecord = function (updateRecordFunction) {
        self.updateRecord = updateRecordFunction;
    }

    self.setApplyValidationParameters = function (applyValidationParametersFunction) {
        self.applyValidationParameters = applyValidationParametersFunction;
    }

    // Methods
    self.initialize = function () {
        // Restore any previous sort order
        const lastSortField = Cookies.get(self.modelName + "!LastSortField");
        const lastSortAscending = Cookies.get(self.modelName + "!LastSortAscending");

        if (lastSortField === undefined)
            self.sortField(self.defaultSortField);
        else
            self.sortField(lastSortField);

        if (lastSortAscending === undefined)
            self.sortAscending(true);
        else
            self.sortAscending(lastSortAscending === "true");

        if (self.dataHubIsConnected()) {
            // Query total record count
            self.queryRecordCount().done(function (total) {
                // Update record count observable
                self.recordCount(total);

                // Force page refresh when record count has been updated
                const currentPage = self.currentPage();
                self._currentPage(0);
                self.currentPage(currentPage);
            });            

            // Initialize current record with an empty row
            self.newRecord().done(function (emptyRecord) {
                self.currentRecord(self.deriveObservableRecord(emptyRecord));
            });
        }

        // Initialize column widths array
        self._columnWidths = [];
        const columns = $("#recordRow").find("td");

        for (let i = 0; i < columns.length; i++) {
            self._columnWidths.push($(columns[i]).width());
        }
    }

    self.calculatePageSize = function (forceRefresh) {
        // Calculate total number of table rows that will fit within current page height
        const remainingHeight = calculateRemainingBodyHeight() -
            $("#contentWell").paddingHeight() -
            $("#responsiveTableDiv").paddingHeight() -
            $("#recordsTable").paddingHeight() -
            $("#pageControlsRow").outerHeight(true);

        // Estimate page size based on height of first record row
        const firstRow = $("#recordRow");
        var pageSize = (remainingHeight / firstRow.outerHeight(true)).truncate();

        if (!pageSize || isNaN(pageSize) || !isFinite(pageSize) || pageSize < 1)
            pageSize = 1;

        if (forceRefresh === undefined)
            forceRefresh = false;

        // Check for dynamic Bootstrap column resizing, in which case we need to refresh data
        // for cases where binding may be truncating data lengths, see $.fn.truncateToWidth
        const columns = firstRow.find("td");
        var columnWidth;

        for (let i = 0; i < columns.length; i++) {
            columnWidth = $(columns[i]).width();

            if (self._columnWidths[i] !== columnWidth) {
                self._columnWidths[i] = columnWidth;
                forceRefresh = true;
            }
        }

        if (pageSize !== self.currentPageSize() || forceRefresh) {
            const currentPage = self.currentPage();

            // Updating page size will validate current page number
            self.currentPageSize(pageSize);

            // Requery data for page unless current page was dynamically changed
            // by page size update and has reloaded data already
            if (currentPage === self.currentPage())
                self.queryPageRecords();
        }
    }

    self.refreshValidationErrors = function () {
        // Force re-evaluation of validation errors property
        self.unassignedFieldCount.valueHasMutated();

        // Make sure any initial validation error messages are visible
        self.errors.showAllMessages();
    }

    // Convert observable object to a simple Javascript record
    self.deriveJSRecord = function () {
        const currentRecord = self.currentRecord();

        // Allow customization of Javascript record
        $(self).trigger("derivingJSRecord");

        return ko.mapping.toJS(currentRecord);
    }

    // Convert simple Javascript record to an observable object
    self.deriveObservableRecord = function (record) {
        const observableRecord = ko.mapping.fromJS(record);

        // Allow customization of new observable record
        $(self).trigger("derivingObservableRecord", [observableRecord]);

        // Apply validation binding to current observable record
        self.errors = ko.validation.group(observableRecord);
        self.refreshValidationErrors();

        return observableRecord;
    }

    self.setDirtyFlag = function (value, target) {
        if (value === undefined)
            value = true;

        self._isDirty = value;

        // Derive unassigned field count based on existence of Bootstrap "has-error" class
        self.unassignedFieldCount($("#addNewEditDialog div.form-group.has-error").length);

        if (value)
            $(self).trigger("currentRecordUpdated", [target]);
    }

    self.setFocusOnInitialField = function () {
        if (!isEmpty(self.initialFocusField))
            $("#" + self.initialFocusField).focus();
    }

    self.nextPage = function () {
        if (self.currentPage() < self.totalPages())
            self.currentPage(self.currentPage() + 1);
    }

    self.previousPage = function () {
        if (self.currentPage() > 1)
            self.currentPage(self.currentPage() - 1);
    }

    self.updateSortOrder = function (fieldName, ascending) {
        self.sortField(fieldName);
        self.sortAscending(ascending);
        self.queryPageRecords();

        // Save last sort order
        Cookies.set(self.modelName + "!LastSortField", self.sortField(), { expires: 365 });
        Cookies.set(self.modelName + "!LastSortAscending", self.sortAscending().toString(), { expires: 365 });
    }

    self.isSortOrder = function (fieldName, ascending) {
        return self.sortField().toUpperCase() === fieldName.toUpperCase() && self.sortAscending() === ascending;
    }

    self.queryPageRecords = function () {
        if (self.dataHubIsConnected())
            self.queryRecords(self.sortField(), self.sortAscending(), self.currentPage(), self.currentPageSize()).done(function (records) {
                self.pageRecords.removeAll();
                self.pageRecords(records);
                refreshHubDependentControlState();
                $("[id='recordRow']").css("visibility", "visible");
                $("#loadingDataLabel").hide();

                // Validate proper page size after any record refresh
                setTimeout(self.calculatePageSize, 150);
            }).fail(function (error) {
                showErrorMessage(error);
            });
    }

    self.removePageRecord = function (record) {
        if (self.dataHubIsConnected() && confirm("Are you sure you want to delete \"" + record[self.labelField] + "\"?")) {
            const keyValues = [];

            for (var i = 0; i < self.primaryKeyFields.length; i++) {
                keyValues.push(record[self.primaryKeyFields[i]]);
            }

            self.deleteRecord(keyValues).done(function () {
                self.pageRecords.remove(record);
                self.initialize();
                $(self).trigger("recordDeleted", [record]);
                showInfoMessage("Deleted record...");
            }).fail(function (error) {
                showErrorMessage(error);
            });
        }
    }

    self.savePageRecord = function () {
        switch (self.recordMode()) {
            case RecordMode.Edit:
                self.saveEditedRecord();
                break;
            case RecordMode.AddNew:
                self.saveNewRecord();
                break;
        }
    }

    self.saveEditedRecord = function () {
        if (self.dataHubIsConnected()) {
            const record = self.deriveJSRecord();

            self.updateRecord(record).done(function () {
                self.initialize();
                $(self).trigger("recordSaved", [record, false]);
                showInfoMessage("Saved updated record...");
            }).fail(function (error) {
                showErrorMessage(error);
            });
        }
    }

    self.saveNewRecord = function () {
        if (self.dataHubIsConnected()) { 
            const record = self.deriveJSRecord();

            self.addNewRecord(record).done(function () {
                self.initialize();
                $(self).trigger("recordSaved", [record, true]);
                showInfoMessage("Saved new record...");
            }).fail(function (error) {
                showErrorMessage(error);
            });
        }
    }

    self.viewPageRecord = function (record) {
        self.recordMode(RecordMode.View);
        self.currentRecord(self.deriveObservableRecord(record));
        $("#addNewEditDialog").modal("show");
    }

    self.editPageRecord = function (record) {
        self.recordMode(RecordMode.Edit);
        self.currentRecord(self.deriveObservableRecord(record));
        $("#addNewEditDialog").modal("show");
    }

    self.addPageRecord = function () {
        if (self.dataHubIsConnected()) {
            self.recordMode(RecordMode.AddNew);
            self.newRecord().done(function (emptyRecord) {
                self.currentRecord(self.deriveObservableRecord(emptyRecord));
                $("#addNewEditDialog").modal("show");
            });
        }
    }

    self.cancelPageRecord = function () {
        if (!self._isDirty || confirm("Are you sure you want to discard unsaved changes?"))
            $("#addNewEditDialog").modal("hide");
    }
};

// Define page scoped view model instance
var viewModel = new PagedViewModel();

(function ($, viewPort) {
    $("#bodyContainer").addClass("fill-height");

    $("#titleText").html("Records: <span data-bind='text: recordCount'>calculating...</span>");

    $("#firstPageButton").click(function () {
        viewModel.currentPage(1);
    });

    $("#previousPageButton").click(function () {
        viewModel.previousPage();
    });

    $("#nextPageButton").click(function () {
        viewModel.nextPage();
    });

    $("#lastPageButton").click(function () {
        viewModel.currentPage(viewModel.totalPages());
    });

    $(window).on("hubConnected", function (event) {
        viewModel.dataHubIsConnected(true);
        viewModel.initialize();
    });

    $(window).on("hubDisconnected", function (event) {
        viewModel.dataHubIsConnected(false);
    });

    $(window).on("messageVisibiltyChanged", function (event) {
        viewModel.calculatePageSize();
    });

    $("#addNewEditDialog").on("shown.bs.modal", function () {
        viewModel.setFocusOnInitialField();
    });

    $(window).resize(
        viewPort.changed(function () {
            viewModel.calculatePageSize();
    }));

    viewModel.calculatePageSize();

    ko.validation.rules.pattern.message = "Invalid";
    
    ko.validation.init({
        registerExtenders: true,
        messagesOnModified: true,
        insertMessages: true,
        parseInputAttributes: true,
        allowHtmlMessages: true,
        messageTemplate: null,
        grouping: { deep: true, observable: true, live: true }
    }, true);

    ko.applyBindings(viewModel);

})(jQuery, ResponsiveBootstrapToolkit);
