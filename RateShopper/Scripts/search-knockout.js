var searchViewModel;
var formulaTtoB, formulaBtoT, SalesTex, AirportFees, Arena, surcharge, VLRF, CFC, total = 0, formulaCalculation, nd = 0, len = 0, LoggedInUserID, selectedSearchSummaryID = 0, rt, selectedFirstSummaryID = 0, dominantrentalLength, AssoMapId;
var TetherformulaTtoB, TetherformulaBtoT, TetherSalesTex, TetherAirportFees, TetherArena, Tethersurcharge, TetherVLRF, TetherCFC, Tethertotal = 0, TetherRT;
var formulaWeekLength = 1, formulaMonthLength = 1, formulaWeekLengthW12toW14 = 2;//Used to set static for calucaltion based on selected length while value will W5 to M30 is set 1
var lastModifiedDate;
var SelectedSearchSummaryScrollHeight = 0;
var searchSummaryData, SearchSummaryId;
var GlobalTetherSetting;
var PreloadGlobaltetherData = [];
var GlobalTetherSettingUndefined = false;
var days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
var monthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
var months = ['01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12'];
var locationIds = [];
var locationBrandIds = [];
var sourceIds = [], carClassIds = [], rentalLengthIds = [];
var sources = [], carClassArr = [], locationArr = [];
var selectedAPICode = '';
var GetCompanyBrandList;
var smartSearchLocations, previousRentalLength;
var tetherShopFirstClick = false;
var FinalTetherValueData = []; //for check if user can reset the value get the data from prevous value
var CheckBindCarClass = false, CheckBindLocation = false, CheckfirstSearchSummary = false, checkLoadSearchSummaryAjax = false;
var FirstSearchSummaryDataCollection;
var GlobalLimitSearchSummaryData;//User for GLobal limit implementation in tether shop popup
var CurrentRentalLength;
var GlobalLimitDetails = "";
var dependantBrandID = 0;
var lastSelectedSearchSummaryStatus = 0;// for check last selected search status fail/inprogress will comes in reset schenario using searchSummart selection
var SearchSummaryRecursiveCall = false;
var CheckTetherFormulaAjaxCall = false, CheckTetherGlobalLimitAjaxCall = false;//For check only one time call particular on one shop 
var isNewArrayFetched = false;
var getRentalLength;
$(document).ready(function () {
    LoggedInUserID = $('#LoggedInUserId').val();

    searchViewModel = new SourceModel();
    ko.applyBindings(searchViewModel);
    TetherCopyAllData();//Copy all rate of selected textbox
    //setTimeout(function () { getCurrentFormula(); },500);//Get the formula from DB based on selected location.

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
    $("#ddlAPI ul.hidden.drop-down").find('li').eq(0).addClass('selected').closest('#ddlAPI').find('li').eq(0).attr({ 'value': ($('#ddlAPI ul.hidden.drop-down').find('li').eq(0).attr('value')), 'prvcode': ($('#ddlAPI ul.hidden.drop-down').find('li').eq(0).attr('prvcode')) }).text($("#ddlAPI ul.hidden.drop-down").find('li').eq(0).text());
    //GetAllRentalLengths();
    BindLocations();
    BindCarClasses();
    //BindUsers();
    BindStatus();
    getCompanyBrand();
    //Get last three day recent search record
    recentSearchAjaxCall();
    //CheckRecentSearchSummaryAjax();
    //setTimeout(function () {
    //    recentSearchAjaxCall();
    //}, 15000);

    recentSearchSelection();

    $("#tethered_rates").draggable();
    $("#viewAppliedRuleSet").draggable();
    $("#closepopup").click(function () {
        $("#tethered_rates").hide();
        $(".popup_bg").hide();
        ResetTetherData();
        //After popup close and again open popup this attribute is set
        $("#tethered_rates #TehterTableValue tbody tr input:text").each(function () {
            $(this).removeAttr("globallimit");
        });
    });
    $("#reset").on("click", function () {
        //ResetTetherData();
        PreloadTetherData();
    });
    getGlobalTetherValue();//Get all Tethervalue from db

    //check if user has istetheraccessing rites or not 

    if ($("#IsTetheringAccess").val() == "True") {
        //$("#IsTetherUser").attr("checked", "checked");
        $("[id=TetherRate]").prop('disabled', false).removeClass("disable-button");
    }
    else {
        //$("#IsTetherUser").removeAttr("checked");
        $("[id=TetherRate]").prop('disabled', true).addClass("disable-button");
    }
    $("#closeTetherPopup").click(function () {
        var GlobalLimitcheck = false;
        var viewSelected = $('#viewSelect li.selected').text().trim();
        if (viewSelected == 'daily') {
            $("#tethered_rates .tableTetherValueDaily tbody tr").each(function () {
                var GlobalLimit = $(this).find("td").eq(3).find("span").attr("globallimit");
                if (JSON.parse(GlobalLimit)) {
                    GlobalLimitcheck = true;
                    return false;
                }
            });
        }
        else {
            $("#tethered_rates .tableTetherValueClassic tbody tr").each(function () {
                var GlobalLimit = $(this).find("td").eq(3).find("span").attr("globallimit");
                if (JSON.parse(GlobalLimit)) {
                    GlobalLimitcheck = true;
                    return false;
                }
            });
        }

        if (GlobalLimitcheck) {
            var message = 'Rate Violation (minimum or maximum) has occurred.';
            ShowConfirmBox(message, false);
        }
        else {
            DiscardGlobalLimitItem();
        }

        //if ($("#IsTetherUser").prop("checked") == false) {
        //    sessionStorage.setItem("activateTether_" + selectedSearchSummaryID, false);
        //    $('#TetherRate').attr("disabled", "disabled").addClass("disable-button");
        //}
    });

    //Fired event on checkbox after load
    $("[id=IsTetherUser]").on("change", function () {
        if ($(this).prop("checked")) {
            $("[id=IsTetherUser]").prop('checked', true);
            if ((GlobalLimitSearchSummaryData.IsGOV == null) || !GlobalLimitSearchSummaryData.IsGOV) {
                $('[id=TetherRate]').removeAttr("disabled").removeClass("disable-button");
            }
            else {
                $('[id=TetherRate]').attr("disabled", "disabled").addClass("disable-button");
            }
            sessionStorage.setItem("activateTether_" + selectedSearchSummaryID, true);
        }
        else {
            $("[id=IsTetherUser]").prop('checked', false);
            $('[id=TetherRate]').attr("disabled", "disabled").addClass("disable-button");
            sessionStorage.setItem("activateTether_" + selectedSearchSummaryID, false);
        }
    });

    // To disable the button on checking the Activate Opaque checkbox.
    $('[id=IsOpaqueChk]').on("change", function () {
        if ($(this).prop("checked")) {
            $('[id=OpaqueRate]').removeAttr('disabled').removeClass("disable-button");
        }
        else {
            $('[id=OpaqueRate]').attr('disabled', 'disable').addClass("disable-button");
        }
    });


    $('#resetSelection').click(function () {

        resetSelection();
    });

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
    })


    //For Lengths checkbox events
    $('#lengths-all').bind('click', function () {

        if ($(this).is(':checked')) {
            $('#lengths-day, #lengths-week').prop('checked', true);
            $('#rentalLengthtd select option').each(function () {
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
            $('#rentalLengthtd select option').each(function () {
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
            $('#rentalLengthtd select option').each(function () {
                if ($(this).text().indexOf($selectedControl.next().text()) >= 0 && !$(this).is(':disabled')) {
                    var lorvalue = parseInt($(this).attr("value"))
                    if (lorvalue < 9 || lorvalue > 13) {
                        $(this).prop('selected', true);
                    }
                }
            });
        }
        else {
            $('#rentalLengthtd select option').each(function () {
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

    $('#Search').click(function () {

        sourceIds = [], carClassIds = [], rentalLengthIds = [], sources = [], carClassArr = [], locationArr = [], locationBrandIds = [], locationIds = [];
        selectedAPICode = '';
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

        //selectedAPICode = $("#ddlAPI ul.hidden li.selected").attr("prvcode");
        //if (typeof (selectedAPICode) == 'undefined') {
        //    selectedAPICode = $("#ddlAPI ul li:first").attr("prvcode");
        //}

        //if (typeof (selectedAPICode) == 'undefined' || selectedAPICode == '') {
        //    addErrorToField("ddlAPI")
        //}

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
            //MakeTagFlashable('.has-error');
            AddFlashingEffect();
        }

        else {
            $('#error-span').hide();
            //RemoveFlashableTag('.has-error');
            AddFlashingEffect();

            startSearch();
        }


    });

    $('#searchLocation').bind('input', function () {
        searchGrid('#searchLocation', '#locations option');
        if ($("#locations option[style$='display: none;']").length == $("#locations option").length) {
            MakeTagFlashable('#searchLocation');
        }
        else {
            RemoveFlashableTag('#searchLocation');
        }
        AddFlashingEffect();
    })

    $('#startDateimg').click(function () { $(this).datepicker('show'); });
    $('#endDateimg').click(function () { $(this).datepicker('show'); });
    $('[id=TetherRate]').click(function (e) {
        e.preventDefault();
        $('#tethered_rates, .popup_bg').show();
        if (navigator.userAgent.match(/iPad|iPhone|Android|Windows Phone/i) != null) {
            $('#tab1').click();
        }
        //$(window).scrollTop('0');
        TetherRateButton();
        //var fullWidth = $(window).width();
        //var popupWidth = $("#tethered_rates").width();
        //var finalWidth = (fullWidth - popupWidth) / 2;
        //console.log(fullWidth + "  " + popupWidth + " " + parseInt(finalWidth));
        //$('#tethered_rates').css("left", parseInt(finalWidth) + "px");
        $("#PercentageID_All").val("");
        $("#DollerID_All").val("");
        $('#TehterTableValue .temp').removeClass('temp').removeClass('flashBorder');
        //if (!tetherShopFirstClick) {
        //    TetherRateButton();
        //    tetherShopFirstClick = true;
        //    $("#PercentageID_All").val("");
        //    $("#DollerID_All").val("");
        //}

    })


    $("#Delete").click(function () {

        if (selectedSearchSummaryID != 0) {
            ShowConfirmBox(" Do you want to delete the <b>" + $("#selectedSummaryID_" + selectedSearchSummaryID + " span ").eq(0).html() + "</b> search?", true, DeleteSelectedSummaryCallBack, selectedSearchSummaryID);
        }
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

    $('#tab1').click(function () {
        event.preventDefault();
        $('#tetherValues').show();
        $('#TehterTableValue').hide();
        $('#tab1').addClass('red');
        $('#tab2').removeClass('red');
        $('#copytoall').hide();
        $("#reset").hide();
        $('#closeTetherPopup').removeClass('orng pointer closeP');
        $('#closeTetherPopup').val('OK');
    });

    $('#tab2').click(function () {
        event.preventDefault();
        $('#TehterTableValue').show();
        $('#tetherValues').hide();
        $('#tab2').addClass('red');
        $('#tab1').removeClass('red');
        $("#reset").show();
        $('#copytoall').show();
        $('#closeTetherPopup').addClass('orng pointer closeP');
        $('#closeTetherPopup').val('Submit');
    });
    SetUserFilter();
});




function DeleteSelectedSummaryCallBack(selectedSearchSummaryID) {
    if (typeof (selectedSearchSummaryID) == 'undefined') {
        selectedSearchSummaryID = this;
    }
    $("#selectedSummaryID_" + selectedSearchSummaryID).hide();
    $.ajax({
        url: 'Search/DeleteSelectedSummary/',
        type: 'GET',
        data: { id: selectedSearchSummaryID },
        dataType: 'json',
        async: true,
        success: function (data) {
            //Empty all search-summary item and reset search item
            $("#selectedSummaryID_" + selectedSearchSummaryID).remove();
            $("#search-summary").find(".searchSummary").html("");
            $('#viewResult').unbind('click');
            $('#viewResult').addClass("disable-button");
            SearchSummaryRecursiveCall = true;//For Settimeout recursive call not create
            recentSearchAjaxCall();
            selectedSearchSummaryID = 0;
            resetSelection();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(errorThrown);
        }
    });
    if (selectedSearchSummaryID == SearchSummaryId) {
        sessionStorage.removeItem('ctrlState');
    }
}
function DiscardGlobalLimitItem() {
    if (validateTetherShop() == true) {
        $("#tethered_rates").hide();
        $(".popup_bg").hide();

        //This is for CR user can close button previous value should be reset
        FinalTetherValueData = [];
        $("#tethered_rates #TehterTableValue tbody tr").each(function () {
            var tetherValue;
            var IsPercentageValue;
            var carClassID = $(this).find(".percentageValue").attr("carclassid");

            if (carClassID != undefined && carClassID != null) {

                var percentValue = $(this).find("td").eq(1).find("input").eq(0);
                var dollerValue = $(this).find("td").eq(1).find("input").eq(1);

                if ($(percentValue).val() != "") {
                    tetherValue = $(percentValue).val();
                    IsPercentageValue = true;
                    if (!$.isNumeric(tetherValue)) {
                        tetherValue = "";
                        $(percentValue).val("");
                    }
                }
                if ($(dollerValue).val() != "") {
                    tetherValue = $(dollerValue).val();
                    IsPercentageValue = false;
                    if (!$.isNumeric(tetherValue)) {
                        tetherValue = "";
                        $(dollerValue).val("");
                    }
                }
                if ($(percentValue).val() != "" || $(dollerValue).val() != "") {
                    var item = {}
                    item["ID"] = carClassID;
                    item["carClass"] = "";
                    item["TetherValue"] = tetherValue;
                    item["IsTeatherValueinPercentage"] = IsPercentageValue;
                    FinalTetherValueData.push(item);
                }
            }
        });
    }
    //copy all selected values to attribute defaultvalue of span associated with car class
    var carClassId;
    var viewSelected = $('#viewSelect li.selected').text().trim();
    if (viewSelected == 'daily') {
        $('#daily-rates-table').find('.car-class-img').each(function () {
            carClassId = $(this).find('img').attr('classID');
            $('#EDBaseRateID_' + carClassId).attr('defaulltValue', $('#EDBaseRateID_' + carClassId).html());
            $('#DollerID_' + carClassId).attr('defaultdollervalue', $('#DollerID_' + carClassId).val());
            $('#PercentageID_' + carClassId).attr('defaultPercentValue', $('#PercentageID_' + carClassId).val());
        });
        //Reset GlobalLimit flag while user can submit button pressed
        $("#tethered_rates .tableTetherValueDaily tbody tr").each(function () {
            $(this).find("td").eq(3).find("span").attr("globallimit", "false");
        });
    }
    else if (viewSelected == 'classic') {
        if ($('#carClass li.selected').length > 0) {
            carClassId = $('#carClass li.selected').attr('value');
            $('#EDBaseRateID_' + carClassId).attr('defaulltValue', $('#EDBaseRateID_' + carClassId).html());
            $('#DollerID_' + carClassId).attr('defaultdollervalue', $('#DollerID_' + carClassId).val());
            $('#PercentageID_' + carClassId).attr('defaultPercentValue', $('#PercentageID_' + carClassId).val());
        }
        //Reset GlobalLimit flag while user can submit button pressed
        $("#tethered_rates .tableTetherValueClassic tbody tr").each(function () {
            $(this).find("td").eq(3).find("span").attr("globallimit", "false");
        });
    }
}
function ResetTetherData() {
    $("#tethered_rates #TehterTableValue tbody tr").find("input").val("");
    $("#tethered_rates #TehterTableValue tbody tr").find("input").removeClass("flashBorder temp");

    //For last change of global limit violation of first time get value and after some change incase user go for close the popup
    $("#tethered_rates #TehterTableValue tbody tr input:text").each(function () {
        $(this).removeAttr("globallimit");
    });

    $("#tethered_rates #TehterTableValue tbody tr").each(function () {
        if (FinalTetherValueData.length != 0) {
            var carClassID = $(this).find(".percentageValue").attr("carclassid");
            if (carClassID != undefined) {
                $(FinalTetherValueData).each(function () {
                    if (carClassID == this.ID) {
                        if (JSON.parse(this.IsTeatherValueinPercentage)) {
                            $('#PercentageID_' + carClassID).val(this.TetherValue);
                            if (this.globallimit == "true") {
                                $('#PercentageID_' + carClassID).attr("globallimit", "true");
                                $("#EDBaseRateID_" + carClassID).attr("globallimit", "true");
                            }
                            CalculationOfFormula("PercentageID_" + carClassID, "%");
                        }
                        else {
                            $('#DollerID_' + carClassID).val(this.TetherValue);
                            if (this.globallimit == "true") {
                                $('#PercentageID_' + carClassID).attr("globallimit", "true");
                                $("#EDBaseRateID_" + carClassID).attr("globallimit", "true");
                            }
                            CalculationOfFormula("DollerID_" + carClassID, "$");
                        }
                        return false;
                    }
                });
            }
        }
        else {
            return false;
        }
    });
}
function PreloadTetherData() {
    $("#tethered_rates #TehterTableValue tbody tr").find("input").val("");
    $("#tethered_rates #TehterTableValue tbody tr").find("input").removeClass("flashBorder temp");

    //For last change of global limit violation of first time get value and after some change incase user go for close the popup
    $("#tethered_rates #TehterTableValue tbody tr input:text").each(function () {
        $(this).removeAttr("globallimit");
    });


    $("#tethered_rates #TehterTableValue tbody tr").each(function () {
        if (PreloadGlobaltetherData.length != 0) {
            var carClassID = $(this).find(".percentageValue").attr("carclassid");
            if (carClassID != undefined) {
                $(PreloadGlobaltetherData).each(function () {
                    if (carClassID == this.ID) {
                        if (JSON.parse(this.IsTeatherValueinPercentage)) {
                            $('#PercentageID_' + carClassID).val(this.TetherValue);
                            if (this.globallimit == "true") {
                                $('#PercentageID_' + carClassID).attr("globallimit", "true");
                                $("#EDBaseRateID_" + carClassID).attr("globallimit", "true");
                            }
                            CalculationOfFormula("PercentageID_" + carClassID, "%");
                        }
                        else {
                            $('#DollerID_' + carClassID).val(this.TetherValue);
                            if (this.globallimit == "true") {
                                $('#PercentageID_' + carClassID).attr("globallimit", "true");
                                $("#EDBaseRateID_" + carClassID).attr("globallimit", "true");
                            }
                            CalculationOfFormula("DollerID_" + carClassID, "$");
                        }
                        return false;
                    }
                });
            }
        }
        else {
            return false;
        }
    });
}
//


//Call when grid is bind for check tether value is set or not 
function CheckTetherValueDiableButton() {
    var TetherValue = 0;
    var TetherValueCheck = false;
    if (typeof (GlobalTetherSetting) == 'undefined') {
        //get global tether settings
        GlobalTetherSettingUndefined = true;
        return;
    }


    //get globalTetherSettings For dependant Brand 
    var globalTetherSettingsForBrand = $.grep(GlobalTetherSetting, function (item) {
        return item.DominentBrandID == brandID && item.LocationID == $("#location li").val();
    });
    if (globalTetherSettingsForBrand.length > 0) {
        if ($("#viewSelect").find(".selected").text() == "daily") {

            //find any tether settings done for car class present in grid
            $("#daily-rates-table tbody tr").each(function () {
                if (TetherValueCheck) {
                    return;
                }
                var $row = $(this);

                $.each(globalTetherSettingsForBrand, function (i, x) {
                    if (x.CarClassID == $row.find(".carClassLogo").attr("classid")) {
                        dependantBrandID = x.DependantBrandID;
                        TetherValue = x.TetherValue;
                        TetherValueCheck = true;

                        return;
                    }
                });
            });

            if (TetherValue == 0 && TetherValueCheck == false) {
                $('[id=TetherRate]').attr("disabled", "disabled").addClass("disable-button");
                $("[id=IsTetherUser]").attr("disabled", "disabled").prop("checked", false);
            }
            else {
                //Check the condition is thether or not that based enabled checkbox and button
                if ($("#IsTetheringAccess").val() == "True") {

                    $('[id=TetherRate]').removeAttr("disabled").removeClass("disable-button");
                    $("[id=IsTetherUser]").removeAttr("disabled").prop("checked", true);

                    var tetherCheck = sessionStorage.getItem("activateTether_" + selectedSearchSummaryID);
                    if (tetherCheck != null) {
                        if (JSON.parse(tetherCheck)) {
                            $('[id=TetherRate]').removeAttr("disabled").removeClass("disable-button");
                            $("[id=IsTetherUser]").removeAttr("disabled").prop("checked", true);
                        }
                        else {
                            $("[id=IsTetherUser]").prop("checked", false);
                            $('[id=TetherRate]').attr("disabled", "disabled").addClass("disable-button");
                        }
                    }

                    GetGlobalLimitForShop();//Get Global limit for that shop
                    getCurrentFormulaTether();//Get Tether Fomual Value

                    //For all time to check if the shop is GOV
                    if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                        $('[id=TetherRate]').attr("disabled", "disabled").addClass("disable-button");
                        //$("[id=IsTetherUser]").removeAttr("disabled", "disabled").prop("checked", true);                   
                        setTimeout(function () {
                            TetherRateButton();
                        }, 500);
                    }
                }
                else {
                    if ($("[id=IsTetherUser]").prop("checked")) {
                        GetGlobalLimitForShop();
                        getCurrentFormulaTether();//Get Tether Fomula Value
                    }
                    $('[id=TetherRate]').attr("disabled", "disabled").addClass("disable-button");
                    $("[id=IsTetherUser]").attr("disabled", "disabled").prop("checked", true);
                }
            }
        }
        else {
            $.each(globalTetherSettingsForBrand, function (i, x) {
                if (x.CarClassID == $("#carClass li").eq(0).val()) {
                    dependantBrandID = x.DependantBrandID;
                    TetherValue = x.TetherValue;
                    TetherValueCheck = true;

                    return;
                }
            });
            //for (var x in GlobalTetherSetting) {
            //    if (GlobalTetherSetting[x].CarClassID == $("#carClass li").eq(0).val() && GlobalTetherSetting[x].LocationID == $("#location li").val()) {
            //        if (brandID == GlobalTetherSetting[x].DominentBrandID) {
            //            dependantBrandID = GlobalTetherSetting[x].DependantBrandID;
            //            TetherValue = GlobalTetherSetting[x].TetherValue;
            //            TetherValueCheck = true;
            //            return;
            //        }
            //    }
            //}
            if (TetherValue == 0 && TetherValueCheck == false) {
                $('[id=TetherRate]').attr("disabled", "disabled").addClass("disable-button");
                $("[id=IsTetherUser]").attr("disabled", "disabled").prop("checked", false);
            }
            else {
                //Check the condition is tether or not that based enabled checkbox and button
                if ($("#IsTetheringAccess").val() == "True") {

                    $('[id=TetherRate]').removeAttr("disabled").removeClass("disable-button");
                    $("[id=IsTetherUser]").removeAttr("disabled").prop("checked", true);
                    var tetherCheck = sessionStorage.getItem("activateTether_" + selectedSearchSummaryID);
                    if (tetherCheck != null) {
                        if (JSON.parse(tetherCheck)) {
                            $('[id=TetherRate]').removeAttr("disabled").removeClass("disable-button");
                            $("[id=IsTetherUser]").removeAttr("disabled").prop("checked", true);
                        }
                        else {
                            $("[id=IsTetherUser]").prop("checked", false);
                            $('[id=TetherRate]').attr("disabled", "disabled").addClass("disable-button");
                        }
                    }
                    GetGlobalLimitForShop();//Get Global limit for that shop
                    getCurrentFormulaTether();//Get Tether Fomual Value

                    //For all time to check if the shop is GOV
                    if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                        $('[id=TetherRate]').attr("disabled", "disabled").addClass("disable-button");
                        //$("[id=IsTetherUser]").attr("disabled", "disabled").prop("checked", true);
                        $('[id=OpaqueRate]').attr("disabled", "disabled").addClass("disable-button");
                        $("[id=IsOpaqueChk]").attr("disabled", "disabled").prop("checked", false);
                        setTimeout(function () {
                            TetherRateButton();
                        }, 500);
                    }
                }
                else {
                    if ($("[id=IsTetherUser]").prop("checked")) {
                        GetGlobalLimitForShop();
                        getCurrentFormulaTether();//Get Tether Fomual Value
                    }
                    $('[id=TetherRate]').attr("disabled", "disabled").addClass("disable-button");
                    $("[id=IsTetherUser]").attr("disabled", "disabled").prop("checked", true);
                }
            }
        }
    }
    else {
        $('[id=TetherRate]').attr("disabled", "disabled").addClass("disable-button");
        $("[id=IsTetherUser]").attr("disabled", "disabled").prop("checked", false);
    }


}
//Click on tethershop setting popup
function TetherRateButton() {
    var sDate = convertToServerTime(new Date(parseInt(GlobalLimitSearchSummaryData.StartDate.replace("/Date(", "").replace(")/", ""), 10)));
    var startDate = sDate.getDate() + "/" + sDate.getMonth() + "/" + sDate.getFullYear();
    $("#TetherSource").html($("#dimension-source li").html());

    $("#ClassicCarClass").html($("#displayDay li").html());
    $("#TetherLength").html($("#length li").html());
    $("#locationID").html($("#location li").html());

    dependantBrandID = 0;// For Set headding of dependant and dominent brand name
    CurrentRentalLength = $.trim($("#length li").eq(0).html()).substr(0, 1).toUpperCase();//used for global limit implmentation on LOR based.Day/Week/Month

    var TetherData = new Array();
    var TetherDataClassic = [];
    var isGlobalValueApplied = false;
    //get globalTetherSettings For dependant Brand 
    var globalTetherSettingsForBrand = $.grep(GlobalTetherSetting, function (item) {
        return item.DominentBrandID == brandID && item.LocationID == $("#location li").val();
    });

    if ($("#viewSelect").find(".selected").text() == "daily") {
        //Top Hadding data
        $("#TetherDate").removeClass("hidden");
        $("#ClassicViewCarClass").addClass("hidden");
        $("#ClassicCarClass").html("<b>Date: </b>" + $("#displayDay li").html());
        //end Heading data

        $(".tableTetherValueDaily").show();
        $(".tableTetherValueClassic").hide();
        var GlobalLimit = false;
        $("#daily-rates-table tbody tr").each(function () {
            GlobalLimit = false;
            var EDBaseRate, EDTotalRate, EZTotalRate, EZBaseRate;
            if ($(this).find(".totalEdit").val() != undefined && $(this).find(".baseEdit").val() != undefined) {
                EZTotalRate = $.trim($(this).find(".totalEdit").val());
                EZBaseRate = $.trim($(this).find(".baseEdit").val());
                if (!$.isNumeric(commaRemovedNumber(EZTotalRate))) {
                    EZTotalRate = "";
                }
                if (!$.isNumeric(commaRemovedNumber(EZBaseRate))) {
                    EZBaseRate = "";
                }
                var calculation;
                var tetherValue;
                var IsPercentageValue;
                var temp;
                if (!tetherShopFirstClick) {
                    var carClassID = $(this).find(".carClassLogo").attr("classid");
                    $.each(globalTetherSettingsForBrand, function (i, x) {
                        if (x.CarClassID == carClassID) {
                            dependantBrandID = x.DependantBrandID;
                            tetherValue = x.TetherValue;
                            IsPercentageValue = x.IsTeatherValueinPercentage;

                            return;
                        }
                    });
                    //for (var x in GlobalTetherSetting) {
                    //    if (GlobalTetherSetting[x].CarClassID == $(this).find(".carClassLogo").attr("classid") && GlobalTetherSetting[x].LocationID == $("#location li").val()) {
                    //        if (brandID == GlobalTetherSetting[x].DominentBrandID) {
                    //            dependantBrandID = GlobalTetherSetting[x].DependantBrandID; //for check to display tether popup  dependant brand name
                    //            tetherValue = GlobalTetherSetting[x].TetherValue;
                    //            IsPercentageValue = GlobalTetherSetting[x].IsTeatherValueinPercentage;
                    //            temp = GlobalTetherSetting[x].ID;
                    //        }
                    //    }
                    //}
                }
                else {
                    var carClassId = $(this).find(".carClassLogo").attr("classid");
                    if (carClassId != undefined) {
                        if ($.trim($('#PercentageID_' + carClassId).val()) != "") {
                            IsPercentageValue = true;
                            tetherValue = $.trim($('#PercentageID_' + carClassId).val());
                        } else {
                            IsPercentageValue = false;
                            if ($.trim($('#DollerID_' + carClassId).val()) != "") {
                                tetherValue = $.trim($('#DollerID_' + carClassId).val());
                            }
                            else {
                                tetherValue = 0;
                            }
                        }
                        GlobalLimit = $("#EDBaseRateID_" + carClassId).attr("globallimit");
                        if (GlobalLimit == undefined && GlobalLimit == null) {
                            GlobalLimit = false;
                        }
                    }
                }

                //Used to set empty value for GOV rate.We want to send both brand have same rate only with GOV rate
                if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                    tetherValue = 0;
                    IsPercentageValue = true;
                }

                if (tetherValue != undefined && EZTotalRate != "" && EZBaseRate != "") {
                    //Standard precalculation calculation 
                    if (JSON.parse(IsPercentageValue)) {
                        if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                            calculation = parseFloat(commaRemovedNumber(EZBaseRate)) * parseFloat(tetherValue) / 100;
                        }
                        else {
                            calculation = parseFloat(commaRemovedNumber(EZTotalRate)) * parseFloat(tetherValue) / 100;
                        }
                    }
                    else {
                        calculation = parseFloat(tetherValue);
                    }
                    //end precalculation
                    //Total value calculation 
                    total = 0;
                    var FinalTotalValue = "";
                    var FinalBaseValue = "";
                    if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                        TetherRT = 0;
                        TetherRT = parseFloat(commaRemovedNumber(EZBaseRate)) + parseFloat(calculation);
                        FinalTotalValue = "";
                        FinalBaseValue = TetherRT.toFixed(2);
                    }
                    else {
                        Tethertotal = parseFloat(commaRemovedNumber(EZTotalRate)) + parseFloat(calculation);
                        FinalTotalValue = Tethertotal.toFixed(2);
                        FinalBaseValue = "";
                    }

                    //Formula Calculation
                    if (TetherformulaTtoB != "" && TetherformulaBtoT != "") {
                        if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                            FinalTotalValue = TetherFormulaCalculation(TetherformulaBtoT);
                        }
                        else {
                            //FinalBaseValue = evaluateFormula(formulaTtoB);
                            FinalBaseValue = TetherFormulaCalculation(TetherformulaTtoB);
                        }
                    }

                    //Global Limit Implementation
                    var displayDate = $("#displayDay li").attr("value");
                    var selectedDate = displayDate.substr(0, 4) + "/" + displayDate.substr(4, 2) + "/" + ('0' + (displayDate.substr(6, displayDate.length - 1))).slice(-2);
                    var carClassID = $(this).find(".carClassLogo").attr("classid");
                    $(GlobalLimitDetails).each(function () {
                        var MinRate = 0, MaxRate = 0;
                        var globalSDate = convertToServerTime(new Date(parseInt(this.StartDate.replace("/Date(", "").replace(")/", ""), 10)));
                        var globalEDate = convertToServerTime(new Date(parseInt(this.EndDate.replace("/Date(", "").replace(")/", ""), 10)));

                        var GlobaleStartDate = globalSDate.getFullYear() + "/" + ((globalSDate.getMonth() + 1).toString().length < 2 ? "0" + globalSDate.getMonth() + 1 : globalSDate.getMonth() + 1) + "/" + (globalSDate.getDate().toString().length < 2 ? "0" + globalSDate.getDate() : globalSDate.getDate());
                        var GlobaleEndDate = globalEDate.getFullYear() + "/" + ((globalEDate.getMonth() + 1).toString().length < 2 ? "0" + globalEDate.getMonth() + 1 : globalEDate.getMonth() + 1) + "/" + (globalEDate.getDate().toString().length < 2 ? "0" + globalEDate.getDate() : globalEDate.getDate());

                        if (new Date(GlobaleStartDate) <= new Date(selectedDate) && new Date(GlobaleEndDate) >= new Date(selectedDate)) {
                            if (carClassID == this.CarClassID) {
                                if (CurrentRentalLength == "D") {
                                    MinRate = ($.isNumeric(this.DayMin)) ? parseFloat(this.DayMin).toFixed(2) : this.DayMin;
                                    MaxRate = ($.isNumeric(this.DayMax)) ? parseFloat(this.DayMax).toFixed(2) : this.DayMax;
                                }
                                if (CurrentRentalLength == "W") {
                                    MinRate = ($.isNumeric(this.WeekMin)) ? parseFloat(this.WeekMin).toFixed(2) : this.WeekMin;
                                    MaxRate = ($.isNumeric(this.WeekMax)) ? parseFloat(this.WeekMax).toFixed(2) : this.WeekMax;
                                }
                                if (CurrentRentalLength == "M") {
                                    MinRate = ($.isNumeric(this.MonthlyMin)) ? parseFloat(this.MonthlyMin).toFixed(2) : this.MonthlyMin;
                                    MaxRate = ($.isNumeric(this.MonthlyMax)) ? parseFloat(this.MonthlyMax).toFixed(2) : this.MonthlyMax;
                                }
                                //console.log("MinRate " + MinRate + " " + FinalBaseValue + "  " + " Max " + MaxRate + " carclass " + carClassID);
                                if (parseFloat(MinRate) > parseFloat(FinalBaseValue) || parseFloat(MaxRate) < parseFloat(FinalBaseValue)) {
                                    isGlobalValueApplied = true;
                                    //console.log(" MAXRATE " + MaxRate + " MINRATE " + MinRate + " EDBASE " + parseFloat(FinalBaseValue) + " EDTOTAL " + EDTotalRate);
                                    if (parseFloat(MaxRate) > parseFloat(FinalBaseValue)) {
                                        FinalBaseValue = MinRate;
                                    }
                                    else {
                                        FinalBaseValue = MaxRate;
                                    }

                                    if (!tetherShopFirstClick) {//Check if user click on second time whatever globallimit flag is set on html
                                        GlobalLimit = true;
                                    }
                                }
                                else {
                                    GlobalLimit = false;
                                }
                                return false;
                            }
                        }
                    });

                    if (isGlobalValueApplied) {
                        TetherRT = FinalBaseValue;
                        if (TetherformulaBtoT != "") {
                            //FinalTotalValue = evaluateFormula(formulaBtoT);
                            FinalTotalValue = TetherFormulaCalculation(TetherformulaBtoT);
                        }
                        isGlobalValueApplied = false;
                    }
                    //End Global Limit Implementation

                    //Case W8-W11
                    if (showAdditionalBase && FinalBaseValue != "") {
                        FinalBaseValue = calculateNewBase(false, FinalBaseValue).toFixed(2);
                    }
                    //If GOV shop then truncate decimal part of Base value
                    if (GlobalLimitSearchSummaryData != null && GlobalLimitSearchSummaryData.IsGOV) {
                        FinalBaseValue = parseInt(FinalBaseValue);
                        FinalBaseValue = FinalBaseValue.toFixed(2);
                    }
                    EDTotalRate = commaSeparateNumber(FinalTotalValue);
                    EDBaseRate = commaSeparateNumber(FinalBaseValue);
                }
                else {
                    EDTotalRate = EZTotalRate;
                    EDBaseRate = EZBaseRate;
                    if (tetherValue == undefined) {
                        tetherValue = null;
                        IsPercentageValue = false;
                    }
                }
                var carClassID = $(this).find(".carClassLogo").attr("classid");

                var item = {}
                item["ID"] = carClassID;
                item["carClass"] = $(this).find(".carClassLogo").attr("alt");
                item["EZBaseRate"] = EZBaseRate;
                item["EZTotalRate"] = EZTotalRate;
                item["EDBaseRate"] = EDBaseRate;
                item["EDTotalRate"] = EDTotalRate;
                item["TetherValue"] = tetherValue;
                item["IsTeatherValueinPercentage"] = IsPercentageValue;
                item["GlobalLimit"] = false;
                if (item["ID"] != undefined) {
                    TetherData.push(item);
                }
            }
        });

        var bindTetherRate = $.map(TetherData, function (item) { return new TetherRateEntity(item); });
        searchViewModel.TetherRateData(bindTetherRate);

    }
    else {
        //This for classic view operation
        TetherDataClassic = [];
        $("#TetherDate").addClass("hidden");
        $("#ClassicCarClass").removeClass("hidden");
        $("#ClassicCarClass").html("<b>Car Class:</b> " + $("#classic_view, #carClass li").eq(0).html());
        $(".tableTetherValueClassic").show();
        $(".tableTetherValueDaily").hide();
        $(".classictable table tbody tr").each(function (index) {
            GlobalLimit = false;
            var EDBaseRate, EDTotalRate, EZTotalRate, EZBaseRate, formatDate;
            if ($(this).find(".totalEdit").val() != undefined && $(this).find(".baseEdit").val() != undefined) {
                EZTotalRate = $.trim($(this).find(".totalEdit").val());
                EZBaseRate = $.trim($(this).find(".baseEdit").val());
                formatDate = $(this).find(".dates").attr('FormatDate');
                if (!$.isNumeric(commaRemovedNumber(EZTotalRate))) {
                    EZTotalRate = "";
                }
                if (!$.isNumeric(commaRemovedNumber(EZBaseRate))) {
                    EZBaseRate = "";
                }
                var calculation;
                var tetherValue;
                var IsPercentageValue;
                if (!tetherShopFirstClick) {
                    $.each(globalTetherSettingsForBrand, function (i, x) {
                        if (x.CarClassID == $("#classic_view, #carClass li").eq(0).val()) {
                            dependantBrandID = x.DependantBrandID;
                            tetherValue = x.TetherValue;
                            IsPercentageValue = x.IsTeatherValueinPercentage;
                            return;
                        }
                    });

                    //for (var x in GlobalTetherSetting) {
                    //    if (GlobalTetherSetting[x].CarClassID == $("#classic_view, #carClass li").eq(0).val() && GlobalTetherSetting[x].LocationID == $("#location li").val()) {
                    //        if (brandID == GlobalTetherSetting[x].DominentBrandID) {
                    //            dependantBrandID = GlobalTetherSetting[x].DependantBrandID; //for check to display tether popup  dependant brand name
                    //            tetherValue = GlobalTetherSetting[x].TetherValue;
                    //            IsPercentageValue = GlobalTetherSetting[x].IsTeatherValueinPercentage;
                    //            temp = GlobalTetherSetting[x].ID;
                    //        }
                    //    }
                    //}
                }
                else {
                    var carClassId = $("#classic_view, #carClass li").eq(0).val();
                    if (carClassId != undefined) {
                        if ($.trim($('#PercentageID_' + carClassId).val()) != "") {
                            IsPercentageValue = true;
                            tetherValue = $.trim($('#PercentageID_' + carClassId).val());
                        } else {
                            IsPercentageValue = false;
                            if ($.trim($('#DollerID_' + carClassId).val()) != "") {
                                tetherValue = $.trim($('#DollerID_' + carClassId).val());
                            }
                            else {
                                tetherValue = 0;
                            }
                        }
                    }
                    GlobalLimit = $("#tableTetherClassicValue tr").eq(index).find("td").eq(3).find("span").attr("globallimit");
                    if (GlobalLimit == undefined && GlobalLimit == null) {
                        GlobalLimit = false;
                    }
                }

                //Used to set empty value for GOV rate.We want to send both brand have same rate only with GOV rate
                if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                    tetherValue = 0;
                    IsPercentageValue = true;
                }

                if (tetherValue != undefined && EZTotalRate != "" && EZBaseRate != "") {
                    //Standard precalculation calculation 
                    if (JSON.parse(IsPercentageValue)) {
                        if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                            calculation = parseFloat(commaRemovedNumber(EZBaseRate)) * parseFloat(tetherValue) / 100;
                        }
                        else {
                            calculation = parseFloat(commaRemovedNumber(EZTotalRate)) * parseFloat(tetherValue) / 100;
                        }
                    }
                    else {
                        calculation = parseFloat(tetherValue);
                    }

                    //end precalculation
                    //Total value calculation 
                    total = 0;
                    var FinalTotalValue = "";
                    var FinalBaseValue = "";
                    if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                        TetherRT = 0;
                        TetherRT = parseFloat(commaRemovedNumber(EZBaseRate)) + parseFloat(calculation);
                        FinalTotalValue = "";
                        FinalBaseValue = TetherRT.toFixed(2);
                    }
                    else {
                        Tethertotal = parseFloat(commaRemovedNumber(EZTotalRate)) + parseFloat(calculation);
                        FinalTotalValue = Tethertotal.toFixed(2);
                        FinalBaseValue = "";
                    }


                    if (TetherformulaTtoB != "" && TetherformulaBtoT != "") {
                        if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                            FinalTotalValue = TetherFormulaCalculation(TetherformulaBtoT);
                        }
                        else {
                            //FinalBaseValue = evaluateFormula(formulaTtoB);
                            FinalBaseValue = TetherFormulaCalculation(TetherformulaTtoB);
                        }
                    }
                    //Global Limit Implementation
                    var displayDate = formatDate;
                    var selectedDate = displayDate.substr(0, 4) + "/" + displayDate.substr(4, 2) + "/" + ('0' + (displayDate.substr(6, displayDate.length - 1))).slice(-2);
                    var carClassID = $("#classic_view, #carClass li").eq(0).val();
                    $(GlobalLimitDetails).each(function () {
                        var MinRate = 0, MaxRate = 0;
                        var globalSDate = convertToServerTime(new Date(parseInt(this.StartDate.replace("/Date(", "").replace(")/", ""), 10)));
                        var globalEDate = convertToServerTime(new Date(parseInt(this.EndDate.replace("/Date(", "").replace(")/", ""), 10)));

                        var GlobaleStartDate = globalSDate.getFullYear() + "/" + ((globalSDate.getMonth() + 1).toString().length < 2 ? "0" + globalSDate.getMonth() + 1 : globalSDate.getMonth() + 1) + "/" + (globalSDate.getDate().toString().length < 2 ? "0" + globalSDate.getDate() : globalSDate.getDate());
                        var GlobaleEndDate = globalEDate.getFullYear() + "/" + ((globalEDate.getMonth() + 1).toString().length < 2 ? "0" + globalEDate.getMonth() + 1 : globalEDate.getMonth() + 1) + "/" + (globalEDate.getDate().toString().length < 2 ? "0" + globalEDate.getDate() : globalEDate.getDate());

                        if (new Date(GlobaleStartDate) <= new Date(selectedDate) && new Date(GlobaleEndDate) >= new Date(selectedDate)) {
                            if (carClassID == this.CarClassID) {
                                if (CurrentRentalLength == "D") {
                                    MinRate = ($.isNumeric(this.DayMin)) ? parseFloat(this.DayMin).toFixed(2) : this.DayMin;
                                    MaxRate = ($.isNumeric(this.DayMax)) ? parseFloat(this.DayMax).toFixed(2) : this.DayMax;
                                }
                                if (CurrentRentalLength == "W") {
                                    MinRate = ($.isNumeric(this.WeekMin)) ? parseFloat(this.WeekMin).toFixed(2) : this.WeekMin;
                                    MaxRate = ($.isNumeric(this.WeekMax)) ? parseFloat(this.WeekMax).toFixed(2) : this.WeekMax;
                                }
                                if (CurrentRentalLength == "M") {
                                    MinRate = ($.isNumeric(this.MonthlyMin)) ? parseFloat(this.MonthlyMin).toFixed(2) : this.MonthlyMin;
                                    MaxRate = ($.isNumeric(this.MonthlyMax)) ? parseFloat(this.MonthlyMax).toFixed(2) : this.MonthlyMax;
                                }
                                if (parseFloat(MinRate) > parseFloat(FinalBaseValue) || parseFloat(MaxRate) < parseFloat(FinalBaseValue)) {
                                    isGlobalValueApplied = true;
                                    if (parseFloat(MaxRate) > parseFloat(FinalBaseValue)) {
                                        FinalBaseValue = MinRate;
                                    }
                                    else {
                                        FinalBaseValue = MaxRate;
                                    }

                                    if (!tetherShopFirstClick) {//Check if user click on second time whatever globallimit flag is set on html
                                        GlobalLimit = true;
                                    }
                                }
                                else {
                                    GlobalLimit = false;
                                }
                                //console.log(this);
                                return false;
                            }
                        }
                    });
                    if (isGlobalValueApplied) {
                        TetherRT = FinalBaseValue;
                        if (TetherformulaBtoT != "") {
                            //FinalTotalValue = evaluateFormula(formulaBtoT);
                            FinalTotalValue = TetherFormulaCalculation(TetherformulaBtoT);
                        }
                        isGlobalValueApplied = false;
                    }


                    //Case W8-W11
                    if (showAdditionalBase && FinalBaseValue != "") {
                        FinalBaseValue = calculateNewBase(false, FinalBaseValue).toFixed(2);
                    }
                    //If GOV shop then truncate decimal part of Base value
                    if (GlobalLimitSearchSummaryData != null && GlobalLimitSearchSummaryData.IsGOV) {
                        FinalBaseValue = parseInt(FinalBaseValue);
                        FinalBaseValue = FinalBaseValue.toFixed(2);
                    }
                    EDTotalRate = commaSeparateNumber(FinalTotalValue);
                    EDBaseRate = commaSeparateNumber(FinalBaseValue);
                }
                else {
                    EDTotalRate = EZTotalRate;
                    EDBaseRate = EZBaseRate;
                    if (tetherValue == undefined) {
                        tetherValue = null;
                        IsPercentageValue = false;
                    }
                }

                var item = {}
                item["ID"] = $("#classic_view, #carClass li").eq(0).val();
                //item["carClass"] = $(this).find(".carClassLogo").attr("alt");
                item["ClassicDates"] = $(this).find(".dates").html().replace("<br>", "");
                item["EZBaseRate"] = EZBaseRate;
                item["EZTotalRate"] = EZTotalRate;
                item["EDBaseRate"] = EDBaseRate;
                item["EDTotalRate"] = EDTotalRate;
                item["TetherValue"] = tetherValue;
                item["IsTeatherValueinPercentage"] = IsPercentageValue;
                item["formatDate"] = formatDate;
                item["GlobalLimit"] = false;
                if (item["ID"] != undefined) {
                    TetherDataClassic.push(item);
                }
            }
            // console.log($(this).find(".dates").html());
        });
        var bindClassicTetherRate = $.map(TetherDataClassic, function (item) { return new TetherRateEntity(item); });
        searchViewModel.TetherRateDataClassic(bindClassicTetherRate);
    }

    //Right Tether Value

    var TetherValueData = [];

    var tetherValue;
    var IsPercentageValue;
    if (!tetherShopFirstClick) {
        FinalTetherValueData = [];
        if ($("#viewSelect").find(".selected").text() == "daily") {
            //Daily View
            $("#daily-rates-table tbody tr").each(function () {
                tetherValue = "";
                IsPercentageValue = false;
                if (!tetherShopFirstClick) {
                    var CarClassID = $(this).find(".carClassLogo").attr("classid");
                    $.each(globalTetherSettingsForBrand, function (i, x) {
                        if (x.CarClassID == CarClassID) {
                            tetherValue = x.TetherValue;
                            IsPercentageValue = x.IsTeatherValueinPercentage;
                            return;
                        }
                    });
                    //for (var x in GlobalTetherSetting) {
                    //    if (GlobalTetherSetting[x].CarClassID == $(this).find(".carClassLogo").attr("classid") && GlobalTetherSetting[x].LocationID == $("#location li").val()) {
                    //        if (brandID == GlobalTetherSetting[x].DominentBrandID) {
                    //            tetherValue = GlobalTetherSetting[x].TetherValue;
                    //            IsPercentageValue = GlobalTetherSetting[x].IsTeatherValueinPercentage;
                    //        }
                    //    }
                    //}
                }
                else {
                    var carClassId = $(this).find(".carClassLogo").attr("classid");
                    $(FinalTetherValueData).each(function () {
                        if (this.ID == carClassID) {
                            tetherValue = this.TetherValue;
                            IsPercentageValue = this.IsTeatherValueinPercentage;
                            return false;
                        }
                    });
                    //if ($('#PercentageID_' + carClassId).val().trim() != "") {
                    //    IsPercentageValue = true;
                    //    tetherValue = $('#PercentageID_' + carClassId).val();
                    //} else {
                    //    IsPercentageValue = false;
                    //    if ($('#DollerID_' + carClassId).val().trim() != "") {
                    //        tetherValue = $('#DollerID_' + carClassId).val();
                    //    }
                    //    else {
                    //        tetherValue = 0;
                    //    }
                    //}
                }

                //Used to set empty value for GOV rate.We want to send both brand have same rate only with GOV rate
                if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                    tetherValue = 0;
                }
                var item = {}
                item["ID"] = $(this).find(".carClassLogo").attr("classid");
                item["carClass"] = $(this).find(".carClassLogo").attr("alt");
                item["TetherValue"] = tetherValue;
                item["IsTeatherValueinPercentage"] = IsPercentageValue;
                item["globallimit"] = "false";
                if (item["ID"] != undefined) {
                    TetherValueData.push(item);
                    FinalTetherValueData.push(item);//For CR from client non close button reset previous button
                }
            });
        }
        else {
            //Classic view
            $("#result-section, #classic_view").find("#carClass ul li").each(function () {
                tetherValue = "";
                IsPercentageValue = false;
                if (!tetherShopFirstClick) {
                    var CarClassID = $(this).val();
                    $.each(globalTetherSettingsForBrand, function (i, x) {
                        if (x.CarClassID == CarClassID) {
                            tetherValue = x.TetherValue;
                            IsPercentageValue = x.IsTeatherValueinPercentage;
                            return;
                        }
                    });
                    //for (var x in GlobalTetherSetting) {
                    //    if (GlobalTetherSetting[x].CarClassID == $(this).val() && GlobalTetherSetting[x].LocationID == $("#location li").val()) {
                    //        if (brandID == GlobalTetherSetting[x].DominentBrandID) {
                    //            tetherValue = GlobalTetherSetting[x].TetherValue;
                    //            IsPercentageValue = GlobalTetherSetting[x].IsTeatherValueinPercentage;
                    //        }
                    //    }
                    //}
                }
                else {
                    var carClassId = $(this).val();
                    $(FinalTetherValueData).each(function () {
                        if (this.ID == carClassID) {
                            tetherValue = this.TetherValue;
                            IsPercentageValue = this.IsTeatherValueinPercentage;
                            return false;
                        }
                    });
                    //if ($('#PercentageID_' + carClassId).val().trim() != "") {
                    //    IsPercentageValue = true;
                    //    tetherValue = $('#PercentageID_' + carClassId).val();
                    //} else {
                    //    IsPercentageValue = false;
                    //    if ($('#DollerID_' + carClassId).val().trim() != "") {
                    //        tetherValue = $('#DollerID_' + carClassId).val();
                    //    }
                    //    else {
                    //        tetherValue = 0;
                    //    }
                    //}
                }

                //Used to set empty value for GOV rate.We want to send both brand have same rate only with GOV rate
                if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                    tetherValue = 0;
                }

                var item = {}
                item["ID"] = $(this).val();
                item["carClass"] = $(this).text();
                item["TetherValue"] = tetherValue;
                item["IsTeatherValueinPercentage"] = IsPercentageValue;
                item["globallimit"] = "false";
                if (item["ID"] != undefined) {
                    TetherValueData.push(item);
                    FinalTetherValueData.push(item);//For CR from client non close button reset previous button
                }
            });
        }
        if (!tetherShopFirstClick) {
            PreloadGlobaltetherData = FinalTetherValueData;
        }
        var bindTetherValue = $.map(TetherValueData, function (item) { return new TetherValueEntity(item); });
        searchViewModel.TetherRateEditableData(bindTetherValue);
        tetherShopFirstClick = true;
    }

    TetherBlurEvent();

    //setTimeout(function () {
    for (var x in GetCompanyBrandList) {
        if (GetCompanyBrandList[x].ID == brandID) {
            $("#leftTetherTitle").html(GetCompanyBrandList[x].Name);
            $("#leftTetherTitle").attr('brandid', GetCompanyBrandList[x].ID);
            $("#leftTetherTitle").attr('brandcode', GetCompanyBrandList[x].Code);
            $("#leftClassicTetherTitle").html(GetCompanyBrandList[x].Name);
            $("#leftClassicTetherTitle").attr('brandid', GetCompanyBrandList[x].ID);
            $("#leftClassicTetherTitle").attr('brandcode', GetCompanyBrandList[x].Code);
        }
        else {
            //Here to check DependantBrandID
            if (GetCompanyBrandList[x].ID == dependantBrandID && dependantBrandID != 0) {
                $("#rightTetherTitle").html(GetCompanyBrandList[x].Name);
                $("#rightTetherTitle").attr('brandid', GetCompanyBrandList[x].ID);
                $("#rightTetherTitle").attr('brandcode', GetCompanyBrandList[x].Code);
                $("#rightClassicTetherTitle").html(GetCompanyBrandList[x].Name);
                $("#rightClassicTetherTitle").attr('brandid', GetCompanyBrandList[x].ID);
                $("#rightClassicTetherTitle").attr('brandcode', GetCompanyBrandList[x].Code);
            }

        }
    };
    // setTimeout(function () { GlobalImplementHightlight(); }, 250);//Global limit implementation check
}
function GlobalImplementHightlight() {
    //This function is used checked after global limit is implemented or not if implemented then higlighted implemented field
    //For Daily View
    if ($("#viewSelect").find(".selected").text() == "daily") {
        $("#tethered_rates .tableTetherValueDaily tbody tr").each(function () {
            var carClassID = $(this).find("td").find(".carClassSelector").attr("carclassid");
            var IsTeatherValueinPercentage = $(this).find("td").find(".carClassSelector").attr("IsTeatherValueinPercentage");
            var GlobalLimit = $(this).find("td").eq(3).find("span").attr("globallimit");
            //console.log("GlobalLimit " + GlobalLimit + " IsTeatherValueinPercentage " + IsTeatherValueinPercentage + " " + carClassID);
            if ((GlobalLimit != undefined && GlobalLimit != null) && (carClassID != undefined && carClassID != null)) {
                if (JSON.parse(GlobalLimit)) {
                    if (JSON.parse(IsTeatherValueinPercentage)) {
                        if ($("#PercentageID_" + carClassID).val().length > 0) {
                            MakeTagFlashable($("#PercentageID_" + carClassID));
                            $("#PercentageID_" + carClassID).attr("globallimit", "true");
                            $(FinalTetherValueData).each(function () {
                                if (this.ID == carClassID) {
                                    this.globallimit = "true";
                                    return false;
                                }
                            });
                        }
                    }
                    else {
                        if ($("#DollerID_" + carClassID).val().length > 0) {
                            MakeTagFlashable($("#DollerID_" + carClassID));
                            $("#DollerID_" + carClassID).attr("globallimit", "true");
                            $(FinalTetherValueData).each(function () {
                                if (this.ID == carClassID) {
                                    this.globallimit = "true";
                                    return false;
                                }
                            });
                        }
                    }
                    //MakeTagFlashable($("#EDTotalRateID_" + carClassID).closest("td"));
                    MakeTagFlashable($("#EDBaseRateID_" + carClassID).closest("td"));
                }
                else {
                    //RemoveFlashableTag($("#EDTotalRateID_" + carClassID).closest("td"));
                    RemoveFlashableTag($("#EDBaseRateID_" + carClassID).closest("td"));
                    RemoveFlashableTag($("#PercentageID_" + carClassID));
                    RemoveFlashableTag($("#DollerID_" + carClassID));
                    $("#PercentageID_" + carClassID).removeAttr("globallimit");
                    $("#DollerID_" + carClassID).removeAttr("globallimit");
                }
            }
        });
    }
    else {

        //For Classic View
        $("#tethered_rates .tableTetherValueClassic tbody tr").each(function () {
            var carClassID = $(this).find("td").find(".carClassSelector").attr("carclassid");
            var IsTeatherValueinPercentage = $(this).find("td").find(".carClassSelector").attr("IsTeatherValueinPercentage");
            var GlobalLimit = $(this).find("td").eq(3).find("span").attr("globallimit");
            //console.log(GlobalLimit + " " + IsTeatherValueinPercentage + " " + index + " " + carClassID);
            if (GlobalLimit != undefined && GlobalLimit != null) {
                if (JSON.parse(GlobalLimit)) {
                    if (JSON.parse(IsTeatherValueinPercentage)) {
                        if ($("#PercentageID_" + carClassID).val().length > 0) {
                            MakeTagFlashable($("#PercentageID_" + carClassID));
                            $("#PercentageID_" + carClassID).attr("globallimit", "true");
                        }
                    }
                    else {
                        if ($("#DollerID_" + carClassID).val().length > 0) {
                            MakeTagFlashable($("#DollerID_" + carClassID));
                            $("#DollerID_" + carClassID).attr("globallimit", "true");
                        }
                    }
                    MakeTagFlashable($(this).find("td").eq(3));
                    // MakeTagFlashable($(this).find("td").eq(4));
                }
                //else {
                //RemoveFlashableTag($(this).find("td").eq(3));
                //RemoveFlashableTag($(this).find("td").eq(4));
                //RemoveFlashableTag($("#PercentageID_" + carClassID));
                //RemoveFlashableTag($("#DollerID_" + carClassID));
                //}

            }
        });
    }
}

