
    $(document).ready(function (e) {       
        $.extend(true, $.fn.dataTable.defaults, {
            "searching": true,
            "ordering": true,
            "paging": true,
            "info": false,
            "destroy": true,
            "scrollY": true,
        });

        $('#tblClaimEvents').DataTable({
            columnDefs: [
            { orderable: false}
            ],
            "order": [[0, "desc"]],
            "dom": '<"top"i>rt<"bottom"flp><"clear">',
            "language": {
                "lengthMenu": "Display _MENU_ records",
                "emptyTable": "Claim events are not available"
            }
        });

        //$('#tblClaimEvents').DataTable({
        //    "dom": '<"top"i>rt<"bottom"flp><"clear">'
        //});
        $("#searchText").keyup(function () {
            $('#tblClaimEvents').DataTable().search($(this).val()).draw();
        });

        $('#tblClaimEvents_filter').remove();


        $("#tblClaimEvents_length").css("margin-top", "10px");
        $("#tblClaimEvents_paginate").css("margin-bottom", "5px");
        
        // $.validator.unobtrusive.parse("#frm-save-claim-header");
    });