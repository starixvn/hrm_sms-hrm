'use strict';
/*global _global, _messages, zeroPrefix*/

function checkInOrOutInNextDay(requestType) {
    var _constTime = 86400000;
    var attendantDate = new Date($("#AttendantDate").val());
    if (requestType === "true") {
        var requestDate = new Date(attendantDate.getTime() + _constTime);
        $("#ForDate").datepicker().val(requestDate.format("dd mmm, yyyy"));
    }
    else {
        $("#ForDate").datepicker().val(attendantDate.format("dd mmm, yyyy"));
    }
}

function updateZeroPrefix() {
    //add zeroPrefix
    $('input[Name=Hour]').val(zeroPrefix($('input[Name=Hour]').val()));
    $('input[Name=Minute]').val(zeroPrefix($('input[Name=Minute]').val()));
}

function bindingHourAndMinute() {
    if ($('#RequestType_1').attr("checked") === "checked") {
        $('#Hour').val($('#InTime').val().substring(0, 2));
        $('#Minute').val($('#InTime').val().trim().substring(3, 5));
    }
    else {
        $('#Hour').val($('#OutTime').val().substring(0, 2));
        $('#Minute').val($('#OutTime').val().trim().substring(3, 5));
    }
}

//this function for load Edit
function getShiftByDate() {
    $.getJSON(_global.root + 'shift/getshiftbydate', { employeeId: $("#EmployeeId").val(), date: $("#AttendantDate").val() },
    function (data) {
        if (data != null) {
            $("#model-state-error").hide();
            $('#shiftname').text(data.Name);
            $('#Shift').val(data.Name);
            $('#InTime').val(data.BeginCheckInTime);
            $('#OutTime').val(data.BeginCheckOutTime);
            $('#CrossDay').val(data.CrossDay);
            $('#IsInAfterDay').val(data.IsInAfterDay);
            $('#IsOutAfterDay').val(data.IsOutAfterDay);
            $('input[type=submit]').show();
        }
        else {
            $("#model-state-error").show();
            $('#shiftname').text(_messages.notAvailable);
            $('#Shift').val(_messages.notAvailable);
            //$('input[type=submit]').hide();
        }
    });
}

function checkCrossDay() {
    var IsInAfterDay = $('#IsInAfterDay').val();
    var IsOutAfterDay = $('#IsOutAfterDay').val();

    if ($("#RequestType_1").attr("checked") === "checked") {
        checkInOrOutInNextDay(IsInAfterDay);
    }

    if ($("#RequestType_2").attr("checked") === "checked") {
        checkInOrOutInNextDay(IsOutAfterDay);
    }

    $("#RequestType_1").change(function () {
        checkInOrOutInNextDay(IsInAfterDay);
    });

    $("#RequestType_2").change(function () {
        checkInOrOutInNextDay(IsOutAfterDay);
    });
}

$(document).ready(function () {
    $("#model-state-error").hide();
    $("#ForDate").attr('readonly', 'readonly');
    $("#AttendantDate").attr('readonly', 'readonly');

    $('#shiftinfo').tooltip({
        track: true,
        delay: 0,
        showURL: false,
        bodyHandler: function () {
            return $('#shifts').html();
        }
    });

    $("form").submit(function (e) {
        var attendantDate = new Date($("#AttendantDate").val());
        var requestDate = new Date($("#ForDate").val());
        var dayDiff = (requestDate - attendantDate) / 1000 / 60 / 60 / 24;
        var IsInAfterDay = $('#IsInAfterDay').val();
        var IsOutAfterDay = $('#IsOutAfterDay').val();

        if ($("#RequestType_1").attr("checked") === "checked") {
            checkConstraintDate(e, IsInAfterDay, dayDiff);
        }

        if ($("#RequestType_2").attr("checked") === "checked") {
            checkConstraintDate(e, IsOutAfterDay, dayDiff);
        }

    });

    var checkConstraintDate = function (event, requesType, dayDiff) {
        if (requesType === "true") {
            if (dayDiff <= 0) {
                event.preventDefault();
                alert('Cross date shift. Request date must be greater than Attendant date!');
            }
        }
        else {
            if (dayDiff < 0) {
                event.preventDefault();
                alert('Request date must be equal or greater than Attendant date!');
            }
        }
    };

    $("#Employee").blur(function () {
        $.getJSON(_global.root + 'employee/GetUnavailableDates', { employeeId: $('#EmployeeId').val(), date: new Date().toDateString() },
                function (response) {
                    if (response != null) {
                        $("#ForDate").attr("disabledDate", response.join(","));
                        $("#AttendantDate").attr("disabledDate", response.join(","));
                        $('#AttendantDate').change();
                    }
                }
            );
    });

    $('#AttendantDate').change(function () {
        $.getJSON(_global.root + 'shift/getshiftbydate', { employeeId: $("#EmployeeId").val(), date: $("#AttendantDate").val() },
        function (data) {
            if (data != null) {
                $("#model-state-error").hide();
                $('#shiftname').text(data.Name);
                $('#Shift').val(data.Name);
                $('#InTime').val(data.BeginCheckInTime);
                $('#OutTime').val(data.BeginCheckOutTime);
                $('#CrossDay').val(data.CrossDay);
                $('#IsInAfterDay').val(data.IsInAfterDay);
                $('#IsOutAfterDay').val(data.IsOutAfterDay);
                bindingHourAndMinute();
                $('input[type=submit]').show();
                checkCrossDay();
            }
            else {
                $("#model-state-error").show();
                $('#shiftname').text(_messages.notAvailable);
                $('#Shift').val(_messages.notAvailable);
               // $('input[type=submit]').hide();
            }
        });
    });

    $('#RequestType_1, #RequestType_2').change(function () {
        bindingHourAndMinute();
    });

    if ($('#RequestType_1').val() !== undefined) {
        $("#Employee").blur();
    }
    else {
        getShiftByDate();
    }

    //Process redirect to Check In/Out request.
    var successed = $("#successed").val();
    var year = $("#year").val();

    if (successed === "true") {
        $('form').removeClass("dirty");
        confirm('Successfull!\nPress OK to continue', function () {
            document.location = _global.root + "checkinout/EmployeeView/" + year;
        });
    }

    updateZeroPrefix();

    //check dirty
    $('form').areYouSure();
});

