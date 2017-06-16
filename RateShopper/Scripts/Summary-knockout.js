
var summaryViewModel;
var smartSearchLocations;
var loggedInUserID;
var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
var FirstSearchSummaryDataCollection;
var SearchSummaryRecursiveCall = true;
var isNewArrayFetched = false;
var selectedFirstSummaryID = 0;
var searchSummaryData, SearchSummaryId;
var lastSelectedSearchSummaryStatus = 0;// for check last selected search status fail/inprogress will comes in reset schenario using searchSummart selection
var GlobalLimitSearchSummaryData;

$(document).ready(SummaryKnockoutDocumentReady);

function SummaryKnockoutDocumentReady() {
    loggedInUserID = $('#LoggedInUserId').val();
    $("#recentSearchTypeLabel,#recentSearchType").hide();//Hide recent search summary type
    $("#recentSummaryTypeLabel,#recentSummaryType").show();
    summaryViewModel = new SourceModel();
    ko.applyBindings(summaryViewModel);

    ShowSearchSummaryButton();
    BindControlEvent();
    BindLocations();
    BindCarClasses();
    SetUserFilter();
    FTBRecentSearchAjaxCall();
    BindStatus();
    recentSearchSelection();
    
}

//View Model
function SourceModel() {
    var self = this;
    self.locations = ko.observableArray([]);
    self.carClasses = ko.observableArray([]);
    self.Status = ko.observableArray([]);
    self.headers = ko.observableArray([]);
    self.rates = ko.observableArray([]);
    self.SearchSummary = ko.observableArray([]);
    self.SearchSummary.subscribe(function (changes) {
    })

    self.SearchSummary.removeSearch = function () {
        self.SearchSummary.remove(function (search) {
            return $.trim(search.StatusIDs).toString() == '6';
        })
    }
}
//End View Model

//Entity
var locations = function (data) {
    this.LocationID = data.LocationID;
    this.Location = data.LocationBrandAlias;
    this.LocationBrandID = data.ID;
    this.LocationCode = data.LocationCode;
    this.LOR = data.LOR;
}
var carClasses = function (data) {
    this.ID = data.ID;
    this.Code = data.Code;
}
function Status(data) {
    this.ID = data.ID;
    this.status = data.Status;
};
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
        if (GlobalLimitSearchSummaryData != null && GlobalLimitSearchSummaryData.IsGOV) {
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

    // this.moveUpDownCSS = data.IsMovedUp != null ? data.IsMovedUp ? 'rateMovedUp' : 'rateMovedDown' : '';
    if (GlobalLimitSearchSummaryData != null && GlobalLimitSearchSummaryData.IsQuickView) {
        this.moveUpDownCSS = data.IsMovedUp != null ? data.IsMovedUp ? 'rateMovedUp' : 'rateMovedDown' : '';
    }
    else {
        this.moveUpDownCSS = '';
    }
}
//End Entity


//#REGION Ajax methods
var StartSummaySearch = function (searchModel) {
    var ajaxURl = '/RateShopper/Summay/InitiateSummarySearch/';
    if (SummayShopAjaxURLSettings != undefined && SummayShopAjaxURLSettings != '') {
        ajaxURl = SummayShopAjaxURLSettings.InitiateSummarySearch;
    }

    $('#searchInitiated').fadeIn(1000).delay(2000).fadeOut(1000);
    $.ajax({
        url: ajaxURl,
        type: 'POST',
        data: searchModel,
        success: function (data) {
            $('#pastSearches').scrollTop(0);
            $('#searchInitiated').hide();
            ResetSelection();
            SearchSummaryRecursiveCall = false;
            FTBRecentSearchAjaxCall();
        },
        error: function (e) {
            //called when there is an error
            console.log(e.message);
            $('#searchInitiated').hide();
        }
    });

}

var BindLocations = function () {
    var ajaxURl = '/RateShopper/Summay/GetLocations/';
    if (SummayShopAjaxURLSettings != undefined && SummayShopAjaxURLSettings != '') {
        ajaxURl = SummayShopAjaxURLSettings.GetLocations;
    }

    $('.loader_container_location').show();
    $.ajax({
        url: ajaxURl,
        data: { UserId: loggedInUserID },
        type: 'GET',
        success: function (data) {
            $('.loader_container_location').hide();
            if (data) {

                var srcs = $.map(data, function (item) { return new locations(item); });
                smartSearchLocations = data;
                summaryViewModel.locations(srcs);
                $("#recentLocations ul li").eq(0).addClass("selected");
            }
        },
        error: function (e) {
            $('.loader_container_location').hide();
            console.log(e.message);
        }
    });
}

var GetLocationSpecificCarClasses = function (locationBrandId) {
    var searchId = locationBrandId.value, $selectedOpt = $('option:selected', locationBrandId);
    searchId = $selectedOpt.attr('brandId');
    if (searchId != null && searchId != '') {
        var ajaxURl = '/RateShopper/Summay/GetLocationCarClasses/';
        if (SummayShopAjaxURLSettings != undefined && SummayShopAjaxURLSettings != '') {
            ajaxURl = SummayShopAjaxURLSettings.GetLocationCarClasses;
        }

        $('.loader_container_carclass').show();
        $.ajax({
            url: ajaxURl,
            data: { locationBrandId: searchId },
            type: 'GET',
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
                }
            },
            error: function (e) {
                console.log("GetLocationSpecificCarClasses: " + e.message);
            }
        });
    }
}

