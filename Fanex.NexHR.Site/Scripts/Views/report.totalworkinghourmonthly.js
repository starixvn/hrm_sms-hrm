'use strict';
/*global _global*/

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

    $("#TeamId").multiselect();

    $("#btnSubmit").click(function (e) {
        e.preventDefault();
        submitForm(false);
    }).focus();

    $("#btnExcel").click(function (e) {
        e.preventDefault();
        submitForm(true);
    });

    function submitForm(isExportExcel) {
        var teams = $("#TeamId").val() != null ? $("#TeamId").val().join() : "";
        var employee = $("#Employee").val();
        var employeeId = employee.length > 0 ? $("#EmployeeId").val() : "";
        var isLocal = $("#IsLocal").val();

        var url = _global.root + "report/TotalWorkingHourMonthly?Month=" + $("#Month").val() +
			"&Year=" + $("#Year").val() +
			"&TeamIds=" + teams +
			"&EmployeeId=" + employeeId +
			"&Employee=" + employee +
			"&IsExportExcel=" + isExportExcel + 
			"&isLocal=" + isLocal;

        location.href = url;
    }
});