//Global Limit implementation in tether popup functions
function GetGlobalLimitForShop() {
    //console.log(GlobalLimitSearchSummaryData);
    //var startDate = convertToServerTime(new Date(parseInt(GlobalLimitSearchSummaryData.StartDate.replace("/Date(", "").replace(")/", ""), 10)));
    //var endDate = convertToServerTime(new Date(parseInt(GlobalLimitSearchSummaryData.EndDate.replace("/Date(", "").replace(")/", ""), 10)));
    //startDate = (startDate.getMonth() < 10 ? '0' : '') + "/" + (startDate.getDate() < 10 ? '0' : '') + "/" + startDate.getFullYear();
    //endDate = (endDate.getMonth() < 10 ? '0' : '') + "/" + (endDate.getDate() < 10 ? '0' : '') + "/" + endDate.getFullYear();
    //console.log(endDate.getDate() + "/" + endDate.getMonth() + "/" + endDate.getFullYear() + "  " + (endDate.getMonth() < 10 ? '0' : '') + "/" + (endDate.getDate() < 10 ? '0' : '') + "/" + endDate.getFullYear());
    //console.log(startDate.getDate() + "/" + startDate.getMonth() + "/" + startDate.getFullYear()+"  "+(startDate.getMonth() < 10 ? '0' : '') + "/" + (startDate.getDate() < 10 ? '0' : '') + "/" + startDate.getFullYear());
    if (!CheckTetherGlobalLimitAjaxCall) {
        var locationID = $('#location ul li.selected').val();//LocationID (LocationBrandID)        
        $.ajax({
            url: 'Search/GetGlobalLimitTetherShop',
            type: 'GET',
            data: { SearchSummaryID: GlobalLimitSearchSummaryData.SearchSummaryID, DependantBrandID: dependantBrandID, LocationID: parseInt(locationID) },
            async: true,
            dataType: 'json',
            success: function (data) {
                GlobalLimitDetails = "";
                GlobalLimitDetails = data;
                CheckTetherGlobalLimitAjaxCall = true;
                //console.log(GlobalLimitDetails);

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                ///console.log(3);
            }
        });
    }
    //console.log(GlobalLimitSearchSummaryData.searchSummary);
}
//End Global limit End..

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

