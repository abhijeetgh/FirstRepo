var defaultOpaqueRate = "-20";
var FirstClickOpaqueRatePopup = false;
var selectedRateCodes = "";
$(document).ready(function () {
    $("#opaque_rates").draggable();
    $("#OpaqueRate").on("click", function () {
        $('#opaque_rates, .popup_bg').show();
        OpaqueRateButton();
    });
    $("#opaque_rates #closepopup").on("click", function () {
        $("#opaque_rates, .popup_bg").hide();
        $("#OpaqueError").hide();

        $("#DailyOpaqueCarClass tbody tr #AllCarClass").val("");
        RemoveFlashableTag($("#opaque_rates .temp"));
        //populate saved rate codes
        //if (selectedRateCodes != "") {
        //    $("select#ratecodes").val('');
        //    $(selectedRateCodes.toString().split(',')).each(function () {
        //        $("select#ratecodes option[value=" + this + "]").prop("selected", true);
        //    });
        //}
        //else {
        //    ResetOpaqueRateCodesSelection();
        //}
        $("#DailyOpaqueCarClass input:text").each(function () {
            var txtId = $(this);
            if ($(txtId).val() != "" && $(txtId).attr("id") != "AllCarClass") {
                $(txtId).val($(txtId).attr("defaultvalue"));
            }
        })
    });

    $("#opaque_reset").on("click", function () {
        PreloadOpaqueData();
    });

    $("#opaque_copytoall").on("click", function () {
        CopyToAllOpaqueData();
    });
    $("#RateCodeAll").on("change", function () {
        if ($(this).prop("checked")) {
            $("#opaque_rates #ratecodes option").prop("selected", true);
        }
        else {
            $("#opaque_rates #ratecodes option").prop("selected", false);
        }
    });
    $("#opaque_rates #ratecodes").on("change", function () {
        if ($(this).val() != null) {
            if ($(this).find("option").length == $(this).val().length) {
                $("#RateCodeAll").prop("checked", true);
            }
            else {
                $("#RateCodeAll").prop("checked", false);
            }
        }
    });
    //save opaque rates
    $("#closeOpaquePopup").on("click", function () {
        if (ValidationOpaqueRate()) {
            $("#DailyOpaqueCarClass input:text").each(function () {
                var txtId = $(this);
                if ($(txtId).val() != "" && $(txtId).attr("id") != "AllCarClass") {
                    $(txtId).attr("defaultvalue", $(txtId).val());
                }
            });
            $("#DailyOpaqueCarClass tbody tr #AllCarClass").val("");
            $('#opaque_rates, .popup_bg').hide();
            selectedRateCodes = $("select#ratecodes").val();
        }

    });
});
//Entity class
var OpaqueRateEntity = function (data) {
    var self = this;
    self.ID = data.CarClassID;
    self.CarClass = $.trim(data.CarClass);
    self.Dates = data.Dates;
    self.ClassicDates = data.ClassicDates;
    self.PercentValue = data.PercentValue;
}
//End entity class

//Ajax Call funcitons
//End Ajax call functions

