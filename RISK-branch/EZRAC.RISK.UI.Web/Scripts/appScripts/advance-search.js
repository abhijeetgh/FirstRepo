$(document).ready(function () {
    $("#linkSearchResults").css("display", "inline");
    //Double click opeation    
    $("#AdvancedSearchGrid .claim-followup-date").dblclick(function () {
        $(this).find("#followUpDate").addClass("display-none");
        $(this).addClass('db-clicked').find(".sandbox-container").removeClass("display-none");
    });

    $("#AdvancedSearchGrid .claim-assigned-coloumn").dblclick(function () {
        $(this).find(".center-align").removeClass("display-none");
        $(this).find("#assignedUser").addClass("display-none");
    });
    //end Double click opeation
    var dateToday = new Date();
    $(".datepicker").datepicker({
        autoclose: true,
        minDate: dateToday,
        beforeShowDay: $.datepicker.noWeekends
    }).on("click", function () {
        $(this).datepicker('show');
    });

    $("#AdvancedSearchGrid .notes-icon").on("click", function () {
        var claimId = getClaimId($(this).parents().eq(3).find("td.claim-id-coloumn").text());
        $("#claimIdForNotes").val(claimId);
        getNotesForClaim(claimId, $(this));
        $(".dropdown-toggle").attr("data-toggle", "dropdown");
        $(this).parent().attr("data-toggle", "");
        $(this).parent().parent().addClass('open');
    });


    $("#AdvancedSearchGrid .claim-followup-date .correct-icon").on("click", function () {
        var previousDate = $(this).parents().eq(2).find("#followUpDate").text();
        if (previousDate != $(this).parent().find("input").val()) {
            var claimId = getClaimId($(this).parents().eq(2).find(".claim-id-coloumn").text());
            AdvancedSearchChangesFollowUpDate(claimId, $(this).parent().find("input").val(), previousDate);
            console.log(previousDate + " " + $(this).parent().find("input").val() + " " + $(this).parents().eq(2).find(".claim-id-coloumn").text());
        }
        $(this).parents().eq(2).find(".sandbox-container").addClass("display-none");
        $(this).parents().eq(2).find("#followUpDate").text($(this).parent().find("input").val()).removeClass("display-none");
    });


    $("#AdvancedSearchGrid .claim-followup-date .incorrect-icon").on("click", function () {
        $(this).parents().eq(2).find("#followUpDate").removeClass("display-none");
        $(this).parents().eq(2).find(".sandbox-container").addClass("display-none");
        $(this).parent().parent().removeClass("db-clicked");
    });

    $("#AdvancedSearchGrid .claim-assigned-coloumn .correct-icon").on("click", function (e) {

        var previousUserId = $(this).parents().eq(2).find("#assignedUserId").text();
        if (parseInt(previousUserId) != parseInt($(this).parent().find('select').val())) {
            var claimId = getClaimId($(this).closest("tr").find('.claim-id-coloumn').text());

            AdvancedSearchChangesAssignedTo(claimId, $(this).parent().find('select').val(), previousUserId);

            $(this).parents().eq(2).find("#assignedUserId").text($(this).parent().find('select').val());
            $(this).parents().eq(2).find("#assignedUser").text($(this).parent().find('select option:selected').text());
        }
        $(this).parents().eq(2).find(".center-align").addClass("display-none");
        $(this).parents().eq(2).find("#assignedUser").removeClass("display-none").text($(this).parents().eq(2).find("#assignedUser").text());

    });

    $("#AdvancedSearchGrid .claim-assigned-coloumn .incorrect-icon").on("click", function (e) {
        $(this).parents().eq(2).find(".center-align").addClass("display-none");
        $(this).parents().eq(2).find("#assignedUser").removeClass("display-none");
    });

    //Select all Claim list using checkbox
    $(".grey-white-table .check-header input").change(function () {
        //Advanced search checkbox select all
        if ($(".claim-group-button .active").attr("id") == "linkSearchResults") {
            $("#AdvancedSearchGrid .child-checkbox input").prop('checked', $(this).prop("checked"));
        }
    });

    //List of record display dropdown operation
    $("#divSearchResults .show-record").change(function () {
        getAdvancedSearchCurrentShowRecords($("#divSearchResults .show-record").val());
        $("#AdSearchClaimsRecordsToShow").val($("#divSearchResults .show-record").val());
    });
});



//---------------Other functions ------------------------------

