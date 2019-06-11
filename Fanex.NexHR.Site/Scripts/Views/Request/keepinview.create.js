'use strict';
/*global _global, _messages*/

var KIVRequest = (function ($) {
    return {
        kivType: null,
        kivByDay: null,
        kivByHour: null,
        verifierId: null,
        approverId: null,
        successed: null,
        year: null,
        date: null,
        halfDay: null,
        timeGroup: null,
        datePicker: null,
        employeeId: null,
        shiftId: null,
        shift: null,
        shiftName: null,
        grbHalfDay: null,
        fromTime: null,
        toTime: null,


        employee: null,
        requestType: null,
        requestId: null,
        isTopManager: false,

        init: function () {
            KIVRequest.kivByDay = "1";
            KIVRequest.kivByHour = "2";
            KIVRequest.kivType = $('input[type="radio"][name="KeepInViewType"]');
            KIVRequest.verifierId = $('#VerifierId');
            KIVRequest.approverId = $('#ApproverId');
            KIVRequest.successed = $('#successed');
            KIVRequest.year = $('#year');
            KIVRequest.date = $('#Date');
            KIVRequest.halfDay = $('#HalfDay');
            KIVRequest.timeGroup = $('#timeGroup');
            KIVRequest.datePicker = $('.datepicker');
            KIVRequest.employeeId = $('#EmployeeId');
            KIVRequest.shiftId = $('#ShiftId');
            KIVRequest.shift = $('#Shift');
            KIVRequest.shiftName = $('#shiftName');
            KIVRequest.grbHalfDay = $('#grbHalfDay');
            KIVRequest.fromTime = $('#FromTime');
            KIVRequest.toTime = $('#ToTime');

            KIVRequest.submitSuccessed();

            KIVRequest.initState();

            KIVRequest.kivType.change(function () {
                KIVRequest.kivTypeChange();
            });

            KIVRequest.halfDay.change(function () {
                KIVRequest.bindHalfDay();
            });

            KIVRequest.datePicker.change(function () {
                KIVRequest.dateChange();
            });
        },
        initState: function () {
            KIVRequest.date.attr('readonly', 'readonly');
            KIVRequest.bindHalfDay();
            KIVRequest.dateChange();
            KIVRequest.kivTypeChange();
        },
        bindHalfDay: function () {
            if (KIVRequest.halfDay.prop('checked')) {
                KIVRequest.timeGroup.show();
            }
            else {
                KIVRequest.timeGroup.hide();
            }
        },
        submitSuccessed: function () {
            var successed = KIVRequest.successed.val();
            var year = KIVRequest.year.val();

            if (successed === "true") {
                confirm('Successfull!\nPress OK to continue', function () {
                    document.location = _global.root + "KeepInview/EmployeeView/" + year;
                });
            }
        },
        kivTypeChange: function () {
            var kivType = $(KIVRequest.kivType.selector + ':checked').val();
            if (kivType === KIVRequest.kivByDay) {
                KIVRequest.initStateForKIVByDay();
            }
            else {
                KIVRequest.initStateForKIVByHour();
            }
        },
        initStateForKIVByDay: function () {
            KIVRequest.grbHalfDay.show();

            if (KIVRequest.halfDay.attr('checked') === "checked") {
                KIVRequest.timeGroup.show();
            }
            else {
                KIVRequest.timeGroup.hide();
            }
        },
        initStateForKIVByHour: function () {
            KIVRequest.grbHalfDay.hide();
            KIVRequest.timeGroup.show();
        },
        dateChange: function () {
            $.getJSON(_global.root + 'shift/getshiftbydate', { employeeId: KIVRequest.employeeId.val(), date: KIVRequest.date.val() },
                function (data) {
                    if (data !== null) {
                        KIVRequest.shiftName.text(data.Name);
                        KIVRequest.shift.val(data.Name);
                        KIVRequest.shiftId.val(data.Id);

                        KIVRequest.fromTime.val(data.BeginCheckInTime);
                        KIVRequest.toTime.val(data.BeginCheckOutTime);
 
                        $('input[type=submit]').show();
                    }
                    else {
                        KIVRequest.shiftName.text(_messages.notAvailable);
                        KIVRequest.shift.val(_messages.notAvailable);
                        KIVRequest.shiftId.val('0');
                        //$('input[type=submit]').hide();
                    }
                });
        }
    };
}(jQuery));

$(document).ready(function () {
    KIVRequest.init();
    $('form').areYouSure();
});
