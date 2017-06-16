var sortOrder = "ASC";
var sortRateCodesBy = "Code";
var isFormModified = false;
var isSortingApplied = false;
var isSupportedBrandsModified = false;
var rateCodeID = 0;
var rateCodeDateRangeID = 0;
var rateCodeManagementModel;
var editedDateRangeObject;
var deletedDateRangeIds = new Array();

$(document).ready(function () {
    rateCodeManagementModel = new RateCodeManagementModel();
    ko.applyBindings(rateCodeManagementModel);
    GetRateCodes()
    $("#btnAddRateCode").click(ResetForm);
    $("#btnCancel").click(function () { CancelClick(); });
    $("#btnSaveRateCode").click(SaveRateCode);
    $("#btnApply").click(function () {
        if ($("#RatecodeStartDate").val().length && $("#RatecodeEndDate").val().length) {
            if (ValidateDateRange()) {
                BindDateRange();
                isFormModified = true;
                EnableDisableButton(true);
            }
            else {
                ShowConfirmBox('Enter unique Date Range', false);
            }
        }
        else {
            ShowConfirmBox('Start Date or End Date can not be empty', false);
        }
    });

    var ValidateDateRange = function () {
        var flag = true;
        var checkStartDate = new Date($("#RatecodeStartDate").val());
        var checkEndDate = new Date($("#RatecodeEndDate").val());
        ko.utils.arrayForEach(rateCodeManagementModel.rateCodeDateRange(), function (objRDateRange) {
            var from = new Date(objRDateRange.StartDate());
            var to = new Date(objRDateRange.EndDate());
            if (typeof (editedDateRangeObject) == 'undefined' || (typeof (editedDateRangeObject) != 'undefined' && ((objRDateRange.ID == 0 && objRDateRange.randomID != editedDateRangeObject.randomID) || objRDateRange.ID != editedDateRangeObject.ID))) {
                if ((checkStartDate >= from && checkStartDate <= to) || (checkEndDate >= from && checkEndDate <= to)) {
                    flag = false;
                    return flag;
                }
                if ((from > checkStartDate && to < checkEndDate)) {
                    flag = false;
                    return flag;
                }
            }
        });
        return flag;
    }

    $('#searchRateCode').bind('keyup', function () {
        RateCodeSmartSearch();
    });
    $("#searchRateCode").focus();
    $("#lmform input[type=text]").change(function () {
        EnableDisableButton(true); isFormModified = true;
    });
    $("#lmform input[type=text]").bind("input", function () {
        EnableDisableButton(true); isFormModified = true;
    });
    $("#divSupportedBrands input[type='checkbox']").bind("change", function () {
        EnableDisableButton(true); isFormModified = true;
    });
    $("#span_isActive input[type='checkbox']").bind("change", function () {
        EnableDisableButton(true); isFormModified = true;
    });


    initializeRatecodeDatePicker();
});

$(document).ajaxStart(function () { $(".loader_container_main").show(); });
$(document).ajaxComplete(function () { $(".loader_container_main").hide(); });

