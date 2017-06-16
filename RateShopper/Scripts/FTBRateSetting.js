/*FTB Rate Settings*/

$(document).ready(FTBRateSettingsDocumentReady);
//Show loader before every ajax call
$(document).ajaxStart(function () { $(".loader_container_main").show(); });
//Hide loader before every ajax call
$(document).ajaxComplete(function () { $(".loader_container_main").hide(); });

var locationBrandID = 0;
var year = 0;
var month = 0;

function FTBRateSettingsDocumentReady() {
    BindControlEvents();
}

var FTBRateDetailsDTO = function (FTBData) {
    var self = this;
    self.DicFTBRateDetails = ko.observableArray([]);
    self.DicFTBRateDetails(MapDictionaryToArray(FTBData.DicFTBRateDetails));
    self.SplitLabels = ko.observableArray([]);
    self.SplitLabels(FTBData.SplitLabels);
    self.LstCopyMonths = ko.observableArray([]);
    self.LstCopyMonths(FTBData.LstCopyMonths);
}

//#REGION "Bind Events"
var BindControlEvents = function () {
    $('#ratesettingslocation ul li').click(function () {
        locationBrandID = $(this).attr('value');
        if (typeof (locationBrandID) != 'undefined') {
            GetFTBRates(locationBrandID);
        }
    });

    $('#year ul li').click(function () {
        year = $(this).attr('value');
        if (typeof (locationBrandID) != 'undefined' && typeof (year) != 'undefined') {
            month = $("#months ul li.selected").attr("value");
            GetFTBRates(locationBrandID, year, month);
            CommonTargetControlSync(month, year);

        }
    });

    $('#months ul li').click(function () {
        month = $(this).attr('value');
        if (typeof (locationBrandID) != 'undefined' && typeof (month) != 'undefined') {
            year = $("#year ul li.selected").attr("value");
            GetFTBRates(locationBrandID, year, month);
            CommonTargetControlSync(month, year);
        }
    });



    $("#chkSplit").change(function () {
        if ($(this).prop('checked')) {
            $(".splittedsection").show();
            if ($(".temp:visible").length > 0) {
                $("#lblMessage").show();
            }
        }
        else {
            $(this).prop('checked', true);
            var message = "Application will clear rates of the selected month for all rental lengths. Are you sure?";
            ShowConfirmBox(message, true, HideSplittedSection);
        }
    });

    $("#chkBlackout").change(function () {
        if ($(this).prop('checked')) {
            $("#blackoutStartDate").prop('disabled', false).val('');
            $("#blackoutEndDate").prop('disabled', false).val('');
        }
        else {
            $("#blackoutStartDate").prop('disabled', true).val('');
            $("#blackoutEndDate").prop('disabled', true).val('');
        }
    });

    $("#btnSaveFTBRates").click(SaveFTBRates);
    $("#btnCancelFTBRates").click(ResetFTBRates);
    DatePickerEvent();
    BindLORSelectable();
}

var DatePickerEvent = function () {
    var selectedDate = new Date(parseInt($("#year ul li.selected").attr("value")), parseInt($("#months ul li.selected").attr("value")) - 1, 1);

    $('#blackoutStartDate.date-picker').datepicker({
        minDate: selectedDate,

        //stepMonths: false,
        maxDate: new Date(parseInt($("#year ul li.selected").attr("value")), parseInt($("#months ul li.selected").attr("value")), 0),
        dateFormat: 'mm/dd/yy',
        onSelect: function (selectedDate, instance) {
            $(this).val(selectedDate);
            $("#blackoutEndDate").datepicker('option', { defaultDate: selectedDate, minDate: selectedDate });
            RemoveFlashableTag($(this));
            if ($(".temp:visible") == 0) {
                $("#lblMessage").hide();
            }
        }
    });

    $("#blackoutEndDate.date-picker").datepicker({
        minDate: selectedDate,

        //stepMonths: false,
        maxDate: new Date(parseInt($("#year ul li.selected").attr("value")), parseInt($("#months ul li.selected").attr("value")), 0),
        dateFormat: 'mm/dd/yy',
        onSelect: function (selectedDate, instance) {
            $(this).val(selectedDate);
            RemoveFlashableTag($(this));
            if ($(".temp:visible") == 0) {
                $("#lblMessage").hide();
            }
        }
    });
}

