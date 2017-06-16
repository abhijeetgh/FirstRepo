var automationViewModel;
var rentalLengths, noOfPages;
var currentPage = 1;
var pageSize = 8;
var jobId1;
var classicViewCarClassId;
var classicViewBrandID;
var classicViewBrandCode;
var showAdditionalBase = false;
var myCompanyId = 0;
var isGOV;
var days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
var months = ['01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12'];
$(document).ready(function () {
    automationViewModel = new automationModel();
    ko.applyBindings(automationViewModel, document.getElementById('markResultsDiv'));
    $("#srpopup").draggable();
    $('[id^="closepopup"]').on('click', function () {
        $('#srpopup ,#popupMark').hide();
        $('#srpopup').css('height', '');
    });

    //previous button click
    $('#markResultsDiv .dailyViewPrevDay').unbind('click').bind('click', function () {
        prevDayClicked();
        enableDisableControls();
    });
    //next button click
    $('#markResultsDiv .dailyViewNextDay').unbind('click').bind('click', function () {
        nextDayClicked();
        enableDisableControls();
    });


});

function openMarkPopup(scheduleJobId) {
    $('#srpopup ,#popupMark').show();
    //automationViewModel.scheduledRun = '';
    //automationViewModel.lors = '';
    //automationViewModel.daysDropdown = '';
    $(window).scrollTop(0);
    jobId1 = scheduleJobId;
    getJobRunLog(scheduleJobId, 0);
}

function automationModel() {
    var self = this;
    self.scheduledRun = ko.observableArray([]);
    self.sources = ko.observableArray([]);
    self.lorsFilter = ko.observableArray([]);
    self.daysFilter = ko.observableArray([]);
    //self.GridResults = ko.observableArray([]);
    self.locations = ko.observableArray([]);
    self.cars = ko.observableArray([]);
    self.headers = ko.observableArray([]);
    self.rates = ko.observableArray([]);
    self.ratesClassic = ko.observableArray([]);
}

function scheduledRun(data) {
    var self = this;
    self.ocurrence = ko.computed(function () {
        return data.RunDate + ' ' + data.RunTime;
    });
    self.SummaryId = data.SearchSummaryID;
    self.Reviewed = data.IsReviewed;
    self.reviewClass = ko.computed(function () {
        if (data.IsReviewed) {
            return "pointer search-entry reviewedCheckMark";
        }
        else {
            return "pointer search-entry"
        }
    });
    self.liItemId = ko.computed(function () {
        return 'li_' + data.SearchSummaryID;
    });
    self.isGOVShop = ko.computed(function () {
        if (data != null && data.IsGOVShop) {
            return "true";
        }
        else {
            return "false";
        }
    });
}

function source(data) {
    this.ID = data.ScrapperSourceId;
    this.Name = data.Source;
}

function lors(data) {
    this.MappedID = data.RentalLengthId;
    this.Code = data.RentalLength;
}

function daysDropdown(data) {
    this.StartDate = data.StartDate;
    this.StartDateVal = data.StartDateVal;
}

function locationFilter(data) {
    this.LocationID = data.LocationId;
    this.LBID = data.BrandId;
    this.Code = data.Code;
}

function GridResult(data) {
    var self = this;
    self.CarClassCode = data.CarClassCode;
    self.CarClassLogo = data.CarClassLogo;
    self.BaseRate = ko.computed(function () {
        if (!isNaN(data.BaseRate) && data.BaseRate > 0) {
            return '$' + data.BaseRate.toFixed(2);
        }
        else {
            return '';
        }
    });
    self.TotalRate = ko.computed(function () {
        if (!isNaN(data.TotalRate) && data.TotalRate > 0) {
            return '$' + data.TotalRate;
        }
        else {
            return '';
        }
    });
    self.CompanyBaseRate = ko.computed(function () {
        return '$' + data.CompanyBaseRate;
    });
    self.CompanyTotalRate = ko.computed(function () {
        return '$' + data.CompanyTotalRate;
    });
    self.IsReviewed = data.IsReviewed;
    self.Day = data.Day;
    self.SuggestedRateId = data.SuggestedRateId;
    self.compareBaseRateClass = ko.computed(function () {
        if (!isNaN(data.BaseRate) && !isNaN(data.CompanyBaseRate)) {
            if (parseFloat(data.BaseRate) > 0) {
                if ((parseFloat(data.BaseRate)) < (parseFloat(data.CompanyBaseRate))) {
                    return "grid-arrow-red-classic";
                }
                else if (parseFloat(data.BaseRate) > parseFloat(data.CompanyBaseRate)) {
                    return "grid-arrow-green-classic";
                }
                else if (parseFloat(data.BaseRate) == parseFloat(data.CompanyBaseRate)) {
                    return "checked-ok";
                }
            }
        }
    });
    self.compareTotalRateClass = ko.computed(function () {
        if (!isNaN(data.TotalRate) && !isNaN(data.CompanyTotalRate)) {
            if (parseFloat(data.TotalRate) > 0) {
                if (parseFloat(data.TotalRate) < parseFloat(data.CompanyTotalRate)) {
                    return "grid-arrow-red-classic";
                }
                else if (parseFloat(data.TotalRate) > parseFloat(data.CompanyTotalRate)) {
                    return "grid-arrow-green-classic";
                }
                else if (parseFloat(data.TotalRate) == parseFloat(data.CompanyTotalRate)) {
                    return "checked-ok";
                }
            }
        }
    });
    self.noBaseRates = ko.computed(function () {
        if (!isNaN(data.BaseRate) && data.BaseRate > 0) {
            return false;
        }
        else {
            return true;
        }
    });
    self.noTotalRates = ko.computed(function () {
        if (!isNaN(data.TotalRate) && data.TotalRate > 0) {
            return false;
        }
        else {
            return true;
        }
    });

    //self.CompanyName = data.CompanyName;

    //tether rates
    //self.TetherBrandID = data.TetherBrandID;
    //self.TetherCompanyName = data.TetherCompanyName;

    self.noTetherBaseRate = false;

    self.TetherBaseRate = ko.computed(function () {
        if (!isNaN(data.TetherBaseRate) && data.TetherBaseRate > 0) {
            return '$' + data.TetherBaseRate.toFixed(2);

        }
        else {
            self.noTetherBaseRate = true;
            return '';
        }
    });

    self.noTetherTotalRate = false;

    self.TetherTotalRate = ko.computed(function () {
        if (!isNaN(data.TetherTotalRate) && data.TetherTotalRate > 0) {
            return '$' + data.TetherTotalRate;
        }
        else {
            self.noTetherTotalRate = true;
            return '';
        }
    });
}

