
$(document).ready(function () {
    $('#rangeStart1, #rangeStart2, #rangeEnd1, #rangeEnd2').datepicker();
});


function bindClassicData() {
    $('.loader_container_main').show();
    //clear Apply-filter selections
    clearApplySelections();

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
    $('.all-dates').removeClass('selected');
    $('.additionalBaseCol').hide();
    showAdditionalBase = false;
    getNDandLength($("#length li").html());
    var data = new Object();
    data.searchSummaryID = SearchSummaryId;
    data.scrapperSourceID = ($('#dimension-source li').eq(0).attr('value'));
    data.locationBrandID = $('#location ul li.selected').attr('lbid');
    data.locationID = $('#location ul li.selected').val();
    data.brandID = $('#location ul li.selected').attr('brandid');
    data.rentallengthID = $('#length li').eq(0).attr('value');
    data.carClassId = $('#carClass li').eq(0).attr('value');

    //alert(JSON.stringify(data));
    $.ajax({
        url: 'Search/GetSearchGridClassicViewData',
        type: 'GET',
        data: data,
        dataType: 'json',
        success: function (data) {
            $('.NoResultsFound').remove();
            bindClassicSearchGrid(data);
            $('.loader_container_main').hide();
            $("#search2letter").val('');
            RemoveFlashableTag("#search2letter");
            $('#TetherRate, #update, #updaten, #IsTetherUser, .resetEdit').prop('disabled', false).removeClass("disable-button");
            DisableTSDUpdateAccess();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            searchViewModel.ratesClassic(null);
            $('.classictable .company-logo').remove();
            $('.NoResultsFound').remove();
            $('<tr class="NoResultsFound"><td colspan="4" class="red bold" style="align:left">No Rates Found</td></tr>').appendTo('#daily-rates-table tbody');
            initializeClassicRangeFilter();
            $('.loader_container_main').hide();
            $("#search2letter").val('');
            RemoveFlashableTag("#search2letter");
            $('#TetherRate, #update, #updaten, #IsTetherUser, .resetEdit').prop('disabled', true).addClass("disable-button");
            SetSessionStorage();
            //alert(errorThrown);
            setLastUpdateTSD('');
        }
    });

    //highlight lor and change extra daily rate value
    var $lengthValue = $('#length li.selected').text().trim();
    enableRentalLengths($lengthValue.charAt(0));
    highLightLOR($lengthValue);
    highLight($lengthValue);
    getExtraDayRateValue($('#location ul li.selected').attr('lbid'), $lengthValue.charAt(0));
    //if not quickview shop and last day of shop is reached then show message Last Shop Day
    if (!isQuickViewShop) {
        if(($('#length ul li').length == $('#length ul li.selected').index() + 1) && ($('#carClass ul li').length == $('#carClass ul li.selected').index() + 1))
        {
            $(".spanlastdayshop").show();
           
        }
        else
        {
            $(".spanlastdayshop").hide();
        }
    }
}

function bindClassicSearchGrid(classicData) {
    brandID = classicData.brandID;
    var finalData = JSON.parse(classicData.finalData);

    var classicViewData = $.map(finalData.RatesInfo, function (item) { return new RateInfoClassic(item) });
    searchViewModel.ratesClassic(classicViewData);

    classicViewBrandCode = finalData.BrandCode;
    classicViewBrandID = finalData.BrandID;
    classicViewCarClassID = finalData.CarClassID;

    setLastUpdateTSD(classicData.lastTSDUpdated);

    setTimeout(function () {
        classicPostManipulation();
        bindSuggestedRatesClassic(classicData.suggestedRate);
        DisableTSDUpdateAccess();
    }, 200)
    //Call for check Tethervalue is empty or not
    setTimeout(function () {
        CheckTetherValueDiableButton();
        commonOpaqueRateBinding();
    }, 250);
    //For tethershop button first click and get the data.
}

