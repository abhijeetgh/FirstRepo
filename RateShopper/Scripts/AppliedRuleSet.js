
var appliedRuleSetsModel;
$(document).ready(function () {

    appliedRuleSetsModel = new AppliedRuleSetsModel();
    ko.applyBindings(appliedRuleSetsModel);

    $('.collapse').eq(0).css('cursor', 'pointer').click(function () {
        if ($(this).attr('src').indexOf('expand') > 0) {
            $(this).attr('src', $(this).attr('src').replace('expand', 'Search-collapse'));
        }
        else {
            $(this).attr('src', $(this).attr('src').replace('Search-collapse', 'expand'));
        }

        $('#topSection').slideToggle();
    });

    $('.collapse').eq(1).css('cursor', 'pointer').click(function () {
        if ($(this).attr('src').indexOf('expand') > 0) {
            $(this).attr('src', $(this).attr('src').replace('expand', 'Search-collapse'));
        }
        else {
            $(this).attr('src', $(this).attr('src').replace('Search-collapse', 'expand'));
        }

        $('.padding15').slideToggle();
    });


    $('#Location ul li').click(function () {
        $('#bottomSection, #appliedRuleSetGrid').show();
        var locationBrandId = $(this).attr('value').toString();
        GetAppliedRuleSets(locationBrandId, false);
        GetAppliedRuleSetDetails(locationBrandId);
        RemoveFlashableTag($('.arsearch'));
        $('#divNoTemplate').hide();
        $('.arsearch').removeAttr('readonly').val('');
    });

    $('.arsearch').bind('input', function () {
        if ($(this).val().length > 0) {

            $('.mtb15').each(function () {
                if ($(this).find('h3 span').eq(0).text().trim().toLowerCase().indexOf($('.arsearch').val().toLowerCase()) >= 0) {
                    $(this).show();
                }
                else {
                    $(this).hide();
                }
            });
        } else {
            $('.mtb15').show();
        }

        if ($(".mtb15[style$='display: none;']").length == $('.mtb15').length) {
            MakeTagFlashable($('.arsearch'));
            $('#divNoTemplate').show();
        }
        else {
            RemoveFlashableTag($('.arsearch'));
            $('#divNoTemplate').hide();
        }

        AddFlashingEffect();
    })
});

//Get Applied Rule Sets
function GetAppliedRuleSets(locationBrandId, hideSpinner) {

    $('.loader_container_main').show();

    var ajaxURl = '/RateShopper/RuleSet/GetAppliedRuleSets';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.AppliedRuleSetUrl;
    }

    $.ajax({
        url: ajaxURl,
        type: 'GET',
        async: true,
        data: { locationBrandId: locationBrandId },
        success: function (data) {
            var appliedRuleSets = $.map(data, function (item) { return new AppliedRuleSet(item); });
            appliedRuleSetsModel.appliedRuleSets(appliedRuleSets);

            if (data.length == 0) {
                $('#divNoAppliedRuleSet, #AppliedRuleSetsNotFound').show();
                $('#AppliedRuleSetsFound').hide();
            }
            else {
                $('#divNoAppliedRuleSet, #AppliedRuleSetsNotFound').hide();
                $('#AppliedRuleSetsFound').show();
            }

            $('[Id^="Deactivate"]').css('cursor', 'pointer').on('click', function () {
                ActivateDeactivateRuleSet($(this).attr('id').split('_')[1], false);
            });

            $('[Id^="Activate"]').css('cursor', 'pointer').on('click', function () {
                ActivateDeactivateRuleSet($(this).attr('id').split('_')[1], true);
            });

            $('[Id^="delete"]').css('cursor', 'pointer').on('click', function () {
                var rulesetId = $(this).attr('id').split('_')[1];
                var RuleSetName = $('#RuleSetName_' + rulesetId).text();
                var message = 'Do you want to remove the <b>' + RuleSetName.toUpperCase() + '</b> from the list of Applied Rule Set? ';
                ShowConfirmBox(message, true, DeleteRuleSet, rulesetId);
            });

            if (hideSpinner) {
                $('.loader_container_main').hide();
            }

        },
        error: function (e) {
            //called when there is an error
            console.log("GetAppliedRuleSets: " + e.message);
            $('.loader_container_main').hide();
        }
    });
}

