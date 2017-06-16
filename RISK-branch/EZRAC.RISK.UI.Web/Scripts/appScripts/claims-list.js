var isContractSearch = false;

$(document).ready(function () {

    $("#PendingApproval").addClass("active");
    //$("#linkSearchResults").css("display", "none");
    $("#DateFrom,#DateTo,#DateOfLoss").hide();

    getPendingClaims(undefined, 1, "FollowUpDate", true, 5, "PendingApproval");

    $(".pending").click(function () {
        getPendingClaims(this);
    });

    sessionStorage.removeItem("CurrentMenuItem");

    $('#twitterSearch').keydown(function (event) {
        var keyCode = (event.keyCode ? event.keyCode : event.which);
        if (keyCode == 13) {
            $('#startSearch').trigger('click');
        }
    });

    $("#linkFollowUp").click(function () {
        DislayFollowUpResult();
    });

    $("#linkSearchResults").click(function () {
        ShowSearchResults();
    });

    function DislayFollowUpResult() {
        $("#divFollowup").css("display", "block");
        $("#divSearchResults").css("display", "none");
        $("#linkFollowUp").addClass("btn-default active").siblings().removeClass("active btn-default");
    }

    function ShowSearchResults() {
        $("#divFollowup").css("display", "none");
        $("#divSearchResults").css("display", "inline");
        $("#linkSearchResults").addClass("btn-default active").siblings().removeClass("active btn-default");
    }

    $('#txtSearchClaim').keydown(function (event) {
        var keyCode = (event.keyCode ? event.keyCode : event.which);
        if (keyCode == 13) {
            event.preventDefault();
            var claimNumber = $("#txtSearchClaim").val();

            if (claimNumber == "") {
                //alert("Please enter Claim/Contracts");
                ShowConfirmBox("Please enter Claim/Contracts", false);
                return false;
            }
            if (!isNaN(claimNumber)) {
                $.ajax({
                    url: claimSearchUrl + "/" + claimNumber,
                    type: "GET",
                    cache: false,
                    success: function (returnData) {
                        if (returnData == "True") {

                            window.location.href = viewClaimUrl + "/" + claimNumber;
                        }
                        else {
                            //alert("Claim not found");
                            ShowConfirmBox("Claim not found", false);
                        }
                    },
                    error: function (data) {
                        //alert("Claim not found");
                        ShowConfirmBox("Claim not found", false);
                    }
                });
            }
            else {
                var resultDiv = $("#divSearchResults");
                var pageNo = 1;
                var sortBy = "Id";
                var sortOrder = false;
                var defaultRecordToShow = 5;
                isContractSearch = true;
                GetCommonAdvancedSearchResult(pageNo, sortBy, sortOrder, defaultRecordToShow);
                //$.ajax({
                //    url: claimContractSearchUrl + "/" + claimNumber,
                //    type: "GET",
                //    cache: false,
                //    success: function (returnData) {

                //        if (returnData.message) {
                //            $("#linkSearchResults").css("display", "none");
                //            DislayFollowUpResult();
                //            alert("Claim/Contracts are not found");
                //        }
                //        else {
                //            $("#linkSearchResults").css("display", "inline");
                //            ShowSearchResults();;
                //            resultDiv.html(returnData);
                //        }
                //    },
                //    error: function (data) {
                //        alert("Claim /Contracts are not found");
                //    }
                //});

            }
        }
    });

    $('#notes .dropdown-toggle').on('click', function (event) {
        $(".dropdown-toggle").attr("data-toggle", "dropdown");
        $(this).attr("data-toggle", "");
        $(this).parent().addClass('open');
    });

    $('html').on('click', function (e) {
        if (!$('.dropdown-toggle').is(e.target) && $('.dropdown-toggle').has(e.target).length === 0 && $('.open').has(e.target).length === 0) {
            $('.dropdown-toggle').removeClass('open');
            $('.dropdown-toggle').attr("data-toggle", "dropdown");
        }
    });

    //Search Criteria Selection operation
    $("#paramAdvanceSearch").on("change", function () {
        $("#DateFrom,#DateTo,#DateOfLoss,#AdvanceSearchItem").hide();
        $("#DateFrom,#DateTo,#AdvanceSearchItem").val("");
        if ($(this).find("option:selected").attr("searchtype") == "followdaterange" || $(this).find("option:selected").attr("searchtype") == "dateofloss") {
            $("#DateFrom").datepicker('option', { maxDate: null });
            $("#DateTo").datepicker('option', { maxDate: null });
            $("#DateFrom,#DateTo").show();
            if ($(this).find("option:selected").attr("searchtype") == "dateofloss")
            {
                $("#DateTo,#DateFrom").datepicker('option', { maxDate: new Date() });
            }
            //$("#AdvanceSearchItem,#DateOfLoss").hide();
            
        }
            //else if ($(this).find("option:selected").attr("searchtype") == "dateofloss") {
            //    //$("#AdvanceSearchItem,#FollowUpDateFrom,#FollowUpDateTo").hide();
            //    $("#DateOfLoss").show();
            //}

        else if ($(this).find("option:selected").attr("searchtype") == "CreditCardType") {
            $("#AdvanceSearchItem").show();
            $('#AdvanceSearchItem').attr('placeholder', 'Eg: AX,DS,MC,VI');
        } else if ($(this).find("option:selected").attr("searchtype") == "CreditCardExpiration") {
            $("#AdvanceSearchItem").show();
            $('#AdvanceSearchItem').attr('placeholder', 'MMYY');
        } else if ($(this).find("option:selected").attr("searchtype") == "CreditCardNumber") {
            $("#AdvanceSearchItem").show();
            $('#AdvanceSearchItem').attr('placeholder', 'Last four digits');
        }
        else {
            $("#AdvanceSearchItem").show();
            $('#AdvanceSearchItem').attr('placeholder', 'Search Text');
            //$("#FollowUpDateFrom,#FollowUpDateTo,#DateOfLoss").hide();
        }
    });
    $("#AdvancedSearchLink").on("click", function () {
        $("#ResetAdvancedSearch").click();
    });
});

