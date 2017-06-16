$.validator.setDefaults({
    ignore: []
});

$.validator.unobtrusive.parse("#writeofftypeForm");

$("#inputwriteoffType,#inputDescription").on('input paste', function () {
    if ($(this).val() == $(this).get(0).defaultValue) return;
    $(".form-buttons").removeAttr('disabled');
    $(".form-input-value").attr('data-isDirty', true);
    $("#errorMessage").text("");
});

$("#cancelWriteOffType").click(function () {
    var isDirty = $(".form-input-value").attr("data-isdirty");
    if (isDirty) {        
        clearValues();
        $('#writeOffTypeDataTable tr').removeClass("rowSelected");
        $(".form-input-value").attr('data-isDirty', false);        
    }
});

function success(data) {
    if (data.Data == false) {
        $("#errorMessage").text("Write off type already exists");
    }
    else {
        $("#writeOffTypeTable").html(data);
        clearValues();
    }

}