function HeaderInfo(data) {
    this.companyID = ko.computed(function () {
        return data.CompanyID;
    });
    this.companyName = ko.computed(function () {
        return data.CompanyName;
    });
    this.logo = ko.computed(function () {
        return data.Logo;
    });
    this.inside = ko.computed(function () {
        return data.Inside;
    });
}

function RatesInfo(data) {
    //console.log(data);
    this.carClassID = ko.computed(function () {
        return data.CarClassID;
    });
    this.carClass = ko.computed(function () {
        return data.CarClass;
    });
    this.carClassLogo = ko.computed(function () {
        return data.CarClassLogo;
    });

    this.IsGOV = ko.computed(function () {
        if (1 == 1) {
            return true;
        }
        else {
            return false;
        }
    });
    this.companyDetails = ko.observableArray($.map(data.CompanyDetails, function (item) { return new CompanyDetails(item); }));

}

function CompanyDetails(data) {

    this.companyID = ko.computed(function () {
        return data.CompanyID;
    });
    this.companyName = ko.computed(function () {
        return data.CompanyName;
    });
    this.totalValue = ko.computed(function () {
        return commaSeparateNumber($.isNumeric(data.TotalValue) ? data.TotalValue.toFixed(2) : data.TotalValue);
    });
    this.baseValue = ko.computed(function () {
        if ($.isNumeric(data.BaseValue)) {
            return commaSeparateNumber(data.BaseValue);
        }
        else {
            return '--';
        }
    });

    this.perDay = ko.computed(function () {
        if ($.isNumeric(data.BaseValue)) {
            return ('/' + $('#length li').eq(0).html().trim().charAt(0));
        }
        else {
            return '';
        }
    });

    this.islowest = ko.computed(function () {
        return data.Islowest;
    });

    this.islowestAmongCompetitors = ko.computed(function () {
        return data.IslowestAmongCompetitors;
    });
}

function carClassFilter(data) {
    var self = this;
    self.ID = data.ID;
    self.Code = data.Code;
}

function RateInfoClassic(item) {
    var self = this;

    var d = convertToServerTime(new Date(parseInt(item.DateInfo.replace("/Date(", "").replace(")/", ""), 10)));
    self.date = new Date(d.getFullYear(), d.getMonth(), d.getDate());
    self.dateInfo = months[d.getMonth()] + '-' + ('0' + (d.getDate())).slice(-2) + '-' + d.getFullYear() + ' <br /> ' + days[d.getDay()].substring(0, 3);
    self.DateFormat = d.getFullYear() + months[d.getMonth()] + d.getDate();
    self.companyDetailsClassic = ko.observableArray($.map(item.CompanyDetails, function (item) { return new CompanyDetailsClassic(item) }));
}

function CompanyDetailsClassic(data) {
    this.companyID = data.CompanyID;
    this.companyCode = data.CompanyCode;
    this.inside = data.Inside;
    this.baseValue = commaSeparateNumber(data.BaseValue);
    this.totalValue = commaSeparateNumber($.isNumeric(data.TotalValue) ? data.TotalValue.toFixed(2) : data.TotalValue);
    this.islowestAmongCompetitors = data.IslowestAmongCompetitors;
}


