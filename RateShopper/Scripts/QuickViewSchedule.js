
var quickViewmodel;
var oldLocationBrandId = 0;
var oldSearchSummaryId = 0;
var defaultSelectedCompetitors = new Array();
var defaultSelectedCarClasses = new Array();
var QuickViewId = 0;
$(document).ready(QuickViewScheduleDocumentReady);

function QuickViewScheduleDocumentReady() {
    $("#quickViewEnhancement").draggable();
    $('.custom-select label').click(customTimeClick);

    $("#btnQuickViewSave").click(function () { return QuickViewSaveClicked(); });
    $(".btnSetAndRunQuickView").click(function () { return SaveQuickViewSchedule(true); });
    $("input[name='scheduletime'][type='radio']").change(function () {
        if (this.value == "1") {
            $("#customInputHours").val('');
            $("#customInputMinutes").val('');
            $(".customtimecontainer input[type='radio']").prop({ "checked": false }).closest("label").removeClass("ui-state-hover");
            $(".customtimecontainer").show();
        }
        else {
            $(".customtimecontainer").hide();
        }
    });
    $('[id=IgnoreAndNext]').on("click", function () { IgnoreAndNext(); });
    setTimeout(function () {
        AddQuickViewGroup();
        $("#AddQuickViewGroup").click();
    }, 250);

    $("#shoppickuptime .hidden li").click(function () {
        $("#dropofftime .hidden li.selected").removeClass("selected");
        $("#dropofftime .hidden li[val='" + $(this).attr("val") + "']").addClass("selected");
        $("#dropofftime li").eq(0).attr("value", $("#dropofftime .hidden li[val='" + $(this).attr("val") + "']").text()).text($("#dropofftime .hidden li[val='" + $(this).attr("val") + "']").text());
    });
}

function markTimeSelection($obj, value) {
    $obj.closest(".custom-select").find("label.ui-state-hover").removeClass("ui-state-hover");
}

var customTimeClick = function () {
    if ($(this).find('input:disabled').length > 0) {
        return;
    }
    markTimeSelection($(this));
    var selectedValue = $($(this).find('input:checked')).val();
    $(this).addClass('ui-state-hover');
    var $input = $(this).closest(".select_tym").find("input[readonly='readonly']");
    $input.val(selectedValue);
    if ($input.attr("id") == "customInputHours") {
        DisablePassedMinutesTimeSlots(selectedValue);
    }
}

var DisablePassedHoursTimeSlots = function () {
    var currentDate = new Date();
    var currentHours = currentDate.getHours();
    $("#customInputHoursSelect input[type='radio']").each(function () {
        if (this.value < currentHours) {
            $(this).prop({ "disabled": true, "checked": false }).closest("label").addClass("hidden");
        }
    });
}

var DisablePassedMinutesTimeSlots = function (selectedHour) {
    var currentDate = new Date();
    var currentHours = currentDate.getHours();
    if (selectedHour == currentHours) {
        var currentMinutes = currentDate.getMinutes();
        $("#customInputMinutesSelect input[type='radio']").each(function () {
            if (this.value < currentMinutes) {
                $(this).prop({ "checked": false }).closest("label").removeClass("ui-state-hover").hide();
            }
        });
        if ($("#customInputMinutes").val() < currentMinutes) {
            $("#customInputMinutes").val('');
        }
    }
    else {
        $("#customInputMinutesSelect label").show();
    }
}

var ValidateScheduleTime = function (fromSetAndRun) {
    var selectedScheduleMinutes = $(".quickviewschedule input[type='radio']:checked").val();
    if (fromSetAndRun) {
        selectedScheduleMinutes = "0";
    }

    var currentDate = new Date();
    if (typeof (selectedScheduleMinutes) != 'undefined' && selectedScheduleMinutes != "1" && selectedScheduleMinutes != "0") {
        var newDateTimeStamp = new Date(currentDate.setMinutes(currentDate.getMinutes() + parseInt(selectedScheduleMinutes)));
        if (currentDate.getDate() < newDateTimeStamp.getDate()) {
            DisplayMessage("Your schedule is exceeding today's date. You can schedule for today's date only.", true);
            return "";
        }
        return newDateTimeStamp;
    }
        //Run now is selected by enduser
    else if (selectedScheduleMinutes == "0") {
        return "0";
    }
        //custom time selected
    else if (selectedScheduleMinutes == "1") {
        if ($(".custom-time-container input[type='radio']:checked").length != 2 || $("#customInputHours").val() == "" || $("#customInputMinutes").val() == "") {
            DisplayMessage("Please select hours and minutes to schedule", true);
            return "";
        }
        var customTime = $("#customInputHours").val() + ":" + $("#customInputMinutes").val();

        var newDateTimeStamp = new Date(currentDate.toLocaleDateString() + " " + customTime);
        if (newDateTimeStamp <= currentDate) {
            DisplayMessage("Selected time has elapsed, please select valid time", true);
            return "";
        }
        return newDateTimeStamp;
    }
    return "";
}

var GetSelectedQuickViewCompetitors = function (fromSetAndRun, brandId, searchSummaryId) {
    if (fromSetAndRun) {
        ResetQuickViewSchedulePopup();
        if (typeof (brandId) == 'undefined') {
            brandId = $("#result-section ul#location .hidden li.selected").attr("lbid");
        }
        if (typeof (brandId) != 'undefined' && brandId > 0) {
            if (typeof (searchSummaryId) != "undefined") {
                GetQuickViewCompetitors(brandId, searchSummaryId);
            }
            else {
                GetQuickViewCompetitors(brandId, SearchSummaryId, GlobalLimitSearchSummaryData.CarClassIDs);
            }
        }
    }
    $("#selectedCompetitors").val('');
    var selectedCompetitorsFromGroup = $("[id$='lstCompetitors'] option:selected").map(function () { return $(this).val(); });
    var removeSelectionOfOptions = $.grep($('#lstQuickViewCompetitors option:selected').map(function () { return $(this) }), function (item) { return $.inArray($(item).val(), selectedCompetitorsFromGroup) == -1 });
    $(removeSelectionOfOptions).each(function () { $(this).prop('selected', false); });

    var selectedCompetitorsCode = "";
    var selectedQuickViewCompetitors = $('#lstQuickViewCompetitors option:selected').map(function () {
        selectedCompetitorsCode = selectedCompetitorsCode + ", " + $(this).attr('code'); return $(this).val();
    }).get().join(',');
    if (selectedQuickViewCompetitors == "") {
        DisplayMessage("Please select competitors for tracking", true);
    }
    else {
        $("#selectedCompetitors").val((selectedCompetitorsCode.substr(1, selectedCompetitorsCode.length)));
    }
    return selectedQuickViewCompetitors;
}

