var defaultOpaqueRate = "-20";
var automationOpaqueModel;
var FirstClickOpaqueRatePopup = false;
var finalOpaqueRate = new Array();
var jobSpecificOpaqueRate = "";
var selectedRateCodes = "";
$(document).ready(function () {
    $("#OpaqueSourceLocation").hide();
    automationOpaqueModel = new AutomationOpaqueModel();
    ko.applyBindings(automationOpaqueModel, document.getElementById('mr_Opaque_popup'));

    $("#mr_Opaque_popup").draggable();

    $("#opaque_reset").on("click", function () {
        PreloadOpaqueData();
    });
    $("#opaque_copytoall").on("click", function () {
        CopyToAllOpaqueData();
    });

    $("#btnOpaqueRate").on("click", function () {

        $('#mr_Opaque_popup, .popup_bg').show();
        FirstClickOpaqueRatePopup = true;
        BindOpaqueRate();        
    });

    
    $("#mr_Opaque_popup #closepopup").on("click", function () {
        $("#mr_Opaque_popup, .popup_bg").hide();
        $("#OpaqueError").hide();

        $("#DailyOpaqueCarClass tbody tr #AllCarClass").val("");
        RemoveFlashableTag($("#mr_Opaque_popup .temp"));
        $("#DailyOpaqueCarClass input:text").each(function () {
            var txtId = $(this);
            if ($(txtId).val() != "" && $(txtId).attr("id") != "AllCarClass") {
                $(txtId).val($(txtId).attr("defaultvalue"));
            }
        });
        //if (selectedRateCodes != "") {
        //    $("select#ratecodes").val('');
        //    $(selectedRateCodes.toString().split(',')).each(function () {
        //        $("select#ratecodes option[value=" + this + "]").prop("selected", true);
        //    });
        //}
        //else {
        //    $("#ratecodes option:selected").prop("selected", false);
        //    $("#RateCodeAll").prop('checked', false);
        //    $("#mr_Opaque_popup #ratecodes option[default='true']").prop("selected", true);
        //}
        FirstClickOpaqueRatePopup = false;
    });
    $("#RateCodeAll").on("change", function () {
        if ($(this).prop("checked")) {
            $("#mr_Opaque_popup #ratecodes option").prop("selected", true);
        }
        else {
            $("#mr_Opaque_popup #ratecodes option").prop("selected", false);
        }
    });
    $("#mr_Opaque_popup #ratecodes").on("change", function () {
        if ($(this).val() != null) {
            if ($(this).find("option").length == $(this).val().length) {
                $("#RateCodeAll").prop("checked", true);
            }
            else {
                $("#RateCodeAll").prop("checked", false);
            }
        }
    });
    //Submit button event fire
    $("#closeOpaquePopup").on("click", function () {
        if (ValidationOpaqueRate()) {
            $("#DailyOpaqueCarClass tbody tr #AllCarClass").val("");
            $("#DailyOpaqueCarClass input:text").each(function () {
                var txtId = $(this);
                if ($(txtId).val() != "" && $(txtId).attr("id") != "AllCarClass") {
                    $(txtId).attr("defaultvalue", $(txtId).val());
                }
            });
            SaveOpaqueRates();
            //selectedRateCodes = $("select#ratecodes").val();
            $('#mr_Opaque_popup, .popup_bg').hide();
        }
    });
    // To disable the button on checking the Activate Opaque checkbox.
    $('[id=activeOpaqueRate]').on("change", function () {
        if ($(this).prop("checked")) {
            $('[id=btnOpaqueRate]').removeAttr('disabled').removeClass("disable-button");
        }
        else {
            $('[id=btnOpaqueRate]').attr('disabled', 'disable').addClass("disable-button");
        }
    });
});


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

function BindRateCodes(lstApplicableRateCodes) {
    //SearchRateCodeModel
    automationOpaqueModel.ApplicableRateCodes($.map(lstApplicableRateCodes, function (item) { return new SearchRateCodeModel(item); }));
}


