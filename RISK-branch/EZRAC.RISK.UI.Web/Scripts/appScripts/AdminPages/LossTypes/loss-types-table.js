$(document).ready(function () {
    $('#lossTypeDataTable').DataTable({
        columnDefs: [
        { orderable: false, targets: -1 }
        ],
        "order": [[0, "asc"]],
        "language": {
            "emptyTable": "Loss types are not available"
        }
    });

    $("#lossTypeDataTable td.details").on('click', function () {
        var url = $(this).attr("data-url");
        var id = $(this).attr("data-id");
        getLossTypeDetail(url, id);
        //$("#lossTypeDataTable tr").css("background-color", "");
        $("#lossTypeDataTable").dataTable().$('tr.rowSelected').removeClass("rowSelected");
        $(this).closest('tr').addClass("rowSelected");
    });

    $("#lossTypeDataTable_wrapper .dataTables_filter").addClass('display-none');

    $("#lossTypeDataTable a.delete-action").on('click',function () {
        var url = $(this).attr("data-url");
        var id = $(this).attr("data-id");
        var validationUrl = $(this).attr("data-url-validation");
        isLossTypeAlreadyUsed(validationUrl, url, id);
    });

});


    function getLossTypeDetail(url, id) {
        $.ajax({
            url: url,
            data: { id: id },
            type: "GET",
            cache: false,
            success: function (returnData) {
                $("#lossTypeDetails").html(returnData);
            },
            error: onErrorHandler,
        });
    }

    //function onAjaxComplete(data) {
    //    $("#lossTypeTable").html(returnData);
    //}

    //function successDelete(data)
    //{
    //    var url = $("#deleteUrl").val();
    //    if (data.Data) {
    //        alert("Can not delete loss type beacause Claim is alerady associated to this loss type ");
    //    }

    //    else {
    //        deleteLossType(url, id)
    //    }

    //}

    function isLossTypeAlreadyUsed(urlValidation, url, id) {
        $.ajax({
            url: urlValidation,
            data: { id: id },
            type: "GET",
            cache: false,
            success: function (returnData) {
                if (returnData.Data) {
                    //alert("Loss type can not be deleted because it is associated with loss type.");
                    ShowConfirmBox("Loss type can not be deleted because it is associated with loss type.", false);
                }
                else {
                    ShowConfirmBox("Do you want to delete loss type?", true, function () {
                        deleteLossType(url, id)
                    });
                    //var confirmDelete = confirm("Do you want to delete loss type?");
                    //if (confirmDelete) {
                    //    deleteLossType(url, id)
                    //}
                }
            },
            error: onErrorHandler,
        });

    }

    function deleteLossType(url, id) {
        var token = $('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            url: url,
            data: { id: id, __RequestVerificationToken: token, },
            type: "POST",
            cache: false,
            success: function (returnData) {
                $("#lossTypeTable").html(returnData);
                clearValues();
            },
            error: onErrorHandler,
        });
    }