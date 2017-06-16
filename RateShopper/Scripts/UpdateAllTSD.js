var prevLOR = '';
var savePreloadData = [];
var isUpdateSent = false;
$(document).ready(function () {
    //New change request from ahmad about daily rate update
    //open popup button
    $("#updateAllTSD_popup").draggable();
    $('[id=UpdateAllTSD]').click(function (e) {
        e.preventDefault();
        UpdateAllTSD();

        if (window.navigator.userAgent.indexOf("MSIE") > 0 || window.navigator.userAgent.indexOf("Firefox") > 0) {
            $('html').scrollTop(0);
        }
        else {
            $('body').scrollTop(0);
        }
    });

    //day selectable
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
                    $(".baserate_section_two").hide();
                }
            }
            else {
                $(this).siblings('.UpdateAll_weeklyAll').prop('checked', false);
                $(".baserate_section_two").show();
            }
        },
    });

    //check box for all
    $('.UpdateAll_weeklyAll').click(function () {
        if ($(this).is(':checked')) {
            $(this).siblings('ul').children().addClass('selected');
            $(this).siblings('select').children().prop('selected', true);
            $(this).closest(".days_section").find(".selectdays_row2 .UpdateAll_weekly_large .selected").removeClass("selected").removeClass("ui-selected");
            $(".baserate_section_two").hide();
        }
        else {
            $(this).siblings('ul').children().removeClass('selected');
            $(this).siblings('select').children().prop('selected', false);
            $(".baserate_section_two").show();
        }

    });

});

//Other functions
//Extra daily rate: update rate factor for startdate and end date
function UpdateAllTSD() {
    //get shop dates

    var startDate, endDate;
    var strStart, strEnd;
    if (isUpdateSent) {
        $("#updateAllTSD_popup, .popup_bg").show();
        return;
    }

    if (GlobalLimitSearchSummaryData != null) {
        strStart = GlobalLimitSearchSummaryData.StartDate;
        strEnd = GlobalLimitSearchSummaryData.EndDate;
        startDate = new Date(parseInt(strStart.substr(6)));
        endDate = new Date(parseInt(strEnd.substr(6)));

        startDate = new Date(startDate);
        endDate = new Date(endDate);
    }
    $("#tsdStartDate,#tsdEndDate").datepicker({
        onSelect: function () {
            if ($('#error-update').is(':visible')) { $('#error-update').hide(); removeErrorToField('tsdStartDate'); removeErrorToField('tsdEndDate'); }
        }
    });

    var bindCarClasses = [];

    $("#classic_view, #carClass ul li").each(function () {
        var item = {};
        item["ID"] = $(this).val();
        item["Code"] = $(this).text();
        bindCarClasses.push(item);
    });
    //Rental Length Binding
    $("#updateAllTSD_popup #UpdateAllTSDLength ul").empty();
    $("#classic_view, #length ul li").each(function () {
        if ($(this).hasClass("selected")) {
            $("<li value='" + $(this).val() + "' class='selected'>" + $(this).text() + "</li>").appendTo($("#updateAllTSD_popup #UpdateAllTSDLength ul"));
            var $lengthValue = $.trim($(this).text());
            showHideRentalLengths($lengthValue.charAt(0));
            highLight($lengthValue);
        }
        else {
            $("<li value='" + $(this).val() + "'>" + $(this).text() + "</li>").appendTo($("#updateAllTSD_popup #UpdateAllTSDLength ul"));
        }
    });
    $("#updateAllTSD_popup #UpdateAllTSDLength li").eq(0).val($("#classic_view, #length ul").find(".selected").val()).text($("#classic_view, #length ul").find(".selected").text());
    prevLOR = $('#UpdateAllTSDLength li.selected').text().trim().charAt(0);

    if (prevLOR.toUpperCase().indexOf('D') >= 0) {
        $('#updateAllLengthText').html('Daily Rate');
        $('#updateCalc').html('*');

    }
    else if (prevLOR.toUpperCase().indexOf('W') >= 0) {
        $('#updateAllLengthText').html('Weekly Rate');
        $('#updateCalc').html('/');
    }

    $('#updateAllExtraDayRate').val($('.extraDayRateFactor').val());
    $('#updateAllExtraDayRate').attr('oldValue', $('.extraDayRateFactor').val());

    RemoveFlashableTag($('#updateAllExtraDayRate'));

    $("#updateAllTSD_popup #UpdateAllTSDLength ul li").unbind('click').bind('click', function (event) {
        $('#TSDUpdateAll tr td input[type="text"]').val('');
        var $lengthValue = $.trim($(this).text());
        showHideRentalLengths($lengthValue.charAt(0));
        highLight($lengthValue);
        getExtraUpdateAll($('#location ul li.selected').attr('lbid'), $lengthValue.charAt(0));
        $('#globalLimitError').hide();
        RemoveFlashableTag('#updateAllExtraDayRate');

    });


    var srcs = $.map(bindCarClasses, function (item) { return new carClasses(item); });
    searchViewModel.allCarClasses(srcs);

    PreloadFillBaseRate(savePreloadData);
    $("#tsdStartDate").datepicker('option', { dateFormat: 'mm/dd/yy', defaultDate: startDate, minDate: startDate, maxDate: endDate });
    $("#tsdEndDate").datepicker('option', { dateFormat: 'mm/dd/yy', defaultDate: endDate, minDate: startDate, maxDate: endDate });
    $("#tsdStartDate").datepicker("setDate", startDate);
    $("#tsdEndDate").datepicker("setDate", endDate);
    $('#tsdStartDate').click(function () { $(this).datepicker('show'); });
    $('#tsdEndDate').click(function () { $(this).datepicker('show'); });
    $("#updateAllTSD_popup, .popup_bg").show();
    //$('#TSDUpdateAll tr td input[type="text"]').val('');
    $(".baserate_section_two").not(":visible").show();
    if (!isUpdateSent) {
        $('#TSDUpdateAll tr td input[type="text"]').val('');
    }
}

function PreloadTSDData() {
    $('#globalLimitError').hide();
    GetPreloaBaseRate();
}

