var ruleSetViewModel;
var ruleSetSmartSearchLocations;
var GlobalRuleSetCarClass;
var globalCompanies;
var carClassID = [], rentalLengthID = [], locationBrandID = [], weekDaysID = [];
var ruleSetGroup = new Array();
var RuleSetID = 0;
var IsCopyAndCreate = false;
var resetLocation = false;
var competitorIds = "";
var allCompanies = "";
//var ruleSetSaveFlag = false;
var IsAutomationRuleSet = false;//This flag is used to check wether the ruleset is automation or not.
var IsCompeteOnBase = false;
$(document).ready(function () {
    ruleSetViewModel = new RuleSetModel();

    ko.applyBindings(ruleSetViewModel, document.getElementById('MainRuleSetPage'));
    BindrentalLengths();
    BindCarClassesRuleSet();
    BindCompanies();
    BindWeekDays();
    BindBrandLocations();
    BindRuleSets();
    BindRuleSetDefaultSetting();
    GetLocationSpecificCarClassesRuleSet();
    GetLocationSpecificBrandCompany();// For check Competitor bind in company drop down

    $("#rightRuleSetCompanies").prop("disabled", true);

    setTimeout(function () {
        AddRuleSetTemplate();
    }, 200);//Add New ruleSetGapTemplate

    $('#RuleSetSearchLocation').bind('input', function () {

        RuleSetSearchLocationGrid('#RuleSetSearchLocation', '#leftRuleSetlocations option');
        if ($("#leftRuleSetlocations option[style$='display: none;']").length == $("#leftRuleSetlocations option").length) {
            MakeTagFlashable('#RuleSetSearchLocation');
        }
        else {
            RemoveFlashableTag('#RuleSetSearchLocation');
        }
        AddFlashingEffect();
    })
    $("#resetSelection").on("click", function () {
        resetLocation = false;
        resetFilterSelection();
    });
    $("#Filter").on("click", function () {
        resetLocation = true;
        $("#RuleSetList li").removeClass("rsselected");
        $("#CreateRuleset").click();
        $("#RuleSetList").scrollTop(0);
        $("#RuleSetList li").show();
        carClassID = [], rentalLengthID = [], locationBrandID = [], weekDaysID = [];
        //if (ValidateRuleSetFilter()) {
        $("#leftRuleSetlengths").find("option:selected").each(function () {
            rentalLengthID.push($(this).attr('value'));
        });
        $("#leftRuleSetCarClass").find("option:selected").each(function () {
            carClassID.push($(this).attr('value'));
        });

        $("#leftRuleSetDays").find("option:selected").each(function () {
            weekDaysID.push($(this).attr('value'));
        });

        $("#leftRuleSetlocations").find("option:selected").each(function () {
            locationBrandID.push($(this).attr('locationbrandid'));
        });
        FilterOperation(carClassID, rentalLengthID, locationBrandID, weekDaysID);
        //}
    });
    $("#rightRuleSetlocations ul").hide();

    $('#left-col .collapse').eq(0).css('cursor', 'pointer').click(function () {
        if ($(this).attr('src').indexOf('expand') > 0) {
            $(this).attr('src', '../images/Search-collapse.png');
        }
        else {
            $(this).attr('src', '../images/expand.png')
        }

        $('#searchLeftPanel').slideToggle();

        if ($('#left-col .collapse[src="../images/expand.png"]').length == 2) {
            AnimateLeftPanel();
        }
    });

    $('#left-col .collapse').eq(1).css('cursor', 'pointer').click(function () {
        if ($(this).attr('src').indexOf('expand') > 0) {
            $(this).attr('src', '../images/Search-collapse.png');
        }
        else {
            $(this).attr('src', '../images/expand.png')
        }

        $('#searchLeftPanelSecond').slideToggle();

        if ($('#left-col .collapse[src="../images/expand.png"]').length == 2) {
            AnimateLeftPanel();
        }
    });

    $('.toggle').css('cursor', 'pointer').click(function () {
        AnimateLeftPanel();
    });
    //Hide Left-Col-Toggle
    $("#main").find(".left-col-toggle").hide();

    $("#UpdateRuleSetTemplate").on("click", function () {
        if (ValidateRuleSetData()) {
            var RuleSetTemplateDTO = new Object();

            RuleSetTemplateDTO.ID = RuleSetID;
            RuleSetTemplateDTO.IsAutomationRuleSet = IsAutomationRuleSet;
            RuleSetTemplateDTO.RuleSetName = $("#rightRuleSetName").val();
            RuleSetTemplateDTO.LoggedInUserID = $('#LoggedInUserId').val();
            if (IsAutomationRuleSet) {
                RuleSetTemplateDTO.OriginalRuleSetID = $("#mr_AutoConsoleRuleSet_popup #RuleSetList").find(".rsselected").attr("originalrulesetid");
                if (RuleSetID > 0) {
                    RuleSetTemplateDTO.ScheduledJobRuleSetID = $("#selectedRuleSetID_" + RuleSetID).attr("scheduledjobrulesetid");
                }
                if (jobId != undefined && jobId != "") {
                    RuleSetTemplateDTO.ScheduledJobID = jobId;
                }
                else {
                    var IntermediateID = $("#mr_AutoConsoleRuleSet_popup #IntermediateID").val().trim();
                    if (IntermediateID != "" && IntermediateID != undefined) {
                        RuleSetTemplateDTO.IntermediateID = IntermediateID;
                    }
                }
            }

            if (RuleSetID != 0) {
                //update Case

                RuleSetTemplateDTO.RuleSetCompanies = $("#rightRuleSetCompanies").val().toString();

                var DeleteRuleSetRentalLengthID = [], AddRuleSetRentalLengthID = [];
                var DeleteRuleSetCarClassID = [], AddRuleSetCarClassID = [];
                var DeleteRuleSetWeekDayID = [], AddRuleSetWeekDayID = [];

                RuleSetTemplateDTO.AddLocationBrandID = 0;
                RuleSetTemplateDTO.LocationBrandID = 0;

                //This for Location Changed
                if ($("#selectedRuleSetID_" + RuleSetID).attr("locationbrandid") != $('#rightRuleSetlocations ul li.selected').attr("LocationBrandID")) {
                    RuleSetTemplateDTO.AddLocationBrandID = parseInt($('#rightRuleSetlocations ul li.selected').attr("LocationBrandID"));
                    RuleSetTemplateDTO.LocationBrandID = parseInt($("#selectedRuleSetID_" + RuleSetID).attr("locationbrandid"));
                }

                //This for RentalLength ruleset if in update case user can add and unselect existing item
                DeleteRuleSetRentalLengthID = $("#selectedRuleSetID_" + RuleSetID).attr("rentallengthid").split(',').filter(function (obj) { return $("#rightRuleSetlengths").val().indexOf(obj) == -1; });
                for (var i = 0; i < $("#rightRuleSetlengths").val().length ; i++) {
                    if ($.inArray($("#rightRuleSetlengths").val()[i], $("#selectedRuleSetID_" + RuleSetID).attr("rentallengthid").split(',')) == -1) {
                        AddRuleSetRentalLengthID.push($("#rightRuleSetlengths").val()[i]);
                    }
                }
                RuleSetTemplateDTO.AddRuleSetLengths = AddRuleSetRentalLengthID.toString();
                RuleSetTemplateDTO.DeleteRuleSetLengths = DeleteRuleSetRentalLengthID.toString();

                //This for CarClass ruleset if in update case user can add and unselect existing item
                DeleteRuleSetCarClassID = $("#selectedRuleSetID_" + RuleSetID).attr("carclassid").split(',').filter(function (obj) { return $("#rightRuleSetCarClass").val().indexOf(obj) == -1; });
                for (var i = 0; i < $("#rightRuleSetCarClass").val().length ; i++) {
                    if ($.inArray($("#rightRuleSetCarClass").val()[i], $("#selectedRuleSetID_" + RuleSetID).attr("carclassid").split(',')) == -1) {
                        AddRuleSetCarClassID.push($("#rightRuleSetCarClass").val()[i]);
                    }
                }
                RuleSetTemplateDTO.AddRuleSetCarClasses = AddRuleSetCarClassID.toString();
                RuleSetTemplateDTO.DeleteRuleSetCarClasses = DeleteRuleSetCarClassID.toString();

                //This for WeekDays ruleset if in update case user can add and unselect existing item
                DeleteRuleSetWeekDayID = $("#selectedRuleSetID_" + RuleSetID).attr("weekdaysid").split(',').filter(function (obj) { return $("#rightRuleSetDays").val().indexOf(obj) == -1; });
                for (var i = 0; i < $("#rightRuleSetDays").val().length ; i++) {
                    if ($.inArray($("#rightRuleSetDays").val()[i], $("#selectedRuleSetID_" + RuleSetID).attr("weekdaysid").split(',')) == -1) {
                        AddRuleSetWeekDayID.push($("#rightRuleSetDays").val()[i]);
                    }
                }
                RuleSetTemplateDTO.AddRuleSetDayOfWeeks = AddRuleSetWeekDayID.toString();
                RuleSetTemplateDTO.DeleteRuleSetDayOfWeeks = DeleteRuleSetWeekDayID.toString();


                //This for IsWideGapTamplate check
                RuleSetTemplateDTO.IsWideGapTemplate = $("#rightRuleSetIsWideGapTemplate").prop("checked");

                //signifies ruleset is GOV
                RuleSetTemplateDTO.IsGOV = $("#isGOV").prop("checked");

                if ($("#rg1:checked").val().trim() == "IsPositionOffsetON") {
                    RuleSetTemplateDTO.IsPositionOffset = 1;
                    RuleSetTemplateDTO.CompanyPositionAbvAvg = $("#rightRuleSetIsPositionOffset").val();
                }
                else if ($("#rg1:checked").val().trim() == "IsPositionOffsetOFF") {
                    RuleSetTemplateDTO.IsPositionOffset = 0;
                    RuleSetTemplateDTO.CompanyPositionAbvAvg = $("#rightRuleSetCompanyPositionAbvAvg").val();
                }
            }
            else {
                //Insert Case
                RuleSetTemplateDTO.LocationBrandID = parseInt($('#rightRuleSetlocations ul li.selected').attr("LocationBrandID"));
                RuleSetTemplateDTO.RuleSetLengths = $("#rightRuleSetlengths").val().toString();
                RuleSetTemplateDTO.RuleSetCompanies = $("#rightRuleSetCompanies").val().toString();
                RuleSetTemplateDTO.RuleSetCarClasses = $("#rightRuleSetCarClass").val().toString();
                RuleSetTemplateDTO.RuleSetDayOfWeeks = $("#rightRuleSetDays").val().toString();
                RuleSetTemplateDTO.IsWideGapTemplate = $("#rightRuleSetIsWideGapTemplate").prop("checked");
                //signifies ruleset is GOV
                RuleSetTemplateDTO.IsGOV = $("#isGOV").prop("checked");

                //if ($("#rightRuleSetIsWideGapTemplate").prop("checked")) {
                if ($("#rg1:checked").val().trim() == "IsPositionOffsetON") {
                    RuleSetTemplateDTO.IsPositionOffset = 1;
                    RuleSetTemplateDTO.CompanyPositionAbvAvg = $("#rightRuleSetIsPositionOffset").val();
                }
                else if ($("#rg1:checked").val().trim() == "IsPositionOffsetOFF") {
                    RuleSetTemplateDTO.IsPositionOffset = 0;
                    RuleSetTemplateDTO.CompanyPositionAbvAvg = $("#rightRuleSetCompanyPositionAbvAvg").val();
                }
                //}
                //else {
                //    RuleSetTemplateDTO.IsPositionOffset = 0;
                //    RuleSetTemplateDTO.CompanyPositionAbvAvg = 0;
                //}
            }

            //RuleSetTemplateDTO.IsPositionOffset = $("#rightRuleSetIsPositionOffset").val();
            //RuleSetTemplateDTO.CompanyPositionAbvAvg = $("#rightRuleSetCompanyPositionAbvAvg").val();


            //Used for RuleSetGroupTemplate Operation
            ruleSetGroup = [];
            if (RuleSetID == 0) {

                //Insert Case

                $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").each(function (index) {
                    var RuleSetGroupDTO = new Object();

                    if ($("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("select").find("option").length != 0) {

                        RuleSetGroupDTO.CompanyIDs = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("select").val().toString();

                        var lstRuleSetGapSettingDay = new Array();
                        var lstRuleSetGapSettingWeek = new Array();
                        var lstRuleSetGapSettingMonth = new Array();

                        var MinAmount = 0, MaxAmount = 0, GapAmount = 0, BaseMinAmount = 0, BaseMaxAmount = 0, BaseGapAmount = 0;// Used for check the loop count of template column(min,max,gap)
                        var mincount = 0, maxcount = 0, gapcount = 0, basemincount = 0, basemaxcount = 0, basegapcount = 0;
                        var rangeIntervalID = 0;
                        var CurrentBaseID = 0;
                        //This is used for Ruleset Day 
                        $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("#GroupDayTemplate tr td").each(function () {

                            if ($(this).find("input[type=text]") != null || $(this).find("input[type=text]") != undefined) {
                                if ($(this).attr("RangeIntervalID") != undefined) {
                                    rangeIntervalID = $(this).attr("RangeIntervalID");
                                    CurrentBaseID = $(this).attr("rulesetgapsettingid");
                                }
                                if ($(this).find("input[type=text]").attr("name") == "DayMinAmount") {
                                    MinAmount = $(this).find("input[type=text]").val();


                                    //This case happen  while user go for copy and create functionality and automation ruleset
                                    if (CurrentBaseID != undefined) {
                                        BaseMinAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseDayMinAmount_" + CurrentBaseID + "]").val();
                                    }
                                    else {
                                        BaseMinAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseDayMinAmount_" + rangeIntervalID + "]").val();
                                    }
                                    mincount = 1;
                                }
                                else if ($(this).find("input[type=text]").attr("name") == "DayMaxAmount") {
                                    MaxAmount = $(this).find("input[type=text]").val();
                                    if (CurrentBaseID != undefined) {
                                        BaseMaxAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseDayMaxAmount_" + CurrentBaseID + "]").val();
                                    }
                                    else {
                                        BaseMaxAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseDayMaxAmount_" + rangeIntervalID + "]").val();
                                    }
                                    maxcount = 1;
                                }
                                else if ($(this).find("input[type=text]").attr("name") == "DayGapAmount") {
                                    GapAmount = $(this).find("input[type=text]").val();
                                    if (CurrentBaseID != undefined) {
                                        BaseGapAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseDayGapAmount_" + CurrentBaseID + "]").val();
                                    }
                                    else {
                                        BaseGapAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseDayGapAmount_" + rangeIntervalID + "]").val();
                                    }
                                    gapcount = 1;
                                }

                                if (gapcount == 1 && maxcount == 1 && mincount == 1) {
                                    var ruleSetGapSettingDay = new Object();

                                    ruleSetGapSettingDay.MinAmount = MinAmount;
                                    ruleSetGapSettingDay.MaxAmount = MaxAmount;
                                    ruleSetGapSettingDay.GapAmount = GapAmount;
                                    ruleSetGapSettingDay.BaseMinAmount = BaseMinAmount;
                                    ruleSetGapSettingDay.BaseMaxAmount = BaseMaxAmount;
                                    ruleSetGapSettingDay.BaseGapAmount = BaseGapAmount
                                    ruleSetGapSettingDay.RangeIntervalID = rangeIntervalID;

                                    lstRuleSetGapSettingDay.push(ruleSetGapSettingDay);

                                    MinAmount = 0, MaxAmount = 0, GapAmount = 0, BaseMinAmount = 0, BaseMaxAmount = 0, BaseGapAmount = 0;
                                    mincount = 0, maxcount = 0, gapcount = 0, basemincount = 0, basemaxcount = 0, basegapcount = 0;
                                    rangeIntervalID = 0;
                                }
                            }
                        });
                        //This is used for RuleSet Week
                        $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("#GroupWeekTemplate tr td").each(function () {
                            if ($(this).find("input[type=text]") != null || $(this).find("input[type=text]") != undefined) {
                                if ($(this).attr("RangeIntervalID") != undefined) {
                                    rangeIntervalID = $(this).attr("RangeIntervalID");
                                    CurrentBaseID = $(this).attr("rulesetgapsettingid");
                                }
                                if ($(this).find("input[type=text]").attr("name") == "WeekMinAmount") {
                                    MinAmount = $(this).find("input[type=text]").val();
                                    if (CurrentBaseID != undefined) {
                                        BaseMinAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseWeekMinAmount_" + CurrentBaseID + "]").val();
                                    }
                                    else {
                                        BaseMinAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseWeekMinAmount_" + rangeIntervalID + "]").val();
                                    }
                                    mincount = 1;
                                }
                                else if ($(this).find("input[type=text]").attr("name") == "WeekMaxAmount") {
                                    MaxAmount = $(this).find("input[type=text]").val();
                                    if (CurrentBaseID != undefined) {
                                        BaseMaxAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseWeekMaxAmount_" + CurrentBaseID + "]").val();
                                    }
                                    else {
                                        BaseMaxAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseWeekMaxAmount_" + rangeIntervalID + "]").val();
                                    }
                                    maxcount = 1;
                                }
                                else if ($(this).find("input[type=text]").attr("name") == "WeekGapAmount") {
                                    GapAmount = $(this).find("input[type=text]").val();
                                    if (CurrentBaseID != undefined) {
                                        BaseGapAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseWeekGapAmount_" + CurrentBaseID + "]").val();
                                    }
                                    else {
                                        BaseGapAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseWeekGapAmount_" + rangeIntervalID + "]").val();
                                    }
                                    gapcount = 1;
                                }

                                if (gapcount == 1 && maxcount == 1 && mincount == 1) {
                                    var ruleSetGapSettingWeek = new Object();

                                    ruleSetGapSettingWeek.MinAmount = MinAmount;
                                    ruleSetGapSettingWeek.MaxAmount = MaxAmount;
                                    ruleSetGapSettingWeek.GapAmount = GapAmount;
                                    ruleSetGapSettingWeek.RangeIntervalID = rangeIntervalID;
                                    ruleSetGapSettingWeek.BaseMinAmount = BaseMinAmount;
                                    ruleSetGapSettingWeek.BaseMaxAmount = BaseMaxAmount;
                                    ruleSetGapSettingWeek.BaseGapAmount = BaseGapAmount
                                    lstRuleSetGapSettingWeek.push(ruleSetGapSettingWeek);

                                    MinAmount = 0, MaxAmount = 0, GapAmount = 0, BaseMinAmount = 0, BaseMaxAmount = 0, BaseGapAmount = 0;
                                    mincount = 0, maxcount = 0, gapcount = 0, basemincount = 0, basemaxcount = 0, basegapcount = 0;
                                    rangeIntervalID = 0;
                                }
                            }
                        });

                        //This is Used for RuleSet Month
                        $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("#GroupMonthTemplate tr td").each(function () {
                            if ($(this).find("input[type=text]") != null || $(this).find("input[type=text]") != undefined) {
                                if ($(this).attr("RangeIntervalID") != undefined) {
                                    rangeIntervalID = $(this).attr("RangeIntervalID");
                                    CurrentBaseID = $(this).attr("rulesetgapsettingid");
                                }

                                if ($(this).find("input[type=text]").attr("name") == "MonthMinAmount") {
                                    MinAmount = $(this).find("input[type=text]").val();
                                    if (CurrentBaseID != undefined) {
                                        BaseMinAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseMonthMinAmount_" + CurrentBaseID + "]").val();
                                    }
                                    else {
                                        BaseMinAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseMonthMinAmount_" + rangeIntervalID + "]").val();
                                    }
                                    mincount = 1;
                                }
                                else if ($(this).find("input[type=text]").attr("name") == "MonthMaxAmount") {
                                    MaxAmount = $(this).find("input[type=text]").val();
                                    if (CurrentBaseID != undefined) {
                                        BaseMaxAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseMonthMaxAmount_" + CurrentBaseID + "]").val();
                                    }
                                    else {
                                        BaseMaxAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseMonthMaxAmount_" + rangeIntervalID + "]").val();
                                    }
                                    maxcount = 1;
                                }
                                else if ($(this).find("input[type=text]").attr("name") == "MonthGapAmount") {
                                    GapAmount = $(this).find("input[type=text]").val();
                                    if (CurrentBaseID != undefined) {
                                        BaseGapAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseMonthGapAmount_" + CurrentBaseID + "]").val();
                                    }
                                    else {
                                        BaseGapAmount = $("#RuleSetGroupTemplate .RuleSetDefaultTemplate").eq(index).find("input[type=text][id=BaseMonthGapAmount_" + rangeIntervalID + "]").val();
                                    }
                                    gapcount = 1;
                                }

                                if (gapcount == 1 && maxcount == 1 && mincount == 1) {
                                    var ruleSetGapSettingMonth = new Object();
                                    ruleSetGapSettingMonth.MinAmount = MinAmount;
                                    ruleSetGapSettingMonth.MaxAmount = MaxAmount;
                                    ruleSetGapSettingMonth.GapAmount = GapAmount;
                                    ruleSetGapSettingMonth.BaseMinAmount = BaseMinAmount;
                                    ruleSetGapSettingMonth.BaseMaxAmount = BaseMaxAmount;
                                    ruleSetGapSettingMonth.BaseGapAmount = BaseGapAmount
                                    ruleSetGapSettingMonth.RangeIntervalID = rangeIntervalID;

                                    lstRuleSetGapSettingMonth.push(ruleSetGapSettingMonth);

                                    MinAmount = 0, MaxAmount = 0, GapAmount = 0, BaseMinAmount = 0, BaseMaxAmount = 0, BaseGapAmount = 0;
                                    mincount = 0, maxcount = 0, gapcount = 0, basemincount = 0, basemaxcount = 0, basegapcount = 0;
                                    rangeIntervalID = 0;
                                }
                            }
                        });

                        RuleSetGroupDTO.lstRuleSetGapSettingDay = lstRuleSetGapSettingDay;
                        RuleSetGroupDTO.lstRuleSetGapSettingWeek = lstRuleSetGapSettingWeek;
                        RuleSetGroupDTO.lstRuleSetGapSettingMonth = lstRuleSetGapSettingMonth;

                        ruleSetGroup.push(RuleSetGroupDTO);
                    }
                });
            }
            else {

                //Update Case

                $("#RuleSetGroupUpdateTemplate .RuleSetDefaultTemplate").each(function (index) {
                    var RuleSetGroupDTO = new Object();
                    //RuleSetGroupDTO.CompanyIDs = $("#RuleSetGroupUpdateTemplate .RuleSetDefaultTemplate").eq(index).find("select").val().toString();

                    var lstRuleSetGapSettingDay = new Array();
                    var lstRuleSetGapSettingWeek = new Array();
                    var lstRuleSetGapSettingMonth = new Array();
                    var AddlstRuleSetGapSettingDay = new Array();
                    var AddlstRuleSetGapSettingWeek = new Array();
                    var AddlstRuleSetGapSettingMonth = new Array();

                    var MinAmount = 0, MaxAmount = 0, GapAmount = 0, BaseMinAmount = 0, BaseMaxAmount = 0, BaseGapAmount = 0;// Used for check the loop count of template column(min,max,gap)
                    var mincount = 0, maxcount = 0, gapcount = 0, basemincount = 0, basemaxcount = 0, basegapcount = 0;
                    var rangeIntervalID = 0;
                    var IsChanged = false;
                    var RuleSetGapSettingID = 0;
                    //This case use for if user add new ruleset group in selected ruleset. that time get the data for insert operation
                    if ($(this).attr("newadd") != undefined) {
                        if ($(this).attr("newadd") == "yes") {
                            if ($("#RuleSetGroupUpdateTemplate .RuleSetDefaultTemplate").eq(index).find("select").find("option").length != 0) {
                                RuleSetGroupDTO.CompanyIDs = $("#RuleSetGroupUpdateTemplate .RuleSetDefaultTemplate").eq(index).find("select").val().toString();
                                $("#RuleSetGroupUpdateTemplate .RuleSetDefaultTemplate").eq(index).find("#GroupDayTemplate tr td").each(function () {

                                    if ($(this).find("input[type=text]") != null || $(this).find("input[type=text]") != undefined) {
                                        if ($(this).attr("RangeIntervalID") != undefined) {
                                            rangeIntervalID = $(this).attr("RangeIntervalID");
                                        }
                                        if ($(this).find("input[type=text]").attr("name") == "DayMinAmount") {
                                            MinAmount = $(this).find("input[type=text]").val();
                                            BaseMinAmount = $("#BaseDayMinAmount_" + RuleSetGapSettingID).val();
                                            mincount = 1;
                                        }
                                        else if ($(this).find("input[type=text]").attr("name") == "DayMaxAmount") {
                                            MaxAmount = $(this).find("input[type=text]").val();
                                            BaseMaxAmount = $("#BaseDayMaxAmount_" + RuleSetGapSettingID).val();
                                            maxcount = 1;
                                        }
                                        else if ($(this).find("input[type=text]").attr("name") == "DayGapAmount") {
                                            GapAmount = $(this).find("input[type=text]").val();
                                            BaseGapAmount = $("#BaseDayGapAmount_" + RuleSetGapSettingID).val();
                                            gapcount = 1;
                                        }

                                        //Base Textbox
                                        if ($(this).find("input[type=text]").attr("name") == "BaseDayMinAmount") {
                                            BaseMinAmount = $(this).find("input[type=text]").val();
                                            MinAmount = $("#DayMinAmount_" + RuleSetGapSettingID).val();
                                            basemincount = 1;
                                        }
                                        else if ($(this).find("input[type=text]").attr("name") == "BaseDayMaxAmount") {
                                            BaseMaxAmount = $(this).find("input[type=text]").val();
                                            MaxAmount = $("#DayMaxAmount_" + RuleSetGapSettingID).val();
                                            basemaxcount = 1;
                                        }
                                        else if ($(this).find("input[type=text]").attr("name") == "BaseDayGapAmount") {
                                            BaseGapAmount = $(this).find("input[type=text]").val();
                                            GapAmount = $("#DayGapAmount_" + RuleSetGapSettingID).val();
                                            basegapcount = 1;
                                        }

                                        if ((gapcount == 1 && maxcount == 1 && mincount == 1) || (basegapcount == 1 && basemaxcount == 1 && basemincount == 1)) {
                                            var ruleSetGapSettingDay = new Object();

                                            ruleSetGapSettingDay.MinAmount = MinAmount;
                                            ruleSetGapSettingDay.MaxAmount = MaxAmount;
                                            ruleSetGapSettingDay.GapAmount = GapAmount;
                                            ruleSetGapSettingDay.BaseMinAmount = BaseMinAmount;
                                            ruleSetGapSettingDay.BaseMaxAmount = BaseMaxAmount;
                                            ruleSetGapSettingDay.BaseGapAmount = BaseGapAmount
                                            ruleSetGapSettingDay.RangeIntervalID = rangeIntervalID;

                                            AddlstRuleSetGapSettingDay.push(ruleSetGapSettingDay);

                                            MinAmount = 0, MaxAmount = 0, GapAmount = 0, BaseMinAmount = 0, BaseMaxAmount = 0, BaseGapAmount = 0;
                                            mincount = 0, maxcount = 0, gapcount = 0, basemincount = 0, basemaxcount = 0, basegapcount = 0;
                                            rangeIntervalID = 0;
                                            IsChanged = false;
                                            RuleSetGapSettingID = 0;
                                        }
                                    }
                                });
                                //This is used for RuleSet Week
                                $("#RuleSetGroupUpdateTemplate .RuleSetDefaultTemplate").eq(index).find("#GroupWeekTemplate tr td").each(function () {
                                    if ($(this).find("input[type=text]") != null || $(this).find("input[type=text]") != undefined) {
                                        if ($(this).attr("RangeIntervalID") != undefined) {
                                            rangeIntervalID = $(this).attr("RangeIntervalID");
                                        }
                                        if ($(this).find("input[type=text]").attr("name") == "WeekMinAmount") {
                                            MinAmount = $(this).find("input[type=text]").val();
                                            mincount = 1;
                                        }
                                        else if ($(this).find("input[type=text]").attr("name") == "WeekMaxAmount") {
                                            MaxAmount = $(this).find("input[type=text]").val();
                                            maxcount = 1;
                                        }
                                        else if ($(this).find("input[type=text]").attr("name") == "WeekGapAmount") {
                                            GapAmount = $(this).find("input[type=text]").val();
                                            gapcount = 1;
                                        }

                                        //Base grid Operation
                                        if ($(this).find("input[type=text]").attr("name") == "BaseWeekMinAmount") {
                                            BaseMinAmount = $(this).find("input[type=text]").val();
                                            basemincount = 1;
                                        }
                                        else if ($(this).find("input[type=text]").attr("name") == "BaseWeekMaxAmount") {
                                            BaseMaxAmount = $(this).find("input[type=text]").val();
                                            basemaxcount = 1;
                                        }
                                        else if ($(this).find("input[type=text]").attr("name") == "BaseWeekGapAmount") {
                                            BaseGapAmount = $(this).find("input[type=text]").val();
                                            basegapcount = 1;
                                        }

                                        if ((gapcount == 1 && maxcount == 1 && mincount == 1) || (basegapcount == 1 && basemincount == 1 && basemaxcount == 1)) {
                                            var ruleSetGapSettingWeek = new Object();

                                            ruleSetGapSettingWeek.MinAmount = MinAmount;
                                            ruleSetGapSettingWeek.MaxAmount = MaxAmount;
                                            ruleSetGapSettingWeek.GapAmount = GapAmount;
                                            ruleSetGapSettingWeek.BaseMinAmount = BaseMinAmount;
                                            ruleSetGapSettingWeek.BaseMaxAmount = BaseMaxAmount;
                                            ruleSetGapSettingWeek.BaseGapAmount = BaseGapAmount
                                            ruleSetGapSettingWeek.RangeIntervalID = rangeIntervalID;

                                            AddlstRuleSetGapSettingWeek.push(ruleSetGapSettingWeek);

                                            MinAmount = 0, MaxAmount = 0, GapAmount = 0, BaseMinAmount = 0, BaseMaxAmount = 0, BaseGapAmount = 0;
                                            mincount = 0, maxcount = 0, gapcount = 0, basemincount = 0, basemaxcount = 0, basegapcount = 0;
                                            rangeIntervalID = 0;
                                            IsChanged = false;
                                            RuleSetGapSettingID = 0;
                                        }
                                    }
                                });

                                //This is Used for RuleSet Month
                                $("#RuleSetGroupUpdateTemplate .RuleSetDefaultTemplate").eq(index).find("#GroupMonthTemplate tr td").each(function () {
                                    if ($(this).find("input[type=text]") != null || $(this).find("input[type=text]") != undefined) {
                                        if ($(this).attr("RangeIntervalID") != undefined) {
                                            rangeIntervalID = $(this).attr("RangeIntervalID");
                                        }
                                        if ($(this).find("input[type=text]").attr("name") == "MonthMinAmount") {
                                            MinAmount = $(this).find("input[type=text]").val();
                                            mincount = 1;
                                        }
                                        else if ($(this).find("input[type=text]").attr("name") == "MonthMaxAmount") {
                                            MaxAmount = $(this).find("input[type=text]").val();
                                            maxcount = 1;
                                        }
                                        else if ($(this).find("input[type=text]").attr("name") == "MonthGapAmount") {
                                            GapAmount = $(this).find("input[type=text]").val();
                                            gapcount = 1;
                                        }

                                        //Base Calculation Operation
                                        if ($(this).find("input[type=text]").attr("name") == "BaseMonthMinAmount") {
                                            BaseMinAmount = $(this).find("input[type=text]").val();
                                            basemincount = 1;
                                        }
                                        else if ($(this).find("input[type=text]").attr("name") == "BaseMonthMaxAmount") {
                                            BaseMaxAmount = $(this).find("input[type=text]").val();
                                            basemaxcount = 1;
                                        }
                                        else if ($(this).find("input[type=text]").attr("name") == "BaseMonthGapAmount") {
                                            BaseGapAmount = $(this).find("input[type=text]").val();
                                            basegapcount = 1;
                                        }

                                        if ((gapcount == 1 && maxcount == 1 && mincount == 1) || (basegapcount == 1 && basemaxcount == 1 && basemincount == 1)) {
                                            var ruleSetGapSettingMonth = new Object();

                                            ruleSetGapSettingMonth.MinAmount = MinAmount;
                                            ruleSetGapSettingMonth.MaxAmount = MaxAmount;
                                            ruleSetGapSettingMonth.GapAmount = GapAmount;
                                            ruleSetGapSettingMonth.BaseMinAmount = BaseMinAmount;
                                            ruleSetGapSettingMonth.BaseMaxAmount = BaseMaxAmount;
                                            ruleSetGapSettingMonth.BaseGapAmount = BaseGapAmount
                                            ruleSetGapSettingMonth.RangeIntervalID = rangeIntervalID;

                                            AddlstRuleSetGapSettingMonth.push(ruleSetGapSettingMonth);

                                            MinAmount = 0, MaxAmount = 0, GapAmount = 0, BaseMinAmount = 0, BaseMaxAmount = 0, BaseGapAmount = 0;
                                            mincount = 0, maxcount = 0, gapcount = 0, basemincount = 0, basemaxcount = 0, basegapcount = 0;
                                            rangeIntervalID = 0;
                                            IsChanged = false;
                                            RuleSetGapSettingID = 0;
                                        }
                                    }
                                });

                                RuleSetGroupDTO.AddlstRuleSetGapSettingDay = AddlstRuleSetGapSettingDay;
                                RuleSetGroupDTO.AddlstRuleSetGapSettingWeek = AddlstRuleSetGapSettingWeek;
                                RuleSetGroupDTO.AddlstRuleSetGapSettingMonth = AddlstRuleSetGapSettingMonth;
                            }
                        }
                    }
                    else {
                        //This one case use for only existing update group template
                        //RuleSetGroupID
                        var RuleSetGroupID = $("#RuleSetGroupUpdateTemplate .RuleSetDefaultTemplate").eq(index).find("select").attr("rulesetgroupid");
                        RuleSetGroupDTO.RuleSetGroupID = RuleSetGroupID;
                        if ($("#RuleSetGroupUpdateTemplate .RuleSetDefaultTemplate").eq(index).find("select").find("option").length == 0) {
                            RuleSetGroupDTO.DeleteRuleSetGroupID = RuleSetGroupID;
                        }

                        if ($("#RuleSetGroupUpdateTemplate .RuleSetDefaultTemplate").eq(index).find("select").find("option").length != 0) {

                            var DeleteCompanyIDs = [], AddCompanyIDs = [];

                            var CompanyID = $("#RuleSetGroupUpdateTemplate .RuleSetDefaultTemplate").eq(index).find("select").attr("companyids");
                            var selectedCompanyID = $("#RuleSetGroupUpdateTemplate .RuleSetDefaultTemplate").eq(index).find("select").val();
                            if (CompanyID != undefined) {
                                DeleteCompanyIDs = CompanyID.split(',').filter(function (obj) { return selectedCompanyID.indexOf(obj) == -1; });
                                for (var i = 0; i < selectedCompanyID.length ; i++) {
                                    if ($.inArray(selectedCompanyID[i], CompanyID.split(',')) == -1) {
                                        AddCompanyIDs.push(selectedCompanyID[i]);
                                    }
                                }
                            }
                            RuleSetGroupDTO.DeleteCompanyIDs = DeleteCompanyIDs.toString();
                            RuleSetGroupDTO.AddCompanyIDs = AddCompanyIDs.toString();

                            //This is used for Ruleset Day 
                            $("#RuleSetGroupUpdateTemplate .RuleSetDefaultTemplate").eq(index).find("#GroupDayTemplate tr td").each(function () {
                                //console.log($(this).parent().siblings($(this)).attr("RuleSetGapSettingID"));
                                if ($(this).find("input[type=text]") != null || $(this).find("input[type=text]") != undefined) {
                                    if ($(this).attr("RangeIntervalID") != undefined) {
                                        rangeIntervalID = $(this).attr("RangeIntervalID");
                                        IsChanged = JSON.parse($(this).attr("IsChanged"));
                                        RuleSetGapSettingID = $(this).attr("RuleSetGapSettingID");
                                    }
                                    if ($(this).find("input[type=text]").attr("name") == "DayMinAmount") {
                                        MinAmount = $(this).find("input[type=text]").val();
                                        BaseMinAmount = $("#BaseDayMinAmount_" + RuleSetGapSettingID).val();
                                        mincount = 1;
                                    }
                                    else if ($(this).find("input[type=text]").attr("name") == "DayMaxAmount") {
                                        MaxAmount = $(this).find("input[type=text]").val();
                                        BaseMaxAmount = $("#BaseDayMaxAmount_" + RuleSetGapSettingID).val();
                                        maxcount = 1;
                                    }
                                    else if ($(this).find("input[type=text]").attr("name") == "DayGapAmount") {
                                        GapAmount = $(this).find("input[type=text]").val();
                                        BaseGapAmount = $("#BaseDayGapAmount_" + RuleSetGapSettingID).val();
                                        gapcount = 1;
                                    }


                                    //Base Calculation Operation
                                    if ($(this).find("input[type=text]").attr("name") == "BaseDayMinAmount") {
                                        BaseMinAmount = $(this).find("input[type=text]").val();
                                        MinAmount = $("#DayMinAmount_" + RuleSetGapSettingID).val();
                                        basemincount = 1;
                                    }
                                    else if ($(this).find("input[type=text]").attr("name") == "BaseDayMaxAmount") {
                                        BaseMaxAmount = $(this).find("input[type=text]").val();
                                        MaxAmount = $("#DayMaxAmount_" + RuleSetGapSettingID).val();
                                        basemaxcount = 1;
                                    }
                                    else if ($(this).find("input[type=text]").attr("name") == "BaseDayGapAmount") {
                                        BaseGapAmount = $(this).find("input[type=text]").val();
                                        GapAmount = $("#DayGapAmount_" + RuleSetGapSettingID).val();
                                        basegapcount = 1;
                                    }


                                    if ((gapcount == 1 && maxcount == 1 && mincount == 1) || (basegapcount == 1 && basemaxcount == 1 && basemincount == 1)) {
                                        var ruleSetGapSettingDay = new Object();

                                        ruleSetGapSettingDay.MinAmount = MinAmount;
                                        ruleSetGapSettingDay.MaxAmount = MaxAmount;
                                        ruleSetGapSettingDay.GapAmount = GapAmount;
                                        ruleSetGapSettingDay.BaseMinAmount = BaseMinAmount;
                                        ruleSetGapSettingDay.BaseMaxAmount = BaseMaxAmount;
                                        ruleSetGapSettingDay.BaseGapAmount = BaseGapAmount
                                        ruleSetGapSettingDay.RangeIntervalID = rangeIntervalID;
                                        ruleSetGapSettingDay.ID = RuleSetGapSettingID;
                                        //This check is used for only textbox is changed
                                        if (IsChanged) {
                                            lstRuleSetGapSettingDay.push(ruleSetGapSettingDay);
                                        }
                                        MinAmount = 0, MaxAmount = 0, GapAmount = 0, BaseMinAmount = 0, BaseMaxAmount = 0, BaseGapAmount = 0;
                                        mincount = 0, maxcount = 0, gapcount = 0, basemincount = 0, basemaxcount = 0, basegapcount = 0;
                                        rangeIntervalID = 0;
                                        IsChanged = false;
                                        RuleSetGapSettingID = 0;
                                    }
                                }
                            });
                            //This is used for RuleSet Week
                            $("#RuleSetGroupUpdateTemplate .RuleSetDefaultTemplate").eq(index).find("#GroupWeekTemplate tr td").each(function () {
                                if ($(this).find("input[type=text]") != null || $(this).find("input[type=text]") != undefined) {
                                    if ($(this).attr("RangeIntervalID") != undefined) {
                                        rangeIntervalID = $(this).attr("RangeIntervalID");
                                        IsChanged = JSON.parse($(this).attr("IsChanged"));
                                        RuleSetGapSettingID = $(this).attr("RuleSetGapSettingID");
                                    }
                                    if ($(this).find("input[type=text]").attr("name") == "WeekMinAmount") {
                                        MinAmount = $(this).find("input[type=text]").val();
                                        BaseMinAmount = $("#BaseWeekMinAmount_" + RuleSetGapSettingID).val();
                                        mincount = 1;
                                    }
                                    else if ($(this).find("input[type=text]").attr("name") == "WeekMaxAmount") {
                                        MaxAmount = $(this).find("input[type=text]").val();
                                        BaseMaxAmount = $("#BaseWeekMaxAmount_" + RuleSetGapSettingID).val();
                                        maxcount = 1;
                                    }
                                    else if ($(this).find("input[type=text]").attr("name") == "WeekGapAmount") {
                                        GapAmount = $(this).find("input[type=text]").val();
                                        BaseGapAmount = $("#BaseWeekGapAmount_" + RuleSetGapSettingID).val();
                                        gapcount = 1;
                                    }


                                    //Base grid Operation
                                    if ($(this).find("input[type=text]").attr("name") == "BaseWeekMinAmount") {
                                        MinAmount = $("#WeekMinAmount_" + RuleSetGapSettingID).val();
                                        BaseMinAmount = $(this).find("input[type=text]").val();
                                        basemincount = 1;
                                    }
                                    else if ($(this).find("input[type=text]").attr("name") == "BaseWeekMaxAmount") {
                                        MaxAmount = $("#WeekMaxAmount_" + RuleSetGapSettingID).val();
                                        BaseMaxAmount = $(this).find("input[type=text]").val();
                                        basemaxcount = 1;
                                    }
                                    else if ($(this).find("input[type=text]").attr("name") == "BaseWeekGapAmount") {
                                        GapAmount = $("#WeekGapAmount_" + RuleSetGapSettingID).val();
                                        BaseGapAmount = $(this).find("input[type=text]").val();
                                        basegapcount = 1;
                                    }

                                    if ((gapcount == 1 && maxcount == 1 && mincount == 1) || (basegapcount == 1 && basemincount == 1 && basemaxcount == 1)) {
                                        var ruleSetGapSettingWeek = new Object();

                                        ruleSetGapSettingWeek.MinAmount = MinAmount;
                                        ruleSetGapSettingWeek.MaxAmount = MaxAmount;
                                        ruleSetGapSettingWeek.GapAmount = GapAmount;
                                        ruleSetGapSettingWeek.BaseMinAmount = BaseMinAmount;
                                        ruleSetGapSettingWeek.BaseMaxAmount = BaseMaxAmount;
                                        ruleSetGapSettingWeek.BaseGapAmount = BaseGapAmount
                                        ruleSetGapSettingWeek.RangeIntervalID = rangeIntervalID;
                                        ruleSetGapSettingWeek.ID = RuleSetGapSettingID;
                                        //This check is used for only textbox is changed
                                        if (IsChanged) {
                                            lstRuleSetGapSettingWeek.push(ruleSetGapSettingWeek);
                                        }

                                        MinAmount = 0, MaxAmount = 0, GapAmount = 0, BaseMinAmount = 0, BaseMaxAmount = 0, BaseGapAmount = 0;
                                        mincount = 0, maxcount = 0, gapcount = 0, basemincount = 0, basemaxcount = 0, basegapcount = 0;
                                        rangeIntervalID = 0;
                                        IsChanged = false;
                                        RuleSetGapSettingID = 0;
                                    }
                                }
                            });

                            //This is Used for RuleSet Month
                            $("#RuleSetGroupUpdateTemplate .RuleSetDefaultTemplate").eq(index).find("#GroupMonthTemplate tr td").each(function () {
                                if ($(this).find("input[type=text]") != null || $(this).find("input[type=text]") != undefined) {
                                    if ($(this).attr("RangeIntervalID") != undefined) {
                                        rangeIntervalID = $(this).attr("RangeIntervalID");
                                        IsChanged = JSON.parse($(this).attr("IsChanged"));
                                        RuleSetGapSettingID = $(this).attr("RuleSetGapSettingID");
                                    }
                                    if ($(this).find("input[type=text]").attr("name") == "MonthMinAmount") {
                                        MinAmount = $(this).find("input[type=text]").val();
                                        BaseMinAmount = $("#BaseMonthMinAmount_" + RuleSetGapSettingID).val();
                                        mincount = 1;
                                    }
                                    else if ($(this).find("input[type=text]").attr("name") == "MonthMaxAmount") {
                                        MaxAmount = $(this).find("input[type=text]").val();
                                        BaseMaxAmount = $("#BaseMonthMaxAmount_" + RuleSetGapSettingID).val();
                                        maxcount = 1;
                                    }
                                    else if ($(this).find("input[type=text]").attr("name") == "MonthGapAmount") {
                                        GapAmount = $(this).find("input[type=text]").val();
                                        BaseGapAmount = $("#BaseMonthGapAmount_" + RuleSetGapSettingID).val();
                                        gapcount = 1;
                                    }

                                    //Base Calculation Operation
                                    if ($(this).find("input[type=text]").attr("name") == "BaseMonthMinAmount") {
                                        BaseMinAmount = $(this).find("input[type=text]").val();
                                        MinAmount = $("#MonthMinAmount_" + RuleSetGapSettingID).val();
                                        basemincount = 1;
                                    }
                                    else if ($(this).find("input[type=text]").attr("name") == "BaseMonthMaxAmount") {
                                        BaseMaxAmount = $(this).find("input[type=text]").val();
                                        MaxAmount = $("#MonthMaxAmount_" + RuleSetGapSettingID).val();
                                        basemaxcount = 1;
                                    }
                                    else if ($(this).find("input[type=text]").attr("name") == "BaseMonthGapAmount") {
                                        BaseGapAmount = $(this).find("input[type=text]").val();
                                        GapAmount = $("#MonthGapAmount_" + RuleSetGapSettingID).val();
                                        basegapcount = 1;
                                    }

                                    if ((gapcount == 1 && maxcount == 1 && mincount == 1) || (basegapcount == 1 && basemaxcount == 1 && basemincount == 1)) {
                                        var ruleSetGapSettingMonth = new Object();

                                        ruleSetGapSettingMonth.MinAmount = MinAmount;
                                        ruleSetGapSettingMonth.MaxAmount = MaxAmount;
                                        ruleSetGapSettingMonth.GapAmount = GapAmount;
                                        ruleSetGapSettingMonth.BaseMinAmount = BaseMinAmount;
                                        ruleSetGapSettingMonth.BaseMaxAmount = BaseMaxAmount;
                                        ruleSetGapSettingMonth.BaseGapAmount = BaseGapAmount
                                        ruleSetGapSettingMonth.RangeIntervalID = rangeIntervalID;
                                        ruleSetGapSettingMonth.ID = RuleSetGapSettingID;

                                        //This check is used for only textbox is changed
                                        if (IsChanged) {
                                            //console.log(ruleSetGapSettingMonth);
                                            lstRuleSetGapSettingMonth.push(ruleSetGapSettingMonth);
                                        }

                                        MinAmount = 0, MaxAmount = 0, GapAmount = 0, BaseMinAmount = 0, BaseMaxAmount = 0, BaseGapAmount = 0;
                                        mincount = 0, maxcount = 0, gapcount = 0, basemincount = 0, basemaxcount = 0, basegapcount = 0;
                                        rangeIntervalID = 0;
                                        IsChanged = false;
                                        RuleSetGapSettingID = 0;
                                    }
                                }
                            });

                            RuleSetGroupDTO.lstRuleSetGapSettingDay = lstRuleSetGapSettingDay;
                            RuleSetGroupDTO.lstRuleSetGapSettingWeek = lstRuleSetGapSettingWeek;
                            RuleSetGroupDTO.lstRuleSetGapSettingMonth = lstRuleSetGapSettingMonth;
                        }
                    }
                    ruleSetGroup.push(RuleSetGroupDTO);
                });
            }
            //console.log(RuleSetTemplateDTO);
            //console.log(ruleSetGroup);


            if (ruleSetGroup.length != 0) {
                $(".loader_container_main").show();
                var ajaxURl = '/RateShopper/RuleSet/CreateUpdateRuleSet';
                if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
                    ajaxURl = AjaxURLSettings.CreateUpdateRuleSetUrl;
                }
                $.ajax({
                    url: ajaxURl,
                    type: 'POST',
                    async: true,
                    data: JSON.stringify({ 'lstRuleSetGroupDTO': ruleSetGroup, 'RuleSetTemplate': RuleSetTemplateDTO }),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (data) {
                        //this check for is ruleset comes from automation or not
                        if (!IsAutomationRuleSet) {
                            $(window).scrollTop(0);
                            $('#spanSave').show();
                            BindRuleSets();
                            setTimeout(function () {
                                if (RuleSetID != 0) {
                                    $("#RuleSetList").scrollTop(0);
                                }
                                else {
                                    $("#RuleSetList").scrollTop($("#RuleSetList")[0].scrollHeight);
                                }

                                //Reset All Control
                                $("#mtab1 select option").each(function () {
                                    $(this).prop("selected", false);
                                })
                            }, 2500);
                            //ruleSetSaveFlag = true;

                            setTimeout(function () {
                                $("#CreateRuleset").click();
                                $(".loader_container_main").hide();
                                $('#spanSave').hide();
                            }, 3000);
                        }
                        else {
                            $(".loader_container_main").hide();
                            //Call this method for AutomationConsoleRuleSet.
                            SetSelectedRulesetAttribute(data);
                        }
                    },
                    error: function (e) {
                        $(".loader_container_main").hide();
                        console.log(e.message);
                    }
                });
            }
        }
    });

    $("#RuleSetTemplateCompanies").on("click", function () {
        //console.log($(this).parent().paren())
    });

    $("#Delete").on("click", function () {
        if (RuleSetID != 0) {
            ShowConfirmBox('Do you want to delete the <b>' + $('#RuleSetList li.rsselected').text().trim() + '</b> Rule Set?', true, DeleteRuleSetCallBack, RuleSetID);
        }
        else {
            ShowConfirmBox('Please select rule set to delete.', false);
        }
    });

    $("#CreateRuleset").on("click", function () {
        //setTimeout(function () {
        //    //$('#spanSave').hide();            
        //}, 2000);
        if (!resetLocation) {
            resetLeftBranLocation();
        }
        RuleSetID = 0;
        RemoveFlasableEffect();
        //if (ruleSetSaveFlag)
        //{
        //    $('#spanSave').delay(2000).hide();
        //    //setTimeout(function () { $('#spanSave').hide(); }, 5000);
        //    ruleSetSaveFlag = false;
        //}
        $("#HaddingTitle").html("CREATE RULE SET");
        $("#RuleSetGroupTemplate").html("");

        $("#RuleSetGroupTemplate").removeClass("hidden");
        $("#RuleSetGroupTemplate").removeAttr("style");
        $("#RuleSetGroupUpdateTemplate").addClass("hidden");

        $("#RuleSetList li").removeClass("rsselected");
        $("#rightRuleSetCompanies").prop("disabled", true);
        $("#rightRuleSetCompanies").attr("selectedcompanyids", "");
        $("#rightPanelControl select option").each(function () {
            $(this).prop("selected", false);
        });

        $("#rightRuleSetName").val("");
        $("#AddRuleSetTemplate").click();
        $("#RuleSetGroupTemplate #RuleSetTemplateCompanies").html("");
        $("#rightRuleSetlocations ul li").eq(0).click();
        $("#rightRuleSetlocations ul li").eq(0).click();
        $("#PositionCompetitor").html("Position of EZ-RAC ");
        $("#rightPanelControl select").scrollTop(0);
        $("#searchLeftPanel select").scrollTop(0);

        $("#rightRuleSetIsWideGapTemplate").prop("checked", false);
        $("#rg1").prop("checked", true)[0];
        $("#rightRuleSetIsPositionOffset").val("1");
        $("#rightRuleSetCompanyPositionAbvAvg").val("");
        $("#MainRuleSetPage input[type=radio]").prop("disabled", true);
        $("#rightRuleSetIsPositionOffset").prop("disabled", true);
        $("#rightRuleSetCompanyPositionAbvAvg").prop("disabled", true);
        $("#isGOV").prop("checked", false);
        $(window).scrollTop(0);
        //$("#RuleSetList").scrollTop(0);

        //$("#RuleSetList li").show();
        //$("#rg1:checked").prop("checked", false);
        //$("#rightRuleSetIsPositionOffset").val("");
        //$("#rightRuleSetCompanyPositionAbvAvg").val("");

        //var srcs = $.map(ruleSetSmartSearchLocations, function (item) { return new brandLocations(item); });
        //ruleSetViewModel.leftBrandLocations(srcs);
    });
    $("#rg1").prop("checked", true)[0];
    $("#rightRuleSetIsPositionOffset").val("1");
    $("#MainRuleSetPage input[type=radio]").prop("disabled", true);
    $("#rightRuleSetIsPositionOffset").prop("disabled", true);
    $("#rightRuleSetCompanyPositionAbvAvg").prop("disabled", true);

    $("#rightRuleSetIsWideGapTemplate").on("change", function () {
        if ($(this).prop("checked")) {
            if ($("#isGOV").prop("checked")) {
                $("#isGOV").prop("checked", false);
            }
            $("input[type=radio]").prop("disabled", false);
            $("#rightRuleSetIsPositionOffset").prop("disabled", false);
            $("#rightRuleSetCompanyPositionAbvAvg").prop("disabled", false);
            $("#rg1").eq(0).prop("checked", true);
            $("#rightRuleSetIsPositionOffset").val(1);
            $("#rightRuleSetCompanyPositionAbvAvg").val('');
        }
        else {
            $("#MainRuleSetPage input[type=radio]").prop("disabled", true);
            $("#rightRuleSetIsPositionOffset").prop("disabled", true);
            $("#rightRuleSetCompanyPositionAbvAvg").prop("disabled", true);
            $("#rightRuleSetIsPositionOffset").val("1");
            $("#rg1").eq(0).prop("checked", true);
            $("#rightRuleSetCompanyPositionAbvAvg").val('');
        }
    })

    $("#CopyAndCreateRuleSet").on("click", function () {
        if (RuleSetID != 0) {
            IsCopyAndCreate = true;
            $("#selectedRuleSetID_" + RuleSetID).click();
        }
        else {
            ShowConfirmBox('Please select rule set to copy and create.', false);
        }
    });
    // $(".loader_container_main").hide();
});

