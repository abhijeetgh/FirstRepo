
var globalLimitModel;
var locationBrandID = 0;

$(document).ready(GlobalLimitDocumentReady);
//Show loader before every ajax call
$(document).ajaxStart(function () { $(".loader_container_main").show(); });
//Hide loader before every ajax call
$(document).ajaxComplete(function () { $(".loader_container_main").hide(); });

function GlobalLimitDocumentReady() {
    globalLimitModel = new GlobalLimitModel();
    //Apply knockout binding
    ko.applyBindings(globalLimitModel);


    $('#ddlBrand-GlobalLimit li').click(function () {
        locationBrandID = $(this).attr('value').toString();
        GetGlobalLimit(locationBrandID);
    });    
}

//Knockout js variables
var GlobalLimitModel = function () {
    var self = this;
    self.globallimits = ko.observableArray([]);
    self.deleteGlobalLimit = function (globalLimit) {
        var message = "Are you sure you want to delete this limit range " + globalLimit.StartDate + " through " + globalLimit.EndDate + "?";
        $("#alertbox").css("top", $(window).scrollTop() + $(window).height()/3);
        ShowConfirmBox(message, true, DeleteGlobalLimit, globalLimit);
    }

    self.saveGlobalLimit = function (globalLimit) {
        if (ValidateCurrentGlobalLimit(globalLimit.GlobalLimitID)) {
            SaveGlobalLimit(globalLimit);
        }
    }

    self.createNew = function (globalLimit) {        
        //$("#GL_0 .globaldetails input[type='text']").val('');       
        RemoveFlashableTag("#StartDate_0");
        RemoveFlashableTag("#EndDate_0");
        $("#lblMessage_0").hide();
        $("#GL_0 input[type='text']").val('');
        $("#GL_0").show();        
        $("#GL_0.globallimits input[type='button'][value='Save']").addClass("btnDisabled").prop("disabled", true);
        window.scrollTo(0, document.body.scrollHeight - ($("#GL_0").height() + 100));
    }

    self.copyAndCreate = function (globalLimit) {
        CopyAndCreate(globalLimit);
    }
}

var CopyAndCreate = function (objGlobalLimit) {    
    $("#GL_0").show();
    $("#GL_0 input[type='text']").val('');
    RemoveFlashableTag("#StartDate_0");
    RemoveFlashableTag("#EndDate_0");
    $("#lblMessage_0").hide();
    for (var i = 0; i < objGlobalLimit.LstGlobalLimitDetails.length; i++) {
        var tr = $("#GL_0 tr[data_class='" + objGlobalLimit.LstGlobalLimitDetails[i].CarClassID + "']");
        if (!(objGlobalLimit.LstGlobalLimitDetails[i].DayMin == null && objGlobalLimit.LstGlobalLimitDetails[i].DayMax == null && objGlobalLimit.LstGlobalLimitDetails[i].WeekMin == null && objGlobalLimit.LstGlobalLimitDetails[i].WeekMax == null && objGlobalLimit.LstGlobalLimitDetails[i].MonthlyMax == null && objGlobalLimit.LstGlobalLimitDetails[i].MonthlyMin == null)) {
            $(tr).attr("data-edit", '1');
        }
        if (tr.length > 0) {
            $($(tr).find("input[name='DayMin']")).val(objGlobalLimit.LstGlobalLimitDetails[i].DayMin);
            $($(tr).find("input[name='DayMax']")).val(objGlobalLimit.LstGlobalLimitDetails[i].DayMax);
            $($(tr).find("input[name='WeekMin']")).val(objGlobalLimit.LstGlobalLimitDetails[i].WeekMin);
            $($(tr).find("input[name='WeekMax']")).val(objGlobalLimit.LstGlobalLimitDetails[i].WeekMax);
            $($(tr).find("input[name='MonthlyMax']")).val(objGlobalLimit.LstGlobalLimitDetails[i].MonthlyMax);            
            $($(tr).find("input[name='MonthlyMin']")).val(objGlobalLimit.LstGlobalLimitDetails[i].MonthlyMin);
        }
    }    
    window.scrollTo(0, document.body.scrollHeight - ($("#GL_0").height() + 100));
    if ($("#GL_0 tr[data-edit='1']").length > 0) {
        $("#GL_0.globallimits input[type='button'][value='Save']").removeClass("btnDisabled").prop("disabled", false);
    }
}

var ValidateCurrentGlobalLimit = function (globalLimitID) {
    if (globalLimitID == 0) {
        if ($("#StartDate_0").val() == "") {
            MakeTagFlashable('#StartDate_0');
        }
        if ($("#EndDate_0").val() == "") {
            MakeTagFlashable('#EndDate_0');
        }
        AddFlashingEffect();
    }
    if ($("#GL_" + globalLimitID + " .temp").length > 0) {
        DisplayMessage("Please review the fields highlighted in Red.", true, globalLimitID);
        return false;
    }
    return true;
}