var BindCarClasses = function () {
    var ajaxURl = '/RateShopper/Summay/GetCarClasses/';
    if (SummayShopAjaxURLSettings != undefined && SummayShopAjaxURLSettings != '') {
        ajaxURl = SummayShopAjaxURLSettings.GetCarClasses;
    }

    $('.loader_container_carclass').show();
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        success: function (data) {
            $('.loader_container_carclass').hide();
            if (data) {
                var srcs = $.map(data, function (item) { return new carClasses(item); });
                summaryViewModel.carClasses(srcs);
                $('#carClasstd select option').bind('click', function () {
                    if ($('#carClasstd option:selected').length == $('#carClasstd select option').length) {
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

function BindStatus() {
    $.ajax({
        url: 'Search/GetStatus/',
        type: 'GET',
        data: {},
        dataType: 'json',
        async: true,
        success: function (data) {
            var srcs = $.map(data, function (item) { return new Status(item); });
            summaryViewModel.Status(srcs);
            $("#recentStatuses ul li").eq(0).addClass("selected");
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(errorThrown);
        }
    });
}

function FTBRecentSearchAjaxCall(isUserFilterChange) {
    var ajaxURl = '/RateShopper/Summay/FTBGetSearchSummary/';
    if (SummayShopAjaxURLSettings != undefined && SummayShopAjaxURLSettings != '') {
        ajaxURl = SummayShopAjaxURLSettings.FTBSummaryList;
    }
    FirstSearchSummaryDataCollection = "";
    var currentDateTime = new Date();
    lastModifiedDate = $("#lastModifiedDate").attr("value");
    if (summaryViewModel.SearchSummary().length == 0) {
        lastModifiedDate = "";
    }
    var IsAdmin = $("#hdnIsAdminUser").val();
    var selectedUserInRecentSearch = $("#recentUsers ul li.selected").attr("value");
    if (typeof (selectedUserInRecentSearch) == 'undefined') {
        if (IsAdmin.toUpperCase() == 'FALSE') {
            selectedUserInRecentSearch = $("#recentUsers ul li[value='" + loggedInUserID + "']").eq(0).attr("value");
        }
        else {
            selectedUserInRecentSearch = $("#recentUsers ul li").eq(0).attr("value");
        }

    }
    $.ajax({
        url: ajaxURl,
        type: 'POST',
        data: { objLastModifieddate: lastModifiedDate, strClientTimezoneOffset: currentDateTime.getTimezoneOffset(), LoggedInUserID: loggedInUserID, userSystemDate: currentDateTime.toDateString(), isAdminUser: IsAdmin, selectedUserId: selectedUserInRecentSearch },
        dataType: 'json',
        async: true,
        success: function (data) {
            FirstSearchSummaryDataCollection = data;
            if (isUserFilterChange) {
                summaryViewModel.SearchSummary('');
                isNewArrayFetched = false;
            }
            if (data.lstSearchSummaryData.length > 0) {

                if (summaryViewModel.SearchSummary().length > 0) {
                    isNewArrayFetched = true;
                    var newSummaries = data.lstSearchSummaryData;//$.map(data.lstSearchSummaryData, function (item) { return new SearchSummary(item); });
                    var newlyAddedList = [];
                    ko.utils.arrayForEach(summaryViewModel.SearchSummary(), function (objSearchSummary) {
                        ko.utils.arrayForEach((newSummaries), function (latestSearchSummary) {
                            if (objSearchSummary != null && latestSearchSummary != null) {

                                if (objSearchSummary.SearchSummaryID == latestSearchSummary.SearchSummaryID) {
                                    if ($.trim(latestSearchSummary.StatusIDs).toString() == '6') {

                                        latestSearchSummary.isFound = true;
                                        objSearchSummary.StatusIDs = latestSearchSummary.StatusIDs;
                                    }
                                    else {
                                        objSearchSummary.StatusIDs = latestSearchSummary.StatusIDs;
                                        objSearchSummary.StatusName = latestSearchSummary.StatusName;
                                        objSearchSummary.FailureResponse = latestSearchSummary.FailureResponse;
                                        objSearchSummary.SearchTypeClass = latestSearchSummary.SearchTypeClass;
                                        objSearchSummary.StatusClass = latestSearchSummary.StatusClass;

                                        $('#pastSearches  li[value=' + objSearchSummary.SearchSummaryID + ']').attr({
                                            'StatusId': latestSearchSummary.StatusIDs,
                                            'class': latestSearchSummary.SearchTypeClass
                                        });
                                        $('#pastSearches  li[value=' + objSearchSummary.SearchSummaryID + '] span').eq(1).attr('class', latestSearchSummary.StatusClass).html(latestSearchSummary.StatusName);

                                        latestSearchSummary.isFound = true;
                                    }
                                    return false;
                                }

                            }
                        })
                    });

                    //remove searches that has deleted status
                    summaryViewModel.SearchSummary.removeSearch();
                    if (newSummaries.length > 0) {
                        $.each(newSummaries, function (index, summary) {
                            if ($.trim(summary.StatusIDs).toString() != '6' && ((typeof (summary.isFound) == 'undefined') || !summary.isFound)) {
                                newlyAddedList.push(summary);
                            }
                        });
                    }
                    if (newlyAddedList != null && newlyAddedList.length > 0) {
                        $.each(newlyAddedList, function (index, summary) {
                            summaryViewModel.SearchSummary.unshift(summary);
                        });
                    }

                    $("#pastSearches li.hidden").removeClass("hidden");
                }
            }
            BindFTBSearchSummaryData();
            
            if (isUserFilterChange && $('#pastSearches li[statusid="4"]:visible').length > 0) {

                $('#viewResult').removeClass("disable-button").unbind('click').bind('click', function () {
                    showResult();
                });
                $('#pastSearches li[statusid="4"]:visible').eq(0).trigger('click');
                $('#viewResult').trigger("click");
                $('.hideNoSummary').show();
                $('#noRecords').hide();
            }
            else if (isUserFilterChange) {
                $('#viewResult').removeClass("disable-button").addClass("disable-button").unbind('click');
                $('.dailyFilter,.daily_view').hide();
                $('#noRecords').show();
            }
            else {
                if ($('#noRecords:visible').length == 0) {
                    HideShowNavigation();
                }
            }
            ShowHideRecentShopsLoader(false);
            if (!SearchSummaryRecursiveCall) {
                SearchSummaryRecursiveCall = true;
            }
            else {
                setTimeout(function () { FTBRecentSearchAjaxCall(); }, 30000);
            }
            
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (!SearchSummaryRecursiveCall) {
                SearchSummaryRecursiveCall = true;
            }
            else {
                setTimeout(function () { FTBRecentSearchAjaxCall(); }, 30000);
            }
        }
    });
}

function DeleteSummaryShopCallBack(selectedSearchSummaryID) {
    if (typeof (selectedSearchSummaryID) == 'undefined') {
        selectedSearchSummaryID = this;
    }
    var userID = $('#LoggedInUserId').val();
    $("#selectedSummaryID_" + selectedSearchSummaryID).hide();
    var ajaxURl = '/RateShopper/Summay/DeleteSummaryShop/';
    if (SummayShopAjaxURLSettings != undefined && SummayShopAjaxURLSettings != '') {
        ajaxURl = SummayShopAjaxURLSettings.DeleteSummaryShop;
    }

    $.ajax({
        url: ajaxURl,
        type: 'GET',
        data: { searchSummaryId: selectedSearchSummaryID, userId: userID },
        dataType: 'json',
        async: true,
        success: function (data) {
            if (typeof (selectedSearchSummaryID) != 'undefined' && typeof (SearchSummaryId) != 'undefined') {
                if (selectedSearchSummaryID == SearchSummaryId) {
                    $('.dailyFilter,.daily_view').hide();
                    $('#noRecords').show();
                }
            }
            $("#selectedSummaryID_" + selectedSearchSummaryID).remove();
            $("#search-summary").find(".searchSummary").html("");
            $('#viewResult').unbind('click');
            $('#viewResult').addClass("disable-button");
            SearchSummaryRecursiveCall = false;
            FTBRecentSearchAjaxCall();
            selectedSearchSummaryID = 0;
            ResetSelection();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(errorThrown);
        }
    });
}
//end Ajax functions




//Other functions
function selectedSearchSummary(data) {
    $('#left-col').find('*').removeClass('flashborder').removeClass('temp');
    $('#error-span').hide();
    RemoveFlashableTag('.has-error');

    searchSummaryData = data;
    //Assign for first time completed search Click
    if (selectedFirstSummaryID == 0) {
        GlobalLimitSearchSummaryData = searchSummaryData;//used for Global limit implementation
    }
    selectedSearchSummaryID = data.SearchSummaryID;
    var $previous = $("#pastSearches").find(".rsselected");
    var $previousVal = $($previous).val();
    //var $previousIsQuickView = $($previous).attr("isquickview");

    var $previousHasQuickView = $("#pastSearches").find(".rsselected");
    //var $previousValHasQuickView = $($previousHasQuickView).val();
    var $previousSelectedHasQuickView = $($previousHasQuickView).attr("HasQuickViewChild");
    if ($previousSelectedHasQuickView == "true") {
        if ($($previousHasQuickView).attr("isquickview") == "true") {
            $previousSelectedHasQuickView = "false";
        }
    }


    $("#search-summary").removeAttr("style");
    $("#selectedSummaryID_" + data.SearchSummaryID).addClass("rsselected");
    $("#selectedSummaryID_" + data.SearchSummaryID).siblings("li").removeClass("rsselected");

    var startDate = convertToServerTime(new Date(parseInt(data.StartDate.replace("/Date(", "").replace(")/", ""), 10)));
    var endDate = convertToServerTime(new Date(parseInt(data.EndDate.replace("/Date(", "").replace(")/", ""), 10)));
    var createdDate = (new Date(parseInt(data.CreatedDate.replace("/Date(", "").replace(")/", ""), 10)));

    startDate.setHours(data.StartTime.split(':')[0]);
    startDate.setMinutes(data.StartTime.split(':')[1]);

    endDate.setHours(data.EndTime.split(':')[0]);
    endDate.setMinutes(data.EndTime.split(':')[1]);

    $("#summaryTimeOf").html(monthNames[createdDate.getMonth()] + " " + createdDate.getDate() + " - " + (createdDate.getHours() < 10 ? '0' : '') + createdDate.getHours() + ":" + (createdDate.getMinutes() < 10 ? '0' : '') + createdDate.getMinutes());
    $("#summaryStartDate").html(monthNames[startDate.getMonth()] + " " + startDate.getDate() + " - " + (startDate.getHours() < 10 ? '0' : '') + startDate.getHours() + ":" + (startDate.getMinutes() < 10 ? '0' : '') + startDate.getMinutes());
    $("#summaryEndDate").html(monthNames[endDate.getMonth()] + " " + endDate.getDate() + " - " + (endDate.getHours() < 10 ? '0' : '') + endDate.getHours() + ":" + (endDate.getMinutes() < 10 ? '0' : '') + endDate.getMinutes());
    $("#summaryUser").html(data.FullName);
    $("#summaryLocation").html(data.LocationName);
    $("#summaryCarClass").html(data.CarClassName);
    $("#summaryLength").html(data.RentalLengthName);
    $("#summarySource").html(data.SourceName);


    $('#viewResult').unbind('click');
    if (data.StatusIDs == 4) {
        if (lastSelectedSearchSummaryStatus == 1 || lastSelectedSearchSummaryStatus == 5)
            ResetSelection();
        $('#viewResult').removeClass("disable-button");
        $('#viewResult').unbind('click').bind('click', function () {
            showResult();
        });
        $("#summaryFailure").html(data.FailureResponse);
        $("#search-summary p").eq(8).addClass("hidden");
        //For any completed selected item incase while ajax calling during check the status of searchsummary
        selectedFirstSummaryID = selectedSearchSummaryID;
        lastSelectedSearchSummaryStatus = 4;
    }
    else {
        $('#viewResult').addClass("disable-button");
        if (data.StatusIDs == 5) {   //show failure reason
            $("#summaryFailure").html(data.FailureResponse);
            $("#search-summary p").eq(8).removeClass("hidden");
            $("#search-summary p").eq(8).css('display', '');
            lastSelectedSearchSummaryStatus = 5;
        }
        else {
            $("#summaryFailure").html(data.FailureResponse);
            $("#search-summary p").eq(8).addClass("hidden");
            lastSelectedSearchSummaryStatus = 1;
        }
        $("#startDate.date-picker").val((startDate.getMonth() + 1) + "/" + startDate.getDate() + "/" + startDate.getFullYear());
        //$("#endDate.date-picker").val((endDate.getMonth() + 1) + "/" + endDate.getDate() + "/" + endDate.getFullYear());

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
        //$('').datepicker('option', { maxDate: maxDate });
        $("#endDate.date-picker").val((endDate.getMonth() + 1) + "/" + endDate.getDate() + "/" + endDate.getFullYear());
        //end MAXDATE SET 

        $("#pickupHour ul li").each(function () {
            if ($(this).text() == formatAMPM(startDate.getHours(), startDate.getMinutes())) {
                $(this).addClass("selected");
                $('#pickupHour li').eq(0).text($(this).text()).attr('value', ($(this).text()));
            }
            else {
                $(this).removeClass("selected");
            }
        });

        $("#dropOffHour ul li").each(function () {
            if ($(this).text() == formatAMPM(endDate.getHours(), endDate.getMinutes())) {
                $(this).addClass("selected");
                $('#dropOffHour li').eq(0).text($(this).text()).attr('value', ($(this).text()));
            }
            else {
                $(this).removeClass("selected");
            }
        });
        //populate Selected summary based on autoselect source dropdown item
        $("#source select option").each(function () {
            if ($.inArray($(this).val(), (data.SourcesIDs).split(',')) == -1)
                $(this).prop("selected", false);
            else
                $(this).prop("selected", true);
        });
        var prvcode = $("select#source option:selected").attr("prvcode");
        if (typeof (prvcode) != 'undefined') {
            //EnableDisableMultipleLOR(prvcode, $("select#source option:selected").attr("isgov"));
        }

        //populate Selected summary based on autoselect RentelLength dropdown item
        $("#lengths select option").each(function () {
            if ($.inArray($(this).val(), (data.RentalLengthsIDs).split(',')) == -1) {
                $(this).prop("selected", false);
            }
            else {
                $(this).prop("selected", true);
            }
        });
        checkUncheckParent('');
        $('#rentalLengthtd #lengths').scrollTop(0);

        //populate Selected summary based on autoselect carclass dropdown item
        $("#carClass select option").each(function () {
            if ($.inArray($(this).val(), (data.CarClassIDs).split(',')) == -1) {
                $(this).prop("selected", false);
            }
            else {
                $(this).prop("selected", true);
            }
        });
        //populate Selected summary based on autoselect location dropdown item
        $("#locations select option").each(function () {
            if ($.inArray($(this).attr("brandid"), (data.LocationsBrandIDs).split(',')) == -1) {
                $(this).prop("selected", false);
            }
            else {
                $(this).prop("selected", true);
            }
        });
    }
}

function recentSearchSelection() {
    var recentUserID = $("#recentUsers li").attr("value"),
        recentStatusID = 0,
        recentLengthID = 0,
        recentLocationID = $("#recentLocations li").attr("value"),
        recentSourceID = $("#recentSources li").attr("value"),
        recentSearchType = 0,
        recentSummaryType = 0;;

    $("#recentUsers ul").on("click", "li", function () {
        recentUserID = $(this).val();
        recentLocationID = $("#recentLocations li").attr("value");
        recentSourceID = $("#recentSources li").attr("value");
        recentSummaryType = $("#recentSummaryType li").attr("value");
        populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType, recentSummaryType);
        var isAdmin = $("#hdnIsAdminUser").val();
        if (isAdmin.toUpperCase() == "FALSE") {
            SearchSummaryRecursiveCall = false;            
            $("#lastModifiedDate").attr("value", "");
            ShowHideRecentShopsLoader(true);
            FTBRecentSearchAjaxCall(true);            
        }
    });
    $("#recentStatuses").on("click", "li", function () {
        recentStatusID = $(this).val();
        recentUserID = $("#recentUsers li").attr("value");
        recentLocationID = $("#recentLocations li").attr("value");
        recentSourceID = $("#recentSources li").attr("value");
        recentSummaryType = $("#recentSummaryType li").attr("value");
        populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType, recentSummaryType);
        ShowFirstRecordIfNoPrevRecordExists();
    });
    $("#recentLengths").on("click", "li", function () {
        recentLengthID = $(this).val();
        recentUserID = $("#recentUsers li").attr("value");
        recentLocationID = $("#recentLocations li").attr("value");
        recentSourceID = $("#recentSources li").attr("value");
        recentSummaryType = $("#recentSummaryType li").attr("value");
        populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType, recentSummaryType);
        ShowFirstRecordIfNoPrevRecordExists();
    });
    $("#recentLocations").on("click", "li", function () {
        recentLocationID = $(this).val();
        recentUserID = $("#recentUsers li").attr("value");
        recentSourceID = $("#recentSources li").attr("value");
        recentSummaryType = $("#recentSummaryType li").attr("value");
        populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType, recentSummaryType);
        ShowFirstRecordIfNoPrevRecordExists();
    });
    $("#recentSources").on("click", "li", function () {
        recentSourceID = $(this).val();
        recentUserID = $("#recentUsers li").attr("value");
        recentLocationID = $("#recentLocations li").attr("value");
        recentSummaryType = $("#recentSummaryType li").attr("value");
        populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType, recentSummaryType);
        ShowFirstRecordIfNoPrevRecordExists();
    });
    $("#recentSearchType").on("click", "li", function () {
        recentSearchType = $(this).val();
        populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType, recentSummaryType);
        ShowFirstRecordIfNoPrevRecordExists();
    });
    $("#recentSummaryType").on("click", "li", function () {
        recentSummaryType = $(this).val();
        recentSourceID = $("#recentSources li").attr("value");
        recentUserID = $("#recentUsers li").attr("value");
        recentLocationID = $("#recentLocations li").attr("value");
        populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType, recentSummaryType);
        ShowFirstRecordIfNoPrevRecordExists();
    });
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


