var AreRuleSetLoadedAfterLocationChanged = false; //For Wide gap check uncheck scenario.
$(document).ready(function () {
    $("#mr_AutoConsoleRuleSet_popup").draggable();
    $("#NotFoundAutomationConsoleRuleSet").hide();
    $("#AutomationConsoleRuleSetLink").on("click", function () {
        if ($("#view1 #locations select").val() != null && $("#view1 #locations select").val() != undefined) {
            $("#mr_AutoConsoleRuleSet_popup, .popup_bg").show();
            $("#CreateRuleset,#Filter").hide();
            $("#rightRuleSetName").prop("disabled", true);//Rulesetname disable mode
            $("#rightRuleSetlocations").hide();
            $("#mr_AutoConsoleRuleSet_popup #rightRuleSetIsWideGapTemplate").prop("disabled", true);
            $('#mr_AutoConsoleRuleSet_popup #isGOV').prop("disabled", true);
            IsAutomationRuleSet = true;
            GetAutomationRentalLength();
            GetAutomationLocationBrand();
            if ($("input[name=rulesetfilter]:radio:checked").val() == "RdBaseCost") {
                IsCompeteOnBase = true;
            }
            else if ($("input[name=rulesetfilter]:radio:checked").val() == "RdTotalCost") {
                IsCompeteOnBase = false;
            }
            $("#mr_AutoConsoleRuleSet_popup #RuleSetList li").eq(0).click();
        }
        else {
            ShowConfirmBox('Please select location', false);
        }
    });
    $("#closepopupAutoRuleSet").on("click", function () {
        $("#mr_AutoConsoleRuleSet_popup #RuleSetList").scrollTop(0);
        $("#mr_AutoConsoleRuleSet_popup, .popup_bg").hide();
        $('#mr_AutoConsoleRuleSet_popup #spanSave').hide();
        $("#mr_AutoConsoleRuleSet_popup #RuleSetList li").eq(0).click();
        IsAutomationRuleSet = false;
    });

});
//Other functions
function GetAutomationRentalLength() {
    var rentalLengthArray = [];
    var SelectedRentaleLength = $("#view1 #lengths select").val();
    $("#view1 #lengths select option").each(function () {
        var $optionItem = $(this);
        if ($.inArray($optionItem.val(), SelectedRentaleLength) != -1) {
            var rentalLength = new Object();
            rentalLength.ID = $optionItem.attr("rid");
            rentalLength.MappedID = $optionItem.val();
            rentalLength.Code = $optionItem.text();
            rentalLengthArray.push(rentalLength);
        }
    });
    if (rentalLengthArray != []) {
        var rsrcs = $.map(rentalLengthArray, function (item) { return new rentalLengths(item); });
        ruleSetViewModel.AutomationRentalLengths(rsrcs);
    }
}
function GetAutomationLocationBrand() {
    var locationBrandArray = [];
    var SelectedLocationBrand;
    $("#view1 #locations select option[value=" + $("#view1 #locations select").val() + "]").each(function () {
        if ($(this).prop("selected")) {
            SelectedLocationBrand = $(this).attr("brandid");
        }
    });

    $("#view1 #locations select option").each(function () {
        var $optionItem = $(this);
        if ($optionItem.attr("brandid") == SelectedLocationBrand) {
            var brandLocations = new Object();
            brandLocations.LocationBrandAlias = $optionItem.text();
            brandLocations.LocationID = $optionItem.val();
            brandLocations.ID = $optionItem.attr("brandid");
            brandLocations.BrandID = $optionItem.attr("companyid");
            brandLocations.LocationCode = $optionItem.attr("locationcode");
            locationBrandArray.push(brandLocations);
        }
    });
    if (locationBrandArray != []) {
        var locationmaparray = $.map(locationBrandArray, function (item) { return new brandLocations(item); });
        ruleSetViewModel.leftAutomationBrandLocations(locationmaparray);
    }
}
function SetSelectedRulesetAttribute(data) {

    $('#mr_AutoConsoleRuleSet_popup #spanSave').show();
    $("#mr_AutoConsoleRuleSet_popup #IntermediateID").val(data.IntermediateID);

    var $selectedAutomationRuleSetID = $("#mr_AutoConsoleRuleSet_popup #RuleSetList").find(".rsselected");
    $selectedAutomationRuleSetID.attr("id", "selectedRuleSetID_" + data.ID);
    $selectedAutomationRuleSetID.attr("isautorulesetinsertupdate", "update");
    $selectedAutomationRuleSetID.attr("scheduledjobrulesetid", data.ScheduledJobRulesetID);
    $selectedAutomationRuleSetID.val(data.ID);

    var RuleSetRentalLength = $("#rightRuleSetlengths").val().toString();
    var RuleSetSelectedCompany = $("#rightRuleSetCompanies").val().toString();
    var RuleSetCarClass = $("#rightRuleSetCarClass").val().toString();
    var RuleSetWeekDay = $("#rightRuleSetDays").val().toString();
    var IsPositionOffset = "";
    var CompanyPositionAbvAvg = "";
    if ($("#rg1:checked").val().trim() == "IsPositionOffsetON") {
        IsPositionOffset = true;
        CompanyPositionAbvAvg = $("#rightRuleSetIsPositionOffset").val();
    }
    else if ($("#rg1:checked").val().trim() == "IsPositionOffsetOFF") {
        IsPositionOffset = false;
        CompanyPositionAbvAvg = $("#rightRuleSetCompanyPositionAbvAvg").val();
    }

    $.each(ruleSetViewModel.automationRuleSets(), function (index, value) {
        if (value.OriginalRuleSetID == $selectedAutomationRuleSetID.attr("originalrulesetid")) {
            value.RuleSetID = data.ID;
            value.CompanyPositionAbvAvg = CompanyPositionAbvAvg;
            value.IsPositionOffset = ko.computed(function () { return IsPositionOffset });
            value.CarClassID = RuleSetCarClass;
            value.RentalLengthID = RuleSetRentalLength;
            value.WeekDaysID = RuleSetWeekDay;
            value.IsAutoRuleSetInsertUpdate = ko.computed(function () { return "update"; });
        }
    });
    $("#mr_AutoConsoleRuleSet_popup #selectedRuleSetID_" + data.ID).click();
    $("#mr_AutoConsoleRuleSet_popup #RuleSetList").scrollTop($("#mr_AutoConsoleRuleSet_popup #RuleSetList")[0].scrollHeight);
    setTimeout(function () {
        $('#mr_AutoConsoleRuleSet_popup #spanSave').hide();
        $("#closepopupAutoRuleSet").click();
        $(window).scrollTop(0);
    }, 2000);
}
//Ened other functions


