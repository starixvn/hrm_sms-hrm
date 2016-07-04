'use strict';
/*global _global*/

$(document).ready(function () {
    var successed = $("#Successed").val();
    if (successed === "True") {
        confirm('Successfull!\nPress OK to redirect to Shift Management.', function () {
            document.location = _global.root + "shift/index";
        });

        $("#Name").val("");
        $("#HourTimeIn").val(0);
        $("#MinuteTimeIn").val(0);
        $("#HourTimeOut").val(0);
        $("#MinuteTimeOut").val(0);
        $("#IsInAfterDay").prop("checked", false);
        $("#IsOutAfterDay").prop("checked", false);
    }
});