function DeleteRuleSetCallBack() {
    RuleSetID = this;
    $('.loader_container_main').show();
    var ajaxURl = '/RateShopper/RuleSet/DeleteSelectedRuleSetData';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.DeleteSelectedRuleSetDataUrl;
    }
    $.ajax({
        url: ajaxURl,
        type: 'POST',
        async: true,
        data: { RuleSetID: RuleSetID, LoggedInUserId: $('#LoggedInUserId').val() },
        success: function (data) {
            if (data) {
                $('.loader_container_main').hide();
                BindRuleSets();
                $("#RuleSetList").scrollTop(0);
                setTimeout(function () {
                    $("#CreateRuleset").click();
                }, 1000);
            }
        },
        error: function (e) {
            $('.loader_container_main').hide();
            console.log(e.message);
        }
    });
}

//ViewModel for RuleSet
function RuleSetModel() {
    var self = this;
    self.rentalLengths = ko.observableArray([]);
    self.AutomationRentalLengths = ko.observableArray([]);
    self.leftCarClasses = ko.observableArray([]);
    self.rightCarClasses = ko.observableArray([]);
    self.masterCompanies = ko.observableArray([]);
    self.ruleSetGroupCompanies = ko.observableArray([]);
    self.weekDays = ko.observableArray([]);
    self.leftBrandLocations = ko.observableArray([]);
    self.leftAutomationBrandLocations = ko.observableArray([]);
    self.rightBrandLocations = ko.observableArray([]);
    self.ruleSets = ko.observableArray([]);
    self.automationRuleSets = ko.observableArray([]);
    self.RuleSetGapSettingsDay = ko.observableArray([]);
    self.RuleSetGapSettingsWeek = ko.observableArray([]);
    self.RuleSetGapSettingsMonth = ko.observableArray([]);
    self.SeletedRuleSetGapSettings = ko.observableArray([]);
}
//End ViewModel

