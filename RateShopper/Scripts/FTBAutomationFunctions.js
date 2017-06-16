var LoggedUserId;
var ftbAutomationJobDetails;
var EditedFTBJobIdAvailable;
var lastHeaderClickedForSorting;
$(document).ready(function () {
    LoggedUserId = $('#LoggedInUserId').val().trim();
    DefaultSelectionOnLoad();

    $("#refreshAutomationJob").on("click", function () {
        ListAllFTBScheduledJobs($("#view2 #location ul li.selected").val(), false);
    });
    $('#view2 input[type="checkbox"]').change(function () {
        var considerCheckBoxValues = false;
        $('#view2 input[type="checkbox"]').slice(0, 4).each(function () {
            if ($(this).prop('checked')) {
                considerCheckBoxValues = true;
                return false;
            }
        });
        FilterFTBScheduledJobsGrid(considerCheckBoxValues);
    });
    $("#view2 #user ul li").on("click", function () {
        setTimeout(function () {
            var considerCheckBoxValues = false;
            $('#view2 input[type="checkbox"]').slice(0, 4).each(function () {
                if ($(this).prop('checked')) {
                    considerCheckBoxValues = true;
                    return false;
                }
            })
            FilterFTBScheduledJobsGrid(considerCheckBoxValues);
        }, 250);

    });
    $("#view2 #location ul li").on("click", function () {
        ListAllFTBScheduledJobs($(this).val(), true);
    });
});