function getPendingClaims(activeGrid, page, sortBy, sortOrder, recordsToShow, claimType) {
   
    var pageNo = $("#pendingClaimsPageNo").val() != undefined ? $("#pendingClaimsPageNo").val() : page;
    var sortBy = $("#pendingClaimsSortBy").val() != undefined ? $("#pendingClaimsSortBy").val() : sortBy;
    var recordsToShow = $("#pendingClaimsRecordsToShow").val() != undefined ? $("#pendingClaimsRecordsToShow").val() : recordsToShow;
    var sortOrder = $("#pendingClaimsSortOrder").val() != undefined ? $("#pendingClaimsSortOrder").val() : sortOrder;
    $(activeGrid).addClass("btn-default active").siblings().removeClass("active btn-default");
    var claimType = $(".pending-claims a.active").attr("id") != undefined ? $(".pending-claims a.active").attr("id") : claimType;

    $.ajax({
        url: getPendingClaimsUrl,
        data: { page: pageNo, sortBy: sortBy, sortOrder: !sortOrder, recordCountToDisplay: recordsToShow, claimType: claimType },
        type: "GET",
        cache: false,
        success: function (returnData) {
            $("#pendingClaimsContainer").html(returnData);
        },
        error: onErrorHandler,
    });
}


function changeAssignedTo(claimId, assignedTo) {

    var pageNo = $("#pageNo").val();
    var sortBy = $("#sortBy").val();
    var recordsToShow = $("#recordsToShow").val();
    var sortOrder = $("#sortOrder").val();
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: changeClaimAssignedToUrl,
        data: {
            claimId: claimId,
            assignedTo: assignedTo,
            page: pageNo,
            sortBy: sortBy,
            sortOrder: !sortOrder,
            recordCountToDisplay: recordsToShow,
            __RequestVerificationToken: token
        },
        type: "POST",
        cache: false,
        success: function (returnData) {
            $("#gridcontainer").html(returnData);

        },
        error: onErrorHandler,
    });
}

function changeFollowUpDate(claimId, date) {

    var pageNo = $("#pageNo").val();
    var sortBy = $("#sortBy").val();
    var recordsToShow = $("#recordsToShow").val();
    var sortOrder = $("#sortOrder").val();
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: changeFollowupDateUrl,
        data: {
            claimId: claimId,
            date: date,
            page: pageNo,
            sortBy: sortBy,
            sortOrder: !sortOrder,
            recordCountToDisplay: recordsToShow,
            __RequestVerificationToken: token
        },
        type: "POST",
        cache: false,
        success: function (returnData) {
            $("#gridcontainer").html(returnData);
        },
        error: onErrorHandler,
    });
}

