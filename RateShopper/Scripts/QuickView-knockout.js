var quickViewRowObject;
var sortOrder = "ASC";
var sortBy = "CreatedDateTime";
var reportsQuickViewId = 0;
var isSortingApplied = false;
$(document).ready(QuickViewBindGridReady);

function QuickViewBindGridReady() {
    $("#qvlocation .hidden li:first").removeClass("selected").addClass("selected");
    $("#qvuser .hidden li:first").removeClass("selected").addClass("selected");
    var loggedInUserId = $('#LoggedInUserId').val();
    if (loggedInUserId > 0) {
        $("#qvuser ul.hidden.drop-down1 li.selected").removeClass('selected');
        $("#qvuser ul.hidden.drop-down1 li[value='" + loggedInUserId + "']").addClass('selected').closest('#qvuser').find('li').eq(0).attr({ 'value': ($("#qvuser ul.hidden.drop-down1 li[value='" + loggedInUserId + "']").attr('value')) }).text($("#qvuser ul.hidden.drop-down1 li[value='" + loggedInUserId + "']").text());
    }
    DisableUserDD();
    FetchQuickViews();

    $("#qvuser .hidden li").click(function () {
        locationBrandId = $("#qvlocation .hidden li.selected").attr("value");
        userId = $(this).attr("value");
        GetQuickViews(locationBrandId, userId);
    });

    $("#qvlocation .hidden li").click(function () {
        locationBrandId = $(this).attr("value");
        userId = $("#qvuser .hidden li.selected").attr("value");
        GetQuickViews(locationBrandId, userId);
    });
    $('#quickViewReport').hide();
    $("#btnRefreshQuickViewGrid").click(FetchQuickViews);

    $('#toggleList').click(function () {
        if ($('#quickViewList').is(':visible')) {
            $('#collapseList').attr('src', 'images/expand.png');
        }
        else {
            $('#collapseList').attr('src', 'images/Search-collapse.png');
        }
        $('#quickViewList').slideToggle(400);
    });
    $('#toggleResult').click(function () {
        if ($('#quickViewReport').is(':visible')) {
            $('#collapseReport').attr('src', 'images/expand.png');
        }
        else {
            $('#collapseReport').attr('src', 'images/Search-collapse.png');
        }
        $('#quickViewReport').slideToggle(400);
    });

}



function QuickViewData(data) {
    var date = new Date(parseInt(data.CreatedDateTime.replace("/Date(", "").replace(")/", "")));

    this.ID = data.ID;
    this.Status = data.Status;
    this.CreatedDateTime = ('0' + (date.getMonth() + 1)).slice(-2) + "/" + ('0' + (date.getDate())).slice(-2) + "/" + date.getFullYear();
    this.Sources = data.Sources;
    this.LocationBrands = computeData(data.LocationsBrandIDs, "locations");
    this.Dates = data.StartDate + "..." + data.EndDate;
    this.RentalLengths = computeData(data.RentalLengthIDs, "lengths");
    this.CarClasses = computeData(data.TrackingCarClassIds, "carClass");
    this.CarClassesIds = data.CarClassesIDs;
    this.Competitors = data.Competitors;
    if (data.LastRunDateTime != null) {
        var lastruntime = new Date(parseInt(data.LastRunDateTime.replace("/Date(", "").replace(")/", "")));
        this.LastRunTime = ('0' + (lastruntime.getMonth() + 1)).slice(-2) + "/" + ('0' + (lastruntime.getDate())).slice(-2) + "/" + lastruntime.getFullYear() + " " + ('0' + lastruntime.getHours()).slice(-2) + ":" + ('0' + lastruntime.getMinutes()).slice(-2);
    }
    else {
        this.LastRunTime = "";
    }
    if (data.NextRunDateTime != null) {
        var nextruntime = new Date(parseInt(data.NextRunDateTime.replace("/Date(", "").replace(")/", "")));
        this.NextRunTime = ('0' + (nextruntime.getMonth() + 1)).slice(-2) + "/" + ('0' + (nextruntime.getDate())).slice(-2) + "/" + nextruntime.getFullYear() + " " + ('0' + nextruntime.getHours()).slice(-2) + ":" + ('0' + nextruntime.getMinutes()).slice(-2);
    }
    else {
        this.NextRunTime = "";
    }
    this.LocationBrandIds = data.LocationsBrandIDs;
    this.User = data.CreatedByUserName;
    this.SearchSummaryId = data.SearchSummaryId;
    this.ChildSummaryId = data.ChildSummaryId;
    this.IsReportReviewed = data.IsReportReviewed;
}

