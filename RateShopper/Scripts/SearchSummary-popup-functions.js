$(document).ready(function () {
    //$("#ViewApplied_RuleSet").show();
    //setTimeout(RuleSetPopup, 500);
    $("#closerulesetpopup").on("click", function () {
        $("#ViewApplied_RuleSet, .popup_bg").hide();
    });

    $("#ShowGaplink").on("click", function () {
        if ($(this).text().toLowerCase().indexOf('show') < 0) {
            $(this).text('+Show Gaps');
        }
        else {
            $(this).text('-Hide Gaps');
        }

        $("#ShowGapTable").toggle();
    })
});
function RuleSetPopup(data) {
    //$("#rulesetLink").bind("click", function () {
    $("#ShowGapTable").hide();
    $("#ViewApplied_RuleSet").show();
    $("#ViewApplied_RuleSet").show();
    $(".popup_bg").toggle();
    $("#ShowGaplink").text('+Show Gaps');
    //$(window).scrollTop('0');

    var parent = $(data).parent().parent();
    var carClass = parent.find(".carClassLogo").attr("alt");
    var SuggRuleSetBaseRate = parent.find(".baseEdit").attr("suggetedoriginalvalue");
    var SuggRuleSetTotalRate = parent.find(".totalEdit").attr("suggetedoriginalvalue");
    var MinBaseRate = parent.find(".baseEdit").attr("minbaserate");
    var MaxBaseRate = parent.find(".baseEdit").attr("maxbaserate");
    var rulesetCurrentBaseRate = "", rulesetCurrentTotalRate = "";
    if (parent.find("td[companyID=" + brandID + "]").find(".daily-rate").html() != undefined) {
        rulesetCurrentBaseRate = parent.find("td[companyID=" + brandID + "]").find(".daily-rate").html().replace('$', '');
    }
    if (parent.find("td[companyID=" + brandID + "]").find(".tv").html() != undefined) {
        rulesetCurrentTotalRate = parent.find("td[companyID=" + brandID + "]").find(".tv").html().replace('$', '');
    }
    //var rulesetCurrentBaseRate = parent.find("td[companyID=" + brandID + "]").find(".daily-rate").html().replace('$', '');
    //var rulesetCurrentTotalRate = parent.find("td[companyID=" + brandID + "]").find(".tv").html().replace('$', '');

    var locationID = $("#location li").val();
    if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
        if (parseFloat(SuggRuleSetBaseRate).toFixed(2) == parseFloat(rulesetCurrentBaseRate).toFixed(2)) {
            $("#RuleSetStatus").html("ok");
            $("#RuleSetStatus").removeClass("red-txt");
            $("#RuleSetStatus").addClass("green-txt");
        }
        else {
            $("#RuleSetStatus").html("x");
            $("#RuleSetStatus").removeClass("green-txt");
            $("#RuleSetStatus").addClass("red-txt");
        }
    }
    else {
        if (parseFloat(SuggRuleSetTotalRate).toFixed(2) == parseFloat(rulesetCurrentTotalRate).toFixed(2)) {
            $("#RuleSetStatus").html("ok");
            $("#RuleSetStatus").removeClass("red-txt");
            $("#RuleSetStatus").addClass("green-txt");
        }
        else {
            $("#RuleSetStatus").html("x");
            $("#RuleSetStatus").removeClass("green-txt");
            $("#RuleSetStatus").addClass("red-txt");
        }
    }

    //Minrate
    if (parseFloat(MinBaseRate) == parseFloat(SuggRuleSetBaseRate).toFixed(2) || parseFloat(MinBaseRate) <= parseFloat(SuggRuleSetBaseRate).toFixed(2) && MinBaseRate != undefined) {
        $("#RuleSetStatusMin").html("ok");
        $("#RuleSetStatusMin").removeClass("red-txt");
        $("#RuleSetStatusMin").addClass("green-txt");
    }
    else {
        $("#RuleSetStatusMin").html("x");
        $("#RuleSetStatusMin").removeClass("green-txt");
        $("#RuleSetStatusMin").addClass("red-txt");
    }
    //Maxrate
    if (parseFloat(MaxBaseRate) == parseFloat(SuggRuleSetBaseRate).toFixed(2) || parseFloat(MaxBaseRate) >= parseFloat(SuggRuleSetBaseRate).toFixed(2) && MaxBaseRate != undefined) {
        $("#RuleSetStatusMax").html("ok");
        $("#RuleSetStatusMax").removeClass("red-txt");
        $("#RuleSetStatusMax").addClass("green-txt");
    }
    else {
        $("#RuleSetStatusMax").html("x");
        $("#RuleSetStatusMax").removeClass("green-txt");
        $("#RuleSetStatusMax").addClass("red-txt");
    }

    (rulesetCurrentBaseRate == "--" || rulesetCurrentBaseRate == "") ? rulesetCurrentBaseRate = "N/A" : rulesetCurrentBaseRate;
    (rulesetCurrentTotalRate == "--" || rulesetCurrentTotalRate == "") ? rulesetCurrentTotalRate = "N/A" : rulesetCurrentTotalRate;
    if (MinBaseRate != undefined) {
        $("#RuleSetMinRate").html(MinBaseRate);
    }
    else {
        $("#RuleSetMinRate").html("N/A");
    }
    if (MaxBaseRate != undefined) {
        $("#RuleSetMaxRate").html(MaxBaseRate);
    }
    else {
        $("#RuleSetMaxRate").html("N/A");
    }
    $("#RuleSetTotalMinRate").html(SuggRuleSetBaseRate);
    $("#RuleSetTotalMaxRate").html(SuggRuleSetBaseRate);
    $("#RuleSetTotalMinRateChange").html(SuggRuleSetBaseRate);
    $("#RuleSetTotalMaxRateChange").html(SuggRuleSetBaseRate);

    //var displayDate = $("#displayDay li").attr("value");
    //$("#RuleSetDate").html(displayDate.substr(0, 4) + "-" + displayDate.substr(4, 2) + "-" + ('0' + (displayDate.substr(6, displayDate.length - 1))).slice(-2));

    $("#RuleSetRentalLength").html($("#length li").html());
    $("#RuleSetLocation").html($("#location li").html());
    $("#RuleSetSource").html($("#dimension-source li").html());
    $("#SuggestedBaseRate").html(commaSeparateNumber(SuggRuleSetBaseRate));
    $("#SuggestedTotalRate").html(commaSeparateNumber(SuggRuleSetTotalRate));

    $("#RuleSetCurrentBaseRate").html(commaSeparateNumber(rulesetCurrentBaseRate));
    $("#RuleSetCurrentTotalRate").html(commaSeparateNumber(rulesetCurrentTotalRate));

    if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
        $("#SuggestedTotalRateRules").html(commaSeparateNumber(parseFloat(SuggRuleSetBaseRate).toFixed(2)));
        $("#RuleSetCurrentTotalRateRules").html(commaSeparateNumber(rulesetCurrentBaseRate));
    }
    else {
        $("#SuggestedTotalRateRules").html(commaSeparateNumber(SuggRuleSetTotalRate));
        $("#RuleSetCurrentTotalRateRules").html(commaSeparateNumber(rulesetCurrentTotalRate));
    }

    $("#RuleSetBaseRateDiff").html((rulesetCurrentBaseRate == "N/A") ? "N/A" : commaSeparateNumber((parseFloat(SuggRuleSetBaseRate) - parseFloat(rulesetCurrentBaseRate)).toFixed(2)));
    $("#RuleSetTotalRateDiff").html((rulesetCurrentTotalRate == "N/A") ? "N/A" : commaSeparateNumber((parseFloat(SuggRuleSetTotalRate) - parseFloat(rulesetCurrentTotalRate)).toFixed(2)));

    var CompetitorYellowTotalRate = 0, CompetitorYellowBaseRate = 0;
    var CompetitorGreenTotalRate = 0, CompetitorGreenBaseRate = 0;
    var CompetitorYellowCompanyID = 0;
    var CompetitorGreenCompanyID = 0;

    var thisTr = ($(data).parent().parent());

    if ($('#viewSelect .selected').attr('value') != 'classic') {
        $("#RuleSetCarClass").html(thisTr.find(".carClassLogo").attr("alt"));

        var displayDate = $("#displayDay li").attr("value");
        $("#RuleSetDate").html(displayDate.substr(0, 4) + "-" + displayDate.substr(4, 2) + "-" + ('0' + (displayDate.substr(6, displayDate.length - 1))).slice(-2));

        if (thisTr.find(".highlight").html() != undefined) {
            CompetitorYellowTotalRate = commaRemovedNumber(thisTr.find(".highlight").eq(0).html().replace('$', ''));
            CompetitorYellowBaseRate = commaRemovedNumber(thisTr.find(".highlight").parent().find(".daily-rate").html().replace('$', ''));
            //for GOV rate calculation to get all competitor check
            if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                var YellowCompanyId = "";
                if (thisTr.find(".highlight").length > 0) {
                    thisTr.find(".highlight").each(function () {
                        YellowCompanyId += $(this).parent().attr("companyid") + ",";
                    });
                    CompetitorYellowCompanyID = YellowCompanyId.trim().substring(0, YellowCompanyId.trim().length - 1);
                }
            }
            else {
                CompetitorYellowCompanyID = thisTr.find(".highlight").eq(0).parent().attr("companyid");
            }

        }
        if (thisTr.find(".highlightGreen").html() != undefined) {
            CompetitorGreenTotalRate = commaRemovedNumber(thisTr.find(".highlightGreen").eq(0).html().replace('$', ''));
            CompetitorGreenBaseRate = commaRemovedNumber(thisTr.find(".highlightGreen").parent().find(".daily-rate").html().replace('$', ''));
            //for GOV rate calculation to get all competitor check
            if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                var GreenCompanyId = "";
                if (thisTr.find(".highlightGreen").length > 0) {
                    thisTr.find(".highlightGreen").each(function () {
                        GreenCompanyId += $(this).parent().attr("companyid") + ",";
                    });
                    CompetitorGreenCompanyID = GreenCompanyId.trim().substring(0, GreenCompanyId.trim().length - 1);
                }
            }
            else {
                CompetitorGreenCompanyID = thisTr.find(".highlightGreen").eq(0).parent().attr("companyid");
            }
            
        }
    }
    else {
        var displayDate = thisTr.find("td").eq(0).attr("formatdate");
        $("#RuleSetDate").html(displayDate.substr(0, 4) + "-" + displayDate.substr(4, 2) + "-" + ('0' + (displayDate.substr(6, displayDate.length - 1))).slice(-2));

        $("#RuleSetCarClass").html($("#carClass li.selected").text());
        var lowestRateProvider = 0;
        var lowestRateCompetitor = 0;
        if (thisTr.find("td").eq(5).find('.tv').html() != undefined) {
            lowestRateProvider = thisTr.find("td").eq(5);
            CompetitorYellowTotalRate = commaRemovedNumber(lowestRateProvider.find('.tv').html());
            CompetitorYellowCompanyID = lowestRateProvider.attr("companyid");
            CompetitorYellowBaseRate = lowestRateProvider.find('.tv').parent().find(".daily-rate").html();
        }

        if (thisTr.find("td span span.base-rate-blue").closest('td').closest('td').find('.tv').html() != undefined) {
            lowestRateCompetitor = thisTr.find("td span span.base-rate-blue").closest('td');
            CompetitorGreenTotalRate = commaRemovedNumber(lowestRateCompetitor.find('.tv').html());
            CompetitorGreenCompanyID = lowestRateCompetitor.attr("companyid");
            CompetitorGreenBaseRate = lowestRateCompetitor.find('.tv').parent().find(".base-rate-blue").html();
        }
    }

    var RuleSetID = $(data).parent().find(".ruleSetLink").attr("RuleSetID");

    if (RuleSetID != undefined) {
        $('.loader_container_main').show();
        //Display selected ruleset hyperlink caption
        $("#RuleSetTemplateName").html($(data).parent().find(".ruleSetLink").attr("value"));
        //get the data which is need to display relevant selected rulseset id
        $.ajax({
            url: 'Search/SearchViewAppliedRuleSet',
            type: 'GET',
            data: { ID: RuleSetID },
            async: true,
            dataType: 'json',
            success: function (data) {
                $('.loader_container_main').hide();
                $("#RuleSetCarClassSpan").html(data.CarClassCode);
                $("#RuleSetRentalLengthSpan").html(data.RentalLengthName);
                $("#RuleSetDayOfWeekSpan").html(data.DaysOfWeek);
                var srcs = $.map(data.lstCompany, function (item) { return new RuleSetCompanyList(item); });
                searchViewModel.RuleSetCompanies(srcs);

                var srcs = $.map(data.lstRuleSetGroupCustom, function (item) { return new RuleSetGroup(item); });
                searchViewModel.RuleSetGroups(srcs);

                //After render Company list and check the companyID which is compitetor or not that based we get that total amount
                setTimeout(function () {
                    var competitorCheck = 0;
                    //Note: First Check competitor in yellow indicate company if not then go to next competitor which is green indecator

                    if (CompetitorYellowTotalRate != 0) {
                        $(".RuleSetlstCompany").each(function () {
                            //Check multiple compitetor are there and in between whichever competitor fall in ruleset company that company rate should go for calculcation.
                            if ($.inArray($(this).attr("companyID").toString(), CompetitorYellowCompanyID.split(',')) != -1) {
                            //Before GOV rate calcualtion this condition used
                            //if (CompetitorYellowCompanyID == $(this).attr("companyID")) {
                                competitorCheck = 1;
                                var GapAmount = "";
                                //this condition is used for GOV rate.
                                if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                                    $("#RulesetLowestGapRateType").html("(Base)");
                                    var tempGapAmount = GetGapAmount(CompetitorYellowBaseRate, $(this).attr("companyID"), $("#length li").html());
                                    GapAmount = commaSeparateNumber(parseFloat(tempGapAmount).toFixed(2));
                                    //GapAmount = commaSeparateNumber(((parseFloat(CompetitorYellowBaseRate) - parseFloat(SuggRuleSetBaseRate)).toFixed(2)));

                                    if (GapAmount.substr(0, 1) != "-") {
                                        $("#RuleSetLowestCompanyGap").html(commaSeparateNumber(CompetitorYellowBaseRate) + " - " + GapAmount + " = " + commaSeparateNumber(SuggRuleSetBaseRate));
                                        $("#RuleSetLowestGapTitle").html("-");
                                    }
                                    else {
                                        $("#RuleSetLowestCompanyGap").html(commaSeparateNumber(CompetitorYellowBaseRate) + " + " + GapAmount.substr(1, GapAmount.length) + " = " + commaSeparateNumber(SuggRuleSetBaseRate));
                                        $("#RuleSetLowestGapTitle").html("+");
                                    }
                                }
                                else {
                                    $("#RulesetLowestGapRateType").html("(Total)");
                                    GapAmount = commaSeparateNumber(((parseFloat(CompetitorYellowTotalRate) - parseFloat(SuggRuleSetTotalRate)).toFixed(2)));
                                    if (GapAmount.substr(0, 1) != "-") {
                                        $("#RuleSetLowestCompanyGap").html(commaSeparateNumber(CompetitorYellowTotalRate) + " - " + GapAmount + " = " + commaSeparateNumber(SuggRuleSetTotalRate));
                                        $("#RuleSetLowestGapTitle").html("-");
                                    }
                                    else {
                                        $("#RuleSetLowestCompanyGap").html(commaSeparateNumber(CompetitorYellowTotalRate) + " + " + GapAmount.substr(1, GapAmount.length) + " = " + commaSeparateNumber(SuggRuleSetTotalRate));
                                        $("#RuleSetLowestGapTitle").html("+");
                                    }
                                }

                                return false;
                            }
                            else {
                                competitorCheck = 0;
                            }
                        });
                    }
                    else {
                        $("#RuleSetLowestCompanyGap").html("N/A");
                    }

                    //Check second  company competitor
                    if (CompetitorGreenTotalRate != 0) {
                        if (competitorCheck == 0) {
                            $(".RuleSetlstCompany").each(function () {
                                //Check multiple compitetor are there and in between whichever competitor fall in ruleset company that company rate should go for calculcation.

                                if ($.inArray($(this).attr("companyID").toString(), CompetitorGreenCompanyID.split(',')) != -1) {
                                    //Before GOV rate calcualtion this condition used
                                //if (CompetitorGreenCompanyID == $(this).attr("companyID")) {
                                    competitorCheck = 1;
                                    var GapAmount = "";
                                    if (GlobalLimitSearchSummaryData.IsGOV != null && GlobalLimitSearchSummaryData.IsGOV) {
                                        $("#RulesetLowestGapRateType").html("(Base)");

                                        var tempGapAmount = GetGapAmount(CompetitorGreenBaseRate, $(this).attr("companyID"), $("#length li").html());
                                        GapAmount = commaSeparateNumber(parseFloat(tempGapAmount).toFixed(2));

                                        //GapAmount = commaSeparateNumber(((parseFloat(CompetitorGreenBaseRate) - parseFloat(SuggRuleSetBaseRate)).toFixed(2)));
                                        if (GapAmount.substr(0, 1) != "-") {
                                            $("#RuleSetLowestCompanyGap").html(commaSeparateNumber(CompetitorGreenBaseRate) + " - " + GapAmount + " = " + commaSeparateNumber(SuggRuleSetBaseRate));
                                            $("#RuleSetLowestGapTitle").html("-");
                                        }
                                        else {
                                            $("#RuleSetLowestCompanyGap").html(commaSeparateNumber(CompetitorGreenBaseRate) + " + " + GapAmount.substr(1, GapAmount.length) + " = " + commaSeparateNumber(SuggRuleSetBaseRate));
                                            $("#RuleSetLowestGapTitle").html("+");
                                        }
                                    }
                                    else {
                                        $("#RulesetLowestGapRateType").html("(Total)");
                                        GapAmount = commaSeparateNumber(((parseFloat(CompetitorGreenTotalRate) - parseFloat(SuggRuleSetTotalRate)).toFixed(2)));
                                        if (GapAmount.substr(0, 1) != "-") {
                                            $("#RuleSetLowestCompanyGap").html(commaSeparateNumber(CompetitorGreenTotalRate) + " - " + GapAmount + " = " + commaSeparateNumber(SuggRuleSetTotalRate));
                                            $("#RuleSetLowestGapTitle").html("-");
                                        }
                                        else {
                                            $("#RuleSetLowestCompanyGap").html(commaSeparateNumber(CompetitorGreenTotalRate) + " + " + GapAmount.substr(1, GapAmount.length) + " = " + commaSeparateNumber(SuggRuleSetTotalRate));
                                            $("#RuleSetLowestGapTitle").html("+");
                                        }
                                    }
                                    return false;
                                }
                                else {
                                    competitorCheck = 0;
                                }
                            });
                        }
                    }
                    else {
                        $("#RuleSetLowestCompanyGap").html("N/A");
                    }
                }, 250);


            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                //alert(errorThrown);
            }
        });

    }
    //})
}
function GetGapAmount(suggestedRate, companyId, rentalLength) {
    var FinalGapAmount = "";
    var IsFound = false;//Used for break or exit the loop
    $("#ShowGapTable #RuleSetGroup").each(function () {
        var CurrentGroup = $(this);
        if ($.inArray(companyId.toString(), $(CurrentGroup).find("input:hidden").val().split(',')) != -1) {
            var charRentalLength = rentalLength.trim().substr(0, 1)
            //console.log("Match company ID " + companyId + " " + $(CurrentGroup).find("input:hidden").val() + " Group Id " + $(CurrentGroup).find("span").html());
            if (charRentalLength == "D" || charRentalLength == "d") {
                $(CurrentGroup).find("table[id=daystable]").each(function () {
                    $(CurrentGroup).find("tr").each(function () {
                        var MinAmount = parseFloat($(this).find("td[id=MinAmount]").html());
                        var GapAmount = parseFloat($(this).find("td[id=GapAmount]").html());
                        var MaxAmount = parseFloat($(this).find("td[id=MaxAmount]").html());
                        if (MinAmount <= parseFloat(suggestedRate) && MaxAmount >= parseFloat(suggestedRate)) {
                            FinalGapAmount = GapAmount;
                            IsFound = true;
                            return false;
                        }
                    });
                });
               
            }
            else if (charRentalLength == "W" || charRentalLength == "w") {
                $(CurrentGroup).find("table[id=weekstable]").each(function () {
                    $(CurrentGroup).find("tr").each(function () {
                        var MinAmount = parseFloat($(this).find("td[id=MinAmount]").html());
                        var GapAmount = parseFloat($(this).find("td[id=GapAmount]").html());
                        var MaxAmount = parseFloat($(this).find("td[id=MaxAmount]").html());
                        if (MinAmount <= parseFloat(suggestedRate) && MaxAmount >= parseFloat(suggestedRate)) {
                            FinalGapAmount = GapAmount;
                            IsFound = true;
                            return false;
                        }
                    });
                });             
            }
            else if (charRentalLength == "M" || charRentalLength == "m") {
                $(CurrentGroup).find("table[id=monthstable]").each(function () {
                    $(CurrentGroup).find("tr").each(function () {
                        var MinAmount = parseFloat($(this).find("td[id=MinAmount]").html());
                        var GapAmount = parseFloat($(this).find("td[id=GapAmount]").html());
                        var MaxAmount = parseFloat($(this).find("td[id=MaxAmount]").html());
                        if (MinAmount <= parseFloat(suggestedRate) && MaxAmount >= parseFloat(suggestedRate)) {
                            FinalGapAmount = GapAmount;
                            IsFound = true;
                            return false;
                        }
                    });
                });
            }
            if (IsFound) {
                return false;
            }
        }
        if (IsFound) {
            return false;
        }
    });
    return parseFloat(FinalGapAmount).toFixed(2);
}
//Bind span in looping structure
function RuleSetCompanyList(data) {
    this.ID = data.ID;
    this.Code = data.Code;
}

function RuleSetGroup(data) {
    this.GroupID = data.ID;
    this.GroupName = data.GroupName;
    this.CompanyId = data.CompanyID;
    this.CompanyName = data.CompanyName;
    this.RuleSetGapSettingsDay = ko.observableArray($.map(data.lstRuleSetGapSettingDay, function (item) { return new RuleSetGapSetting(item); }));
    this.RuleSetGapSettingsWeek = ko.observableArray($.map(data.lstRuleSetGapSettingWeek, function (item) { return new RuleSetGapSetting(item); }));
    this.RuleSetGapSettingsMonth = ko.observableArray($.map(data.lstRuleSetGapSettingMonth, function (item) { return new RuleSetGapSetting(item); }));
}
function RuleSetGapSetting(data) {
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
}
