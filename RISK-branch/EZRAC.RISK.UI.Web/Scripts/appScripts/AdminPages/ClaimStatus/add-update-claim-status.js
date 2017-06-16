$(document).ready(function () {
    $.validator.setDefaults({
        ignore: []
    });

    $.validator.unobtrusive.parse("#claimStatusForm");

    $("#claimStatus").on('input paste', function () {
        $(".claim-stauts-buttons").removeAttr('disabled');
        $("#claimStatus").attr('data-isDirty', true);
    });

    $("#cancelStatus").click(function () {
        var isDirty = $('#claimStatus').attr("data-isdirty");
        if (isDirty) {
            //ShowConfirmBox("Do you want to discard changes?", true, function () {
                clearValues();
                $('.claim-status-table tr').removeClass("rowSelected")
                $("#claimStatus").attr('data-isDirty', false);
            //});      
        }
    });
});

function success(data) {
    if (data.Data == false) {
        $("#errorMessage").text("Claim Status already exists.");
    } else {
        $("#updateContainer").html(data);
        clearValues();
    }
   
}

function clearValues() {
    $("#claimStatus").val("");
    $("#claimStatusId").val(0);
    $("#claimStatusPanelTitle").text("New claim status");
    $(".claim-stauts-buttons").attr('disabled', 'disabled');
    $("#searchStatus").val("");
    $("#errorMessage").text("");
    $(".field-validation-error span").text("");
}