function RateCodeManagementModel() {
    var self = this;

    self.rateCodes = ko.observableArray([]);

    self.rateCodeDateRange = ko.observableArray([]);

    self.DeleteRateCode = function (ratecode, event) {
        var message = "Do you want to delete the <b>" + ratecode.Code.toUpperCase() + "</b> rate code?";
        ShowConfirmBox(message, true, DeleteRateCode, ratecode);
        event.stopPropagation();
    }

    self.EditRateCode = function (ratecode) {
        if (isFormModified) {
            var message = "Are you sure you want to discard the changes?";
            ShowConfirmBox(message, true, EditRateCode, ratecode);
        }
        else {
            EditRateCode(ratecode);
        }
    }

    self.EditRateCodeDateRange = function (dateRange) {
        EditRateCodeDateRange(dateRange);
    }

    self.DeleteRateCodeDateRange = function (dateRange, event) {
        var message = "Do you want to delete this date range ?";
        ShowConfirmBox(message, true, DeleteRateCodeDateRange, dateRange);
        event.stopPropagation();
    }

    self.SortRateCode = function () {
        switch (sortRateCodesBy) {
            case "Code":
                if (sortOrder == "DESC") {
                    self.rateCodes.sort(function (left, right) {
                        return left.Code.toLowerCase() == right.Code.toLowerCase() ? 0 : (left.Code.toLowerCase() < right.Code.toLowerCase() ? 1 : -1)
                    });
                }
                else {
                    self.rateCodes.sort(function (left, right) {
                        return left.Code.toLowerCase() == right.Code.toLowerCase() ? 0 : (left.Code.toLowerCase() < right.Code.toLowerCase() ? -1 : 1)
                    });
                }
                break;

            case "Name":
                if (sortOrder == "DESC") {
                    self.rateCodes.sort(function (left, right) {
                        return left.Name.toLowerCase() == right.Name.toLowerCase() ? (left.Name.toLowerCase() < right.Name.toLowerCase() ? 1 : -1) :
                            (left.Name.toLowerCase() < right.Name.toLowerCase() ? 1 : -1)
                    });
                }
                else {
                    self.rateCodes.sort(function (left, right) {
                        return left.Name.toLowerCase() == right.Name.toLowerCase() ? (left.Name.toLowerCase() < right.Name.toLowerCase() ? -1 : 1) :
                            (left.Name.toLowerCase() < right.Name.toLowerCase() ? -1 : 1)
                    });
                }
                break;
        }
    }
}

function ratecode(data) {
    var self = this;
    self.ID = data.ID;
    self.Description = data.Description;
    self.Name = data.Name;
    self.Code = data.Code;
    self.SupportedBrandIDs = data.SupportedBrandIDs;
    self.DateRangeList = data.DateRangeList;
}
//Entity of Rate Code Date Range
function rateCodeDateRangeModel(data) {
    var self = this;
    self.ID = data.ID;
    self.RateCodeID = data.rateCodeID;

    if (data.StartDate.indexOf("Date") == -1) {
        self.StartDate = ko.observable(data.StartDate);
        self.EndDate = ko.observable(data.EndDate);
    }
    else {
        var sDate = new Date(parseInt(data.StartDate.replace("/Date(", "").replace(")/", "")));
        self.StartDate = ko.observable((parseInt(sDate.getMonth()) + 1) + "/" + sDate.getDate() + "/" + sDate.getFullYear());

        var eDate = new Date(parseInt(data.EndDate.replace("/Date(", "").replace(")/", "")));
        self.EndDate = ko.observable((parseInt(eDate.getMonth()) + 1) + "/" + eDate.getDate() + "/" + eDate.getFullYear());
    }

    self.randomID = Math.floor(Math.random() * 101);
    self.isAdded = data.isAdded;
    self.isUpdated = data.isUpdated;
    self.isDeleted = false;
}

function CancelClick() {
    if (rateCodeID > 0 && isFormModified) {
        var message = "Do you want to reset the data?";
        ShowConfirmBox(message, true, CancelChangesAndResetForm);
    }
    else {
        ResetForm();
    }
}

function CancelChangesAndResetForm() {
    var rateCodeIDTemp = rateCodeID;
    ResetForm();
    $($("#rateCodeId_" + rateCodeIDTemp).closest("tr")).addClass("grey_bg");
    GetRateCode(rateCodeIDTemp, false);
    rateCodeIDTemp = 0;
}

function ResetForm() {
    $("#HaddingTitle").html("CREATE Rate Code");
    $("#lblMessage").hide();
    $("#spanCarClassError").hide();
    $("#tblRateCodes tbody tr").removeClass("grey_bg");
    $(".textfields input[type='text']").each(function () { $(this).val('') });
    $("#divSupportedBrands input[type='checkbox']:checked").each(function () { this.checked = false; });
    $("[id=chkbkIsActive]").prop("checked", true);
    $("#RatecodeEndDate").val("");
    $("#RatecodeStartDate").val("");
    //$("#tblRateCodeDateDetails").hide();
    rateCodeManagementModel.rateCodeDateRange([]);
    rateCodeID = 0;
    rateCodeDateRangeID = 0;
    deletedDateRangeIds = [];
    isFormModified = false;
    EnableDisableButton(false);
    $(".table-ul-right ul").removeClass("deactive");
    $(".textfields input[type='text'].temp").each(function () {
        RemoveFlashableTag(this);
    });
}

