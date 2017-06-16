
var jobSchedulesViewModel;
var userLocationBrandIds;
var lastHeaderClickedForSorting;
var previousSortOrder;
var maintainSelectedJobId;
$(document).ready(function () {

    jobSchedulesViewModel = new JobSchedulesViewModel();
    ko.applyBindings(jobSchedulesViewModel, document.getElementById("view2"));

    $('.scheduleNewJob').on('click', function () {
        FinalMinRateData = [];//Reset Minrate popup data
        FinalTetherValueData = "";
        JobSelectedTetherSetting = [];//Reset privious selected job tether data
        showHideTab('view1', 'view2');
        $('#view2 tr').removeClass('grey_bg');
        maintainSelectedJobId = "";
        EditedJobIdAvailable = "";
        jobId = '';
        resetFormSelection();
    });

    if ($('#LoggedInUserId') != undefined && $('#LoggedInUserId').val().trim() != '') {
        var loggedInUserId = parseInt($('#LoggedInUserId').val());
        $('#view2 #user ul li[value="' + loggedInUserId + '"]').addClass('selected');
        $('#view2 #user li').eq(0).val($('#view2 #user ul li[value="' + loggedInUserId + '"]').val()).text($('#view2 #user ul li[value="' + loggedInUserId + '"]').text());
        RemoveAnyUserOption();
    }

    userLocationBrandIds = '';
    $('#view2 #location ul li').slice(1).each(function () {
        if ($(this).attr('value') > 0) {
            userLocationBrandIds = userLocationBrandIds.trim() != '' ? (userLocationBrandIds + ',' + $(this).attr('value')) : $(this).attr('value');
        }
    });

    //For maintain session storage
    checkSessionStorageNLoadData();


    $('#view2 #user ul li, #view2 #location ul li').on('click', function () {
        setTimeout(function () {
            var considerCheckBoxValues = false;
            $('#view2 input[type="checkbox"]').slice(0, 4).each(function () {
                if ($(this).prop('checked')) {
                    considerCheckBoxValues = true;
                    return false;
                }
            })
            SetSessionStorage();
            FilterScheduledJobsGrid(considerCheckBoxValues);
        }, 200);
    });

    $('#view2 input[type="checkbox"]').change(function () {
        var considerCheckBoxValues = false;
        $('#view2 input[type="checkbox"]').slice(0, 4).each(function () {
            if ($(this).prop('checked')) {
                considerCheckBoxValues = true;
                return false;
            }
        })
        SetSessionStorage();
        FilterScheduledJobsGrid(considerCheckBoxValues);
    });

    $("#refreshAutomationJob").on("click", function () {
        if (maintainSelectedJobId != undefined && maintainSelectedJobId != "") {
            //for maintain selected item 
            ListAllScheduledJobs(false, maintainSelectedJobId);
        }
        else {
            ListAllScheduledJobs(false);
        }
    });
});