function BindFTBSearchSummaryData() {
    if (FirstSearchSummaryDataCollection != null && FirstSearchSummaryDataCollection.lastModifiedDate != "undefined" && FirstSearchSummaryDataCollection.lastModifiedDate != null) {
        $("#lastModifiedDate").val(FirstSearchSummaryDataCollection.lastModifiedDate);
    }
    if (FirstSearchSummaryDataCollection.lstSearchSummaryData.length != 0) {
        if (!isNewArrayFetched) {
            //var srcs = $.map(FirstSearchSummaryDataCollection.lstSearchSummaryData, function (item) { return new SearchSummary(item); });
            summaryViewModel.SearchSummary(FirstSearchSummaryDataCollection.lstSearchSummaryData);
            $("#pastSearches li.hidden").removeClass("hidden");
        }
        recentSearchOnLoadCheck();
        // $("#bindSearchSummary #pastSearches li").each(function () {
        if (selectedFirstSummaryID == 0 && $("#bindSearchSummary #pastSearches li[statusid=4]").length != 0) {
            //$(this).click();
            if (!loadingPrevResult) {
                $("#bindSearchSummary #pastSearches li[statusid=4]").eq(0).click();
                //FetchLastUpdateTSD();
            }
            return false;
        }
        else {
            if (typeof (selectedSearchSummaryID) != 'undefined' && selectedSearchSummaryID != 0) {
                $("#selectedSummaryID_" + selectedSearchSummaryID).addClass("rsselected").siblings("li").removeClass("rsselected");
                //$("#selectedSummaryID_" + selectedSearchSummaryID).siblings("li").removeClass("rsselected");

                //If user select InProgress item and after it automatically complete then viewresult button enabled
                //$("#bindSearchSummary #pastSearches li[value=" + selectedSearchSummaryID + "]") == selectedSearchSummaryID && 
                if (($("#bindSearchSummary #pastSearches li[value=" + selectedSearchSummaryID + "]").attr("statusid")) == "4") {
                    $('#viewResult').removeClass("disable-button").unbind('click').bind('click', function () {
                        showResult();
                    });
                }
                //return false;
            }
        }
        // });
    }
    //Change request by ahmed current will make search and that search in in-progress mode then scroll automatically go top 
    if ($("#pastSearches li").length > 0) {
        var SearchSummaryFirstElement = $("#pastSearches li").eq(0);
        if ($(SearchSummaryFirstElement).attr("statusid") == "1" && $(SearchSummaryFirstElement).attr("userid") == loggedInUserID) {
            $("#pastSearches").scrollTop(0);
        }
    }
}