function CloseTSDUpdate() {
    if ($("#TSDUpdateAll .temp").length > 0) {
        $("#TSDUpdateAll .temp").val("").removeClass('temp').removeClass('flashBorder');
    }
    if ($("#updateAllTSD_popup .has-error").length > 0) {
        $("#updateAllTSD_popup .has-error").removeClass('has-error').removeClass('temp').removeClass('flashBorder');
    }
    if (!isUpdateSent) {
        $('.UpdateAll_weeklyAll').prop('checked', false);
        $(".UpdateAll_weekly_large .selected").removeClass("selected").removeClass("ui-selected");
    }
    $('#globalLimitError,#error-update').hide();
    $("#updateAllTSD_popup, .popup_bg").hide();
    PreloadFillBaseRate(savePreloadData);
}
function TSDApply() {
    if (ValidateDaysSelected()) {
        $('#globalLimitError').hide();
        var allTextBoxEmpty = true;
        $('#TSDUpdateAll tr td:nth-child(2) input[type="text"]').each(function () {
            //console.log($.trim($(this).val()));
            if ($.trim($(this).val()) != '') {
                allTextBoxEmpty = false;
                return;
            }
        });

        var extraDayRate = $('#updateAllExtraDayRate').val();
        if (!$.isNumeric(extraDayRate) || parseFloat(extraDayRate) <= 0) {
            ShowConfirmBox('Extra Daily Rate factor cannot be empty and should be numeric', false);
            return false;
        }

        if (allTextBoxEmpty) {
            ShowConfirmBox('At least one car class must have Base Value for TSD Update.', false);
            return false;
        }
        if ($('#updateAllLOR').is(":visible")) {
            if ($('#updateAllLOR li.selected').length == 0) {
                ShowConfirmBox('At least one LOR must be selected for TSD Update.', false);
                return false;
            }
        }
        TetherRateButton();
        if (ValidationTSDUpdateAll()) {
            SaveTSDBaseRate();
            //send for update 

            $('#tsdStartDate').val() == '' ? addErrorToField("tsdStartDate") : removeErrorToField("tsdStartDate");
            $('#tsdEndDate').val() == '' ? addErrorToField("tsdEndDate") : removeErrorToField("tsdEndDate");

            if ($('#tsdStartDate').val() != '' && $('#tsdEndDate').val() != '') {
                var startDate = new Date($('#tsdStartDate').val());
                var endDate = new Date($('#tsdEndDate').val());
                //check if startDate is less than Current Date
                if (endDate < startDate) {
                    addErrorToField('tsdStartDate'); addErrorToField('tsdEndDate');
                }
                else { removeErrorToField('tsdStartDate'); removeErrorToField('tsdEndDate'); }
            }
            if ($('#updateAllTSD_popup').find('.has-error').length <= 0) {
                createModel();
                $('#error-update').hide();
            }
            else {
                $('#error-update').show();
            }
        }
    }
}
//End Entra Daily rate update operation


//Commom preload function use for reset and preload data
function PreloadFillBaseRate(preloadBaseData) {

    if ($("#TSDUpdateAll .temp").length > 0) {
        $("#TSDUpdateAll .temp").val("").removeClass('temp').removeClass('flashBorder');
    }

    $("#TSDUpdateAll [type=text]").val("");
    $(preloadBaseData).each(function () {
        if (SearchSummaryId == this.SearchSummaryId) {
            var baseEdit = "";
            var totalEdit = "";
            if (this.BaseEdit != "" && this.BaseEdit != null && this.BaseEdit != NaN) {
                baseEdit = parseFloat(this.BaseEdit).toFixed(2);
            }
            rt = baseEdit;
            if (rt != "" && rt != null) {
                if (formulaBtoT != "") {
                    totalEdit = EvaluateExtraRate(formulaBtoT, true);
                }
                $("#Base_" + this.CarClassId).val(commaSeparateNumber(baseEdit));
                $("#Total_" + this.CarClassId).val(commaSeparateNumber(totalEdit));
            }
        }
    });
    //this one is second grid
    $(preloadBaseData).each(function () {
        if (SearchSummaryId == this.SearchSummaryId) {
            var baseEdit = "";
            var totalEdit = "";
            if (this.BaseEditTwo != "" && this.BaseEditTwo != null && this.BaseEditTwo != NaN) {
                baseEdit = parseFloat(this.BaseEditTwo).toFixed(2);
            }
            rt = baseEdit;
            if (rt != "" && rt != null) {
                if (formulaBtoT != "") {
                    totalEdit = EvaluateExtraRate(formulaBtoT, true);
                }
                $("#Basert_" + this.CarClassId).val(commaSeparateNumber(baseEdit));
                $("#Totalrt_" + this.CarClassId).val(commaSeparateNumber(totalEdit));
            }
        }
    });

    TextboxEvent();
}

