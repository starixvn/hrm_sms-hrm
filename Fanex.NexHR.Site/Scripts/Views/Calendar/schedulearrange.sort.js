var ScheduleArrangeSort = function ($) {
    return {
        btnSave: null,
        btnCancel: null,

        init: function () {
            ScheduleArrangeSort.btnSave = $("#btnSaveSort");
            ScheduleArrangeSort.btnCancel = $("#btnCancel");

            ScheduleArrangeSort.btnSave.on("click", function () {
                var employeeOrders = [];
                var employees = $("#tblSupervisor").find("tbody tr");

                for (var i = 0; i < employees.length; i++) {
                    var employeeId = $(employees[i]).children().eq(2).attr("data-id");
                    var orderCode = $($(employees[i]).find("input.ordercode").first()).val();

                    employeeOrders.push({
                        EmployeeId: employeeId,
                        OrderCode: $.trim(orderCode)
                    });
                }

                $.ajax({
                    type: 'POST',
                    url: _global.root + 'calendar/MemberScheduleInOrder',
                    data: {
                        employeeOrders: JSON.stringify(employeeOrders)
                    },
                    success: function (data) {
                        if (data.errorCode == 0) {
                            //alert("Saved success!");
                            location.href = _global.root + "Calendar/arrangeschedule";
                        }
                        else {
                            alert(data.message);
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
