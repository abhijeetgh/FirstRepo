/*
File: LocationManagement.js
Created By: Abhijeet Ghule
Functionality: This file includes functionality for the add/edit/delete brand locations.
*/


var isCarClassesModified = false;
//While editing location set below flag to false
var isRentalLengthsModified = true;
var locationManagementModel;
var locationID = 0;
var locationBrandID = 0;
var sortOrder = "ASC";
var sortLocationsBy = "Code";
var isFormModified = false;
var isSortingApplied = false;
var allCompanies;
var OriginalDirectCompetitor = "";

$(document).ready(LocationManagmentDocumentReady);

//Show loader before every ajax call
$(document).ajaxStart(function () { $(".loader_container_main").show(); });
//Hide loader before every ajax call
$(document).ajaxComplete(function () { $(".loader_container_main").hide(); });

//This function initiates on Document ready
function LocationManagmentDocumentReady() {
    locationManagementModel = new LocationManagementModel();
    //Apply knockout binding
    ko.applyBindings(locationManagementModel);
    BindLocations();
    getCompanies();
    $("#btnAddLocation").click(ResetForm);
    $("#btnCancel").click(function () { CancelClick(); });
    $("#btnSaveLocation").click(SaveLocation);
    $("#chkUse-LOR").change(ShowHideRentalLengths);
    $("#divCarClasses input[type='checkbox']").bind("change", function () {
        isCarClassesModified = true;
        if ($(this).is(":checked") && $("#spanCarClassError").css("display") != "none") {
            $("#spanCarClassError").hide();
        }
    });

    $("#divRentalLengths #divRentalCheckboxes input[type='checkbox']").bind("change", function () {
        isRentalLengthsModified = true;
    });
    $("ul.hidden.drop-down1").find('li').eq(0).addClass('selected').closest('.dropdown').find('li').eq(0).attr({ 'value': ($('ul.hidden.drop-down1').find('li').eq(0).attr('value')) }).text($("ul.hidden.drop-down1").find('li').eq(0).text());

    //Bind Location Search keyup event
    $('#searchLocation').bind('keyup', function () {
        LocationSmartSearch();

    });

    $("#divRentalLengths #divRentalCheckboxes input:checkbox").on("change", function () {
        if (($(this).prop("checked"))) {
            var associatedId = $(this).attr("associatedId");
            if (associatedId != "") {
                $("#chkRL_" + associatedId).attr("checked", false);
            }
            var id = ($(this).attr("id")).match(/\d+/);
            $("#divRentalLengths #divRentalCheckboxes input:checkbox[associatedId='" + id + "']").prop("checked", false);
        }
    })

    $("#searchLocation").focus();
    $("#lmform input[type=text], #lmform input[type=checkbox]").change(function () { EnableDisableButton(true); isFormModified = true; });
    $("#lmform input[type=text]").bind("input", function () { EnableDisableButton(true); isFormModified = true; });
    $("#ddlBrand-loc li").click(function () { EnableDisableButton(true); isFormModified = true; });

    $("#competitorCompanyIds").click(function () { EnableDisableButton(true); isFormModified = true; });
    $("#ignoreCompanyIds").click(function () { EnableDisableButton(true); isFormModified = true; });

    $('#dimension-source ul li').click(function () {
        filterBrand($(this).attr('id'));
    });
    $("#LocationPriceMgr").html("Not Assgined");
}

//Location smart search
var LocationSmartSearch = function () {
    var $inpuTextSelector = $("#searchLocation").val();
    if ($inpuTextSelector.length > 0) {
        $("#tblLocations tbody td[class='code']").each(function () {
            if ($.trim($(this).text()).toLowerCase().indexOf($inpuTextSelector.toLowerCase()) == 0) {
                $(this).closest("tr").show();
            }
            else {
                $(this).closest("tr").hide();
            }
        });
    } else {
        $("#tblLocations tbody tr").show();
    }

    if ($inpuTextSelector.length > 0 && $("#tblLocations tbody tr[style$='display: none;']:not(.remove_if_datafound)").length == $("#tblLocations tbody tr:not(.remove_if_datafound)").length) {
        MakeTagFlashable("#searchLocation");
        if ($("#tblLocations tbody tr.remove_if_datafound").length == 0) {
            $("#tblLocations tbody").append("<tr class='remove_if_datafound'><td style='text-align:center;font-weight:bold;color:red;text-transform:none;' colspan='3'>No location found.</td></tr>");
            $("#locationheaders").hide();
            $("#no_rows").show();
        }
    }
    else {
        RemoveFlashableTag("#searchLocation");
        $("#tblLocations tbody tr.remove_if_datafound").remove();
        $("#locationheaders").show();
        $("#no_rows").hide();
    }
    AddFlashingEffect();
}