function classicPostManipulation() {
    addEmptyTd();
    //Create column headers. (1,2,3)    
    createHeaders();
    hightlightBrand();
    bindNumberOnly();
    bindBaseTotalTextbox();
    //cursor hinting removed for performance issue
    //bindCursorHinting();
    bindGridSelectable();
    initializeClassicRangeFilter();
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
    $('.classicheader').append($("<td class='company-logo classicBrand' align='center'>" + classicViewBrandCode + "</td>"));

    var x = "<td class='company-logo cheader' align='center'>[[ID]]</td>";
    var totalTdToAdd = $(".classictable table tbody tr:first td").length - 6;
    for (var c = 1; c <= totalTdToAdd ; c++) {
        $('.classicheader').append(x.replace('[[ID]]', c));
    }
}

function hightlightBrand() {
    $('.classictable tbody tr').each(function (i, o) {
        var self = o;

        var selfindex = $(self).children().index($(self).children('td[companyID = ' + classicViewBrandID + ']:last'));
        var compindex = $(self).children().index($(self).find('.base-rate-blue').closest('td'));

        if (selfindex > 5) {
            if (compindex != -1 && (selfindex > compindex)) {
                $(self).children('td').eq(selfindex).html(classicViewBrandCode).addClass('classic-not-lowest');
            }
            else {
                $(self).children('td').eq(selfindex).html(classicViewBrandCode).addClass('classic-lowest');
            }
        }
    });
    //$('.classictable').unbind('mouseleave').bind('mouseleave', function () {
    //    $('.classictable td').removeClass('highlightCursorHitting');
    //});
}

function bindCursorHinting() {
    var rows1 = $('.classictable tr');
    rows1.children().bind('mouseover', function () {
        rows1.children().removeClass('highlightCursorHitting');
        var index = $(this).prevAll().length;
        if (index > 1 && $(this).closest("tr")[0].rowIndex > 0) {
            //console.log(rows.find(':nth-child(' + (index + 1) + ')').eq(0));
            rows1.find(':nth-child(' + (index + 1) + ')').eq(0).addClass('highlightCursorHitting');
            $(this).addClass('highlightCursorHitting').siblings().first().addClass('highlightCursorHitting');
        }
    });
}

function bindGridSelectable() {
    //Region for Rubber band feature start here
    //apply rubber band on date column
    $('.classictable .grid').selectable({
        filter: ".dates",
        selected: function (event, ui) {
            var $selected = $(ui.selected);
            if ($selected.hasClass('selected')) {
                var $allCarClasses = $('.all-dates');
                $selected.removeClass('selected');
                if ($allCarClasses.hasClass('selected')) {
                    $allCarClasses.removeClass('selected');
                }
            } else {
                $selected.addClass("selected");
            }
        },
        stop: function (event, ui) {
            if ($('td.dates').not('.selected').length == 0) {
                $('.all-dates').addClass('selected');
            }
        },
    });

    //select all carClasses when header clicked
    $('.all-dates').unbind('mousedown').bind('mousedown', function () {
        var $this = $(this);
        if ($this.hasClass('selected')) {
            $this.removeClass('selected');
            $('.classictable td.dates').removeClass("selected");
        } else {
            $('.classictable td.dates').addClass("selected");
            $this.addClass('selected');
        }
    });
}

