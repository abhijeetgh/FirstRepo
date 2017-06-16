var automationTetherModel;
var GlobalTetherSettingGlobal;
var JobSelectedTetherSetting = "";
var FinalTetherValueData = ""; //for check if user can reset the value get the data from prevous value
var IsEditjobLocationChanged = false;//Used for in edit job user can menually change the existing location to new location in this schenario maintain the location selection
$(document).ready(function () {
    automationTetherModel = new AutomationTetherModel();
    ko.applyBindings(automationTetherModel, document.getElementById('mr_Tether_popup'));

    //getGlobalTetherValue();
    $("#mr_Tether_popup").draggable();
    $("#view1 #TetherRate").on("click", function (e) {
        if ($("#view1 #carClass select").val() != null && $("#view1 #carClass select").val() != undefined && $("#view1 #locations select").val() != null && $("#view1 #locations select").val() != undefined) {
            $("#PercentageID_All").val("");
            $("#DollerID_All").val("");
            $("#mr_Tether_popup, .popup_bg").show();

        } else {
            if ($("#view1 #carClass select").val() == null && $("#view1 #carClass select").val() == undefined && $("#view1 #locations select").val() == null && $("#view1 #locations select").val() == undefined) {
                ShowConfirmBox('Select location and car class', false);
                //alert("Select Location and Car Class");
            }
            else if ($("#view1 #carClass select").val() == null && $("#view1 #carClass select").val() == undefined) {
                ShowConfirmBox('Please select car class', false);
                //alert("Please select car class");
            }
            else if ($("#view1 #locations select").val() == null && $("#view1 #locations select").val() == undefined) {
                ShowConfirmBox('Please select location', false);
                //alert("Please select location");
            }
        }
    });

    //Close button reset the data
    $("#closepopupTetherRate").on("click", function () {
        $("#mr_Tether_popup, .popup_bg").hide();
        $("#TehterTableValue tbody tr").find("input").val("");
        $("#TehterTableValue tbody tr").find("input").removeClass("flashBorder temp");

        $("#TehterTableValue tbody tr").each(function () {
            if (FinalTetherValueData != "") {
                var carClassID = $(this).find("td").eq(0).find("span").attr("carclassid");
                if (carClassID != undefined) {
                    $(FinalTetherValueData.lstScheduledJobTetherCarClass).each(function () {
                        if (carClassID == this.CarClassID) {
                            if (JSON.parse(this.IsTetherValueinPercentage)) {
                                $('#PercentageID_' + carClassID).val(this.TetherValue);
                            }
                            else {
                                $('#DollerID_' + carClassID).val(this.TetherValue);
                            }
                            return false;
                        }
                    });
                }
            }
            else {
                return false;
            }
        });
    });

    //Copy to all button functionalities
    $("#copytoall").on("click", function () {
        var AllCarClassPercentage = $("#PercentageID_All").val();
        var AllCarClassDollar = $("#DollerID_All").val();
        if ($.trim(AllCarClassPercentage) != "" && $.trim(AllCarClassDollar) != "") {
            validateTetherShop();
        }
        else {
            if (AllCarClassPercentage.trim() != "") {
                $("input[name=percentageValue]").each(function () {
                    $("#" + $(this).attr("ID")).val(AllCarClassPercentage);
                    $("input[name=dollerValue]").each(function () {
                        $("#" + $(this).attr("ID")).val("");
                    });
                });
                $("#DollerID_All").val("");
            }
            if (AllCarClassDollar.trim() != "") {
                $("input[name=dollerValue]").each(function () {
                    $("#" + $(this).attr("ID")).val(AllCarClassDollar);
                    $("input[name=percentageValue]").each(function () {
                        $("#" + $(this).attr("ID")).val("");
                    });
                });
                $("#PercentageID_All").val("");
            }
        }
    });

    //preload functionality
    $("#GlobalTetherPreload").on("click", function () {
        $('#TehterTableValue input[type=text]').val("");
        //If validation highlight will be occured
        $('#TehterTableValue input[type=text]').removeClass("temp").removeClass("flashBorder");

        var DominentBrandID = "";
        var DependantBrandID = "";
        var BrandID = "";
        var locationID = "";
        var $Locationdata = $("#view1 #locations select option:selected[value=" + $("#view1 #locations select").val() + "]");
        BrandID = $Locationdata.attr("companyid");
        locationID = $Locationdata.val();
        var globalTetherSettingsForBrand = $.grep(GlobalTetherSettingGlobal, function (item) {
            return item.DominentBrandID == BrandID && item.LocationID == locationID;
        });
        $('#TehterTableValue tbody tr').each(function () {
            var carclassid = $(this).find("td").eq(0).find("span").attr("carclassid");
            var tetherValue = "";
            var IsPercentageValue = false;
            if (carclassid != undefined && carclassid != "") {
                //for (var x in GlobalTetherSettingGlobal) {
                //    if (GlobalTetherSettingGlobal[x].CarClassID == carclassid && GlobalTetherSettingGlobal[x].LocationID == locationID) {
                //        if (BrandID == GlobalTetherSettingGlobal[x].DominentBrandID) {
                //            tetherValue = GlobalTetherSettingGlobal[x].TetherValue;
                //            IsPercentageValue = GlobalTetherSettingGlobal[x].IsTeatherValueinPercentage;
                //            DominentBrandID = GlobalTetherSettingGlobal[x].DominentBrandID;
                //            DependantBrandID = GlobalTetherSettingGlobal[x].DependantBrandID;
                //        }
                //    }
                //}
                $(globalTetherSettingsForBrand).each(function () {
                    if (this.CarClassID == carclassid) {
                        tetherValue = this.TetherValue;
                        IsPercentageValue = this.IsTeatherValueinPercentage;
                        DominentBrandID = this.DominentBrandID;
                        DependantBrandID = this.DependantBrandID;
                    }
                });

                $(this).find("td").eq(0).find("span").attr("DominentBrandID", DominentBrandID);
                $(this).find("td").eq(0).find("span").attr("DependantBrandID", DependantBrandID);
                if (IsPercentageValue) {
                    $('#PercentageID_' + carclassid).val(tetherValue);
                }
                else {
                    $('#DollerID_' + carclassid).val(tetherValue);
                }
            }

        });
    });

    //save button save the final data
    $("#TetherRateSave").on("click", function () {
        if (validateTetherShop() == true) {
            $("#mr_Tether_popup, .popup_bg").hide();
            FinalTetherData();
        }
    });

    $("#activeTethering").on("change", function () {
        if ($(this).prop("checked")) {
            if ($("#view1 #source select option:selected[value=" + $("#view1 #source select").val() + "]").attr("isgov") != "True") {
                $("#TetherRate").prop('disabled', false).removeClass("disable-button");
            }
            BindTetherData();
        }
        else {
            $("#TetherRate").prop('disabled', true).addClass("disable-button");
        }
    });
});

