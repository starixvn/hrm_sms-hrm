'use strict';
/*global _global, _messages*/

function toggleHalfDay() {
    if ($('#HalfDay').prop('checked')) {
        $('#timeGroup').show();
    }
    else {
        $('#timeGroup').hide();
    }
}

function bindHalfDay() {
    toggleHalfDay();
}

$(document).ready(function () {
    var successed = $("#successed").val();
    var year = $("#year").val();
    $('form').areYouSure();

    if (successed === "true") {
        confirm('Successfull!\nPress OK to continue', function () {
            document.location = _global.root + "KeepInview/EmployeeView/" + year;
        });
    }

    $("#Date").attr('readonly', 'readonly');

    toggleHalfDay();

    $('#HalfDay').change(function () {
        bindHalfDay();
    });

    $('.datepicker').change(function () {
        $.getJSON(_global.root + 'api/shift/getshiftbydate', { employeeId: $("#EmployeeId").val(), date: $("#Date").val() },
        function (data) {
            if (data != null) {
                $('#shiftName').text(data.Name);
                $('#Shift').val(data.Name);
                $('#ShiftId').val(data.ShiftID);
                $('input[type=submit]').show();
            }
            else {
                $('#shiftName').text(_messages.notAvailable);
                $('#Shift').val(_messages.notAvailable);
                $('#ShiftId').val('0');
                //$('input[type=submit]').hide();
            }
        });
    });
});
