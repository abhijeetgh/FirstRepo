$(document).ready(function () {
    var getCurrentPage = sessionStorage.getItem("CurrentMenuItem");
    var currentPage = $(".header-navOption").find(".navOptionSelected").attr("currentpage");
    if (getCurrentPage == null) {
        sessionStorage.setItem("CurrentMenuItem", currentPage);
    }
    else {
        $(".header-navOption").find("." + getCurrentPage).addClass("navOptionSelected").siblings("a").remove("navOptionSelected");
    }
    $(".header-navOption a").on("click", function () {
        sessionStorage.setItem("CurrentMenuItem", $(this).attr("currentpage"));
    });
})