//Get Applied Rule Sets
function GetAppliedRuleSetDetails(locationBrandId) {

    $('.loader_container_main').show();

    var ajaxURl = '/RateShopper/RuleSet/GetAppliedRuleSetDetails';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.AppliedRuleSetDetails;
    }

    $.ajax({
        url: ajaxURl,
        type: 'GET',
        async: true,
        data: { locationBrandId: locationBrandId },
        success: function (data) {
            var appliedRuleSetDetails = $.map(data, function (item) { return new RuleSetDetail(item); });
            appliedRuleSetsModel.ruleSetDetails(appliedRuleSetDetails);

            if (appliedRuleSetDetails.length == 0) {
                $('#divNoShowAllTemplates').show();
                $('.arsearch').attr('readonly', 'readonly');
            }
            else {
                $('#divNoShowAllTemplates').hide();
            }

            $('input[id^=arrStartDate]').datepicker({
                minDate: 0,
                dateFormat: 'mm/dd/yy',
                numberOfMonths: 4,
                onSelect: function (selectedDate, instance) {
                    $(this).closest('.padding15').find('input[id^=arrEndDate]').datepicker('option', { defaultDate: selectedDate, minDate: selectedDate });
                    RemoveFlashableTag($(this));
                    AddFlashingEffect();
                }
            });

            $('input[id^=arrEndDate]').datepicker({
                numberOfMonths: 4,
                dateFormat: 'mm/dd/yy',
                onSelect: function (selectedDate, instance) {
                    RemoveFlashableTag($(this));
                    AddFlashingEffect();
                }
            });

            $('.loader_container_main').hide();

            $('[id^=applyRuleSet_]').on('click', function () {

                RemoveFlashableTag($('[id^=arrStartDate_]'));
                RemoveFlashableTag($('[id^=arrEndDate_]'));
                AddFlashingEffect();

                var ruleSetId = $(this).attr('id').split('_')[1];
                if (ruleSetId != undefined) {
                    ApplyNewRuleSet(ruleSetId);
                }

            });
        },
        error: function (e) {
            //called when there is an error
            console.log("GetAppliedRuleSetDetails: " + e.message);
            $('.loader_container_main').hide();
        }
    });
}

function ApplyNewRuleSet(ruleSetId) {

    if ($.trim($('#arrStartDate_' + ruleSetId).val()) == "" || $.trim($('#arrStartDate_' + ruleSetId).val()) == 'mm/dd/yyyy') {
        MakeTagFlashable($('#arrStartDate_' + ruleSetId));
        AddFlashingEffect();
    } else {
        RemoveFlashableTag($('#arrStartDate_' + ruleSetId));
        AddFlashingEffect();
    }

    if ($.trim($('#arrEndDate_' + ruleSetId).val()) == "" || $.trim($('#arrEndDate_' + ruleSetId).val()) == 'mm/dd/yyyy') {
        MakeTagFlashable($('#arrEndDate_' + ruleSetId));
        AddFlashingEffect();
    } else {
        RemoveFlashableTag($('#arrEndDate_' + ruleSetId));
        AddFlashingEffect();
    }

    if ($('div[id="' + ruleSetId + '"] .temp').length > 0) {
        return false;
    } else {

        $('.loader_container_main').show();

        var ajaxURl = '/RateShopper/RuleSet/ApplyNewRuleSet';
        if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
            ajaxURl = AjaxURLSettings.ApplyNewRuleSet;
        }

        var objRuleSet = new Object();
        objRuleSet.loggedInUserId = $('#LoggedInUserId').val();
        objRuleSet.startDate = $('#arrStartDate_' + ruleSetId).val();
        objRuleSet.endDate = $('#arrEndDate_' + ruleSetId).val();
        objRuleSet.rulesetId = ruleSetId.toString();


        $.ajax({
            url: ajaxURl,
            type: 'POST',
            async: true,
            data: objRuleSet,
            success: function (data) {
                $('.loader_container_main').hide();
                var locationBrandId = $('#Location li').eq(0).attr('value');
                $('#arrStartDate_' + ruleSetId + ', #arrEndDate_' + ruleSetId).val('mm/dd/yyyy');
                GetAppliedRuleSets(locationBrandId, true);
                $(document).scrollTop(0);
            },
            error: function (e) {
                //called when there is an error
                console.log("ApplyNewRuleSet: " + e.message);
                $('.loader_container_main').hide();
            }
        });
    }

}

