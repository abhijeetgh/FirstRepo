$("document").ready(function () {
    $(".mtab2").click(function (event) {

        event.preventDefault();
        $("#mtab2").css("display", "block");
        $("#mtab1").css("display", "none");
        $(".mtab2").addClass('red');
        $(".mtab1").removeClass('red');
    });
    $(".mtab1").click(function (event) {
        event.preventDefault();

        $("#mtab1").css("display", "block")

        $("#mtab2").css("display", "none");
        $(".mtab1").addClass('red');
        $(".mtab2").removeClass('red');

    });
    
    $("#pastSearches li").click(function () {
        $(this).addClass("rsselected");
        $(this).siblings("li").removeClass("rsselected")
    });

    $('#tsdSystemSelect_Weblink li').css('cursor', 'pointer').click(function () {
        $(this).toggleClass('selected');
    });
})


