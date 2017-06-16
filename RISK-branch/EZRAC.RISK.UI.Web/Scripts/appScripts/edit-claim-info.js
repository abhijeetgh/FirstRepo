$(document).ready(function (e) {

    $(".selectpicker").selectpicker({});

    var dateToday = new Date();
    $(".datepickerOpenDate").datepicker({
        minDate: dateToday,
        //beforeShowDay: $.datepicker.noWeekends
    });

    var dateToday = new Date();
    $(".datepickerCloseDate").datepicker({
        minDate: $('#OpenDate').val(),
        //beforeShowDay: $.datepicker.noWeekends
    });

    $.validator.unobtrusive.parse("#claim-info-form");
});


function EditClaimInfoSuccess(data) {

    if (data.indexOf("field-validation-error") > -1) {
        $("#claim-info").html(data);
    }
    else {
        $("#result-panel").html(data);
        $(".field-validation-error").text("");
        
    }
}