var ScheduleNewJobViewModel, smartSearchLocations, jobId, GlobalTetherSetting;
//Save click
var locationIds = [];
var locationBrandIds = [];
var sourceIds = [], carClassIds = [], rentalLengthIds = [], jobScheduleWeekDays = [], sendUpdateWeekDays = [];
var sources = [], carClassArr = [], locationArr = [];
var selectedAPICode = '';
var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
//Save click end
$(document).ready(function () {

    setCustomDropdownKeyPress();
    DisableTSDUpdateAccess();
    ScheduleNewJobViewModel = new ScheduleNewJobModel();
    ko.applyBindings(ScheduleNewJobViewModel, document.getElementById('view1'));

    LoggedInUserID = $('#LoggedInUserId').val();
    initializeControlEvents();
    bindControls();
    showHideTab('view2', 'view1');
    $("input[name=rulesetfilter][value=RdTotalCost]:radio").prop("checked", true);///Automation Console enhacement changes
    $("#RulesetCompeteSelection").hide();
    $("#ddlAPI ul.hidden.drop-down").find('li').eq(0).addClass('selected').closest('#ddlAPI').find('li').eq(0).attr({ 'value': ($('#ddlAPI ul.hidden.drop-down').find('li').eq(0).attr('value')), 'prvcode': ($('#ddlAPI ul.hidden.drop-down').find('li').eq(0).attr('prvcode')) }).text($("#ddlAPI ul.hidden.drop-down").find('li').eq(0).text());

    var prvcode = $("select#source option:selected").attr("prvcode");
    if (typeof (prvcode) != 'undefined') {
        EnableDisableMultipleLOR(prvcode, $("select#source option:selected").attr("isgov"));
        EnableDisableGovTemplate();
        //LoadScrapperSource(selectedAPI);
    }
    $("select#source").change(function () {
        EnableDisableMultipleLOR($("select#source option:selected").attr("prvcode"), $("select#source option:selected").attr("isgov"));
        //LoadScrapperSource($(this).attr("value"));
        EnableDisableGovTemplate();
    });
    //job schedule section
    $('.frequency input[type="radio"]').click(function () {
        frequencyRadioClicked($(this));
    });
    $('.stand-horizon-box input[type="radio"]').click(function () {
        if ($('#rb-standardShop:checked').length > 0) {
            $('div.standardShopDiv').show();
            $('div.horizonShopDiv').hide();
        }
        else {
            $('div.standardShopDiv').hide();
            $('div.horizonShopDiv').show();
        }
    });

    $('#SaveCreateJob').click(function () {
        SaveClicked();
    });

    $('#pickupHour ul li').bind('click', function () {
        $('#dropOffHour').closest('.select-ul').find('li').removeClass('selected');
        $('#dropOffHour li').eq(0).text($(this).text()).attr('value', ($(this).text()));
        var pickupText = $(this).text();
        $('#dropOffHour ul li').each(function () {
            if ($(this).text() == pickupText) {
                $(this).addClass("selected");
                $('#dropOffHour li').eq(0).text($(this).text()).attr('value', ($(this).text()));
                return false;
            }
            else {
                $(this).removeClass("selected");
            }
        });
    });

    $('#rb-standardShop').click();
    $("#rb-e15min").click();
    InitiateTetherSettings();

    $("#divAutomationRulesetAvailable").hide();
    $('input[name="rulesetfilter"]:checkbox').on("change", function () {
        if (($(this).val() == "WideGap" || $(this).val() == "IsGOV") && $(this).prop("checked")) {
            $("#divAutomationRulesetAvailable").show();
            $("#RulesetCompeteSelection").show();
            //if (!AreRuleSetLoadedAfterLocationChanged) {
            GetLocationRuleSet();
            //}
        }
        else {
            $("#divAutomationRulesetAvailable").hide();
            $("#RulesetCompeteSelection").hide();
            //$("#NotFoundAutomationConsoleRuleSet").hide();
        }
    });
});

//set the type event for dropdown created using li Ul
function setCustomDropdownKeyPress() {
    $(document).keypress(function (e) {
        if ($('.hidden').is(":visible")) {
            //console.log(e.which);
            if (e.which !== 0) {
                var char = String.fromCharCode(e.which);
                //console.log("Charcter was typed. It was: " + char);
                var $hiddenLi = $('.hidden:visible');

                if ($hiddenLi.find('li[value^="' + char + '"]').not('.selected').length > 0) {
                    var $li = null;

                    var selectedIndex = $hiddenLi.find('.selected').index();
                    $hiddenLi.find('li.selected').removeClass('selected');

                    //find next occurance
                    var found = false;
                    $.each($hiddenLi.find('li[value^="' + char + '"]').not('.selected'), function (index, ele) {
                        if ($(ele).index() > selectedIndex) {
                            $li = $(ele).addClass('selected');
                            found = true;
                            return false;
                        }

                    });
                    //next not found then set to first
                    if (!found) {
                        $li = $hiddenLi.find('li[value^="' + char + '"]').eq(0).addClass('selected');
                    }
                    //$hiddenLi.find('li[value^="' + char + '"]').eq(0).addClass('selected');
                    $hiddenLi.siblings('li').eq(0).val($hiddenLi.find('.selected').val()).html($hiddenLi.find('.selected').html());

                    //scroll to front
                    scrollTo = $li;
                    if (scrollTo.length > 0) {
                        $hiddenLi.scrollTop(
                            scrollTo.offset().top - 30 - $hiddenLi.offset().top + $hiddenLi.scrollTop()
                        );
                    }
                }
            }
        }
    });
}


function initializeControlEvents() {
    $('#jobScheduled').on('click', function () {
        showHideTab('view2', 'view1');
    });

    $('#newJob').on('click', function () {
        showHideTab('view1', 'view2');
    });

    //For Lengths checkbox events
    $('#lengths-all').bind('click', function () {

        if ($(this).is(':checked')) {
            $('#lengths-day, #lengths-week').prop('checked', true);
            $('#rentallengthdiv select option').each(function () {
                if (!$(this).is(':disabled')) {
                    var lorvalue = parseInt($(this).attr("value"))
                    if (lorvalue < 9 || lorvalue > 13) {
                        $(this).prop('selected', true);
                    }
                    if ($(this).text() == 'M30' && !$('#lengths-month').prop('checked')) {
                        $(this).prop('selected', false);
                    }
                }
            });
        }
        else {
            $('#lengths-day, #lengths-week').prop('checked', false);
            $('#rentallengthdiv select option').each(function () {
                var lorvalue = parseInt($(this).attr("value"))
                if (lorvalue < 9 || lorvalue > 13) {
                    $(this).prop('selected', false);
                }
                if ($(this).text() == 'M30' && $('#lengths-month').prop('checked')) {
                    $(this).prop('selected', true);
                }
            });

        }

    });

    $('#lengths-day, #lengths-week, #lengths-month').bind('click', function () {
        var $selectedControl = $(this);
        if ($selectedControl.is(':checked')) {
            $('#rentallengthdiv select option').each(function () {
                if ($(this).text().indexOf($selectedControl.next().text()) >= 0 && !$(this).is(':disabled')) {
                    var lorvalue = parseInt($(this).attr("value"))
                    if (lorvalue < 9 || lorvalue > 13) {
                        $(this).prop('selected', true);
                    }
                }
            });
        }
        else {
            $('#rentallengthdiv select option').each(function () {
                if ($(this).text().indexOf($selectedControl.next().text()) >= 0) {
                    var lorvalue = parseInt($(this).attr("value"))
                    if (lorvalue < 9 || lorvalue > 13) {
                        $(this).prop('selected', false);
                    }
                }
            });
        }

        selectAllLengthCheckboxes();
    });

    $('#searchLocation').bind('input', function () {
        searchGrid('#searchLocation', '#locations option');
        if ($("#locations option[style$='display: none;']").length == $("#locations option").length) {
            MakeTagFlashable('#searchLocation');
        }
        else {
            RemoveFlashableTag('#searchLocation');
        }
        AddFlashingEffectAutomation();
    })



    $('#jobTime').timepicker({
        timeFormat: 'hh:mm tt',
        hourGrid: 4,
        minuteGrid: 10,
        stepMinute: 5
    });

    initializeDatePicker();
    initializeTimePicker();

    var isAdmin = $("#hdnIsAdminUser").val();
    if (isAdmin.toUpperCase() == "FALSE") {
        $('#view2 #user').addClass("disable-UL");
    }
}

