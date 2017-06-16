

$(document).ready(function (e) {

    $("#claimBasicInfo").trigger("click");

    var dateToday = new Date();
    $(".datepickerFollowUpdate").datepicker({
        minDate: dateToday,
        beforeShowDay: $.datepicker.noWeekends
    });


    var dateToday = new Date();
    $(".datepickerOpenDate").datepicker({
        minDate: dateToday,
        //beforeShowDay: $.datepicker.noWeekends
    });

    var dateToday = new Date();
    $(".datepickerCloseDate").datepicker({
        //minDate: dateToday,
        //beforeShowDay: $.datepicker.noWeekends
    });

    $(".add-update-icon li .dropdown-toggle").click(function () {
        getNotesForClaim($("#claimIdForNotes").val(), $(this));
        $(".dropdown-toggle").attr("data-toggle", "dropdown");
        $(this).attr("data-toggle", "");
        $(this).parent().addClass('open');
    });


    $(".claimInfoTabs").click(function () {
      
        $("#nav-tabs-wrapper li").removeClass("active");
        $(this).parent().addClass("active");
    });

    //$('.dropdown-toggle').on('click', function (event) {
    //    $(".dropdown-toggle").attr("data-toggle", "dropdown");
    //    $(this).attr("data-toggle", "");
    //    $(this).parent().addClass('open');
    //});

    $('#email-generator').click(function () {

        RiskAjax($(this).data('url'), null, 'html', "GET", false, function (returnData) {
            $(".cla-footer").after(returnData);

            $("#emailGen").modal('show');
        }, null);

    });
    
    $('#claimEvents').click(function () {
        $.ajax({
            url: $(this).data('url'),            
            type: "GET",
            cache: false,
            success: function (returnData) {

                $(".cla-footer").after(returnData);

                $("#claimEventModel").modal('show');
            },
            error: onErrorHandler,
        });


    });

    $("#deleteClaim").click(function () {
        var url = $(this).attr("data-url");
        var id = $(this).attr("data-id");
        var redirectUrl = $(this).attr("data-redirectUrl");
        ShowConfirmBox("Do you want to delete claim?", true, function () {
            deleteClaim(url, id, redirectUrl);
        });
        //if (confirm("Do you want to delete claim?")) {
        //    deleteClaim(url, id, redirectUrl);
        //}
    });

    $("#approveClaimForViewClaim").click(function () {
        //debugger;
        var url = $(this).attr("data-approve-url");
        var claimId = $(this).attr("data-id");
        getApprovaNotesForm(url, claimId);
        //approveClaim(url, id);
    });

    $("#rejectClaimForViewClaim").click(function () {
        var url = $(this).attr("data-url");
        var id = $(this).attr("data-id");
        var rejectUrl = $(this).attr("data-reject-url");
        getRejectNotesForm(rejectUrl, id)
    });

});

function getNotesForClaim(claimId, area) {

    $.ajax({
        url: quickNotesUrl,
        data: { claimNumber: claimId },
        type: "GET",
        cache: false,
        success: function (returnData) {
            $(area).parents().eq(2).find("ul.notes-area").empty().append(returnData);           
        },
        error: onErrorHandler,
    });

}

function approveClaim(url, id) {
    //debugger;
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: url,
        data: { id: id, status: true, __RequestVerificationToken: token },
        type: "POST",
        cache: false,
        success: function (data) {
            $("#claim-header-secton").html(data);
            $(".approveReject").hide();
        },
        error: onErrorHandler,
    });
}

function approveOrRejectCLaim(url, id) {
    //debugger;
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: url,
        data: { id: id, status: false, __RequestVerificationToken: token },
        type: "POST",
        cache: false,
        success: function (data) {
            $("#claim-header-secton").html(data);
            $(".approveReject").hide();
        },
        error: onErrorHandler,
    });
}

function getRejectNotesForm(url, id) {
    $.ajax({
        url: url,
        type: "Get",
        cache: false,
        success: function (returnData) {
            $("#popup").html(returnData);
            $("#claimId").val(id);
            $("#popup").find("#rejectPop").modal('show');
        },
        error: onErrorHandler,
    });
}

function deleteClaim(url, id, redirectUrl) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: url,
        data: { id: id, __RequestVerificationToken: token },
        type: "POST",
        cache: false,
        success: function (data) {
            if (data.Success) {
                window.location = redirectUrl;
            } else {
                //alert("Can not delete claim");
                ShowConfirmBox("Can not delete claim", false);
            }
        },
        error: onErrorHandler,
    });

}

function getApprovaNotesForm(url, id) {
    //debugger;
    $.ajax({
        url: url,
        type: "Get",
        cache: false,
        success: function (returnData) {
            $("#popup").html(returnData);
            $("#claimId").val(id);
            $("#popup").find("#approvalPop").modal('show');
        },
        error: onErrorHandler,
    });

}

