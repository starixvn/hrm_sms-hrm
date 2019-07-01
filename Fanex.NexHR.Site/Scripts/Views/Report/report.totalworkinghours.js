'use strict';
/*global _global*/

$(document).ready(function () {
    $("#TeamId").multiselect();

    $("#btnExcel").click(function (e) {
        var teams = $("#TeamId").val() != null ? $("#TeamId").val().join() : "";
        var url = _global.root + "ExportTotalWorkingHour/TotalWorkingHours?FromDate=" + $("#FromDate").val() +
            "&ToDate=" + $("#ToDate").val() +
            "&TeamIds=" + teams +
            "&Employee=" + $("#Employee").val() +
            "&EmployeeId=" + $("#EmployeeId").val() +
            "&DateOption=" + $("#Date").val() +
            "&IsExportExcel=true" +
            "&IsLocal=" + $("#IsLocal").val();

        location.href = url;
    });

    $("#FromDate").on("change", function () {
        var value = $(this).val();
        $("#ToDate").val(value);
    });
});