var EnableDisableButton = function (isEnable) {
    if (!isEnable) {
        $("#btnCancel").addClass("disable-button");
        $("#btnCancel").prop("disabled", true);
        $("#btnSaveRateCode").addClass("btnDisabled");
        $("#btnSaveRateCode").prop("disabled", true);
    }
    else {
        $("#btnCancel").removeClass("disable-button");
        $("#btnCancel").prop("disabled", false);
        $("#btnSaveRateCode").removeClass("btnDisabled");
        $("#btnSaveRateCode").prop("disabled", false);
    }
}

function GetRateCode(rateCodeID, showLoader) {
    var ajaxURl = '/RateShopper/RateCoed/GetRateCode/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetRateCode;
    }
    $.ajax({
        url: ajaxURl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        global: showLoader,
        data: JSON.stringify({ 'rateCodeID': rateCodeID }),
        success: function (data) {
            console.log(data);
            if (data != null && data.status == true) {
                PopulateRateCode(data.result);
            }
            else {
                DisplayMessage("Unable to edit the Rate Code", true);
            }
        },
        error: function (jqXHR, exception) {
            console.log("Exception: " + exception);
        }
    });
}

function SaveRateCode() {
    var selectedSupportedBrands = '';
    if ($("#divSupportedBrands input[type = 'checkbox']:checked").length > 0) {
        selectedSupportedBrands = $("#divSupportedBrands input[type = 'checkbox']:checked").map(function () { return String(this.id).replace("chkbxSupportedBrand_", ""); }).get();
    }
    if (selectedSupportedBrands.length <= 0) {
        ShowConfirmBox("Please select supported brands", false);
        return false;
    }
    var isactive = $("#chkbkIsActive").is(":checked");
    var objRateCode = new Object();
    objRateCode.Code = $.trim($("#txtCode").val()).toUpperCase();
    objRateCode.Name = $.trim($("#txtName").val()).toUpperCase();
    objRateCode.Description = $.trim($("#txtDescription").val());
    objRateCode.CreatedBy = $("#LoggedInUserId").val();
    //objRateCode.SupportedBrandIDs = $("#divSupportedBrands input[type = 'checkbox']:checked").map(function () { return String(this.id).replace("chkbxSupportedBrand_", ""); }).get();
    objRateCode.SupportedBrandIDs = selectedSupportedBrands.join();
    objRateCode.ID = rateCodeID;
    objRateCode.DateRangeList = CreateDateRangeList(rateCodeID);
    objRateCode.isActive = isactive;
    objRateCode.DeletedDateRangeIds = deletedDateRangeIds;

    if (ValidateObject(objRateCode) && IsDuplicateCodeExists(objRateCode)) {
        SaveRateCodeDetails(objRateCode);
    }
    else {
        if ($(".temp").length > 0) {
            DisplayMessage("Please review the fields highlighted in Red.", true);
            AddFlashingEffect();
        }
    }
}

function ValidateObject(object) {
    var isValid = true;
    for (var prop in object) {
        if (typeof (prop) != 'undefined' && typeof (object[prop]) != "object") {
            if (prop == "Description" || (object.hasOwnProperty(prop) && object[prop] !== "" && ValidateProperty(prop, object[prop]))) {
                isValid = isValid & true;
            }
            else if ($('.textfields input[type="text"]#txt' + prop) != null) {
                MakeTagFlashable('.textfields input[type="text"]#txt' + prop);
                $('.textfields input[type="text"]#txt' + prop).bind("keyup", function () {
                    RemoveFlashableTag(this);
                    $(this).unbind("keyup");
                });
                isValid = false;
            }
        }
    }
    return isValid;
}

function ValidateProperty(property, value) {
    switch (String(property).toUpperCase()) {
        case "DESCRIPTION":
        case "CODE":
            var regExp = new RegExp("[<>]");
            if (regExp.test(value)) {
                return false;
            }
            break;
    }
    return true;
}

function IsDuplicateCodeExists(objRateCode) {
    var rateCode = ko.utils.arrayFirst(rateCodeManagementModel.rateCodes(), function (currentRateCode) {
        return (currentRateCode.Code.toLowerCase() == objRateCode.Code.toLowerCase() && currentRateCode.ID != objRateCode.ID);
    });
    if (rateCode) {
        DisplayMessage("The rate code already exists.", true);
        return false;
    }
    return true;
}

