var FTBJobViewModel, filterLocations, jobId, startMonth, startYear;
var locationBrandIds = [], locationIds = [], jobScheduleWeekDays = [];
$(document).ready(function () {
    FTBJobViewModel = new FTBScheduleJobModel();
    ko.applyBindings(FTBJobViewModel);

    InitializeDatePickerControls();
    LoggedInUserID = $('#LoggedInUserId').val();
    InitializeControlEvents();
    BindLocations();
});

function FTBScheduleJobModel() {
    var self = this;
    self.locations = ko.observableArray([]);
    self.jobDetails = ko.observableArray([]);
    self.headersFirstGrp = [
        { title: 'JOB STATUS', sortPropertyName: 'Status', asc: true },
        { title: 'NEXT RUN TIME', sortPropertyName: 'NextRunDateTime', asc: true }
    ];

    self.headersSecondGrp = [
        { title: 'SUBMITTED BY', sortPropertyName: 'CreatedBy', asc: true }
    ];
    self.sort = function (header) {
        var prop = header.sortPropertyName;
        getSortedFTBData(prop, true);
    };
    self.jobSplitDetails = ko.observableArray([]);
}

function locations(data) {
    this.LocationBrandID = data.LocationBrandID;
    this.LocationID = data.LocationID;
    this.Location = data.LocationBrandAlias;
    this.IsActiveTether = ko.computed(function () {
        if (data.DominentBrandID != null && data.DominentBrandID > 0) {
            return "true";
        }
        else {
            return "false";
        }
    });
    this.FtbJobStatus = ko.computed(function () {
        if (data.Flag == 0) { return "red"; }
        //else if (data.Flag == 1) { return "blue"; }
    });
    this.BranchCode = data.BranchCode;
}

function InitializeDatePickerControls() {

    $('#runStartDate').datepicker({
        minDate: 0,
        numberOfMonths: 2,
        dateFormat: 'mm/dd/yy',
        onSelect: function (selectedDate) {
            var startDate = $('#runStartDate').datepicker('getDate');
            var dSelectedDate = new Date(selectedDate);
        }
    });
    $('#runStartDate').click(function () { $(this).datepicker('show'); });

    $('#CreateFTBJobPopup #jobTime').timepicker({
        timeFormat: 'hh:mm tt',
        hourGrid: 4,
        minuteGrid: 10,
        stepMinute: 5
    });
}

function InitializeControlEvents() {

    $('#CreateFTBJobPopup #searchLocation').bind('input', function () {
        SearchGrid('#searchLocation', '#FTBlocations option');
        if ($("#FTBlocations option[style$='display: none;']").length == $("#FTBlocations option").length) {
            MakeTagFlashable('#searchLocation');
        }
        else {
            RemoveFlashableTag('#searchLocation');
        }
    });

    $('#CreateFTBJobPopup #Tether').prop('disabled', true);

    $('#CreateFTBJobPopup .frequency input[type="radio"]').click(function () {
        if ($.trim($(this).attr('id')).toString() == 'rb-e2week' || $.trim($(this).attr('id')).toString() == 'rb-e1day') {
            $("#CreateFTBJobPopup #jobDay").val('');
            $('#CreateFTBJobPopup .jobScheduleWeekDays').attr('multiple', 'multiple');
        }
        else {
            $("#jobDay").val('');
            $('#CreateFTBJobPopup .jobScheduleWeekDays').removeAttr('multiple');
        }

        if ($.trim($(this).attr('id')).toString() == 'rb-e1week' || $.trim($(this).attr('id')).toString() == 'rb-e2week') {
            $('#chkAllDays').prop('checked', false).attr('disabled', 'disabled');
        }
        else
        {
            $('#chkAllDays').removeAttr('disabled', 'disabled');
        }

    });

    $('#CreateFTBJobPopup #SaveFTBJob').click(function () {
        SaveClicked();
    });

    $("#CreateFTBJobPopup .closeP").on("click", function () {
        jobId = '';
        ResetFormData(true);
        $('#view2 tr').removeClass('grey_bg');
        $('#CreateFTBJobPopup, .popup_bg').hide();
    });

    $("#ftbNewJob").on("click", function () {
        ResetFormData();
        $('#CreateFTBJobPopup, .popup_bg').show();
        $('#view2 tr').removeClass('grey_bg');
    });

    $('#CreateFTBJobPopup #popup-main-inner').draggable();


    $('#CreateFTBJobPopup #jobScheduleWeekDays ul').change(function () {
        if ($('#rb-e2week').is(':checked')) {
            if ($('#jobScheduleWeekDays ul option:selected').length > 2) {
                ShowConfirmBox('Error : only 2 week days can be selected', false);
                $('#chkAllDays').prop('checked', false).attr('disabled', 'disabled');
                $("#jobDay").val('');
                return false;
            }
        }
    });
    if ($("#IsTetheringAccess").val() != "True") {
        $("#Tether").addClass('noAccess');
    }

    //if not admin then filter FTB automation jobs and add class 
    if ($('#LoggedInUserId') != undefined && $('#LoggedInUserId').val().trim() != '') {
        var loggedInUserId = parseInt($('#LoggedInUserId').val());
        $('#view2 #user ul li[value="' + loggedInUserId + '"]').addClass('selected');
        $('#view2 #user li').eq(0).val($('#view2 #user ul li[value="' + loggedInUserId + '"]').val()).text($('#view2 #user ul li[value="' + loggedInUserId + '"]').text());

    }
    var isAdmin = $("#hdnIsAdminUser").val();
    if (isAdmin.toUpperCase() == "FALSE") {
        //$('#view2 #user').addClass("disable-UL");
        $('#view2 #user ul li[value="0"]').hide();
    }

}

