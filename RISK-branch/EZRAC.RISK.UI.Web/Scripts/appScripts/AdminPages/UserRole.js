var IsCopyCreate = false;
var IsNew = false;
$(document).ready(function () {
    resetUserRolForm();
    $("#RoleForm").show();
    //Previous implemented user rolelist grid operation
    //$("#UserRoleListTable tbody .field").on("click", function () {
    //    $(this).closest("tr").addClass("grey_bg").siblings().removeClass("grey_bg");
    //    var roleID = $(this).closest("tr").find(".roleID").val();
    //    if (roleID != undefined && roleID != null) {
    //        GetSelectedRoleProfile(roleID);
    //    }
    //});
    $("#cancelStatus").on("click", function () {
        //$("#UserRoleListTable tbody").find(".grey_bg td").click();
        //var flag = confirm("Do you want to discard changes?");
        //if (flag) {
            resetUserRolForm();
            $("#RoleForm").show();
        //}
    });
    //$("#UserList table tbody").find(".grey_bg td").eq(0).click();
    $("#UserRoleUpdate input:checkbox").on("change", function () {
        RemoveDisableButton();
    });
    $("#UserRoleHeader input:text").bind("input", function () {
        RemoveDisableButton();
    });

    $("#saveStatus").on("click", function () {
        var RoleName = $.trim($("#RoleName").val());
        var deletedPermission = [], addPermission = [], selectedItem = [];
        $("#UserRoleUpdate input:checkbox:checked").each(function () {
            if ($.inArray($(this).val().toString(), $("#OriginalPermission").val().split(",")) == -1) {
                addPermission.push($(this).val());
            }
            selectedItem.push($(this).val());
        });
        deletedPermission = $("#OriginalPermission").val().split(',').filter(function (obj) { return selectedItem.toString().split(',').indexOf(obj) == -1 });
        //console.log(addPermission.toString() + " " + $("#OriginalPermission").val() + " " + deletedPermission.toString() + " " + selectedItem.toString());

        var RoleID = ($("#CurrentSelectedRoleID").val() != "" ? $("#CurrentSelectedRoleID").val() : 0);
        var DuplicateName = false;
        $("#UserRoleList option").each(function () {
            var RoleOptionText = $.trim($(this).text()).toLowerCase();
            var RoleOptionId = $(this).val();
            if (RoleName.toLowerCase() == RoleOptionText) {
                if (IsNew || IsCopyCreate) {
                    $("#erromsg").text("Role name already exists.").show();
                    DuplicateName = true;
                    return false;
                }
                else if (RoleOptionId != RoleID) {
                    $("#erromsg").text("Role name already exists.").show();
                    DuplicateName = true;
                    return false;
                }
            }
        })
        if (DuplicateName) {
            return false;
        }

        if (RoleName != "") {
            $("#erromsg").hide();
            saveUserRolePermission(addPermission.toString(), deletedPermission.toString(), RoleName);
        }
        else {
            $("#erromsg").text("Please enter role name").show();
        }
    });
    //setTimeout(function () {
    //    $("#UserRoleListTable tbody tr td").eq(0).click();
    //}, 250);

    $("#UserRoleList").on("change", function () {
        $("#RoleForm").hide();
        IsNew = false;
        IsCopyCreate = false;
        $("#erromsg,#savesuccess").hide();
        if ($(this).val() == 0)
        {
            $("#lblRoleName").text("");
            $("#cancelStatus").attr("disabled", "disabled");
            $("#saveStatus").attr("disabled", "disabled");
            resetUserRolForm();
            $("#RoleForm").show();
            return false;
        }
        
        GetSelectedRoleProfile($(this).val());
    });
    $("#CopyNCreate").on("click", function () {
        if ($("#UserRoleList").val() == "0") {
            //alert("please select role");
            $("#erromsg").text("Please select any role").show();
        }
        else {
            
            $("#erromsg").hide();
            IsNew = false;
            IsCopyCreate = true;
            $("#UserRoleUpdate input:hidden").val("");
            $("#UserRoleHeader input:text").val("");
            $("#UserRoleHeader input:text"), focus();
            $("#RoleForm").show();
            RemoveDisableButton();
        }
    });
});