var GetSelectedQuickViewCarClasses = function (fromSetAndRun, brandId, searchSummaryId, carClassIds, trackingCarClassIds) {
    if (fromSetAndRun) {
        ResetQuickViewSchedulePopup();
        if (typeof (brandId) == 'undefined') {
            brandId = $("#result-section ul#location .hidden li.selected").attr("lbid");
        }
        if (typeof (brandId) != 'undefined' && brandId > 0) {
            if (typeof (searchSummaryId) != "undefined" && typeof (carClassIds) != "undefined") {
                GetQuickViewCompetitors(brandId, searchSummaryId, carClassIds, trackingCarClassIds);
            }
            else {
                var carClassIds = GlobalLimitSearchSummaryData.CarClassIDs;
                if (typeof (carClassIds) != 'undefined' && carClassIds != '') {
                    if (carClassIds.lastIndexOf(",") == carClassIds.length - 1) {
                        carClassIds = carClassIds.substring(0, carClassIds.lastIndexOf(","));
                    }
                }
                GetQuickViewCompetitors(brandId, SearchSummaryId, carClassIds);
            }
        }
    }
    $("#selectedTrackingCarClasses").val('');
    var selectedCarClassCode = "";
    var selectedTrackingCarClasses = $('#lstCarClasses option:selected').map(function () {
        selectedCarClassCode = selectedCarClassCode + ", " + $(this).attr('code'); return $(this).val();
    }).get().join(',');
    if (selectedTrackingCarClasses == "") {
        DisplayMessage("Please select car classes for tracking", true);
    }
    else {
        $("#selectedTrackingCarClasses").val((selectedCarClassCode.substr(1, selectedCarClassCode.length)));
    }
    return selectedTrackingCarClasses;
}

//Show the success/error messages
var DisplayMessage = function (message, isError) {
    $("#quickviewmessage").html(message).show().css("color", isError ? "red" : "green");
    if (!isError) {
        setTimeout(function () { $("#quickviewmessage").hide(); }, 3000);
    }
}

var GetUTCString = function (date) {
    return ("0" + (date.getUTCMonth() + 1)).slice(-2) + "-" + ("0" + date.getUTCDate()).slice(-2) + "-" + date.getUTCFullYear() + " " + ("0" + date.getUTCHours()).slice(-2) + ":" + ("0" + date.getUTCMinutes()).slice(-2);
}

var ResetQuickViewSchedulePopup = function () {
    $("#quickviewmessage").hide();
    $(".custom-select>label.ui-state-hover").removeClass("ui-state-hover");
    $('#lstQuickViewCompetitors option:selected').prop("selected", false);
    $("#rdosch_120").prop("checked", true);
    DisablePassedHoursTimeSlots();
    $("#customInputMinutesSelect label").show();
    $("#customInputHours").val('');
    $("#customInputMinutes").val('');
    $(".custom-select input[type='radio']:checked").prop("checked", false).closest("label").removeClass("ui-state-hover");
    $("#lstQuickViewCompetitors option").each(function () {
        if ($.inArray(this.value, defaultSelectedCompetitors) > -1) {
            $(this).prop("selected", true);
        }
    });
    $("#lstQuickViewCompetitors").scrollTop(0);
    $(".customtimecontainer").hide();
    if (QuickViewId == 0) {
        $("#QuickViewGroupContainer select").each(function () {
            RemoveFlashableTag($(this));
        });
        $("#QuickViewGroupContainer input[type=text]").each(function () {
            RemoveFlashableTag($(this));
        });
    }
    else {
        $("#QuickViewUpdateGroupContainer select").each(function () {
            RemoveFlashableTag($(this));
        });
        $("#QuickViewUpdateGroupContainer input[type=text]").each(function () {
            RemoveFlashableTag($(this));
        });
    }
    QuickViewId = 0;
    HideShowQuickViewGroupSetting(false);
    //$("#QuickViewGroupContainer").empty();
    $("#QuickViewGroupContainer table").remove();
    $("#QuickViewUpdateGroupContainer").addClass("hidden");
    $("#QuickViewUpdateGroupContainer table[newadd]").remove();
    $("#RBTotalRate").prop("checked", true);
    $("#EmailNotification").prop("checked", false);
    if (GlobalLimitSearchSummaryData != undefined && GlobalLimitSearchSummaryData != null) {
        $("#shoppickuptime .hidden li.selected").removeClass("selected");
        $("#shoppickuptime .hidden li[val='" + GlobalLimitSearchSummaryData.StartTime + "']").addClass("selected");
        $("#shoppickuptime li").eq(0).attr("value", $("#shoppickuptime .hidden li[val='" + GlobalLimitSearchSummaryData.StartTime + "']").text()).text($("#shoppickuptime .hidden li[val='" + GlobalLimitSearchSummaryData.StartTime + "']").text());

        $("#dropofftime .hidden li.selected").removeClass("selected");
        $("#dropofftime .hidden li[val='" + GlobalLimitSearchSummaryData.EndTime + "']").addClass("selected");
        $("#dropofftime li").eq(0).attr("value", $("#dropofftime .hidden li[val='" + GlobalLimitSearchSummaryData.EndTime + "']").text()).text($("#dropofftime .hidden li[val='" + GlobalLimitSearchSummaryData.EndTime + "']").text());
    }
}

var QuickViewSaveClicked = function () {
    if ($(".quick-view").is(":visible")) {
        EditQuickView(false);
    }
    else {
        SaveQuickViewSchedule(false);
    }
    return false;
}

var BindTrackingCarClasses = function (shopCarClassIds, trackingCarClassIds) {

    var shopClassIdsArray = shopCarClassIds.split(',');
    var CarClassArray = new Array();
    $(shopClassIdsArray).each(function () {
        var option = $("#carClass option[value=" + this + "]");
        if (typeof (option) != "undefined") {
            var item = new Object();
            item.ID = this;
            item.Code = $(option).text();
            item.SelectedForQuickView = $.inArray(this.toString(), trackingCarClassIds.toString().split(",")) != -1;
            CarClassArray.push(item);
        }
    });
    searchViewModel.trackingCarClasses(CarClassArray);
}

