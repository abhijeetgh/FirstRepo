$(document).ready(function () {
    $(".selectpicker").selectpicker({});
    $.validator.setDefaults({
        ignore: []
    });
    $.validator.unobtrusive.parse("#add-writeoff-form");
    $.extend(true, $.fn.dataTable.defaults, {
        "searching": false,
        "ordering": true,
        "paging": false,
        "info": false,
        "destroy": true
    });
    $('#writeOffTable').DataTable({
        columnDefs: [
        { orderable: false, targets: -1 }
        ],
        "order": [[0, "asc"]],
        "language": {
            "emptyTable": "WriteOff are not available"
        }
    });

});



var clearValues = function () {
    $("#Amount").val("");
    $('#WriteOffTypes').selectpicker('val', "");
    $(".field-validation-error span").text("");
}

var addWriteOffSuccess = function (data) {
    if (data.indexOf("field-validation-error") > -1) {
        $("#addWriteOffDiv").html(data);
    }
    else {
        //$("#view-writeoff").html("");
        $("#view-writeoff").html(data);
        $(".field-validation-error").text("");
        clearValues();
    }
    RefreshPayment();
    $(".selectpicker").selectpicker({});
}
var DeleteWriteOff = function (thisObj, writeOffId, claimId) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    //if (confirm('Do you want to delete billing')) {

    ShowConfirmBox("Do you want to delete writeoff", true, function () {

        $.ajax({
            url: $(thisObj).data('url'),
            data: {
                WriteOffId: writeOffId,
                __RequestVerificationToken: token,
                ClaimId: claimId
            },
            type: "POST",
            cache: false,
            success: function (returnData) {
                $("#view-writeoff").html(returnData);
                RefreshPayment();
            },
            error: onErrorHandler,
        });

    });
}
var RefreshPayment = function () {
    $("#total-billing").html($("#writeoff").val());
    $("#total-due").html($('#due').val());
    $(".totalDue").html($('#total-due').html());
}