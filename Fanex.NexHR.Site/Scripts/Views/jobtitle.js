/// <reference path="../Libs/jquery.min.js" />
/*global actingButton, _global, noInformation, reloadButtonText, _messages, getParam*/
/*exported deleteJobTitle, index, editJobTile, CancelUpdate, UpdateJobTitle*/
"use strict";


var rowIndex = 1;
function canNotUpdate(id) {
	$('#txtJobTile_' + id).val($('#spanJobTitle_' + id).text());
}

function index() {
	return rowIndex++;
}

function query() {
	var value = $('#keyword').val();
	actingButton('#query', true);

	$.getJSON(
	_global.root + 'api/jobtitle/getjobtitle',
	{ query: value }, 
	function (data) {
		//console.log(data);
		if (data.length > 0) {
			rowIndex = 1;

			$('#data tbody').children().remove();
			$('#itemTemplate').tmpl(data).appendTo('#data');
		}
		else {
			$('#data tbody').children().remove();
			$(noInformation(12)).appendTo('#data');
		}
		reloadButtonText('#query');
	}).error(function () {
		reloadButtonText('#query');
		alert(_messages.errorTryAgain);
	});
}

function deleteJobTitle(id) {
	confirm('Are you sure you want to delete?', function () {
		$.post(_global.root + 'ajax/DeleteJobTitle',
			{ id: id },
			function (response) {
				if (response) {
					document.location = '/jobtitle/jobtitlemanagement';
				}
				else {
					alert(_messages.errorTryAgain);
				}
			}).error(function () {
				alert(_messages.errorTryAgain);
			});
	});
}

function editJobTile(id) {
	$('#spanJobTitle_' + id).hide();
	$('#txtJobTile_' + id).show();
	$('#btnEditJobTitle_' + id).hide();
	$('#btnUpdateJT_' + id).show();
	$('#btnCancelUpdateJT_' + id).show();
}

function cancelUpdate(id) {
	$('#spanJobTitle_' + id).show();
	$('#txtJobTile_' + id).hide();
	$('#btnEditJobTitle_' + id).show();
	$('#btnUpdateJT_' + id).hide();
	$('#btnCancelUpdateJT_' + id).hide();
}


function updateJobTitle(id) {
	var o = $('#txtJobTile_' + id);
	if (o.val().length > 100 || o.val().length < 3) {
		alert("Length of job title must be between " +
					3 + " and " + 100 + ".");
		canNotUpdate(id);
		return false;
	}

	confirm('Are you sure you want to change?', function () {
		var jobTitle = $('#txtJobTile_' + id).val();
		if (jobTitle === '') {
			alert(_messages.errorTryAgain);
			canNotUpdate(id);
		}
		var oldTitle = $('#spanJobTitle_' + id).val();
		if (jobTitle !== oldTitle) {
			$.post(_global.root + 'ajax/UpdatetJobTitle',
			{ id: id, name: jobTitle },
			function (response) {
				if (response) {
					document.location = '/jobtitle/jobtitlemanagement';
				}
				else {
					alert(_messages.errorTryAgain);
					canNotUpdate(id);
				}
			}).error(function () {
				alert(_messages.errorTryAgain);
				canNotUpdate(id);
			});
		}
		else {
			alert(_messages.errorTryAgain);
			canNotUpdate(id);
		}
	});
}

$(function () {
	var name = $("#name"),
			allFields = $([]).add(name),
			tips = $(".validateTips");

	function updateTips(t) {
		tips.text(t);
	}

	function checkLength(o, n, min, max) {
		if (o.val().length > max || o.val().length < min) {
			o.addClass("ui-state-error");
			updateTips("Length of " + n + " must be between " +
					min + " and " + max + ".");
			return false;
		} else {
			return true;
		}
	}

	$("#dialog-form").dialog({
		autoOpen: false,
		height: 180,
		width: 350,
		modal: true,
		buttons: {
			"Create": function () {
				var bValid = true;
				allFields.removeClass("ui-state-error");
				bValid = bValid && checkLength(name, "job title", 3, 100);

				if (bValid) {
					$.post(_global.root + 'ajax/InsertJobTitle',
						{ name: name.val() },
						function (response) {
							if (response) {
								document.location = '/jobtitle/jobtitlemanagement';
							}
							else {
								alert(_messages.errorTryAgain);
							}
						}).error(function () {
							alert(_messages.errorTryAgain);
						});
				}
			},
			Cancel: function () {
				$(this).dialog("close");
			}
		},
		close: function () {
			updateTips('');
			allFields.val("").removeClass("ui-state-error");
		}
	});

	$("#btnNewJobTitle").click(function () {
		$("#dialog-form").dialog("open");
	});
});

$(document).ready(function () {

	$('#keyword').keyup(function (event) {
		if (event.keyCode === 13) {
			query();
		}
	});

	var q = getParam('query');
	if (q != null) {
		$('#keyword').val(q);
	}

	//search onload
	query();

	$('#query').click(function (evt) {
		evt.preventDefault();
		query();
	});

	$('#keyword').keypress(function () {
		if ($(this).val().length >= 2) {
			query();
		}
	});

	$("#data").on("click", ".lnkEdit", function () {
		var jobTitleId = $(this).attr("data-id");
		editJobTile(jobTitleId);
	});

	$("#data").on("click", ".lnkSave", function () {
		var jobTitleId = $(this).attr("data-id");
		updateJobTitle(jobTitleId);
	});

	$("#data").on("click", ".lnkCancel", function () {
		var jobTitleId = $(this).attr("data-id");
		cancelUpdate(jobTitleId);
	});

	$("#data").on("click", ".lnkDelete", function () {
		var jobTitleId = $(this).attr("data-id");
		deleteJobTitle(jobTitleId);
	});
});