function getJobRunLog(scheduledJobId, summaryId) {
    //$('.loader_container_main').show();
    currentPage = 1;
    $('#reviewWaitSpinner').show();
    $('#searchResultRates').hide();
    $('#daily-rates-table').hide();
    $('#NoRecordFoundMessage').hide();
    $('.tetherCol').hide();
    $('#markResultsDiv #jobHistory').empty();
    $('#markResultsDiv #locationFilter #lengths').empty();
    $('#markResultsDiv #scrapperSources').empty();
    var ajaxURl = '/RateShopper/AutomationConsole/GetJobOccurences';
    if (MarkResult != undefined && MarkResult != '') {
        ajaxURl = MarkResult.GetJobLog;
    }
    $.ajax({
        type: "GET",
        url: ajaxURl,
        dataType: "json",
        data: { scheduledJobID: scheduledJobId, firstPage: true, searchSummaryId: summaryId },
        success: function (data) {
            $('#reviewWaitSpinner').hide();
            if (data) {
                $('#viewSelect li').eq(0).text($('#viewSelect ul li').removeClass('selected').eq(0).text()).attr('value', $('#markResultsDiv #viewSelect ul li').eq(0).addClass('selected').attr('value'));
                if (data.jobs != null) {
                    setCurrentSearchSummaryStatus(data.jobs[0].StatusId);
                    $('#markResultsDiv #jobHistory').empty();
                    if (data.jobs) {
                        var jobs = $.map(data.jobs, function (item) { return new scheduledRun(item); });
                        automationViewModel.scheduledRun(jobs);
                    }
                    if (parseInt(data.jobs[0].StatusId) == 4 && data.searchResults == null) {
                        $("#NoRecordFoundMessage").html("No records found.").show();
                    }
                    if (data.resultFilters) {
                        $('#markResultsDiv #companyLogo').attr('src', data.resultFilters.CompanyLogo);
                        $('#markResultsDiv #companyLogo').attr('alt', data.resultFilters.BrandId);

                        $('#markResultsDiv #locationFilter').empty();
                        $('#markResultsDiv #scrapperSources').empty();
                    }

                    if (jobs != null) {
                        if (jobs.length > pageSize) {
                            noOfPages = Math.ceil(jobs.length / pageSize);//calculate no.of pages
                        }
                        else {
                            noOfPages = 1;
                        }
                    }
                    $('#markResultsDiv #pagingList').empty();
                    for (i = 0; i < noOfPages; i++) {
                        var pageNum = i + 1;
                        $('#markResultsDiv #pagingList').append('<a href="#" class="inActivePage" rel="' + pageNum + '" id="pageNumber_' + i + '">' + pageNum + '</a> ');
                    }
                    $('#markResultsDiv #jobHistory li').hide().slice(0, pageSize).show();
                    $('#markResultsDiv #jobHistory li:first').addClass('rsselected');
                    $('#markResultsDiv #pagingList a:first').removeClass('inActivePage');
                    $('#markResultsDiv #pagingList a:first').addClass('activePage');
                    var isReviewed = $('#markResultsDiv #jobHistory li:first').attr('reviewStatus');
                    if (isReviewed) {
                        $('#reviewButton').hide();
                        $('#spnReviewedStatus').show();
                    }
                    else if (!isReviewed) {
                        $('#reviewButton').show();
                        $('#spnReviewedStatus').hide();
                    }
                }
                $('#markResultsDiv #pagingList a').bind('click', function () {
                    var currPage = $(this).attr('rel');
                    if (currentPage != currPage) {
                        $(this).siblings().addClass('inActivePage');
                        $(this).removeClass('inActivePage');
                        $(this).addClass('activePage');
                        var startItem = (currPage - 1) * pageSize;
                        var endItem = startItem + pageSize;
                        currentPage = currPage;
                        $('#markResultsDiv #jobHistory li').removeClass('rsselected').hide().slice(startItem, endItem).show();
                        $('#markResultsDiv #jobHistory li').eq(startItem).addClass('rsselected');
                        getJobDetails(0, false, $('#markResultsDiv #jobHistory li').eq(startItem).attr('value'));
                        var isReviewed = $('#markResultsDiv #jobHistory li').eq(startItem).attr('reviewStatus');
                        if (isReviewed) {
                            $('#reviewButton').hide();
                            $('#spnReviewedStatus').show();
                        }
                        else if (!isReviewed) {
                            $('#reviewButton').show();
                            $('#spnReviewedStatus').hide();
                        }
                    }
                });

                if (data.allFilters != null) {
                    if (data.allFilters.daysFilter) {
                        var days = $.map(data.allFilters.daysFilter, function (item) { return new daysDropdown(item); });
                        automationViewModel.daysFilter(days);
                    }
                    if (data.allFilters.lorFilters) {
                        var lorFilter = $.map(data.allFilters.lorFilters, function (item) { return new lors(item); });
                        automationViewModel.lorsFilter(lorFilter);
                    }
                    if (data.allFilters.scrapperSources) {
                        var scrapperSources = $.map(data.allFilters.scrapperSources, function (item) { return new source(item); });
                        automationViewModel.sources(scrapperSources);
                    }
                    if (data.allFilters.locations) {
                        var locationsFilter = $.map(data.allFilters.locations, function (item) { return new locationFilter(item); });
                        automationViewModel.locations(locationsFilter);
                    }
                    if (data.allFilters.carClassFilters) {
                        var carFilter = $.map(data.allFilters.carClassFilters, function (item) { return new carClassFilter(item); });
                        automationViewModel.cars(carFilter);
                    }
                }

                $("#markResultsDiv ul.hidden.drop-down1").each(function () {
                    $(this).find('li').eq(0).addClass('selected').closest('.dropdown').find('li').eq(0).attr('value', $(this).find('li').eq(0).attr('value')).text($(this).find('li').eq(0).text());
                });

                myCompanyId = $('#location ul li.selected').attr('lbid');

                //bind right side grid 
                if (data.jobs != null) {
                    $('#markResultsDiv #NoRecordFoundMessage').hide();
                    if (data.jobs && (parseInt(data.jobs[0].StatusId) != 5) || data.jobs[0].Response.toUpperCase() == "OK") {
                        if (data.searchResults != undefined && data.searchResults != null) {

                            var finalData = JSON.parse(data.searchResults.finalData);
                            var headerData = $.map(finalData.HeaderInfo, function (item) { return new HeaderInfo(item) });

                            automationViewModel.headers(headerData);
                            $('#markResultsDiv #searchResultRates').show();

                            var rateData = $.map(finalData.RatesInfo, function (item) { return new RatesInfo(item); });
                            automationViewModel.rates(rateData);
                            $('.classic_view').hide();
                            $('.daily_view, .classic_hide').show();
                            $('.dailyHeader').show();
                            $("#NoRecordFoundMessage").hide();
                            BindSuggestedRates(data.searchResults.suggestedRate);
                        } else {
                            $('.classic_view').hide();
                            $('#markResultsDiv #searchResultRates').hide();
                            $('#markResultsDiv #pagingList').empty();
                            $('#markResultsDiv #NoRecordFoundMessage').html("No Records Found").show();
                        }
                    }
                    else {
                        $('.classic_view').hide();
                        $('#markResultsDiv #NoRecordFoundMessage').html("Job Failed:</br>" + data.jobs[0].Response + "</br>").show();
                        $('#markResultsDiv #searchResultRates').hide();
                        $('#markResultsDiv #pagingList').empty();
                    }
                }
                else {
                    $('#markResultsDiv #pagingList').empty();
                    $('#markResultsDiv #searchResultRates, #daily-rates-table').hide();
                    $('#markResultsDiv #NoRecordFoundMessage').html("No Records Found").show();

                }

                if ($('#markResultsDiv #jobHistory li').length > 0) {
                    $("#markResultsDiv #jobHistory li").on('click', function () {
                        $(this).addClass("rsselected");
                        $(this).siblings("li").removeClass("rsselected");
                        getJobDetails(0, false, $(this).attr('value'));
                        var isReviewed = $(this).attr('reviewStatus');
                        if (isReviewed) {
                            $('#reviewButton').hide();
                            $('#spnReviewedStatus').show();
                        }
                        else if (!isReviewed) {
                            $('#reviewButton').show();
                            $('#spnReviewedStatus').hide();
                        }
                    });
                }
                $('#markResultsDiv .dailyFilter ul ul li').unbind('click').bind('click', function () {

                    setLORChangeDateReset($(this));

                    enableDisableControls();
                    setTimeout(function () { changeView(); }, 100);

                });

                enableDisableControls();

                var rentalLength = $.trim($('#markResultsDiv #length li.selected').text().toLowerCase());
                if (rentalLength.indexOf('w') > -1) {
                    $('[id^="spnRateDenom"]').html('/W');
                }
                else if (rentalLength.indexOf('d') > -1) {
                    $('[id^="spnRateDenom"]').html('/D');
                }
            }
        },
        error: function (e) {
            $('#reviewWaitSpinner').hide();
            console.log(e.message);
        }
    });
}

