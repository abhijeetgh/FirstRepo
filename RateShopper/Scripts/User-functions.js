var userViewModel;
var UserID = 0;
var sortOrder = "ASC";
var sortUserBy = "username";
var FirstLoadUser = true;
var PriceMgrLocation = [];
$(document).ready(function () {
    userViewModel = new UserViewModel();
    ko.applyBindings(userViewModel);
    BindUser();
    
    $("#AddUser").on("click", function () {
        EmptyField();
        $('#searchUser').focus();
    });
    $("#reset").on("click", function () {
        if (UserID != 0) {
            $("#username_" + UserID).click();
        }
        else {
            $("#AddUser").click();
        }
    });

    $(".LocationBrand").on("click", function () {
        var id = $(this).val();
        var locText = $("label[for=LocationBrandCheckbox_" + id + "]").html();
        var IsDominentBrand = $(this).attr("isdominentbrand");
        if ($(this).prop("checked")) {
            if (JSON.parse(IsDominentBrand)) {
                var mgrLoc = new Object();
                mgrLoc.id = id;
                mgrLoc.loctext = locText;
                mgrLoc.selected = false;
                PriceMgrLocation.push(mgrLoc);

                if ($("#mainDivPriceMgr select option").eq(0).val() == "0") {
                    $("#mainDivPriceMgr select").empty();
                    $("#mainDivPriceMgr input:text,select").prop("disabled", false);
                }
                $("#mainDivPriceMgr select").append($("<option></option>")
                .attr("value", id)
                .text(locText));
            }
        }
        else {
            $("#mainDivPriceMgr select option[value=" + id + "]").remove();
            PriceMgrLocation = $.grep(PriceMgrLocation, function (item) {
                return (item.loctext.toUpperCase().indexOf(locText.toUpperCase()) == 0);
            });
        }
        if ($("#mainDivPriceMgr select option").length == 0) {
            PriceMgrLocation = [];
            $("#mainDivPriceMgr input:text").val("").prop("disabled", true);
            $("#mainDivPriceMgr select").empty().append("<option value=0>Not Available</option>").prop("disabled", true);
        }

    });

    $('#searchUser').on('input', function () {
        UserSmartSearch();
    });
    $('#searchUser').focus();
    $("#save").on("click", function () {

        //Update case
        var user = new Object();
        var userDTO = new Object();
        if (ValidationUserData()) {

            if ($("input[type=radio]").eq(0).prop("checked")) {
                user.UserRoleID = parseFloat($("input[type=radio]").eq(0).attr("userroleid"));
                userDTO.UserRoleID = parseFloat($("input[type=radio]").eq(0).attr("userroleid"));
            }
            else if ($("input[type=radio]").eq(1).prop("checked")) {
                user.UserRoleID = parseFloat($("input[type=radio]").eq(1).attr("userroleid"));
                userDTO.UserRoleID = parseFloat($("input[type=radio]").eq(1).attr("userroleid"));
            }
            user.FirstName = $.trim($("#FirstName").val());
            user.LastName = $.trim($("#LastName").val());
            user.UserName = $.trim($("#UserName").val());
            user.EmailAddress = $.trim($("#EmailID").val());

            if ($("#ActivateTether").prop("checked")) {
                user.IsTetheringAccess = 1;
            }
            if ($("#AutomationUser").prop("checked")) {
                user.IsAutomationUser = 1;
            }
            if ($("#DisableTSDUpdate").prop("checked")) {
                user.IsTSDUpdateAccess = 0;
            }
            else {
                user.IsTSDUpdateAccess = 1;
            }
            user.Password = $.trim($("#txtConfirmPassword").val());

            var selectedSource = "";
            $(".Source").each(function () {
                if ($(this).prop("checked")) {
                    selectedSource += $(this).val() + ",";
                }
            });
            selectedSource = selectedSource.substr(0, selectedSource.length - 1);

            var selectedLocationBrand = "";
            $(".LocationBrand").each(function () {
                if ($(this).prop("checked")) {
                    selectedLocationBrand += $(this).val() + ",";
                }
            });
            selectedLocationBrand = selectedLocationBrand.substr(0, selectedLocationBrand.length - 1);
            userDTO.LoggedUserID = $('#LoggedInUserId').val();

            var selectedPermission = "";
            $("input[type=checkbox].dynamicpermission").each(function () {
                if ($(this).prop("checked")) {
                    selectedPermission += $(this).val() + ",";
                }
            });
            selectedPermission = selectedPermission.substr(0, selectedPermission.length - 1);

            var selectedPriceMgrLocationID = "";
            selectedPriceMgrLocationID = ($("#mainDivPriceMgr select").val() != null) ? $("#mainDivPriceMgr select").val().toString() : "";

            var deleteLocationBrand = [], deleteSource = [], deleteuserPermission = [], deletePriceMgrLocationId = [];
            var addLocationBrand = [], addSource = [], adduserPermission = [], addPriceMgrLocationId = [];
            //Update case
            if (UserID != 0) {
                user.ID = UserID;
                userDTO.UserID = UserID;
                var hiddenSourceID = $("#selectedSourceID").val();
                var hiddenLocationBrand = $("#selectedLocationBrandID").val();
                var hiddenPermissionId = $("#selectedPermission").val();
                var hiddenPriceMgtLocationId = $("#PriceMgrLocationBrand").val();

                deleteSource = hiddenSourceID.split(',').filter(function (obj) { return selectedSource.split(',').indexOf(obj) == -1; });
                deleteLocationBrand = hiddenLocationBrand.split(',').filter(function (obj) { return selectedLocationBrand.split(',').indexOf(obj) == -1; });
                deleteuserPermission = hiddenPermissionId.split(',').filter(function (obj) { return selectedPermission.split(',').indexOf(obj) == -1; });
                deletePriceMgrLocationId = hiddenPriceMgtLocationId.split(',').filter(function (obj) { return selectedPriceMgrLocationID.split(',').indexOf(obj) == -1; });

                if (hiddenPermissionId != "" && hiddenPermissionId != undefined) {
                    for (var i = 0; i < selectedPermission.split(',').length ; i++) {
                        if ($.inArray(selectedPermission.split(',')[i], hiddenPermissionId.split(',')) == -1) {
                            adduserPermission.push(selectedPermission.split(',')[i]);
                        }
                    }
                }
                else {
                    adduserPermission = selectedPermission.toString();
                }

                if (hiddenSourceID != "" && hiddenSourceID != undefined) {
                    for (var i = 0; i < selectedSource.split(',').length ; i++) {
                        if ($.inArray(selectedSource.split(',')[i], hiddenSourceID.split(',')) == -1) {
                            addSource.push(selectedSource.split(',')[i]);
                        }
                    }
                }
                else {
                    addSource = selectedSource.toString();
                }

                if (hiddenLocationBrand != "" && hiddenLocationBrand != undefined) {
                    for (var i = 0; i < selectedLocationBrand.split(',').length ; i++) {
                        if ($.inArray(selectedLocationBrand.split(',')[i], hiddenLocationBrand.split(',')) == -1) {
                            addLocationBrand.push(selectedLocationBrand.split(',')[i]);
                        }
                    }
                }
                else {
                    addLocationBrand = selectedLocationBrand.toString();
                }

                if (hiddenPriceMgtLocationId != "" && hiddenPriceMgtLocationId != undefined) {
                    for (var i = 0; i < selectedPriceMgrLocationID.split(',').length ; i++) {
                        if ($.inArray(selectedPriceMgrLocationID.split(',')[i], hiddenPriceMgtLocationId.split(',')) == -1) {
                            addPriceMgrLocationId.push(selectedPriceMgrLocationID.split(',')[i]);
                        }
                    }
                }
                else {
                    addPriceMgrLocationId = selectedPriceMgrLocationID.toString();
                }
                //console.log("locationbrand " + hiddenLocationBrand + "  " + hiddenSourceID);
                //console.log("source add "+addSource.toString() + " delete " + deleteSource)
                //console.log("location add " + addLocationBrand.toString() + " delete " + deleteLocationBrand);
                //Dynamic Permission access
                userDTO.AddUserPermission = adduserPermission.toString();
                userDTO.DeleteUserPermission = deleteuserPermission.toString();

                userDTO.AddSourceID = addSource.toString();
                userDTO.DeleteSourceID = deleteSource.toString();

                userDTO.AddLocationBrandID = addLocationBrand.toString();
                userDTO.DeletLocationBrandID = deleteLocationBrand.toString();

                userDTO.DeletePriceMgrLocationID = deletePriceMgrLocationId.toString();
                userDTO.AddPriceMgrLocationID = addPriceMgrLocationId.toString();
            }
            else {
                //New user Insert case
                user.ID = 0;
                userDTO.UserID = 0;
                userDTO.AddLocationBrandID = selectedLocationBrand;
                userDTO.AddSourceID = selectedSource;
                userDTO.AddUserPermission = selectedPermission;
                userDTO.AddPriceMgrLocationID = selectedPriceMgrLocationID;
            }


            var ajaxURl = '/RateShopper/User/InsertUpdateUser';
            if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
                ajaxURl = AjaxURLSettings.InsertUpdateUserURL;
            }

            //console.log(userDTO);
            //console.log(user);

            $.ajax({
                url: ajaxURl,
                type: 'POST',
                async: true,
                data: JSON.stringify({ 'user': user, 'userDTO': userDTO }),
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (data) {
                    if (data) {
                        $("#spanSave").show();
                        setTimeout(function () { $("#spanSave").hide(); }, 3000);
                        $("#updateMesage").hide();
                        BindUser();
                        EmptyField();
                        //setTimeout(function () {
                        //$("#AddUser").click();
                        $('.loader_container_main').hide();
                        //}, 2600);

                    }
                },
                error: function (e) {
                    $(".loader_container_main").hide();
                    console.log(e.message);
                }
            });
        }
    });

    $("#rcontainer input").on("keyup", function () {
        $("#save").removeAttr("disabled").removeClass("btnDisabled");
        $("#reset").removeAttr("disabled").removeClass("disable-button");
        if ($(this).val() != "") {
            RemoveFlashableTag($(this));
        }
    });

    $("#rcontainer input[type=checkbox],input[type=radio]").not("#searchLocation").on("change", function () {
        $("#save").removeAttr("disabled").removeClass("btnDisabled");
        $("#reset").removeAttr("disabled").removeClass("disable-button");
        if ($(this).val() != "") {
            RemoveFlashableTag($(this));
        }
    });
    $("#rcontainer select").on("change", function () {
        $("#save").removeAttr("disabled").removeClass("btnDisabled");
        $("#reset").removeAttr("disabled").removeClass("disable-button");
    })
    //create mode always set as disable mode pricing manager
    $("#mainDivPriceMgr input:text").val("").prop("disabled", true);
    $("#mainDivPriceMgr select").empty().append("<option value=0>Not Available</option>").prop("disabled", true);
    $("#rcontainer input:radio[value=Normal]").prop("checked", true);
    $("#updateMesage").hide();
    $("#searchLocation").bind("input", function () {
        var inputText = $(this).val();
        //console.log(inputText);
        if (inputText.length > 0) {

            BindPriceMgrLoc(true, inputText);
            //$(PriceMgrLocation).each(function () {
            //    var locText = this.loctext;
            //    if (locText.toUpperCase().indexOf(inputText.toUpperCase()) == 0) {
            //        $(this).show();
            //    }
            //    else {
            //        $(this).hide();
            //    }
            //});

        } else {
            BindPriceMgrLoc(false, "");
            $("#mainDivPriceMgr select option").show();
        }
    });
    EventPricingMgrLocation();
});
//ViewModel for RuleSet
function UserViewModel() {
    var self = this;
    self.UserList = ko.observableArray([]);
    self.SortFunction = function () {
        switch (sortUserBy) {
            case "username":
                if (sortOrder == "DESC") {
                    self.UserList.sort(function (left, right) {
                        return left.UserName.toLowerCase() == right.UserName.toLowerCase() ? 0 : (left.UserName.toLowerCase() < right.UserName.toLowerCase() ? 1 : -1)
                    });
                }
                else {
                    self.UserList.sort(function (left, right) {
                        return left.UserName.toLowerCase() == right.UserName.toLowerCase() ? 0 : (left.UserName.toLowerCase() < right.UserName.toLowerCase() ? -1 : 1)
                    });
                }
                break;

            case "firstname":
                if (sortOrder == "DESC") {
                    self.UserList.sort(function (left, right) {
                        return left.FirstName.toLowerCase() == right.FirstName.toLowerCase() ? (left.FirstName.toLowerCase() < right.FirstName.toLowerCase() ? 1 : -1) : (left.FirstName.toLowerCase() < right.FirstName.toLowerCase() ? 1 : -1)
                    });
                }
                else {
                    self.UserList.sort(function (left, right) {
                        return left.FirstName.toLowerCase() == right.FirstName.toLowerCase() ? (left.FirstName.toLowerCase() < right.FirstName.toLowerCase() ? -1 : 1) : (left.FirstName.toLowerCase() < right.FirstName.toLowerCase() ? -1 : 1)
                    });
                }
                break;
            case "lastname":
                if (sortOrder == "DESC") {
                    self.UserList.sort(function (left, right) {
                        return left.LastName.toLowerCase() == right.LastName.toLowerCase() ? (left.LastName.toLowerCase() < right.LastName.toLowerCase() ? 1 : -1) : (left.LastName.toLowerCase() < right.LastName.toLowerCase() ? 1 : -1)
                    });
                }
                else {
                    self.UserList.sort(function (left, right) {
                        return left.LastName.toLowerCase() == right.LastName.toLowerCase() ? (left.LastName.toLowerCase() < right.LastName.toLowerCase() ? -1 : 1) : (left.LastName.toLowerCase() < right.LastName.toLowerCase() ? -1 : 1)
                    });
                }
                break;
        }
    }
}
//End ViewModel