function selectAllLengthCheckboxes() {
    if ($('#lengths-day').prop('checked') && $('#lengths-week').prop('checked')) {
        $('#lengths-all').prop('checked', true);
    }
    else {
        $('#lengths-all').prop('checked', false);
    }
}


function getCurrentFormula() {

    formulaBtoT = formulaTtoB = undefined;

    //get specific formula from db based on selected location 
    var locationID = $('#location ul li.selected').attr('lbid');//LocationID (LocationBrandID) 
    $.ajax({
        url: 'Search/GetFormula',
        type: 'GET',
        data: { locationID: locationID },
        async: true,
        dataType: 'json',
        success: function (data) {
            surcharge = data["Surcharge"];
            sales = data["SalesTax"];
            airport = data["AirportFee"];
            arena = data["Arena"];
            cfc = data["CFC"];
            vlrf = data["VLRF"];
            formulaTtoB = data["TotalCostToBase"];
            formulaBtoT = data["BaseToTotalCost"];
            dominantrentalLength = data["RentalLength"];
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            ///console.log(3);
            formulaTtoB = '';
            formulaBtoT = '';
        }
    });

    //set Length and ND(no of days while user can on change length for used in formula
    setTimeout(function () { getNDandLength($("#length li").html()); }, 250);

};

//function GetAllRentalLengths() {
//    $.ajax({
//        url: 'Search/GetRentalLengths',
//        type: 'GET',
//        async: true,
//        data: {},
//        dataType: 'json',
//        success: function (data) {
//            getRentalLength = data;
//            // console.log(getRentalLength);
//        },
//        error: function (e) {
//            console.log(e.message);
//        }
//    });
//};

