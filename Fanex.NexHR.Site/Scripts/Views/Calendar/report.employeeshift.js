'use strict';
/*global _global*/

var EmployeeShift = (function ($) {
    return {
        teamId: null,
        workDate: null,
        shiftId: null,
        employeeId: null,
        isLocal: null,
        isShowWholeCompany: null,
        submit: null,

        init: function () {
            EmployeeShift.teamId = $("#TeamId");
            EmployeeShift.workDate = $("#WorkDate");
            EmployeeShift.shiftId = $("#ShiftId");
            EmployeeShift.employeeId = $("#EmployeeId");
            EmployeeShift.employee = $("#Employee");
            EmployeeShift.isLocal = $("#IsLocal");
            EmployeeShift.isShowWholeCompany = $("#IsShowWholeCompany");
            EmployeeShift.submit = $("#btnSubmit");

            EmployeeShift.workDate.inputReadonly();

            EmployeeShift.teamId.multiselect();

            EmployeeShift.bindEmployeeShiftContent();

            EmployeeShift.submit.on("click", function () {
                EmployeeShift.bindEmployeeShiftContent();
            });
        },
        bindEmployeeShiftContent: function () {
            var teams = EmployeeShift.teamId.val();
            var teamIds = teams != null ? teams.join() : "";
            var employeeId = EmployeeShift.employee.val().length > 0 ? EmployeeShift.employeeId.val() : null;
            var shiftId = EmployeeShift.shiftId.val() === "-1" ? null : EmployeeShift.shiftId.val();

            var employeeShiftContainer = $("#employeeshift");
            var params = {
                "workDate": EmployeeShift.workDate.val(),
                "shiftId": shiftId,
                "employeeId": employeeId,
                "teamIds": teamIds,
                "isLocal": EmployeeShift.isLocal.val(),
                "isShowWholeCompany": EmployeeShift.isShowWholeCompany.attr("checked") === "checked"
            };

            if (employeeShiftContainer.length > 0) {
                $.get(_global.root + "Calendar/EmployeeShiftContent", params)
                        .done(function (content) {
                            employeeShiftContainer.empty();
                            employeeShiftContainer.html(content);
                        });
            }
        }
    };
} (jQuery));

$(document).ready(function () {
    EmployeeShift.init();
});