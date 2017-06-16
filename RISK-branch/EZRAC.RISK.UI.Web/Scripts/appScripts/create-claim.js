$(document).ready(function (e) {

    var dateToday = new Date(); 
    $(".datepickerDateOfLoss").datepicker({
        maxDate: dateToday,
        //beforeShowDay: $.datepicker.noWeekends
    });

    $(document).ajaxStart(function () {
        $("#wait").css("display", "block");
    });
    $(document).ajaxComplete(function () {
        $("#wait").css("display", "none");
    });
    
    String.prototype.isEmpty = function () {
        return (this.length === 0 || !this.trim());
    };

    $('#FetchDetails').click(function () {

        var contractNo = $('#contractNo').val();
        $('#IsContractFetched').val("true");

        if (!contractNo.isEmpty()) {
            $.ajax({
                method: "GET",
                url: $('#FetchDetailsUrl').data('url'),
                data: { contractNumber: contractNo },
                success: function (data) {
                    if (!data.isEmpty() && data != "ContractNotFound") { // Need to remove hard code message check
                        $('#FetchedContractInfo').html(data);
                        $('#contractNoRequired').hide();
                        $('#fetchRequired').hide();
                        $('#FetchedContractInfo').addClass('collapse in');

                    } else {
                        $('#contractNoRequired').show();
                        $('#fetchRequired').hide();
                    }
                },
                error: onErrorHandler,
            });
        } else {

            $('#contractNoRequired').show();
            $('#fetchRequired').hide();
        }
    });

    $('#contractNo').change(function () {
        $('#IsContractFetched').val("false");
    });

    $('#createClaimsbmt').click(function(event){
       
      

        $('#SelectedUnitNumber').val($('input[name=UnitNumber]:checked').val());

        if ($('#contractNo').val() != null && $('#contractNo').val() != '') {
            if ($('#IsContractFetched').val() == "false") {
                $('#contractNoRequired').hide();
                $('#fetchRequired').show();
                return false;
            }
            
        }

        if ($('#verifiedContractDetails').prop('checked') ||
                $('#verifiedContractDetails').prop('checked') == undefined ||
                            $('#verifiedContractDetails').is(":visible") == false) {
            // something when checked
            $('#verifyContractValidation').hide();
            //alert("Checked");
        } else {
            // something else when not
            $('#verifyContractValidation').show();
            return false;
        }

        //$('#contractNoRequired').hide();
        $('#fetchRequired').hide();

        var dateIn = new Date($('#contract-date-in').html()).setHours(0, 0, 0, 0);
        var dateOut = new Date($('#contract-date-out').html()).setHours(0, 0, 0, 0);
        var dateOfLoss = new Date($('#DateOfLoss').val()).setHours(0, 0, 0, 0);
        var lossType = $('#loss-type').val();

        // $('#loss-type').val()
        if ((dateOfLoss < dateOut || dateOfLoss > dateIn) && lossType == Constant.LossTypes.StolenVehicle) {
            //alert(ShowConfirmBox("Date of loss is beyond contract period do you want to continue?", true, undefined));
            event.preventDefault();
            ShowConfirmBox("Date of loss is beyond contract period do you want to continue?", true, function () {
                $('#frm-createClaim').submit();
            });

        }
                         
    });

 
});


var Constant = {

    LossTypes: {
        StolenVehicle: "2",
        
    }
}