function getCurrentFormulaTether() {
    //Check call for specific shop and get call a new while change another shop
    if (!CheckTetherFormulaAjaxCall) {
        TetherformulaBtoT = TetherformulaTtoB = undefined;

        //get specific formula from db based on selected location 
        var locationID = $('#location ul li.selected').val();//LocationID (LocationBrandID)        
        $.ajax({
            url: 'Search/GetFormulaTether',
            type: 'GET',
            data: { locationID: parseInt(locationID), DependantBrandID: parseInt(dependantBrandID) },
            async: true,
            dataType: 'json',
            success: function (data) {
                if (data != null) {
                    Tethersurcharge = data["Surcharge"];
                    TetherSalesTex = data["SalesTax"];
                    TetherAirportFees = data["AirportFee"];
                    TetherArena = data["Arena"];
                    TetherCFC = data["CFC"];
                    TetherVLRF = data["VLRF"];
                    TetherformulaTtoB = data["TotalCostToBase"];
                    TetherformulaBtoT = data["BaseToTotalCost"];
                    CheckTetherFormulaAjaxCall = true;//Remove extra ajax call for every shop filter operation
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                ///console.log(3);
                TetherformulaTtoB = '';
                TetherformulaBtoT = '';
            }
        });
    }
};

function getCompanyBrand() {
    $.ajax({
        url: 'Search/GetCompanyBrands/',
        data: {},
        type: 'GET',
        async: true,
        success: function (data) {
            GetCompanyBrandList = data;
        },
        error: function (e) {
            console.log(e.message);
        }
    });
}


//To set ND an len value of the formula parameter of selected Length 
function getNDandLength(paranlength) {

    //console.log("Call GetNDandLength function");
    //if (dominantrentalLength != undefined && getRentalLength != undefined) {
    //    //  console.log(getRentalLength.length + "  " + dominantrentalLength.length);

    //    var CheckContentOfParanlength = paranlength;

    //    getRentalLength.forEach(function (getRentalLengths) {
    //        if (getRentalLengths.Code.trim() == CheckContentOfParanlength.trim()) {
    //            AssoMapId = getRentalLengths.AssociateMappedId;
    //        }
    //    });
    //    dominantrentalLength.forEach(function (dominantrentalLengths) {
    //        if (dominantrentalLengths.MappedId == AssoMapId) {
    //            getRentalLength.forEach(function (getRentalLengths) {
    //                if (getRentalLengths.AssociateMappedId == AssoMapId) {
    //                    paranlength = getRentalLengths.Code;
    //                    return false;
    //                }
    //            });
    //        }
    //    });
    //}


    var TempLengthFirstChar = paranlength.trim().substr(0, 1);//Used to get first digit of length
    var tempLenghtLastDigit = paranlength.trim().substr(1, paranlength.length);//used to get last digit of length

    if (TempLengthFirstChar == "D") {
        len = tempLenghtLastDigit;
        nd = tempLenghtLastDigit;
    }
    if (TempLengthFirstChar == "W") {
        var TempLastDigit = parseInt(paranlength.trim().substr(1, paranlength.trim().length)); //Get last digit for check in W12-W14 would come then we need len=2
        if (TempLastDigit >= 12 && TempLastDigit <= 14) {
            len = formulaWeekLengthW12toW14;
        }
        else {
            len = formulaWeekLength;
        }
        nd = tempLenghtLastDigit;
    }
    if (TempLengthFirstChar == "M") {
        len = formulaMonthLength;
        nd = tempLenghtLastDigit;
    }
}

function getGlobalTetherValue() {
    $.ajax({
        url: 'Search/GetGlobalTetherSettings/',
        type: 'GET',
        async: true,
        success: function (data) {
            if (data) {
                GlobalTetherSetting = data;
                //CheckTetherValueDiableButton method is called before getting this response then call again
                if (GlobalTetherSettingUndefined) {
                    GlobalTetherSettingUndefined = false;
                    CheckTetherValueDiableButton();
                }
            }
        },
        error: function (e) {
            console.log(e.message);
        }
    });
}

function showTetherRates() {
    $('#tethered_rates').show();
}
//validation of tethershop popup
function validateTetherShop() {
    var flag = false;
    $('#TehterTableValue input[type="text"]').each(function () {
        var globalLimit = $(this).attr("globallimit");

        if ($.trim($(this).val()) != "" && !$.isNumeric($.trim($(this).val()))) {
            flag = false;
            MakeTagFlashable($(this));
        } else {
            flag = true;
            RemoveFlashableTag($(this));
            if (globalLimit != undefined && globalLimit != null) {
                if ($.trim($(this).val()) != "" && $.isNumeric($.trim($(this).val()))) {
                    MakeTagFlashable($(this));
                }
                else {
                    $(this).removeAttr("globallimit");
                }
            }
            else {
                RemoveFlashableTag($(this));
            }
        }
    });
    $('#TehterTableValue tr').each(function () {
        if ($.trim($(this).find('input[type="text"]').eq(0).val()) != "" && $.trim($(this).find('input[type="text"]').eq(1).val()) != "") {
            flag = false;
            MakeTagFlashable($(this).find('input[type="text"]'));
        }
        else {
            if (!$(this).find('input[type="text"]').hasClass('temp')) {
                RemoveFlashableTag($(this));
            }
        }
    });
    if ($("#tethered_rates #TehterTableValue tbody tr").find(".temp").length > 0) {
        flag = false;
    }
    else {
        flag = true;
    }
    AddFlashingEffect();
    return flag;
}
//Used generic formula operation
//For tether calculation
function TetherFormulaCalculation(formula) {
    var formulaCalculation = "";
    if (formula != undefined && formula != null) {
        //If GOV shop then change total value
        if (GlobalLimitSearchSummaryData != null && GlobalLimitSearchSummaryData.IsGOV) {
            var selectedLor = $("#result-section #length ul li.selected").eq(0).val();
            if (typeof (selectedLor) != 'undefined' && selectedLor > 0) {
                Tethertotal = GetGovernmentRate(Tethertotal, selectedLor, true);
            }
        }
        var tempFormulaCalculation = formula;
        tempFormulaCalculation = tempFormulaCalculation.replace(/airport/g, TetherAirportFees);
        tempFormulaCalculation = tempFormulaCalculation.replace(/sales/g, TetherSalesTex);
        tempFormulaCalculation = tempFormulaCalculation.replace(/arena/g, TetherArena);
        tempFormulaCalculation = tempFormulaCalculation.replace(/cfc/g, TetherCFC);
        tempFormulaCalculation = tempFormulaCalculation.replace(/vlrf/g, TetherVLRF);
        tempFormulaCalculation = tempFormulaCalculation.replace(/nd/g, nd);
        tempFormulaCalculation = tempFormulaCalculation.replace(/len/g, len);
        tempFormulaCalculation = tempFormulaCalculation.replace(/surcharge/g, Tethersurcharge);
        tempFormulaCalculation = tempFormulaCalculation.replace(/rt/g, TetherRT);
        tempFormulaCalculation = tempFormulaCalculation.replace(/total/g, Tethertotal.toFixed(2));
        //incase wrong eneter formula or execute formula fail.
        try {
            formulaCalculation = eval(tempFormulaCalculation);//Caculate the fromula meths opertaion
            return formulaCalculation.toFixed(2);
        }
        catch (error) {
            return formulaCalculation = "";
        }
    }
    return formulaCalculation;
}
function CalculationOfFormula(selectedID, sign) {
    var splitID = selectedID.split('_');//Get the unique ID
    var isGlobalValueApplied = false;//used for global limit implementation

    //for check if incase some developer make problem of id creation that case
    if (splitID.length >= 2) {
        var carClassID = splitID[1];
        if ($("#viewSelect").find(".selected").text() == "daily") {
            var EZTotalRate = $("#hiddenEZTotalRateID_" + carClassID).val();
            var EZBaseRate = $("#hiddenEZBaseRateID_" + carClassID).val();
            var EDTotalRate = $("#hiddenEDTotalRateID_" + carClassID).val();
            var EDBaseRate = $("#hiddenEDBaseRateID_" + carClassID).val();
            var selectedTetherValue = $('#' + selectedID).val();//Get the selected value
            var calculation;
            if (selectedTetherValue.trim() != "" && EZTotalRate != "" && EDTotalRate != "") {
                if (sign == "%") {
                    if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                        calculation = parseFloat(commaRemovedNumber(EZBaseRate)) * parseFloat(selectedTetherValue) / 100;
                    }
                    else {
                        calculation = parseFloat(commaRemovedNumber(EZTotalRate)) * parseFloat(selectedTetherValue) / 100;
                    }
                    // calculation = parseFloat(commaRemovedNumber(EZTotalRate)) * parseFloat(selectedTetherValue) / 100;
                }
                else {
                    calculation = parseFloat(selectedTetherValue);
                }
                //check minus sign

                //Total value calculation 
                total = 0;
                var FinalTotalValue = "";
                var FinalBaseValue = "";

                if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                    TetherRT = 0;
                    TetherRT = parseFloat(commaRemovedNumber(EDBaseRate)) + parseFloat(calculation);
                    FinalTotalValue = "";
                    FinalBaseValue = TetherRT.toFixed(2);
                }
                else {
                    Tethertotal = parseFloat(commaRemovedNumber(EDTotalRate)) + parseFloat(calculation);
                    FinalTotalValue = Tethertotal.toFixed(2);
                    FinalBaseValue = "";
                }

                //if (selectedTetherValue.indexOf('-') != '-') {
                //    total = parseFloat(commaRemovedNumber(EDTotalRate)) + parseFloat(calculation);
                //}
                //else {
                //    total = parseFloat(commaRemovedNumber(EDTotalRate)) - parseFloat(calculation);
                //}
                //Tethertotal = total;
                //var FinalTotalValue = total.toFixed(2);
                //var FinalBaseValue = "";
                //Formula Calculation
                if (TetherformulaTtoB != "" && TetherformulaBtoT != "") {
                    if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                        FinalTotalValue = TetherFormulaCalculation(TetherformulaBtoT);
                    }
                    else {
                        //FinalBaseValue = evaluateFormula(formulaTtoB);
                        FinalBaseValue = TetherFormulaCalculation(TetherformulaTtoB);
                    }
                }

                //Global Limit Implementation
                var displayDate = $("#displayDay li").attr("value");
                var selectedDate = displayDate.substr(0, 4) + "/" + displayDate.substr(4, 2) + "/" + ('0' + (displayDate.substr(6, displayDate.length - 1))).slice(-2);

                $(GlobalLimitDetails).each(function () {
                    var MinRate = 0, MaxRate = 0;
                    var globalSDate = convertToServerTime(new Date(parseInt(this.StartDate.replace("/Date(", "").replace(")/", ""), 10)));
                    var globalEDate = convertToServerTime(new Date(parseInt(this.EndDate.replace("/Date(", "").replace(")/", ""), 10)));

                    var GlobaleStartDate = globalSDate.getFullYear() + "/" + ((globalSDate.getMonth() + 1).toString().length < 2 ? "0" + (globalSDate.getMonth() + 1) : globalSDate.getMonth() + 1) + "/" + (globalSDate.getDate().toString().length < 2 ? "0" + globalSDate.getDate() : globalSDate.getDate());
                    var GlobaleEndDate = globalEDate.getFullYear() + "/" + ((globalEDate.getMonth() + 1).toString().length < 2 ? "0" + (globalEDate.getMonth() + 1) : globalEDate.getMonth() + 1) + "/" + (globalEDate.getDate().toString().length < 2 ? "0" + globalEDate.getDate() : globalEDate.getDate());

                    if (new Date(GlobaleStartDate) <= new Date(selectedDate) && new Date(GlobaleEndDate) >= new Date(selectedDate)) {
                        if (carClassID == this.CarClassID) {
                            if (CurrentRentalLength == "D") {
                                MinRate = ($.isNumeric(this.DayMin)) ? parseFloat(this.DayMin).toFixed(2) : this.DayMin;
                                MaxRate = ($.isNumeric(this.DayMax)) ? parseFloat(this.DayMax).toFixed(2) : this.DayMax;
                            }
                            if (CurrentRentalLength == "W") {
                                MinRate = ($.isNumeric(this.WeekMin)) ? parseFloat(this.WeekMin).toFixed(2) : this.WeekMin;
                                MaxRate = ($.isNumeric(this.WeekMax)) ? parseFloat(this.WeekMax).toFixed(2) : this.WeekMax;
                            }
                            if (CurrentRentalLength == "M") {
                                MinRate = ($.isNumeric(this.MonthlyMin)) ? parseFloat(this.MonthlyMin).toFixed(2) : this.MonthlyMin;
                                MaxRate = ($.isNumeric(this.MonthlyMax)) ? parseFloat(this.MonthlyMax).toFixed(2) : this.MonthlyMax;
                            }
                            //console.log("MinRate " + MinRate + " " + FinalBaseValue + "  " + " Max " + MaxRate + " carclass " + carClassID);
                            if (parseFloat(MinRate) > parseFloat(FinalBaseValue) || parseFloat(MaxRate) < parseFloat(FinalBaseValue)) {
                                isGlobalValueApplied = true;
                                //console.log(" MAXRATE " + MaxRate + " MINRATE " + MinRate + " EDBASE " + parseFloat(FinalBaseValue) + " EDTOTAL " + EDTotalRate);
                                //if (parseFloat(MaxRate) >= parseFloat(FinalBaseValue)) {
                                //    FinalBaseValue = MinRate;
                                //}
                                //else {
                                //    FinalBaseValue = MaxRate;
                                //}
                                //MakeTagFlashable($("#EDTotalRateID_" + carClassID).closest("td"));
                                MakeTagFlashable($("#EDBaseRateID_" + carClassID).closest("td"));
                                $("#EDBaseRateID_" + carClassID).attr("globallimit", "true");
                                if (sign == "%") {
                                    MakeTagFlashable($("#PercentageID_" + carClassID));
                                    $("#PercentageID_" + carClassID).attr("globallimit", "true");
                                }
                                else {
                                    MakeTagFlashable($("#DollerID_" + carClassID));
                                    $("#DollerID_" + carClassID).attr("globallimit", "true");
                                }
                            }
                            else {
                                //RemoveFlashableTag($("#EDTotalRateID_" + carClassID).closest("td"));
                                RemoveFlashableTag($("#EDBaseRateID_" + carClassID).closest("td"));
                                RemoveFlashableTag($("#PercentageID_" + carClassID));
                                RemoveFlashableTag($("#DollerID_" + carClassID));
                                $("#PercentageID_" + carClassID).removeAttr("globallimit");
                                $("#DollerID_" + carClassID).removeAttr("globallimit");
                                $("#EDBaseRateID_" + carClassID).attr("globallimit", "false");
                            }
                            return false;
                        }
                    }
                });

                if (isGlobalValueApplied) {
                    //TetherRT = FinalBaseValue;
                    //if (TetherformulaBtoT != "") {
                    //    //FinalTotalValue = evaluateFormula(formulaBtoT);
                    //    FinalTotalValue = TetherFormulaCalculation(TetherformulaBtoT);
                    //}
                    isGlobalValueApplied = false;
                }
                //End Global Limit Implementation


                if (showAdditionalBase && FinalBaseValue != "") {
                    FinalBaseValue = calculateNewBase(false, FinalBaseValue).toFixed(2);
                }
                //If GOV shop then truncate decimal part of Base value
                if (GlobalLimitSearchSummaryData != null && GlobalLimitSearchSummaryData.IsGOV) {
                    FinalBaseValue = parseInt(FinalBaseValue);
                    FinalBaseValue = FinalBaseValue.toFixed(2);
                }
                $("#EDTotalRateID_" + carClassID).html(commaSeparateNumber(FinalTotalValue));
                $('#EDBaseRateID_' + carClassID).html(commaSeparateNumber(FinalBaseValue));//bind to base rate of comparator
            }
            else {
                $('#EDBaseRateID_' + carClassID).html($("#hiddenEDBaseRateID_" + carClassID).val());
                $("#EDTotalRateID_" + carClassID).html($("#hiddenEDTotalRateID_" + carClassID).val());
                var checkGlobalLimit = $('#EDBaseRateID_' + carClassID).attr("globallimit");
                if (checkGlobalLimit && checkGlobalLimit != undefined) {
                    $('#EDBaseRateID_' + carClassID).attr("globallimit", "false");
                }
                //RemoveFlashableTag($("#EDTotalRateID_" + carClassID).closest("td"));
                RemoveFlashableTag($("#EDBaseRateID_" + carClassID).closest("td"));
                RemoveFlashableTag($("#PercentageID_" + carClassID));
                RemoveFlashableTag($("#DollerID_" + carClassID));
            }
        }
        else {
            //This condition is use for Classic tether popup
            $("#tableTetherClassicValue tr").each(function (index) {
                var ID = $(this).find('[id^="EZBaseRateID_"]').attr("ID").split('_')[1];
                if (ID == carClassID) {
                    var EZTotalRate = $(this).find('[id^="hiddenEZTotalRateID_"]').val();
                    var EZBaseRate = $(this).find('[id^="hiddenEZBaseRateID_"]').val();
                    var EDTotalRate = $(this).find('[id^="hiddenEDTotalRateID_"]').val();
                    var EDBaseRate = $(this).find('[id^="hiddenEDBaseRateID_"]').val();
                    var selectedTetherValue = $('#' + selectedID).val();//Get the selected value
                    //console.log(EZTotalRate + "  " + EDTotalRate + " " + selectedTetherValue);
                    var calculation;
                    var $rightBaseRate = $(this).find("td").eq(3);
                    var $rightTotalRate = $(this).find("td").eq(4);
                    var $percentageCarClass = $("#PercentageID_" + carClassID);
                    var $dollerCarClass = $("#DollerID_" + carClassID);

                    if (selectedTetherValue.trim() != "" && EZTotalRate != "" && EDTotalRate != "") {
                        if (sign == "%") {
                            if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                                calculation = parseFloat(commaRemovedNumber(EZBaseRate)) * parseFloat(selectedTetherValue) / 100;
                            }
                            else {
                                calculation = parseFloat(commaRemovedNumber(EZTotalRate)) * parseFloat(selectedTetherValue) / 100;
                            }
                        }
                        else {
                            calculation = parseFloat(selectedTetherValue);
                        }
                        total = 0;
                        var FinalTotalValue = "";
                        var FinalBaseValue = "";

                        if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                            TetherRT = 0;
                            TetherRT = parseFloat(commaRemovedNumber(EDBaseRate)) + parseFloat(calculation);
                            FinalTotalValue = "";
                            FinalBaseValue = TetherRT.toFixed(2);
                        }
                        else {
                            Tethertotal = parseFloat(commaRemovedNumber(EDTotalRate)) + parseFloat(calculation);
                            FinalTotalValue = Tethertotal.toFixed(2);
                            FinalBaseValue = "";
                        }
                        if (TetherformulaTtoB != "" && TetherformulaBtoT != "") {
                            if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                                FinalTotalValue = TetherFormulaCalculation(TetherformulaBtoT);
                            }
                            else {
                                //FinalBaseValue = evaluateFormula(formulaTtoB);
                                FinalBaseValue = TetherFormulaCalculation(TetherformulaTtoB);
                            }
                        }
                        //check minus sign
                        //if (selectedTetherValue.indexOf('-') != '-') {
                        //    total = parseFloat(commaRemovedNumber(EDTotalRate)) + parseFloat(calculation);
                        //}
                        //else {
                        //    total = parseFloat(commaRemovedNumber(EDTotalRate)) - parseFloat(calculation);
                        //}
                        //Tethertotal = total;
                        //var FinalTotalValue = total.toFixed(2);
                        //var FinalBaseValue = "";
                        //if (TetherformulaTtoB != "") {
                        //    //FinalBaseValue = evaluateFormula(formulaTtoB);
                        //    FinalBaseValue = TetherFormulaCalculation(TetherformulaTtoB);

                        //}
                        //Global Limit Implementation
                        var displayDate = $(this).attr("formatdate");
                        var selectedDate = displayDate.substr(0, 4) + "/" + displayDate.substr(4, 2) + "/" + ('0' + (displayDate.substr(6, displayDate.length - 1))).slice(-2);

                        $(GlobalLimitDetails).each(function () {
                            var MinRate = 0, MaxRate = 0;
                            var globalSDate = convertToServerTime(new Date(parseInt(this.StartDate.replace("/Date(", "").replace(")/", ""), 10)));
                            var globalEDate = convertToServerTime(new Date(parseInt(this.EndDate.replace("/Date(", "").replace(")/", ""), 10)));

                            var GlobaleStartDate = globalSDate.getFullYear() + "/" + ((globalSDate.getMonth() + 1).toString().length < 2 ? "0" + (globalSDate.getMonth() + 1) : globalSDate.getMonth() + 1) + "/" + (globalSDate.getDate().toString().length < 2 ? "0" + globalSDate.getDate() : globalSDate.getDate());
                            var GlobaleEndDate = globalEDate.getFullYear() + "/" + ((globalEDate.getMonth() + 1).toString().length < 2 ? "0" + (globalEDate.getMonth() + 1) : globalEDate.getMonth() + 1) + "/" + (globalEDate.getDate().toString().length < 2 ? "0" + globalEDate.getDate() : globalEDate.getDate());

                            if (new Date(GlobaleStartDate) <= new Date(selectedDate) && new Date(GlobaleEndDate) >= new Date(selectedDate)) {
                                if (carClassID == this.CarClassID) {
                                    if (CurrentRentalLength == "D") {
                                        MinRate = ($.isNumeric(this.DayMin)) ? parseFloat(this.DayMin).toFixed(2) : this.DayMin;
                                        MaxRate = ($.isNumeric(this.DayMax)) ? parseFloat(this.DayMax).toFixed(2) : this.DayMax;
                                    }
                                    if (CurrentRentalLength == "W") {
                                        MinRate = ($.isNumeric(this.WeekMin)) ? parseFloat(this.WeekMin).toFixed(2) : this.WeekMin;
                                        MaxRate = ($.isNumeric(this.WeekMax)) ? parseFloat(this.WeekMax).toFixed(2) : this.WeekMax;
                                    }
                                    if (CurrentRentalLength == "M") {
                                        MinRate = ($.isNumeric(this.MonthlyMin)) ? parseFloat(this.MonthlyMin).toFixed(2) : this.MonthlyMin;
                                        MaxRate = ($.isNumeric(this.MonthlyMax)) ? parseFloat(this.MonthlyMax).toFixed(2) : this.MonthlyMax;
                                    }
                                    if (parseFloat(MinRate) > parseFloat(FinalBaseValue) || parseFloat(MaxRate) < parseFloat(FinalBaseValue)) {
                                        isGlobalValueApplied = true;
                                        //console.log(" MAXRATE " + MaxRate + " MINRATE " + MinRate + " EDBASE " + parseFloat(FinalBaseValue) + " EDTOTAL " + EDTotalRate);
                                        //if (parseFloat(MaxRate) >= parseFloat(FinalBaseValue)) {
                                        //    FinalBaseValue = MinRate;
                                        //}
                                        //else {
                                        //    FinalBaseValue = MaxRate;
                                        //}
                                        //MakeTagFlashable($rightTotalRate);
                                        MakeTagFlashable($rightBaseRate);
                                        $($rightBaseRate).find("span").attr("globallimit", "true");
                                        if (sign == "%") {
                                            MakeTagFlashable($percentageCarClass);
                                            $($percentageCarClass).attr("globallimit", "true");
                                        }
                                        else {
                                            MakeTagFlashable($dollerCarClass);
                                            $($dollerCarClass).attr("globallimit", "true");
                                        }
                                    }
                                    else {
                                        RemoveFlashableTag($rightBaseRate);
                                        //RemoveFlashableTag($rightTotalRate);
                                        RemoveFlashableTag($percentageCarClass);
                                        RemoveFlashableTag($dollerCarClass);
                                        $($percentageCarClass).removeAttr("globallimit");
                                        $($dollerCarClass).removeAttr("globallimit");
                                        $($rightBaseRate).find("span").attr("globallimit", "false");
                                    }
                                    return false;
                                }
                            }
                        });
                        if (isGlobalValueApplied) {
                            //TetherRT = FinalBaseValue;
                            //if (TetherformulaBtoT != "") {
                            //    //FinalTotalValue = evaluateFormula(formulaBtoT);
                            //    FinalTotalValue = TetherFormulaCalculation(TetherformulaBtoT);
                            //}
                            isGlobalValueApplied = false;
                        }

                        //End global limit implementation

                        //Case W8-W11
                        if (showAdditionalBase && FinalBaseValue != "") {
                            FinalBaseValue = calculateNewBase(false, FinalBaseValue).toFixed(2);
                        }
                        //If GOV shop then truncate decimal part of Base value
                        if (GlobalLimitSearchSummaryData != null && GlobalLimitSearchSummaryData.IsGOV) {
                            FinalBaseValue = parseInt(FinalBaseValue);
                            FinalBaseValue = FinalBaseValue.toFixed(2);
                        }
                        $(this).find('[id^="EDTotalRateID_"]').html(commaSeparateNumber(FinalTotalValue));
                        $(this).find('[id^="EDBaseRateID_"]').html(commaSeparateNumber(FinalBaseValue));//bind to base rate of comparator
                    }
                    else {
                        $(this).find('[id^="EDBaseRateID_"]').html($(this).find('[id^="hiddenEDBaseRateID_"]').val());
                        $(this).find('[id^="EDTotalRateID_"]').html($(this).find('[id^="hiddenEDTotalRateID_"]').val());

                        var checkGlobalLimit = $(this).find('[id^="EDBaseRateID_"]').attr("globallimit");
                        if (checkGlobalLimit && checkGlobalLimit != undefined) {
                            $(this).find('[id^="EDBaseRateID_"]').attr("globallimit", "false");
                        }
                        //RemoveFlashableTag($rightTotalRate);
                        RemoveFlashableTag($rightBaseRate);
                        RemoveFlashableTag($percentageCarClass);
                        RemoveFlashableTag($dollerCarClass);
                    }
                }
            })
        }
    }
}

