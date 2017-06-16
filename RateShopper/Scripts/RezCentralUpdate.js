var rezCentralModel;
var loggedInUserID;
var smartSearchLocations;
var globalSelectedLBId = "";

$(document).ready(function () {
    rezCentralModel = new RezCentralModel();
    $('.loader_container_location').hide();
    var objPayload = new Object();
    ko.applyBindings(rezCentralModel);

    loggedInUserID = $('#LoggedInUserId').val();
    if ($("#hdnIsTSDUpdateAccess").val() != "True") {
        $("#updaterezcentral").hide();
    }

    BindLocations();
    $("#updaterezcentral").on("click", function () {
        if (ValidateFields()) {
            UpdateRates();
        }
    });

    $("#reset").on("click", function () {
        Reset();
    });

    $("#preload").on("click", function () {
        if (ValidateFieldsForPreload()) {
            PreloadRates();
        }
    });

    var ValidateFieldsForPreload = function () {
        var startDate = $("#runStartDate").val();
        if (startDate == "") {
            ShowConfirmBox('Please Select Start Date', false);
            return false;
        }
        if ($("select[id=locations]").val() == null) {
            ShowConfirmBox('Please Select Location', false);
            return false;
        }
        else {
            if ($("select[id=locations]").val().length > 1) {
                ShowConfirmBox('Select Only One Location', false);
                return false;
            }
        }
        if ($("select[id=lengths]").val() == null) {
            ShowConfirmBox('Please Select Rate Code', false);
            return false;
        }
        else {
            if ($("select[id=lengths]").val().length > 1) {
                ShowConfirmBox('Select Only One Rate Code', false);
                return false;
            }
        }
        if ($("select[id=System]").val() == null) {
            ShowConfirmBox('Please Select Rate Source', false);
            return false;
        }
        return true;
    }

    var PreloadRates = function () {
        var id = $("select[id=locations]").val();
        objPayload.RateSource = $("select[id=System]").val();
        objPayload.LocationCode = $('#locations option:selected').attr('locationCode');
        objPayload.RateCode = $("select[id=lengths]").val()[0];
        objPayload.RequestDate = $("#runStartDate").val();
        PullRates(objPayload);
    }

    var PullRates = function (objPayload) {
        $('.loader_container_location').show();
        var ajaxURl = '/RezCentral/PullRates/';
        if (RezCentralURLSettings != undefined && RezCentralURLSettings != '') {
            ajaxURl = RezCentralURLSettings.PullRates;
        }
        $.ajax({
            url: ajaxURl,
            type: 'POST',
            data: JSON.stringify({ 'objPayload': objPayload }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (data) {
                $('.loader_container_location').hide();
                ShowPreloadRates(data);
            },
            error: function (e) {
                $('.loader_container_location').hide();
                console.log("error");
                console.log(e.message);
            }
        });
    }
    var ShowPreloadRates = function (data) {
        if (data.length > 0) {
            $.each(data, function (index, value) {
                if ($("#DailyLeft_" + value.CarClass + ':visible').length > 0) {
                    if (value.DailyRate > 0)
                        $.trim($("#DailyLeft_" + value.CarClass).val(value.DailyRate));
                }
                if ($("#WeeklyLeft_" + value.CarClass + ':visible').length > 0) {
                    if (value.WeeklyRate > 0)
                        $.trim($("#WeeklyLeft_" + value.CarClass).val(value.WeeklyRate));
                }
                if ($("#DailyRight_" + value.CarClass + ':visible').length > 0) {
                    if (value.DailyRate > 0)
                        $.trim($("#DailyRight_" + value.CarClass).val(value.DailyRate));
                }
                if ($("#WeeklyRight_" + value.CarClass + ':visible').length > 0) {
                    if (value.WeeklyRate > 0)
                        $.trim($("#WeeklyRight_" + value.CarClass).val(value.WeeklyRate));
                }
            });
        }
        else {
            ShowConfirmBox('No Rates Found for the search', false);
        }
    }

    $('#runEndDate').datepicker({
        minDate: 0,
        numberOfMonths: 2,
        dateFormat: 'mm/dd/yy'
    }).datepicker("setDate", new Date());
    $('#runStartDate').datepicker({
        minDate: 0,
        numberOfMonths: 2,
        dateFormat: 'mm/dd/yy',
        onSelect: function (selectedDate) {
            var startDate = $('#runStartDate').datepicker('getDate');
            var dSelectedDate = new Date(selectedDate);
            var maxDate = new Date(selectedDate);
            // month is December, we need to roll over to January of the next year
            if (dSelectedDate.getMonth() == 11) {
                maxDate.setMonth(0);
                maxDate.setFullYear(maxDate.getFullYear() + 1);
            }
            else {
                maxDate.setMonth(maxDate.getMonth() + 1);
            }
            $('#runEndDate').datepicker('option', { defaultDate: selectedDate, minDate: dSelectedDate });
            $("#runEndDate").datepicker("setDate", selectedDate);
        }
    }).datepicker("setDate", new Date());


    $(".UpdateAll_weekly_large").selectable({
        filter: "li",
        selected: function (event, ui) {
            var $selected = $(ui.selected);
            if ($selected.hasClass('selected')) {
                $selected.removeClass('selected');
            } else {
                $selected.addClass("selected");
            }

        },
        stop: function (event, ui) {
            if ($(this).children().not('.selected').length == 0) {
                $(this).siblings('.UpdateAll_weeklyAll').prop('checked', true);
                if ($(this).siblings('.UpdateAll_weeklyAll').length > 0) {
                    $("#DivSecondTable #TSDUpdateRezRightAll,#DivSecondTable #RezWeekdaysSelection").hide();
                }
            }
            else {
                $(this).siblings('.UpdateAll_weeklyAll').prop('checked', false);
                if (!$("#openEndDate").prop("checked")) {
                    $("#DivSecondTable #TSDUpdateRezRightAll,#DivSecondTable #RezWeekdaysSelection").show();
                }
            }
        },
    });
    $('.UpdateAll_weeklyAll').click(function () {
        if ($(this).is(':checked')) {
            $(this).siblings('ul').children().addClass('selected');
            $(this).siblings('select').children().prop('selected', true);
            $(this).closest(".days_section").find(".selectdays_row2 .UpdateAll_weekly_large .selected").removeClass("selected").removeClass("ui-selected");

            if (!$("#openEndDate").prop("checked")) {
                $("#DivSecondTable #TSDUpdateRezRightAll,#DivSecondTable #RezWeekdaysSelection").hide();
            }
        }
        else {
            $(this).siblings('ul').children().removeClass('selected');
            $(this).siblings('select').children().prop('selected', false);
            $("#DivSecondTable #RezWeekdaysSelection .selected").removeClass('selected');

            if (!$("#openEndDate").prop("checked")) {
                $("#DivSecondTable #TSDUpdateRezRightAll,#DivSecondTable #RezWeekdaysSelection").show();
            }
        }

    });
    $("#openEndDate").on("click", function () {
        if ($(this).prop("checked")) {
            $("#DivSecondTable #RezWeekdaysSelection").find("li").removeClass('selected');
            $("#DivSecondTable #TSDUpdateRezRightAll,#DivSecondTable #RezWeekdaysSelection").hide();
            $("#DivFirstTable #RezWeekdaysSelection").hide();
            $("#DivFirstTable #RezWeekdaysSelection input:checkbox").prop("checked", false);
            $("#DivFirstTable #RezWeekdaysSelection li").removeClass("selected");
        }
        else {
            if (!$('.UpdateAll_weeklyAll').prop("checked")) {
                $("#DivSecondTable #TSDUpdateRezRightAll,#DivSecondTable #RezWeekdaysSelection").show();
            }
            $("#DivFirstTable #RezWeekdaysSelection").show();
        }
    });

    $('#searchLocation').bind('input', function () {
        SearchGrid('#searchLocation', '#locations option');
        if ($("#locations option[style$='display: none;']").length == $("#locations option").length) {
            MakeTagFlashable('#searchLocation');
        }
        else {
            RemoveFlashableTag('#searchLocation');
        }
        AddFlashingEffect();
    });
});

//Model Class
//ViewModel for RuleSet
function RezCentralModel() {
    var self = this;
    self.CarClassGridLeft = ko.observableArray([]);
    self.CarClassGridRight = ko.observableArray([]);
    self.Location = ko.observableArray([]);
}
//End Model Class

//Entity Class
function CarClassEntity(data) {
    this.CarClassID = data.ID;
    this.CarClassCode = data.Code;
    this.CarClassOrder = data.CarClassOrder;
}
function locations(data) {
    this.Alias = data.Alias;
    this.BrandLocationId = data.BrandLocationId;
    this.BrandId = data.BrandID;
    this.LocationCode = data.LocationCode;
}
//End Entity Class


//Ajax call functions

var GetLocationSpecificCarClassGrid = function (locationBrandId) {
    if (locationBrandId != null && locationBrandId != '') {
        var ajaxURl = '/RateShopper/RezCentral/GetLocationCarClasses/';
        if (RezCentralURLSettings != undefined && RezCentralURLSettings != '') {
            ajaxURl = RezCentralURLSettings.GetLocationCarClasses;
        }
        $.ajax({
            url: ajaxURl,
            data: { locationBrandId: locationBrandId },
            type: 'GET',
            async: true,
            success: function (data) {
                if (data) {
                    var SplitCarClass = (data).split(',');
                    $("#TSDUpdateRezLeftAll tbody tr").each(function () {
                        var CarClassID = $(this).attr("carclassid");
                        if ($.inArray(CarClassID, SplitCarClass) != -1) {
                            $(this).show();
                            $("#TSDUpdateRezRightAll tbody tr[carclassid=" + CarClassID + "]").show();
                        }
                        else {
                            $(this).hide();
                            $("#TSDUpdateRezRightAll tbody tr[carclassid=" + CarClassID + "]").hide();
                        }
                    });
                    TextboxBlurEvent();
                }
            },
            error: function (e) {
                console.log("GetLocationSpecificCarClasses: " + e.message);
            }
        });
    }
}
var BindLocations = function () {
    var ajaxURl = '/RateShopper/RezCentral/GetLocations/';
    if (RezCentralURLSettings != undefined && RezCentralURLSettings != '') {
        ajaxURl = RezCentralURLSettings.GetLocations;
    }

    $.ajax({
        url: ajaxURl,
        data: { userId: loggedInUserID },
        type: 'GET',
        success: function (data) {
            //$('.loader_container_location').hide();
            if (data) {
                var srcs = $.map(data, function (item) { return new locations(item); });
                smartSearchLocations = data;
                rezCentralModel.Location(srcs);
                //Default first location is selected with triggered
                $("#locations option:first").prop("selected", true);
                globalSelectedLBId = $("#locations option").eq(0).val();
                GetLocationSpecificCarClassGrid($("#locations option").eq(0).val().toString());
                // $("#recentLocations ul li").eq(0).addClass("selected");
            }
        },
        error: function (e) {
            //$('.loader_container_location').hide();
            console.log(e.message);
        }
    });
}

var PushRates = function (objRezCentral) {
    var ajaxURl = '/RateShopper/RezCentral/PushRates/';
    if (RezCentralURLSettings != undefined && RezCentralURLSettings != '') {
        ajaxURl = RezCentralURLSettings.PushRates;
    }
    $("#rateupdatenotification").html('');
    $.ajax({
        url: ajaxURl,
        type: 'POST',
        data: JSON.stringify({ 'objRezCentralDTO': objRezCentral }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            if (data.IsPushSuccess) {
                $("#rateupdatenotification").html("Last rates update for " + data.message);
            }
            else {
                $("#rateupdatenotification").html("Rates not updated, kindly check rate code settings");
            }
        },
        error: function (e) {
            console.log(e.message);
        }
    });
}
//End Ajax call functions



//Other Function 
var ValidateFields = function () {
    var Code = false, Branch = false, weekDays = false, notConfiguredRate = false, leftGridTextboxBlank = false, rightGridTextboxBlank = false, weekdaysAll = false, openEndDateCheck = false; message = "";
    if ($("#hdnIsTSDUpdateAccess").val() != "True") {
        ShowConfirmBox('You do not have access to update the rates', false);
        return false;
    }
    if ($("select[id=lengths]").val() != null) {
        RemoveFlashableTag("#lengths");
    }
    else {
        MakeTagFlashable("#lengths");
        Code = true;
    }
    if ($("select[id=locations]").val() != null) {
        RemoveFlashableTag("#locations");
    }
    else {
        MakeTagFlashable("#locations");
        Branch = true;
    }

    var rightGridDays = $("#rightGridDays li.selected").map(function () { return this.value }).get();
    var leftGridDays = $("#leftGridDays li.selected").map(function () { return this.value }).get()

    var leftGridTextboxCount = $("#DivRezRateSection #DivFirstTable input:text").length;
    var rightGridTextboxCount = $("#DivRezRateSection #DivSecondTable input:text:visible").length;
    var lefttablecount = 0, righttablecount = 0; //used for get the no data filed in any textbox
    openEndDateCheck = $("#openEndDate").prop("checked");
    if ($("#UpdateRezCentralAll").prop("checked")) {
        weekdaysAll = true;
        $("#TSDUpdateRezLeftAll input:text").each(function () {
            if ($.isNumeric($(this).val()) || $(this).val().trim() == '') {
                RemoveFlashableTag($(this));
                if ($(this).val().trim() == '') {
                    lefttablecount++;
                }
            }
            else {
                MakeTagFlashable($(this));
            }
        });
    }
    else {
        var commondays = $.grep(rightGridDays, function (element) {
            return $.inArray(element, leftGridDays) !== -1;
        });
        if (commondays.length > 0) {
            ShowConfirmBox('Please select unique days', false);
            return false;
        }
        else {
            $("#DivRezRateSection #DivFirstTable input:text").each(function () {
                if ($.isNumeric($(this).val()) || $(this).val().trim() == '') {
                    RemoveFlashableTag($(this));
                    if ($(this).val().trim() == '') {
                        lefttablecount++;
                    }
                }
                else {
                    MakeTagFlashable($(this));
                }
            });
            $("#DivRezRateSection #DivSecondTable input:text:visible").each(function () {
                if ($.isNumeric($(this).val()) || $(this).val().trim() == '') {
                    RemoveFlashableTag($(this));
                    if ($(this).val().trim() == '') {
                        righttablecount++;
                    }
                }
                else {
                    MakeTagFlashable($(this));
                }
            });
        }
    }

    if (leftGridTextboxCount == lefttablecount) {
        MakeTagFlashable($("#DivRezRateSection"));
    }
    else {
        RemoveFlashableTag($("#DivRezRateSection"));
    }

    if (leftGridDays.length > 0 || rightGridDays.length > 0) {
        var startDate = $("#runStartDate").val();
        var stopDate = $("#runEndDate").val();
        if (startDate != "" && stopDate != "") {
            var currentDate = new Date(startDate);
            stopDate = new Date(stopDate);
            var isDateAndDaysMatch = false;
            while (currentDate <= stopDate) {
                //also check for selected days for overlapping     
                if ($.inArray(currentDate.getDay(), leftGridDays) > -1 || $.inArray(currentDate.getDay(), rightGridDays) > -1) {
                    isDateAndDaysMatch = true;
                    break;
                }
                currentDate.setDate(currentDate.getDate() + 1);
            }
            if (!isDateAndDaysMatch) {
                ShowConfirmBox('Selected days and dates are not matching', false);
                return false;
            }
        }
        else {
            ShowConfirmBox('Please select start and end date', false);
            return false;
        }
    }

    if (!$("#openEndDate").prop("checked") && $("#DivFirstTable #RezWeekdaysSelection").find(".selected").length == 0) {
        MakeTagFlashable($("#DivFirstTable #RezWeekdaysSelection"));
        weekDays = true;
    }
    else if ($("#DivSecondTable #RezWeekdaysSelection li:visible.selected").length != 0 && rightGridTextboxCount == righttablecount) {
        MakeTagFlashable($("#DivRezRateSection"));
        message = "Please configure rates in right grid";
        notConfiguredRate = true;
    }
    else if ($("#DivSecondTable #RezWeekdaysSelection li:visible.selected").length == 0 && rightGridTextboxCount != righttablecount) {
        MakeTagFlashable($("#DivRezRateSection"));
        weekDays = true;
    }

    else {
        RemoveFlashableTag($("#DivSecondTable #RezWeekdaysSelection"));
        RemoveFlashableTag($("#DivFirstTable #RezWeekdaysSelection"));
    }

    AddFlashingEffect();
    if ($(".pageminht").find('.temp').length <= 0) {
        return true;
    }
    else {
        if (Code && weekDays && Branch) {
            message = 'Please select Branch,Code and Weekdays.';
        }
        else if (Code) {
            message = 'Please select Code';
        }
        else if (weekDays) {
            message = 'Please select Weekdays';
        }
        else if (Branch) {
            message = 'Please select Branches';
        }
        //console.log(leftGridTextboxCount + "    " + lefttablecount + " ---- " + rightGridTextboxCount + " " + righttablecount);
        if (weekdaysAll || openEndDateCheck) {
            if (leftGridTextboxCount == lefttablecount) {
                message = "Please configure rates";
                notConfiguredRate = true;
            }
        }
        else {
            if (leftGridTextboxCount == lefttablecount && rightGridTextboxCount == righttablecount) {
                message = "Please configure rates";
                notConfiguredRate = true;
            }
            else if (leftGridTextboxCount == lefttablecount) {
                message = "Please configure rates in left grid";
                notConfiguredRate = true;
            }
        }
        //console.log(Code + " " + weekDays + " " + notConfiguredRate);
        if (Code || weekDays || Branch || notConfiguredRate) {
            ShowConfirmBox(message, false);
        }
        else {
            message = "Please review the fields highlighted in Red.";
            ShowConfirmBox(message, false);
        }
        return false;
    }
}
var TextboxBlurEvent = function () {
    $("#DivRezRateSection input:text:visible").bind("input", function () {
        if ($.isNumeric($(this).val()) || $(this).val().trim() == '') {
            RemoveFlashableTag($(this));
        }
        else {
            MakeTagFlashable($(this));
        }
    })
}
var GetLocationSpecificCarClasses = function (locationBrandId) {
    if ($(locationBrandId).val() != null) {
        globalSelectedLBId = ($(locationBrandId).val().toString());
        GetLocationSpecificCarClassGrid(globalSelectedLBId);
    }
}
var SearchGrid = function (inpuText, controlId) {
    if ($(inpuText).val().length > 0) {
        var matches = $.map(smartSearchLocations, function (item) {
            if (item.Alias.toUpperCase().indexOf($(inpuText).val().toUpperCase()) == 0) {
                return new locations(item);
            }
        });
        rezCentralModel.Location(matches);

    } else {
        var srcs = $.map(smartSearchLocations, function (item) { return new locations(item); });
        rezCentralModel.Location(srcs);
        $(globalSelectedLBId.split(',')).each(function () {
            $("#locations option[value=" + this + "]").prop("selected", true);
        });
        GetLocationSpecificCarClassGrid($("select[id=locations]").val().toString());
    }
}

var UpdateRates = function () {
    var objRezCentral = new Object();
    objRezCentral.StartDate = $("#runStartDate").val();
    objRezCentral.EndDate = $("#runEndDate").val();
    objRezCentral.IsOpenEndedRates = $("#openEndDate").prop("checked");
    objRezCentral.Days1 = $.map($("#leftGridDays li.selected"), function (element) {
        return $(element).attr("value");
    }).join(',');
    objRezCentral.Days2 = $.map($("#rightGridDays li.selected"), function (element) {
        return $(element).attr("value");
    }).join(',');

    objRezCentral.Locations = new Array();
    $("select#locations option:selected").each(function () {
        var objLocation = new Object();
        objLocation.LocationBrand = this.text;
        objLocation.LocationBrandId = this.value;
        objLocation.Location = $(this).text().split('-').length > 0 ? $(this).text().split('-')[0] : '';
        objRezCentral.Locations.push(objLocation);
    });

    objRezCentral.RateCodes = $("select#lengths option:selected").map(function () { return this.value }).get().join(",");
    objRezCentral.System = $("select#System").val();

    var rates = new Array();
    $("#TSDUpdateRezLeftAll tbody tr").each(function (index) {
        var carClassId = $(this).attr("carclassid");
        var carCode = $(this).attr("carcode");
        var $dailyLeftRate = $.trim($("#DailyLeft_" + carCode).val());
        var $weeklyLeftRate = $.trim($("#WeeklyLeft_" + carCode).val());
        var $dailyRightRate = $.trim($("#DailyRight_" + carCode + ":visible").val());
        var $weeklyRightRate = $.trim($("#WeeklyRight_" + carCode + ":visible").val());

        if ($dailyLeftRate.length > 0 || $weeklyLeftRate.length > 0 || $dailyRightRate.length > 0 || $weeklyRightRate.length > 0) {
            var updateRates = new Object();
            updateRates.CarClass = carCode;
            updateRates.CarClassId = carClassId;
            if ($dailyLeftRate.length > 0) { updateRates.DailyLeftRate = $dailyLeftRate; }
            if ($dailyRightRate.length > 0) { updateRates.DailyRightRate = $dailyRightRate };
            if ($weeklyLeftRate.length > 0) { updateRates.WeeklyLeftRate = $weeklyLeftRate };
            if ($weeklyRightRate.length > 0) { updateRates.WeeklyRightRate = $weeklyRightRate };
            rates.push(updateRates);
        }
    });

    objRezCentral.Rates = rates;
    objRezCentral.UserId = $("#LoggedInUserId").val();
    objRezCentral.Username = $("#hdnUserName").val();

    if (objRezCentral.Rates.length > 0) {
        $("#tsdNotification").show();
        setTimeout(function () { $("#tsdNotification").hide(); }, 3000);
        PushRates(objRezCentral);
    }
}

var Reset = function () {
    $(".temp").removeClass("temp").removeClass("flashBorder");
    $("td input:text").val('');
    $("#DivSecondTable #RezWeekdaysSelection li.selected").removeClass('selected');
    $("#DivSecondTable #TSDUpdateRezRightAll, #DivSecondTable #RezWeekdaysSelection").show();
    $("#DivFirstTable #RezWeekdaysSelection").show();
    $("#DivFirstTable #RezWeekdaysSelection input:checkbox").prop("checked", false);
    $("#DivFirstTable #RezWeekdaysSelection li.selected").removeClass("selected");

    var currentDate = new Date();
    $("#runStartDate").datepicker("setDate", currentDate);
    $('#runEndDate').datepicker('option', { defaultDate: currentDate, minDate: currentDate });
    $("#runEndDate").datepicker("setDate", currentDate);

    $("#openEndDate").prop("checked", false);
    $("#lengths option:selected").removeAttr("selected");
    if ($("#searchLocation").val().length > 0) {
        $("#searchLocation").val('');
        SearchGrid('#searchLocation', '#locations option');
    }
}
//End Other Function