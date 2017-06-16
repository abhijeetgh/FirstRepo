$(document).ready(function () {

    $.extend(true, $.fn.dataTable.defaults, {
        "searching": true,
        "ordering": true,
        "paging": false,
        "info": false,
        "destroy": true
    });

    $('#vehicleSectionTable').DataTable({
        columnDefs: [
        { orderable: false, targets: -1 }
        ],
        "order": [[0, "asc"]],
        "language": {
            "emptyTable": "Vehicle sections are not available"
        }
    });

    $("#searchVehicleSection").keyup(function () {
        $('#vehicleSectionTable').DataTable().search($(this).val()).draw();
    })

    $(".dataTables_filter").addClass('display-none');

    $("#vehicleSectionTable td.vehicle-section").click(function () {
        //$("#vehicleSectionTable tr").removeClass("rowSelected");
        var url = $(this).attr("data-url");
        var id = $(this).attr("data-id");
        getVehicleSection(url, id);
        $("#vehicleSectionTable").dataTable().$('tr.rowSelected').removeClass("rowSelected");
        $(this).closest('tr').addClass("rowSelected");
    });

    $(".delete-action").click(function (e) {
        var sectionId = $(this).attr("data-id"); //$(this).parents().eq(1).find("#statusId").text();
        var url = $(this).attr("data-url");
        var validationUrl = $(this).attr("data-url-validation");
        isVehicleSectionAlreadyUsed(validationUrl, url, sectionId);
    });

});

function getVehicleSection(url, id) {
    $.ajax({
        url: url,
        data: { id: id },
        type: "GET",
        cache: false,
        success: function (returnData) {
            $("#vehicleSection").html(returnData);
        },
        error: onErrorHandler,
    });
}

//function successDelete(data)
//{
//    if (data.Data == false) {
//        alert("Can not delete vehicle section  beacause it is alerady associated with claims");
//    }
//    else {
//        $("#updateContainer").html(data);
//    }
    
//}

function isVehicleSectionAlreadyUsed(urlValidation, url, id) {
    $.ajax({
        url: urlValidation,
        data: { id: id },
        type: "GET",
        cache: false,
        success: function (returnData) {
            if (returnData.Data) {
                //alert("Vehicle section can not be deleted because it is associated with damage.");
                ShowConfirmBox("Vehicle section can not be deleted because it is associated with damage.", false);
            }
            else {
                ShowConfirmBox("Do you want to delete vehicle section?", true, function () {
                    deleteVehicleSection(url, id);
                });
                //var confirmDelete = confirm("Do you want to delete vehicle section?");
                //if (confirmDelete) {
                //    deleteVehicleSection(url, id);
                //}
            }
        },
        error: onErrorHandler,
    });

}

function deleteVehicleSection(url, id) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: url,
        data: { id: id, __RequestVerificationToken: token },
        type: "POST",
        cache: false,
        success: function (returnData) {
            $("#updateContainer").html(returnData);
            clearValues();
        },
        error: onErrorHandler,
    });
}