function evaluateFormula(formula, returnTruncated) {
    //assign variable 'total' for total rate to base conversion before execution of this method
    //assign variable 'rt' for base rate to total conversion before execution of this method
    if (formula == '' || formula == null) {
        return 0;
    }
    if (returnTruncated != null && returnTruncated == true) {
        return eval(formula);
    }
    else {
        return eval(formula).toFixed(2);
    }

}

function TetherCopyAllData() {
    $("#PercentageID_All").keyup(function () {
        $("#hiddenCurrentTxtName").val($(this).attr("id"));
        //console.log("percentage call");
    });
    $("#DollerID_All").keyup(function () {
        $("#hiddenCurrentTxtName").val($(this).attr("id"));
        //console.log("doller call");
    });
    $("#copytoall").click(function () {
        var GlobalLimitApplied = false;
        $("#TehterTableValue input[type=text]").each(function () {
            if ($(this).attr("globallimit") != undefined && $(this).attr("globallimit") != null) {
                GlobalLimitApplied = true;
                return false;
            }
            else {
                GlobalLimitApplied = false;
            }
        });
        if (GlobalLimitApplied || $("#TehterTableValue").find(".temp").length == 0) {
            var AllCarClassPercentage = $("#PercentageID_All").val();
            var AllCarClassDollar = $("#DollerID_All").val();
            if ($.trim(AllCarClassPercentage) != "" && $.trim(AllCarClassDollar) != "") {
                validateTetherShop();
            }
            else {
                if (AllCarClassPercentage.trim() != "") {
                    $("input[name=percentageValue]").each(function () {
                        $("#" + $(this).attr("ID")).val(AllCarClassPercentage);
                        CalculationOfFormula($(this).attr("ID"), "%");
                        $("input[name=dollerValue]").each(function () {
                            $("#" + $(this).attr("ID")).val("");
                        });
                    });
                    $("#DollerID_All").val("");
                }
                if (AllCarClassDollar.trim() != "") {
                    $("input[name=dollerValue]").each(function () {
                        $("#" + $(this).attr("ID")).val(AllCarClassDollar);
                        CalculationOfFormula($(this).attr("ID"), "$");
                        $("input[name=percentageValue]").each(function () {
                            $("#" + $(this).attr("ID")).val("");
                        });
                    });
                    $("#PercentageID_All").val("");
                }
            }
            validateTetherShop();
        }
    });
}

