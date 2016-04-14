'use strict';
/*global _global*/

$(document).ready(function () {
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

    function getActualLateInEarlyOut() {
        $.getJSON(_global.root + 'lateinearlyout/GetActualLateInEarlyOut', { employeeId: $('#EmployeeId').val(), workDate: $('#Date').val() },
        function (data) {
            if (data != null) {
                if (data.LateIn > 0) {
                    $("#lateInHours").html(data.LateIn + "'");
                }
                else {
                    $("#lateInHours").html("N/A");
                }

                if (data.EarlyOut > 0) {
                    $("#earlyOutHours").html(data.EarlyOut + "'");
                }
                else {
                    $("#earlyOutHours").html("N/A");
                }

                showHideActualLateInEarlyOut();
            }
        });
    }

    function showHideActualLateInEarlyOut() {
        if ($("#RequestType_3").attr("checked") === "checked") {
            $("#lateIn").show();
            $("#earlyOut").hide();
        }
        else {
            $("#lateIn").hide();
            $("#earlyOut").show();
        }
    }

    $("#lateIn").hide();
    $("#earlyOut").hide();
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

    $(".radioButton").change(function () {
        showHideActualLateInEarlyOut();
    });

    $("#Employee").blur(function () {
        $.getJSON(_global.root + 'api/employee/GetUnavailableDates', { employeeId: $('#EmployeeId').val(), date: new Date().toDateString() },
                function (response) {
                    if (response != null) {
                        $("#Date").attr("disableddate", response.join(","));
                        shiftChange();
                        getActualLateInEarlyOut();
                    }
                }
            );
    });

    $("#Employee").blur();
    $('.datepicker').datepicker("option", "onSelect", function () {
        shiftChange();
        getActualLateInEarlyOut();
    });

    $("#FromTime, #ToTime").blur(function () {
        var arrFromTime = $("#FromTime").val().split(":");
        var arrToTime = $("#ToTime").val().split(":");

        var fromTime = (parseInt(arrFromTime[0]) * 60) + parseInt(arrFromTime[1]);
        var toTime = (parseInt(arrToTime[0]) * 60) + parseInt(arrToTime[1]);
        $("#Minute").val((toTime - fromTime));
    });

    $('form').areYouSure();

});

