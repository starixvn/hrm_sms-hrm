'use strict';
/*global _global*/

function queryDataHrRequestMonitoring(isExcel) {
	var isLocal = $("#category").val();
	if (isLocal === "0") {
		isLocal = 0;
	}
	if (isLocal === "1") {
		isLocal = 1;
	}

	var employeeText = $("#Employee").val();
	var employeeId = $("#EmployeeId").val();
	if (employeeText === "" || employeeText == null) {
		employeeId = null;
	}

	var group = $("#group").val() != null ? $("#group").val().join() : "";
	var maxRequest = $("#MaxRequest").val();
	var currentUserId = $("#CurrentUserId").val();
	var viewRole = $("#ViewRole").val();

	var from = new Date($("#FromDate").val());
	var to = new Date($("#ToDate").val());
	if (from > to) {
		alert("From Date must be less than To Date");
		return;
	}
	else {
		var fromDate = $.datepicker.formatDate('d M, yy', from);
		var toDate = $.datepicker.formatDate('d M, yy', to);
		var template = $("#HrRequestMonitoringTemplate");

		if (isExcel) {
			var url = _global.root + "report/HRRequestMonitoringDetails?currentUserId=" + currentUserId +
			"&fromDate=" + fromDate + "&toDate=" + toDate + "&maxRequest=" + maxRequest + "&empId=" + employeeId +
			"&group=" + group + "&isLocal=" + isLocal + "&isExcel=" + isExcel + "&viewRole=" + viewRole;
			location.href = url;
		}
		else {

			var params = {
				currentUserId: currentUserId,
				fromDate: fromDate,
				toDate: toDate,
				maxRequest: maxRequest,
				empId: employeeId,
				group: group,
				isLocal: isLocal,
				isExcel: isExcel,
				viewRole: viewRole
			};

			$.ajax({
				url: _global.root + "report/HRRequestMonitoringDetails",
				data: params,
				async: false
			}).done(function (response) {
				template.empty();
				template.html(response);
			});
		}
	}
}

$(document).ready(function () {
	queryDataHrRequestMonitoring(false);

	$('#group').multiselect();

	$("#queryData").click(function () {
		queryDataHrRequestMonitoring(false);
	});

	$("#excel").click(function () {
		queryDataHrRequestMonitoring(true);
	});
});