function bindSuggestedRatesClassic(suggestedRate) {

    if (typeof formulaTtoB == "undefined") {
        //variable not exists, wait for some time 
        setTimeout(function () { bindSuggestedRatesClassic(suggestedRate); }, 100);
        return false;
    }

    //remove time part of the date
    suggestedRate.forEach(function (ele, i) {
        if (ele.Date != null) {
            ele.Date = convertToServerTime(new Date(parseInt(ele.Date.replace("/Date(", "").replace(")/", ""), 10)));
            ele.Date.setHours(0);
            ele.Date.setMinutes(0);

        }
    });



    //got the formula but empty; return
    if (formulaTtoB.trim() == '') {
        return false;
    }

    //for W8 to W11 calculate and bind value for 'Additional Base' rate    
    lengthFactor = parseInt($('#length>ul>li.selected').text().substring(1));
    if (lengthFactor != null && lengthFactor != '' && lengthFactor >= 8 && lengthFactor <= 11) {
        showAdditionalBase = true;
        $('.additionalBase').val('');
        $('.additionalBaseCol').show();
    }

    $('.glv').removeClass('glv');

    //disable 'update and next' button    
    //if ($('#carClass ul li').last().hasClass('selected') && $('#length ul li').last().hasClass('selected')) {
    //    $('[id^="updaten"]').prop('disabled', true).addClass("disable-button");
    //}
    //else {
    //    $('[id^="updaten"]').prop('disabled', false).removeClass("disable-button");
    //}
    //$('.perDay').html($('#length li').eq(0).html().trim().charAt(0));
    $('.arrow').removeAttr('class').addClass('arrow');
    $('.arrowb').removeAttr('class').addClass('arrowb');
    $('.ruleSetLink').val('').hide();
    $('.classictable tr[date]').each(function () {
        var self = $(this);
        var parentTr = self;
        suggestedRate.forEach(function (element, i) {
            if (element.Date == self.attr("date")) {

                var $baseEdit = parentTr.find(".baseEdit");

                parentTr.find(".suggestedRateId").attr('suggesteRtId', element.ID);

                //set min max base rate
                $baseEdit.attr('minBaseRate', element.MinBaseRate);
                $baseEdit.attr('maxBaseRate', element.MaxBaseRate);

                if (element.RuleSetID != 0 && $.isNumeric(element.TotalRate) && $.isNumeric(element.BaseRate) && element.TotalRate > 0 && element.BaseRate > 0) {

                    //calculate base value using suggested total value
                    //total = parseFloat(element.TotalRate);
                    //var base = parseFloat(evaluateFormula(formulaTtoB));
                    var total = parseFloat(element.TotalRate);
                    var base = parseFloat(element.BaseRate);


                    //compare base value with global limit and if exceed then set to global limit
                    //accordingly set total value based on global limit
                    var modifiedBaseVal = 0;
                    var modifiedTotalVal = 0;

                    if ($.isNumeric(element.MinBaseRate)) {
                        var minBase = parseFloat(element.MinBaseRate);
                        if (minBase >= base) {
                            modifiedBaseVal = minBase;
                            //$baseEdit.addClass('glv');
                            //MakeTagFlashable($baseEdit);
                        }
                    }
                    //check for max only if modifiedBaseVal is not set to min
                    if (modifiedBaseVal == 0 && $.isNumeric(element.MaxBaseRate)) {
                        var maxBase = parseFloat(element.MaxBaseRate);
                        if (maxBase > 0 && maxBase <= base) {
                            modifiedBaseVal = maxBase;
                            //$baseEdit.addClass('glv');
                            //MakeTagFlashable($baseEdit);
                        }
                    }

                    //if base value is modified then calculate total value accordingly
                    if (modifiedBaseVal > 0) {
                        rt = modifiedBaseVal;
                        modifiedTotalVal = parseFloat(evaluateFormula(formulaBtoT));
                        if (GlobalLimitSearchSummaryData != null && GlobalLimitSearchSummaryData.IsGOV) {
                            var selectedLor = $("#result-section #length ul li.selected").eq(0).val();
                            if (typeof (selectedLor) != 'undefined' && selectedLor > 0) {
                                modifiedTotalVal = GetGovernmentRate(modifiedTotalVal, selectedLor, false);
                            }
                        }
                    }
                    else {
                        modifiedBaseVal = base;
                        modifiedTotalVal = total;
                    }

                    var $totalEdit = parentTr.find(".totalEdit");

                    $baseEdit.attr('suggetedOriginalValue', modifiedBaseVal.toFixed(2)).val(commaSeparateNumber(modifiedBaseVal.toFixed(2))).addClass('required');
                    $totalEdit.attr('suggetedOriginalValue', modifiedTotalVal.toFixed(2)).val(commaSeparateNumber(modifiedTotalVal.toFixed(2))).addClass('required');

                    //for W8 to W11 calculate and bind value for 'Additional Base' rate                    
                    if (showAdditionalBase) {
                        var additionalBase = calculateAdditionalBase(modifiedBaseVal);
                        if (GlobalLimitSearchSummaryData != null && GlobalLimitSearchSummaryData.IsGOV) {
                            additionalBase = parseInt(additionalBase);
                        }
                        var ab = commaSeparateNumber(additionalBase.toFixed(2));
                        parentTr.find(".additionalBase").attr('suggetedoriginalvalue', ab).val(ab);
                    }

                    parentTr.find(".ruleSetLink").attr("value", element.RuleSetName).attr("RuleSetID", element.RuleSetID).show();
                    parentTr.find(".ruleSetLink").text(element.RuleSetName);

                    //bind up down arrow, arrowb for suggested rates, get total value for my company from grid
                    var companyTd = parentTr.find('td[companyid=' + brandID + ']').first();
                    var totalvalue = parseFloat(companyTd.find('.tv').html());

                    var basevalue = parseFloat(companyTd.find('.daily-rate').html());

                    //console.log(total + 'tv' + totalvalue + ' fd ' + (total < totalvalue));

                    if ($.isNumeric(totalvalue) && $.isNumeric(basevalue)) {
                        if (modifiedTotalVal < totalvalue) {
                            parentTr.find('.arrow').addClass('checked');
                        }
                        else if (modifiedTotalVal > totalvalue) {
                            parentTr.find('.arrow').addClass('checked-gr');
                        }
                        else {
                            parentTr.find('.arrow').addClass('checked-ok');
                        }

                        if (modifiedBaseVal < basevalue) {
                            parentTr.find('.arrowb').addClass('checked');
                        }
                        else if (modifiedBaseVal > basevalue) {
                            parentTr.find('.arrowb').addClass('checked-gr');
                        }
                        else {
                            parentTr.find('.arrowb').addClass('checked-ok');
                        }
                    }
                }
                else {
                    parentTr.find(".totalEdit").attr('suggetedOriginalValue', '').val('');
                    parentTr.find(".baseEdit").attr('suggetedOriginalValue', '').val('');
                }
            }
        });
    });

    SetSessionStorage();
    setTimeout(function () { AddFlashingEffect(); }, 1000);
}

