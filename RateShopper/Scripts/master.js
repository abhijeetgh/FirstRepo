
$.ajaxSetup({ cache: false });

$(document).ready(function () {

    if ($.trim($('#LoggedInUserId').val()) == '' && window.location.href.toLowerCase().indexOf('login') < 0) {
        window.location.href = MasterAjaxURL.GetLoginUrl;
    }

    $(document).on("keydown", function (e) {
        if (e.which === 8 && !$(e.target).is("input:text, input:password,input[type=number]")) {
            return false;
        }
    });

    $('#aLogout').css('cursor', 'pointer').click(function () {
        $('#LoggedInUserId').val('');
        //remove session storage for keep user state for search grid
        sessionStorage.removeItem('ctrlState');
        sessionStorage.removeItem('FTBctrlState');
        sessionStorage.removeItem('AutomationctrlState');
        $('#logoutForm').submit();

    });

    if ($('#footer-Ulinks a').length == 0) {
        $('#footer-links a').each(function () {
            if ($(this).attr('href') == location.pathname) {
                $(this).addClass('navigationOnSelected');
            }
        })
    }
    else {
        $('#footer-Ulinks a').each(function () {
            if ($(this).attr('href') == location.pathname) {
                $(this).addClass('navigationOnSelected');
                parentNav = $(this).attr('parentnav');
                $('#footer-links a').each(function () {
                    if ($(this).attr('currentnav') == parentNav) {
                        $(this).addClass('navigationOnSelected');
                    }
                });
            }

            if (location.pathname.toLowerCase().indexOf('applyruleset') > 0) {
                $('.subruleC a').eq(0).addClass('navigationOnSelected');
            }
        });
    }

    $('#spnUserName').css('cursor', 'pointer').click(function () {
        $('.logout').toggle(500);
    });

    $('.profile').bind('click', function () {
        if (confirm('Do you really want to Logout?')) {
            document.getElementById('logoutForm').submit();
        }
    });

    //$(".hidden, .select-ul, .table-ul-right ul").hover(function () {
    //    $(this).show();
    //}, function () {
    //    $('.hidden').hide();
    //});
    $(".subruleC, .subrules").hover(function () {
        $('.subrules').show();
    }, function () {
        var isChild = true;
        $('.subrules').hover(function () {
            $('.subrules').show();
            isChild = false;
        }, function () {
            $('.subrules').hide();
        });

        setTimeout(function () {
            if (isChild) {
                $('.subrules').hide();
            }
        }, 500);
    });

    $(".select-ul").bind("click", function () {
        var $itemClicked = $(this);
        if ($itemClicked.children('ul').css('display') == "none") {
            $('.hidden').hide();
        }

        $itemClicked.children('ul').toggle().find('li').bind('click', function () {
            $(this).closest('.select-ul').find('li').removeClass('selected');
            $(this).addClass('selected').closest('.select-ul').find('li').eq(0).text($(this).text()).attr('value', ($(this).attr('value')));
            setTimeout(function () { $('.hidden').hide(); }, 200);
        });
    });

    $(".table-ul-right ul").bind("click", function () {
        var $itemClicked = $(this);
        if ($itemClicked.children('ul').css('display') == "none") {
            $('.hidden').hide();
        }

        $itemClicked.children('ul').toggle().find('li').bind('click', function () {
            $(this).closest('.table-ul-right ul').find('li').removeClass('selected');
            $(this).addClass('selected');
            $itemClicked.find('li').eq(0).text($(this).text()).attr('value', ($(this).attr('value')));
            setTimeout(function () { $('.hidden').hide(); }, 200);
        });
    });
    //This click event is fix all drop down control hide when user click outside the control
    $(document).click(function (e) {
        if (!$(e.target).closest(".hidden, .select-ul, .table-ul-right ul").length) {
            $('.hidden').hide();
        }
    });
    //This click event is add to fix the ul-li doesn't hide issue, if user clicks on any other div or control
    if (/ipad|iphone/i.test(navigator.userAgent)) {
        $("#main").bind('click', function () { });
    }
})


//Add Smart search functionality [Need to pass input text selector & selector of the tags where the smart search will be applicable]
function SmartSearch(inpuTextSelector, controlIdSelector) {
    var $inpuTextSelector = $(inpuTextSelector).val();
    if ($inpuTextSelector.length > 0) {
        $(controlIdSelector).each(function () {
            if ($(this).text().trim().toLowerCase().indexOf($inpuTextSelector.toLowerCase()) == 0) {
                $(this).show();
            }
            else {
                $(this).hide();
            }
        });
    } else {
        $(controlIdSelector).show();
    }
}

//Pass the HTML tag which needs to be flashable
function MakeTagFlashable(selectorTag) {
    $(selectorTag).addClass('temp').addClass('flashBorder');
}

//Remove the flashable tag
function RemoveFlashableTag(selectorTag) {
    $(selectorTag).removeClass('temp').removeClass('flashBorder');
}

//Add flasing effect
var flash = false;
setInterval(function () {
    if (flash) {
        $('.temp').addClass('flashBorder');
        flash = false;
    }
    else {
        $('.temp').removeClass('flashBorder');
        flash = true;
    }
}, 500);
function AddFlashingEffect() {

    //This code is removed, as we have added code above this method to sync the flasing.

    //for (var i = 1; i < 999; i++) {
    //    window.clearInterval(i);
    //    $('.temp').removeClass('flashBorder');
    //}
    //setInterval(function () {
    //    $('.temp').toggleClass('flashBorder');
    //}, 500);
}

function commaSeparateNumber(val) {

    if (val != null && val != '') {
        while (/(\d+)(\d{3})/.test(val.toString())) {
            val = val.toString().replace(/(\d+)(\d{3})/, '$1' + ',' + '$2');
        }
    }
    return val;
    //return val.toLocaleString('en');
}
function commaRemovedNumber(val) {
    if (val != null && val != '') {
        return val.replace(/\,/g, '');
    }
    else {
        return val;
    }
}

function ShowConfirmBox(message, isConfirmBox, functionName, id) {
    $('.popup_bg_master').show();
    $('#alertbox').show().draggable().find('.padding15 #ConfirmText').html(message).closest('#alertbox').find('#ConfirmCancel, #closepopup').click(function () {
        $('#alertbox, .popup_bg_master').hide();
    }).closest('#alertbox').find('#ConfirmOk').unbind('click').bind('click', function () {
        $('#alertbox, .popup_bg_master').hide();
        if (functionName != undefined) {
            functionName.call(id);
        }
    });

    isConfirmBox ? $('#alertbox #ConfirmCancel').show() : $('#alertbox #ConfirmCancel').hide();
}

function callMe() {
    alert(this);
}

function convertToServerTime(date) {
    if (isNaN(Date.parse(date))) {
        //return date;
        date = new Date(parseInt(date));
    }
    //return date;
    //For US
    //Server_TZO = -240;
    //new Date().getTimezoneOffset() = 240

    //For india
    //Server_TZO = 330;
    //new Date().getTimezoneOffset() = -330

    myNewDate = new Date(date.getTime() + (60000 * (new Date().getTimezoneOffset() + Server_TZO)));
    return (myNewDate);
}

function scrollUserTop() {
    if (window.navigator.userAgent.indexOf("MSIE") > 0 || window.navigator.userAgent.indexOf("Firefox") > 0 || navigator.userAgent.match(/Trident.*rv\:11\./)) {
        $('html').scrollTop(0);
    }
    else {
        $('body').scrollTop(0);
    }
}