function SaveRateCodeDetails(objRateCode) {
    var ajaxURl = '/RateShopper/RateCode/SaveRateCode/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.SaveRateCode;
    }
    $.ajax({
        url: ajaxURl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        data: JSON.stringify({ 'objRateCodeDTO': objRateCode }),
        success: function (data) {
            if (data.ID > 0) {
                var rateCodeModel = new ratecode(data);
                if (rateCodeID == 0) {
                    rateCodeManagementModel.rateCodes.push(rateCodeModel);
                }
                else {
                    GetRateCodes();
                }
                RateCodeSmartSearch();
                if (isSortingApplied) {
                    rateCodeManagementModel.SortRateCode();
                }
                else if (rateCodeID == 0) {
                    $("#divListing").scrollTop($("#divListing").height());
                }
                ResetForm();
                DisplayMessage("The rate code saved successfully", false);
            }
            else if (data.LocationBrandID == 0) {
                DisplayMessage("The rate code already exist.", true);
            }
        },
        error: function (e) {
            //called when there is an error
            console.log("SaveLocation: " + e.message);
        }
    });
}

function PopulateRateCode(objRateCode) {
    $("#HaddingTitle").html("UPDATE / VIEW RATE CODE");
    $("#txtCode").val(objRateCode.Code);
    $("#txtDescription").val(objRateCode.Description);
    $("#txtName").val(objRateCode.Name);
    var SelectedSupportedIDs = 0

    if (objRateCode.SupportedBrandIDs != null) {
        SelectedSupportedIDs = objRateCode.SupportedBrandIDs.split(',');
    }
    $("#divSupportedBrands input[type='checkbox']:checked").prop("checked", false);

    for (var i = 0; i < SelectedSupportedIDs.length; i++) {
        if ($("#chkbxSupportedBrand_" + SelectedSupportedIDs[i]).length > 0) {
            $("#chkbxSupportedBrand_" + SelectedSupportedIDs[i]).prop("checked", true);
        }
    }

    $("[id=chkbkIsActive]").prop("checked", objRateCode.IsActive);

    rateCodeID = objRateCode.ID;
    $("#tblRateCodeDateDetails").show();

    var srcs = $.map(objRateCode.DateRangeList, function (item) { return new rateCodeDateRangeModel(item); });
    rateCodeManagementModel.rateCodeDateRange(srcs);
}

function EditRateCode(objRateCode) {
    if (typeof (objRateCode) == 'undefined') {
        objRateCode = this;
    }
    ResetForm();
    $($("#rateCodeId_" + objRateCode.ID).closest("tr")).addClass("grey_bg");
    GetRateCodeDetails(objRateCode.ID, true);
}

function DeleteRateCode(objRateCode) {
    if (typeof (objRateCode) == 'undefined') {
        objRateCode = this;
    }
    objRateCode.CreatedBy = $("#LoggedInUserId").val();
    ResetForm();
    DeleteRateCodeFromDB(objRateCode);
}

var BindDateRange = function () {
    if ($("#RatecodeStartDate").val() != '' && $("#RatecodeEndDate").val() != '') {
        if (rateCodeDateRangeID == 0) {
            if (typeof (editedDateRangeObject) == 'undefined') {
                //new entry added
                editedDateRangeObject = new Object();
                editedDateRangeObject.isAdded = true;
                editedDateRangeObject.isUpdated = false;
                editedDateRangeObject.StartDate = $("#RatecodeStartDate").val();
                editedDateRangeObject.EndDate = $("#RatecodeEndDate").val();
                editedDateRangeObject.ID = 0;
                editedDateRangeObject = new rateCodeDateRangeModel(editedDateRangeObject);
            }
            else if (typeof (editedDateRangeObject) != 'undefined' && editedDateRangeObject.randomID > 0)        //temporary updation
            {
                //editedDateRangeObject.isAdded = false;
                editedDateRangeObject.isUpdated = true;
                editedDateRangeObject.StartDate($("#RatecodeStartDate").val());
                editedDateRangeObject.EndDate($("#RatecodeEndDate").val());
                //editedDateRangeObject.ID = 0;
            }
        }
        else {
            editedDateRangeObject.isAdded = false;
            editedDateRangeObject.StartDate($("#RatecodeStartDate").val());
            editedDateRangeObject.EndDate($("#RatecodeEndDate").val());
            editedDateRangeObject.isUpdated = true;
            editedDateRangeObject.ID = rateCodeDateRangeID;
        }

        $("#tblRateCodeDateDetails").show();

        if (editedDateRangeObject.StartDate() != "" && editedDateRangeObject.EndDate() != "" && editedDateRangeObject.isAdded == true && editedDateRangeObject.isUpdated == false) {       //new entry added
            rateCodeManagementModel.rateCodeDateRange.push(editedDateRangeObject);
        }

        $("#RatecodeStartDate").val("");
        $("#RatecodeEndDate").val("");
        rateCodeDateRangeID = 0;
        editedDateRangeObject = undefined;
    }
    else {
        alert("The Date Ranges cannot be null");
    }
}