function showHideTetherColumn(suggestedRateMaster) {
    if (suggestedRateMaster != null && suggestedRateMaster.SearchSuggestedRates.length <= 0) {
        return false;
    }
    $('.dominantName').html(suggestedRateMaster.CompanyName);
    if (suggestedRateMaster.TetherBrandID != null && suggestedRateMaster.TetherBrandID > 0) {
        $('.tetherCol').show(); //$('.tetherCol').css('display', 'block');
        $('.dependantName').html(suggestedRateMaster.TetherCompanyName).attr('brandId', suggestedRateMaster.TetherBrandID);
    }
    else {
        $('.tetherCol').hide(); //$('.tetherCol').css('display', 'none');
    }
}

function setLORChangeDateReset(self) {
    var elementId = self.parents('ul').parents('ul').attr('id');
    if (elementId.indexOf('length') >= 0) {
        //if user change the length then set date to default value
        $('#markResultsDiv #displayDay li').eq(0).text($('#markResultsDiv #displayDay ul li')
            .removeClass('selected').eq(0).text()).attr('value', $('#markResultsDiv #displayDay ul li').eq(0).addClass('selected').attr('value'));
    }
}

function setCurrentSearchSummaryStatus(statusId) {
    $("#CurrentJobSearchSummaryStatus").val(statusId);
}

function getJobDetails(scheduledJobId, firstPage, SummaryId) {
    $('#reviewWaitSpinner').show();
    $('#searchResultRates').hide();
    $('#NoRecordFoundMessage').hide();
    var searchSummaryId = SummaryId;
    var ajaxURl = '/RateShopper/AutomationConsole/GetJobOccurences';
    if (MarkResult != undefined && MarkResult != '') {
        ajaxURl = MarkResult.GetJobLog;
    }
    $.ajax({
        type: "GET",
        url: ajaxURl,
        dataType: "json",
        data: { scheduledJobID: scheduledJobId, firstPage: firstPage, searchSummaryId: SummaryId },
        success: function (data) {
            $('#reviewWaitSpinner').hide();
            if (data) {
                $('#viewSelect li').eq(0).text($('#viewSelect ul li').removeClass('selected').eq(0).text()).attr('value', $('#markResultsDiv #viewSelect ul li').eq(0).addClass('selected').attr('value'));
                //bind right side grid 
                $('#markResultsDiv #NoRecordFoundMessage').hide();
                $('.loader_container_main').hide();
                if (data.allFilters != null) {
                    if (data.allFilters.daysFilter) {
                        var days = $.map(data.allFilters.daysFilter, function (item) { return new daysDropdown(item); });
                        automationViewModel.daysFilter(days);
                    }
                    if (data.allFilters.lorFilters) {
                        var lorFilter = $.map(data.allFilters.lorFilters, function (item) { return new lors(item); });
                        automationViewModel.lorsFilter(lorFilter);
                    }
                    if (data.allFilters.scrapperSources) {
                        var scrapperSources = $.map(data.allFilters.scrapperSources, function (item) { return new source(item); });
                        automationViewModel.sources(scrapperSources);
                    }
                    if (data.allFilters.locations) {
                        var locationsFilter = $.map(data.allFilters.locations, function (item) { return new locationFilter(item); });
                        automationViewModel.locations(locationsFilter);
                    }
                    if (data.resultFilters) {
                        $('#markResultsDiv #companyLogo').attr('src', data.resultFilters.CompanyLogo);
                    }
                    if (data.allFilters.carClassFilters) {
                        var carFilter = $.map(data.allFilters.carClassFilters, function (item) { return new carClassFilter(item); });
                        automationViewModel.cars(carFilter);
                    }
                }
                $("#markResultsDiv ul.hidden.drop-down1").each(function () {
                    $(this).find('li').eq(0).addClass('selected').closest('.dropdown').find('li').eq(0).attr('value', $(this).find('li').eq(0).attr('value')).text($(this).find('li').eq(0).text());
                });

                //Set Default first search summary status id
                if (data.jobs != null) {
                    setCurrentSearchSummaryStatus(data.jobs[0].StatusId);
                    if (data.jobs && (parseInt(data.jobs[0].StatusId) != 5 || data.jobs[0].Response.toUpperCase() == "OK")) {
                        if (data.searchResults != undefined && data.searchResults != null) {
                            var finalData = JSON.parse(data.searchResults.finalData);
                            var headerData = $.map(finalData.HeaderInfo, function (item) { return new HeaderInfo(item) });

                            automationViewModel.headers(headerData);
                            $('#markResultsDiv #searchResultRates').show();

                            var rateData = $.map(finalData.RatesInfo, function (item) { return new RatesInfo(item); });
                            automationViewModel.rates(rateData);
                            $('.classic_view').hide();
                            $('.daily_view, .classic_hide').show();
                            $('.dailyHeader').show();
                            $("#NoRecordFoundMessage").hide();
                            BindSuggestedRates(data.searchResults.suggestedRate);
                        }
                        else if (parseInt(data.jobs[0].StatusId) == 4 && data.searchResults == null) {
                            $("#NoRecordFoundMessage").html("No records found.").show();
                        }
                    }
                    else {
                        $('.classic_view').hide();
                        $('#markResultsDiv #NoRecordFoundMessage').html("Job Failed:</br>" + data.jobs[0].Response + "</br>").show();
                        $('#markResultsDiv #searchResultRates').hide();

                        $('#markResultsDiv #pagingList').empty();
                    }

                }
                $("#markResultsDiv .dailyFilter ul ul li").unbind('click').bind('click', function () {

                    setLORChangeDateReset($(this));

                    setTimeout(function () { changeView(); }, 100);
                });
                $("#markResultsDiv ul.hidden.drop-down1").each(function () {
                    $(this).find('li').eq(0).addClass('selected').closest('.dropdown').find('li').eq(0).attr('value', $(this).find('li').eq(0).attr('value')).text($(this).find('li').eq(0).text());
                });
                //
                var rentalLength = $.trim($('#markResultsDiv #length li.selected').text().toLowerCase());
                if (rentalLength.indexOf('w') > -1) {
                    $('[id^="spnRateDenom"]').html('/W');
                }
                else if (rentalLength.indexOf('d') > -1) {
                    $('[id^="spnRateDenom"]').html('/D');
                }
                enableDisableControls();
            }
        },
        error: function (e) {
            $('#reviewWaitSpinner').hide();
            console.log(e.message);
        }
    });
}