function approveOrRejectClaim(claimId, approvedOrRejected) {
    var pageNo = $("#pendingClaimsPageNo").val();
    var sortBy = $("#pendingClaimsSortBy").val();
    var recordsToShow = $("#pendingClaimsRecordsToShow").val();
    var sortOrder = $("#pendingClaimsSortOrder").val();
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: approveOrRejectClaimsUrl,
        data: {
            claimId: claimId,
            approvedOrRejected: approvedOrRejected,
            page: pageNo,
            sortBy: sortBy,
            sortOrder: !sortOrder,
            recordCountToDisplay: recordsToShow,
            claimType: $("#pendingClaimsType").val(),
            __RequestVerificationToken: token
        },
        type: "POST",
        cache: false,
        success: function (returnData) {
            $("#pendingClaimsContainer").html(returnData);
        },
        error: onErrorHandler,
    });
}

function getCurrentShowRecordsForFollowupClaims(recordToGet) {

    var sortOrder = $("#sortOrder").val();

    $.ajax({
        url: getCurrentGridDataUrl,
        data: {
            page: 1,
            sortBy: "FollowUpDate",
            sortOrder: !sortOrder,
            recordCountToDisplay: recordToGet
        },
        type: "GET",
        cache: false,
        success: function (returnData) {
            $("#gridcontainer").html(returnData);
        },
        error: onErrorHandler,
    });
}

function getCurrentShowRecordsForPendingClaims(recordToGet) {
    var sortOrder = $("#pendingClaimsSortOrder").val();

    $.ajax({
        url: getPendingClaimsUrl,
        data: {
            page: 1,
            sortBy: "Id",
            sortOrder: !sortOrder,
            recordCountToDisplay: recordToGet,
            claimType: $("#pendingClaimsType").val()
        },
        type: "GET",
        cache: false,
        success: function (returnData) {
            $("#pendingClaimsContainer").html(returnData);
        },
        error: onErrorHandler,
    });
}

function DeleteClaims(claimsToDelete) {
    //var dataRows = $(".grey-white-table tr.dataRow").length;

    var pageNo = $("#pageNo").val();
    var sortBy = $("#sortBy").val();
    var recordsToShow = $("#recordsToShow").val();
    var sortOrder = $("#sortOrder").val();
    var token = $('input[name="__RequestVerificationToken"]').val();

    //if (dataRows == 1)
    //{
    //    var newPage = parseInt($("#pageNo").val());
    //    pageNo = newPage--;
    //}

    $.ajax({
        url: deleteClaimsUrl,
        data: {
            claimIds: claimsToDelete,
            page: pageNo,
            sortBy: sortBy,
            sortOrder: !sortOrder,
            recordCountToDisplay: recordsToShow,
            __RequestVerificationToken: token
        },
        traditional: true,
        type: "POST",
        cache: false,
        success: function (returnData) {
            $("#gridcontainer").html(returnData);
        },
        error: onErrorHandler,
    });
}


function getNotesForClaim(claimId, area) {

    $.ajax({
        url: quickNotesUrl,
        data: { claimNumber: claimId },
        type: "GET",
        cache: false,
        success: function (returnData) {
            $(area).parents().eq(2).find("ul.notes-area").empty().append(returnData);
        },
        error: onErrorHandler,
    });

}

function getClaimId(claimString) {
    return claimString.split('-')[1];
}


function GetadvancedSearchRecords() {
    var sortBy = "Id";
    var recordsToShow = 5;
    var sortOrder = false;
    var token = $('input[name="__RequestVerificationToken"]').val();

    var pageNo = $("#divAdvancedSearch .pagination-list .active a").html();
    if (pageNo == undefined) {
        pageNo = 1;
    }
    isContractSearch = false;
    //console.log(pageNo + " " + sortBy + " " + recordsToShow + " " + sortOrder);
    //Call common function for advanced search option


    GetCommonAdvancedSearchResult(pageNo, sortBy, sortOrder, recordsToShow);

}

