
$(document).ready(function () {
    $.extend(true, $.fn.dataTable.defaults, {
        "searching": false,
        "ordering": true,
        "paging": false,
        "info": false,
        "destroy": true
    });

    $.validator.setDefaults({
        ignore: []
    });

    $('#damageTable').DataTable({
        columnDefs: [
        { orderable: false, targets: -1 }
        ],
        "order": [[1, "asc"]],
        "language": {
            "emptyTable": "Damages are not available"
        },
    });

    $('#damageTable').removeClass("no-footer").parent().removeClass("dataTables_scrollBody");

    $(".damageInfoTable").on("click", '.delete-damage', function () {
        var dmgId = $(this).parents().eq(1).find("#damageId").text();
        ShowConfirmBox("Do you want to delete section?", true, function () {
            var dmgInfo = {
                DamageId: dmgId
            }
            var deleteColumn = $(this);
            deleteDamage(dmgId);
        });
        //var confirmDelete = confirm("Do you want to delete section?");

        //if (confirmDelete) {
        //    var dmgId = $(this).parents().eq(1).find("#damageId").text();
        //    var dmgInfo = {
        //        DamageId: dmgId
        //    }
        //    var deleteColumn = $(this);
        //    deleteDamage(dmgId);
        //}
    });

    $.validator.unobtrusive.parse("#addDamageForm");

    $(".selectpicker").selectpicker({});

    $(".close-container").click(function () {
        clearValues();
    });
});

function deleteDamage(dmgId) {
   
    var token = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: $('#deleteDamage').data('url'),
        data: {
            damageId: dmgId,
            claimNumber: $("#claimIdForNotes").val(),
            __RequestVerificationToken: token
        },
        type: "POST",
        cache: false,
        success: function (returnData) {
            if (returnData != null || returnData != undefined) {
                $("#damageTable").html(returnData);
            }
        },
        error: onErrorHandler,
    });
}

function clearValues() {
    $("#damageDetails").val("");
    $('#damageSection').selectpicker('val', "");
    $(".field-validation-error span").text("");
}
