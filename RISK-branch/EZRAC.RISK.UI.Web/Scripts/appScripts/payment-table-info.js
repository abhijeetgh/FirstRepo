$(document).ready(function () {
    $.extend(true, $.fn.dataTable.defaults, {
        "searching": false,
        "ordering": true,
        "paging": false,
        "info": false,
        "destroy": true
    });

    $('#paymentsTable').DataTable({
        columnDefs: [
        { orderable: false, targets: -1 }
        ],
        "order": [[0, "asc"]],
        "language": {
            "emptyTable": "Payments are not available"
        }
    });

    $('#paymentsTable').removeClass("no-footer").parent().removeClass("dataTables_scrollBody");

    $("#paymentsTable").on("click", '.delete', function () {
        var paymentId = $(this).parents().eq(2).find("input[name='PaymentId']").val();
        ShowConfirmBox("Do you want to delete payment?", true,
            function () {
           deletePayment(paymentId);
        });
        //var confirmDelete = confirm("Do you want to delete payment?");

        //if (confirmDelete) {var paymentId = $(this).parents().eq(2).find("input[name='PaymentId']").val();
        //    deletePayment(paymentId);
        //    
        //}
    });

    $("#totalPayment").html($("#payments").val());
    $(".totalDue").html($('#due').val());

})


function deletePayment(paymentId) {

    var viewModel = {
        ClaimId: $("#claimIdForNotes").val(),
        PaymentId: paymentId,
    }
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: $('#deletePayment').data('url'),
        data: { claimId: $("#claimIdForNotes").val(), paymentId: paymentId, __RequestVerificationToken: token },
        type: "POST",
        cache: false,
        success: function (returnData) {
            if (returnData != null || returnData != undefined) {
                $("#gridContainer").html(returnData);
            }
        },
        error: onErrorHandler,
    });
}