//Knockout js variables
var LocationManagementModel = function () {
    var self = this;
    self.locations = ko.observableArray([]);
    self.DeleteLocation = function (location, event) {
        var message = "Do you want to delete the <b>" + location.LocationBrandAlias().toUpperCase() + "</b> brand location?";
        ShowConfirmBox(message, true, DeleteLocation, location);
        event.stopPropagation();
    }
    self.EditLocation = function (location) {
        if (isFormModified) {
            var message = "Are you sure you want to discard the changes?";
            ShowConfirmBox(message, true, EditLocation, location);
        }
        else {
            EditLocation(location);
        }
    }

    self.SortLocation = function () {
        switch (sortLocationsBy) {
            case "Code":
                if (sortOrder == "DESC") {
                    self.locations.sort(function (left, right) {
                        return left.LocationBrandAlias().toLowerCase() == right.LocationBrandAlias().toLowerCase() ? 0 : (left.LocationBrandAlias().toLowerCase() < right.LocationBrandAlias().toLowerCase() ? 1 : -1)
                    });
                }
                else {
                    self.locations.sort(function (left, right) {
                        return left.LocationBrandAlias().toLowerCase() == right.LocationBrandAlias().toLowerCase() ? 0 : (left.LocationBrandAlias().toLowerCase() < right.LocationBrandAlias().toLowerCase() ? -1 : 1)
                    });
                }
                break;

            case "Description":
                if (sortOrder == "DESC") {
                    self.locations.sort(function (left, right) {
                        return left.Description().toLowerCase() == right.Description().toLowerCase() ? (left.LocationBrandAlias().toLowerCase() < right.LocationBrandAlias().toLowerCase() ? 1 : -1) : (left.Description().toLowerCase() < right.Description().toLowerCase() ? 1 : -1)
                    });
                }
                else {
                    self.locations.sort(function (left, right) {
                        return left.Description().toLowerCase() == right.Description().toLowerCase() ? (left.LocationBrandAlias().toLowerCase() < right.LocationBrandAlias().toLowerCase() ? -1 : 1) : (left.Description().toLowerCase() < right.Description().toLowerCase() ? -1 : 1)
                    });
                }
                break;
        }
    }

    self.companies = ko.observableArray([]);
}

