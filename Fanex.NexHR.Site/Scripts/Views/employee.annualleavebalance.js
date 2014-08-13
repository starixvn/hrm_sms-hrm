"use strict";
/*global _global*/

$(document).ready(function () {

	$('#AddBalance').click(function (event) {
		event.preventDefault();
		var $form = $(this);
		$('#LocalLeave').val($('#localLeave').val());
		if ($form.valid()) {
			$.post(_global.root + 'AnnualLeaveBalance/Create', $('form').serialize());
		}
	});
});