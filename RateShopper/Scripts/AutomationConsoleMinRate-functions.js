var automationMinRateModel;
var FinalMinRateData = [];
var SelectedScheduleMinRateData = [];
var carClassIds = [];
var locationBrandIds = [];
var masterCarClasses = [];
var FlagSaveData = false, firstClickEditData = false;

$(document).ready(function () {
    automationMinRateModel = new AutomationMinRateModel();
    ko.applyBindings(automationMinRateModel, document.getElementById('mr_popup'));

    $("#mr_popup").draggable();

    $("#view1 #SetMinRate").on("click", function (e) {
        e.preventDefault();
        if ($("#view1 #carClass select").val() != null && $("#view1 #carClass select").val() != undefined && $("#view1 #locations select").val() != null && $("#view1 #locations select").val() != undefined) {
            $('#mr_popup, .popup_bg').show();
            var MinRateList = [];
            masterCarClasses = [];
            var leftDays = ""; rightDays = "";
            for (var i = 0; i < $("#view1 #carClass select").val().length; i++) {
                var carClassID = $("#view1 #carClass select").val()[i];
                var DayMin = "", WeekMin = "", MonthMin = "", DayMax = "", WeekMax = "", MonthMax = "", Day2Min = "", Day2Max = "", Week2Min = "", Week2Max = "", Month2Min = "", Month2Max = "", Days1 = "", Days2 = "", chkAllDays=false;

                //For edit mode existing data 
                
                //Use for user does not save button till that time showing exising item from database
                if (jobId != undefined && jobId != "" && !firstClickEditData) {
                    FinalMinRateData = SelectedScheduleMinRateData;
                    $(SelectedScheduleMinRateData).each(function () {
                        if (carClassID == this.CarClassID) {
                            DayMin = this.DayMin;
                            WeekMin = this.WeekMin;
                            MonthMin = this.MonthMin;
                            DayMax = this.DayMax;
                            WeekMax = this.WeekMax;
                            MonthMax = this.MonthMax;
                            Day2Min = this.Day2Min;
                            Day2Max = this.Day2Max;
                            Week2Min = this.Week2Min;
                            Week2Max = this.Week2Max;
                            Month2Min = this.Month2Min;
                            Month2Max = this.Month2Max;
                            Days1 = this.Days1;
                            Days2 = this.Days2;
                            leftDays = this.Days1;
                            rightDays = this.Days2;
                            return false;
                        }
                    });
                }
                else {
                    if (FlagSaveData) {
                        DayMin = $('#DayMin_' + carClassID).val();
                        WeekMin = $('#WeekMin_' + carClassID).val();
                        MonthMin = $('#MonthMin_' + carClassID).val();
                        DayMax = $('#DayMax_' + carClassID).val();
                        WeekMax = $('#WeekMax_' + carClassID).val();
                        MonthMax = $('#MonthMax_' + carClassID).val();
                        Day2Min = $('#Day2Min_' + carClassID).val();
                        Day2Max = $('#Day2Max_' + carClassID).val();
                        Week2Min = $('#Week2Min_' + carClassID).val();
                        Week2Max = $('#Week2Max_' + carClassID).val();
                        Month2Min = $('#Month2Min_' + carClassID).val();
                        Month2Max = $('#Month2Max_' + carClassID).val();
                        Days1 = $.map($(".row1 ul li.selected"), function (element) {
                            return $(element).attr("value");
                        }).join(',');
                        Days2 = $.map($(".selectdays_row2 ul li.selected"), function (element) {
                            return $(element).attr("value");
                        }).join(',');
                        leftDays = Days1;
                        rightDays = Days2;
                    }
                    else {
                        $(FinalMinRateData).each(function () {
                            if (carClassID == this.CarClassID) {
                                DayMin = this.DayMin;
                                WeekMin = this.WeekMin;
                                MonthMin = this.MonthMin;
                                DayMax = this.DayMax;
                                WeekMax = this.WeekMax;
                                MonthMax = this.MonthMax;
                                Day2Min = this.Day2Min;
                                Day2Max = this.Day2Max;
                                Week2Min = this.Week2Min;
                                Week2Max = this.Week2Max;
                                Month2Min = this.Month2Min;
                                Month2Max = this.Month2Max;
                                Days1 = this.Days1;
                                Days2 = this.Days2;
                                leftDays = Days1;
                                rightDays = Days2;
                                return false;
                            }
                        });
                    }
                }                
               
                var item = {}
                item["CarClassID"] = carClassID;
                item["CarCode"] = $("#view1 #carClass select option[value=" + carClassID + "]").text();
                item["DayMin"] = DayMin;
                item["WeekMin"] = WeekMin;
                item["MonthMin"] = MonthMin;
                item["DayMax"] = DayMax;
                item["WeekMax"] = WeekMax;
                item["MonthMax"] = MonthMax;
                item["Day2Min"] = Day2Min;
                item["Day2Max"] = Day2Max;
                item["Week2Min"] = Week2Min;
                item["Week2Max"] = Week2Max;
                item["Month2Min"] = Month2Min;
                item["Month2Max"] = Month2Max;
                item["Days1"] = Days1;
                item["Days2"] = Days2;
                MinRateList.push(item);
            }

            if (MinRateList.length > 0) {
                var Days1 = leftDays;
                var Days2 = rightDays;
                $("#MinRates .Days_ul li.selected").removeClass("selected").removeClass("ui-selected");

                if (typeof (Days1) != 'undefined') {
                    var Days1Array = Days1.split(',');
                    if (Days1Array.length == 7) {
                        $(".minrate_chkAlldays").prop("checked", true);
                        $("#MinRates").find(".selectdays_row2 .RateLimit_days .selected").removeClass("selected").removeClass("ui-selected");
                        $(".minrate_section_two").hide();
                    }
                    else {
                        $(".minrate_chkAlldays").prop("checked", false);
                        $(".minrate_section_two").show();
                    }
                    $(Days1Array).each(function () {
                        $(".row1 ul li[value='" + this + "']").removeClass("selected").addClass("selected");
                    });
                }
                if (Days2 != null && typeof (Days2) != 'undefined') {
                    $(Days2.split(',')).each(function () {
                        $(".selectdays_row2 ul li[value='" + this + "']").removeClass("selected").addClass("selected");
                    });
                }
            }

            var BindMinRateData = $.map(MinRateList, function (item) { return new MinRatedata(item); });
            automationMinRateModel.MinRateList(BindMinRateData);
            masterCarClasses = BindMinRateData;
            ValidateMinData();
        }
        else {
            if ($("#view1 #carClass select").val() == null && $("#view1 #carClass select").val() == undefined && $("#view1 #locations select").val() == null && $("#view1 #locations select").val() == undefined) {
                ShowConfirmBox('Select location and car class', false);
                //alert("Select Location and Car Class");
            }
            else if ($("#view1 #carClass select").val() == null && $("#view1 #carClass select").val() == undefined) {
                ShowConfirmBox('Please select car class', false);
                //alert("Please select car class");
            }
            else if ($("#view1 #locations select").val() == null && $("#view1 #locations select").val() == undefined) {
                ShowConfirmBox('Please select location', false);
                //alert("Please select location");
            }
        }
    });
    $("#closepopupMinRate").on("click", function () {
        $("#mr_popup, .popup_bg").hide();
        FlagSaveData = false;
    });

    $("#MinRateSave").on("click", function (e) {
        e.preventDefault();
        if (IsMinRatesValid()) {
            FlagSaveData = true;
            firstClickEditData = true;
            var ScheduleJobID = 0;
            if (jobId != undefined) {
                ScheduleJobID = jobId;
            }
            GetMinRateData(ScheduleJobID);
        }
    });
    $('#MinRatePreload').on("click", function () {

        if (!$('#rb-horizonShop').prop('checked')) {
            var ScheduleJobID = 0;
            if (jobId != undefined && jobId != "") {
                ScheduleJobID = jobId;
            }
            else {
                ScheduleJobID = 0;
            }
            preloadMinRates(ScheduleJobID);
        }
    });

    //day selectable
    $(".RateLimit_days").selectable({
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
                $(this).siblings('.minrate_chkAlldays').prop('checked', true);
                if ($(this).siblings('.minrate_chkAlldays').length > 0) {
                    $(".minrate_section_two").hide();
                    $("#MinRates").find(".selectdays_row2 .RateLimit_days .selected").removeClass("selected").removeClass("ui-selected");
                }
            }
            else {
                $(this).siblings('.minrate_chkAlldays').prop('checked', false);
                $(".minrate_section_two").show();
            }
        },
    });

    //check box for all
    $('.minrate_chkAlldays').click(function () {
        if ($(this).is(':checked')) {
            $(this).siblings('ul').children().addClass('selected');           
            $("#MinRates").find(".selectdays_row2 .RateLimit_days .selected").removeClass("selected").removeClass("ui-selected");
            $(".minrate_section_two").hide();
        }
        else {
            $(this).siblings('ul').children().removeClass('selected');            
            $(".minrate_section_two").show();
        }

    });
});

