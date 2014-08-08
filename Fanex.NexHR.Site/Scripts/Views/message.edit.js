'use strict';
/*global _global*/

var bindingDataIfEditMode = function () {
	var messageId = $('#messageId').val(),
		messageSubject = $('#messageSubject').val(),
		messageContents = $('#messageContents').val(),
		messageFromDate = $('#messageFromDate').val(),
		messageToDate = $('#messageToDate').val(),
		messageEmployeeIds = $('#messageEmployeeIds').val(),
		messageGroupIds = $('#messageGroupIds').val(),
		messageCategory = $('#messageCategory').val();

	$('#Subject').val(messageSubject);
	$('#Contents').val(messageContents);
	$('#MessageId').val(messageId);
	$('#category').val(messageCategory);

	if (messageFromDate) {
		var newFromDate = new Date(messageFromDate);
		$('#FromDate').val($.datepicker.formatDate('d M, yy', newFromDate));
	}
	if (messageToDate) {
		var newToDate = new Date(messageToDate);
		$('#ToDate').val($.datepicker.formatDate('d M, yy', newToDate));
	}
	if (messageEmployeeIds) {
		$('#employee').val(messageEmployeeIds.split(','));
	}
	if (messageGroupIds) {
		$('#group').val(messageGroupIds.split(','));
	}
};

var init = function () {
	bindingDataIfEditMode();

	$('#employee').chosen();
	$('#employee').trigger("chosen:updated");
	$("#group").multiselect();
};

var validateForm = function () {
	var errorString = [],
		check = true;

	if (!$('#Subject').val()) {
		errorString.push('The Subject field is required!');
	}
	if (!$('#category').val()) {
		errorString.push('The Category field is required!');
	}
	if (!$('#employee').val() && !$('#group').val()) {
		errorString.push('The Send To field is required!');
	}
	if (!$('#Contents').val()) {
		errorString.push('The Contents field is required!');
	}

	if (errorString.length > 0) {
		$("#error").show();
		$("#error").html(errorString.join("</br>"));
		check = false;
	}

	return check;
};

var createStringResult = function (items) {
	var result = '';

	if (items) {
		for (var i = 0; i < items.length; i++) {
			if (!result) {
				result = items[i];
			}
			else {
				result += ',' + items[i];
			}
		}
	}

	return result;
};

var getEmail = function (result, items) {

	if (items) {
		for (var i = 0; i < items.length; i++) {
			var item = items[i],
				email = $(item).attr('email');

			if (email) {
				if (!result) {
					result = email;
				}
				else {
					result += "," + email;
				}
			}
		}
	}

	return result;
};

$(document).ready(function () {
	init();

	$("form").submit(function (event) {
		var validate = validateForm();

		if (validate) {
			$("#error").hide();
			$("#error").html('');
			event.preventDefault();

			var form = $(this),
				employees = createStringResult($('#employee').val()),
				groups = createStringResult($('#group').val()),
				sendToEmails = getEmail("", $('#employee option:selected'));

			sendToEmails = sendToEmails !== undefined ? getEmail(sendToEmails, $('#group option:selected'))
													: getEmail("", $('#group option:selected'));

			$('#EmployeeIds').val(employees);
			$('#GroupIds').val(groups);
			$('#Category').val($('#category').val());
			$('#SendToEmail').val(sendToEmails);
			$('#HasSendEmail').val($('#sendEmail').is(':checked'));

			$.ajax({
				url: form.action,
				type: "Post",
				data: form.serialize(),
				success: function (commandResult) {
					if (commandResult.Code === 0) {
						document.location = _global.root + "Message/Index";
					}
					else {
						$("#error").html(commandResult.message);
					}
				},
				error: function (response) {
					$("#error").html(response.message);
				}
			});
		}
		else {
			event.preventDefault();
		}
	});
});