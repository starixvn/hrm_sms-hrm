var TimeRecord = function ($) {
    return {
        teamIds: null,
        timeRecordContent: null,
        fromDate: null,
        toDate: null,
        employeeId: null,
        employee: null,
        btnSubmit: null,
        init: function () {
            $('#checkin').attr('checked', true);
            TimeRecord.teamIds = $("#TeamIds");
            TimeRecord.timeRecordContent = $("#TimeRecordContent");
            TimeRecord.fromDate = $("#FromDate");
            TimeRecord.toDate = $("#ToDate");
            TimeRecord.employeeId = $("#EmployeeId");
            TimeRecord.employee = $("#Employee");
            TimeRecord.btnSubmit = $("#btnSubmit");
            TimeRecord.IsLocal = $("#IsLocal");

            TimeRecord.teamIds.multiselect();
            TimeRecord.fillData();

            TimeRecord.btnSubmit.on("click", function () {
                TimeRecord.fillData();
            });

            TimeRecord.employee.change(function () {
                if ($.trim(TimeRecord.employee.val()).length == 0) {
                    TimeRecord.employeeId.val("");
                }

                return false;
            });
        },
        fillData: function () {
            var groups = TimeRecord.teamIds.val();
            var groupIds = groups != null ? groups.join() : "";
            var employeeId = TimeRecord.employeeId.val();
            var isLocal = TimeRecord.IsLocal.val();

            var params = {
                "fromDate": TimeRecord.fromDate.val(),
                "toDate": TimeRecord.toDate.val(),
                "employeeId": ((employeeId == 0) ? null : employeeId),
                "groupIds": groupIds,
                "isLocal": isLocal
            };

            if (TimeRecord.timeRecordContent.length > 0) {
                $.get(_global.root + "Report/TimeRecordDetail", params)
                        .done(function (content) {
                            TimeRecord.timeRecordContent.empty();
                            TimeRecord.timeRecordContent.html(content);
                        });
            }
        }
    }
} (jQuery);

$(document).ready(function () {
    TimeRecord.init();
});