function BindLocations() {
    var ajaxURl = 'FTBAutomation/GetLocations/';
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
                filterLocations = data;
                FTBJobViewModel.locations(srcs);
            }
        },
        error: function (e) {
            console.log(e.message);
        }
    });
}

function SearchGrid(inpuText, controlId) {
    if ($(inpuText).val().length > 0) {
        var matches = $.map(filterLocations, function (item) {
            if (item.LocationBrandAlias.toUpperCase().indexOf($(inpuText).val().toUpperCase()) == 0) {
                return new locations(item);
            }
        });
        FTBJobViewModel.locations(matches);

    } else {
        //$(controlId).show();
        var srcs = $.map(filterLocations, function (item) { return new locations(item); });
        FTBJobViewModel.locations(srcs);
    }
}


function SaveClicked() {
    var errorMsg = 'Please review the fields highlighted in Red.';
    locationBrandIds = [];
    locationIds = [];
    jobScheduleWeekDays = [];
    RemoveFlashableTag('.has-error');

    $('#CreateFTBJobPopup #locations ul option:selected').each(function () {
        locationBrandIds.push($(this).attr('LocationBrandId'));
        locationIds.push($(this).attr('value'));
    });

    $('#CreateFTBJobPopup #jobScheduleWeekDays ul option:selected').each(function () {
        jobScheduleWeekDays.push($(this).attr('value'));
    });

    $('#CreateFTBJobPopup #monthsdiv ul option:selected').length <= 0 ? AddErrorToField("monthsdiv") : RemoveErrorToField("monthsdiv");


    startMonth = $('#CreateFTBJobPopup #monthsdiv ul option:selected').attr('Month');
    startYear = $('#CreateFTBJobPopup #monthsdiv ul option:selected').attr('value');

    locationBrandIds.length == 0 ? AddErrorToField("locations") : RemoveErrorToField("locations");
    $('#CreateFTBJobPopup #runStartDate').val() == '' ? AddErrorToField("runStartDate") : RemoveErrorToField("runStartDate");
    $('#CreateFTBJobPopup #jobTime').val() == '' ? AddErrorToField("jobTime") : RemoveErrorToField("jobTime");
    jobScheduleWeekDays.length == 0 ? AddErrorToField("jobScheduleWeekDays") : RemoveErrorToField("jobScheduleWeekDays");

    var jobStartDate = new Date($('#runStartDate').val());
    var startDate = new Date(startYear, (startMonth - 1), 1);
    if (jobStartDate >= startDate) {
        ShowConfirmBox('Start Date should be less than FTB job start date.', false);
        AddErrorToField("runStartDate");
    }

    if ($('#rb-e2week').is(':checked')) {
        jobScheduleWeekDays.length != 2 ? AddErrorToField("jobScheduleWeekDays") : RemoveErrorToField("jobScheduleWeekDays");
    }

    if ($('#CreateFTBJobPopup .job-shedule-form').find('.has-error').length > 0) {
        $('#CreateFTBJobPopup #error-span').html(errorMsg);
        $('#CreateFTBJobPopup #error-span').show();
    }
    else {
        $('#CreateFTBJobPopup #error-span').hide();
        SaveEditJob();
    }

}

