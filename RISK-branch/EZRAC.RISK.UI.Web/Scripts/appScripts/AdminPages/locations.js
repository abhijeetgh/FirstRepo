$(document).ready(function () {

    $.validator.setDefaults({
        ignore: []
    });



    $(document).find('.main-nav li').removeClass('active');
    $(document).find('.main-nav li').eq(1).addClass('active');

    $.extend(true, $.fn.dataTable.defaults, {
        "searching": true,
        "ordering": true,
        "paging": false,
        "info": false,
        "destroy": true
    });

    if ($("#locationTable .dataTables_wrapper").length == 0) {
        $('#locationDataTable').DataTable({
            columnDefs: [
            { orderable: false, targets: -1 }
            ],
            "order": [[0, "asc"]],
            "language": {
                "emptyTable": "Locations are not available"
            }
        });
    }

    $("#locationDataTable_wrapper .dataTables_filter").addClass('display-none');

    $.validator.unobtrusive.parse("#locationForm");

    $(".selectpicker").selectpicker({});

    $("#inputcode,#inputDescription,#inputState").off().on('input paste', function () {
        $(".form-buttons").removeAttr('disabled');
        $(".form-input-value").attr('data-isDirty', true);
        $("#errorMessage").text("");
    });


    $("#addLocation").off().on('click', function () {
        var url = $(this).attr("data-url");
        getLocationDetails(url, 0);
        $("#locationDataTable tr").removeClass('rowSelected');

    });

    $("#searchLocations").off().on('keyup', function () {
        $('#locationDataTable').DataTable().search($(this).val()).draw();
    });

    $("#locationDataTable td.details").off().on('click', function () {
        var url = $(this).data("url");
        var id = $(this).data("id");
        getLocationDetails(url, id);
        //$("#locationDataTable tr").removeClass('rowSelected');
        $("#locationDataTable").dataTable().$('tr.rowSelected').removeClass("rowSelected");
        $(this).closest('tr').addClass('rowSelected');
    });


    $("#btn-cancel-location").off().on('click', function () {
        var isDirty = $("#locationForm .form-control").attr("data-isDirty");
        if (isDirty) {
            //ShowConfirmBox("Do you want to discard changes", true, function () {
                clearValues();
            //});
        }
    });


    //$('#lossTypeDataTable').DataTable({
    //    columnDefs: [
    //    { orderable: false, targets: -1 }
    //    ],
    //    "order": [[0, "asc"]],
    //    "language": {
    //        "emptyTable": "Loss types are not available"
    //    }
    //});

    //$("#lossTypeDataTable_wrapper .dataTables_filter").addClass('display-none');

    $("#inputcode,#inputDescription,#inputState").off().on('input paste', function () {
        $(".form-buttons").removeAttr('disabled');
        $(".form-input-value").attr('data-isDirty', true);
        $("#errorMessage").text("");
    });

    $('#companies').change(function () {
       // alert('test');
        $(".form-buttons").removeAttr('disabled');
        $(".form-input-value").attr('data-isDirty', true);
        $("#errorMessage").text("");
    });

});


function clearValues() {
    $(".details-header").text("New Location");
    $(".form-input-value").val("");
    $(".form-buttons").attr("disabled", "disabled");
    $("#searchLocations").val("");
    $('#locationDataTable tr').removeClass("rowSelected");
    $('#errorMessage').text("");
    $('#companies').selectpicker('val', "");
    $(".field-validation-error span").text("");
}



function getLocationDetails(url, id) {
    $.ajax({
        url: url,
        data: { id: id },
        type: "GET",
        cache: false,
        success: function (returnData) {
            $("#location-details").html(returnData);

        },
    });
}

//function onAjaxComplete(data) {
//    $("#lossTypeTable").html(returnData);
//}


function AddUpdateLocationSuccess(data) {

    if (data.indexOf("field-validation-error") > -1) {

        $("#location-details").html(data);

    } else {

        $("#locationTable").html(data);
        reSetForm();

    }
}
$('.delete-location').off().on('click', function () {

    var token = $('input[name="__RequestVerificationToken"]').val();

    // alert($(this).data('islocationused'));
    var deleteUrl = $(this).data('url');
    $.ajax({
        url: $(this).data('islocationused'),
        data: { __RequestVerificationToken: token },
        type: "GET",
        cache: false,
        success: function (returnData) {

            if (returnData == false) {
                ShowConfirmBox("Are you sure you want to delele location.", true, function () {
                    $.ajax({
                        url: deleteUrl,
                        data: { __RequestVerificationToken: token },
                        type: "POST",
                        cache: false,
                        success: function (returnData) {
                            $("#locationTable").html(returnData);
                            reSetForm();
                        },
                    });
                });
                //if (confirm('Are you sure you want to delele location.')) {

                //    $.ajax({
                //        url: deleteUrl,
                //        data: { __RequestVerificationToken: token },
                //        type: "POST",
                //        cache: false,
                //        success: function (returnData) {
                //            $("#locationTable").html(returnData);
                //            reSetForm();
                //        },
                //    });
                //}
            } else {

                //alert('Location cannot be deleted');
                ShowConfirmBox("Location cannot be deleted", false);

            }
        },
    });

});

function reSetForm() {
    $('#inputcode').val('');
    $('#inputDescription').val('');
    $('#Id').val('0');
    $('#companies').selectpicker('val', "");
    $('#inputState').val('');
}