//ViewModel//
function AutomationMinRateModel() {
    var self = this;
    self.MinRateList = ko.observableArray([]);
}
//End ViewModel//

//Entities//
function MinRatedata(data) {
    this.CarClassID = data.CarClassID;
    this.CarCode = data.CarCode;
    this.DayMin = data.DayMin;
    this.WeekMin = data.WeekMin;
    this.MonthMin = data.MonthMin;
    this.DayMax = data.DayMax;
    this.WeekMax = data.WeekMax;
    this.MonthMax = data.MonthMax;
    this.Day2Min = data.Day2Min;
    this.Day2Max = data.Day2Max;
    this.Week2Min = data.Week2Min;
    this.Week2Max = data.Week2Max;
    this.Month2Min = data.Month2Min;
    this.Month2Max = data.Month2Max;
}
//End Entities


//Other functions
function GetMinRateData(ScheduleJobID) {
    FinalMinRateData = [];
    if (validateData() == true) {
        var Days1 = $.map($(".row1 ul li.selected"), function (element) {
            return $(element).attr("value");
        }).join(',');
        var Days2 = $.map($(".selectdays_row2 ul li.selected"), function (element) {
            return $(element).attr("value");
        }).join(',');

        $("#searchResultRatesOne tbody tr").each(function (index) {
            var carClassId = $(this).find("span").attr("CarClassID");
            var DayMin = $("#DayMin_" + carClassId).val(), DayMax = $("#DayMax_" + carClassId).val(),
                WeekMin = $("#WeekMin_" + carClassId).val(), WeekMax = $("#WeekMax_" + carClassId).val(),
                MonthMin = $("#MonthMin_" + carClassId).val(), MonthMax = $("#MonthMax_" + carClassId).val(),
                Day2Min = "", Day2Max = "", Week2Min = "", Week2Max = "", Month2Min = "", Month2Max = "";

            if (typeof (Days2) != 'undefined' && Days2 != "") {
                Day2Min = $("#Day2Min_" + carClassId).val();
                Day2Max = $("#Day2Max_" + carClassId).val();
                Week2Min = $("#Week2Min_" + carClassId).val();
                Week2Max = $("#Week2Max_" + carClassId).val();
                Month2Min = $("#Month2Min_" + carClassId).val();
                Month2Max = $("#Month2Max_" + carClassId).val();
            }
            else {
                $("#Day2Min_" + carClassId).val('');
                $("#Day2Max_" + carClassId).val('');
                $("#Week2Min_" + carClassId).val('');
                $("#Week2Max_" + carClassId).val('');
                $("#Month2Min_" + carClassId).val('');
                $("#Month2Max_" + carClassId).val('');
            }


            if ($("#MinRates input[type='number'][id$='_" + carClassId + "']").filter(function () { return $(this).val() != "" && $(this).val() > 0 }).length > 0) {
                var scheduledJobMinRates = new Object();
                scheduledJobMinRates.ScheduleJobID = ScheduleJobID;
                scheduledJobMinRates.CarClassID = $(this).find("span").attr("CarClassID");
                if (DayMin > 0) { scheduledJobMinRates.DayMin = DayMin; }
                if (WeekMin > 0) { scheduledJobMinRates.WeekMin = WeekMin; }
                if (MonthMin > 0) { scheduledJobMinRates.MonthMin = MonthMin; }
                if (DayMax > 0) { scheduledJobMinRates.DayMax = DayMax; }
                if (WeekMax > 0) { scheduledJobMinRates.WeekMax = WeekMax; }
                if (MonthMax > 0) { scheduledJobMinRates.MonthMax = MonthMax; }
                if (Day2Min > 0) { scheduledJobMinRates.Day2Min = Day2Min; }
                if (Day2Max > 0) { scheduledJobMinRates.Day2Max = Day2Max; }
                if (Week2Min > 0) { scheduledJobMinRates.Week2Min = Week2Min; }
                if (Week2Max > 0) { scheduledJobMinRates.Week2Max = Week2Max; }
                if (Month2Min > 0) { scheduledJobMinRates.Month2Min = Month2Min; }
                if (Month2Max > 0) { scheduledJobMinRates.Month2Max = Month2Max; }
                if (Days1 != "") { scheduledJobMinRates.Days1 = Days1; }
                if (Days2 != "" && typeof (Days2) != 'undefined') {
                    scheduledJobMinRates.Days2 = Days2;
                }
                else {
                    scheduledJobMinRates.Days2 = "";
                }
                
                FinalMinRateData.push(scheduledJobMinRates);
            }
        });
        // console.log(JSON.stringify(FinalMinRateData));
    }
    $("#mr_popup, .popup_bg").hide();
}
function ValidateMinData() {
    var flag = false;
    $("#mr_popup input[type=number]").on("keyup", function () {
        var textbox = $(this).val().trim();
        //if (textbox != "" && !$.isNumeric(textbox)) {
        //    flag = false;
        //    MakeTagFlashable($(this));
        //}
        //else {

        if (!$.isNumeric(textbox)) {
            $(this).val("");
            flag = false;
            return false;
        }
        else {
            flag = true;
        }
        //    flag = true;
        //    RemoveFlashableTag($(this));
        //}
    });
    //AddFlashingEffect();

    $("#mr_popup input[type=number]").on("input", function () {
        var $this = $(this)
        if ($this.hasClass("temp")) {
            RemoveFlashableTag($this);
        }
        //if ($this.val() == "" || $this.val() == 0) {
            var isMinValue = $this.hasClass("min");
            if (isMinValue) {
                var maxInput = $this.closest('tr').find("input[type='number'][name='" + $this.attr('name') + "'].max");
                if (parseInt($this.val()) <= parseInt($(maxInput).eq(0).val())) {
                    RemoveFlashableTag(maxInput);
                }
            }
            else {
                var minInput = $this.closest('tr').find("input[type='number'][name='" + $this.attr('name') + "'].min");
                if (parseInt($this.val()) >= parseInt($(minInput).eq(0).val())) {
                    RemoveFlashableTag(minInput);
                }
            }
        //}
    });


    return flag;
}
function validateData() {
    var flag = false;
    $("#mr_popup input[type=number]").each(function () {
        //if ($(this).hasClass("temp")) {
        //    MakeTagFlashable($(this));
        //    flag = false;
        //    return false;
        //}
        //else {
        if (!$.isNumeric($(this).val().trim())) {
            $(this).val("");
        }
        else {
            flag = true;
        }
        //    flag = true;
        //    RemoveFlashableTag($(this));
        //}
        //AddFlashingEffect()
    });
    return flag;
}

