var ScheduleArrangeSort = function ($) {
    return {
        btnSave: null,
        btnCancel: null,

        init: function () {
            ScheduleArrangeSort.btnSave = $("#btnSaveSort");
            ScheduleArrangeSort.btnCancel = $("#btnCancel");

            ScheduleArrangeSort.btnSave.on("click", function () {
                var employeeIds = "";
                var employees = $("#tblSupervisor").find("tbody tr");

                for (var i = 0; i < employees.length; i++) {
                    var employeeTR = $(employees[i]).children().eq(1);
                    var employeeId = employeeTR.attr("data-id");
                    employeeIds += employeeId + ",";
                }

                $.ajax({
                    type: 'POST',
                    url: _global.root + 'calendar/MemberScheduleInOrder',
                    data: {
                        employeeIds: employeeIds
                    },
                    success: function (data) {
                        if (data) {
                            //alert("Saved success!");
                            location.href = _global.root + "Calendar/arrangeschedule";
                        }
                        else {
                            alert("An error occurred!");
                        }
                    }
                });
            });

            ScheduleArrangeSort.btnCancel.on("click", function () {
                //$("#dialogSortcontent").dialog('close');
                //$(".ui-dialog").find(".dialog").dialog("close");
            });

            $(".moveTop, .up, .down").on("click", function () {
                var firstRowInTable = $("#tblSupervisor tbody tr a.moveTop:first");
                var firstRow = $("#tblSupervisor tbody tr:first");
                var currentRow = $(this).parents("tr:first");

                if ($(this).is(".up")) {
                    currentRow.insertBefore(currentRow.prev());
                }
                else if ($(this).is(".down")) {
                    currentRow.insertAfter(currentRow.next());
                }
                else if ($(this).is(".moveTop")) {
                    if ($(this).parent().get(0) === firstRowInTable.parent().get(0)) {
                        return false;
                    }
                    else {
                        currentRow.insertBefore(firstRow);
                    }
                }
            });
        }
    };
} (jQuery);

$(document).ready(function () {
    ScheduleArrangeSort.init();
});
