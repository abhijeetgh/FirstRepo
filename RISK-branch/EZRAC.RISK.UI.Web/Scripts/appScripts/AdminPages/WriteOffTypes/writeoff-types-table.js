$(document).ready(function () {
    $('#writeOffTypeDataTable').DataTable({
        columnDefs: [
        { orderable: false, targets: -1 }
        ],
        "order": [[0, "asc"]],
        "language": {
            "emptyTable": "Write off types are not available"
        }
    });

    $("#writeOffTypeDataTable td.details").on('click', function () {
        var url = $(this).attr("data-url");
        var id = $(this).attr("data-id");
        getwriteOffTypeDetail(url, id);
        $("#writeOffTypeDataTable").dataTable().$('tr.rowSelected').removeClass("rowSelected");
        $(this).closest('tr').addClass("rowSelected");
    });

    $("#writeOffTypeDataTable_wrapper .dataTables_filter").addClass('display-none');

    $("#writeOffTypeDataTable a.delete-action").on('click', function () {
        var url = $(this).attr("data-url");
        var id = $(this).attr("data-id");
        var validationUrl = $(this).attr("data-url-validation");
        isWriteOffTypeAlreadyUsed(validationUrl, url, id);
    });

});


function getwriteOffTypeDetail(url, id) {
    $.ajax({
        url: url,
        data: { id: id },
        type: "GET",
        cache: false,
        success: function (returnData) {
            $("#writeOffTypeDetails").html(returnData);
        },
        error: onErrorHandler,
    });
}

function isWriteOffTypeAlreadyUsed(urlValidation, url, id) {
    $.ajax({
        url: urlValidation,
        data: { id: id },
        type: "GET",
        cache: false,
        success: function (returnData) {
            if (returnData.Data) {                
                ShowConfirmBox("Write Off type can not be deleted because it is associated with one of the claim.", false);
            }
            else {
                ShowConfirmBox("Do you want to delete write off type?", true, function () {
                    deleteWriteOffType(url, id)
                });               
            }
        },
        error: onErrorHandler,
    });

}

function deleteWriteOffType(url, id) {
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: url,
        data: { id: id, __RequestVerificationToken: token, },
        type: "POST",
        cache: false,
        success: function (returnData) {
            $("#writeOffTypeTable").html(returnData);
            clearValues();
        },
        error: onErrorHandler,
    });
}