function bindControls() {
    BindLocations();
    BindCarClasses();
}

function showHideTab(showTab, hideTab) {
    $('#' + showTab).show();
    $('.' + showTab).addClass('selected');

    $('#' + hideTab).hide();
    $('.' + hideTab).removeClass('selected');
}

function initializeDatePicker() {
    $('#startDate.date-picker,#startDateimg').datepicker({
        minDate: 0,
        numberOfMonths: 2,
        dateFormat: 'mm/dd/yy',
        onSelect: function (selectedDate) {
            var startDate = $('#startDate').datepicker('getDate');
            var dSelectedDate = new Date(selectedDate);
            var maxDate = new Date(selectedDate);
            // month is December, we need to roll over to January of the next year
            if (dSelectedDate.getMonth() == 11) {
                maxDate.setMonth(0);
                maxDate.setFullYear(maxDate.getFullYear() + 1);
                //Added extra for FTB job
                //maxDate.setDate(0);
            }
            else {
                //maxDate.setFullYear(maxDate.getFullYear());//= new date(dSelectedDate.getFullYear(), dSelectedDate.getMonth() + 1, 0);
                maxDate.setMonth(maxDate.getMonth() + 1);
                //maxDate.setDate(0);
                //maxDate.setMonth(maxDate.getMonth() + 1);
            }
            var date2 = $('#startDate').datepicker('getDate');
            date2.setDate(date2.getDate() + 3);
            $('#endDate.date-picker').datepicker('option', { defaultDate: date2, minDate: dSelectedDate, maxDate: maxDate });
            $('#endDateimg').datepicker('option', { defaultDate: date2, minDate: dSelectedDate, maxDate: maxDate });

            if ($("#source option:selected").length && $("#locations option:selected").length) {
                DisableOpaqueRatesforGOV();
                GetRateCodes();
            }
            //$("#endDate").datepicker("setDate", date2);
        }
    });
    $('#endDate.date-picker,#endDateimg').datepicker({
        minDate: 0,
        numberOfMonths: 2,
        dateFormat: 'mm/dd/yy',
        onSelect: function () {
            if ($("#source option:selected").length && $("#locations option:selected").length) {
                DisableOpaqueRatesforGOV();
                GetRateCodes();
            }
        }
    });

    $('#runStartDate.date-picker,#runStartDateimg').datepicker({
        minDate: 0,
        numberOfMonths: 2,
        dateFormat: 'mm/dd/yy',
        onSelect: function (selectedDate) {
            var startDate = $('#runStartDate').datepicker('getDate');
            var dSelectedDate = new Date(selectedDate);
            var date2 = $('#runStartDate').datepicker('getDate');
            date2.setDate(date2.getDate() + 3);
            $('#runEndDate.date-picker').datepicker('option', { defaultDate: date2, minDate: dSelectedDate });
            $('#runEndDateimg').datepicker('option', { defaultDate: date2, minDate: dSelectedDate });

            $('#startDate.date-picker').datepicker('option', { defaultDate: date2, minDate: dSelectedDate });
            $('#startDateimg').datepicker('option', { defaultDate: date2, minDate: dSelectedDate });

            $('#endDate.date-picker').datepicker('option', { defaultDate: date2, minDate: dSelectedDate });
            $('#endDateimg').datepicker('option', { defaultDate: date2, minDate: dSelectedDate });

            //$("#runEndDate").datepicker("setDate", date2);
        }
    });
    $('#runEndDate.date-picker,#runEndDateimg').datepicker({
        minDate: 0,
        numberOfMonths: 2,
        dateFormat: 'mm/dd/yy',
    });
}

function initializeTimePicker() {
    $('input[id^=customInput]').change(function () {
        var hours = this.value.split(',');
        var fixed = '';
        $('#' + this.id + 'Select input:checked').each(function () {
            markTimeSelection($(this).parent(), false);
        });
        for (var i = 0; i < hours.length; ++i) {
            var value = parseInt(hours[i])
            if (!isNaN(value)) {
                fixed += ',' + value;
                markTimeSelection($('#' + this.id + 'Select input[value=' + value + ']').parent(), true);
            }
        }
        $(this).val(fixed.substring(1));
    });
    $('[id^=customInput]').click(function () {
        var $this = $(this);
        var pos = $this.position();

        var $this = $(this);
        var pos = $this.position();
        $('#' + this.id + 'Select').css({ left: pos.left, top: pos.top + $this.outerHeight() });
        $('#' + this.id + 'Select').fadeIn(100);
    });
    $('.custom-select label').click(function () {
        markTimeSelection($(this));
        var hours = '';
        $(this).parent().find('input:checked').each(function () {
            hours += ',' + this.value;
        });

        var $input = $(this).parent().parent().prev();
        $input.val(hours.substring(1));
        $input.attr('title', $input.val());
    });
    // hide custom time select dialogs when clicking anywhere in the page
    $('body').click(function (e) {
        if (!$(e.target).parents('.custom-select-hours').length) {
            $('#customInputHoursSelect').fadeOut(100);
        }
        if (!$(e.target).parents('.custom-select-minutes').length) {
            $('#customInputMinutesSelect').fadeOut(100);
        }
        else {
            $('#customInputHoursSelect').fadeOut(100);
        }
    });
}
function markTimeSelection($obj, value) {
    var $option = $obj.children('input');
    if (($option.is(':checked') && typeof (value) == 'undefined') || value) {
        $option.prop('checked', true);
        $obj.addClass('ui-state-hover');
    } else {
        $option.prop('checked', false);
        $obj.removeClass('ui-state-hover');
    }
}

function BindLocations() {
    $('.loader_container_location').show();
    var ajaxURl = 'AutomationConsole/GetLocations/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetLocations;
    }
    $.ajax({
        url: ajaxURl,
        data: { UserId: LoggedInUserID },
        type: 'GET',
        async: true,
        success: function (data) {
            $('.loader_container_location').hide();
            if (data) {
                var srcs = $.map(data, function (item) { return new locations(item); });
                smartSearchLocations = data;
                ScheduleNewJobViewModel.locations(srcs);
                setTimeout(function () { AddAttrToLocations(); }, 200);
            }
        },
        error: function (e) {
            $('.loader_container_location').hide();
            console.log(e.message);
        }
    });
}

function BindCarClasses() {
    $('.loader_container_carclass').show();
    var ajaxURl = 'GetCarClasses/GetCarClasses/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetCarClasses;
    }
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        async: true,
        success: function (data) {
            $('.loader_container_carclass').hide();
            if (data) {
                var srcs = $.map(data, function (item) { return new carClasses(item); });
                ScheduleNewJobViewModel.carClasses(srcs);
                $('#carClasstd select option').bind('click', function () {
                    if ($('#carClass').find('option:selected').length == $('#carClass select option').length) {
                        $('#carClass-all').prop('checked', true);
                    }
                    else {
                        $('#carClass-all').prop('checked', false);
                    }

                });
            }
        },
        error: function (e) {
            $('.loader_container_carclass').hide();
            console.log(e.message);
        }
    });
}