function SaveEditJob() {

    SaveJobModel = new Object();
    SaveJobModel.JobId = jobId;
    SaveJobModel.LocationBrandID = $.unique(locationBrandIds).toString();
    SaveJobModel.ScheduledJobFrequencyID = $('input[name=freq]:checked', '.frequency').attr('freqId');
    SaveJobModel.StartDate = $('#CreateFTBJobPopup #runStartDate').val();
    SaveJobModel.RunTime = $('#CreateFTBJobPopup #jobTime').val().trim();
    SaveJobModel.StartMonth = startMonth;
    SaveJobModel.StartYear = startYear;
    SaveJobModel.IsActiveTethering = $('#CreateFTBJobPopup #Tether').prop('checked');
    SaveJobModel.JobScheduleWeekDays = $.unique(jobScheduleWeekDays).toString();
    SaveJobModel.LoggedInUserId = $('#LoggedInUserId').val();
    var ajaxURl = 'FTBAutomation/SaveJob/';

    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.SaveFTBJob;
    }
    $.ajax({
        url: ajaxURl,
        type: 'POST',
        async: true,
        data: JSON.stringify({ 'ftbJobEditDTO': SaveJobModel }),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            $('.loader_container_main').hide();
            if ($.trim(data).toString() == 'Success') {
                $('#spanSave').show();
                jobId = '';
                if (locationBrandIds[0] == $("#view2 #location ul li.selected").val()) {
                    ListAllFTBScheduledJobs(locationBrandIds[0], true);
                }
                ResetFormData(true);
                //ListAllScheduledJobs(false, jobId);
                setTimeout(function () { $('#spanSave').hide(); }, 3000);
            }
            else if ($.trim(data).toString() == 'NoMatchingDate') {
                $('#error-span').html("<b>Save Failed.</b> The Job Schedule settings are incorrect.");
                $('#error-span').show();
            }
            else if ($.trim(data).toString() == 'NoRatesTargets') {
                $('#error-span').html("<b>Error!</b> Either Rate or Target settings are not configured ");
                $('#error-span').show();
            }
            else if (data.indexOf("SuccessAutomationJobDisabled") == 0 || data.indexOf("Successnotconfiguredblackoutdates") == 0)// "AutomationJobDisabled-6/15/2016-6/18/2016"
            {
                ShowConfirmBox('All regular automation jobs for this month are disabled.', false);
                $('#spanSave').show();
                jobId = '';
                if (locationBrandIds[0] == $("#view2 #location ul li.selected").val()) {
                    ListAllFTBScheduledJobs(locationBrandIds[0], true);
                }
                ResetFormData(true);
                //ListAllScheduledJobs(false, jobId);
                setTimeout(function () { $('#spanSave').hide(); }, 3000);
            }
            else {
                $('#error-span').html("Something went wrong. Please try again.");
                $('#error-span').show();
            }
        },
        error: function (e) {
            $('.loader_container_main').hide();
            console.log(e.message);
        }
    });
}

function AddErrorToField(field) {
    $('#' + field).addClass('has-error');
    MakeTagFlashable('#' + field);
    //AddFlashingEffect();
}

function RemoveErrorToField(field) {
    $('#' + field).removeClass('has-error');
    RemoveFlashableTag('#' + field);
    //AddFlashingEffect();
}

function ResetFormData(clearForm) {
    var today = new Date();
    $('#CreateFTBJobPopup #searchLocation').val('');
    SearchGrid('#searchLocation', '#FTBlocations option');
    $('#CreateFTBJobPopup, .popup_bg').show();
    $('#CreateFTBJobPopup #runEndDate.date-picker').datepicker('option', { defaultDate: today, minDate: 0, maxDate: null });
    $("#CreateFTBJobPopup #rb-e1day").trigger('click');
    $('#view1').find('*').removeClass('flashborder').removeClass('temp');
    $('#CreateFTBJobPopup #error-span').hide();
    RemoveFlashableTag('.has-error');
    $('#CreateFTBJobPopup .has-error').removeClass('has-error');
    $('#popup-main input[type="text"]').val('');
    $("#CreateFTBJobPopup #jobDay").val('');
    $('#popup-main select').val('');
    $('#ftbjobSpinner').hide();
    $('#CreateFTBJobPopup #jobTime').val('12:00 am');
    $("#CreateFTBJobPopup #monthsdiv ul option").removeClass();
    $("#CreateFTBJobPopup #monthsdiv ul option").removeAttr('disabled');
    $('#CreateFTBJobPopup #searchLocation').removeAttr('disabled');
    $("#Tether").prop('checked', false).attr('disabled', false);
    $('#chkAllDays').prop('checked', false);
    if ((clearForm == undefined || clearForm == null) && jobId != null && jobId != '') {
        JobSelected(jobId);
    }

}

function EditJob(newJobId) {
    jobId = newJobId;
    ResetFormData();
}

