
$(document).ready(function (e) {

          if ($(".selectpicker")[0]) {
              $(".selectpicker").selectpicker();
              $('.selectpicker').parents('form:first').validate().settings.ignore = ':not(select:hidden, input:visible, textarea:visible)';
          }


          var dateToday = new Date();
          $(".datepicker-followupdate").datepicker({
              minDate: dateToday,
              beforeShowDay: $.datepicker.noWeekends
          });


          var dateToday = new Date();
          $(".datepicker-opendate").datepicker({
              minDate: dateToday,
              //beforeShowDay: $.datepicker.noWeekends
          });

          var dateToday = new Date();
          $(".datepicker-closeDate").datepicker({
              //minDate: dateToday,
              //beforeShowDay: $.datepicker.noWeekends
          });

});


function onErrorHandler(xhr) {
    if (xhr.status == 401) {
        window.location = loginUrl + "?returnURL=" + window.location.pathname;
    }
}

function RiskAjax(url, data, dataType, formType, cache, success, error) {


    dataType = dataType == null ? 'html' : dataType;
    formType = formType == null ? 'GET' : formType;
    cache = cache == null ? false : cache;
    

    $.ajax({
        url: url,
        data: data,
        dataType: dataType,
        type: formType,
        cache: cache,
        //contentType: "application/json;charset=utf-8",
        success: function (returnData) {

            if (success != null && success != undefined) {
                success(returnData);
            }
        },
        error: function (xhr, ajaxOptions, thrownError) {
           
            onErrorHandler(xhr);
            if (error != null && error != undefined) {
                error(xhr, ajaxOptions, thrownError);
            }

        }
    });
}


function ShowConfirmBox(message, isConfirmBox, successFunction, result,cancelFunction) {
    $('.popup_bg_master').show();
    $('#alertbox').show().draggable().find('#ConfirmText').html(message).closest('#alertbox').find('#ConfirmCancel, #closepopup').click(function () {

        $('#alertbox, .popup_bg_master').hide();

    }).closest('#alertbox').find('#ConfirmOk').unbind('click').bind('click', function () {
        $('#alertbox, .popup_bg_master').hide();
  
        if (successFunction != undefined) {
            successFunction(result);
        }
    });

    $('#alertbox').find('#ConfirmCancel').unbind('click').bind('click', function () {
        $('#alertbox, .popup_bg_master').hide();
        if (cancelFunction != undefined) {
           
            cancelFunction();
        }
    });

    isConfirmBox ? $('#alertbox #ConfirmCancel').show() : $('#alertbox #ConfirmCancel').hide();
}
function callMe() {
    //alert(this);
    ShowConfirmBox(this, false);
}