//Entity
function rentalLengths(data) {
    this.MappedID = data.MappedID;
    this.Code = data.Code;
    this.ID = data.ID;
}
function carClasses(data) {
    this.ID = data.ID;
    this.Code = data.Code;
}
function companies(data) {
    this.ID = data.ID;
    this.Code = data.Code;
    this.Name = data.Name;
    this.Logo = data.logo;
    this.IsBrand = ko.computed(function () {
        if (JSON.parse(data.IsBrand)) {
            return "true";
        }
        else {
            return "false";
        }
    });
    this.CompanySelection = "false";
}
function weekDays(data) {
    this.ID = data.ID;
    this.Day = data.Day;
}
function brandLocations(data) {
    this.LocationID = data.LocationID;
    this.Location = data.LocationBrandAlias;
    this.LocationBrandID = data.ID;
    this.BrandID = data.BrandID;
    this.LocationCode = data.LocationCode;
}
function ruleSets(data) {
    this.ScheduledJobRuleSetID = data.ScheduledJobRuleSetID;
    this.RuleSetID = data.ruleSetID;
    this.OriginalRuleSetID = data.ruleSetID;
    this.LocationBrandID = data.locationBrandID;
    this.RuleSetName = data.ruleSetName;
    this.CompanyPositionAbvAvg = data.companyPositionAbvAvg;
    this.IsPositionOffset = ko.computed(function () {
        if (JSON.parse(data.isPositionOffset))
            return "true";
        else
            return "false";
    });
    this.IsWideGapTemple = ko.computed(function () {
        if (JSON.parse(data.isWideGapTemple))
            return "true";
        else
            return "false";
    });
    this.IsGov = ko.computed(function () {
        if (JSON.parse(data.isGov))
            return "true";
        else
            return "false";
    });
    this.CarClassID = data.carClassID;
    this.RentalLengthID = data.rentalLengthID;
    this.WeekDaysID = data.weekDaysID;
    this.AppliedStartDate = data.AppliedStartDate;
    this.AppliedEndDate = data.AppliedEndDate;
    this.IsAutoRuleSetInsertUpdate = ko.computed(function () {
        if (data.ScheduledJobRuleSetID == 0) {
            return "insert";
        }
        else {
            return "update";
        }
    }); //This Entity use to identify wether automation ruleset is insert mode or update mode.
}
function ruleSetDefaultSetting(data) {
    this.ID = data.ID;
    this.RuleSetGroupID = data.RuleSetGroupID;
    this.RangeIntervalID = data.RangeIntervalID;
    this.RangeName = ko.computed(function () {
        if (data.RangeName.substring(0, 1) == 'D') {
            return "Day Range";
        } else if (data.RangeName.substring(0, 1) == 'W') {
            return "Week Range";
        }
        else if (data.RangeName.substring(0, 1) == 'M') {
            return "Month Range";
        }
    }); //data.RangeName;
    this.MinAmount = parseFloat(data.MinAmount).toFixed(2);
    this.MaxAmount = parseFloat(data.MaxAmount).toFixed(2);
    this.GapAmount = parseFloat(data.GapAmount).toFixed(2);
    this.BaseMinAmount = parseFloat(data.BaseMinAmount).toFixed(2);
    this.BaseMaxAmount = parseFloat(data.BaseMaxAmount).toFixed(2);
    this.BaseGapAmount = parseFloat(data.BaseGapAmount).toFixed(2);
    this.IsChanged = ko.computed(function () {
        return "false";
    });
}
function ruleSetGroupCompany(data) {
    this.CompanyID = data.CompanyID;
    this.CompanyName = data.CompanyName;
    this.IsBrand = ko.computed(function () {
        if (JSON.parse(data.IsBrand)) {
            return "true";
        }
        else {
            return "false";
        }
    });
}
function RuleSetGroupCustomDTO(data) {
    this.RuleSetGroupID = data.ID;
    this.GroupName = data.GroupName;
    this.CompanyName = data.CompanyName;
    this.SelectedCompanyIDs = data.selectedCompanyIDs;

    this.CompanyIDs = ko.computed(function () {
        if (data.lstGroupCompany != null && data.lstGroupCompany.length != 0) {
            var tempCompanyID = "";
            for (var i = 0; i < data.lstGroupCompany.length; i++) {
                tempCompanyID += data.lstGroupCompany[i].CompanyID + ",";
            }
            return tempCompanyID.trim().substring(0, tempCompanyID.trim().length - 1);
        }
    });
    this.lstGroupCompany = $.map(data.lstGroupCompany, function (item) { return new ruleSetGroupCompany(item); });
    this.lstRuleSetGapSettingDay = $.map(data.lstRuleSetGapSettingDay, function (item) { return new ruleSetDefaultSetting(item); });
    this.lstRuleSetGapSettingWeek = $.map(data.lstRuleSetGapSettingWeek, function (item) { return new ruleSetDefaultSetting(item); });
    this.lstRuleSetGapSettingMonth = $.map(data.lstRuleSetGapSettingMonth, function (item) { return new ruleSetDefaultSetting(item); });
}
//End Entity