//other operation function
// Using on add click and other save click 
function EventPricingMgrLocation() {
    $("#mainDivPriceMgr select option").on("click", function () {
        //console.log($("#mainDivPriceMgr select option[value=" + $(this).val() + "]").prop("selected"));
        var flag = $("#mainDivPriceMgr select option[value=" + $(this).val() + "]").prop("selected");
        ChangeLocationMgrArray($(this).val(), flag);
    });
}
function ChangeLocationMgrArray(id, flag) {
    PriceMgrLocation.forEach(function (item, i) {
        if ($.inArray(item.id, id.split(',')) != -1) {
            PriceMgrLocation[i].selected = flag;
            //item.selected = flag;
            return false;
        }
    });
}
function BindPriceMgrLoc(isFilter, inputText) {
    $("#mainDivPriceMgr select").empty();
    var result = [];
    if (isFilter) {
        result = $.grep(PriceMgrLocation, function (item) {
            return (item.loctext.toUpperCase().indexOf(inputText.toUpperCase()) == 0);
        });
    }
    else {
        result = PriceMgrLocation;
    }
    $(result).each(function () {
        $("#mainDivPriceMgr select").append($("<option></option>")
            .attr("value", this.id)
             .prop("selected", this.selected)
            .text(this.loctext));
    });
    EventPricingMgrLocation();
}
function EmptyField() {
    PriceMgrLocation = [];
    $("#HaddingTitle").html("CREATE USER");
    $("#updateMesage").hide();
    $("#rcontainer input[type=text]").val("");
    $("#rcontainer input[type=password]").val("");

    $("#rcontainer input[type=hidden]").val("");

    $("#rcontainer input[type=radio]").prop("checked", false);
    $("#rcontainer input[type=checkbox]").prop("checked", false);

    $("#UserName").prop("disabled", false);

    $("#tblUsers").find("tr").removeClass("grey_bg");
    $("#save").attr("disabled", "disabled").addClass("btnDisabled");
    $("#reset").attr("disabled", "disabled").addClass("disable-button");
    $("#rcontainer input:radio[value=Normal]").prop("checked", true);
    $("#lblDeleteMessage").hide();
    UserID = 0;
    RemoveValidation();
    $("#mainDivPriceMgr input:text").val("").prop("disabled", true);
    $("#mainDivPriceMgr select").empty().append("<option value=0>Not Available</option>").prop("disabled", true);
    //$('#searchUser').focus();
    return true;
}
function RemoveValidation() {
    RemoveFlashableTag($("#FirstName,#LastName,#UserName,#txtConfirmPassword,#txtPassword,#EmailID"));
    $("#lblMessage").hide();
    //AddFlashingEffect();
    return true;
}
function boolTemplate(template) {
    return JSON.parse(template());
}
function UserSmartSearch() {
    var $inpuTextSelector = $("#searchUser").val().trim();
    if ($inpuTextSelector.length > 0) {
        $("#tblUsers tbody td span[name='username']").each(function () {
            if ($(this).text().trim().toLowerCase().indexOf($inpuTextSelector.toLowerCase()) >= 0) {
                $(this).closest("tr").show();
            }
            else {
                $(this).closest("tr").hide();
            }
        });
    } else {
        $("#tblUsers tbody tr").show();
    }

    if ($inpuTextSelector.length > 0 && $("#tblUsers tbody tr[style$='display: none;']:not(.remove_if_datafound)").length == $("#tblUsers tbody tr:not(.remove_if_datafound)").length) {
        MakeTagFlashable("#searchUser");
        if ($("#tblUsers tbody tr.remove_if_datafound").length == 0) {
            $("#tblUsers tbody").append("<tr class='remove_if_datafound'><td style='text-align:center;font-size: 1.17em !important; color:red;font-weight: bold;' colspan='4'>No user found.</td></tr>");
            $("#WithoutSorHeader").show();
            $("#WithSorHeader").hide();
        }
    }
    else {
        RemoveFlashableTag("#searchUser");
        $("#tblUsers tbody tr.remove_if_datafound").remove();
        $("#WithoutSorHeader").hide();
        $("#WithSorHeader").show();
    }
    AddFlashingEffect();
}
function ValidationUserData() {
    var flagFirstName = false, flagLastName = false, flagUserName = false, flagEmailID = false, flagUserType = false, flagSource = false, flagLocation = false, flagPassword = false;
    var IsExistUser = false;
    var flag = false;
    var filter = /^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$/;
    $("#spanSave").hide();
    if ($.trim($("#FirstName").val()) == "") {
        MakeTagFlashable($("#FirstName"));
    }
    else {
        RemoveFlashableTag($("#FirstName"));
        flagFirstName = true;
    }

    if ($.trim($("#LastName").val()) == "") {
        MakeTagFlashable($("#LastName"));
    }
    else {
        RemoveFlashableTag($("#LastName"));
        flagLastName = true;
    }

    //username validation
    if ($.trim($("#UserName").val()) == "") {
        MakeTagFlashable($("#UserName"));
    }
    else {
        if (UserID == 0) {
            $("#tblUsers tbody tr[class!='remove_if_datafound']").each(function () {
                if ($(this).find(".username").html() == $.trim($("#UserName").val())) {
                    MakeTagFlashable($("#UserName"));
                    flagUserName = false;
                    IsExistUser = true;
                    //$("#lblMessageCheckUserName").show();
                    return false;
                }
                else {
                    RemoveFlashableTag($("#UserName"));
                    IsExistUser = false;
                    //$("#lblMessageCheckUserName").hide();
                    flagUserName = true;
                }
            });
        }
        else {
            $("#tblUsers tbody tr[class!='remove_if_datafound']").each(function () {
                if (UserID != $(this).attr("UserID")) {
                    if ($.trim($(this).find(".username").html()) == $.trim($("#UserName").val())) {
                        MakeTagFlashable($("#UserName"));
                        flagUserName = false;
                        IsExistUser = true;
                        return false;
                    }
                    else {
                        RemoveFlashableTag($("#UserName"));
                        IsExistUser = false;
                        flagUserName = true;
                    }
                }
                else {
                    flagUserName = true;
                }
            });
        }
    }
    if ($("#rcontainer input[name=userType]").eq(0).prop("checked") || $("#rcontainer input[name=userType]").eq(1).prop("checked")) {
        //console.log("true");
        flagUserType = true;
    }
    else {
        flagUserType = false;
        //console.log("false");
    }
    if (UserID == 0) {
        //insert Case
        var confirmpassword = $.trim($("#txtConfirmPassword").val());
        var Password = $.trim($("#txtPassword").val());
        var EmailID = $.trim($("#EmailID").val().trim());
        if (Password != "" && confirmpassword != "") {
            if (Password == confirmpassword) {
                flagPassword = true;
                RemoveFlashableTag($("#txtConfirmPassword"));
                RemoveFlashableTag($("#txtPassword"));
            }
            else {
                MakeTagFlashable($("#txtConfirmPassword"));
                MakeTagFlashable($("#txtPassword"));
                flagPassword = false;
            }
        }
        else {
            MakeTagFlashable($("#txtConfirmPassword"));
            MakeTagFlashable($("#txtPassword"));
            flagPassword = false;
        }
        if (EmailID == "") {
            flagEmailID = true;
            RemoveFlashableTag($("#EmailID"));
        }
        else if (EmailID != "") {
            if (filter.test(EmailID)) {
                flagEmailID = true;
                RemoveFlashableTag($("#EmailID"));
            }
            else {
                flagEmailID = false;
                MakeTagFlashable($("#EmailID"));
            }
        }
    }
    else {
        //update case
        var confirmpassword = $.trim($("#txtConfirmPassword").val());
        var Password = $.trim($("#txtPassword").val());
        var EmailID = $.trim($("#EmailID").val());

        if (Password != "" && confirmpassword != "") {
            if (Password == confirmpassword) {
                flagPassword = true;
                RemoveFlashableTag($("#txtConfirmPassword"));
                RemoveFlashableTag($("#txtPassword"));
            }
            else {
                MakeTagFlashable($("#txtConfirmPassword"));
                MakeTagFlashable($("#txtPassword"));
                flagPassword = false;
            }
        }
        else {
            flagPassword = true;
        }

        if (EmailID == "") {
            flagEmailID = true;
            RemoveFlashableTag($("#EmailID"));
        }
        else if (EmailID != "") {
            if (filter.test(EmailID)) {
                flagEmailID = true;
                RemoveFlashableTag($("#EmailID"));
            }
            else {
                flagEmailID = false;
                MakeTagFlashable($("#EmailID"));
            }
        }
        //flagPassword = true;
        //flagEmailID = true;
    }
    //if ($("#EmailID").val() != "") {
    //    MakeTagFlashable($("#EmailID"));
    //}
    //else {
    //    RemoveFlashableTag($("#EmailID"));
    //    flagEmailID = true;
    //}

    //flagUserType = true;

    //if ($("#userType:checked").val() != undefined) {
    //    flagUserType = true;
    //}

    //if ($("#Source input[type=checkbox]:checked").length == 0) {
    //    $("#spanSourceError").show();
    //}
    //else {
    //    $("#spanSourceError").hide();
    //    flagSource = true;
    //}


    //if ($("#LocationBrand input[type=checkbox]:checked").length == 0) {
    //    $("#spanLocationError").show();
    //}
    //else {
    //    $("#spanLocationError").hide();
    //    flagLocation = true;
    //}

    //console.log(flagFirstName + "  " + flagLastName + " " + flagUserName + " " + flagEmailID + " " + flagUserType + " " + flagSource + " " + flagLocation);
    if (flagFirstName == true && flagLastName == true && flagUserName == true && flagUserType == true && flagPassword == true && flagEmailID == true) {
        flag = true;
        $("#lblMessage").hide();
        // $("#spanLocationError").hide();
        // $("#spanSourceError").hide();
    }
    else {
        flag = false;
        if (IsExistUser) {
            $("#lblMessageCheckUserName").show();
        }
        else {
            $("#lblMessageCheckUserName").hide();
        }
        if ($("#rcontainer").find(".temp") != "")
            $("#lblMessage").show();
    }

    AddFlashingEffect();
    return flag;
}
function ApplySorting(control, sortBy) {

    var prevSortOrder = $(control).attr("sort");
    sortUserBy = sortBy;//"username";
    if (prevSortOrder == "Asc") {
        $(control).attr("sort", "Desc");
        sortOrder = "DESC";
        $(control).find("img").eq(0).show();
        $(control).find("img").eq(1).hide();
        //$(control).find("img").attr("src", "../images/ardown.png");
    }
    else {
        $(control).attr("sort", "Asc");
        $(control).find("img").eq(1).show();
        $(control).find("img").eq(0).hide();
        //$(control).find("img").attr("src", "../images/arup.png");
        sortOrder = "ASC";
    }
    userViewModel.SortFunction();
}
//End other operation function