var GridInputEvents = function () {
    $(".carclasscheckbox").on('change', function () {
        if ($(this).prop('checked')) {
            $(this).closest('tr').attr("data-edit", '1').find("input[type='text']").each(function () { $(this).prop("disabled", false) });
        }
        else {
            $(this).closest('tr').attr("data-edit", '1').find("input[type='text']").each(function () { $(this).prop("disabled", true).val(''); RemoveFlashableTag('#' + this.id); });
        }
    });

    $(".ftb-rating-table tr input[type='text']").on({
        input: function () {
            $($(this).closest("tr")).attr("data-edit", '1');
            if ($.trim(this.value) != '' && !ValidateTextBoxValue(this.value)) {
                MakeTagFlashable('#' + this.id);
            }
            else {
                if (this.value.indexOf('.') != -1) {
                    if (this.value.split(".")[1].length > 2) {
                        if (isNaN(parseFloat(this.value))) return;
                        this.value = parseFloat(this.value).toFixed(2);
                    }
                }

                RemoveFlashableTag('#' + this.id);
            }
            AddFlashingEffect();

        },
        change: function () {
            $($(this).closest("tr")).attr("data-edit", '1');
            if ($.trim(this.value) != '' && !ValidateTextBoxValue(this.value)) {
                MakeTagFlashable('#' + this.id);
            }
            else {
                if (this.value.indexOf('.') != -1) {
                    if (this.value.split(".")[1].length > 2) {
                        if (isNaN(parseFloat(this.value))) return;
                        this.value = parseFloat(this.value).toFixed(2);
                    }
                }
                RemoveFlashableTag('#' + this.id);
            }
            AddFlashingEffect();
        }
    });

    $(".copyfrom").on("click", function () {
        var $itemClicked = $(this);
        if ($itemClicked.children('ul').css('display') == "none") {
            $('.hidden').hide();
        }

        $itemClicked.children('ul').toggle().find('li').bind('click', function () {
            $(this).closest('.table-ul-right ul').find('li').removeClass('selected');
            $(this).addClass('selected');
            $itemClicked.find('li').eq(0).text($(this).text()).attr('value', ($(this).attr('value')));
            setTimeout(function () { $('.hidden').hide(); }, 200);
        });
    });

    $(".copyfrom ul li").on('click', function () {
        if ($(this).attr('issplit') == 'false') {
            $(this).closest('div').siblings('div.splitoptions').find('li').each(function () { $(this).hide(); });
            $(this).closest('div.divCopymonth').css({ 'width': 'auto' });
        }
        else {
            var $splitoptions = $($(this).closest(".split-month-box").find(".splitoptions"));
            $splitoptions.find("li").remove();
            var index = $(this).attr('copyforindex');
            var label = $(this).attr("labels");
            var labels = label.split(",");
            for (i = labels.length - 1; i >= 0; i--) {
                var option;
                if (i == 0) {
                    option = "<li><input type='radio' value=" + (i + 1) + " checked='checked' name='splitoptionof_" + index + "'><label>" + labels[i] + "</label></li>";
                }
                else {
                    option = "<li><input type='radio' value=" + (i + 1) + "  name='splitoptionof_" + index + "'><label>" + labels[i] + "</label></li>";
                }
                $splitoptions.prepend(option);
            }

            $(this).closest('div').siblings('div.splitoptions').find('li').each(function () { $(this).show(); });
            $(this).closest('div.divCopymonth').css({ 'width': '360px' });
        }
    });

    $(".copysave").on('click', function () {
        var $copySaveButton = $(this);
        var $selectedMonth = $copySaveButton.closest('.split-month-box').find(".copyfrom ul li.selected");
        if (typeof ($selectedMonth) != 'undefined' && $selectedMonth.length > 0) {
            var isSplittedMonth = $selectedMonth.attr("issplit");
            var copyFromIndex = 1;
            var copyToIndex = $selectedMonth.attr("copyforindex");
            var copyFromMonth = $selectedMonth.attr("month");
            var copyFromYear = $selectedMonth.attr("year");

            if (isSplittedMonth == 'true') {
                var $selectedRadioButton = $copySaveButton.closest("div.splitoptions").find("input[type='radio']:checked");
                if ($selectedRadioButton.length > 0) {
                    copyFromIndex = $selectedRadioButton.attr("value");
                }
            }
            CopyAndSave(copyFromMonth, copyFromYear, copyFromIndex, copyToIndex);

        }
    });

    $('#splitDatePicker_0.date-picker').datepicker({
        minDate: new Date(parseInt($("#year ul li.selected").attr("value")), parseInt($("#months ul li.selected").attr("value")) - 1, 1),

        stepMonths: false,
        maxDate: new Date(parseInt($("#year ul li.selected").attr("value")), parseInt($("#months ul li.selected").attr("value")), -1),
        dateFormat: 'mm/dd/yy',
        onSelect: function (selectedDate, instance) {
            $(this).val(selectedDate);
            var nextDay = parseInt(instance.selectedDay) + 1;
            var month = ('0' + $("#months ul li.selected").attr("value")).slice(-2);
            var year = $("#year ul li.selected").attr("value");
            var endDay = new Date(year, parseInt($("#months ul li.selected").attr("value")), 0).getDate();
            var startstring = month + '/' + ('0' + String(nextDay)).slice(-2) + '/' + year;
            var endstring = month + '/' + ('0' + String(endDay)).slice(-2) + '/' + year;
            $(".splitted_date_range_1").html('Date Range ' + startstring + ' through ' + endstring);
        }
    });
}
//#ENDREGION "Bind Events"

