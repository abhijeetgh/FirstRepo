var companyManagementModel;
var sortOrder = "ASC";
var sortCompaniesBy = "Code";
var companyID = 0;
var isCompanyLocationsModified = false;
var validExtensions = "jpg,png,bmp,jpeg";
var validFileSize = "200000";
var companyLogo = "fake";
var oldCompanyLogo = "";
var isNewImageUploaded = false;
var isFormModified = false;
var isSortingApplied = false;

$(document).ready(CompanyManagmentDocumentReady);

//Show loader before every ajax call
$(document).ajaxStart(function () { $(".loader_container_main").show(); });
//Hide loader before every ajax call
$(document).ajaxComplete(function () { $(".loader_container_main").hide(); });

//This function initiates on Document ready
function CompanyManagmentDocumentReady() {
    companyManagementModel = new CompanyManagementModel();
    //Apply knockout binding
    ko.applyBindings(companyManagementModel);
    BindCompanies();

    $("#btnAddCompany").click(ResetForm);
    $("#btnCancel").click(function () { CancelClick(); });
    $("#btnSaveCompany").click(SaveCompany);
    $(".chkLocation").change(function () {
        isCompanyLocationsModified = true;        
        ShowHideInsideOutside(this)
    });
    $(".rdoIO").change(function () {
        isCompanyLocationsModified = true;
        $(this).closest(".divLocation").find("input[type='checkbox'].chkLocation").prop("checked", true);
    });
    $("#imgLogo").click(function () { $("#FileUpload").val(''); $("#FileUpload").trigger('click'); });
    $("#FileUpload").change(function () {
        var formData = new FormData();
        var totalFiles = document.getElementById("FileUpload").files.length;
        var file;
        var oldImageName = "";
        if(totalFiles>0) {
            file = document.getElementById("FileUpload").files[0];            
            if (CheckForValidExtensions(file.name) && CheckForValidSize(file.size)) {
                formData.append("FileUpload", file);
                formData.append("CompanyID", companyID);
                var oldImagePath = $("#imgLogo").attr("src");               
                if (oldImagePath != "") {
                    if (isNewImageUploaded) {
                        oldImageName = oldImagePath.substring(oldImagePath.lastIndexOf("/") + 1, oldImagePath.length);
                    }
                    else {
                        oldCompanyLogo = oldImagePath.substring(oldImagePath.lastIndexOf("/") + 1, oldImagePath.length);
                    }
                }
                formData.append("OldImageName", oldImageName);
            }
            else {
                return false;
            }            
        }
        
        var ajaxURl = '/RateShopper/Company/Upload';
        if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
            ajaxURl = AjaxURLSettings.Upload;
        }

        $.ajax({
            type: "POST",
            url: ajaxURl,
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                $("#imgLogo").prop("src", $("#hdnLogoInitial").val() + response);               
                companyLogo = response.substring(response.lastIndexOf("/") + 1, response.length);
                isNewImageUploaded = true;
                isFormModified = true;
                EnableDisableButton(true);
                RemoveFlashableTag("#imgLogo");
            },
            error: function (error) {                
                ShowConfirmBox("Oops! Something went wrong while uploading image. Please try again", false);
            }
        });
    });

    //Bind Company Search keyup event
    $('#searchCompany').bind('keyup', function () {
        CompanySmartSearch();        
    });

    $("#searchCompany").focus();
    $("#lmform input[type=text], #lmform input[type=checkbox], #lmform input[type=radio]").change(function () { EnableDisableButton(true); isFormModified = true; });
    $("#lmform input[type=text]").bind("input", function () { EnableDisableButton(true); isFormModified = true; });
}

//Check for valid extensions
function CheckForValidExtensions(inputPath) {
    if ($.inArray(inputPath.substring(inputPath.lastIndexOf('.') + 1, inputPath.length).toLowerCase(), validExtensions.split(',')) != -1) {        
        return true;
    }        
    ShowConfirmBox("Allowed file types: jpg, png, tif, tiff, bmp, jpeg", false);
    return false;
}

//Check for valid size
function CheckForValidSize(fileSize) {
    if (fileSize <= validFileSize) {
        return true;
    }
    ShowConfirmBox('Allowed file size: ' + validFileSize / 1000 + ' KB', false);
    return false;
}

