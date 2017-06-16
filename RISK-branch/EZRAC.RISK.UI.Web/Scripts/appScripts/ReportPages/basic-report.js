$('.view-billings').click(function () {

    RiskAjax($(this).data('url'), null, 'html', 'GET', false, function (returnData) {
        $(".cla-footer").after(returnData);
        $("#billings").modal('show');

    });

});


$('.view-payments').click(function () {

    RiskAjax($(this).data('url'), null, 'html', 'GET', false, function (returnData) {
        $(".cla-footer").after(returnData);

        $("#payments").modal('show');
    });

});