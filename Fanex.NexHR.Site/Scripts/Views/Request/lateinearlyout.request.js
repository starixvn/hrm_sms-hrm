'use strict';
/*global _global*/

function shiftChange() {
    $.getJSON(_global.root + 'api/shift/getshiftbydate', { employeeId: $('#EmployeeId').val(), date: $('#Date').val() },
    function (data) {
        if (data != null) {
            $("#model-state-error").hide();
            $('input[type=submit]').show();
        }
        else {
            $("#model-state-error").show();
            $('input[type=submit]').hide();
        }
    });
}

$(document).ready(function () {
    $("#model-state-error").hide();
    $("#Date").attr('readonly', 'readonly');
    //Process redirect to My late in/earlt out request.
    var successed = $("#successed").val();
    var year = $("#year").val();
    if (successed === "true") {
        $('form').removeClass("dirty");
        confirm('Successfull!\nPress OK to continue', function () {
            document.location = _global.root + "LateInEarlyOut/EmployeeView/" + year;
        });
    }

    $("#Employee").blur(function () {
        $.getJSON(_global.root + 'api/employee/GetUnavailableDates', { employeeId: $('#EmployeeId').val(), date: new Date().toDateString() },
                function (response) {
                    if (response != null) {
                        $("#Date").attr("disableddate", response.join(","));
                        shiftChange();
                    }
                }
            );
    });

    $("#Employee").blur();
    $('.datepicker').datepicker("option", "onSelect", function () {
        shiftChange();
    });

    $("#FromTime, #ToTime").blur(function () {
        var arrFromTime = $("#FromTime").val().split(":");
        var arrToTime = $("#ToTime").val().split(":");

        var fromTime = parseInt(arrFromTime[0] * 60) + parseInt(arrFromTime[1]);
        var toTime = parseInt(arrToTime[0] * 60) + parseInt(arrToTime[1]);
        $("#Minute").val((toTime - fromTime));
    });

    $('form').areYouSure();

});