//Show hide inside/outside radio buttons
var ShowHideInsideOutside = function (control) {
    $(control).closest(".divLocation").find("input[type='radio']:checked").prop("checked", false);
    //if ($(control).is(":checked")) {
    //    $($(control).closest(".divLocation").find(".spanIO")).css("visibility", "visible");
    //}
    //else {        
    //    $($(control).closest(".divLocation").find(".spanIO")).css("visibility", "hidden");
    //}
}

//Company smart search
var CompanySmartSearch = function () {
    var $inpuTextSelector = $("#searchCompany").val();
    if ($inpuTextSelector.length > 0) {
        $("#tblCompanies tbody td[class='code']").each(function () {
            if ($.trim($(this).text()).toLowerCase().indexOf($inpuTextSelector.toLowerCase()) == 0) {
                $(this).closest("tr").show();
            }
            else {
                $(this).closest("tr").hide();
            }
        });
    } else {
        $("#tblCompanies tbody tr").show();
    }

    if ($inpuTextSelector.length > 0 && $("#tblCompanies tbody tr[style$='display: none;']:not(.remove_if_datafound)").length == $("#tblCompanies tbody tr:not(.remove_if_datafound)").length) {
        MakeTagFlashable("#searchCompany");
        if ($("#tblCompanies tbody tr.remove_if_datafound").length == 0) {
            $("#tblCompanies tbody").append("<tr class='remove_if_datafound'><td style='text-align:center;font-weight:bold;color:red;text-transform:none;' colspan='3'>No company found.</td></tr>")
            $("#companyheaders").hide();
            $("#no_rows").show();
        }
    }
    else {
        RemoveFlashableTag("#searchCompany");
        $("#tblCompanies tbody tr.remove_if_datafound").remove();
        $("#companyheaders").show();
        $("#no_rows").hide();
    }
    AddFlashingEffect();
}

//Knockout js variables
var CompanyManagementModel = function () {
    var self = this;
    self.companies = ko.observableArray([]);
    self.DeleteCompany = function (company, event) {
        var message = "Do you want to delete the <b>" + company.Name().toUpperCase() + "</b> company?";
        ShowConfirmBox(message, true, DeleteCompany, company);        
        event.stopPropagation();
    }
    self.EditCompany = function (company) {
        if (isFormModified) {
            var message = "Are you sure you want to discard the changes?";
            ShowConfirmBox(message, true, EditCompany, company);            
        }
        else {
            EditCompany(company);
        }
    }

    self.SortCompany = function () {
        switch (sortCompaniesBy) {
            case "Name":
                if (sortOrder == "DESC") {
                    self.companies.sort(function (left, right) {
                        return left.Name().toLowerCase() == right.Name().toLowerCase() ? 0 : (left.Name().toLowerCase() < right.Name().toLowerCase() ? 1 : -1)
                    });
                }
                else {
                    self.companies.sort(function (left, right) {
                        return left.Name().toLowerCase() == right.Name().toLowerCase() ? 0 : (left.Name().toLowerCase() < right.Name().toLowerCase() ? -1 : 1)
                    });
                }
                break;

            case "Code":
                if (sortOrder == "DESC") {
                    self.companies.sort(function (left, right) {
                        return left.Code().toLowerCase() == right.Code().toLowerCase() ? (left.Name().toLowerCase() < right.Name().toLowerCase() ? 1 : -1) : (left.Code().toLowerCase() < right.Code().toLowerCase() ? 1 : -1)
                    });
                }
                else {
                    self.companies.sort(function (left, right) {
                        return left.Code().toLowerCase() == right.Code().toLowerCase() ? (left.Name().toLowerCase() < right.Name().toLowerCase() ? -1 : 1) : (left.Code().toLowerCase() < right.Code().toLowerCase() ? -1 : 1)
                    });
                }
                break;
        }
    }
}

