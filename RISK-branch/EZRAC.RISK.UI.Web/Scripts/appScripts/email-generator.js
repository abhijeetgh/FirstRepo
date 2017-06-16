var Constant = {

        EmailRecipients: {
            PrimaryDriver: 1,
            AdditionalDriver: 2,
            ThirdPartyDriver: 3,
            AdditionalDriverInsurance: 4,
            PrimaryDriverInsurance: 5,
            ThirdPartyDriverInsurance: 6,
            OtherRiskUser: 7,
            CustomEmail: 8
        }
    }

$(document).ready(function (e) {
    $.validator.setDefaults({
        ignore: []
    });

    $('.for-credit-card[value=12]').attr('disabled', 'disabled');

    $('.for-credit-card[value=12]').removeClass('info-to-send');

    $.validator.unobtrusive.parse("#email-generator-send");

    $(".selectpicker").selectpicker({});

   
    $(".notesDescription").shorten();

    $('#send-email').click(function () {

        // alert('tets');
        var recipient = $("#receipients option:selected").val();
        if (recipient == undefined || recipient == '') {
            $('#recipient-required').removeClass('display-none');
            return false;
        } else {

            $('#recipient-required').addClass('display-none');
        }

        //alert(recipient);
        if (recipient == Constant.EmailRecipients.OtherRiskUser) {

            if ($('#risk-user-emails option:selected').val() == undefined) {
                $('#risk-user-required').removeClass('display-none');
                return false;
            }else{
                $('#risk-user-required').addClass('display-none');
            }
          
        }

        if (recipient == Constant.EmailRecipients.CustomEmail) {

            var email = $('#custom-email-address').val();

            if (email == undefined || email == '' || !IsEmail(email)) {

                $('#email-required').removeClass('display-none');
                return false;
            } else {
                $('#email-required').addClass('display-none');

            }
        }

        if ($("#emailGen input:checked").length < 1) {
            $('#items-required').removeClass('display-none');
            return false;
        } else {
            $('#items-required').addClass('display-none');
        }

    });

    $('#receipients').change(function () {
        $('#recipient-required').addClass('display-none');
        $('#risk-user-required').addClass('display-none');
        $('#email-required').addClass('display-none');
        $('#items-required').addClass('display-none');
        $('#custom-email-address').val('');
        $('#risk-user-emails').val('');
    });

    $('#reset-email-generator').click(function () {

        $("#emailGen input:checked").attr('checked', false);
      
        $('#recipient-required').addClass('display-none');
        $('#risk-user-required').addClass('display-none');
        $('#email-required').addClass('display-none');
        $('#items-required').addClass('display-none');
        $('#risk-users').addClass('display-none');
        $('#custom-email').addClass('display-none');

        $('#custom-email-address').val('');
        $('#risk-user-emails').val('');
        $('#remarks').val('');

       
        $('#receipients').selectpicker('val', "");
        $('#SuccessMessage').addClass('display-none');

    });
   
});



$('#select-all-info').click(function () {

    $('.info-to-send').prop('checked', this.checked);

});

$('#select-all-files').click(function () {

    $('.file-to-send').prop('checked', this.checked);

});

$('#select-all-notes').click(function () {

    $('.notes-to-send').prop('checked', this.checked);

});





$('#receipients').change(function () {

    if ($('#receipients').val() == Constant.EmailRecipients.OtherRiskUser) {
        $('#risk-users').removeClass('display-none');
    } else {
        $('#risk-users').addClass('display-none');

    }

    if ($('#receipients').val() == Constant.EmailRecipients.CustomEmail) {

        $('#custom-email').removeClass('display-none');
    } else {
        $('#custom-email').addClass('display-none');

    }


});

function IsEmail(email) {

    var emails = email.split(';');

    var invalidEmails = [];

    for (i = 0; i < emails.length; i++) {
        if (!validateEmail(emails[i].trim())) {
            invalidEmails.push(emails[i].trim())
        }
    }

    if (invalidEmails.length > 0) {
        return false;
    } else {
        return true;
    }
 
}

function validateEmail(email) {
    var re = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i;
    return re.test(email);
}


function OnSuccessEmail(data) {

   
    if (data == true) {
       
        $('#SuccessMessage').removeClass('display-none');
    }


}

function OnBeginEmail() {
    $('#SuccessMessage').addClass('display-none');
}

$('.for-credit-card[value=1]').click(EnableCreditCardDetails);

$('#select-all-info').click(EnableCreditCardDetails);

function EnableCreditCardDetails() {
    //debugger;
    if ($('.for-credit-card[value=1]').is(':checked')) {

        $('.for-credit-card[value=12]').removeAttr('disabled');
        $('.for-credit-card[value=12]').addClass('info-to-send');
       

    } else {
        $('.for-credit-card[value=12]').attr('checked', false);
        $('.for-credit-card[value=12]').attr('disabled', 'disabled');
        $('.for-credit-card[value=12]').removeClass('info-to-send');

    }

}