//Other functions
var OpaqueRateButton = function () {
    FirstClickOpaqueRatePopup = true;
    $("#opaqueSource").html($("#dimension-source li").html());
    //$("#opaqueClassicCarClass").html($("#displayDay li").html());
    var startDate = convertToServerTime(new Date(parseInt(GlobalLimitSearchSummaryData.StartDate.replace("/Date(", "").replace(")/", ""), 10)));
    var endDate = convertToServerTime(new Date(parseInt(GlobalLimitSearchSummaryData.EndDate.replace("/Date(", "").replace(")/", ""), 10)));
    var DateRange = (monthNames[startDate.getMonth()]).toString().substr(0, 3) + " " + startDate.getDate() + " - " + monthNames[endDate.getMonth()].toString().substr(0, 3) + " " + endDate.getDate();
    $("#opateDate").html(DateRange);
    $("#opaqueLocationID").html($("#location li").html());

    //    if ($("#viewSelect").find(".selected").text() == "daily") {
    // $("#DailyOpaqueCarClass").removeClass("hidden").show();
    //$("#ClassicOpaqueCarClass").addClass("hidden").hide();

    commonOpaqueRateBinding();
    //}
    //else {
    //var ClassicOpaqueRateData = [];
    //    $("#ClassicOpaqueCarClass").removeClass("hidden").show();
    //    $("#DailyOpaqueCarClass").addClass("hidden").hide();
    //    $(".classictable table tbody tr").each(function () {
    //        var formatDate;
    //        formatDate = $(this).find(".dates").attr('FormatDate');

    //        var item = {}
    //        item["CarClassID"] = $("#classic_view, #carClass li").eq(0).val();
    //        item["CarClass"] = $("#classic_view, #carClass li").eq(0).html();
    //        item["ClassicDates"] = $(this).find(".dates").html().replace("<br>", "");
    //        item["Dates"] = formatDate;
    //        if (item["CarClassID"] != undefined) {
    //            ClassicOpaqueRateData.push(item);
    //        }
    //    });
    //    var bindOpaqueRate = $.map(ClassicOpaqueRateData, function (item) { return new OpaqueRateEntity(item); });
    //    searchViewModel.ClassicOpaqueRateData(bindOpaqueRate);
    //}
}
var commonOpaqueRateBinding = function () {
    var OpaqueRateData = new Array();
    $("#result-section #carClass ul li").each(function () {
        var carClassID = $(this).val();
        var carClass = $(this).text();
        var item = {}
        item["CarClassID"] = carClassID;
        item["CarClass"] = carClass;
        item["PercentValue"] = (!FirstClickOpaqueRatePopup) ? defaultOpaqueRate : $("#Percent_" + carClassID).val();
        if (item["CarClassID"] != undefined) {
            OpaqueRateData.push(item);
        }
    })
    //$("#daily-rates-table tbody tr").each(function () {
    //    var carClassID = $(this).find(".carClassLogo").attr("classid");
    //    var carClass = $(this).find(".carClassLogo").attr("alt");
    //    var item = {}
    //    item["CarClassID"] = carClassID;
    //    item["CarClass"] = carClass;
    //    item["PercentValue"] = (!FirstClickOpaqueRatePopup) ? defaultOpaqueRate : $("#Percent_" + carClassID).val();
    //    if (item["CarClassID"] != undefined) {
    //        OpaqueRateData.push(item);
    //    }
    //});
    var bindOpaqueRate = $.map(OpaqueRateData, function (item) { return new OpaqueRateEntity(item); });
    searchViewModel.OpaqueRateData(bindOpaqueRate);
    TextboxBlurEvent();
}

var SaveDailyViewOpaqueRates = function (selectedCarClassArray) {
    var carClassId = '';
    var percent = '';
    var Date = $('#displayDay li.selected').attr('value');

    var opaqueRatesConfiguration = new Object();
    opaqueRatesConfiguration.IsDailyView = true;
    opaqueRatesConfiguration.RateCodes = searchViewModel.ApplicableRateCodes().map(function (i) { return i.Code }).join(",");
    opaqueRatesConfiguration.OpaqueRates = new Array();
    var objDailyViewCarClass;
    if (opaqueRatesConfiguration.RateCodes != '' && opaqueRatesConfiguration.RateCodes.length > 0) {


        ko.utils.arrayForEach(searchViewModel.OpaqueRateData(), function (currentItem) {
            if (currentItem != null && typeof (currentItem) != 'undefined') {
                carClassId = currentItem.ID;
                if ($("img[classid='" + carClassId + "']").closest("td").hasClass("selected") || (typeof (selectedCarClassArray) != 'undefined' && selectedCarClassArray.length > 0 && $.inArray(String(carClassId), selectedCarClassArray) > -1)) {
                    percent = $.trim($("#Percent_" + carClassId).val());
                    if (percent != "" && $.isNumeric(percent)) {
                        objDailyViewCarClass = new Object();
                        objDailyViewCarClass.CarClassId = carClassId;
                        objDailyViewCarClass.PercentValue = percent;
                        objDailyViewCarClass.CarCode = $.trim(currentItem.CarClass);
                        objDailyViewCarClass.Date = Date;

                        opaqueRatesConfiguration.OpaqueRates.push(objDailyViewCarClass);
                    }
                }
            }
        });
    }
    return opaqueRatesConfiguration;
}