function initializeClassicRangeFilter() {

    //Base value to total value
    $('.totalEditRange').bind('keyup', function () {
        var self = $(this);
        var val = commaRemovedNumber(self.val());
        if ($.isNumeric(val)) {
            var parentdiv = self.closest('div');
            var bv = parentdiv.find(".baseEditRange");
            total = val;
            if (GlobalLimitSearchSummaryData.IsGOV) {
                var selectedLor = $("#result-section #length ul li.selected").eq(0).val();
                if (typeof (selectedLor) != 'undefined' && selectedLor > 0) {
                    total = GetGovernmentRate(total, selectedLor, true);
                }
            }
            var baseValue = evaluateFormula(formulaTtoB, true);
            if (showAdditionalBase) {
                baseValue = calculateNewBase(false, baseValue);
                var additionalBase = calculateAdditionalBase(baseValue);
                if (GlobalLimitSearchSummaryData.IsGOV) {
                    additionalBase = parseInt(additionalBase);
                }
                parentdiv.find(".additionalBaseRange").val(commaSeparateNumber(additionalBase.toFixed(2)));
            }
            if (GlobalLimitSearchSummaryData.IsGOV) {
                baseValue = parseInt(baseValue);
            }
            bv.val(commaSeparateNumber(baseValue.toFixed(2)));
            RemoveFlashableTag(self);
            RemoveFlashableTag(bv);
            AddFlashingEffect();
        }
    });

    //total to base
    $('.baseEditRange').bind('input', function () {
        var self = $(this);
        if (typeof(GlobalLimitSearchSummaryData) != 'undefined' && GlobalLimitSearchSummaryData.IsGOV && self.val().indexOf(".") > -1) {
            var valArray = self.val().split(".");
            if (valArray.length > 1 && valArray[1] > 0) {
                var val = parseInt(self.val());                
                self.val(val.toFixed(2));
            }
           
        }
        var val = commaRemovedNumber(self.val());
        if ($.isNumeric(val)) {
            var parentdiv = self.closest('div');
            if (showAdditionalBase) {
                var additionalBase = calculateAdditionalBase(val);
                if (GlobalLimitSearchSummaryData.IsGOV) {
                    additionalBase = parseInt(additionalBase);
                }
                parentdiv.find(".additionalBaseRange").val(commaSeparateNumber(additionalBase.toFixed(2)));
                val = calculateNewBase(true, val);
            }
            rt = val;
            var tv = parentdiv.find(".totalEditRange");
            var totalValue = evaluateFormula(formulaBtoT, true)
            if (GlobalLimitSearchSummaryData.IsGOV) {
                var selectedLor = $("#result-section #length ul li.selected").eq(0).val();
                if (typeof (selectedLor) != 'undefined' && selectedLor > 0) {
                    totalValue = GetGovernmentRate(totalValue, selectedLor, false);
                }
            }
            tv.val(commaSeparateNumber(totalValue.toFixed(2)));

            RemoveFlashableTag(self);
            RemoveFlashableTag(tv);
            AddFlashingEffect();
        }
    });
    //day selectable
    $(".weekly_large").selectable({
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
                $(this).siblings('.weeklyAll').prop('checked', true);
            }
            else {
                $(this).siblings('.weeklyAll').prop('checked', false);
            }
        },
    });


    var startDate = new Date($('.classictable tbody tr:first').attr('date'));
    var endDate = new Date($('.classictable tbody tr:last').attr('date'));
    if ($('.classictable tbody tr:first').attr('date') != undefined) {
        $('#rangeStart1,#rangeStart2, #rangeEnd1, #rangeEnd2').removeAttr('disabled').css('cursor', 'pointer');
        $('#rangeStart1').datepicker('option', { dateFormat: 'mm/dd/yy', defaultDate: startDate, minDate: startDate, maxDate: endDate });
        $('#rangeStart2').datepicker('option', { dateFormat: 'mm/dd/yy', defaultDate: startDate, minDate: startDate, maxDate: endDate });
        $('#rangeEnd1').datepicker('option', { dateFormat: 'mm/dd/yy', defaultDate: endDate, minDate: startDate, maxDate: endDate });
        $('#rangeEnd2').datepicker('option', { dateFormat: 'mm/dd/yy', defaultDate: endDate, minDate: startDate, maxDate: endDate });
    }
    else {
        $('#rangeStart1,#rangeStart2, #rangeEnd1, #rangeEnd2').attr('disabled', 'disabled').css('cursor', 'default');
    }


    //check box for all
    $('.weeklyAll').click(function () {
        if ($(this).is(':checked')) {
            $(this).siblings('ul').children().addClass('selected');
        }
        else {
            $(this).siblings('ul').children().removeClass('selected');
        }

    });

    $('.apply-button').unbind('click').bind('click', function (event) {
        //validate Apply Filter
        if (validateApplyFilter()) {

            applyFilter('.row1');
            applyFilter('.row2');
            if ($('td.dates').not('.selected').length == 0) {
                $('.all-dates').addClass('selected');
            }
            else {
                $('.all-dates').removeClass('selected');
            }
            clearApplySelections();


        }
    });
}

