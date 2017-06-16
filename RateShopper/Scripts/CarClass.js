var carClassManagementModel;
var sortOrder = "ASC";
var sortCarClassesBy = "DisplayOrder";
var carClassID = 0;
var validExtensions = "jpg,png,bmp,jpeg";
var validFileSize = "200000";
var carClassLogo = "fake";
var oldCarClassLogo = "";
var isNewImageUploaded = false;
var isFormModified = false;
var isSortingApplied = true;
var positionChanged = false;
$(document).ready(CarClassManagmentDocumentReady);

//Show loader before every ajax call
$(document).ajaxStart(function () { $(".loader_container_main").show(); });
//Hide loader before every ajax call
$(document).ajaxComplete(function () { $(".loader_container_main").hide(); });

//This function initiates on Document ready
function CarClassManagmentDocumentReady() {
    carClassManagementModel = new CarClassManagementModel();
    //Apply knockout binding
    ko.applyBindings(carClassManagementModel);
    BindCarClasses();

    $("#btnAddCarClass").click(ResetForm);
    $("#btnCancel").click(function () { CancelClick(); });
    $("#btnSaveCarClass").click(SaveCarClass);
    $("#imgLogo").click(function () { $("#FileUpload").val(''); $("#FileUpload").trigger('click'); });
    $("#FileUpload").change(function () {
        var formData = new FormData();
        var totalFiles = document.getElementById("FileUpload").files.length;
        var file;
        var oldImageName = "";
        if (totalFiles > 0) {
            file = document.getElementById("FileUpload").files[0];
            if (CheckForValidExtensions(file.name) && CheckForValidSize(file.size)) {
                formData.append("FileUpload", file);
                formData.append("CarClassID", carClassID);

                var oldImagePath = $("#imgLogo").attr("src");
                if (oldImagePath != "") {
                    if (isNewImageUploaded) {
                        oldImageName = oldImagePath.substring(oldImagePath.lastIndexOf("/") + 1, oldImagePath.length);
                    }
                    else {
                        oldCarClassLogo = oldImagePath.substring(oldImagePath.lastIndexOf("/") + 1, oldImagePath.length);
                    }
                }
                formData.append("OldImageName", oldImageName);
            }
            else {
                return false;
            }
        }

        var ajaxURl = '/RateShopper/CarClass/Upload';
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
                carClassLogo = response.substring(response.lastIndexOf("/") + 1, response.length);
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

    //Bind CarClass Search keyup event
    $('#searchCarClass').bind('keyup', function () {
        CarClassSmartSearch();
    });

    $("#searchCarClass").focus();
    $("#lmform input[type=text],input[type=number]").change(function () { EnableDisableButton(true); isFormModified = true; });
    $("#lmform input[type=text],input[type=number]").bind("input", function () { EnableDisableButton(true); isFormModified = true; });
}

//Check for valid extensions
function CheckForValidExtensions(inputPath) {
    if ($.inArray(inputPath.substring(inputPath.lastIndexOf('.') + 1, inputPath.length).toLowerCase(), validExtensions.split(',')) != -1) {
        return true;
    }
    ShowConfirmBox('Allowed file types: jpg, png, tif, tiff, bmp, jpeg', false);
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

//CarClass smart search
var CarClassSmartSearch = function () {
    var $inpuTextSelector = $("#searchCarClass").val();
    if ($inpuTextSelector.length > 0) {
        $("#tblCarClasses tbody td[class='code']").each(function () {
            if ($.trim($(this).text()).toLowerCase().indexOf($inpuTextSelector.toLowerCase()) == 0) {
                $(this).closest("tr").show();
            }
            else {
                $(this).closest("tr").hide();
            }
        });
    } else {
        $("#tblCarClasses tbody tr").show();
    }

    if ($inpuTextSelector.length > 0 && $("#tblCarClasses tbody tr[style$='display: none;']:not(.remove_if_datafound)").length == $("#tblCarClasses tbody tr:not(.remove_if_datafound)").length) {
        MakeTagFlashable("#searchCarClass");
        if ($("#tblCarClasses tbody tr.remove_if_datafound").length == 0) {
            $("#tblCarClasses tbody").append("<tr class='remove_if_datafound'><td style='text-align:center;font-weight:bold;color:red;text-transform:none;' colspan='3'>No car class found.</td></tr>")
            $("#carclassheaders").hide();
            $("#no_rows").show();
        }
    }
    else {
        RemoveFlashableTag("#searchCarClass");
        $("#tblCarClasses tbody tr.remove_if_datafound").remove();
        $("#carclassheaders").show();
        $("#no_rows").hide();
    }
    AddFlashingEffect();
}

//Knockout js variables
var CarClassManagementModel = function () {
    var self = this;
    self.carclasses = ko.observableArray([]);
    self.DeleteCarClass = function (carClass, event) {
        var message = "Do you want to delete the <b>" + carClass.Description().toUpperCase() + "</b> car class?";
        ShowConfirmBox(message, true, DeleteCarClass, carClass);
        event.stopPropagation();
    }
    self.EditCarClass = function (carClass) {
        if (isFormModified) {
            var message = "Are you sure you want to discard the changes?";
            ShowConfirmBox(message, true, EditCarClass, carClass);
        }
        else {
            EditCarClass(carClass);
        }
    }

    self.SortCarClass = function () {
        switch (sortCarClassesBy) {
            case "Description":
                if (sortOrder == "DESC") {
                    self.carclasses.sort(function (left, right) {
                        return left.Description().toLowerCase() == right.Description().toLowerCase() ? 0 : (left.Description().toLowerCase() < right.Description().toLowerCase() ? 1 : -1)
                    });
                }
                else {
                    self.carclasses.sort(function (left, right) {
                        return left.Description().toLowerCase() == right.Description().toLowerCase() ? 0 : (left.Description().toLowerCase() < right.Description().toLowerCase() ? -1 : 1)
                    });
                }
                break;

            case "Code":
                if (sortOrder == "DESC") {
                    self.carclasses.sort(function (left, right) {
                        return left.Code().toLowerCase() == right.Code().toLowerCase() ? (left.Description().toLowerCase() < right.Description().toLowerCase() ? 1 : -1) : (left.Code().toLowerCase() < right.Code().toLowerCase() ? 1 : -1)
                    });
                }
                else {
                    self.carclasses.sort(function (left, right) {
                        return left.Code().toLowerCase() == right.Code().toLowerCase() ? (left.Description().toLowerCase() < right.Description().toLowerCase() ? -1 : 1) : (left.Code().toLowerCase() < right.Code().toLowerCase() ? -1 : 1)
                    });
                }
                break;
            case "DisplayOrder":
              
                if (sortOrder == "DESC") {
                    self.carclasses.sort(function (left, right) {
                        return left.DisplayOrder() == right.DisplayOrder() ? (left.Description().toLowerCase() < right.Description().toLowerCase() ? 1 : -1) : (left.DisplayOrder() < right.DisplayOrder() ? 1 : -1)
                    });
                }
                else {
                    self.carclasses.sort(function (left, right) {
                        return left.DisplayOrder() == right.DisplayOrder() ? (left.Description().toLowerCase() < right.Description().toLowerCase() ? -1 : 1) : (left.DisplayOrder() < right.DisplayOrder() ? -1 : 1)
                    });
                }
                break;
        }
    }
}

//This method is used to identify order of sorting
function ApplySorting(control, sortBy) {
    isSortingApplied = true;
    var prevSortOrder = $(control)[0].className;
    switch (sortBy) {
        case "Description":
            sortCarClassesBy = "Description";
            break;
        case "Code":
            sortCarClassesBy = "Code";
            break;
        case "DisplayOrder":
            sortCarClassesBy = "DisplayOrder";
    }

    if (prevSortOrder == "Asc") {
        $(control).removeClass("Asc").addClass("Desc");
        sortOrder = "DESC";
    }
    else {
        $(control).removeClass("Desc").addClass("Asc");
        sortOrder = "ASC";
    }
    carClassManagementModel.SortCarClass();
}

//Convert simple properties to Knockout observable properties 
var BindObservableToModelProperty = function (data) {
    this.ID = data.ID;
    this.Code = ko.observable(data.Code);
    this.Description = ko.observable(data.Description);
    this.CreatedBy = data.CreatedBy;
    this.DisplayOrder = ko.observable(data.CarClassOrder);
}

//Fetch the car class data from the form, validate it then save
var SaveCarClass = function () {
    $("#lblMessage").hide();
    var objCarClass = new Object();
    if (carClassID > 0) {
        objCarClass.ID = carClassID;
    }
    objCarClass.Code = $.trim($("#txtCode-carclass").val()).toUpperCase();
    objCarClass.Description = $.trim($("#txtDescription-carclass").val());
    objCarClass.Logo = carClassLogo;
    objCarClass.CreatedBy = $("#LoggedInUserId").val();
    objCarClass.CarClassOrder = $("#carClassOrder").val();
    if (ValidateObject(objCarClass) && IsDuplicateCodeExists(objCarClass) && (carClassID > 0 || (carClassID == 0 && objCarClass.Logo != "fake"))) {
        objCarClass.Logo = objCarClass.Logo == "fake" ? "" : objCarClass.Logo;
        // SaveCarClassDetails(objCarClass);
        //check car class order exist
        checkCarClassOrder(objCarClass);
    }
    else {
        if (carClassID == 0 && objCarClass.Logo == "fake") {
            MakeTagFlashable('#imgLogo');
        }

        if ($(".temp").length > 0) {
            DisplayMessage("Please review the fields highlighted in Red.", true);
            AddFlashingEffect();
        }
    }
}

//Delete CarClass
var DeleteCarClass = function (objCarClass) {
    if (typeof (objCarClass) == 'undefined') {
        objCarClass = this;
    }
    objCarClass.CreatedBy = $("#LoggedInUserId").val();
    //Reset the edited/add mode data
    ResetForm();
    DeleteCarClassFromDB(objCarClass);
}

//Edit CarClass functionality, highlight the selected row
var EditCarClass = function (objCarClass) {
    if (typeof (objCarClass) == 'undefined') {
        objCarClass = this;
    }
    ResetForm();
    $($("#carclass_" + objCarClass.ID).closest("tr")).addClass("grey_bg");
    GetCarClass(objCarClass.ID, true);
}

//Populate the edited CarClass data
var PopulateCarClassData = function (objCarClass) {
    $("#HaddingTitle").html("UPDATE / VIEW CAR CLASS");
    $("#txtCode-carclass").val(objCarClass.Code);
    $("#txtDescription-carclass").val(objCarClass.Description);
    //objCarClass.Logo = "path";
    $("#imgLogo").prop("src", $("#hdnLogoInitial").val() + objCarClass.Logo);

    carClassID = objCarClass.ID;
    $("#carClassOrder").val(objCarClass.CarClassOrder);
}

//Reset the CarClass form
var ResetForm = function () {
    $("#HaddingTitle").html("CREATE CAR CLASS");
    $("#lblMessage").hide();
    $("#tblCarClasses tbody tr").removeClass("grey_bg");
    $(".textfields input[type='text']").each(function () { $(this).val('') });

    $("#imgLogo").prop("src", $("#hdnLogoInitial").val() + 'images/default_logo.png');

    if (carClassLogo.length > 0 && carClassLogo != "fake") {
        DeleteUploadedImage(carClassLogo);
    }
    carClassID = 0;
    carClassLogo = "fake";
    isNewImageUploaded = false;
    isFormModified = false;
    EnableDisableButton(false);
    oldCarClassLogo = "";
    $(".textfields input[type='text'].temp").each(function () {
        RemoveFlashableTag(this);
    });
    RemoveFlashableTag("#imgLogo");
    $('#carClassOrder').val('');
}

var CancelClick = function () {
    if (carClassID > 0 && isFormModified) {
        var message = "Do you want to reset the data?";
        ShowConfirmBox(message, true, CancelChangesAndResetForm);
    }
    else {
        ResetForm();
    }
}

function CancelChangesAndResetForm() {
    var tempcarClassID = 0;
    tempcarClassID = carClassID;
    ResetForm();
    $($("#carclass_" + tempcarClassID).closest("tr")).addClass("grey_bg");
    GetCarClass(tempcarClassID, false);
    tempcarClassID = 0;
}

//Update Edited carClass details in obseravable array
var UpdateEditedCarClass = function (objCarClass) {
    ko.utils.arrayForEach(carClassManagementModel.carclasses(), function (currentCarClass) {
        if (currentCarClass.ID == objCarClass.ID) {
            currentCarClass.Code(objCarClass.Code());
            currentCarClass.Description(objCarClass.Description());
            currentCarClass.DisplayOrder(objCarClass.DisplayOrder());
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
            else if ($('.textfields input[type="text"]#txt' + prop + '-carclass').length != 0) {
                MakeTagFlashable('#txt' + prop + '-carclass');
                $('#txt' + prop + '-carclass').bind("keyup", function () {
                    RemoveFlashableTag(this);
                    $(this).unbind("keyup");
                });
                isValid = false;
            }
            else if ($('.textfields input[type="number"]').length != 0) {
                MakeTagFlashable('#carClassOrder');
                $('#carClassOrder').bind("keyup", function () {
                    RemoveFlashableTag(this);
                    $(this).unbind("keyup");
                });
                isValid = false;
            }
        }
    }
    return isValid;
}

//Validate the value of the property
var ValidateProperty = function (property, value) {
    switch (String(property).toUpperCase()) {
        case "CODE":
            var regExp = new RegExp("^[a-zA-Z]+$");
            if (value.length != 4 || !regExp.test(value)) {
                return false;
            }
            break;
        case "DESCRIPTION":
            var regExp = new RegExp("[<>]");
            if (regExp.test(value)) {
                return false;
            }
            break;
        case "CARCLASSORDER":
            if (value > 100 || value <= 0) {
                return false;
            }
            break;
    }
    return true;
}

//Validate duplicate code
var IsDuplicateCodeExists = function (objCarClass) {
    var carclass = ko.utils.arrayFirst(carClassManagementModel.carclasses(), function (currentCarClass) {
        return (currentCarClass.Code().toLowerCase() == objCarClass.Code.toLowerCase() && currentCarClass.ID != objCarClass.ID);
    });

    if (carclass) {
        DisplayMessage("The car class name exists.", true);
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
        $("#btnSaveCarClass").addClass("btnDisabled");
        $("#btnSaveCarClass").prop("disabled", true);
    }
    else {
        $("#btnCancel").removeClass("disable-button");
        $("#btnCancel").prop("disabled", false);
        $("#btnSaveCarClass").removeClass("btnDisabled");
        $("#btnSaveCarClass").prop("disabled", false);
    }
}

/*#region Ajax Calls*/
var SaveCarClassDetails = function (objCarClass) {

    if (typeof (objCarClass) == 'undefined') {
        objCarClass = this;
    }
    var ajaxURl = '/RateShopper/CarClass/SaveCarClass/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.SaveCarClass;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        //data: JSON.stringify({ 'objCarClassDTO': objCarClass, 'oldImage': oldCarClassLogo }),
        data: JSON.stringify({ 'objCarClassDTO': objCarClass }),
        success: function (data) {
            if (data != null && data.status && data.objCarClass != null) {
                if (data.objCarClass.ID > 0) {
                    var carClassModel = new BindObservableToModelProperty(data.objCarClass);
                    if (carClassID == 0) {
                        carClassManagementModel.carclasses.push(carClassModel);
                    }
                    else {
                        UpdateEditedCarClass(carClassModel);
                    }
                    CarClassSmartSearch();
                    BindCarClasses();
                    if (isSortingApplied) {
                        carClassManagementModel.SortCarClass();
                    }
                    else if (carClassID == 0) {
                        $("#divListing").scrollTop($("#divListing").height());
                    }
                    carClassLogo = "fake";
                    ResetForm();
                    DisplayMessage("The car class saved successfully.", false);
                }
                else if (data.objCarClass.ID == 0) {
                    DisplayMessage("The car class name exists.", true);
                }
            }
            else if (data != null && data.status == false) {
                var sortOrder = $('#carClassOrder').val();
                DisplayMessage(data.carclass + ' is already using the display order ' + sortOrder + '.', true);
            }
            else {
                DisplayMessage("Oops Something went wrong please try again.", true);
            }
        },
        error: function (e) {
            //called when there is an error
            console.log("SaveCarClass: " + e.message);
        }
    });
}

var DeleteCarClassFromDB = function (objCarClass) {
    var ajaxURl = '/RateShopper/CarClass/DeleteCarClass/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.DeleteCarClass;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        data: JSON.stringify({ 'carClassID': objCarClass.ID, 'userID': objCarClass.CreatedBy }),
        success: function (result) {
            if (result) {
                carClassManagementModel.carclasses.remove(objCarClass);
                BindCarClasses();
                DisplayMessage("The car class deleted successfully", false);
                CarClassSmartSearch();
            }
            else {
                DisplayMessage("Unable to delete the car class.", true);
            }
        },
        error: function (e) {
            //called when there is an error
            console.log("DeleteCarClass: " + e.message);
        }
    });
}

