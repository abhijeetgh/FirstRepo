/*
File: Formula.js
Created By: Abhijeet Ghule
Functionality: This file includes functionality to validate and show the formulas for different brand locations.
*/

$(document).ready(FormulaDocumentReady);

//Show loader till page load complete
$(window).load(function () {
    $(".loader_container_main").fadeOut("slow");
    setTimeout(function () { $("#lblMessage").hide(); }, 3000);
});

//This function is executed on document ready
function FormulaDocumentReady() {
    SetSelectedCompany();
    $(".numeric").bind("input", function () {
        if (this.value != "") {
            validateNumeric(this.id);
            $(this).closest("tr").find(".numeric").each(function () {
                if (this.value == "") {
                    RemoveFlashableTag("#" + this.id);
                }
            });
        }
        else {
            RemoveFlashableTag("#" + this.id);
            var areAllFieldEmpty = true;
            $(this).closest("tr").find(".numeric").each(function () {
                if ($.trim(this.value) == "") {
                    areAllFieldEmpty = areAllFieldEmpty & true;
                }
                else {
                    areAllFieldEmpty = areAllFieldEmpty & false;
                }
            });
            $baseToTotalCost = $($(this).closest("tr").find(".BaseToTotalCost"));
            $totalCostToBase = $($(this).closest("tr").find(".TotalCostToBase"));
            if (areAllFieldEmpty) {
                RemoveFlashableTag($totalCostToBase);
                RemoveFlashableTag($baseToTotalCost);
            }            
        }
        SetIsEditedForCurrentRow(this.id);
        if ($(".temp").length == 0) {
            $("#lblMessage").hide();            
        }
    });

    $(".TotalCostToBase,.BaseToTotalCost").keyup(function () {
        if (this.value != "") {
            RemoveFlashableTag("#" + this.id);
        }
        else {
            $baseToTotalCost = $($(this).closest("tr").find(".BaseToTotalCost"));
            $totalCostToBase = $($(this).closest("tr").find(".TotalCostToBase"));
            if ($.trim($totalCostToBase.val()) == "" && $.trim($baseToTotalCost.val()) == "") {
                $(this).closest("tr").find(".numeric").each(function () {
                    if ($.trim(this.value) == "") {
                        RemoveFlashableTag("#" + this.id);
                    }                    
                });
                RemoveFlashableTag($totalCostToBase);
                RemoveFlashableTag($baseToTotalCost);
            }
        }
        SetIsEditedForCurrentRow(this.id);
        if ($(".temp").length == 0) {
            $("#lblMessage").hide();
        }
    });
    
    $("#btnFormulaSave").click(function () { return SaveClick() });
}

//Set selected company in brand drop down
var SetSelectedCompany = function () {
    var companyID = $("#selectedCompanyID").val();
    $("ul.hidden.drop-down1 li").removeClass("selected");
    if (companyID != null && companyID > 0) {
        $("ul.hidden.drop-down1 li#" + companyID).eq(0).addClass('selected').closest('.dropdown').find('li').eq(0).attr({ 'value': ($('ul.hidden.drop-down1 li#' + companyID).eq(0).attr('value')) }).text($("ul.hidden.drop-down1").find('li#' + companyID).eq(0).text());
    }
    else {
        $("ul.hidden.drop-down1 li").eq(0).addClass('selected').closest('.dropdown').find('li').eq(0).attr({ 'value': ($('ul.hidden.drop-down1 li').eq(0).attr('value')) }).text($("ul.hidden.drop-down1").find('li').eq(0).text());
    }    
}

//This function is used to validate the numeric values
var validateNumeric = function (controlId) {
    var regExp = new RegExp(/^\d{1,10}(\.\d{1,4})?$/);
    var originalValue = $("#" + controlId).val();
    var value = originalValue.replace(/\,/g, '');
    
    //if (!regExp.test(value)) {
    if (!$.isNumeric(value)) {
        MakeTagFlashable("#" + controlId);
        
    }
    else {
        RemoveFlashableTag("#" + controlId);
    }
    if (String($.trim(originalValue)).indexOf(',') > -1) {
        $("#" + controlId).val(value);
    }
    AddFlashingEffect();    
}