function selectAllLengthCheckboxes() {
    if ($('#lengths-day').prop('checked') && $('#lengths-week').prop('checked')) {
        $('#lengths-all').prop('checked', true);
    }
    else {
        $('#lengths-all').prop('checked', false);
    }
}
function checkUncheckParent(controlId) {
    checkedValCnt = 0, weekCheckedCnt = 0;

    $('#lengths option').each(function () {
        if ($(this).prop('selected') && $(this).text().toUpperCase().indexOf('D') == 0) {
            checkedValCnt += 1;
        }

        if ($(this).prop('selected') && $(this).text().toUpperCase().indexOf('W') == 0) {
            weekCheckedCnt += 1;
        }

        if ($(this).prop('selected') && $(this).text().toUpperCase().indexOf('M') == 0) {
            $('#lengths-month').prop('checked', true);
        }
        else {
            $('#lengths-month').prop('checked', false);
        }
    });

    if (checkedValCnt == 4) {
        $('#lengths-day').prop('checked', true);
    }
    else {
        $('#lengths-day').prop('checked', false);
    }

    if (weekCheckedCnt == 5) {
        $('#lengths-week').prop('checked', true);
    }
    else {
        $('#lengths-week').prop('checked', false);
    }
    selectAllLengthCheckboxes();

}

function checkUncheckCarClasses() {
    if ($('#carClass option:selected').length == $('#carClass option').length) {
        $('#carClass-all').prop('checked', true);

    } else {
        $('#carClass-all').prop('checked', false);
    }

    setTimeout(function () { BindTetherData(); BindOpaqueRate() }, 250);
}

//This function is call when user change or select any source.
function getSourceBaseChangeTetherSettings(source) {
    //this scenario is applicable on GOV shop. Set as zero.
    var isGov = $("#view1 #source select option:selected[value=" + $(source).val() + "]").attr("isgov");
    var isGOV = (isGov == "True");
    if (isGOV && $("#view1 #locations select").val() != null && $("#view1 #locations select").val() != "") {
        BindTetherData();
    }

    //For Only GOV shop tether button should disbale and tether check box should visible to allowing if user want to apply tether setting or not.
    checkGOVTetherButtonHideShow();
}

function GetLocationSpecificCarClasses(locationBrandId) {
    ResetLORs();
    DisableLORs();
    IsEditjobLocationChanged = true;
    AreRuleSetLoadedAfterLocationChanged = false;
    $("#mr_AutoConsoleRuleSet_popup #IntermediateID").val("");
    //If menually changed the location.
    $("#TehterTableValue tbody tr").find("input").val("");
    $("#TehterTableValue tbody tr").find("input").removeClass("flashBorder temp");
    if ($('#wideGap').prop("checked") || $('#IsGov').prop("checked")) {
        GetLocationRuleSet();
    }

    //Set tethering checkbox
    //LocationChanged();
    var searchId = locationBrandId.value, $selectedOpt = $('option:selected', locationBrandId);
    searchId = $selectedOpt.attr('brandId');
    if (searchId != null && searchId != '') {
        $('.loader_container_carclass').show();
        $.ajax({
            url: 'AutomationConsole/GetLocationCarClasses/',
            data: { locationBrandId: searchId },
            type: 'GET',
            async: true,
            success: function (data) {
                if (data) {
                    $('.loader_container_carclass').hide();
                    $("#carClass option").each(function () {
                        if ($.inArray(parseInt($(this).val()), data) == -1)
                            $(this).prop("selected", false);
                        else
                            $(this).prop("selected", true);
                    });

                    if ($('#carClass option:selected').length == $('#carClass option').length) {
                        $('#carClass-all').prop('checked', true);

                    } else {
                        $('#carClass-all').prop('checked', false);
                    }
                    //setTimeout(function () { BindTetherData(); }, 250);
                    getGlobalTetherValueSettingLocationSpecific();
                    BindOpaqueRate();
                    DisableOpaqueRatesforGOV();
                    GetRateCodes();
                }
            },
            error: function (e) {
                console.log("GetLocationSpecificCarClasses: " + e.message);
            }
        });
    }
}
function ResetLORs() {
    //$('#rentallengthdiv select option').each(function () {
    //    $(this).prop({'disabled': false});
    //});
    $('#rentallengthdiv select option:disabled').prop('disabled', false);
    $("#LORcheckboxes input:checkbox").removeAttr('checked');
}
function DisableLORs() {
    var isGov = $("select#source option:selected").attr("isgov");
    if (isGov != "True") {
        var lors = $("#locations option:selected").attr('lor');
        if (typeof (lors) != "undefined" && lors != "") {
            var rentalLength = lors.split(',');
            $.each(rentalLength, function (index, val) {
                $("#lengths select option[value=" + val + "]").prop({ 'disabled': true, 'selected': false });
            })
        }
    }
}
function selectAllCheckBoxClick(caller, elId) {
    if ($(caller).is(':checked')) { // the checkbox was checked, select all elements
        $('#' + elId + ' select option').each(function () {
            $(this).prop('selected', true);
        });
    }
    else { // the checkbox was unchecked, deselect all elements
        $('#' + elId + ' select option').each(function () {
            $(this).prop('selected', false);
        });
    }
}

function checkUncheckWeekDays(ctrl) {
    if ($('#' + ctrl + ' option:selected').length == $('#' + ctrl + ' option').length) {
        $('.' + ctrl).prop('checked', true);

    } else {
        $('.' + ctrl).prop('checked', false);
    }
}

function searchGrid(inpuText, controlId) {
    if ($(inpuText).val().length > 0) {
        var matches = $.map(smartSearchLocations, function (item) {
            if (item.LocationBrandAlias.toUpperCase().indexOf($(inpuText).val().toUpperCase()) == 0) {
                return new locations(item);
            }
        });
        ScheduleNewJobViewModel.locations(matches);

    } else {
        //$(controlId).show();
        var srcs = $.map(smartSearchLocations, function (item) { return new locations(item); });
        ScheduleNewJobViewModel.locations(srcs);
    }
    AddAttrToLocations();
}

//Knockout 
function ScheduleNewJobModel() {
    var self = this;
    self.locations = ko.observableArray([]);
    self.carClasses = ko.observableArray([]);
}

function locations(data) {
    this.LocationID = data.LocationID;
    this.Location = data.LocationBrandAlias;
    this.LocationBrandID = data.ID;
    this.LocationCode = data.LocationCode;
    this.BrandId = data.BrandID;
    this.LOR = data.LOR;
}

function carClasses(data) {
    this.ID = data.ID;
    this.Code = data.Code;
}
//Knockout end