function FilterScheduledJobsGrid(ConsiderCheckboxValues) {

    var existingSelectedJobId = $('#view2 tr.grey_bg').attr('id');

    var userId = $('#view2 #user li').eq(0).attr('value');
    var selectedBrandLocationId = $('#view2 #location li').eq(0).attr('value');
    var isWideGap = $('#chkWideGap').prop('checked');
    var isGov = $('#chkIsGov').prop('checked');
    var isReadOnly = $("#chkIsReadOnly").prop('checked');

    var filteredJobDetails = $.map(automationJobDetails, function (item) {
        
        if (!item.IsDeleted) {

            var isFinished = false, isInProgress = false, isStopped = false, isScheduled = false;

            var isVisible = true;
            if (userId > 0) {
                if (item.CreatedByID != userId) {
                    isVisible = false;
                }
            }

            if (selectedBrandLocationId > 0) {
                if ($.inArray(selectedBrandLocationId, item.LocationBrands.split(',')) == -1) {
                    isVisible = false;
                }
            }

            if (isWideGap) {
                if (!item.IsWideGap) {
                    isVisible = false;
                }
            }
            if (isGov) {
                if (!item.IsGov) {
                    isVisible = false;
                }
            }
            if (isReadOnly) {
                if (JSON.parse(item.IsReadOnly)) {
                    isVisible = false;
                }
            }

            if (ConsiderCheckboxValues && isVisible) {
                if ($('#view2 #chkFinished').prop('checked')) {
                    if (!item.IsPresentNextRunDateTime) {
                        isFinished = true;
                    }
                }

                if ($('#view2 #chkInProgress').prop('checked')) {
                    if (item.ExecutionInProgress) {
                        isInProgress = true;
                    }
                }

                if ($('#view2 #chkStopped').prop('checked')) {
                    if (item.IsPresentNextRunDateTime && !item.IsEnabled) {
                        isStopped = true;
                    }
                }

                if ($('#view2 #chkScheduled').prop('checked')) {
                    if (item.IsPresentNextRunDateTime && item.IsEnabled && !item.ExecutionInProgress) {
                        isScheduled = true;
                    }
                }

                if (!(isFinished || isStopped || isInProgress || isScheduled)) {
                    isVisible = false;
                }
            }

            if (isVisible) {
                return new JobDetail(item);
            }
        }
    });
    if (filteredJobDetails.length == 0) {
        $('#view2 .noResults, #view2 .noResultsHeader').show();
        $('#view2 .resultsHeader').hide();
    }
    else {
        $('#view2 .noResults, #view2 .noResultsHeader').hide();
        $('#view2 .resultsHeader').show();
    }
    jobSchedulesViewModel.jobDetails(filteredJobDetails);

    if (lastHeaderClickedForSorting) {
        getSortedData(lastHeaderClickedForSorting, false);
    }

    $('#view2 [ID^="pending_"]').on('click', function () {
        var jobId = EditedJobIdAvailable = $(this).attr('id').split('_')[1];
        $('#view2 tr[id="' + EditedJobIdAvailable + '"]').addClass('grey_bg').siblings().removeClass('grey_bg');
        openMarkPopup(jobId);
    });

    $('#view2 [Id^="start"]').on('click', function () {
        EditedJobIdAvailable = $(this).attr('id').split('_')[1];
        $('#view2 tr[id="' + EditedJobIdAvailable + '"]').addClass('grey_bg').siblings().removeClass('grey_bg');
        StartStopScheduledJob($(this).attr('id').split('_')[1], false);
    });

    $('#view2 [Id^="stop"]').on('click', function () {
        EditedJobIdAvailable = $(this).attr('id').split('_')[1];
        $('#view2 tr[id="' + EditedJobIdAvailable + '"]').addClass('grey_bg').siblings().removeClass('grey_bg');
        StartStopScheduledJob($(this).attr('id').split('_')[1], true);
    });

    $('#view2 [Id^="edit"]').on('click', function () {
        var jobId = EditedJobIdAvailable = $(this).attr('id').split('_')[1];
        $('#view2 tr[id="' + jobId + '"]').addClass('grey_bg').siblings().removeClass('grey_bg');
        showHideTab('view1', 'view2');
        EditJob(jobId);
    });

    $('#view2 [Id^="delete"]').on('click', function () {
        var message = "Are you sure you want to delete job?";
        ShowConfirmBox(message, true, DeleteScheduledJob, $(this).attr('id').split('_')[1]);
    });


    if (EditedJobIdAvailable != undefined && $.trim(EditedJobIdAvailable) != '') {
        maintainSelectedJobId = EditedJobIdAvailable;
        $('#view2 tr[id="' + parseInt(EditedJobIdAvailable) + '"]').addClass('grey_bg').siblings().removeClass('grey_bg');
    }

}