//----------------Ajax functions--------------------------
//advanced search ajax call
function GetCommonAdvancedSearchResult(pageNo, sortBy, sortOrder, recordsToShow) {
    var claimSearchDto = new Object();
    claimSearchDto = AdvancedSearchCriteria(isContractSearch);
    var token = $('input[name="__RequestVerificationToken"]').val();

    var flag = AdvancedSearchValidation();
    console.log(flag);
    if (!flag) {
        return false;
    }

    var resultDiv = $("#divSearchResults");

    $.ajax({
        url: advancedSearchUrl,
        data: JSON.stringify({ 'page': pageNo, 'sortBy': sortBy, 'sortOrder': sortOrder, 'recordCountToDisplay': recordsToShow, 'claimSearchDto': claimSearchDto }),
        contentType: 'application/json; charset=utf-8',
        type: "POST",
        cache: false,
        success: function (returnData) {
            //if (returnData.message) {
            //    $("#linkSearchResults").css("display", "none");
            //    DislayFollowUpResult();
            //    alert("Claim/Contracts are not found");
            //}
            //else {            
            $("#linkSearchResults").css("display", "inline");
            ShowSearchResults();;
            resultDiv.html(returnData);
            $("#divSearchResults .show-record option").each(function () {
                if (this.value.toString() == recordsToShow) {
                    $(this).attr("selected", "selected");
                    return false;
                }
            });
            //}
        },
        error: onErrorHandler,
    });
}
//----------------End Ajax functions--------------------------
//----------------Other functions-----------------------------
function AdvancedSearchCriteria(isContractSearch) {
    var claimSearchDto = new Object();
    if (isContractSearch) {
        claimSearchDto.SearchType = "contract";
        claimSearchDto.SearchItem = $.trim($("#txtSearchClaim").val());
        return claimSearchDto;
    }
    claimSearchDto.SearchTypeId = parseInt($("#paramAdvanceSearch option:selected").val());
    claimSearchDto.SearchType = $("#paramAdvanceSearch option:selected").attr("searchtype");
    claimSearchDto.SearchItem = $.trim($("#AdvanceSearchItem").val());

    //var CurrentDate = new Date();
    //CurrentDate = ((CurrentDate.getMonth().toString().length > 1) ? "" : "0") + CurrentDate.getMonth() + "/" + ((CurrentDate.getDate().toString().length > 1) ? "" : "0") + CurrentDate.getDate() + "/" + CurrentDate.getFullYear();

    claimSearchDto.DateFrom = $.trim($("#DateFrom").val());
    claimSearchDto.DateTo = ($.trim($("#DateTo").val()) == "") ? new Date() : $.trim($("#DateTo").val());
    //claimSearchDto.DateOfLoss = $.trim($("#DateOfLoss").val());
    return claimSearchDto;
}
AdvancedSearchValidation = function () {
    if ($("#paramAdvanceSearch").find("option:selected").attr("searchtype") == "followdaterange" || $("#paramAdvanceSearch").find("option:selected").attr("searchtype") == "dateofloss") {
        if ($.trim($("#DateFrom").val()) == "" && $.trim($("#DateTo").val()) == "") {
            //alert("Please select date range");
            ShowConfirmBox("Please select date range", false);
            return false;
        }
        else {
            if ($("#paramAdvanceSearch").find("option:selected").attr("searchtype") == "followdaterange" || $("#paramAdvanceSearch").find("option:selected").attr("searchtype") == "dateofloss") {
                if (Date.parse($("#DateFrom").val()) > Date.parse($("#DateTo").val())) {
                    //alert("To date should be greater than from date.");
                    ShowConfirmBox("To date should be greater than from date.", false);
                    return false;
                }
            }
        }
    }
    else {
        if ($("#paramAdvanceSearch option:selected").attr("searchType") == "0" && $('#txtSearchClaim').val() == "") {
            //alert("Please select search option");
            ShowConfirmBox("Please select search option", false);
            return false;
        }
        if ($.trim($("#AdvanceSearchItem").val()) == "" && $('#txtSearchClaim').val()=="" ) {
            ShowConfirmBox("Please enter search text", false);
            return false;
        }
    }
    return true;
}

var NotesType = {

    ApprovalNotesType: 9

}
//---------------End Other functions-------------------------