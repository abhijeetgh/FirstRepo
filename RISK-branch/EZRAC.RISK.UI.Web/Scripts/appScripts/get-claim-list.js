$(document).ready(function () {
    var dateToday = new Date();
    $(".datepicker").datepicker({
        autoclose: true,
        minDate: dateToday,
        beforeShowDay: $.datepicker.noWeekends
    }).on("click", function () {
        $(this).datepicker('show');
    });
    var dateToday = new Date();

    $(".AdvancedSearchDatepickerFrom").datepicker({
        autoclose: true,
        //maxDate: dateToday
        //beforeShowDay: $.datepicker.noWeekends
        //onSelect: function (selectedDate) {
        //    $('.AdvancedSearchDatepickerTo').datepicker('option', { minDate: selectedDate });
        //    if ($(".AdvancedSearchDatepickerTo").val() != "") {
        //        $(".AdvancedSearchDatepickerFrom").datepicker('option', { maxDate: null });
        //    }
        //}
    }).on("click", function () {
        $(this).datepicker('show');
    });
    $(".AdvancedSearchDatepickerTo").datepicker({
        autoclose: true
        //beforeShowDay: $.datepicker.noWeekends
        //onSelect: function (selectedDate) {
        //    $('.AdvancedSearchDatepickerFrom').datepicker('option', { maxDate: selectedDate });
        //    if ($(".AdvancedSearchDatepickerFrom").val() != "") {
        //    $(".AdvancedSearchDatepickerTo").datepicker('option', { maxDate: null });
        //    }
        //}
    }).on("click", function () {
        $(this).datepicker('show');
    });

    $(".claim-followup-date").dblclick(function () {
        $(this).find("#followUpDate").addClass("display-none");
        $(this).addClass('db-clicked').find(".sandbox-container").removeClass("display-none");
    });

    $(".claim-assigned-coloumn").dblclick(function () {
        $(this).find(".center-align").removeClass("display-none");
        $(this).find("#assignedUser").addClass("display-none");
    });


    $('.claim-followup-date .correct-icon').click(function () {
       
        var url = $(this).parent().attr("data-url");
        var claimId = $(this).parent().attr("data-id");
        var followUpDate = $(this).prev().val();

        var previousDate = $(this).parents().eq(2).find("#followUpDate").text();
        if (previousDate != $(this).parent().find("input").val()) {
            var claimId = getClaimId($(this).parents().eq(2).find(".claim-id-coloumn").text());

            getFollowupNotesForm(url, claimId, followUpDate);
      
        }

    });


    $(".claim-followup-date .incorrect-icon").click(function () {
        $(this).parents().eq(2).find("#followUpDate").removeClass("display-none");
        $(this).parents().eq(2).find(".sandbox-container").addClass("display-none");
        $(this).parent().parent().removeClass("db-clicked");
    });


    $(".claim-assigned-coloumn .correct-icon").click(function (e) {
        var previousUserId = $(this).parents().eq(2).find("#assignedUserId").text();
        if (parseInt(previousUserId) != parseInt($(this).parent().find('select').val())) {
            var claimId = getClaimId($(this).closest("tr").find('.claim-id-coloumn').text());
            changeAssignedTo(claimId, $(this).parent().find('select').val());
        }
        $(this).parents().eq(2).find(".center-align").addClass("display-none");
        $(this).parents().eq(2).find("#assignedUser").removeClass("display-none").text($(this).parents().eq(2).find("#assignedUser").text());

    });

    $(".claim-assigned-coloumn .incorrect-icon").click(function (e) {
        $(this).parents().eq(2).find(".center-align").addClass("display-none");
        $(this).parents().eq(2).find("#assignedUser").removeClass("display-none");
    });


    $(".claim-complete-coloumn input").change(function () {
        if ($(this).is(":checked")) {
            var claimId = getClaimId($(this).closest("tr").find(".claim-id-coloumn").text());
            markClaimComplete(claimId);
        }
    });

    $(".notes-icon").click(function () {
        var claimId = getClaimId($(this).parents().eq(3).find("td.claim-id-coloumn").text());
        $("#claimIdForNotes").val(claimId);
        getNotesForClaim(claimId, $(this));
        $(".dropdown-toggle").attr("data-toggle", "dropdown");
        $(this).parent().attr("data-toggle", "");
        $(this).parent().parent().addClass('open');
    });


    $(".grey-white-table .check-header input").change(function () {
        //followup search checkbox select all
        if ($(".claim-group-button .active").attr("id") == "linkFollowUp") {
            $("#divFollowup .child-checkbox input").prop('checked', $(this).prop("checked"));
        }
    });


    // when changing show records drop down
    $(".show-record").change(function () {
        getCurrentShowRecordsForFollowupClaims($(".show-record").val());
        $("#recordsToShow").val($(".show-record").val());
    });

    $(".pageNo").click(function () {
        var showRecordCount = $(".show-record").val();
        if (showRecordCount != undefined) {
            this.href = this.href.replace("xx", showRecordCount);
        } else {
            this.href = this.href.replace("xx", 5);
        }
    });

    $('#delete-claim').on("click", function () {
        if ($(".claim-group-button .active").attr("id") == "linkFollowUp") {
            if ($('#divFollowup input[name="delete-claim-checkbox"]:checked').length > 0) {
                ShowConfirmBox("Do you want to delete selected claims?", true, function () {
                    var claimsToDelete = new Array();
                    $('#divFollowup input[name="delete-claim-checkbox"]:checked').each(function () {
                        claimsToDelete.push(this.id);
                    });

                    DeleteClaims(claimsToDelete);
                });
                //if (confirm("Do you want to delete selected claims?")) {
                //    var claimsToDelete = new Array();
                //    $('#divFollowup input[name="delete-claim-checkbox"]:checked').each(function () {
                //        claimsToDelete.push(this.id);
                //    });

                //    DeleteClaims(claimsToDelete);
                //}
            }
            else {
                //alert("Select at least 1 claim to delete");
                ShowConfirmBox("Select at least 1 claim to delete", false);
            }
        }
        //This one is for advanced search checkbox delete operation.
        if ($(".claim-group-button .active").attr("id") == "linkSearchResults") {
            if ($('#AdvancedSearchGrid input[name="delete-claim-checkbox"]:checked').length > 0) {
                ShowConfirmBox("Do you want to delete selected advanced claims?", true, function () {
                    var claimsToDelete = new Array();
                    $('#AdvancedSearchGrid input[name="delete-claim-checkbox"]:checked').each(function () {
                        claimsToDelete.push(this.id);
                    });
                    DeleteAdvancedSearchClaims(claimsToDelete);
                });
                //if (confirm("Do you want to delete selected advanced claims?")) {
                //    var claimsToDelete = new Array();
                //    $('#AdvancedSearchGrid input[name="delete-claim-checkbox"]:checked').each(function () {
                //        claimsToDelete.push(this.id);
                //    });
                //    DeleteAdvancedSearchClaims(claimsToDelete);
                //}
            }
            else {
                //alert("Select at least 1 claim to delete");
                ShowConfirmBox("Select at least 1 claim to delete", false);
            }
        }
    });


    //$("#linkSearchResults").css("display", "none");
    $("#linkFollowUp").click(function () {
        DislayFollowUpResult();
    });

    $("#linkSearchResults").click(function () {
        ShowSearchResults();
    });

    if (parseInt($("#totalRecords").val()) < 1) {
        $("#divFollowup .info").removeClass('pull-right');
        $("#divFollowup").find('.paging-footer').addClass('text-center');
    }
    else {
        $("#divFollowup .info").addClass('pull-right');
        $("#divFollowup").find('.paging-footer').removeClass('text-center');
    }

    $("#ResetAdvancedSearch").on("click", function () {
        $('#paramAdvanceSearch').selectpicker('val', "1");
        $("#paramAdvanceSearch").selectpicker({});
        $("#DateFrom,#DateTo,#AdvanceSearchItem").val("");
        $("#AdvanceSearchItem").show();
        $("#DateFrom,#DateTo,#DateOfLoss").hide();
        
        //$("#DateFrom").datepicker('option', { maxDate: new Date() });
        //$("#DateTo").datepicker('option', { maxDate: null });
    });
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

var NotesType = {
    ChangingFollowUp: 8,

}
function getFollowupNotesForm(url, id, followUpDate) {
    //debugger;
    $.ajax({
        url: url,
        type: "Get",
        cache: false,
        success: function (returnData) {
            //debugger;
            $("#popup").html(returnData);

            $("#update-followup-date-Pop #claimId").val(id);
            $("#update-followup-date-Pop #follow-update").val(followUpDate);
            $("#popup").find("#update-followup-date-Pop").modal('show');
        },
        error: onErrorHandler,
    });

}