//Entity
function JobDetail(data) {
    var self = this;
    self.Id = data.ID;
    self.LocationBrandsIds = data.LocationBrandID;
    self.LocationBrands = data.LocationBrandAlias;
    self.CarClassIds = data.CarClassIds;
    self.CarClasses = data.CarClasses;
    self.CarClassWithSpaces = data.CarClasses.split(',').join(', ');
    self.RentalLengthIds = data.RentalLengthIds;
    self.RentalLengths = data.RentalLengths;
    self.RentalLengthsWithSpaces = data.RentalLengths.split(',').join(', ');

    self.Status = data.Status;
    self.ExecutionInProgress = data.ExecutionInProgress;
    self.IsEnabled = data.IsEnabled;
    self.IsPresentNextRunDateTime = data.IsPresentNextRunDateTime;
    self.CreatedDateAsString = data.CreatedDateAsString;
    self.NextRunDateAsString = data.NextRunDateAsString;
    self.CreatedDate = data.CreatedDate;
    self.CreatedBy = data.CreatedBy;
    self.NextRunDateTime = data.NextRunDateTime;
    self.CreatedByID = data.CreatedByID;
    self.Month_Year = data.Month_Year;
    self.StartDate = data.StartDate;
    self.EndDate = data.EndDate;
    self.ShopStartDate = data.ShopStartDate;
    self.ShopEndDate = data.ShopEndDate;
    self.RunDates = data.RunDates;
    //this.AreReviewButtonsRequired = data.AreReviewButtonsRequired;
    //this.IsReviewPending = data.IsReviewPending;
    self.SourceIds = data.SourceIds;
    self.Sources = data.Sources;

    self.pendingId = ko.computed(function () {
        return 'pending_' + data.ID;
    });

    self.startId = ko.computed(function () {
        return 'start_' + data.ID;
    });

    self.stopId = ko.computed(function () {
        return 'stop_' + data.ID;
    });

    self.deleteId = ko.computed(function () {
        return 'delete_' + data.ID;
    });

    self.editId = ko.computed(function () {
        return 'edit_' + data.ID;
    });

    self.IsSplitMonth = data.IsSplitMonth;
    self.SplitAndSearchDetails = data.SplitAndSearchDetails;

    self.SearchStatusId = ko.observable();
    self.jobSplitDetails = new JobSplitDetails(self.SplitAndSearchDetails, self.LocationBrands, self.Month_Year, self.Id, false);

    if (self.jobSplitDetails.SplitDetails().length > 0) {
        if (self.jobSplitDetails.SplitDetails().length > 1) {
            if (self.jobSplitDetails.SplitDetails()[0].SearchId > self.jobSplitDetails.SplitDetails()[1].SearchId) {
                self.SearchId = self.jobSplitDetails.SplitDetails()[0].SearchId;
                self.Sources = self.jobSplitDetails.SplitDetails()[0].SourceId;
                self.ShopCreatedById = self.jobSplitDetails.SplitDetails()[0].ShopCreatedById;
                self.SearchStatusId(self.jobSplitDetails.SplitDetails()[0].StatusId);
            }
            else {
                self.SearchId = self.jobSplitDetails.SplitDetails()[1].SearchId;
                self.Sources = self.jobSplitDetails.SplitDetails()[1].SourceId;
                self.ShopCreatedById = self.jobSplitDetails.SplitDetails()[1].ShopCreatedById;
                self.SearchStatusId(self.jobSplitDetails.SplitDetails()[1].StatusId);
            }
        }
        else {
            self.SearchId = self.jobSplitDetails.SplitDetails()[0].SearchId;
            self.Sources = self.jobSplitDetails.SplitDetails()[0].SourceId;
            self.ShopCreatedById = self.jobSplitDetails.SplitDetails()[0].ShopCreatedById;
            self.SearchStatusId(self.jobSplitDetails.SplitDetails()[0].StatusId);
        }
    }
    self.SearchStatus = ko.computed(function () {
        switch (self.SearchStatusId()) {
            case 2:
            case 3:
                return "IN PROGRESS";
                break;
            case 4:
                return "COMPLETE";
                break;
            case 5:
                return "FAILED";
                break;
            default:
                return "-";
                break;
        }
    });
    self.ShowViewReportIcon = ko.computed(function () {
        if (self.SearchStatusId() == 4) {
            return true;
        }
        else if (self.jobSplitDetails.SplitDetails().length > 1 && self.jobSplitDetails.SplitDetails()[0].SearchId > 0 && self.jobSplitDetails.SplitDetails()[1].SearchId > 0) {
            return true;
        }
    });


    self.InitiateSummary = function (job) {
        $('#view2 tr[id="' + job.Id + '"]').addClass('grey_bg').siblings().removeClass('grey_bg');
        var sourceId = $("#ddlSource_" + job.Id + " li.selected").attr("value");
        var sourcecode = $("#ddlSource_" + job.Id + " li.selected").attr("srccode");
        if (typeof (sourceId) == 'undefined') {
            ShowConfirmBox("Please select a source before initiating a Summary Shop", false);
            return;
        }

        if (job.IsSplitMonth) {
            FTBJobViewModel.jobSplitDetails(new JobSplitDetails(job.SplitAndSearchDetails, job.LocationBrands, job.Month_Year, job.Id, false));
            $('.popup_bg_master').show();
            $('#popupShowSplitIntervals').show().draggable().closest('#popupShowSplitIntervals').find('#closepopup').click(function () {
                $('#view2 tr.grey_bg').removeClass('grey_bg');
                $('#popupShowSplitIntervals, .popup_bg_master').hide();
            });
        }
        else {
            var searchModel = GetSearchModel(job);
            InitiateSummaryShop(searchModel, job);
        }
    }

    self.ViewSummary = function (job) {
        $('#view2 tr[id="' + job.Id + '"]').addClass('grey_bg').siblings().removeClass('grey_bg');
        if (job.IsSplitMonth) {
            FTBJobViewModel.jobSplitDetails(new JobSplitDetails(job.SplitAndSearchDetails, job.LocationBrands, job.Month_Year, job.Id, true));
            $('.popup_bg_master').show();
            $('#popupShowSplitIntervals').show().draggable().closest('#popupShowSplitIntervals').find('#closepopup').click(function () {
                $('#view2 tr.grey_bg').removeClass('grey_bg');
                $('#popupShowSplitIntervals, .popup_bg_master').hide();
            });
        }
        else {
            sessionStorage.setItem("SearchSummaryDetails", JSON.stringify({
                SearchSummaryId: job.SearchId,
                ScrapperSourceIDs: job.Sources,
                LocationBrandsIds: job.LocationBrandsIds,
                ShopCreatedBy: job.ShopCreatedById,
                SummaryType: 2
            }));
            var ajaxURl = '/RateShopper/Summary/';
            if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
                ajaxURl = AjaxURLSettings.ShowSummary;
            }
            sessionStorage.removeItem('FTBctrlState');
            window.location.href = ajaxURl;
        }
    }

    self.DeleteSummaryShop = function (job) {
        $('#view2 tr[id="' + job.Id + '"]').addClass('grey_bg').siblings().removeClass('grey_bg');
        if (job.IsSplitMonth) {
            FTBJobViewModel.jobSplitDetails(new JobSplitDetails(job.SplitAndSearchDetails, job.LocationBrands, job.Month_Year, job.Id, false, true));
            $('.popup_bg_master').show();
            $('#popupShowSplitIntervals').show().draggable().closest('#popupShowSplitIntervals').find('#closepopup').click(function () {
                $('#view2 tr.grey_bg').removeClass('grey_bg');
                $('#popupShowSplitIntervals, .popup_bg_master').hide();
            });
        }
        else {
            var objDelete = new Object();
            objDelete.SearchId = job.SearchId;
            objDelete.JobId = job.Id;
            ShowConfirmBox("Do you want to delete summary shop?", true, DeleteSummaryShop, objDelete);
        }
    }
}

