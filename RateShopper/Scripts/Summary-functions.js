var loadingPrevResult = false;
$(document).ready(SummaryFunctionDocumentReady);

function SummaryFunctionDocumentReady() {
    setTimeout(function () {
        CheckSessionStorageNLoadData();
    }, 250);
}



//Ajax call functions
function getFTBSummary(getDefaultData, selector) {
    var t = new Date(); console.log("call to getsearchdata at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
    //scroll top and left for the grid
    $('.body-section-body').scrollLeft(0);

    //if IE then use only scrolltop
    if (window.navigator.userAgent.indexOf("MSIE ") > 0 || !!navigator.userAgent.match(/Trident.*rv\:11\./)) {
        $('html').scrollTop(0);
    }
    else {
        $('body').animate({
            scrollTop: 0
        }, 600);
    }
    if (selector == null) {
        selector = '';
    }
    $('.NoResultsFound').remove();
    $('.loader_container_main').show();
    //click on appropriate search from Recent Searches 
    //so summary for this search will be visible
    var $pastSearchLi = $('.pastSearchul li[value="' + SearchSummaryId + '"]');
    $pastSearchLi.click();

    var container = $('.pastSearchul');
    scrollTo = $pastSearchLi;
    if (scrollTo.length > 0) {
        container.scrollTop(
            scrollTo.offset().top - 30 - container.offset().top + container.scrollTop()
        );
    }
    var data = new Object();
    data.searchSummaryID = SearchSummaryId;
    data.scrapperSourceID = ($('#dimension-source' + selector + ' li').eq(0).attr('value'));
    data.locationBrandID = $('#location ul li.selected').attr('lbid');
    data.locationID = $('#location ul li.selected').val();
    data.brandID = $('#location ul li.selected').attr('brandid');
    data.rentallengthID = ($('#length' + selector + ' li').eq(0).attr('value'));
    data.arrivalDate = $('#displayDay' + selector + ' li').eq(0).attr('value');
    var t = new Date(); console.log("start ajax call to get daily view data at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
    var ajaxURl = '/RateShopper/Summay/GetFTBSummaryData/';
    if (SummayShopAjaxURLSettings != undefined && SummayShopAjaxURLSettings != '') {
        ajaxURl = SummayShopAjaxURLSettings.GetFTBSummary;
    }

    if (data.searchSummaryID > 0 && typeof (data.scrapperSourceID) != 'undefined' && typeof (data.locationBrandID) != 'undefined') {

        $.ajax({
            url: ajaxURl,
            type: 'GET',
            data: data,
            dataType: 'json',
            success: function (data) {
                var t = new Date(); console.log("Got daily view response and bind it to grid at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
                bindDailySearchGrid(data);
                $('.NoResultsFound').hide();
                var t = new Date(); console.log("Binding completes at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $('.loader_container_main').hide();
                summaryViewModel.rates(null);
                summaryViewModel.headers(null);
                $('<tr class="NoResultsFound"><td colspan="4" class="red bold" style="align:left">No Rates Found</td></tr>').appendTo('#daily-rates-table tbody');
                FTBSetSessionStorage();
                //setLastUpdateTSD('');
            }
        });
    }
    else {
        $('.loader_container_main').hide();
        summaryViewModel.rates(null);
        summaryViewModel.headers(null);
        $('<tr class="NoResultsFound"><td colspan="4" class="red bold" style="align:left">No Rates Found</td></tr>').appendTo('#daily-rates-table tbody');
        FTBSetSessionStorage();
    }
}


function CheckSessionStorageNLoadData() {

    //$('.dailyFilter,.daily_view').show();
    //$('#noRecords').hide();
    if (typeof Storage !== "undefined" && sessionStorage.getItem('FTBctrlState') != null) {
        loadingPrevResult = true;
        if ($('#pastSearches li').length <= 0) {
            setTimeout(function () {
                CheckSessionStorageNLoadData();
            }, 150);
            return;
        }


        var ctrlState = JSON.parse(sessionStorage.getItem('FTBctrlState'));

        var $pastSearchLi = $('.pastSearchul li[value="' + ctrlState.searchSummaryID + '"]');

        if ($pastSearchLi.length <= 0) {
            sessionStorage.removeItem('FTBctrlState');
            CheckSessionStorageNLoadData();
        }
        currentView = $.trim($('#viewSelect .selected').attr('value'));
        SearchSummaryId = ctrlState.searchSummaryID;
        $pastSearchLi.click();
        $('.loader_container_main').show();
        //FetchLastUpdateTSD();
        setTimeout(function () { bindDataUsingSessionStorage(ctrlState); }, 250);
        //get selection ctrl

    }
    else if (typeof Storage !== "undefined" && sessionStorage.getItem('SearchSummaryDetails') != null) {
        //showResultOfSelectedSummary();
        if ($('#pastSearches li').length <= 0) {
            setTimeout(function () {
                CheckSessionStorageNLoadData();
            }, 150);
            return;
        }
        else {
            showResultOfSelectedSummary();
        }
    }
    else {
        //get default data
        $.ajax({
            url: 'Summary/GetFTBSearchGridDailyViewDataDefault',
            type: 'GET',
            data: { LoggedInUserID: $('#LoggedInUserId').val(), isAdmin: $("#hdnIsAdminUser").val() },
            dataType: 'json',
            success: function (data) {
                if (data.SearchSummaryId > 0) {
                    var $pastSearchLi = $('.pastSearchul li[value="' + data.SearchSummaryId + '"]');
                    $pastSearchLi.click();
                }
                waitandbindSearchFilters(data);
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $('.loader_container_main').hide();
                summaryViewModel.rates(null);
                summaryViewModel.headers(null);
                bindSearchFilters();
                setTimeout(function () {
                    SearchSummaryId = searchSummaryData.SearchSummaryID;
                    FTBSetSessionStorage();
                }, 550);
                $('<tr class="NoResultsFound"><td colspan="4" class="red bold" style="align:left">No Rates Found</td></tr>').appendTo('#daily-rates-table tbody');
                $('#TetherRate, #update, #updaten, #IsTetherUser, .resetEdit').prop('disabled', true).addClass("disable-button");
                //setLastUpdateTSD('');
            }
        });
    }
}
//End Ajax call functions


//Other functions
var FTBSetSessionStorage = function () {

    //Expecting user to wait for at least 250ms on search page and then save the last paramters
    setTimeout(function () {
        var ctrlState = new Object();
        ctrlState.searchSummaryID = SearchSummaryId;
        ctrlState.scrapperSourceID = ($('#dimension-source li').eq(0).attr('value'));
        ctrlState.locationBrandID = $('#location ul li.selected').attr('lbid');
        ctrlState.rentallengthID = ($('#length li').eq(0).attr('value'));
        ctrlState.selectedLOR = $('.showSelectedDate').text();
        sessionStorage.setItem('FTBctrlState', JSON.stringify(ctrlState));
    }, 250);
}
var bindDataUsingSessionStorage = function (ctrlState) {

    bindSearchFilters();
    var $source = $('#dimension-source ul li[value="' + ctrlState.scrapperSourceID + '"]').first();
    var $location = $('#location ul li[lbid="' + ctrlState.locationBrandID + '"]').first();
    var $rentalLength = $('#length ul li[value="' + ctrlState.rentallengthID + '"]').first();

    //set text between next and prev buttons
    $('.showSelectedDate').html(ctrlState.selectedLOR);

    //Set filter selections
    $('#dimension-source li').removeClass('selected').eq(0).text($source.text()).attr('value', $source.addClass('selected').attr('value'));


    $('#location li').removeClass('selected').eq(0).text($location.text()).attr('value', $location.addClass('selected').attr('value'));

    $('#length li').removeClass('selected').eq(0).text($rentalLength.text()).attr('value', $rentalLength.addClass('selected').attr('value'));

    $('.LOR').hide();


    //Bind controls for all element for all view


    //$('.showSelectedDate').html($date.text());
    getFTBSummary(false);

    //View Result End
    //sessionStorage.removeItem('ctrlState');
}
function waitandbindSearchFilters(data) {
    if (typeof searchSummaryData !== "undefined") {
        //variable exists, do what you want
        //getSearchData(true);        
        $('.NoResultsFound').hide();
        SearchSummaryId = searchSummaryData.SearchSummaryID;

        //bind filters      
        bindSearchFilters();
        bindDailySearchGrid(data);
    }
    else {
        setTimeout(function () {
            waitandbindSearchFilters(data);
        }, 250);
    }
    //fetch dependent locationBrandid and extra day for tethered update.
    //  getLocationBrand();
}
function bindSearchFilters(selector) {
    if (typeof searchSummaryData == "undefined" || searchSummaryData == undefined || searchSummaryData == null || searchSummaryData == '') {
        setTimeout(function () {
            bindSearchFilters(selector);
        }, 100);
        return false;
    }
    if (selector == null) {
        selector = '';
    }
    //set dimension-source
    $('#dimension-source' + selector + ' ul').empty();
    var SSvalue = searchSummaryData.SourceName.split(',');
    var SSid = searchSummaryData.SourcesIDs.split(',');
    for (var i = 0; i < SSid.length; i++) {
        $("<li value='" + SSid[i] + "'>" + SSvalue[i] + "</li>").appendTo('#dimension-source' + selector + ' ul');
    }
    $('#dimension-source' + selector + ' li').eq(0).text($('#dimension-source' + selector + ' ul li').eq(0).text()).attr('value', $('#dimension-source' + selector + ' ul li').eq(0).addClass('selected').attr('value'));

    //set location
    $('#location' + selector + ' ul').empty();
    var Lvalue = searchSummaryData.LocationName.split(',');
    var Lid = searchSummaryData.LocationIDs.split(',');
    var LBid = searchSummaryData.LocationsBrandIDs.split(',');
    var Bid = searchSummaryData.BrandIDs.split(',');
    for (var i = 0; i < Lid.length; i++) {
        $("<li value='" + Lid[i] + "' lbid='" + LBid[i] + "' brandid='" + Bid[i] + "'>" + Lvalue[i] + "</li>").appendTo('#location' + selector + ' ul');
    }
    $('#location' + selector + ' li').eq(0).text($('#location' + selector + ' ul li').eq(0).text()).attr('value', $('#location' + selector + ' ul li').eq(0).addClass('selected').attr('value'));

    //set length
    $('#length' + selector + ' ul').empty();
    var RLvalue = searchSummaryData.RentalLengthName.split(',');
    var RLid = searchSummaryData.RentalLengthsIDs.split(',');
    for (var i = 0; i < RLid.length; i++) {
        $("<li value='" + RLid[i] + "'>" + RLvalue[i].trim() + "</li>").appendTo('#length' + selector + ' ul');
    }
    $('#length' + selector + ' li').eq(0).text($('#length' + selector + ' ul li').eq(0).text()).attr('value', $('#length' + selector + ' ul li').eq(0).addClass('selected').attr('value'));

    //set text between next and prev buttons
    $('.showSelectedDate').html($('#length' + selector + ' li').eq(0).html());


    //on change of filters, get new grid
    $(".dailyFilter ul ul li").unbind('click').bind('click', function (event) {

        self = $(this);
        setTimeout(function () {
            var elementId = self.parents('ul').parents('ul').attr('id');
            if (elementId.indexOf('location') >= 0 && $('#location ul li').length <= 1) {
                return false;
            }
            if (elementId.indexOf('dimension-source') >= 0 && $('#dimension-source ul li').length <= 1) {
                return false;
            }
            if (elementId.indexOf('length') >= 0) {
                //set text between next and prev buttons
                $('.showSelectedDate').html($('#length li').eq(0).html());
            }
            getFTBSummary(false);
        }, 250);
        enableDisableNavigation();
    });

    $('.dailyViewPrevDay').hide();
    enableNextPrevButton('.dailyViewPrevDay', false);
    if ($('#length ul li').length <= 1) {
        enableNextPrevButton('.dailyViewNextDay', false);
    }
    else {
        enableNextPrevButton('.dailyViewNextDay', true);
    }

    //Prev day of Summary is clicked    
    $('.dailyViewPrevDay').unbind('click').bind('click', function () {
        prevSummaryDay();
        setTimeout(function () { getFTBSummary(false); }, 250);
        enableDisableNavigation();
    });

    //Next day Clicked
    $('.dailyViewNextDay').unbind('click').bind('click', function () {
        nextSummaryDay();
        getFTBSummary(false);
        enableDisableNavigation();
    });

    var startDate = convertToServerTime(new Date(parseInt(GlobalLimitSearchSummaryData.StartDate.replace("/Date(", "").replace(")/", ""), 10)));
    var endDate = convertToServerTime(new Date(parseInt(GlobalLimitSearchSummaryData.EndDate.replace("/Date(", "").replace(")/", ""), 10)));

    var displayStartMonth = monthNames[startDate.getMonth()];
    var displayEndMonth = monthNames[endDate.getMonth()];

    $("#ShopStartDate").html(displayStartMonth.substr(0, 3) + " " + startDate.getDate() + ", " + startDate.getFullYear());
    $("#ShopEndDate").html(displayEndMonth.substr(0, 3) + " " + endDate.getDate() + ", " + endDate.getFullYear());
}
function bindDailySearchGrid(d) {
    var t = new Date(); console.log("start binding daily view--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
    setTimeout(function () {
        $('.loader_container_main').hide();
    }, 300);

    brandID = d.brandID;
    var finalData = JSON.parse(d.finalData);
    var headerData = $.map(finalData.HeaderInfo, function (item) { return new HeaderInfo(item) });
    summaryViewModel.headers(headerData);

    var rateData = $.map(finalData.RatesInfo, function (item) { return new RatesInfo(item) })
    summaryViewModel.rates(rateData);
    highlightLowestCompetitor();
    FTBSetSessionStorage();
    var t = new Date(); console.log("complete binding rates--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());

    //set '$' before total and base rate
    $('.perDay').each(function () {
        if ($(this).text().trim().length) {
            $(this).siblings('.daily-rate').prepend('$');
            $(this).siblings('.tv').prepend('$');
        }
    });
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

function highlightLowestCompetitor() {

    $('.dailytable tbody tr').each(function () {
        var $thisTr = $(this);
        if ($thisTr.find(".highlightGreen").html() != undefined) {
            var competitorGreenTotalRate;
            var myCompanyTotalRate;

            var $highlightGreen = $thisTr.find(".highlightGreen");
            var $myComp;
            if (GlobalLimitSearchSummaryData != null) {
                $myComp = $thisTr.find("td[companyID=" + brandID + "] .tv").not('.highlight');
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

function prevSummaryDay() {
    if ($('#length ul li.selected').index() != 0) {
        $('#length ul li.selected').removeClass('selected').prev().addClass('selected');
    }
    $('#length li').eq(0).text($('#length ul li.selected').text()).attr('value', $('#length ul li.selected').attr('value'));

    $('.showSelectedDate').html($('#length li').eq(0).html());
    return false;
}

function nextSummaryDay(selector) {
    if (selector == null) {
        selector = '';
    }
    //enableNextPrevButton('.dailyViewPrevDay' + selector + '', true);
    if ($('#length' + selector + ' ul li').length != $('#length' + selector + ' ul li.selected').index() + 1) {
        $('#length' + selector + ' ul li.selected').removeClass('selected').next().addClass('selected');
    }
    else {
        //if ($('#length' + selector + ' ul li').length != $('#length' + selector + ' ul li.selected').index() + 1) {
        //    $('#displayDay' + selector + ' ul li.selected').removeClass('selected');
        //    $('#displayDay' + selector + ' ul li').first().addClass('selected');
        //    $('#length' + selector + ' ul li.selected').removeClass('selected').next().addClass('selected');
        //    $('.LOR').show();


        //}
    }
    $('#length' + selector + ' li').eq(0).text($('#length' + selector + ' ul li.selected').text()).attr('value', $('#length' + selector + ' ul li.selected').attr('value'));

    $('.showSelectedDate').html($('#length' + selector + ' li').eq(0).html());
    return false;
}

function enableDisableNavigation() {
    setTimeout(function () {
        if ($('#length ul li').length <= 1) {
            enableNextPrevButton('.dailyViewPrevDay', false);
            enableNextPrevButton('.dailyViewNextDay', false);
        }
        else {
            if ($('#length ul li').first().hasClass('selected')) {
                enableNextPrevButton('.dailyViewPrevDay', false);
                if ($('#length ul li').length > 1) {
                    enableNextPrevButton('.dailyViewNextDay', true);
                }
            }
            else if ($('#length ul li').last().hasClass('selected')) {
                enableNextPrevButton('.dailyViewNextDay', false);
                if ($('#length ul li').length > 1) {
                    enableNextPrevButton('.dailyViewPrevDay', true);
                }
            }
            else {
                enableNextPrevButton('.dailyViewPrevDay', true);
                enableNextPrevButton('.dailyViewNextDay', true);
            }
        }

        showLastShopDay();
    }, 250);
}


function showResultOfSelectedSummary() {

    var summaryDetails;
    summaryDetails = JSON.parse(sessionStorage.getItem('SearchSummaryDetails'));
    var $pastSearchLi = $('#pastSearches  li[value=' + summaryDetails.SearchSummaryId + ']');

    if ($pastSearchLi.length <= 0) {
        sessionStorage.removeItem('SearchSummaryDetails');
        CheckSessionStorageNLoadData();
    }
    else {
        sessionStorage.removeItem('SearchSummaryDetails');

        $("#tableSearchFilters").toggle();
        if ($("#SearchFilters img").attr('src').indexOf('plus') > 0) {
            $("#SearchFilters img").attr('src', 'images/minus.png').css('margin-bottom', '3px');
        }
        $('#recentSources ul li, #recentLocations ul li,#recentUsers ul li').removeClass('selected');
        var $source = $('#recentSources ul li[value="' + summaryDetails.ScrapperSourceIDs + '"]').addClass('selected');
        var $brandLocation = $('#recentLocations ul li[value="' + summaryDetails.LocationBrandsIds + '"]').addClass('selected');
        var $users = $('#recentUsers ul li[value="' + summaryDetails.ShopCreatedBy + '"]').addClass('selected');
        var $summaryType = $('#recentSummaryType ul li[value="' + summaryDetails.SummaryType + '"]').addClass('selected');
        //set selected values to controls
        $('#recentSources li').eq(0).text($source.text()).attr('value', $source.attr('value'));
        $('#recentLocations li').eq(0).text($brandLocation.text()).attr('value', $brandLocation.attr('value'));
        $('#recentUsers li').eq(0).text($users.text()).attr('value', $users.attr('value'));
        $('#recentSummaryType li').eq(0).text($summaryType.text()).attr('value', $summaryType.attr('value'));

        populateSearchSummaryDropDown(summaryDetails.ShopCreatedBy, 0, 0, summaryDetails.LocationBrandsIds, summaryDetails.ScrapperSourceIDs, 0, $summaryType.attr('value'));
        $pastSearchLi.click();
        showResult();
    }

}

function showLastShopDay() {

    if ($('.dailyViewNextDayDisabled').is(':visible')) {
        //console.log('1');
        $("#lastShopDay").show();
    }
    else {
        $("#lastShopDay").hide();
    }
}
//---End Other functions