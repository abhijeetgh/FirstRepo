$(document).ready(function () {
    $.validator.setDefaults({
        ignore: []
    });

    $.validator.unobtrusive.parse("#vehicleSectionForm");


    $("#vehicleSectionText").on("input", function () {
        $(this).attr("data-isdirty", true);
        $(".form-input").removeAttr("disabled");
        $("#errorMessage").text("");
    });

    $("#cancelVehicleSection").click(function () {
        if ($("#vehicleSectionText").attr("data-isdirty")) {
            //ShowConfirmBox("Do you want to discard changes?", true, function () {
                clearValues();
           // });
            //if (confirm("Do you want to discard changes?")) {
            //    clearValues();
            //}
        }
    });

});

function success(data) {
    if (data.Data == false) {
        $("#errorMessage").text("Vehicle section already exists");
    }
    else {
        $("#updateContainer").html(data);
        clearValues();
    }
}

function clearValues() {
    $(".form-input").attr("disabled", "disabled");
    $("#vehicleSectionText").val("");
    $("#vehicleSectionTable tr").removeClass("rowSelected");
    $("#vehicleSectionTitle").text("New Vehicle Section");
    $("#searchVehicleSection").val("");
    $("#errorMessage").text("");
    $("#vehicleSectionId").val(0);
    $(".field-validation-error span").text("");
}