function TetherValueEntity(data) {
    this.ID = data.ID;
    this.carClass = data.carClass;
    //this.IsTeatherValueinPercentage = data.IsTeatherValueinPercentage;
    this.DollarValue = ko.computed(function () {
        if (JSON.parse(data.IsTeatherValueinPercentage)) {
            return "";
        }
        else {
            return data.TetherValue;
        }
    });
    this.PercentageValue = ko.computed(function () {
        if (JSON.parse(data.IsTeatherValueinPercentage)) {
            return data.TetherValue;
        }
        else {
            return "";
        }
    });
}
function TetherRateEntity(data) {

    var self = this;
    self.ID = data.ID;
    self.carClass = data.carClass;
    self.ClassicDates = data.ClassicDates;
    self.EZBaseRate = data.EZBaseRate;
    self.EZTotalRate = data.EZTotalRate;
    self.EDBaseRate = data.EDBaseRate;
    self.EDTotalRate = data.EDTotalRate;
    self.DollarValue = ko.computed(function () {
        if (JSON.parse(data.IsTeatherValueinPercentage)) {
            return "";
        }
        else {
            return data.TetherValue;
        }
    });
    self.PercentageValue = ko.computed(function () {
        if (JSON.parse(data.IsTeatherValueinPercentage)) {
            return data.TetherValue;
        }
        else {
            return "";
        }
    });
    self.GlobalLimit = ko.computed(function () {
        if (JSON.parse(data.GlobalLimit)) {
            return "true";
        }
        else {
            return "false";
        }
    });
    self.IsTeatherValueinPercentage = ko.computed(function () {
        if (JSON.parse(data.IsTeatherValueinPercentage)) {
            return "true";
        }
        else {
            return "false";
        }
    })
    self.DateFormat = data.formatDate;
}

function ActualTetherEntity(data) {
    var self = this;
    self.ID = data.ID;
    self.Location = data.Location;
    self.carClass = data.carClass;
    self.TetherValue = data.TetherValue;
    self.IsValuePercentage = data.IsValuePercentage;
}

function BindLocations() {
    $('.loader_container_location').show();
    $.ajax({
        url: 'Search/GetLocations/',
        data: { UserId: LoggedInUserID },
        type: 'GET',
        async: true,
        success: function (data) {
            $('.loader_container_location').hide();
            if (data) {

                var srcs = $.map(data, function (item) { return new locations(item); });
                smartSearchLocations = data;
                searchViewModel.locations(srcs);
                $("#recentLocations  ul li").eq(0).addClass("selected");

                //Check if search panel all control bind after call search Summary
                CheckBindLocation = true;
                //CheckRecentSearchSummaryAjax();
            }
        },
        error: function (e) {
            $('.loader_container_location').hide();
            console.log(e.message);
        }
    });
}

function GetLocationSpecificCarClasses(locationBrandId) {
    ResetLORs();
    DisableLORs();
    var searchId = locationBrandId.value, $selectedOpt = $('option:selected', locationBrandId);
    searchId = $selectedOpt.attr('brandId');
    if (searchId != null && searchId != '') {
        $('.loader_container_carclass').show();
        $.ajax({
            url: 'Search/GetLocationCarClasses/',
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
                }
            },
            error: function (e) {
                console.log("GetLocationSpecificCarClasses: " + e.message);
            }
        });
    }
}
function ResetLORs() {
    //$('#rentalLengthtd select option').each(function () {
    //    $(this).prop({'disabled': false });
    //});
    $('#rentalLengthtd select option:disabled').prop('disabled', false);
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
                //$("#lengths select option[value=" + val + "]").prop('selected', false);
            })
        }
    }
}
function BindCarClasses() {
    $('.loader_container_carclass').show();
    $.ajax({
        url: 'Search/GetCarClasses/',
        type: 'GET',
        async: true,
        success: function (data) {
            $('.loader_container_carclass').hide();
            if (data) {
                var srcs = $.map(data, function (item) { return new carClasses(item); });
                searchViewModel.carClasses(srcs);
                $('#carClasstd select option').bind('click', function () {
                    if ($('#carClasstd').find('option:selected').length == $('#carClasstd select option').length) {
                        $('#carClass-all').prop('checked', true);
                    }
                    else {
                        $('#carClass-all').prop('checked', false);
                    }

                });

                //Check if search panel all control bind after call search Summary
                CheckBindCarClass = true;
                //CheckRecentSearchSummaryAjax();
            }
        },
        error: function (e) {
            $('.loader_container_carclass').hide();
            console.log(e.message);
        }
    });
}

//function BindUsers() {
//    $.ajax({
//        url: 'Search/GetSearchSummaryUserList/',
//        type: 'GET',
//        data: {},
//        dataType: 'json',
//        async: true,
//        success: function (data) {
//            var srcs = $.map(data, function (item) { return new Users(item); });
//            searchViewModel.Users(srcs);
//            $("#recentUsers ul li").eq(0).addClass("selected");
//        },
//        error: function (XMLHttpRequest, textStatus, errorThrown) {
//            console.log(errorThrown);
//        }
//    });
//}

function SourceModel() {
    var self = this;
    self.locations = ko.observableArray([]);
    self.carClasses = ko.observableArray([]);
    self.TetherRateData = ko.observableArray([]);
    self.TetherRateDataClassic = ko.observableArray([]);
    self.TetherRateEditableData = ko.observableArray([]);
    self.ActualTetherData = ko.observableArray([]);
    //self.Users = ko.observableArray([]);
    self.Status = ko.observableArray([]);
    self.headers = ko.observableArray([]);
    self.rates = ko.observableArray([]);
    self.SearchSummary = ko.observableArray([]);
    self.RuleSetCompanies = ko.observableArray([]);
    self.RuleSetGroups = ko.observableArray([]);
    self.competitors = ko.observableArray([]);
    self.trackingCarClasses = ko.observableArray([]);

    //Classic View
    self.ratesClassic = ko.observableArray([]);

    //Quickview
    self.lengthDateCombinationChanged = ko.observableArray([]);
    self.lengthDateCombinationUnChanged = ko.observableArray([]);
    //Methods
    self.RerunQuickView = function (quickViewRow) { RerunQuickView(quickViewRow); }
    self.EditQuickView = function (quickViewRow) { RescheduleQuickView(quickViewRow); }
    self.DeleteQuickView = function (quickViewRow) {
        var message = "This will delete existing quick view shop if any. Are you sure?";
        ShowConfirmBox(message, true, DeleteQuickView, quickViewRow);
    }
    self.ShowQuickDailyView = function (quickViewRow) { ShowQuickDailyView(quickViewRow); }
    self.ShowQuickViewReport = function (quickViewRow) { ShowQuickViewReport(quickViewRow); }
    self.SortQuickView = function () {
        SortQuickView();
    };
    self.quickViewData = ko.observableArray([]);
    self.quickViewRentalLengths = ko.observableArray([]);
    self.quickViewReportData = ko.observableArray([]);
    self.quickViewGroupData = ko.observableArray([]);

    //update all
    self.allCarClasses = ko.observableArray([]);
    self.SearchSummary.subscribe(function (changes) {
    })

    self.SearchSummary.removeSearch = function () {
        self.SearchSummary.remove(function (search) {
            return $.trim(search.StatusIDs).toString() == '6';
        })
    }
    self.OpaqueRateData = ko.observableArray([]);
    //self.ClassicOpaqueRateData = ko.observableArray([]);
    self.ApplicableRateCodes = ko.observableArray([]);
}

//function Users(data) {
//    var self = this;
//    self.ID = data.ID;
//    self.FirstName = data.FirstName;
//    self.LastName = data.LastName;
//    self.fullName = data.FirstName + " " + data.LastName;
//};

function locations(data) {
    this.LocationID = data.LocationID;
    this.Location = data.LocationBrandAlias;
    this.LocationBrandID = data.ID;
    this.LocationCode = data.LocationCode;
    this.LOR = data.LOR;
}

function carClasses(data) {
    this.ID = data.ID;
    this.Code = data.Code;
}

function SearchRateCodeModel(data) {
    this.Code = data.Code;
    this.ApplicableDates = ko.observableArray($.map(data.DateRangeList, function (item) { return new RateCodeDateModel(item); }));
}

function RateCodeDateModel(data) {
    this.StartEndDateRange = ko.computed(function () {
        var startDate = convertToServerTime(new Date(parseInt(data.StartDate.replace("/Date(", "").replace(")/", ""))));
        var endDate = convertToServerTime(new Date(parseInt(data.EndDate.replace("/Date(", "").replace(")/", ""))));
        return startDate.getFullYear() + "/" + ('0' + (startDate.getMonth() + 1)).slice(-2) + "/" + ('0' + (startDate.getDate())).slice(-2) + " through " + endDate.getFullYear() + "/" + ('0' + (endDate.getMonth() + 1)).slice(-2) + "/" + ('0' + (endDate.getDate())).slice(-2);
    });
}

function searchGrid(inpuText, controlId) {
    if ($(inpuText).val().length > 0) {
        var matches = $.map(smartSearchLocations, function (item) {
            if (item.LocationBrandAlias.toUpperCase().indexOf($(inpuText).val().toUpperCase()) == 0) {
                return new locations(item);
            }
        });
        searchViewModel.locations(matches);

    } else {
        //$(controlId).show();
        var srcs = $.map(smartSearchLocations, function (item) { return new locations(item); });
        searchViewModel.locations(srcs);
    }
}
function getUserDate(picker) {
    if ($('#startDate').val().length > 0) {
        var theDate = picker.get('select')

        var $end = $('#endDate').pickadate();
        //$('#endDate').prop('disabled', false)
        var endPicker = $end.pickadate('picker');
        //console.log(theDate.year,theDate.month,theDate.day)
        endPicker.set('min', new Date(theDate.year, theDate.month, theDate.date + 1))
    }
}

function startSearch() {

    searchModel = new Object();
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
    $('#searchInitiated').fadeIn(1000).delay(2000).fadeOut(1000);



    $.ajax({
        url: 'Search/InitiateSearch/',
        type: 'POST',
        async: true,
        data: searchModel,
        success: function (data) {
            $('#pastSearches').scrollTop(0);
            $('#searchInitiated').hide();
            resetSelection();
            // To get last search data in summary
            SearchSummaryRecursiveCall = true;//For Settimeout recursive call not create
            recentSearchAjaxCall();
        },
        error: function (e) {
            //called when there is an error
            console.log(e.message);
            $('#searchInitiated').hide();
        }
    });

}