//Ajax functions
function DeleteUser() {
    $('#tblUsers tbody  img').click(function () {
        var message = 'Are you sure you want to delete user?';
        ShowConfirmBox(message, true, DeleteUserCallBack, this);
        return false;
    });

    $('#tblUsers tbody tr[UserId="' + parseInt($('#LoggedInUserId').val()) + '"] img').unbind('click').css("cursor", "default").css("opacity", "0.6");
    $('#tblUsers tbody tr[UserId="' + parseInt($('#LoggedInUserId').val()) + '"] img').click(function (e) {
        e.stopPropagation();
        return false;
    });

}

function DeleteUserCallBack() {
    if ($(this).attr("alt") == "Delete") {
        $('.loader_container_main').show();
        var DeleteUserID = $(this).closest("tr").attr("UserID");
        var ajaxURl = '/RateShopper/User/DeleteUser';
        var selectedTR = $(this).closest("tr");

        if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
            ajaxURl = AjaxURLSettings.DeleteUserURL;
        }
        $.ajax({
            url: ajaxURl,
            type: 'POST',
            async: true,
            data: { UserID: DeleteUserID, LoggedInUserId: $('#LoggedInUserId').val() },
            success: function (data) {
                $("#user_m").scrollTop(0);
                if (data == "success") {
                    $(selectedTR).remove();
                    $('.loader_container_main').hide();
                    $("#lblDeleteMessage").hide();
                    if (DeleteUserID == UserID) {
                        $("#AddUser").click();
                    }
                }
                else {
                    $("#lblDeleteMessage").show();
                    setTimeout(function () {
                        $("#lblDeleteMessage").hide();
                    }, 10000);

                    $('.loader_container_main').hide();
                }
            },
            error: function (e) {
                $('.loader_container_main').hide();
                console.log(e.message);
            }
        });
    }
    return false; // OR stop event bubbling
}