function EditRateCodeDateRange(objDateRange) {
    $("#RatecodeStartDate").val(objDateRange.StartDate());    
    $("#RatecodeEndDate").datepicker('option', { minDate: new Date(objDateRange.StartDate()) });
    $("#RatecodeEndDate").val(objDateRange.EndDate());
    rateCodeDateRangeID = objDateRange.ID;
    editedDateRangeObject = objDateRange;
}

function DeleteRateCodeDateRange(objRateCode) {
    if (typeof (objRateCode) == 'undefined') {
        objRateCode = this;
    }
    if (objRateCode.ID > 0) {
        deletedDateRangeIds.push(objRateCode.ID);
    }
    if ((typeof (editedDateRangeObject) != 'undefined' && (objRateCode.ID == editedDateRangeObject.ID && objRateCode.randomID == editedDateRangeObject.randomID))) {
        rateCodeDateRangeID = 0;
        editedDateRangeObject = undefined;
        $("#RatecodeStartDate").val("");
        $("#RatecodeEndDate").val("");
    }
    rateCodeManagementModel.rateCodeDateRange.remove(objRateCode);
    EnableDisableButton(true);
    isFormModified = true;
}

function DeleteRateCodeFromDB(objRateCode) {
    var ajaxURl = '/RateShopper/RateCode/DeleteRateCode/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.DeleteRateCode;
    }
    $.ajax({
        url: ajaxURl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        data: JSON.stringify({ 'rateCodeId': objRateCode.ID, 'userId': objRateCode.CreatedBy }),
        success: function (result) {
            if (result.status == true) {
                rateCodeManagementModel.rateCodes.remove(objRateCode);
                DisplayMessage("The rate Code deleted successfully.", false);
                RateCodeSmartSearch();
            }
            else {
                DisplayMessage("Unable to delete the rate code.", true);
            }
        },
        error: function (jqXHR, exception) {
            console.log("Exception: " + exception);
        }
    });
}

function RateCodeSmartSearch() {
    var $inpuTextSelector = $("#searchRateCode").val();
    if ($inpuTextSelector.length > 0) {
        $("#tblRateCodes tbody td[class='code']").each(function () {
            if ($.trim($(this).text()).toLowerCase().indexOf($inpuTextSelector.toLowerCase()) == 0) {
                $(this).closest("tr").show();
            }
            else {
                $(this).closest("tr").hide();
            }
        });
    } else {
        $("#tblRateCodes tbody tr").show();
    }

    if ($inpuTextSelector.length > 0 && $("#tblRateCodes tbody tr[style$='display: none;']:not(.remove_if_datafound)").length == $("#tblRateCodes tbody tr:not(.remove_if_datafound)").length) {
        MakeTagFlashable("#searchRateCode");
        if ($("#tblRateCodes tbody tr.remove_if_datafound").length == 0) {
            $("#tblRateCodes tbody").append("<tr class='remove_if_datafound'><td style='text-align:center;font-weight:bold;color:red;text-transform:none;' colspan='3'>No Rate Code found.</td></tr>");
            $("#ratecodeheaders").hide();
            $("#no_rows").show();
        }
    }
    else {
        RemoveFlashableTag("#searchRateCode");
        $("#tblRateCodes tbody tr.remove_if_datafound").remove();
        $("#ratecodeheaders").show();
        $("#no_rows").hide();
    }
    AddFlashingEffect();
}