function JobSplitDetails(data, locationBrand, month, jobid, isviewsummary, isdeletesummary) {
    var self = this;
    self.JobId = jobid;
    self.LocationBrand = ko.observable(locationBrand);
    self.Month_Year = ko.observable(month);
    self.SplitDetails = ko.observable([]);
    self.IsViewSummary = isviewsummary;
    self.IsDeleteSummary = !(!isdeletesummary);
    var splitDetails = new Array();
    var splitdata = data.split('|');
    for (var i = 0; i < splitdata.length; i++) {
        if (splitdata[i].length > 1) {
            var splitObjectArray = splitdata[i].split('~');
            //split index 0-Label, 1-StartDay, 2-EndDay, 3-SearchId, 4-StatusId, 5-SourceId, 6-ShopCreatedById

            var splitObject = new Object();
            splitObject.Label = splitObjectArray[0];
            splitObject.StartDay = splitObjectArray[1];
            splitObject.EndDay = splitObjectArray[2];
            splitObject.SearchId = parseInt(splitObjectArray[3]);
            if (splitObject.SearchId > 0) {
                splitObject.StatusId = parseInt(splitObjectArray[4]);
                splitObject.SourceId = parseInt(splitObjectArray[5]);
                splitObject.ShopCreatedById = parseInt(splitObjectArray[6]);
            }
            else {
                splitObject.StatusId = 0;
                splitObject.SourceId = 0;
                splitObject.ShopCreatedById = 0;
            }
            splitDetails.push(splitObject);
        }
    }
    self.SplitDetails(splitDetails);

    self.PopupViewSummary = function (splitdetail) {
        var $selectedSplitOption = $(".splitdetails input[type='radio']:checked");
        var viewSearchId = parseInt($selectedSplitOption.attr("searchid"));
        var shopCreatedById = parseInt($selectedSplitOption.attr("createdbyid"));
        var shopStatusId = parseInt($selectedSplitOption.attr("statusId"));
        var sourceId = parseInt($selectedSplitOption.attr("sourceId"));
        if (viewSearchId > 0 && shopStatusId == '4') {
            var job;
            ko.utils.arrayForEach(FTBJobViewModel.jobDetails(), function (currentJob) {
                if (currentJob.Id == splitdetail.JobId) {
                    job = currentJob;
                    return false;
                }
            });
            sessionStorage.setItem("SearchSummaryDetails", JSON.stringify({
                SearchSummaryId: viewSearchId,
                ScrapperSourceIDs: sourceId,
                LocationBrandsIds: job.LocationBrandsIds,
                ShopCreatedBy: shopCreatedById,
                SummaryType: 2
            }));
            var ajaxURl = '/RateShopper/Summary/';
            if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
                ajaxURl = AjaxURLSettings.ShowSummary;
            }
            sessionStorage.removeItem('FTBctrlState');
            window.location.href = ajaxURl;
        }
        else {
            $("#lblSplitMessage").show();
            setTimeout(function () { $("#lblSplitMessage").hide(); }, 2000);
        }
    }

    self.PopupInitiateSummary = function (splitdetail) {
        var searchModel;
        var job;
        ko.utils.arrayForEach(FTBJobViewModel.jobDetails(), function (currentJob) {
            if (currentJob.Id == splitdetail.JobId) {
                job = currentJob;
                searchModel = GetSearchModel(currentJob);
                return false;
            }
        });
        if (!(!searchModel)) {
            $selectedSplitOption = $(".splitdetails input[type='radio']:checked");
            var startDay = $selectedSplitOption.attr("startday");
            var endDay = $selectedSplitOption.attr("endday");
            if (!(!startDay) && !(!endDay)) {
                var startDate = new Date(searchModel.StartDate);
                var endDate = new Date(searchModel.EndDate);
                startDate.setDate(startDay);
                endDate.setDate(endDay);
                searchModel.StartDate = ("0" + (startDate.getMonth() + 1)).slice(-2) + "/" + ("0" + (startDate.getDate())).slice(-2) + "/" + startDate.getFullYear();
                searchModel.EndDate = ("0" + (endDate.getMonth() + 1)).slice(-2) + "/" + ("0" + (endDate.getDate())).slice(-2) + "/" + endDate.getFullYear();
            }
            InitiateSummaryShop(searchModel, job);
            $('#popupShowSplitIntervals, .popup_bg_master').hide();
        }
    }

    self.DeleteSummary = function (splitdetail) {
        var deleteSearchId = parseInt($(".splitdetails input[type='radio']:checked").attr("searchid"));
        //ShowConfirmBox("Do you want to delete summary shop?", true, DeleteSummaryShop, deleteSearchId);
        if (deleteSearchId > 0) {
            var objDelete = new Object();
            objDelete.SearchId = deleteSearchId;
            objDelete.JobId = splitdetail.JobId;
            DeleteSummaryShop(objDelete);
        }
        else {
            $("#lblSplitMessage").show();
            setTimeout(function () { $("#lblSplitMessage").hide(); }, 2000);
        }
    }
}