//#REGION FUNCTIONS
var MapDictionaryToArray = function (dictionary) {
    var result = [];
    for (var key in dictionary) {
        if (dictionary.hasOwnProperty(key)) {
            result.push({ key: key, value: dictionary[key] });
        }
    }
    return result;
}

var BindFTBRates = function (FTBData) {
    targetViewModel.FTBRateDTO(new FTBRateDetailsDTO(FTBData));

    $("#months ul li.selected").removeClass("selected")
    $("#year ul li.selected").removeClass("selected")
    $("#ratesettingsLOR ul li.selected").removeClass("selected")

    $("#months ul.drop-down1 li[value='" + FTBData.Month + "']").eq(0).addClass('selected').closest('.dropdown').find('li').eq(0).attr({ 'value': ($("#months ul.drop-down1 li[value='" + FTBData.Month + "']").eq(0).attr('value')) }).text($("#months ul.drop-down1 li[value='" + FTBData.Month + "']").eq(0).text());
    $("#year ul.drop-down1 li[value='" + FTBData.Year + "']").eq(0).addClass('selected').closest('.dropdown').find('li').eq(0).attr({ 'value': ($("#year ul.drop-down1 li[value='" + FTBData.Year + "']").eq(0).attr('value')) }).text($("#year ul.drop-down1 li[value='" + FTBData.Year + "']").eq(0).text());
    $("#ratesettingsLOR ul.drop-down1 li[value='" + FTBData.LowestRentalLengthId + "']").eq(0).addClass('selected').closest('.dropdown').find('li').eq(0).attr({ 'value': ($("#ratesettingsLOR ul.drop-down1 li[value='" + FTBData.LowestRentalLengthId + "']").eq(0).attr('value')) }).text($("#ratesettingsLOR ul.drop-down1 li[value='" + FTBData.LowestRentalLengthId + "']").eq(0).text());
    month = FTBData.Month;
    year = FTBData.Year;
    locationBrandID = FTBData.LocationBrandId;
    $("#chkSplit").prop('checked', FTBData.IsSplitMonth);

    $("#chkBlackout").prop('checked', FTBData.HasBlackOutPeroid);
    $("#oldBlackoutCheckbox").val(FTBData.HasBlackOutPeroid);
    if (FTBData.HasBlackOutPeroid) {
        var blackoutStartDate = new Date(parseInt(FTBData.BlackOutStartDate.replace("/Date(", "").replace(")/", "")));
        var blackoutEndDate = new Date(parseInt(FTBData.BlackOutEndDate.replace("/Date(", "").replace(")/", "")));

        var formatBlackoutStartDate = ('0' + (blackoutStartDate.getMonth() + 1)).slice(-2) + "/" + ('0' + (blackoutStartDate.getDate())).slice(-2) + "/" + blackoutStartDate.getFullYear();
        var formatBlacoutEndDate = ('0' + (blackoutEndDate.getMonth() + 1)).slice(-2) + "/" + ('0' + (blackoutEndDate.getDate())).slice(-2) + "/" + blackoutEndDate.getFullYear();
        $("#blackoutStartDate").prop('disabled', false).val(formatBlackoutStartDate);
        $("#blackoutEndDate").prop('disabled', false).val(formatBlacoutEndDate);
        $("#oldBlackoutStartDate").val(formatBlackoutStartDate);
        $("#oldBlackoutEndDate").val(formatBlacoutEndDate);
    }
    else {
        $("#blackoutStartDate").prop('disabled', true).val('');
        $("#blackoutEndDate").prop('disabled', true).val('');
        $("#oldBlackoutStartDate").val('');
        $("#oldBlackoutEndDate").val('');
    }

    BindSplitLabels(FTBData.SplitLabels);
    HideShowSelectableLOR();
    HighlightSelectableLOR(FTBData.ID, FTBData.SelectedLORs);
    CheckLORIsInCreationMode(FTBData.LowestRentalLengthId, FTBData.SelectedLORs);
    GridInputEvents();

    var selectedDate = new Date(parseInt($("#year ul li.selected").attr("value")), parseInt($("#months ul li.selected").attr("value")) - 1, 1);
    var maxDate = new Date(parseInt($("#year ul li.selected").attr("value")), parseInt($("#months ul li.selected").attr("value")), 0);
    $('#blackoutStartDate.date-picker').datepicker('option', { minDate: selectedDate, maxDate: maxDate });
    $("#blackoutEndDate.date-picker").datepicker('option', { minDate: selectedDate, maxDate: maxDate });

    //Target management events
    OpenTargetPopup();
    CommonTargetControlSync(month, year);
}

