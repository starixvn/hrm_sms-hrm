'use strict';
/*global _global*/
/*exported deleteShift*/

$(document).ready(function () {
    $("#Name").attr("maxlength", "20");
    if ($("#IsUsed").val() === "True") {
        $("#Name").attr("disabled", "disabled");
        $("#HourTimeIn").attr("disabled", "disabled");
        $("#MinuteTimeIn").attr("disabled", "disabled");
        $("#HourTimeOut").attr("disabled", "disabled");
        $("#MinuteTimeOut").attr("disabled", "disabled");
        $("#IsInAfterDay").attr("disabled", "disabled");
        $("#IsOutAfterDay").attr("disabled", "disabled");
    }

    $('form').bind('submit', function () {
        $(this).find(':input').removeAttr('disabled');
    });

    var successed = $("#Successed").val();
    if (successed === "True") {
        confirm('Successfull!\nPress OK to continue', function () {
            document.location = _global.root + "shift/index";
        });
    }
});

function deleteShift(shiftId) {
    confirm('Are you sure you want to delete this shift?', function () {
        $.post(_global.root + 'shift/delete', { shiftId: shiftId }, function (data) {
            if (data === "True") {
                confirm('Successfull!\nPress OK to continue', function () {
                    document.location = _global.root + "shift/index";
                });
            }
            else {
                alert("Delete shift failed. It has already used another");
            }
        });
    });
}