//All Functions
//selecetd ruleset
function selectedRuleSet(data) {
    //console.log(data);
    var InternalRuleSetID = 0;//For using this function 
    //Check selected item come from automation console  or not
    if (IsAutomationRuleSet) {
        var scheduleJobRuleSetID = $("#selectedRuleSetID_" + data.RuleSetID).attr("scheduledjobrulesetid");
        var IsAutoRulesetInsertUpdate = $("#selectedRuleSetID_" + data.RuleSetID).attr("isautorulesetinsertupdate");
        $("#rightRuleSetlocationslbl").show();
        $("#rightRuleSetlocationslbl").val($("#view1 #locations select option:selected[value=" + $("#view1 #locations select").val() + "]").text());
        if (IsAutoRulesetInsertUpdate == "insert") {
            InternalRuleSetID = data.RuleSetID;
            IsCopyAndCreate = true; //Reuse copy and create funtionality
            //RuleSetID = 0;
        }
        else if (scheduleJobRuleSetID != 0 && IsAutoRulesetInsertUpdate == "update") {
            InternalRuleSetID = data.RuleSetID;
            RuleSetID = data.RuleSetID;
        }
    }
    else {
        InternalRuleSetID = data.RuleSetID;
        RuleSetID = data.RuleSetID;
    }

    RemoveFlasableEffect();
    $("#HaddingTitle").html("UPDATE / VIEW RULE SET");
    $("#RuleSetGroupUpdateTemplate").removeClass("hidden");
    $("#RuleSetGroupUpdateTemplate").removeAttr("style");
    $("#rightRuleSetCompanies").prop("disabled", false);
    $("#rightRuleSetName").val(data.RuleSetName);
    //$('#LoggedInUserId').val();    
    $("#rightRuleSetlengths option").each(function () {

        if ($.inArray($(this).val(), (data.RentalLengthID).split(',')) == -1) {
            $(this).prop("selected", false);
        }
        else {
            $(this).prop("selected", true);
        }
    });
    $("#rightRuleSetCarClass option").each(function () {
        if ($.inArray($(this).val(), (data.CarClassID).split(',')) == -1) {
            $(this).prop("selected", false);
        }
        else {
            $(this).prop("selected", true);
        }
    });
    $("#rightRuleSetDays option").each(function () {
        if ($.inArray($(this).val(), (data.WeekDaysID).split(',')) == -1) {
            $(this).prop("selected", false);
        }
        else {
            $(this).prop("selected", true);
        }
    });
    $("#rightRuleSetlocations ul li").each(function () {
        if ($(this) != undefined) {
            if ($(this).attr("locationbrandid") == data.LocationBrandID) {
                $(this).click();
                $(this).click();
                return false;
            }
        }
    });

    //$("#rightRuleSetCompanies").val().toString();       
    $("#rightRuleSetIsWideGapTemplate").prop("checked", boolTemplate(data.IsWideGapTemple));

    $('#isGOV').prop("checked", boolTemplate(data.IsGov));
    $("#rightRuleSetIsPositionOffset").val("");
    $("#rightRuleSetCompanyPositionAbvAvg").val("");

    if (JSON.parse($("#selectedRuleSetID_" + InternalRuleSetID).attr("iswidegaptemple")) || JSON.parse($("#selectedRuleSetID_" + InternalRuleSetID).attr("IsGov"))) {
        $("#MainRuleSetPage input[type=radio]").prop("disabled", false);
        $("#rightRuleSetIsPositionOffset").prop("disabled", false);
        $("#rightRuleSetCompanyPositionAbvAvg").prop("disabled", false);
    }
    else {
        $("#MainRuleSetPage input[type=radio]").prop("disabled", true);
        $("#rightRuleSetIsPositionOffset").prop("disabled", true);
        $("#rightRuleSetCompanyPositionAbvAvg").prop("disabled", true);
    }

    if (boolTemplate(data.IsPositionOffset)) {
        $("#rightPanelControl input[type=radio]").eq(0).prop("checked", true);
        $("#rightRuleSetIsPositionOffset").val(data.CompanyPositionAbvAvg);
    }
    else {
        $("#rightPanelControl input[type=radio]").eq(1).prop("checked", true);
        $("#rightRuleSetCompanyPositionAbvAvg").val(data.CompanyPositionAbvAvg);
    }

    //If user add new ruleset in previous selected ruleset and user dont save it. after that user can select another ruleset that time remove existing created Ruleset
    $("#RuleSetGroupUpdateTemplate #RuleSetDefaultTemplate").each(function () {
        if ($(this).attr("newadd") != undefined) {
            if ($(this).attr("newadd") == "yes") {
                $(this).remove();
            }
        }
    });
    $(".loader_container_main").show();
    var ajaxURl = '/RateShopper/RuleSet/GetSelectedRuleSetData';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetSelectedRuleSetDataUrl;
    }
    $.ajax({
        url: ajaxURl,
        type: 'POST',
        async: true,
        data: { RuleSetID: InternalRuleSetID },
        //contentType: 'application/json; charset=utf-8',
        //dataType: 'json',
        success: function (data) {

            $(".loader_container_main").hide();
            if (data) {
                var rsrcs = $.map(data.lstRuleSetGroupCustomDTO, function (item) { return new RuleSetGroupCustomDTO(item); });
                //console.log(rsrcs);
                ruleSetViewModel.SeletedRuleSetGapSettings(rsrcs);
                if (data.RuleSetCompanyIDs != null && data.RuleSetCompanyIDs != "") {
                    $("#rightRuleSetCompanies option").each(function () {
                        if ($.inArray($(this).val(), (data.RuleSetCompanyIDs).split(',')) == -1) {
                            $(this).prop("selected", false);
                        }
                        else {
                            $(this).prop("selected", true);
                        }
                    });
                }
                else {
                    $("#rightRuleSetCompanies option").prop("selected", false);
                }

                $("#rightRuleSetCompanies").attr("selectedcompanyids", $("#rightRuleSetCompanies").val());
                $(".RuleSetGroupUpdateTemplate").find(".RuleSetDefaultTemplate").each(function (index) {
                    //$(this).find(".RuleSetGroupTitle").html("RULE " + (index + 1) + " (Total Cost)");
                    $(this).find(".RuleSetGroupTitle").html("RULE " + (index + 1));
                    $(this).find("select").attr("GroupIndex", (index + 1));
                });
                $("#RuleSetGroupTemplate").addClass("hidden");

                $('[id^="RuleSetGroupCollapase_"]').unbind("click");
                $('[id^="RuleSetGroupCollapase_"]').on("click", function () {
                    if ($(this).attr('src').indexOf('expand') > 0) {
                        if (IsAutomationRuleSet) {
                            $(this).attr('src', 'images/Search-collapse.png');
                        }
                        else {
                            $(this).attr('src', '../images/Search-collapse.png');
                        }
                    }
                    else {
                        if (IsAutomationRuleSet) {
                            $(this).attr('src', 'images/expand.png')
                        }
                        else {
                            $(this).attr('src', '../images/expand.png')
                        }
                    }
                    $(this).closest('.RuleSetDefaultTemplate').find('.crs-blk').slideToggle();
                });

                //To  Set selected Attbute
                var allSelectedItem = "";
                $("#RuleSetGroupUpdateTemplate select").each(function (index) {
                    var selectedArray = [];
                    $(this).find("option").each(function () {
                        if ($(this).prop("selected")) {
                            selectedArray.push($(this).val());
                        }
                    });
                    $(this).attr("selectedItem", selectedArray.toString());


                    allSelectedItem = allSelectedItem + selectedArray.toString() + ",";
                });

                var finalSelectedItem = allSelectedItem.trim().substring(0, allSelectedItem.trim().length - 1);
                var AddRuleSetGroupCompany;
                if ($("#rightRuleSetCompanies").attr("selectedcompanyids") != undefined) {
                    AddRuleSetGroupCompany = $("#rightRuleSetCompanies").attr("selectedcompanyids").split(',').filter(function (obj) { return finalSelectedItem.split(',').indexOf(obj) == -1; });
                }

                if (AddRuleSetGroupCompany != "" && AddRuleSetGroupCompany != undefined) {
                    $("#RuleSetGroupUpdateTemplate select").each(function (index) {
                        var selectedItem = $(this).attr("selecteditem");
                        var currentSelect = $(this);
                        $(this).html("");
                        $("#rightRuleSetCompanies").find("option").each(function () {
                            if ($.inArray($(this).val(), selectedItem.split(',')) != -1) {
                                $("<option value='" + $(this).val() + "' selected='selected'>" + $(this).text() + "</option>").appendTo(currentSelect);
                            }
                            else if ($.inArray($(this).val(), AddRuleSetGroupCompany.toString().split(',')) != -1) {
                                $("<option value='" + $(this).val() + "'>" + $(this).text() + "</option>").appendTo(currentSelect);
                            }
                        });
                    });
                }

                //for check button enable/disble if competitor has some unselect item
                $("#RuleSetGroupUpdateTemplate select").each(function (index) {
                    //Button Operation hide/show
                    var splitValue = $(this).attr("selectedItem").split(',');
                    if ($(this).find("option").length == splitValue.length) {
                        $("#AddRuleSetTemplate").prop('disabled', true).addClass("disable-button");
                    }
                    else {
                        $("#AddRuleSetTemplate").prop('disabled', false).removeClass("disable-button");
                    }
                });

                //User in update mode
                //Check for if there is no rulesetgroup incase user has to remove any company from competitor and any one ruleset has one group with one company.
                if ($("#RuleSetGroupUpdateTemplate #RuleSetDefaultTemplate").length == 0) {
                    $("#AddRuleSetTemplate").click();
                    $("#RuleSetGroupUpdateTemplate #RuleSetTemplateCompanies").html("");
                }

                ValidateRuleSetGroupTemplateTextboxEvent();

                //Automation console enhancement changes base rate grid operation
                setTimeout(function () {
                    if (IsAutomationRuleSet) {
                        if (IsCompeteOnBase) {
                            if (RuleSetID == 0) {
                                $("#RuleSetGroupTemplate #TotalMasterDiv").hide();
                                $("#RuleSetGroupTemplate #BaseMasterDiv").show();
                                $("#RuleSetGroupTemplate #BaseMasterDiv span").css("margin-left", "0.5%");
                            }
                            else {
                                $("#RuleSetGroupUpdateTemplate #TotalMasterDiv").hide();
                                $("#RuleSetGroupUpdateTemplate #BaseMasterDiv").show();
                                $("#RuleSetGroupUpdateTemplate #BaseMasterDiv span").css("margin-left", "0.5%");
                            }
                        }
                        else {
                            if (RuleSetID == 0) {
                                $("#RuleSetGroupTemplate #TotalMasterDiv").show();
                                $("#RuleSetGroupTemplate #BaseMasterDiv").hide();
                                $("#RuleSetGroupTemplate #BaseMasterDiv span").css("margin-left", "16.5%");
                            }
                            else {
                                $("#RuleSetGroupUpdateTemplate #TotalMasterDiv").show();
                                $("#RuleSetGroupUpdateTemplate #BaseMasterDiv").hide();
                                $("#RuleSetGroupUpdateTemplate #BaseMasterDiv span").css("margin-left", "16.5%");
                            }
                        }
                    }
                }, 150);

                setTimeout(function () {
                    $('#spanSave').hide();
                }, 2000);
                if (IsCopyAndCreate) {
                    CopyAndCreate();
                    IsCopyAndCreate = false;
                }
            }
        },
        error: function (e) {
            $('.loader_container_source').hide();
            console.log(e.message);
        }
    });
}

