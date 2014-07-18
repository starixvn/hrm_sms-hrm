$(document).ready(function () {
	$('#group').multiselect();
	$("#title").multiselect({
		noneSelectedText: 'Select positions',
		selectedText: '# selected',
		selectedList: 0
	});
	//submitForm(false);

	$("#submit").click(function () {
		submitForm(false);
	});
	$("#excel").click(function () {
		submitForm(true);
	});
});

function submitForm(isExcel) {
	var url;
	var employeeText = $("#Employee").val();
	var employeeId = $("#EmployeeId").val();
	if (employeeText === "" || employeeText == null) {
		employeeId = null;
	}

	var group = $("#group").val() != null ? $("#group").val().join() : "";
	var supervisorType = $("#Supervisor").val();

	var isLocal = $("#category").val();
	if (isLocal === "0") {
		isLocal = 0;
	}
	if (isLocal === "1") {
		isLocal = 1;
	}
	if (isExcel) {
		url = _global.root + "report/EmployeeSupervisorDetails?employeeId=" + employeeId + "&groups=" + group + "&supervisorType=" + supervisorType
		+ "&isExcel=" + isExcel + "&isLocal=" + isLocal;

		location.href = url;
	}
	else {
		var template = $("#EmployeeSupervisorDetails");
		var params = {
			employeeId: employeeId,
			groups: group,
			supervisorType: supervisorType,
			isExcel: isExcel,
			isLocal: isLocal
		};

		$.ajax({
			url: _global.root + "report/EmployeeSupervisorDetails",
			data: params,
			async: false
		}).done(function (response) {
			template.empty();
			template.html(response);
		});
	}
}