//End Entity


//---Other functions operation
var GetSearchModel = function (job) {
    var sourceId = $("#ddlSource_" + job.Id + " li.selected").attr("value");
    var sourcecode = $("#ddlSource_" + job.Id + " li.selected").attr("srccode");
    if (typeof (sourceId) == 'undefined') {
        ShowConfirmBox("Please select a source before initiating a Summary Shop", false);
        return;
    }
    var searchModel = new Object();
    searchModel.FTBScheduledJobID = job.Id;
    searchModel.ScrapperSourceIDs = sourceId;
    searchModel.LocationBrandIDs = job.LocationBrandsIds;
    searchModel.RentalLengthIDs = job.RentalLengthIds;
    searchModel.CarClassesIDs = job.CarClassIds;
    searchModel.StartDate = job.ShopStartDate;
    searchModel.EndDate = job.ShopEndDate;
    searchModel.PickUpTime = "11:00am";
    searchModel.DropOffTime = "11:00am";
    searchModel.CreatedBy = $('#LoggedInUserId').val();
    searchModel.ScrapperSource = sourcecode;
    searchModel.CarClasses = job.CarClasses;
    searchModel.location = job.LocationBrands.split('-')[0];
    searchModel.SelectedAPI = $("#ddlSource_" + job.Id + " li.selected").attr("prvcode");
    if (typeof (searchModel.SelectedAPI) != "undefined" && searchModel.SelectedAPI != "") {
        searchModel.ProviderId = $("#ddlSource_" + job.Id + " li.selected").attr("providerid");
    }
    else {
        searchModel.ProviderId = 1;
    }
    searchModel.IsGovShop = false;
    searchModel.UserName = $('#hdnUserName').val();

    return searchModel;
}

function getSortedFTBData(sortPropertyName, headerClicked) {

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

    FTBJobViewModel.jobDetails.sort(sortFunc);
    //SetSessionStorage();
}