//This method is used to identify order of sorting
function ApplySorting(control, sortBy) {
    isSortingApplied = true;
    switch (sortBy) {
        case "Name":
            var prevSortOrder = $(control)[0].className;
            sortCompaniesBy = "Name";
            if (prevSortOrder == "Asc") {
                $(control).removeClass("Asc").addClass("Desc");
                sortOrder = "DESC";
            }
            else {
                $(control).removeClass("Desc").addClass("Asc");
                sortOrder = "ASC";
            }
            break;
        case "Code":
            var prevSortOrder = $(control)[0].className;
            sortCompaniesBy = "Code";
            if (prevSortOrder == "Asc") {
                $(control).removeClass("Asc").addClass("Desc");
                sortOrder = "DESC";
            }
            else {
                $(control).removeClass("Desc").addClass("Asc");
                sortOrder = "ASC";
            }
            break;
    }
    companyManagementModel.SortCompany();
}

//Convert simple properties to Knockout observable properties 
var BindObservableToModelProperty = function (data) {
    this.ID = data.ID;
    this.Code = ko.observable(data.Code);    
    this.Name = data.Name != null ? ko.observable(data.Name) : ko.observable("");
    this.CreatedBy = data.CreatedBy;
}

//Fetch the brand location data from the form, validate it then save
var SaveCompany = function () {
    $("#lblMessage").hide();
    $("#spanLocationsError").hide();
    var objCompany = new Object();
    if (companyID > 0) {
        objCompany.ID = companyID;
    }
    objCompany.Code = $.trim($("#txtCode-company").val()).toUpperCase();
    objCompany.Name = $.trim($("#txtName-company").val());
    objCompany.Logo = companyLogo;
    objCompany.lstLocations = GetSelectedLocations();
    objCompany.CreatedBy = $("#LoggedInUserId").val();
    //if (objCompany.lstLocations == null) {
    //    alert("dsfsdf");
    //    DisplayMessage("Please specify inside/ouside for selected location(s).", true);
    //    return false;
    //}

    if (ValidateObject(objCompany) && IsDuplicateCodeExists(objCompany) && (companyID > 0 || (companyID == 0 && objCompany.Logo != "fake"))) {        
        objCompany.Logo = objCompany.Logo == "fake" ? "" : objCompany.Logo;
        SaveCompanyDetails(objCompany);
    }    
    else {
        if (companyID == 0 && objCompany.Logo == "fake") {
            MakeTagFlashable('#imgLogo');
        }

        if ($(".temp").length > 0) {
            DisplayMessage("Please review the fields highlighted in Red.", true);
            AddFlashingEffect();
        }        
    }
}

var GetSelectedLocations = function () {
    var lstCompanyLocations = new Array();
    var IsInsideOutsideSelected = true;
    $("#divCompanyLocations input[type='checkbox']:checked").each(function () {
        var companyLocation = new Object();
        companyLocation.LocationID = String(this.id).replace("chkLL_", "");
        var IO = $($(this).closest(".divLocation").find(".spanIO")).find("input[type='radio']:checked");
        if (IO.length > 0) {
            companyLocation.IsTerminalInside = $(IO).val() == "inside" ? true : false;
        }
        else {
            IsInsideOutsideSelected = false;
            return false;
            //companyLocation.IsTerminalInside = true;
        }        
        companyLocation.CompanyID = companyID;
        lstCompanyLocations.push(companyLocation);
    });
    if (!IsInsideOutsideSelected) {
        return null;
    }
    return lstCompanyLocations;
}

//Delete Company
var DeleteCompany = function (objCompany) {
    if (typeof (objCompany) == 'undefined') {
        objCompany = this;
    }
    objCompany.CreatedBy = $("#LoggedInUserId").val();
    //Reset the edited/add mode data
    ResetForm();
    DeleteCompanyFromDB(objCompany);
}

//Edit company functionality, highlight the selected row
var EditCompany = function (objCompany) {
    if (typeof (objCompany) == 'undefined') {
        objCompany = this;
    }
    ResetForm();
    $($("#company_" + objCompany.ID).closest("tr")).addClass("grey_bg");
    GetCompany(objCompany.ID, true);
}