var BindLORSelectable = function () {
    $("#ftbLORSelect").selectable({
        filter: "li",
        selected: function (event, ui) {
            var $selected = $(ui.selected);
            if ($selected.hasClass('selected')) {

                if ($('#ratesettingsLOR ul li.selected').attr('value') != $selected.attr("value")) {
                    $selected.removeClass('selected');
                }
            } else {
                $selected.addClass("selected");
            }
            $(ui.selected).addClass('pSelected');
            $("#ftbLORSelect li[value='" + $('#ratesettingsLOR ul li.selected').attr('value') + "']").addClass('pSelected');
        },
        unselected: function (event, ui) {
            if (event.ctrlKey) {
                $(ui.unselected).addClass('pSelected');

                var $selected = $(ui.unselected);
                if ($selected.hasClass('selected')) {
                    if ($('#ratesettingsLOR ul li.selected').attr('value') != $selected.attr("value")) {
                        $selected.removeClass('selected');
                    }
                } else {
                    $selected.addClass("selected");
                }
            }
        },
        stop: function (event, ui) {
            //If user not clicked ctrl also if iPad then ignore un-selection when another LOR selected
            var $selected = $(ui.selected);
            if (!event.ctrlKey && navigator.userAgent.match(/iPad|iPhone|Android|Windows Phone/i) == null) {
                $("#ftbLORSelect li").not('.pSelected').removeClass('selected');
                $("#ftbLORSelect li.pSelected").addClass('selected');
            }
            $("#ftbLORSelect li.pSelected").removeClass('pSelected');
        },
    });
}

var HideShowSelectableLOR = function () {
    var lorCode = String($("#ratesettingsLOR ul li.selected").eq(0).text());
    //get the first letter of the code
    var firstLetterOfLOR = lorCode.substring(0, 1);
    var mappedId = $("#ratesettingsLOR ul li.selected").attr("value");
    $("#ftbLORSelect li").each(function () {
        var $this = $(this);
        $this.removeClass("selected");
        if ($this.text().toUpperCase().indexOf(firstLetterOfLOR) >= 0) {
            $this.show();
        }
        else {
            $this.hide();
        }
    });

    //$("#ftbLORSelect li[value='" + lorCode.substring(1, lorCode.length) + "']").addClass("selected");
    $("#ftbLORSelect li[value='" + mappedId + "']").addClass("selected");
}

var BindSplitLabels = function (lstSplitLabels) {
    for (i = 0; i < lstSplitLabels.length; i++) {
        var startday = lstSplitLabels[i].StartDay;
        var endDay = lstSplitLabels[i].EndDay;

        var month = ('0' + $("#months ul li.selected").attr("value")).slice(-2);
        var year = $("#year ul li.selected").attr("value");
        //if it is last option then find last day of the selected month
        if (i + 1 == lstSplitLabels.length) {
            endDay = new Date(year, parseInt($("#months ul li.selected").attr("value")), 0).getDate();
        }
        var startstring = month + '/' + ('0' + String(startday)).slice(-2) + '/' + year;
        var endstring = month + '/' + ('0' + String(endDay)).slice(-2) + '/' + year;
        if (i == lstSplitLabels.length - 1) {
            $(".splitted_date_range_" + i).html('Date Range ' + startstring + ' through ' + endstring);
        }
        else {
            $(".splitted_date_range_" + i).html('Date Range ' + startstring + ' through ');
            $("#splitDatePicker_" + i).val(endstring);
        }
    }
}

