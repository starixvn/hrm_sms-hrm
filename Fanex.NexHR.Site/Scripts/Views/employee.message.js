'use strict';
/*global _global,autoIndex*/
/*exported deleteMessage*/

var init = function () {
	var effectedDateModel = $('#EffectedDate').val();

	if (!effectedDateModel) {
		$('#EffectedDate').val($.datepicker.formatDate('d M, yy', new Date()));
	}
};

var deleteMessage = function (messageId) {
	confirm('Are you sure you want to delete this message?', function () {
		$.post(_global.root + 'Message/DeleteMessageByEmployee', { id: messageId }, function () {
			$('#row_' + messageId).remove();
			autoIndex();
		});
	});
};

$(document).ready(function () {
	init();

	$("#submit").submit(function (event) {
		var form = $(this);
		event.preventDefault();

		$.ajax({
			url: form.action,
			type: "Post",
			data: form.serialize(),
			success: function () {
				document.location = _global.root + "Message/viewMessageOfEmployee";
			},
			error: function (response) {
				$("#error").html(response.message);
			}
		});
	});
});