//-----Ajax function------
var GetSelectedRoleProfile = function (roleID) {
    $("#UserRoleUpdate input:checkbox").prop("checked", false);

    $.ajax({
        url: SelectedRoleUrl,
        data: {
            roleID: roleID
            //__RequestVerificationToken: token
        },
        type: "GET",
        //cache: false,
        //async: true,
        //contentType: 'application/json; charset=utf-8',
        success: function (returnData) {
            $("#CurrentSelectedRoleID").val(returnData.Id);
            $("#OriginalPermission").val(returnData.PermissionIds);
            $("#lblRoleName").text(returnData.Name);
            $("#RoleName").val(returnData.Name);
            $("#UserRoleUpdate input:checkbox").each(function () {
                if ($.inArray($(this).val().toString(), returnData.PermissionIds.split(",")) != -1) {
                    $(this).prop("checked", true);
                }
            });
            $("#cancelStatus").attr("disabled", "disabled");
            $("#saveStatus").attr("disabled", "disabled");
        },
        //error: function (data) {
        //    resetUserRolForm();
        //    console.log("user role permissions are not found");
        //}
        error: onErrorHandler,
    });
}
var saveUserRolePermission = function (addPermission, deletePermission, RoleName) {
    var RoleID = ($("#CurrentSelectedRoleID").val() != "" ? $("#CurrentSelectedRoleID").val() : 0);
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: SaveRoleUrl,
        data: {
            addPermission: addPermission,
            deletePermission: deletePermission,
            roleID: parseInt(RoleID),
            isCopyCreate: IsCopyCreate,
            isNew: IsNew,
            roleName: RoleName,
            __RequestVerificationToken: token
        },
        type: "POST",
        //cache: false,
        //async: true,
        //dataType: "json",
        //contentType: 'application/json',
        success: function (returnData) {
            $("#savesuccess").show();
            //console.log(returnData);
            if (IsCopyCreate || IsNew) {
                
                $("#UserRoleList").append("<option value=" + returnData.RoleId + " selected=selected>" + returnData.RoleName + "</option>");
                $('#UserRoleList').selectpicker('refresh');
                //$("#CurrentSelectedRoleID").val(returnData.RoleId);
                $('#UserRoleList').selectpicker('val', returnData.RoleId);
                IsCopyCreate = false;
                IsNew = false;
            }
            else {
                $("#UserRoleList option[value=" + RoleID + "]").text(RoleName);
                //$('#UserRoleList').selectpicker('refresh');
            }
            $("#lblRoleName").text(RoleName);
            $("#OriginalPermission").val(returnData.PermissionIds);
            //            $("#UserRoleListTable tbody").find(".grey_bg td").click();
            setTimeout(function () { $("#erromsg,#savesuccess").hide(); }, 1000);
        },
        error: onErrorHandler,
    });
}


//-----End Ajax function------

//------Other functions-----------
var RemoveDisableButton = function () {
    $("#cancelStatus").removeAttr("disabled");
    $("#saveStatus").removeAttr("disabled");
}
var resetUserRolForm = function () {
    IsCopyCreate = false;
    IsNew = true;    
    $("#UserRoleUpdate input:checkbox").prop("checked", false);
    $("#UserRoleUpdate input:text").val("");
    $("#lblRoleName").text("");
    $("#UserRoleUpdate input:hidden").val("");
    //$("#UserRoleList option[value=0]").prop("selected", true);
    $("#erromsg,#savesuccess").hide();
    $(".field-validation-error span").text("");
    $('#UserRoleList').selectpicker('refresh');
    $('#UserRoleList').selectpicker('val', "0");
    $("#cancelStatus").attr("disabled", "disabled");
    $("#saveStatus").attr("disabled", "disabled");
}
//------End Other functions--------