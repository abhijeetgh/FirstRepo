$(document).ready(function () {
    resetUserData();
    $("#addUser").on("click", function () {
        resetUserData();
    });
});

//---------ajax call functions-----------
var GetUserDatails = function (userID) {

    $.ajax({
        url: SelectedUserUrl,
        data: {
            userId: userID
            //__RequestVerificationToken: token
        },
        type: "GET",
        cache: false,
        //async: true,
        //dataType: "html",
        success: function (returnData) {
            $("#UserEditProfile").html(returnData);
            $("#UserName").attr("disabled", "disabled");
            $("#heddinglable").html("Update User Profile");
        },
        error: onErrorHandler,
    });
}
var DeleteUser = function (userID) {
    if (typeof (userID) == 'undefined') {
        userID = this;
    }
    //var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: DeleteUserUrl,
        data: {
            userId: userID,
            //__RequestVerificationToken: token
        },
        type: "POST",
        cache: false,
        async: true,
        dataType: "html",
        success: function (returnData) {
            $("#UserList").html(returnData);
            if ($("#UserEditProfile .hiddenUserID").val() == userID) {
                resetUserData();
            }
        },
        error: onErrorHandler,
    });
}
var CheckDeleteUser = function (userID) {
    $.ajax({
        url: CheckDeleteUserUrl,
        data: {
            userId: parseInt(userID)
            //__RequestVerificationToken: token
        },
        type: "GET",
        //cache: false,
        //async: true,
        dataType: "json",
        success: function (returnData) {
            if (returnData == "True") {
                ShowConfirmBox("Can not delete user already in use.", false);
                //alert("Can not delete user already in use.");
                //$("#errormsg").show();
            }
            else {
                ShowConfirmBox("Do you want to delete the user?", true, DeleteUser, userID);
                //var flag = confirm("Do you want to delete the user?");
                //if (flag) {
                //    $("#errormsg").hide();
                //    DeleteUser(userID);
                //}
            }
        },
        error: onErrorHandler,
    });
}
var CheckUserName = function (username, userId) {
    $.ajax({
        url: CheckUserNameUrl,
        data: {
            username: username,
            userId: parseInt(userId)
            //__RequestVerificationToken: token
        },
        type: "GET",
        //cache: false,
        //async: true,
        dataType: "json",
        success: function (returnData) {
            if (returnData == "True") {
                $("#checkusername").show();
            }
            else {
                $("#checkusername").hide();
            }
        },
        error: onErrorHandler,
    });
}

//---------End Ajax call functions-------

//--------Other functions---------
var resetUserData = function () {
    //$("#errormsg").hide();
    $("#heddinglable").html("New User Profile");
    $("#UserName").removeAttr("disabled");
    $("#UserEditProfile input:text,input:password").val("");
    $("#UserEditProfile .hiddenUserID").val("0");
    $('#UserRoleId').selectpicker('val', "");
    $("input[name=IsActiveUser]").eq(0).prop("checked", true);
    $("#UserEditProfile input:hidden[id=IsActive]").val("True");
    $("#isactive").val("true");

    $("#cancel").attr("disabled", "disabled");
    $("#Submitform").attr("disabled", "disabled");
    $("#UserList table").dataTable().$('tr.grey_bg').removeClass("grey_bg");
    $("#checkusername").hide();
    $(".field-validation-error span").text("");
    $(".anothertabletoul input").prop("checked", false);
}
var selectedUserProfile = function () {
    $("#UserList table tbody .field").on("click", function () {
        resetUserData();
        $("#UserList table").dataTable().$('tr.grey_bg').removeClass("grey_bg");
        $(this).closest("tr").addClass("grey_bg");
        var userID = $(this).closest("tr").find(".userID").val();
        if (userID != undefined && userID != null) {
            GetUserDatails(userID);
        }
    });
    $("#UserList table tbody .delete-action").on("click", function () {
        var userID = $(this).closest("tr").find(".userID").val();
        var CurrentLoggedUserId = $("#CurrentLoggedUserId").val()
        if (userID != undefined && userID != null) {
            if (CurrentLoggedUserId != userID) {
                CheckDeleteUser(userID);//Check if user as assigned active claims or not
                //DeleteUser(userID);
            }
            else {
                if (CurrentLoggedUserId == userID) {
                    ShowConfirmBox("Current user should not be delete.", false);
                }
                //alert("Current user should not be delete...");
            }
        }
    });
}
var RemoveDisableButton = function () {
    $("#cancel").removeAttr("disabled");
    $("#Submitform").removeAttr("disabled");
}
//--------End Other functions---------