var GetRateCodes = function () {
    var startDate = $('#startDate').val();
    var endDate = $('#endDate').val();
    if (startDate != "" || endDate != "") {
        $.ajax({
            url: 'AutomationConsole/GetApplicableRateCodes',
            type: "POST",
            dataType: "json",
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify({ 'strStartDate': startDate, 'strEndDate': endDate }),
            //data: { 'userId': loggedInUserId },
            success: function (data) {
                BindRateCodes(data);
            },
            error: function (e) {
                console.log(e.message);
            }
        });
    }
}

//Entity class
var OpaqueRateEntity = function (data) {
    var self = this;
    self.ID = data.CarClassID;
    self.CarClass = data.CarClass;
    self.PercentValue = data.PercentValue;
}
//End entity class

//ViewModel Class
var AutomationOpaqueModel = function () {
    var self = this;
    self.OpaqueRateData = ko.observableArray([]);
    self.ApplicableRateCodes = ko.observableArray([]);
}
//End ViewModel Class

//Ajax functions
var getEditJobOpaqueRateData = function (scheduledJobId, carClassIds) {
    var ajaxURl = '/RateShopper/AutomationConsole/GetEditJobOpaqueRates';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetOpaqueRateUrl;
    }
    $.ajax({
        url: ajaxURl,
        type: 'POST',
        data: { jobId: scheduledJobId },
        async: true,
        success: function (data) {
            if (data) {
                jobSpecificOpaqueRate = data;
                var OpaqueRateData = new Array();
                var SelectedCarClass = $("#view1 #carClass select").val();
                if (SelectedCarClass != undefined && SelectedCarClass != '') {
                    $(SelectedCarClass).each(function () {
                        var CarClassID = $("#view1 #carClass select option[value=" + this + "]").val();
                        var CarClass = $("#view1 #carClass select option[value=" + this + "]").text();
                        var item = {}
                        item["CarClassID"] = CarClassID;
                        item["CarClass"] = CarClass;
                        item["PercentValue"] = "";
                        if (item["CarClassID"] != undefined) {
                            OpaqueRateData.push(item);
                        }
                    });
                }
                var bindOpaqueRate = $.map(OpaqueRateData, function (item) { return new OpaqueRateEntity(item); });
                automationOpaqueModel.OpaqueRateData(bindOpaqueRate);

                //Populated only configured value
                setTimeout(function () {
                    $(data).each(function () {
                        console.log(this);
                        $("#Percent_" + this.CarClassId).val(this.PercenValue).attr("defaultvalue", this.PercenValue);
                        console.log($("#Percent_" + this.CarClassId).val());
                    });
                    TextboxBlurEvent();
                    SaveOpaqueRates();//This call used for user do not any operation and click on save button
                }, 250);



            }
        },
        error: function (e) {
            console.log(e.message);
        }
    });
}
//End Ajax functions