var HighlightSelectableLOR = function (ftbRateid, LORsForWhichDataExist) {
    if (ftbRateid > 0) {
        var lorArray = LORsForWhichDataExist.split(",");
        $("#ftbLORSelect li:visible").each(function () {
            if ($.inArray($(this).attr('value'), lorArray) != -1) {
                $(this).removeClass("red").addClass("green");
            }
            else {
                $(this).removeClass("green").addClass("red");
            }
        });
    }
    else {
        $("#ftbLORSelect li.green").removeClass("green");
        $("#ftbLORSelect li.red").removeClass("red");
    }
}

var CheckLORIsInCreationMode = function (rentalLength, dataExistsForLORs) {
    if (dataExistsForLORs == null || $.inArray(String(rentalLength), dataExistsForLORs.split(",")) == -1) {
        $(".ftb-rating-table input:text").prop("disabled", false);
        $(".ftb-rating-table input:checkbox").prop("checked", true);
        $("tr[data-edit='0']").attr("data-edit", "1");
    }
}

var ValidateTextBoxValue = function (value) {
    return $.isNumeric(value) && parseFloat(value) > 0
}

var GetEditedData = function () {
    //Validate the empty textboxes
    var emptyTextboxes = $(".carclasscheckbox:checked").closest("tr[data-edit='1']").find("input[type='text']").filter(function () { return $.trim(this.value) == "" });
    if (emptyTextboxes.length > 0) {
        emptyTextboxes.each(function () {
            if ($(this).is(":visible")) {
                MakeTagFlashable('#' + this.id);
            }
        });
    }
    if ($("#chkBlackout").prop("checked")) {
        if ($("#blackoutStartDate").val() == "" && $("#blackoutEndDate").val() == "") {
            MakeTagFlashable('#blackoutStartDate');
            MakeTagFlashable('#blackoutEndDate');
        }
        else {
            if ($("#blackoutStartDate").val() == "") {
                MakeTagFlashable('#blackoutStartDate');
            }
            if ($("#blackoutEndDate").val() == "") {
                MakeTagFlashable('#blackoutEndDate');
            }
        }
        AddFlashingEffect();
    }

    if ($(".temp:visible").length > 0) {
        DisplayMessage("Please review the fields highlighted in Red.", true);
        return false;
    }

    var objFTBRate = new Object();

    objFTBRate.ID = String($(".ftb-rating-table").eq(0).attr("id")).replace("ftb-rating-table_", "");
    objFTBRate.IsSplitMonth = $("#chkSplit").prop("checked");
    objFTBRate.HasBlackOutPeroid = $("#chkBlackout").prop("checked");
    objFTBRate.BlackOutStartDate = $("#blackoutStartDate").val();
    objFTBRate.BlackOutEndDate = $("#blackoutEndDate").val();
    objFTBRate.OldBlackOutStartDate = $("#oldBlackoutStartDate").val();
    objFTBRate.OldBlackOutEndDate = $("#oldBlackoutEndDate").val();
    objFTBRate.OldHasBlackOutPeroid = $("#oldBlackoutCheckbox").val();
    objFTBRate.Month = $("#months ul li.selected").attr("value");
    objFTBRate.Year = $("#year ul li.selected").attr("value");
    objFTBRate.LocationBrandId = $('#ratesettingslocation ul li.selected').attr("value");
    //pass this rental length as selected rental length
    objFTBRate.LowestRentalLengthId = $('#ratesettingsLOR ul li.selected').attr("value");
    objFTBRate.SplitLabels = GetSplitDetails(objFTBRate.IsSplitMonth, objFTBRate.Month, objFTBRate.Year);
    objFTBRate.SelectedLORs = "";
    $('#ftbLORSelect li.selected').each(function (index) {
        if (index == $('#ftbLORSelect li.selected').length - 1) {
            objFTBRate.SelectedLORs += $(this).attr('value');
        }
        else {
            objFTBRate.SelectedLORs += $(this).attr('value').trim() + ',';
        }
    });

    var $ftbRatesEditRows = $("tr[data-edit='1']");
    var selectedRentalLengthId = $("#ratesettingsLOR ul li.selected").eq(0).attr('value');

    var dict = new Array();
    //create dictionary for split list
    $(".ftb-rating-table").each(function (index) {
        dict.push({
            key: String(index + 1),
            value: new Array(),
        });
    });

    $ftbRatesEditRows.each(function () {
        if ($(this).find('.carclasscheckbox').eq(0).prop('checked')) {
            var objFTBRateDetail = new Object();
            objFTBRateDetail.SplitIndex = $(this).attr("data_splitIndex");
            //check split checkbox is checked then only add splitted section rows
            if (parseInt(objFTBRateDetail.SplitIndex) == 1 || (parseInt(objFTBRateDetail.SplitIndex) > 1 && objFTBRate.IsSplitMonth)) {
                objFTBRateDetail.FTBRateDetailsId = this.id;
                objFTBRateDetail.CarClassId = $(this).attr("data_class");

                objFTBRateDetail.Sunday = $.trim($(this).find("input[name='Sunday']")[0].value);
                objFTBRateDetail.Monday = $.trim($(this).find("input[name='Monday']")[0].value);
                objFTBRateDetail.Tuesday = $.trim($(this).find("input[name='Tuesday']")[0].value);
                objFTBRateDetail.Wednesday = $.trim($(this).find("input[name='Wednesday']")[0].value);
                objFTBRateDetail.Thursday = $.trim($(this).find("input[name='Thursday']")[0].value);
                objFTBRateDetail.Friday = $.trim($(this).find("input[name='Friday']")[0].value);
                objFTBRateDetail.Saturday = $.trim($(this).find("input[name='Saturday']")[0].value);

                objFTBRateDetail.FTBRatesId = objFTBRate.ID;
                objFTBRateDetail.RentalLengthId = selectedRentalLengthId;


                dict[parseInt(objFTBRateDetail.SplitIndex) - 1].value.push(objFTBRateDetail);
            }
        }
        else {
            if (this.id > 0) {
                var objFTBRateDetail = new Object();
                objFTBRateDetail.FTBRateDetailsId = this.id;
                objFTBRateDetail.FTBRatesId = objFTBRate.ID;
                objFTBRateDetail.SplitIndex = $(this).attr("data_splitIndex");

                if (parseInt(objFTBRateDetail.SplitIndex) == 1 || (parseInt(objFTBRateDetail.SplitIndex) > 1 && objFTBRate.IsSplitMonth)) {
                    dict[parseInt(objFTBRateDetail.SplitIndex) - 1].value.push(objFTBRateDetail);
                }
            }
        }
    });


    //Remove object if no record is there of specific index
    if ($ftbRatesEditRows.length > 0) {
        for (i = dict.length - 1; i >= 0; i--) {
            if (dict[i].value.length == 0) {
                dict.splice(i, 1);
            }
        }
    }

    objFTBRate.DicFTBRateDetails = $ftbRatesEditRows.length > 0 ? dict : null;
    objFTBRate.CreatedBy = $("#LoggedInUserId").val();
    objFTBRate.UpdatedBy = $("#LoggedInUserId").val();
    return objFTBRate;
}