var GetCarClass = function (carClassID, showLoader) {
    var ajaxURl = '/RateShopper/CarClass/GetCarClass/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetCarClass;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        contentType: "application/json;charset=utf-8",
        dataType: 'json',
        global: showLoader,
        data: JSON.stringify({ 'carClassID': carClassID }),
        success: function (data) {
            if (data != null && data.ID > 0) {
                PopulateCarClassData(data);
            }
            else {
                DisplayMessage("Unable to edit the carclass.", true);
            }
        },
        error: function (e) {
            //called when there is an error
            console.log("Getcarclass: " + e.message);
        }
    });
}

var BindCarClasses = function () {
    var ajaxURl = '/RateShopper/CarClass/GetAllCarClasses/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetAllCarClasses;
    }

    $.ajax({
        url: ajaxURl,
        type: 'GET',
        success: function (data) {
            if (data) {
                var srcs = $.map(data, function (item) { return new BindObservableToModelProperty(item); });
                carClassManagementModel.carclasses(srcs);
                //locationManagementModel.SortLocation();           
                if (isSortingApplied) {
                    carClassManagementModel.SortCarClass();
                }
                else if (carClassID == 0) {
                    $("#divListing").scrollTop($("#divListing").height());
                }
            }
        },
        error: function (e) {
            console.log("BindCarClasses: " + e.message);
        }
    });
}