//JOb Schedule Section
function frequencyRadioClicked(sender) {
    $('.customTime, .runTime, .runDay, .runDayofMonth, .daysTodays').hide();
    $('.jobScheduleWeekDays').attr('multiple', 'multiple');
    $('input[type="checkbox"].jobScheduleWeekDays').removeAttr('disabled');
    $('.daysTodays').show();

    switch (sender.attr('id')) {

        case 'rb-e1day':
            $('.runTime').show();
            $('#jobTime').val('12:00 am');
            break;
        case 'rb-e1week':
            $('.runTime').val('12:00 am').show();
            $('.jobScheduleWeekDays').removeAttr('multiple');
            $('#jobTime').val('12:00 am');
            $('input[type="checkbox"].jobScheduleWeekDays').prop('checked', false).attr('disabled', 'disabled')
            break;
        case 'rb-e1month':
            $('.daysTodays').hide();
            $('.runTime').show();
            $('.runDayofMonth').show();
            $('#jobTime').val('12:00 am');
            $('#jobDay').val('1');
            break;
        case 'rb-custom-time':
            $('.customTime').show();
            break;
    }
}
//JOb Schedule Section end
function resetFormSelection(clearForm) {
    var today = new Date();
    $('#startDate,#startDateimg').datepicker('option', { defaultDate: today, minDate: 0, maxDate: null });
    $('#endDate,#endDateimg').datepicker('option', { defaultDate: today, minDate: 0, maxDate: null });
    $('#runEndDate.date-picker').datepicker('option', { defaultDate: today, minDate: 0, maxDate: null });
    $('#rb-standardShop').prop('checked', true).trigger('click');
    $("#rb-e15min").trigger('click');

    $('#view1').find('*').removeClass('flashborder').removeClass('temp');
    $('#error-span').hide();
    RemoveFlashableTag('.has-error');
    $('.has-error').removeClass('has-error');
    $('#view1 input[type="text"]').val('');
    $('#view1 input[type="number"]').val('');
    $('#view1 select').val('')
    $('#view1 input:checkbox').removeAttr('checked');
    $("#activeTethering").prop('disabled', true);
    $("#activeOpaqueRate").prop('disabled', true);
    $("#mr_Opaque_popup #ratecodes option:selected").prop("selected", false);
    $("#mr_Opaque_popup #RateCodeAll").prop("checked", false);
    //$("#mr_Opaque_popup #ratecodes option[value=OPAQUE],#mr_Opaque_popup #ratecodes option[value=EN1]").prop("selected", true);

    $('#jobTime').val('12:00 am');
    $("#jobDay").val($("#jobDay option:first").val());
    $('#pickupHour li').eq(0).attr('value', '11:00 am').text("11:00am");
    $('#dropOffHour li').eq(0).attr('value', '11:00 am').text("11:00am");
    $('#pickupHour ul li').removeClass('selected');
    $('#dropOffHour ul li').removeClass('selected');

    searchGrid('#searchLocation', '#locations option');
    $('div[id^=customInput] .ui-state-hover').removeClass('ui-state-hover');
    $("#TetherRate").prop('disabled', true).addClass("disable-button");
    $("#btnOpaqueRate").prop('disabled', true).addClass("disable-button");
    $("#divAutomationRulesetAvailable").hide();
    $("#NotFoundAutomationConsoleRuleSet").hide();
    $("#view1 input[type='radio'][value='Standard']").prop("checked", true);
    $("input[name=rulesetfilter][value=RdTotalCost]:radio").prop("checked", true);
    $("#RulesetCompeteSelection").hide();
    //min rate popup items
    FlagSaveData = false;
    firstClickEditData = false;
    FirstClickOpaqueRatePopup = false;
    IsEditjobLocationChanged = false; //Reset the edit job location setting flag
    if (jobId === undefined || jobId === "") {
        FinalMinRateData = [];
    }
    //reset job to previous state if in edit mode
    if ((clearForm == undefined || clearForm == null) && jobId != null && jobId != '') {
        JobSelected(jobId);
    }
    else {
        //var currentAPI = $("#ddlAPI li:first").attr("value");
        //var firstOption = $("#ddlAPI ul.hidden li:first").attr("value");
        //if (currentAPI != firstOption) {
        //    $("#ddlAPI ul.hidden.drop-down li.selected").removeClass('selected');
        //    $("#ddlAPI ul.hidden.drop-down li[value='" + firstOption + "']").addClass('selected').closest('#ddlAPI').find('li').eq(0).attr({ 'value': ($("#ddlAPI ul.hidden.drop-down li[value='" + firstOption + "']").attr('value')), 'prvcode': ($("#ddlAPI ul.hidden.drop-down li[value='" + firstOption + "']").attr('prvcode')) }).text($("#ddlAPI ul.hidden.drop-down li[value='" + firstOption + "']").text());
        //    LoadScrapperSource(firstOption);
        //    EnableDisableMultipleLOR($("#ddlAPI ul.hidden.drop-down li[value='" + firstOption + "']").attr("prvcode"));
        //}
        EnableDisableMultipleLOR('SS', "False");
        EnableDisableGovTemplate();
        ResetLORs();
    }
}

function SaveClicked() {

    sourceIds = [], carClassIds = [], rentalLengthIds = [], sources = [], carClassArr = [], locationArr = [], locationBrandIds = [], locationIds = [];
    jobScheduleWeekDays = [], sendUpdateWeekDays = []; selectedAPICode = '';

    RemoveFlashableTag('.has-error');
    $('.has-error').removeClass('has-error');

    $('.jobScheduleWeekDays option:selected').each(function () {
        jobScheduleWeekDays.push($(this).attr('value'));
    });

    if ($("#hdnIsTSDUpdateAccess").length == 0 || $("#hdnIsTSDUpdateAccess").val() == "True") {
        $('.sendUpdateWeekDays').find('option:selected').each(function () {
            sendUpdateWeekDays.push($(this).attr('value'));
        });
    }

    $('#locations ul option:selected').each(function () {
        locationBrandIds.push($(this).attr('BrandId'));
        locationIds.push($(this).attr('value'));
        locationArr.push($(this).attr('LocationCode'));
    });
    locationBrandIds.length == 0 ? addErrorToField("locations") : removeErrorToField("locations");

    $('#source option:selected').each(function () {
        sourceIds.push($(this).attr('value'));
        sources.push($(this).attr('srcCode'));
        selectedAPICode = $(this).attr('prvcode');
    });
    sourceIds.length == 0 ? addErrorToField("source") : removeErrorToField("source");
    !selectedAPICode ? addErrorToField("source") : removeErrorToField("source");

    $('#lengths option:selected').each(function () {
        var lorvalue = parseInt($(this).attr('value'));
        if (lorvalue < 9 || lorvalue > 13) {
            rentalLengthIds.push($(this).attr('value'));
        }
    });
    rentalLengthIds.length == 0 ? addErrorToField("lengths") : removeErrorToField("lengths");

    $('#carClass option:selected').each(function () {
        carClassIds.push($(this).attr('value'));
        carClassArr.push($(this).text());
    });
    carClassIds.length == 0 ? addErrorToField("carClass") : removeErrorToField("carClass");

    //selectedAPICode = $("#ddlAPI ul.hidden li.selected").attr("prvcode");
    //if (typeof (selectedAPICode) == 'undefined') {
    //    selectedAPICode = $("#ddlAPI ul li:first").attr("prvcode");
    //}

    //if (typeof (selectedAPICode) == 'undefined' || selectedAPICode == '') {
    //    addErrorToField("ddlAPI")
    //}

    $('#runStartDate').val() == '' ? addErrorToField("runStartDate") : removeErrorToField("runStartDate");
    $('#runEndDate').val() == '' ? addErrorToField("runEndDate") : removeErrorToField("runEndDate");

    if ($('#startDate').val() != '' && $('#endDate').val() != '') {
        var startDate = new Date($('#startDate').val());
        var endDate = new Date($('#endDate').val());
        //check if startDate is less than Current Date
        var currentDate = new Date();
        currentDate.setHours(0, 0, 0, 0);
        if (startDate < currentDate) {
            addErrorToField('startDate');
        }
        else if (endDate < startDate) {
            addErrorToField('startDate'); addErrorToField('endDate');
        }
        else { removeErrorToField('startDate'); removeErrorToField('endDate'); }
    }

    if ($('#runStartDate').val() != '' && $('#runEndDate').val() != '') {
        var startDate = new Date($('#runStartDate').val());
        var endDate = new Date($('#runEndDate').val());
        var shopEndDate = new Date($('#endDate').val());
        //check if startDate is less than Current Date
        var currentDate = new Date();
        currentDate.setHours(0, 0, 0, 0);
        if (startDate < currentDate) {
            addErrorToField('runStartDate');
        }
        else if (endDate < startDate) {
            addErrorToField('runStartDate'); addErrorToField('runEndDate');
        }
        else { removeErrorToField('runStartDate'); removeErrorToField('runEndDate'); }
        if (endDate > shopEndDate) {
            addErrorToField('endDate'); addErrorToField('runEndDate');
        }
        else { removeErrorToField('runStartDate'); removeErrorToField('runEndDate'); removeErrorToField('endDate'); }
    }

    var selectedFreq = $('input[name=freq]:checked', '.frequency').attr('id');
    var ctrlToValidate = selectedFreq.replace('rb-', '.');
    $.each($(ctrlToValidate), function (i, o) {
        var $ctrl = $(o);
        if ($ctrl.val() == null || $ctrl.val() == undefined || $ctrl.val() == '') {
            if ($ctrl.prop('type').indexOf('select') > -1) {
                $ctrl.closest('ul').addClass('has-error');
                MakeTagFlashable($ctrl.closest('ul'));
            }
            else {
                $ctrl.addClass('has-error');
                MakeTagFlashable(o);
            }
        }
    });

    var selectedFreq1 = $('.stand-horizon-box input[type="radio"]:checked').attr('id');
    var ctrlToValidate1 = selectedFreq1.replace('rb-', '.');
    $.each($(ctrlToValidate1), function (i, o) {
        var $ctrl = $(o); val = $ctrl.val();
        if (val == '' || ($ctrl.attr('type') == 'number' && (val != '' && !($.isNumeric(val) && parseInt(val) > 0) && val.indexOf('.') < 0))) {
            $ctrl.addClass('has-error');
            MakeTagFlashable(o);
        }
    });

    if ($('#rb-horizonShop:checked').length > 0
        && parseInt($('#startDateOffset').val().trim()) > parseInt($('#endDateOffset').val().trim())) {
        $('#startDateOffset, #endDateOffset').addClass('has-error');
        MakeTagFlashable('#startDateOffset'); MakeTagFlashable('#endDateOffset');
    }

    //To check automation end date validation
    var flag = false;
    var errorMsg = 'Please review the fields highlighted in Red.';

    if ($('#wideGap').prop('checked') || $('#IsGov').prop('checked')) {
        flag = AutomationRuleSetValidation();
        if (flag) {
            //errorMsg += " Set proper end date under applied ruleset.";
            //Makrand to suggest
            ShowConfirmBox('One of the applied ruleset validity is not meeting the selected shop start & end date interval.', false);
            if ($('#IsGov').prop('checked')) {
                $('#IsGov').addClass('has-error');
            }
            else {
                $('#wideGap').addClass('has-error');
            }
            MakeTagFlashable('#wideGap');
        }
        else {
            $('#wideGap').removeClass('has-error');
            $('#IsGov').removeClass('has-error');
        }

        //Automation console enhancement changes and covered scenario that Weekly LOR and BaseCost selected than show pop msg
        var flagLOR = false;
        $('#view1 select[id=lengths] option:selected').each(function () {
            var optiontext = $(this).text().trim().substr(0, 1);
            if (optiontext == 'W' && !($('#view1 select[id=source] option:selected').attr("prvcode") == 'RH')) {
                flagLOR = true;
                return false;
            }
        });
        if (flagLOR && $("input[name=rulesetfilter][value=RdBaseCost]:radio").prop("checked")) {
            addErrorToField('RdBaseCost');
            ShowConfirmBox("Plaese change the LOR for Compete setting setup.", false);
        }
        else {
            removeErrorToField('RdBaseCost');
        }
    }


    if ($('#view1').find('.has-error').length > 0) {
        $('#error-span').html(errorMsg);
        if (!flag) {
            $('#error-span').show();
            AddFlashingEffectAutomation();
        }
        //MakeTagFlashable('.has-error');
    }

    else {
        $('#error-span').hide();
        //RemoveFlashableTag('.has-error');
        AddFlashingEffectAutomation();

        SaveData();
    }
}