function IsMinRatesValid() {
    if (ValidateDaysSelected()) {
        var valuesInLeftSection = $("#searchResultRatesOne input[type='number']").filter(function () { return $(this).val().length > 0 && $(this).val() > 0 });
        var valuesInRightSection  = $(".minrate_section_two:visible input[type='number']").filter(function () { return $(this).val().length > 0 && $(this).val() > 0 });        

        ValidateRates(valuesInLeftSection);
        ValidateRates(valuesInRightSection);

        AddFlashingEffect();

        if ($("#MinRates input[type=number].temp").length==0) {
            return true;
        }
    }
    return false;
}

var ValidateRates = function (section) {
    for (i = 0; i < section.length; i++) {
        var $item = $(section[i]);
        var isMinValue = $item.hasClass("min");
        if (isMinValue) {
            var maxInput = $item.closest('tr').find("input[type='number'][name='" + $item.attr('name') + "'].max");
            if (maxInput.length > 0) {
                if (parseInt($(maxInput).eq(0).val()) > 0 && parseInt($item.val()) <= parseInt($(maxInput).eq(0).val())) {
                    continue;
                }
                else {
                    MakeTagFlashable($(maxInput));
                }
            }
        }
        else {
            var minInput = $item.closest('tr').find("input[type='number'][name='" + $item.attr('name') + "'].min");
            if (minInput.length > 0) {
                if (parseInt($(minInput).eq(0).val()) > 0 && parseInt($item.val()) >= parseInt($(minInput).eq(0).val())) {
                    continue;
                }
                else {
                    MakeTagFlashable($(minInput));
                }
            }
        }
    }
}

