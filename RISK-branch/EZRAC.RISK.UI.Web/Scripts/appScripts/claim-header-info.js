$(document).ready(function (e) {

    $(".selectpicker").selectpicker({});

    var dateToday = new Date();
    $(".datepicker-followupdate").datepicker({
        minDate: dateToday,
        beforeShowDay: $.datepicker.noWeekends
    });

    $.validator.unobtrusive.parse("#frm-save-claim-header");

    if ($("#hasAccessToApprove").val().toLowerCase() == 'true')
    {
        $(".approveReject").show();
    }
    else {
        $(".approveReject").hide();
    }


    $("#followup-date").change(function () {
        $('#followupDateChangeFlag').val(true);
    });

    $('#frm-save-claim-header').submit(function () {
        
        var orgFollowupDate = new Date($('#originalFollowUpDate').val());

        var currentFollowupDate = new Date($('#followup-date').val());

        if (orgFollowupDate.getTime() != currentFollowupDate.getTime() && $('#followupDateChangeFlag').val().toLowerCase() == "true") {

            $.ajax({
                url: $("#add-followup-url").data('url'),
                type: "Get",
                cache: false,
                success: function (returnData) {
                    $("#popup").html(returnData);
                    $("#popup").find("#update-followup-date-Pop").modal('show');

                    $("#update-followup-date").click(function () {

                        if ($("#followupNoteDescription").val() == "") {
                            $("#errorDescription").text("Please enter note");
                            return false;
                        }

                        addQuickNotes($("#ClaimID").val(), NotesType.ChangingFollowUp, $("#followupNoteDescription").val(), false);

                        $('.modal').remove();
                        $('.modal-backdrop').remove();
                        $('body').removeClass("modal-open");
                        $('#update-followup-date-Pop').modal('hide');

                        $('#followupDateChangeFlag').val(false);
                        $('#frm-save-claim-header').submit();
                    });
                },
                error: onErrorHandler,
            });
            return false;
        }
    });
});


function approveClaim(url, id) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: url,
        data: { id: id, status: true, __RequestVerificationToken: token },
        type: "POST",
        cache: false,
        success: function (data) {
            $("#claim-header-secton").html(data);
        },
        error: onErrorHandler,
    });
}

var NotesType = {
    ChangingFollowUp: 8,
   
}

   
