
$.validator.unobtrusive.parse("#insuranceCompanyForm");

$(document).find('.main-nav li').removeClass('active');
$(document).find('.main-nav li').eq(1).addClass('active');

$(".header-navOption a").removeClass("active");
$(".header-navOption a.insurancecompany").addClass("navOptionSelected");

$.extend(true, $.fn.dataTable.defaults, {
    "searching": true,
    "ordering": true,
    "paging": false,
    "info": false,
    "destroy": true
});

if ($("#insuranceCompanyList .dataTables_wrapper").length == 0)
{
    $('#insuranceCompanyTable').DataTable({
        columnDefs: [
        { orderable: false, targets: -1 }
        ],
        "order": [[0, "asc"]],
        "language": {
            "emptyTable": "Insurance companies are not available"
        }
    });
}

$(".dataTables_filter").addClass('display-none');



$("#searchCompanyName").off().on('keyup', function () {
    $('#insuranceCompanyTable').DataTable().search($(this).val()).draw();
});


$(".delete-action").off().on('click', function () {
    var id = $(this).attr("data-id");
    var url = $(this).attr("data-url-delete");
    var urlForValidation = $(this).attr("data-url-valid");
    insuranceCompanyAlreadyUsed(urlForValidation, url, id);

});

$("#addInsuranceCompany").off().on('click', function () {
    var url = $(this).attr("data-url");
    $("#insuranceCompanyTable tr").removeClass("rowSelected");
    $.ajax({
        url: url,
        type: "GET",
        cache: false,
        success: function (returnData) {
            $("#divInsuranceCompany").html(returnData);
        },
        error: onErrorHandler,
    });
});

$("#cancelInsurance").off().on('click', function () {
    var isDirty = $("#insuranceCompanyForm .form-control").attr("data-isDirty");
    if (isDirty) {
        //if (confirm("Do you want to discard changes")) {
        //    clearValues();
        //}
        //ShowConfirmBox("Do you want to discard changes", true, function () {
            clearValues();
        //});
    }
});

$(".insurance-column").off().on('click', function () {
    var id = $(this).attr("data-id");
    var url = $(this).attr("data-url");
    getInsuranceDetails(url, id);
    //$("#insuranceCompanyTable tr").removeClass("rowSelected");
    $("#insuranceCompanyTable").dataTable().$('tr.rowSelected').removeClass("rowSelected");
    $(this).closest('tr').addClass('rowSelected');
});


$("#insuranceCompanyForm .form-control").off().on("input", function () {
    if ($(this).val() == $(this).get(0).defaultValue) return;
    $("#insuranceCompanyForm .form-control").attr('data-isDirty', true);
    $(".form-buttons").removeAttr("disabled");
});




function successData(data) {
    if (data.Data == false) {
        $("#errorCompanyname").text("Company name already exists");
    }
    else {
        $("#insuranceCompanyList").html(data);
        clearValues();
    }
}

function clearValues() {
    $("#insuranceCompanyForm .form-control").val("");
    $("#insuranceCompanyTable tr").removeClass("rowSelected");
    $("#insuranceCompanyTitle").text("New Insurance Company");
    $(".form-buttons").attr("disabled", "disabled");
    $("#errorCompanyname").text("");
    $("#searchCompanyName").val("");
    $(".field-validation-error").text("");
    $("#insuranceCompanyId").val(0);
}

function getInsuranceDetails(url, id) {
    $.ajax({
        url: url,
        data: { id: id },
        type: "GET",
        cache: false,
        success: function (returnData) {
            $("#divInsuranceCompany").html(returnData);
        },
        error: onErrorHandler,
    });
}

function deleteInsuranceCompany(url, id) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: url,
        data: { id: id, __RequestVerificationToken: token },
        type: "POST",
        cache: false,
        success: function (returnData) {
            clearValues();
            $("#insuranceCompanyList").html(returnData);
        },
        error: onErrorHandler,
    });
}

function insuranceCompanyAlreadyUsed(urlForValidation, url, id) {
    $.ajax({
        url: urlForValidation,
        data: { id: id },
        type: "GET",
        cache: false,
        success: function (returnData) {
            if (returnData.Data) {
                //alert("Insurance company can not be deleted because it is associated with claim.")
                ShowConfirmBox("Insurance company can not be deleted because it is associated with claim.", false);
            }
            else {
                //if (confirm("Do you want to delete insurance company")) {
                //    deleteInsuranceCompany(url, id);
                //}
                ShowConfirmBox("Do you want to delete insurance company?", true, function () {
                    deleteInsuranceCompany(url, id);
                });
            }
        },
        error: onErrorHandler,
    });
}

