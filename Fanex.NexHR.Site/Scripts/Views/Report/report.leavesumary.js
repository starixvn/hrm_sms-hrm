'use strict';
/*global _global*/

$(document).ready(function () {
    $("#TeamId").multiselect();

    $("form").submit(function (e) {
        e.preventDefault();
        var teams = $("#TeamId").val() != null ? $("#TeamId").val().join() : "";
        var url = _global.root + "report/UnpaidLeaveSummary?year=" + $("#Year").val() +
			"&TeamIds=" + teams + "&Employee=" + $("#Employee").val() +
			"&EmployeeId=" + $("#EmployeeId").val() + "&category=" + $("#Category").val();
        location.href = url;
    });

    $("#btnExcel").click(function () {
        var teams = $("#TeamId").val() != null ? $("#TeamId").val().join() : "";
        var url = _global.root + "export/UnpaidLeaveSummary?year=" + $("#Year").val() +
		"&TeamIds=" + teams + "&Employee=" + $("#Employee").val() + 
		"&EmployeeId=" + $("#EmployeeId").val() + "&category=" + $("#Category").val();
        location.href = url;
    });
});
