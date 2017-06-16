/*
File: TSDAudit.js
Created By: Abhijeet Ghule
Functionality: This file includes functionality to show the details of TSD audits
*/

//TSD document ready
$(document).ready(TSDAuditDocumentReady);

//Show loader before every ajax call
$(document).ajaxStart(function () { $(".loader_container_main").show(); });
//Hide loader before every ajax call
$(document).ajaxComplete(function () { $(".loader_container_main").hide(); });

function TSDAuditDocumentReady() {
    $("#lstTSDAudits").on("change", TSDAuditSelectedIndexChange);
    $("#resetAuditSelection").click(ResetAll);
    $("ul.hidden.drop-down1").find('li').eq(0).addClass('selected').closest('.dropdown').find('li').eq(0).attr({ 'value': ($('ul.hidden.drop-down1').find('li').eq(0).attr('value')) }).text($("ul.hidden.drop-down1").find('li').eq(0).text());
    $("ul.hidden.drop-down1 li").bind("click", function () { BrandSelectedIndexChange(this.id); });
}

//Fetch audit logs of selected brand
var BrandSelectedIndexChange = function (selectedBrandID) {
    ResetSelectedAudit();
    var ajaxURl = '/RateShopper/TSDAudit/GetAuditLogs/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetAuditLogs;
    }

    $.ajax({
        type: "GET",
        url: ajaxURl,
        dataType: "json",
        contentType: "application/json; charset=utf-8;",
        data: { brandID: selectedBrandID },//JSON.stringify({ brandID: selectedBrandID }),
        success: function (data) {
            $("#lstTSDAudits option").remove();            
            if (data.length > 0) {
                for (i = 0; i < data.length; i++) {
                    $("#lstTSDAudits").append("<option value=log_" + data[i].ID + ">" + data[i].Name + " " + data[i].LocationCode + " " + data[i].StrLogDateTime + "</option>");
                }
            }
            else {
                $("#lstTSDAudits").append("<option value=0>No Audit Logs Found</option>");
            }
        },
        error: function (e) {
            console.log(e.message);
        }
    });
}

//Get selected audit log and fetch details from DB
var TSDAuditSelectedIndexChange = function () {
    
    var selectedAuditID = $("#lstTSDAudits").val();
    if (selectedAuditID == 0) {
        return;
    }
    selectedAuditID = String(selectedAuditID).replace("log_", "");
    
    var ajaxURl = '/RateShopper/TSDAudit/GetAuditDetails/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetAuditDetails;
    }

    if (parseInt(selectedAuditID) > 0) {
        $.ajax({
            type: "POST",
            url: ajaxURl,
            dataType: "json",
            contentType: "application/json; charset=utf-8;",
            data: JSON.stringify({ auditID: selectedAuditID }),
            success: function (data) {
                if (data.ID > 0) {
                    $("#spnResponseCode").html(data.ResponseCode);
                    $("#spnResponseMessage").html(data.Message);
                    $("#spnStatus").html(data.RequestStatus);
                    $("#txtRequestXML").val(data.XMLRequest);
                    if (data.ErrorMessage!= null && data.ErrorMessage != '') {
                        $("#spnErrorMessageContainer").show();
                        $("#spnErrorMessage").html(data.ErrorMessage);
                    }
                    else {
                        $("#spnErrorMessage").html('');
                        $("#spnErrorMessageContainer").hide();
                    }
                }
                else {
                    $("#spnResponseCode").html('');
                    $("#spnResponseMessage").html('');
                    $("#spnStatus").html('');
                    $("#txtRequestXML").val('');
                    $("#spnErrorMessage").html('');                   
                    $("#spnErrorMessageContainer").hide();
                }
            },
            error: function (e) {
                console.log(e.message);
            }
        });
    }
}

//Reset selected audit log
var ResetSelectedAudit = function () {
    $("#lstTSDAudits option").prop("selected", false);
    $("#spnResponseCode").html('');
    $("#spnResponseMessage").html('');
    $("#spnStatus").html('');
    $("#txtRequestXML").val('');
    $("#spnErrorMessage").html('');
    $("#spnErrorMessageContainer").hide();
}

//Reset form
var ResetAll = function () {
    ResetSelectedAudit();
    ResetBrandSelection();
}

//Reset brand selection
var ResetBrandSelection = function () {
    if ($("#ddlBrand-TSD li.selected").attr("id") > 0) {
        $("ul.hidden.drop-down1").find('li').removeClass("selected");
        $("ul.hidden.drop-down1").find('li').eq(0).addClass('selected').closest('.dropdown').find('li').eq(0).attr({ 'value': ($('ul.hidden.drop-down1').find('li').eq(0).attr('value')) }).text($("ul.hidden.drop-down1").find('li').eq(0).text());
        BrandSelectedIndexChange($("#ddlBrand-TSD li.selected").attr("id"));
    }
}