function changeView() {
    var viewName = $('#viewSelect .selected').attr('value');

    $('#reviewWaitSpinner').show();
    $('#NoRecordFoundMessage').hide();
    $('#srpopup').css('height', '');
    if (viewName == 'classic') {
        $('#daily-rates-table').show();
        $('.daily_view, .classic_hide').hide();
        $('.classic_view').show();
        $('[id^="length"]').show();
        $('.carClassItem').css('display', 'inline');
        FilterClassicViewResult();
    }
    else {
        $('.classic_view').hide();
        //$('#markResultsDiv #searchResultRates').show();
        $('.daily_view, .classic_hide').show();
        //HideShowNavigation(false);
        FilterResults();
    }
}

function prevDayClicked() {

    if ($('#displayDay ul li.selected').index() != 0) {
        $('#displayDay ul li.selected').removeClass('selected').prev().addClass('selected');

    }
    else {
        if ($('#length ul li.selected').index() != 0) {
            $('#length ul li.selected').removeClass('selected').prev().addClass('selected');
            $('#displayDay ul li.selected').removeClass('selected');
            $('#displayDay ul li').last().addClass('selected');
        }
    }
    $('#displayDay li').eq(0).text($('#displayDay ul li.selected').text()).attr('value', $('#displayDay ul li.selected').attr('value'));
    $('#length li').eq(0).text($('#length ul li.selected').text()).attr('value', $('#length ul li.selected').attr('value'));
    //filterResults();
    var selectedView = $('#viewSelect .selected').attr('value');
    if (selectedView.toLowerCase() == 'classic') {
        $('#daily-rates-table').show();
        $('.daily_view, .classic_hide').hide();
        $('.classic_view').show();
        $('[id^="length"]').show();
        $('.carClassItem').css('display', 'inline');
        FilterClassicViewResult();
    }
    else {
        $('.classic_view').hide();
        //$('#markResultsDiv #searchResultRates').show();
        $('.daily_view, .classic_hide').show();
        FilterResults();
    }
}

function nextDayClicked() {
    if ($('#displayDay ul li').length != $('#displayDay ul li.selected').index() + 1) {
        $('#displayDay ul li.selected').removeClass('selected').next().addClass('selected');
    }
    else {
        if ($('#length ul li').length != $('#length ul li.selected').index() + 1) {
            $('#displayDay ul li.selected').removeClass('selected');
            $('#displayDay ul li').first().addClass('selected');
            $('#length ul li.selected').removeClass('selected').next().addClass('selected');
        }
    }
    $('#displayDay li').eq(0).text($('#displayDay ul li.selected').text()).attr('value', $('#displayDay ul li.selected').attr('value'));
    $('#length li').eq(0).text($('#length ul li.selected').text()).attr('value', $('#length ul li.selected').attr('value'));
    // filterResults();
    var selectedView = $('#viewSelect .selected').attr('value');
    if (selectedView.toLowerCase() == 'classic') {
        FilterClassicViewResult();
    }
    else {
        $('.classic_view').hide();
        $('.daily_view, .classic_hide').show();
        FilterResults();
    }
}

function enableDisableControls() {
    setTimeout(function () {

        if ($('#displayDay ul li').length <= 1 && $('#length ul li').length <= 1) {
            enableNextPrevButton('.dailyViewPrevDay', false);
            enableNextPrevButton('.dailyViewNextDay', false);
        }
        else {
            if ($('#displayDay ul li').first().hasClass('selected') && $('#length ul li').first().hasClass('selected')) {
                enableNextPrevButton('.dailyViewPrevDay', false);
                if ($('#displayDay ul li').length > 1 || $('#length ul li').length > 1) {
                    enableNextPrevButton('.dailyViewNextDay', true);
                }
            }
            else if ($('#displayDay ul li').last().hasClass('selected') && $('#length ul li').last().hasClass('selected')) {
                enableNextPrevButton('.dailyViewNextDay', false);
                if ($('#displayDay ul li').length > 1 || $('#length ul li').length > 1) {
                    enableNextPrevButton('.dailyViewPrevDay', true);
                }
            }
            else {
                enableNextPrevButton('.dailyViewPrevDay', true);
                enableNextPrevButton('.dailyViewNextDay', true);
            }
        }
        //if  last day of shop is reached then show message Last Shop Day
        if ($('.dailyViewNextDayDisabled').is(':visible')) {
            $('.spanlastdayshop').show();
        }
        else {
            $('.spanlastdayshop').hide();
        }
        var viewSelected =   $('#viewSelect .selected').attr('value');
        if (viewSelected == 'classic') {
            if (($('#length ul li').length == $('#length ul li.selected').index() + 1) && ($('#carClassFilter ul li').length == $('#carClassFilter ul li.selected').index() + 1)) {
                $(".spanlastdayshop").show();
            }
            else {
                $(".spanlastdayshop").hide();
            }
        }

    }, 275);
}