//Other Functions
var BindOpaqueRate = function () {
    if ($("#view1 #carClass select").val() != null && $("#view1 #carClass select").val() != undefined && $("#view1 #locations select").val() != null && $("#view1 #locations select").val() != undefined) {
        var SelectedCarClass = $("#view1 #carClass select").val();
        if (SelectedCarClass != undefined && SelectedCarClass != '') {
            var OpaqueRateData = new Array();
            $(SelectedCarClass).each(function () {
                var CarClassID = $("#view1 #carClass select option[value=" + this + "]").val();
                var CarClass = $("#view1 #carClass select option[value=" + this + "]").text();

                var item = {}
                item["CarClassID"] = CarClassID;
                item["CarClass"] = CarClass;
                item["PercentValue"] = (!FirstClickOpaqueRatePopup) ? ((jobId == undefined || jobId == "") ? defaultOpaqueRate : "") : $("#Percent_" + CarClassID).val();
                if (item["CarClassID"] != undefined) {
                    OpaqueRateData.push(item);
                }
            });
            var bindOpaqueRate = $.map(OpaqueRateData, function (item) { return new OpaqueRateEntity(item); });
            automationOpaqueModel.OpaqueRateData(bindOpaqueRate);

            //Edit job scenario if already saved carclass value and user can change it selection at the moment but however user select saved carcClass value 
            // at this moment user will get old value 
            if (jobId != "" || jobId != undefined) {
                $("#DailyOpaqueCarClass  input:text").filter(function () { return this.value.trim() == ""; }).each(function () {
                    if ($(this).attr("id") != "AllCarClass") {
                        var CarClassId = $(this).attr("id").split('_')[1];

                        var getConfiguredOpaqueRate = $(jobSpecificOpaqueRate).filter(function () { return this.CarClassId == CarClassId; });
                        if (getConfiguredOpaqueRate.length > 0) {
                            var percentValue = getConfiguredOpaqueRate[0].PercenValue;
                            $("#Percent_" + CarClassId).val(percentValue).attr("defaultvalue", percentValue);
                        }
                        else {
                            //Scenario in Edit job : Not configured opaque rate, however user delibaratly activate checkbox opeque rate and clicks on save button
                            if ($("#activeOpaqueRate").attr("isconfigured") == "false") {
                                $("#Percent_" + CarClassId).val(defaultOpaqueRate);
                            }
                        }
                    }
                });
            }
            else {
                //New job creation scenario
                $("#DailyOpaqueCarClass  input:text").filter(function () { return this.value.trim() == ""; }).each(function () {
                    if ($(this).attr("id") != "AllCarClass") {
                        var CarClassId = $(this).attr("id").split('_')[1];
                        $("#Percent_" + CarClassId).val(defaultOpaqueRate).attr("defaultvalue", defaultOpaqueRate);
                    }
                });
            }
            TextboxBlurEvent();
            SaveOpaqueRates();//This call used for user do not any operation and click on save button
        }
    }
}
var SaveOpaqueRates = function () {
    finalOpaqueRate = new Array();
    var objDailyViewCarClass;
    //if (automationOpaqueModel.ApplicableRateCodes().length > 0) {
        ko.utils.arrayForEach(automationOpaqueModel.OpaqueRateData(), function (currentItem) {
            if (currentItem != null && typeof (currentItem) != 'undefined') {
                var carClassId = '';
                var percent = '';
                carClassId = currentItem.ID;
                percent = $.trim($("#Percent_" + carClassId).val());
                if (percent != "" && $.isNumeric(percent)) {
                    objDailyViewCarClass = new Object();
                    objDailyViewCarClass.CarClassId = carClassId;
                    objDailyViewCarClass.PercenValue = percent;
                    objDailyViewCarClass.CarCode = currentItem.CarClass;
                    finalOpaqueRate.push(objDailyViewCarClass);
                }
            }
        });
    //}
    //console.log(finalOpaqueRate);
}
function PreloadOpaqueData() {
    RemoveFlashableTag($("#mr_Opaque_popup .temp"));
    $("#DailyOpaqueCarClass tbody tr").find("input:input").val(defaultOpaqueRate);
    $("#DailyOpaqueCarClass tbody tr #AllCarClass").val("");
}

function CopyToAllOpaqueData() {
    //for daily view
    RemoveFlashableTag($("#mr_Opaque_popup .temp"));
    var allcarclassdata = $("#DailyOpaqueCarClass tbody tr #AllCarClass").val();
    if (allcarclassdata.trim() != "" && $.isNumeric(allcarclassdata)) {
        $("#DailyOpaqueCarClass tbody tr").find("input:input").val(allcarclassdata);
    }
    else {
        ShowConfirmBox("Please enter value in 'All Car Class' box", false);
    }
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
    if ($("#mr_Opaque_popup select").val() != null) {
        RemoveFlashableTag($("#mr_Opaque_popup select"));
    }
    else {
        MakeTagFlashable($("#mr_Opaque_popup select"));
    }
    var i = 0;
    $("#DailyOpaqueCarClass input:text").each(function () {
        if ($(this).val() == "") {
            i++;
        }
    });
    if ($("#DailyOpaqueCarClass input:text").length != i) {
        AllTxtboxBlank = false;
    }

    if ($("#mr_Opaque_popup").find('.temp').length <= 0 && !AllTxtboxBlank) {
        $("#OpaqueError").hide();
        return true;

    }
    else {
        if (AllTxtboxBlank) {
            ShowConfirmBox("Please enter percent in textbox", false);
        }
        $("#OpaqueError").show();
        return false;
    }
    AddFlashingEffect();
    return false;
}
//End Other Functions