/*Create array of days using start date and end date*/
Date.prototype.addDays = function (days) {
    var dat = new Date(this.valueOf())
    dat.setDate(dat.getDate() + days);
    return dat;
}

function getDates(startDate, stopDate, selector) {

    //get selected days
    var dayArray = new Array();
    $(selector).find('.selected').each(function () {
        dayArray.push(parseInt($(this).attr('value')));
    });

    var dateArray = new Array();
    var currentDate = new Date(startDate);
    stopDate = new Date(stopDate);
    while (currentDate <= stopDate) {
        //also check for selected days for overlapping     
        if ($.inArray(currentDate.getDay(), dayArray) > -1) {
            dateArray.push(currentDate.getTime());
        }
        currentDate = currentDate.addDays(1);
    }
    return dateArray;
}
/*end here*/

function clearApplySelections() {
    //Clear selection
    $('.baseEditRange, .totalEditRange, .additionalBaseRange, .rangeStart, .rangeEnd').val('');
    $('.row1, .row2').find('.selected').removeClass('selected');
    $('.weeklyAll').prop('checked', false);

    $('#filterError').html('').hide();
    RemoveFlashableTag('.baseEditRange, .totalEditRange, .row1.rangeStart, .row1.rangeEnd, .row2.rangeStart, .row2.rangeEnd');
    AddFlashingEffect();
}