function DeleteScheduledJob() {
    jobId = this;
    $('.loader_container_main').show();
    var loggedInUserId = $('#LoggedInUserId').val();

    var ajaxURl = '/RateShopper/AutomationConsole/DeleteScheduledJob';
    if (AjaxURLSettingsScheduledJobs != undefined && AjaxURLSettingsScheduledJobs != '') {
        ajaxURl = AjaxURLSettingsScheduledJobs.DeleteScheduledJob;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        async: true,
        data: { jobId: jobId.toString(), loggedInUserId: loggedInUserId },
        success: function (data) {

            if (data.toUpperCase() == 'SUCCESS') {

                $('#view2 tr[id="' + jobId + '"]').hide();
                $.each(automationJobDetails, function (index, value) {
                    if (value.Id == jobId) {
                        value.IsDeleted = true;
                    }
                });
                $.each(jobSchedulesViewModel.jobDetails(), function (index, value) {
                    if (value.Id == jobId) {
                        value.IsDeleted = true;
                    }
                });

                $('#spanDelete').show();
                setTimeout(function () {
                    $('#spanDelete').hide();
                }, 3000);

            } else {
                $('#spanError').show();
                setTimeout(function () {
                    $('#spanError').hide();
                }, 3000);
            }
            $('.loader_container_main').hide();
        },
        error: function (e) {
            //called when there is an error
            console.log("DeleteScheduledJob: " + e.message);
            $('.loader_container_main').hide();
        }
    });

}

var automationJobDetails;
var EditedJobIdAvailable;
function ListAllScheduledJobs(isFirstTime, EditedJobId) {

    //isFirstTime ? $('.loader_container_main').show() : $('.loader_container_main').hide();
    $('.loader_container_main').show();

    var ajaxURl = '/RateShopper/AutomationConsole/GetAllScheduledJobs';
    if (AjaxURLSettingsScheduledJobs != undefined && AjaxURLSettingsScheduledJobs != '') {
        ajaxURl = AjaxURLSettingsScheduledJobs.GetAllScheduledJobsUrl;
    }

    $.ajax({
        url: ajaxURl,
        type: 'GET',
        async: true,
        data: { userLocationBrandIds: userLocationBrandIds },
        success: function (data) {
            automationJobDetails = data;
            if (isFirstTime) {
                FilterScheduledJobsGrid(false);
            } else {
                var considerCheckBoxValues = false;
                $('#view2 input[type="checkbox"]').slice(0, 4).each(function () {
                    if ($(this).prop('checked')) {
                        considerCheckBoxValues = true;
                        return false;
                    }
                })

                FilterScheduledJobsGrid(considerCheckBoxValues);
                EditedJobIdAvailable = EditedJobId;
                $('#view2 tr[id="' + EditedJobId + '"]').addClass('grey_bg').siblings().removeClass('grey_bg');

            }
            //Auto refresh
            //setTimeout(function () {
            //    if (maintainSelectedJobId != undefined && maintainSelectedJobId != "") {
            //        //for maintain selected item 
            //        ListAllScheduledJobs(false, maintainSelectedJobId);
            //    }
            //    else {
            //        ListAllScheduledJobs(false);
            //    }

            //}, 59000);
            $('.loader_container_main').hide();
        },
        error: function (e) {
            //called when there is an error
            //Auto refresh
            //setTimeout(function () {
            //    if (maintainSelectedJobId != undefined && maintainSelectedJobId != "") {
            //        //for maintain selected item 
            //        ListAllScheduledJobs(false, maintainSelectedJobId);
            //    }
            //    else {
            //        ListAllScheduledJobs(false);
            //    }

            //}, 59000);
            console.log("ListAllScheduledJobs: " + e.message);
            $('.loader_container_main').hide();
        }

    })

}