function CopyAndCreate() {

    $("#RuleSetGroupTemplate").html("");
    //$("#RuleSetGroupTemplate").removeClass("hidden");
    $("#RuleSetGroupUpdateTemplate").children().appendTo("#RuleSetGroupTemplate");
    $("#RuleSetGroupTemplate").removeClass("hidden");
    $("#RuleSetGroupTemplate").removeAttr("style");
    $("#RuleSetGroupUpdateTemplate").addClass("hidden");
    if (!IsAutomationRuleSet) {
        $("#HaddingTitle").html("CREATE RULE SET");
        $("#rightRuleSetName").val("");
        $("#selectedRuleSetID_" + RuleSetID).removeClass("rsselected");
        $(window).scrollTop(0);
    }

    //Check for if there is no rulesetgroup incase user has to remove any company from competitor and any one ruleset has one group with one company.
    if ($("#RuleSetGroupTemplate #RuleSetDefaultTemplate").length == 0) {
        $("#AddRuleSetTemplate").click();
        $("#RuleSetGroupTemplate #RuleSetTemplateCompanies").html("");
    }
    RuleSetID = 0;
}

function boolTemplate(template) {
    return JSON.parse(template());
}

//Selection Event of company
function AutoPopulateCompanies(companiesData) {
    if ($(companiesData).val() == null) {
        $(companiesData).attr("selectedcompanyids", "");
    }
    else {
        $(companiesData).attr("selectedcompanyids", $(companiesData).val());
    }
    var matches = null;
    matches = $.map(globalCompanies, function (item) {
        if ($("#rightRuleSetCompanies option[value='" + item.ID + "']").prop('selected')) {
            return new companies(item);
        }
    });

    if (RuleSetID == 0) {
        //Insert Case
        var tempSelectedArrayID = "";
        var SelectedArrayID = "";
        //get all selected competitor id 
        $("#RuleSetGroupTemplate select").each(function () {
            if ($(this).attr("selecteditem") != "" && $(this).attr("selecteditem") != undefined) {
                tempSelectedArrayID += ($(this).attr("selecteditem").trim() + ",");
            }
        });
        if (tempSelectedArrayID != "") {
            SelectedArrayID = tempSelectedArrayID.trim().substring(0, tempSelectedArrayID.trim().length - 1);
        }
        SelectedArrayID = tempSelectedArrayID.trim().substring(0, tempSelectedArrayID.trim().length - 1);
        $("#RuleSetGroupTemplate select").each(function (index) {
            $(this).empty();

            if ($(this).attr("selectedItem") == '' && $(this).attr("selectedItem") == undefined) {
                $(matches).each(function () {
                    if ($.inArray(this.ID, SelectedArrayID.trim().split(",")) == -1) {
                        $("<option value='" + this.ID + "'>" + this.Name + "</option>").appendTo($("#RuleSetGroupTemplate .RuleSetTemplateCompanies").eq(index));
                    }
                });
            }
            else {
                var selectedID = $(this).attr("selectedItem");
                var allSelectedItem = "";
                $("#RuleSetGroupTemplate select").each(function (index) {
                    if ($(this).attr("selectedItem") != undefined) {
                        allSelectedItem += $(this).attr("selectedItem") + ",";
                    }
                });
                var finalSelectedID = allSelectedItem.trim().substring(0, allSelectedItem.trim().length - 1);
                $(matches).each(function (matchIndex) {
                    if (!JSON.parse(this.CompanySelection)) {
                        $("<option value='" + this.ID + "'>" + this.Name + "</option>").appendTo($("#RuleSetGroupTemplate .RuleSetTemplateCompanies").eq(index));
                    }

                    if (selectedID != undefined) {
                        if ($.inArray(this.ID.toString(), selectedID.split(',')) != -1) {
                            var matchItem = this;
                            for (var i = 0; i < selectedID.split(',').length; i++) {
                                if (selectedID.split(',')[i] == matchItem.ID) {
                                    this.CompanySelection = "true";
                                }
                            }
                        }
                    }

                });

                var Deleteoption = "";
                if (finalSelectedID.length != 0) {
                    if (selectedID == undefined) {
                        selectedID = "";
                    }
                    Deleteoption = finalSelectedID.split(',').filter(function (obj) { return selectedID.indexOf(obj) == -1; });
                }

                $(this).find("option").each(function () {
                    var optionVal = $(this).val();
                    if (selectedID != undefined) {
                        if ($.inArray(optionVal.toString(), selectedID.split(',')) == -1) {
                            $(this).prop("selected", false);
                        }
                        else {
                            $(this).prop("selected", true);
                        }
                        //This Condition is for AllControl item selected removed
                        if ($.inArray(optionVal.toString(), Deleteoption.toString().split(',')) != -1) {
                            $(this).remove();
                        }
                    }
                });
                $(this).attr("selectedItem", $(this).val());
            }
        });

        //Second loop is used for check the addNewGroup button disabled/enable
        var CheckAddButton = false;
        $("#RuleSetGroupTemplate select").each(function () {
            if ($(this).val() == null) {
                CheckAddButton = false;
                return false;
            }
            if ($(this).val().length != $(this).find("option").length) {
                CheckAddButton = true;
                return false;
            }
        });
        if (!CheckAddButton) {
            $("#AddRuleSetTemplate").prop('disabled', true).addClass("disable-button");
        }
        else {
            $("#AddRuleSetTemplate").prop('disabled', false).removeClass("disable-button");
        }
    }
    else {
        //Update Case

        var tempSelectedArrayID = "";
        var SelectedArrayID = "";
        //get all selected competitor id 
        $("#RuleSetGroupUpdateTemplate select").each(function () {
            if ($(this).attr("selecteditem") != "" && $(this).attr("selecteditem") != undefined) {
                tempSelectedArrayID += ($(this).attr("selecteditem").trim() + ",");
            }
        });
        if (tempSelectedArrayID != "") {
            SelectedArrayID = tempSelectedArrayID.trim().substring(0, tempSelectedArrayID.trim().length - 1);
        }

        $("#RuleSetGroupUpdateTemplate select").each(function (index) {
            var ID = $(this).attr("ID");
            $(this).empty();
            if ($(this).attr("selectedItem") == '' && $(this).attr("selectedItem") == undefined) {
                $(matches).each(function () {
                    if ($.inArray(this.ID, SelectedArrayID.trim().split(",")) == -1) {
                        $("<option value='" + this.ID + "'>" + this.Name + "</option>").appendTo($("#RuleSetGroupTemplate .RuleSetTemplateCompanies").eq(index));
                    }
                });
            }
            else {
                var selectedID = $(this).attr("selectedItem");
                var allSelectedItem = "";
                $("#RuleSetGroupUpdateTemplate select").each(function (index) {
                    //if ($(this).attr("selectedItem") != undefined) {
                    allSelectedItem += $(this).attr("selectedItem") + ",";
                    //}
                });
                var finalSelectedID = allSelectedItem.trim().substring(0, allSelectedItem.trim().length - 1);

                $(matches).each(function (matchIndex) {
                    if (!JSON.parse(this.CompanySelection)) {
                        $("<option value='" + this.ID + "'>" + this.Name + "</option>").appendTo($("#RuleSetGroupUpdateTemplate .RuleSetTemplateCompanies").eq(index));
                    }

                    if (selectedID != undefined) {
                        if ($.inArray(this.ID.toString(), selectedID.split(',')) != -1) {
                            var matchItem = this;
                            for (var i = 0; i < selectedID.split(',').length; i++) {
                                if (selectedID.split(',')[i] == matchItem.ID) {
                                    this.CompanySelection = "true";
                                }
                            }
                        }
                    }
                });

                var Deleteoption = "";
                if (finalSelectedID.length != 0) {
                    if (selectedID == undefined) {
                        selectedID = "";
                    }
                    Deleteoption = finalSelectedID.split(',').filter(function (obj) { return selectedID.indexOf(obj) == -1; });
                }

                $(this).find("option").each(function () {
                    var optionVal = $(this).val();
                    if (selectedID != undefined) {
                        if ($.inArray(optionVal.toString(), selectedID.split(',')) == -1) {
                            $(this).prop("selected", false);
                        }
                        else {
                            $(this).prop("selected", true);
                        }
                        //This Condition is for AllControl item selected removed
                        if ($.inArray(optionVal.toString(), Deleteoption.toString().split(',')) != -1) {
                            $(this).remove();
                        }
                    }
                });
                $(this).attr("selectedItem", $(this).val());
            }
        });

        //Second loop is used for check the addNewGroup button disabled/enable
        var CheckAddButton = false;
        $("#RuleSetGroupUpdateTemplate select").each(function () {
            if ($(this).val() == null) {
                CheckAddButton = false;
                return false;
            }
            if ($(this).val().length != $(this).find("option").length) {
                CheckAddButton = true;
                return false;
            }
        });
        if (!CheckAddButton) {
            $("#AddRuleSetTemplate").prop('disabled', true).addClass("disable-button");
        }
        else {
            $("#AddRuleSetTemplate").prop('disabled', false).removeClass("disable-button");
        }
    }
    ruleSetViewModel.ruleSetGroupCompanies(matches);
}
//This function is RuleSetGroup Competitor Event
function AutoPopulateGroupCompanies(CompaniesData) {
    var companySelectedCount = 0;

    if (RuleSetID == 0) {
        var GroupIndex = $(CompaniesData).attr("groupindex");
        //This option is use to get before selectedID
        var PreviousSelectedItem = $(CompaniesData).attr("selectedItem");
        $("#RuleSetGroupTemplate select").each(function (index) {
            var selectedArray = [];
            $(this).find("option").each(function () {
                if ($(this).prop("selected")) {
                    selectedArray.push($(this).val());
                }
            });

            $(this).attr("selectedItem", selectedArray.toString());

            //Button Operation hide/show
            //if ($(this).find("option").length == selectedArray.length) {
            //    $("#AddRuleSetTemplate").prop('disabled', true).addClass("disable-button");
            //}
            //else {
            //    $("#AddRuleSetTemplate").prop('disabled', false).removeClass("disable-button");
            //}
        });

        //Logic for selected value is remove
        var selectedItem = $(CompaniesData).attr("selectedItem");
        $("#RuleSetGroupTemplate select").each(function (index) {

            var otherSelectedItem = $(this).attr("selectedItem");
            if ($(this).attr("groupindex") != GroupIndex) {
                $(this).find("option").each(function () {
                    for (var i = 0; i < selectedItem.split(',').length; i++) {
                        //Selected Item Checked for remove operation
                        if ($.inArray(selectedItem.split(',')[i].toString(), otherSelectedItem.split(',')) != -1) {
                            if (selectedItem.split(',')[i] == $(this).val()) {
                                $(this).remove();
                            }
                        }
                        //Other unselect Item remove
                        if (selectedItem.split(',')[i] == $(this).val()) {
                            $(this).remove();
                        }
                    }
                });
            }
            else {
                //This else for selected value can be unselect then that unselected item filled to another group select item
                //This one current selected selectboxitem stored in selectedGroupItemData
                var selectedGroupItemData = [];
                $(this).find("option").each(function () {
                    selectedGroupItemData.push(this);
                });

                //This check on previuosSelctedItem in current selectedItem 
                if (PreviousSelectedItem != undefined) {
                    for (var i = 0; i < PreviousSelectedItem.split(',').length; i++) {
                        //if use for check current selected item is exist or not in previousSelectedItem
                        if ($.inArray(PreviousSelectedItem.split(',')[i].toString(), selectedItem.split(',')) == -1) {
                            //check here for not check value to addd current option tag

                            //This one is used for internal excapt selected another selecte box item
                            $("#RuleSetGroupTemplate select").each(function (index) {
                                var currentSelect = $(this);
                                if ($(this).attr("groupindex") != GroupIndex) {
                                    //Already stored in 
                                    $(selectedGroupItemData).each(function () {
                                        if (PreviousSelectedItem.split(',')[i] == $(this).val()) {
                                            var tempOptionID = $(this).val();
                                            var tempText = $(this).text();
                                            var IsAppend = false;
                                            var lastOptionVal = "";
                                            currentSelect.find("option").each(function () {
                                                lastOptionVal = parseInt($(this).val());
                                                if (parseInt(tempOptionID) < lastOptionVal) {
                                                    $("<option value='" + tempOptionID + "'>" + tempText + "</option>").insertBefore(currentSelect.find('option[value="' + $(this).val() + '"]')).eq(index);
                                                    IsAppend = true;
                                                    return false;
                                                }
                                            });
                                            if (!IsAppend) {
                                                $("<option value='" + tempOptionID + "'>" + tempText + "</option>").appendTo(currentSelect).eq(index);
                                                //$("<option value='" + tempOptionID + "'>" + tempText + "</option>").insertAfter(currentSelect.find('option[value="' + lastOptionVal + '"]')).eq(index);;
                                                return false;
                                            }
                                            // $("<option value='" + $(this).val() + "'>" + $(this).text() + "</option>").appendTo(currentSelect).eq(index);
                                            return false; //Stop current loop itration
                                        }
                                    });
                                }
                            });
                        } //End If array
                    } //End For
                }//End If
            }
        });

        //Third loop is used for check the addNewGroup button disabled/enable
        var CheckAddButton = false;
        $("#RuleSetGroupTemplate select").each(function () {
            if ($(this).attr("groupindex") != GroupIndex) {
                if ($(this).val() == null) {
                    CheckAddButton = false;
                    return false;
                }
            }
            else {
                if ($(this).val() == null) {
                    CheckAddButton = false;
                    return false;
                }
            }
            if ($(this).val().length != $(this).find("option").length) {
                CheckAddButton = true;
                return false;
            }
        });
        if (!CheckAddButton) {
            $("#AddRuleSetTemplate").prop('disabled', true).addClass("disable-button");
            CheckAddButton = false;
        }
        else {
            $("#AddRuleSetTemplate").prop('disabled', false).removeClass("disable-button");
            CheckAddButton = false;
        }

    }
    else {
        var GroupIndex = $(CompaniesData).attr("groupindex");
        //This option is use to get before selectedID
        var PreviousSelectedItem = $(CompaniesData).attr("selectedItem");

        $("#RuleSetGroupUpdateTemplate select").each(function (index) {
            var selectedArray = [];

            $(this).find("option").each(function () {
                if ($(this).prop("selected")) {
                    selectedArray.push($(this).val());
                }
            });
            $(this).attr("selectedItem", selectedArray.toString());

            ////Button Operation hide/show
            //if ($(this).find("option").length == selectedArray.length) {
            //    $("#AddRuleSetTemplate").prop('disabled', true).addClass("disable-button");
            //}
            //else {
            //    $("#AddRuleSetTemplate").prop('disabled', false).removeClass("disable-button");
            //}
        });

        //Logic for selected value is remove
        var selectedItem = $(CompaniesData).attr("selectedItem");
        $("#RuleSetGroupUpdateTemplate select").each(function (index) {
            var otherSelectedItem = $(this).attr("selectedItem");
            if ($(this).attr("groupindex") != GroupIndex) {
                $(this).find("option").each(function () {
                    for (var i = 0; i < selectedItem.split(',').length; i++) {
                        //Selected Item Checked for remove operation
                        if ($.inArray(selectedItem.split(',')[i].toString(), otherSelectedItem.split(',')) != -1) {
                            if (selectedItem.split(',')[i] == $(this).val()) {
                                $(this).remove();
                            }
                        }
                        //Other unselect Item remove
                        if (selectedItem.split(',')[i] == $(this).val()) {
                            $(this).remove();
                        }
                    }
                });
            }
            else {
                //This else for selected value can be unselect then that unselected item filled to another group select item
                //This one current selected selectboxitem stored in selectedGroupItemData
                var selectedGroupItemData = [];
                $(this).find("option").each(function () {
                    selectedGroupItemData.push(this);
                });

                //This check on previuosSelctedItem in current selectedItem 
                if (PreviousSelectedItem != undefined) {
                    for (var i = 0; i < PreviousSelectedItem.split(',').length; i++) {
                        //if use for check current selected item is exist or not in previousSelectedItem
                        if ($.inArray(PreviousSelectedItem.split(',')[i].toString(), selectedItem.split(',')) == -1) {
                            //check here for not check value to addd current option tag

                            //This one is used for internal excapt selected another selecte box item
                            $("#RuleSetGroupUpdateTemplate select").each(function (index) {
                                var currentSelect = $(this);
                                if ($(this).attr("groupindex") != GroupIndex) {
                                    //Already stored in 
                                    $(selectedGroupItemData).each(function () {
                                        if (PreviousSelectedItem.split(',')[i] == $(this).val()) {
                                            var tempOptionID = $(this).val();
                                            var tempText = $(this).text();
                                            var IsAppend = false;
                                            var lastOptionVal = "";
                                            currentSelect.find("option").each(function () {
                                                lastOptionVal = parseInt($(this).val());
                                                if (parseInt(tempOptionID) < lastOptionVal) {
                                                    $("<option value='" + tempOptionID + "'>" + tempText + "</option>").insertBefore(currentSelect.find('option[value="' + $(this).val() + '"]')).eq(index);
                                                    IsAppend = true;
                                                    return false;
                                                }
                                            });
                                            if (!IsAppend) {
                                                $("<option value='" + tempOptionID + "'>" + tempText + "</option>").appendTo(currentSelect).eq(index);
                                                //$("<option value='" + tempOptionID + "'>" + tempText + "</option>").insertAfter(currentSelect.find('option[value="' + lastOptionVal + '"]')).eq(index);;
                                                return false;
                                            }
                                            //$("<option value='" + $(this).val() + "'>" + $(this).text() + "</option>").appendTo(currentSelect).eq(index);
                                            return false; //Stop current loop itration
                                        }
                                    });
                                }
                            });
                        } //End If array
                    } //End For
                }//End If
            }
        });

        //Third loop is used for check the addNewGroup button disabled/enable
        var CheckAddButton = false;
        $("#RuleSetGroupUpdateTemplate select").each(function () {
            if ($(this).attr("groupindex") != GroupIndex) {
                if ($(this).val() == null) {
                    return false;
                }
            }
            else {
                if ($(this).val() == null) {
                    return false;
                }
            }
            if ($(this).val().length != $(this).find("option").length) {
                CheckAddButton = true;
                return false;
            }
        });
        if (!CheckAddButton) {
            $("#AddRuleSetTemplate").prop('disabled', true).addClass("disable-button");
        }
        else {
            $("#AddRuleSetTemplate").prop('disabled', false).removeClass("disable-button");
        }
    }
}
function AddRuleSetTemplate() {
    $("#AddRuleSetTemplate").on("click", function () {
        $("#AddRuleSetTemplate").prop('disabled', true).addClass("disable-button");
        if (RuleSetID == 0) {
            //in New ruleSet Creation
            $(".DefaultTemplate").clone().appendTo("#RuleSetGroupTemplate");
            $("</br>").clone().appendTo("#RuleSetGroupTemplate");
            $("#RuleSetGroupTemplate #RuleSetDefaultTemplate").removeClass("DefaultTemplate");
            $(".RuleSetGroupTemplate").find(".RuleSetDefaultTemplate").each(function (index) {
                //$(this).find(".RuleSetGroupTitle").html("RULE " + (index + 1) + " (Total Cost)");
                $(this).find(".RuleSetGroupTitle").html("RULE " + (index + 1));
                $(this).find("img").attr("id", "RuleSetGroupCollapase_" + (index + 1));
                $(this).find("select").attr("GroupIndex", (index + 1));
            });

            //for only one ruleset template is on grid
            if ($("#RuleSetGroupTemplate #RuleSetDefaultTemplate select").length != 1) {
                //Get the last parent selecte tag option
                var totalSelect = $("#RuleSetGroupTemplate #RuleSetDefaultTemplate select").length - 2;

                $("#RuleSetGroupTemplate #RuleSetDefaultTemplate select").eq(totalSelect).each(function () {
                    $("#RuleSetGroupTemplate #RuleSetDefaultTemplate select").last().html("");
                    $($(this)).find("option").each(function () {
                        if ($(this).prop("selected")) {
                            $("#RuleSetGroupTemplate #RuleSetDefaultTemplate select").last().append();
                        }
                        else {
                            $("#RuleSetGroupTemplate #RuleSetDefaultTemplate select").last().append("<option value=" + $(this).val() + ">" + $(this).text() + "</option>");
                        }
                    });
                });
                //incase user dont select any one option in group compaetitor and go to add company scenario only add extra select company
                $("#RuleSetGroupTemplate #RuleSetDefaultTemplate select").each(function () {
                    if ($(this).attr("selecteditem") == "" || $(this).attr("selecteditem") == undefined) {
                        $(this).removeAttr("selecteditem");
                    }
                });
            }
            // $(".RuleSetTemplateCompanies").click();
        }
        else {
            //in Update ruleSet Creation
            $(".DefaultTemplate").clone().appendTo("#RuleSetGroupUpdateTemplate");

            //Check for if there is no rulesetgroup incase user has to remove any company from competitor and any one ruleset has one group with one company.
            if ($("#RuleSetGroupUpdateTemplate #RuleSetDefaultTemplate").length > 1) {
                $("</br>").clone().appendTo("#RuleSetGroupUpdateTemplate");
            }
            $("#RuleSetGroupUpdateTemplate #RuleSetDefaultTemplate");
            $("#RuleSetGroupUpdateTemplate ").find(".DefaultTemplate").attr("newadd", "yes");
            $("#RuleSetGroupUpdateTemplate #RuleSetDefaultTemplate").removeClass("DefaultTemplate");

            $(".RuleSetGroupUpdateTemplate").find(".RuleSetDefaultTemplate").each(function (index) {
                //$(this).find(".RuleSetGroupTitle").html("RULE " + (index + 1) + " (Total Cost)");
                $(this).find(".RuleSetGroupTitle").html("RULE " + (index + 1));
                $(this).find("img").attr("id", "RuleSetGroupCollapase_" + (index + 1));
                $(this).find("select").attr("GroupIndex", (index + 1));
            });

            //for only one ruleset template is on grid
            if ($("#RuleSetGroupUpdateTemplate #RuleSetDefaultTemplate select").length != 1) {
                //Get the last parent selecte tag option
                var totalSelect = $("#RuleSetGroupUpdateTemplate #RuleSetDefaultTemplate select").length - 2;

                $("#RuleSetGroupUpdateTemplate #RuleSetDefaultTemplate select").eq(totalSelect).each(function () {
                    $("#RuleSetGroupUpdateTemplate #RuleSetDefaultTemplate select").last().html("");
                    $($(this)).find("option").each(function () {
                        if ($(this).prop("selected")) {
                            $("#RuleSetGroupUpdateTemplate #RuleSetDefaultTemplate select").last().append();
                        }
                        else {
                            $("#RuleSetGroupUpdateTemplate #RuleSetDefaultTemplate select").last().append("<option value=" + $(this).val() + ">" + $(this).text() + "</option>");
                        }
                    });
                });

                //incase user dont select any one option in group compaetitor and go to add company scenario only add extra select company
                $("#RuleSetGroupUpdateTemplate #RuleSetDefaultTemplate select").each(function () {
                    if ($(this).attr("selecteditem") == "" || $(this).attr("selecteditem") == undefined) {
                        $(this).removeAttr("selecteditem");
                    }
                });
            }
            //$(".RuleSetTemplateCompanies").click();
        }
        $('[id^="RuleSetGroupCollapase_"]').unbind("click");
        $('[id^="RuleSetGroupCollapase_"]').on("click", function () {
            if ($(this).attr('src').indexOf('expand') > 0) {
                if (IsAutomationRuleSet) {
                    $(this).attr('src', 'images/Search-collapse.png');
                }
                else {
                    $(this).attr('src', '../images/Search-collapse.png');
                }
            }
            else {
                if (IsAutomationRuleSet) {
                    $(this).attr('src', 'images/expand.png')
                }
                else {
                    $(this).attr('src', '../images/expand.png')
                }
            }
            $(this).closest('.RuleSetDefaultTemplate').find('.crs-blk').slideToggle();
        });
        ValidateRuleSetGroupTemplateTextboxEvent();
    });

}
function FilterOperation(carClassID, rentalLengthID, locationBrandID, weekDaysID) {
    $("#RuleSetList li").each(function () {
        var IsVisible = true;
        var IsLocationVisible = true, IsCarClassVisible = true, IsRentalLengthVisible = true, IsWeekDayVisible = true;

        var tempLocationID = $(this).attr("locationbrandid");
        var countLocationBrandMatch = 0;
        $(locationBrandID).each(function () {
            var compareLocationID = this;
            //console.log(compareLocationID.toString());
            if ($.inArray(compareLocationID.toString(), tempLocationID.split(',')) == -1) {
                IsLocationVisible = false;
                //console.log("false " + compareLocationID + " brandID " + tempLocationID + "  status " + IsVisible);
            }
            else {
                countLocationBrandMatch++;
                //IsLocationVisible = true;
                //if (locationBrandID.length == countLocationBrandMatch) {
                IsLocationVisible = true;
                return false;
                //}
                //else {
                //    IsLocationVisible = false;
                //}
            }
        });


        var tempCarClassID = $(this).attr("carclassid");
        var countCarClassMatch = 0;
        $(carClassID).each(function () {
            var compareCarClassID = this;
            if ($.inArray(compareCarClassID.toString(), tempCarClassID.split(',')) == -1) {
                //console.log("false " + compareCarClassID + " carclass " + tempCarClassID);
                IsCarClassVisible = false;
            }
            else {
                countCarClassMatch++;
                IsCarClassVisible = true;
                if (carClassID.length == countCarClassMatch) {
                    IsCarClassVisible = true;
                    return false;
                }
                else {
                    IsCarClassVisible = true;
                }
            }
        });


        var tempWeekDaysID = $(this).attr("weekdaysid");
        var countWeekDayMatch = 0;
        $(weekDaysID).each(function () {
            var compareWeekDaysID = this;
            if ($.inArray(compareWeekDaysID.toString(), tempWeekDaysID.split(',')) == -1) {
                //console.log("false " + compareWeekDaysID + " weekDays " + tempWeekDaysID);
                IsWeekDayVisible = false;
            }
            else {
                countWeekDayMatch++;
                IsWeekDayVisible = true;
                if (weekDaysID.length == countWeekDayMatch) {
                    IsWeekDayVisible = true;
                    return false;
                }
                else {
                    IsWeekDayVisible = false;
                }
            }
        });


        var tempRentalLengthID = $(this).attr("rentallengthid");
        var countRentalLenthMatch = 0;
        $(rentalLengthID).each(function () {
            var compareRentalLengthID = this;
            if ($.inArray(compareRentalLengthID.toString(), tempRentalLengthID.split(',')) == -1) {
                //console.log("false " + compareRentalLengthID + " weekDays " + tempRentalLengthID);
                IsRentalLengthVisible = false;
            }
            else {
                countRentalLenthMatch++;
                IsRentalLengthVisible = true;
                if (rentalLengthID.length == countRentalLenthMatch) {
                    IsRentalLengthVisible = true;
                    return false;
                }
                else {
                    IsRentalLengthVisible = false;
                }
            }
        });
        // console.log(" value  " + $(this).val() + "  " + IsVisible + "  " + IsLocationVisible + "   " + IsCarClassVisible + "  " + IsWeekDayVisible + "  " + IsRentalLengthVisible);

        //if ((JSON.parse(IsLocationVisible) == true) && (JSON.parse(IsCarClassVisible) == true) && (JSON.parse(IsWeekDayVisible) == true) && (JSON.parse(IsRentalLengthVisible) == true)) {
        if ((JSON.parse(IsLocationVisible) == true) && (JSON.parse(IsRentalLengthVisible) == true)) {
            $(this).show();
            //console.log("Show " + $(this).val());
        }
        else {
            $(this).hide();
            //console.log("hide " + $(this).val());
        }
    });
}
function resetFilterSelection() {
    $("#RuleSetList").scrollTop(0);
    //$("#RuleSetSearchLocation").val("");
    $("#mtab1 select option").each(function () {
        $(this).prop("selected", false);
    })
    $("#RuleSetList li").show();
    $("#CreateRuleset").click();
    resetLeftBranLocation();
    //$("#rightPanelControl select option").each(function () {
    //    $(this).prop("selected", false);
    //})
    //$("#RuleSetList li").show();
    //$("#rightRuleSetIsPositionOffset").val("");
    //$("#rightRuleSetCompanyPositionAbvAvg").val("");
    //$("#rightRuleSetName").val("");
    //$("#rightRuleSetIsWideGapTemplate").prop("checked", false);
    //$("#rg1:checked").prop("checked", false);
    //var srcs = $.map(ruleSetSmartSearchLocations, function (item) { return new brandLocations(item); });
    //ruleSetViewModel.leftBrandLocations(srcs);
}
function resetLeftBranLocation() {
    $('#RuleSetSearchLocation').val("");
    RemoveFlashableTag("#RuleSetSearchLocation");
    var srcs = $.map(ruleSetSmartSearchLocations, function (item) { return new brandLocations(item); });
    ruleSetViewModel.leftBrandLocations(srcs);
    AddFlashingEffect();
}
function AnimateLeftPanel() {

    $('#searchLeftPanel, #searchLeftPanelSecond').slideUp();
    $('#left-col').hide('slide', { direction: 'left' }, 1000);
    $('#right-col').addClass('calculatedWidth');
    $('#left-col .collapse').attr('src', '../images/expand.png');

    setTimeout(function () {
        $('.left-col-toggle').show(250).click(function () {
            $(this).hide(250);
            setTimeout(function () {
                $('#left-col').show('slide', { direction: 'left' }, 750);
                setTimeout(function () {
                    $('#searchLeftPanel, #searchLeftPanelSecond').slideDown();
                    $('#right-col').removeClass('calculatedWidth');
                    $('#left-col .collapse').attr('src', '../images/Search-collapse.png');
                }, 750);
            }, 250);
        });
    }, 750);
}
function GetLocationSpecificBrandCompany() {
    $("#rightRuleSetlocations").on("click", "li", function () {
        //var selectedLocationID = parseInt($("#rightRuleSetlocations ul").find(".selected").attr("BrandID"));
        if (parseInt($("#rightRuleSetlocations ul").find(".selected").val()) != 0) {

            //filter competitors on selected brand

            setTimeout(function () {
                getBrandSpecificCompetitors(parseInt($("#rightRuleSetlocations ul").find(".selected").attr("locationbrandid")))
            }, 200);
        }
        else {
            ruleSetViewModel.masterCompanies(globalCompanies);
            ruleSetViewModel.ruleSetGroupCompanies(globalCompanies);
            $("#rightRuleSetCompanies").prop("disabled", true);
        }
    });
}
function GetLocationSpecificBrandCompanyRuleSetGroupOperation() {
    var selectedLocationID = parseInt($("#rightRuleSetlocations ul").find(".selected").attr("BrandID"));
    var matches = [];
    var lastCode = "";
    var competitors = [];

    $("#rightRuleSetCompanies").val("");
    //$("#RuleSetGroupTemplate #RuleSetTemplateCompanies").html("");//Remove select box exmpty
    $("#rightRuleSetCompanies").prop("disabled", false);
    $("#rightRuleSetCompanies option").prop("selected", false);
    if (competitorIds != "" && competitorIds != undefined) {
        var competitorCompanies = allCompanies;
        $(competitorCompanies).each(function () {
            if ($.inArray(this.ID.toString(), competitorIds.trim().split(",")) != -1) {
                competitors.push(this);
            }
        });
        competitorCompanies = competitors;
        globalCompanies = competitorCompanies;

        $(globalCompanies).each(function () {

            if (this.ID == selectedLocationID) {
                lastCode = this.Name;
            }
            if (this.ID != selectedLocationID) {
                matches.push(this);
            }
        });
    }


    ruleSetViewModel.masterCompanies(matches);
    ruleSetViewModel.ruleSetGroupCompanies(matches);


    $("#rightRuleSetCompanies option").each(function () {
        if ($("#rightRuleSetCompanies").attr("selectedCompanyIDs") != undefined) {
            if ($.inArray($(this).val(), ($("#rightRuleSetCompanies").attr("selectedCompanyIDs")).split(',')) == -1) {
                $(this).prop("selected", false);
            }
            else {
                $(this).prop("selected", true);
            }
        }
    });

    var DeletedCompanyID = "";
    if ($("#rightRuleSetCompanies").attr("selectedCompanyIDs") != undefined && $("#rightRuleSetCompanies option").length > 0 && $("#rightRuleSetCompanies").val() != null && $("#rightRuleSetCompanies").val() == undefined) {
        if ($("#rightRuleSetCompanies").attr("selectedCompanyIDs") != "") {
            DeletedCompanyID = ($("#rightRuleSetCompanies").attr("selectedCompanyIDs")).split(',').filter(function (obj) { return $("#rightRuleSetCompanies").val().indexOf(obj) == -1; });
        }
    }

    if (DeletedCompanyID != "") {
        if (RuleSetID == 0) {
            $("#RuleSetGroupTemplate select").each(function () {
                $(this).find("option").each(function () {
                    if ($(this).val() == DeletedCompanyID) {
                        $(this).remove();
                    }
                });
                $(this).attr("selecteditem", $(this).val())
            });
        }
        else {
            $("#RuleSetGroupUpdateTemplate select").each(function () {
                $(this).find("option").each(function () {
                    if ($(this).val() == DeletedCompanyID) {
                        $(this).remove();
                    }
                });
                $(this).attr("selecteditem", $(this).val())
            });
        }
    }

    //Set time out for all above each condition executed
    // setTimeout(function () {
    $("#rightRuleSetCompanies").attr("selectedCompanyIDs", $("#rightRuleSetCompanies").val());
    //}, 250);

    var lastCodeChar = $("#rightRuleSetlocations ul").find(".selected").text().split("-");
    //if (lastCode != undefined) {
    //    $("#PositionCompetitor").html("Position of " + lastCode);
    //}
    //temporaray fix to display brand name 
    //Todo: get brand name  from database ("globalCompanies")
    if (lastCodeChar[1] == "EZ") {
        $("#PositionCompetitor").html("Position of EZ-RAC");
    }
    else {
        $("#PositionCompetitor").html("Position of Advantage");
    }
}
function GetLocationSpecificCarClassesRuleSet() {
    $("#rightRuleSetlocations").on("click", "li", function () {
        if (RuleSetID == 0) {
            var ajaxURl = '/RateShopper/Search/GetLocationCarClasses';
            if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
                ajaxURl = AjaxURLSettings.GetLocationCarClassesUrl;
            }
            var searchId = $(this).val(), $selectedOpt = $(this).attr("LocationBrandID");
            if ($selectedOpt != null && $selectedOpt != '') {
                // $('.loader_container_carclass').show();
                $("#rightRuleSetCarClass option").prop("selected", false);
                $.ajax({
                    url: ajaxURl,
                    data: { locationBrandId: $selectedOpt },
                    type: 'GET',
                    async: true,
                    success: function (data) {
                        if (data) {
                            //$('.loader_container_carclass').hide();
                            //var matches = [];
                            //selected locationwise get carclass ids
                            $(data).each(function () {
                                var tempCarClassID = this;
                                //To check globalRuleSetCarClass list while first time carclass get
                                $(GlobalRuleSetCarClass).each(function () {
                                    if (this.ID == tempCarClassID) {
                                        $("#rightRuleSetCarClass option[value=" + tempCarClassID + "]").prop("selected", true);
                                        //matches.push(this);
                                        return false;
                                    }
                                });
                            });
                            //ruleSetViewModel.rightCarClasses(matches);
                        }
                    },
                    error: function (e) {
                        console.log("in RuleSet-function GetLocationSpecificCarClasses: " + e.message);
                    }
                });
            }
        }
    });

}
function RuleSetSearchLocationGrid(inpuText, controlId) {
    if ($(inpuText).val().trim().length > 0) {
        var matches = $.map(ruleSetSmartSearchLocations, function (item) {
            if (item.LocationBrandAlias.toUpperCase().indexOf($(inpuText).val().trim().toUpperCase()) == 0) {
                return new brandLocations(item);
            }
        });
        ruleSetViewModel.leftBrandLocations(matches);
    } else {
        //$(controlId).show();
        var srcs = $.map(ruleSetSmartSearchLocations, function (item) { return new brandLocations(item); });
        ruleSetViewModel.leftBrandLocations(srcs);
    }
}
function ValidateRuleSetGroupTemplateTextboxEvent() {
    var Flag = false;
    if (RuleSetID == 0) {
        $("#RuleSetGroupTemplate input[type=text]").on("keyup", function () {
            if (!$.isNumeric($.trim($(this).val()))) {
                MakeTagFlashable($(this));
                flag = false;
            }
            else {
                RemoveFlashableTag($(this));
                Flag = true;
            }
        });
    }
    else {
        $("#RuleSetGroupUpdateTemplate input[type=text]").on("keyup", function () {
            //console.log("update" + $(this).parent().siblings($(this)).attr("IsChanged", "true"));
            $(this).parent().siblings($(this)).attr("IsChanged", "true");
            if (!$.isNumeric($.trim($(this).val()))) {
                MakeTagFlashable($(this));
                flag = false;
            }
            else {
                RemoveFlashableTag($(this));
                Flag = true;
            }
        });
    }
    AddFlashingEffect();
    return Flag;
}
function ValidateRuleSetData() {
    var Flag = false;
    var FlagRuleSetName = false, FlagRightSelectControl = false, FlagRightRentalLength = false, FlagRightCompanies = false, FlagRightCarClass = false, FlagRightweek = false, FlagRadio = false, FlagRightRuleSetLocation = false, FlagTextBox = false;
    var IsExistRuleSet = false;

    //$("#rightPanelControl input[type=text]").each(function () {
    if ($("#rightRuleSetName").val().trim() == "") {
        MakeTagFlashable($("#rightRuleSetName"));
        FlagRuleSetName = false;
    }
    else {
        if (RuleSetID == 0) {
            //insert case
            if ($("#RuleSetList li").length != 0) {
                if (!IsAutomationRuleSet) {
                    $("#RuleSetList li").each(function () {
                        if ($(this).text().trim() == $("#rightRuleSetName").val().trim()) {
                            MakeTagFlashable($("#rightRuleSetName"));
                            FlagRuleSetName = false;
                            IsExistRuleSet = true;
                            return false;
                        }
                        else {
                            RemoveFlashableTag($("#rightRuleSetName"));
                            IsExistRuleSet = false;
                            FlagRuleSetName = true;
                        }
                    });
                }
                else {
                    IsExistRuleSet = false;
                    FlagRuleSetName = true;
                }
            }
            else {
                RemoveFlashableTag($("#rightRuleSetName"));
                FlagRuleSetName = true;
            }
        }
        else {
            //update case
            $("#RuleSetList li").each(function () {
                if ($(this).text().trim() == $("#rightRuleSetName").val().trim() && $(this).val() != RuleSetID) {
                    MakeTagFlashable($("#rightRuleSetName"));
                    FlagRuleSetName = false;
                    IsExistRuleSet = true;
                    return false;
                }
                else {
                    RemoveFlashableTag($("#rightRuleSetName"));
                    IsExistRuleSet = false;
                    FlagRuleSetName = true;
                }
            });
            //FlagRuleSetName = true;
            //RemoveFlashableTag($("#rightRuleSetName"));
        }
    }
    //    });

    //$("#rightPanelControl select").each(function () {
    //    if ($(this).val() == null) {
    //        MakeTagFlashable($(this));
    //        FlagRightRentalLength = false;
    //    }
    //    else {
    //        FlagRightRentalLength = true;
    //        RemoveFlashableTag($(this));
    //    }
    //});
    if (RuleSetID == 0) {
        $("#rightRuleSetlengths").each(function () {
            if ($(this).val() == null) {
                MakeTagFlashable($(this));
                FlagRightRentalLength = false;
            }
            else {
                FlagRightRentalLength = true;
                RemoveFlashableTag($(this));
            }
        });
        $("#rightRuleSetCompanies").each(function () {
            if ($(this).val() == null) {
                MakeTagFlashable($(this));
                FlagRightCompanies = false;
            }
            else {
                FlagRightCompanies = true;
                RemoveFlashableTag($(this));
            }
        });
        $("#rightRuleSetCarClass").each(function () {
            if ($(this).val() == null) {
                MakeTagFlashable($(this));
                FlagRightCarClass = false;
            }
            else {
                FlagRightCarClass = true;
                RemoveFlashableTag($(this));
            }
        });
        $("#rightRuleSetDays").each(function () {
            if ($(this).val() == null) {
                MakeTagFlashable($(this));
                FlagRightweek = false;
            }
            else {
                FlagRightweek = true;
                RemoveFlashableTag($(this));
            }
        });
        $("#RuleSetGroupTemplate select").each(function () {
            if ($(this).find("option").length != "0") {
                if ($(this).val() == null) {
                    MakeTagFlashable($(this));
                    FlagRightSelectControl = false;
                }
                else {
                    FlagRightSelectControl = true;
                    RemoveFlashableTag($(this));
                }
            }
            else {
                RemoveFlashableTag($(this));
            }
        });
    }
    else {
        $("#rightRuleSetlengths").each(function () {
            if ($(this).val() == null) {
                MakeTagFlashable($(this));
                FlagRightRentalLength = false;
            }
            else {
                FlagRightRentalLength = true;
                RemoveFlashableTag($(this));
            }
        });
        $("#rightRuleSetCompanies").each(function () {
            if ($(this).val() == null) {
                MakeTagFlashable($(this));
                FlagRightCompanies = false;
            }
            else {
                FlagRightCompanies = true;
                RemoveFlashableTag($(this));
            }
        });
        $("#rightRuleSetCarClass").each(function () {
            if ($(this).val() == null) {
                MakeTagFlashable($(this));
                FlagRightCarClass = false;
            }
            else {
                FlagRightCarClass = true;
                RemoveFlashableTag($(this));
            }
        });
        $("#rightRuleSetDays").each(function () {
            if ($(this).val() == null) {
                MakeTagFlashable($(this));
                FlagRightweek = false;
            }
            else {
                FlagRightweek = true;
                RemoveFlashableTag($(this));
            }
        });
        $("#RuleSetGroupUpdateTemplate select").each(function () {
            if ($(this).find("option").length != "0") {
                if ($(this).val() == null) {
                    MakeTagFlashable($(this));
                    FlagRightSelectControl = false;
                }
                else {
                    FlagRightSelectControl = true;
                    RemoveFlashableTag($(this));
                }
            }
            else {
                RemoveFlashableTag($(this));
            }
        });
    }

    if ($("#rightPanelControl input[type=radio]:checked").length == 0) {
        $("#error-span").show();
        FlagRadio = false;
    }
    else {
        if ($("#rg1:checked").val().trim() == "IsPositionOffsetON") {
            RemoveFlashableTag($("#rightRuleSetCompanyPositionAbvAvg"));
            if ($("#rightRuleSetIsPositionOffset").val().trim() == "") {
                MakeTagFlashable($("#rightRuleSetIsPositionOffset"));
                FlagRadio = false;
            }
            else if (!$.isNumeric($.trim($("#rightRuleSetIsPositionOffset").val()))) {
                MakeTagFlashable($("#rightRuleSetIsPositionOffset"));
                FlagRadio = false;
            }
            else if ($.trim($("#rightRuleSetIsPositionOffset").val()) == 0) {
                MakeTagFlashable($("#rightRuleSetIsPositionOffset"));
                FlagRadio = false;
            }
            else {
                FlagRadio = true;
                RemoveFlashableTag($("#rightRuleSetIsPositionOffset"));
            }
        }
        else if ($("#rg1:checked").val().trim() == "IsPositionOffsetOFF") {
            RemoveFlashableTag($("#rightRuleSetIsPositionOffset"));
            if ($("#rightRuleSetCompanyPositionAbvAvg").val().trim() == "") {
                MakeTagFlashable($("#rightRuleSetCompanyPositionAbvAvg"));
                FlagRadio = false;
            }
            else if (!$.isNumeric($.trim($("#rightRuleSetCompanyPositionAbvAvg").val()))) {
                MakeTagFlashable($("#rightRuleSetCompanyPositionAbvAvg"));
                FlagRadio = false;
            }
            else if ($.trim($("#rightRuleSetCompanyPositionAbvAvg").val()) == 0) {
                MakeTagFlashable($("#rightRuleSetCompanyPositionAbvAvg"));
                FlagRadio = false;
            }
            else {
                FlagRadio = true;
                RemoveFlashableTag($("#rightRuleSetCompanyPositionAbvAvg"));
            }
        }
        $("#error-span").hide();
        //flag = true;
    }
    if ($("#rightRuleSetlocations li").val() == 0) {
        MakeTagFlashable($("#rightRuleSetlocations"));
        FlagRightRuleSetLocation = false;
    }
    else {
        RemoveFlashableTag($("#rightRuleSetlocations"));
        FlagRightRuleSetLocation = true;
    }
    if (RuleSetID == 0) {
        $("#RuleSetGroupTemplate input[type=text]").each(function () {
            if (!$.isNumeric($.trim($(this).val()))) {
                MakeTagFlashable($(this));
                FlagTextBox = false;
                return false;
            }
            else {
                RemoveFlashableTag($(this));
                FlagTextBox = true;
            }
        });
    }
    else {
        $("#RuleSetGroupUpdateTemplate input[type=text]").each(function () {
            if (!$.isNumeric($.trim($(this).val()))) {
                MakeTagFlashable($(this));
                FlagTextBox = false;
                return false;
            }
            else {
                RemoveFlashableTag($(this));
                FlagTextBox = true;
            }
        });
    }

    if (FlagRuleSetName == true && FlagRightSelectControl == true && FlagRadio == true && FlagRightRuleSetLocation == true && FlagTextBox == true && FlagRightRentalLength == true && FlagRightCompanies == true && FlagRightCarClass == true && FlagRightweek == true) {
        Flag = true;
        $("#lblMessage").hide();
    }
    else {
        Flag = false;
        if (IsExistRuleSet) {
            $("#lblMessageCheckRuleSetName").show();
        }
        else {
            $("#lblMessageCheckRuleSetName").hide();
        }
        if ($("#right-col").find(".temp") != "")
            $("#lblMessage").show();
    }
    return Flag;
}
function ValidateRuleSetFilter() {
    var Flag = false;
    $("#searchLeftPanel select").each(function () {
        if ($(this).val() == null) {
            MakeTagFlashable($(this));
            flag = false;
        }
        else {
            flag = true;
            RemoveFlashableTag($(this));
        }
    });
    AddFlashingEffect();
    return Flag;
}
//Remove flag effect while you create and update scenario
function RemoveFlasableEffect() {
    RemoveFlashableTag("#rightRuleSetName");
    RemoveFlashableTag("#rightRuleSetlocations");
    RemoveFlashableTag("#rightRuleSetlengths");
    RemoveFlashableTag("#rightRuleSetCompanies");
    RemoveFlashableTag("#rightRuleSetCarClass");
    RemoveFlashableTag("#rightRuleSetDays");
    RemoveFlashableTag("#rightRuleSetIsPositionOffset");
    RemoveFlashableTag("#rightRuleSetCompanyPositionAbvAvg");
    $("#lblMessage").hide();
}

