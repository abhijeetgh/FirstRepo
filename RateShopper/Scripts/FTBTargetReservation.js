var targetViewModel;
var IsUpdateMode = false;
var SrinkUpdateStatus = false, CheckDataExist = false;
$(document).ready(function () {
    targetViewModel = new TargetViewModel();
    ko.applyBindings(targetViewModel);

    initializeEvent();
});
//--------------Other functions------------------
var initializeEvent = function () {
    getTargetTamplete();
    $('#TargetReservationMgtPopup #popup-main-inner').draggable();

    //Close popup event
    $(".closeP").on("click", function () {
        resetCancelButton();
        ClosedTargetPopup();
    });

    //Cancel target popup button event
    $("#TargetCancel").on("click", function () {
        resetCancelButton();
        //ClosedTargetPopup();
    });

    //Save target popup operation
    $("#TargetSave").on("click", function () {
        SaveTargetDetails();
        //ClosedTargetPopup();
    });

    $("#TargetReservationMgtPopup #targetyear ul li").on("click", function () {
        SrinkUpdateStatus = false;
        var yearValue = $(this).attr("value");
        var monthValue = $("#TargetReservationMgtPopup #targetmonth ul li.selected").attr("value");
        if (monthValue != '0' && yearValue != '0' && yearValue != undefined && monthValue != undefined) {
            GetTargetData(monthValue, yearValue, getCopyFromFlag(monthValue, yearValue));
        }
    });

    $("#TargetReservationMgtPopup #targetmonth ul li").on("click", function () {
        SrinkUpdateStatus = false;
        var monthValue = $(this).attr("value");
        var yearValue = $("#TargetReservationMgtPopup #targetyear ul li.selected").attr("value");
        if (monthValue != '0' && yearValue != '0' && yearValue != undefined && monthValue != undefined) {
            GetTargetData(monthValue, yearValue, getCopyFromFlag(monthValue, yearValue));
        }
    });
}

var OpenTargetPopup = function () {
    //Open target popup
    $("#targetMgtPopup").on("click", function () {
        $("#TargetReservationMgtPopup, .popup_bg").show();
        $("#RateLocationBrand").html($("#ftb-rating #ratesettingslocation ul li.selected").html());
        $("#RateMonth").html($("#ftb-rating #months ul li.selected").html());
        $("#RateYear").html($("#ftb-rating #year ul li.selected").html());
    });
}
var resetCancelButton = function () {
    $("#targeterrormsg,#targeterrormsgnotfilled").hide();
    $("#TargetReservationMgtPopup #targetyear li").eq(0).attr("value", "0").html("--select--");
    $("#TargetReservationMgtPopup #targetyear ul li[value=0]").addClass("selected").siblings("li").removeClass("selected");
    $("#TargetReservationMgtPopup #targetmonth li").eq(0).attr("value", "0").html("--select--");
    $("#TargetReservationMgtPopup #targetmonth ul li[value=0]").addClass("selected").siblings("li").removeClass("selected");

    $("#TargetReservationMgtPopup #targetyear").val(0);
    $("#TargetReservationMgtPopup #targetmonth").val(0);

    RemoveFlashableTag($("#TblTargetDetails input:text"));//Remove all validation while reset form 

    if (IsUpdateMode) {
        $("#TblTargetDetails input:text").val("");
        $("#TblTargetDetails input:text").each(function () {
            var oldValue = $(this).attr("oldValue");
            $(this).val(oldValue);
        });
    }
    else {
        if (CheckDataExist) {
            var month = $("#ftb-rating #months ul li.selected").attr("value");
            var year = $("#ftb-rating #year ul li.selected").attr("value");
            CommonTargetControlSync(month, year);//If reload existing value in update mode
        }
        else {
            $("#TblTargetDetails input:text").val("");
        }
    }
}

var getCopyFromFlag = function (targetMonth, TargetYear) {
    var rateYear = $("#ftb-rating #year ul li.selected").attr("value");
    var rateMonth = $("#ftb-rating #months ul li.selected").attr("value");
    var flag = true;//user can set up data using copy from than this flag set as true for operation is in create mode/update mode identifier
    if (rateMonth == targetMonth && rateYear == TargetYear) {
        flag = false;
    }
    return flag;
}

var ClosedTargetPopup = function () {
    $("#TargetReservationMgtPopup, .popup_bg").hide();
}