function enableNextPrevButton(ctrl, isEnable) {
    if (isEnable) {
        disabledCtrl = ctrl + 'Disabled';
        $(ctrl + 'Disabled').hide();
        $(ctrl).show();
    }
    else {
        disabledCtrl = ctrl + 'Disabled';
        $(ctrl + 'Disabled').show();
        $(ctrl).hide();
    }
}

function markResults() {
    $('#reviewWaitSpinner').show();
    var searchSummaryId = $('#markResultsDiv #jobHistory li.rsselected').attr('value');
    var UserId = $('#LoggedInUserId').val();
    var ajaxURl = '/RateShopper/AutomationConsole/MarkResults';
    if (MarkResult != undefined && MarkResult != '') {
        ajaxURl = MarkResult.MarkResultsReviewed;
    }
    $.ajax({
        type: "GET",
        url: ajaxURl,
        dataType: "json",
        data: { searchSummaryId: searchSummaryId, userId: UserId },
        success: function (data) {
            $('#reviewWaitSpinner').hide();
            if (data) {
                if (data == true) {
                    $('#markResultsDiv #reviewButton').hide();
                    $('#markResultsDiv #spnReviewedStatus').show();
                    $('#markResultsDiv #jobHistory #li_' + searchSummaryId).removeClass('');
                    $('#markResultsDiv #jobHistory #li_' + searchSummaryId).addClass('pointer search-entry reviewedCheckMark');
                    $('#markResultsDiv #jobHistory #li_' + searchSummaryId).attr('reviewStatus', true);
                    if ($('#markResultsDiv #jobHistory li').length == $('#markResultsDiv #jobHistory li.reviewedCheckMark').length) {
                        ListAllScheduledJobs(false, jobId1);
                    }
                }
                else {
                    //something went wrong
                }
            }
        },
        error: function (e) {
            $('#reviewWaitSpinner').hide();
            console.log(e.message);
        }
    });
}

function FilterResults() {
    var searchSummaryId = $('#markResultsDiv #jobHistory li.rsselected').attr('value');
    var rentalLengthId = $('#markResultsDiv #length li.selected').attr('value');
    var brandId = $('#markResultsDiv #location li.selected').attr('lbid');
    var locationID = $('#markResultsDiv #location li.selected').attr('value');
    var selectedDate = $('#markResultsDiv #displayDay li.selected').attr('filter');
    var carClassId = $('#markResultsDiv #carClassFilter').is(':visible') ? $('#markResultsDiv #carClassFilter li.selected').attr('value') : 0;
    var rentalLength = $('#markResultsDiv #length li.selected').text();
    var ajaxURl = '/RateShopper/AutomationConsole/GetHistoryDetails';
    $('#reviewWaitSpinner').show();
    enableDisableControls();
    if (MarkResult != undefined && MarkResult != '') {
        ajaxURl = MarkResult.GetJobDetails;
    }
    $.ajax({
        type: "GET",
        url: ajaxURl,
        dataType: "json",
        data: { summaryId: searchSummaryId, locationId: locationID, brandId: brandId, rentalLengthId: rentalLengthId, selectedDate: selectedDate, carClassId: carClassId, isDailyView: true },
        success: function (suggestedRateMaster) {
            if (suggestedRateMaster != undefined && suggestedRateMaster != null && suggestedRateMaster.suggestedRate != null) {
                //$('.loader_container_main').hide();
                $('.dailyHeader').show();
                $("#NoRecordFoundMessage").hide();
                var finalData = JSON.parse(suggestedRateMaster.finalData);
                var headerData = $.map(finalData.HeaderInfo, function (item) { return new HeaderInfo(item) });
                automationViewModel.headers(headerData);

                var rateData = $.map(finalData.RatesInfo, function (item) { return new RatesInfo(item); });
                automationViewModel.rates(rateData);

                BindSuggestedRates(suggestedRateMaster.suggestedRate);
            }
            else {
                $('.classic_view').hide();
                $("#NoRecordFoundMessage").html("No records found.").show();
            }
            if (parseInt($("#CurrentJobSearchSummaryStatus").val()) == 4 && suggestedRateMaster != undefined && suggestedRateMaster != null && suggestedRateMaster.suggestedRate == null) {
                $("#NoRecordFoundMessage").html("No records found.").show();
            }
            if (rentalLength.indexOf('w') > -1) {
                $('.perDay').html('/W');
            }
            else if (rentalLength.indexOf('d') > -1) {
                $('.perDay').html('/D');
            }
            $('#reviewWaitSpinner').hide();
        },
        error: function (jqXHR, exception) {
            automationViewModel.rates(null);
            $('.classic_view').hide();
            $('.dailyHeader').hide();
            $("#NoRecordFoundMessage").html("No records found.").show();
            $('#reviewWaitSpinner').hide();
        }
    });
}