//Convert simple properties to Knockout observable properties 
var BindObservableToModelProperty = function (data) {
    this.StartDate = data.StartDate;
    this.EndDate = data.EndDate;
    this.GlobalLimitID = data.GlobalLimitID;
    this.LstGlobalLimitDetails = data.LstGlobalLimitDetails;    
}

var GetEditedData = function (globalLimitID) {
    var $globalLimitEditedRows;
    $globalLimitEditedRows = $("#GL_" + globalLimitID + " tr[data-edit='1']");    
    
    var lstGlobalDetails = new Array();

    $globalLimitEditedRows.each(function () {
        var objGlobalDetail = new Object();
        objGlobalDetail.GlobalDetailsID = $(this).attr("id");
        objGlobalDetail.CarClassID = $(this).attr("data_class");
        objGlobalDetail.CarClass = $(this).find("td.carclass")[0].innerHTML;
        objGlobalDetail.DayMin = $(this).find("input[name='DayMin']")[0].value;
        objGlobalDetail.DayMax = $(this).find("input[name='DayMax']")[0].value;
        objGlobalDetail.MonthlyMax = $(this).find("input[name='MonthlyMax']")[0].value;
        objGlobalDetail.MonthlyMin = $(this).find("input[name='MonthlyMin']")[0].value;
        objGlobalDetail.WeekMax = $(this).find("input[name='WeekMax']")[0].value;
        objGlobalDetail.WeekMin = $(this).find("input[name='WeekMin']")[0].value;
        
        lstGlobalDetails.push(objGlobalDetail);        
    });

    return lstGlobalDetails;
}

var SaveGlobalLimit = function (objGlobalLimit) {   
    var emptyTextboxes = $("#GL_" + objGlobalLimit.GlobalLimitID + " tbody input[type='text']").filter(function () { return this.value == ""; });
    var totalTextboxes = $("#GL_" + objGlobalLimit.GlobalLimitID + " tbody input[type='text']");
    
    if (emptyTextboxes.length == totalTextboxes.length) {
        DisplayMessage("Please enter values.", true, objGlobalLimit.GlobalLimitID);
        return;
    }

    var objNewGlobalLimit = new Object();
    objNewGlobalLimit.LstGlobalLimitDetails = GetEditedData(objGlobalLimit.GlobalLimitID);
    objNewGlobalLimit.CreatedBy = $("#LoggedInUserId").val();
    objNewGlobalLimit.BrandLocation = locationBrandID;
    objNewGlobalLimit.GlobalLimitID = objGlobalLimit.GlobalLimitID;
    if (objGlobalLimit.GlobalLimitID == 0) {
        objNewGlobalLimit.StartDate = $("#StartDate_0").val();
        objNewGlobalLimit.EndDate = $("#EndDate_0").val();
    }
    if (objNewGlobalLimit.LstGlobalLimitDetails.length > 0) {
        SaveGlobalLimitInDB(objNewGlobalLimit);
    }
    //else {
    //    DisplayMessage("Please enter min/max values to save", true, objGlobalLimit.GlobalLimitID);
    //}
}