var TextboxValidation = function () {
    $("#TblTargetDetails input:text").on("keyup", function () {
        var txtVal = $.trim($(this).val());
        if (!($.isNumeric(txtVal)) && txtVal != "" && !isPositiveInteger(txtVal)) {
            MakeTagFlashable($(this));
        }
        else if ($.isNumeric(txtVal)) {
            if (!isPositiveInteger(txtVal) && txtVal != "") {
                MakeTagFlashable($(this));
            }
            else {
                RemoveFlashableTag($(this));
            }
        }
        else {
            RemoveFlashableTag($(this));
        }

        //Validation Part on slot defination
        var currentId = $(this).attr("id");
        var slotId = currentId.slice(-1);//console.log($(this).val() + " " + );
        var weekdayId = "";
        var currentTargetValue = "";
        var targetClass = $(this).hasClass("targetInit");
        //To get only target box validation identification
        if (!targetClass) {
            weekdayId = currentId.split('_')[2];
            currentTargetValue = $("#target_" + weekdayId).val();
        }
        else {
            currentTargetValue = $("#target_" + slotId).val();
        }

        var previousSlotId = slotId - 1; //Previoud slot value validation

        //Increase slot validation
        if (("increase_slot_" + weekdayId + "_" + slotId) == currentId) {
            if (1 != slotId) {
                var preTargetValue = parseFloat($("#increase_slot_" + weekdayId + "_" + previousSlotId).val());
                if (preTargetValue != NaN && preTargetValue >= parseFloat(txtVal) && txtVal != "") {
                    MakeTagFlashable($("#increase_slot_" + weekdayId + "_" + previousSlotId));
                }
                else {
                    RemoveFlashableTag($("#increase_slot_" + weekdayId + "_" + previousSlotId));
                }
            }
            else {
                if (txtVal == "" && $("#target_" + weekdayId).val() == "") {
                    RemoveFlashableTag($("#target_" + weekdayId));
                }
                else if (txtVal != "" && $("#target_" + weekdayId).val() == "") {
                    MakeTagFlashable($("#target_" + weekdayId));
                }
            }
            //if ($("#increase_slot_" + weekdayId + "_" + slotId).val() != "" && $("#target_slot_" + weekdayId + "_" + slotId).val() == "") {
            //    MakeTagFlashable($("#target_slot_" + weekdayId + "_" + slotId));
            //}
            //else {
            //    RemoveFlashableTag($("#target_slot_" + weekdayId + "_" + slotId));
            //}
        }
            //target slot validation
        else if (!targetClass && ("target_slot_" + weekdayId + "_" + slotId) == currentId) {
            if (1 != slotId) {
                var preTargetValue = parseFloat($("#target_slot_" + weekdayId + "_" + previousSlotId).val());
                if (preTargetValue != NaN && preTargetValue >= parseFloat(txtVal) && txtVal != "") {
                    MakeTagFlashable($("#target_slot_" + weekdayId + "_" + previousSlotId));
                }
                else {
                    RemoveFlashableTag($("#target_slot_" + weekdayId + "_" + previousSlotId));
                }
            }
            else {
                if (txtVal == "" && $("#target_" + weekdayId).val() == "") {
                    RemoveFlashableTag($("#target_" + weekdayId));
                }
                else if (txtVal != "" && $("#target_" + weekdayId).val() == "") {
                    MakeTagFlashable($("#target_" + weekdayId));
                }
            }
            //if ($("#target_slot_" + weekdayId + "_" + slotId).val() != "" && $("#increase_slot_" + weekdayId + "_" + slotId).val() == "") {
            //    MakeTagFlashable($("#increase_slot_" + weekdayId + "_" + slotId));
            //}
            //else {
            //    RemoveFlashableTag($("#increase_slot_" + weekdayId + "_" + slotId));
            //}
        }
        //Target validation
        if (targetClass && parseInt(txtVal) <= 0) {
            MakeTagFlashable($("#target_" + slotId));
        }
        //else {
        //    RemoveFlashableTag($("#target_" + slotId));
        //}
        AddFlashingEffect();
    });

    //$("#TblTargetDetails input:text").on("keyup", function () {
    //    var currentSlot = $(this).parent("td").attr("slot");
    //    console.log($(this).closest("td").prev("td").attr("slot"));
    //});
}

