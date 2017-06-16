
$(document).ready(function () {
    $(".selectpicker").selectpicker({});

    $(".addNotes").on("click", function (e) {

        var noteTypeValue = $(this).parents().eq(2).find(".noteTypes").val();
        var descriptionValue = $(this).parents().eq(2).find(".description-area").val();
        var isNotifyChecked = $(this).parents().eq(2).find("#isNotify").is(":checked");
        if (noteTypeValue == -1) {
            $(".errorMsgNoteType").text("Please select note type");
        }
        if (descriptionValue == "") {
            $(".errorMsgDescription").text("Please enter note description");
        }
        if (noteTypeValue != -1 && descriptionValue != "") {
            addQuickNotes($("#claimIdForNotes").val(), noteTypeValue, descriptionValue, $("#isNotify").is(":checked"), $("#notes-id #IsPrivilege").prop("checked"));
            $('.dropdown-toggle').removeClass('open');
            $('.dropdown-toggle').attr("data-toggle", "dropdown");
            $("#notes-id").empty();
        }
    });

    $(".description-area").keyup(function () {
        if ($(this).val() != "") {
            $(".errorMsgDescription").text("");
        }
    });

    $(".noteTypes").change(function () {
        if ($(this).val() != -1) {
            $(".errorMsgNoteType").text("");
        }
    });


    $('.dropdown-menu').on('click', function () {
        $(".noteTypes").removeClass('open').find('button').attr('aria-expanded', false);
        $(".noteTypes").selectpicker('close');
    })

    //$('.dropdown-toggle').on('click', function (event) {
    //    $(".dropdown-toggle").attr("data-toggle", "dropdown");
    //    $(this).attr("data-toggle", "");
    //    $(this).parent().addClass('open');
    //});


    $('html').on('click', function (e) {
        if (!$('.dropdown-toggle').is(e.target) && $('.dropdown-toggle').has(e.target).length === 0 && $('.open').has(e.target).length === 0) {
            $('.dropdown-toggle').removeClass('open');
            $('.dropdown-toggle').attr("data-toggle", "dropdown");
        }
    });

    var constants = {
        approve: true,
        reject: false,
    }

    var NotesType = {
        ChangingFollowUp: 8,
        RejectNotes: 10,
        Approve: 9
    }

    $("#rejectClaim").click(function () {
        if ($("#rejectNoteDescription").val() == "") {
            $("#errorDescription").text("Please enter note");
            return false;
        }
        var claimId = $("#claimId").val();
        addQuickNotes(claimId, NotesType.RejectNotes, $("#rejectNoteDescription").val(), constants.reject, $("#notes-id #IsPrivilege").prop("checked"))
        approveOrRejectClaim(claimId, false);
        $('.modal').remove();
        $('.modal-backdrop').remove();
        $('body').removeClass("modal-open");
        $('#rejectPop').modal('hide');
    });

    $("#approveClaim").click(function () {
        if ($("#approveNoteDescription").val() == "") {
            $("#errorDescription").text("Please enter note");
            return false;
        }
        var claimId = $("#claimId").val();

        addQuickNotes(claimId, NotesType.Approve, $("#approveNoteDescription").val(), constants.reject, $("#notes-id #IsPrivilege").prop("checked"));

        approveOrRejectClaim(claimId, true);

        $('.modal').remove();
        $('.modal-backdrop').remove();
        $('body').removeClass("modal-open");
        $('#approvalPop').modal('hide');
    });

    $("#update-followup-date").click(function () {

        if ($("#followupNoteDescription").val() == "") {
            $("#errorDescription").text("Please enter note");
            return false;
        }
        var claimId = $("#update-followup-date-Pop #claimId").val();
        var followUpDate = $("#update-followup-date-Pop #follow-update").val();

        changeFollowUpDate(claimId, followUpDate);

        addQuickNotes(claimId, NotesType.ChangingFollowUp, $("#followupNoteDescription").val(), constants.reject, $("#notes-id #IsPrivilege").prop("checked"))

        $('.claim-followup-date .correct-icon').parents().eq(2).find(".sandbox-container").addClass("display-none");
        $('.claim-followup-date .correct-icon').parents().eq(2).find("#followUpDate").text($(this).parent().find("input").val()).removeClass("display-none");

        $('.modal').remove();
        $('.modal-backdrop').remove();
        $('body').removeClass("modal-open");
        $('#update-followup-date-Pop').modal('hide');


    });



    $("#rejectNoteDescription").on('input', function () {
        $("#errorDescription").text("");
    });

});

function addQuickNotes(claimNumber, type, description, notify, isPrivilege) {
    var token = $('input[name="__RequestVerificationToken"]').val();

    var viewModel = {
        ClaimId: claimNumber,
        SelectedNoteTypeId: type,
        Description: description,
        SendNotification: notify,
        IsPrivilege: isPrivilege,
        IsNotesTab: false,
        __RequestVerificationToken: token
    };
    $.ajax({
        url: addQuickNotesUrl,
        data: viewModel,
        type: "POST",
        cache: false,
        success: function (returnData) {
            //debugger;
            //if (returnData) {
            //    alert("Quick Note has been added successfully");
            //}
        },
        error: onErrorHandler,
    });
}