function BindrentalLengths() {
    $('.loader_container_length').show();
    var ajaxURl = '/RateShopper/RuleSet/GetRentalLengths';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetRentalLengthsUrl;
    }
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        async: true,
        success: function (data) {
            $('.loader_container_length').hide();
            if (data) {
                var rsrcs = $.map(data, function (item) { return new rentalLengths(item); });
                ruleSetViewModel.rentalLengths(rsrcs);
                //$("#recentLengths ul li").eq(0).addClass("selected");
            }
        },
        error: function (e) {
            $('.loader_container_length').hide();
            console.log(e.message);
        }
    });
}
function BindCarClassesRuleSet() {
    $('.loader_container_carclass').show();
    var ajaxURl = "/RateShopper/RuleSet/GetCarClasses/";
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetCarClassesUrl;
    }
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        async: true,
        success: function (data) {
            $('.loader_container_carclass').hide();
            if (data) {
                GlobalRuleSetCarClass = data;
                var srcs = $.map(data, function (item) { return new carClasses(item); });
                ruleSetViewModel.leftCarClasses(srcs);
                ruleSetViewModel.rightCarClasses(srcs);

                //$('#carClasstd select option').bind('click', function () {
                //    if ($('#carClasstd').find('option:selected').length == $('#carClasstd select option').length) {
                //        $('#carClass-all').prop('checked', true);
                //    }
                //    else {
                //        $('#carClass-all').prop('checked', false);
                //    }

                //});
            }
        },
        error: function (e) {
            $('.loader_container_carclass').hide();
            console.log(e.message);
        }
    });
}
function BindCompanies() {
    $('.loader_container_carclass').show();
    var ajaxURl = '/RateShopper/RuleSet/GetCompanies';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetCompaniesUrl;
    }
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        async: true,
        success: function (data) {
            $('.loader_container_length').hide();
            if (data) {
                globalCompanies = data;
                allCompanies = data;
                var rsrcs = $.map(data, function (item) { return new companies(item); });
                ruleSetViewModel.masterCompanies(rsrcs);
                ruleSetViewModel.ruleSetGroupCompanies(rsrcs);
                //$("#recentLengths ul li").eq(0).addClass("selected");

                //only for Firefox doesn't allow drag selection option
                $("#rightRuleSetCompanies").on("change", function () {
                    AutoPopulateCompanies($(this));
                });
            }
        },
        error: function (e) {
            $('.loader_container_length').hide();
            console.log(e.message);
        }
    });
}
function BindBrandLocations() {
    $('.loader_container_location').show();
    var ajaxURl = '/RateShopper/RuleSet/GetBrandLocation';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetBrandLocationUrl;
    }
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        async: true,
        success: function (data) {
            $('.loader_container_location').hide();
            if (data) {
                var rsrcs = $.map(data, function (item) { return new brandLocations(item); });
                ruleSetSmartSearchLocations = data;
                ruleSetViewModel.leftBrandLocations(rsrcs);
                ruleSetViewModel.rightBrandLocations(rsrcs);
            }
        },
        error: function (e) {
            $('.loader_container_location').hide();
            console.log(e.message);
        }
    });
}
function BindWeekDays() {
    $('.loader_container_days').show();
    var ajaxURl = '/RateShopper/RuleSet/GetWeekDays';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetWeekDaysUrl;
    }
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        async: true,
        success: function (data) {
            $('.loader_container_days').hide();
            if (data) {
                var rsrcs = $.map(data, function (item) { return new weekDays(item); });
                ruleSetViewModel.weekDays(rsrcs);
                //$("#recentLengths ul li").eq(0).addClass("selected");
            }
        },
        error: function (e) {
            $('.loader_container_days').hide();
            console.log(e.message);
        }
    });
}
function BindRuleSets() {
    $('.loader_container_ruleset').show();
    var ajaxURl = '/RateShopper/RuleSet/GetRuleSet';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetRuleSetUrl;
    }
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        async: true,
        success: function (data) {
            $('.loader_container_ruleset').hide();
            var rsrcs = $.map(data, function (item) { return new ruleSets(item); });
            ruleSetViewModel.ruleSets(rsrcs);
            //$("#recentLengths ul li").eq(0).addClass("selected");
            $("#RuleSetList li").on("click", function () {
                $(this).addClass("rsselected");
                $(this).siblings("li").removeClass("rsselected");
            });
        },
        error: function (e) {
            $('.loader_container_ruleset').hide();
            console.log(e.message);
        }
    });
}
function BindRuleSetDefaultSetting() {
    $('.loader_container_main').show();
    AddRuleSetSelectID = 0;
    var ajaxURl = '/RateShopper/RuleSet/GetRuleSetDefaultSetting';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetRuleSetDefaultSettingUrl;
    }
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        async: true,
        success: function (data) {
            //$('.loader_container_length').hide();
           // console.log(data);
            var days = $.map(data.lstRuleSetGapSettingDay, function (item) { return new ruleSetDefaultSetting(item); });
            var weeks = $.map(data.lstRuleSetGapSettingWeek, function (item) { return new ruleSetDefaultSetting(item); });
            var months = $.map(data.lstRuleSetGapSettingMonth, function (item) { return new ruleSetDefaultSetting(item); });
            ruleSetViewModel.RuleSetGapSettingsDay(days);
            ruleSetViewModel.RuleSetGapSettingsWeek(weeks);
            ruleSetViewModel.RuleSetGapSettingsMonth(months);
            //$("#recentLengths ul li").eq(0).addClass("selected");
            if (RuleSetID == 0) {
                setTimeout(function () {
                    $("#AddRuleSetTemplate").click();
                    $("#RuleSetGroupTemplate #RuleSetTemplateCompanies").html("");
                    //$("#RuleSetGroupTemplate").removeClass("hidden");
                    //$("#RuleSetDefaultTemplate").clone().appendTo("#RuleSetGroupTemplate");
                    //$("</br>").clone().appendTo("#RuleSetGroupTemplate");
                    //$("#RuleSetGroupTemplate #RuleSetDefaultTemplate").removeClass("DefaultTemplate");

                    //ValidateRuleSetGroupTemplateTextboxEvent();
                    $("#AddRuleSetTemplate").prop('disabled', true).addClass("disable-button");

                    $('.loader_container_main').hide();
                }, 300);

            }
        },
        error: function (e) {
            //$('.loader_container_length').hide();
            console.log(e.message);
        }
    });

}