//ViewModel
function AutomationTetherModel() {
    var self = this;
    self.TetherRateList = ko.observableArray([]);
}
//End ViewModel

//Entities
function TetherValueEntity(data) {
    this.ID = data.ID;
    this.carClass = data.carClass;
    this.DominentBrandID = data.DominentBrandID;
    this.DependantBrandID = data.DependantBrandID;
    this.IsTeatherValueinPercentage = data.IsTetherValueinPercentage;
    this.DollarValue = ko.computed(function () {
        if (JSON.parse(data.IsTetherValueinPercentage)) {
            return "";
        }
        else {
            return data.TetherValue;
        }
    });
    this.PercentageValue = ko.computed(function () {
        if (JSON.parse(data.IsTetherValueinPercentage)) {
            return data.TetherValue;
        }
        else {
            return "";
        }
    });
}
//End Entities


//Other functions
//Bind data on tether popup
function BindTetherData() {
    FinalTetherValueData = [];
    if ($("#view1 #carClass select").val() != null && $("#view1 #carClass select").val() != undefined && $("#view1 #locations select").val() != null && $("#view1 #locations select").val() != undefined) {
        FinalTetherValueData = "";
        var TetherValueData = [];
        var ScheduleJobID = 0;
        var DominentBrandID = "";
        var DependantBrandID = "";
        var SelectedCarClass = $("#view1 #carClass select").val();
        if (SelectedCarClass != undefined && SelectedCarClass != '') {
            if ($("#activeTethering").prop('checked')) {
                var BrandID = "";
                var locationBrandID = "";
                var locationID = "";
                var $Locationdata = $("#view1 #locations select option:selected[value=" + $("#view1 #locations select").val() + "]");
                BrandID = $Locationdata.attr("companyid");
                locationBrandID = $Locationdata.attr("brandid");
                locationID = $Locationdata.val();
                //$("#view1 #locations select option[value=" + $("#view1 #locations select").val() + "]").each(function () {
                //    if ($(this).prop("selected")) {
                //        BrandID = $(this).attr("companyid");
                //        locationBrandID = $(this).attr("brandid");
                //        locationID = $(this).val();
                //    }
                //});
                var globalTetherSettingsForBrand = $.grep(GlobalTetherSettingGlobal, function (item) {
                    return item.DominentBrandID == BrandID && item.LocationID == locationID;
                });
                var ScheduledJobTetheringsDTO = new Object();
                ScheduledJobTetheringsDTO.lstScheduledJobTetherCarClass = [];

                $(SelectedCarClass).each(function () {
                    var tetherValue = "";
                    var IsPercentageValue = false;
                    var CarClassID = $("#view1 #carClass select option[value=" + this + "]").val();
                    var CarClass = $("#view1 #carClass select option[value=" + this + "]").text();


                    if (jobId != undefined && jobId != "") {
                        //Update job
                        ScheduleJobID = jobId;
                        if (JobSelectedTetherSetting != undefined && JobSelectedTetherSetting != null) {
                            if (!IsEditjobLocationChanged) {
                                if (JobSelectedTetherSetting.lstScheduledJobTetherCarClass.length > 0) {
                                    $(JobSelectedTetherSetting.lstScheduledJobTetherCarClass).each(function () {
                                        if (this.CarClassID == CarClassID) {
                                            tetherValue = this.TetherValue;
                                            IsPercentageValue = this.IsTetherValueinPercentage;
                                            DominentBrandID = JobSelectedTetherSetting.DominentBrandID;
                                            DependantBrandID = JobSelectedTetherSetting.DependantBrandID;
                                        }
                                    });
                                }
                                else {
                                    $.each(globalTetherSettingsForBrand, function (i, x) {
                                        if (this.CarClassID == CarClassID) {
                                            DependantBrandID = x.DependantBrandID;
                                            DominentBrandID = x.DominentBrandID;
                                            return;
                                        }
                                    });
                                }
                            }
                            else {
                                //For if location will be changed by user that case all time tether value shoulb be display from global tether setting
                                $.each(globalTetherSettingsForBrand, function (i, x) {
                                    if (x.CarClassID == CarClassID) {
                                        DependantBrandID = x.DependantBrandID;
                                        DominentBrandID = x.DominentBrandID;
                                        tetherValue = x.TetherValue;
                                        IsPercentageValue = x.IsTeatherValueinPercentage;
                                        return;
                                    }
                                });

                                //for (var x in GlobalTetherSetting) {
                                //    if (GlobalTetherSetting[x].CarClassID == CarClassID && GlobalTetherSetting[x].LocationID == locationID) {
                                //        if (BrandID == GlobalTetherSetting[x].DominentBrandID) {
                                //            tetherValue = GlobalTetherSetting[x].TetherValue;
                                //            IsPercentageValue = GlobalTetherSetting[x].IsTeatherValueinPercentage;
                                //            DominentBrandID = GlobalTetherSetting[x].DominentBrandID;
                                //            DependantBrandID = GlobalTetherSetting[x].DependantBrandID;
                                //        }
                                //    }
                                //}
                            }
                        }
                    }
                    else {
                        //create new   
                        $.each(globalTetherSettingsForBrand, function (i, x) {
                            if (x.CarClassID == CarClassID) {
                                DependantBrandID = x.DependantBrandID;
                                DominentBrandID = x.DominentBrandID;
                                tetherValue = x.TetherValue;
                                IsPercentageValue = x.IsTeatherValueinPercentage;
                                return;
                            }
                        });
                        //for (var x in GlobalTetherSetting) {
                        //    if (GlobalTetherSetting[x].CarClassID == CarClassID && GlobalTetherSetting[x].LocationID == locationID) {
                        //        if (BrandID == GlobalTetherSetting[x].DominentBrandID) {
                        //            tetherValue = GlobalTetherSetting[x].TetherValue;
                        //            IsPercentageValue = GlobalTetherSetting[x].IsTeatherValueinPercentage;
                        //            DominentBrandID = GlobalTetherSetting[x].DominentBrandID;
                        //            DependantBrandID = GlobalTetherSetting[x].DependantBrandID;
                        //        }
                        //    }
                        //}
                    }
                    if ($("#view1 #source select option:selected[value=" + $("#view1 #source select").val() + "]").attr("isgov") == "True") {
                        tetherValue = 0;
                    }

                    var ScheduledJobTetherCarClassDTO = new Object();
                    ScheduledJobTetherCarClassDTO.CarClassID = CarClassID;
                    ScheduledJobTetherCarClassDTO.TetherValue = tetherValue;
                    ScheduledJobTetherCarClassDTO.IsTetherValueinPercentage = IsPercentageValue;

                    var item = {}
                    item["ID"] = CarClassID;
                    item["CarClassID"] = CarClassID;
                    item["ScheduleJobID"] = ScheduleJobID;
                    item["LocationBrandID"] = locationBrandID;
                    item["DominentBrandID"] = DominentBrandID;
                    item["DependantBrandID"] = DependantBrandID;
                    item["carClass"] = CarClass;
                    item["TetherValue"] = tetherValue;
                    item["IsTetherValueinPercentage"] = IsPercentageValue;
                    if (item["CarClassID"] != undefined) {
                        TetherValueData.push(item);
                        ScheduledJobTetheringsDTO.lstScheduledJobTetherCarClass.push(ScheduledJobTetherCarClassDTO);
                        //FinalTetherValueData.push(item);//For CR from client non close button reset previous button
                    }
                });
                ScheduledJobTetheringsDTO.LocationBrandID = locationBrandID;
                ScheduledJobTetheringsDTO.DominentBrandID = DominentBrandID;
                ScheduledJobTetheringsDTO.DependantBrandID = DependantBrandID;
                ScheduledJobTetheringsDTO.ScheduleJobID = ScheduleJobID;

                //console.log(ScheduledJobTetheringsDTO);
                FinalTetherValueData = ScheduledJobTetheringsDTO;
            }

            
            var bindTetherValue = $.map(TetherValueData, function (item) { return new TetherValueEntity(item); });
            automationTetherModel.TetherRateList(bindTetherValue);

            TetherBlurEvent();//Tether blur event execute after all control bind
        }
    } else {
        if ($("#view1 #carClass select").val() == null && $("#view1 #carClass select").val() == undefined && $("#view1 #locations select").val() == null && $("#view1 #locations select").val() == undefined) {
            ShowConfirmBox('Select location and car class', false);
            //alert("Select Location and Car Class");
        }
        else if ($("#view1 #carClass select").val() == null && $("#view1 #carClass select").val() == undefined) {
            ShowConfirmBox('Please select car class', false);
            //alert("Please select car class");
        }
        else if ($("#view1 #locations select").val() == null && $("#view1 #locations select").val() == undefined) {
            ShowConfirmBox('Please select location', false);
            //alert("Please select location");
        }
    }
}

