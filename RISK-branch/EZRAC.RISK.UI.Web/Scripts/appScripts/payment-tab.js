$(document).ready(function () {
    $(".selectpicker").selectpicker({});

    var dateToday = new Date();
    $(".datepicker").datepicker({
        maxDate: dateToday
        //beforeShowDay: $.datepicker.noWeekends
    });
    $.validator.setDefaults({
        ignore: []
    });

    $.validator.unobtrusive.parse("#addPaymentsForm");

    $(".close-container").click(function () {
        clearValues();
    });

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

});


function clearValues() {
    $("#paymentAmount").val("");
    $('#paymentType').selectpicker('val', "");
    $("#paymentDate").val("");
    $(".field-validation-error span").text("");
}

function addPaymentSuccess(data)
{
    if (data.indexOf("field-validation-error") > -1) {
        $("#addPaymentDiv").html(data);
    }
    else
    {
        $("#gridContainer").html(data);
        $(".field-validation-error").text("");
        clearValues();
    }
    $(".selectpicker").selectpicker({});
    var dateToday = new Date();
    $(".datepicker").datepicker({
        maxDate: dateToday
    });
}