function RentalLengthsInfo(data) {
    var self = this;
    self.RentalLength = data.RentalLength;
    self.RentalLengthId = data.RentalLengthId;
}
function QuickViewReport(data) {
    var self = this;
    self.Day = data.Date;
    self.quickViewResult = ko.observableArray($.map(data.QuickViewResult, function (item) { return new QuickViewRow(item); }));
}
function QuickViewRow(data) {
    var self = this;
    self.ResultId = data.ID;
    self.SearchSummaryId = data.SearchSummaryId;
    self.QuickViewId = data.QuickViewId;
    self.RentalId = data.RentalLengthId;
    self.IsMovedUp = ko.computed(function () {
        if (data.IsMovedUp == null) {
            return "";
        }
        else if (data.IsMovedUp == true) {
            return "true";
        }
        else if (data.IsMovedUp == false) {
            return "false";
        }
    });

    self.PositionCss = ko.computed(function () {
        if (data.IsPositionChange == true) {
            return "spn_P";
        }
        else {
            return "";
        }
    });

    self.IsChanged = ko.computed(function () {
        if (data.IsMovedUp == null) {
            return "false";
        }
        else if (data.IsMovedUp == true || data.IsMovedUp == false) {
            return "true";
        }
    });
    self.ColorCode = ko.computed(function () {
        if (!data.IsReviewed) {
            if (data.IsMovedUp == null) {
                return "";
            }
            else if (data.IsMovedUp == true) {
                //one of competiotor rates moved up
                return "prevent-grey blue-background";
            }
            else if (data.IsMovedUp == false) {
                //one of competiotor rates moved down
                return "prevent-grey red-background";
            }
        }
        else {
            if (data.IsMovedUp == null) {
                return "quickCellReviewed";
            }
            else if (data.IsMovedUp == true) {
                //one of competiotor rates moved up
                return "prevent-grey blue-background quickCellReviewed";
            }
            else if (data.IsMovedUp == false) {
                //one of competiotor rates moved down
                return "prevent-grey red-background quickCellReviewed";
            }
        }
    });
    self.IsReviewed = ko.computed(function () {
        if (data.IsReviewed == null) {
            return "";
        }
        else if (data.IsReviewed == true) {
            return "true";
        }
        else if (data.IsReviewed == false) {
            return "false";
        }
    });
    self.ResultDate = data.FormattedDate;
    self.ViewDailyRecord = function (rowClick) {
        showQuickViewShop(rowClick.SearchSummaryId, rowClick.IsChanged(), rowClick.RentalId, rowClick.ResultDate);
    }
}

var FetchQuickViews = function () {
    var locationBrandId = $("#qvlocation .hidden li.selected").attr("value");
    var userId = $("#qvuser .hidden li.selected").attr("value");
    GetQuickViews(locationBrandId, userId);
}

