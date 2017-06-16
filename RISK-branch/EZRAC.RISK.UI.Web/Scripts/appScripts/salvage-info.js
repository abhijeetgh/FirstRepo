var dateToday = new Date();
$(".datepicker").datepicker({
    maxDate: dateToday,
});

$(document).ready(function () {
    $.validator.setDefaults({
        ignore: []
    });

    $.validator.unobtrusive.parse("#salvageForm");

    $("#salvageSaveStatus").text("");

    $("#salvageAmount").keyup(function () {
        $("#salvageSaveStatus").text("");
    });

    $("#salvageReceiptDate").change(function () {
        $("#salvageSaveStatus").text("");
    });

    $('.reset-salvage').click(function () {
        $("#salvageSaveStatus").text('');
    })

})

function salvageSaveSuccessful(data) {
    if(data)
    {
        $("#result-panel").html(data);
        $("#salvageSaveStatus").text("Salvage information saved successfully");
    }
    else
        $("#salvageSaveStatus").text("Salvage information save failed");
    }