var ValidateFTBObject = function (objFTBRate) {
    if (!objFTBRate) {
        return false;
    }

    if ($(".temp:visible").length > 0) {
        DisplayMessage("Please review the fields highlighted in Red.", true);
        return false;
    }
    return true;
}

var GetSplitDetails = function (isSplitMonth, month, year) {
    var splitDetails = [];
    var monthEndDay = new Date(year, month, 0).getDate();
    if (!isSplitMonth) {
        var splitDetail = new Object();
        splitDetail.StartDay = 1;
        splitDetail.EndDay = monthEndDay;
        splitDetail.SplitIndex = 1;
        splitDetail.Label = splitDetail.StartDay + " - Last day of month";
        splitDetails.push(splitDetail);
    }
    else {
        var splitSectionCount = $(".ftb-rating-table").length;
        $(".ftb-rating-table").each(function () {
            var splitDetail = new Object();

            //its a zero base index
            var index = parseInt($(this).attr("index"));
            splitDetail.SplitIndex = index + 1;
            if (index == 0) {
                splitDetail.StartDay = 1;
                var selectedDate = $("#splitDatePicker_" + index).val();
                if (selectedDate != '') {
                    //get the date part of selected day
                    splitDetail.EndDay = parseInt(selectedDate.split("/")[1]);
                }
                splitDetail.Label = splitDetail.StartDay + " - " + splitDetail.EndDay + " of month";

            }
            else {
                var selectedDate = $("#splitDatePicker_" + (index - 1)).val();
                if (selectedDate != '') {
                    splitDetail.StartDay = parseInt(selectedDate.split("/")[1]) + 1;
                }
                if (splitSectionCount == index + 1) {
                    splitDetail.EndDay = monthEndDay;
                    splitDetail.Label = splitDetail.StartDay + " - Last day of month";
                }
                else {
                    var selectedDate = $("#splitDatePicker_" + index).val();
                    if (selectedDate != '') {
                        splitDetail.EndDay = parseInt(selectedDate.split("/")[1]);
                    }
                    splitDetail.Label = splitDetail.StartDay + " - " + splitDetail.EndDay + " of month";
                }
            }
            splitDetails.push(splitDetail);
        });

    }
    return splitDetails;
}