function selectedUser(data) {
    var attr = $("#reset").prop("disabled");
    if (typeof attr === typeof undefined || attr === true) {
        ResetAndFetchSelectedUser(data);
    } else {
        ShowConfirmBox("Do you want to discard the changes?", true, DisableSaveButtonCallBack, data);
    }
}

function DisableSaveButtonCallBack() {
    $("#save").attr("disabled", "disabled").addClass("btnDisabled");
    $("#reset").attr("disabled", "disabled").addClass("disable-button");
    ResetAndFetchSelectedUser(this)
}

function ResetAndFetchSelectedUser(data) {
    $("#HaddingTitle").html("UPDATE / VIEW USER");
    $('.loader_container_main').show();
    $("#spanSave").hide();
    RemoveValidation();
    $("#updateMesage").show();
    $("#username_" + data.UserID).closest("tr").addClass("grey_bg").siblings().removeClass("grey_bg");
    //$("#username_" + data.UserID).closest("tr").siblings().removeClass("grey_bg");
    $("#txtPassword").val("");
    $("#txtConfirmPassword").val("");
    $("#UserName").val(data.UserName);
    if ($('#LoggedInUserId').val() == data.UserID) {
        $("#UserName").prop("disabled", true);
    }
    else {
        $("#UserName").prop("disabled", false);
    }
    $("#FirstName").val(data.FirstName);
    $("#LastName").val(data.LastName);
    $("#EmailID").val(data.EmailID);
    $("#UserRoleID").val(data.UserRoleID);
    if (boolTemplate(data.IsAutomationUser)) {
        $("#AutomationUser").prop("checked", true);
    }
    else {
        $("#AutomationUser").prop("checked", false);
    }

    if (boolTemplate(data.IsTetheringAccess)) {
        $("#ActivateTether").prop("checked", true);
    }
    else {
        $("#ActivateTether").prop("checked", false);
    }

    if (boolTemplate(data.IsTSDUpdateAccess)) {
        $("#DisableTSDUpdate").prop("checked", true);
    }
    else {
        $("#DisableTSDUpdate").prop("checked", false);
    }
    //  $("input[type=checkbox].dynamicpermission").prop("checked", false)

    UserID = data.UserID;
    var UserRoleID = data.UserRoleID;

    var ajaxURl = '/RateShopper/User/selectedUserData';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.selectedUserDataURL;
    }
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        data: { userID: UserID, userRoleID: UserRoleID },
        async: true,
        success: function (data) {
            $('.loader_container_main').hide();
            if (data) {

                $("#selectedLocationBrandID").val(data.LocationBrandID);
                $("#selectedSourceID").val(data.SourceID);
                $("#selectedPermission").val(data.SelectedPermissionID);
                if (data.UserRole == "Admin") {
                    $("input[type=radio]").eq(1).prop("checked", true);
                }
                if (data.UserRole == "Normal") {
                    $("input[type=radio]").eq(0).prop("checked", true);
                }

                if (data.SelectedPermissionID != null) {
                    $(".dynamicpermission").each(function () {
                        if ($.inArray($(this).val(), data.SelectedPermissionID.split(",")) != -1) {
                            $(this).prop("checked", true);
                        }
                        else {
                            $(this).prop("checked", false);
                        }
                    });
                }
                else {
                    $(".dynamicpermission").prop("checked", false);
                }

                if (data.SourceID != null) {
                    $(".Source").each(function () {
                        if ($.inArray($(this).val(), data.SourceID.split(",")) != -1) {
                            $(this).prop("checked", true);
                        }
                        else {
                            $(this).prop("checked", false);
                        }
                    });
                }
                else {
                    $(".Source").prop("checked", false);
                }

                if (data.LocationBrandID != null) {
                    $(".LocationBrand").each(function () {
                        if ($.inArray($(this).val(), data.LocationBrandID.split(",")) != -1) {
                            $(this).prop("checked", true);
                        }
                        else {
                            $(this).prop("checked", false);
                        }
                    });
                }
                else {
                    $(".LocationBrand").prop("checked", false);
                }
                $("#mainDivPriceMgr select").empty();
                PriceMgrLocation = [];
                for (var i = 0; i < data.LocationBrandID.split(",").length; i++) {
                    var mgrLoc = new Object();
                    var id = data.LocationBrandID.split(",")[i];
                    var loctext = $("label[for=LocationBrandCheckbox_" + id + "]").html();
                    var IsDominentBrand = JSON.parse($("#LocationBrandCheckbox_" + id).attr("isdominentbrand"));

                    mgrLoc.id = id;
                    mgrLoc.loctext = loctext;
                    mgrLoc.selected = false;
                    if (IsDominentBrand) {
                        if ($.inArray(id.toString(), data.PriceMgrLocationID.split(',')) == -1) {
                            $("#mainDivPriceMgr select").append($("<option></option>")
                         .attr("value", id)
                         .text(loctext));
                        }
                        else {
                            mgrLoc.selected = true;
                            $("#mainDivPriceMgr select").append($("<option></option>")
                            .attr("value", id)
                             .prop("selected", true)
                            .text(loctext));
                        }
                        PriceMgrLocation.push(mgrLoc);
                    }
                }
                $("#mainDivPriceMgr input:text,select").prop("disabled", false);
                $("#mainDivPriceMgr #PriceMgrLocationBrand").val(data.PriceMgrLocationID);

                //$(".LocationBrand[value=47]")

                // var rsrcs = $.map(data, function (item) { return new User(item); });
                //userViewModel.UserList(rsrcs);
                //$("#recentLengths ul li").eq(0).addClass("selected");
                EventPricingMgrLocation();
            }
        },
        error: function (e) {
            $('.loader_container_main').hide();
            console.log(e.message);
        }
    });
}

