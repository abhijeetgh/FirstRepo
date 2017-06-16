$(document).ready(function () {
    $(".selectpicker").selectpicker({});

    $("#searchTxt").keydown(function (event) {
        var token = $('input[name="__RequestVerificationToken"]').val();
        var keyCode = (event.keyCode ? event.keyCode : event.which);
        if (keyCode == 13) {
            event.preventDefault();
            $.ajax({
                url: searchNotesUrl,
                data: { claimNumber: $("#noteclaimId").val(), text: $("#searchTxt").val(), __RequestVerificationToken: token },
                type: "POST",
                cache: false,
                success: function (returnData) {
                    $("#gridContainer").html(returnData);
                    $("#searchResult").text("Search results");
                    if ($.trim($("#searchTxt").val()) === "") {
                        $("#searchResult").text("");
                    }
                },
                error: onErrorHandler,
            });
        }
    });

    $(".close-container").click(function () {
        clearValues();
    });

});