//This function is used to validate the input fields of form
var SaveClick = function () {    
    $("#lblMessage").hide();

    if ($(".norecord").length > 0) {
        DisplayMessage("No records to save.", true);
        return false;
    }

    $(".numeric").each(function () {
        if ($.trim(this.value) != "") {
            validateNumeric(this.id);            
        }
    });

    var invalidFormulaError = '';
    $(".row").each(function () {
        var areAllFieldEmpty = true;
        $(this).find(".numeric").each(function () {
            if ($.trim(this.value) == "") {
                areAllFieldEmpty = areAllFieldEmpty & true;
            }
            else {
                areAllFieldEmpty = areAllFieldEmpty & false;
            }
        });
        $baseToTotalCost = $($(this).closest("tr").find(".BaseToTotalCost"));
        $totalCostToBase = $($(this).closest("tr").find(".TotalCostToBase"));

        //if formula present and if all values are empty then show validation
        if (areAllFieldEmpty && ($.trim($totalCostToBase.val()) != "" || $.trim($baseToTotalCost.val()) != "")) {
            $(this).find(".numeric").each(function () {
                if (this.value == "") {
                    MakeTagFlashable("#" + this.id);
                }
            });
            if ($.trim($totalCostToBase.val()) == "") {
                MakeTagFlashable($totalCostToBase);
            }
            if ($.trim($baseToTotalCost.val()) == "") {
                MakeTagFlashable($baseToTotalCost);
            }
        }
        else if (areAllFieldEmpty) {
            RemoveFlashableTag($totalCostToBase);
            RemoveFlashableTag($baseToTotalCost);
        }
        else {
            if ($.trim($totalCostToBase.val()) == "") {
                MakeTagFlashable($totalCostToBase);
            }
            if ($.trim($baseToTotalCost.val()) == "") {
                MakeTagFlashable($baseToTotalCost);
            }
        }
        if (!evaluateFormula($(this))) {
            invalidFormulaError += ", " + $(this).find('td').eq(0).text().trim();

        }
    });
    
    if ($(".temp").length > 0) {
        var errorMsg = "Please review the fields highlighted in Red.";
        if (invalidFormulaError != '') {
            errorMsg += " Formula validation failed for" + invalidFormulaError.substring(1) + ".";
        }

        DisplayMessage(errorMsg, true);
        return false;
    }
    $("#formula_UserID").val($("#LoggedInUserId").val());
    $("#formula_BrandID").val($("#selectedCompanyID").val());

    


    //if ($(".numeric.temp").length > 0) {
    //    DisplayMessage("Please review all fields hi-lighted in red", true);
    //    return false;
    //}
    return true;
}

//Set edit flag for current row
var SetIsEditedForCurrentRow = function (controlID) {
    var hdnIsEdited = $("#" + controlID).closest("tr").find(".isEdited");
    if (hdnIsEdited.length > 0) {
        $(hdnIsEdited).prop("value", true);
    }
}

//Show the success/error messages
var DisplayMessage = function (message, isError) {
    $("#lblMessage").html(message).css({ "color": isError ? "red" : "green", "display": "inline-block", "width": "30%" });
    if (!isError) {
        setTimeout(function () { $("#lblMessage").hide(); }, 3000);
    }
}
//test
//Execute formula for validation
function evaluateFormula(thisRow) {

    var returnVal = true;

    var $totalToBase = $(thisRow).find($('[id$="TotalCostToBase"]'));
    var $baseToTotal = $(thisRow).find($('[id$="BaseToTotalCost"]'));

    var totalTobaseFormula = $totalToBase.val().trim();
    var baseToTotalFormula = $baseToTotal.val().trim();

    if (totalTobaseFormula != '' || baseToTotalFormula != '') {

        //hardcoded values
        var total = 100;
        var rt = 100;
        var nd = 3;
        var len = 3;

        var sales = $(thisRow).find($('[id^="SalesTax"]')).val().trim();
        var airport = $(thisRow).find($('[id^="AirportFee"]')).val().trim();
        var arena = $(thisRow).find($('[id^="Arena"]')).val().trim();
        var surcharge = $(thisRow).find($('[id^="Surcharge"]')).val().trim();
        var vlrf = $(thisRow).find($('[id^="VLRF"]')).val().trim();
        var cfc = $(thisRow).find($('[id^="CFC"]')).val().trim();

        sales = sales != '' ? parseFloat(sales) : undefined;
        airport = airport != '' ? parseFloat(airport) : undefined;
        arena = arena != '' ? parseFloat(arena) : undefined;
        surcharge = surcharge != '' ? parseFloat(surcharge) : undefined;
        vlrf = vlrf != '' ? parseFloat(vlrf) : undefined;
        cfc = cfc != '' ? parseFloat(cfc) : undefined;

        if (totalTobaseFormula != '') {
            var outputTtoB;
            try {
                outputTtoB = eval(totalTobaseFormula);
                if (isNaN(outputTtoB)) {
                    MakeTagFlashable($totalToBase);
                    returnVal = false;
                }
                else {
                    RemoveFlashableTag($totalToBase)
                }
            }
            catch (err) {
                console.log(err.message);
                MakeTagFlashable($totalToBase);
                returnVal = false;
            }
}

        if (baseToTotalFormula != '') {
            var outputBtoT;
            try {
                outputBtoT = eval(baseToTotalFormula);
                if (isNaN(outputBtoT)) {
                    MakeTagFlashable($baseToTotal);
                    returnVal = false;
                }
                else {
                    RemoveFlashableTag($baseToTotal)
                }
            }
            catch (err) {
                console.log(err.message);
                MakeTagFlashable($baseToTotal);
                returnVal = false;
            }
        }
    }
    return returnVal;
}