//Validation on tethervalue
function validateTetherShop() {
    var flag = false;
    $('#TehterTableValue input[type="text"]').each(function () {
        if ($.trim($(this).val()) != "" && !$.isNumeric($.trim($(this).val()))) {
            flag = false;
            MakeTagFlashable($(this));
        } else {
            flag = true;
            RemoveFlashableTag($(this));
        }
    });
    $('#TehterTableValue tr').each(function () {
        if ($.trim($(this).find('input[type="text"]').eq(0).val()) != "" && $.trim($(this).find('input[type="text"]').eq(1).val()) != "") {
            flag = false;
            MakeTagFlashable($(this).find('input[type="text"]'));
        }
        else {
            if (!$(this).find('input[type="text"]').hasClass('temp')) {
                RemoveFlashableTag($(this));
            }
        }
    });
    if ($("#TehterTableValue tbody tr").find(".temp").length > 0) {
        flag = false;
    }
    else {
        flag = true;
    }
    AddFlashingEffect();
    return flag;
}

//Tether textbox event
function TetherBlurEvent() {
    $('#TehterTableValue [type="text"]').on('keyup', function () {
        var selectedID = $(this).attr('id');//get the selected item        
        validateTetherShop();

        ////for use cross check
        //var TempSelectedSplit = selectedID.split('_');
        ////Percetnage Condition
        //if ($.isNumeric($(this).val()) || $(this).val().trim() == "") {
        //    if ($(this).attr('name') == 'percentageValue') {
        //        //If percentage can remove then if doller value is exist then apply that
        //        if ($("#" + selectedID).val().trim() == "" && $("#DollerID_" + TempSelectedSplit[1]).val() != "") {
        //            CalculationOfFormula("DollerID_" + TempSelectedSplit[1], "$");
        //        }
        //        else {
        //            //normal percentage value execute
        //            CalculationOfFormula(selectedID, "%")
        //        }
        //    }
        //    // Doller value condition
        //    if ($(this).attr('name') == 'dollerValue') {
        //        //If doller can remove then if percentage value is exist then apply that
        //        if ($("#" + selectedID).val().trim() == "" && $("#PercentageID_" + TempSelectedSplit[1]).val() != "") {
        //            CalculationOfFormula("PercentageID_" + TempSelectedSplit[1], "%");
        //        }
        //        else {
        //            //Normal doller value Execute 
        //            CalculationOfFormula(selectedID, "$");
        //        }
        //    }
        //}
        //validateTetherShop();
        AddFlashingEffect();
    });
}