function recentSearchOnLoadCheck() {
    var recentUserID = 0, recentStatusID = 0, recentLengthID = 0, recentLocationID = 0, recentSourceID = 0, recentSearchType = 0, recentSummaryType = 0;
    var DropDownSummaryShopFlag = false;//Check if any filter is selected than populateSearchSummaryDropDown function is call

    if (($("#recentUsers li").attr("value")) != "0") {
        DropDownSummaryShopFlag = true;
        recentUserID = $("#recentUsers li").attr("value");
    }

    if (($("#recentStatuses li").attr("value")) != "0") {
        DropDownSummaryShopFlag = true;
        recentStatusID = $("#recentStatuses li").attr("value");
    }

    if (($("#recentLengths li").attr("value")) != "0") {
        DropDownSummaryShopFlag = true;
        recentLengthID = $("#recentLengths li").attr("value");
    }

    if (($("#recentSources li").attr("value")) != "0") {
        DropDownSummaryShopFlag = true;
        recentSourceID = $("#recentSources li").attr("value");
    }

    if (($("#recentLocations li").attr("value")) != "0") {
        DropDownSummaryShopFlag = true;
        recentLocationID = $("#recentLocations li").attr("value");
    }

    if (($("#recentSummaryType li").attr("value")) != "0") {
        DropDownSummaryShopFlag = true;
        recentSummaryType = $("#recentSummaryType li").attr("value");
    }

    if (DropDownSummaryShopFlag) {
        populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType, recentSummaryType);
    }
}

function populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType, recentSummaryType) {
    var conditionStr = "";//"[userid='26'][locationbrandid='10'][rentallengthsid*='7']";
    $("#bindSearchSummary ul li").show();
    var IsVisible = true;
    var DropDownSummaryShopFlag = false;
    if (recentSourceID != 0) {
        DropDownSummaryShopFlag = true;
        conditionStr += "[sourcesid='" + recentSourceID + "']";
    }
    if (recentStatusID != 0) {
        DropDownSummaryShopFlag = true;
        conditionStr += "[StatusId='" + recentStatusID + "']";
    }
    if (recentLengthID != 0) {
        var rentalLength = computeData(recentLengthID.toString(), "lengths");
        DropDownSummaryShopFlag = true;
        conditionStr += "[rentallengthname*='" + rentalLength + "']";
    }
    if (recentLocationID != 0) {
        DropDownSummaryShopFlag = true;
        conditionStr += "[locationbrandid='" + recentLocationID + "']";
    }
    if (recentUserID != 0) {
        DropDownSummaryShopFlag = true;
        conditionStr += "[UserId='" + recentUserID + "']";

    }
    if (recentSummaryType != 0) {
        DropDownSummaryShopFlag = true;
        if (recentSummaryType == 3) {
            conditionStr += "[isautomationsummary=false][isftbsummary=false]";
        }
        if (recentSummaryType == 1) {
            conditionStr += "[isautomationsummary=true]";
        }
        if (recentSummaryType == 2) {
            conditionStr += "[isftbsummary=true]";
        }
    }

    if (DropDownSummaryShopFlag) {
        $("#bindSearchSummary ul li").show().not(conditionStr).hide();
    }
}

