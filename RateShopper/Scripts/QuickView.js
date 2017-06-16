function getQuickViewResults() {
    $.ajax({
        type: "GET",
        url: '/QuickView/FetchQuickViewResult',
        dataType: 'json',
        async: true,
        success: function (response) {
            console.log(response);
        },
        error: function (error) {

        }
    });
}

$(document).ready(function () {
    getQuickViewResults();
});

function quickView(data)
{
    this.ID = data.ID;
    this.DisplayDate = data.Date;
    this.formatDate = data.FormattedDate;
 //   this.
}