var checkTargetValidationOnSubmit = function () {
    var flag = true;
    var NotsetTxtVal = false;
    var NotSetValueCount = 0;
    $("#TblTargetDetails .Target").closest("tr").each(function () {
        var targetValue = $(this).find("input:text").eq(0).val();
        var firstSlotValue = $(this).find("input:text").eq(1).val();
        if (targetValue != "" && firstSlotValue == "") {
            MakeTagFlashable($(this).find("input:text").eq(1));
            MakeTagFlashable($(this).find("input:text").eq(2));
        }

        var trCheckPercent = false;
        var weekDayId = $(this).find("td:first span").attr("id");

        $(this).find("input:text").each(function (index) {
            //Check validation all filed should be mandatory
            var CurrentTxtVal = $(this).val();
            if ($.trim(CurrentTxtVal) == "") {
                MakeTagFlashable($(this));
                NotsetTxtVal = true;
            }
            else {
                RemoveFlashableTag($(this));
            }

            var TargetClass = $(this).hasClass("targetInit");//Check target box is set or not
            if (!TargetClass) {
                var currentTextboxId = $(this).attr("id");
                var curentTextboxValue = $.trim($("#" + currentTextboxId).val());
                var slotId = currentTextboxId.slice(-1);
                if (1 != slotId)//check first slot of target
                {
                    var previousSlotId = slotId - 1;
                    var previousTargetSlotValue = $("#target_slot_" + weekDayId + "_" + previousSlotId);
                    var previousIncreaseSlotValue = $("#increase_slot_" + weekDayId + "_" + previousSlotId);

                    if (curentTextboxValue != "") {
                        if (currentTextboxId.indexOf("target_slot_") >= 0) {
                            if ($.trim($(previousTargetSlotValue).val()) != "") {
                                if (parseFloat($(previousTargetSlotValue).val()) >= parseFloat(curentTextboxValue)) {
                                    MakeTagFlashable($(previousTargetSlotValue));
                                }
                                else {
                                    RemoveFlashableTag($(previousTargetSlotValue));
                                }
                            }
                            else {
                                MakeTagFlashable($(previousTargetSlotValue));
                            }
                            //logic check greter or not for target
                            if ($("#target_slot_" + weekDayId + "_" + slotId).val() != "" && $("#increase_slot_" + weekDayId + "_" + slotId).val() == "") {
                                MakeTagFlashable($("#increase_slot_" + weekDayId + "_" + slotId));
                            }
                            else if ($("#target_slot_" + weekDayId + "_" + slotId).val() == "" && $("#increase_slot_" + weekDayId + "_" + slotId).val() != "") {
                                MakeTagFlashable($("#target_slot_" + weekDayId + "_" + slotId));
                            }
                            else {
                                RemoveFlashableTag($("#increase_slot_" + weekDayId + "_" + slotId));
                            }
                        }
                        if (currentTextboxId.indexOf("increase_slot_") >= 0) {
                            if ($.trim($(previousIncreaseSlotValue).val()) != "") {
                                if (parseFloat($(previousIncreaseSlotValue).val()) >= parseFloat(curentTextboxValue)) {
                                    MakeTagFlashable($(previousIncreaseSlotValue));
                                }
                                else {
                                    RemoveFlashableTag($(previousIncreaseSlotValue));
                                }
                            }
                            else {
                                MakeTagFlashable($(previousIncreaseSlotValue));
                            }
                            //logic check greter or not for increase slot
                            if ($("#increase_slot_" + weekDayId + "_" + slotId).val() != "" && $("#target_slot_" + weekDayId + "_" + slotId).val() == "") {
                                MakeTagFlashable($("#target_slot_" + weekDayId + "_" + slotId));
                            }
                            else if ($("#target_slot_" + weekDayId + "_" + slotId).val() == "" && $("#increase_slot_" + weekDayId + "_" + slotId).val() != "") {
                                MakeTagFlashable($("#increase_slot_" + weekDayId + "_" + slotId));
                            }
                            else {
                                RemoveFlashableTag($("#target_slot_" + weekDayId + "_" + slotId));
                            }
                        }
                    }
                }
                else {
                    if (curentTextboxValue == "" && $("#target_" + weekDayId).val() == "") {
                        RemoveFlashableTag($("#target_" + weekDayId));
                    }
                    else if (curentTextboxValue != "" && $("#target_" + weekDayId).val() == "") {
                        MakeTagFlashable($("#target_" + weekDayId));
                    }
                }


                //Add Validation value should not not allow <=0 and >=100 
                var currentSlotID = "#" + currentTextboxId;
                if (currentTextboxId.indexOf("target_slot_") >= 0 && curentTextboxValue != "") {
                    if (parseFloat(curentTextboxValue) > 100) {
                        MakeTagFlashable($("#target_" + weekDayId));
                        //MakeTagFlashable($("#target_slot_" + weekDayId + "_" + slotId));
                        trCheckPercent = true;
                    }
                    else if (parseFloat(curentTextboxValue) <= 0) {
                        MakeTagFlashable($("#target_" + weekDayId));
                        //MakeTagFlashable($("#target_slot_" + weekDayId + "_" + slotId));
                        trCheckPercent = true;
                    }
                    else {
                        if (!trCheckPercent && currentTextboxId.indexOf("target_slot_") >= 0) {
                            if ($("#target_" + weekDayId).val() < 100) {
                                RemoveFlashableTag($("#target_" + weekDayId));
                                //RemoveFlashableTag($("#target_slot_" + weekDayId + "_" + slotId));
                            }
                        }
                    }
                }



                //Onlyn first slot validation if value is not filled then check slot both value is filled or not
                //if (currentTextboxId.indexOf("target_slot_") >= 0 && 1 == slotId) {
                //    if ($("#target_slot_" + weekDayId + "_" + slotId).val() != "" && $("#increase_slot_" + weekDayId + "_" + slotId).val() == "") {
                //        MakeTagFlashable($("#increase_slot_" + weekDayId + "_" + slotId));
                //    }
                //    else if ($("#target_slot_" + weekDayId + "_" + slotId).val() == "" && $("#increase_slot_" + weekDayId + "_" + slotId).val() != "") {
                //        MakeTagFlashable($("#target_slot_" + weekDayId + "_" + slotId));
                //    }
                //    else {
                //        RemoveFlashableTag($("#increase_slot_" + weekDayId + "_" + slotId));
                //    }
                //}
                //if (currentTextboxId.indexOf("increase_slot_") >= 0 && 1 == slotId) {
                //    if ($("#increase_slot_" + weekDayId + "_" + slotId).val() != "" && $("#target_slot_" + weekDayId + "_" + slotId).val() == "") {
                //        MakeTagFlashable($("#target_slot_" + weekDayId + "_" + slotId));
                //    }
                //    else if ($("#target_slot_" + weekDayId + "_" + slotId).val() == "" && $("#increase_slot_" + weekDayId + "_" + slotId).val() != "") {
                //        MakeTagFlashable($("#increase_slot_" + weekDayId + "_" + slotId));
                //    }
                //    else {
                //        RemoveFlashableTag($("#target_slot_" + weekDayId + "_" + slotId));
                //    }
                //}

                //if ("target_slot_" + weekDayId + "_" + slotId == $(this).attr("id")) {
                //    console.log($(this).attr("id") + "----slot no---- " + $(this).attr("id").slice(-1) + " index " + index + "week day " + weekDayId);
                //}
                //else if ("increase_slot_" + weekDayId + "_" + index == $(this).attr("id")) {
                //}
                index = index + 1;
            }
            else {
                //    if (targetValue != "" && $("#target_" + weekDayId).val() != "") {
                //        if (parseFloat(targetValue) <= 100) {
                //            RemoveFlashableTag($("#target_" + weekDayId));
                //        }
                //        else {
                //            MakeTagFlashable($("#target_" + weekDayId));

                //        }
                //}
                index = 1;
            }
        });

        AddFlashingEffect();

        if (targetValue == "") {
            NotSetValueCount = NotSetValueCount + 1;
        }
    });
    if (NotSetValueCount == 7) {
        $("#targeterrormsg").hide();
        $("#targeterrormsgnotfilled").show();
        flag = false;
    }
    if ($("#TblTargetDetails").find(".temp").length > 0) {
        $("#targeterrormsgnotfilled").hide();
        $("#targeterrormsg").show();
        flag = false;
    }
    else {
        $("#targeterrormsgnotfilled").hide();
        $("#targeterrormsg").hide();
    }
    if (NotsetTxtVal) {
        flag = false;
    }
    return flag;
}
//Check only positive number function
function isPositiveInteger(s) {
    //    return !!s.match(/^[0-9]+$/);
    return /^[0-9]\d*(\.\d+)?$/.test(s);
}
//--------------Other functions end------------------