var SaveClassicViewOpaqueRates = function () {
    var carClassId = $("#classic_view, #carClass li").eq(0).val();
    var percent = '';
    var Date = '';
    var carClass = $.trim($("#classic_view, #carClass li").eq(0).html());
    var opaqueRatesConfiguration = new Object();
    opaqueRatesConfiguration.IsDailyView = true;
    opaqueRatesConfiguration.RateCodes = searchViewModel.ApplicableRateCodes().map(function (i) { return i.Code }).join(",");
    opaqueRatesConfiguration.OpaqueRates = new Array();
    var objDailyViewCarClass;
    if (opaqueRatesConfiguration.RateCodes != '' && opaqueRatesConfiguration.RateCodes.length > 0) {
        $("table.classictable td.selected").each(function () {
            percent = $.trim($("#Percent_" + carClassId).val());
            if (percent != "" && $.isNumeric(percent)) {
                objDailyViewCarClass = new Object();
                objDailyViewCarClass.CarClassId = carClassId;
                objDailyViewCarClass.PercentValue = percent;
                objDailyViewCarClass.CarCode = carClass;
                objDailyViewCarClass.Date = $(this).attr("formatdate");

                opaqueRatesConfiguration.OpaqueRates.push(objDailyViewCarClass);
            }
        });


        //ko.utils.arrayForEach(searchViewModel.OpaqueRateData(), function (currentItem) {

        //    if (currentItem != null && typeof (currentItem) != 'undefined') {
        //        carClassId = currentItem.ID;
        //        if ($("td[formatdate='" + carClassId + "']").closest("td").hasClass("selected")) {
        //            percent = $.trim($("#Percent_" + carClassId).val());
        //            if (percent != "" && $.isNumeric(percent)) {
        //                objDailyViewCarClass = new Object();
        //                objDailyViewCarClass.CarClassId = carClassId;
        //                objDailyViewCarClass.PercentValue = percent;
        //                objDailyViewCarClass.CarCode = $.trim(currentItem.CarClass);
        //                objDailyViewCarClass.Date = Date;

        //                opaqueRatesConfiguration.OpaqueRates.push(objDailyViewCarClass);
        //            }
        //        }
        //    }
        //});
    }
    return opaqueRatesConfiguration;
}

function PreloadOpaqueData() {
    //for daily view
    RemoveFlashableTag($("#opaque_rates .temp"));
    $("#DailyOpaqueCarClass tbody tr").find("input:input").val(defaultOpaqueRate);
    $("#DailyOpaqueCarClass tbody tr #AllCarClass").val("");

    //for classic view
    //$("#ClassicOpaqueCarClass tbody tr").find("input").val(defaultOpaqueRate);
    //$("#ClassicOpaqueCarClass tbody tr #AllCarClass").val("");
}

function CopyToAllOpaqueData() {
    //for daily view
    var allcarclassdata = $("#DailyOpaqueCarClass tbody tr #AllCarClass").val();
    RemoveFlashableTag($("#opaque_rates .temp"));
    if (allcarclassdata.trim() != "" && $.isNumeric(allcarclassdata)) {
        $("#DailyOpaqueCarClass tbody tr").find("input:input").val(allcarclassdata);
    }
    else {
        var message = "";
        if (!$.isNumeric(allcarclassdata)) {
            message = "Please enter valid value";
        }
        else {
            message = "Please enter value in 'All Car Class' box";
        }


        ShowConfirmBox(message, false);
    }

    //for classic view
    //var allCarclassdata = $("#ClassicOpaqueCarClass tbody tr #AllCarClass").val();
    //$("#ClassicOpaqueCarClass tbody tr").find("input").val(allCarclassdata);
}
var TextboxBlurEvent = function () {
    $("#DailyOpaqueCarClass input:text").bind("input", function () {
        if ($.isNumeric($(this).val()) || $(this).val().trim() == '') {
            RemoveFlashableTag($(this));
        }
        else {
            MakeTagFlashable($(this));
        }
        AddFlashingEffect();
    });
}

function ValidationOpaqueRate() {
    var AllTxtboxBlank = true;
    if ($("#opaque_rates select").val() != null) {
        RemoveFlashableTag($("#opaque_rates select"));
    }
    else {
        MakeTagFlashable($("#opaque_rates select"));
    }

    var i = 0;
    $("#DailyOpaqueCarClass input:text").each(function () {
        if ($(this).val().trim() == "") {
            i++;
        }
    });

    if ($("#DailyOpaqueCarClass input:text").length == i) {
        AllTxtboxBlank = false;
    }

    if ($("#opaque_rates").find('.temp').length <= 0 && AllTxtboxBlank) {
        $("#OpaqueError").hide();
        return true;
    }
    else {
        if (!AllTxtboxBlank) {
            ShowConfirmBox("Please enter percent in textbox", false);
        }
        else {
            $("#OpaqueError").show();
        }
        return false;
    }
    AddFlashingEffect();
}

//var ResetOpaqueRateCodesSelection = function () {
//    $("#ratecodes option:selected").prop("selected", false);
//    $("#RateCodeAll").prop('checked', false);
//    $("#ratecodes option[default='true']").prop("selected", true);
//    selectedRateCodes = "";
//}

//End Other functions


