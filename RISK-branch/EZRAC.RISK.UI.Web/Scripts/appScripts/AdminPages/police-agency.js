

$(document).ready(function () {
    $.validator.unobtrusive.parse("#policeAgencyForm");

    $(document).find('.main-nav li').removeClass('active');
    $(document).find('.main-nav li').eq(1).addClass('active');

    $(".header-navOption a").removeClass("active");
    $(".header-navOption a.policeagency").addClass("navOptionSelected");

    $.extend(true, $.fn.dataTable.defaults, {
        "searching": true,
        "ordering": true,
        "paging": false,
        "info": false,
        "destroy": true
    });

    if ($("#policeAgencyList .dataTables_wrapper").length == 0) {
        $('#policeAgencyTable').DataTable({
            columnDefs: [
            { orderable: false, targets: -1 }
            ],
            "order": [[0, "asc"]],
            "language": {
                "emptyTable": "Police agencies are not available"
            }
        });
    }

    $(".dataTables_filter").addClass('display-none');

    $("#searchPoliceAgency").off().on('keyup', function () {
        //$("#policeAgencyTable tr").removeClass("rowSelected");
        $('#policeAgencyTable').DataTable().search($(this).val()).draw();
    });


    $(".delete-action").off().on('click', function () {
        var id = $(this).attr("data-id");
        var url = $(this).attr("data-url-delete");
        var urlForValidation = $(this).attr("data-url-valid");
        policeAgencyAlreadyUsed(urlForValidation, url, id);

    });

    $("#addPoliceAgency").off().on('click', function () {
        var url = $(this).attr("data-url");
        $("#policeAgencyTable tr").removeClass("rowSelected");
        $.ajax({
            url: url,
            type: "GET",
            cache: false,
            success: function (returnData) {
                $("#policeAgenciesDetail").html(returnData);
            },
            error: onErrorHandler,
        });
    });

    $("#cancelPoliceAgency").off().on('click', function () {
        var isDirty = $("#policeAgencyForm .form-control").attr("data-isDirty");
        if (isDirty) {
            //ShowConfirmBox("Do you want to discard changes", true, function () {
                clearValues();
            //});
            //if (confirm("Do you want to discard changes")) {
            //    clearValues();
            //}
        }
    });

    $(".police-agency-column").off().on('click', function () {
        var id = $(this).attr("data-id");
        var url = $(this).attr("data-url");
        getPoliceAgencyDetails(url, id);
        //$("policeAgencyTable tr").removeClass("rowSelected");
        $("#policeAgencyTable").dataTable().$('tr.rowSelected').removeClass("rowSelected");
        $(this).closest('tr').addClass('rowSelected');
    });


    $("#policeAgencyForm .form-control").off().on("input", function () {
        if ($(this).val() == $(this).get(0).defaultValue) return;
        $("#policeAgencyForm .form-control").attr('data-isDirty', true);
        $(".form-buttons").removeAttr("disabled");
    });

});





function successData(data) {
    if (data.Data == false) {
        $("#errorAgencyName").text("Police agency already exists.");
    }
    else {
        $("#policeAgencyList").html(data);
        clearValues();
    }
}

function clearValues() {
    $("#policeAgencyForm .form-control").val("");
    $("#policeAgencyTable tr").removeClass("rowSelected");
    $("#policeAgencyTitle").text("New Police Agency");
    $(".policeAgencyForm").attr("disabled", "disabled");
    $("#errorAgencyName").text("");
    $("#searchPoliceAgency").val("");
    $(".field-validation-error span").text("");
    $("#policeAgencyId").val(0);
    $("#policeAgencyForm .form-buttons").attr("disabled", "disabled");
}

function getPoliceAgencyDetails(url, id) {
    $.ajax({
        url: url,
        data: { id: id },
        type: "GET",
        cache: false,
        success: function (returnData) {
            $("#policeAgenciesDetail").html(returnData);
        },
        error: onErrorHandler,
    });
}

function deletePoliceAgency(url, id) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: url,
        data: { id: id, __RequestVerificationToken: token },
        type: "POST",
        cache: false,
        success: function (returnData) {
            clearValues();
            $("#policeAgencyList").html(returnData);
        },
        error: onErrorHandler,
    });
}

function policeAgencyAlreadyUsed(urlForValidation, url, id) {
    $.ajax({
        url: urlForValidation,
        data: { id: id },
        type: "GET",
        cache: false,
        success: function (returnData) {
            if (returnData.Data) {
                //alert("Police agency can not be deleted because it is associated with claim.")
                ShowConfirmBox("Police agency can not be deleted because it is associated with claim.", false);
            }
            else {
                ShowConfirmBox("Do you want to delete police agency?.", true, function () {
                    deletePoliceAgency(url, id);
                });
                //if (confirm("Do you want to delete police agency?.")) {
                //    deletePoliceAgency(url, id);
                //}
            }
        },
        error: onErrorHandler,
    });
}

