$(document).ready(function () {
	$("#TeamId").multiselect();
	$("#StartDate, #EndDate").attr('readonly', true);
	$('.week-picker').hide();

	$("#btnSubmit").click(function (e) {
		e.preventDefault();
		submitForm(false);
	}).focus();

	$("#btnExcel").click(function (e) {
		e.preventDefault();
		submitForm(true);
	});

	$(document).click(function (e) {
		var ele = $(e.target) ? $(e.target) : $(e.toElement);
		var acb = ele.parents();
		var elementId = ele.attr("id");
		if (elementId == "weekPickerImg" || elementId == "StartDate" || elementId == "EndDate") {
			$('.week-picker').toggle();
		}
		else {
			var elementParent = ele.parents();
			if (elementParent.hasClass('ui-datepicker-next') || elementParent.hasClass('ui-datepicker-prev') || elementParent.hasClass("hasDatepicker")) {
			}
			else {
				$('.week-picker').hide();
			}
		}
	});

	function submitForm(isExcel) {
		var teams = $("#TeamId").val() != null ? $("#TeamId").val().join() : "";
		var employeeId = $("#EmployeeId").val();
		var isLocal = $("#IsLocal").val();
		var startDate = $("#StartDate").val();
		var endDate = $("#EndDate").val();
		var employee = $("#Employee").val();

		var url = _global.root + "report/TotalWorkingHourWeekly?startDate=" + startDate
		+ "&endDate=" + endDate
		+ "&Employee=" + employee
		+ "&TeamIds=" + teams
		+ "&EmployeeId=" + employeeId
		+ "&isLocal=" + isLocal
		+ "&requestData=" + true
		+ "&isExcel=" + isExcel;

		location.href = url;
	}
});