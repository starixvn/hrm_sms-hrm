"use strict";
/*global _global*/

function exportExcel() {
    var url = null;
    var employeeText = $('#Employee').val();
    var employeeId = $('#EmployeeId').val();
    if (employeeText === '' || employeeText === null) {
        employeeId = null;
    }
    var isLocal = $('#category').val();
    if (isLocal === '0') {
        isLocal = 0;
    }
    if (isLocal === '1') {
        isLocal = 1;
    }

    var from = new Date($('#FromDate').val());
    var to = new Date($('#ToDate').val());

    if (from > to) {
        alert('From Date must be less than To Date');
        return;
    }
    else {
        var fromDate = $.datepicker.formatDate('d M, yy', from);
        var toDate = $.datepicker.formatDate('d M, yy', to);
        var group = $('#group').val() !== null ? $('#group').val().join() : '';

        url = _global.root + 'Report/WorkingCalendarSummary?fromDate=' + fromDate + '&toDate=' + toDate + '&teamIds=' + group +
            '&employeeId=' + employeeId + '&isLocal=' + isLocal + '&isExportExcel=' + true;
        location.href = url;
    }
}

$(document).ready(function () {
    $('#group').multiselect();
    $('#title').multiselect({
        noneSelectedText: 'Select positions',
        selectedText: '# selected',
        selectedList: 0
    });

    var defaultFromDate = $('#defaultFromDate').val();
    var defaultToDate = $('#defaultToDate').val();
    $('#FromDate').val(defaultFromDate);
    $('#ToDate').val(defaultToDate);

    $('#FromDate').on('change', function () {
        var value = $(this).val();
        $('#ToDate').val(value);
    });
    $('#excelCalendarSummary').click(function () {
        exportExcel();
    });
});