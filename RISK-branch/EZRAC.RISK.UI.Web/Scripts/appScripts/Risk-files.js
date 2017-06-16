$(document).ready(function () {    
    var url = $('.delete-action').attr('href');
    var claimId = $('#ClaimID').val();
    $('.delete-action').attr('href', url + '?claimId=' + claimId);

    GetFilesDetail();

    $('#fileTypes').change(function () {
        $('#fileUploadResult').text('');
        $('#validationMessage').text("");
    })

    $('#txtUploadfile').on('change', function (e) {
        var files = e.target.files;
        $("#fileValidation").text("");
        if (files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                $("#fileinfo").html("");
                for (var x = 0; x < files.length; x++) {
                    $("#fileinfo").append($("<div>" + files[x].name + " </div>"));
                    //data.append("file" + x, files[x]);
                }

                //formData.append("fileCategoryId", 1);
                //data.append("claimId", 23456);
                //$.ajax({
                //    type: "POST",
                //    url: '/RiskFiles/Post',
                //    contentType: false,
                //    processData: false,
                //    data: data,
                //    success: function (result) {
                //        console.log(result);
                //    },
                //    error: function (xhr, status, p3, p4) {
                //        var err = "Error " + " " + status + " " + p3 + " " + p4;
                //        if (xhr.responseText && xhr.responseText[0] == "{")
                //            err = JSON.parse(xhr.responseText).Message;
                //        console.log(err);
                //    }
                //});
            } else {
                //alert("This browser doesn't support HTML5 file uploads!");
                ShowConfirmBox("This browser doesn't support HTML5 file uploads!", false);
            }
        }
        $('#fileUploadResult').text('');
    });

    $('#btnFileSave').on('click', function (e) {
        var target = document.getElementById("txtUploadfile");
        var files = $('#txtUploadfile').files;
        var claimId = $('#ClaimID').val();
        var fileType = $("#fileTypes option:selected").val();
        $("#validationMessage").html("");
        if (fileType == "" || fileType < 0) {
            $("#validationMessage").html("Please select a category");
            return;
        }

        if ($("#txtUploadfile").val() == "") {
            $("#fileValidation").html("Please select atleast 1 file");
            return;
        }


        if (target.files.length > 0) {
            if (window.FormData !== undefined) {
                var data = new FormData();
                for (var x = 0; x < target.files.length; x++) {
                    data.append("file" + x, target.files[x]);
                }
                var token = $('input[name="__RequestVerificationToken"]').val();

                var fileType = $("#fileTypes option:selected").val();
                data.append("SelectedFileTypeId", fileType);
                data.append("ClaimID", claimId);
                data.append("__RequestVerificationToken", token);                
                $.ajax({
                    type: "POST",
                    url: postFileUrl,
                    contentType: false,
                    processData: false,
                    data: data,
                    success: function (result) {
                        $("#txtUploadfile").val('');
                        $("#fileinfo").html("");
                        $("#fileUploadResult").html("File Uploaded successfully");
                        clearValues();
                        GetFilesDetail();
                    },
                    error: function (xhr, status, p3, p4) {
                        onErrorHandler(xhr);
                        var err = "Error " + " " + status + " " + p3 + " " + p4;
                        if (xhr.responseText && xhr.responseText[0] == "{")
                            err = JSON.parse(xhr.responseText).Message;
                        $("#fileUploadResult").html("File Uploaded Failed");
                    }
                });
            } else {
                //alert("This browser doesn't support HTML5 file uploads!");
                ShowConfirmBox("This browser doesn't support HTML5 file uploads!", false);
            }
        }
    });

    $('#btnFileCancel').on('click', function (e) {
        //$("#txtUploadfile").val('');
        //$("#fileinfo").html("");
        $("#fileUploadResult").html("");
        //$('#fileValidation').text('');
        //$('#fileTypes').selectpicker('val', '');
        clearValues();
    });
    $(".selectpicker").selectpicker({});    
});

function clearValues() {
    $("#txtUploadfile").val('');
    $("#fileinfo").html("");
    //$("#fileUploadResult").html("");
    $('#fileValidation').text('');
    $('#fileTypes').selectpicker('val', '');
    $("#validationMessage").html("");
}

function GetFilesDetail() {

        var claimId = $('#ClaimID').val();
        $.ajax({
            url: getFileDetailsUrl,
            data: { id: claimId },
            type: "GET",
            cache: false,
            success: function (returnData) {
                $("#fileList").html(returnData);
            },
            error: onErrorHandler,
        });
    }

