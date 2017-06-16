$(document).ready(function (e) {

    $(".selectpicker").selectpicker({});

    var dateToday = new Date();
    $(".datepickerOpenDate").datepicker({
        minDate: dateToday,
        //beforeShowDay: $.datepicker.noWeekends
    });

    var dateToday = new Date();
    $(".datepicker").datepicker({
        //minDate: dateToday,
        //beforeShowDay: $.datepicker.noWeekends
    });

    $.validator.unobtrusive.parse("#incident-info-form");

    //Added temporarily need to change
    if ($('#lossType').html() == 'Stolen Vehicle') {
        $('#loss-date').removeAttr('disabled');
    } else {
        $('#loss-date').attr("disabled", "disabled");
    }

    $("#addPoliceAgency").click(function () {
        window.location = $(this).attr("data-url");
    })

});