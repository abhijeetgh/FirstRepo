
var SelectedLocationId;
var allTextBoxesEmpty = true;
$(document).ready(function () {

    globalTetheringModel = new GlobalTetheringModel();
    ko.applyBindings(globalTetheringModel);

    BindCarClasses();
    BindExistingTetheredLocations();

    $('.hidden').eq(0).find('li').eq(1).addClass('selected').closest('.dropdown').find('li').eq(0).text($('.hidden').eq(0).find('li').eq(1).text()).attr('value', ($('.hidden').eq(0).find('li').eq(1).attr('value')));



    $('.hidden').eq(1).find('li').eq(0).addClass('selected').closest('.dropdown').find('li').eq(0).text($('.hidden').eq(1).find('li').eq(0).text()).attr('value', ($('.hidden').eq(1).find('li').eq(0).attr('value')));


    $('#btnSave').bind("click", function (e) {

        e.preventDefault();

        if ($('#tblLocations .spnTetheredLocationsClicked').length != 0 && $('#tblTetheredLocations tbody tr').length != 0) {
            var isTetheringExists = false;
            $('#tblTetheredLocations tbody tr').each(function () {
                var dependantBrandId = $(this).find('td').eq(0).find('span').eq(0).attr('id');
                var dominantBrandId = $(this).find('td').eq(0).find('span').eq(1).attr('id');
                if (dependantBrandId != undefined || dependantBrandId != undefined) {
                    if (dependantBrandId == $('#location li').attr('value') && dominantBrandId == $('#dimension-source li').attr('value')) {
                        var tetheredLocationIds = $(this).find('.tetheredLocations').attr('locationids');

                        if ($.inArray($('#tblLocations .spnTetheredLocationsClicked').attr('id').split('_')[1], tetheredLocationIds.split(',')) >= 0) {
                            isTetheringExists = true;
                            return;
                        }
                    }
                }
            });
            //This validation is for scenario: If tether exist for MCO AD>EZ then don't allow tether for MCO EZ>AD: Removed validation
            //if (isTetheringExists) {
            //    $('#spantetherSetingExists').show();
            //    $('#spanSave').hide();
            //    $(document).scrollTop(0);
            //    return false;
            //}
            //else {
            //    $('#spantetherSetingExists').hide();
            //}
        }

        ValidateBrandDropdowns();
        ValidateTetherValues();
        AddFlashingEffect();

        if ($('#globalTether').find('*').hasClass('temp')) {
            $('#spanError').show();
            return false;
        }
        else {
            $('#spanError').hide();
        }

        SaveGlobalTetheringValues();
    });

    $('#btnDelete').bind("click", function (e) {
        e.preventDefault();
        if ($('.spnTetheredLocationsClicked').length > 0) {
            ShowConfirmBox('Do you want to delete the Tether Setting for <b>' + $('.spnTetheredLocationsClicked')[0].text.trim() + "  " + $('#location li.selected').text().trim() + '</b>?', true, deleteTethering, 0);
        }
    });

    $('#dimension-source ul li').click(function () {
        $('#location ul li[value="' + $(this).attr('value') + '"]').removeClass('selected').siblings().first().addClass('selected').closest('.dropdown').find('li').eq(0).text($('#location ul li[class="selected"]').text()).attr('value', ($('#location ul li[class="selected"]').attr('value')));

        setTimeout(function () {
            ResetAll();
        }, 250);
    });


    $('#location ul li').click(function () {
        $('#dimension-source ul li[value="' + $(this).attr('value') + '"]').removeClass('selected').siblings().first().addClass('selected').closest('.dropdown').find('li').eq(0).text($('#dimension-source ul li[class="selected"]').text()).attr('value', ($('#dimension-source ul li[class="selected"]').attr('value')));

        if (SelectedLocationId != undefined) {
            SelectedLocationId = "";
        }
        setTimeout(function () {
            ResetAll();
        }, 250);
    })


    //Add smart search, call method from master js
    var $inputTextSelector = $('#searchLocation');
    var $controlIdSelector = $("#tblLocations tbody td[class!='relative']");
    $('#searchLocation').bind('input', function () {
        SmartSearch($inputTextSelector, $controlIdSelector);
        if ($("#tblLocations tbody td[style$='display: none;']").length == $controlIdSelector.length) {
            MakeTagFlashable($inputTextSelector);
        }
        else {
            RemoveFlashableTag($inputTextSelector);
        }

        AddFlashingEffect();
    })


    $('#btnCopyToAll').click(function () {

        ValidateTetherValues();

        if ($('#allPercentage').hasClass('flashBorder') || $('#allDollar').hasClass('flashBorder')) {
            return;
        }
        else {

            if ($.trim($('#allPercentage').val()) != "" && $.isNumeric($('#allPercentage').val())) {
                $('#tblTetherRates tr').each(function () {
                    if ($(this).is(':visible')) {
                        $(this).find('input[type="text"]').eq(0).val($.trim($('#allPercentage').val()));
                        $(this).find('input[type="text"]').eq(1).val('');
                    }
                })
            }

            if ($.trim($('#allDollar').val()) != "" && $.isNumeric($('#allDollar').val())) {
                $('#tblTetherRates tr').each(function () {
                    if ($(this).is(':visible')) {
                        $(this).find('input[type="text"]').eq(1).val($.trim($('#allDollar').val()));
                        $(this).find('input[type="text"]').eq(0).val('');
                    }
                })
            }
        }

        ValidateTetherValues();
    })

    $('[ID^="location_"]').bind("click", function () {
        ValidateBrandDropdowns();
        AddFlashingEffect();
        if ($('#dimension-source').hasClass('temp')) {
            return false;
        }

        SelectedLocationId = $(this).attr('id').split('_')[1];

        var attr = $('#btnSave').attr('disabled');
        if (typeof attr !== typeof undefined && attr !== false) {
            var locationId = $(this).attr('id').split('_')[1];
            var dependantBrandId = $('#dimension-source li').val();
            var dominantBrandId = $('#location li').val();
            StartLoadingLocationTetherDetails(locationId, dependantBrandId, dominantBrandId);
        } else {

            var message = "Do you want to discard the changes? ";
            var objPassedToConfirm = new Object();
            objPassedToConfirm.locationId = $(this).attr('id').split('_')[1];
            objPassedToConfirm.dependantBrandId = $('#dimension-source li').val();
            objPassedToConfirm.dominantBrandId = $('#location li').val();
            ShowConfirmBox(message, true, LoadLocationTetherDetailsAfterConfirm, objPassedToConfirm);
        }

    });

});

