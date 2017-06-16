$(document).ready(function () {


    $(document).find('.main-nav li').removeClass('active');
    $(document).find('.main-nav li.reports').addClass('active');


    $("#main").find(".left-col-toggle").hide();
    $('#left-col .collapse').eq(0).css('cursor', 'pointer').click(function () {
        //if ($(this).attr('src').indexOf('expand') > 0) {
        //    $(this).attr('src', '../Images/Search-collapse.png');
        //}
        //else {
        //    $(this).attr('src', '../Images/expand.png')
        //}

        $('#reportLeftPanel').slideToggle();

        //if ($('#left-col .collapse[src="../images/expand.png"]').length == 2) {
        //    AnimateLeftPanel();
        //}
    });

    $('.toggle').css('cursor', 'pointer').click(function () {
        AnimateLeftPanel();
    });
})


//------Other functions--------
var ExportAllClaims = function () {
    var dateRange = true, IsDateEmpty = false;
    if ($("#FromDate").is(":visible")) {
        if ($("#FromDate").val() != "" && $("#ToDate").val() != "") {
            IsDateEmpty = true;
        }
        if (IsDateEmpty && Date.parse($("#FromDate").val()) > Date.parse($("#ToDate").val())) {
            IsDateEmpty = true;
            dateRange = false;
        }
    }
    if (dateRange && IsDateEmpty) {
        var fromDate = $("#FromDate").val();
        var toDate = $("#ToDate").val();
        var type = $("#ExportType").val();
        window.location.href = "../Report/ExportAllClaims?fromDate=" + fromDate + "&toDate=" + toDate + "&type=" + type;
    }
    else {
        var message = "To date should be greater than from date.";
        if (!IsDateEmpty) {
            message = "Please select date range";
        }
        ShowConfirmBox(message, false);
    }
}
function AnimateLeftPanel() {

    $('#reportLeftPanel').slideUp();
    $('#left-col').hide('slide', { direction: 'left' }, 1000);
    //$('#right-col').addClass('calculatedWidth');
    $('#right-col .report-rightcol').css("width", "100%");
    //$('#left-col .collapse').attr('src', '../Images/expand.png');

    setTimeout(function () {
        $('.left-col-toggle').show(250).click(function () {
            $(this).hide(250);
            setTimeout(function () {
                $('#left-col').show('slide', { direction: 'left' }, 750);
                setTimeout(function () {
                    $('#reportLeftPanel').slideDown();
                    // $('#right-col').removeClass('calculatedWidth');
                    $('#right-col .report-rightcol').css("width", "75%");
                    // $('#left-col .collapse').attr('src', '../Images/Search-collapse.png');
                }, 750);
            }, 250);
        });
    }, 750);
}
function GetReportCriteria(data) {
    $("#ReportSearchCriteria").html(data);
    $("#ReportResult").hide();
}
function GetReportResult(data) {
    $("#ReportResult").show();
    $("#ReportResult").html(data);

    //ReportSummary();
    //console.log(data);
}
function ReportSummary() {
    var CurrentReportKey = $("#CurrentReportKey").val();
    if (CurrentReportKey == "tag-platereports") {
        $("#tagnumber").text($("#TagNumber").val());
    }
    if (CurrentReportKey == "adminreports") {
        var UserName = '';
        $("#UserIds option:selected").each(function () {
            UserName += $(this).text() + ", ";
        });
        $("#Usernames").text(UserName.substr(0, UserName.length - 2));
    }
    //Date range
    if (CurrentReportKey == "basicreports" || CurrentReportKey == "adminreports" || CurrentReportKey == "writeoffreport" || CurrentReportKey == "collectionreport" || CurrentReportKey == "accountsreceivablereport" || CurrentReportKey == "chargebacklossreport" || CurrentReportKey == "vehicledamagesectionreport" || CurrentReportKey == "adminfeecommissionreport" || CurrentReportKey == "crispininvoice" || CurrentReportKey == "depositdatereport" || CurrentReportKey == "chargesreport" || CurrentReportKey == "vehicledamagesectionreport") {
        if ($("#FromDate").val() != "" || $("#ToDate").val()) {
            $("#SummaryDateRange").text($("#FromDate").val() + " - " + $("#ToDate").val());
        }
    }
    if (CurrentReportKey == "agingreport") {
        var AsOfdate = "";
        if ($("#AsOfDate").val() == "") {
            var CurrentDate = new Date();
            AsOfdate = CurrentDate.toLocaleDateString();
        }
        else {
            AsOfdate = $("#AsOfDate").val();
        }
        $("#SummaryAsOfDate").text(AsOfdate);
    }
    if (CurrentReportKey == "basicreports") {
        $("#DateType").text($("#DateTypeKey option:selected").text());
    }

    //Location
    if (CurrentReportKey == "basicreports" || CurrentReportKey == "vehicledamagesectionreport" || CurrentReportKey == "vehiclestobereleasedreport" || CurrentReportKey == "stolen-recoveredreport" || CurrentReportKey == "chargebacklossreport" || CurrentReportKey == "agingreport" || CurrentReportKey == "accountsreceivablereport" || CurrentReportKey == "collectionreport" || CurrentReportKey == "writeoffreport" || CurrentReportKey == "vehicledamagesectionreport" || CurrentReportKey == "adminfeecommissionreport" || CurrentReportKey == "chargesreport") {
        var LocationName = '';
        $("#LocationIds option:selected").each(function () {
            LocationName += $(this).text() + ", ";
        });
        $("#LocationNames").text(LocationName.substr(0, LocationName.length - 2));
    }

    if (CurrentReportKey == "depositdatereport") {
        $("#SummaryPaidFrom").text($("#PaymentTypeId option:Selected").text());
    }
    if (CurrentReportKey == "chargesreport") {

    }
    var CurrentDate = new Date();
    $("#ReportDate").text(CurrentDate.toLocaleDateString() + " " + CurrentDate.toLocaleTimeString());
}
function validationReportCriteria() {
    var isSelectBoxSelected = false, dateRange = false, isTextbox = false, IsAdminReportType = false, IsLocation = false, IsDateType = false;
    var CurrentReportKey = $("#CurrentReportKey").val();
    if ($("#TagNumber").val() == "") {
        isSelectBoxSelected = false;
        isTextbox = true;
    }

    //Select box validation
    if ($("#ReportSearchCriteria select").length > 0) {
        $("#ReportSearchCriteria select").each(function () {
            if ($(this).val() == null || $(this).val() == "" || $(this).val() == "0") {
                isSelectBoxSelected = false;
            }
            else {
                isSelectBoxSelected = true;
                return false;
            }
        });
    }
    else {
        if (!isTextbox) {
            isSelectBoxSelected = true;
        }
    }

    if ($("#FromDate").is(":visible")) {
        if ($("#FromDate").val() != "" && $("#ToDate").val() != "") {
            isSelectBoxSelected = true;
        }
        if ($("#FromDate").val() != "" && $("#ToDate").val() != "" && Date.parse($("#FromDate").val()) > Date.parse($("#ToDate").val())) {
            isSelectBoxSelected = false;
            dateRange = true;
        }
    }
    if (CurrentReportKey == "adminreports") {
        if ($("#ReportTypeKey").val() == "") {
            isSelectBoxSelected = false;
            IsAdminReportType = true;
        }
    }
    if (CurrentReportKey == "basicreports") {
        if ($("#DateTypeKey").val() == "") {
            isSelectBoxSelected = false;
            IsDateType = true;
        }
    }

    if ($("#LocationIds").is(":visible")) {

        if ($("#LocationIds").val() == null) {

            isSelectBoxSelected = false;
        }
        IsLocation = true;
    }
    if (!isSelectBoxSelected) {
        if (dateRange) {
            ShowConfirmBox("To date should be greater than from date.", false);
        }
        else if (isTextbox) {
            ShowConfirmBox("Please enter value", false);
        }
        else if (IsAdminReportType) {
            ShowConfirmBox("Please select report type", false);
        }
        else if (IsDateType) {
            ShowConfirmBox("Please select date type", false);
        }
        else if (IsLocation) {
            ShowConfirmBox("Please select location", false);
        }
        else {
            ShowConfirmBox("Please fill any report criteria", false);
        }
    }
    return isSelectBoxSelected;
}
//-----End other report Criteria-------