function AddFlashingEffectAutomation() {

    //This code is removed, as we have added code to sync flashing in master.js

    //for (var i = 1; i < 999; i++) {
    //    //window.clearInterval(i);
    //    $('.temp').removeClass('flashBorder');
    //}
    //setInterval(function () {
    //    $('.temp').toggleClass('flashBorder');
    //}, 500);
}

function SaveData() {
    $('.loader_container_main').show()
    SaveJobModel = new Object();
    SaveJobModel.JobId = jobId;

    //Frequency



    SaveJobModel.ScheduledJobFrequencyID = $('input[name=freq]:checked', '.frequency').attr('freqId');
    SaveJobModel.JobScheduleWeekDays = $.unique(jobScheduleWeekDays).toString();

    //post value only for applicable field based on freq selection
    var selectedFreq = $('input[name=freq]:checked', '.frequency').attr('id');
    switch (selectedFreq) {
        case 'rb-e1day':
            SaveJobModel.RunTime = $('#jobTime').val().trim();
        case 'rb-e1week':
            SaveJobModel.RunTime = $('#jobTime').val().trim();
            break;
        case 'rb-e1month':
            SaveJobModel.JobScheduleWeekDays = '';
            SaveJobModel.RunTime = $('#jobTime').val().trim();
            SaveJobModel.RunDay = $('#jobDay').val();
            break;
        case 'rb-custom-time':
            SaveJobModel.CustomHoursToRun = $('#customInputHours').val().trim();
            SaveJobModel.CustomMinutesToRun = $('#customInputMinutes').val().trim();
            break;
    }

    //Days to Run
    SaveJobModel.DaysToRunStartDate = $('#runStartDate').val();
    SaveJobModel.DaysToRunEndDate = $('#runEndDate').val();

    //Price Gap
    SaveJobModel.IsWideGapTemplate = $('#wideGap').prop('checked');
    SaveJobModel.IsActiveTethering = $('#activeTethering').prop('checked');
    SaveJobModel.IsOpaqueActive = $('#activeOpaqueRate').prop('checked');
    SaveJobModel.IsGovTemplate = $('#IsGov').prop('checked');

    //send for Update
    SaveJobModel.TSDUpdateWeekDay = $.unique(sendUpdateWeekDays).toString();

    //Rate featch parameters
    SaveJobModel.IsStandardShop = ($('#rb-standardShop:checked').length > 0) ? true : false;
    if (SaveJobModel.IsStandardShop) {
        SaveJobModel.StartDate = $('#startDate').val();
        SaveJobModel.EndDate = $('#endDate').val();
    }
    else {
        SaveJobModel.StartDateOffset = $('#startDateOffset').val().trim();
        SaveJobModel.EndDateOffset = $('#endDateOffset').val().trim();
    }
    SaveJobModel.ScrapperSourceIDs = $.unique(sourceIds).toString();
    SaveJobModel.LocationBrandIDs = $.unique(locationBrandIds).toString();
    SaveJobModel.RentalLengthIDs = $.unique(rentalLengthIds).toString();
    SaveJobModel.CarClassesIDs = $.unique(carClassIds).toString();
    SaveJobModel.PickUpTime = $('#pickupHour li').eq(0).text();
    SaveJobModel.DropOffTime = $('#dropOffHour li').eq(0).text();
    SaveJobModel.LoggedInUserId = $('#LoggedInUserId').val();
    SaveJobModel.ScrapperSource = $.unique(sources).toString();
    SaveJobModel.CarClasses = $.unique(carClassArr).toString();
    SaveJobModel.location = $.unique(locationArr).toString();
    SaveJobModel.ProviderCode = selectedAPICode;
    if (typeof (selectedAPICode) != "undefined" && selectedAPICode != "") {
        SaveJobModel.ProviderId = $("#source option[prvcode='" + selectedAPICode + "']").attr("providerid");
    }
    else {
        SaveJobModel.ProviderId = 1;
    }
    //popup minrate data
    SaveJobModel.ScheduledJobMinRates = FinalMinRateData;
    SaveJobModel.ScheduledJobTetherings = FinalTetherValueData;
    SaveJobModel.IntermediateID = $("#mr_AutoConsoleRuleSet_popup #IntermediateID").val().trim();
    //isGOV Job
    SaveJobModel.IsGov = $.trim($('#source').find('option:selected').attr('isgov'));
    //Logged in User Name
    SaveJobModel.UserName = $('#hdnUserName').val();
    SaveJobModel.CompeteOnBase = $("input[name=rulesetfilter][value=RdBaseCost]:radio").prop("checked");
    SaveJobModel.OpaqueRateCodes = automationOpaqueModel.ApplicableRateCodes().map(function (i) { return i.Code }).join(",");
    SaveJobModel.OpaqueRates = finalOpaqueRate;

    var ajaxURl = 'AutomationConsole/SaveJob/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.SaveJob;
    }
    $.ajax({
        url: ajaxURl,
        type: 'POST',
        async: true,
        data: JSON.stringify({ 'autoConsoleJobEditDTO': SaveJobModel }),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $('.loader_container_main').hide();
            if (data == 'Success') {
                $('#spanSave').show();

                //This condition used in Automation scheduled job ruleset for newly creation intermediate id
                $("#mr_AutoConsoleRuleSet_popup #IntermediateID").val("");

                jobId = '';
                resetFormSelection(true);
                ListAllScheduledJobs(false, jobId);
                setTimeout(function () { $('#spanSave').hide(); }, 3000);
            }
            else if (data == 'NoMatchingDate') {
                $('#error-span').html("<b>Save Failed.</b> The Job Schedule settings are incorrect.")
                $('#error-span').show();
            }
            else if (data.indexOf("FTBJobScheduled") == 0) {
                $('#error-span').html("Something went wrong. Please try again.")
                $('#error-span').show();

                ShowConfirmBox('Either Shop Start date or End date is out of Black-Out period (' + data.split('-')[1] + ' - ' + data.split('-')[2] + '). Automation Job cannot be scheduled.', false);
            }
            else if (data.indexOf("notconfiguredblackoutdates") == 0) {
                $('#error-span').html("Something went wrong. Please try again.")
                $('#error-span').show();
                ShowConfirmBox("FTB Automation is scheduled for this Location. Regular automation can not be scheduled.", false);
            }
            else {
                $('#error-span').html("Something went wrong. Please try again.")
                $('#error-span').show();
            }
        },
        error: function (e) {
            $('.loader_container_main').hide();
            console.log(e.message);
        }
    });
}