var EditQuickView = function (fromSetAndRun) {    
    var quickView = quickViewRowObject;
    var selectedQuickViewCompetitors = GetSelectedQuickViewCompetitors(fromSetAndRun, quickView.LocationBrandIds, quickView.SearchSummaryId);
    var selectedQuickViewCarClasses = GetSelectedQuickViewCarClasses(fromSetAndRun, quickView.LocationBrandIds, quickView.SearchSummaryId, quickView.CarClassesIds, quickView.TrackingCarClassIds);
    if (ValidateQuickViewGroup()) {
        if (selectedQuickViewCompetitors != "" && selectedQuickViewCarClasses != "") {
            var selectedDate = ValidateScheduleTime(fromSetAndRun);
            var selectedControlId = null;
            if (!fromSetAndRun) {
                selectedControlId = $(".quickviewschedule input[type='radio']:checked").attr("id");
            }
            else {
                selectedControlId = "rdosch_0";
            }
            if (selectedDate != "") {
                var dateTimeStamp;
                //selected run now option
                if (selectedDate == "0") {
                    dateTimeStamp = "0";
                }
                else {
                    dateTimeStamp = GetUTCString(selectedDate);
                }

                var GroupData = SaveQuickViewGroupData();

                var ajaxURl = '/Search/SaveQuickView';
                if (quickURLSettings != undefined && quickURLSettings != '') {
                    ajaxURl = quickURLSettings.SaveQuickView;
                }

                $(".loader_container_quickviewgrid").show();
                $.ajax({
                    url: ajaxURl,
                    type: "POST",
                    data: JSON.stringify({ 'quickViewId': quickView.ID, 'userId': $('#LoggedInUserId').val(), 'scheduleTime': dateTimeStamp, 'quickViewCompetitors': selectedQuickViewCompetitors, 'clientDate': GetUTCString(new Date()), 'scheduleControlId': selectedControlId, 'carClassIds': selectedQuickViewCarClasses, 'quickViewGroupData': GroupData }),
                    contentType: "application/json;charset=utf-8",
                    dataType: 'json',
                    success: function (data) {
                        $(".loader_container_quickviewgrid").hide();
                        if (data.status) {
                            if (!fromSetAndRun) {
                                quickView.Competitors = $("#selectedCompetitors").val();
                                $("#qv_competitor_" + quickView.ID).html(quickView.Competitors);
                                if (selectedDate != "0") {
                                    quickView.NextRunTime = ('0' + (selectedDate.getMonth() + 1)).slice(-2) + "/" + ('0' + (selectedDate.getDate())).slice(-2) + "/" + selectedDate.getFullYear() + " " + ('0' + selectedDate.getHours()).slice(-2) + ":" + ('0' + selectedDate.getMinutes()).slice(-2);
                                    $("#qv_nextrun_" + quickView.ID).html(quickView.NextRunTime);
                                    DisplayQuickViewGridMessages("QuickView scheduled successfully", false);
                                }
                                else {
                                    DisableQuickViewActionButtons(quickView.ID);
                                    var lastruntime = new Date();
                                    quickView.LastRunTime = ('0' + (lastruntime.getMonth() + 1)).slice(-2) + "/" + ('0' + (lastruntime.getDate())).slice(-2) + "/" + lastruntime.getFullYear() + " " + ('0' + lastruntime.getHours()).slice(-2) + ":" + ('0' + lastruntime.getMinutes()).slice(-2);
                                    $("#qv_lastrun_" + quickView.ID).html(quickView.LastRunTime);
                                    $("#qv_nextrun_" + quickView.ID).html('');
                                    DisplayQuickViewGridMessages("QuickView run successfully", false);
                                }
                                $(".close-quick-view-popup").click();

                            }
                            else {
                                DisableQuickViewActionButtons(quickView.ID);
                                var lastruntime = new Date();
                                quickView.LastRunTime = ('0' + (lastruntime.getMonth() + 1)).slice(-2) + "/" + ('0' + (lastruntime.getDate())).slice(-2) + "/" + lastruntime.getFullYear() + " " + ('0' + lastruntime.getHours()).slice(-2) + ":" + ('0' + lastruntime.getMinutes()).slice(-2);
                                $("#qv_lastrun_" + quickView.ID).html(quickView.LastRunTime);
                                $("#qv_nextrun_" + quickView.ID).html('');
                                DisplayQuickViewGridMessages("QuickView run successfully", false);
                            }
                            quickView.Status = data.quickViewRow.Status;
                            quickView.CarClasses = computeData(selectedQuickViewCarClasses, "carClass");
                            $("#qv_carclasses_" + quickView.ID).html(quickView.CarClasses);
                            $("#qv_status_" + quickView.ID).html(quickView.Status);
                            UpdateEditedQuickView(quickView);
                            if (isSortingApplied) {
                                searchViewModel.SortQuickView();
                            }
                            if (quickView.ChildSummaryId != null && quickView.ChildSummaryId > 0) {
                                UpdateSearchSummaryStatus(quickView.ChildSummaryId, fromSetAndRun);
                            }
                        }
                    },
                    error: function (e) {
                        $(".loader_container_quickviewgrid").hide();
                        console.log(e.message);
                    }
                });
            }
        }
    }
}

var RerunQuickView = function (quickView) {
    quickViewRowObject = quickView;
    EditQuickView(true);
}

var RescheduleQuickView = function (quickView) {
    quickViewRowObject = quickView;
    ResetQuickViewSchedulePopup();
    GetQuickViewCompetitors(quickViewRowObject.LocationBrandIds, quickView.SearchSummaryId, quickView.CarClassesIds, quickView.TrackingCarClassIds);
    $(".quickview-schedule-modal").removeClass('hidden').show();
    $(".modal-backdrop").removeClass('hidden').show();
}

function DeleteQuickView(quickView) {
    if (typeof (quickView) == 'undefined') {
        quickView = this;
    }
    DeleteQuickViewFromDB(quickView);
}

