'use strict';
/*global _global*/

$(document).ready(function () {
    $('#TotalMinutes').prop('readonly', true);
    var successed = $("#successed").val();
    var year = $("#year").val();
    if (successed === "true") {
        confirm('Successfull!\nPress OK to continue', function () {
            document.location = _global.root + "Overtime/EmployeeView/" + year;
        });
    }

    $('#shiftinfo').tooltip({
        track: true,
        delay: 0,
        showURL: false,
        bodyHandler: function () {
            return $('#shifts').html();
        }
    });

    $('#IsOvertimeHours').change(function () {
        var isOvertimeHours = $("#IsOvertimeHours").attr("checked");
        if (isOvertimeHours === "checked") {
            $('#shift-row').hide();
            $('#time-row').show();
            $('#totalhours-row').show();
        }
        else {
            $('#shift-row').show();
            $('#time-row').hide();
            $('#totalhours-row').hide();
        }
    });

    $('#IsOvertimeHours').change();

    $("#FromTime, #ToTime, #IsGrossDay").change(function () {
        var isGrossDay = $("#IsGrossDay").attr("checked");

        var arrFromTime = $("#FromTime").val().split(":");
        var arrToTime = $("#ToTime").val().split(":");

        var fromTime = (parseInt(arrFromTime[0], 10) * 60) + parseInt(arrFromTime[1], 10);
        var toTime = (parseInt(arrToTime[0], 10) * 60) + parseInt(arrToTime[1], 10);
        var totalMunites = 0;
        if (isGrossDay) {
            totalMunites = 1440 + (toTime - fromTime); // 1440 = 24h * 60
        }
        else {
            totalMunites = toTime - fromTime;
        }

        $("#TotalMinutes").val(totalMunites);
    });
});