var HideSplittedSection = function () {
    $("#chkSplit").prop('checked', false);
    $(".splittedsection").hide();
    $(".ftb-rating-table input:text").not(".date-picker").val('');
    $(".temp").each(function () {
        RemoveFlashableTag('#' + this.id);
    });
    if ($(".temp:visible").length == 0) {
        $("#lblMessage").hide();
    }
}

//Show the success/error messages
var DisplayMessage = function (message, isError) {
    $("#lblMessage").html(message).show().css("color", isError ? "red" : "green");
    if (!isError) {
        setTimeout(function () { $("#lblMessage").hide(); }, 3000);
    }
}

var ResetFTBRates = function () {
    var ftbId = parseInt(String($(".ftb-rating-table").eq(0).attr("id")).replace("ftb-rating-table_", ""));
    if (ftbId > 0) {
        var month = $("#months ul li.selected").attr("value");
        var year = $("#year ul li.selected").attr("value");
        var brandLocationId = $('#ratesettingslocation ul li.selected').attr("value");
        var rentalLengthId = $('#ratesettingsLOR ul li.selected').attr("value");
        GetFTBRates(brandLocationId, year, month, rentalLengthId, 0);
    }
    else {
        $(".ftb-rating-table input:text").prop('disabled', false).val('');
        $(".ftb-rating-table input:checkbox").prop("checked", true);
        $("tr[data-edit='0']").attr("data-edit", "1");
        $(".ftb-rating-table .temp").each(function () {
            RemoveFlashableTag('#' + this.id);
        });
        $("#lblMessage").hide();
        $("#chkBlackout").prop("checked", false);
        $("#blackoutStartDate").prop('disabled', true).val('');
        $("#blackoutEndDate").prop('disabled', true).val('');
        RemoveFlashableTag('#blackoutStartDate');
        RemoveFlashableTag('#blackoutEndDate');
        $("#ftbLORSelect li.selected").removeClass("selected");
        $("#ftbLORSelect li[value='" + $('#ratesettingsLOR ul li.selected').attr("value") + "']").addClass("selected");
    }
}

var CommonTargetControlSync = function (monthValue, yearValue) {
    if (monthValue != '0' && yearValue != '0') {
        SrinkUpdateStatus = true;
        CheckDataExist = false
        GetTargetData(monthValue, yearValue, getCopyFromFlag(monthValue, yearValue));
    }
}

var CheckPastDate = function () {
    var serverDate = new Date($("#hdnServerDate").val());
    serverDate = new Date(String(serverDate.getMonth() + 1) + "-01-" + String(serverDate.getFullYear()));
    var selectedMonth = $("#months ul li.selected").attr("value");
    var selectedYear = $("#year ul li.selected").attr("value");
    var selectedDate = new Date(selectedMonth + "-01-" + selectedYear);
    if (selectedDate < serverDate) {
        $("#lblNotAllowedToModify").show();
        $("input.validatepastdate").addClass("disable-button").prop("disabled", true);
    }
    else {
        $("#lblNotAllowedToModify").hide();
        $("input.validatepastdate").removeClass("disable-button").prop("disabled", false);
    }
}
//#ENDREGION FUNCTIONS

//#Region " AJAX CALLS"
var GetFTBRates = function (brandLocationId, year, month, rentalLengthId, selectedFTBRateId) {
    var ajaxURl = 'FTBRates/GetFTBRate/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetFTBRate;
    }

    $("#lblMessage").hide();
    $(".temp").each(function () {
        RemoveFlashableTag('#' + this.id);
    });
    //assign default values
    rentalLengthId = rentalLengthId || 0;
    year = year || 0;
    month = month || 0;
    selectedFTBRateId = selectedFTBRateId || 0;

    $.ajax({
        url: ajaxURl,
        type: 'GET',
        contentType: "application/json;charset=utf-8",
        data: { 'brandLocationID': brandLocationId, 'rentalLengthId': rentalLengthId, 'year': year, 'month': month, 'selectedFTBRateId': selectedFTBRateId },
        dataType: 'json',
        success: function (data) {
            $(".FTB_setting_details").show();
            BindLOR(data.rentalLengths);
            BindFTBRates(data);
            CheckPastDate();
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(errorThrown);
        }
    });
}