//Column level sorting
function AdvancedSearchOperation(data) {
    var pageNo = $(data).attr("page")
    var sortBy = $(data).attr("sortby");
    var recordsToShow = $(data).attr("recordcounttodisply");
    var sortOrder = $(data).attr("sortorder");

    //console.log(pageNo + " " + sortBy + " " + recordsToShow + " " + sortOrder + " global " + AdSearchSortBy + " " + AdSearchSortOrder);
    GetCommonAdvancedSearchResult(pageNo, sortBy, sortOrder, recordsToShow);
}
//--------------- End Other functions ------------------------------


//--------------- Ajax calling function ----------------------------
function DeleteAdvancedSearchClaims(claimsToDelete) {
    //var dataRows = $(".grey-white-table tr.dataRow").length;
    var isContractSearch = isNaN($("#txtSearchClaim").val());
    var adSearchCt = AdvancedSearchCriteria(isContractSearch);
    var pageNo = $("#AdSearchClaimsPageNo").val();
    var sortBy = $("#AdSearchClaimsSortBy").val();
    var recordsToShow = $("#AdSearchClaimsRecordsToShow").val();
    var sortOrder = $("#AdSearchClaimsSortOrder").val();
    var token = $('input[name="__RequestVerificationToken"]').val();

    var resultDiv = $("#divSearchResults");
    $.ajax({
        url: deleteAdvancedSearchClaimsUrl,
        data: JSON.stringify({
            'claimIds': claimsToDelete,
            'page': pageNo,
            'sortBy': sortBy,
            'sortOrder': !sortOrder,
            'recordCountToDisplay': recordsToShow,
            'claimSearchDto': adSearchCt
            //__RequestVerificationToken: token
        }),
        contentType: 'application/json; charset=utf-8',
        type: "POST",
        cache: false,
        success: function (returnData) {
            ////$("#linkSearchResults").css("display", "inline");
            // ShowSearchResults();
            resultDiv.html(returnData);
            //$("#gridcontainer").html(returnData);
        },
        error: onErrorHandler,
    });
}

function AdvancedSearchChangesAssignedTo(claimId, assignedTo, previousUserId) {

    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: AdSearchChangeClaimAssignedToUrl,
        data: {
            claimId: claimId,
            assignedTo: assignedTo,
            __RequestVerificationToken: token
        },
        type: "POST",
        cache: false,
        success: function (returnData) {
            if (!returnData) {
                //Find the selected claim row
                var RootTr = $("#AdvancedSearchGrid tbody input[id=" + claimId + "]").closest("tr");

                //Assign ID to label
                $(RootTr).find(".claim-assigned-coloumn label[id=assignedUserId]").text(previousUserId);

                //Assign text to label
                var SelectedUserText = $(RootTr).find(".claim-assigned-coloumn option[value=" + previousUserId + "]").text();
                $(RootTr).find(".claim-assigned-coloumn label[id=assignedUser]").text(SelectedUserText);

                //Set selectbox option
                $(RootTr).find(".claim-assigned-coloumn option[value=" + previousUserId + "]").attr("selected", "selected").siblings().removeAttr("selected", "selected");
            }
        },
        error: onErrorHandler,
    });
}

function AdvancedSearchChangesFollowUpDate(claimId, date, previousDate) {

    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: AdSearchChangeFollowupDateUrl,
        data: {
            claimId: claimId,
            date: date,
            __RequestVerificationToken: token
        },
        type: "POST",
        cache: false,
        success: function (returnData) {
            if (!returnData) {
                //Find the selected claim row
                $("#AdvancedSearchGrid tbody input[id=" + claimId + "]").closest("tr").find(".datepicker").val(previousDate);
                $("#AdvancedSearchGrid tbody input[id=" + claimId + "]").closest("tr").find(".claim-followup-date label").html(previousDate);
            }
        },
        error: onErrorHandler,
    });

}

//user want to change list of record showing on grid
function getAdvancedSearchCurrentShowRecords(recordToGet) {
    var sortBy = "Id";
    var sortOrder = $("#AdSearchClaimsSortOrder").val();

    var pageNo = $("#divAdvancedSearch .pagination-list .active a").html();
    if (pageNo == undefined) {
        pageNo = 1;
    }
    GetCommonAdvancedSearchResult(pageNo, sortBy, sortOrder, recordToGet);
}
//--------------- End ajax function --------------------------------