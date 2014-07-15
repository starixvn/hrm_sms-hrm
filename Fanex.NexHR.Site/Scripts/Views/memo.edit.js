'use strict';
/*global _global,deleteMemoFile,initEditor,tinyMCE*/
/*exported deleteFile*/

function removeAttachFile(filePath) {
	var attachFiles = $("#AttachFile").val().split(";"),
		attachFileRemain = "";

	for (var i = 0; i < attachFiles.length; i++) {
		var attachFileItem = attachFiles[i];

		if (attachFileItem !== filePath) {
			if (attachFileRemain === "") {
				attachFileRemain = attachFileItem;
			}
			else {
				attachFileRemain += ";" + attachFileItem;
			}
		}
	}

	$("#AttachFile").val(attachFileRemain);
}

function deleteFile(filePath, index) {
	$("#file_" + index).remove();
	removeAttachFile(filePath);
	deleteMemoFile(filePath);
}

function attachFile(attachPath) {
	var attachFiles = $('#allAttachFile');

	if (attachPath) {
		var attachPathFiles = attachPath.split(";");

		for (var i = 0; i < attachPathFiles.length; i++) {
			var attachLink = attachPathFiles[i],
				attachLinkSplit = attachLink.split('/'),
				attachFileName = attachLinkSplit[attachLinkSplit.length - 1],
				pattern = '<div id="file_' + i + '"><a href="' + attachLink + '">' + attachFileName + '</a>&nbsp\
								<a href="javascript:deleteFile(\'' + attachLink + '\', ' + i + ');">\
									<span class="icon delete"></span>\
								</a>\
							</div>';

			attachFiles.append(pattern);
		}
	}
}

var form;

function changeType() {
	if ($("#Type_1").attr("checked") === "checked") {
		$("#content").hide();
		$("#upload").show();
		$("#Type").val("1");
	}
	else {
		$("#content").show();
		$("#upload").hide();
		$("#Type").val("0");
		$("#Contents_tbl").css("width", "612");
		$("#Contents_ifr").css("height", "335");

		if ($("#attachFile").children().length === 0) {
			$("span.delete").hide();
		}

	}
}

function uploadComplete(evt) {
	if (evt.target.status === 200) {
		var attachFile = $("#AttachFile").val();

		if (attachFile.indexOf(_global.root + evt.target.response) === -1) {
			if (attachFile) {
				$("#AttachFile").val(attachFile + ";" + _global.root + evt.target.response);
			}
			else {
				$("#AttachFile").val(_global.root + evt.target.response);
			}
		}

		$.ajax({
			url: form.action,
			type: "Post",
			data: form.serialize(),
			async: false,
			success: function () {
				document.location = _global.root + "Memo/Index";
			}
		});
	}
}

function uploadFailed(response) {
	$("#error").html(response.message);
}

function upload() {
	var files = document.getElementById('fileName').files;
	if (files === undefined) {
		return;
	}

	var fd = new FormData();

	for (var i = 0; i < files.length; i++) {
		var file = files[i];

		if (file.size / 1024 > 5120) {
			alert('file is to large (' + file.size / 1024 + 'kb) to upload. maximum = 5120kb');
			return;
		}

		fd.append('files[' + i + ']', file);
	}

	fd.append('categoryID', $("#Category").val());
	fd.append('overRide', $("#override").attr("checked") === "checked" ? "1" : "0");

	var xhr = new XMLHttpRequest();
	xhr.addEventListener("load", uploadComplete, false);
	xhr.addEventListener("error", uploadFailed, false);
	xhr.open("POST", _global.root + "file/UploadFile");
	xhr.send(fd);
}

function validateForm() {
	var check = true;
	var errorString = [];
	var Subject = $("#Subject").val();
	var RefNo = $("#RefNo").val();
	var SentTo = $("#SentTo").val();
	var SentFrom = $("#SentFrom").val();
	var attachFile = $("#allAttachFile").children().length;
	var fileName = $("#fileName").val();

	var editorText = tinyMCE.activeEditor.getContent();

	if (Subject === null || Subject === "") {
		errorString.push("* The Subject field is required.");
	}
	if (RefNo === null || RefNo === "") {
		errorString.push("* The RefNo field is required.");
	}
	if (SentTo === null || SentTo === "") {
		errorString.push("* The SentTo field is required.");
	}
	if (SentFrom === null || SentFrom === "") {
		errorString.push("* The SentFrom field is required.");
	}

	if ($("#Type").val() === "1") {
		if (attachFile === 0 && fileName === "") {
			errorString.push("* The Attached file is required.");
		}
	}

	if ($("#Type").val() === "0") {
		if (editorText === null || editorText === "") {
			errorString.push("* The Content field is required.");
		}
	}

	if (errorString.length > 0) {
		$("#error").show();
		$("#error").html(errorString.join("</br>"));
		return false;
	}
	return check;
}

$(document).ready(function () {
	initEditor('Contents', 'basic');
	$("#error").hide();
	$("#SentDate").attr('readonly', 'readonly');

	$("#SentDate").datepicker({
		changeMonth: true,
		changeYear: true
	});
	var refNo = $("#RefNo").val();
	var subJect = $("#Subject").val();
	var forInfo = $("#ForInfo").val();
	var sentDate = $("#SentDate").val();
	var sentTo = $("#SentTo").val();
	var sentFrom = $("#SentFrom").val();
	var contents = $("#Contents").val();

	$("#btnClear").click(function () {
		$("#RefNo").val(refNo);
		$("#Subject").val(subJect);
		$("#ForInfo").val(forInfo);
		$("#SentDate").val(sentDate);
		$("#SentTo").val(sentTo);
		$("#SentFrom").val(sentFrom);
		$("#Contents").val(contents);
	});

	$("#Type_0, #Type_1").change(function () {
		changeType();
		$("#error").hide();
	});

	if ($("#Type").val() === "1") {
		$("#Type_1").attr("checked", "checked");

		if ($("#AttachFile").val().length > 0) {
			$('#attach').show();
			attachFile($("#AttachFile").val());
		}
		else {
			$('#attach').hide();
		}
	}
	else {
		$("#Type_0").attr("checked", "checked");
	}

	changeType();

	$("form").submit(function (e) {
		var validate = validateForm();
		form = $(this);

		if ($("#Type").val() === "1") {
			var attachFile = $("#allAttachFile").children().length;
			var fileName = $("#fileName").val();

			if (!validate) {
				e.preventDefault();
				$("#error").show();
			}

			else {
				if (attachFile === 0 || (attachFile !== 0 && fileName !== "")) {
					e.preventDefault();
					upload();
				}
				else {
					e.preventDefault();
					$.ajax({
						url: form.action,
						type: "Post",
						data: form.serialize(),
						success: function () {
							document.location = _global.root + "Memo/Index";

						}
					});
				}
			}
		}
		else {
			if (!validate) {
				$("#error").show();
				return false;
			}
			else {
				e.preventDefault();
				$("#Contents").val(tinyMCE.activeEditor.getContent());
				$.ajax({
					url: form.action,
					type: "Post",
					data: form.serialize(),
					success: function () {
						document.location = _global.root + "Memo/Index";

					}
				});
			}

		}

	});

});


