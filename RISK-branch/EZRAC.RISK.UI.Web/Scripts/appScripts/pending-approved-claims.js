
$(document).ready(function () {

    $(".show-record-pendingClaims").change(function () {
        getCurrentShowRecordsForPendingClaims($(".show-record-pendingClaims").val());
        $("#pendingClaimsRecordsToShow").val($(".show-record-pendingClaims").val());
    });


    $(".pendingPageNo").click(function () {
        var showRecordValue = $(".show-record-pendingClaims").val();
        if (showRecordValue != undefined) {
            this.href = this.href.replace("xx", showRecordValue);
        }
        else {
            this.href = this.href.replace("xx", 5);
        }
    });

    $(".approve-coloumn .correct-icon").click(function () {
     
        var url = $(this).attr("data-url");
        var claimId = $(this).attr("data-id");
        getApprovaNotesForm(url, claimId);
    });

    $(".approve-coloumn .incorrect-icon").click(function () {
        var url = $(this).attr("data-url");
        var claimId = $(this).attr("data-id");
        getRejectNotesForm(url, claimId);
    });

    $(".pending-claims-notes").click(function () {   
        var claimId = getClaimId($(this).parents().eq(3).find("td.claim-id-coloumn").text());
        $("#claimIdForNotes").val(claimId);
        getNotesForClaim(claimId, $(this));
    });

    $('#notes .dropdown-toggle').on('click', function (event) {
        $(".dropdown-toggle").attr("data-toggle", "dropdown");
        $(this).attr("data-toggle", "");
        $(this).parent().addClass('open');
    });

    $(".notes-icon").click(function () {
        var claimId = getClaimId($(this).parents().eq(3).find("td.claim-id-coloumn").text());
        $("#claimIdForNotes").val(claimId);
        getNotesForClaim(claimId, $(this));
        $(this).parent().attr("data-toggle", "");
        $(this).parent().parent().addClass('open');
    });

    $('body').on('click', function (e) {
        if (!$('.dropdown-toggle').is(e.target) && $('.dropdown-toggle').has(e.target).length === 0 && $('.open').has(e.target).length === 0) {
            $('.dropdown-toggle').removeClass('open');
            $('.dropdown-toggle').attr("data-toggle", "dropdown");
        }
    });

    if (parseInt($("#pendingTotalRecords").val()) < 1) {
        $(".paging-footer-pending .info").removeClass('pull-right');
        $(".paging-footer-pending").addClass('text-center');
    }
    else {
        $(".paging-footer-pending .info").addClass('pull-right');
        $(".paging-footer-pending").removeClass('text-center');
    }
});



function getRejectNotesForm(url,id) {
    $.ajax({
        url: url,
        type: "Get",
        cache: false,
        success: function (returnData) {
            $("#popup").html(returnData);
            $("#claimId").val(id);
            $("#popup").find("#rejectPop").modal('show');
        },
        error: onErrorHandler,
    });

}

function getApprovaNotesForm(url, id) {
    $.ajax({
        url: url,
        type: "Get",
        cache: false,
        success: function (returnData) {
            $("#popup").html(returnData);
            $("#claimId").val(id);
            $("#popup").find("#approvalPop").modal('show');
        },
        error: onErrorHandler,
    });

}

