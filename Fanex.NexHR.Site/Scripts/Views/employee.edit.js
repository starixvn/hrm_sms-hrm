'use strict';
/*global _global, _messages, reloadButtonText, actingButton*/
/* exported */

function bindSARole(onload) {
	var combineSARole = $('#RoleArray2, #RoleArray3, #RoleArray4, #RoleArray5, #RoleArray6');
	if ($('#RoleArray7').prop('checked')) {
		combineSARole.prop('checked', true).prop('disabled', true);
	}
	else if (!onload) {
		combineSARole.prop('checked', false).prop('disabled', false);
	}
}

function uploadComplete(evt) {
	if (evt.target.status === 200) {
		var url = $('#avatar').attr('src');
		var parts = url.split('?f=');
		$('#avatar').attr('src', parts[0] + '?f=' + Math.random());
	}
	reloadButtonText('#upload');
}

function uploadFailed() {
	$('#m').text('Upload fail, please try again!');
	reloadButtonText('#upload');
}

function upload() {
	var file = document.getElementById('file').files[0];
	if (file === undefined) {
		return;
	}
	if (file.size / 1024 > 5120) {
		alert('File is to big (' + file.size / 1024 + 'KB) to upload. Maximum = 5120KB');
		return;
	}

	var pass = /\.(gif|jpe?g|png)$/i.test(file.name);
	if (!pass) {
		alert('Please choose an image');
		return;
	}

	//notice
	actingButton('#upload', true);
	$('#m').text('');

	var fd = new FormData();
	fd.append('code', $('#Code').val());
	fd.append('file', file);

	var xhr = new XMLHttpRequest();
	xhr.addEventListener("load", uploadComplete, false);
	xhr.addEventListener("error", uploadFailed, false);
	xhr.open("POST", _global.root + "employee/uploadavatar");
	xhr.send(fd);
}

function resetPassword() {
	var id = $("#Id").val();
	var nickName = $("#NickName").val();
	confirm("Are you sure you want to reset " + nickName + "'s password?", function () {
		$.post(_global.root + 'ajax/ResetPassword', { id: id }, function () {
		});
	});
}

function checkAuthorizeToGrantAccess() {
	var canEditUserRole = $("#Authorize").val();
	if (canEditUserRole === "False") {
		$('#RoleArray7').unbind("click");
		$("#userRole label").attr('onclick', 'return false');
		$("#userRole label").last().hide();
	}
}

function showExpatApplicableHoliday() {
	var value = $("#IsLocal").val();
	if (value === "true") {
		$("#expatHoliday").hide();
	}
	else {
		$("#expatHoliday").show();
	}
}

$(document).ready(function () {

	$(".dropdown").chosen();
	$('#JobTitleId').chosen();

	bindSARole(true);
	$('#RoleArray7').click(function () {
		bindSARole(false);
	});

	//checkUserIsSystemAdmin
	checkAuthorizeToGrantAccess();

	//Birthday picker.
	$("#DOBPicker").birthdaypicker({
		futureDates: true,
		maxYear: new Date().getFullYear(),
		maxAge: 75,
		defaultDate: $('#DateOfBirth').val()
	});

	$("#postform").click(function () {
		var isCheckedTopManager = $("input[name='IsTopManager']").prop('checked');

		if (isCheckedTopManager === true) {
			$("#formEditUser")[0].submit();
		}
		else {
			var verifier = $("#lstVerifier").text();
			var approver = $("#lstApprover").text();
			if (verifier == null || verifier === "") {
				if (approver == null || approver === "") {
					confirm("This user does not have any Verifier or Approver! Continue?", function () {
						$("#formEditUser")[0].submit();
					});
				}
				else {
				    confirm("This user does not have any Verifier! Continue?", function () {
						$("#formEditUser")[0].submit();
					});
				}
			}
			else {
				if (approver == null || approver === "") {
				    confirm("This user does not have any Approver! Continue?", function () {
						$("#formEditUser")[0].submit();
					});
				}
				else {
					$("#formEditUser")[0].submit();
				}
			}
		}

	});

	$("#resetpassword").on("click", function () {
		resetPassword();
	});

	$("#month, #day, #year").change(function () {
		var year = $("#year").val();
		var month = $("#month").val();
		var day = $("#day").val();
		var date = new Date(year, month - 1, day);

		$("#DateOfBirth").val(date.format("dd mmm yyyy"));
	});

	$('#upload').click(function () {
		upload();
	});

	$('#delete').click(function () {
		confirm(_messages.confirmDelete, function () {
			var resignedDate = $('#ResignedDate').val();
			if (resignedDate.length === 0) {
				alert(_messages.requireResignedDate);
				$('#ResignedDate').focus();
				return;
			}

			$.post(_global.root + 'ajax/DeleteEmployee',
			{ id: $('#Id').val(), resignedDate: resignedDate },
			function (response) {
				if (response) {
					document.location = '/employee/usermanagement';
				}
				else {
					alert(_messages.errorTryAgain);
				}
			}).error(function () { alert(_messages.errorTryAgain); });
		});
	});

	var collapse = $(".boxtitle");
	var itemContent = collapse.next();
	itemContent.show();

	$(".boxtitle").click(function () {
		var $header = $(this);
		//getting the next element
		var $content = $header.next();
		//open up the content needed - toggle the slide- if visible, slide up, if not slidedown.
		$content.slideToggle(500, function () { });
	});

	$("#IsLocal").on("change", function () {
		showExpatApplicableHoliday();
	});
	showExpatApplicableHoliday();

});