$(".trackerTypes").off().on("click", function () {
    $(".tracking-selector-list li").removeClass("active");
    $(this).parent().addClass("active");
    $("#gridHeader").text($(this).text());
});

$(".doneEvent").off().on("click", function () {
    var claimId = $("#claimIdForNotes").val();
    var trackingId = $(this).attr("data-id");
    var type = $(this).attr("data-type");
    var url = $(this).attr("data-url");
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: url,
        data: { claimId: claimId, trackingId: trackingId, type: type,__RequestVerificationToken: token },
    type: "POST",
    cache: false,
    success: function (returnData) {
        $("#trackerDiv").html(returnData);
    },
    error: onErrorHandler,
    });
});