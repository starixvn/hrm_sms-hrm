'use strict';
/*global _global*/

$(document).ready(function () {
	$("#TeamId").multiselect();

	$("form").submit(function (e) {
		e.preventDefault();
		$("#TeamId").val() != null ? $("#TeamId").val().join() : "";
		var postToUrl = "AnnualLeaveBalance/index?year=";
		var url = submitData(postToUrl);
		location.href = url;
	});

	$("#btnExcel").click(function () {
		var postToUrl = "export/leavesummary?year=";
		var url = submitData(postToUrl);
		location.href = url;
	});

	var submitData = function (postToUrl) {
		var teams = $("#TeamId").val() != null ? $("#TeamId").val().join() : "";
		var url = _global.root + postToUrl + $("#Year").val() +
		"&TeamIds=" + teams + "&Employee=" + $("#Employee").val() +
		"&EmployeeId=" + $("#EmployeeId").val() +
		"&MinAnnualBalance=" + $("#MinAnnualBalance").val() +
		"&Category=" + $("#Category").val();

		return url;
	};
});