var ShowQuickDailyView = function (quickView) {
    showQuickViewShop(quickView.ChildSummaryId);
}

var ShowQuickViewReport = function (quickView) {
    reportsQuickViewId = quickView.ID;
    getQuickViewResults(quickView.ID, quickView.ChildSummaryId);
}

var UpdateEditedQuickView = function (quickView) {
    ko.utils.arrayForEach(searchViewModel.quickViewData(), function (currentQuickView) {
        if (currentQuickView.ID == quickView.ID) {
            currentQuickView.Competitors = quickView.Code;
            currentQuickView.LastRunTime = quickView.LastRunTime;
            currentQuickView.NextRunTime = quickView.NextRunTime;
            currentQuickView.Status = quickView.Status;
            currentQuickView.ChildSummaryId = quickView.ChildSummaryId;
            currentQuickView.SearchSummaryId = quickView.SearchSummaryId;
        }
    });
}


function ApplySorting(control, sortByProperty) {
    isSortingApplied = true;
    var prevSortOrder;
    if ($(control).hasClass("Asc")) {
        prevSortOrder = "Asc";
    }
    else {
        prevSortOrder = "Desc";
    }
    sortBy = sortByProperty;

    if (prevSortOrder == "Asc") {
        $(control).removeClass("Asc").addClass("Desc");
        sortOrder = "DESC";
    }
    else {
        $(control).removeClass("Desc").addClass("Asc");
        sortOrder = "ASC";
    }

    searchViewModel.SortQuickView();
}

function SortQuickView() {
    var prop = sortBy;
    var ascSort = function (a, b) {
        return a[prop] < b[prop] ? -1 : a[prop] > b[prop] ? 1 : a[prop] == b[prop] ? 0 : 0;
    };
    var descSort = function (a, b) {
        return a[prop] > b[prop] ? -1 : a[prop] < b[prop] ? 1 : a[prop] == b[prop] ? 0 : 0;
    };

    var sortFunc;
    if (sortOrder == "DESC") {
        sortFunc = descSort;
    }
    else {
        sortFunc = ascSort;
    }
    searchViewModel.quickViewData.sort(sortFunc);
}

function DisableQuickViewActionButtons(quickViewRowId) {
    $("#qv_row_" + quickViewRowId + " ul li .enable").hide();
    $("#qv_row_" + quickViewRowId + " ul li .disable").show();
}

function getQuickViewResults(quickViewId, parentSearchSummaryId) {
    var ajaxURl = '/RateShopper/QuickView/FetchQuickViewResult/';
    if (quickURLSettings != undefined && quickURLSettings != '') {
        ajaxURl = quickURLSettings.GetQuickViewReport;
    }
    $.ajax({
        type: "GET",
        url: ajaxURl,
        data: { 'quickViewId': quickViewId, 'searchSummaryId': parentSearchSummaryId },
        dataType: 'json',
        async: true,
        success: function (data) {
            if (data.Status) {
                var finalData = data.reportData;
                var rentalLengths = $.map(finalData.LORs, function (item) { return new RentalLengthsInfo(item) });
                searchViewModel.quickViewRentalLengths(rentalLengths);

                var quickViewReData = $.map(finalData.Dates, function (item) { return new QuickViewReport(item) })
                searchViewModel.quickViewReportData(quickViewReData);
                $('#collapseReport').attr('src', 'images/Search-collapse.png');
                $('#quickViewReport').show();
                $('#noReportRecord').hide();
                $('#quickViewReportGrid').show();
                //console.log(rentalLengths);
                //console.log(quickViewReData);
            }
            else {
                $('#collapseReport').attr('src', 'images/Search-collapse.png');
                $('#quickViewReport').show();
                $('#quickViewReportGrid').hide();
                searchViewModel.quickViewRentalLengths();
                $('#noReportRecord').show();
            }
        },
        error: function (error) {

        }
    });
}


var DisplayQuickViewGridMessages = function (message, isError) {
    $("#lblQuickViewGridMessage").html(message).show().css("color", isError ? "red" : "green");
    if (!isError) {
        setTimeout(function () { $("#lblQuickViewGridMessage").hide(); }, 3000);
    }
}