//For get the final data to save in database.
function FinalTetherData() {
    FinalTetherValueData = "";
    var DominentBrandID = "";
    var DependantBrandID = "";
    var LocationBrandID = 0;
    var ScheduleJobID = 0;

    LocationBrandID = $("#view1 #locations select option:selected[value=" + $("#view1 #locations select").val() + "]").attr("brandid");

    var ScheduledJobTetheringsDTO = new Object();
    ScheduledJobTetheringsDTO.lstScheduledJobTetherCarClass = [];


    $("#TehterTableValue tbody tr").each(function () {
        var tetherValue = "";
        var IsPercentageValue = false;

        var carClassID = $(this).find("td").eq(0).find("span").attr("carclassid");
        var carClass = $(this).find("td").eq(0).find("span").text();
        DominentBrandID = $(this).find("td").eq(0).find("span").attr("dominentbrandid");
        DependantBrandID = $(this).find("td").eq(0).find("span").attr("dependantbrandid");


        //$("#view1 #locations select option[value=" + $("#view1 #locations select").val() + "]").each(function () {
        //    if ($(this).prop("selected")) {
        //        LocationBrandID = $(this).attr("brandid");
        //    }
        //});
        if (jobId != undefined && jobId != "") {
            //Update job
            ScheduleJobID = jobId;
        }
        if (carClassID != undefined && carClassID != null) {
            var percentValue = $(this).find("td").eq(1).find("input").eq(0);
            var dollerValue = $(this).find("td").eq(1).find("input").eq(1);

            if ($(percentValue).val() != "") {
                tetherValue = $(percentValue).val();
                IsPercentageValue = true;
                if (!$.isNumeric(tetherValue)) {
                    tetherValue = "";
                    $(percentValue).val("");
                }
            }
            if ($(dollerValue).val() != "") {
                tetherValue = $(dollerValue).val();
                IsPercentageValue = false;
                if (!$.isNumeric(tetherValue)) {
                    tetherValue = "";
                    $(dollerValue).val("");
                }
            }
            //if ($(percentValue).val() != "" || $(dollerValue).val() != "") {
            var ScheduledJobTetherCarClassDTO = new Object();
            ScheduledJobTetherCarClassDTO.CarClassID = carClassID;
            ScheduledJobTetherCarClassDTO.TetherValue = tetherValue;
            ScheduledJobTetherCarClassDTO.IsTetherValueinPercentage = IsPercentageValue;

            //var item = {}
            //item["ScheduleJobID"] = ScheduleJobID;
            //item["LocationBrandID"] = LocationBrandID;
            //item["DominentBrandID"] = DominentBrandID;
            //item["DependantBrandID"] = DependantBrandID;
            //item["CarClassID"] = carClassID;
            //item["carClass"] = carClass;
            //item["TetherValue"] = tetherValue;
            //item["IsTetherValueinPercentage"] = IsPercentageValue;
            ////FinalTetherValueData.push(item);
            ScheduledJobTetheringsDTO.lstScheduledJobTetherCarClass.push(ScheduledJobTetherCarClassDTO);
            // }
        }
    });
    ScheduledJobTetheringsDTO.LocationBrandID = LocationBrandID;
    ScheduledJobTetheringsDTO.DominentBrandID = DominentBrandID;
    ScheduledJobTetheringsDTO.DependantBrandID = DependantBrandID;
    ScheduledJobTetheringsDTO.ScheduleJobID = ScheduleJobID;

    FinalTetherValueData = ScheduledJobTetheringsDTO;
}
//End Other functions


