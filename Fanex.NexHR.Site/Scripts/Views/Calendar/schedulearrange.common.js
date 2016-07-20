'use strict';

var ScheduleArrangeCommon = (function () {
    return {
        messageComfirm: null,
        hasChangeValue: null,
        currentShiftDate: null,

        init: function () {
            ScheduleArrangeCommon.messageComfirm = "Are you sure to refresh the screen based on your filter condition? " +
                "Data you have entered may not be saved." +
                " </br></br>  Click OK to proceed or Cancel to come back and save first";
            ScheduleArrangeCommon.hasChangeValue = false;
        },
        saveArrangeSchedule: function () {
            var scheduleArranges = [];
            var shiftMultiples = [];
            var employeeId = "";
            var workDate = "";
            var shiftMultipleId = "";
            var isPublic = false;
            var trObjects = $("#tblScheduleArrange tbody tr");
            var tdObjects = [];
            if (trObjects.length > 0) {
                for (var i = 0; i < trObjects.length; i++) {
                    employeeId = $("td.tdemployee", trObjects[i]).first().attr("data-employeeid");
                    // for schedule arrange
                    if (employeeId !== "undefined" && employeeId > 0) {
                        isPublic = $("input[type='checkbox'].chkIsPublic", trObjects[i]).attr("checked") === "checked";
                        shiftMultipleId = $("input[type='text'].multipleShift", trObjects[i]).first().attr("data-shiftid");
                        workDate = $("input[type='text'].multipleShift", trObjects[i]).first().attr("data-date");

                        // for shift multiple
                        shiftMultiples.push({
                            EmployeeId: employeeId,
                            ForDate: workDate,
                            ShiftId: shiftMultipleId.length === 0 ? 0 : shiftMultipleId
                        });

                        tdObjects = $("td.shiftdate", trObjects[i]);
                        for (var j = 0; j < tdObjects.length; j++) {
                            ScheduleArrangeCommon.pushToScheduleArrange(scheduleArranges, tdObjects[j], employeeId, workDate, isPublic);
                        }
                    }
                }

                $.ajax({
                    type: 'POST',
                    dataType: "json",
                    url: '/Calendar/ArrangeSchedule',
                    data: { scheduleArranges: JSON.stringify(scheduleArranges), shiftMultiples: JSON.stringify(shiftMultiples) },
                    success: function (response) {
                        if (response.IsSuccessful) {
                            ScheduleArrangeCommon.showSuccessMsg();
                            ScheduleArrangeCommon.hasChangeValue = false;
                        }
                        else {
                            ScheduleArrangeCommon.showErrorMsg(response.Message);
                        }
                    }
                });
            }
        },
        pushToScheduleArrange: function (scheduleArranges, tdObject, employeeId, WorkDate, isPublic) {
            var workDate = $("input[type='text'].shift", tdObject).first().attr("data-date");
            var shiftId = $("input[type='text'].shift", tdObject).first().attr("data-shiftid");
            var isOffDay = $("input[type='text'].shift", tdObject).first().hasClass("offday");
            var isHoliday = $("input[type='text'].shift", tdObject).first().hasClass("holiday");

            shiftId = (shiftId === "undefined" || shiftId === "-1") ? "" : shiftId;
            isOffDay = isHoliday ? false : isOffDay;

            if (workDate !== "undefined" && Date.parse(workDate)) {
                scheduleArranges.push({
                    EmployeeId: employeeId,
                    WorkDate: workDate,
                    ShiftId: shiftId,
                    IsHoliday: isHoliday,
                    IsOffDay: isOffDay,
                    IsPublic: isPublic
                });
            }
        },
        showSuccessMsg: function () {
            $(".summary").show();
            $(".success").show();
            $(".validation-summary-errors").hide();

            setTimeout(function () {
                $(".summary").hide();
            }, 4000);

            alert('Saved Successfull!\nPress OK to continue');
        },
        showErrorMsg: function (message) {
            $(".summary").show();
            $(".success").hide();
            $(".validation-summary-errors").show();

            setTimeout(function () {
                $(".summary").hide();
            }, 4000);

            alert(message + '\nPress OK to continue');
        },
        showSuccessMsgInRow: function () {
            alert('Saved Successfull!\nPress OK to continue');
        },
        showErrorMsgInRow: function (message) {
            alert(message + '\nPress OK to continue');
        },
        confirmExit: function () {
            if (ScheduleArrangeCommon.hasChangeValue) {
                return "You have unsaved changes on this page. If you leave this page, those changes will be lost.";
            }
        }
    };
}(jQuery));

$(document).ready(function () {
    ScheduleArrangeCommon.init();
});