function LoadLocationTetherDetailsAfterConfirm() {
    var locationId = this.locationId;
    var dependantBrandId = this.dependantBrandId;
    var dominantBrandId = this.dominantBrandId;
    $('#spantetherSetingExists').hide();
    $('#btnSave').attr('disabled', 'disabled').addClass('btnDisabled');
    $('#tblLocations').removeClass('temp').removeClass('.flashBorder');
    ValidateTetherValues();
    $('#tblTetherRates input[type="text"]').removeClass('temp').removeClass('flashBorder');
    AddFlashingEffect();
    StartLoadingLocationTetherDetails(locationId, dependantBrandId, dominantBrandId);
}

function StartLoadingLocationTetherDetails(locationId, dependantBrandId, dominantBrandId) {

    $('#tblLocations tr[class="location"]').addClass('grey_bg');
    $('#tblLocations a[id="location_' + locationId + '"]').closest('.location').removeClass('grey_bg');
    $('.tetheredLocations').find('a').removeClass('spnTetheredLocationsClicked');
    $('#tblLocations').find('a').removeClass('spnTetheredLocationsClicked');
    $('#tblLocations a[id="location_' + locationId + '"]').addClass('spnTetheredLocationsClicked');
    $('#allPercentage, #allDollar').val("");
    $('#btnSave').attr('disabled', 'disabled').addClass('btnDisabled');
    $('#tblTetherRates tr').find('td [type="text"]').removeClass('temp').removeClass('flashBorder')

    LoadLocationTetheringDetails(locationId, dependantBrandId, dominantBrandId);
}

function ResetAll() {
    $('.trTetherValues').show().find('td [type="text"]').val("");
    $('#allPercentage, #allDollar').val("");
    ValidateBrandDropdowns();
    AddFlashingEffect();
    $('#tblLocations').find('tr[class="location"]').addClass('grey_bg').find('a').removeClass('spnTetheredLocationsClicked');
    $('.tetheredLocations').find('a').removeClass('spnTetheredLocationsClicked');
    $('#spantetherSetingExists, #spanSave, #spanError').hide();
    $('#btnDelete').attr('disabled', 'disabled').addClass('disable-button');
}