function InitiateShopForScheduledJob(job) {

    if (job.ProviderId != 0 && job.ScrapperSourceIDs != '0' && job.ScrapperSourceIDs != '') {

        $('.loader_container_main').show();
        var loggedInUserId = $('#LoggedInUserId').val();

        var ajaxURl = '/RateShopper/AutomationConsole/InitiateShopForScheduledJob';
        if (AjaxURLSettingsScheduledJobs != undefined && AjaxURLSettingsScheduledJobs != '') {
            ajaxURl = AjaxURLSettingsScheduledJobs.InitiateShopForScheduledJob;
        }

        var searchModel = GetSearchModel(job);

        $.ajax({
            url: ajaxURl,
            type: 'POST',
            async: true,
            contentType: 'application/json',
            data: JSON.stringify({ searchModel: searchModel }),
            success: function (data) {
                $('.loader_container_main').hide();
                if (data.toUpperCase() == 'SUCCESS') {

                    var startDateList = job.StartDate.split("/");
                    var startDate = new Date(startDateList[2], startDateList[0] - 1, startDateList[1]);

                    if (startDate < new Date()) {
                        DisplayMessage("Shop start date is adjusted to current date.", false);
                    }

                    job.SearchSummaryStatus('IN PROGRESS');
                    job.IsInitiateShopEnable(false);
                    job.IsShopComplete(false);
                }
            }
        });

    } else {

        ShowConfirmBox('Something error occurred.', false);
    }
}

//Show the success/error messages
var DisplayMessage = function (message, isError) {
    $("#lblMessage").html(message).show().css("color", isError ? "red" : "green");
    if (!isError) {
        setTimeout(function () { $("#lblMessage").hide(); }, 3000);
    }
}


function GetSearchModel(job) {

    var searchModel = {
        ScrapperSourceIDs: job.ScrapperSourceIDs,
        LocationBrandIDs: job.LocationBrandsIds,
        RentalLengthIDs: job.RentalLengthIDs,
        CarClassesIDs: job.CarClassIds,
        StartDate: job.StartDate,
        EndDate: job.EndDate,
        CreatedBy: job.CreatedByID,
        PickUpTime: "11:00am",
        DropOffTime: "11:00am",
        location: job.LocationBrands().split('-')[0],
        CarClasses: job.CarClasses.replace(/\s+/g, ''),
        SelectedAPI: selectedAPICode,
        DropOffLocationBrandID: null,
        DropOffLocation: null,
        VendorCodes: null,
        SearchSummaryID: 0,
        PostData: null,
        ScheduleJobId: job.Id,
        UserName: $('#hdnUserName').val(),
        ProviderId: job.ProviderId,
        IsGovShop: job.IsGov
    };
    return searchModel;
}

function ShowSummaryForScheduledJob(job) {

    $('.loader_container_main').show();

    sessionStorage.setItem("SearchSummaryDetails", JSON.stringify({
        SearchSummaryId: job.SearchSummaryId,
        ScrapperSourceIDs: job.ScrapperSourceIDs,
        LocationBrandsIds: job.LocationBrandsIds,
        ShopCreatedBy: job.ShopCreatedBy,
        SummaryType: 1
    }));

    var ajaxURl = '/RateShopper/Summary/Index';
    if (AjaxURLSettingsScheduledJobs != undefined && AjaxURLSettingsScheduledJobs != '') {
        ajaxURl = AjaxURLSettingsScheduledJobs.ShowSummaryForScheduledJob;
    }
    sessionStorage.removeItem('FTBctrlState');
    window.location.href = ajaxURl;
}

