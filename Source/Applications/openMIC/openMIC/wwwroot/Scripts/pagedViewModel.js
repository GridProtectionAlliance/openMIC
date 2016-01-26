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

// Define paged view model class
function PagedViewModel() {
    var self = this;

    // Fields
    self.pageRecords = ko.observableArray();
    self.currentRecord = ko.observable();
    self.recordCount = ko.observable(0);
    self.pageCount = ko.observable(1);
    self.modelName = "{name}";
    self.labelField = "{name}";
    self.identityField = "{id}";
    self.defaultSortField = "{id}";
    self.sortField = ko.observable(self.defaultSortField);
    self.sortAscending = ko.observable(true);
    self.editMode = ko.observable(true);
    self._currentPageSize = ko.observable(1);
    self._currentPage = ko.observable(0);
    self._executingCalculation = false;

    // Properties
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

    self.currentPage = ko.pureComputed({
        read: self._currentPage,
        write: function (value) {
            if (value < 1)
                value = 1;
            else if (value > self.totalPages())
                value = self.totalPages();

            if (value !== self._currentPage()) {
                self._currentPage(value);
                self.queryPageRecords();
            }
        },
        owner: self
    });

    self.totalPages = ko.pureComputed(function () {
        return Math.max(Math.ceil(self.recordCount() / self.currentPageSize()), 1);
    });

    self.onFirstPage = ko.pureComputed(function () {
        return self.currentPage() <= 1;
    });

    self.onLastPage = ko.pureComputed(function () {
        return self.currentPage() >= self.totalPages();
    });

    // Delegates
    self.queryRecordCount = function () { };
    self.queryRecords = function (/* sortField, ascending, page, pageSize */) { };
    self.deleteRecord = function (/* id */) { };
    self.newRecord = function () { };
    self.addNewRecord = function (/* record */) { };
    self.updateRecord = function (/* record */) { };

    // Setters needed to assign delegate properties, "'cause Javascript"
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

    // Methods
    self.initialize = function () {
        // Restore any previous sort order
        var lastSortField = Cookies.get(self.modelName + "!LastSortField");
        var lastSortAscending = Cookies.get(self.modelName + "!LastSortAscending");

        if (lastSortField === undefined)
            self.sortField(self.defaultSortField);
        else
            self.sortField(lastSortField);

        if (lastSortAscending === undefined)
            self.sortAscending(true);
        else
            self.sortAscending(lastSortAscending == "true");

        if (hubIsConnected) {
            // Query total record count
            self.queryRecordCount().done(function (total) {
                // Update record count observable
                self.recordCount(total);

                // Force page refresh when record count has been updated
                var currentPage = self.currentPage();
                self._currentPage(0);
                self.currentPage(currentPage);
            });
        }
    }

    self.calculatePageSize = function () {
        if (self._executingCalculation)
            return;

        self._executingCalculation = true;

        // Calculate total number of table rows that will fit within current page height
        var remainingHeight = calculateRemainingBodyHeight() -
            $("#contentWell").paddingHeight() -
            $("#responsiveTableDiv").paddingHeight() -
            $("#recordsTable").paddingHeight() -
            $("#pageControlsRow").outerHeight(true);

        var pageSize = truncateNumber(remainingHeight / $("#recordRow").outerHeight(true));

        if (!pageSize || isNaN(pageSize) || !isFinite(pageSize) || pageSize < 1)
            pageSize = 1;

        if (pageSize !== self.currentPageSize()) {
            var currentPage = self.currentPage();

            // Updating page size will validate current page number
            self.currentPageSize(pageSize);

            // Requery data for page unless current page was dynamically changed
            // and has reloaded data already
            if (currentPage === self.currentPage())
                self.queryPageRecords();
        }

        self._executingCalculation = false;
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
        if (hubIsConnected)
            self.queryRecords(self.sortField(), self.sortAscending(), self.currentPage(), self.currentPageSize()).done(function (records) {
                self.pageRecords.removeAll();
                self.pageRecords(records);
                refreshHubDependentControlState();
                $("[id='recordRow']").css("visibility", "visible");
                $("#loadingDataLabel").hide();
            }).fail(function (error) {
                showErrorMessage(error);
            });
    }

    self.removePageRecord = function (record) {
        if (hubIsConnected && confirm("Are you sure you want to delete \"" + record[self.labelField] + "\"?")) {
            self.deleteRecord(record[self.identityField]).done(function () {
                self.pageRecords.remove(record);
                self.initialize();
                showInfoMessage("Record deleted...");
            }).fail(function (error) {
                showErrorMessage(error);
            });
        }
    }

    self.editPageRecord = function (record) {
        self.editMode(true);
        self.currentRecord(ko.mapping.fromJS(record));
        $("#addNewEditDialog").modal("show");
    }

    self.addPageRecord = function () {
        self.editMode(false);
        self.newRecord().done(function (emptyRecord) {
            self.currentRecord(ko.mapping.fromJS(emptyRecord));
            $("#addNewEditDialog").modal("show");
        });
    }

    self.savePageRecord = function () {
        if (self.editMode())
            self.saveEditedRecord();
        else
            self.saveNewRecord();
    }

    self.saveEditedRecord = function () {
        self.updateRecord(ko.mapping.toJS(self.currentRecord())).done(function () {
            self.initialize();
            showInfoMessage("Saved updated record...");
        }).fail(function (error) {
            showErrorMessage(error);
        });
    }

    self.saveNewRecord = function () {
        self.addNewRecord(ko.mapping.toJS(self.currentRecord())).done(function () {
            self.initialize();
            showInfoMessage("Saved new record...");
        }).fail(function (error) {
            showErrorMessage(error);
        });
    }
};

// Define page scoped view model instance
var viewModel = new PagedViewModel();

function onHubConnected() {
    viewModel.initialize();
}

$(function () {
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

    viewModel.calculatePageSize();
    ko.applyBindings(viewModel);
});

$(window).resize(function () {
    viewModel.calculatePageSize();
});