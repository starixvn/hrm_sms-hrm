'use strict';
/*global _global*/

function exportExcel() {
	var year = $("#Year").val();
	var month = $("#Month").val();
	var calendar = $("#calendar").val();
	var employeeId = $("#EmployeeId").val();
	var url = null;

	url = _global.root + "export/EmployeeOvertimeReport?year=" + year + "&month=" + month +
		"&calendar=" + calendar + "&empId=" + employeeId;
	location.href = url;
}

$(document).ready(function () {
    var dateRangePicker = $('#DateControl');

    dateRangePicker.monPickr({
        showDateRange: false,
        sortDesc: true,
        minDate: $("#MinDate").val(),
        maxDate: $("#MaxDate").val(),
        today: undefined,
        month: undefined,
        afterChangeMonth: function () {
            var Month = dateRangePicker.monPickr("getMonth", "MM");
            var Year = dateRangePicker.monPickr("getMonth", "yyyy");
            $('#Month').val(Month);
            $('#Year').val(Year);
        }
    });

    $(".monpickr-dropdown").hide();

    $("#btnExcel").click(function () {
        exportExcel();
    });

});