function getBrandSpecificCompetitors(locationBrandID) {
    competitorIds = "";
    var ajaxURl = '/RateShopper/Location/GetCompetitors/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetCompetitorsUrl;
    }
    if (locationBrandID != undefined && locationBrandID != 0 && !isNaN(locationBrandID)) {
        $.ajax({
            url: ajaxURl,
            data: { locationBrandId: locationBrandID },
            type: 'GET',
            async: true,
            success: function (data) {
                if (data.status) {
                    competitorIds = data.directCompetitors;
                }
                GetLocationSpecificBrandCompanyRuleSetGroupOperation();
            },
            error: function (e) {
                console.log(e.message);
                GetLocationSpecificBrandCompanyRuleSetGroupOperation();
            }
        });
    }
}

$("#isGOV").on("change", function () {
    if ($(this).prop("checked")) {
        if ($('#rightRuleSetIsWideGapTemplate').prop('checked')) {
            $('#rightRuleSetIsWideGapTemplate').prop('checked', false);
        }
        $("input[type=radio]").prop("disabled", false);
        $("#rightRuleSetIsPositionOffset").prop("disabled", false);
        $("#rightRuleSetCompanyPositionAbvAvg").prop("disabled", false);
        $("#rg1").eq(0).prop("checked", true);
        $("#rightRuleSetIsPositionOffset").val(1);
        $("#rightRuleSetCompanyPositionAbvAvg").val('');
    }
    else {
        $("input[type=radio]").prop("disabled", true);
        $("#rightRuleSetIsPositionOffset").prop("disabled", true);
        $("#rightRuleSetCompanyPositionAbvAvg").prop("disabled", true);
        $("#rg1").eq(0).prop("checked", true);
        $("#rightRuleSetIsPositionOffset").val(1);
        $("#rightRuleSetCompanyPositionAbvAvg").val('');
    }
});

//End Functions