// add an error display to a given field
function addErrorToField(field) {
    $('#' + field).addClass('has-error');
    MakeTagFlashable('#' + field);
    AddFlashingEffect();
}
function removeErrorToField(field) {
    $('#' + field).removeClass('has-error');
    RemoveFlashableTag('#' + field);
    AddFlashingEffect();
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
function resetSelection() {
    // $('#startDate, #endDate, #searchLocation').val('');
    $('#searchLocation').val('');
    $("#startDate").datepicker("hide");
    $("#endDate").datepicker("hide");
    var today = new Date();
    $("#startDate").val('');
    $("#endDate").val('');
    $('#endDate,#endDateimg').datepicker('option', { defaultDate: today, minDate: 0, maxDate: null });

    var srcs = $.map(smartSearchLocations, function (item) { return new locations(item); });
    searchViewModel.locations(srcs);
    //AddFlashingEffect();
    $('#searchLeftPanel select option').each(function () {
        $(this).prop("selected", false);
    });

    $('#lengths-day, #lengths-week, #lengths-month, #lengths-all, #carClass-all').prop('checked', false);

    $('#pickupHour li').eq(0).attr('value', '11:00 am').text("11:00am");
    $('#dropOffHour li').eq(0).attr('value', '11:00 am').text("11:00am");
    $("#error-span").hide();
    $("#searchLeftPanel").find(".has-error").removeClass("has-error").removeClass("temp").removeClass("flashBorder");
    $('#searchLocation').removeClass('temp').removeClass("flashBorder");

    $('#source select').val('');
    $('select#carClass, select#lengths, select#locations,select#source').scrollTop(0);
    lastSelectedSearchSummaryStatus = 0;
    ResetLORs();
}
function resetDate() {
    $("#startDate").datepicker("hide");
    $("#endDate").datepicker("hide");
    $("#startDate").datepicker("setDate", null);
    $("#endDate").datepicker("setDate", null)
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
    this.dragClass = ko.computed(function () {
        if (data.CompanyID == GlobalLimitSearchSummaryData.BrandIDs) {
            return "drag_bar";
        }
        return "drag_bar_disable";
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
        this.postionChangeCss = data.IsPositionChanged ? 'red_cell' : '';
    }
    else {
        this.moveUpDownCSS = '';
        this.postionChangeCss = '';
    }


}

/*Binding Search Grid end here*/

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

//Display selected search summary data to show summary details block
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
    var $previousIsQuickView = $($previous).attr("isquickview");

    var $previousHasQuickView = $("#pastSearches").find(".rsselected");
    var $previousValHasQuickView = $($previousHasQuickView).val();
    var $previousSelectedHasQuickView = $($previousHasQuickView).attr("HasQuickViewChild");
    if ($previousSelectedHasQuickView == "true") {
        if ($($previousHasQuickView).attr("isquickview") == "true") {
            $previousSelectedHasQuickView = "false";
        }
    }

    if (data.IsQuickView) {
        $("#selectedSummaryID_" + data.SearchSummaryID).removeClass("quick-view-past-search");
        $("#selectedSummaryID_" + data.SearchSummaryID).addClass("rsselected");
        $("#selectedSummaryID_" + data.SearchSummaryID).siblings("li").removeClass("rsselected");
        $("#search-summary").css("background", "#F0DCF1");
    }
    else if (data.HasQuickViewChild && !data.IsQuickView) {
        $("#selectedSummaryID_" + data.SearchSummaryID).removeClass("has-quick-view-search");
        $("#selectedSummaryID_" + data.SearchSummaryID).addClass("rsselected");
        $("#selectedSummaryID_" + data.SearchSummaryID).siblings("li").removeClass("rsselected");
        $("#search-summary").css("background", "#80d2f1");
    }
    else {
        $("#search-summary").removeAttr("style");
        $("#selectedSummaryID_" + data.SearchSummaryID).addClass("rsselected");
        $("#selectedSummaryID_" + data.SearchSummaryID).siblings("li").removeClass("rsselected");
    }

    //Logic for last selected status.
    if ($previousIsQuickView != undefined && $previousIsQuickView == "true" && $previousVal != data.SearchSummaryID) {
        $("#selectedSummaryID_" + $previousVal).addClass("quick-view-past-search");
    }
    if ($previousSelectedHasQuickView != undefined && $previousSelectedHasQuickView == "true" && $previousValHasQuickView != data.SearchSummaryID) {
        $("#selectedSummaryID_" + $previousValHasQuickView).addClass("has-quick-view-search");
    }


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
            resetSelection();
        $('#viewResult').removeClass("disable-button");
        $('#viewResult').unbind('click').bind('click', function () {
            viewResultclick();
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
            EnableDisableMultipleLOR(prvcode, $("select#source option:selected").attr("isgov"));
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
        ResetLORs();
        DisableLORs();
    }
}


function viewResultclick() {
    $(".spanlastdayshop").hide();
    HideShowGridControls(true);
    if (searchSummaryData.IsQuickView) {
        showQuickViewControl(true);
        showQuickViewShopResult(searchSummaryData.SearchSummaryID);
        return false;
    }
    else {
        showQuickViewControl(false);
        $("#headersection").html('RESULTS');
    }
    $("input[name='gapvalue']").prop("checked", false);
    $("#txtDragGapValue").val('');
    //$("input[type='radio'][name='position'][value='below']").prop('checked', true);
    $('#chkAbovePos').prop('checked', false);
    CheckTetherGlobalLimitAjaxCall = false;
    CheckTetherFormulaAjaxCall = false;
    FirstClickOpaqueRatePopup = false;
    //ResetOpaqueRateCodesSelection();
    GlobalLimitSearchSummaryData = "";
    $("#OpaqueRate").prop("disabled", false).removeClass("disable-button");
    $("#IsOpaqueChk").prop("checked", true);
    GlobalLimitSearchSummaryData = searchSummaryData;//used for Global limit implementation
    FinalTetherValueData = [];
    savePreloadData = [];
    SearchSummaryId = searchSummaryData.SearchSummaryID;

    //Check Quick view button enable/disable logic
    CheckQuickViewButtonEnableDisable();
    GetApplicableOpaqueRateCodes();
    //load grid view
    if ($('.mobileSearchICO').is(':visible')) {
        $('.mobileSearchICO').click();
    }
    else {
        scrollTop = $('#pastSearches').scrollTop();
        //AnimateLeftPanel();
    }
    //FetchLastUpdateTSD();
    bindSearchFilters();
    //getCurrentFormula();
    //getLOR();
    $('.LOR').hide();
    if ($('.extraDayRateFactor').hasClass('temp')) {
        $('.extraDayRateFactor').removeClass('temp');
        RemoveFlashableTag($('.extraDayRateFactor'));
    }
    if ($('#viewSelect .selected').attr('value') != 'classic') {
        //Bind controls for all element for all view

        bindSearchFilters('_m');
        bindSearchFilters('_ml');
        getSearchData(false);
    }
    else {
        showView('classic');
    }
    currentView = $.trim($('#viewSelect .selected').attr('value'));
    //to check wether user can uncheck istether on popup then disbale the tethershop popup button
    //var tetherCheck = sessionStorage.getItem("activateTether_" + selectedSearchSummaryID);
    //if (JSON.parse(tetherCheck) == true || JSON.parse(tetherCheck) == null) {
    //    $("#IsTetherUser").prop("checked", true);
    //    $("#TetherRate").prop('disabled', false).removeClass("disable-button");
    //}
    //Reset selected location brand id and bool value tether popup opened
    $('#hdnTetherBrandLocationId').val('0');
    tetherShopFirstClick = false;
    ResetQuickViewSchedulePopup();
}

function recentSearchSelection() {
    var recentUserID = 0, recentStatusID = 0, recentLengthID = 0, recentLocationID = 0, recentSourceID = 0, recentSearchType = 0;
    $("#recentUsers ul").on("click", "li", function () {
        recentUserID = $(this).val();
        populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType);
        var isAdmin = $("#hdnIsAdminUser").val();
        if (isAdmin.toUpperCase() == "FALSE") {
            SearchSummaryRecursiveCall = true;
            $("#lastModifiedDate").attr("value", "");
            ShowHideRecentShopsLoader(true);
            recentSearchAjaxCall(true);
        }
    });
    $("#recentStatuses").on("click", "li", function () {
        recentStatusID = $(this).val();
        populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType);
        ShowFirstRecordIfNoPrevRecordExists();
    });
    $("#recentLengths").on("click", "li", function () {
        recentLengthID = $(this).val();
        populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType);
        ShowFirstRecordIfNoPrevRecordExists();
    });
    $("#recentLocations").on("click", "li", function () {
        recentLocationID = $(this).val();
        populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType);
        ShowFirstRecordIfNoPrevRecordExists();
    });
    $("#recentSources").on("click", "li", function () {
        recentSourceID = $(this).val();
        populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType);
        ShowFirstRecordIfNoPrevRecordExists();
    });
    $("#recentSearchType").on("click", "li", function () {
        recentSearchType = $(this).val();
        populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType);
        ShowFirstRecordIfNoPrevRecordExists();
    });
}
//This function is used to check while page is on search at the time to check wether any filter drop down is selected or not.
function recentSearchOnLoadCheck() {
    var recentUserID = 0, recentStatusID = 0, recentLengthID = 0, recentLocationID = 0, recentSourceID = 0, recentSearchType = 0;
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

    if (($("#recentSearchType li").attr("value")) != "0") {
        DropDownSummaryShopFlag = true;
        recentSearchType = $("#recentSearchType li").attr("value");
    }

    if (DropDownSummaryShopFlag) {
        populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType);
    }
}

function populateSearchSummaryDropDown(recentUserID, recentStatusID, recentLengthID, recentLocationID, recentSourceID, recentSearchType) {

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
    if (recentSearchType != 0) {
        DropDownSummaryShopFlag = true;
        if (recentSearchType == 1) {
            conditionStr += "[IsQuickView=false]";
        }
        if (recentSearchType == 2) {
            conditionStr += "[IsQuickView=true]";
        }
    }

    if (DropDownSummaryShopFlag) {
        $("#bindSearchSummary ul li").show().not(conditionStr).hide();
    }


}