var SaveFTBRates = function () {
    var ajaxURl = 'FTBRates/SaveFTBRates/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.SaveFTBRates;
    }

    if ($(".carclasscheckbox:checked:visible").length == 0) {
        DisplayMessage("Please configure initial average rates", true);
        return false;
    }
    else {
        $("#lblMessage").hide();
    }

    var objFTBRate = GetEditedData();

    if (ValidateFTBObject(objFTBRate)) {
        if (objFTBRate != null) {
            $.ajax({
                url: ajaxURl,
                type: 'POST',
                contentType: "application/json;charset=utf-8",
                data: JSON.stringify({ 'objFTBRatesDTO': objFTBRate }),
                dataType: 'json',
                success: function (data) {
                    if (data.Message.toUpperCase() == 'SUCCESS' || data.Message.indexOf("SuccessAutomationJobDisabled") == 0 || data.Message.indexOf("Successnotconfiguredblackoutdates") == 0) {
                        if (data.Message.indexOf("SuccessAutomationJobDisabled") == 0 || data.Message.indexOf("Successnotconfiguredblackoutdates") == 0)// "AutomationJobDisabled-6/15/2016-6/18/2016"
                        {
                            DisplayMessage("All Automation jobs falling outside black out period are disabled.", false);
                        }
                        else {
                            DisplayMessage("Rates saved successfully", false);
                        }
                        BindFTBRates(data.ftbRatesDTO);
                    }
                },
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    console.log(errorThrown);
                }
            });
        }
    }
}

var CopyAndSave = function (copyFromMonth, copyFromYear, copyFromSplitIndex, copyToSplitIndex) {
    var ajaxURl = 'FTBRates/CopyFTBRates/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.CopyFTBRates;
    }

    if (copyFromMonth > 0 && copyFromYear > 0 && copyFromSplitIndex > 0 && copyToSplitIndex > 0) {

        var objFTBCopyMonths = new Object();
        objFTBCopyMonths.IsSplitMonth = $("#chkSplit").prop("checked");
        objFTBCopyMonths.HasBlackOutPeroid = $("#chkBlackout").prop("checked");
        objFTBCopyMonths.BlackOutStartDate = $("#blackoutStartDate").val();
        objFTBCopyMonths.BlackOutEndDate = $("#blackoutEndDate").val();
        objFTBCopyMonths.Month = $("#months ul li.selected").attr("value");
        objFTBCopyMonths.Year = $("#year ul li.selected").attr("value");
        objFTBCopyMonths.LocationBrandId = $('#ratesettingslocation ul li.selected').attr("value");
        objFTBCopyMonths.CreatedBy = $("#LoggedInUserId").val();
        objFTBCopyMonths.SourceYear = copyFromYear;
        objFTBCopyMonths.SourceMonth = copyFromMonth;
        objFTBCopyMonths.SourceSplitIndex = copyFromSplitIndex;
        objFTBCopyMonths.DestinationSplitIndex = copyToSplitIndex;
        objFTBCopyMonths.SplitLabels = GetSplitDetails(objFTBCopyMonths.IsSplitMonth, objFTBCopyMonths.Month, objFTBCopyMonths.Year);

        $.ajax({
            url: ajaxURl,
            type: 'POST',
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify({ 'objFTBCopyMonthsDTO': objFTBCopyMonths }),
            dataType: 'json',
            success: function (data) {
                if (data.status == 1) {
                    BindFTBRates(data.Rates);
                    DisplayMessage("Rates copied successfully", false);
                }
                else {
                    DisplayMessage("No data exists for selected month", true);
                }

            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
    }
}
function BindLOR(jsonData) {
    if (jsonData != null && jsonData != '') {
        $("#ratesettingsLOR ul").empty();
        $("#ftbLORSelect").empty();
        $.each(jsonData, function (key, val) {
            $("#ratesettingsLOR ul").append('<li id=' + val.ID + ' value=' + val.MappedID + '>' + val.Code + '</li>');
            $("#ftbLORSelect").append('<li value=' + val.MappedID + '>' + val.Code + '</li>');
        });
        $("#ratesettingsLOR ul li").eq(0).addClass("selected");
        console.log(jsonData);

        $('#ratesettingsLOR ul li').click(function () {
            var rentalLengthId = $(this).attr('value');
            if (typeof (locationBrandID) != 'undefined' && typeof (rentalLengthId) != 'undefined') {
                year = $("#year ul li.selected").attr("value");
                month = $("#months ul li.selected").attr("value");
                GetFTBRates(locationBrandID, year, month, rentalLengthId);
            }
        });
    }
}
//#ENDREGION AJAX CALLS