var ValidateTextBoxValue = function (textbox) {
    var regExp = new RegExp(/^\d{1,4}(\.\d{1,2})?$/);
    if (($.isNumeric(textbox.value) && parseFloat(textbox.value) > -1 )|| textbox.value == "") {
        var MaxValue = 0;
        var MinValue = 0;
        switch (textbox.name) {
            case "DayMin":
                MaxValue = $("#DayMax" + String(textbox.id).replace("DayMin", "")).val();
                MinValue = textbox.value;
                if (MaxValue == "" || MinValue == "" || parseFloat(MinValue) <= parseFloat(MaxValue)) {
                    if ($.isNumeric(MaxValue) && parseFloat(MaxValue) > -1) {
                        RemoveFlashableTag($("#DayMax" + String(textbox.id).replace("DayMin", "")));
                    }
                    return true;
                }
                else {
                    MakeTagFlashable($("#DayMax" + String(textbox.id).replace("DayMin", "")));
                }
                break;
            case "DayMax":
                MaxValue = textbox.value;
                MinValue = $("#DayMin" + String(textbox.id).replace("DayMax", "")).val();
                if (MaxValue == "" || MinValue == "" || parseFloat(MinValue) <= parseFloat(MaxValue)) {
                    if ($.isNumeric(MinValue) && parseFloat(MinValue) > -1) {
                        RemoveFlashableTag($("#DayMin" + String(textbox.id).replace("DayMax", "")));
                    }
                    return true;
                }
                else {
                    MakeTagFlashable($("#DayMin" + String(textbox.id).replace("DayMax", "")));
                }
                break;
            case "WeekMin":
                MaxValue = $("#WeekMax" + String(textbox.id).replace("WeekMin", "")).val();
                MinValue = textbox.value;
                if (MaxValue == "" || MinValue == "" || parseFloat(MinValue) <= parseFloat(MaxValue)) {
                    if ($.isNumeric(MaxValue) && parseFloat(MaxValue) > -1) {
                        RemoveFlashableTag($("#WeekMax" + String(textbox.id).replace("WeekMin", "")));
                    }
                    return true;
                }
                else {
                    MakeTagFlashable($("#WeekMax" + String(textbox.id).replace("WeekMin", "")));
                }
                break;
            case "WeekMax":
                MaxValue = textbox.value;
                MinValue = $("#WeekMin" + String(textbox.id).replace("WeekMax", "")).val();                
                if (MaxValue == "" || MinValue == "" || parseFloat(MinValue) <= parseFloat(MaxValue)) {
                    if ($.isNumeric(MinValue) && parseFloat(MinValue) > -1) {
                        RemoveFlashableTag($("#WeekMin" + String(textbox.id).replace("WeekMax", "")));
                    }
                    return true;
                }
                else {
                    MakeTagFlashable($("#WeekMin" + String(textbox.id).replace("WeekMax", "")));
                }
                break;
            case "MonthlyMin":
                MaxValue = $("#MonthlyMax" + String(textbox.id).replace("MonthlyMin", "")).val();
                MinValue = textbox.value;
                if (MaxValue == "" || MinValue == "" || parseFloat(MinValue) <= parseFloat(MaxValue)) {
                    if ($.isNumeric(MaxValue) && parseFloat(MaxValue) > -1) {
                        RemoveFlashableTag($("#MonthlyMax" + String(textbox.id).replace("MonthlyMin", "")));
                    }
                    return true;
                }
                else {
                    MakeTagFlashable($("#MonthlyMax" + String(textbox.id).replace("MonthlyMin", "")));
                }
                break;
            case "MonthlyMax":
                MaxValue = textbox.value;
                MinValue = $("#MonthlyMin" + String(textbox.id).replace("MonthlyMax", "")).val();
                if (MaxValue == "" || MinValue == "" || parseFloat(MinValue) <= parseFloat(MaxValue)) {
                    if ($.isNumeric(MinValue) && parseFloat(MinValue) > -1) {
                        RemoveFlashableTag($("#MonthlyMin" + String(textbox.id).replace("MonthlyMax", "")));
                    }
                    return true;
                }
                else {
                    MakeTagFlashable($("#MonthlyMin" + String(textbox.id).replace("MonthlyMax", "")));
                }
                break;
        }        
        return false;
    }
    return false;
}

//Show the success/error messages
var DisplayMessage = function (message, isError, id) {
    $("#lblMessage_" + id).html(message).show().css("color", isError ? "red" : "green");
    if (!isError) {
        setTimeout(function () { $("#lblMessage_" + id).hide(); }, 3000);
    }
}

//Update Edited global limit in obseravable array
var UpdateEditedGlobalLimit = function (objGlobalLimit) {
    ko.utils.arrayForEach(globalLimitModel.globallimits(), function (currentGlobalLimit) {
        if (currentGlobalLimit.GlobalLimitID == objGlobalLimit.GlobalLimitID) {
            //currentGlobalLimit.LstGlobalLimitDetails = objGlobalLimit.LstGlobalLimitDetails();
            for (var i = 0; i < objGlobalLimit.LstGlobalLimitDetails.length; i++) {
                var result = $.grep(currentGlobalLimit.LstGlobalLimitDetails, function (e) { return e.CarClassID == objGlobalLimit.LstGlobalLimitDetails[i].CarClassID; });
                if (result.length == 1) {
                    result[0].DayMin = objGlobalLimit.LstGlobalLimitDetails[i].DayMin;
                    result[0].DayMax = objGlobalLimit.LstGlobalLimitDetails[i].DayMax;
                    result[0].WeekMin = objGlobalLimit.LstGlobalLimitDetails[i].WeekMin;
                    result[0].WeekMax = objGlobalLimit.LstGlobalLimitDetails[i].WeekMax;
                    result[0].MonthlyMin = objGlobalLimit.LstGlobalLimitDetails[i].MonthlyMin;
                    result[0].MonthlyMax = objGlobalLimit.LstGlobalLimitDetails[i].MonthlyMax;
                }
            }            
        }
    });
}

