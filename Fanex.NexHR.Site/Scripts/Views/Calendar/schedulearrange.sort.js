'use strict';

var ScheduleArrangeSort = (function ($) {
    return {
        init: function () {
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
} (jQuery));

$(document).ready(function () {
    ScheduleArrangeSort.init();
});