function BindExistingTetheredLocations() {

    $.ajax({
        url: 'GlobalTethering/GetExistingTetheredLocations/',
        type: 'GET',
        async: true,
        data: {},
        success: function (data) {
            var tetheredLocations = $.map(data, function (item) { return new ExistingTetheredLocation(item); });
            globalTetheringModel.existingTetheredLocations(tetheredLocations);

            setTimeout(function () {

                $('.tetheredLocations').each(function () {

                    var locIds = $(this).attr('LocationIds').split(',');
                    var locns = $(this).attr('Locations').split(',');

                    for (i = 0; i < locns.length; i++) {
                        $("<a href='#' class='spnTetheredLocations' id=" + locIds[i] + " >" + locns[i] + "</a>").appendTo($(this));
                    }

                    $(this).find('a').bind("click", function (e) {
                        SelectedLocationId = $(this).attr('id');
                        var attr = $('#btnSave').attr('disabled');
                        if (typeof attr !== typeof undefined && attr !== false) {
                            var locationId = SelectedLocationId = $(this).attr('id');
                            var dependantBrandId = $(this).parent().parent().find('span').eq(0).attr('id');
                            var dominantBrandId = $(this).parent().parent().find('span').eq(1).attr('id');
                            SetandLoadTetheredLocationDetails(locationId, dependantBrandId, dominantBrandId);
                        } else {

                            var message = "Do you want to discard the changes? ";
                            var objPassedToConfirm = new Object();
                            objPassedToConfirm.locationId = SelectedLocationId = $(this).attr('id');
                            objPassedToConfirm.dependantBrandId = $(this).parent().parent().find('span').eq(0).attr('id');
                            objPassedToConfirm.dominantBrandId = $(this).parent().parent().find('span').eq(1).attr('id');
                            ShowConfirmBox(message, true, SetandLoadTetheredLocationDetailsAfterConfirm, objPassedToConfirm);
                        }
                    })

                });

            }, 200);


        },
        error: function (e) {
            //called when there is an error
            console.log("BindExistingTetheredLocations: " + e.message);
        }
    });

}

function SetandLoadTetheredLocationDetailsAfterConfirm() {
    var locationId = this.locationId;
    var dependantBrandId = this.dependantBrandId;
    var dominantBrandId = this.dominantBrandId;
    $('#spantetherSetingExists').hide();
    $('#btnSave').attr('disabled', 'disabled').addClass('btnDisabled');
    $('#tblLocations').removeClass('temp').removeClass('.flashBorder');
    ValidateTetherValues();
    $('#tblTetherRates input[type="text"]').removeClass('temp').removeClass('flashBorder');
    AddFlashingEffect();
    SetandLoadTetheredLocationDetails(locationId, dependantBrandId, dominantBrandId);
}

function SetandLoadTetheredLocationDetails(locationId, dependantBrandId, dominantBrandId) {

    $('.table-ul-right ul ul').find('li').removeClass('selected');

    //set dependent brand dropdown
    $('.table-ul-right ul ul').eq(0).find('li[id="' + dependantBrandId + '"]').addClass('selected');
    $('#dimension-source li').eq(0).text($('.table-ul-right ul ul').eq(0).find('li[id="' + dependantBrandId + '"]').text()).attr('value', $('.table-ul-right ul ul').eq(0).find('li[id="' + dependantBrandId + '"]').attr('value'));

    //set dominant brand dropdown
    $('.table-ul-right ul ul').eq(1).find('li[id="' + dominantBrandId + '"]').addClass('selected');
    $('#location li').eq(0).text($('.table-ul-right ul ul').eq(1).find('li[id="' + dominantBrandId + '"]').text()).attr('value', $('.table-ul-right ul ul').eq(1).find('li[id="' + dominantBrandId + '"]').attr('value'));

    ValidateBrandDropdowns();
    LoadLocationTetheringDetails(locationId, dependantBrandId, dominantBrandId);

    $('.tetheredLocations').find('a').removeClass('spnTetheredLocationsClicked');
    $('#tblLocations').find('tr[class="location"]').addClass('grey_bg').find('a').removeClass('spnTetheredLocationsClicked');
    setTimeout(function () { SetLocationHighlighing(); }, 100);
    $('#allPercentage, #allDollar').val("");
    $('#btnSave').attr('disabled', 'disabled').addClass('btnDisabled');
    $('#btnDelete').removeAttr('disabled').removeClass('disable-button');
    $('#tblTetherRates tr').find('td [type="text"]').removeClass('temp').removeClass('flashBorder');
}

