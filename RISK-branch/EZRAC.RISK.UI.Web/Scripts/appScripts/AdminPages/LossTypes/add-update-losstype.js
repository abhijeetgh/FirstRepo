$.validator.setDefaults({
    ignore: []
});

$.validator.unobtrusive.parse("#lossTypeForm");

$("#inputLossType,#inputDescription").on('input paste', function () {
    if ($(this).val() == $(this).get(0).defaultValue) return;
    $(".form-buttons").removeAttr('disabled');
    $(".form-input-value").attr('data-isDirty', true);
    $("#errorMessage").text("");
});

$("#cancelLossType").click(function () {
    var isDirty = $(".form-input-value").attr("data-isdirty");
    if (isDirty) {
        //ShowConfirmBox("Do you want to discard changes?", true, function () {
            clearValues();
            $('#lossTypeDataTable tr').removeClass("rowSelected");
            $(".form-input-value").attr('data-isDirty', false);
       // });
        //if (confirm("Do you want to discard changes?")) {
        //    clearValues();
        //    $('#lossTypeDataTable tr').removeClass("rowSelected");
        //    $(".form-input-value").attr('data-isDirty', false);
        //}
    }
});

function success(data) {
    if (data.Data == false) {
        $("#errorMessage").text("Loss type already exists");
    }
    else {
        $("#lossTypeTable").html(data);
        clearValues();
    }

}