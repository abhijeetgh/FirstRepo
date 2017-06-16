
$(document).ready(function () {
    $('#UserName').focus();
    $('#BtnLogin').click(function () {
        ValidateInputs();
        AddFlashingEffect();
        if ($('#UserName').hasClass('temp') || $('#Password').hasClass('temp')) {
            return false;
        }
    });

    $('#UserName').bind("input", function () {
        
        if ($.trim($('#UserName').val()) == "") {
            MakeTagFlashable($('#UserName'));
        }
        else {
            RemoveFlashableTag($('#UserName'));
        }
        AddFlashingEffect();
    })

    $('#Password').bind("input", function () {
        if ($.trim($('#Password').val()) == "") {
            MakeTagFlashable($('#Password'));
        }
        else {
            RemoveFlashableTag($('#Password'));
        }
        AddFlashingEffect();
    })
})

function ValidateInputs() {
    if ($.trim($('#UserName').val()) == "") {
        MakeTagFlashable($('#UserName'));
    }
    else {
        RemoveFlashableTag($('#UserName'));
    }

    if ($.trim($('#Password').val()) == "") {
        MakeTagFlashable($('#Password'));
    }
    else {
        RemoveFlashableTag($('#Password'));
    }

}