//Ajax calling function
function GetLocationRuleSet() {
    if ($("#view1 #locations select").val() != null && $("#view1 #locations select").val() != undefined) {
        var IsWideGap = $("#view1 #wideGap").prop("checked");
        var IsGov = $("#view1 #IsGov").prop("checked");
        var LocationBrandID = $("#view1 #locations select option:selected[value=" + $("#view1 #locations select").val() + "]").attr("brandid");
        var ScheduledJobID = 0;
        if (jobId != undefined && jobId != "") {
            ScheduledJobID = jobId;
        }
        if (LocationBrandID != null && (IsWideGap || IsGov)) {
            $('.loader_container_ruleset').show();

            var ajaxURl = '/RateShopper/RuleSet/GetAutomationRuleSet';
            if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
                ajaxURl = AjaxURLSettings.GetAutomationRuleSetUrl;
            }

            $.ajax({
                url: ajaxURl,
                type: 'GET',
                data: { LocationBrandID: LocationBrandID, IsWideGap: IsWideGap, ScheduledJobID: ScheduledJobID, IsGov: IsGov },
                async: true,
                success: function (data) {
                    //console.log(data);
                    $('.loader_container_ruleset').hide();
                    var rsrcs = $.map(data, function (item) { return new ruleSets(item); });
                    ruleSetViewModel.automationRuleSets(rsrcs);
                    //console.log(rsrcs);
                    $("#mr_AutoConsoleRuleSet_popup #RuleSetList li").on("click", function () {
                        $(this).addClass("rsselected");
                        $(this).siblings("li").removeClass("rsselected");
                    });

                    if (rsrcs.length > 0) {
                        $("#AutomationConsoleRuleSetLink").show();
                        $("#NotFoundAutomationConsoleRuleSet").hide();
                        IsAutomationRuleSet = true;
                        if ($("#mr_AutoConsoleRuleSet_popup #RuleSetList li").eq(0).attr("isautorulesetinsertupdate") == "insert") {
                            IsCopyAndCreate = true;
                        }
                        $("#mr_AutoConsoleRuleSet_popup #RuleSetList li").eq(0).click();
                    }
                    else {
                        $("#AutomationConsoleRuleSetLink").hide();
                        $("#NotFoundAutomationConsoleRuleSet").show();
                    }
                    //$("#recentLengths ul li").eq(0).addClass("selected");
                    //$("#RuleSetList li").on("click", function () {
                    //    $(this).addClass("rsselected");
                    //    $(this).siblings("li").removeClass("rsselected");
                    //});
                    $("#divAutomationRulesetAvailable").show();
                    AreRuleSetLoadedAfterLocationChanged = true;

                },
                error: function (e) {
                    $('.loader_container_ruleset').hide();
                    console.log(e.message);
                }
            });
        }
    }
}
//End ajax calling function