function FilterClassicViewResult() {
    var searchSummaryId = $('#markResultsDiv #jobHistory li.rsselected').attr('value');
    var rentalLengthId = $('#markResultsDiv #length li.selected').attr('value');
    var brandId = $('#markResultsDiv #location li.selected').attr('lbid');
    var locationID = $('#markResultsDiv #location li.selected').attr('value');
    var selectedDate = $('#markResultsDiv #displayDay li.selected').attr('filter');
    var carClassId = $('#markResultsDiv #carClassFilter').is(':visible') ? $('#markResultsDiv #carClassFilter li.selected').attr('value') : 0;
    var rentalLength = $('#markResultsDiv #length li.selected').text();
    var ajaxURl = '/RateShopper/AutomationConsole/GetHistoryDetails';
    $('#reviewWaitSpinner').show();
    if (MarkResult != undefined && MarkResult != '') {
        ajaxURl = MarkResult.GetJobDetails;
    }
    $.ajax({
        type: "GET",
        url: ajaxURl,
        dataType: "json",
        data: { summaryId: searchSummaryId, locationId: locationID, brandId: brandId, rentalLengthId: rentalLengthId, selectedDate: selectedDate, carClassId: carClassId, isDailyView: false },
        success: function (suggestedRateMaster) {
            HideShowNavigation(true);
            if (suggestedRateMaster != undefined && suggestedRateMaster != null && suggestedRateMaster.suggestedRate != null) {

                var finalData = JSON.parse(suggestedRateMaster.finalData);
                //var headerData = $.map(finalData.HeaderInfo, function (item) { return new HeaderInfo(item) });

                //automationViewModel.headers(headerData);
                var classicViewData = $.map(finalData.RatesInfo, function (item) { return new RateInfoClassic(item) });
                automationViewModel.ratesClassic(classicViewData);

                classicViewBrandCode = finalData.BrandCode;
                classicViewBrandID = finalData.BrandID;
                classicViewCarClassID = finalData.CarClassID;

                setTimeout(function () {
                    classicPostManipulation();
                    BindSuggestedRatesClassic(suggestedRateMaster.suggestedRate);
                }, 100)
            }
            else {
                $('.classic_view').hide();
                $("#NoRecordFoundMessage").html("No records found.").show();
            }
            if (parseInt($("#CurrentJobSearchSummaryStatus").val()) == 4 && suggestedRateMaster != undefined && suggestedRateMaster != null && suggestedRateMaster.suggestedRate == null) {
                $("#NoRecordFoundMessage").html("No records found.").show();
            }
            if (rentalLength.indexOf('w') > -1) {
                $('.perDay').html('/W');
            }
            else if (rentalLength.indexOf('d') > -1) {
                $('.perDay').html('/D');
            }
            $('#reviewWaitSpinner').hide();
        },
        error: function (jqXHR, exception) {
            automationViewModel.ratesClassic(null);
            $('#daily-rates-table').hide();
            $("#NoRecordFoundMessage").html("No records found.").show();
            $('.classictable .company-logo').remove();
            $('.NoResultsFound').remove();
            $('#reviewWaitSpinner').hide();
            // $('.classic_view').hide();
        }
    });
}
function BindSuggestedRates(suggestedRates) {
    if (suggestedRates != null) {
        $('.additionalBaseCol').hide();
        lengthFactor = parseInt($('#length>ul>li.selected').text().substring(1));
        if (lengthFactor != null && lengthFactor != '' && lengthFactor >= 8 && lengthFactor <= 11) {
            showAdditionalBase = true;
            $('.additionalBase').val('');
            $('.additionalBaseCol').show();
        }
        $('#searchResultRates tr [classid]').each(function () {
            var self = $(this);
            var parentTr = self.parents('tr');
            suggestedRates.forEach(function (element, i) {
                if (element.CarClassID == self.attr("classid")) {

                    var $baseEdit = parentTr.find(".baseEdit");

                    var $totalEdit = parentTr.find(".totalEdit");

                    if (element.RuleSetID != 0 && $.isNumeric(element.TotalRate) && $.isNumeric(element.BaseRate) && element.TotalRate > 0 && element.BaseRate > 0) {

                        var base = parseFloat(element.BaseRate);
                        var total = parseFloat(element.TotalRate);

                        $baseEdit.html(commaSeparateNumber(base.toFixed(2)));
                        $totalEdit.html(commaSeparateNumber(total.toFixed(2)));

                        //for W8 to W11 calculate and bind value for 'Additional Base' rate                    
                        isGOV = $('#jobHistory li.rsselected').attr('isgov').toLowerCase() == "true" ? true : false;
                        if (showAdditionalBase) {
                            var additionalBase = calculateAdditionalBase(base);
                            if (isGOV) {
                                additionalBase = parseInt(additionalBase);
                            }
                            var ab = commaSeparateNumber(additionalBase.toFixed(2));
                            parentTr.find(".additionalBase").html(ab);
                        }
                    }
                    else {
                        parentTr.find(".baseEdit").attr('suggetedOriginalValue', '').html('');
                        parentTr.find(".baseEdit").attr('suggetedOriginalValue', '').html('');
                    }
                }
            });
        });
    }

    $('.perDay').each(function () {
        if ($(this).text().trim().length) {
            $(this).siblings('.daily-rate').prepend('$');
            $(this).siblings('.tv').prepend('$');
        }
    });

    $('.suggested').each(function () {
        if ($(this).text().trim().length) {
            $(this).prepend('$');
        }
    });

    //If myBrand is providing lowest rate than lowestRateAmoungCompetitor then highlight my brand as Green.
    //The calculation should be done on copetitor only. Not using myBrand rates.
    $('#searchResultRates tbody tr').each(function () {
        var $thisTr = $(this);
        if ($thisTr.find(".highlightGreen").html() != undefined) {
            var competitorGreenTotalRate;
            var myCompanyTotalRate;

            var $highlightGreen = $thisTr.find(".highlightGreen");
            var $myComp;
            //if not GOV shop then compare total value for highlighting
            //if GOV shop then compare base value for highlighting
            isGOV = $('#jobHistory li.rsselected').attr('isgov').toLowerCase() == "true" ? true : false;
            if (!isGOV) {
                $myComp = $thisTr.find("td[companyID=" + myCompanyId + "] .tv").not('.highlight');
            }
            else {
                $myComp = $thisTr.find("td[companyID=" + myCompanyId + "] .dummybase").not('.highlight');
            }
            if ($highlightGreen.length > 0) {
                competitorGreenTotalRate = commaRemovedNumber($highlightGreen.eq(0).html().replace('$', ''));
            }
            if ($myComp.length > 0) {
                myCompanyTotalRate = commaRemovedNumber($myComp.html().replace('$', ''));
            }
            if ($.isNumeric(myCompanyTotalRate) && $.isNumeric(competitorGreenTotalRate) > 0 && parseFloat(myCompanyTotalRate) <= parseFloat(competitorGreenTotalRate)) {
                $myComp.addClass('highlightGreen1');
                if (parseFloat(myCompanyTotalRate) == parseFloat(competitorGreenTotalRate)) {
                    $highlightGreen.addClass('highlightGreen1');
                }
            }
            else {
                $highlightGreen.addClass('highlightGreen1');
            }
        }
    });
}