//Populate the edited company data
var PopulateCompanyData = function (objCompany) {
    $("#HaddingTitle").html("UPDATE / VIEW COMPANY");
    $("#txtCode-company").val(objCompany.Code);
    $("#txtName-company").val(objCompany.Name);
    //objCompany.Logo = "path";
    $("#imgLogo").prop("src", $("#hdnLogoInitial").val() + objCompany.Logo);
    
    companyID = objCompany.ID;
    for (var i = 0; i < objCompany.lstLocations.length; i++) {
        var locationSelector = "#chkLL_" + objCompany.lstLocations[i].LocationID;
        if ($(locationSelector).length > 0) {
            $(locationSelector).prop("checked", true);
            //$($(locationSelector).closest(".divLocation").find(".spanIO")).css("visibility", "visible");
            if (objCompany.lstLocations[i].IsTerminalInside == false) {
                $($(locationSelector).closest(".divLocation").find("input[type='radio'][value='outside']")).prop("checked", true);
            }
            else if (objCompany.lstLocations[i].IsTerminalInside == true) {
                $($(locationSelector).closest(".divLocation").find("input[type='radio'][value='inside']")).prop("checked", true);
            }
            //else {
            //    $($(locationSelector).closest(".divLocation").find("input[type='radio'][value='inside']")).prop("checked", true);
            //}
        }
    }    
}

//Reset the company form
var ResetForm = function () {
    $("#HaddingTitle").html("CREATE COMPANY");
    $("#lblMessage").hide();
    $("#spanLocationsError").hide();
    $("#tblCompanies tbody tr").removeClass("grey_bg");
    $(".textfields input[type='text']").each(function () { $(this).val('') });

    //$(".spanIO:visible").css("visibility", "hidden");
    
    $("#imgLogo").prop("src", $("#hdnLogoInitial").val() + 'images/default_logo.png');
    isCompanyLocationsModified = false;        
    
    if (companyLogo.length > 0 && companyLogo != "fake") {
        DeleteUploadedImage(companyLogo);
    }
    companyID = 0;
    companyLogo = "fake";
    oldCompanyLogo = "";
    isNewImageUploaded = false;
    isFormModified = false;
    EnableDisableButton(false);
    
    $("#divCompanyLocations input[type='checkbox']:checked").prop("checked", false);
    $("#divCompanyLocations input[type='radio']:checked").prop("checked", false);
    //$("#divCompanyLocations input[type='radio'][value='inside']:not(':checked')").prop("checked", true);

    $(".textfields input[type='text'].temp").each(function () {
        RemoveFlashableTag(this);
    });
    RemoveFlashableTag("#imgLogo");
}

var CancelClick = function () {
    if (companyID > 0 && isFormModified) {
        var message = "Do you want to reset the data?";
        ShowConfirmBox(message, true, CancelChangesAndResetForm);        
    }
    else {
        ResetForm();
    }
}

var CancelChangesAndResetForm = function () {
    var tempcompanyID = 0;
    tempcompanyID = companyID;
    ResetForm();
    $($("#company_" + tempcompanyID).closest("tr")).addClass("grey_bg");
    GetCompany(tempcompanyID, false);
    tempcompanyID = 0;
}

//Update Edited company details in obseravable array
var UpdateEditedCompany = function (objCompany) {
    ko.utils.arrayForEach(companyManagementModel.companies(), function (currentCompany) {
        if (currentCompany.ID == objCompany.ID) {
            currentCompany.Code(objCompany.Code());
            currentCompany.Name(objCompany.Name());
        }
    });
}

//Validate the object
var ValidateObject = function (object) {
    var isValid = true;
    for (var prop in object) {
        if (typeof (prop) != 'undefined' && typeof (object[prop]) != "object") {
            if (object.hasOwnProperty(prop) && object[prop] !== "" && ValidateProperty(prop, object[prop])) {
                isValid = isValid & true;
            }
            else if ($('.textfields input[type="text"]#txt' + prop + '-company') != null) {
                MakeTagFlashable('#txt' + prop + '-company');
                $('#txt' + prop + '-company').bind("keyup", function () {
                    RemoveFlashableTag(this);
                    $(this).unbind("keyup");
                });
                isValid = false;
            }
        }
        else if (typeof (object[prop]) == "object" && prop == "lstLocations") {
            if (object[prop] == null) {
                isValid = false;
                $("#spanLocationsError").show();
            }
            else {
                $("#spanLocationsError").hide();
            }
        }
    }
    return isValid;
}

//Validate the value of the property
var ValidateProperty = function (property, value) {
    switch (String(property).toUpperCase()) {
        case "CODE":
            var regExp = new RegExp("^[a-zA-Z0-9]+$");
            if (value.length != 2 || !regExp.test(value)) {
                return false;
            }
            break;
        case "NAME":
            var regExp = new RegExp("[<>]");
            if (regExp.test(value)) {
                return false;
            }
            break;        
    }
    return true;
}

