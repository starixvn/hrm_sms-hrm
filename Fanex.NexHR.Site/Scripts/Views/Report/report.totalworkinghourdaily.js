'use strict';

$(document).ready(function () {
    $("#TeamId").multiselect();

    $("#btnExcel").click(function (e) {
        e.preventDefault();

        var teams = $("#TeamId").val() != null ? $("#TeamId").val().join() : "";
        var employeeId = $("#EmployeeId").val();
        var isLocal = $("#IsLocal").val();
        var workDate = $("#WorkDate").val();
        var employee = $("#Employee").val();

        var url = _global.root + "report/TotalWorkingHourDaily?workDate=" + workDate +
		"&Employee=" + employee +
		"&TeamIds=" + teams +
		"&EmployeeId=" + employeeId +
		"&isLocal=" + isLocal +
		"&isExportExcel=" + true;

        location.href = url;
    });
});