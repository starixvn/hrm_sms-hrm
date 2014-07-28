'use strict';
/*global _global,autoIndex*/
/*exported deleteMessage*/

var init = function () {
	var effectedDateModel = $('#EffectedDate').val(),
		status = $('#IsDisabled').val();

	if (!effectedDateModel) {
		$('#EffectedDate').val($.datepicker.formatDate('d M, yy', new Date()));
	}

	$('#status').val(status);
};

var deleteMessage = function (messageId) {
	confirm('Are you sure you want to delete this message?', function () {
		$.post(_global.root + 'Message/DeleteMessageByEmployee', { id: messageId }, function () {
			$('#row_' + messageId).remove();
			autoIndex();
		});
	});
};

var detailMessage = function (messageId, isRead) {
	if (!isRead) {
		var jqxhr = $.post(_global.root + 'Message/UpdateReadStatusOfMessage', { id: messageId }, function () {
		}).always(function () {
			window.location.href = '/message/detailByEmployee/' + messageId;
		});
	}
	else {
		window.location = '/message/DetailByEmployee/' + messageId;
		return false;
	}
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
				document.location = _global.root + "Message/viewMessageOfEmployee";
			},
			error: function (response) {
				$("#error").html(response.message);
			}
		});
	});
});