var GetQuickViewCompetitors = function (locationBrandId, searchSummaryId, carClassesIds, trackingCarClassIds) {
    //if (oldLocationBrandId == locationBrandId || oldSearchSummaryId == searchSummaryId) {
    //    return;
    //}
    //else {
    //    oldLocationBrandId = locationBrandId;
    //    oldSearchSummaryId = searchSummaryId;
    //}

    var ajaxURl = '/Search/GetQuickViewCompetitors';
    if (quickURLSettings != undefined && quickURLSettings != '') {
        ajaxURl = quickURLSettings.GetQuickViewCompetitors;
    }
    $(".loader_container_quickviewcompetitors").show();
    $.ajax({
        url: ajaxURl,
        type: "GET",
        data: { locationBrandId: locationBrandId, searchSummaryId: searchSummaryId },
        dataType: "json",
        async: false,
        contentType: "charset=utf-8; application/json",
        success: function (data) {
            $(".loader_container_quickviewcompetitors").hide();
            if (data.status) {
                searchViewModel.competitors(data.companies);
                defaultSelectedCompetitors = data.selectedCompetitors.split(",");
                if (data.selectedCarClasses != null) {
                    trackingCarClassIds = data.selectedCarClasses.split(",");
                }
                if (typeof (trackingCarClassIds) == 'undefined' || trackingCarClassIds == "") {
                    trackingCarClassIds = carClassesIds;
                }
                if (typeof (carClassesIds) != 'undefined' && typeof (trackingCarClassIds) != 'undefined') {
                    BindTrackingCarClasses(carClassesIds, trackingCarClassIds);
                }

                if (data.quickviewobject != null) {
                    QuickViewId = data.quickviewobject.ID;

                    if (data.quickviewobject.IsTotal) {
                        $("#RBTotalRate").prop("checked", true);
                    }
                    else {
                        $("#RBBaseRate").prop("checked", true);
                    }
                    $("#EmailNotification").prop("checked", data.quickviewobject.IsEmailNotification);

                    if (typeof(data.quickviewobject.PickupTime) == 'undefined' || data.quickviewobject.PickupTime == null) {
                        data.quickviewobject.PickupTime = "11:00";
                    }
                    if (typeof (data.quickviewobject.DropOffTime) == 'undefined' || data.quickviewobject.DropOffTime == null) {
                        data.quickviewobject.DropOffTime = "11:00";
                    }

                    $("#shoppickuptime .hidden li.selected").removeClass("selected");
                    $("#shoppickuptime .hidden li[val='" + data.quickviewobject.PickupTime + "']").addClass("selected");
                    $("#shoppickuptime li").eq(0).attr("value", $("#shoppickuptime .hidden li[val='" + data.quickviewobject.PickupTime + "']").text()).text($("#shoppickuptime .hidden li[val='" + data.quickviewobject.PickupTime + "']").text());

                    $("#dropofftime .hidden li.selected").removeClass("selected");
                    $("#dropofftime .hidden li[val='" + data.quickviewobject.DropOffTime + "']").addClass("selected");
                    $("#dropofftime li").eq(0).attr("value", $("#dropofftime .hidden li[val='" + data.quickviewobject.DropOffTime + "']").text()).text($("#dropofftime .hidden li[val='" + data.quickviewobject.DropOffTime + "']").text());

                    
                    var srcs = $.map(data.quickviewgroupobject, function (item) { return new QuickViewGroupItem(item); });
                    searchViewModel.quickViewGroupData(srcs);
                    console.log(srcs);
                    setTimeout(function () { HideShowQuickViewGroupSetting(true); }, 100);

                    if (data.quickviewgroupobject.length == 0 && $("#QuickViewUpdateGroupContainer table").length < 1) {
                        setTimeout(function () { AddQuickViewGroupLogic(); }, 100);
                    }
                    //else {
                    //    setTimeout(function () { HideShowQuickViewGroupSetting(true); }, 250);
                    //    //AddQuickViewGroupLogic();
                    //}
                    var scheduleControlId = data.quickviewobject.UIControlId;
                    if (scheduleControlId != null) {
                        $("#" + scheduleControlId).prop("checked", true);
                        if (scheduleControlId == "rdosch_1") {
                            $(".customtimecontainer").show();
                            var scheduleDateTime = new Date(parseInt(data.quickviewobject.NextRunDate.replace("/Date(", "").replace(")/", "")));
                            if (scheduleDateTime >= new Date()) {
                                var $customHour = $("#customInputHoursSelect input[type='radio'][value='" + scheduleDateTime.getHours() + "']");
                                var $customMin = $("#customInputMinutesSelect input[type='radio'][value='" + scheduleDateTime.getMinutes() + "']");

                                $(".custom-select input[type='radio']:checked").prop("checked", false).closest("label").removeClass("ui-state-hover");

                                $("#customInputHours").val(scheduleDateTime.getHours());
                                $customHour.prop('checked', true).closest("label").addClass("ui-state-hover");

                                $("#customInputMinutes").val(scheduleDateTime.getMinutes());
                                $customMin.prop('checked', true).closest("label").addClass("ui-state-hover");
                            }
                        }
                    }
                    //AddQuickViewGroupLogic();
                }
                else {
                    QuickViewId = 0;
                    AddQuickViewGroupLogic();
                    $("#QuickViewGroupContainer select").empty();
                    $("#lstQuickViewCompetitors option:selected").each(function () {
                        $("#QuickViewGroupContainer select").last().append("<option value=" + $(this).val() + ">" + $(this).text() + "</option>");
                    });
                }
                ValidateQuickViewGroupTemplateTextboxEvent();
                //Enable update mode/insert mode
            }
            //AddQuickViewGroupLogic();
        },
        error: function (e) {
            $(".loader_container_quickviewcompetitors").hide();
            console.log(e.message);
        }
    });
}

function SaveQuickViewSchedule(fromSetAndRun) {
    if (typeof (fromSetAndRun) == "undefined") {
        fromSetAndRun = false;
    }
    $("#quickviewmessage").hide();
    if (typeof (SearchSummaryId) != 'undefined' && SearchSummaryId > 0) {
        var selectedQuickViewCompetitors = GetSelectedQuickViewCompetitors(fromSetAndRun);
        var selectedQuickViewCarClasses = GetSelectedQuickViewCarClasses(fromSetAndRun);
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
                    var ajaxURl = '/Search/SaveQuickViewSchedule';
                    if (typeof (quickURLSettings) != "undefined" && quickURLSettings != '') {
                        ajaxURl = quickURLSettings.SaveQuickViewSchedule;
                    }
                    $(".loader_container_main").show();
                    $.ajax({
                        url: ajaxURl,
                        type: "POST",
                        data: JSON.stringify({ 'searchSummaryId': SearchSummaryId, 'scheduleTime': dateTimeStamp, 'quickViewCompetitors': selectedQuickViewCompetitors, 'userId': $('#LoggedInUserId').val(), 'clientDate': GetUTCString(new Date()), 'scheduleControlId': selectedControlId, 'carClassIds': selectedQuickViewCarClasses, 'quickViewGroupData': GroupData }),
                        contentType: "application/json;charset=utf-8",
                        dataType: 'json',
                        success: function (data) {
                            $(".loader_container_main").hide();
                            if (data.status) {
                                if (!fromSetAndRun) {
                                    $(".close-quick-view-popup").click();
                                    if (selectedDate == "0") {
                                        $("#quickviewgridmessage").show().html("QuickView run successfully");
                                    }
                                    else {
                                        $("#quickviewgridmessage").show().html("QuickView schedule successfully");
                                    }

                                }
                                else {
                                    $("#quickviewgridmessage").show().html("QuickView run successfully");
                                }
                                setTimeout(function () { $("#quickviewgridmessage").hide(); }, 3000);
                                $(".btnSetAsQuickView").addClass("disable-button").prop("disabled", true);
                                $(".btnSetAndRunQuickView").addClass("disable-button").prop("disabled", true);
                                FetchQuickViews();
                            }
                        },
                        error: function (e) {
                            $(".loader_container_main").hide();
                            console.log(e.message);
                        }
                    });
                }
            }
        }
        else {
            return false;
        }
    }
    return false;
}

