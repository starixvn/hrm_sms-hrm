'use strict';
/*global _global, _messages*/

var LateInEarlyOutRequest = (function ($) {
    return {
        verifierId: null,
        approverId: null,
        successed: null,
        year: null,
        onDate: null,
        
        datePicker: null,
        employeeId: null,
        employee: null,

        modalError: null,
        lateInHours: null,
        earlyOutHours: null,
        requestTypeLateIn: null,
        requestTypeEarlyOut: null,
        lateIn: null,
        earlyOut: null,


        init: function () {
            LateInEarlyOutRequest.verifierId = $('#VerifierId');
            LateInEarlyOutRequest.approverId = $('#ApproverId');
            LateInEarlyOutRequest.successed = $('#successed');
            LateInEarlyOutRequest.year = $('#year');
            LateInEarlyOutRequest.onDate = $('#OnDate');
            LateInEarlyOutRequest.datePicker = $('.datepicker');
            LateInEarlyOutRequest.employeeId = $('#EmployeeId');
            LateInEarlyOutRequest.employee = $("#Employee");

            LateInEarlyOutRequest.modalError = $("#model-state-error");
            LateInEarlyOutRequest.lateInHours = $("#lateInHours");
            LateInEarlyOutRequest.earlyOutHours = $("#earlyOutHours");
            LateInEarlyOutRequest.requestTypeLateIn = $("#RequestType_3");
            LateInEarlyOutRequest.requestTypeEarlyOut = $("#RequestType_4");
            LateInEarlyOutRequest.lateIn = $("#lateIn");
            LateInEarlyOutRequest.earlyOut = $("#earlyOut");

            LateInEarlyOutRequest.initState();
 
            var successed = LateInEarlyOutRequest.successed.val();
            var year = LateInEarlyOutRequest.year.val();
            if (successed === "true") {
                $('form').removeClass("dirty");
                confirm('Successfull!\nPress OK to continue', function () {
                    document.location = _global.root + "LateInEarlyOut/EmployeeView/" + year;
                });
            }

            LateInEarlyOutRequest.requestTypeLateIn.change(function () {
                LateInEarlyOutRequest.showHideActualLateInEarlyOut();
            });

            LateInEarlyOutRequest.requestTypeEarlyOut.change(function () {
                LateInEarlyOutRequest.showHideActualLateInEarlyOut();
            });

            LateInEarlyOutRequest.employee.blur(function () {
                $.getJSON(_global.root + 'employee/GetUnavailableDates', 
                    {
                        employeeId: LateInEarlyOutRequest.employeeId.val(), 
                        date: new Date().toDateString() 
                    },
                    function (response) {
                        if (response !== null) {
                            LateInEarlyOutRequest.onDate.attr("disableddate", response.join(","));
                            LateInEarlyOutRequest.shiftChange();
                            LateInEarlyOutRequest.getActualLateInEarlyOut();
                        }
                    }
                );
            });

            LateInEarlyOutRequest.employee.blur();
            $('.datepicker').datepicker("option", "onSelect", function () {
                LateInEarlyOutRequest.shiftChange();
                LateInEarlyOutRequest.getActualLateInEarlyOut();
            });
        },
        initState: function () {
            LateInEarlyOutRequest.lateIn.hide();
            LateInEarlyOutRequest.earlyOut.hide();
            LateInEarlyOutRequest.modalError.hide();
            LateInEarlyOutRequest.onDate.attr('readonly', 'readonly');
        },
        shiftChange: function () {
            $.getJSON(_global.root + 'shift/getshiftbydate', 
                { 
                    employeeId: LateInEarlyOutRequest.employeeId.val(), 
                    date: LateInEarlyOutRequest.onDate.val() 
                },
                function (data) {
                    if (data !== null) {
                        LateInEarlyOutRequest.modalError.hide();
                        $('input[type=submit]').show();
                    }
                    else {
                        LateInEarlyOutRequest.modalError.show();
                    }
                });
        },
        getActualLateInEarlyOut: function () {
                $.getJSON(_global.root + 'lateinearlyout/GetActualLateInEarlyOut', 
                {
                    employeeId: LateInEarlyOutRequest.employeeId.val(),
                    workDate: LateInEarlyOutRequest.onDate.val()
                },
                function (data) {
                    if (data !== null) {
                        if (data.LateIn > 0) {
                            LateInEarlyOutRequest.lateInHours.html(data.LateIn + "'");
                        }
                        else {
                            LateInEarlyOutRequest.lateInHours.html("N/A");
                        }

                        if (data.EarlyOut > 0) {
                            LateInEarlyOutRequest.earlyOutHours.html(data.EarlyOut + "'");
                        }
                        else {
                            LateInEarlyOutRequest.earlyOutHours.html("N/A");
                        }

                        LateInEarlyOutRequest.showHideActualLateInEarlyOut();
                    }
                });
        },
        showHideActualLateInEarlyOut: function () {
            if (LateInEarlyOutRequest.requestTypeLateIn.attr("checked") === "checked") {
                LateInEarlyOutRequest.lateIn.show();
                LateInEarlyOutRequest.earlyOut.hide();
            }
            else {
                LateInEarlyOutRequest.lateIn.hide();
                LateInEarlyOutRequest.earlyOut.show();
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    LateInEarlyOutRequest.init();
    $('form').areYouSure();
});
