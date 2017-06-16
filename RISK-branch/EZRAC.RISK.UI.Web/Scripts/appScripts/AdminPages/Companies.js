$(document).ready(function () {
    resetCompanyForm();
    $("#addCompany").on("click", function () {
        resetCompanyForm();
    });
});


//-------Ajax Functions-------
function GetCompanyDatails(companyId) {
    $.ajax({
        url: GetCompanyDetailsUrl,
        data: {
            companyId: companyId
            //__RequestVerificationToken: token
        },
        type: "GET",
        cache: false,
        //async: true,
        dataType: "html",
        success: function (returnData) {
            $("#CompanyEditProfile").html(returnData);
            //$("#UserName").attr("disabled", "disabled");
            $("#heddinglable").html("Update Company Profile");
        },
        error: onErrorHandler,
    });
}
function DeleteCompany(companyId) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: DeleteCompanyUrl,
        data: {
            companyId: companyId,
            __RequestVerificationToken: token
        },
        type: "POST",
        cache: false,
        async: true,
        //dataType: "html",
        success: function (returnData) {
            //console.log(returnData);
            if (returnData == "True") {
                //if current user selected in update mode then we have to reset form
                if (companyId == parseInt($("#CompanyEditProfile .hiddenCompanyID").val())) {
                    resetCompanyForm();
                }
                $("#CompanyList table tbody .companyID[value=" + companyId + "]").closest("tr").remove();
            }
            else {
                //alert("Can not delete company already in use.");
                ShowConfirmBox("Can not delete company already in use.", false);
                //$("#errormsg").show();
            }
            //$("#UserList").html(returnData);
        },
        error: onErrorHandler,
    });
}
function IsCompanyMapped(companyId) {
    $.ajax({
        url: CheckDeleteCompanyrUrl,
        data: {
            companyId: parseInt(companyId)
            //__RequestVerificationToken: token
        },
        type: "GET",
        //cache: false,
        //async: true,
        dataType: "json",
        success: function (returnData) {
            if (returnData == "True") {
                //alert("Can not delete company already in use.");
                ShowConfirmBox("Can not delete company already in use.", false);
                //$("#errormsg").show();
            }
            else {
                //var flag = confirm("Do you want to delete the company?");
                //if (flag) {
                //    $("#errormsg").hide();
                //    DeleteCompany(companyId);
                //}
                ShowConfirmBox("Do you want to delete the company?", true, function () {
                    $("#errormsg").hide();
                    DeleteCompany(companyId);
                });
            }
        },
        error: onErrorHandler,
    });
}
//------End Functions--------

//-------Other Functions---------
var GetCompanyById = function () {
    $("#CompanyList table tbody .field").on("click", function () {
        resetCompanyForm();
        $("#tblCompanyList").dataTable().$('tr.grey_bg').removeClass("grey_bg");
        $(this).closest("tr").addClass("grey_bg");
        var CompanyId = $(this).closest("tr").find(".companyID").val();
        if (CompanyId != undefined && CompanyId != null) {
            GetCompanyDatails(CompanyId);
        }
    });
}
var DeletCompanyById = function () {
    $("#CompanyList table tbody .delete-action").on("click", function () {
        var CompanyId = $(this).closest("tr").find(".companyID").val();
        IsCompanyMapped(CompanyId);
    })
}
var resetCompanyForm = function () {
    //$("#errormsg").hide();
    $("#heddinglable").html("New Company Profile");
    $("#adminProfile input:text").val("");
    $("#adminProfile textarea").val("");
    $("#checkabbrcode").hide();
    $("#CompanyEditProfile .hiddenCompanyID").val("0");
    $("#cancel").attr("disabled", "disabled");
    $("#Submitform").attr("disabled", "disabled");
    $("#tblCompanyList").dataTable().$('tr.grey_bg').removeClass("grey_bg");
    $(".field-validation-error span").text("");   
}
var RemoveDisableButton = function () {
    $("#cancel").removeAttr("disabled");
    $("#Submitform").removeAttr("disabled");
}
//---.----End Other Functions------