function recentSearchAjaxCall(isUserFilterChange) {
    var t = new Date(); console.log("Call to Recent search--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
    FirstSearchSummaryDataCollection = "";
    var currentDateTime = new Date();
    lastModifiedDate = $("#lastModifiedDate").attr("value");
    if (searchViewModel.SearchSummary().length == 0) {
        lastModifiedDate = "";
    }
    var IsAdminUser = $("#hdnIsAdminUser").val();
    var selectedUserInRecentSearch = $("#recentUsers ul li.selected").attr("value");
    if (typeof (selectedUserInRecentSearch) == 'undefined') {
        if (IsAdminUser.toUpperCase() == 'FALSE') {
            selectedUserInRecentSearch = $("#recentUsers ul li[value='" + LoggedInUserID + "']").eq(0).attr("value");
        }
        else {
            selectedUserInRecentSearch = $("#recentUsers ul li").eq(0).attr("value");
        }

    }
    var ajaxURl = 'Search/GetSearchSummary/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetSearches;
    }
    $.ajax({
        url: ajaxURl,
        type: 'POST',
        data: { objLastModifieddate: lastModifiedDate, strClientTimezoneOffset: currentDateTime.getTimezoneOffset(), LoggedInUserID: LoggedInUserID, userSystemDate: currentDateTime.toDateString(), isAdminUser: IsAdminUser, selectedUserId: selectedUserInRecentSearch },
        dataType: 'json',
        async: true,
        success: function (data) {
            var t = new Date(); console.log("Response from Recent search--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
            FirstSearchSummaryDataCollection = data;
            if (isUserFilterChange) {
                searchViewModel.SearchSummary('');
                isNewArrayFetched = false;
            }
            if (data.lstSearchSummaryData.length > 0) {
                var t = new Date(); console.log("Data found in Response from Recent search--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
                if (searchViewModel.SearchSummary().length > 0) {
                    isNewArrayFetched = true;
                    var newSummaries = data.lstSearchSummaryData;//$.map(data.lstSearchSummaryData, function (item) { return new SearchSummary(item); });
                    var newlyAddedList = [];
                    var t = new Date(); console.log("searchViewModel search summaries length--->" + searchViewModel.SearchSummary().length.toString() + "---time at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
                    var t = new Date(); console.log("new search summaries length--->" + newSummaries.length.toString() + "---time at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
                    var counter = 0;

                    ko.utils.arrayForEach(searchViewModel.SearchSummary(), function (objSearchSummary, index) {
                        var t = new Date(); console.log("searchViewModel search summaries for loop--->" + index.toString() + "---time at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
                        ko.utils.arrayForEach((newSummaries), function (latestSearchSummary, ind) {
                            var t = new Date(); console.log("new search summaries for loop--->" + ind.toString() + "---time at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
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
                                        objSearchSummary.IsQuickView = latestSearchSummary.IsQuickView;
                                        objSearchSummary.HasQuickViewChild = latestSearchSummary.HasQuickViewChild;
                                        objSearchSummary.SearchTypeClass = latestSearchSummary.SearchTypeClass;
                                        objSearchSummary.StatusClass = latestSearchSummary.StatusClass;

                                        $('#pastSearches  li[value=' + objSearchSummary.SearchSummaryID + ']').attr({
                                            'StatusId': latestSearchSummary.StatusIDs,
                                            'IsQuickView': latestSearchSummary.IsQuickView,
                                            'HasQuickViewChild': latestSearchSummary.HasQuickViewChild,
                                            'class': latestSearchSummary.SearchTypeClass
                                        });
                                        $('#pastSearches  li[value=' + objSearchSummary.SearchSummaryID + '] span').eq(1).attr('class', latestSearchSummary.StatusClass).html(latestSearchSummary.StatusName);

                                        latestSearchSummary.isFound = true;
                                    }
                                    // newSummaries.splice($.inArray(latestSearchSummary, newSummaries), 1);
                                    return false;
                                }

                            }
                        })
                    });
                    var t = new Date(); console.log("Array update complete in Recent search at --->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
                    //remove searches that has deleted status
                    searchViewModel.SearchSummary.removeSearch();
                    var t = new Date(); console.log("Array remove complete in Recent search at --->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
                    if (newSummaries.length > 0) {
                        for (i = 0; i < newSummaries.length; i++) {
                            var summary = newSummaries[i];
                            if ($.trim(summary.StatusIDs).toString() != '6' && ((typeof (summary.isFound) == 'undefined') || !summary.isFound)) {
                                newlyAddedList.push(summary);
                            }
                        }
                        //$.each(newSummaries, function (index, summary) {
                        //    if ($.trim(summary.StatusIDs).toString() != '6' && ((typeof (summary.isFound) == 'undefined') || !summary.isFound)) {
                        //        newlyAddedList.push(summary);
                        //    }
                        //});
                    }
                    var t = new Date(); console.log("Start adding new searches in Recent search at --->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
                    if (newlyAddedList != null && newlyAddedList.length > 0) {
                        $.each(newlyAddedList, function (index, summary) {
                            searchViewModel.SearchSummary.unshift(summary);
                        });
                    }
                    var t = new Date(); console.log("Complete adding new searches in Recent search at --->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
                    $("#pastSearches li.hidden").removeClass("hidden");
                }
            }
            BindSearchSummaryData();
            //if (!checkLoadSearchSummaryAjax) {
            //    CheckfirstSearchSummary = true;
            //    CheckRecentSearchSummaryAjax();
            //}
            //else {
            //    BindSearchSummaryData();
            //}
            if (isUserFilterChange && $('#pastSearches li[statusid="4"]:visible').length > 0) {

                $('#viewResult').removeClass("disable-button").unbind('click').bind('click', function () {
                    viewResultclick();
                });
                $('#pastSearches li[statusid="4"]:visible').eq(0).trigger('click');
                $('#viewResult').trigger("click");
                HideShowGridControls(true);
            }
            else if (isUserFilterChange) {
                $('#viewResult').removeClass("disable-button").addClass("disable-button").unbind('click');
                HideShowGridControls(false);
            }
            else {
                if ($('#noRecords:visible').length == 0) {
                    //check if past searches length is 0 
                    if ($('#pastSearches li[statusid="4"]').length <= 0) {
                        HideShowGridControls(false);
                    } else {
                        HideShowGridControls(true);
                    }
                }
            }
            ShowHideRecentShopsLoader(false);
            if (SearchSummaryRecursiveCall) {
                SearchSummaryRecursiveCall = false;
            }
            else {
                setTimeout(function () { recentSearchAjaxCall(); }, 30000);
            }
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            if (SearchSummaryRecursiveCall) {
                SearchSummaryRecursiveCall = false;
            }
            else {
                setTimeout(function () { recentSearchAjaxCall(); }, 30000);
            }
        }
    });
};
//for call first time while all search control bind
function CheckRecentSearchSummaryAjax() {
    if (!checkLoadSearchSummaryAjax && CheckBindCarClass && CheckBindLocation && CheckfirstSearchSummary) {
        //BindSearchSummaryData();
        checkLoadSearchSummaryAjax = true;
    }
}
//Operation of search summary on success data
function BindSearchSummaryData() {
    var t = new Date(); console.log("Bind Search summary data --->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
    if (FirstSearchSummaryDataCollection != null && FirstSearchSummaryDataCollection.lastModifiedDate != "undefined" && FirstSearchSummaryDataCollection.lastModifiedDate != null) {
        $("#lastModifiedDate").val(FirstSearchSummaryDataCollection.lastModifiedDate);
    }
    if (FirstSearchSummaryDataCollection.lstSearchSummaryData.length != 0) {
        if (!isNewArrayFetched) {
            //var srcs = $.map(FirstSearchSummaryDataCollection.lstSearchSummaryData, function (item) { return new SearchSummary(item); });
            searchViewModel.SearchSummary(FirstSearchSummaryDataCollection.lstSearchSummaryData);
            $("#pastSearches li.hidden").removeClass("hidden");
        }
        var t = new Date(); console.log("Previous to RecentSearchLoadCheck --->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
        recentSearchOnLoadCheck();
        var t = new Date(); console.log("After RecentSearchLoadCheck --->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
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
            var t = new Date(); console.log("Bind view result click event --->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
            if (typeof (selectedSearchSummaryID) != 'undefined' && selectedSearchSummaryID != 0) {
                $("#selectedSummaryID_" + selectedSearchSummaryID).addClass("rsselected").siblings("li").removeClass("rsselected");
                //$("#selectedSummaryID_" + selectedSearchSummaryID).siblings("li").removeClass("rsselected");

                //If user select InProgress item and after it automatically complete then viewresult button enabled
                //$("#bindSearchSummary #pastSearches li[value=" + selectedSearchSummaryID + "]") == selectedSearchSummaryID && 
                if (($("#bindSearchSummary #pastSearches li[value=" + selectedSearchSummaryID + "]").attr("statusid")) == "4") {
                    $('#viewResult').removeClass("disable-button").unbind('click').bind('click', function () {
                        viewResultclick();
                    });
                }
                //return false;
                var t = new Date(); console.log("Complete Bind view result click event --->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
            }
        }
        // });
    }
    //Change request by ahmed current will make search and that search in in-progress mode then scroll automatically go top 
    if ($("#pastSearches li").length > 0) {
        var SearchSummaryFirstElement = $("#pastSearches li").eq(0);
        if ($(SearchSummaryFirstElement).attr("statusid") == "1" && $(SearchSummaryFirstElement).attr("userid") == LoggedInUserID) {
            $("#pastSearches").scrollTop(0);
        }
    }
    var t = new Date(); console.log("Complete bind search summary data method --->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
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
            searchViewModel.Status(srcs);
            $("#recentStatuses ul li").eq(0).addClass("selected");
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(errorThrown);
        }
    });
}

//Status Entity
function Status(data) {
    this.ID = data.ID;
    this.status = data.Status;
};
//SearchSummary Entity
function SearchSummary(data) {

    var self = this;

    this.SearchSummaryID = data.SearchSummaryID;
    this.StartDate = data.StartDate;
    this.EndDate = data.EndDate;
    this.StartTime = data.StartTime;
    this.EndTime = data.EndTime;
    this.CreatedDate = data.CreatedDate;
    this.LocationCode = data.LocationCode;
    this.UserName = data.UserName;
    this.FullName = data.FullName;
    this.UserID = data.UserID;
    this.StatusIDs = data.StatusIDs;
    this.StatusName = ko.computed(function () {
        if (data.SourcesIDs == 1 || data.SourcesIDs == 2 || data.SourcesIDs == 3) {
            return "IN PROGRESS";
        }
        else if (data.SourcesIDs == 4) {
            return "COMPLETE";
        }
        else if (data.SourcesIDs == 5) {
            return "FAILED";
        }

    }); //data.StatusName;
    this.SourcesIDs = data.SourcesIDs;
    this.BrandIDs = data.BrandIDs;
    //this.SourceName = computeData(data.SourcesIDs, "source");
    this.SourceName = data.SourceName;//remove for multiple user have diff. source access and current user has not access of other usersource
    this.RentalLengthsIDs = data.RentalLengthsIDs;
    this.RentalLengthName = computeData(data.RentalLengthsIDs, "lengths");
    this.CarClassIDs = getLatestCarClassIds(data.CarClassIDs);
    this.CarClassName = computeData(data.CarClassIDs, "carClass");
    this.LocationsBrandIDs = data.LocationsBrandIDs;
    this.LocationIDs = computeLocationID(data.LocationsBrandIDs, "locations");
    this.LocationName = computeData(data.LocationsBrandIDs, "locations");
    this.FailureResponse = data.FailureResponse;
    this.IsQuickView = data.IsQuickView;
    this.HasQuickViewChild = data.HasQuickViewChild;
    var varCreateDate = (new Date(parseInt(data.CreatedDate.replace("/Date(", "").replace(")/", ""), 10)));
    this.dropdowntext = ko.computed(function () {
        var tempLocationName = "", TempLocationBrandID = data.LocationsBrandIDs.split(',');
        for (var i = 0; i < TempLocationBrandID.length; i++) {
            tempLocationName += $("#locations option[brandid=" + TempLocationBrandID[i] + "]").text() + ", ";
        }
        return monthNames[varCreateDate.getMonth()] + " " + varCreateDate.getDate() + " - " + (varCreateDate.getHours() < 10 ? '0' : '') + varCreateDate.getHours() + ":" + (varCreateDate.getMinutes() < 10 ? '0' : '') + varCreateDate.getMinutes() + " - " + tempLocationName.trim().substring(0, tempLocationName.trim().length - 1) + " - " + data.UserName;
    });


}

//Get latest car class order
function getLatestCarClassIds(Ids) {
    var newIds = "", TempIDs = Ids.split(',');

    $("#carClass option").each(function () {
        if ($.inArray($(this).attr('value'), Ids.split(',')) != -1) {
            newIds += $(this).attr('value') + ",";
        }
    });
    return newIds.trim();
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
//For used to get IDs
function computeLocationID(Ids, controlID) {
    var tempItemIds = "", TempIDs = Ids.split(',');
    for (var i = 0; i < TempIDs.length; i++) {
        tempItemIds += $("#" + controlID + " option[brandid=" + TempIDs[i] + "]").val() + ",";
    }
    return tempItemIds.trim().substring(0, tempItemIds.trim().length - 1);
}
//Tether Percentage and doller value event
function TetherBlurEvent() {
    $('#TehterTableValue [type="text"]').on('keyup', function () {
        var selectedID = $(this).attr('id');//get the selected item        
        validateTetherShop();

        //for use cross check
        var TempSelectedSplit = selectedID.split('_');
        //Percetnage Condition
        if ($.isNumeric($(this).val()) || $(this).val().trim() == "") {
            if ($(this).attr('name') == 'percentageValue') {
                //If percentage can remove then if doller value is exist then apply that
                if ($("#" + selectedID).val().trim() == "" && $("#DollerID_" + TempSelectedSplit[1]).val() != "") {
                    CalculationOfFormula("DollerID_" + TempSelectedSplit[1], "$");
                }
                else {
                    //normal percentage value execute
                    CalculationOfFormula(selectedID, "%")
                }
            }
            // Doller value condition
            if ($(this).attr('name') == 'dollerValue') {
                //If doller can remove then if percentage value is exist then apply that
                if ($("#" + selectedID).val().trim() == "" && $("#PercentageID_" + TempSelectedSplit[1]).val() != "") {
                    CalculationOfFormula("PercentageID_" + TempSelectedSplit[1], "%");
                }
                else {
                    //Normal doller value Execute 
                    CalculationOfFormula(selectedID, "$");
                }
            }
        }
        //validateTetherShop();
        AddFlashingEffect();
    });
}

function getExtraDayRateValue(locationBrandId, rentalLength) {
    var ajaxURl = 'TSDAudit/GetExtraDayRate/';
    if (TSDAjaxURL != undefined && TSDAjaxURL != '') {
        ajaxURl = TSDAjaxURL.GetExtraDayRate;
    }
    if (typeof (locationBrandId) != 'undefined' && (previousRentalLength == undefined || (rentalLength.toUpperCase() != previousRentalLength.toUpperCase()))) {
        //need to fetch extra day rate if rental length changed
        $.ajax({
            url: ajaxURl,
            type: 'GET',
            async: true,
            data: { LocationBrandId: locationBrandId, rentalLength: rentalLength },
            dataType: 'json',
            success: function (data) {
                if (data) {
                    $('.extraDayRateFactor').each(function () {
                        $(this).val(data);
                        $(this).attr('oldValue', data);
                    });
                    // $('.extraDayRateFactor]').val(data);
                    $('#updateAllExtraDayRate').val(data);
                    $('#updateAllExtraDayRate').attr('oldValue', data);
                }
            },
            error: function (e) {
                $('.loader_container_source').hide();
                console.log(e.message);
            }
        });
        previousRentalLength = rentalLength;
        if (rentalLength.toUpperCase().indexOf('D') >= 0) {
            $('[id=rentalLengthText]').html('Daily Rate');
            $('[id=calcOperation]').html('*');
            $('#updateAllLengthText').html('Daily Rate');
            $('#updateCalc').html('*');
        }
        else if (rentalLength.toUpperCase().indexOf('W') >= 0) {
            $('[id=rentalLengthText]').html('Weekly Rate');
            $('[id=calcOperation]').html('/');
            $('#updateAllLengthText').html('Weekly Rate');
            $('#updateCalc').html('/');
        }
    }
    else {
        $('.extraDayRateFactor').each(function () {
            //console.log($(this).attr('oldValue'));
            $(this).val($(this).attr('oldValue'));
        });
        $('#updateAllExtraDayRate').val($('#updateAllExtraDayRate').attr('oldValue'));
    }
}
function enableRentalLengths(lor) {
    ////hide show selection of Extra Day rental Lengths
    if (lor != null && lor != '') {
        $('#tsdSystemSelect_Weblink li').each(function () {
            $this = $(this);
            if ($this.text().toUpperCase().indexOf(lor) >= 0) {
                $this.show();
            }
            else {
                $this.hide();
            }
        });

        $('#updateAllLOR li,#updateAllLOR li,#UpdateAllLOR_ml option,#UpdateAllLOR_m option').each(function () {
            $this = $(this);
            if ($this.text().toUpperCase().indexOf(lor) >= 0) {
                $this.show();
            }
            else {
                $this.hide();
            }
        });


        $('#LOR_m').empty();
        $('#LOR_ml').empty();

        $('#dummyLor option').each(function () {
            $this = $(this);
            if ($this.text().toUpperCase().indexOf(lor) >= 0) {
                $("<option value='" + $this.val() + "'>" + $this.text() + "</option>").appendTo('#LOR_m,#LOR_ml');
            }

        });
    }
}
function highLightLOR(lorsel) {
    if (lorsel != null && lorsel != '') {
        $('#tsdSystemSelect_Weblink .prevSelected, #LOR_m .prevSelected, #LOR_ml .prevSelected').removeClass('prevSelected');
        var firstChar = lorsel.charAt(0);
        var digit = lorsel.match(/\d+/);
        $('#tsdSystemSelect_Weblink li').each(function () {
            $this = $(this);
            //if ($this.text().toUpperCase().indexOf(lor) >= 0) {
            //    $this.addClass('selected').addClass('prevSelected');
            //}
            //else {
            //    $this.removeClass('selected');
            //}

            if ($this.text().toUpperCase().indexOf(firstChar) >= 0 && $this.text().toUpperCase().indexOf(digit) >= 0) {
                $this.addClass('selected').addClass('prevSelected');
            }
            else {
                $this.removeClass('selected');
            }
        });
        $('#LOR_m option').each(function () {
            $this = $(this);
            //if ($this.text().toUpperCase().indexOf(lor) >= 0) {
            //    $this.addClass('selected').addClass('prevSelected');
            //    $this.prop('selected', true);
            //}
            //else {
            //    $this.removeClass('selected');
            //    $this.prop('selected', false);
            //}

            if ($this.text().toUpperCase().indexOf(firstChar) >= 0 && $this.text().toUpperCase().indexOf(digit) >= 0) {
                $this.addClass('selected').addClass('prevSelected');
                $this.prop('selected', true);
            }
            else {
                $this.removeClass('selected');
                $this.prop('selected', false);
            }
        });
        $('#LOR_ml option').each(function () {
            $this = $(this);
            //if ($this.text().toUpperCase().indexOf(lor) >= 0) {
            //    $this.addClass('selected').addClass('prevSelected');
            //    $this.prop('selected', true);
            //}
            //else {
            //    $this.removeClass('selected');
            //    $this.prop('selected', false);
            //}

            if ($this.text().toUpperCase().indexOf(firstChar) >= 0 && $this.text().toUpperCase().indexOf(digit) >= 0) {
                $this.addClass('selected').addClass('prevSelected');
                $this.prop('selected', true);
            }
            else {
                $this.removeClass('selected');
                $this.prop('selected', false);
            }
        });
    }
}
function validateDigitLen(element) {
    var $this = element;
    var value = $this.val();
    if ($.isNumeric(value)) {
        if (value.indexOf('.') != -1) {
            if (value.split(".")[0].length <= 4 && value.split(".")[1].length <= 2) {
            } else {
                return false;
            }
        }
        else if (value.length > 4) {
            return false;
        }
    }

}

function CheckQuickViewButtonEnableDisable() {
    //if (typeof (GlobalLimitSearchSummaryData) != 'undefined' && typeof (GlobalLimitSearchSummaryData.CreatedDate) != 'undefined' && new Date(parseInt(GlobalLimitSearchSummaryData.CreatedDate.replace("/Date(", "").replace(")/", ""), 10)).getDate() < new Date().getDate()) {
    //    $(".btnSetAsQuickView, .btnSetAndRunQuickView").hide();
    //}
    //else {
    //    $(".btnSetAsQuickView, .btnSetAndRunQuickView").show();
    //}

    //Here check if the shop is gov and accordingly write the code for opqaue checkbox
    if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
        $('[id=OpaqueRate]').attr("disabled", "disabled").addClass("disable-button");    //for disabling the opaque rate button
        $('[id=IsOpaqueChk]').attr("disabled", "disabled").prop("checked", false);
    }
    else {
        $('[id=OpaqueRate]').removeAttr("disabled", "disabled").removeClass("disable-button");    //for enabling the opaque rate button
        $('[id=IsOpaqueChk]').removeAttr("disabled", "disabled").prop("checked", true);
    }

    if (GlobalLimitSearchSummaryData != null && GlobalLimitSearchSummaryData.HasQuickViewChild) {
        $(".btnSetAsQuickView, .btnSetAndRunQuickView").addClass("disable-button").prop("disabled", true);
    }
    else {
        $(".btnSetAsQuickView, .btnSetAndRunQuickView").removeClass("disable-button").prop("disabled", false);
    }
    if (GlobalLimitSearchSummaryData != null && GlobalLimitSearchSummaryData.IsQuickView) {
        $(".QuickViewFilter").show();
        $("[id=IgnoreAndNext]").show();
        $("#headersection").html('QUICK VIEW RESULTS');
    }
    else {
        $(".QuickViewFilter").hide();
        $("[id=IgnoreAndNext]").hide();
        $("#headersection").html('RESULTS');
    }
}
//if (($("#bindSearchSummary #pastSearches li[value=" + selectedSearchSummaryID + "]").attr("isquickview")) == "true") {
//    $("#selectedSummaryID_" + selectedSearchSummaryID).addClass("quickViewSelected");
//}

function SetUserFilter() {
    var isAdmin = $("#hdnIsAdminUser").val();
    if (isAdmin.toUpperCase() == "FALSE") {
        if ($('#LoggedInUserId') != undefined && $('#LoggedInUserId').val().trim() != '') {
            var loggedInUserId = parseInt($('#LoggedInUserId').val());
            $('#tableSearchFilters #recentUsers ul li[value="' + loggedInUserId + '"]').addClass('selected');
            $('#tableSearchFilters #recentUsers li').eq(0).val($('#tableSearchFilters #recentUsers ul li[value="' + loggedInUserId + '"]').val()).text($('#tableSearchFilters #recentUsers ul li[value="' + loggedInUserId + '"]').text());
        }
        $('#tableSearchFilters #recentUsers ul li[value="0"]').hide();
        //$('#tableSearchFilters #recentUsers').addClass("disable-UL");
    }
}

function HideShowGridControls(showControls) {

    if (showControls) {
        $('.hideNoSummary').show();
        $('#noRecords').hide();
    }
    else {
        $('.hideNoSummary').hide();
        $('#noRecords').show();
    }
}

function ShowFirstRecordIfNoPrevRecordExists() {
    if ($('#noRecords').is(':visible') && $('#pastSearches li[statusid="4"]:visible').length > 0) {
        $('#pastSearches li[statusid="4"]:visible').eq(0).trigger('click');
        $('#viewResult').trigger("click");
        HideShowGridControls(true);
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

function BindRateCodes(lstApplicableRateCodes) {
    //SearchRateCodeModel
    searchViewModel.ApplicableRateCodes($.map(lstApplicableRateCodes, function (item) { return new SearchRateCodeModel(item); }));
}

function GetApplicableOpaqueRateCodes() {
    if (typeof (GlobalLimitSearchSummaryData) != 'undefined' && GlobalLimitSearchSummaryData.StartDate && GlobalLimitSearchSummaryData.EndDate) {
        var startDate = convertToServerTime(new Date(parseInt(GlobalLimitSearchSummaryData.StartDate.replace("/Date(", "").replace(")/", ""))));
        var endDate = convertToServerTime(new Date(parseInt(GlobalLimitSearchSummaryData.EndDate.replace("/Date(", "").replace(")/", ""))));

        startDate = ('0' + (startDate.getMonth() + 1)).slice(-2) + "/" + ('0' + (startDate.getDate())).slice(-2) + "/" + startDate.getFullYear()
        endDate = ('0' + (endDate.getMonth() + 1)).slice(-2) + "/" + ('0' + (endDate.getDate())).slice(-2) + "/" + endDate.getFullYear()

        $.ajax({
            url: 'Search/GetApplicableRateCodes',
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify({ 'strStartDate': startDate, 'strEndDate': endDate }),
            //data: { 'userId': loggedInUserId },
            success: function (data) {
                $('.loader_container_source').hide();
                BindRateCodes(data);
            },
            error: function (e) {
                $('.loader_container_source').hide();
                console.log(e.message);
            }
        });
    }
}