var DeleteUploadedImage = function (uploadedImageName) {
    var ajaxURl = '/RateShopper/CarClass/DeleteUploadedImage/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.DeleteUploadedImage;
    }

    if (uploadedImageName != "") {
        $.ajax({
            type: "GET",
            url: ajaxURl,
            global: false,
            data: { oldImage: uploadedImageName, carClassID: carClassID },
            contentType: "application/json;charset=utf-8;",
            success: function (response) {
            },
            error: function (error) {
            }
        });
    }
}

function checkCarClassOrder(objCarClass) {
    var ajaxURl = '/RateShopper/CarClass/CheckCarClassOrder/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.CarClassOrder;
    }
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        data: { 'CarClassOrder': objCarClass.CarClassOrder, 'carClassCode': objCarClass.Code },
        success: function (data) {
            if (data.status) {
                var carName = data.carclass;
                var message = data.carclass + " is already using this car class order.Do You want to re-arrange car class order?"
                positionChanged = true;
                ShowConfirmBox(message, true, SaveCarClassDetails, objCarClass);
            }
            else {
                positionChanged = false;
                SaveCarClassDetails(objCarClass);
            }

        },
        error: function (e) {
            console.log("BindCarClasses: " + e.message);
        }
    });

}


/*#End Region*/


/*#End Region*/