function IgnoreAndNext() {
    var SearchSummaryID, SearchDate, RentalLengthID;
    SearchSummaryID = SearchSummaryId;
    var formatteddate = "";
    if ($('#rbChanged:checked').length > 0) {
        var changedData = $("[id=lengthDateCombinationChanged] ul").find(".selected").eq(0);
        formatteddate = $(changedData).attr("formatteddate");
        SearchDate = formatteddate.substr(0, 4) + "-" + formatteddate.substr(4, 2) + "-" + ((formatteddate.substr(6, 2).length == 1) ? "0" + formatteddate.substr(6, 2) : formatteddate.substr(6, 2));
        RentalLengthID = $(changedData).attr("rentallengthid");
    }
    else {
        var changedData = $("[id=lengthDateCombinationUnChanged] ul").find(".selected").eq(0);
        formatteddate = $(changedData).attr("formatteddate");
        SearchDate = formatteddate.substr(0, 4) + "-" + formatteddate.substr(4, 2) + "-" + ((formatteddate.substr(6, 2).length == 1) ? "0" + formatteddate.substr(6, 2) : formatteddate.substr(6, 2));
        RentalLengthID = $(changedData).attr("rentallengthid");
    }
    //SearchDate = "2015-11-06"; //$("#displayDay li").eq(0).val();
    //RentalLengthID = $("#length li").eq(0).val();
    if (RentalLengthID != undefined && RentalLengthID != "" && SearchDate != undefined && SearchDate != "") {
        var ajaxURl = '/RateShopper/QuickView/IgnoreAndNext/';
        if (quickURLSettings != undefined && quickURLSettings != '') {
            ajaxURl = quickURLSettings.IgnoreAndNextUrl;
        }

        $.ajax({
            url: ajaxURl,
            type: "POST",
            data: { 'searchDate': SearchDate, 'RentalLengthId': RentalLengthID, 'searchSummaryId': SearchSummaryID },
            dataType: 'json',
            async: true,
            success: function (data) {
                if (data == "success") {
                    $("#lblQuickViewGridMessage").show().html("QuickView reviewed successfully");
                    setTimeout(function () { $("#lblQuickViewGridMessage").hide(); }, 3000);
                    //Pending one functionality is to move next search screen

                    if (isQuickViewShop) {
                        //Use for while user has to clicks on update/update&next button shop will not go to next page on this ignore & next call function.
                        if (!IsQuickUpdateAndNext) {
                            $('#rbChanged:checked').length > 0 ? ShowNextQuickView('#lengthDateCombinationChanged') : ShowNextQuickView('#lengthDateCombinationUnChanged');
                            getSearchData(false);
                        }
                        else {
                            IsQuickUpdateAndNext = false;
                        }
                    }

                    if ($("#quickViewReportGrid tbody tr").length > 0) {
                        if ($("#quickViewReportGrid tbody tr td span[rentallengthid=" + RentalLengthID + "][formatdate=" + formatteddate + "]").length > 0) {
                            $("#quickViewReportGrid tbody tr td span[rentallengthid=" + RentalLengthID + "][formatdate=" + formatteddate + "]").attr("isreviewed", "true").parent("td").addClass("quickCellReviewed");
                        }
                        //show report reviewed if all the changed cells are reviewed by user
                        var isReviewed = true;
                        if (SearchSummaryID == $("#quickViewReportGrid tbody tr td span").attr('searchsummaryid')) {
                            $('#quickViewReportGrid tbody tr td').not('.dates').each(function () {
                                if ($(this).find('span').attr('ischanged') == "true" || $(this).find('span').hasClass('spn_P')) {
                                    if (($.trim($(this).find('span').attr('isreviewed')) == "" || $.trim($(this).find('span').attr('isreviewed')) == "false") ||
                                                                (($(this).find('span').hasClass('spn_P')) && ($.trim($(this).find('span').attr('isreviewed')) == "false"))) {
                                        isReviewed = false;
                                        return false;
                                    }
                                }
                            });
                            if (isReviewed) {
                                if ($('.quick-view-table tbody tr').length > 0) {
                                    $('.quick-view-table tbody tr').each(function () {
                                        if ($(this).attr('childsearchsummaryid') == SearchSummaryID) {
                                            $(this).find('td.actions ul li span.SRP').css('display', 'none');
                                            $(this).find('td.actions ul li span.review').parent('a').css('display', 'block');
                                            $(this).find('td.actions ul li span.review').css('display', 'block');
                                        }
                                    });
                                }
                            }
                        }
                    }
                }
            },
            error: function (e) {
                console.log("Oops! something went wrong in Ignore and next.");
            }
        });
    }
}

var UpdateSearchSummaryStatus = function (searchSummaryId, fromSetAndRun) {
    if (searchSummaryId == SearchSummaryId) {
        $(".btnSetAsQuickView").addClass("disable-button").prop("disabled", true);
        $(".btnSetAndRunQuickView").addClass("disable-button").prop("disabled", true);
    }
    ko.utils.arrayForEach(searchViewModel.SearchSummary(), function (currentSearchModel) {
        if (currentSearchModel.ID == searchSummaryId) {
            currentSearchModel.HasQuickViewChild = true;
        }
    });
}

//Other functions//
var AddQuickViewGroup = function () {
    $("#AddQuickViewGroup").on("click", function () {
        //$("[id=AddQuickViewGroup]").prop('disabled', true).addClass("disable-button");        
        AddQuickViewGroupLogic();
    });
}


