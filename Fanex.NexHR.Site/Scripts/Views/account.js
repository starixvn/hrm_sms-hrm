'use strict';
/*global _global*/

$(document).ready(function () {
	// Generate new employee code.
	$("#generate").on("click", function () {
		$.ajax({
            url: _global.root + 'employee/GenerateEmployeeCode',
			success: function (data) {
				$("#Code").val("X" + data);
			}
		});
	});
});
