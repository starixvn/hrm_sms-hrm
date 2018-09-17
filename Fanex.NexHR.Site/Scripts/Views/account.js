'use strict';
/*global _global*/

$(document).ready(function () {
	//Show or hide default calendar.
	var defaultCalendar = $("#dfsCalendar");

	if ($('#IsDefaultSchedule').is(':checked')) {
		defaultCalendar.show();
	} else {
		defaultCalendar.hide();
	}

	// Toggle default schedule.
	$('#IsDefaultSchedule').click(function () {
		defaultCalendar.toggle();
	});

	// Generate new employee code.
	$("#generate").on("click", function () {
		$.ajax({
			url: _global.root + 'employee/GetNewEmployeeCode',
			success: function (data) {
				$("#Code").val("X" + data);
			}
		});
	});
});
