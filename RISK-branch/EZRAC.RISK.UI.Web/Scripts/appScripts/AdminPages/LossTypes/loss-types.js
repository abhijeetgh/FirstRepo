$(document).ready(function () {

    $(document).find('.main-nav li').removeClass('active');
    $(document).find('.main-nav li').eq(1).addClass('active');

    $.extend(true, $.fn.dataTable.defaults, {
        "searching": true,
        "ordering": true,
        "paging": false,
        "info": false,
        "destroy": true
    });

    $('#lossTypeDataTable').DataTable({
        columnDefs: [
        { orderable: false, targets: -1 }
        ],
        "order": [[0, "asc"]],
        "language": {
            "emptyTable": "Loss types are not available"
        }
    });

    $("#lossTypeDataTable_wrapper .dataTables_filter").addClass('display-none');

    $("#lossTypeDataTable td.details").off().on('click', function () {
        var url = $(this).attr("data-url");
        var id = $(this).attr("data-id");
        getLossTypeDetail(url, id);
        $("#lossTypeDataTable tr").removeClass("rowSelected");
        $(this).closest('tr').addClass("rowSelected");
    });

    $("#lossTypeDataTable a.delete-action").off().on('click', function () {
        var url = $(this).attr("data-url");
        var id = $(this).attr("data-id");
        var validationUrl = $(this).attr("data-url-validation");
        isLossTypeAlreadyUsed(validationUrl, url, id);
    });

});

$("#addLossType").click(function () {
    var url = $(this).attr("data-url");
    $.ajax({
        url: url,
        type: "GET",
        cache: false,
        success: function (returnData) {
            $("#lossTypeDetails").html(returnData);
            clearValues();
        },
        error: onErrorHandler,
    });
});

$("#searchLossType").keyup(function () {
    $('#lossTypeDataTable').DataTable().search($(this).val()).draw();
});


function clearValues() {
    $(".details-header").text("New Loss Type");
    $(".form-input-value").val("");
    $(".form-buttons").attr("disabled", "disabled");
    $("#searchLossType").val("");
    $('#lossTypeDataTable tr').removeClass("rowSelected");
    $('#errorMessage').text("");
    $(".field-validation-error span").text("");
}