var SetDatePickerEvent = function () {
    $('#StartDate_0').datepicker({
        minDate: 0,
        dateFormat: 'mm/dd/yy',
        numberOfMonths: 4,
        onSelect: function (selectedDate, instance) {
            $(this).val(selectedDate);
            $("#EndDate_0").datepicker('option', { defaultDate: selectedDate, minDate: selectedDate });
            RemoveFlashableTag($(this));
        }
    });

    $("#EndDate_0").datepicker({
        numberOfMonths: 4,
        dateFormat: 'mm/dd/yy',
        onSelect: function (selectedDate, instance) {
            $(this).val(selectedDate);
            RemoveFlashableTag($(this));
        }
    });

    $(".globaldetails input[type='text']").on({
        keyup: function () {
            $($(this).closest("tr")).attr("data-edit", '1');
            $(this).closest(".globallimits").find("input[type='button'][value='Save']").eq(0).removeClass("btnDisabled").prop("disabled", false);
            if (!ValidateTextBoxValue(this)) {
                MakeTagFlashable('#' + this.id);
            }
            else {
                RemoveFlashableTag('#' + this.id);
            }
            AddFlashingEffect();
        },
        change: function () {
            $($(this).closest("tr")).attr("data-edit", '1');
            $(this).closest(".globallimits").find("input[type='button'][value='Save']").eq(0).removeClass("btnDisabled").prop("disabled", false);
            if (!ValidateTextBoxValue(this)) {
                MakeTagFlashable('#' + this.id);
            }
            else {
                RemoveFlashableTag('#' + this.id);
            }
            AddFlashingEffect();
        }
    });

}

var GetGlobalLimit = function (locationBrandID) {
    var ajaxURl = '/RateShopper/GlobalLimit/GetGlobalLimits/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetGlobalLimits;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        data: JSON.stringify({ 'brandLocationID': locationBrandID }),
        success: function (data) {
            //var srcs = $.map(data, function (item) { return new BindObservableToModelProperty(item); });
            globalLimitModel.globallimits(data);
            $("#spanBrandName_0").html($("#dimension-source li").eq(0).html());
            setTimeout(SetDatePickerEvent, 250);            
        },
        error: function (e) {
            //called when there is an error
            console.log("GetGlobalLimit: " + e.message);
        }
    });
}

var DeleteGlobalLimit = function (objGlobalLimit) {
    if (typeof (objGlobalLimit) == 'undefined') {
        objGlobalLimit = this;
    }
    var ajaxURl = '/RateShopper/GlobalLimit/DeleteGlobalLimit/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.DeleteGlobalLimit;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        data: JSON.stringify({ 'globalLimitID': objGlobalLimit.GlobalLimitID }),
        success: function (data) {
            globalLimitModel.globallimits.remove(objGlobalLimit);
            //DisplayMessage("Global limit deleted successfully", false);                
            $("#lblMessage").css({ "color": "green" }).html("Global limit deleted successfully");
            $("#lblMessage").show();
            setTimeout(function () { $("#lblMessage").hide(); }, 3000);
            $(window).scrollTop(0);

        },
        error: function (e) {
            //called when there is an error
            console.log("DeleteGlobalLimit: " + e.message);
        }
    });
}

var SaveGlobalLimitInDB = function (objGlobalLimit) {
    var ajaxURl = '/RateShopper/GlobalLimit/SaveGlobalLimit/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.SaveGlobalLimit;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        data: JSON.stringify({ 'objGlobalLimit': objGlobalLimit }),
        success: function (data) {
            if (data == "0") {
                DisplayMessage("The Global Limit exists for the selected date range.", true, objGlobalLimit.GlobalLimitID);
            }
            else if (data != null) {
                if (objGlobalLimit.GlobalLimitID > 0) {
                    UpdateEditedGlobalLimit(data);
                    $($("#GL_" + objGlobalLimit.GlobalLimitID + " tr[data-edit='1']")).each(function () { $(this).attr("data-edit", '0'); });
                    DisplayMessage("The Global Limits saved successfully.", false, objGlobalLimit.GlobalLimitID);
                    setTimeout(function () { $("#lblMessage_" + objGlobalLimit.GlobalLimitID).hide(); }, 3000);
                }
                else {
                    globalLimitModel.globallimits(data);
                    $("#lblMessage").css({ "color": "green" }).html("Global limit saved successfully");
                    $("#lblMessage").show();
                    setTimeout(function () { $("#lblMessage").hide(); }, 3000);
                    $(window).scrollTop(0);

                    setTimeout(SetDatePickerEvent, 250);
                }                
            }
        },
        error: function (e) {
            //called when there is an error
            console.log("SaveGlobalLimit: " + e.message);
        }
    });
}