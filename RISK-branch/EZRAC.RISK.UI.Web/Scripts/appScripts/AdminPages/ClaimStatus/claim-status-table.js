$(document).ready(function () {
    $.extend(true, $.fn.dataTable.defaults, {
        "searching": true,
        "ordering": true,
        "paging": false,
        "info": false,
        "destroy": true
    });

    $('#claimStatusTable').DataTable({
        columnDefs: [
        { orderable: false, targets: -1 }
        ],
        "order": [[0, "asc"]],
        "language": {
            "emptyTable": "Claim status are not available"
        }
    });

    $("#updateContainer .dataTables_filter").addClass('display-none');

    $('.claim-status-table td.claim-status').click(function () {
        var statusId = $(this).attr("data-id");
        var url = $(this).attr("data-url");
        getClaimStatus(url, statusId);
        //$('.claim-status-table tr').removeClass("rowSelected");
        $(".claim-status-table").dataTable().$('tr.rowSelected').removeClass("rowSelected");
        $(this).closest('tr').addClass("rowSelected");

        //var isDirty = $('#claimStatus').attr("data-isdirty");
        //if (isDirty && isDirty != undefined) {
        //    if(confirm("Do you want to discard changes?"))
        //    getClaimStatus($(this).parent().find('td').eq(0).text());
        //    $('.claim-status-table tr').css('background-color', '');
        //    $(this).closest('tr').css('background-color', 'red')
        //}
        //else {
        //    getClaimStatus($(this).parent().find('td').eq(0).text());
        //    $('.claim-status-table tr').css('background-color', '');
        //    $(this).closest('tr').css('background-color', 'red')
        //}
    });

    $(".delete-action").click(function (e) {
        var statusId = $(this).attr("data-id"); //$(this).parents().eq(1).find("#statusId").text();
        var url = $(this).attr("data-url");
        var validationUrl = $(this).attr("data-url-validation");
        isClaimStatusAlreadyUsed(validationUrl, url, statusId);
    });

});

function isClaimStatusAlreadyUsed(urlValidation, url, id) {
    $.ajax({
        url: urlValidation,
        data: { id: id },
        type: "GET",
        cache: false,
        success: function (returnData) {
            if (returnData.Data) {
                //alert("Claim status can not be deleted because it is associated to claim");
                ShowConfirmBox("Claim status can not be deleted because it is associated to claim", false);
            }
            else
            {
                //var confirmDelete = confirm("Do you want to delete claim status?");
                //if (confirmDelete) {
                //    deleteClaimStatus(url, id);
                ShowConfirmBox("Do you want to delete claim status?", true, function () {
                    var token = $('input[name="__RequestVerificationToken"]').val();
                    $.ajax({
                        url: url,
                        data: { statusId: id, __RequestVerificationToken: token },
                        type: "POST",
                        cache: false,
                        success: function (returnData) {
                            $("#updateContainer").html(returnData);
                            clearValues();
                        },
                        error: onErrorHandler,
                    });
                });
                //}
            }
        },
        error: onErrorHandler,
    });

}


