
$.extend(true, $.fn.dataTable.defaults, {
    "searching": false,
    "ordering": true,
    "paging": false,
    "info": false,
    "destroy": true
});

$(document).ready(function () {
    $(".totalDue").html($('#TotalDue').val());

    $('#view-billing').removeClass("no-footer");

    $('#total-billing').html($('#TotalBilling').val()); 

    var totalBilling = $('#TotalBilling').val();
    $('#total-due').html($('#TotalDue').val());


    if ($('#AdminFee').val() != undefined && $('#AdminFee').val() != '') {

        $('#admin-fee').html('<span>The current admin fee is $' + $('#AdminFee').val() + '.</span>');
    } else {
        $('#admin-fee').html('');
    }
    $('#view-billing').DataTable().destroy();


    $('#view-billing').DataTable({
        columnDefs: [
        { orderable: false, targets: -1 }
        ],
        "order": [[0, "asc"]],
        "language": {
            "emptyTable": "Billings are not available"
        }
    });

    $('#footer-total-billing').html(totalBilling);
    
 

});