function validateApplyFilter() {

    $('#filterError').html('').hide();
    RemoveFlashableTag('.row1.rangeStart, .row1.rangeEnd, .row2.rangeStart, .row2.rangeEnd');

    //get all dates in interval into a range1   
    var rangeStartDate1 = Date.parse($('.row1.rangeStart').val());
    var rangeEndDate1 = Date.parse($('.row1.rangeEnd').val());

    //if null then get default date
    if ($('.row1.baseEditRange').val() != '' && isNaN(rangeStartDate1)) {
        rangeStartDate1 = new Date($('.classictable tbody tr:first').attr('date'));
    }
    if ($('.row1.baseEditRange').val() != '' && isNaN(rangeEndDate1)) {
        rangeEndDate1 = new Date($('.classictable tbody tr:last').attr('date'));
    }

    var range1 = getDates(rangeStartDate1, rangeEndDate1, '.row1');

    //get all dates in interval into a range2
    var rangeStartDate2 = Date.parse($('.row2.rangeStart').val());
    var rangeEndDate2 = Date.parse($('.row2.rangeEnd').val());

    //if null then get default date
    if ($('.row2.baseEditRange').val() != '' && isNaN(rangeStartDate2)) {
        rangeStartDate2 = new Date($('.classictable tbody tr:first').attr('date'));
    }
    if ($('.row2.baseEditRange').val() != '' && isNaN(rangeEndDate2)) {
        rangeEndDate2 = new Date($('.classictable tbody tr:last').attr('date'));
    }

    var range2 = getDates(rangeStartDate2, rangeEndDate2, '.row2');

    //get common days
    var common = $.grep(range1, function (element) {
        return $.inArray(element, range2) !== -1;
    });

    var isValid = true;

    if (rangeStartDate1 > rangeEndDate1 || rangeStartDate2 > rangeEndDate2) {
        $('#filterError').html("'Range start' must less than or equal to 'Range End'.");
        isValid = false;

        if (rangeStartDate1 > rangeEndDate1) {
            MakeTagFlashable($('.row1.rangeStart, .row1.rangeEnd'));
        }
        if (rangeStartDate2 > rangeEndDate2) {
            MakeTagFlashable($('.row2.rangeStart, .row2.rangeEnd'));
        }
    }

    //check for common days
    if (common.length > 0) {
        $('#filterError').append(" Please select unique days for the intervals.");
        isValid = false;
    }
    //check for days selection if date is correct
    if ((rangeStartDate1 <= rangeEndDate1 && $('.row1').find('.selected').length <= 0)
        || (rangeStartDate2 <= rangeEndDate2 && $('.row2').find('.selected').length <= 0)) {

        $('#filterError').append("Please select the day(s) for the interval(s).");
        isValid = false;
    }

    //check for base and total values
    if ((range1.length > 0 && ($('.row1.baseEditRange').val() == '')) || (range2.length > 0 && ($('.row2.baseEditRange').val() == ''))) {
        $('#filterError').append(" Please add 'Base' and 'Total' for the interval(s).");
        isValid = false;
    }
    if (!isValid) {
        $('#filterError').show();
    }
    else {
        //now check for grid valid record for selected filter
        if ((!isNaN(rangeStartDate1) && !isNaN(rangeEndDate1) && range1.length <= 0)
            || (!isNaN(rangeStartDate2) && !isNaN(rangeEndDate2) && range2.length <= 0)) {

            $('#filterError').append(" No record found for the interval(s)").show();
            isValid = false;
        }
    }
    AddFlashingEffect();
    return isValid;
}