function ActivateDeactivateRuleSet(rulesetId, activate) {

    $('.loader_container_main').show();
    var loggedInUserId = $('#LoggedInUserId').val();

    var ajaxURl = '/RateShopper/RuleSet/ActivateDeactivateRuleSet';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.ActivateDeactivateRuleSetUrl;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        async: true,
        data: { rulesetId: rulesetId.toString(), Activate: activate, loggedInUserId: loggedInUserId },
        success: function (data) {
            $('.loader_container_main').hide();
            if (activate) {
                $('#Activate_' + rulesetId).hide();
                $('#Deactivate_' + rulesetId).show();
                $('tr[id="' + rulesetId + '"] td').eq(0).text("ACTIVE").removeAttr('value', 'false');
            }
            else {
                $('#Activate_' + rulesetId).show();
                $('#Deactivate_' + rulesetId).hide();
                $('tr[id="' + rulesetId + '"] td').eq(0).text("INACTIVE").attr('value', 'true');
            }

            $.each(appliedRuleSetsModel.appliedRuleSets(), function (index, value) {
                if (value.Id == rulesetId) {
                    value.NotActive = activate ? false : true;
                }
            })

        },
        error: function (e) {
            //called when there is an error
            console.log("ActivateDeactivateRuleSet: " + e.message);
            $('.loader_container_main').hide();
        }
    });
}

function DeleteRuleSet() {

    rulesetId = this;
    $('.loader_container_main').show();
    var loggedInUserId = $('#LoggedInUserId').val();

    var ajaxURl = '/RateShopper/RuleSet/DeleteRuleSet';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.DeleteRuleSet;
    }

    $.ajax({
        url: ajaxURl,
        type: 'POST',
        async: true,
        data: { rulesetId: rulesetId.toString(), loggedInUserId: loggedInUserId },
        success: function (data) {
            $('tr[id="' + rulesetId + '"]').hide();
            $('.loader_container_main').hide();
            if ($('#appliedRuleSetGrid table tbody tr[style$="display: none;"]').length == $('#appliedRuleSetGrid table tbody tr').length) {
                $('#divNoAppliedRuleSet, #AppliedRuleSetsNotFound').show();
                $('#AppliedRuleSetsFound').hide();
            } else {
                $('#divNoAppliedRuleSet, #AppliedRuleSetsNotFound').hide();
                $('#AppliedRuleSetsFound').show();
            }
        },
        error: function (e) {
            //called when there is an error
            console.log("DeleteRuleSet: " + e.message);
            $('.loader_container_main').hide();
        }
    });
}

function AppliedRuleSetsModel() {
    var self = this;
    self.appliedRuleSets = ko.observableArray([]);
    self.ruleSetDetails = ko.observableArray([]);
    self.headers = [
        { title: 'STATUS', sortPropertyName: 'NotActive', asc: true },
        { title: 'RULE SET APPLIED', sortPropertyName: 'Name', asc: true },
        { title: 'DATE RANGE', sortPropertyName: 'StartDate', asc: true }
    ];

    self.sort = function (header, event) {
        //if this header was just clicked a second time
        if (self.activeSort === header) {
            header.asc = !header.asc; //toggle the direction of the sort
        } else {
            self.activeSort = header; //first click, remember it
        }
        var prop = self.activeSort.sortPropertyName;

        var ascSort = function (a, b) { return a[prop] < b[prop] ? -1 : a[prop] > b[prop] ? 1 : a[prop] == b[prop] ? 0 : 0; };
        var descSort = function (a, b) { return a[prop] > b[prop] ? -1 : a[prop] < b[prop] ? 1 : a[prop] == b[prop] ? 0 : 0; };

        if (prop.toUpperCase() == 'NAME') {
            ascSort = function (a, b) {
                return a.Name().toUpperCase() < b.Name().toUpperCase() ? -1 : a.Name().toUpperCase() > b.Name().toUpperCase() ? 1 : a.Name().toUpperCase() == b.Name().toUpperCase() ? 0 : 0;
            };

            descSort = function (a, b) {
                return a.Name().toUpperCase() > b.Name().toUpperCase() ? -1 : a.Name().toUpperCase() < b.Name().toUpperCase() ? 1 : a.Name().toUpperCase() == b.Name().toUpperCase() ? 0 : 0;
            };
        }

        var sortFunc;
        if (self.activeSort.asc) {
            sortFunc = ascSort;
            $('td[value="' + self.activeSort.sortPropertyName + '"] .aru').show();
            $('td[value="' + self.activeSort.sortPropertyName + '"] .ard').hide();
        }
        else {
            sortFunc = descSort;
            $('td[value="' + self.activeSort.sortPropertyName + '"] .aru').hide();
            $('td[value="' + self.activeSort.sortPropertyName + '"] .ard').show();
        }
        self.appliedRuleSets.sort(sortFunc);
    };
}

