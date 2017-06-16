$(document).ready(function () {

    $(document).find('.main-nav li').removeClass('active');
    $(document).find('.main-nav li').eq(1).addClass('active');

    $(".header-navOption a").removeClass("active");
    $(".header-navOption a.writeoff").addClass("navOptionSelected");

    $.extend(true, $.fn.dataTable.defaults, {
        "searching": true,
        "ordering": true,
        "paging": false,
        "info": false,
        "destroy": true
    });

    $('#writeOffTypeDataTable').DataTable({
        columnDefs: [
        { orderable: false, targets: -1 }
        ],
        "order": [[0, "asc"]],
        "language": {
            "emptyTable": "Write off types are not available"
        }
    });

    $("#writeOffTypeDataTable_wrapper .dataTables_filter").addClass('display-none');

    $("#writeOffTypeDataTable td.details").off().on('click', function () {
        var url = $(this).attr("data-url");
        var id = $(this).attr("data-id");
        getwriteOffTypeDetail(url, id);
        $("#writeOffTypeDataTable tr").removeClass("rowSelected");
        $(this).closest('tr').addClass("rowSelected");
    });

    $("#writeOffTypeDataTable a.delete-action").off().on('click', function () {
        var url = $(this).attr("data-url");
        var id = $(this).attr("data-id");
        var validationUrl = $(this).attr("data-url-validation");
        isWriteOffTypeAlreadyUsed(validationUrl, url, id);
    });

    $("#addwriteOffType").click(function () {
        var url = $(this).attr("data-url");
        $.ajax({
            url: url,
            type: "GET",
            cache: false,
            success: function (returnData) {
                $("#writeOffTypeDetails").html(returnData);
                clearValues();
            },
            error: onErrorHandler,
        });
    });

    $("#searchWriteOffType").keyup(function () {
        $('#writeOffTypeDataTable').DataTable().search($(this).val()).draw();
    });

});

function clearValues() {
    $(".details-header").text("New Write Off Type");
    $(".form-input-value").val("");
    $(".form-buttons").attr("disabled", "disabled");
    $("#searchWriteOffType").val("");
    $('#writeOffTypeDataTable tr').removeClass("rowSelected");
    $("#WriteOffTypeId").val(0);
    $('#errorMessage').text("");
    $(".field-validation-error span").text("");
}