function StartStopScheduledJob(jobId, stop) {

    $('.loader_container_main').show();
    var loggedInUserId = $('#LoggedInUserId').val();

    var ajaxURl = '/RateShopper/AutomationConsole/StartStopScheduledJob';
    if (AjaxURLSettingsScheduledJobs != undefined && AjaxURLSettingsScheduledJobs != '') {
        ajaxURl = AjaxURLSettingsScheduledJobs.StartStopScheduledJob;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        async: true,
        data: { jobId: jobId.toString(), stop: stop, loggedInUserId: loggedInUserId },
        success: function (data) {

            $('.loader_container_main').hide();
            if (data.Status.toUpperCase() == 'SUCCESS') {
                if (stop) {
                    $('#stop_' + jobId).hide();
                    $('#start_' + jobId).show();
                    if ($('#view2 tr[id="' + jobId + '"]').eq(0).attr('ExecutionInProgress') != undefined && JSON.parse($('#view2 tr[id="' + jobId + '"]').eq(0).attr('ExecutionInProgress'))) {
                        $('#view2 tr[id="' + jobId + '"] td').eq(1).text("IN PROGRESS");
                    }
                    else {
                        $('#view2 tr[id="' + jobId + '"] td').eq(1).text("STOPPED");
                    }
                    //$('#view2  tr[id="' + jobId + '"] td').eq(1).text("STOPPED");
                }
                else {
                    $('#stop_' + jobId).show();
                    $('#start_' + jobId).hide();
                    $('#view2 tr[id="' + jobId + '"] td.nextruntime').eq(0).text(data.NextRunTime);
                    
                    if ($('#view2 tr[id="' + jobId + '"]').eq(0).attr('ExecutionInProgress') != undefined && JSON.parse($('#view2 tr[id="' + jobId + '"]').eq(0).attr('ExecutionInProgress'))) {
                        $('#view2 tr[id="' + jobId + '"] td').eq(1).text("IN PROGRESS");                        
                    }
                    else {
                        $('#view2 tr[id="' + jobId + '"] td').eq(1).text("SCHEDULED");
                    }
                }

                //$('#view2 tr[id="' + jobId + '"]').removeClass("grey_bg_stopbyFTB");//Remove color hightlight while force fully stoped by FTB operation
                $('#view2 tr[id="' + jobId + '"] td').eq(1).removeClass("status_color").removeAttr("title");
                
                $.each(automationJobDetails, function (index, value) {
                    if (value.Id == jobId) {
                        value.IsEnabled = stop ? false : true;
                        value.IsStopByFTB = false;//Change array while user start job menually
                        value.Status = stop ? 'STOPPED' : (value.ExecutionInProgress) ? 'IN PROGRESS' : 'SCHEDULED';
                    }
                });

                $.each(jobSchedulesViewModel.jobDetails(), function (index, value) {
                    if (value.Id == jobId) {
                        value.IsEnabled = stop ? false : true;
                        value.IsStopByFTB = false;//Change array while user start job menually 
                        value.Status = stop ? 'STOPPED' : (value.ExecutionInProgress) ? 'IN PROGRESS' : 'SCHEDULED';
                    }
                });

            }
            else if (data.Status.toUpperCase() == 'NO_NEXT_RUN_FOUND') {

                $('#view2 tr[id="' + jobId + '"] .stop').hide();
                $('#view2 tr[id="' + jobId + '"] .start').hide();
                $('#view2 tr[id="' + jobId + '"] .start').not('[title="Start"]').show();

                //$('#view2 tr[id="' + jobId + '"]').css("background-color", "#ffffff");//Remove color hightlight while force fully stoped by FTB operation
                //$('#view2 tr[id="' + jobId + '"]').removeClass("grey_bg_stopbyFTB");//Remove color hightlight while force fully stoped by FTB operation
                HighlightFTBOperation(jobId);

                $.each(jobSchedulesViewModel.jobDetails(), function (index, value) {
                    if (value.Id == jobId) {
                        value.IsEnabled = false;
                        value.Status = 'FINISHED';
                        $('#view2 tr[id="' + jobId + '"] td').eq(1).text("FINISHED");
                    }
                });
                $('#spanNoNextRun').show();
                setTimeout(function () {
                    $('#spanNoNextRun').hide();
                }, 3000);
            }
            else if (data.Status.indexOf("notconfiguredblackoutdates") == 0) {
                $('#spanError').show();
                setTimeout(function () {
                    $('#spanError').hide();
                }, 3000);
                ShowConfirmBox('FTB Automation is scheduled for this Location. Regular automation can not be scheduled. ', false);
            }
            else if (data.Status.indexOf("FTBJobScheduled") == 0) {
                $('#spanError').show();
                setTimeout(function () {
                    $('#spanError').hide();
                }, 3000);
                ShowConfirmBox('Either Shop Start date or End date is out of Black-Out period ' + data.Status.split('-')[1] + " - " + data.Status.split('-')[2] + '. Automation Job cannot be scheduled.', false);
            }
            else {
                $('#spanError').show();
                setTimeout(function () {
                    $('#spanError').hide();
                }, 3000);
            }

        },
        error: function (e) {
            //called when there is an error
            console.log("Error StartStopScheduledJob: " + e.message);
            $('.loader_container_main').hide();
        }
    });
}