function SaveTSDBaseRate() {
    savePreloadData = [];
    var lstSearchJobUpdateAll = [];
    var IsDaily = false;
    var IsDailyCheck = $.trim($("#updateAllTSD_popup #UpdateAllTSDLength li").eq(0).text()).substr(0, 1);
    if (IsDailyCheck == 'D') {
        IsDaily = true;
    }
    $("#updateAllTSD_popup #TSDUpdateAll tbody tr").each(function () {
        var preloadData = new Object();
        var SearchJobUpdateAll = new Object();
        preloadData.SearchSummaryId = SearchSummaryId;
        preloadData.CarClassId = $(this).find(".BaseEdit").attr("carclassid");
        //first grid data
        preloadData.BaseEdit = commaRemovedNumber($(this).find(".BaseEdit").val());
        preloadData.TotalEdit = $(this).find(".TotalEdit").val();

        //second grid data
        preloadData.BaseEditTwo = commaRemovedNumber($(this).find(".BaseEditrt").val());
        preloadData.TotalEditTwo = $(this).find(".TotalEditrt").val();
        savePreloadData.push(preloadData);

        //Database entity cal
        if (parseFloat(preloadData.BaseEdit) == parseFloat(0).toFixed(0) || parseFloat(preloadData.BaseEditTwo) == parseFloat(0).toFixed(0)) {
            preloadData.BaseEdit = "";
            preloadData.BaseEditTwo = "";
        }
        SearchJobUpdateAll.SearchSummaryId = preloadData.SearchSummaryId;
        SearchJobUpdateAll.CarClassId = preloadData.CarClassId;
        SearchJobUpdateAll.BaseEdit = (preloadData.BaseEdit != "") ? parseFloat(commaRemovedNumber(preloadData.BaseEdit)) : preloadData.BaseEdit;
        SearchJobUpdateAll.BaseEditTwo = (preloadData.BaseEditTwo != "") ? parseFloat(commaRemovedNumber(preloadData.BaseEditTwo)) : preloadData.BaseEditTwo;
        SearchJobUpdateAll.IsDaily = IsDaily;

        lstSearchJobUpdateAll.push(SearchJobUpdateAll);
    });

    if (lstSearchJobUpdateAll.length > 0) {
        var ajaxURl = 'Search/InsertUpdateTSDUpdateAll';
        if (TSDUpdateAllURL != undefined && TSDUpdateAllURL != '') {
            ajaxURl = TSDUpdateAllURL.InsertUpdateTSDUpdateAllURL;
        }
        $.ajax({
            url: ajaxURl,
            type: 'POST',
            data: JSON.stringify({ 'SearchSummaryId': SearchSummaryId, 'searchJobUpdateAllDTO': lstSearchJobUpdateAll }),
            contentType: 'application/json; charset=utf-8',
            async: true,
            dataType: 'json',
            success: function (data) {
                if (data) {

                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                ///console.log(3);
            }
        });
    }
    //TetherRateButton();
    //logic to calculate new tether rate on updated values
}

function TSDUpdateBlurEvent() {
    $("#TSDUpdateAll  [type='text']").on("keyup", function () {
        if ($.trim($(this).val()) != "" && !$.isNumeric($.trim($(this).val()))) {
            MakeTagFlashable($(this));
        }
        else {
            RemoveFlashableTag($(this));
        }
    });
    AddFlashingEffect();
}
function ValidationTSDUpdateAll() {
    var flag = false;
    if ($("#updateAllTSD_popup .temp").length > 0) {
        flag = false;
    }
    else {
        flag = true;
    }
    return flag;
}

function EvaluateExtraRate(formula, isBase) {
    var showAdditionalBaseForTSD = false;
    var LenTSDUpdate = 0;
    var NDTSDUpdate = 0;
    var GetCurretnLOR = $("#updateAllTSD_popup #UpdateAllTSDLength li").eq(0).text();
    var TempLengthFirstChar = GetCurretnLOR.trim().substr(0, 1);//Used to get first digit of length
    var tempLenghtLastDigit = parseInt(GetCurretnLOR.trim().substr(1, GetCurretnLOR.trim().length));//used to get last digit of length


    if (TempLengthFirstChar == "D") {
        LenTSDUpdate = tempLenghtLastDigit;
        NDTSDUpdate = tempLenghtLastDigit;
    }
    if (TempLengthFirstChar == "W") {
        var TempLastDigit = parseInt(GetCurretnLOR.trim().substr(1, GetCurretnLOR.trim().length)); //Get last digit for check in W12-W14 would come then we need len=2
        //coverd w8-w11 special scenario
        if (TempLastDigit >= 8 && TempLastDigit <= 11) {
            showAdditionalBaseForTSD = true;
        }

        if (TempLastDigit >= 12 && TempLastDigit <= 14) {
            LenTSDUpdate = formulaWeekLengthW12toW14;
        }
        else {
            LenTSDUpdate = formulaWeekLength;
        }
        NDTSDUpdate = tempLenghtLastDigit;
    }
    if (TempLengthFirstChar == "M") {
        LenTSDUpdate = formulaMonthLength;
        NDTSDUpdate = tempLenghtLastDigit;
    }

    //This one logic for for W8 to W11 before calculating get base value and next apply for formula calculation
    if (showAdditionalBaseForTSD) {
        //if (isBase) {
        //Change Value of RT before formula execution to get total value
        rt = calculateNewBaseForUpdateAllTSD(isBase, rt, tempLenghtLastDigit);
        //}
    }


    var formulaCalculation = "";
    if (formula != undefined && formula != null) {
        var tempFormulaCalculation = formula;
        tempFormulaCalculation = tempFormulaCalculation.replace(/nd/g, NDTSDUpdate);
        tempFormulaCalculation = tempFormulaCalculation.replace(/len/g, LenTSDUpdate);

        try {
            formulaCalculation = eval(tempFormulaCalculation);//Calculate the formula meths opertaion


            //This one logic for for W8 to W11 after calculating the total to base
            if (showAdditionalBaseForTSD) {
                if (!isBase) {
                    //After get base value which fall in w8-w11 special scenario to get actual base value
                    formulaCalculation = calculateNewBaseForUpdateAllTSD(false, formulaCalculation, tempLenghtLastDigit);
                }
            }

            return formulaCalculation.toFixed(2);
        }
        catch (error) {
            showAdditionalBaseForTSD = false;
            return formulaCalculation = "";
        }
    }
    return formulaCalculation;
}

function TextboxEvent() {
    //Keyup event
    $('[id^="Base_"],[id^="Basert_"]').on("input", function () {
        var carClassId = $(this).attr("carclassid");
        var textType = ($(this).attr("texttype") == "left") ? true : false;
        var rt = commaRemovedNumber($(this).val());
        var val = rt;
        if (typeof (GlobalLimitSearchSummaryData) != 'undefined' && GlobalLimitSearchSummaryData.IsGOV && val.indexOf(".") > -1) {
            var valArray = val.split(".");
            if (valArray.length > 1 && valArray[1] > 0) {
                val = parseInt(val).toFixed(2);
                rt = val;
                $(this).val(val);
            }
        }
        if (formulaBtoT != "") {
            BaseTotalCalCulation(formulaBtoT, true, rt, carClassId, textType);
        }
        else {
            if (rt != "") {
                //Check left/right textbox
                if (textType) {
                    $("#Total_" + carClassId).val(parseFloat(0).toFixed(2));
                }
                else {
                    $("#Totalrt_" + carClassId).val(parseFloat(0).toFixed(2));
                }
            }
        }
    });
    $('[id^="Total_"],[id^="Totalrt_"]').on("input", function () {
        var carClassId = $(this).attr("carclassid");
        var textType = ($(this).attr("texttype") == "left") ? true : false;
        var total = commaRemovedNumber($(this).val());
        if (formulaTtoB != "") {
            BaseTotalCalCulation(formulaTtoB, false, total, carClassId, textType);
        }
        else {
            if (total != "") {
                //Check left/right textbox
                if (textType) {
                    $("#Base_" + carClassId).val(parseFloat(0).toFixed(2));
                }
                else {
                    $("#Basert_" + carClassId).val(parseFloat(0).toFixed(2));
                }
            }
        }
    });

    //focusout event
    $('[id^="Base_"],[id^="Basert_"]').bind("focusout", function () {
        var carClassId = $(this).attr("carclassid");
        var rt = commaRemovedNumber($(this).val());
        var textType = ($(this).attr("texttype") == "left") ? true : false;
        var val = rt;
        if (typeof (GlobalLimitSearchSummaryData) != 'undefined' && GlobalLimitSearchSummaryData.IsGOV && val.indexOf(".") > -1) {
            var valArray = val.split(".");
            if (valArray.length > 1 && valArray[1] > 0) {
                rt = parseInt(val);
            }
        }
        BaseTotalCalCulation(formulaBtoT, true, rt, carClassId, textType);

        if (rt != "" && rt != null) {
            //Check left/right textbox
            if (textType) {
                $("#Base_" + carClassId).val(commaSeparateNumber(parseFloat(rt).toFixed(2)));
            }
            else {
                $("#Basert_" + carClassId).val(commaSeparateNumber(parseFloat(rt).toFixed(2)));
            }
        }
    });
    $('[id^="Total_"],[id^="Totalrt_"]').bind("focusout", function () {
        var carClassId = $(this).attr("carclassid");
        var total = commaRemovedNumber($(this).val());
        var textType = ($(this).attr("texttype") == "left") ? true : false;

        BaseTotalCalCulation(formulaTtoB, false, total, carClassId, textType);
        if (total != "" && total != null) {
            //Check left/right textbox
            if (textType) {
                $("#Total_" + carClassId).val(commaSeparateNumber(parseFloat(total).toFixed(2)));
            }
            else {
                $("#Totalrt_" + carClassId).val(commaSeparateNumber(parseFloat(total).toFixed(2)));
            }
        }
    });
}
//to get final calaultion
function BaseTotalCalCulation(formula, baseFlag, Rate, carClassId, baseFirst) {
    var FinalRate = "";
    if (Rate != "" && $.isNumeric(Rate) && formula != "") {
        if (baseFlag) {
            rt = Rate;
            FinalRate = EvaluateExtraRate(formula, true);

            //check gov shop and add rates to finalrate
            if (GlobalLimitSearchSummaryData.IsGOV) {
                var selectedLor = $("#UpdateAllTSDLength ul li.selected").eq(0).val();
                if (typeof (selectedLor) != 'undefined' && selectedLor > 0) {
                    FinalRate = GetGovernmentRate(FinalRate, selectedLor, false);
                }
            }
            if (FinalRate != "") {
                FinalRate = parseFloat(FinalRate).toFixed(2);
            }

            //Check left/right textbox
            if (baseFirst) {
                $("#Total_" + carClassId).val(commaSeparateNumber(FinalRate));
            }
            else {
                $("#Totalrt_" + carClassId).val(commaSeparateNumber(FinalRate));
            }
        }
        else {
            total = Rate;

            if (GlobalLimitSearchSummaryData.IsGOV) {
                var selectedLor = $("#UpdateAllTSDLength ul li.selected").eq(0).val();
                if (typeof (selectedLor) != 'undefined' && selectedLor > 0) {
                    total = GetGovernmentRate(total, selectedLor, true);
                }
            }
            FinalRate = EvaluateExtraRate(formula, false);
            if (FinalRate != "") {
                FinalRate = parseFloat(FinalRate).toFixed(2);
            }
            if (GlobalLimitSearchSummaryData.IsGOV) {
                FinalRate = parseInt(FinalRate).toFixed(2);
            }
            var temp = commaSeparateNumber(FinalRate);
            //Check left/right textbox
            if (baseFirst) {
                $("#Base_" + carClassId).val(commaSeparateNumber(FinalRate));
            }
            else {
                $("#Basert_" + carClassId).val(commaSeparateNumber(FinalRate));
            }
        }
        Rate = "";
    }
    else {
        if (formula == "") {
            if (baseFlag) {
                if (Rate != "") {
                    $("#Total_" + carClassId + ",#Totalrt_" + carClassId).val(parseFloat(0).toFixed(2));
                }
            }
            else {
                if (Rate != "") {
                    $("#Base_" + carClassId + ",#Basert_" + carClassId).val(parseFloat(0).toFixed(2));
                }
            }
        }
        else {
            if (baseFirst) {
                $("#Base_" + carClassId).val("");
                $("#Total_" + carClassId).val("");
            }
            else {
                $("#Basert_" + carClassId).val("");
                $("#Totalrt_" + carClassId).val("");
            }
        }
    }
}


function calculateNewBaseForUpdateAllTSD(isBaseEdited, baseValue, lengthFactor) {
    if (isBaseEdited) {
        //user edited for base value. This will be used before calculating total value
        return (parseFloat(baseValue) * lengthFactor / 7);
    }
    else {
        //user edited for total value. This will be used before calculating base value
        return (parseFloat(baseValue) * 7 / lengthFactor);
    }
}
//End other functions

//read model list 
function createModel() {
    var IsValid = true;
    $('#globalLimitError').hide();
    var startDate = $('#tsdStartDate').val();
    var endDate = $('#tsdEndDate').val();
    var LocationBrandId = $('#location ul li.selected').attr('lbid');
    var selectedLocation = $('#location li.selected').val();
    var tetherBrandId = $("#rightTetherTitle").attr('brandid');
    var BrandLocation = $('#location li.selected').text().split('-').length > 0 ? $('#location li.selected').text().split('-')[0] : '';
    var LoggedInUserName = $('#spnUserName span').eq(0).text();
    var RateSystem = $('#RateSystemSource').text();
    var LoggedInuserId = $('#LoggedInUserId').val();
    var SearchSummaryId = searchSummaryData.SearchSummaryID;
    var DominentBrand = '';
    if ($('#location ul li.selected').length > 0) {
        DominentBrand = $('#location ul li.selected').text().split('-')[1];
    }
    var dependentBrandName = $('#tetherValues #rightTetherTitle').attr('brandCode');
    var updateAllModel;
    updateCarList = [];
    rightGridUpdatedCarList = [];
    rentalLengthsList = [];
    leftDaysOfWeek = [];
    rightDaysOfWeek = [];
    var selectedCarClassArray = new Array();
    var SummaryIdCounter = 0;
    var tsdUpdateAll = new Object();
    //tsdUpdateAll.rentalLenghts = [];
    //tsdUpdateAll.CarClasses = [];
    $('#globalLimitError').hide();
    tsdUpdateAll.SearchSummaryId = GlobalLimitSearchSummaryData.SearchSummaryID;
    tsdUpdateAll.StartDate = $('#tsdStartDate').val();
    tsdUpdateAll.EndDate = $('#tsdEndDate').val();
    tsdUpdateAll.ExtraDayRateFactor = $('.extraDayRateFactor').val();
    tsdUpdateAll.UserId = LoggedInuserId;
    tsdUpdateAll.UserName = LoggedInUserName;
    tsdUpdateAll.LocationBrandId = LocationBrandId;
    tsdUpdateAll.LocationID = selectedLocation;
    tsdUpdateAll.TetherBrandId = tetherBrandId;
    tsdUpdateAll.BrandLocation = BrandLocation;
    tsdUpdateAll.DominentBrandName = DominentBrand;
    tsdUpdateAll.DependentBrandName = dependentBrandName;
    tsdUpdateAll.BaseRentalLength = $.trim($('#UpdateAllTSDLength li.selected').text());
    tsdUpdateAll.IsTetherActive = $('[id=IsTetherUser]').prop('checked');
    if ($('#updateAllLOR,#UpdateAllLOR_m,#UpdateAllLOR_ml').is(":visible")) {
        var $selectItem = $("#updateAllLOR li.selected");
        $($selectItem).each(function (index, element) {
            var lorList = new Object();
            lorList.LOR = $.trim($(this).text());
            lorList.ID = $(this).attr('value').toString();
            rentalLengthsList.push(lorList);
        });
    }
    else {
        var lorList = new Object();
        lorList.LOR = $.trim($('#UpdateAllTSDLength li.selected').text());
        lorList.ID = $.trim($('#UpdateAllTSDLength li.selected').attr('value')).toString();
        rentalLengthsList.push(lorList);
    }
    if ($('#weekUpdateAll').prop('checked')) {
        $('#TSDUpdateAll tbody tr').each(function () {
            if ($(this).find("td").find('input.BaseEdit').val() != '') {

                if (commaRemovedNumber($(this).find("td").find('input.BaseEdit').val()) != 0 && commaRemovedNumber($(this).find("td").find('input.BaseEdit').val()) > 0) {
                    var carList = new Object();
                    var carClassID = $(this).find("td").find('input.BaseEdit').attr('carClassid');
                    var dollerValue = $('#DollerID_' + carClassID).attr('defaultdollervalue');
                    var percentValue = $('#PercentageID_' + carClassID).attr('defaultPercentValue');
                    if (typeof (dollerValue) == "undefined") { dollerValue = ''; }
                    if (typeof (percentValue) == "undefined") { percentValue = ''; }
                    carList.ID = carClassID;
                    carList.Code = $(this).find("td").find('span').html().trim();
                    carList.BaseValue = commaRemovedNumber($(this).find("td").find('input.BaseEdit').val());
                    carList.TotalValue = commaRemovedNumber($(this).find("td").find('input.TotalEdit').val());
                    if (dollerValue != '') {
                        carList.TetherValue = dollerValue;
                        carList.IsDollar = true;
                    }
                    else if (percentValue != '') {
                        carList.TetherValue = percentValue;
                        carList.IsDollar = false;
                    }
                    updateCarList.push(carList);
                    if ($.inArray(carList.ID, selectedCarClassArray) == -1) {
                        selectedCarClassArray.push(carList.ID);
                    }
                }
                else {
                    ShowConfirmBox('Values entered for car classes should be greater than zero', false);
                    IsValid = false;
                    return false;
                }
            }
        });
        if (!IsValid) {
            return false;
        }
        leftDaysOfWeek.push('All');
    }
    else if ($('#leftGridDays li.selected').length > 0 || $('#rightGridDays li.selected').length > 0) {

        if ($('#leftGridDays li.selected').length > 0) {

            $('#TSDUpdateAll tbody tr').each(function () {
                if ($(this).find("td").find('input.BaseEdit').val() != '') {
                    if (commaRemovedNumber($(this).find("td").find('input.BaseEdit').val()) != 0 && commaRemovedNumber($(this).find("td").find('input.BaseEdit').val()) > 0) {
                        var carList = new Object();
                        var carClassID = $(this).find("td").find('input.BaseEdit').attr('carClassid');
                        var dollerValue = $('#DollerID_' + carClassID).attr('defaultdollervalue');
                        var percentValue = $('#PercentageID_' + carClassID).attr('defaultPercentValue');
                        if (typeof (dollerValue) == "undefined") { dollerValue = ''; }
                        if (typeof (percentValue) == "undefined") { percentValue = ''; }
                        carList.ID = carClassID;
                        carList.Code = $(this).find("td").find('span').html().trim();
                        carList.BaseValue = commaRemovedNumber($(this).find("td").find('input.BaseEdit').val());
                        carList.TotalValue = commaRemovedNumber($(this).find("td").find('input.TotalEdit').val());
                        if (dollerValue != '') {
                            carList.TetherValue = dollerValue;
                            carList.IsDollar = true;
                        }
                        else if (percentValue != '') {
                            carList.TetherValue = percentValue;
                            carList.IsDollar = false;
                        }
                        updateCarList.push(carList);
                        if ($.inArray(carList.ID, selectedCarClassArray) == -1) {
                            selectedCarClassArray.push(carList.ID);
                        }
                    }
                    else {
                        ShowConfirmBox('Values entered for car classes should be greater than zero', false);
                        IsValid = false;
                        return false;
                    }
                }
            });
            if (!IsValid) {
                return false;
            }
            $('#leftGridDays li.selected').each(function (index, element) {
                leftDaysOfWeek.push($.trim($(this).attr('day')).toString());
            });
        }
        if ($('#rightGridDays li.selected').length > 0) {

            $('#TSDUpdateAll tbody tr').each(function () {
                if ($(this).find("td").find('input.BaseEditrt').val() != '') {
                    if (commaRemovedNumber($(this).find("td").find('input.BaseEditrt').val()) != 0 && commaRemovedNumber($(this).find("td").find('input.BaseEditrt').val()) > 0) {
                        var carList = new Object();
                        var carClassID = $(this).find("td").find('input.BaseEditrt').attr('carClassid');
                        var dollerValue = $('#DollerID_' + carClassID).attr('defaultdollervalue');
                        var percentValue = $('#PercentageID_' + carClassID).attr('defaultPercentValue');
                        if (typeof (dollerValue) == "undefined") { dollerValue = ''; }
                        if (typeof (percentValue) == "undefined") { percentValue = ''; }
                        carList.ID = carClassID;
                        carList.Code = $(this).find("td").find('span').html().trim();
                        carList.BaseValue = commaRemovedNumber($(this).find("td").find('input.BaseEditrt').val());
                        carList.TotalValue = commaRemovedNumber($(this).find("td").find('input.TotalEditrt').val());
                        if (dollerValue != '') {
                            carList.TetherValue = dollerValue;
                            carList.IsDollar = true;
                        }
                        else if (percentValue != '') {
                            carList.TetherValue = percentValue;
                            carList.IsDollar = false;
                        }
                        rightGridUpdatedCarList.push(carList);
                        if ($.inArray(carList.ID, selectedCarClassArray) == -1) {
                            selectedCarClassArray.push(carList.ID);
                        }
                    }
                    else {
                        ShowConfirmBox('Values entered for car classes should be greater than zero', false);
                        IsValid = false;
                        return false;
                    }
                }
            });
            if (!IsValid) {
                return false;
            }

            $('#rightGridDays li.selected').each(function (index, element) {
                rightDaysOfWeek.push($.trim($(this).attr('day')).toString());
            });
        }
    }
    else {
        $('#TSDUpdateAll tbody tr').each(function () {
            if ($(this).find("td").find('input.BaseEdit').val() != '') {
                if (commaRemovedNumber($(this).find("td").find('input.BaseEdit').val()) != 0 && commaRemovedNumber($(this).find("td").find('input.BaseEdit').val()) > 0) {
                    var carList = new Object();
                    var carClassID = $(this).find("td").find('input.BaseEdit').attr('carClassid');
                    var dollerValue = $('#DollerID_' + carClassID).attr('defaultdollervalue');
                    var percentValue = $('#PercentageID_' + carClassID).attr('defaultPercentValue');
                    if (typeof (dollerValue) == "undefined") { dollerValue = ''; }
                    if (typeof (percentValue) == "undefined") { percentValue = ''; }
                    carList.ID = carClassID;
                    carList.Code = $(this).find("td").find('span').html().trim();
                    carList.BaseValue = commaRemovedNumber($(this).find("td").find('input.BaseEdit').val());
                    carList.TotalValue = commaRemovedNumber($(this).find("td").find('input.TotalEdit').val());
                    if (dollerValue != '') {
                        carList.TetherValue = dollerValue;
                        carList.IsDollar = true;
                    }
                    else if (percentValue != '') {
                        carList.TetherValue = percentValue;
                        carList.IsDollar = false;
                    }
                    updateCarList.push(carList);
                    if ($.inArray(carList.ID, selectedCarClassArray) == -1) {
                        selectedCarClassArray.push(carList.ID);
                    }
                }
                else {
                    ShowConfirmBox('Values entered for car classes should be greater than zero', false);
                    IsValid = false;
                    return false;
                }
            }
        });
        if (!IsValid) {
            return false;
        }
        leftDaysOfWeek.push('All');
    }
    var CarClassesFirst = new Object();
    CarClassesFirst.CarClasses = updateCarList;
    CarClassesFirst.CalendarDays = leftDaysOfWeek;

    var CarClassesSecond = new Object();
    CarClassesSecond.CarClasses = rightGridUpdatedCarList;
    CarClassesSecond.CalendarDays = rightDaysOfWeek;

    tsdUpdateAll.RentalLenghts = rentalLengthsList;
    tsdUpdateAll.CarClassesFirst = CarClassesFirst;
    tsdUpdateAll.CarClassesSecond = CarClassesSecond;

    updateAllModel = tsdUpdateAll;
    $('#updateAllNotification').text('Sending Rates to TSD...').show();

    var isGOV = false;
    if (GlobalLimitSearchSummaryData != null) {
        isGOV = GlobalLimitSearchSummaryData.IsGOV;
    }
    
    tsdUpdateAll.ExtraDayRateFactor = $('#updateAllExtraDayRate').val();

    var ajaxURl = '/RateShopper/TSDAudit/UpdateAllTSD/';
    if (TSDUpdateAllURL != undefined && TSDUpdateAllURL != '') {
        ajaxURl = TSDUpdateAllURL.UpdateAll;
    }
    if (updateCarList.length > 0 || rightGridUpdatedCarList.length > 0) {
        setTimeout(function () {
            $('#updateAllNotification').hide();
            if (!$('#globalLimitError').is(':visible')) {
                CloseTSDUpdate();
            }
        }, 1500);
        $('#updateAllSpinner').show();
        isUpdateSent = true

        var opaqueRatesConfiguration = null;
        if ($("#IsOpaqueChk").is(":checked")) {
            opaqueRatesConfiguration = SaveDailyViewOpaqueRates(selectedCarClassArray);
        }
       
        $.ajax({
            url: ajaxURl,
            type: 'POST',
            data: JSON.stringify({ 'tsdList': updateAllModel, 'isGOV': isGOV, 'opaqueRatesConfiguration': opaqueRatesConfiguration }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (data) {
                if (data.status) {
                    isUpdateSent = false;
                    FetchLastUpdateTSD();
                    savePreloadData = [];
                    var $lengthValue = $.trim($('#updateAllTSD_popup #UpdateAllTSDLength ul li.selected').text());
                    showHideRentalLengths($lengthValue.charAt(0));
                    highLight($lengthValue);
                    $('#TSDUpdateAll tr td input[type="text"]').val('');
                    $('#globalLimitError').hide();
                    $('#updateAllSpinner').hide();
                    clearDaysSelected();
                    $(".baserate_section_two").not(":visible").show();
                    //check if quick view report grid is open
                    if ($('#quickViewReportGrid tbody td').length > 0) {
                        if ($.trim($('#quickViewReportGrid tbody td span').eq(0).attr('searchsummaryid')) == $.trim(GlobalLimitSearchSummaryData.SearchSummaryID)) {
                            $('.quick-view-table tbody tr').each(function () {
                                if ($(this).attr('searchsummaryid') == $.trim(GlobalLimitSearchSummaryData.SearchSummaryID)
                                    || $(this).attr('childsearchsummaryid') == $.trim(GlobalLimitSearchSummaryData.SearchSummaryID)) {
                                    if ($(this).attr('id').split('_').length > 0) {
                                        var quickViewId = $(this).attr('id').split('_')[2];
                                        getQuickViewResults(quickViewId, GlobalLimitSearchSummaryData.SearchSummaryID);
                                    }
                                }
                            });
                        }
                    }
                }
                else {
                    isUpdateSent = false;
                    $('#updateAllSpinner').hide();
                    $('#updateAllNotification').hide();
                    $('#globalLimitError').html(data.message).show();
                }
            },
            error: function (e) {
                setTimeout(function () { $('#updateAllNotification').text('Something went wrong, please try again.').hide(); }, 1000);
                console.log(e.message);
            }
        });
    }
    else {
        $('#updateAllNotification').hide();
        $('#globalLimitError').html("Selected Car Classes should have value greater than 0").show();
    }

}


//Ajax function
function GetPreloaBaseRate() {
    var PreloadBaseRate = [];
    var lorLength = $('#UpdateAllTSDLength li.selected').text().trim();
    var rentalLengthSel = '';
    var isDaily = false;
    if (lorLength != '' && lorLength != null) {
        rentalLengthSel = lorLength.charAt(0);
    }
    if (SearchSummaryId > 0) {
        //if (prevLOR == undefined || prevLOR != '' || (rentalLengthSel.toUpperCase() != prevLOR.toUpperCase())) {
        if (rentalLengthSel.toUpperCase().indexOf('D') > -1) {
            isDaily = true;
        }

        $.ajax({
            url: 'Search/GetPreloaBaseRate',
            type: 'GET',
            data: { SearchSummaryID: SearchSummaryId, isDailyPreLoad: isDaily },
            async: true,
            dataType: 'json',
            success: function (data) {
                if (data.length > 0) {
                    PreloadBaseRate = data;
                    PreloadFillBaseRate(PreloadBaseRate);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                ///console.log(3);
            }
        });
        //}
        prevLOR = rentalLengthSel;
    }
}
//End Ajax function

function showHideRentalLengths(lor) {
    if (lor != null && lor != '') {
        $('#updateAllLOR li').each(function () {
            $this = $(this);
            if ($this.text().toUpperCase().indexOf(lor) >= 0) {
                $this.show();
            }
            else {
                $this.hide();
            }
        });

        //for mobile control
        $('#UpdateAllLOR_ml,#UpdateAllLOR_m').empty();
        $('#updateAllLOR li').each(function () {
            $this = $(this);
            if ($this.text().toUpperCase().indexOf(lor) >= 0) {
                $("<option value='" + $this.val() + "'>" + $this.text() + "</option>").appendTo('#UpdateAllLOR_m, #UpdateAllLOR_ml');
            }
        });
    }
}

function highLight(lorsel) {
    if (lorsel != null && lorsel != '') {
        var firstChar = lorsel.charAt(0);
        var digit = lorsel.match(/\d+/);
        $('#updateAllLOR li,#UpdateAllLOR_ml option,#UpdateAllLOR_m option').each(function () {
            $this = $(this);
            if ($this.text().toUpperCase().indexOf(firstChar) >= 0 && $this.text().toUpperCase().indexOf(digit) >= 0) {
                $this.addClass('selected').addClass('prevSelected');
                $this.prop("selected", true);//for selecte drop down
            }
            else {
                $this.removeClass('selected');
                $this.prop("selected", false);//for selecte drop down
            }
        });
    }
}


$('#UpdateAllLOR_ml').bind('change', function () {
    $('#UpdateAllLOR_ml option').each(function () {
        if ($(this).prop('selected')) {
            $('#UpdateAllLOR_m option[value=' + $(this).attr('value') + ']').prop('selected', true);
            $('#updateAllLOR li[value=' + $(this).attr('value') + ']').addClass('selected');
        } else {
            $('#UpdateAllLOR_m option[value=' + $(this).attr('value') + ']').prop('selected', false);
            $('#updateAllLOR li[value=' + $(this).attr('value') + ']').removeClass('selected');
        }
    });
});

$('#UpdateAllLOR_m').bind('change', function () {
    $('#UpdateAllLOR_m option').each(function () {
        if ($(this).prop('selected')) {
            $('#UpdateAllLOR_ml option[value=' + $(this).attr('value') + ']').prop('selected', true);
            $('#updateAllLOR li[value=' + $(this).attr('value') + ']').addClass('selected');
        } else {
            $('#UpdateAllLOR_ml option[value=' + $(this).attr('value') + ']').prop('selected', false);
            $('#updateAllLOR li[value=' + $(this).attr('value') + ']').removeClass('selected');
        }
    });
});

$('#leftGridDays_m').bind('change', function () {
    $('#leftGridDays_m option').each(function () {
        if ($(this).prop('selected')) {
            $('#leftGridDays li[value=' + $(this).attr('value') + ']').addClass('selected');

        } else {
            $('#leftGridDays li[value=' + $(this).attr('value') + ']').removeClass('selected');
        }
    });
    if ($("#leftGridDays").children().not('.selected').length == 0) {
        $("#leftGridDays").siblings('.UpdateAll_weeklyAll').prop('checked', true);
        $(".baserate_section_two").hide();
    }
    else {
        $("#leftGridDays").siblings('.UpdateAll_weeklyAll').prop('checked', false);
        $(".baserate_section_two").show();
    }
});

$('#rightGridDays_m').bind('change', function () {
    $('#rightGridDays_m option').each(function () {
        if ($(this).prop('selected')) {
            $('#rightGridDays li[value=' + $(this).attr('value') + ']').addClass('selected');
        } else {
            $('#rightGridDays li[value=' + $(this).attr('value') + ']').removeClass('selected');
        }
    });
});


var ValidateDaysSelected = function () {
    var isValueEnterInSecondSection = $(".baserate_section_two:visible input[type='text']").filter(function () { return $(this).val().length > 0 }).length > 0;

    var itemsSelectedFromFirstRow = $.map($(".days_section .row1 ul li.selected"), function (element) {
        return $(element).attr("value");
    });
    var itemsSelectedFromSecondRow = $.map($(".days_section .selectdays_row2 ul li.selected"), function (element) {
        return $(element).attr("value");
    });

    if (itemsSelectedFromFirstRow.length == 0 && itemsSelectedFromSecondRow.length == 0) {
        ShowConfirmBox('Please select the day(s) for the interval(s)', false);
        return false;
    }

    if (itemsSelectedFromFirstRow.length > 0 || itemsSelectedFromSecondRow.length > 0) {
        var startDate = $("#tsdStartDate").val();
        var stopDate = $("#tsdEndDate").val();
        if (startDate != "" && stopDate != "") {
            var currentDate = new Date(startDate);
            stopDate = new Date(stopDate);
            var isDateAndDaysMatch = false;
            while (currentDate <= stopDate) {
                //also check for selected days for overlapping     
                if ($.inArray(String(currentDate.getDay()), itemsSelectedFromFirstRow) > -1 || $.inArray(String(currentDate.getDay()), itemsSelectedFromSecondRow) > -1) {
                    isDateAndDaysMatch = true;
                    break;
                }
                currentDate = currentDate.addDays(1);
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

    if (isValueEnterInSecondSection) {
        if (itemsSelectedFromFirstRow.length > 0 && itemsSelectedFromSecondRow.length > 0) {
            var commondays = $.grep(itemsSelectedFromFirstRow, function (element) {
                return $.inArray(element, itemsSelectedFromSecondRow) !== -1;
            });
            if (commondays.length > 0) {
                ShowConfirmBox('Please select unique days', false);
                return false;
            }
            return true;
        }
        else {
            ShowConfirmBox('Please select days', false);
            return false;
        }
    }
    else if (!isValueEnterInSecondSection && itemsSelectedFromSecondRow.length > 0) {
        ShowConfirmBox('At least one car class must have Base Value for selected days for TSD Update.', false);
        return false;
    }
    return true;
}

function clearDaysSelected() {

    if ($('#leftGridDays li.selected').length > 0) {
        $('#leftGridDays li.selected').removeClass('selected');
    }

    if ($('#rightGridDays li.selected').length > 0) {
        $('#rightGridDays li.selected').removeClass('selected');
    }

    //same for mobile controls too;
    $('#leftGridDays_m option').each(function () {
        if ($(this).prop('selected')) {
            $(this).prop('selected', false);
        }
    });

    $('#rightGridDays_m option').each(function () {
        if ($(this).prop('selected')) {
            $(this).prop('selected', false);
        }
    });
    $('#weekUpdateAll').prop('checked', false);
}

function getExtraUpdateAll(locationBrandId, rentalLength) {
    var ajaxURl = 'TSDAudit/GetExtraDayRate/';
    if (TSDAjaxURL != undefined && TSDAjaxURL != '') {
        ajaxURl = TSDAjaxURL.GetExtraDayRate;
    }

    if (prevLOR == undefined || (rentalLength.toUpperCase() != prevLOR.toUpperCase())) {
        //need to fetch extra day rate if rental length changed
        $.ajax({
            url: ajaxURl,
            type: 'GET',
            async: true,
            data: { LocationBrandId: locationBrandId, rentalLength: rentalLength },
            dataType: 'json',
            success: function (data) {
                if (data) {
                    $('#updateAllExtraDayRate').val(data);
                    $('#updateAllExtraDayRate').attr('oldValue', data);
                }
            },
            error: function (e) {
                $('.loader_container_source').hide();
            }
        });
        prevLOR = rentalLength;
        if (rentalLength.toUpperCase().indexOf('D') >= 0) {
            $('#updateAllLengthText').html('Daily Rate');
            $('#updateCalc').html('*');
        }
        else if (rentalLength.toUpperCase().indexOf('W') >= 0) {
            $('#updateAllLengthText').html('Weekly Rate');
            $('#updateCalc').html('/');
        }
    }
    else {
        $('#updateAllExtraDayRate').val($('#updateAllExtraDayRate').attr('oldValue'));
    }
}

$('#updateAllExtraDayRate').bind('keyup', function () {
    var extraVal = $(this).val();
    //console.log(extraVal);
    if (!$.isNumeric(extraVal) || parseFloat(extraVal) <= 0) {
        MakeTagFlashable('#updateAllExtraDayRate');
        return false;
    }
    else {
        RemoveFlashableTag('#updateAllExtraDayRate');
    }
});