function BindUser() {
    var internalUserID = UserID;
    $('.loader_container_main').show();
    var ajaxURl = '/RateShopper/User/GetUserList';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetUserURL;
    }
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        async: true,
        success: function (data) {
            $('.loader_container_main').hide();
            if (data) {
                var rsrcs = $.map(data, function (item) { return new User(item); });
                userViewModel.UserList(rsrcs);
                //$("#recentLengths ul li").eq(0).addClass("selected");
                if ($.trim($('#searchUser').val()) != "") {
                    $('#searchUser').keyup();
                }
                if (internalUserID == 0 && !(FirstLoadUser)) {
                    $("#user_m").scrollTop($("#user_m")[0].scrollHeight);
                } else if (FirstLoadUser) {
                    $("#user_m").scrollTop(0);
                    FirstLoadUser = false;
                }
                else {
                    $("#user_m").scrollTop(0);
                }
                //if ($("#tblUsers thead tr td").eq(2).attr("sort") == "Desc") {
                //    ApplySorting($("#tblUsers thead tr td").eq(2), 'username');
                //}
                DeleteUser();
            }
        },
        error: function (e) {
            $('.loader_container_main').hide();
            console.log(e.message);
        }
    });
}
//End AjaxFunction


//Entity
function User(data) {
    this.UserID = data.ID;
    this.UserRoleID = data.UserRoleID;
    this.UserName = data.UserName;
    this.FirstName = data.FirstName;
    this.LastName = data.LastName;
    this.EmailID = data.EmailAddress;
    this.SelectedPermission = data.SelectedPermissionID;
    this.IsAutomationUser = ko.computed(function () {
        if (data.IsAutomationUser)
            return "true";
        else
            return "false";
    });
    this.IsTetheringAccess = ko.computed(function () {
        if (data.IsTetheringAccess)
            return "true";
        else
            return "false";
    });
    this.IsTSDUpdateAccess = ko.computed(function () {
        if (data.IsTSDUpdateAccess)
            return "false";
        else
            return "true";
    });
}
//End Entity