function JobSchedulesViewModel() {
    var self = this;
    self.jobDetails = ko.observableArray([]);
    self.headersFirstGrp = [
        { title: 'STATUS', sortPropertyName: 'Status', asc: true },
        { title: 'DATE CREATED', sortPropertyName: 'CreatedDate', asc: true }
    ];

    self.headersSecondGrp = [
        { title: 'WIDE GAP', sortPropertyName: 'IsWideGap', asc: true }
    ];

    self.headersThirdGrp = [
        { title: 'SUBMITTED BY', sortPropertyName: 'CreatedBy', asc: true }
    ];

    self.InitiateShop = function (job) {

        InitiateShopForScheduledJob(job);

    };

    self.ShowSummaryForScheduledJob = function (job) {
        ShowSummaryForScheduledJob(job);
    }

    self.sort = function (header) {
        var prop = header.sortPropertyName;
        getSortedData(prop, true);
    };
}


function getSortedData(sortPropertyName, headerClicked) {

    var prop = lastHeaderClickedForSorting = sortPropertyName;
    var currentSortOrder;

    if (headerClicked) {
        currentSortOrder = previousSortOrder = JSON.parse($('th[value = "' + sortPropertyName + '"]').attr('asc'))
        $('th[value = "' + sortPropertyName + '"]').attr('asc', !currentSortOrder);
    } else {
        currentSortOrder = previousSortOrder;
    }
    var ascSort = function (a, b) {
        return a[prop] < b[prop] ? -1 : a[prop] > b[prop] ? 1 : a[prop] == b[prop] ? 0 : 0;
    };
    var descSort = function (a, b) {
        return a[prop] > b[prop] ? -1 : a[prop] < b[prop] ? 1 : a[prop] == b[prop] ? 0 : 0;
    };

    var sortFunc;
    if (currentSortOrder) {
        sortFunc = ascSort;
        if (headerClicked) {
            $('#view2 th[value="' + sortPropertyName + '"] .aru').show();
            $('#view2 th[value="' + sortPropertyName + '"] .ard').hide();
        }
    }
    else {
        sortFunc = descSort;
        if (headerClicked) {
            $('#view2 th[value="' + sortPropertyName + '"] .aru').hide();
            $('#view2 th[value="' + sortPropertyName + '"] .ard').show();
        }
    }

    jobSchedulesViewModel.jobDetails.sort(sortFunc);
    SetSessionStorage();
}

