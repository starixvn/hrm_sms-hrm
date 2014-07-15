'use strict';
/*global _global, autoIndex*/
/*exported deleteMemo*/

function deleteComplete(evt) {
	if (evt.target.status === 200) {
		if ($('#allAttachFile').children().length === 0) {
			$('#attach').hide();
			$("#AttachFile").val("");
			$("#Contents").val("");
		}
	}
}

function uploadFailed() {
}

function deleteMemoFile(fileName) {
	var fd = new FormData();
	fd.append('fileName', fileName);

	var xhr = new XMLHttpRequest();
	xhr.addEventListener("load", deleteComplete, false);
	xhr.addEventListener("error", uploadFailed, false);
	xhr.open("POST", _global.root + "file/Delete");
	xhr.send(fd);
}

function deleteMemo(id, allFiles, type) {

	confirm('Are you sure you want to delete this memo?', function () {
		$.post(_global.root + 'ajax/deletememo', { id: id }, function () {
			$('#row_' + id).remove();
			autoIndex();
			if (type === 1) {
				var attachFiles = allFiles.split(";");

				for (var i = 0; i < attachFiles.length; i++) {
					deleteMemoFile(attachFiles[i]);
				}
			}
		});

	});
}