function JobSelected(jobId) {
    if (jobId == null || jobId == '') {
        return;
    }
    var ajaxURl = 'FTBAutomation/GetFTBJob/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetFTBJobDetails;
    }
    $('.loader_container_main').show();
    $.ajax({
        url: ajaxURl,
        data: { JobId: jobId },
        type: 'GET',
        async: true,
        success: function (data) {
            if (data.status && data.result != null) {
                BindJobDetails(data.result);
                $('.loader_container_main').hide();
            }
            else {
                $('#error-span').html(data.message).show();
            }
        },
        error: function (e) {
            console.log(e.message);
            $('.loader_container_main').hide();
        }
    });
}

function BindJobDetails(data) {
    jobId = data.ID;
    $('#CreateFTBJobPopup .frequency input[freqid="' + data.ScheduledJobFrequencyID + '"]').trigger('click');
    var DaysToRunStartDate = convertToServerTime(new Date(parseInt(data.DaysToRunStartDate.replace("/Date(", "").replace(")/", ""), 10)));
    var StartDate = convertToServerTime(new Date(parseInt(data.StartDate.replace("/Date(", "").replace(")/", ""), 10)));
    var Year = StartDate.getFullYear();
    var Month = StartDate.getMonth();

    $("#CreateFTBJobPopup #runStartDate.date-picker").val(('0' + (DaysToRunStartDate.getMonth() + 1)).slice(-2) + "/" + ('0' + DaysToRunStartDate.getDate()).slice(-2) + "/" + DaysToRunStartDate.getFullYear());
    $('#CreateFTBJobPopup #monthsdiv ul option[value=' + Year + '][Month=' + (Month + 1) + ']').prop('selected', true);
    $('#CreateFTBJobPopup #monthsdiv ul option').attr('disabled', 'disabled');
    $('#CreateFTBJobPopup #locations ul option').attr('disabled', 'disabled');

    if (data.JobScheduleWeekDays != null && data.JobScheduleWeekDays.length > 0) {
        $("#CreateFTBJobPopup select.jobScheduleWeekDays option").each(function () {
            if ($.inArray($(this).val(), (data.JobScheduleWeekDays).split(',')) > -1) {
                $(this).prop("selected", true);
            }
        });
    }
    $('#CreateFTBJobPopup #jobTime').val(data.RunTime);
    $("#CreateFTBJobPopup #locations select option[LocationBrandId=" + data.LocationBrandID + "]").prop('selected', true);

    $('#Tether').prop('checked', data.IsActiveTethering);
    $('#Tether').attr('disabled', 'disabled');
    $('#CreateFTBJobPopup #searchLocation').attr('disabled', 'disabled');

    checkUncheckWeekDays('jobScheduleWeekDays');
}

function GetLocationSpecificMonths() {

    var brandLocation = $('#CreateFTBJobPopup #locations ul option:selected').attr('LocationBrandId');

    $("#Tether").prop('checked', false).attr('disabled', true);
    var isTetherActive = $('#CreateFTBJobPopup #locations ul option:selected').attr('Tether');

    if (isTetherActive == "true") {
        $("#Tether").prop('checked', true);
    }
    else if (isTetherActive == "false") {
        $("#Tether").prop('checked', false);
    }

    $('#CreateFTBJobPopup #monthsdiv ul select').scrollTop(0);
    $("#CreateFTBJobPopup #monthsdiv ul option:selected").prop('selected', false);
    $("#CreateFTBJobPopup #monthsdiv ul option").removeClass();
    $("#CreateFTBJobPopup #monthsdiv ul option").removeAttr('disabled');
    $('.loader_container_months').show();

    var ajaxURl = 'FTBAutomation/GetScheduledJobMonths/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetLocationJobMonths;
    }
    $.ajax({
        url: ajaxURl,
        data: { locationBrandId: brandLocation },
        type: 'GET',
        async: true,
        success: function (data) {

            if (data.status && data.result != null) {
                $(data.result).each(function () {
                    if (this.IsScheduledStopped == false) {
                        $('#CreateFTBJobPopup #monthsdiv ul option[value=' + this.Year + '][Month=' + this.Month + ']').addClass('blue').attr('disabled', 'disabled');
                    }
                    else {
                        $('#CreateFTBJobPopup #monthsdiv ul option[value=' + this.Year + '][Month=' + this.Month + ']').addClass('scheduled').attr('disabled', 'disabled');
                    }
                });
                $("#CreateFTBJobPopup #monthsdiv ul option").not('.blue').not('.scheduled').addClass('red');
            }
            else if (data.result == null) {
                $("#CreateFTBJobPopup #monthsdiv ul option").addClass('red');
            }
            $('.loader_container_months').hide();
        },
        error: function (e) {
            console.log(e.message);
            $('.loader_container_months').hide();
        }
    });
}

function selectAllDays(caller, elId) {
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