//---ViewModel----
var TargetViewModel = function () {
    var self = this;
    self.TargetDTO = ko.observableArray([]);
    self.FTBRateDTO = ko.observableArray([]);
}
//--End ViewModel---
//-------Entities--------
var FTBTargetDTO = function (data) {
    var self = this;
    self.FTBTargetId = data.FTBTargetId;
    self.LocationBrandId = data.LocationBrandId;
    self.Date = data.Date;
    self.DayOfWeekId = data.DayOfWeekId;
    self.Day = data.Day;
    self.Target = data.Target;
    self.LoggedUserId = data.LoggedUserId;
    self.IsUpdate = ko.computed(function () {
        if (!data.IsUpdate) {
            return "false"
        }
        else {
            return "true";
        }
    });
    self.FTBTargetsDetailDTO = ko.observableArray($.map(data.FTBTargetsDetailDTOs, function (item) { return new FTBTargetsDetailDTO(item); }));
}
var FTBTargetsDetailDTO = function (data) {
    var self = this;
    self.FTBTargetId = data.FTBTargetId;
    self.FTBTargetDetailId = data.FTBTargetDetailId;
    self.PercentTarget = data.PercentTarget;
    self.PercentRateIncrease = data.PercentRateIncrease;
    self.SlotOrder = data.SlotOrder;
}
//------End entity