function GetRateCodes() {
    var ajaxURl = '/RateShopper/RateCode/GetRateCodes/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetRateCodes;
    }
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        success: function (data) {
            if (data && data.status == true) {
                var srcs = $.map(data.result, function (item) { return new ratecode(item); });
                rateCodeManagementModel.rateCodes(srcs);
            }
        },
        error: function (jqXHR, exception) {
            console.log("Exception: " + exception);
        }
    });
}

function GetRateCodeDetails(rateCodeId, showLoader) {
    var ajaxURl = '/RateShopper/RateCode/GetRateCode/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetRateCode;
    }
    $.ajax({
        url: ajaxURl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        global: showLoader,
        data: JSON.stringify({ 'rateCodeId': rateCodeId }),
        success: function (data) {
            if (data && data.status == true) {
                PopulateRateCode(data.result);
            }
            else {
                DisplayMessage("Unable to edit the Rate Code", true);
            }
        },
        error: function (jqXHR, exception) {
            console.log("Exception: " + exception);
        }
    });
}

var DisplayMessage = function (message, isError) {
    $("#lblMessage").html(message).show().css("color", isError ? "red" : "green");
    if (!isError) {
        setTimeout(function () { $("#lblMessage").hide(); }, 3000);
    }
}

function ApplySorting(control, sortBy) {
    isSortingApplied = true;
    var prevSortOrder = $(control)[0].className;
    switch (sortBy) {
        case "Code":
            sortRateCodesBy = "Code";
            break;
        case "Name":
            sortRateCodesBy = "Name";
            break;
    }
    if (prevSortOrder == "Asc") {
        $(control).removeClass("Asc").addClass("Desc");
        sortOrder = "DESC";
    }
    else {
        $(control).removeClass("Desc").addClass("Asc");
        sortOrder = "ASC";
    }

    rateCodeManagementModel.SortRateCode();
}

function initializeRatecodeDatePicker() {
    $('#RatecodeStartDate.date-picker,#RatecodeStartDateimg').datepicker
        ({
            minDate: 0,
            numberOfMonths: 2,
            dateFormat: 'mm/dd/yy',
            onSelect: function (selectedDate) {
                var dSelectedDate = new Date(selectedDate);
                $('#RatecodeEndDate').datepicker('option', { defaultDate: selectedDate, minDate: dSelectedDate });
                var endDate = $('#RatecodeEndDate.date-picker').val();
                if (endDate == '') {
                    $("#RatecodeEndDate").datepicker("setDate", selectedDate);
                }
                else {
                    var endDateDetails = endDate.split('/');
                    var prevDate = new Date(parseInt(endDateDetails[2]), parseInt(endDateDetails[0]) - 1, parseInt(endDateDetails[1]));
                    if (dSelectedDate > prevDate) {
                        $("#RatecodeEndDate").datepicker("setDate", selectedDate);
                    }
                }
            }
        }).datepicker(new Date());

    $('#RatecodeEndDate.date-picker,#RatecodeEndDateimg').datepicker
        ({
            minDate: 0,
            numberOfMonths: 2,
            dateFormat: 'mm/dd/yy',
            onSelect: function (selectedDate) {
                var dSelectedDate = new Date(selectedDate);
                $('#RatecodeEndDate').datepicker('option', { defaultDate: selectedDate });
                $("#RatecodeEndDate").datepicker("setDate", selectedDate);
            }
        }).datepicker(new Date());

}


var CreateDateRangeList = function (rateCodeID) {
    var dateRangeList = new Array();
    var dateRange = null;
    ko.utils.arrayForEach(rateCodeManagementModel.rateCodeDateRange(), function (objRDateRange) {
        dateRange = new Object();
        dateRange.ID = objRDateRange.ID;
        dateRange.RateCodeID = rateCodeID;
        dateRange.isAdded = objRDateRange.isAdded;
        dateRange.isUpdated = objRDateRange.isUpdated;
        dateRange.isDeleted = objRDateRange.isDeleted;
        dateRange.StartDate = objRDateRange.StartDate();
        dateRange.EndDate = objRDateRange.EndDate();

        dateRangeList.push(dateRange);
    });
    return dateRangeList;
}