//set controls values for the selected job

//User this method to set the edit mode
function EditJob(newJobId) {
    FinalMinRateData = [];//Reset Minrate popup data
    jobId = newJobId;
    resetFormSelection();
    GetSelectedScheduleMinRate(jobId);
    //Use to get job again tether data.
    GetSelectedJobTetherSetting(jobId);
    //JobSelected(jobId);
}

function JobSelected(jobId) {
    if (jobId == null || jobId == '') {
        return;
    }
    var ajaxURl = 'AutomationConsole/GetJobDetails/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetJobDetails;
    }
    $('.loader_container_main').show();
    $.ajax({
        url: ajaxURl,
        data: { JobId: jobId },
        type: 'GET',
        async: true,
        success: function (data) {
            if (data) {
                BindJob(data);
                $('.loader_container_main').hide();
            }
        },
        error: function (e) {
            console.log(e.message);
            $('.loader_container_main').hide();
        }
    });
}

function BindJob(data) {
    jobId = data.ID;

    //BindSources(data.ProviderId, data.ScrapperSourceIDs);

    //JOB SCHEDULED SECTION START
    $('.frequency input[freqid="' + data.ScheduledJobFrequencyID + '"]').trigger('click');
    var DaysToRunStartDate = convertToServerTime(new Date(parseInt(data.DaysToRunStartDate.replace("/Date(", "").replace(")/", ""), 10)));
    var DaysToRunEndDate = convertToServerTime(new Date(parseInt(data.DaysToRunEndDate.replace("/Date(", "").replace(")/", ""), 10)));

    $("#runStartDate.date-picker").val(('0' + (DaysToRunStartDate.getMonth() + 1)).slice(-2) + "/" + ('0' + DaysToRunStartDate.getDate()).slice(-2) + "/" + DaysToRunStartDate.getFullYear());

    //$("#runEndDate.date-picker").val((DaysToRunEndDate.getMonth() + 1) + "/" + DaysToRunEndDate.getDate() + "/" + DaysToRunEndDate.getFullYear());
    $("#runEndDate.date-picker").val(('0' + (DaysToRunEndDate.getMonth() + 1)).slice(-2) + "/" + ('0' + DaysToRunEndDate.getDate()).slice(-2) + "/" + DaysToRunEndDate.getFullYear());

    $('#runEndDate.date-picker').datepicker('option', { minDate: DaysToRunStartDate });

    if (data.JobScheduleWeekDays != null && data.JobScheduleWeekDays.length > 0) {
        $("select.jobScheduleWeekDays option").each(function () {
            if ($.inArray($(this).val(), (data.JobScheduleWeekDays).split(',')) > -1) {
                $(this).prop("selected", true);
            }
        });
    }
    $('#jobTime').val(data.RunTime);
    $('#jobDay').val(data.RunDay);
    $('#customInputHours').val(data.CustomHoursToRun);
    $('#customInputMinutes').val(data.CustomMinutesToRun);
    $('input[id^=customInput]').trigger('change');
    //JOB SCHEDULED SECTION END

    //RATE FETCH PARAMETERS START
    if (data.IsStandardShop) {
        var startDate = convertToServerTime(new Date(parseInt(data.StartDate.replace("/Date(", "").replace(")/", ""), 10)));
        var endDate = convertToServerTime(new Date(parseInt(data.EndDate.replace("/Date(", "").replace(")/", ""), 10)));
        $("#startDate.date-picker").val(('0' + (startDate.getMonth() + 1)).slice(-2) + "/" + ('0' + startDate.getDate()).slice(-2) + "/" + startDate.getFullYear());


        //SET MaxDate on END DATE
        var dSelectedDate = new Date(startDate);
        var maxDate = new Date(startDate);
        // month is December, we need to roll over to January of the next year
        if (maxDate.getMonth() == 11) {
            maxDate.setMonth(0);
            maxDate.setFullYear(maxDate.getFullYear() + 1);
        }
        else {
            maxDate.setMonth(maxDate.getMonth() + 1);
        }
        $('#endDate,#endDateimg').datepicker('option', { minDate: dSelectedDate, maxDate: maxDate });
        $("#endDate.date-picker").val(('0' + (endDate.getMonth() + 1)).slice(-2) + "/" + ('0' + endDate.getDate()).slice(-2) + "/" + endDate.getFullYear());
        $('#rb-standardShop').prop('checked', true).trigger('click');
        //end MAXDATE SET 
    }
    else {
        $('#startDateOffset').val(data.StartDateOffset);
        $('#endDateOffset').val(data.EndDateOffset);

        $('#rb-horizonShop').prop('checked', true).trigger('click');
    }

    $("#pickupHour ul li").each(function () {
        if ($(this).text() == data.PickUpTime) {
            $(this).addClass("selected");
            $('#pickupHour li').eq(0).text($(this).text()).attr('value', ($(this).val()));
        }
    });

    $("#dropOffHour ul li").each(function () {
        if ($(this).text() == data.DropOffTime) {
            $(this).addClass("selected");
            $('#dropOffHour li').eq(0).text($(this).text()).attr('value', ($(this).val()));
        }
    });
    ////populate Selected summary based on autoselect source dropdown item
    $("#source select option").each(function () {
        if ($.inArray($(this).val(), (data.ScrapperSourceIDs).split(',')) > -1) {
            $(this).prop("selected", true);
        }
    });
    EnableDisableMultipleLOR($("#source option:selected").attr("prvcode"), $("select#source option:selected").attr("isgov"));
    EnableDisableGovTemplate();
    //populate Selected summary based on autoselect RentelLength dropdown item
    $("#lengths select option").each(function () {
        if ($.inArray($(this).val(), (data.RentalLengthIDs).split(',')) > -1) {
            $(this).prop("selected", true);
        }
    });


    //populate Selected summary based on autoselect carclass dropdown item
    $("#carClass select option").each(function () {
        if ($.inArray($(this).val(), (data.CarClassesIDs).split(',')) > -1) {
            $(this).prop("selected", true);
        }
    });
    //populate Selected summary based on autoselect location dropdown item
    $("#locations select option").each(function () {
        if ($.inArray($(this).attr("brandid"), (data.LocationBrandIDs).split(',')) > -1) {
            $(this).prop("selected", true);
        }
    });
    ResetLORs();
    DisableLORs();
    //RATE FETCH PARAMETERS END

    //RATE SUGGESSIONS START
    if (data.TSDUpdateWeekDay != null && data.TSDUpdateWeekDay.length > 0) {
        $("select.sendUpdateWeekDays option").each(function () {
            if ($.inArray($(this).val(), (data.TSDUpdateWeekDay).split(',')) > -1) {
                $(this).prop("selected", true);
            }
        });
    }

    if (data.IsGovTemplate) {
        $('#IsGov').prop('checked', true);
        $('#wideGap').prop({ 'checked': false, 'disabled': true });
    }
    else if (data.IsWideGapTemplate) {
        $('#wideGap').prop('checked', true);
        $('#IsGov').prop({ 'checked': false, 'disabled': true });
    }



    //for check if wide gap is true or not that based automation ruleset popup appear.
    if (data.IsWideGapTemplate || data.IsGovTemplate) {
        $("#divAutomationRulesetAvailable").show();
        $("#RulesetCompeteSelection").show();
        GetLocationRuleSet();
    }

    if (data.CompeteOnBase) {
        $("input[name=rulesetfilter][id=RdBaseCost]:radio").prop("checked", true);
    }


    $('#activeTethering').prop('checked', data.IsActiveTethering);
    $('#activeOpaqueRate').prop('checked', data.IsOpaqueActive).attr("IsConfigured", "false").prop("disabled", false);
    $("#mr_Opaque_popup #ratecodes option:selected").prop("selected", false);
    $("#mr_Opaque_popup #RateCodeAll").prop("checked", false);
    if (data.IsOpaqueActive) {
        $('#activeOpaqueRate').attr("IsConfigured", "true")
        $("#btnOpaqueRate").prop('disabled', false).removeClass("disable-button");
        //$(data.OpaqueRateCodes.split(',')).each(function () {
        //    $("#mr_Opaque_popup #ratecodes option[value=" + this + "]").prop("selected", true);
        //});
        //if ($("#mr_Opaque_popup #ratecodes option").length == data.OpaqueRateCodes.split(',').length)
        //{
        //    $("#mr_Opaque_popup #RateCodeAll").prop("checked", true);
        //}
        //selectedRateCodes = $("#mr_Opaque_popup #ratecodes").val();
    }
    //else {
    //    $("#mr_Opaque_popup #ratecodes option[default='true']").prop("selected", true);
    //}
    
    //RATE SUGGESSSION END

    //set checkboxes for 'all, D, W' etc.
    checkUncheckParent();
    checkUncheckCarClasses();
    checkUncheckWeekDays('jobScheduleWeekDays');
    checkUncheckWeekDays('sendUpdateWeekDays');

    //setTimeout(function () { LocationChanged(data.IsActiveTethering); }, 300);
    getGlobalTetherValueSettingLocationSpecific(data.IsActiveTethering);
    GetRateCodes();
    if (data.IsOpaqueActive) {
        getEditJobOpaqueRateData(jobId, data.CarClassesIDs);//To get opaqueRate data and generate popup
    }
    else {
        jobSpecificOpaqueRate = "";
        BindOpaqueRate();
    }
}