function JobDetail(data) {

    this.Id = data.Id;
    this.LocationBrandsIds = data.LocationBrands;
    this.LocationBrands = ko.computed(function () {
        if (data.LocationBrands.indexOf(',') > 0) {
            var locationBrandsArray = data.LocationBrands.split(',');
            var LocationBrandText = '';
            for (i = 0; i < locationBrandsArray.length; i++) {
                if ($('#view2 #location ul li[value="' + $.trim(parseInt(locationBrandsArray[i]) + '"]').text()) != '') {
                    LocationBrandText = LocationBrandText.trim() != '' ? (LocationBrandText + ', ' + $('#view2 #location ul li[value="' + parseInt(locationBrandsArray[i]) + '"]').text()) : $('#view2 #location ul li[value="' + parseInt(locationBrandsArray[i]) + '"]').text();
                }
            }
            return LocationBrandText;
        }
        else {
            return $('#view2 #location ul li[value="' + parseInt(data.LocationBrands) + '"]').text();
        }
    });
    this.CarClassIds = data.CarClasses;
    this.CarClasses = computeCarClasses(data.CarClasses, "carClass");
    this.RentalLengths = data.RentalLengths;
    this.IsWideGap = data.IsWideGap;
    this.IsWideGapTemplate = ko.computed(function () {
        if (data.IsWideGap) {
            return "Yes";
        }
        else {
            return "No";
        }
    });
    this.Status = data.Status;
    this.ExecutionInProgress = data.ExecutionInProgress;
    this.IsEnabled = data.IsEnabled;
    this.IsPresentNextRunDateTime = data.IsPresentNextRunDateTime;
    this.CreatedDateAsString = data.CreatedDateAsString;
    this.CreatedDate = data.CreatedDate;
    this.CreatedBy = data.CreatedBy;

    this.CreatedByID = data.CreatedByID;
    this.ShopDates = data.ShopDates;
    this.RunDates = data.RunDates;
    this.AreReviewButtonsRequired = data.AreReviewButtonsRequired;
    this.IsReviewPending = data.IsReviewPending;
    this.IsStopByFTB = data.IsStopByFTB;
    this.Source = data.Source;
    this.CanInititateShop = data.CanInititateShop;
    this.IsInitiateShopEnable = ko.observable(data.IsInitiateShopEnable);
    this.IsShopComplete = ko.observable(data.IsShopComplete);
    this.SearchSummaryStatus = ko.observable(data.SearchSummaryStatus);
    this.StartDate = data.StartDate;
    this.EndDate = data.EndDate;
    this.IsGov = data.IsGov;
    this.ProviderId = data.ProviderId;
    this.RentalLengthIDs = data.RentalLengthIDs;
    this.ScrapperSourceIDs = data.ScrapperSourceIDs;
    this.SearchSummaryId = data.SearchSummaryId;
    this.ShopCreatedBy = data.ShopCreatedBy;
    this.IsReadOnly = data.IsReadOnly;
    this.LastRunDate = data.LastRunDate;
    this.NextRunDate = data.NextRunDate;

    this.pendingId = ko.computed(function () {
        return 'pending_' + data.Id;
    });

    this.startId = ko.computed(function () {
        return 'start_' + data.Id;
    });

    this.stopId = ko.computed(function () {
        return 'stop_' + data.Id;
    });

    this.deleteId = ko.computed(function () {
        return 'delete_' + data.Id;
    });

    this.editId = ko.computed(function () {
        return 'edit_' + data.Id;
    });
    this.initiateShop = ko.computed(function () {
        return 'initiateShop_' + data.Id;
    });

    this.showSummaryForScheduledJob = ko.computed(function () {
        return 'showSummaryForScheduledJob' + data.Id;
    });
}

function checkSessionStorageNLoadData() {
    var AutomationctrlState = JSON.parse(sessionStorage.getItem('AutomationctrlState'));
    if (AutomationctrlState != null && AutomationctrlState.length != 0) {

        if (AutomationctrlState.users != undefined && AutomationctrlState.users != null) {
            $('#view2 #user ul li').removeClass('selected');
            $('#view2 #user li').eq(0).text($('#view2 #user ul li[value=' + AutomationctrlState.users + ']').text()).attr("value", $('#view2 #user ul li[value=' + AutomationctrlState.users + ']').val())
            $('#view2 #user ul li[value=' + AutomationctrlState.users + ']').addClass("selected");
        }
        if (AutomationctrlState.locationBrandID != undefined && AutomationctrlState.locationBrandID != null) {
            $('#view2 #location ul li').removeClass('selected');
            $('#view2 #location li').eq(0).text($('#view2 #location ul li[value=' + AutomationctrlState.locationBrandID + ']').text()).attr("value", $('#view2 #location ul li[value=' + AutomationctrlState.locationBrandID + ']').val());
            $('#view2 #location ul li[value=' + AutomationctrlState.locationBrandID + ']').addClass("selected");
        }
        if (AutomationctrlState.CheckboxJobStatus != "" && AutomationctrlState.CheckboxJobStatus != null) {
            var StatusCheckbox = AutomationctrlState.CheckboxJobStatus.split(',');
            for (var i = 0; i < StatusCheckbox.length; i++) {
                $('#view2 input[type="checkbox"]').slice(0, 4).eq(StatusCheckbox[i]).prop("checked", true);
            }
        }
        $('#chkWideGap').prop("checked", AutomationctrlState.IsWideGapCheckbox);
        $('#chkIsGov').prop("checked", AutomationctrlState.IsGovCheckbox);
        if (AutomationctrlState.lastHeaderClickedForSorting != "" && AutomationctrlState.lastHeaderClickedForSorting != null) {
            lastHeaderClickedForSorting = AutomationctrlState.lastHeaderClickedForSorting;
        }
        if (AutomationctrlState.previousSortOrder != "" && AutomationctrlState.previousSortOrder != null) {
            previousSortOrder = AutomationctrlState.previousSortOrder;
        }
        //Get ALL Scheduled Jobs
        ListAllScheduledJobs(false);
    }
    else {
        //Get ALL Scheduled Jobs
        ListAllScheduledJobs(true);
    }
}

