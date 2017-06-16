
$(document).ready(function () {
   
    var locationId = $('#LocationId').val();
    $('#location').selectpicker('val', locationId);
    $('#location').attr('disabled', true);
    $('.selectpicker').selectpicker('refresh');


});