//This method is used to identify order of sorting
function ApplySorting(control, sortBy) {
    isSortingApplied = true;
    var prevSortOrder = $(control)[0].className;
    switch (sortBy) {
        case "Code":
            sortLocationsBy = "Code";
            break;
        case "Description":
            sortLocationsBy = "Description";
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

    locationManagementModel.SortLocation();
}

//Convert simple properties to Knockout observable properties 
var BindObservableToModelProperty = function (data) {
    this.ID = data.ID;
    this.Code = data.Code != null ? ko.observable(data.Code) : ko.observable("");
    this.LocationBrandID = data.LocationBrandID;
    this.Description = data.Description != null ? ko.observable(data.Description) : ko.observable("");
    this.WeeklyExtraDenominator = ko.observable(data.WeeklyExtraDenominator);
    this.DailyExtraDayFactor = ko.observable(data.DailyExtraDayFactor);
    this.TSDCustomerNumber = ko.observable(data.TSDCustomerNumber);
    this.TSDPassCode = ko.observable(data.TSDPassCode);
    this.RentalLengths = data.RentalLengths;
    this.CarClasses = data.CarClasses;
    this.UseLORRateCode = data.UseLORRateCode;
    this.BrandID = data.BrandID;
    this.LocationBrandAlias = ko.observable(data.LocationBrandAlias);
    this.CreatedBy = data.CreatedBy;
}

//Show hide different LORs on checkbox. Default are D1-W7
var ShowHideRentalLengths = function () {
    if ($("#chkUse-LOR").is(":checked")) {
        $("#divRentalLengths").show();
    }
    else {
        $("#divRentalLengths").hide();
    }
}

//Fetch the brand location data from the form, validate it then save
var SaveLocation = function () {
    //get selected competitor 
    var selectedCompetitors = '';
    var quickViewCompetitors = '';
    if ($('#competitorCompanyIds option:selected').length > 0) {
        selectedCompetitors = $('#competitorCompanyIds').val().toString();
    }
    if ($('#ignoreCompanyIds option').length > 0 && !$('#ignoreCompanyIds').prop('disabled')) {
        if ($('#ignoreCompanyIds option:selected').length > 0) {
            $('#ignoreCompanyIds option:selected').each(function (index, element) {
                if (index == $('#ignoreCompanyIds option:selected').length - 1) {
                    quickViewCompetitors += $(this).attr('value').trim();
                }
                else {
                    quickViewCompetitors += $(this).attr('value').trim() + ',';
                }
            });
        }
    }

    $("#lblMessage").hide();
    var objLocation = new Object();
    if (locationID > 0 && locationBrandID > 0) {
        objLocation.ID = locationID;
        objLocation.LocationBrandID = locationBrandID;
    }
    objLocation.Code = $.trim($("#txtCode-loc").val()).toUpperCase();
    objLocation.BranchCode = $.trim($("#txtBranchCode-loc").val()).toUpperCase();
    objLocation.Description = $.trim($("#txtDescription-loc").val());
    objLocation.WeeklyExtraDenominator = $.trim($("#txtWeeklyExtraDenominator-loc").val());
    objLocation.DailyExtraDayFactor = $.trim($("#txtDailyExtraDayFactor-loc").val());
    objLocation.TSDCustomerNumber = $.trim($("#txtTSDCustomerNumber-loc").val());
    objLocation.TSDPassCode = $.trim($("#txtTSDPassCode-loc").val());
    objLocation.RentalLengths = $("#divRentalLengths #divRentalCheckboxes input[type='checkbox']:checked").map(function () { return String(this.id).replace("chkRL_", ""); }).get();
    objLocation.CarClasses = $("#divCarClasses input[type='checkbox']:checked").map(function () { return String(this.id).replace("chkCC_", ""); }).get();
    objLocation.UseLORRateCode = $("#chkUse-LOR").prop("checked");
    objLocation.BrandID = $("#ddlBrand-loc").find("li.selected").attr("id");
    objLocation.LocationBrandAlias = objLocation.Code + "-" + $("#ddlBrand-loc").find("li.selected").attr("value");
    objLocation.CreatedBy = $("#LoggedInUserId").val();
    //configure competitor companyids and quickViewCompetitorids
    var DeletedCompanyID = "";
    if ($("#competitorCompanyIds").attr("selectedcompanyids") != undefined && $("#competitorCompanyIds").attr("selectedcompanyids") != '') {
        DeletedCompanyID = OriginalDirectCompetitor.split(',').filter(function (obj) { return $("#competitorCompanyIds").val().indexOf(obj) == -1; });
    }
    objLocation.DeletedCompetitorCompanyIds = DeletedCompanyID.toString();
    objLocation.CompetitorCompanyIds = selectedCompetitors;
    objLocation.QuickViewCompetitors = quickViewCompetitors;

    objLocation.ApplyDependantBrandLOR = $("#applyDependantLOR").prop("checked");

    if (ValidateObject(objLocation) && IsDuplicateCodeExists(objLocation)) {
        if (objLocation.DeletedCompetitorCompanyIds != undefined && objLocation.DeletedCompetitorCompanyIds != "") {
            $(window).scrollTop(0);
            ShowConfirmBox('Changing direct competitors list would affect rulesets groups. Do you want to continue?', true, SaveDirectCompetitorConfrimMsg, objLocation);
        }
        else {
            SaveLocationDetails(objLocation);
        }

    }
    else {
        if ($(".temp").length > 0) {
            DisplayMessage("Please review the fields highlighted in Red.", true);
            AddFlashingEffect();
        }
    }
}
function SaveDirectCompetitorConfrimMsg() {
    var objLocation = this;
    SaveLocationDetails(objLocation);
}

//Validate duplicate code
var IsDuplicateCodeExists = function (objLocation) {
    var location = ko.utils.arrayFirst(locationManagementModel.locations(), function (currentLocation) {
        return (currentLocation.LocationBrandAlias().toLowerCase() == objLocation.LocationBrandAlias.toLowerCase() && currentLocation.ID != objLocation.ID);
    });

    if (location) {
        DisplayMessage("The location code already exists.", true);
        return false;
    }
    return true;
}

//Delete brand location
var DeleteLocation = function (objLocation) {
    if (typeof (objLocation) == 'undefined') {
        objLocation = this;
    }
    objLocation.CreatedBy = $("#LoggedInUserId").val();
    //Reset the edited/add mode data
    ResetForm();
    DeleteLocationFromDB(objLocation);
}

//Edit brand location functionality, highlight the selected row
var EditLocation = function (objLocation) {
    if (typeof (objLocation) == 'undefined') {
        objLocation = this;
    }
    ResetForm();
    $($("#locationBrand_" + objLocation.LocationBrandID).closest("tr")).addClass("grey_bg");
    GetLocation(objLocation.ID, objLocation.LocationBrandID, true);
}

//Populate the edited brand-location data
var PopulateLocationData = function (objLocation) {
    $("#HaddingTitle").html("UPDATE / VIEW LOCATION");
    $("#txtCode-loc").val(objLocation.Code);
    //$("#txtCode-loc").attr("disabled", true);
    $("#txtDescription-loc").val(objLocation.Description);
    $("#txtWeeklyExtraDenominator-loc").val(objLocation.WeeklyExtraDenominator);
    $("#txtDailyExtraDayFactor-loc").val(objLocation.DailyExtraDayFactor);
    $("#txtTSDCustomerNumber-loc").val(objLocation.TSDCustomerNumber);
    $("#txtTSDPassCode-loc").val(objLocation.TSDPassCode);
    $("#txtBranchCode-loc").val(objLocation.BranchCode);
    $("#LocationPriceMgr").html(objLocation.LocationPricingManagerName);

    $("#chkUse-LOR")[0].checked = objLocation.UseLORRateCode;

    locationID = objLocation.ID;
    locationBrandID = objLocation.LocationBrandID;

    ShowHideRentalLengths();
    //$("ul.hidden.drop-down1").find('li').removeClass("selected");
    $("ul.hidden.drop-down1 li").removeClass("selected");
    $("ul.hidden.drop-down1 li#" + objLocation.BrandID).eq(0).addClass('selected').closest('.dropdown').find("li").eq(0).attr({ 'value': ($('ul.hidden.drop-down1 li#' + objLocation.BrandID).eq(0).attr('value')) }).text($("ul.hidden.drop-down1 li#" + objLocation.BrandID).eq(0).text());
    $(".table-ul-right ul").unbind("click");
    $(".table-ul-right ul").addClass("deactive");

    $("#divRentalLengths #divRentalCheckboxes input[type='checkbox']:checked, #divCarClasses input[type='checkbox']:checked").each(function () { this.checked = false; });
    $("#applyDependantLOR").prop("checked", objLocation.ApplyDependantBrandLOR);

    for (var i = 0; i < objLocation.RentalLengths.length; i++) {
        if ($("#chkRL_" + objLocation.RentalLengths[i]).length > 0) {
            $("#chkRL_" + objLocation.RentalLengths[i])[0].checked = true;
        }
    }

    for (var i = 0; i < objLocation.CarClasses.length; i++) {
        if ($("#chkCC_" + objLocation.CarClasses[i]).length > 0) {
            $("#chkCC_" + objLocation.CarClasses[i])[0].checked = true;
        }
    }
    configureSelectList();
    if (objLocation.CompetitorCompanyIds != null && objLocation.CompetitorCompanyIds != '') {
        OriginalDirectCompetitor = objLocation.CompetitorCompanyIds;
        $("#competitorCompanyIds").attr('selectedcompanyids', objLocation.CompetitorCompanyIds);
        $("#competitorCompanyIds option").each(function () {
            if ($.inArray($(this).val(), (objLocation.CompetitorCompanyIds).split(',')) == -1) {
                $(this).prop("selected", false);
            }
            else {
                $(this).prop("selected", true);
            }
        });

    }

    if (objLocation.QuickViewCompetitors != null && objLocation.QuickViewCompetitors != '') {
        $("#ignoreCompanyIds").prop("disabled", false);
        $("#ignoreCompanyIds option").each(function () {
            $("#ignoreCompanyIds").attr('selectedcompanyids', objLocation.CompetitorCompanyIds);
            if ($.inArray($(this).val(), (objLocation.QuickViewCompetitors).split(',')) == -1) {
                $(this).prop("selected", false);
            }
            else {
                $(this).prop("selected", true);
            }
        });
    }

}

//Reset the location form
var ResetForm = function () {
    $("#HaddingTitle").html("CREATE LOCATION");
    $("#LocationPriceMgr").html("Not Assgined");
    $("#lblMessage").hide();
    $("#spanCarClassError").hide();
    $("#tblLocations tbody tr").removeClass("grey_bg");
    $(".textfields input[type='text']").each(function () { $(this).val('') });

    isCarClassesModified = false;
    isRentalLengthsModified = true;
    locationID = 0;
    locationBrandID = 0;
    isFormModified = false;
    OriginalDirectCompetitor = "";
    $("#chkUse-LOR").removeAttr("checked");
    $("#txtCode-loc").removeAttr("disabled");
    ShowHideRentalLengths();
    EnableDisableButton(false);
    $("#applyDependantLOR").prop("checked", false);

    $("#divRentalLengths #divRentalCheckboxes input[type='checkbox']").each(function () {
        //Checked only default D1-W7 checkboxes using mappedID in id attribute
        if (parseInt(String(this.id).replace("chkRL_", "")) > 7) {
            this.checked = false;
        }
        else {
            this.checked = true;
        }
    });

    $(".textfields input[type='text'].temp,#competitorCompanyIds,#ignoreCompanyIds").each(function () {
        RemoveFlashableTag(this);
    });
    //Reset the car classes checkboxes
    $("#divCarClasses input[type='checkbox']:checked").each(function () { this.checked = false; });

    //Reset Brand dropdown
    //$("ul.hidden.drop-down1").find('li').removeClass("selected");
    $("ul.hidden.drop-down1 li").removeClass("selected");
    $("ul.hidden.drop-down1 li").eq(0).addClass('selected').closest('.dropdown').find('li').eq(0).attr({ 'value': ($('ul.hidden.drop-down1 li').eq(0).attr('value')) }).text($("ul.hidden.drop-down1 li").eq(0).text());

    //Rebind click of Brand dropdown
    $(".table-ul-right ul.LocationMgmtDropdown").unbind("click").bind("click", function () {
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

    $(".table-ul-right ul").removeClass("deactive");

    configureSelectList();

}

var CancelClick = function () {
    if (locationBrandID > 0 && isFormModified) {
        var message = "Do you want to reset the data?";
        ShowConfirmBox(message, true, CancelChangesAndResetForm);
    }
    else {
        ResetForm();
    }
}

function CancelChangesAndResetForm() {
    var tempLocationBrandID = 0;
    var tempLocationID = 0;
    tempLocationID = locationID;
    tempLocationBrandID = locationBrandID;
    ResetForm();
    $($("#locationBrand_" + tempLocationBrandID).closest("tr")).addClass("grey_bg");
    GetLocation(tempLocationID, tempLocationBrandID, false);
    tempLocationBrandID = 0;
    tempLocationID = 0;
}

//Update Edited location-description in obseravable array
var UpdateEditedLocation = function (objLocation) {
    ko.utils.arrayForEach(locationManagementModel.locations(), function (currentLocation) {
        if (currentLocation.LocationBrandID == objLocation.LocationBrandID) {
            currentLocation.Description(objLocation.Description());
            if (currentLocation.LocationBrandAlias() != objLocation.LocationBrandAlias()) {
                currentLocation.LocationBrandAlias(objLocation.LocationBrandAlias());

                ko.utils.arrayForEach(locationManagementModel.locations(), function (location) {
                    if (location.ID == objLocation.ID && location.LocationBrandID != objLocation.LocationBrandID) {
                        var alias = location.LocationBrandAlias();
                        var newBrandAlias = String(alias).substring(alias.indexOf("-"), alias.length);
                        location.LocationBrandAlias(objLocation.Code() + newBrandAlias);
                    }
                });
            }
            else {
                currentLocation.LocationBrandAlias(objLocation.LocationBrandAlias());
            }
        }
    });
}

//Validate the object
var ValidateObject = function (object) {
    var isValid = true;
    for (var prop in object) {
        if (typeof (prop) != 'undefined' && typeof (object[prop]) != "object") {
            if (prop == "BranchCode" || prop == "TSDCustomerNumber" || prop == "TSDPassCode" || prop == "DeletedCompetitorCompanyIds" || (object.hasOwnProperty(prop) && object[prop] !== "" && ValidateProperty(prop, object[prop]))) {
                isValid = isValid & true;
            }
            else if (prop == "CompetitorCompanyIds" || prop == "QuickViewCompetitors") {
                if ($.trim($('#competitorCompanyIds').attr('selectedCompanyids')) == '' && $.trim($('#ignoreCompanyIds').attr('selectedCompanyids')) == '') {
                    MakeTagFlashable($('select'));
                    isValid = false;
                }
                else if ($.trim($('#competitorCompanyIds').attr('selectedCompanyids')) == '') {
                    MakeTagFlashable($('#competitorCompanyIds'));
                    isValid = false;
                }
                else if ($.trim($('#ignoreCompanyIds').attr('selectedCompanyids')) == '') {
                    MakeTagFlashable($('#ignoreCompanyIds'));
                    isValid = false;
                }
                $('#competitorCompanyIds, #ignoreCompanyIds').on("change", function () {
                    if ($('#competitorCompanyIds').val() != '' && $('#ignoreCompanyIds').val() != '') {
                        RemoveFlashableTag($('#competitorCompanyIds'));
                        RemoveFlashableTag($('#ignoreCompanyIds'));
                    }
                    $(this).unbind("change");
                });
            }
            else if ($('.textfields input[type="text"]#txt' + prop + '-loc') != null) {
                MakeTagFlashable('.textfields input[type="text"]#txt' + prop + '-loc');
                $('.textfields input[type="text"]#txt' + prop + '-loc').bind("keyup", function () {
                    RemoveFlashableTag(this);
                    $(this).unbind("keyup");
                });
                isValid = false;
            }
        }
        else if (typeof (object[prop]) == "object" && prop == "CarClasses") {
            if (object[prop].length == 0) {
                isValid = false;
                $("#spanCarClassError").show();
            }
            else {
                $("#spanCarClassError").hide();
            }
        }
    }
    return isValid;
}

//Validate the value of the property
var ValidateProperty = function (property, value) {
    switch (String(property).toUpperCase()) {

        //var regExp = new RegExp("^[a-zA-Z]+$");
        //if (value.length != 3 || !regExp.test(value)) {
        //    return false;
        //}
        //break;
        case "WEEKLYEXTRADENOMINATOR":
        case "DAILYEXTRADAYFACTOR":
            //var regExp = new RegExp(/^\d{1,4}(\.\d{1,2})?$/);
            //if (!regExp.test(value)) {
            //    return false;
            //}
            if (!$.isNumeric(value) || parseFloat(value) > 9999.99) {
                return false;
            }
            break;
        case "TSDCUSTOMERNUMBER":
        case "TSDPASSCODE":
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

//Show the success/error messages
var DisplayMessage = function (message, isError) {
    $("#lblMessage").html(message).show().css("color", isError ? "red" : "green");
    if (!isError) {
        setTimeout(function () { $("#lblMessage").hide(); }, 3000);
    }
}

var EnableDisableButton = function (isEnable) {
    if (!isEnable) {
        $("#btnCancel").addClass("disable-button");
        $("#btnCancel").prop("disabled", true);
        $("#btnSaveLocation").addClass("btnDisabled");
        $("#btnSaveLocation").prop("disabled", true);
    }
    else {
        $("#btnCancel").removeClass("disable-button");
        $("#btnCancel").prop("disabled", false);
        $("#btnSaveLocation").removeClass("btnDisabled");
        $("#btnSaveLocation").prop("disabled", false);
    }
}

/*#region Ajax Calls*/
var SaveLocationDetails = function (objLocation) {
    var ajaxURl = '/RateShopper/Location/SaveLocation/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.SaveLocation;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        data: JSON.stringify({ 'objLocationDTO': objLocation, 'isCarClassesModified': isCarClassesModified, 'isRentalLengthsModified': isRentalLengthsModified }),
        success: function (data) {
            if (data.LocationBrandID > 0) {
                var locationModel = new BindObservableToModelProperty(data);
                if (locationID == 0) {

                    locationManagementModel.locations.push(locationModel);
                }
                else {
                    UpdateEditedLocation(locationModel);
                }
                LocationSmartSearch();
                if (isSortingApplied) {
                    locationManagementModel.SortLocation();
                }
                else if (locationID == 0) {
                    $("#divListing").scrollTop($("#divListing").height());
                }
                ResetForm();
                DisplayMessage("The location saved successfully", false);
            }
            else if (data.LocationBrandID == 0) {
                DisplayMessage("The location code already exist.", true);
            }
        },
        error: function (e) {
            //called when there is an error
            console.log("SaveLocation: " + e.message);
        }
    });
}

var DeleteLocationFromDB = function (objLocation) {
    var ajaxURl = '/RateShopper/Location/DeleteLocation/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.DeleteLocation;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        data: JSON.stringify({ 'locationID': objLocation.ID, 'locationBrandID': objLocation.LocationBrandID, 'userID': objLocation.CreatedBy }),
        success: function (result) {
            if (result) {
                locationManagementModel.locations.remove(objLocation);
                DisplayMessage("The location deleted successfully.", false);
                LocationSmartSearch();
            }
            else {
                DisplayMessage("Unable to delete the location.", true);
            }
        },
        error: function (e) {
            //called when there is an error
            console.log("DeleteLocation: " + e.message);
        }
    });
}

var GetLocation = function (locationID, locationBrandID, showLoader) {
    var ajaxURl = '/RateShopper/Location/GetLocation/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetLocation;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        global: showLoader,
        data: JSON.stringify({ 'locationID': locationID, 'locationBrandID': locationBrandID }),
        success: function (data) {
            if (data.ID > 0) {
                PopulateLocationData(data);
            }
            else {
                DisplayMessage("Unable to edit the location", true);
            }
        },
        error: function (e) {
            //called when there is an error
            console.log("GetLocation: " + e.message);
        }
    });
}