//Validate duplicate code
var IsDuplicateCodeExists = function (objCompany) {
    var company = ko.utils.arrayFirst(companyManagementModel.companies(), function (currentCompany) {
        return (currentCompany.Code().toLowerCase() == objCompany.Code.toLowerCase() && currentCompany.ID != objCompany.ID);
    });

    if (company) {
        DisplayMessage("The company name already exist.", true);
        return false;
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
        $("#btnSaveCompany").addClass("btnDisabled");
        $("#btnSaveCompany").prop("disabled", true);
    }
    else {
        $("#btnCancel").removeClass("disable-button");
        $("#btnCancel").prop("disabled", false);
        $("#btnSaveCompany").removeClass("btnDisabled");
        $("#btnSaveCompany").prop("disabled", false);
    }
}

/*#region Ajax Calls*/
var SaveCompanyDetails = function (objCompany) {
    var ajaxURl = '/RateShopper/Company/SaveCompany/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.SaveCompany;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        data: JSON.stringify({ 'objCompanyDTO': objCompany, 'isCompanyLocationsModified': isCompanyLocationsModified }),
        success: function (data) {
            if (data != null && data.ID > 0) {
                var companyModel = new BindObservableToModelProperty(data);
                if (companyID == 0) {
                    companyManagementModel.companies.push(companyModel);
                    
                }
                else {
                    UpdateEditedCompany(companyModel);
                }
                CompanySmartSearch();
                if (isSortingApplied) {
                    companyManagementModel.SortCompany();
                }
                else if (companyID == 0) {
                    $("#divListing").scrollTop($("#divListing").height());
                }
                companyLogo = "fake";
                ResetForm();
                DisplayMessage("The company saved successfully", false);
            }
            else if (data.ID == 0) {
                DisplayMessage("The company name already exist.", true);
            }
        },
        error: function (e) {
            //called when there is an error
            console.log("SaveCompany: " + e.message);
        }
    });
}

var DeleteCompanyFromDB = function (objCompany) {
    var ajaxURl = '/RateShopper/Company/DeleteCompany/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.DeleteCompany;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        data: JSON.stringify({ 'companyID': objCompany.ID, 'userID': objCompany.CreatedBy }),
        success: function (result) {
            if (result) {
                companyManagementModel.companies.remove(objCompany);                
                DisplayMessage("Company deleted successfully", false);
                CompanySmartSearch();
            }
            else {
                DisplayMessage("Unable to delete the company", true);
            }
        },
        error: function (e) {
            //called when there is an error
            console.log("DeleteCompany: " + e.message);
        }
    });
}

var GetCompany = function (companyID, showLoader) {
    var ajaxURl = '/RateShopper/Company/GetCompany/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetCompany;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        global: showLoader,
        data: JSON.stringify({ 'companyID': companyID }),
        success: function (data) {
            if (data != null && data.ID > 0) {
                PopulateCompanyData(data);
            }
            else {
                DisplayMessage("Unable to edit the company", true);
            }
        },
        error: function (e) {
            //called when there is an error
            console.log("GetCompany: " + e.message);
        }
    });
}

var BindCompanies = function () {
    var ajaxURl = '/RateShopper/Company/GetAllCompanies/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetAllCompanies;
    }

    $.ajax({
        url: ajaxURl,
        type: 'GET',
        success: function (data) {
            if (data) {
                var srcs = $.map(data, function (item) { return new BindObservableToModelProperty(item); });

                companyManagementModel.companies(srcs);
                //locationManagementModel.SortLocation();                
            }
        },
        error: function (e) {
            console.log("BindCompanies: " + e.message);
        }
    });
}

var DeleteUploadedImage = function (uploadedImageName) {
    var ajaxURl = '/RateShopper/Company/DeleteUploadedImage/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.DeleteUploadedImage;
    }

    if (uploadedImageName != "") {
        $.ajax({
            type: "GET",
            url: ajaxURl,
            global: false,
            data: { oldImage: uploadedImageName, companyID: companyID },
            contentType: "application/json;charset=utf-8;",
            success: function (response) {
            },
            error: function (error) {                
            }
        });
    }
}
/*#End Region*/