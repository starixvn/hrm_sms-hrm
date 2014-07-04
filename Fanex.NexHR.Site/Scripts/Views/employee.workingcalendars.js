$(document).ready(function () {

	var fromDate = $("#OriginalFromDate").val();
	var toDate = $("#OriginalToDate").val();
	$("#displayFrom, #StartDate").text($.datepicker.formatDate('d M, yy', new Date(fromDate)));
	$("#displayTo, #EndDate").text($.datepicker.formatDate('d M, yy', new Date(toDate)));

	getWorkingCalendarDetails($("#submit"), fromDate, toDate);
	$('#group').multiselect();
	$("#title").multiselect({
		noneSelectedText: 'Select positions',
		selectedText: '# selected',
		selectedList: 0
	});

	//Click next link
	$('#next').click(function (e) {
		destroyDatatable($(this));
		var fromNextDate = $("#FromNextDate").val();
		var toNextDate = $("#ToNextDate").val();
		var object = prepareDataToQuery(fromNextDate, toNextDate);
		getWorkingCalendarDetails($(this), fromNextDate, toNextDate, object.group, object.title);
	});

	//Click previous link
	$('#prev').click(function (e) {
		destroyDatatable($(this));
		var fromPrevDate = $("#FromPreviousDate").val();
		var toPrevDate = $("#ToPreviousDate").val();
		var object = prepareDataToQuery(fromPrevDate, toPrevDate);
		getWorkingCalendarDetails($(this), fromPrevDate, toPrevDate, object.group, object.title);
	});

	//Click submit button
	$('#submit').click(function (e) {
		destroyDatatable($(this));
		var fromDate = $("#StartDate").val();
		var toDate = $("#EndDate").val();
		var object = prepareDataToQuery(fromDate, toDate);
		getWorkingCalendarDetails($(this), fromDate, toDate, object.group, object.title);
	});

	function prepareDataToQuery(fromPrevOrNexDate, toPrevOrNexDate) {
		var dataObject = {};
		var displayDateFrom = $.datepicker.formatDate('d M, yy', new Date(fromPrevOrNexDate));
		var displayDateTo = $.datepicker.formatDate('d M, yy', new Date(toPrevOrNexDate));
		$("#displayFrom").text(displayDateFrom);
		$("#displayTo").text(displayDateTo);
		$("#StartDate").val(displayDateFrom);
		$("#EndDate").val(displayDateTo);
		var group = $("#group").val() != null ? $("#group").val().join() : "";
		var title = $("#title").val() != null ? $("#title").val().join() : "";
		dataObject = { group: group, title: title };

		return dataObject;
	}

	function destroyDatatable(element) {
		buttonLoading(element, true);
		$("#workingCalendarDetails").dataTable().fnDestroy();
	}

	$("#exportExcel").click(function () {
		exportExcel();
	});

	function getWorkingCalendarDetails(button, fromDate, toDate, group, title) {
		var employeeText = $("#Employee").val();
		var workingCalendarContainer = $("#workingCalendarDetails");
		var employeeId = $("#EmployeeId").val();
		if (employeeText == "" || employeeText == null) {
			employeeId = null;
		}
		var isLocal = $("#category").val();
		if (isLocal == "0") {
			isLocal = 0;
		}
		if (isLocal == "1") {
			isLocal = 1;
		}
		$.get(_global.root + "calendar/WorkingCalendarDetails",
            { "empId": employeeId, "fromDate": fromDate, "toDate": toDate, "groupId": group, "jobTitleId": title, isLocal: isLocal })
        .done(function (content) {
        	workingCalendarContainer.empty();
        	workingCalendarContainer.html(content).dataTable({
        		"sScrollY": "520px",
        		"sScrollX": "100%",
        		"sScrollXInner": "100%",
        		"bScrollCollapse": true,
        		"bFilter": true,
        		"bSort": false,
        		"bPaginate": true,
        		"sPaginationType": "full_numbers",
        		"bInfo": true,
        		"bJQueryUI": true,
        		'bLengthChange': false,
        		'iDisplayLength': 50,
        		"aoColumnDefs": [{ 'bSortable': false, 'aTargets': [2, 3, 4, 5, 6, 7, 8]}]
        	});
        	reload(button);
        });
	}
});

function exportExcel() {
    var from = new Date($("#displayFrom").text());
    var to = new Date($("#displayTo").text());

    var fromDate = $.datepicker.formatDate('d M, yy', from);
    var toDate = $.datepicker.formatDate('d M, yy', to);
    var group = $("#group").val() != null ? $("#group").val().join() : "";
    var title = $("#title").val() != null ? $("#title").val().join() : "";
    var url = null;

    url = _global.root + "export/EmployeeWorkingCalendar?fromDate=" + fromDate + "&toDate=" + toDate
                        + "&group=" + group + "&title=" + title;
    location.href = url;
}

function buttonLoading(element, disabled) {
    element.attr("origin-value", $(element).attr("value"));
    element.attr("value", 'Processing');
    element.attr("disabled", "disabled");
}

function reload(element) {
    element.attr("value", $(element).attr("origin-value"));
    element.removeAttr("disabled");
    $("#blocker").hide();
}