$(document).ready(function () {
	$('#group').multiselect();
	$("#title").multiselect({
		noneSelectedText: 'Select positions',
		selectedText: '# selected',
		selectedList: 0
	});

	var defaultFromDate = $("#defaultFromDate").val();
	var defaultToDate = $("#defaultToDate").val();
	$("#FromDate").val(defaultFromDate);
	$("#ToDate").val(defaultToDate);

	$("#excel").click(function () {
		exportExcel();
	});
});

function exportExcel() {
	var url = null;
	var employeeText = $("#Employee").val();
	var employeeId = $("#EmployeeId").val();
	if (employeeText == "" || employeeText == null) {
		employeeId = null;
	}
	var isLocal = $("#category").val();
	if (isLocal == "0") {
		isLocal = 0;
	}
	if (isLocal == "1") {
		isLocal = 1;
	}

	var from = new Date($("#FromDate").val());
	var to = new Date($("#ToDate").val());

	var fromDate = $.datepicker.formatDate('d M, yy', from);
	var toDate = $.datepicker.formatDate('d M, yy', to);
	var group = $("#group").val() != null ? $("#group").val().join() : "";
	var title = $("#title").val() != null ? $("#title").val().join() : "";
	
	debugger;

	url = _global.root + "export/WorkingCalendarSummary?fromDate=" + fromDate + "&toDate=" + toDate
                        + "&group=" + group + "&title=" + title + "&employeeId" + employeeId + "&isLocal" + isLocal;
	location.href = url;
}