function AppliedRuleSet(data) {
    console.log(data);
    this.Id = data.Id;
    this.Name = ko.computed(function () {
        if (data.IsWideGap) {
            return data.Name + ' (Automation)';
        }
        else if (data.IsGOV) {
            return data.Name + ' (GOV)';
        }
        else {
            return data.Name;
        }
        data.Name
    });
    this.StatusText = ko.computed(function () {
        if (data.Status) {
            return "ACTIVE";
        }
        else {
            return "INACTIVE";
        }
    });

    this.NotActive = !(data.Status);

    this.StartEndDate = ko.computed(function () {
        var startDate = convertToServerTime(new Date(parseInt(data.StartDate.replace("/Date(", "").replace(")/", ""))));
        var endDate = convertToServerTime(new Date(parseInt(data.EndDate.replace("/Date(", "").replace(")/", ""))));
        return startDate.getFullYear() + "/" + ('0' + (startDate.getMonth() + 1)).slice(-2) + "/" + ('0' + (startDate.getDate())).slice(-2) + " through " + endDate.getFullYear() + "/" + ('0' + (endDate.getMonth() + 1)).slice(-2) + "/" + ('0' + (endDate.getDate())).slice(-2);
    });

    this.StartDate = data.StartDate;

    this.ActivateImgId = ko.computed(function () {
        return "Activate_" + data.Id;
    });

    this.DeactivateImgId = ko.computed(function () {
        return "Deactivate_" + data.Id;
    });

    this.DeleteImgId = ko.computed(function () {
        return "delete_" + data.Id;
    });

    this.RuleSetNameTdId = ko.computed(function () {
        return "RuleSetName_" + data.Id;
    });

    this.WideGapClass = ko.computed(function () {
        if (data.IsWideGap) {
            return 'IsWideGapTemplate';
        }
        else if (data.IsGOV) {
            return 'IsGOVTemplate';
        }
        else {
            return 'IsNormalTemplate';
        }
    });
}

function RuleSetDetail(data) {
    this.Id = data.Id;
    this.Name = ko.computed(function () {
        if (data.IsWideGap) {
            return data.Name + ' (Automation)';
        }
        else if (data.IsGOV) {
            return data.Name + ' (GOV)';
        }
        else {
            return data.Name;
        }
        data.Name
    });
    this.Companies = data.Companies;
    //this.CarClasses = data.CarClasses;
    this.CarClasses = computeCarClasses(data.CarClasses, 'carClass');
    this.RentalLengths = data.RentalLengths;
    this.DaysOfWeeks = data.DaysOfWeek;
    this.WideGapClass = ko.computed(function () {
        if (data.IsWideGap) {
            return 'IsWideGapTemplate padding15 record';
        }
        else if (data.IsGOV) {
            return 'IsGOVTemplate padding15 record';
        }
        else {
            return 'padding15 record';
        }
    });
    this.startCalendarId = ko.computed(function () {
        return 'arrStartDate_' + data.Id;
    });
    this.endCalendarId = ko.computed(function () {
        return 'arrEndDate_' + data.Id;
    });
    this.applyButtonId = ko.computed(function () {
        return 'applyRuleSet_' + data.Id;
    });
}
//Order car Classes based on hidden select list 
//hidden select list has sorted car classes 
function computeCarClasses(Ids, controlID) {
    var tempName = "";
    if (controlID == "carClass") {
        $("#" + controlID + " option").each(function () {
            if ($.inArray($(this).attr('value'), Ids.split(',')) != -1) {
                tempName += $(this).text() + ", ";
            }
        });
    }
    return tempName.trim().substring(0, tempName.trim().length - 1);
}