var BindLocations = function () {
    var ajaxURl = '/RateShopper/Location/GetLocations/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetLocations;
    }
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        success: function (data) {
            if (data) {
                var srcs = $.map(data, function (item) { return new BindObservableToModelProperty(item); });
                locationManagementModel.locations(srcs);
                //locationManagementModel.SortLocation();                
            }
        },
        error: function (e) {
            console.log("BindLocations: " + e.message);
        }
    });
}

function getCompanies() {
    var ajaxURl = '/RateShopper/Location/GetCompanies/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetCompanies;
    }
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        success: function (data) {
            if (data.status) {
                allCompanies = $.map(data.companies, function (item) { return new MasterCompanies(item); });
                locationManagementModel.companies(allCompanies);
                $("#ignoreCompanyIds").prop("disabled", true);
                filterBrand($('#dimension-source ul li.selected').attr('id'));
            }
        },
        error: function (e) {
            console.log("Companies data bind: " + e.message);
        }
    });
}


/*#End Region*/

//companies model
function MasterCompanies(data) {
    this.ID = data.ID;
    this.Name = data.Name;
    this.Code = data.Code;
    this.IsBrand = data.IsBrand;
}

//show - hide brand in competitor and Ignore competitor list on selection of Brand from dropdown
function filterBrand(selectedBrandId) {
    var matches = $.map(allCompanies, function (item) {
        if (item.ID != selectedBrandId) {
            return new MasterCompanies(item);
        }
    });
    locationManagementModel.companies(matches);
    $("#ignoreCompanyIds").empty();
    $(matches).each(function () {
        $("<option value='" + this.ID + "' IsBrand ='" + this.IsBrand + " '>" + this.Name + "</option>").appendTo($("#ignoreCompanyIds"));
    });
    $('#ignoreCompanyIds').prop('disabled', true);
    $('#ignoreCompanyIds option').click(function () {
        if ($(this).prop('selected')) {
            //$(this).attr('autoSelected', false);
        }
        else {
            //  $(this).attr('autoSelected', true);
        }
    });

    $('#competitorCompanyIds').attr("selectedcompanyids", "");
    $('#ignoreCompanyIds').attr("selectedcompanyids", "");
    $('#ignoreCompanyIds option:selected').removeAttr('selected');
    $('select#competitorCompanyIds, select#ignoreCompanyIds').scrollTop(0);
}