function applyFilter(selector) {
    var rateBase = $('.baseEditRange' + selector + '').val();
    var rateAdditionalBase = $('.additionalBaseRange' + selector + '').val();
    var rateTotal = $('.totalEditRange' + selector + '').val();
    var rangeStartDate = Date.parse($('.rangeStart' + selector + '').val());
    var rangeEndDate = Date.parse($('.rangeEnd' + selector + '').val());

    if (rateBase != null && rateBase.length > 0 && rateTotal != null && rateTotal.length > 0) {
        if ($(selector + ' input[type="checkbox"]').is(':checked')) {
            $('.classictable > tbody > tr').each(function () {
                var targetDate = new Date($(this).attr('date'));
                if ((!rangeStartDate || rangeStartDate <= targetDate) && (!rangeEndDate || rangeEndDate >= targetDate)) {
                    $(this).find('.baseEdit').val(rateBase);
                    if (showAdditionalBase) {
                        $(this).find('.additionalBase').val(rateAdditionalBase)
                    }
                    $(this).find('.totalEdit').val(rateTotal);
                    $(this).find('.baseEdit').trigger('focusout');
                    $(this).find('.dates').addClass('selected');
                }
            });
        } else {
            $(selector).find('.selected').each(function () {
                var day = $(this).attr('value');
                $('.classictable > tbody > tr').each(function () {
                    var targetDate = new Date($(this).attr('date'));
                    var targetDay = targetDate.getDay();
                    if ((!rangeStartDate || rangeStartDate <= targetDate) && (!rangeEndDate || rangeEndDate >= targetDate) && targetDay == day) {
                        $(this).find('.baseEdit').val(rateBase);
                        if (showAdditionalBase) {
                            $(this).find('.additionalBase').val(rateAdditionalBase)
                        }
                        $(this).find('.totalEdit').val(rateTotal);
                        $(this).find('.baseEdit').trigger('focusout');
                        $(this).find('.dates').addClass('selected');
                    }
                });
            });
        }
    }
}
function nextCarClassClicked() {
    if ($('#carClass ul li').length != $('#carClass ul li.selected').index() + 1) {
        $('#carClass ul li.selected').removeClass('selected').next().addClass('selected');
        $('#carClass li').eq(0).text($('#carClass ul li.selected').text()).attr('value', $('#carClass ul li.selected').attr('value'));
        bindClassicData();
        if ($('#carClass ul li').length == $('#carClass ul li.selected').index() + 1) {
            //$('[id=updaten]').prop('disabled', true).addClass("disable-button");
        }
    }
    else {
        //$('#carClass ul li.selected').removeClass('selected')
        //$('#carClass ul li').eq(0).addClass('selected');
        //$('[id=updaten]').prop('disabled', true).addClass("disable-button");
        var $lengthValue = $('#length li.selected').text().trim();
        enableRentalLengths($lengthValue.charAt(0));
        highLightLOR($lengthValue);
        highLight($lengthValue);
    }

}