var DefaultSelectionOnLoad = function () {
    $("#view2 #location ul li").eq(0).addClass("selected");
    $("#view2 #location li").eq(0).attr("value", $("#view2 #location ul li").eq(0).val()).html($("#view2 #location ul li").eq(0).html());
    ListAllFTBScheduledJobs($("#view2 #location ul li").eq(0).val(), true);
}
var FilterFTBScheduledJobsGrid = function (ConsiderCheckboxValues) {

    //var existingSelectedJobId = $('#view2 tr.grey_bg').attr('id');

    var userId = $('#view2 #user li').eq(0).attr('value');
    var selectedBrandLocationId = $('#view2 #location li').eq(0).attr('value');
    var filteredJobDetails = $.map(ftbAutomationJobDetails, function (item) {

        if (!item.IsDeleted) {

            var isFinished = false, isInProgress = false, isStopped = false, isScheduled = false;

            var isVisible = true;
            if (userId > 0) {
                if (item.CreatedByID != userId) {
                    isVisible = false;
                }
            }

            if (selectedBrandLocationId > 0) {
                if (selectedBrandLocationId == item.LocationBrands) {
                    //isVisible = false;
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
    FTBJobViewModel.jobDetails(filteredJobDetails);

    if (lastHeaderClickedForSorting) {
        getSortedFTBData(lastHeaderClickedForSorting, false);
    }

    //$('#view2 [ID^="pending_"]').on('click', function () {
    //    var jobId = EditedFTBJobIdAvailable = $(this).attr('id').split('_')[1];
    //    $('#view2 tr[id="' + EditedFTBJobIdAvailable + '"]').addClass('grey_bg').siblings().removeClass('grey_bg');
    //    openMarkPopup(jobId);
    //});

    $('#view2 [Id^="start"]').on('click', function () {
        EditedFTBJobIdAvailable = $(this).attr('id').split('_')[1];
        $('#view2 tr[id="' + EditedFTBJobIdAvailable + '"]').addClass('grey_bg').siblings().removeClass('grey_bg');
        StartStopFTBScheduledJob(EditedFTBJobIdAvailable, false);
    });

    $('#view2 [Id^="stop"]').on('click', function () {
        EditedFTBJobIdAvailable = $(this).attr('id').split('_')[1];
        $('#view2 tr[id="' + EditedFTBJobIdAvailable + '"]').addClass('grey_bg').siblings().removeClass('grey_bg');
        StartStopFTBScheduledJob(EditedFTBJobIdAvailable, true);
    });

    $('#view2 [Id^="edit"]').on('click', function () {
        var jobId = EditedFTBJobIdAvailable = $(this).attr('id').split('_')[1];
        $('#view2 tr[id="' + jobId + '"]').addClass('grey_bg').siblings().removeClass('grey_bg');
        EditJob(jobId);
        //showHideTab('view1', 'view2');
        //EditJob(jobId);
    });

    $('#view2 [Id^="delete"]').on('click', function () {
        var message = "Are you sure you want to delete job?";
        ShowConfirmBox(message, true, DeleteFTBScheduledJob, $(this).attr('id').split('_')[1]);
    });

    $(".data-panel .table-ul-right ul").on("click", function () {
        var $itemClicked = $(this);
        if ($itemClicked.children('ul').css('display') == "none") {
            $('.hidden').hide();
        }

        $itemClicked.children('ul').toggle().find('li').on('click', function () {
            $(this).closest('.table-ul-right ul').find('li').removeClass('selected');
            $(this).addClass('selected');
            $itemClicked.find('li').eq(0).text($(this).text()).attr('value', ($(this).attr('value')));
            setTimeout(function () { $('.hidden').hide(); }, 200);
        });
    });

    $(".ddlSource").each(function () {
        var source = $(this).find(".drop-down1 li.selected").text();
        var sourcevalue = $(this).find(".drop-down1 li.selected").attr('value');
        if (typeof (sourcevalue) != 'undefined') {
            $(this).find("ul li:first").text(source).attr('value', sourcevalue);
        }
    });

    //if (EditedFTBJobIdAvailable != undefined && $.trim(EditedFTBJobIdAvailable) != '') {
    //    maintainSelectedJobId = EditedFTBJobIdAvailable;
    //    $('#view2 tr[id="' + parseInt(EditedFTBJobIdAvailable) + '"]').addClass('grey_bg').siblings().removeClass('grey_bg');
    //}

}

//Show the success/error messages
var DisplayMessage = function (message, isError) {
    $("#lblMessage").html(message).show().css("color", isError ? "red" : "green");
    if (!isError) {
        setTimeout(function () { $("#lblMessage").hide(); }, 3000);
    }
}

var RebindJob = function (data, jobId) {
    ko.utils.arrayForEach(FTBJobViewModel.jobDetails(), function (currentJob) {
        if (currentJob.Id == jobId) {
            currentJob.SplitAndSearchDetails = data;

            currentJob.jobSplitDetails = new JobSplitDetails(currentJob.SplitAndSearchDetails, currentJob.LocationBrands, currentJob.Month_Year, currentJob.Id, false);

            if (currentJob.jobSplitDetails.SplitDetails().length > 0) {
                if (currentJob.jobSplitDetails.SplitDetails().length > 1) {
                    if (currentJob.jobSplitDetails.SplitDetails()[0].SearchId > currentJob.jobSplitDetails.SplitDetails()[1].SearchId) {
                        currentJob.SearchId = currentJob.jobSplitDetails.SplitDetails()[0].SearchId;
                        currentJob.Sources = currentJob.jobSplitDetails.SplitDetails()[0].SourceId;
                        currentJob.ShopCreatedById = currentJob.jobSplitDetails.SplitDetails()[0].ShopCreatedById;
                        currentJob.SearchStatusId(currentJob.jobSplitDetails.SplitDetails()[0].StatusId);
                    }
                    else {
                        currentJob.SearchId = currentJob.jobSplitDetails.SplitDetails()[1].SearchId;
                        currentJob.Sources = currentJob.jobSplitDetails.SplitDetails()[1].SourceId;
                        currentJob.ShopCreatedById = currentJob.jobSplitDetails.SplitDetails()[1].ShopCreatedById;
                        currentJob.SearchStatusId(currentJob.jobSplitDetails.SplitDetails()[1].StatusId);
                    }
                }
                else {
                    currentJob.SearchId = currentJob.jobSplitDetails.SplitDetails()[0].SearchId;
                    currentJob.Sources = currentJob.jobSplitDetails.SplitDetails()[0].SourceId;
                    currentJob.ShopCreatedById = currentJob.jobSplitDetails.SplitDetails()[0].ShopCreatedById;
                    currentJob.SearchStatusId(currentJob.jobSplitDetails.SplitDetails()[0].StatusId);
                }
            }

            $("#ddlSource_" + currentJob.Id + " li.selected").removeClass("selected");
            if (currentJob.Sources != '0') {
                $("#ddlSource_" + currentJob.Id + " li[value='" + currentJob.Sources + "']").addClass("selected");
                $("#ddlSource_" + currentJob.Id).closest(".ddlSource").find("ul li").eq(0).text($("#ddlSource_" + currentJob.Id + " li[value='" + currentJob.Sources + "']").text()).attr('value', $("#ddlSource_" + currentJob.Id + " li[value='" + currentJob.Sources + "']").attr('value'));
            }
            else {
                $("#ddlSource_" + currentJob.Id).closest(".ddlSource").find("ul li").eq(0).text('- Select Source -').attr('value', '0');
            }
            return false;
        }
    });

}
//--End other functions operation


//Ajax Call
var ListAllFTBScheduledJobs = function (locationBrandId, isFirstTime, EditedJobId) {

    //isFirstTime ? $('.loader_container_main').show() : $('.loader_container_main').hide();
    // $('.loader_container_main').show();
    var ajaxURl = '/FTBAutomation/GetFTBAutomationJobList';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetFTBAutomationJobListURL;
    }
    var IsAdmin = $("#hdnIsAdminUser").val();
    $('.loader_container_main').show();
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        async: true,
        data: { loggedUserId: LoggedUserId, locationBrandId: locationBrandId, isAdminUser: IsAdmin },
        success: function (data) {
            $('.loader_container_main').hide();
            if (data != undefined) {
                ftbAutomationJobDetails = data;
                //console.log(ftbAutomationJobDetails);
                if (isFirstTime) {
                    FilterFTBScheduledJobsGrid(false);
                }
                else {
                    var considerCheckBoxValues = false;
                    $('#view2 input[type="checkbox"]').slice(0, 4).each(function () {
                        if ($(this).prop('checked')) {
                            considerCheckBoxValues = true;
                            return false;
                        }
                    })

                    FilterFTBScheduledJobsGrid(considerCheckBoxValues);
                    //EditedFTBJobIdAvailable = EditedJobId;
                    //$('#view2 tr[id="' + EditedJobId + '"]').addClass('grey_bg').siblings().removeClass('grey_bg');

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
                //  $('.loader_container_main').hide();
            }
            else {
                $('#view2 .noResults, #view2 .noResultsHeader').show();
                $('#view2 .resultsHeader').hide();
            }
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
var StartStopFTBScheduledJob = function (jobId, stop) {
    var loggedInUserId = $('#LoggedInUserId').val();

    var ajaxURl = '/FTBAutomation/StartStopFTBScheduledJob';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.StartStopFTBScheduledJobURL;
    }
    $('.loader_container_main').show();
    $.ajax({
        url: ajaxURl,
        type: 'POST',
        async: true,
        data: { jobId: jobId.toString(), stop: stop, loggedInUserId: loggedInUserId },
        success: function (data) {
            $('.loader_container_main').hide();
            if (data.toUpperCase() == 'SUCCESS' || data.indexOf("SuccessAutomationJobDisabled") == 0 || data.indexOf("Successnotconfiguredblackoutdates") == 0) {
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
                    if ((data.indexOf("SuccessAutomationJobDisabled") == 0) || (data.indexOf("Successnotconfiguredblackoutdates") == 0))// "AutomationJobDisabled-6/15/2016-6/18/2016"
                    {
                        ShowConfirmBox('All regular automation jobs for this month are disabled.', false);
                    }
                    $('#stop_' + jobId).show();
                    $('#start_' + jobId).hide();
                    if ($('#view2 tr[id="' + jobId + '"]').eq(0).attr('ExecutionInProgress') != undefined && JSON.parse($('#view2 tr[id="' + jobId + '"]').eq(0).attr('ExecutionInProgress'))) {
                        $('#view2 tr[id="' + jobId + '"] td').eq(1).text("IN PROGRESS");
                    }
                    else {
                        $('#view2 tr[id="' + jobId + '"] td').eq(1).text("SCHEDULED");
                    }
                }


                $.each(ftbAutomationJobDetails, function (index, value) {
                    if (value.Id == jobId) {
                        value.IsEnabled = stop ? false : true;
                        value.Status = stop ? 'STOPPED' : (value.ExecutionInProgress) ? 'IN PROGRESS' : 'SCHEDULED';
                    }
                });

                $.each(FTBJobViewModel.jobDetails(), function (index, value) {
                    if (value.Id == jobId) {
                        value.IsEnabled = stop ? false : true;
                        value.Status = stop ? 'STOPPED' : (value.ExecutionInProgress) ? 'IN PROGRESS' : 'SCHEDULED';
                    }
                });

            }
            else if (data.toUpperCase() == 'NO_NEXT_RUN_FOUND') {

                $('#view2 tr[id="' + jobId + '"] .stop').hide();
                $('#view2 tr[id="' + jobId + '"] .start').hide();
                $('#view2 tr[id="' + jobId + '"] .start').not('[title="Start"]').show();
                $.each(FTBJobViewModel.jobDetails(), function (index, value) {
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
function DeleteFTBScheduledJob() {
    var jobId = this;
    //$('.loader_container_main').show();
    var loggedInUserId = $('#LoggedInUserId').val();

    var ajaxURl = '/FTBAutomation/DeleteFTBScheduledJob';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.DeleteFTBScheduledJobURL;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        async: true,
        data: { jobId: jobId.toString(), loggedInUserId: loggedInUserId },
        success: function (data) {

            if (data.toUpperCase() == 'SUCCESS') {

                $('#view2 tr[id="' + jobId + '"]').hide();
                $.each(ftbAutomationJobDetails, function (index, value) {
                    if (value.Id == jobId) {
                        value.IsDeleted = true;
                    }
                });
                $.each(FTBJobViewModel.jobDetails(), function (index, value) {
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

function InitiateSummaryShop(searchModel, job) {
    var ajaxURl = '/FTBAutomation/InitiateSummaryShop';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.InitiateSummaryShop;
    }
    $('.loader_container_main').show();
    $.ajax({
        url: ajaxURl,
        type: 'POST',
        data: searchModel,
        success: function (data) {
            $('.loader_container_main').hide();
            if (data != 'Failed') {
                RebindJob(data, searchModel.FTBScheduledJobID);
            }
            else {
                ShowConfirmBox("Something went wrong. Please try again.", false);
            }

        },
        error: function (e) {
            $('.loader_container_main').hide();
            //called when there is an error
            console.log(e.message);

        }
    });
}

function DeleteSummaryShop(objDelete) {
    if (typeof (objDelete) == 'undefined') {
        objDelete = this;
    }
    var ajaxURl = '/FTBAutomation/DeleteSummaryShop';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.DeleteSummaryShop;
    }
    $('.loader_container_main').show();
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        data: { searchSummaryId: objDelete.SearchId, loggedUserId: LoggedUserId, locationBrandId: $("#view2 #location ul li.selected").val(), jobId: objDelete.JobId },
        success: function (data) {
            $('.loader_container_main').hide();
            $('#popupShowSplitIntervals, .popup_bg_master').hide();
            RebindJob(data, objDelete.JobId);
            DisplayMessage("Summary shop deleted successfully", false);
        },
        error: function (e) {
            $('.loader_container_main').hide();
            //called when there is an error
            console.log(e.message);

        }
    });
}
//End Ajax call 