function pushToScheduleArrange(scheduleArranges, tdObject, employeeId, WorkDate, isPublic) {
    var workDate = $("input[type='text'].shift", tdObject).first().attr('data-date');
    var shiftId = $("input[type='text'].shift", tdObject).first().attr('data-shiftid');
    var isOffDay = $("input[type='text'].shift", tdObject).first().hasClass('offday');
    var isHoliday = $("input[type='text'].shift", tdObject).first().hasClass('holiday');

    shiftId = (shiftId === 'undefined' || shiftId === '-1') ? '' : shiftId;
    isOffDay = isHoliday ? false : isOffDay;

    if (workDate !== 'undefined' && Date.parse(workDate)) {
        scheduleArranges.push({
            EmployeeId: employeeId,
            WorkDate: workDate,
            ShiftId: shiftId,
            IsHoliday: isHoliday,
            IsOffDay: isOffDay,
            IsPublic: isPublic
        });
    }
}

function saveArrangeSchedule() {
    var scheduleArranges = [];
    var shiftMultiples = [];
    var employeeId = '';
    var workDate = '';
    var shiftMultipleId = '';
    var isPublic = false;
    var trObjects = $('#tblScheduleArrange tbody tr');
    var tdObjects = [];
    if (trObjects.length > 0) {
        for (var i = 0; i < trObjects.length; i++) {
            employeeId = $('td.tdemployee', trObjects[i]).first().attr('data-employeeid');
            // for schedule arrange
            if (employeeId !== 'undefined' && employeeId > 0) {
                isPublic = $("input[type='checkbox'].chkIsPublic", trObjects[i]).attr('checked') === 'checked';
                shiftMultipleId = $("input[type='text'].multipleShift", trObjects[i]).first().attr('data-shiftid');
                workDate = $("input[type='text'].multipleShift", trObjects[i]).first().attr("data-date");

                // for shift multiple
                shiftMultiples.push({
                    EmployeeId: employeeId,
                    ForDate: workDate,
                    ShiftId: shiftMultipleId.length === 0 ? 0 : shiftMultipleId
                });

                tdObjects = $("td.shiftdate", trObjects[i]);
                for (var j = 0; j < tdObjects.length; j++) {
                    pushToScheduleArrange(scheduleArranges, tdObjects[j], employeeId, workDate, isPublic);
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
}

function showSuccessMsg() {
    $('.summary').show();
    $('.success').show();
    $('.validation-summary-errors').hide();

    setTimeout(function () {
        $('.summary').hide();
    }, 4000);

    alert('Saved Successfull!\nPress OK to continue');
}

function showErrorMsg(message) {
    $('.summary').show();
    $('.success').hide();
    $('.validation-summary-errors').show();

    setTimeout(function () {
        $('.summary').hide();
    }, 4000);

    alert(message + '\nPress OK to continue');
}

function showSuccessMsgInRow() {
    alert('Saved Successfull!\nPress OK to continue');
}

function showErrorMsgInRow(message) {
    alert(message + '\nPress OK to continue');
}

function confirmExit() {
    return 'You have unsaved changes on this page. If you leave this page, those changes will be lost.';
}