function BindSources(providerId, scrapperSourceIds) {
    currentSelectedProvider = $("#ddlAPI li.selected").attr("value");
    if (providerId != null && providerId > 0 && currentSelectedProvider != providerId) {
        //$("#ddlAPI ul.hidden.drop-down li.selected").removeClass('selected');
        //$("#ddlAPI ul.hidden.drop-down li[value='" + providerId + "']").addClass('selected').closest('#ddlAPI').find('li').eq(0).attr({ 'value': ($("#ddlAPI ul.hidden.drop-down li[value='" + providerId + "']").attr('value')), 'prvcode': ($("#ddlAPI ul.hidden.drop-down li[value='" + providerId + "']").attr('prvcode')) }).text($("#ddlAPI ul.hidden.drop-down li[value='" + providerId + "']").text());
        EnableDisableMultipleLOR($("#ddlAPI ul.hidden.drop-down li.selected").attr("prvcode"));
        //LoadScrapperSource(providerId, scrapperSourceIds)
    }
    else {
        //populate Selected summary based on autoselect source dropdown item
        $("#source select option").each(function () {
            if ($.inArray($(this).val(), scrapperSourceIds.split(',')) > -1) {
                $(this).prop("selected", true);
            }
        });
    }

}

//calculate am/pm and get 12 hours format of time
function formatAMPM(hours, minutes) {
    //var hours = date.getHours();
    //var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'pm' : 'am';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ampm;
    return strTime;
}

// add an error display to a given field

//for Check aumation ruleset enddate incase standard or horizon shop
function AutomationRuleSetValidation() {
    var endDate;
    var flag = false;
    var enddateoffset = 1;
    if ($('#view1 input:checked[name=shopType]').attr("id") == "rb-standardShop") {
        //Standard shop check
        endDate = new Date($('#view1 #endDate').val());
    }
    else {
        endDate = new Date($('#view1 #runEndDate').val());
        enddateoffset = $("#view1 #endDateOffset").val();
        endDate.setDate(endDate.getDate() + parseInt(enddateoffset));
    }
    $("#mr_AutoConsoleRuleSet_popup #RuleSetList li").each(function () {
        var rulesetEnddate;
        if ($(this).attr("appliedenddate") != "") {
            rulesetEnddate = new Date($(this).attr("appliedenddate"));
        }
        if (endDate > rulesetEnddate) {
            flag = true;
            return;
        }
    });
    return flag;
}

function addErrorToField(field) {
    $('#' + field).addClass('has-error');
    MakeTagFlashable('#' + field);
    //AddFlashingEffect();
}

function removeErrorToField(field) {
    $('#' + field).removeClass('has-error');
    RemoveFlashableTag('#' + field);
    //AddFlashingEffect();
}

//Tethering access
function InitiateTetherSettings() {
    $("#activeTethering").prop('disabled', true);
    $("#TetherRate").prop('disabled', true).addClass("disable-button");

    $("#activeOpaqueRate").prop('disabled', true);
    $("#btnOpaqueRate").prop('disabled', true).addClass("disable-button");

    if ($("#IsTetheringAccess").val() != "True") {
        $("#activeTethering").addClass('noAccess');
    }
    GetGlobalTetherValue();
}

function GetGlobalTetherValue() {
    $.ajax({
        url: 'AutomationConsole/GetGlobalTetherSettings/',
        type: 'GET',
        async: true,
        success: function (data) {
            if (data) {
                GlobalTetherSetting = data;
            }
        },
        error: function (e) {
            console.log(e.message);
        }
    });
}
function AddAttrToLocations() {
    $('#locations option').each(function () {
        var $thisOption = $(this);
        $thisOption.removeAttr('isTetherEnabled');

        if (GlobalTetherSetting != null && GlobalTetherSetting > 0) {
            var globalTetherSettingsForBrand = $.grep(GlobalTetherSetting, function (item) {
                return item.DominentBrandID == $thisOption.attr('companyid') && item.LocationID == $thisOption.val();
            });

            if (globalTetherSettingsForBrand != null && globalTetherSettingsForBrand.length > 0) {
                for (var x in globalTetherSettingsForBrand) {
                    if (globalTetherSettingsForBrand[x].LocationID == $thisOption.val() && globalTetherSettingsForBrand[x].DominentBrandID == $thisOption.attr('companyid')) {
                        $thisOption.attr('isTetherEnabled', true)
                    }
                }
            }
        }
    });
}