var AddQuickViewGroupLogic = function () {
    if (QuickViewId == 0) {
        $(".defaultQuickViewGroup").clone().appendTo("#QuickViewGroupContainer");
        $("#QuickViewGroupContainer").find(".defaultQuickViewGroup").removeClass("defaultQuickViewGroup").removeAttr("style");

        var totalGroupSelectLength = $("#QuickViewGroupContainer select ").length;
        if (totalGroupSelectLength != 1) {
            var totalSelect = totalGroupSelectLength - 2;
            $("#QuickViewGroupContainer select").eq(totalSelect).each(function (index) {
                $("#QuickViewGroupContainer select").last().html("");
                $("#lstQuickViewCompetitors option:not(:selected)")
                $($(this)).find("option:not(:selected)").each(function () {
                    $("#QuickViewGroupContainer select").last().append("<option value=" + $(this).val() + ">" + $(this).text() + "</option>");
                });
            });
            var quickviewAddBtbValidation = $("#QuickViewGroupContainer select");
            if ($(quickviewAddBtbValidation).eq(0).find("option:selected").length == 0 || ($(quickviewAddBtbValidation).eq(totalSelect).find("option").length == $(quickviewAddBtbValidation).eq(0).find("option:selected").length)) {
                //$("#AddQuickViewGroup").prop('disabled', true).addClass("disable-button");
            }
        }
        else {
            $("#lstQuickViewCompetitors option:selected").each(function () {
                $("#QuickViewGroupContainer select").last().append("<option value=" + $(this).val() + ">" + $(this).text() + "</option>");
            });
            var quickviewAddBtbValidation = $("#QuickViewGroupContainer select");
            if ($(quickviewAddBtbValidation).eq(0).find("option:selected").length == 0 || ($(quickviewAddBtbValidation).eq(totalSelect).find("option").length == $(quickviewAddBtbValidation).eq(0).find("option:selected").length)) {
                $("#AddQuickViewGroup").prop('disabled', true).addClass("disable-button");
            }
        }
        if ($("#QuickViewGroupContainer table").length == 3) {
            $("#AddQuickViewGroup").prop('disabled', true).addClass("disable-button");
        }
        $("#QuickViewGroupContainer table").each(function (index) {
            $(this).find("select").attr("GroupIndex", (index + 1));
        });
    }
    else {
        $(".defaultQuickViewGroup").clone().appendTo("#QuickViewUpdateGroupContainer");
        $("#QuickViewUpdateGroupContainer").find(".defaultQuickViewGroup").attr("newadd", "yes");
        $("#QuickViewUpdateGroupContainer").find(".defaultQuickViewGroup").removeClass("defaultQuickViewGroup").removeAttr("style");

        //var totalGroupSelectLength = $("#QuickViewUpdateGroupContainer select ").length;
        //if (totalGroupSelectLength != 1) {
        var totalSelect = $("#QuickViewUpdateGroupContainer select").length - 2;
        $("#QuickViewUpdateGroupContainer select").eq(totalSelect).each(function () {
            $("#QuickViewUpdateGroupContainer select").last().html("");
            $($(this)).find("option").each(function () {
                if ($(this).prop("selected")) {
                    $("#QuickViewUpdateGroupContainer select").last().append();
                }
                else {
                    $("#QuickViewUpdateGroupContainer select").last().append("<option value=" + $(this).val() + ">" + $(this).text() + "</option>");
                }
            });
        });


        $("#QuickViewUpdateGroupContainer table").each(function (index) {
            $(this).find("select").attr("GroupIndex", (index + 1));
        });
        if ($("#QuickViewUpdateGroupContainer table").length == 3) {
            $("#AddQuickViewGroup").prop('disabled', true).addClass("disable-button");
        }
    }
    //ResetQuickViewSchedulePopup();
    ValidateQuickViewGroupTemplateTextboxEvent();
}
function AutoPopulateQuickViewGroupCompanies(CompaniesData) {
    if (QuickViewId == 0) {
        var GroupIndex = $(CompaniesData).attr("groupindex");
        //This option is use to get before selectedID
        var PreviousSelectedItem = $(CompaniesData).attr("selectedItem");
        $("#QuickViewGroupContainer select").each(function (index) {
            var selectedArray = [];
            if ($(this).val() != null) {
                selectedArray = $(this).val().toString();
            }
            $(this).attr("selectedItem", selectedArray.toString());
        });
        var selectedItem = $(CompaniesData).attr("selectedItem");
        $("#QuickViewGroupContainer select").each(function (index) {
            var otherSelectedItem = $(this).attr("selectedItem");

            if ($(this).attr("groupindex") != GroupIndex) {
                $(this).find("option").each(function () {
                    for (var i = 0; i < selectedItem.split(',').length; i++) {
                        //Selected Item Checked for remove operation
                        if ($.inArray(selectedItem.split(',')[i].toString(), otherSelectedItem.split(',')) != -1) {
                            if (selectedItem.split(',')[i] == $(this).val()) {
                                $(this).remove();
                            }
                        }
                        //Other unselect Item remove
                        if (selectedItem.split(',')[i] == $(this).val()) {
                            $(this).remove();
                        }
                    }
                });
            }
            else {
                //This else for selected value can be unselect then that unselected item filled to another group select item
                //This one current selected selectboxitem stored in selectedGroupItemData
                var selectedGroupItemData = [];
                $(this).find("option").each(function () {
                    selectedGroupItemData.push(this);
                });
                //This check on previuosSelctedItem in current selectedItem 
                if (PreviousSelectedItem != undefined) {
                    for (var i = 0; i < PreviousSelectedItem.split(',').length; i++) {
                        //if use for check current selected item is exist or not in previousSelectedItem
                        if ($.inArray(PreviousSelectedItem.split(',')[i].toString(), selectedItem.split(',')) == -1) {
                            //check here for not check value to addd current option tag

                            //This one is used for internal excapt selected another selecte box item
                            $("#QuickViewGroupContainer select").each(function (index) {
                                var currentSelect = $(this);
                                if ($(this).attr("groupindex") != GroupIndex) {
                                    //Already stored in 
                                    $(selectedGroupItemData).each(function () {
                                        if (PreviousSelectedItem.split(',')[i] == $(this).val()) {
                                            var tempOptionID = $(this).val();
                                            var tempText = $(this).text();
                                            var IsAppend = false;
                                            var lastOptionVal = "";
                                            currentSelect.find("option").each(function () {
                                                lastOptionVal = parseInt($(this).val());
                                                if (parseInt(tempOptionID) < lastOptionVal) {
                                                    $("<option value='" + tempOptionID + "'>" + tempText + "</option>").insertBefore(currentSelect.find('option[value="' + $(this).val() + '"]')).eq(index);
                                                    IsAppend = true;
                                                    return false;
                                                }
                                            });
                                            if (!IsAppend) {
                                                $("<option value='" + tempOptionID + "'>" + tempText + "</option>").appendTo(currentSelect).eq(index);
                                                //$("<option value='" + tempOptionID + "'>" + tempText + "</option>").insertAfter(currentSelect.find('option[value="' + lastOptionVal + '"]')).eq(index);;
                                                return false;
                                            }
                                            // $("<option value='" + $(this).val() + "'>" + $(this).text() + "</option>").appendTo(currentSelect).eq(index);
                                            return false; //Stop current loop itration
                                        }
                                    });
                                }
                            });
                        } //End If array
                    } //End For
                }//End If
            }
        });
        //Third loop is used for check the addNewGroup button disabled/enable
        var CheckAddButton = false;
        $("#QuickViewGroupContainer select").each(function () {
            if ($(this).attr("groupindex") != GroupIndex) {
                if ($(this).val() == null) {
                    CheckAddButton = false;
                    return false;
                }
            }
            else {
                if ($(this).val() == null) {
                    CheckAddButton = false;
                    return false;
                }
            }
            if ($(this).val().length != $(this).find("option").length) {
                CheckAddButton = true;
                return false;
            }
        });
        if (!CheckAddButton) {
            $("#AddQuickViewGroup").prop('disabled', true).addClass("disable-button");
            CheckAddButton = false;
        }
        else {
            $("#AddQuickViewGroup").prop('disabled', false).removeClass("disable-button");
            CheckAddButton = false;
        }
        if ($("#QuickViewGroupContainer select").length == 3) {
            $("#AddQuickViewGroup").prop('disabled', true).addClass("disable-button");
        }
    }
    else {
        var GroupIndex = $(CompaniesData).attr("groupindex");
        //This option is use to get before selectedID
        var PreviousSelectedItem = $(CompaniesData).attr("selectedItem");
        $("#QuickViewUpdateGroupContainer select").each(function (index) {
            var selectedArray = [];
            if ($(this).val() != null) {
                selectedArray = $(this).val().toString();
            }
            $(this).attr("selectedItem", selectedArray.toString());
        });
        var selectedItem = $(CompaniesData).attr("selectedItem");
        $("#QuickViewUpdateGroupContainer select").each(function (index) {
            var otherSelectedItem = $(this).attr("selectedItem");

            if ($(this).attr("groupindex") != GroupIndex) {
                $(this).find("option").each(function () {
                    for (var i = 0; i < selectedItem.split(',').length; i++) {
                        //Selected Item Checked for remove operation
                        if ($.inArray(selectedItem.split(',')[i].toString(), otherSelectedItem.split(',')) != -1) {
                            if (selectedItem.split(',')[i] == $(this).val()) {
                                $(this).remove();
                            }
                        }
                        //Other unselect Item remove
                        if (selectedItem.split(',')[i] == $(this).val()) {
                            $(this).remove();
                        }
                    }
                });
            }
            else {
                //This else for selected value can be unselect then that unselected item filled to another group select item
                //This one current selected selectboxitem stored in selectedGroupItemData
                var selectedGroupItemData = [];
                $(this).find("option").each(function () {
                    selectedGroupItemData.push(this);
                });
                //This check on previuosSelctedItem in current selectedItem 
                if (PreviousSelectedItem != undefined) {
                    for (var i = 0; i < PreviousSelectedItem.split(',').length; i++) {
                        //if use for check current selected item is exist or not in previousSelectedItem
                        if ($.inArray(PreviousSelectedItem.split(',')[i].toString(), selectedItem.split(',')) == -1) {
                            //check here for not check value to addd current option tag
                            //This one is used for internal excapt selected another selecte box item
                            $("#QuickViewUpdateGroupContainer select").each(function (index) {
                                var currentSelect = $(this);
                                if ($(this).attr("groupindex") != GroupIndex) {
                                    //Already stored in 
                                    $(selectedGroupItemData).each(function () {

                                        if (PreviousSelectedItem.split(',')[i] == $(this).val()) {
                                            var tempOptionID = $(this).val();
                                            var tempText = $(this).text();
                                            var IsAppend = false;
                                            var lastOptionVal = "";
                                            currentSelect.find("option").each(function () {
                                                lastOptionVal = parseInt($(this).val());
                                                if (parseInt(tempOptionID) < lastOptionVal) {
                                                    $("<option value='" + tempOptionID + "'>" + tempText + "</option>").insertBefore(currentSelect.find('option[value="' + $(this).val() + '"]')).eq(index);
                                                    IsAppend = true;
                                                    return false;
                                                }
                                            });
                                            if (!IsAppend) {
                                                $("<option value='" + tempOptionID + "'>" + tempText + "</option>").appendTo(currentSelect).eq(index);
                                                //$("<option value='" + tempOptionID + "'>" + tempText + "</option>").insertAfter(currentSelect.find('option[value="' + lastOptionVal + '"]')).eq(index);;
                                                return false;
                                            }
                                            // $("<option value='" + $(this).val() + "'>" + $(this).text() + "</option>").appendTo(currentSelect).eq(index);
                                            return false; //Stop current loop itration
                                        }
                                    });
                                }
                            });
                        } //End If array
                    } //End For
                }//End If
            }
        });
        //Third loop is used for check the addNewGroup button disabled/enable
        var CheckAddButton = false;
        $("#QuickViewUpdateGroupContainer select").each(function () {
            if ($(this).attr("groupindex") != GroupIndex) {
                if ($(this).val() == null) {
                    CheckAddButton = false;
                    return false;
                }
            }
            else {
                if ($(this).val() == null) {
                    CheckAddButton = false;
                    return false;
                }
            }
            if ($(this).val().length != $(this).find("option").length) {
                CheckAddButton = true;
                return false;
            }
        });
        if (!CheckAddButton) {
            $("#AddQuickViewGroup").prop('disabled', true).addClass("disable-button");
            CheckAddButton = false;
        }
        else {
            $("#AddQuickViewGroup").prop('disabled', false).removeClass("disable-button");
            CheckAddButton = false;
        }
        if ($("#QuickViewUpdateGroupContainer select").length == 3) {
            $("#AddQuickViewGroup").prop('disabled', true).addClass("disable-button");
        }
    }
}
//Competitor Company selection operation
var AddPopulateCompanies = function (companiesData) {
    var matches = [];
    $("#lstQuickViewCompetitors option:selected").each(function () {
        var companies = new Object();
        companies.ID = $(this).val();
        companies.Name = $(this).text();
        companies.CompanySelection = "false";
        matches.push(companies);
    });
    if (QuickViewId == 0) {
        //Insert Case
        var tempSelectedArrayID = "";
        var SelectedArrayID = "";
        //get all selected competitor id 
        $("#QuickViewGroupContainer select").each(function () {
            if ($(this).attr("selecteditem") != "" && $(this).attr("selecteditem") != undefined) {
                tempSelectedArrayID += ($(this).attr("selecteditem").trim() + ",");
            }
        });
        if (tempSelectedArrayID != "") {
            SelectedArrayID = tempSelectedArrayID.trim().substring(0, tempSelectedArrayID.trim().length - 1);
        }
        SelectedArrayID = tempSelectedArrayID.trim().substring(0, tempSelectedArrayID.trim().length - 1);
        $("#QuickViewGroupContainer select").each(function (index) {
            $(this).empty();
            if ($(this).attr("selectedItem") == '' && $(this).attr("selectedItem") == undefined) {
                $(matches).each(function () {
                    if ($.inArray(this.ID.toString(), SelectedArrayID.trim().split(",")) == -1) {
                        $("<option value='" + this.ID + "'>" + this.Name + "</option>").appendTo($("#QuickViewGroupContainer select").eq(index));
                    }
                });
            }
            else {
                var selectedID = $(this).attr("selectedItem");
                var allSelectedItem = "";
                $("#QuickViewGroupContainer select").each(function (index) {
                    if ($(this).attr("selectedItem") != undefined) {
                        allSelectedItem += $(this).attr("selectedItem") + ",";
                    }
                });
                var finalSelectedID = allSelectedItem.trim().substring(0, allSelectedItem.trim().length - 1);
                $(matches).each(function (matchIndex) {
                    if (!JSON.parse(this.CompanySelection)) {
                        $("<option value='" + this.ID + "'>" + this.Name + "</option>").appendTo($("#QuickViewGroupContainer select").eq(index));
                    }

                    if (selectedID != undefined) {
                        if ($.inArray(this.ID.toString(), selectedID.split(',')) != -1) {
                            var matchItem = this;
                            for (var i = 0; i < selectedID.split(',').length; i++) {
                                if (selectedID.split(',')[i] == matchItem.ID) {
                                    this.CompanySelection = "true";
                                }
                            }
                        }
                    }
                });
                var Deleteoption = "";
                if (finalSelectedID.length != 0) {
                    if (selectedID == undefined) {
                        selectedID = "";
                    }
                    Deleteoption = $.grep(finalSelectedID.split(','), function (element) {
                        return $.inArray(element, selectedID.split(',')) == -1;
                    });
                    //Deleteoption = finalSelectedID.split(',').filter(function (obj) { return selectedID.toString().indexOf(obj) == -1; });
                }

                $(this).find("option").each(function () {
                    var optionVal = $(this).val();
                    if (selectedID != undefined) {
                        if ($.inArray(optionVal.toString(), selectedID.split(',')) == -1) {
                            $(this).prop("selected", false);
                        }
                        else {
                            $(this).prop("selected", true);
                        }
                        //This Condition is for AllControl item selected removed
                        if ($.inArray(optionVal.toString(), Deleteoption.toString().split(',')) != -1) {
                            $(this).remove();
                        }
                    }
                });
                $(this).attr("selectedItem", $(this).val());
            }
        });

        ///button disbale or enable
        var CheckAddButton = false;
        $("#QuickViewGroupContainer select").each(function () {
            if ($(this).val() == null) {
                CheckAddButton = false;
                return false;
            }
            if ($(this).val().length != $(this).find("option").length) {
                CheckAddButton = true;
                return false;
            }
        });
        if (!CheckAddButton) {
            $("#AddQuickViewGroup").prop('disabled', true).addClass("disable-button");
        }
        else {
            if ($("#QuickViewGroupContainer select").length != 3) {
                $("#AddQuickViewGroup").prop('disabled', false).removeClass("disable-button");
            }
        }
    }
    else {
        //Update case
        var tempSelectedArrayID = "";
        var SelectedArrayID = "";
        $("#QuickViewUpdateGroupContainer select").each(function () {
            if ($(this).attr("selecteditem") != "" && $(this).attr("selecteditem") != undefined) {
                tempSelectedArrayID += ($(this).attr("selecteditem").trim() + ",");
            }
        });
        if (tempSelectedArrayID != "") {
            SelectedArrayID = tempSelectedArrayID.trim().substring(0, tempSelectedArrayID.trim().length - 1);
        }
        SelectedArrayID = tempSelectedArrayID.trim().substring(0, tempSelectedArrayID.trim().length - 1);
        $("#QuickViewUpdateGroupContainer select").each(function (index) {
            $(this).empty();
            if ($(this).attr("selectedItem") == '' && $(this).attr("selectedItem") == undefined) {
                $(matches).each(function () {
                    if ($.inArray(this.ID.toString(), SelectedArrayID.trim().split(",")) == -1) {
                        $("<option value='" + this.ID + "'>" + this.Name + "</option>").appendTo($("#QuickViewUpdateGroupContainer select").eq(index));
                    }
                });
            }
            else {
                var selectedID = $(this).attr("selectedItem");
                var allSelectedItem = "";
                $("#QuickViewUpdateGroupContainer select").each(function (index) {
                    if ($(this).attr("selectedItem") != undefined) {
                        allSelectedItem += $(this).attr("selectedItem") + ",";
                    }
                });
                var finalSelectedID = allSelectedItem.trim().substring(0, allSelectedItem.trim().length - 1);
                $(matches).each(function (matchIndex) {
                    if (!JSON.parse(this.CompanySelection)) {
                        $("<option value='" + this.ID + "'>" + this.Name + "</option>").appendTo($("#QuickViewUpdateGroupContainer select").eq(index));
                    }

                    if (selectedID != undefined) {
                        if ($.inArray(this.ID.toString(), selectedID.split(',')) != -1) {
                            var matchItem = this;
                            for (var i = 0; i < selectedID.split(',').length; i++) {
                                if (selectedID.split(',')[i] == matchItem.ID) {
                                    this.CompanySelection = "true";
                                }
                            }
                        }
                    }
                });
                var Deleteoption = "";
                if (finalSelectedID.length != 0) {
                    if (selectedID == undefined) {
                        selectedID = "";
                    }
                    Deleteoption = $.grep(finalSelectedID.split(','), function (element) {
                        return $.inArray(element, selectedID.split(',')) == -1;
                    });
                    //Deleteoption1 = finalSelectedID.toString().split(',').filter(function (obj) { return selectedID.indexOf(obj) == -1; });
                }
                $(this).find("option").each(function () {
                    var optionVal = $(this).val();
                    if (selectedID != undefined) {
                        if ($.inArray(optionVal.toString(), selectedID.split(',')) == -1) {
                            $(this).prop("selected", false);
                        }
                        else {
                            $(this).prop("selected", true);
                        }
                        //This Condition is for AllControl item selected removed
                        if ($.inArray(optionVal.toString(), Deleteoption.toString().split(',')) != -1) {
                            $(this).remove();
                        }
                    }
                });
                $(this).attr("selectedItem", $(this).val());
            }
        });

        ///button disbale or enable
        var CheckAddButton = false;
        $("#QuickViewUpdateGroupContainer select").each(function () {
            if ($(this).val() == null) {
                CheckAddButton = false;
                return false;
            }
            if ($(this).val().length != $(this).find("option").length) {
                CheckAddButton = true;
                return false;
            }
        });
        if (!CheckAddButton) {
            $("#AddQuickViewGroup").prop('disabled', true).addClass("disable-button");
        }
        else {
            if ($("#QuickViewUpdateGroupContainer select").length != 3) {
                $("#AddQuickViewGroup").prop('disabled', false).removeClass("disable-button");
            }
        }
    }
}
var SaveQuickViewGroupData = function () {
    var QuickViewData = new Object();
    QuickViewData.IsTotal = $("#RBTotalRate").prop("checked");
    QuickViewData.IsEmailNotification = $("#EmailNotification").prop("checked");
    QuickViewData.PickupTime = $("#shoppickuptime .hidden li.selected").eq(0).attr("val");
    QuickViewData.DropOffTime = $("#dropofftime .hidden li.selected").eq(0).attr("val");

    QuickViewData.GroupData = [];
    if (QuickViewId == 0) {
        //    //insert case
        $("#QuickViewGroupContainer table").each(function () {
            if ($(this).find("select option").length != 0) {
                var GroupObject = new Object();
                GroupObject.CompetitorIds = $(this).find("select").attr("selectedItem");
                GroupObject.QuickViewCarClassGroups = [];
                for (var i = 1; i <= 2; i++) {
                    var carClassGroup = new Object();
                    //carClassGroup.GapValueGroup = $(this).find("input:text[id=GapValueGroup_" + i + "]").val();
                    carClassGroup.DeviationValueGroup = $(this).find("input:text[id=DeviationValueGroup_" + i + "]").val();
                    GroupObject.QuickViewCarClassGroups.push(carClassGroup);
                }
                QuickViewData.GroupData.push(GroupObject);
            }
        });
    }
    else {
        //update case
        $("#QuickViewUpdateGroupContainer table").each(function (index) {

            //if ($(this).find("select option").length != 0) {

            var deltegroupid = "";
            var GroupObject = new Object();
            var NotAddInGroup = false;//If new group can be add in existing quickview and some how user dont want to add the group. At the moment this value should not applicable in saving logic
            //Competitor operation
            GroupObject.groupid = $(this).attr("groupid");
            GroupObject.CompetitorIds = $(this).find("select").attr("selectedItem");
            if (typeof ($(this).attr("groupid")) != "undefined") {
                if ($(this).find("select option").length == 0) {
                    deltegroupid = $(this).attr("groupid");
                    NotAddInGroup = false;
                }
            }
            else {//this else is use for not if user add new group and dont want it for save.
                if ($(this).find("select option").length == 0) {
                    NotAddInGroup = true;
                }
            }
            if (!NotAddInGroup) {
                GroupObject.DeleteGroupId = deltegroupid;
                var DeleteCompanyIDs = [], AddCompanyIDs = [];
                if ($(this).attr("newadd") == undefined) {
                    GroupObject.IsNewGroup = "false";
                }
                else {
                    GroupObject.IsNewGroup = "true";
                }
                var CompanyID = $(this).find("select").attr("companyids");
                var selectedCompanyID = $(this).find("select").val();
                if (CompanyID != undefined && selectedCompanyID != null) {
                    DeleteCompanyIDs = CompanyID.split(',').filter(function (obj) { return selectedCompanyID.indexOf(obj) == -1; });
                    for (var i = 0; i < selectedCompanyID.length ; i++) {
                        if ($.inArray(selectedCompanyID[i], CompanyID.split(',')) == -1) {
                            AddCompanyIDs.push(selectedCompanyID[i]);
                        }
                    }
                }
                GroupObject.DeleteCompetitorIds = DeleteCompanyIDs.toString();
                GroupObject.AddCompetitorIds = AddCompanyIDs.toString();

                GroupObject.QuickViewCarClassGroups = [];
                for (var i = 1; i <= 2; i++) {
                    var carClassGroup = new Object();
                    carClassGroup.Id = $(this).find("input:hidden[id=carClassGroupId" + i + "]").attr("groupgapid");
                    //carClassGroup.GapValueGroup = $(this).find("input:text[id=GapValueGroup_" + i + "]").val();
                    carClassGroup.DeviationValueGroup = $(this).find("input:text[id=DeviationValueGroup_" + i + "]").val();
                    GroupObject.QuickViewCarClassGroups.push(carClassGroup);
                }
                QuickViewData.GroupData.push(GroupObject);
            }
            // }
        });
    }
    return QuickViewData;
}
var HideShowQuickViewGroupSetting = function (update) {
    if (update) {
        $("#QuickViewUpdateGroupContainer").removeClass("hidden").show();
        $("#QuickViewGroupContainer").hide();
    }
    else {
        $("#QuickViewUpdateGroupContainer").hide();
        $("#QuickViewGroupContainer").show();
    }
}
var ValidateQuickViewGroupTemplateTextboxEvent = function () {
    var Flag = false;
    if (QuickViewId == 0) {
        $("#QuickViewGroupContainer input[type=text]").on("keyup", function () {
            if (!$.isNumeric($.trim($(this).val()))) {
                MakeTagFlashable($(this));
                flag = false;
            }
            else {
                if ($.trim(parseFloat($(this).val())) == 0) {
                    MakeTagFlashable($(this));
                    flag = false;
                }
                else {
                    RemoveFlashableTag($(this));
                    Flag = true;
                }
            }
        });
    }
    else {
        $("#QuickViewUpdateGroupContainer input[type=text]").on("keyup", function () {
            //console.log("update" + $(this).parent().siblings($(this)).attr("IsChanged", "true"));
            if (!$.isNumeric($.trim($(this).val()))) {
                MakeTagFlashable($(this));
                flag = false;
            }
            else {
                if ($.trim(parseFloat($(this).val())) == 0) {
                    MakeTagFlashable($(this));
                    flag = false;
                }
                else {
                    RemoveFlashableTag($(this));
                    Flag = true;
                }
            }
        });
    }
    AddFlashingEffect();
    return Flag;
}
var ValidateQuickViewGroup = function () {
    var flag = false, FlagRightSelectControl = false, FlagGroupTextBox = false, GroupValidationApplication = false;
    if (QuickViewId == 0) {
        $("#QuickViewGroupContainer select").each(function () {
            if ($(this).find("option").length != "0") {
                if ($(this).val() == null) {
                    MakeTagFlashable($(this));
                    FlagRightSelectControl = false;
                }
                else {
                    FlagRightSelectControl = true;
                    RemoveFlashableTag($(this));
                }
            }
            else {
                FlagRightSelectControl = true;
                RemoveFlashableTag($(this));
            }
        })
        $("#QuickViewGroupContainer input[type=text]").each(function () {
            if (!$.isNumeric($.trim($(this).val()))) {
                MakeTagFlashable($(this));
                FlagGroupTextBox = false;
            }
            else {
                if ($.trim(parseFloat($(this).val())) == 0) {
                    MakeTagFlashable($(this));
                    FlagGroupTextBox = false;
                }
                else {
                    RemoveFlashableTag($(this));
                    FlagGroupTextBox = true;
                }
            }
        });
        $("#QuickViewGroupContainer table").each(function () {
            if ($(this).find("select option").length == 0) {
                $(this).find("input:text").each(function () {
                    RemoveFlashableTag($(this));
                });
            }
        });
        if ($("#QuickViewGroupContainer").find(".temp").length > 0) {
            flag = false;
        }
        else {
            flag = true;
        }
    }
    else {
        $("#QuickViewUpdateGroupContainer select").each(function () {
            if ($(this).find("option").length != "0") {
                if ($(this).val() == null) {
                    MakeTagFlashable($(this));
                    FlagRightSelectControl = false;
                }
                else {
                    FlagRightSelectControl = true;
                    RemoveFlashableTag($(this));
                }
            }
            else {
                FlagRightSelectControl = true;
                RemoveFlashableTag($(this));
            }
        });

        $("#QuickViewUpdateGroupContainer input[type=text]").each(function () {
            if (!$.isNumeric($.trim($(this).val()))) {
                MakeTagFlashable($(this));
                FlagGroupTextBox = false;
            }
            else {
                if ($.trim(parseFloat($(this).val())) == 0) {
                    MakeTagFlashable($(this));
                    FlagGroupTextBox = false;
                }
                else {
                    RemoveFlashableTag($(this));
                    FlagGroupTextBox = true;
                }
            }
        });

        $("#QuickViewUpdateGroupContainer table").each(function () {
            if ($(this).find("select option").length == 0) {
                GroupValidationApplication = true;
                $(this).find("input:text").each(function () {
                    RemoveFlashableTag($(this));
                });
            }
        });
        if ($("#QuickViewUpdateGroupContainer").find(".temp").length > 0) {
            flag = false;
        }
        else {
            flag = true;
        }
    }

    return flag;
    //return false;
}
//End Other functions//

//Enhancement Entity
var QuickViewGroupItem = function (data) {
    var self = this;
    self.groupId = data.groupId;
    self.CompetitorIds = data.CompetitorIds
    self.lstQuickViewGroupCompany = $.map(data.lstQuickViewGroupCompany, function (item) { return new QuickViewGroupCompany(item) });
    self.QuickViewCarClassGroups = $.map(data.QuickViewCarClassGroups, function (item) { return new QuickViewCarClassGroup(item) });
}
var QuickViewGroupCompany = function (data) {
    var self = this;
    self.CompanyID = data.CompanyID;
    self.CompanyName = data.CompanyName;
}
var QuickViewCarClassGroup = function (data) {
    var self = this;
    self.groupId = data.groupId;
    self.Id = data.Id;
    //self.GapValueGroup = data.GapValueGroup;
    self.DeviationValueGroup = data.DeviationValueGroup;
}
//End Enhancement Entity
