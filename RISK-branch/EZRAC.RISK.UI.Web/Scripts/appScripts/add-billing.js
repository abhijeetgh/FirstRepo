$(document).ready(function (e) {

    $.validator.setDefaults({
        ignore: []
    });

    $(".selectpicker").selectpicker({});
    $.validator.unobtrusive.parse("#add-billing-form");

    
    $('#save-labour-hours').click(function () {
        
        var labourLours = $('.labour-hours-value').val();
        //alert(labourLours);
        if (labourLours == '' || labourLours == undefined || !CheckDecimalVal(labourLours)) {
            $('#labour-hours-required').show();
            return false;
        } else {
            $('#labour-hours-required').hide();
        }

        
    });

    $('.labour-hours-value').change(function () {
        var labourLours = $('.labour-hours-value').val();
        if (labourLours == '' || labourLours == undefined) {
            $('#labour-hours-required').show();
            return false;
        } else {
            $('#labour-hours-required').hide();
        }
    });

    function CheckDecimalVal(data)
    {
        var decimalOnly = /^\s*-?[0-9]\d*(\.\d{1,2})?\s*$/;
        if (data !== '') {
            if (decimalOnly.test(data)) {
                return true;
            } else {
                return false;
            }
        } else {
            return false;
        }
        return;

    }

});

function DeleteBilling(thisObj, billingId,claimId) {

    var token = $('input[name="__RequestVerificationToken"]').val();
    //if (confirm('Do you want to delete billing')) {

    ShowConfirmBox("Do you want to delete billing", true, function () {

        $.ajax({
            url: $(thisObj).data('url'),
            data: {
                id: billingId,
                __RequestVerificationToken: token,
                claimId: claimId
            },
            type: "POST",
            cache: false,
            success: function (returnData) {
                $("#view-billing").html(returnData);
            },
            error: onErrorHandler,
        });

    });
}

function clearBillingValues() {
    $("#Amount").val("");
    $("#Discount").val("");
    $('#billing-types').selectpicker('val', "");
    $(".field-validation-error span").text("");

}