var ShowHideQuickViewReports = function () {
    if (reportsQuickViewId > 0) {
        var quickViewArray = searchViewModel.quickViewData();
        var quickViewReportArray = $.grep(searchViewModel.quickViewData(), function (data) {
            return data.ID == reportsQuickViewId;
        });
        //alert(quickViewReportArray.length);
        if (quickViewReportArray.length == 0) {
            $('#collapseReport').attr('src', 'images/expand.png');
            $('#quickViewReport').hide();
            $('#quickViewReportGrid').hide();
            $('#noReportRecord').hide();
        }
        else {
            $('#collapseReport').attr('src', 'images/Search-collapse.png');
            $('#quickViewReport').show();
            $('#noReportRecord').hide();
            $('#quickViewReportGrid').show();
        }
    }
}

var retryCountIfErrorOccured = 0;

var GetQuickViews = function (locationBrandId, userId, doNotResetCount) {
    if (typeof (doNotResetCount) == 'undefined') {
        retryCountIfErrorOccured = 0;
    }

    var ajaxURl = '/Search/GetQuickViewGridData';
    if (quickURLSettings != undefined && quickURLSettings != '') {
        ajaxURl = quickURLSettings.GetQuickViewGridData;
    }

    if (locationBrandId == 0) {
        var userLocationBrandIds = '';
        $('#qvlocation ul li').slice(1).each(function () {
            if ($(this).attr('value') > 0) {
                userLocationBrandIds = userLocationBrandIds.trim() != '' ? (userLocationBrandIds + ',' + $(this).attr('value')) : $(this).attr('value');
            }
        });
        locationBrandId = userLocationBrandIds;
    }

    if (locationBrandId == null || locationBrandId == "" || userId == null || userId == "" || userId < -1) {
        return false;
    }

    $(".loader_container_quickviewgrid").show();
    $.ajax({
        url: ajaxURl,
        type: "GET",
        data: { 'locationBrandIds': locationBrandId, 'userId': userId, 'clientDate': GetUTCString(new Date()) },
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        success: function (data) {
            $(".loader_container_quickviewgrid").hide();
            if (data.status) {
                var srcs = $.map(data.gridData, function (item) { return new QuickViewData(item); });
                searchViewModel.quickViewData(srcs);
                ShowHideQuickViewReports();
                if (isSortingApplied) {
                    searchViewModel.SortQuickView();
                }
            }
        },
        error: function (e) {
            retryCountIfErrorOccured = retryCountIfErrorOccured + 1;
            if (retryCountIfErrorOccured < 3) {
                GetQuickViews(locationBrandId, userId, true);
            }
            $(".loader_container_quickviewgrid").hide();
            console.log(e.message);
        }
    });
}

var DeleteQuickViewFromDB = function (quickView) {
    var ajaxURl = '/Search/DeleteQuickView';
    if (quickURLSettings != undefined && quickURLSettings != '') {
        ajaxURl = quickURLSettings.DeleteQuickView;
    }
    $(".loader_container_quickviewgrid").show();
    $.ajax({
        url: ajaxURl,
        type: "POST",
        data: JSON.stringify({ 'quickViewId': quickView.ID, 'userId': $("#LoggedInUserId").val(), 'clientDate': GetUTCString(new Date()) }),
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        success: function (data) {
            $(".loader_container_quickviewgrid").hide();
            if (data.status) {
                searchViewModel.quickViewData.remove(quickView);
                if (quickView.SearchSummaryId == SearchSummaryId) {
                    $(".btnSetAsQuickView").removeClass("disable-button").prop("disabled", false);
                }
                quickViewRowObject = null;
                SearchSummaryRecursiveCall = true;//For Settimeout recursive call not create
                recentSearchAjaxCall();
                DisplayQuickViewGridMessages("QuickView deleted successfully", false);
            }
        },
        error: function (e) {
            $(".loader_container_quickviewgrid").hide();
            console.log(e.message);
        }
    });
}

function DisableUserDD() {
    var isAdmin = $("#hdnIsAdminUser").val();
    if (isAdmin.toUpperCase() == "FALSE") {
        if ($('#LoggedInUserId') != undefined && $('#LoggedInUserId').val().trim() != '') {
            var loggedInUserId = parseInt($('#LoggedInUserId').val());
            $('#quickViewList #qvuser ul li[value="' + loggedInUserId + '"]').addClass('selected');
            $('#quickViewList #qvuser li').eq(0).val($('#quickViewList #qvuser ul li[value="' + loggedInUserId + '"]').val()).text($('#quickViewList #qvuser ul li[value="' + loggedInUserId + '"]').text());
        }
        //$('#quickViewList #qvuser').addClass("disable-UL");
        $('#quickViewList #qvuser ul li[value="0"]').hide();
    }
}