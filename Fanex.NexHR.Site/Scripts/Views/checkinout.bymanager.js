'use strict';

$(document).ready(function () {
	$("#FromDate, #ToDate").datepicker({
		changeMonth: true,
		changeYear: true,
		dateFormat: "dd M, yy"
	});
});