function LoadLocationTetheringDetails(locationId, dependantBrandId, dominantBrandId) {

    var locationDetailsObject = new Object();
    locationDetailsObject.locationId = locationId.toString();
    locationDetailsObject.dominantBrandId = dominantBrandId.toString();
    locationDetailsObject.dependantBrandId = dependantBrandId.toString();

    $('[ID^="text_percent_"], [ID^="text_dollar_"]').val("");

    //TODO: Show spinner if required

    $.ajax({
        url: 'GlobalTethering/GetLocationTetheringDetails/',
        type: 'POST',
        async: true,
        data: locationDetailsObject,
        success: function (data) {
            $('#tblTetherRates tr[class="trTetherValues"]').each(function () {
                var carClassId = parseInt($(this).find('td').eq(0).attr('value'));
                if ($.inArray(carClassId, data.LocationCarclassIds) > -1) {
                    $(this).show();
                    $.each(data.GlobalTetherValues, function (index, value) {
                        if (carClassId == parseInt(value["CarClassId"])) {
                            if (JSON.parse(value["IsPercentage"])) {
                                $('#text_percent_' + carClassId).val(value["TetherValue"]);
                            }
                            else {
                                $('#text_dollar_' + carClassId).val(value["TetherValue"]);
                            }
                        }
                    });

                    $('#tblTetherRates input[type="text"]').unbind("input").bind("input", function () {
                        ValidateTetherValues();
                        AddFlashingEffect();
                        $('#btnSave').removeAttr('disabled').removeClass('btnDisabled');
                    });

                }
                else {
                    $(this).hide();

                }
                if (data.GlobalTetherValues.length <= 0) {
                    $('#btnDelete').attr('disabled', 'disabled').addClass('disable-button');
                }
                else {
                    $('#btnDelete').removeAttr('disabled').removeClass('disable-button');
                }
            });
        },
        error: function (e) {
            //called when there is an error
            console.log("LoadLocationTetheringDetails: " + e.message);
        }
    });

}

function SaveGlobalTetheringValues() {

    var GlobalTetherValues = new Object();

    if (SelectedLocationId == undefined || SelectedLocationId == "") {
        $('#tblLocations').addClass('temp').addClass('.flashBorder');
        $('#spanError').show();
        return false;
    }
    else {
        $('#tblLocations').removeClass('temp').removeClass('.flashBorder');
        $('#spanError').hide();
    }

    GlobalTetherValues.locationId = SelectedLocationId.toString();
    GlobalTetherValues.dependantBrandId = $('#dimension-source li').val();
    GlobalTetherValues.dominantBrandId = $('#location li').val();
    GlobalTetherValues.loggedInUserId = $('#LoggedInUserId').val();

    var tetherValues = {};
    $('.trTetherValues').each(function () {
        carClassId = $(this).find('td').eq(0).attr('value');
        tetherValues[carClassId] = {};

        if ($.trim($('#text_percent_' + carClassId).val()) != "") {
            tetherValues[carClassId]["IsValueInPercentage"] = true;
            tetherValues[carClassId]["TetherValue"] = $('#text_percent_' + carClassId).val();
        }
        else {
            tetherValues[carClassId]["IsValueInPercentage"] = false;
            tetherValues[carClassId]["TetherValue"] = $('#text_dollar_' + carClassId).val();
        }

    });

    GlobalTetherValues.TetherValues = JSON.stringify(tetherValues);

    $.ajax({
        url: 'GlobalTethering/SaveLocationTetheringDetails/',
        type: 'POST',
        async: true,
        data: GlobalTetherValues,
        success: function (data) {
            $('#btnSave').attr('disabled', 'disabled').addClass('btnDisabled');
            $('#spanSave').show();
            setTimeout(function () {
                $(document).scrollTop('0');
            }, 1000);
            BindExistingTetheredLocations();
            setTimeout(function () {
                SetLocationHighlighing();
                $('#tblLocations a[id="location_' + SelectedLocationId + '"]').addClass('spnTetheredLocationsClicked').closest('.location').removeClass('grey_bg');
            }, 2000);
            setTimeout(function () {
                $('#spanSave').hide();
            }, 4000);
            $('#btnDelete').removeAttr('disabled').removeClass('disable-button');
        },
        error: function (e) {
            //called when there is an error
            console.log("SaveGlobalTetheringValues: " + e.message);
        }
    });

}

