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

$(function () {
	var startDate;
	var endDate;

	var selectCurrentWeek = function () {
		window.setTimeout(function () {
			$('.week-picker').find('.ui-datepicker-current-day a').addClass('ui-state-active')
		}, 1);
	}

	$('.week-picker').datepicker({
		showOtherMonths: true,
		selectOtherMonths: true,
		dateFormat: "dd M, yy",
		firstDay: 1,
		showOn: "button",
		buttonImageOnly: true,
		buttonImage: "../../images/calendar.png",
		onSelect: function (dateText, inst) {
			var date = $(this).datepicker('getDate');
			startDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() - date.getDay() + 1);
			endDate = new Date(date.getFullYear(), date.getMonth(), date.getDate() - date.getDay() + 7);
			var dateFormat = inst.settings.dateFormat || $.datepicker._defaults.dateFormat;
			$('#StartDate').val($.datepicker.formatDate(dateFormat, startDate, inst.settings));
			$('#EndDate').val($.datepicker.formatDate(dateFormat, endDate, inst.settings));
			selectCurrentWeek();
			$('.week-picker').hide();
		},
		beforeShowDay: function (date) {
			var cssClass = '';
			if (date >= startDate && date <= endDate)
				cssClass = 'ui-datepicker-current-day';
			return [true, cssClass];
		},
		onChangeMonthYear: function (year, month, inst) {
			selectCurrentWeek();
		}
	});

	$('.week-picker .ui-datepicker-calendar tr').live('mousemove', function () { $(this).find('td a').addClass('ui-state-hover'); });
	$('.week-picker .ui-datepicker-calendar tr').live('mouseleave', function () { $(this).find('td a').removeClass('ui-state-hover'); });
	$('.week-picker .ui-datepicker').addClass('custom-weekpicker');
});