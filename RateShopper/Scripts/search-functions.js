/********# '_m' for mobile; '_ml' for mobile landscape mode#***********/
var brandID = 0;
var checkedValCnt = 0;
var weekCheckedCnt = 0;
var monthCheckedCnt = 0;
var tsdModel = new Array();
var tetherModel = new Array();
var selectedCarClasses = [];
var ajaxURl = '/RateShopper/TSDAudit/TSDUpdate/';
var checkIfRateFetched = false;
var tetherPrevRental = '';
var showAdditionalBase = false;
var lengthFactor = 0;
var loadingPrevResult = false;
var isQuickViewShop;
var IsQuickUpdateAndNext = false;//Use for while user has to clicks on update/update& next button then we have to call ignore & next in quickview shop
var currentView;
var governmentFluctuationRate = 5;
var isDominantBrandSelectedForDrag = false;

$(document).ready(function () {
    if (typeof ($("#hdnGARSFees").val()) != 'undefined') {
        governmentFluctuationRate = $("#hdnGARSFees").val();
    }
    setCustomDropdownKeyPress();

    //Check navigation in ipad hide viewselect drop down
    if (navigator.userAgent.match(/iPad/i) != null) {
        $(".viewSelect").hide();
    }

    CheckSessionStorageNLoadData();

    $("#ExportToCSV").click(function (e) { GetSearchShopCSV(e) });
    DisableTSDUpdateAccess();

    $('.loader_container').hide();

    $("#txtDragGapValue").on("input", function () {
        $("input[name='gapvalue']").prop('checked', false);
    });

    $("input[name='gapvalue']").on('change', function () {
        $("#txtDragGapValue").val('');
    });

    //Two letter search code
    $("#search2letter").keyup(function (e) {
        var searchedLetter = $.trim($(this).val().toLowerCase());
        if (searchedLetter.length > 1) {
            $("#daily-rates-table span.cname").each(function () {
                if ($(this).html().toLowerCase().indexOf(searchedLetter) == 0) {
                    $(this).addClass("twolettersearch");
                }
                else {
                    $(this).removeClass("twolettersearch");
                }
            });

            if ($("#daily-rates-table span.twolettersearch").length == 0) {
                MakeTagFlashable("#search2letter");
            }
            else {
                RemoveFlashableTag("#search2letter");
            }
            AddFlashingEffect();
        }
        else {
            $("#daily-rates-table span.twolettersearch").removeClass("twolettersearch");
            RemoveFlashableTag("#search2letter");
        }
    });

    //Device View: full Screen toggling effects
    $('.mfullscr').click(function () {
        $('#mobile-header, .body-section-top').slideToggle("slow");
        if ($('.mfullscr').attr('src').indexOf('mfullscr.png') > 0) {
            $('.mfullscr').attr('src', 'images/mfullscrOut.png');
        }
        else {
            $('.mfullscr').attr('src', 'images/mfullscr.png');
        }
    });


    //Device View: Show hide search panel
    $('.mobileSearchICO').unbind('click').bind('click', function () {
        if ($('#left-col').is(':visible')) {
            $('#left-col').hide();
            $('#right-col').show();
        }
        else {
            $('#left-col').show();
            $('#right-col').hide();

            //For mobile, click on relevent search summary. Also scroll
            //summaries to relevent search
            var $pastSearchLi = $('.pastSearchul li[value="' + SearchSummaryId + '"]');
            $pastSearchLi.click();

            //showQuickViewControl($pastSearchLi.attr('isQuickView'));
            //bindlengthDateCombination(SearchSummaryId);

            var container = $('.pastSearchul');
            scrollTo = $pastSearchLi;
            if (scrollTo.length > 0) {
                container.scrollTop(
                    scrollTo.offset().top - 30 - container.offset().top + container.scrollTop()
                );
            }
        }
    });

    if ($(window).width() <= 768) {
        if ($(window).height() > $(window).width()) {
            $('.portrait').show();
            //$('.landscape').hide();
        }
        else {
            $('.portrait').hide();
            // $('.landscape').show();
        }
    }


    //Not for apple device: if user changes the orientation of device and only for tablet  then load both search and grid panel (default view)
    $(window).on("orientationchange", function (event) {

        //if in landscape and mobile then hide the 'portrait' class because it zooming the screen                      
        setTimeout(function () {
            if ($(window).height() > $(window).width() && $(window).width() <= 768) {
                $('.portrait').show();
                $('.landscape').hide();
            }
            else {
                $('.portrait').hide();
                $('.landscape').show();
            }
        }, 250);

        setTimeout(function () {
            if ($('#LOR_ml').is(':visible')) {
                $('#LOR_m option').each(function () {
                    if ($(this).prop('selected')) {
                        $('#LOR_ml option[value="' + $(this).attr('value') + '"]').prop('selected', true);
                    }
                    else {
                        $('#LOR_ml option[value="' + $(this).attr('value') + '"]').prop('selected', false);
                    }
                });
            }
            if ($('#LOR_m').is(':visible')) {
                $('#LOR_ml option').each(function () {
                    if ($(this).prop('selected')) {
                        $('#LOR_m option[value="' + $(this).attr('value') + '"]').prop('selected', true);
                    }
                    else {
                        $('#LOR_m option[value="' + $(this).attr('value') + '"]').prop('selected', false);
                    }
                });
            }
        }, 250);


        setTimeout(function () {
            if (/Android|webOS|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {

                //if horizontal then load web view; if vertical then load mobile view
                if ($(window).width() > 767 && ($(window).width() > $(window).height())) {
                    $('#mobile-header').hide();


                    //console.log( "This device is in " + event.orientation + " mode!" );                                        
                    $('#left-col').show('slow');
                    $('#right-col').show('slow');
                }
                else if ($(window).height() > 767 && ($(window).width() < $(window).height())) {

                    $('#mobile-header').show();
                    $('#mobile-header').show();
                    $('#left-col').show('slow');
                    $('#right-col').hide('slow');
                }
            }
        }, 500);
    });

    //TSD Update 
    $('[id=update]').unbind('click').bind('click', function (e) {

        //show violation Popup if selected record crossed global limit
        var showViolationPopup = false;
        if ($('.glv').length > 0) {
            $('.glv').each(function () {
                if ($(this).closest('tr').find('td.selected').length > 0) {
                    showViolationPopup = true;
                    return;
                }
            });
        }
        if (showViolationPopup) {
            var message = "Rate Violation (minimum or maximum) has occurred.";
            ShowConfirmBox(message, false);
        }
        else {
            UpdateClickCallBack(e);
        }
        //if (!acceptGlobalLimit()) {
        //    return;
        //}

    });

    $('[id=updaten]').click(function (e) {

        //show violation Popup if selected record crossed global limit
        var showViolationPopup = false;
        if ($('.glv').length > 0) {
            $('.glv').each(function () {
                if ($(this).closest('tr').find('td.selected').length > 0) {
                    showViolationPopup = true;
                    return;
                }
            });
        }
        if (showViolationPopup) {
            var message = "Rate Violation (minimum or maximum) has occurred.";
            ShowConfirmBox(message, false);
        }
        else {
            UpdateNextClickCallBack(e);
        }
        //if (!acceptGlobalLimit()) {
        //    return;
        //}

    });

    //detect phone device and change text of submit button
    if (/Android|webOS|iphone|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {

        //if horizontal then load web view; if vertical then load mobile view 
        if ($(window).width() < 767) {
            $('#closeTetherPopup').val('Ok');
            $('#closeTetherPopup').removeClass('orng fright pointer closeP');
        }
    }

    //Get url to update TSD used for daily/Classic/Tethered

    if (TSDAjaxURL != undefined && TSDAjaxURL != '') {
        ajaxURl = TSDAjaxURL.TSDUpdateRate;
    }
    //end

    $('.extraDayRateFactor').bind('keyup', function () {
        var extraVal = $(this).val();
        $('.extraDayRateFactor').each(function () {
            $(this).val(extraVal);
        });
        //validateDecimal($(this));
        if (!$.isNumeric(extraVal) || parseFloat(extraVal) <= 0) {
            MakeTagFlashable($(this));
            AddFlashingEffect();
            return false;
        }
        else {
            RemoveFlashableTag($(this));
            AddFlashingEffect();
        }
    });

    $('.resetEdit').click(function () { resetEditClicked(); });
    var prvcode = $("select#source option:selected").attr("prvcode");
    if (typeof (prvcode) != 'undefined') {
        EnableDisableMultipleLOR(prvcode, $("select#source option:selected").attr("isgov"));
        //LoadScrapperSource(selectedAPI);
    }
    $("select#source").change(function () {
        EnableDisableMultipleLOR($("select#source option:selected").attr("prvcode"), $("select#source option:selected").attr("isgov"));
        //LoadScrapperSource($(this).attr("value"));
    });


    $(".view-tabs").on('click', 'li', function (e) {


        var self = $(this);



        var $target = $(self.data('target'));
        var $targetParent = $target.parent();
        $targetParent.children().hide();
        $target.show();


        var $selfParent = self.parent();
        $selfParent.children().removeClass('selected');
        self.addClass('selected');


    });

    $("#mobile-header").on('click', 'img', function (e) {


        var self = $(this);
        var $target = $(self.data('target'));
        var $targetParent = $target.parent();
        $targetParent.children().hide();
        $target.show();
    });

    //$('.search-results').on('click', '.quick-view-button', function (e) {
    //    ResetQuickViewSchedulePopup();
    //    var locationBrandId = $("#result-section ul#location .hidden li.selected").attr("lbid");
    //    if (typeof (locationBrandId) != 'undefined' && locationBrandId > 0) {
    //        alert("2");
    //        GetQuickViewCompetitors(locationBrandId, SearchSummaryId, GlobalLimitSearchSummaryData.CarClassIDs);
    //    }

    //    $(".quickview-schedule-modal").removeClass('hidden').show();
    //    $(".modal-backdrop").removeClass('hidden').show();
    //    e.preventDefault();
    //});

    //$('.quickview-schedule-popup').on('click', '.close-quick-view-popup', function (e) {
    //    ResetQuickViewSchedulePopup();
    //    e.preventDefault();
    //    $(".quickview-schedule-modal").addClass('hidden').hide();
    //    $(".modal-backdrop").addClass('hidden').hide();

    //});

    $('.collapse-anchor').click(function () {


        var self = $(this);
        var $target = $(self.data('collapse-target'));
        var image = self.find('img');

        handleImageSrc.call(image);


        function handleImageSrc() {
            if ($(this).attr('src').indexOf('expand') > 0) {
                $(this).attr('src', 'images/Search-collapse.png');
            } else {
                $(this).attr('src', 'images/expand.png')
            }

        }

        $target.slideToggle();
    });

    $('[name^="quickview-change-status"]').change(function () {
        changeUnChangeClicked($(this).attr('id'));
    });

    $('#chkAbovePos').bind('click', function () {
        if ($(".drag_bar").length > 0) {
            DragMouseUpEvent($(".dropped_bar"));
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


function UpdateClickCallBack(e) {
    console.log("------------------------------------------------");
    var t = new Date(); console.log("Update click event at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
    if (typeof (e) == 'undefined') {
        e = this;
    }

    if ($('.glv').length > 0) {
        $('.glv').each(function () {
            if ($(this).closest('tr').find('td.selected').length > 0) {
                $(this).removeClass('glv');
                RemoveFlashableTag($(this));
                return;
            }
        });
    }

    var viewSelected = $('#viewSelect li.selected').text().trim();
    e.preventDefault();
    var validTran = true;
    if (validateLorSelected(viewSelected)) {
        if (!$('.extraDayRateFactor').hasClass('temp')) {
            if (viewSelected == 'daily' && $('.dailytable .car-class-img.selected').closest('tr').find('input.temp').not('.glv').length <= 0) {
                $('#daily-rates-table .car-class-img.selected').each(function () {
                    if ($(this).parent().find('.baseEdit').val().trim() == '' || isNaN(parseInt($(this).parent().find('.baseEdit').val()))
                     || parseInt($(this).parent().find('.baseEdit').val()) <= 0) {
                        validTran = false;
                        return false;
                    }
                });
                if (validTran) {
                    validTran = readDailyViewRates();
                    if (!$('[id=IsTetherUser]').prop('checked')) {
                        $('#daily-rates-table .selected').removeClass('selected');
                    }
                }
                else {
                    ShowConfirmBox('Please enter valid rates for selected car classes', false);
                    return false;
                }
            }
            else if (viewSelected == 'classic' && $('.classictable .dates.selected').closest('tr').find('input.temp').not('.glv').length <= 0) {
                $('.classictable .dates.selected').each(function () {
                    if ($(this).parent().find('.baseEdit').val().trim() == '' || isNaN(parseInt($(this).parent().find('.baseEdit').val()))
                     || parseInt($(this).parent().find('.baseEdit').val()) <= 0) {
                        validTran = false;
                        return false;
                    }
                });
                if (validTran) {
                    validTran = readClassicViewRates();
                    if (!$('[id=IsTetherUser]').prop('checked')) {
                        $('table.classictable .ui-selectee').removeClass('selected');
                    }
                }
                else {
                    ShowConfirmBox('Please enter valid rates for selected Dates', false);
                    return false;
                }
            }
            else {
                validTran = false;
                ShowConfirmBox('Please review the fields highlighted in Red.', false);
                return false;
            }
        }
        else {
            validTran = false;
            ShowConfirmBox('Please review the fields highlighted in Red.', false);
            return false;
        }
        if (!$('[id=IsTetherUser]').prop('checked') && validTran) {
            resetEdit();
        }

        if ($('[id=IsTetherUser]').prop('checked') && validTran) {
            TetherRateButton();
            //getLocationBrand(true);
            //var viewSelected = $('#viewSelect li.selected').text().trim();
            if (viewSelected == 'daily') {
                readTetheredRates(true);
            }
            else if (viewSelected == 'classic') {
                readClassicTetheredRates(true);
            }
        }

    }
}

function UpdateNextClickCallBack(e) {
    console.log("------------------------------------------------");
    var t = new Date(); console.log("Update & next click event at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
    if (typeof (e) == 'undefined') {
        e = this;
    }

    if ($('.glv').length > 0) {
        $('.glv').each(function () {
            if ($(this).closest('tr').find('td.selected').length > 0) {
                $(this).removeClass('glv');
                RemoveFlashableTag($(this));
                return;
            }
        });
    }

    var view = $('#viewSelect li.selected').text().trim();
    e.preventDefault();
    var validTran = true;
    if (validateLorSelected(view)) {
        if (!$('.extraDayRateFactor').hasClass('temp')) {
            if (view == 'daily' && $('.dailytable .car-class-img.selected').closest('tr').find('input.temp').not('.glv').length <= 0) {
                $('#daily-rates-table .car-class-img.selected').each(function () {
                    if ($(this).parent().find('.baseEdit').val().trim() == '' || isNaN(parseInt($(this).parent().find('.baseEdit').val()))
                        || parseInt($(this).parent().find('.baseEdit').val()) <= 0) {
                        validTran = false;
                        return false;
                    }
                });
                if (validTran) {
                    validTran = readDailyViewRates();
                    if (!$('[id=IsTetherUser]').prop('checked')) {
                        $('#daily-rates-table .selected').removeClass('selected');
                    }
                    //update quick view shop status
                    if (isQuickViewShop) {
                        IsQuickUpdateAndNext = true;
                        IgnoreAndNext();
                    }
                }
                else {
                    ShowConfirmBox('Please enter valid rates for selected car classes', false);
                    return false;
                }
            }
            else if (view == 'classic' && $('.classictable .dates.selected').closest('tr').find('input.temp').not('.glv').length <= 0) {
                $('.classictable .dates.selected').each(function () {
                    if ($(this).parent().find('.baseEdit').val().trim() == '' || isNaN(parseInt($(this).parent().find('.baseEdit').val()))
                       || parseInt($(this).parent().find('.baseEdit').val()) <= 0) {
                        validTran = false;
                        return false;
                    }
                });
                if (validTran) {
                    validTran = readClassicViewRates();
                    //console.log('clasic view rate 1');
                    if (!$('[id=IsTetherUser]').prop('checked')) {
                        $('table.classictable .ui-selectee').removeClass('selected');
                    }
                }
                else {
                    ShowConfirmBox('Please enter valid rates for selected Dates', false);
                    return false;
                }
            }
            else {
                validTran = false;
                ShowConfirmBox('Please review the fields highlighted in Red.', false);
                return false;
            }
        }
        else {
            validTran = false;
            ShowConfirmBox('Please review the fields highlighted in Red.', false);
            return false;
        }
        if ($('[id=IsTetherUser]').prop('checked') && validTran) {
            TetherRateButton();
            //getLocationBrand();
            if (view == 'daily') {
                readTetheredRates(false);
            }
            else if (view == 'classic') {
                readClassicTetheredRates(false);
            }
        }
        else {
            setTimeout(function () {
                if (validTran && view == 'daily') {
                    nextDayClicked();
                    getSearchData(false);
                    assignCtrlValues('', '_m');
                    assignCtrlValues('', '_ml');
                }
                else if (validTran && view == 'classic') {
                    nextCarClassClicked();
                }
            }, 100);
        }
    }
}

function waitandbindSearchFilters(data) {
    if (typeof searchSummaryData !== "undefined") {
        //variable exists, do what you want
        //getSearchData(true);        
        $('.NoResultsFound').hide();
        $('#TetherRate, #update, #updaten, #IsTetherUser, .resetEdit').prop('disabled', false).removeClass("disable-button");
        DisableTSDUpdateAccess();
        SearchSummaryId = searchSummaryData.SearchSummaryID;

        if (searchSummaryData.IsQuickView) {
            showQuickViewControl(true);
            showQuickViewShopResult(searchSummaryData.SearchSummaryID);
            return false;
        }

        //bind filters
        bindSearchFilters('_m');
        bindSearchFilters('_ml');
        bindSearchFilters();
        bindDailySearchGrid(data);
        CheckQuickViewButtonEnableDisable();//For first time load grid.
        GetApplicableOpaqueRateCodes();
        //get current formula
        //setTimeout(function () { getCurrentFormula(); getLOR(); }, 550);
    }
    else {
        setTimeout(function () {
            waitandbindSearchFilters(data);
        }, 250);
    }
    //fetch dependent locationBrandid and extra day for tethered update.
    //  getLocationBrand();
}

//Region for Rubber band feature start here
//apply rubber band on car class column
$('.dailytable').selectable({
    filter: ".car-class-img",
    selected: function (event, ui) {
        var $selected = $(ui.selected);
        if ($selected.hasClass('selected')) {
            var $allCarClasses = $('#all-car-classes');
            $selected.removeClass('selected');
            if ($allCarClasses.hasClass('selected')) {
                $allCarClasses.removeClass('selected');
            }
        } else {
            $selected.addClass("selected");
        }
    },
    stop: function (event, ui) {
        if ($('td.car-class-img').not('.selected').length == 0) {
            $('#all-car-classes').addClass('selected');
        }
    },
});

//select all carClasses when header clicked
$('#all-car-classes').bind('mousedown', function () {
    var $this = $(this);
    if ($this.hasClass('selected')) {
        $this.removeClass('selected');
        $('#daily-rates-table td.car-class-img').removeClass("selected");
    } else {
        $('#daily-rates-table td.car-class-img').addClass("selected");
        $this.addClass('selected');
    }
});
$("#tsdSystemSelect_Weblink").selectable({
    filter: "li",
    selected: function (event, ui) {
        var $selected = $(ui.selected);
        if ($selected.hasClass('selected')) {
            $selected.removeClass('selected');
        } else {
            $selected.addClass("selected");
        }
        $(ui.selected).addClass('pSelected');
    },
    unselected: function (event, ui) {
        //console.log('unselected');
        if (event.ctrlKey) {
            $(ui.unselected).addClass('pSelected');

            var $selected = $(ui.unselected);
            if ($selected.hasClass('selected')) {
                $selected.removeClass('selected');
            } else {
                $selected.addClass("selected");
            }
        }
    },
    stop: function (event, ui) {

        //If user not clicked ctrl also if iPad then ignore un-selection when another LOR selected

        if (!event.ctrlKey && navigator.userAgent.match(/iPad|iPhone|Android|Windows Phone/i) == null) {
            $("#tsdSystemSelect_Weblink li").not('.pSelected').removeClass('selected');
            $("#tsdSystemSelect_Weblink li.pSelected").addClass('selected');
        }
        $("#tsdSystemSelect_Weblink li.pSelected").removeClass('pSelected');
    },
});

$("#updateAllLOR").selectable({
    filter: "li",
    selected: function (event, ui) {
        var $selected = $(ui.selected);
        if ($selected.hasClass('selected')) {
            $selected.removeClass('selected');
        } else {
            $selected.addClass("selected");
        }
        $(ui.selected).addClass('pSelected');
    },
    unselected: function (event, ui) {
        //console.log('unselected');
        if (event.ctrlKey) {
            $(ui.unselected).addClass('pSelected');

            var $selected = $(ui.unselected);
            if ($selected.hasClass('selected')) {
                $selected.removeClass('selected');
            } else {
                $selected.addClass("selected");
            }
        }
    },
    stop: function (event, ui) {

        //If user not clicked ctrl also if iPad then ignore un-selection when another LOR selected
        if (!event.ctrlKey && navigator.userAgent.match(/iPad|iPhone|Android|Windows Phone/i) == null) {
            $("#updateAllLOR li").not('.pSelected').removeClass('selected');
            $("#updateAllLOR li.pSelected").addClass('selected');
        }
        $("#updateAllLOR li.pSelected").removeClass('pSelected');
    },
});


//Region for Rubber band feature end here

//Region for bind filters for search grid start here
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
    $("#search2letter").val('');
    RemoveFlashableTag("#search2letter");

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

    //set displayDay
    $('#displayDay' + selector + ' ul').empty();
    var end = convertToServerTime(new Date(parseInt(searchSummaryData.EndDate.replace("/Date(", "").replace(")/", ""), 10)).setHours(23, 59, 59, 0)),
        currentDate = convertToServerTime(new Date(parseInt(searchSummaryData.StartDate.replace("/Date(", "").replace(")/", ""), 10)));
    //var between = [];
    while (currentDate <= end) {
        var d = new Date(currentDate);
        var date = days[d.getDay()].substring(0, 3) + ' - ' + monthNames[d.getMonth()].substring(0, 3) + ' ' + d.getDate();
        var value = d.getFullYear() + months[d.getMonth()] + d.getDate();

        //between.push(date);
        $("<li value='" + value + "'>" + date + "</li>").appendTo('#displayDay' + selector + ' ul');
        currentDate.setDate(currentDate.getDate() + 1);
    }
    $('#displayDay' + selector + ' li').eq(0).text($('#displayDay' + selector + ' ul li').eq(0).text()).attr('value', $('#displayDay' + selector + ' ul li').eq(0).addClass('selected').attr('value'));

    //set text between next and prev buttons
    $('.showSelectedDate').html($('#displayDay' + selector + ' li').eq(0).html());

    $('#carClass ul').empty();
    var CCvalue = searchSummaryData.CarClassName.split(',');
    var CCid = searchSummaryData.CarClassIDs.split(',');
    for (var i = 0; i < CCvalue.length; i++) {
        $("<li value='" + CCid[i] + "'>" + CCvalue[i] + "</li>").appendTo('#carClass ul');
    }
    $('#carClass li').eq(0).text($('#carClass ul li').eq(0).text()).attr('value', $('#carClass ul li').eq(0).addClass('selected').attr('value'));

    //bind lor get formula
    if ($.trim(selector) == "") {
        setTimeout(function () { getCurrentFormula(); getLOR(); }, 200);
    }

    //on change of filters, get new grid
    $(".dailyFilter ul ul li").not('.quickView').unbind('click').bind('click', function (event) {

        $('.glv').removeClass('glv');
        //remove flashable of extra daily rate factor
        if ($('.extraDayRateFactor').hasClass('temp')) {
            $('.extraDayRateFactor').removeClass('temp');
            RemoveFlashableTag($('.extraDayRateFactor'));
        }
        //get formula again only if location is changed
        self = $(this);
        setTimeout(function () {
            var elementId = self.parents('ul').parents('ul').attr('id');
            if (elementId.indexOf('viewSelect') >= 0) {
                currentView = $.trim($('#viewSelect .selected').attr('value'));
            }

            if (elementId.indexOf('location') >= 0 && $('#location ul li').length <= 1) {
                return false;
            }
            if (elementId.indexOf('dimension-source') >= 0 && $('#dimension-source ul li').length <= 1) {
                return false;
            }
            if (elementId.indexOf('location') >= 0 && $('#location ul li').length > 1) {
                getCurrentFormula();
                getLOR();
            }
            if ($('#viewSelect .selected').attr('value') != 'classic') {
                showView('daily');

                if (elementId.indexOf('length') >= 0) {
                    //if user change the length then set data to default value
                    $('#displayDay li').eq(0).text($('#displayDay ul li').removeClass('selected').eq(0).text()).attr('value', $('#displayDay ul li').eq(0).addClass('selected').attr('value'));
                    //set text between next and prev buttons
                    $('.showSelectedDate').html($('#displayDay li').eq(0).html());
                }
                getSearchData(false);
            }
                //and hide 

            else {
                showView('classic');
                return;
            }
        }, 250);
        assignCtrlValues('', '_m');
    });

    //for mobile portrait view on change of filters, get new grid
    $(".ul_m ul li").not('.quickView').unbind('click').bind('click', function () {
        self = $(this);
        setTimeout(function () {
            var elementId = self.parents('ul').parents('ul').attr('id');
            if (elementId.indexOf('location') >= 0 && $('#location_m ul li').length > 1) {
                getCurrentFormula();
                getLOR();
            }
            else if (elementId.indexOf('length') >= 0) {
                //if user change the length then set data to default value
                $('#displayDay_m li').eq(0).text($('#displayDay_m ul li').removeClass('selected').eq(0).text()).attr('value', $('#displayDay_m ul li').eq(0).addClass('selected').attr('value'));
                //set text between next and prev buttons
                $('.showSelectedDate').html($('#displayDay_m li').eq(0).html());
            }
            getSearchData(false, '_m');
        }, 250);

        assignCtrlValues('_m', '_ml');
        assignCtrlValues('_m', '');

    });

    //for mobile landscape view on change of filters, get new grid
    $(".ul_ml ul li").not('.quickView').unbind('click').bind('click', function () {
        self = $(this);
        setTimeout(function () {
            var elementId = self.parents('ul').parents('ul').attr('id');
            if (elementId.indexOf('location') >= 0 && $('#location_ml ul li').length > 1) {
                getCurrentFormula();
                getLOR();
            }
            else if (elementId.indexOf('length') >= 0) {
                //if user change the length then set data to default value
                $('#displayDay_ml li').eq(0).text($('#displayDay_ml ul li').removeClass('selected').eq(0).text()).attr('value', $('#displayDay_ml ul li').eq(0).addClass('selected').attr('value'));
                //set text between next and prev buttons
                $('.showSelectedDate').html($('#displayDay_ml li').eq(0).html());
            }
            getSearchData(false, '_ml');
        }, 250);

        assignCtrlValues('_ml', '_m');
        assignCtrlValues('_ml', '');

    });

    $('.dailyViewPrevDay, .dailyViewPrevDay_m, .dailyViewPrevDay_ml').hide();
    enableNextPrevButton('.dailyViewPrevDay', false);
    enableNextPrevButton('.dailyViewPrevDay_m', false);
    enableNextPrevButton('.dailyViewPrevDay_ml', false);
    if ($('#displayDay ul li').length <= 1 && $('#length ul li').length <= 1) {
        enableNextPrevButton('.dailyViewNextDay', false);
    }
    else {
        enableNextPrevButton('.dailyViewNextDay', true);
    }

    if ($('#displayDay_m ul li').length <= 1 && $('#length_m ul li').length <= 1) {
        enableNextPrevButton('.dailyViewNextDay_m', false);
    }
    else {
        enableNextPrevButton('.dailyViewNextDay_m', true);
    }

    if ($('#displayDay_ml ul li').length <= 1 && $('#length_ml ul li').length <= 1) {
        enableNextPrevButton('.dailyViewNextDay_ml', false);
    }
    else {
        enableNextPrevButton('.dailyViewNextDay_ml', true);
    }

    //Prev day clicked    
    $('.dailyViewPrevDay').unbind('click').bind('click', function () {
        //remove flashable of extra daily rate factor
        if ($('.extraDayRateFactor').hasClass('temp')) {
            $('.extraDayRateFactor').removeClass('temp');
            RemoveFlashableTag($('.extraDayRateFactor'));
        }
        prevDayClicked();
        setTimeout(function () { getSearchData(false); }, 250);
        assignCtrlValues('', '_m');
        assignCtrlValues('', '_ml');
    });

    //Next day Clicked
    $('.dailyViewNextDay').unbind('click').bind('click', function () {
        //remove flashable of extra daily rate factor
        if ($('.extraDayRateFactor').hasClass('temp')) {
            $('.extraDayRateFactor').removeClass('temp');
            RemoveFlashableTag($('.extraDayRateFactor'));
        }
        nextDayClicked();
        getSearchData(false);
        assignCtrlValues('', '_m');
        assignCtrlValues('', '_ml');
    });

    //Filter: Mobile portrait view; next date button; on last date go to next length; set value for both mobile landscape and portrait
    //controls to make consistancy
    $('.dailyViewPrevDay_m').unbind('click').bind('click', function () {
        prevDayClicked('_m');
        setTimeout(function () { getSearchData(false, '_m'); }, 250);
        assignCtrlValues('_m', '_ml');
        assignCtrlValues('_m', '');
    });

    //Filter: Mobile landscape view; next date button; on last date go to next length; set value for both mobile landscape and portrait
    //controls to make consistancy
    $('.dailyViewPrevDay_ml').unbind('click').bind('click', function () {
        prevDayClicked('_ml');
        setTimeout(function () { getSearchData(false, '_ml'); }, 250);
        assignCtrlValues('_ml', '_m');
        assignCtrlValues('_ml', '');
    });


    //Filter: Mobile portrait view; prev date button; on first selection, go to prev length selection; set value for both landscape and portrait
    //controls to make consistancy
    $('.dailyViewNextDay_m').unbind('click').bind('click', function () {
        nextDayClicked('_m');
        setTimeout(function () { getSearchData(false, '_m'); }, 250);
        assignCtrlValues('_m', '_ml');
        assignCtrlValues('_m', '');
    });

    //Filter: Mobile landscape view; prev date button; on first selection, go to prev length selection; set value for both landscape and portrait
    //controls to make consistancy
    $('.dailyViewNextDay_ml').unbind('click').bind('click', function () {
        nextDayClicked('_ml');
        setTimeout(function () { getSearchData(false, '_ml'); }, 250);
        assignCtrlValues('_ml', '_m');
        assignCtrlValues('_ml', '');
    });

    $('#LOR_m').bind('change', function () {
        $('#LOR_m option').each(function () {
            if ($(this).prop('selected')) {
                $('#LOR_ml option[value=' + $(this).attr('value') + ']').prop('selected', true);
                $('#tsdSystemSelect_Weblink li[value=' + $(this).attr('value') + ']').addClass('selected');

                //update all popup 
                $('#updateAllLOR li[value=' + $(this).attr('value') + ']').addClass('selected');
                $("#UpdateAllLOR_ml option[value=" + $(this).attr('value') + "]").prop("selected", true);
                $("#UpdateAllLOR_m option[value=" + $(this).attr('value') + "]").prop("selected", true);
            } else {
                $('#LOR_ml option[value=' + $(this).attr('value') + ']').prop('selected', false);
                $('#tsdSystemSelect_Weblink li[value=' + $(this).attr('value') + ']').removeClass('selected');

                //update all popup 
                $('#updateAllLOR li[value=' + $(this).attr('value') + ']').removeClass('selected');
                $("#UpdateAllLOR_ml option[value=" + $(this).attr('value') + "]").prop("selected", false);
                $("#UpdateAllLOR_m option[value=" + $(this).attr('value') + "]").prop("selected", false);
            }

        });
    })

    $('#LOR_ml').bind('change', function () {
        $('#LOR_ml option').each(function () {
            if ($(this).prop('selected')) {
                $('#LOR_m option[value=' + $(this).attr('value') + ']').prop('selected', true);
                $('#tsdSystemSelect_Weblink li[value=' + $(this).attr('value') + ']').addClass('selected');

                //update all popup 
                $('#updateAllLOR li[value=' + $(this).attr('value') + ']').addClass('selected');
                $("#UpdateAllLOR_ml option[value=" + $(this).attr('value') + "]").prop("selected", true);
                $("#UpdateAllLOR_m option[value=" + $(this).attr('value') + "]").prop("selected", true);
            } else {
                $('#LOR_m option[value=' + $(this).attr('value') + ']').prop('selected', false);
                $('#tsdSystemSelect_Weblink li[value=' + $(this).attr('value') + ']').removeClass('selected');

                //update all popup 
                $('#updateAllLOR li[value=' + $(this).attr('value') + ']').removeClass('selected');
                $("#UpdateAllLOR_ml option[value=" + $(this).attr('value') + "]").prop("selected", false);
                $("#UpdateAllLOR_m option[value=" + $(this).attr('value') + "]").prop("selected", false);
            }

        });
    })
}

function showView(viewName) {
    HideLastDaySpan();
    if (viewName == 'classic') {
        $('.quickView').hide();
        $('[id^="length"]').show();
        $('.classic_view').show();

        //add css force-fully as this element was not rendering properly
        $('.carClassItem').css('display', 'inline');
        $('.viewTwoLetterSearch').css('display', 'inline-block');

        $('.daily_view, .classic_hide').hide();
        bindClassicData();

        //For call auto refresh is break.
        SearchSummaryRecursiveCall = true;//For Settimeout recursive call not create
        recentSearchAjaxCall();
        if (isQuickViewShop) {
            $("[id=IgnoreAndNext]").hide();
        }
    }
    else {
        $('.classic_view').hide();
        $('.daily_view, .classic_hide').show();
        if (isQuickViewShop) {
            showQuickViewControl(true);
            if ($("#rbChanged").prop("checked")) {
                showLengthDateCombinationChanged(true);
            }
            else {
                showLengthDateCombinationChanged(false);
            }
            $("[id=IgnoreAndNext]").show();
        }
        else {
            showQuickViewControl(false);
        }

    }
    ShowClassicQuickViewRentalLength();
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

    if (isQuickViewShop) {
        $('.dailyViewNextDayDisabled, .dailyViewNextDay_mlDisabled, .dailyViewNextDay_mDisabled').show();
        $('.dailyViewNextDay, .dailyViewNextDay_m, .dailyViewNextDay_ml').hide();
    }
}

function assignCtrlValues(sourceCtrl, destCtrl) {
    setTimeout(function () {
        //here you need to make all control values for desktop view same as mobile view

        //copy value and text for outer li
        $('#dimension-source' + destCtrl + ' li').eq(0).text($('#dimension-source' + sourceCtrl + ' li').eq(0).text()).attr('value', $('#dimension-source' + sourceCtrl + ' li').eq(0).attr('value'));
        //add selected class to inner li
        $('#dimension-source' + destCtrl + ' ul li').removeClass('selected').eq($('#dimension-source' + sourceCtrl + ' ul li.selected').index()).addClass('selected');

        $('#location' + destCtrl + ' li').eq(0).text($('#location' + sourceCtrl + ' li').eq(0).text()).attr('value', $('#location' + sourceCtrl + ' li').eq(0).attr('value'));
        $('#location' + destCtrl + ' ul li').removeClass('selected').eq($('#location' + sourceCtrl + ' ul li.selected').index()).addClass('selected');

        $('#length' + destCtrl + ' li').eq(0).text($('#length' + sourceCtrl + ' li').eq(0).text()).attr('value', $('#length' + sourceCtrl + ' li').eq(0).attr('value'));
        $('#length' + destCtrl + ' ul li').removeClass('selected').eq($('#length' + sourceCtrl + ' ul li.selected').index()).addClass('selected');

        $('#displayDay' + destCtrl + ' li').eq(0).text($('#displayDay' + sourceCtrl + ' li').eq(0).text()).attr('value', $('#displayDay' + sourceCtrl + ' li').eq(0).attr('value'));
        $('#displayDay' + destCtrl + ' ul li').removeClass('selected').eq($('#displayDay' + sourceCtrl + ' ul li.selected').index()).addClass('selected');

        $('.showSelectedDate').html($('#displayDay' + sourceCtrl + ' li').eq(0).html());

        if (!isQuickViewShop) {
            if ($('#displayDay' + sourceCtrl + ' ul li').length <= 1 && $('#length' + sourceCtrl + ' ul li').length <= 1) {
                enableNextPrevButton('.dailyViewPrevDay' + sourceCtrl, false);
                enableNextPrevButton('.dailyViewPrevDay' + destCtrl, false);
                enableNextPrevButton('.dailyViewNextDay' + sourceCtrl, false);
                enableNextPrevButton('.dailyViewNextDay' + destCtrl, false);
            }
            else {
                if ($('#displayDay' + sourceCtrl + ' ul li').first().hasClass('selected') && $('#length' + sourceCtrl + ' ul li').first().hasClass('selected')) {
                    enableNextPrevButton('.dailyViewPrevDay' + sourceCtrl, false);
                    enableNextPrevButton('.dailyViewPrevDay' + destCtrl, false);
                    if ($('#displayDay' + sourceCtrl + ' ul li').length > 1 || $('#length' + sourceCtrl + ' ul li').length > 1) {
                        enableNextPrevButton('.dailyViewNextDay' + sourceCtrl, true);
                        enableNextPrevButton('.dailyViewNextDay' + destCtrl, true);
                    }
                }
                else if ($('#displayDay' + sourceCtrl + ' ul li').last().hasClass('selected') && $('#length' + sourceCtrl + ' ul li').last().hasClass('selected')) {
                    enableNextPrevButton('.dailyViewNextDay' + sourceCtrl, false);
                    enableNextPrevButton('.dailyViewNextDay' + destCtrl, false);
                    if ($('#displayDay' + sourceCtrl + ' ul li').length > 1 || $('#length' + sourceCtrl + ' ul li').length > 1) {
                        enableNextPrevButton('.dailyViewPrevDay' + sourceCtrl, true);
                        enableNextPrevButton('.dailyViewPrevDay' + destCtrl, true);
                    }
                }
                else {
                    enableNextPrevButton('.dailyViewPrevDay' + sourceCtrl, true);
                    enableNextPrevButton('.dailyViewPrevDay' + destCtrl, true);
                    enableNextPrevButton('.dailyViewNextDay' + sourceCtrl, true);
                    enableNextPrevButton('.dailyViewNextDay' + destCtrl, true);
                }
            }
            //if not quickview shop and last day of shop is reached then show message Last Shop Day
            if ($.trim(currentView) == '') {
                currentView = $.trim($('#viewSelect .selected').attr('value'));
            }
            if (currentView.toLowerCase() == 'daily') {
                if ($('.dailyViewNextDayDisabled').is(':visible') || $('.dailyViewNextDay_mlDisabled').is(':visible') || $('.dailyViewNextDay_mDisabled').is(':visible')) {
                    $(".spanlastdayshop").show();
                }
                else {
                    $(".spanlastdayshop").hide();
                }
            }
        }
        else {

            if ($('[Id$="rbChanged"]').prop('checked')) {
                if ($('#lengthDateCombinationChanged ul li').length <= 1) {
                    enableNextPrevButton('.dailyViewPrevDay' + sourceCtrl, false);
                    enableNextPrevButton('.dailyViewPrevDay' + destCtrl, false);
                    //enableNextPrevButton('.dailyViewNextDay' + sourceCtrl, false);
                    //enableNextPrevButton('.dailyViewNextDay' + destCtrl, false);
                }
                else {
                    if ($('#lengthDateCombinationChanged ul li').first().hasClass('selected')) {
                        enableNextPrevButton('.dailyViewPrevDay' + sourceCtrl, false);
                        enableNextPrevButton('.dailyViewPrevDay' + destCtrl, false);
                    }
                    else {
                        enableNextPrevButton('.dailyViewPrevDay' + sourceCtrl, true);
                        enableNextPrevButton('.dailyViewPrevDay' + destCtrl, true);
                    }
                }
            }
            else if ($('[Id$="rbUnChanged"]').prop('checked')) {
                if ($('#lengthDateCombinationUnChanged ul li').length <= 1) {
                    enableNextPrevButton('.dailyViewPrevDay' + sourceCtrl, false);
                    enableNextPrevButton('.dailyViewPrevDay' + destCtrl, false);
                }
                else {
                    if ($('#lengthDateCombinationUnChanged ul li').first().hasClass('selected')) {
                        enableNextPrevButton('.dailyViewPrevDay' + sourceCtrl, false);
                        enableNextPrevButton('.dailyViewPrevDay' + destCtrl, false);
                    }
                    else {
                        enableNextPrevButton('.dailyViewPrevDay' + sourceCtrl, true);
                        enableNextPrevButton('.dailyViewPrevDay' + destCtrl, true);
                    }
                }

            }
        }
        var $lengthValue = $('#length li.selected').text().trim();
        enableRentalLengths($lengthValue.charAt(0));
        highLightLOR($lengthValue);
        highLight($lengthValue);
        getExtraDayRateValue($('#location ul li.selected').attr('lbid'), $lengthValue.charAt(0));


        setTimeout(function () {
            if ($('.dailytable').is(':visible')) {
                //disable 'update and next' button    
                //if ($('.dailyViewNextDay').eq(0).is(':visible')) {
                //    $('[id^="updaten"]').prop('disabled', false).removeClass("disable-button");
                //}
                //else {
                //    $('[id^="updaten"]').prop('disabled', true).addClass("disable-button");
                //}
                if ($('.dailytable .NoResultsFound').is(':visible')) {
                    $('[id^="updaten"]').prop('disabled', true).addClass("disable-button");
                }
            }
        }, 700);


    }, 250);

    $("#search2letter").val('');
    RemoveFlashableTag("#search2letter");
}

function nextDayClicked(selector) {
    console.log("*********************************");
    var t = new Date(); console.log("call to nextDayClicked at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
    if (isQuickViewShop) {
        $('#rbChanged:checked').length > 0 ? ShowNextQuickView('#lengthDateCombinationChanged') : ShowNextQuickView('#lengthDateCombinationUnChanged');
        return false;
    }
    if (selector == null) {
        selector = '';
    }
    $('.LOR').hide();
    //enableNextPrevButton('.dailyViewPrevDay' + selector + '', true);
    if ($('#displayDay' + selector + ' ul li').length != $('#displayDay' + selector + ' ul li.selected').index() + 1) {
        $('#displayDay' + selector + ' ul li.selected').removeClass('selected').next().addClass('selected');
    }
    else {
        if ($('#length' + selector + ' ul li').length != $('#length' + selector + ' ul li.selected').index() + 1) {
            $('#displayDay' + selector + ' ul li.selected').removeClass('selected');
            $('#displayDay' + selector + ' ul li').first().addClass('selected');
            $('#length' + selector + ' ul li.selected').removeClass('selected').next().addClass('selected');
            $('.LOR').show();


        }
    }
    //if ($('#length' + selector + ' ul li').last().hasClass('selected') && $('#displayDay' + selector + ' ul li').last().hasClass('selected')) {
    //    enableNextPrevButton('.dailyViewNextDay' + selector + '', false);
    //}
    //else {
    //    enableNextPrevButton('.dailyViewNextDay' + selector + '', true);
    //}

    $('#displayDay' + selector + ' li').eq(0).text($('#displayDay' + selector + ' ul li.selected').text()).attr('value', $('#displayDay' + selector + ' ul li.selected').attr('value'));
    $('#length' + selector + ' li').eq(0).text($('#length' + selector + ' ul li.selected').text()).attr('value', $('#length' + selector + ' ul li.selected').attr('value'));

    $('.showSelectedDate').html($('#displayDay' + selector + ' li').eq(0).html());
    //getSearchData(false);
    return false;
}

function prevDayClicked(selector) {
    if (isQuickViewShop) {
        $('#rbChanged:checked').length > 0 ? ShowPrevQuickView('#lengthDateCombinationChanged') : ShowPrevQuickView('#lengthDateCombinationUnChanged');
        return false;
    }
    if (selector == null) {
        selector = '';
    }
    //event.preventDefault();
    $('.LOR').hide();
    //enableNextPrevButton('.dailyViewNextDay' + selector + '', true);
    if ($('#displayDay' + selector + ' ul li.selected').index() != 0) {
        $('#displayDay' + selector + ' ul li.selected').removeClass('selected').prev().addClass('selected');

    }
    else {
        if ($('#length' + selector + ' ul li.selected').index() != 0) {
            $('#length' + selector + ' ul li.selected').removeClass('selected').prev().addClass('selected');
            $('#displayDay' + selector + ' ul li.selected').removeClass('selected');
            $('#displayDay' + selector + ' ul li').last().addClass('selected');
            $('.LOR').show();
        }
    }
    //if ($('#length' + selector + ' ul li').first().hasClass('selected') && $('#displayDay' + selector + ' ul li').first().hasClass('selected')) {
    //    enableNextPrevButton('.dailyViewPrevDay' + selector + '', false);
    //}
    $('#displayDay' + selector + ' li').eq(0).text($('#displayDay' + selector + ' ul li.selected').text()).attr('value', $('#displayDay' + selector + ' ul li.selected').attr('value'));
    $('#length' + selector + ' li').eq(0).text($('#length' + selector + ' ul li.selected').text()).attr('value', $('#length' + selector + ' ul li.selected').attr('value'));

    $('.showSelectedDate').html($('#displayDay' + selector + ' li').eq(0).html());
    //getSearchData(false);
    return false;
}

function getSearchData(getDefaultData, selector) {
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

    $('#all-car-classes').removeClass('selected');
    $('.additionalBaseCol').hide();
    showAdditionalBase = false;

    if (selector == null) {
        selector = '';
    }
    $('.NoResultsFound').remove();
    //set Length and ND(no of days while user can on change length for used in formula
    getNDandLength($("#length" + selector + " li").html());
    $('.loader_container_main').show();


    //click on appropriate search from Recent Searches 
    //so summary for this search will be visible
    var $pastSearchLi = $('.pastSearchul li[value="' + SearchSummaryId + '"]');
    $pastSearchLi.click();

    //showQuickViewControl($pastSearchLi.attr('isQuickView'));
    //bindlengthDateCombination(SearchSummaryId);

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
    if (data.searchSummaryID > 0 && typeof (data.scrapperSourceID) != 'undefined' && typeof (data.locationBrandID) != 'undefined') {

        var t = new Date(); console.log("start ajax call to get daily view data at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
        $.ajax({
            url: 'Search/GetSearchGridDailyViewData',
            type: 'GET',
            data: data,
            dataType: 'json',
            success: function (data) {
                var t = new Date(); console.log("Got daily view response and bind it to grid at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
                bindDailySearchGrid(data);
                $('.NoResultsFound').hide();
                $('#TetherRate, #update, #updaten, #IsTetherUser, .resetEdit').prop('disabled', false).removeClass("disable-button");
                DisableTSDUpdateAccess();
                var t = new Date(); console.log("Binding completes at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $('.loader_container_main').hide();
                searchViewModel.rates(null);
                searchViewModel.headers(null);
                $('<tr class="NoResultsFound"><td colspan="4" class="red bold" style="align:left">No Rates Found</td></tr>').appendTo('#daily-rates-table tbody');
                $('#TetherRate, #update, #updaten, #IsTetherUser, .resetEdit').prop('disabled', true).addClass("disable-button");
                SetSessionStorage();
                setLastUpdateTSD('');
            }
        });
    }
    else {
        $('.loader_container_main').hide();
        searchViewModel.rates(null);
        searchViewModel.headers(null);
        $('<tr class="NoResultsFound"><td colspan="4" class="red bold" style="align:left">No Rates Found</td></tr>').appendTo('#daily-rates-table tbody');
        $('#TetherRate, #update, #updaten, #IsTetherUser, .resetEdit').prop('disabled', true).addClass("disable-button");
        SetSessionStorage();
        setLastUpdateTSD('');
    }
}


function bindDailySearchGrid(d) {
    var t = new Date(); console.log("start binding daily view--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
    setTimeout(function () {
        $('.loader_container_main').hide();
    }, 300);

    brandID = d.brandID;
    var finalData = JSON.parse(d.finalData);
    var headerData = $.map(finalData.HeaderInfo, function (item) { return new HeaderInfo(item) });
    searchViewModel.headers(headerData);


    var rateData = $.map(finalData.RatesInfo, function (item) { return new RatesInfo(item) })
    searchViewModel.rates(rateData);

    setLastUpdateTSD(d.lastTSDUpdated);

    if (GlobalLimitSearchSummaryData != null && GlobalLimitSearchSummaryData.IsQuickView) {
        $('#daily-rates-table .car-class-img').each(function () {
            if ($(this).siblings(".companyRates").length > 0) {
                if ($(this).siblings(".companyRates").find('span').hasClass('rateMovedUp') || $(this).siblings(".companyRates").find('span').hasClass('rateMovedDown')) {
                    $(this).addClass('selected');
                    //console.log($(this));
                    //console.log('found')
                }
            }
        });
        if ($('#daily-rates-table .car-class-img.selected').length > 0) {
            if ($('#daily-rates-table .car-class-img.selected').length == $('#daily-rates-table .car-class-img').length) {
                $('#all-car-classes').addClass('selected');
            }
        }
    }
    var t = new Date(); console.log("binding complete and set timeout to bind numbers--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
    setTimeout(function () {
        var t = new Date(); console.log("start binding numbers--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
        bindNumberOnly();
        bindBaseTotalTextbox();
        var t = new Date(); console.log("complete binding numbers--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
        /*//cursor hinting removed for performance issue
        //Highlight column header and carClass on mouse over for search grid
        var rows = $('.dailytable tr');
        rows.children().bind('mouseover', function () {
            rows.children().removeClass('highlightCursorHitting');
            var index = $(this).prevAll().length;
            if (index > 1 && $(this).closest("tr")[0].rowIndex > 0) {
                //console.log(rows.find(':nth-child(' + (index + 1) + ')').eq(0));
                rows.find(':nth-child(' + (index + 1) + ')').eq(0).addClass('highlightCursorHitting');
                $(this).addClass('highlightCursorHitting').siblings().first().addClass('highlightCursorHitting');
            }
        });*/
    }, 300);

    //$('.dailytable').bind('mouseleave', function () {
    //    $('.dailytable td').removeClass('highlightCursorHitting');
    //});
    //console.log(d.suggestedRate);
    var t = new Date(); console.log("start binding suggested rates--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
    BindSuggestedRates(d.suggestedRate);
    var t = new Date(); console.log("complete binding suggested rates--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
    //Call for check Tethervalue is empty or not
    setTimeout(function () {
        CheckTetherValueDiableButton();
        commonOpaqueRateBinding();
    }, 250);
    //For tethershop button first click and get the data.

    DisableTSDUpdateAccess();
}

function bindNumberOnly() {
    //Add class 'numbersOnly' for numeric/ decimal fields    
    $('.numbersOnly').not('.extraDayRateFactor').bind('keyup', function () {
        var val = commaRemovedNumber($(this).val());
        //$(this).val(commaSeparateNumber(val));
        if (val.trim() != '') {
            if ($.isNumeric(val)) {
                if (val.indexOf('.') != -1) {
                    if (val.split(".")[1].length > 2) {
                        if (isNaN(parseFloat(val))) return;
                        this.value = parseFloat(val).toFixed(2);
                    }
                }
                //numeric
                RemoveFlashableTag($(this));
            }
            else {
                // not numeric
                MakeTagFlashable($(this));
            }

        }
        else if ($(this).hasClass('required')) {
            //this is required field;
            MakeTagFlashable($(this));
        }
        else {
            //this is not required field
            RemoveFlashableTag($(this));
        }
        AddFlashingEffect();
    });
}

var GetGovernmentRate = function (totalRate, lor, isTotaltoBase) {
    lor = parseInt(lor);
    totalRate = parseFloat(totalRate);
    if (lor > 0 && totalRate > 0) {
        if (isTotaltoBase) {
            return totalRate - (lor * governmentFluctuationRate);
        }
        else {
            return totalRate + (lor * governmentFluctuationRate);
        }
    }
    return 0;
}


function bindBaseTotalTextbox() {

    $('.totalEdit').bind('input', function (e) {
        var val = commaRemovedNumber($(this).val());
        if ($.isNumeric(val)) {
            var parentTr = $(this).parents('tr');
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
                parentTr.find(".additionalBase").val(commaSeparateNumber(additionalBase.toFixed(2)));
            }
            if (GlobalLimitSearchSummaryData.IsGOV) {
                baseValue = parseInt(baseValue);
            }

            parentTr.find(".baseEdit").val(commaSeparateNumber(baseValue.toFixed(2)));
            var keyCode = e.keyCode || e.which;
            if (keyCode != 9 || !parentTr.find(".baseEdit").hasClass('glv')) {
                RemoveFlashableTag(parentTr.find(".baseEdit"));
            }
            AddFlashingEffect();
        }
    });


    $('.totalEdit').bind('focusout', function () {
        var self = $(this);
        self.removeClass('glv');
        var newVal = parseFloat(commaRemovedNumber(self.val()));
        if ($.isNumeric(newVal) && formulaTtoB != null && formulaBtoT != null) {
            self.val(commaSeparateNumber(newVal.toFixed(2)));
            var parentTr = self.parents('tr');
            total = newVal;
            if (GlobalLimitSearchSummaryData.IsGOV) {
                var selectedLor = $("#result-section #length ul li.selected").eq(0).val();
                if (typeof (selectedLor) != 'undefined' && selectedLor > 0) {
                    total = GetGovernmentRate(total, selectedLor, true);
                }
            }
            var newbase = parseFloat(evaluateFormula(formulaTtoB, true));
            if (showAdditionalBase) {
                newbase = calculateNewBase(false, newbase);
            }
            var $basectrl = parentTr.find(".baseEdit");
            RemoveFlashableTag($basectrl);
            $basectrl.removeClass('glv');
            //check min max base rate exist? if yes then apply            
            var attr = $basectrl.attr('minbaserate');
            var attr1 = $basectrl.attr('maxbaserate');
            if (typeof attr !== typeof undefined && attr !== false && typeof attr1 !== typeof undefined && attr1 !== false) {
                //var modifiedbase = getminMaxBaseRate($basectrl, newbase);
                var nb = parseFloat(newbase).toFixed(2);
                //$basectrl.val(commaSeparateNumber(nb));

                if (parseFloat(attr) > nb) {
                    $basectrl.addClass('glv');
                    MakeTagFlashable($basectrl);
                }
                else if (parseFloat(attr1) > 0 && parseFloat(attr1) < nb) {
                    $basectrl.addClass('glv');
                    MakeTagFlashable($basectrl);
                }
                /*
                if (showAdditionalBase) {
                    parentTr.find(".additionalBase").val(commaSeparateNumber(calculateAdditionalBase(modifiedbase).toFixed(2)));
                    modifiedbase = calculateNewBase(true, modifiedbase);
                }
                if (modifiedbase.toFixed(2) != newbase.toFixed(2)) {
                    rt = modifiedbase;
                    self.val(evaluateFormula(formulaBtoT, true).toFixed(2));
                }
                */
            }
            //remove flashing effect of base ctrl if flasing
            //RemoveFlashableTag($basectrl);
            AddFlashingEffect();
        }
    });

    $('.baseEdit').bind('input', function () {
        var val = commaRemovedNumber($(this).val());
        if (typeof (GlobalLimitSearchSummaryData) != 'undefined' && GlobalLimitSearchSummaryData.IsGOV && val.indexOf(".") > -1) {
            var valArray = val.split(".");
            if (valArray.length > 1 && valArray[1] > 0) {
                val = parseInt(val);
                $(this).val(val.toFixed(2));
            }
        }
        if ($.isNumeric(val) && formulaBtoT != null) {
            var parentTr = $(this).parents('tr');
            if (showAdditionalBase) {
                var additionalBase = calculateAdditionalBase(val);
                if (GlobalLimitSearchSummaryData.IsGOV) {
                    additionalBase = parseInt(additionalBase);
                }
                parentTr.find(".additionalBase").val(commaSeparateNumber(additionalBase.toFixed(2)));
                val = calculateNewBase(true, val);
            }
            rt = val;
            var totalValue = evaluateFormula(formulaBtoT, true);
            if (GlobalLimitSearchSummaryData.IsGOV) {
                var selectedLor = $("#result-section #length ul li.selected").eq(0).val();
                if (typeof (selectedLor) != 'undefined' && selectedLor > 0) {
                    totalValue = GetGovernmentRate(totalValue, selectedLor, false);
                }
            }
            parentTr.find(".totalEdit").val(commaSeparateNumber(totalValue.toFixed(2)));
            RemoveFlashableTag(parentTr.find(".totalEdit"));
            AddFlashingEffect();
        }
    });

    $('.baseEdit').bind('focusout', function () {
        var val = commaRemovedNumber($(this).val());
        if ($.isNumeric(val) && formulaBtoT != null) {
            var self = $(this);
            self.removeClass('glv');
            RemoveFlashableTag(self);
            if (typeof (GlobalLimitSearchSummaryData) != 'undefined' && GlobalLimitSearchSummaryData.IsGOV && val.indexOf(".") > -1) {
                var valArray = val.split(".");
                if (valArray.length > 1 && valArray[1] > 0) {
                    val = parseInt(val);
                }
            }
            self.val(commaSeparateNumber(parseFloat(val).toFixed(2)));
            var parentTr = $(this).parents('tr');
            RemoveFlashableTag(parentTr.find(".totalEdit"));
            parentTr.find(".totalEdit").removeClass('glv');
            //check min max base rate exist? if yes then apply
            var attr = self.attr('minbaserate');
            var attr1 = self.attr('maxbaserate');
            if (typeof attr !== typeof undefined && attr !== false && typeof attr1 !== typeof undefined && attr1 !== false) {
                //val = getminMaxBaseRate(self, parseFloat(val));


                if (parseFloat(attr) > parseFloat(val)) {
                    self.addClass('glv');
                    MakeTagFlashable(self);
                }
                else if (parseFloat(attr1) > 0 && parseFloat(attr1) < parseFloat(val)) {
                    self.addClass('glv');
                    MakeTagFlashable(self);
                }
                /*
                self.val(val.toFixed(2));
                if (showAdditionalBase) {
                    parentTr.find(".additionalBase").val(commaSeparateNumber(calculateAdditionalBase(val).toFixed(2)));
                    val = calculateNewBase(true, val);
                }
                rt = val;
                parentTr.find(".totalEdit").val(commaSeparateNumber(evaluateFormula(formulaBtoT, true).toFixed(2)));
                */
            }
            //remove flashing effect of base ctrl if flasing
            //RemoveFlashableTag(parentTr.find(".totalEdit"));
            AddFlashingEffect();
        }
    });

    $(".drag_bar_disable").on("click", function () { DragMouseUpEvent(this); });
    $(".dropped_bar").on("click", function () { return false; });
}

var DragMouseUpEvent = function (ele) {
    if ($(".drag_bar").length > 0) {
        var anyCarSelected = $('#daily-rates-table .car-class-img.selected').length > 0;
        if (!anyCarSelected) {
            ShowConfirmBox('At least one car class must be selected to suggest rates', false);
            return false;
        }
        var gapValue = 0.0;
        gapValue = $.trim($("#txtDragGapValue").val()).length > 0 ? $.trim($("#txtDragGapValue").val()) : $("input[name='gapvalue']:checked").val();
        if (!gapValue) {
            ShowConfirmBox('At least one gap value must be selected to suggest rates', false);
            return false;
        }
        var droppedOnCompayId = $(ele).attr('cid');
        if (parseInt(droppedOnCompayId) > 0 && parseFloat(gapValue) > 0) {
            $('#daily-rates-table .car-class-img.selected img').each(function () {
                var $carClassImg = $(this);
                var carClassId = $carClassImg.attr("classid");

                if ((GlobalLimitSearchSummaryData.IsGOV == null) || !GlobalLimitSearchSummaryData.IsGOV) {
                    //Not GOV shops
                    var competitorTV = commaRemovedNumber($("td[companyandcarid='" + droppedOnCompayId + "_" + carClassId + "']").attr("tv"));
                    if (typeof (competitorTV) != 'undefined' && parseFloat(competitorTV) > 0) {
                        //var newTotalValue = $("input[type='radio'][name='position']:checked").val() == "below" ? (parseFloat(competitorTV) - parseFloat(gapValue)) :( parseFloat(competitorTV) + parseFloat(gapValue));
                        var newTotalValue = $("#chkAbovePos").prop('checked') ? (parseFloat(competitorTV) + parseFloat(gapValue)) : (parseFloat(competitorTV) - parseFloat(gapValue));
                        $carClassImg.closest("tr").find("input.totalEdit").eq(0).val(newTotalValue.toFixed(2));
                        $carClassImg.closest("tr").find("input.totalEdit").trigger("input").trigger("focusout");
                    }
                }
                else {
                    //For GOV shops
                    var competitorBV = commaRemovedNumber($("td[companyandcarid='" + droppedOnCompayId + "_" + carClassId + "']").attr("bv"));
                    if (typeof (competitorBV) != 'undefined' && parseFloat(competitorBV) > 0) {
                        //var newBaseValue = $("input[type='radio'][name='position']:checked").val() == "below" ? (parseInt(parseFloat(competitorBV) - parseFloat(gapValue))) : (parseInt(parseFloat(competitorBV) + parseFloat(gapValue)));
                        var newBaseValue = $("#chkAbovePos").prop('checked') ? (parseInt(parseFloat(competitorBV) + parseFloat(gapValue))) : (parseInt(parseFloat(competitorBV) - parseFloat(gapValue)));
                        $carClassImg.closest("tr").find("input.baseEdit").eq(0).val(newBaseValue.toFixed(2));
                        $carClassImg.closest("tr").find("input.baseEdit").trigger("input").trigger("focusout");
                    }
                }
            });
            $(".dropped_bar").removeClass("dropped_bar").addClass("drag_bar_disable");
            $(ele).removeClass("drag_bar_disable").addClass("dropped_bar");
            $("#lblDragMessage").show().html("New rates suggested as per " + $(ele).attr('cname'));
            setTimeout(function () { $("#lblDragMessage").hide(); }, 4000);
        }

    }
    //isDominantBrandSelectedForDrag = false;
}

var DragMouseDownEvent = function () {
    var anyCarSelected = $('#daily-rates-table .car-class-img.selected').length > 0;
    if (!anyCarSelected) {
        ShowConfirmBox('At least one car class must be selected to suggest rates', false);
        return false;
    }
    var isGapValueSpecified = $.trim($("input[name='gapvalue']:checked").val()).length > 0 || $.trim($("#txtDragGapValue").val()).length > 0;
    if (!isGapValueSpecified) {
        ShowConfirmBox('At least one gap value must be selected to suggest rates', false);
        return false;
    }
    isDominantBrandSelectedForDrag = true;
}

function getminMaxBaseRate(basectrl, value) {
    if ($.isNumeric(value)) {
        //get min max rate
        var minbaserate = parseFloat(basectrl.attr('minbaserate'));
        var maxbaserate = parseFloat(basectrl.attr('maxbaserate'));
        if ($.isNumeric(minbaserate) && value < minbaserate) {
            return minbaserate;
        }
        else if (maxbaserate > 0 && $.isNumeric(maxbaserate) && value > maxbaserate) {
            return maxbaserate;
        }
    }
    return value;
}

var isBindSuggestedRatesReady;
function BindSuggestedRates(suggestedRate) {

    if (typeof formulaTtoB == "undefined") {
        isBindSuggestedRatesReady = false;

        //variable not exists, wait for some time 
        setTimeout(function () { BindSuggestedRates(suggestedRate); return false; }, 200);
    }
    else {
        isBindSuggestedRatesReady = true;
    }
    if (!isBindSuggestedRatesReady) {
        return false;
    }

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

    //$('.perDay').html($('#length li').eq(0).html().trim().charAt(0));
    $('.arrow').removeAttr('class').addClass('arrow');
    $('.arrowb').removeAttr('class').addClass('arrowb');
    $('.ruleSetLink').val('').hide();
    $('#daily-rates-table tr [classid]').each(function () {
        var self = $(this);
        var parentTr = self.parents('tr');
        suggestedRate.forEach(function (element, i) {

            if (element.CarClassID == self.attr("classid")) {

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
                    parentTr.find(".baseEdit").attr('suggetedOriginalValue', '').val('');
                    parentTr.find(".baseEdit").attr('suggetedOriginalValue', '').val('');
                }
            }
        });
    });

    setTimeout(function () { AddFlashingEffect(); }, 1000);

    //set '$' before total and base rate
    $('.perDay').each(function () {
        if ($(this).text().trim().length) {
            $(this).siblings('.daily-rate').prepend('$');
            $(this).siblings('.tv').prepend('$');
        }
    });

    //If myBrand is providing lowest rate than lowestRateAmoungCompetitor then highlight my brand as Green.
    //The calculation should be done on copetitor only. Not using myBrand rates.
    $('.dailytable tbody tr').each(function () {
        var $thisTr = $(this);
        if ($thisTr.find(".highlightGreen").html() != undefined) {
            var competitorGreenTotalRate;
            var myCompanyTotalRate;

            var $highlightGreen = $thisTr.find(".highlightGreen");
            var $myComp;
            //if not GOV shop then compare total value for highlighting
            //if GOV shop then compare base value for highlighting
            if (GlobalLimitSearchSummaryData != null && !GlobalLimitSearchSummaryData.IsGOV) {
                $myComp = $thisTr.find("td[companyID=" + brandID + "] .tv").not('.highlight');
            }
            else {
                $myComp = $thisTr.find("td[companyID=" + brandID + "] .dummybase").not('.highlight');
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

    SetSessionStorage();
}
//AdditionalBase Calcuations
function calculateAdditionalBase(baseValue) {
    var additionalBase = (parseFloat(baseValue) / 7);
    if (typeof (GlobalLimitSearchSummaryData) != 'undefined' && GlobalLimitSearchSummaryData.IsGOV) {
        return parseInt(additionalBase);
    }
    return additionalBase;
}

function calculateNewBase(isBaseEdited, baseValue) {
    if (isBaseEdited) {
        //user edited for base value. This will be used before calculating total value
        return (parseFloat(baseValue) * lengthFactor / 7);
    }
    else {
        //user edited for total value. This will be used before calculating base value
        return (parseFloat(baseValue) * 7 / lengthFactor);
    }
}

//Region for bind filters for search grid end here

//Bind LOR section
function getLOR() {
    var locationBrandID = $('#location ul li.selected').attr('lbid');//LocationID (LocationBrandID)        
    var locationBrandAlias = $('#location ul li.selected').text();
    $.ajax({
        url: 'Search/GetLOR',
        type: 'GET',
        data: { locationBrandID: locationBrandID, locationBrandName: locationBrandAlias },
        async: true,
        dataType: 'json',
        success: function (data) {

            if (data.useLorRtCode != null) {
                $('#hdnUseRateCode').val(data.useLorRtCode);
            }
            if (data.rateSystemName != '') {
                $('#RateSystemSource').html(data.rateSystemName);
            }
            bindDummyLor(data.lors);
            bindLOR(data.lors);
            //send selected rental length value to filter lor bind
            var firstChar;
            if ($.trim($('#length li.selected').text()).length > 0) {
                firstChar = $('#length li.selected').text();
                enableRentalLengths(firstChar.charAt(0));
                highLightLOR(firstChar);
                highLight(firstChar);
            }
            previousRentalLength = '';
            getExtraDayRateValue($('#location ul li.selected').attr('lbid'), firstChar.charAt(0));

        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
        }
    });
}

function bindLOR(jsonData) {
    $('#tsdSystemSelect_Weblink, #LOR_ml, #LOR_m,#updateAllLOR,#UpdateAllLOR_m,#UpdateAllLOR_ml').empty();
    if (jsonData != null && jsonData.length > 0) {
        $.each(jsonData, function (i, val) {
            //var text = $('#lengths [rid=' + jsonData[i] + ']').html();
            //var text = $('#recentLengths [value=' + jsonData[i] + ']').html();
            var text = val.Code;
            $("<li value='" + jsonData[i].MappedID + "'class='ui-selectee' style='cursor: pointer'>" + text + "</li>").appendTo('#tsdSystemSelect_Weblink,#updateAllLOR');
            $("<option value='" + jsonData[i].MappedID + "'>" + text + "</option>").appendTo('#LOR_ml, #LOR_m, #UpdateAllLOR_m, #UpdateAllLOR_ml');
        });
        //set Length in selectable LOR panel as well
        var LORselected = $('#length li.selected').text().trim();
        $('#tsdSystemSelect_Weblink li').each(function () {
            if ($(this).text().trim() == LORselected) {
                $(this).addClass("selected");
                return false;
            }
        });
        $('#updateAllLOR li').each(function () {
            if ($(this).text().trim() == LORselected) {
                $(this).addClass("selected");
                return false;
            }
        });
        $('#UpdateAllLOR_m option, #UpdateAllLOR_ml option').each(function () {
            if ($(this).text().trim() == LORselected) {
                $(this).addClass("selected");
                $(this).prop("selected", true);
                return false;
            }
        });
        // $('.LORSection').show();       
        hideShowRateCode();
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
}

//Send Rates to TSD in daily View mode
function readDailyViewRates() {
    var t = new Date(); console.log("Read daily view rates at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
    if (TSDAjaxURL != undefined && TSDAjaxURL != '') {
        ajaxURl = TSDAjaxURL.TSDUpdateRate;
    }
    var SummaryIdCounter = 0;
    var sendData;
    tsdModel = [];
    var useRateCode = $('#hdnUseRateCode').val() == "true" ? true : false;

    if ($('#daily-rates-table .car-class-img').hasClass('selected')) {
        $('.loader_container_main').show();
        $('#tsdNotification').show();
        $('#daily-rates-table .car-class-img').each(function () {
            if ($(this).hasClass('selected')) {
                SummaryIdCounter += 1;
                var TSD = new Object();
                TSD.RemoteID = searchSummaryData.SearchSummaryID + '-' + SummaryIdCounter;
                TSD.Branch = $('#location li.selected').text().split('-').length > 0 ? $('#location li.selected').text().split('-')[0] : '';
                TSD.CarClass = $(this).find('img').attr('alt');
                TSD.RentalLength = '';
                TSD.RentalLengthIDs = '';
                if (useRateCode) {
                    $('#tsdSystemSelect_Weblink li.selected').each(function (index, element) {
                        if (index == $('#tsdSystemSelect_Weblink li.selected').length - 1) {
                            TSD.RentalLength += $(this).text();
                            TSD.RentalLengthIDs += $(this).attr('value');
                        }
                        else {
                            TSD.RentalLength += $(this).text().trim() + ',';
                            TSD.RentalLengthIDs += $(this).attr('value').trim() + ',';
                        }
                    });
                }
                else {
                    TSD.RentalLength = $('#length li.selected').text().trim();
                    TSD.RentalLengthIDs = $('#length li.selected').attr('value').trim();
                }
                TSD.StartDate = $('#displayDay li.selected').attr('value');
                TSD.DailyRate = commaRemovedNumber($(this).parent().find('.baseEdit').val());
                TSD.ExtraDayRateFactor = $('.extraDayRateFactor').val();
                TSD.ExtraDayRateValue = Math.round((parseFloat(commaRemovedNumber(TSD.DailyRate)) * parseFloat(TSD.ExtraDayRateFactor)), 2);
                //TO DO bind Rate System Source to "RateSystemSource"
                TSD.RentalLengthCount = $('#tsdSystemSelect_Weblink li.selected').length;
                TSD.SuggestedRateId = $(this).parent().find('.suggestedRateId').attr('suggesteRtId');
                TSD.TotalRate = $(this).parent().find('.totalEdit').val();
                if (TSD.DailyRate.trim() != '') {
                    tsdModel.push(TSD);
                }
            }
        });
        var LoggedInUserName = $('#spnUserName span').eq(0).text();
        var RateSystem = $('#RateSystemSource').text();
        var LocationBrandId = $('#location ul li.selected').attr('lbid');
        var LoggedInuserId = $('#LoggedInUserId').val();
        var SearchSummaryId = searchSummaryData.SearchSummaryID;
        var BrandLocation = '';
        if ($('#location ul li.selected').length > 0) {
            BrandLocation = $('#location ul li.selected').text().split('-')[1];
        }
        var Location = '';
        if ($('#location ul li.selected').length > 0) {
            Location = $('#location ul li.selected').text().split('-')[0];
        }

        var opaqueRatesConfiguration = null;
        if ($("#IsOpaqueChk").is(":checked")) {
            opaqueRatesConfiguration = SaveDailyViewOpaqueRates();
        }
        //console.log('daily view rate');
        //console.log(tsdModel);
        //return true;
        if (tsdModel.length > 0) {
            var t = new Date(); console.log("sending daily view rates to TSD at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
            $.ajax({
                url: ajaxURl,
                type: 'POST',
                data: JSON.stringify({
                    'tsdModels': tsdModel, 'UserName': LoggedInUserName, 'RateSystem': RateSystem,
                    'LocationBrandID': LocationBrandId, 'UserId': LoggedInuserId, 'SearchSummaryID': SearchSummaryId, 'IsTetheredUpdate': false
                    , 'BrandLocation': BrandLocation, 'Location': Location, 'opaqueRatesConfiguration': opaqueRatesConfiguration
                }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (data) {
                    var t = new Date(); console.log("got response after sending rates to TSD--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
                    $('#tsdNotification').hide();
                    $('.loader_container_main').hide();
                    FetchLastUpdateTSD();
                    if (data) {

                    }
                },
                error: function (e) {
                    $('#tsdNotification').hide();
                    console.log(e.message);
                    $('.loader_container_main').hide();
                }
            });
            return true;
        }
        else {
            ShowConfirmBox('Please enter valid rates for selected car classes', false);
            return false;
        }
    }
    else {
        ShowConfirmBox('At least one car class must be selected for TSD Update.', false);
        return false;
    }
}

function readClassicViewRates() {
    var t = new Date(); console.log("Read classic view rates at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
    if (TSDAjaxURL != undefined && TSDAjaxURL != '') {
        ajaxURl = TSDAjaxURL.TSDUpdateRate;
    }
    var SummaryIdCounter = 0;
    var sendData;
    tsdModel = [];
    var useRateCode = $('#hdnUseRateCode').val() == "true" ? true : false;
    if ($('table.classictable').find('.ui-selectee').hasClass('selected')) {
        $('.loader_container_main').show();
        $('#tsdNotification').show();
        $('table.classictable').find('.ui-selectee').not('.all-dates').each(function () {
            if ($(this).hasClass('selected')) {
                SummaryIdCounter += 1;
                var TSD = new Object();
                TSD.RemoteID = searchSummaryData.SearchSummaryID + '-' + SummaryIdCounter;
                TSD.Branch = $('#location li.selected').text().split('-').length > 0 ? $('#location li.selected').text().split('-')[0] : '';
                TSD.CarClass = $('#carClass li.selected').text();
                TSD.RentalLength = '';
                TSD.RentalLengthIDs = '';
                if (useRateCode) {
                    $('#tsdSystemSelect_Weblink li.selected').each(function (index, element) {
                        if (index == $('#tsdSystemSelect_Weblink li.selected').length - 1) {
                            TSD.RentalLength += $(this).text();
                            TSD.RentalLengthIDs += $(this).attr('value');
                        }
                        else {
                            TSD.RentalLength += $(this).text().trim() + ',';
                            TSD.RentalLengthIDs += $(this).attr('value').trim() + ',';
                        }
                    });
                }
                else {
                    TSD.RentalLength = $('#length li.selected').text().trim();
                    TSD.RentalLengthIDs = $('#length li.selected').attr('value').trim();
                }
                TSD.StartDate = $(this).attr('formatdate');
                TSD.DailyRate = commaRemovedNumber($(this).parent().find('.baseEdit').val());
                TSD.ExtraDayRateFactor = $('.extraDayRateFactor').val();
                TSD.ExtraDayRateValue = Math.round((parseFloat(commaRemovedNumber(TSD.DailyRate)) * parseFloat(TSD.ExtraDayRateFactor)), 2);
                //TO DO bind Rate System Source to "RateSystemSource"
                TSD.RentalLengthCount = $('#tsdSystemSelect_Weblink li.selected').length;
                TSD.SuggestedRateId = $(this).parent().find('.suggestedRateId').attr('suggesteRtId');
                TSD.TotalRate = $(this).parent().find('.totalEdit').val();
                if (TSD.DailyRate.trim() != '') {
                    tsdModel.push(TSD);
                }
            }
        });
        var LoggedInUserName = $('#spnUserName span').eq(0).text();
        var RateSystem = $('#RateSystemSource').text();
        var LocationBrandId = $('#location ul li.selected').attr('lbid');
        var LoggedInuserId = $('#LoggedInUserId').val();
        var SearchSummaryId = searchSummaryData.SearchSummaryID;
        //console.log('clasic view rate');
        //console.log(tsdModel);
        var BrandLocation = '';
        if ($('#location ul li.selected').length > 0) {
            BrandLocation = $('#location ul li.selected').text().split('-')[1];
        }
        //return true;
        var Location = '';
        if ($('#location ul li.selected').length > 0) {
            Location = $('#location ul li.selected').text().split('-')[0];
        }

        var opaqueRatesConfiguration = null;
        if ($("#IsOpaqueChk").is(":checked")) {
            opaqueRatesConfiguration = SaveClassicViewOpaqueRates();
        }

        if (tsdModel.length > 0) {
            var t = new Date(); console.log("sending classic view rates to TSD at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
            $.ajax({
                url: ajaxURl,
                type: 'POST',
                data: JSON.stringify({
                    'tsdModels': tsdModel, 'UserName': LoggedInUserName, 'RateSystem': RateSystem,
                    'LocationBrandID': LocationBrandId, 'UserId': LoggedInuserId, 'SearchSummaryID': SearchSummaryId, 'IsTetheredUpdate': false
                      , 'BrandLocation': BrandLocation, 'Location': Location, 'opaqueRatesConfiguration': opaqueRatesConfiguration
                }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (data) {
                    var t = new Date(); console.log("got response after sending classic view rates to TSD--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
                    $('#tsdNotification').hide();
                    $('.loader_container_main').hide();
                    FetchLastUpdateTSD();
                    if (data) {

                    }
                },
                error: function (e) {
                    $('#tsdNotification').hide();
                    console.log(e.message);
                    $('.loader_container_main').hide();
                }
            });
            return true;
        }
        else {
            ShowConfirmBox('Please enter valid rates for selected car classes', false);
            return false;
        }
    }
    else {
        ShowConfirmBox('At least one Date must be selected for TSD Update.', false);
        return false;
    }
}

function GetSearchShopCSV(e) {
    e.preventDefault();
    var ajaxURl = '/RateShopper/Search/GetSearchShopCSV/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetSearchShopCSV;
    }
    //var tempDate = new Date();
    //var dateTimeStamp = ("0" + (tempDate.getMonth() + 1)).slice(-2) + "-" + ("0" + tempDate.getDate()).slice(-2) + "-" + tempDate.getFullYear() + " " + ("0" + tempDate.getHours()).slice(-2) + "_" + ("0" + tempDate.getMinutes()).slice(-2);
    var months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var shopDate = "";
    if (typeof (GlobalLimitSearchSummaryData.CreatedDate) != "undefined") {
        shopDate = new Date(parseInt(GlobalLimitSearchSummaryData.CreatedDate.replace("/Date(", "").replace(")/", ""), 10));
        shopDate = ("0" + shopDate.getDate()).slice(-2) + "-" + months[shopDate.getMonth()] + "-" + String(shopDate.getFullYear()).substr(2, 2) + " " + ("0" + shopDate.getHours()).slice(-2) + ":" + ("0" + shopDate.getMinutes()).slice(-2);
    }
    //window.location.href = ajaxURl + "?searchSummaryID=" + SearchSummaryId;
    if (typeof (SearchSummaryId) != 'undefined' && SearchSummaryId > 0) {
        var win = window.open(ajaxURl + "?searchSummaryID=" + SearchSummaryId + "&shopDate=" + shopDate, '_blank');
        win.focus();
    }
    return false;
}

function readTetheredRates(isUpdateClicked) {
    var t = new Date(); console.log("calculating tether rates at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
    if (TSDAjaxURL != undefined && TSDAjaxURL != '') {
        ajaxURl = TSDAjaxURL.TSDUpdateRate;
    }
    var SummaryIdCounter = 0;
    selectedCarClasses = [];
    tetherModel = [];
    var useRateCode = $('#hdnUseRateCode').val() == "true" ? true : false;
    if ($('#daily-rates-table .car-class-img').hasClass('selected')) {
        $('#daily-rates-table .car-class-img').each(function () {
            if ($(this).hasClass('selected')) {
                SummaryIdCounter += 1;
                var carClassId = $(this).find('img').attr('classid');
                var TETHER = new Object();
                TETHER.RemoteID = searchSummaryData.SearchSummaryID + '-' + SummaryIdCounter;
                TETHER.Branch = $('#location li.selected').text().split('-').length > 0 ? $('#location li.selected').text().split('-')[0] : '';
                TETHER.CarClass = $(this).find('img').attr('alt');
                TETHER.RentalLength = '';
                TETHER.RentalLengthIDs = '';
                if (useRateCode) {
                    $('#tsdSystemSelect_Weblink li.selected').each(function (index, element) {
                        if (index == $('#tsdSystemSelect_Weblink li.selected').length - 1) {
                            TETHER.RentalLength += $(this).text();
                            TETHER.RentalLengthIDs += $(this).attr('value');
                        }
                        else {
                            TETHER.RentalLength += $(this).text().trim() + ',';
                            TETHER.RentalLengthIDs += $(this).attr('value').trim() + ',';
                        }
                    });
                }
                else {
                    TETHER.RentalLength = $('#length li.selected').text().trim();
                    TETHER.RentalLengthIDs = $('#length li.selected').attr('value').trim();
                }
                TETHER.StartDate = $('#displayDay li.selected').attr('value');
                var dollerValue = $('#DollerID_' + carClassId).attr('defaultdollervalue');
                var percentValue = $('#PercentageID_' + carClassId).attr('defaultPercentValue');
                if (typeof (dollerValue) == "undefined") { dollerValue = ''; }
                if (typeof (percentValue) == "undefined") { percentValue = ''; }
                //if not GOV then 
                if (GlobalLimitSearchSummaryData != null && !GlobalLimitSearchSummaryData.IsGOV) {
                    if (dollerValue != '' || percentValue != '') {
                        TETHER.DailyRate = commaRemovedNumber($('#EDBaseRateID_' + carClassId).attr('defaulltValue'));
                    }
                    else {
                        TETHER.DailyRate = 0;
                    }
                }
                    //else do not check for blank/zero in tether gap settings
                else {
                    TETHER.DailyRate = commaRemovedNumber($('#EDBaseRateID_' + carClassId).attr('defaulltValue'));
                }
                //TETHER.ExtraDayRateFactor = $('#hdnTetherBrandExtraDayRate').val();
                TETHER.ExtraDayRateFactor = $('.extraDayRateFactor').val();
                TETHER.ExtraDayRateValue = Math.round((parseFloat(TETHER.DailyRate) * parseFloat(TETHER.ExtraDayRateFactor)), 2);;
                //TO DO bind Rate System Source to "RateSystemSource"
                TETHER.RentalLengthCount = $('#tsdSystemSelect_Weblink li.selected').length;
                tetherModel.push(TETHER);
            }
        });

        var LoggedInUserName = $('#spnUserName span').eq(0).text();
        var RateSystem = $('#RateSystemSource').text();
        var LocationBrandId = $('#location ul li.selected').attr('lbid');
        var LoggedInuserId = $('#LoggedInUserId').val();
        var SearchSummaryId = searchSummaryData.SearchSummaryID;
        //console.log('tether view rate daily');
        //console.log(tetherModel);
        var BrandLocation = $('#tetherValues #rightTetherTitle').attr('brandCode');
        var Location = '';
        if ($('#location ul li.selected').length > 0) {
            Location = $('#location ul li.selected').text().split('-')[0];
        }
        //return true;
        //if (isQuickViewShop)
        //{
        //    IsQuickUpdateAndNext = true;
        //    IgnoreAndNext();
        //}

        var opaqueRatesConfiguration = null;
        if ($("#IsOpaqueChk").is(":checked")) {
            opaqueRatesConfiguration = SaveDailyViewOpaqueRates();
        }
        $('#daily-rates-table .selected').removeClass('selected');
        var t = new Date(); console.log("Sending tether daily view rates to TSD at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
        $.ajax({
            url: ajaxURl,
            type: 'POST',
            data: JSON.stringify({
                'tsdModels': tetherModel, 'UserName': LoggedInUserName, 'RateSystem': RateSystem,
                'LocationBrandID': LocationBrandId, 'UserId': LoggedInuserId, 'SearchSummaryID': SearchSummaryId, 'IsTetheredUpdate': true
                 , 'BrandLocation': BrandLocation, 'Location': Location, 'opaqueRatesConfiguration': opaqueRatesConfiguration
            }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (data) {
                var t = new Date(); console.log("Got response after tether daily view update to TSD at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());

            },
            error: function (e) {
                $('.loader_container_source').hide();
                console.log(e.message);
            }
        });
        if (isUpdateClicked) {
            resetEdit();
        }
        else {
            setTimeout(function () {
                nextDayClicked();
                getSearchData(false);
                assignCtrlValues('', '_m');
                assignCtrlValues('', '_ml');
            }, 100);
        }
    } else {
        ShowConfirmBox('At least one car class must be selected for TSD Update.', false);
        return false;
    }
}


function readClassicTetheredRates(isUpdateClicked) {
    var t = new Date(); console.log("Calculating tether classic view rates at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
    if (TSDAjaxURL != undefined && TSDAjaxURL != '') {
        ajaxURl = TSDAjaxURL.TSDUpdateRate;
    }
    var SummaryIdCounter = 0;
    selectedCarClasses = [];
    tetherModel = [];
    var i = 0;
    var carClassId = $('#carClass li.selected').attr('value');
    var useRateCode = $('#hdnUseRateCode').val() == "true" ? true : false;
    if ($('.classictable .dates').hasClass('selected')) {
        $('.classictable .dates.selected').each(function () {
            var selectedDate = $(this).attr('formatdate');
            SummaryIdCounter += 1;
            var TETHER = new Object();
            TETHER.RemoteID = searchSummaryData.SearchSummaryID + '-' + SummaryIdCounter;
            TETHER.Branch = $('#location li.selected').text().split('-').length > 0 ? $('#location li.selected').text().split('-')[0] : '';
            TETHER.CarClass = $('#carClass li.selected').text().trim();
            TETHER.RentalLength = '';
            TETHER.RentalLengthIDs = '';
            if (useRateCode) {
                $('#tsdSystemSelect_Weblink li.selected').each(function (index, element) {
                    if (index == $('#tsdSystemSelect_Weblink li.selected').length - 1) {
                        TETHER.RentalLength += $(this).text();
                        TETHER.RentalLengthIDs += $(this).attr('value');
                    }
                    else {
                        TETHER.RentalLength += $(this).text().trim() + ',';
                        TETHER.RentalLengthIDs += $(this).attr('value').trim() + ',';
                    }
                });
            }
            else {
                TETHER.RentalLength = $('#length li.selected').text().trim();
                TETHER.RentalLengthIDs = $('#length li.selected').attr('value').trim();
            }
            var controlToIterate = $('#tableTetherClassicValue tr');
            TETHER.StartDate = selectedDate;
            var dollerValue = $('#DollerID_' + carClassId).attr('defaultdollervalue');
            var percentValue = $('#PercentageID_' + carClassId).attr('defaultPercentValue');
            if (typeof (dollerValue) == "undefined") { dollerValue = ''; }
            if (typeof (percentValue) == "undefined") { percentValue = ''; }
            //if not GOV then 
            if (GlobalLimitSearchSummaryData != null && !GlobalLimitSearchSummaryData.IsGOV) {
                if (dollerValue != '' || percentValue != '') {
                    if ($('#tableTetherClassicValue tr[formatDate="' + selectedDate + '"]').length > 0) {
                        TETHER.DailyRate = commaRemovedNumber($('#tableTetherClassicValue tr[formatDate="' + selectedDate + '"]').find('#EDBaseRateID_' + carClassId).attr('defaulltValue'));
                    }
                }
                else {
                    TETHER.DailyRate = 0;
                }
            }
                //else do not check for blank/zero in tether gap settings
            else {
                if ($('#tableTetherClassicValue tr[formatDate="' + selectedDate + '"]').length > 0) {
                    TETHER.DailyRate = commaRemovedNumber($('#tableTetherClassicValue tr[formatDate="' + selectedDate + '"]').find('#EDBaseRateID_' + carClassId).attr('defaulltValue'));
                }
            }
            TETHER.ExtraDayRateFactor = $('.extraDayRateFactor').val();
            TETHER.ExtraDayRateValue = Math.round((parseFloat(commaRemovedNumber(TETHER.DailyRate)) * parseFloat(TETHER.ExtraDayRateFactor)), 2);;
            //TO DO bind Rate System Source to "RateSystemSource"
            TETHER.RentalLengthCount = $('#tsdSystemSelect_Weblink li.selected').length;
            tetherModel.push(TETHER);
            // console.log(tetherModel);

        });

        var LoggedInUserName = $('#spnUserName span').eq(0).text();
        var RateSystem = $('#RateSystemSource').text();
        var LocationBrandId = $('#location ul li.selected').attr('lbid');
        var LoggedInuserId = $('#LoggedInUserId').val();
        var SearchSummaryId = searchSummaryData.SearchSummaryID;
        //console.log('tether classic view rate');
        //console.log(tetherModel);
        var BrandLocation = $('#tetherValues #rightClassicTetherTitle').attr('brandCode');
        var Location = '';
        if ($('#location ul li.selected').length > 0) {
            Location = $('#location ul li.selected').text().split('-')[0];
        }

        var opaqueRatesConfiguration = null;
        if ($("#IsOpaqueChk").is(":checked")) {
            opaqueRatesConfiguration = SaveClassicViewOpaqueRates();
        }
        $('table.classictable .ui-selectee').removeClass('selected');
        var t = new Date(); console.log("Sending tether classic view rates at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());
        //return true;
        $.ajax({
            url: ajaxURl,
            type: 'POST',
            data: JSON.stringify({
                'tsdModels': tetherModel, 'UserName': LoggedInUserName, 'RateSystem': RateSystem,
                'LocationBrandID': LocationBrandId, 'UserId': LoggedInuserId, 'SearchSummaryID': SearchSummaryId, 'IsTetheredUpdate': true
                 , 'BrandLocation': BrandLocation, 'Location': Location, 'opaqueRatesConfiguration': opaqueRatesConfiguration
            }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (data) {
                var t = new Date(); console.log("Got response of tether classic view rates to TSD at--->" + t.toLocaleTimeString() + "." + t.getMilliseconds());

            },
            error: function (e) {
                $('.loader_container_source').hide();
                console.log(e.message);
            }
        });

        if (isUpdateClicked) {
            resetEdit();
        }
        else {
            setTimeout(function () {
                nextCarClassClicked();
            }, 100);
        }
    } else {
        ShowConfirmBox('At least one Date must be selected for TSD Update.', false);
        return false;
    }
}
//commented as not used now
//function getLocationBrand(isUpdateClicked) {
//    if (TSDAjaxURL != undefined && TSDAjaxURL != '') {
//        ajaxURl = TSDAjaxURL.GetLocationBrandDet;
//    }
//    var selectedLocation = $('#location li.selected').val();
//    var selectedBrand = $("#rightTetherTitle").attr('brandid');
//    var $lenghSelected = $('#length li.selected').text().trim().charAt(0);

//    $.ajax({
//        url: ajaxURl,
//        type: 'GET',
//        async: true,
//        data: { locationId: selectedLocation, BrandId: selectedBrand, rentalLength: $lenghSelected },
//        dataType: 'text',
//        success: function (data) {
//            if (data != '') {
//                var locationBrandId = data;
//                $('#hdnTetherBrandLocationId').val(locationBrandId);
//                //assign the extra daily rate entered value to hidden field 
//                $('#hdnTetherBrandExtraDayRate').val($('.extraDayRateFactor').val());
//                var viewSelected = $('#viewSelect li.selected').text().trim();
//                if (viewSelected == 'daily') {
//                    readTetheredRates(isUpdateClicked);
//                }
//                else if (viewSelected == 'classic') {
//                    readClassicTetheredRates(isUpdateClicked);
//                }
//            }
//        },
//        error: function (e) {
//            $('.loader_container_source').hide();
//            console.log(e.message);
//        }
//    });
//    tetherPrevRental = $lenghSelected;
//}

function validateDecimal(element) {
    var $this = element;
    var value = $this.val();
    var regExp = new RegExp(/^\d{1,2}(\.\d{1,2})?$/);
    if (!regExp.test(value)) {
        MakeTagFlashable($this);
        AddFlashingEffect();
        return false;
    }
    else {
        RemoveFlashableTag($this);
        AddFlashingEffect();
    }
}

function validateLorSelected(view) {
    var useRateCode = $('#hdnUseRateCode').val() == "true" ? true : false;
    if (useRateCode) {
        if ($('#tsdSystemSelect_Weblink li').hasClass('selected')) {
            return true;
        }
        else if (view == 'daily' && $('#daily-rates-table .car-class-img.selected').length == 0 && $('#tsdSystemSelect_Weblink li.selected').length == 0) {
            ShowConfirmBox('At least one LOR and one car class must be selected for TSD Update.', false);
            return false;
        }
        else if (view == 'classic' && $('.classictable .dates.selected').length == 0 && $('#tsdSystemSelect_Weblink li.selected').length == 0) {
            ShowConfirmBox('At least one LOR and one Date must be selected for TSD Update.', false);
            return false;
        }
        else {
            ShowConfirmBox('At least one LOR must be selected for TSD Update.', false);
            return false;
        }
    }
    else {
        return true;
    }
}
function bindDummyLor(data) {
    $('#dummyLor').empty();
    if (data != null && data.length > 0) {
        $.each(data, function (i, val) {
            //   var text = $('#lengths [rid=' + data[i] + ']').html();
            //var text = $('#recentLengths [value=' + data[i] + ']').html();
            var text = val.Code;
            $("<option value='" + data[i] + "'>" + text + "</option>").appendTo('#dummyLor');
        });
    }
}

function resetEditClicked() {
    scrollUserTop();
    var message = "Do you want to reset the values?";
    ShowConfirmBox(message, true, resetEdit, $(this));
}

function resetEdit() {
    var table = $('.dailytable').is(':visible') ? '.dailytable' : '.classictable';
    if ($(table).is(':visible')) {
        $(table + ' .baseEdit, .totalEdit, .additionalBase').each(function () {
            var $this = $(this);
            RemoveFlashableTag($this);
            var orgionalVal = $this.attr('suggetedoriginalvalue');
            $this.val(orgionalVal);

            //start blinking again if value equal to global limit value
            if ($this.hasClass('baseEdit')) {
                var minBase = $this.attr('minbaserate');
                var maxBase = $this.attr('maxbaserate');
                if (typeof minBase !== typeof undefined && minBase !== false && typeof maxBase !== typeof undefined && maxBase !== false) {
                    if ($.isNumeric(minBase) && $.isNumeric(maxBase)) {
                        if (parseFloat(minBase) >= parseFloat(orgionalVal)) {
                            //$this.addClass('glv');
                            //MakeTagFlashable($this);
                        }
                        else if (parseFloat(maxBase) > 0 && parseFloat(maxBase) <= parseFloat(orgionalVal)) {
                            //$this.addClass('glv');
                            //MakeTagFlashable($this);
                        }
                    }
                }
            }
        });
        //reset carClass/ Date selection for TSD update
        $(table + ' td.ui-selectee.selected').removeClass('selected');
        $('#all-car-classes').removeClass('selected');

        //Reset Rental length selection for Extra rate update
        $('#tsdSystemSelect_Weblink .selected, #LOR_m .selected, #LOR_ml .selected').removeClass('selected');
        $('#tsdSystemSelect_Weblink .prevSelected, #LOR_m .prevSelected, #LOR_ml .prevSelected').addClass('selected');

        //Reset Extra rate value
        $('.extraDayRateFactor').val($('.extraDayRateFactor').attr('oldvalue'));
        RemoveFlashableTag('.extraDayRateFactor');
        $(".dropped_bar").removeClass("dropped_bar").addClass("drag_bar_disable");
    }
    AddFlashingEffect();
}


function SetSessionStorage() {

    //Expecting user to wait for at least 250ms on search page and then save the last paramters
    setTimeout(function () {
        var ctrlState = new Object();
        ctrlState.searchSummaryID = SearchSummaryId;
        ctrlState.scrapperSourceID = ($('#dimension-source li').eq(0).attr('value'));
        ctrlState.locationBrandID = $('#location ul li.selected').attr('lbid');
        ctrlState.rentallengthID = ($('#length li').eq(0).attr('value'));
        ctrlState.arrivalDate = $('#displayDay li').eq(0).attr('value');
        ctrlState.carClassId = ($('#carClass li').eq(0).attr('value'));
        ctrlState.view = $('#viewSelect li').eq(0).attr('value');


        //For Quickview
        ctrlState.isQuickView = isQuickViewShop;
        ctrlState.isChanged = $('#rbChanged').eq(0).prop('checked');
        if (isQuickViewShop != undefined && JSON.parse(isQuickViewShop)) {
            ctrlState.changedRentalLength = $('#lengthDateCombinationChanged ul li.selected').attr('rentalLengthId') != undefined ? $('#lengthDateCombinationChanged ul li.selected').attr('rentalLengthId') : ctrlState.changedRentalLength;
            ctrlState.changedDate = $('#lengthDateCombinationChanged ul li.selected').attr('formattedDate');
            ctrlState.unChangedRentalLength = $('#lengthDateCombinationUnChanged ul li.selected').attr('rentalLengthId');
            ctrlState.unChangedDate = $('#lengthDateCombinationUnChanged ul li.selected').attr('formattedDate');
        }

        sessionStorage.setItem('ctrlState', JSON.stringify(ctrlState));
    }, 250);
}

function CheckSessionStorageNLoadData() {
    if (typeof Storage !== "undefined" && sessionStorage.getItem('ctrlState') != null) {
        loadingPrevResult = true;
        if ($('#pastSearches li').length <= 0) {
            setTimeout(function () {
                CheckSessionStorageNLoadData();
            }, 150);
            return;
        }


        var ctrlState = JSON.parse(sessionStorage.getItem('ctrlState'));

        var $pastSearchLi = $('.pastSearchul li[value="' + ctrlState.searchSummaryID + '"]');

        if ($pastSearchLi.length <= 0) {
            sessionStorage.removeItem('ctrlState');
            CheckSessionStorageNLoadData();
        }
        currentView = $.trim($('#viewSelect .selected').attr('value'));
        SearchSummaryId = ctrlState.searchSummaryID;

        $pastSearchLi.click();
        //Check Quick view button enable/disable logic
        CheckQuickViewButtonEnableDisable();
        GetApplicableOpaqueRateCodes();

        //showQuickViewControl($pastSearchLi.attr('isQuickView'));
        //bindlengthDateCombination(SearchSummaryId);

        $('.loader_container_main').show();
        //FetchLastUpdateTSD();
        setTimeout(function () { bindDataUsingSessionStorage(ctrlState); }, 250);
        //get selection ctrl

    }
    else {
        //get default data
        $.ajax({
            url: 'Search/GetSearchGridDailyViewDataDefault',
            type: 'GET',
            data: { LoggedInUserID: $('#LoggedInUserId').val(), isAdminUser: $("#hdnIsAdminUser").val() },
            dataType: 'json',
            success: function (data) {
                if (data != null) {
                    if (data.SearchSummaryId > 0) {
                        var $pastSearchLi = $('.pastSearchul li[value="' + data.SearchSummaryId + '"]');
                        $pastSearchLi.click();
                    }
                    waitandbindSearchFilters(data);
                }
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                $('.loader_container_main').hide();
                searchViewModel.rates(null);
                searchViewModel.headers(null);
                bindSearchFilters('_m');
                bindSearchFilters('_ml');
                bindSearchFilters();
                setTimeout(function () { SearchSummaryId = searchSummaryData.SearchSummaryID; SetSessionStorage(); }, 550);
                $('<tr class="NoResultsFound"><td colspan="4" class="red bold" style="align:left">No Rates Found</td></tr>').appendTo('#daily-rates-table tbody');
                $('#TetherRate, #update, #updaten, #IsTetherUser, .resetEdit').prop('disabled', true).addClass("disable-button");
                setLastUpdateTSD('');
            }
        });
    }
}

function bindDataUsingSessionStorage(ctrlState) {
    //SearchSummaryId = ctrlState.searchSummaryID;
    //var $pastSearchLi = $('.pastSearchul li[value="' + SearchSummaryId + '"]');
    //$pastSearchLi.click();
    bindSearchFilters();
    var $source = $('#dimension-source ul li[value="' + ctrlState.scrapperSourceID + '"]').first();
    var $location = $('#location ul li[lbid="' + ctrlState.locationBrandID + '"]').first();
    var $rentalLength = $('#length ul li[value="' + ctrlState.rentallengthID + '"]').first();
    var $date = $('#displayDay ul li[value="' + ctrlState.arrivalDate + '"]').first();
    var $carClass = $('#carClass ul li[value="' + ctrlState.carClassId + '"]').first();
    var $viewSelect = $('#viewSelect ul li[value="' + ctrlState.view + '"]').first();

    if (typeof (ctrlState.isQuickView) != "undefined") {
        showQuickViewControl(ctrlState.isQuickView);
    }
    else {
        showQuickViewControl(false);
    }

    //Set filter selections
    $('#dimension-source li').removeClass('selected').eq(0).text($source.text()).attr('value', $source.addClass('selected').attr('value'));


    $('#location li').removeClass('selected').eq(0).text($location.text()).attr('value', $location.addClass('selected').attr('value'));

    $('#length li').removeClass('selected').eq(0).text($rentalLength.text()).attr('value', $rentalLength.addClass('selected').attr('value'));

    $('#displayDay li').removeClass('selected').eq(0).text($date.text()).attr('value', $date.addClass('selected').attr('value'));

    $('#carClass li').removeClass('selected').eq(0).text($carClass.text()).attr('value', $carClass.addClass('selected').attr('value'));

    $('#viewSelect li').removeClass('selected').eq(0).text($viewSelect.text()).attr('value', $viewSelect.addClass('selected').attr('value'));

    //For Quick View
    if (typeof (ctrlState.isQuickView) != "undefined" && JSON.parse(ctrlState.isQuickView)) {

        if (JSON.parse(ctrlState.isChanged) == 1) {
            $('[Id$="rbChanged"]').each(function () {
                $(this).prop('checked', true);
            });
        } else {
            $('[Id$="rbUnChanged"]').each(function () {
                $(this).prop('checked', true);
            });
        }

        var quickViewDropdownParameters = new Object();
        quickViewDropdownParameters.changedRentalLength = ctrlState.changedRentalLength;
        quickViewDropdownParameters.changedDate = ctrlState.changedDate;
        quickViewDropdownParameters.unChangedRentalLength = ctrlState.unChangedRentalLength;
        quickViewDropdownParameters.unChangedDate = ctrlState.unChangedDate;
        quickViewDropdownParameters.isCalledForSettingSessionStorage = true;

        bindlengthDateCombination(SearchSummaryId, undefined, undefined, undefined, quickViewDropdownParameters);
    }

    $('.LOR').hide();

    if ($('#viewSelect .selected').attr('value') != 'classic') {
        //Bind controls for all element for all view

        bindSearchFilters('_m');
        bindSearchFilters('_ml');
        assignCtrlValues('', '_m');
        assignCtrlValues('', '_ml');
        $('.showSelectedDate').html($date.text());
        getSearchData(false);
    }
    else {
        showView('classic');
        setTimeout(function () {
            var $lengthValue = $('#length li.selected').text().trim();
            enableRentalLengths($lengthValue.charAt(0));
            highLightLOR($lengthValue);
            highLight($lengthValue);
            getExtraDayRateValue($('#location ul li.selected').attr('lbid'), $lengthValue.charAt(0));
        }, 250);
    }

    //Reset selected location brand id and bool value tether popup opened
    $('#hdnTetherBrandLocationId').val('0');
    tetherShopFirstClick = false;
    //View Result End
    //sessionStorage.removeItem('ctrlState');
}

function hideShowRateCode() {
    var useRateCode = $('#hdnUseRateCode').val() == "true" ? true : false;
    if (useRateCode) {
        $('#tsdSystemSelect_Weblink,#updateAllLOR,#UpdateAllLOR_m, #UpdateAllLOR_ml').show();

        $('#LOR_m').show();
        $('#LOR_ml').show();
        $('.rateCode').html('LOR');
        $('[id=lorRateCode]').hide();
        $('#UpdateTSDRateCode').hide();
    }
    else {
        $('#tsdSystemSelect_Weblink,#updateAllLOR,#UpdateAllLOR_m, #UpdateAllLOR_ml').hide();
        $('#LOR_m').hide();
        $('#LOR_ml').hide();
        $('.rateCode').html('Rate Code :');
        $('[id=lorRateCode]').show();
        $('#UpdateTSDRateCode').show();
    }
}

//Methods for Last TSD Udpated Message in footer
function FetchLastUpdateTSD() {
    $('p.red').html('');

    var searchSummaryID = SearchSummaryId;
    var scrapperSourceID = ($('#dimension-source li').eq(0).attr('value'));
    var locationBrandID = $('#location ul li.selected').attr('lbid');
    var locationID = $('#location ul li.selected').val();
    var brandID = $('#location ul li.selected').attr('brandid');
    var rentallengthID = ($('#length li').eq(0).attr('value'));
    var arrivalDate = $('#displayDay li').eq(0).attr('value');
    var carClassId = $('#carClass li').eq(0).attr('value');
    var view = $('#viewSelect li.selected').text().trim();

    var ajaxURl = '/RateShopper/Search/GetLastUpdateOnTSD/';
    if (MasterAjaxURL != undefined && MasterAjaxURL != '') {
        ajaxURl = MasterAjaxURL.GetLastUpdated;
    }
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        async: true,
        dataType: 'text',
        data: {
            searchSummaryID: searchSummaryID, scrapperSourceID: scrapperSourceID
            , locationBrandID: locationBrandID, locationID: locationID
        , brandID: brandID, rentallengthID: rentallengthID
        , carClassId: carClassId, arrivalDate: arrivalDate, view: view
        },
        success: function (data) {
            setLastUpdateTSD(data)
        },
        error: function (e) {
            $('.loader_container_source').hide();
            setLastUpdateTSD('');
            console.log(e.message);
        }
    });
}

function setLastUpdateTSD(data) {
    var stringGen = '';
    if (data != '') {
        var userName = data.split('|')[0];
        var dateTime = data.split('|')[1];
        stringGen = 'Last Rates updated by ' + userName + ' at ' + dateTime + ', Eastern Standard Time (EST)';
    }
    $('p.red').html(stringGen);
}
//Methods end for Last TSD Udpated Message in footer

/* QUICK VIEW INTERACTON UI PURPOSE ONLY*/
$(document).ready(function () {



    $(".view-tabs").on('click', 'li', function (e) {


        var self = $(this);



        var $target = $(self.data('target'));
        var $targetParent = $target.parent();
        $targetParent.children().hide();
        $target.show();


        var $selfParent = self.parent();
        $selfParent.children().removeClass('selected');
        self.addClass('selected');


    });

    $("#mobile-header").on('click', 'img', function (e) {


        var self = $(this);
        var $target = $(self.data('target'));
        var $targetParent = $target.parent();
        $targetParent.children().hide();
        $target.show();
    });

    $('.search-results').on('click', '.quick-view-button', function (e) {
        ResetQuickViewSchedulePopup();
        var locationBrandId = $("#result-section ul#location .hidden li.selected").attr("lbid");
        if (typeof (locationBrandId) != 'undefined' && locationBrandId > 0) {
            var carClassIds = GlobalLimitSearchSummaryData.CarClassIDs;
            if (typeof (carClassIds) != 'undefined' && carClassIds != '') {
                if (carClassIds.lastIndexOf(",") == carClassIds.length - 1) {
                    carClassIds = carClassIds.substring(0, carClassIds.lastIndexOf(","));
                }
            }
            GetQuickViewCompetitors(locationBrandId, SearchSummaryId, carClassIds);
        }

        $(".quickview-schedule-modal").removeClass('hidden').show();
        $(".modal-backdrop").removeClass('hidden').show();
        e.preventDefault();
    });

    $('.quickview-schedule-popup').on('click', '.close-quick-view-popup', function (e) {
        ResetQuickViewSchedulePopup();
        e.preventDefault();
        $(".quickview-schedule-modal").addClass('hidden').hide();
        $(".modal-backdrop").addClass('hidden').hide();

    });

    $('.collapse-anchor').click(function () {


        var self = $(this);
        var $target = $(self.data('collapse-target'));
        var image = self.find('img');

        handleImageSrc.call(image);


        function handleImageSrc() {
            if ($(this).attr('src').indexOf('expand') > 0) {
                $(this).attr('src', 'images/Search-collapse.png');
            } else {
                $(this).attr('src', 'images/expand.png')
            }

        }

        $target.slideToggle();
    });
});


function lengthDateCombination(data) {
    this.rentalLengthId = data.RentalLengthId;
    this.formattedDate = data.FormattedDate;
    this.displayText = data.DisplayText;
    this.hitCall = function (rowClick) {
        lengthDateCombinationClicked(rowClick.rentalLengthId, rowClick.formattedDate);
    };
}


function showQuickViewShop(searchSummaryID, isChanged, rentalLengthId, formattedDate) {
    if ($('.mobileSearchICO').is(':visible')) {
        $('.mobileSearchICO').click();//For viewing the direct search Grid in mobile view
    }
    var $pastSearchLi = $('.pastSearchul li[value="' + searchSummaryID + '"]').eq(0);
    var userName = $('#qvuser ul li.selected').text();
    if ($pastSearchLi.length <= 0) {
        var isAdmin = $("#hdnIsAdminUser").val();
        if (isAdmin.toUpperCase() == "FALSE") {
            var quickViewSelectedUser = $("#qvuser .hidden li.selected").attr("value");
            var recentsearchSelectedUser = $('#tableSearchFilters #recentUsers ul li.selected').attr("value");
            if (quickViewSelectedUser != recentsearchSelectedUser) {
                ShowConfirmBox("Please filter shops with user " + userName + " in the Recent Search section of the Search page.", false);
                return false;
            }
        }
        ShowConfirmBox("No result found", false);
        return false;
    }

    SearchSummaryId = searchSummaryID;
    $pastSearchLi.click();
    showQuickViewControl(true);
    showQuickViewShopResult(searchSummaryID, isChanged, rentalLengthId, formattedDate);

    //The set timeout is intentional to switch tabs after a delay so that user don't see the dropdown open/close however, the dropdowns event will not wait for .3 seconds
    setTimeout(function () {
        $('.defaultViewTab').click();
    }, 300);

}


function showQuickViewShopResult(searchSummaryID, isChanged, rentalLengthId, formattedDate) {
    CheckTetherGlobalLimitAjaxCall = false;
    CheckTetherFormulaAjaxCall = false;
    GlobalLimitSearchSummaryData = "";
    GlobalLimitSearchSummaryData = searchSummaryData;//used for Global limit implementation
    FinalTetherValueData = [];
    SearchSummaryId = searchSummaryID;
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
    if (rentalLengthId != null && rentalLengthId != undefined && rentalLengthId > 0) {
        bindlengthDateCombination(searchSummaryID, isChanged, rentalLengthId, formattedDate);
    }
    else {
        bindlengthDateCombination(searchSummaryID, true)
    }


    bindSearchFilters();
    //getCurrentFormula();
    //getLOR();
    $('.LOR').hide();

    //switch to daily view
    if ($('#viewSelect ul li.selected').attr('value') == 'classic') {
        //Bind controls for all element for all view
        $('#viewSelect ul li.selected').removeClass('selected').siblings('li').eq(0).addClass('selected');
        $('#viewSelect li').eq(0).text($('#viewSelect ul li.selected').text()).attr('value', $('#viewSelect ul li.selected').attr('value'));
    }

    //getSearchData(false)

    bindSearchFilters('_m');
    bindSearchFilters('_ml');


    $('#hdnTetherBrandLocationId').val('0');
    tetherShopFirstClick = false;
}


function bindlengthDateCombination(searchSummaryID, isChanged, rentalLengthId, formattedDate, quickViewDropdownParameters) {
    $('.loader_container_carclass').show();

    var ajaxURl = 'QuickView/GetlengthDateCombination/';
    if (quickURLSettings != undefined && quickURLSettings != '') {
        ajaxURl = quickURLSettings.GetlengthDateCombination;
    }
    $.ajax({
        url: ajaxURl,
        data: { SearchSummaryID: searchSummaryID },
        type: 'GET',
        async: true,
        dataType: 'json',
        success: function (data) {
            $('.loader_container_carclass').hide();
            if (data) {
                var srcsChanged = $.map(data.LengthDateCombinationChanged, function (item) { return new lengthDateCombination(item); });
                searchViewModel.lengthDateCombinationChanged(srcsChanged);

                var srcsUnchanged = $.map(data.LengthDateCombinationUnChanged, function (item) { return new lengthDateCombination(item); });
                searchViewModel.lengthDateCombinationUnChanged(srcsUnchanged);

                //Disable radio buttons if there are no changed/uncghanged tags
                srcsChanged.length > 0 ? $('[Id$="rbChanged"]').removeAttr('disabled') : $('[Id$="rbChanged"]').attr('disabled', 'disabled');
                srcsUnchanged.length > 0 ? $('[Id$="rbUnChanged"]').removeAttr('disabled') : $('[Id$="rbUnChanged"]').attr('disabled', 'disabled');

                if (quickViewDropdownParameters != undefined && quickViewDropdownParameters.isCalledForSettingSessionStorage) {

                    setTimeout(function () {
                        var changedCombo = $('#lengthDateCombinationChanged ul li[rentallengthid="' + quickViewDropdownParameters.changedRentalLength + '"][formatteddate="' + quickViewDropdownParameters.changedDate + '"]').addClass('selected').eq(0);
                        $('[Id$="lengthDateCombinationChanged"]').each(function () {
                            $(this).find('li').eq(0).text(changedCombo.text()).attr('value', changedCombo.attr('value')).attr('rentallengthid', rentalLengthId).attr('formattedDate', formattedDate);
                        });

                        var unChangedCombo = $('#lengthDateCombinationUnChanged ul li[rentallengthid="' + quickViewDropdownParameters.unChangedRentalLength + '"][formatteddate="' + quickViewDropdownParameters.unChangedDate + '"]').addClass('selected').eq(0);
                        $('[Id$="lengthDateCombinationUnChanged"]').each(function () {
                            $(this).find('li').eq(0).text(unChangedCombo.text()).attr('value', unChangedCombo.attr('value')).attr('rentallengthid', rentalLengthId).attr('formattedDate', formattedDate);
                        });

                        $('.noQuickView').hide();
                        if ($('#rbChanged:checked').length > 0) {
                            showLengthDateCombinationChanged(true);
                        } else {
                            showLengthDateCombinationChanged(false);
                        }

                        ShowClassicQuickViewRentalLength();
                    }, 500);
                }
                else {
                    var ctrlId;
                    if (isChanged != undefined && JSON.parse(isChanged) && srcsChanged.length > 0) {

                        $('[Id$="rbChanged"]').each(function () {
                            $(this).prop('checked', true);
                        });

                        ctrlId = '[Id$="lengthDateCombinationChanged"]';

                        $('[Id$="lengthDateCombinationUnChanged"]').each(function () {
                            $(this).find('ul li').eq(0).addClass('selected');
                            $(this).find('li').eq(0).text($(this).find('ul li.selected').text()).attr('value', $(this).find('ul li.selected').attr('value'));
                        });
                    }
                    else {
                        $('[Id$="rbUnChanged"]').each(function () {
                            $(this).prop('checked', true);
                        });

                        ctrlId = '[Id$="lengthDateCombinationUnChanged"]';

                        $('[Id$="lengthDateCombinationChanged"]').each(function () {
                            $(this).find('ul li').eq(0).addClass('selected');
                            $(this).find('li').eq(0).text($(this).find('ul li.selected').text()).attr('value', $(this).find('ul li.selected').attr('value'));
                        });
                    }

                    if (rentalLengthId != null && rentalLengthId != undefined && rentalLengthId > 0) {
                        $(ctrlId).each(function () {
                            $(this).find('li.selected').removeClass('selected');
                            var lengthDayCombo = $(this).find('ul li[rentallengthid="' + rentalLengthId + '"][formatteddate="' + formattedDate + '"]').addClass('selected').eq(0);
                            $(this).find('li').eq(0).text(lengthDayCombo.text()).attr('value', lengthDayCombo.attr('value')).attr('rentallengthid', rentalLengthId).attr('formattedDate', formattedDate);
                        });

                        lengthDateCombinationClicked($(ctrlId + ' ul li.selected').eq(0).attr('rentallengthid'), $(ctrlId + ' ul li.selected').eq(0).attr('formattedDate'));
                    }
                    else {
                        $(ctrlId).each(function () {
                            $(this).find('li').removeClass('selected');
                            var lengthDayCombo = $(this).find('ul li').eq(0).addClass('selected');
                            $(this).find('li').eq(0).text(lengthDayCombo.text()).attr('value', lengthDayCombo.attr('value'));
                        });
                        lengthDateCombinationClicked($(ctrlId + ' ul li').eq(0).attr('rentallengthid'), $(ctrlId + ' ul li').eq(0).attr('formattedDate'));
                    }
                }
                $('#lengthDateCombinationChanged .drop-down1').scrollTop(0);
                $('#lengthDateCombinationUnChanged .drop-down1').scrollTop(0);
            }
        },
        error: function (e) {
            $('.loader_container_carclass').hide();
            console.log(e);
        }
    });
}

function lengthDateCombinationClicked(rentalLengthId, formattedDate) {
    //Set length dropdown
    $('#length ul li, #displayDay ul li').removeClass('selected');
    var length = $('#length ul li[value="' + rentalLengthId + '"]').addClass('selected');
    $('#length li').eq(0).text(length.text()).attr('value', length.attr('value'));

    //Set date dropdown
    var day = $('#displayDay ul li[value="' + formattedDate + '"]').addClass('selected');
    $('#displayDay li').eq(0).text(day.text()).attr('value', day.attr('value'));

    //Fire click event of date dropdown
    $('#displayDay ul li.selected').click();
    //user to shrink all drop down control of mobile as well as web
    if (isQuickViewShop) {
        if ($('#rbChanged:checked').length > 2) {
            $('[Id$="lengthDateCombinationChanged"]').each(function () {
                $(this).find('li').removeClass('selected');
                var changedCombo = $(this).find('ul li[rentallengthid="' + rentalLengthId + '"][formatteddate="' + formattedDate + '"]').addClass('selected').eq(0);
                $(this).find('li').eq(0).text($(changedCombo).text()).attr('value', changedCombo.attr('value')).attr('rentallengthid', rentalLengthId).attr('formattedDate', formattedDate);
            });
        }
        else {
            $('[Id$="lengthDateCombinationUnChanged"]').each(function () {
                $(this).find('li').removeClass('selected');
                var unChangedCombo = $(this).find('ul li[rentallengthid="' + rentalLengthId + '"][formatteddate="' + formattedDate + '"]').addClass('selected').eq(0);
                $(this).find('li').eq(0).text($(unChangedCombo).text()).attr('value', unChangedCombo.attr('value')).attr('rentallengthid', rentalLengthId).attr('formattedDate', formattedDate);
            });

        }

    }

    //Wait for the tab to get clicked and then hide the items not required for quick view
    setTimeout(function () {
        $('.noQuickView').hide();
        if ($('#rbChanged:checked').length > 2) {
            showLengthDateCombinationChanged(true);
        } else {
            showLengthDateCombinationChanged(false);
        }
        ShowClassicQuickViewRentalLength();
    }, 250);
}

function changeUnChangeClicked(radioButtonClickedId) {

    if (radioButtonClickedId == 'rbChanged') {

        $('[Id$="rbChanged"]').each(function () {
            $(this).prop('checked', true);
        });

        showLengthDateCombinationChanged(true);
        // $('[id=rbChanged]').prop('checked', true);
        //try to maintain previous state if available
        if ($('[id$=lengthDateCombinationChanged] ul li.selected').length > 2) {
            $('[id$=lengthDateCombinationChanged] ul li.selected').click();
        }
        else {
            $('[id$=lengthDateCombinationChanged] ul li').eq(0).click();
        }

    }
    else {

        $('[Id$="rbUnChanged"]').each(function () {
            $(this).prop('checked', true);
        });

        showLengthDateCombinationChanged(false);

        //try to maintain previous state if available
        if ($('[id$=lengthDateCombinationUnChanged] ul li.selected').length > 2) {
            $('[id$=lengthDateCombinationUnChanged] ul li.selected').click();
        }
        else {
            $('[id$=lengthDateCombinationUnChanged] ul li').eq(0).click();

        }
    }
}

function showLengthDateCombinationChanged(showChanged) {
    if (showChanged) {
        $('[id=lengthDateCombinationChanged]').show();
        $('[id=lengthDateCombinationUnChanged]').hide();

    }
    else {
        $('[id=lengthDateCombinationChanged]').hide();
        $('[id=lengthDateCombinationUnChanged]').show();
    }
    //alert("call");
    enableNextPrevButton('.dailyViewNextDay', false);
    enableNextPrevButton('.dailyViewNextDay_m', false);
    enableNextPrevButton('.dailyViewNextDay_ml', false);
}

function showQuickViewControl(isShow) {
    HideLastDaySpan();
    if (isShow) {
        $('.quickView').css('display', 'inline-block');
        $('.noQuickView').css('display', 'none');
        isQuickViewShop = true;
    }
    else {
        $('.quickView').css('display', 'none');
        $('.noQuickView').css('display', 'inline-block');
        isQuickViewShop = false;
    }
}

function ShowNextQuickView(ctrl) {
    if ($(ctrl + ' ul:first li').length == $(ctrl + ' ul:first li.selected').index() + 1) {
        $(".spanlastdayshop").show();
    }
    else {
        $(".spanlastdayshop").hide();
    }
    if ($(ctrl).eq(0).find("ul li").length != $(ctrl).eq(0).find("ul li.selected").index() + 1) {
        //$(ctrl + ' ul li.selected').next().click();

        var ctrlArray = ctrl.replace('#', '');
        $('[id="' + ctrlArray + '"]').each(function () {
            $(this).find('ul li.selected').removeClass('selected').next().addClass('selected').closest('.drop-down1');
            $(this).find("li").eq(0).text($(this).find('ul li.selected').text()).attr('rentallengthid', ($(this).find('ul li.selected').attr('rentallengthid')))
                    .attr('formatteddate', ($(this).find('ul li.selected').attr('formatteddate')));
        });

        QuickViewLengthDateCombination($(ctrl).eq(0).find("ul li.selected").attr('rentallengthid'), $(ctrl).eq(0).find("ul li.selected").attr('formatteddate'));

    } else
        return false;
}

function ShowPrevQuickView(ctrl) {
    if ($(ctrl + ' ul li.selected').index() > 0) {
        $(ctrl + ' ul li.selected').prev().click();
    }
    else
        return false;
}

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

function LoadScrapperSource(selectedAPI) {
    var loggedInUserId = $('#LoggedInUserId').val();

    $.ajax({
        url: 'Search/GetScrapperSourcess',
        type: "GET",
        dataType: "json",
        contentType: "application/json; charset=utf-8;",
        //data: { 'userId': loggedInUserId, 'providerId': selectedAPI },
        data: { 'userId': loggedInUserId },
        success: function (data) {
            $('.loader_container_source').hide();
            $("select#source option").remove();
            if (data.length > 0) {
                for (i = 0; i < data.length; i++) {
                    $("select#source").append("<option value=" + data[i].ID + " srccode=" + data[i].Code + ">" + data[i].Name + "</option>");
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

function ShowClassicQuickViewRentalLength() {
    //In Classic View Rental Length drop down is visible mode
    if (isQuickViewShop && $("#viewSelect").find(".selected").text() == "classic") {
        $(".noQuickView #length").parent().css('display', 'inline-block');
    }
}

function HideLastDaySpan() {
    $(".spanlastdayshop").hide();
}

var DisableTSDUpdateAccess = function () {
    if ($("#hdnIsTSDUpdateAccess").length > 0 && $("#hdnIsTSDUpdateAccess").val() == "False") {
        $("#UpdateAllTSD, #update, #updaten").prop('disabled', true).addClass("disable-button");
        $("#daily-rates-table td input.baseEdit:enabled, #daily-rates-table td input.totalEdit:enabled").prop('disabled', true);
        $("#classic input:enabled").prop('disabled', true);
        $(".extraDayRateFactor").prop('disabled', true);
    }

}

function QuickViewLengthDateCombination(rentalLengthId, formattedDate) {
    //Set length dropdown
    $('#length ul li, #displayDay ul li').removeClass('selected');
    var length = $('#length ul li[value="' + rentalLengthId + '"]').addClass('selected');
    $('#length li').eq(0).text(length.text()).attr('value', length.attr('value'));

    //Set date dropdown
    $('#displayDay ul li').removeClass('selected');
    var day = $('#displayDay ul li[value="' + formattedDate + '"]').addClass('selected');
    $('#displayDay li').eq(0).text(day.text()).attr('value', day.attr('value'));

    //Fire click event of date dropdown
    //$('#displayDay ul li.selected').click();
    //$('#displayDay li').eq(0).text($('#displayDay ul li').removeClass('selected').eq(0).text()).attr('value', $('#displayDay ul li').eq(0).addClass('selected').attr('value'));
    //user to shrink all drop down control of mobile as well as web
    if (isQuickViewShop) {
        if ($('#rbChanged:checked').length > 2) {
            $('[Id$="lengthDateCombinationChanged"]').each(function () {
                $(this).find('li').removeClass('selected');
                var changedCombo = $(this).find('ul li[rentallengthid="' + rentalLengthId + '"][formatteddate="' + formattedDate + '"]').addClass('selected').eq(0);
                $(this).find('li').eq(0).text($(changedCombo).text()).attr('value', changedCombo.attr('value')).attr('rentallengthid', rentalLengthId).attr('formattedDate', formattedDate);
            });
        }
        else {
            $('[Id$="lengthDateCombinationUnChanged"]').each(function () {
                $(this).find('li').removeClass('selected');
                var unChangedCombo = $(this).find('ul li[rentallengthid="' + rentalLengthId + '"][formatteddate="' + formattedDate + '"]').addClass('selected').eq(0);
                $(this).find('li').eq(0).text($(unChangedCombo).text()).attr('value', unChangedCombo.attr('value')).attr('rentallengthid', rentalLengthId).attr('formattedDate', formattedDate);
            });

        }
        $('.showSelectedDate').html($('#displayDay li').eq(0).html());
    }

    //Wait for the tab to get clicked and then hide the items not required for quick view
    setTimeout(function () {
        $('.noQuickView').hide();
        if ($('#rbChanged:checked').length > 2) {
            showLengthDateCombinationChanged(true);
        } else {
            showLengthDateCombinationChanged(false);
        }
        ShowClassicQuickViewRentalLength();
        assignCtrlValues('', '_m');
        assignCtrlValues('', '_ml');
    }, 250);
}

