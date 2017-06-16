$(document).ready(function () {

    $(document).find('.main-nav li').removeClass('active');
    $(document).find('.main-nav li').eq(1).addClass('active');

    $("#newVehicleSection").click(function () {
        var url = $(this).attr("data-url");
        getNewVehicleSection(url);
    });
});

function getNewVehicleSection(url) {
    $.ajax({
        url: url,
        type: "GET",
        cache: false,
        success: function (returnData) {
            $("#vehicleSection").html(returnData);
        },
        error: onErrorHandler,
    });
}