$(document).ready(function () {

    $(document).find('.main-nav li').removeClass('active');
    $(document).find('.main-nav li').eq(1).addClass('active');


    $(".header-navOption a").removeClass("active");
    $(".header-navOption a.claimstatus").addClass("navOptionSelected");

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

    $("#addClaimStatus").click(function () {
        var url = $(this).attr("data-url");
        getNewClaimStatus(url);
        $(".claim-stauts-buttons").attr('disabled', 'disabled');
        $('.claim-status-table tr').removeClass("rowSelected");
    });

    //$('.claim-status-table td.claim-status').click(function () {
    //    getClaimStatus($(this).parent().find('td').eq(0).text());
    //    $('.claim-status-table tr').css('background-color', '');
    //    $(this).closest('tr').css('background-color', 'red')
    //    //var isDirty = $('#claimStatus').attr("data-isdirty");
    //    //if (isDirty & isDirty != undefined) {
    //    //    if(confirm("Do you want to discard changes?"))
    //    //    getClaimStatus($(this).parent().find('td').eq(0).text());
    //    //    $('.claim-status-table tr').css('background-color', '');
    //    //    $(this).closest('tr').css('background-color', 'red')
    //    //}
    //    //else {
    //    //    getClaimStatus($(this).parent().find('td').eq(0).text());
    //    //    $('.claim-status-table tr').css('background-color', '');
    //    //    $(this).closest('tr').css('background-color', 'red')
    //    //}
    //});

    $("#searchStatus").keyup(function () {
        $('#claimStatusTable').DataTable().search($('#searchStatus').val()).draw();
    });

});

function deleteClaimStatus(url, id) {
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
}


function getClaimStatus(url, id) {
    $.ajax({
        url: url,
        data: { statusId: id },
        type: "GET",
        cache: false,
        success: function (returnData) {
            $("#adminClaimStatus").html(returnData);
        },
        error: onErrorHandler,
    });
}

function getNewClaimStatus(url) {
    $.ajax({
        url: url,
        type: "GET",
        cache: false,
        success: function (returnData) {
            $("#adminClaimStatus").html(returnData);
        },
        error: onErrorHandler,
    });
}