var ValidateDaysSelected = function () {
    var itemsSelectedFromFirstRow = $.map($(".row1 ul li.selected"), function (element) {
        return $(element).attr("value");
    });

    if (itemsSelectedFromFirstRow.length == 0) {
        ShowConfirmBox('Please select the day(s) in left section', false);
        return false;
    }
    var isValueEnterInFirstSection = $("#searchResultRatesOne input[type='number']").filter(function () { return $(this).val().length > 0 && $(this).val() > 0 }).length > 0;
    if (!isValueEnterInFirstSection) {
        ShowConfirmBox('Please enter min rates in left section', false);
        return false;
    }

    var isValueEnterInSecondSection = $(".minrate_section_two:visible input[type='number']").filter(function () { return $(this).val().length > 0 && $(this).val()>0 }).length > 0;
    var itemsSelectedFromSecondRow = $.map($(".selectdays_row2 ul li.selected"), function (element) {
        return $(element).attr("value");
    });    

    if (isValueEnterInSecondSection && itemsSelectedFromSecondRow.length == 0) {
        ShowConfirmBox('Please select the day(s) in right section', false);
        return false;
    }    

    if (itemsSelectedFromFirstRow.length > 0 && itemsSelectedFromSecondRow.length > 0) {
        var commondays = $.grep(itemsSelectedFromFirstRow, function (element) {
            return $.inArray(element, itemsSelectedFromSecondRow) !== -1;
        });
        if (commondays.length > 0) {
            ShowConfirmBox('Please select unique days', false);
            return false;
        }        
    }

    if (!isValueEnterInSecondSection && itemsSelectedFromSecondRow.length > 0) {
        ShowConfirmBox('Please enter min rates in right section', false);
        return false;
    }
    return true;
}
//End Other functions