//--------------Ajax functions ------------------
//get sample grid template of grid 
var getTargetTamplete = function () {
    var ajaxURl = 'FTBRates/GetTargetTemplate/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.GetTargetTemplate;
    }
    $.ajax({
        url: ajaxURl,
        type: 'GET',
        //contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        async: true,
        success: function (data) {
            //targetViewModel.TargetDTO = data;
            var srcs = $.map(data, function (item) { return new FTBTargetDTO(item); });
            targetViewModel.TargetDTO(srcs);

            TextboxValidation(); //textbox validation 
        },
        error: function (XMLHttpRequest, textStatus, errorThrown) {
            console.log(errorThrown);
        }
    });
}
//add and update operation
var SaveTargetDetails = function () {
    var ftbTargetDTO = [];
    var month = $("#ftb-rating #months ul li.selected").attr("value"); //$("#ftb-rating #month").val();
    var year = $("#ftb-rating #year ul li.selected").attr("value");  //$("#ftb-rating #year").val();
    //Prepared model for save target details..
    $("#TblTargetDetails #createMode tr").each(function () {
        var IsTargetValueSet = true;//if Target value set then this flag set as true
        var ftbTargetObject = new Object();
        var parentTD = $(this).find("td");
        var targetID = $(parentTD).eq(0).find("span").attr("FTBTargetId");
        var weekDayId = $(parentTD).eq(0).find("span").attr("id");
        ftbTargetObject.FTBTargetId = targetID;
        ftbTargetObject.LocationBrandId = $("#ftb-rating #ratesettingslocation ul li.selected").attr("value");
        ftbTargetObject.Date = year + "-" + month + "-1";
        ftbTargetObject.DayOfWeekId = weekDayId;
        ftbTargetObject.Target = $(parentTD).eq(1).find("input:text").val();
        ftbTargetObject.LoggedUserId = $.trim($('#LoggedInUserId').val());
        ftbTargetObject.IsUpdate = $(parentTD).eq(0).find("span").attr("IsUpdateMode");
        if (ftbTargetObject.Target == "") {
            IsTargetValueSet = false;
        }

        var SlotOrder = [];
        var currentTr = $(this).find("td");

        for (var i = 1; i <= 5; i++) {
            var target = $(currentTr).find("input[id=target_slot_" + weekDayId + "_" + i + "]").val();
            var Increase = $(currentTr).find("input[id=increase_slot_" + weekDayId + "_" + i + "]").val();
            // console.log(target + " " + Increase);
            //validation part
            //This condition indicate weather target is set or not.
            if (!IsTargetValueSet && target != "" && Increase != "") {
                MakeTagFlashable($(currentTr).find("input[id=target_slot_" + weekDayId + "_" + i + "],[id=increase_slot_" + weekDayId + "_" + i + "]"));
            }

            var SlotOrderObject = new Object();
            SlotOrderObject.FTBTargetDetailId = $(currentTr).find("input[id=target_slot_" + weekDayId + "_" + i + "]").attr("ftbtargetdetailid");
            SlotOrderObject.FTBTargetId = targetID;
            SlotOrderObject.PercentTarget = (target == "") ? null : target;
            SlotOrderObject.PercentRateIncrease = (Increase == "") ? null : Increase;
            SlotOrderObject.SlotOrder = i;
            SlotOrder.push(SlotOrderObject);
        }

        ftbTargetObject.FTBTargetsDetailDTOs = SlotOrder;
        ftbTargetDTO.push(ftbTargetObject)
    });
    var flag = checkTargetValidationOnSubmit();
    //console.log("submit status " + flag);
    //console.log(ftbTargetDTO);
    if (ftbTargetDTO.length > 0 && flag) {
        $("#targeterrormsg,#targeterrormsgnotfilled").hide();

        var ajaxURl = 'FTBRates/SaveTargetDetails/';
        if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
            ajaxURl = AjaxURLSettings.SaveTargetDetails;
        }
        $.ajax({
            url: ajaxURl,
            type: 'POST',
            data: JSON.stringify({ "ftbTargetDTO": ftbTargetDTO }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            success: function (data) {
                if (data) {
                    $("#targetmsg").show();
                    setTimeout(function () { $("#targetmsg").hide(); }, 2000);
                    $("#TargetReservationMgtPopup #targetyear li").eq(0).attr("value", "0").html("--select--");
                    $("#TargetReservationMgtPopup #targetyear ul li[value=0]").addClass("selected").siblings("li").removeClass("selected");
                    $("#TargetReservationMgtPopup #targetmonth li").eq(0).attr("value", "0").html("--select--");
                    $("#TargetReservationMgtPopup #targetmonth ul li[value=0]").addClass("selected").siblings("li").removeClass("selected");
                    //$("#TargetReservationMgtPopup #targetyear").val(0);
                    //$("#TargetReservationMgtPopup #targetmonth").val(0);
                    GetTargetData(month, year, getCopyFromFlag(month, year));
                }
                //var srcs = $.map(data, function (item) { return new Status(item); });
                //searchViewModel.Status(srcs);
                //$("#recentStatuses ul li").eq(0).addClass("selected");
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
    }
}
//get target and targetdetails by selected locationbrand
var GetTargetData = function (Month, Year, isCopyFrom) {
    var locationBrandId = $("#ftb-rating #ratesettingslocation ul li.selected").attr("value");//$("#ftb-rating #LocationBrand option:selected").val();
    var month = Month; //$("#ftb-rating #month option:selected").val();
    var year = Year //$("#ftb-rating #year option:selected").val();
    var isCopyFrom = isCopyFrom;
    //console.log(" Month " + Month + " year " + Year + " copy from " + isCopyFrom);
    var ajaxURl = 'FTBRates/FetchTargetDetails/';
    if (AjaxURLSettings != undefined && AjaxURLSettings != '') {
        ajaxURl = AjaxURLSettings.FetchTargetDetails;
    }
    if (locationBrandId != undefined) {
        $.ajax({
            url: ajaxURl,
            type: 'GET',
            data: {
                locationBrandId: locationBrandId,
                month: month,
                year: year,
                isCopyFrom: isCopyFrom
            },
            //contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            async: true,
            success: function (data) {
                IsUpdateMode = data[0].IsUpdate;
                if (IsUpdateMode)
                { CheckDataExist = true; }//This flag used while reset old value in update mode after doing some copy from operation
                else if (!IsUpdateMode && isCopyFrom && data[0].Target == null) {
                    $("#targetothermsg").show().html("No data found...");
                    setTimeout(function () { $("#targetothermsg").hide() }, 2000);
                }


                $("#TblTargetDetails input:text").val("");//clear all textbox value

                var srcs = $.map(data, function (item) { return new FTBTargetDTO(item); });
                targetViewModel.TargetDTO(srcs);

                TextboxValidation(); //textbox validation
            },
            error: function (XMLHttpRequest, textStatus, errorThrown) {
                console.log(errorThrown);
            }
        });
    }
}
//--------------Ajax functions end ------------------