function calculateAdditionalBase(baseValue) {
    var additionalBase = (parseFloat(baseValue) / 7);
    if (isGOV) {
        return parseInt(additionalBase);
    }
    return additionalBase;
}

function classicPostManipulation() {
    addEmptyTd();
    //Create column headers. (1,2,3)    
    createHeaders();
    hightlightBrand();
}

//add empty td to table which does not part of json. This is to add '--' in classic table
function addEmptyTd() {
    //get max col in table body
    var maxCol = 0;
    $('.classictable table tbody tr').each(function () {
        maxCol = Math.max($(this).children().length, maxCol);
    });

    //add td to with '--' to remaining column in each row
    var x = "<td class='relative'>--</td>";
    $('.classictable table tbody tr').each(function () {
        var totalColPresent = $(this).children().length;
        if (totalColPresent < maxCol) {
            for (var col = 0; col < (maxCol - totalColPresent) ; col++) {
                $(this).append(x);
            }
        }
    });
}

function createHeaders() {
    $('.company-logo').remove();
    //add brand Code
    $('.classicheader').append($("<th class='company-logo classicBrand' align='center'>" + classicViewBrandCode + "</th>"));
    var x = "<th class='company-logo cheader' align='center'>[[ID]]</th>";
    var totalTdToAdd = $(".classictable table tbody tr:first td").length - 5;
    for (var c = 1; c <= totalTdToAdd ; c++) {
        $('.classicheader').append(x.replace('[[ID]]', c));
    }
}

function hightlightBrand() {
    $('.classictable tbody tr').each(function (i, o) {
        var self = o;
        var selfindex = $(self).children().index($(self).children('td[companyID = ' + classicViewBrandID + ']:last'));
        var compindex = $(self).children().index($(self).find('.base-rate-blue').closest('td'));
        if (selfindex > 4) {
            if (compindex != -1 && (selfindex > compindex)) {
                $(self).children('td').eq(selfindex).html(classicViewBrandCode).addClass('classic-not-lowest');
            }
            else {
                $(self).children('td').eq(selfindex).html(classicViewBrandCode).addClass('classic-lowest');
            }
        }
    });
}

function BindSuggestedRatesClassic(suggestedRate) {
    //remove time part of the date
    suggestedRate.forEach(function (ele, i) {
        if (ele.Date != null) {
            ele.Date = convertToServerTime(new Date(parseInt(ele.Date.replace("/Date(", "").replace(")/", ""), 10)));
            ele.Date.setHours(0);
            ele.Date.setMinutes(0);
        }
    });
    $('.additionalBaseCol').hide();
    //for W8 to W11 calculate and bind value for 'Additional Base' rate    
    lengthFactor = parseInt($('#length>ul>li.selected').text().substring(1));
    if (lengthFactor != null && lengthFactor != '' && lengthFactor >= 8 && lengthFactor <= 11) {
        showAdditionalBase = true;
        $('.additionalBase').val('');
        $('.additionalBaseCol').show();
    }
    $('.arrow').removeAttr('class').addClass('arrow');
    $('.arrowb').removeAttr('class').addClass('arrowb');

    $('.classictable tr[date]').each(function () {
        var self = $(this);
        var parentTr = self;
        suggestedRate.forEach(function (element, i) {
            if (element.Date == self.attr("date")) {

                var $baseEdit = parentTr.find(".baseEdit");

                parentTr.find(".suggestedRateId").attr('suggesteRtId', element.ID);

                if (element.RuleSetID != 0 && $.isNumeric(element.TotalRate) && $.isNumeric(element.BaseRate) && element.TotalRate > 0 && element.BaseRate > 0) {

                    var total = parseFloat(element.TotalRate);
                    var base = parseFloat(element.BaseRate);

                    var $totalEdit = parentTr.find(".totalEdit");

                    $baseEdit.html(commaSeparateNumber(base.toFixed(2)));
                    $totalEdit.html(commaSeparateNumber(total.toFixed(2)));

                    //for W8 to W11 calculate and bind value for 'Additional Base' rate                    
                    if (showAdditionalBase) {
                        isGOV = $('#jobHistory li.rsselected').attr('isgov').toLowerCase() == "true" ? true : false;
                        var additionalBase = calculateAdditionalBase(base);
                        if (isGOV) {
                            additionalBase = parseInt(additionalBase);
                        }
                        var ab = commaSeparateNumber(additionalBase.toFixed(2));
                        parentTr.find(".additionalBase").html(ab);
                    }
                }
                else {
                    parentTr.find(".totalEdit").attr('suggetedOriginalValue', '').html('');
                    parentTr.find(".baseEdit").attr('suggetedOriginalValue', '').html('');
                }
            }
        });
    });
}

function HideShowNavigation(hideNav) {
    if (hideNav) {
        $('.dailyViewPrevDay').hide();
        $('.dailyViewNextDay').hide();
    }
    else {
        $('.dailyViewPrevDay').show();
        $('.dailyViewNextDay').show();
    }
}
//value="http://107.190.142.58/cgi-bin/dot_net_uat_call_to_scraper.cgi?"