function LocationChanged(isChked) {
    $("#activeTethering").prop('checked', false).attr('disabled', true);
    $("#TetherRate").attr('disabled', 'disabled').addClass("disable-button");
    var $thisOption = $('#locations option:selected');
    var attr = $thisOption.attr('isTetherEnabled');

    if (typeof attr !== typeof undefined && attr !== false) {
        $("#activeTethering").prop('checked', true);
        if (!$("#activeTethering").hasClass('noAccess')) {
            $("#activeTethering").attr('disabled', false);
            $("#TetherRate").removeAttr('disabled').removeClass("disable-button");
            // setTimeout(function () { BindTetherData(); }, 250);
        }
        if (isChked != null && isChked != undefined) {
            $("#activeTethering").prop('checked', isChked);

            if (isChked) {
                if (!$("#activeTethering").prop("disabled") && $("#activeTethering").prop("checked")) {
                    $("#TetherRate").removeAttr('disabled').removeClass("disable-button");
                    //setTimeout(function () { BindTetherData(); }, 250);//If user has no tethering acces and tether value is access for that car class then it should be go for tether value
                }
                else {
                    $("#TetherRate").attr('disabled', 'disabled').addClass("disable-button");
                }
            }
            else {
                $("#TetherRate").attr('disabled', 'disabled').addClass("disable-button");
            }
        }
    }
}
//Tethering access end


function EnableDisableMultipleLOR(selectedAPICode, isGov) {
    if (selectedAPICode == "RH") {
        $("select#lengths").removeAttr("multiple");
        $(".LORcheckboxes input[type='checkbox']").prop({ "checked": false, "disabled": true });
        if ($("select#lengths option:selected").length > 1) {
            $("select#lengths option:selected").prop("selected", false);
            $("select#lengths").scrollTop(0);
        }
    }
    else {
        $(".LORcheckboxes input[type='checkbox']").prop({ "disabled": false });
        $("select#lengths").attr("multiple", "multiple");
    }
    if (isGov == "True" && $("select#lengths option").length > 2) {
        $("select#lengths option").remove();
        for (i = 0; i < allRentalLengths.length; i++) {
            if (allRentalLengths[i].MappedID == 1 || allRentalLengths[i].MappedID == 7) {
                $("select#lengths").append("<option value=" + allRentalLengths[i].MappedID + " rid=" + allRentalLengths[i].ID + ">" + allRentalLengths[i].Code + "</option>");
            }
        }
    }
    else if (isGov != "True" && $("select#lengths option").length == 2) {
        $("select#lengths option").remove();
        for (i = 0; i < allRentalLengths.length; i++) {
            if (allRentalLengths[i].MappedID > 8 && allRentalLengths[i].MappedID < 14) {
                $("select#lengths").append("<option value=" + allRentalLengths[i].MappedID + " rid=" + allRentalLengths[i].ID + " style='display:none;'>" + allRentalLengths[i].Code + "</option>");
            }
            else {
                $("select#lengths").append("<option value=" + allRentalLengths[i].MappedID + " rid=" + allRentalLengths[i].ID + ">" + allRentalLengths[i].Code + "</option>");
            }
        }
        DisableLORs();
    }
    //$("select#lengths option:selected").prop("selected", false);

}

//$("#startDate").datepicker({
//    onSelect: function () {
//        if ($("#source option:selected").length && $("#locations option:selected").length) {
//            DisableOpaqueRatesforGOV();
//        }
//    }
//});

//$("#endDate").datepicker({
//    onSelect: function () {
//        if ($("#source option:selected").length && $("#locations option:selected").length) {
//            DisableOpaqueRatesforGOV();
//        }
//}
//});

function DisableOpaqueRatesforGOV() {
    if ($("select#source option:selected").length > 0 && $("select#source option:selected").attr("isgov") == "True") {
        $('[id=btnOpaqueRate]').attr("disabled", "disabled").addClass("disable-button");
        $('[id=activeOpaqueRate]').attr("disabled", "disabled").prop("checked", false);
    }
    else {
        if ($('#startDate').val() != '' && $('#endDate').val() != '' && $("#source option:selected").length) {
        if ($("#view1 #locations select").val() != null && $("#view1 #locations select").val() != undefined) {
            $('[id=btnOpaqueRate]').removeAttr("disabled", "disabled").removeClass("disable-button");    //for enabling the opaque rate button
            $('[id=activeOpaqueRate]').removeAttr("disabled", "disabled").prop("checked", true);
        }
        }
    }
}

function EnableDisableGovTemplate() {
    DisableOpaqueRatesforGOV();
    if ($("select#source option:selected").length > 0 && $("select#source option:selected").attr("isgov") == "True") {
        $("#IsGov").prop({ "disabled": true, "checked": true });
        $("#RulesetCompeteSelection").show();
        $("input[name=rulesetfilter][value=RdBaseCost]:radio").prop("checked", true);
        $("#RulesetCompeteSelection input:radio").attr("Disabled", true);

        if ($('#wideGap').is(":checked")) {
            $("#divAutomationRulesetAvailable").hide();
        }
        GetLocationRuleSet();
        $('#wideGap').prop({ 'checked': false, 'disabled': true });
    }
    else {
        $('#wideGap').prop({ 'disabled': false });
        if (!$('#wideGap').prop("checked")) {
            $("#RulesetCompeteSelection").hide();
        }
        if ($('#IsGov').is(":checked")) {
            $("#divAutomationRulesetAvailable").hide();
        }
        $('#IsGov').prop({ 'checked': false, 'disabled': true });
        $("input[name=rulesetfilter][value=RdTotalCost]:radio").prop("checked", true);
        $("#RulesetCompeteSelection input:radio").attr("Disabled", false);
    }
}

function LoadScrapperSource(selectedAPI, scrapperSourceIds) {
    var loggedInUserId = $('#LoggedInUserId').val();

    $.ajax({
        url: 'Search/GetScrapperSourcess',
        type: "GET",
        dataType: "json",
        contentType: "application/json; charset=utf-8;",
        //data: { 'userId': loggedInUserId, 'providerId': selectedAPI },
        data: { 'userId': loggedInUserId },
        success: function (data) {
            $("select#source option").remove();
            if (data.length > 0) {
                for (i = 0; i < data.length; i++) {
                    $("select#source").append("<option value=" + data[i].ID + " srccode=" + data[i].Code + ">" + data[i].Name + "</option>");
                }

                if (typeof (scrapperSourceIds) != "undefined") {
                    //populate Selected summary based on autoselect source dropdown item
                    $("#source select option").each(function () {
                        if ($.inArray($(this).val(), scrapperSourceIds.split(',')) > -1) {
                            $(this).prop("selected", true);
                        }
                    });
                }
            }
            else {
                $("select#source").append("<option value=0 srccode=''>No Source Found</option>");
            }
        },
        error: function (e) {
            $('.loader_container_source').hide();
            console.log(e.message);
        }
    });
}

var DisableTSDUpdateAccess = function () {
    if ($("#hdnIsTSDUpdateAccess").length > 0 && $("#hdnIsTSDUpdateAccess").val() == "False") {
        $(".sendforupdate input,.sendforupdate select").prop('disabled', true);
    }
}