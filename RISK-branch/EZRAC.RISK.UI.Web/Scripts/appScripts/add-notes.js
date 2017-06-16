
$(document).ready(function () {
    $.validator.setDefaults({
        ignore: []
    });

    $.validator.unobtrusive.parse("#addNotesForm");
});

$("#resetForm").click(function () {
    clearValues();
});

function clearValues() {

    $("#sendEmailNotification").attr("checked", false);
    $("#IsPrivilege").attr("checked", false);
    $("#description").val("");
    $("#searchTxt").val("");
    $("#searchResult").text("");

    $('#noteTypes').selectpicker('val', "");
    $(".field-validation-error span").text("");
}