function SetSessionStorage() {
    if (sessionStorage.getItem('AutomationctrlState') != null) {
        sessionStorage.removeItem('AutomationctrlState');
    }
    var AutomationctrlState = new Object();
    AutomationctrlState.locationBrandID = ($('#view2 #location li').eq(0).attr('value'));
    AutomationctrlState.users = ($('#view2 #user li').eq(0).attr('value'));
    AutomationctrlState.CheckboxJobStatus = "";
    var checkboxindex = "";
    $('#view2 input[type="checkbox"]').slice(0, 4).each(function (index) {
        if ($(this).prop('checked')) {
            checkboxindex += index + ",";
        }
    });
    AutomationctrlState.IsWideGapCheckbox = $('#chkWideGap').prop("checked");
    AutomationctrlState.IsGovCheckbox = $('#chkIsGov').prop("checked");

    if (checkboxindex != "") {
        AutomationctrlState.CheckboxJobStatus = checkboxindex.substr(0, checkboxindex.length - 1);
    }

    AutomationctrlState.lastHeaderClickedForSorting = "";
    if (lastHeaderClickedForSorting != undefined && lastHeaderClickedForSorting != null) {
        AutomationctrlState.lastHeaderClickedForSorting = lastHeaderClickedForSorting;
    }
    AutomationctrlState.previousSortOrder = "";
    if (previousSortOrder != undefined && previousSortOrder != null) {
        AutomationctrlState.previousSortOrder = previousSortOrder;
    }

    sessionStorage.setItem('AutomationctrlState', JSON.stringify(AutomationctrlState));
}
//order car classes based on select list of car classes on create scheduled job tab
function computeCarClasses(Ids, controlID) {
    var tempName = "";
    if (controlID == "carClass") {
        $("#" + controlID + " option").each(function () {
            if ($.inArray($(this).attr('value'), Ids.split(',')) != -1) {
                tempName += $(this).text() + ", ";
            }
        });
    }
    return tempName.trim().substring(0, tempName.trim().length - 1);
}

function HighlightFTBOperation(jobId) {
    $('#view2 tr[id="' + jobId + '"] td').eq(0).find("span").removeClass("spn_R");
    $('#view2 tr[id="' + jobId + '"] td').eq(1).removeClass("status_color").removeAttr("title");
}

function RemoveAnyUserOption() {
    var isAdmin = $("#hdnIsAdminUser").val();
    if (isAdmin.toUpperCase() == "FALSE") {
        //if ($('#LoggedInUserId') != undefined && $('#LoggedInUserId').val().trim() != '') {
        //    var loggedInUserId = parseInt($('#LoggedInUserId').val());
        //    $('#quickViewList #qvuser ul li[value="' + loggedInUserId + '"]').addClass('selected');
        //    $('#quickViewList #qvuser li').eq(0).val($('#quickViewList #qvuser ul li[value="' + loggedInUserId + '"]').val()).text($('#quickViewList #qvuser ul li[value="' + loggedInUserId + '"]').text());
        //}
        //$('#quickViewList #qvuser').addClass("disable-UL");
        $('#view2 #user ul li[value="0"]').hide();
    }
}