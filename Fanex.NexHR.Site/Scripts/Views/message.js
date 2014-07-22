'use strict';
/*global _global,autoIndex*/
/*exported deleteMessage*/

var deleteMessage = function (messageId) {
	confirm('Are you sure you want to delete this message?', function () {
		$.post(_global.root + 'Message/Delete', { id: messageId }, function () {
			$('#row_' + messageId).remove();
			autoIndex();
		});
	});
};

var init = function () {
	var fromDateModel = $('#FromDate').val(),
		toDateModel = $('#ToDate').val(),
		status = $('#IsDisabled').val(),
		toDate = new Date(),
		fromDate = new Date();
	fromDate.setMonth(fromDate.getMonth() - 3);

	if (!fromDateModel) {
		$('#FromDate').val($.datepicker.formatDate('d M, yy', fromDate));
	}
	if (!toDateModel) {
		$('#ToDate').val($.datepicker.formatDate('d M, yy', toDate));
	}

	$('#status').val(status);
};

$(document).ready(function () {
	init();

	$("#submit").click(function () {
		var form = $(this);
		$('#IsDisabled').val($('#status').val());

		$.ajax({
			url: form.action,
			type: "Post",
			data: form.serialize(),
			success: function () {
				document.location = _global.root + "Message/Index";
			},
			error: function (response) {
				$("#error").html(response.message);
			}
		});
	});
});