//Ajax functions
function getGlobalTetherValueSettingLocationSpecific(isChked) {
    GlobalTetherSettingGlobal = [];
    FinalTetherValueData = [];//When use manually change after setup tethering data then we have empty this varible while user will go for save the job.
    var ajaxURl = '/RateShopper/Search/GetGlobalTetherSettingsLocationSpecific';
    if (TetherRates != undefined && TetherRates != '') {
        ajaxURl = TetherRates.GetGlobalTetheringURL;
    }
    var locationId = $("#view1 #locations select").val();
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        data: { locationId: locationId },
        async: true,
        success: function (data) {
            if (data) {
                GlobalTetherSettingGlobal = data;
                CheckTetherActualData(isChked);
            }
        },
        error: function (e) {
            console.log(e.message);
        }
    });
}
function CheckTetherActualData(isChked) {
    var BrandID = "";
    var $Locationdata = $("#view1 #locations select option:selected[value=" + $("#view1 #locations select").val() + "]");
    BrandID = $Locationdata.attr("companyid");
    locationID = $Locationdata.val();

    var tetherCheckValue = false;
    var globalTetherSettingsForBrand = $.grep(GlobalTetherSettingGlobal, function (item) {
        return item.DominentBrandID == BrandID && item.LocationID == locationID;
    });
    var SelectedCarClass = $("#view1 #carClass select").val();
    $(SelectedCarClass).each(function () {
        var CarClassID = $("#view1 #carClass select option[value=" + this + "]").val();
        $(globalTetherSettingsForBrand).each(function () {
            if (this.CarClassID == CarClassID) {
                if (this.TetherValue != "" || this.TetherValue == 0) {
                    tetherCheckValue = true;
                    return false;//internal tethering loop exit
                }
            }
        });
        if (jobId != undefined && jobId != "") {//Edit job case
            if (!IsEditjobLocationChanged && tetherCheckValue) { //If location will change then check data in globaltether setting 
                tetherCheckValue = false;
                if (JobSelectedTetherSetting.lstScheduledJobTetherCarClass.length > 0) {
                    $(JobSelectedTetherSetting.lstScheduledJobTetherCarClass).each(function () {
                        if (this.CarClassID == CarClassID && (this.TetherValue != "" || this.TetherValue == 0)) {
                            tetherCheckValue = true;
                            return false;//internal tethering loop exit
                        }
                    });
                }
            }
            else {
                $(globalTetherSettingsForBrand).each(function () {
                    if (this.CarClassID == CarClassID) {
                        if (this.TetherValue != "" || this.TetherValue == 0) {
                            tetherCheckValue = true;
                            return false;//internal tethering loop exit
                        }
                    }
                });
            }
        }
        if (tetherCheckValue) {
            return false;//Final loop exit
        }
    });

    if (tetherCheckValue) {
        if ($("#IsTetheringAccess").val() == "True") {
            $("#activeTethering").attr('disabled', false).prop("checked", true);
            $("#TetherRate").removeAttr('disabled').removeClass("disable-button");
        }
        else {
            $("#activeTethering").attr('disabled', true).prop("checked", true);
            $("#TetherRate").attr('disabled', 'disabled').addClass("disable-button");
        }
        BindTetherData();

    }
    else {

        $("#activeTethering").prop('checked', false).attr('disabled', true);
        $("#TetherRate").attr('disabled', 'disabled').addClass("disable-button");

    }
    if (isChked != null && isChked != undefined) {
        if (tetherCheckValue) {
            $("#activeTethering").prop('checked', isChked);
        }

        if (isChked) {
            if (!$("#activeTethering").prop("disabled") && $("#activeTethering").prop("checked")) {
                $("#TetherRate").removeAttr('disabled').removeClass("disable-button");
                BindTetherData();
            }
            else {
                $("#TetherRate").attr('disabled', 'disabled').addClass("disable-button");
            }
        }
        else {
            $("#TetherRate").attr('disabled', 'disabled').addClass("disable-button");
            // $("#activeTethering").prop('checked', false).attr('disabled', true);
        }
    }
    //For Only GOV shop tether button should disbale and tether check box should visible to allowing if user want to apply tether setting or not.
    checkGOVTetherButtonHideShow();
    //console.log(tetherCheckValue);
}

//Used to check GOV tether button functionality disbale/enable logic.
function checkGOVTetherButtonHideShow()
{
    if ($("#view1 #source select option:selected[value=" + $("#view1 #source select").val() + "]").attr("isgov") == "True") {
        $("#TetherRate").prop('disabled', true).addClass("disable-button");
    }
    else {
        if ($("#activeTethering").prop("checked")) {
            $("#TetherRate").removeAttr('disabled').removeClass("disable-button");
        }
    }
}
function GetSelectedJobTetherSetting(jobId) {
    JobSelectedTetherSetting = "";
    var ajaxURl = '/RateShopper/AutomationConsole/GetJobSelectedTetherSettings';
    if (TetherRates != undefined && TetherRates != '') {
        ajaxURl = TetherRates.GetJobSelectedTetherSettingsURL;
    }
    $.ajax({
        url: ajaxURl,
        type: 'POST',
        data: { jobId: parseInt(jobId) },
        async: true,
        success: function (data) {
            if (data) {
                JobSelectedTetherSetting = data;
            }
        },
        error: function (e) {
            console.log(e.message);
        }
    });

}
//End ajax functions