function AutoPopulateCompanies(companiesData) {
    if ($(companiesData).val() == null) {
        $(companiesData).attr("selectedcompanyids", "");
    }
    else {
        $(companiesData).attr("selectedcompanyids", $(companiesData).val());
    }
    var matches = null;
    var tempSelectedArrayID = "";
    var SelectedArrayID = "";
    matches = $.map(allCompanies, function (item) {
        if ($("#competitorCompanyIds option[value='" + item.ID + "']").prop('selected')) {
            return new MasterCompanies(item);
        }
    });
    var competitorSelectedIds = $.trim($("#competitorCompanyIds").val()).toString();
    $("#ignoreCompanyIds").prop("disabled", false);
    SelectedArrayID = competitorSelectedIds;
    $('#ignoreCompanyIds option').each(function () {
        if ($.inArray($(this).val(), SelectedArrayID.toString().trim().split(",")) != -1) {
            //$(this).attr('autoSelected', "true");
            $(this).prop('selected', true);
        }
        else {
            $(this).prop('selected', false);
        }
    });
    if ($('#ignoreCompanyIds').val() != null) {
        $('#ignoreCompanyIds').attr("selectedcompanyids", $('#ignoreCompanyIds').val().toString());
    }
    else {
        $('#ignoreCompanyIds').attr("selectedcompanyids", "");
    }
}

function SelectedCompanies(companiesData) {
    var selectedArray = [];
    var companySelectedCount = 0;
    $('#ignoreCompanyIds').attr("selectedcompanyids", $.trim($('#ignoreCompanyIds').val()).toString());
}

function configureSelectList() {
    var selectedBrandId = $('#dimension-source ul li.selected').attr('id');
    filterBrand(selectedBrandId);

    //reset select list values
    $('#competitorCompanyIds option:selected').removeAttr('selected');
    $('#competitorCompanyIds').attr('selectedCompanyIds', '');

    $('#ignoreCompanyIds option:selected').removeAttr('selected');
    $('#ignoreCompanyIds').attr('selectedCompanyIds', '');
    $('#ignoreCompanyIds').empty();

    var matches = $.map(allCompanies, function (item) {
        if (item.ID != selectedBrandId) {
            return new MasterCompanies(item);
        }
    });
    $(matches).each(function () {
        $("<option value='" + this.ID + "' IsBrand ='" + this.IsBrand + " '>" + this.Name + "</option>").appendTo($("#ignoreCompanyIds"));
    });
    $('select#competitorCompanyIds, select#ignoreCompanyIds').scrollTop(0);
    $('#ignoreCompanyIds').prop('disabled', true);
}