var BindControlEvent = function () {
    $('.collapse').eq(0).css('cursor', 'pointer').click(function () {
        if ($(this).attr('src').indexOf('expand') > 0) {
            $(this).attr('src', 'images/Search-collapse.png');
        }
        else {
            $(this).attr('src', 'images/expand.png')
        }

        $('#searchLeftPanel').slideToggle();

        if ($('#left-col .collapse[src="images/expand.png"]').length == 2) {
            AnimateLeftPanel();
        }
    });

    $('.collapse').eq(1).css('cursor', 'pointer').click(function () {
        scrollTop = $('#pastSearches').scrollTop();
        if ($(this).attr('src').indexOf('expand') > 0) {
            $(this).attr('src', 'images/Search-collapse.png');
        }
        else {
            $(this).attr('src', 'images/expand.png')
        }

        $('.side-section-body').eq(1).slideToggle();

        if ($('#left-col .collapse[src="images/expand.png"]').length == 2) {
            scrollTop = $('#pastSearches').scrollTop();
            AnimateLeftPanel();
        }
    });

    $('.toggle').css('cursor', 'pointer').click(function () {
        scrollTop = $('#pastSearches').scrollTop();
        AnimateLeftPanel();
    });

    $("#SearchFilters img").css('cursor', 'pointer').bind("click", function () {
        $("#tableSearchFilters").toggle();
        if ($(this).attr('src').indexOf('plus') > 0) {
            $(this).attr('src', 'images/minus.png').css('margin-bottom', '3px');
        }
        else {
            $(this).attr('src', 'images/plus.png').css('margin-bottom', '0px');;
        }
    });

    //For Lengths checkbox events
    $('#lengths-all').bind('click', function () {

        if ($(this).is(':checked')) {
            $('#lengths-day, #lengths-week').prop('checked', true);
            $('#rentalLengthtd select option').each(function () {
                var lorvalue= parseInt($(this).attr("value"))
                if (lorvalue < 9 || lorvalue > 13) {
                    $(this).prop('selected', true);
                }
                if ($(this).text() == 'M30' && !$('#lengths-month').prop('checked')) {
                    $(this).prop('selected', false);
                }
            });
        }
        else {
            $('#lengths-day, #lengths-week').prop('checked', false);
            $('#rentalLengthtd select option').each(function () {
                var lorvalue= parseInt($(this).attr("value"))
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
            $('#rentalLengthtd select option').each(function () {
                if ($(this).text().indexOf($selectedControl.next().text()) >= 0) {
                    var lorvalue= parseInt($(this).attr("value"))
                    if (lorvalue < 9 || lorvalue > 13) {
                        $(this).prop('selected', true);
                    }
                }
            });
        }
        else {
            $('#rentalLengthtd select option').each(function () {
                if ($(this).text().indexOf($selectedControl.next().text()) >= 0) {
                    var lorvalue= parseInt($(this).attr("value"))
                    if (lorvalue < 9 || lorvalue > 13) {
                        $(this).prop('selected', false);
                    }
                }
            });
        }

        SelectAllLengthCheckboxes();
    });

    $('#pickupHour ul li').click(function () {

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

    $('#resetSelection').click(function () {
        ResetSelection();
    });

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
            }
            else {
                maxDate.setMonth(maxDate.getMonth() + 1);
            }
            var date2 = $('#startDate').datepicker('getDate');
            date2.setDate(date2.getDate() + 3);
            $('#endDate.date-picker').datepicker('option', { defaultDate: date2, minDate: dSelectedDate, maxDate: maxDate });
            $('#endDateimg').datepicker('option', { defaultDate: date2, minDate: dSelectedDate, maxDate: maxDate });
            $("#endDate").datepicker("setDate", date2);
        }
    });
    $('#endDate.date-picker,#endDateimg').datepicker({
        minDate: 0,
        numberOfMonths: 2,
        dateFormat: 'mm/dd/yy',
    });

    $('#startDateimg').click(function () { $(this).datepicker('show'); });
    $('#endDateimg').click(function () { $(this).datepicker('show'); });

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

    $('#SummarySearch').click(function () {
        var sourceIds = [], carClassIds = [], rentalLengthIds = [], sources = [], carClassArr = [], locationArr = [], locationBrandIds = [], locationIds = [];
        var selectedAPICode = '';
        $('#locations').children('ul').find('option:selected').each(function () {
            locationBrandIds.push($(this).attr('BrandId'));
            locationIds.push($(this).attr('value'));
            locationArr.push($(this).attr('LocationCode'));
        });
        locationBrandIds.length == 0 ? addErrorToField("locations") : removeErrorToField("locations");

        $('#source').find('option:selected').each(function () {
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

        $('#carClass').find('option:selected').each(function () {
            carClassIds.push($(this).attr('value'));
            carClassArr.push($(this).text());
        });
        carClassIds.length == 0 ? addErrorToField("carClass") : removeErrorToField("carClass");

        $('#startDate').val() == '' ? addErrorToField("startDate") : removeErrorToField("startDate");
        $('#endDate').val() == '' ? addErrorToField("endDate") : removeErrorToField("endDate");

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
        if ($('#searchLeftPanel').find('.has-error').length > 0) {
            $('#error-span').show();
            AddFlashingEffect();
        }

        else {
            $('#error-span').hide();
            AddFlashingEffect();

            var searchModel = new Object();
            searchModel.ScrapperSourceIDs = $.unique(sourceIds).toString();
            searchModel.LocationBrandIDs = $.unique(locationBrandIds).toString();
            searchModel.RentalLengthIDs = $.unique(rentalLengthIds).toString();
            searchModel.CarClassesIDs = $.unique(carClassIds).toString();
            searchModel.StartDate = $('#startDate').val();
            searchModel.EndDate = $('#endDate').val();
            searchModel.PickUpTime = $('#pickupHour li').eq(0).text();
            searchModel.DropOffTime = $('#dropOffHour li').eq(0).text();
            searchModel.CreatedBy = $('#LoggedInUserId').val();
            searchModel.ScrapperSource = $.unique(sources).toString();
            searchModel.CarClasses = $.unique(carClassArr).toString();
            searchModel.location = $.unique(locationArr).toString();
            searchModel.SelectedAPI = selectedAPICode;
            if (typeof (selectedAPICode) != "undefined" && selectedAPICode != "") {
                searchModel.ProviderId = $("#source option[prvcode='" + selectedAPICode + "']").attr("providerid");
            }
            else {
                searchModel.ProviderId = 1;
            }
            //is GOV attribute of scrapper source 
            searchModel.IsGovShop = $.trim($('#source').find('option:selected').attr('isgov'));
            //Logged in User Name
            searchModel.UserName = $('#hdnUserName').val();
            searchModel.ShopType = "SummaryShop";
            StartSummaySearch(searchModel);
        }


    });

    $("#Delete").click(function () {
        if (selectedSearchSummaryID != 0) {
            ShowConfirmBox(" Do you want to delete the <b>" + $("#selectedSummaryID_" + selectedSearchSummaryID + " span ").eq(0).html() + "</b> summary shop?", true, DeleteSummaryShopCallBack, selectedSearchSummaryID);
        }
    });
}

//#REGION Funtions called from partial view
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
    SelectAllLengthCheckboxes();

}

function checkUncheckCarClasses() {
    if ($('#carClass option:selected').length == $('#carClass option').length) {
        $('#carClass-all').prop('checked', true);

    } else {
        $('#carClass-all').prop('checked', false);
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
//#END REGION

// add an error display to a given field
var addErrorToField = function (field) {
    $('#' + field).addClass('has-error');
    MakeTagFlashable('#' + field);
    AddFlashingEffect();
}

var removeErrorToField = function (field) {
    $('#' + field).removeClass('has-error');
    RemoveFlashableTag('#' + field);
    AddFlashingEffect();
}

var ShowSearchSummaryButton = function () {
    $("#Search").hide();
    $("#SummarySearch").show();
}

var SearchGrid = function (inpuText, controlId) {
    if ($(inpuText).val().length > 0) {
        var matches = $.map(smartSearchLocations, function (item) {
            if (item.LocationBrandAlias.toUpperCase().indexOf($(inpuText).val().toUpperCase()) == 0) {
                return new locations(item);
            }
        });
        summaryViewModel.locations(matches);

    } else {
        var srcs = $.map(smartSearchLocations, function (item) { return new locations(item); });
        summaryViewModel.locations(srcs);
    }
}

var scrollTop = $('#pastSearches').scrollTop();
function AnimateLeftPanel(takeScrollheight) {
    $('#searchLeftPanel').slideUp();

    $('.side-section-body').eq(1).slideUp();
    $('#left-col').hide('slide', { direction: 'left' }, 1000);
    $('#right-col').addClass('calculatedWidth');
    $('.collapse').attr('src', 'images/expand.png');

    setTimeout(function () {
        $('.left-col-toggle').show(250).click(function () {
            $(this).hide(250);
            setTimeout(function () {
                $('#left-col').show('slide', { direction: 'left' }, 750);
                setTimeout(function () {
                    $('#searchLeftPanel').slideDown();
                    $('.side-section-body').eq(1).slideDown();
                    $('#right-col').removeClass('calculatedWidth');
                    $('.collapse').attr('src', 'images/Search-collapse.png');

                    setTimeout(function () {
                        $('#pastSearches').scrollTop(scrollTop);
                    }, 300);

                }, 750);
            }, 250);
        });
    }, 750);
}

var SelectAllLengthCheckboxes = function () {
    if ($('#lengths-day').prop('checked') && $('#lengths-week').prop('checked')) {
        $('#lengths-all').prop('checked', true);
    }
    else {
        $('#lengths-all').prop('checked', false);
    }
}

var ResetSelection = function () {
    $('#searchLocation').val('');
    $("#startDate").datepicker("hide");
    $("#endDate").datepicker("hide");
    var today = new Date();
    $("#startDate").val('');
    $("#endDate").val('');
    $('#endDate,#endDateimg').datepicker('option', { defaultDate: today, minDate: 0, maxDate: null });

    var srcs = $.map(smartSearchLocations, function (item) { return new locations(item); });
    summaryViewModel.locations(srcs);

    $('#searchLeftPanel select option').each(function () {
        $(this).prop("selected", false);
    });

    $('#lengths-day, #lengths-week, #lengths-month, #lengths-all, #carClass-all').prop('checked', false);

    $('#pickupHour li').eq(0).attr('value', '11:00 am').text("11:00am");
    $('#dropOffHour li').eq(0).attr('value', '11:00 am').text("11:00am");
    $("#error-span").hide();
    $("#searchLeftPanel .has-error").removeClass("has-error temp flashBorder");
    $('#searchLocation').removeClass("temp flashBorder");

    $('#source select').val('');
    $('select#carClass, select#lengths, select#locations,select#source').scrollTop(0);
}

function showResult() {
    $("#lastShopDay").hide();
    GlobalLimitSearchSummaryData = "";
    GlobalLimitSearchSummaryData = searchSummaryData;//used for Global limit implementation
    SearchSummaryId = searchSummaryData.SearchSummaryID;
    scrollTop = $('#pastSearches').scrollTop();
    bindSearchFilters();
    $('.dailyFilter,.daily_view').show();
    $('#noRecords').hide();
    getFTBSummary(false);
}

//For used get names
function computeData(Ids, controlID) {
    var tempName = "", TempIDs = Ids.split(',');

    if (controlID == "carClass") {
        $("#" + controlID + " option").each(function () {
            if ($.inArray($(this).attr('value'), Ids.split(',')) != -1) {
                tempName += $(this).text() + ", ";
            }
        });
    }
    else {
        for (var i = 0; i < TempIDs.length; i++) {
            if (controlID == "locations") {
                var $ctrl = $("#" + controlID + " option[brandID=" + TempIDs[i] + "]");
                if ($ctrl.length > 0) {
                    tempName += $ctrl.text() + ", ";
                }
            }
            else {
                var $ctrl = $("#" + controlID + " option[value=" + TempIDs[i] + "]");
                if ($ctrl.length > 0) {
                    tempName += $ctrl.text() + ", ";
                }
            }
        }
    }
    return tempName.trim().substring(0, tempName.trim().length - 1);
}


function SetUserFilter() {
    var isAdmin = $("#hdnIsAdminUser").val();
    if (sessionStorage.getItem('SearchSummaryDetails') != null) {
        var summaryDetails;
        summaryDetails = JSON.parse(sessionStorage.getItem('SearchSummaryDetails'));
        var $users = $('#recentUsers ul li[value="' + summaryDetails.ShopCreatedBy + '"]').addClass('selected');
        $('#recentUsers li').eq(0).text($users.text()).attr('value', $users.attr('value'));
        if (isAdmin.toUpperCase() == "FALSE") {
            $('#tableSearchFilters #recentUsers ul li[value="0"]').hide();
        }
    }
    else if (isAdmin.toUpperCase() == "FALSE") {
        if ($('#LoggedInUserId') != undefined && $('#LoggedInUserId').val().trim() != '') {
            var loggedInUserId = parseInt($('#LoggedInUserId').val());
            $('#tableSearchFilters #recentUsers ul li[value="' + loggedInUserId + '"]').addClass('selected');
            $('#tableSearchFilters #recentUsers li').eq(0).val($('#tableSearchFilters #recentUsers ul li[value="' + loggedInUserId + '"]').val()).text($('#tableSearchFilters #recentUsers ul li[value="' + loggedInUserId + '"]').text());
        }
        $('#tableSearchFilters #recentUsers ul li[value="0"]').hide();
        //$('#tableSearchFilters #recentUsers').addClass("disable-UL");
    }
}

function HideShowNavigation()
{
    if ($('#pastSearches li[statusid="4"]').length <= 0) {
        $('.dailyFilter,.daily_view').hide();
        $('#noRecords').show();
    }
    else 
    {
        $('.dailyFilter,.daily_view').show();
        $('#noRecords').hide();
    }
}


function ShowFirstRecordIfNoPrevRecordExists() {
    if ($('#noRecords').is(':visible') && $('#pastSearches li[statusid="4"]:visible').length > 0) {
        $('#pastSearches li[statusid="4"]:visible').eq(0).trigger('click');
        $('#viewResult').trigger("click");
        $('#viewResult').removeClass("disable-button").unbind('click').bind('click', function () {
            showResult();
        });
        HideShowNavigation();
    }
}

function ShowHideRecentShopsLoader(show) {
    if (!show) {
        $(".loader_container_shops").hide();
    }
    else {
        $(".loader_container_shops").show();
    }
}
//End other functions