//Ajax functions
function GetSelectedScheduleMinRate(jobId) {
    SelectedScheduleMinRateData = [];
    var ajaxURl = '/RateShopper/AutomationConsole/GetSelectedMinRate';
    if (MinRates != undefined && MinRates != '') {
        ajaxURl = MinRates.GetSelectedMinRateURL;
    }
    $.ajax({
        type: "GET",
        url: ajaxURl,
        dataType: "json",
        data: { ScheduleJobID: jobId },
        success: function (data) {
            if (data) {
                SelectedScheduleMinRateData = data;
                FinalMinRateData = SelectedScheduleMinRateData;//For incase user didnt open minrate popup and direct goes to save data.
            }
        },
        error: function (e) {
            //$('.loader_container_main').hide();
            console.log(e.message);
        }
    });
}
//End Ajax functions


function preloadMinRates(scheduledJobId) {
    var ajaxURl = '/RateShopper/AutomationConsole/PreLoadMinRates';
    if (MinRates != undefined && MinRates != '') {
        ajaxURl = MinRates.PreLoadMinRates;
    }
    carClassIds = [];
    locationBrandIds = [];
    $("#searchResultRatesOne td span.carClasses").each(function () {
        carClassIds.push(parseInt($(this).attr('carclassid')));
    });
    $('#locations option:selected').each(function () {
        locationBrandIds.push($(this).attr('BrandId'));
    });
    $.ajax({
        type: "POST",
        url: ajaxURl,
        data: JSON.stringify({ 'CarClassId': carClassIds, 'locationBrandIds': locationBrandIds.toString(), 'ScheduledJobId': scheduledJobId }),
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            //console.log(data);
            if (JSON.parse(data.status)) {
                //console.log(data);
                //console.log(data.rateList);
                //$('#MinRates #searchResultRates input[type="number"]').val('');
                var Days1 = "", Days2 = "";
                var mappedData = $.map(masterCarClasses, function (item) {
                    var carClassFound = false;                   
                    for (i = 0; i < data.rateList.length; i++) {
                        if (data.rateList[i].CarClassId == parseInt(item.CarClassID)) {
                            item.DayMin = data.rateList[i].DayMinRate;
                            item.WeekMin = data.rateList[i].WeekMin;
                            item.MonthMin = data.rateList[i].MonthMin;
                            item.DayMax = data.rateList[i].DayMax;
                            item.WeekMax = data.rateList[i].WeekMax;
                            item.MonthMax = data.rateList[i].MonthMax;
                            item.Day2Min = data.rateList[i].Day2Min;
                            item.Day2Max = data.rateList[i].Day2Max;
                            item.Week2Min = data.rateList[i].Week2Min;
                            item.Week2Max = data.rateList[i].Week2Max;
                            item.Month2Min = data.rateList[i].Month2Min;
                            item.Month2Max = data.rateList[i].Month2Max;
                            Days1 = data.rateList[i].Days1;
                            Days2 = data.rateList[i].Days2;
                            carClassFound = true;
                        }
                    }
                    if (!carClassFound) {
                        item.DayMin = "";
                        item.WeekMin = "";
                        item.MonthMin = "";
                        item.DayMax = "";
                        item.WeekMax = "";
                        item.MonthMax = "";
                        item.Day2Min = "";
                        item.Day2Max = "";
                        item.Week2Min = "";
                        item.Week2Max = "";
                        item.Month2Min = "";
                        item.Month2Max = "";
                    }
                    return new MinRatedata(item);
                });
                var MinRateList = mappedData;
                if (MinRateList.length > 0) {                    
                    $("#MinRates .Days_ul li.selected").removeClass("selected").removeClass("ui-selected");

                    if (Days1 != null && typeof (Days1) != 'undefined' && Days1 !="") {
                        var Days1Array = Days1.split(',');
                        if (Days1Array.length == 7) {
                            $(".minrate_chkAlldays").prop("checked", true);
                            $("#MinRates").find(".selectdays_row2 .RateLimit_days .selected").removeClass("selected").removeClass("ui-selected");
                            $(".minrate_section_two").hide();
                        }
                        else {
                            $(".minrate_chkAlldays").prop("checked", false);
                            $(".minrate_section_two").show();
                        }
                        $(Days1Array).each(function () {
                            $(".row1 ul li[value='" + this + "']").removeClass("selected").addClass("selected");
                        });
                    }
                    if (Days2 != null && typeof (Days2) != 'undefined' && Days2 != "") {
                        $(Days2.split(',')).each(function () {
                            $(".selectdays_row2 ul li[value='" + this + "']").removeClass("selected").addClass("selected");
                        });
                    }
                }

                automationMinRateModel.MinRateList(mappedData);
            }
        },
        error: function (e) {
            //$('.loader_container_main').hide();
            console.log(e.message);
        }
    });
}