function SetLocationHighlighing() {
    $('#tblTetheredLocations tbody tr').each(function () {
        var dependantBrandId = $(this).find('td').eq(0).find('span').eq(0).attr('id');
        var dominantBrandId = $(this).find('td').eq(0).find('span').eq(1).attr('id');
        if (dependantBrandId != undefined || dependantBrandId != undefined) {
            if (dominantBrandId == $('#location li').attr('value') && dependantBrandId == $('#dimension-source li').attr('value')) {
                $(this).find('a[id="' + SelectedLocationId + '"]').addClass('spnTetheredLocationsClicked')
            }
        }
    });
}

function ValidateTetherValues() {

    $('#tblTetherRates input[type="text"]').each(function () {

        if ($.trim($(this).val()) != "" && !$.isNumeric($.trim($(this).val()))) {
            $('#spanError').show();
            MakeTagFlashable($(this));
        } else {
            $('#spanError').hide();
            RemoveFlashableTag($(this));
        }

        $('#tblTetherRates tr').each(function () {
            if ($.trim($(this).find('input[type="text"]').eq(0).val()) != "" && $.trim($(this).find('input[type="text"]').eq(1).val()) != "") {
                $('#spanError').show();
                MakeTagFlashable($(this).find('input[type="text"]'));
            }
            else {
                if (!$(this).find('input[type="text"]').hasClass('temp')) {
                    $('#spanError').hide();
                    RemoveFlashableTag($(this));
                }
            }
        })

        //if ($.trim($(this).val()) != "") {
        //    allTextBoxesEmpty = false;
        //}

    })

    //allTextBoxesEmpty ? $('#tblTetherRates').addClass('temp').addClass('flashBorder') : $('#tblTetherRates').removeClass('temp').removeClass('flashBorder');
}

function ValidateBrandDropdowns() {
    if ($('#dimension-source li').val() == $('#location li').val()) {
        $('#spanError').show();
        MakeTagFlashable('#dimension-source, #location');
    }
    else {

        $('#spanError').hide();
        RemoveFlashableTag('#dimension-source, #location');
    }
}

var globalTetheringModel;

function GlobalTetheringModel() {
    var self = this;
    self.carClasses = ko.observableArray([]);
    self.existingTetheredLocations = ko.observableArray([]);
}

function BindCarClasses() {
    $.ajax({
        url: 'Search/GetCarClasses/',
        type: 'GET',
        async: true,
        success: function (data) {
            if (data) {
                var srcs = $.map(data, function (item) { return new carClassWithTetheringValue(item); });
                globalTetheringModel.carClasses(srcs);

                setTimeout(function () {

                    $('#tblTetherRates input[type="text"]').unbind("input").bind("input", function () {

                        ValidateTetherValues();
                        AddFlashingEffect();
                        $('#btnSave').removeAttr('disabled').removeClass('btnDisabled');
                    });
                }, 500);
            }
        },
        error: function (e) {
            console.log("BindCarClasses: " + e.message);
        }
    });
}

function carClassWithTetheringValue(data) {
    this.ID = data.ID;
    this.Code = data.Code;
    this.PercentTextBoxId = ko.computed(function () {
        return "text_percent_" + data.ID;
    });
    this.DollarTextBoxId = ko.computed(function () {
        return "text_dollar_" + data.ID;
    });
}

function ExistingTetheredLocation(data) {
    this.DominantBrandId = data.DominantBrandId;
    this.DependantBrandId = data.DependantBrandId;
    this.LocationIds = data.LocationIds;
    this.DominantBrand = data.DominantBrand;
    this.DependantBrand = data.DependantBrand;
    this.Locations = data.Locations;
}

function deleteTethering() {
    var ajaxURl = '/RateShopper/GlobalTethering/DeleteLocationTethering';
    if (GlobalTetherURLSettings != undefined && GlobalTetherURLSettings != '') {
        ajaxURl = GlobalTetherURLSettings.DeleteTether;
    }
    var DominentId = $('#location li.selected').attr('value');//right side dropdown box
    var DependentId = $('#dimension-source li.selected').attr('value');//left side dropdownbox
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        async: true,
        data: { locationId: SelectedLocationId, dominentBrandId: DominentId, dependentBrandId: DependentId },
        success: function (data) {
            if (data != "") {
                console.log(data);
                $('#spanDelete').show();
                if (data.toLowerCase() == "success") {
                    BindExistingTetheredLocations();
                    ResetAll();
                    setTimeout(function () {
                        $(document).scrollTop('0');
                    }, 250);
                    setTimeout(function () {
                        $('#spanDelete').hide();
                    }, 4000);
                }
                else {
                    $('#spanDelete').hide();
                }
            }
            else {
                $('#spanDelete').hide();
            }
        },
        error: function (e) {
            //called when there is an error

        }
    });
}