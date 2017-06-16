$(document).ready(function () {
    $.extend(true, $.fn.dataTable.defaults, {
        "searching": false,
        "ordering": true,
        "paging": false,
        "info": false,
        "destroy": true
    });

    $('#damageTable').DataTable().destroy();

    $('#damageTable').DataTable({
        columnDefs: [
        { orderable: false, targets: -1 }
        ],
        "order": [[1, "asc"]],
        "language": {
            "emptyTable": "Damages are not available"
        }
    });

    $('#damageTable').removeClass("no-footer").parent().removeClass("dataTables_scrollBody");
});