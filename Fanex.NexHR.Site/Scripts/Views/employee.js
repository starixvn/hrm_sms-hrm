'use strict';
/*global _global, _messages, getParam*/
/* exported index, resetPassword*/

var rowIndex = 1;
function index() {
	return rowIndex++;
}

function actingLoading(element) {
	$(element).attr("origin-value", $(element).attr("value"));
	$(element).attr("value", 'Processing');
	$(element).attr("disabled", "disabled");
}

function reloadButtonText(element) {
	$(element).attr("value", $(element).attr("origin-value"));
	$(element).removeAttr("disabled");
	$("#blocker").hide();
}

function checkDisabledUser() {
	if ($("#enable").attr("checked")) {
		$('#userdata .resignedDate').show();
	}
	else {
		$('#userdata .resignedDate').hide();
	}
}

function query(jobTitleId) {
	if (jobTitleId === undefined || jobTitleId === '') {
		jobTitleId = $('#title').val();
	}

	var keyword = $('#keyword').val();
	actingLoading('#query', true);

	var enable = null;
	if ($("#enable").attr("checked")) {
		enable = true;
	}
	else {
		if ($("#active").attr("checked")) {
			enable = false;
		}
		else {
			enable = null;
		}
	}

	var isDelete = null;
	if ($("#IsDeleted").attr("checked")) {
		isDelete = true;
	}
	else {
		isDelete = null;
	}

	var groups = $("#group").val() != null ? $("#group").val().join() : "";
	if (groups === "") {
		var groupid = $('#groupid').val();
		groups = groupid;
		$("#group option[value='" + groups + "']").prop("selected", 1);
		$("#group").multiselect("refresh");
	}

	var isLocal = $("#category").val();
	if (isLocal === "0") {
		isLocal = false;
	}
	if (isLocal === "1") {
		isLocal = true;
	}

	$.getJSON(
	_global.root + 'api/employee/getemployees',
	{
		query: keyword,
		groupIds: groups,
		enable: enable,
		isDelete: isDelete,
		titleId: jobTitleId,
		isLocal: isLocal
	},
	function (data) {
		if (data.length > 0) {
			rowIndex = 1;
			$('#userdata tbody').children().remove();
			$('#itemTemplate').tmpl(data).appendTo('#userdata');
			checkDisabledUser();

			$('.nick').tooltip({
				track: true,
				delay: 0,
				showURL: false,
				bodyHandler: function () {
					return '<img width=64px; height=64px; src="/employee/avatar/' + $(this).attr('data-code') + '" />';
				}
			});

			$("#userdata").dataTable({
				"sScrollY": "100%",
				"sScrollX": "100%",
				"sScrollXInner": "100%",
				"bScrollCollapse": true,
				"bFilter": true,
				"bSort": false,
				"bSortClasses": false,
				"bPaginate": true,
				"sPaginationType": "full_numbers",
				"bInfo": true,
				"bJQueryUI": true,
				'bLengthChange': true,
				"aLengthMenu": [[10, 20, 30, 50, 100, -1], [10, 20, 30, 50, 100, "All"]],
				'iDisplayLength': 20,
				"aoColumnDefs": [{ 'bSortable': false, 'aTargets': [7, 9, 10]}]
			});
			$("#title").trigger("liszt:updated");
		}
		else {
			$('#userdata tbody').children().remove();
			$("#userdata").dataTable({
				"bScrollCollapse": false,
				"bFilter": false,
				"bSort": false,
				"bSortClasses": false,
				"bPaginate": false,
				"bInfo": false,
				"bJQueryUI": true,
				'bLengthChange': false
			});
			checkDisabledUser();
		}
		reloadButtonText('#query');
	}).error(function () {
		reloadButtonText('#query');
		alert(_messages.errorTryAgain);
	});
	$('#keyword').focus().select();
}

function resetPassword(id, nickName) {
	confirm("Are you sure you want to reset " + nickName + "'s password?", function () {
		$.post(_global.root + 'ajax/ResetPassword', { id: id }, function () {
		});
	});
}

function exportExcel(object) {

	var jobTitleId = $('#title').val();
	var groups = $("#group").val() != null ? $("#group").val().join() : "";
	var keyword = $('#keyword').val();
	var url = null;

	var enable = null;
	if ($("#enable").attr("checked")) {
		enable = true;
	}
	else {
		if ($("#active").attr("checked")) {
			enable = false;
		}
		else {
			enable = null;
		}
	}

	var isDelete = null;
	if ($("#IsDeleted").attr("checked")) {
		isDelete = true;
	}
	else {
		isDelete = null;
	}

	var isLocal = $("#category").val();
	if (isLocal === "0") {
		isLocal = false;
	}
	if (isLocal === "1") {
		isLocal = true;
	}

	var controllerPath;
	if (object.attr("id") === "basicExcel") {
		controllerPath = "export/EmployeeBasicInfo?query=";
	}
	if (object.attr("id") === "detailsExcel") {
		controllerPath = "export/EmployeeDetailsInfo?query=";
	}

	url = _global.root + controllerPath + keyword + "&groupIds=" + groups + 
		"&enable=" + enable + "&isDelete=" + isDelete + "&titleId=" + jobTitleId + "&isLocal=" + isLocal;

	location.href = url;
}

$(document).ready(function () {
	var jobTitleId = $('#jobtitleid').val();
	var dropDownTittle = $('#title');
	checkDisabledUser();
	$("#group").multiselect();


	dropDownTittle.val(jobTitleId);
	dropDownTittle.chosen();
	dropDownTittle.trigger("liszt:updated");

	$('#keyword').keyup(function (event) {
		if (event.keyCode === 13) {
			$("#userdata").dataTable().fnDestroy();
			query(jobTitleId);
		}
	});

	var q = getParam('query');
	if (q != null) {
		$('#keyword').val(q);
	}

	//search onload
	query(jobTitleId);

	$('#query').live("click", function (evt) {
		evt.preventDefault();

		$("#userdata").dataTable().fnDestroy();
		query(jobTitleId);
	});

	$("#basicExcel").click(function () {
		exportExcel($(this));
	});

	$("#detailsExcel").click(function () {
		exportExcel($(this));
	});
});