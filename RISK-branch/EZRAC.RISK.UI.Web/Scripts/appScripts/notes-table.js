/// <reference path="../../Views/Notes/_RejectNotes.cshtml" />
$(document).ready(function () {

    $(".notesPageNo").click(function () {
        var showRecordCount = $(".show-record").val();
        if (showRecordCount != undefined) {
            this.href = this.href.replace("xx", showRecordCount);
        }
        else {
            this.href = this.href.replace("xx", 5);
        }
        this.href = this.href.replace("xxx", $("#searchTxt").val());
    });

    $('.notesPageNo').each(function (index, value) {
        var url = $(this).attr('href');
        var searchText = $('#searchTxt').val();
        $(this).attr('href', url + '&searchText=' + searchText);
    });

    $(".show-record").change(function () {
        getCurrentShowNotesForGrid($(".show-record").val());
        $("#recordsToShow").val($(".show-record").val());
    });

    $("#notesInfoTable").on("click", '.delete-notes', function () {
        var noteId = $(this).parents().eq(1).find("#noteId").text();
        ShowConfirmBox("Do you want to delete note?", true, function () {
            deleteNote(noteId);
        });
        //var confirmDelete = confirm("Do you want to delete note?");
        //if (confirmDelete) {
        //    var noteId = $(this).parents().eq(1).find("#noteId").text();
        //    deleteNote(noteId);
        //}
    });

    $(".notesDescription").shorten();

    //$("#noteId").click(function () {
    //    debugger;
    //    var url = $(this).attr("data-url");
    //    showDetailedNote("abc", url);
    //});
});


function showDetailedNote(data, url)
{
    $.ajax({
        url: url,
        async: false,
        success: function (data) {
            $("#openDetailed").html(data);
            $("#detailPop").modal('show');
           // $("#myNavbar").append(data);
        },
        error: onErrorHandler,
    });
}


function getCurrentShowNotesForGrid(recordToGet) {
    $.ajax({
        url: getCurrentNotesGridDateUrl,
        data: { claimNumber: $("#noteclaimId").val(), page: $("#notesPageNo").val(), sortBy: "Date", sortOrder: !$("#sortOrder").val(), recordCountToDisplay: $(".show-record").val(), searchText: $("#searchTxt").val() },
        type: "GET",
        cache: false,
        success: function (returnData) {
            $("#gridContainer").html(returnData);
        },
        error: onErrorHandler,
    });
}


function deleteNote(noteId) {
    var token = $('input[name="__RequestVerificationToken"]').val();
    $.ajax({
        url: deleteNotesUrl,
        data: {
            claimId: $("#noteclaimId").val(),
            noteId: noteId,
            page: $("#notesPageNo").val(),
            sortBy: $("#notesSortBy").val(),
            sortOrder: !$("#sortOrder").val(),
            recordCountToDisplay: $("#recordsToShow").val(),
            searchText: $("#searchTxt").val(),
            __RequestVerificationToken: token
        },
        type: "POST",
        cache: false,
        success: function (returnData) {
            $("#gridContainer").html(returnData);
            $("#searchTxt").val("");
        },
        error: onErrorHandler,
    });
}