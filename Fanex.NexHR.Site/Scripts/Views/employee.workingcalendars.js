$(document).ready(function () {

    var fromDate = $("#OriginalFromDate").val();
    var toDate = $("#OriginalToDate").val();
    var displayDateFrom = new Date(fromDate);
    var displayDateTo = new Date(toDate);
    $("#displayFrom").text($.datepicker.formatDate('d M, yy', displayDateFrom));
    $("#displayTo").text($.datepicker.formatDate('d M, yy', displayDateTo));

    getWorkingCalendarDetails(fromDate, toDate);
    $('#group').multiselect();
    $("#title").multiselect({
        noneSelectedText: 'Select positions',
        selectedText: '# selected',
        selectedList: 0
    });

    //Click next link
    var next_click_allowed = true;
    $('#next').click(function (e) {
        var element = $(this);

        actingLoading(element, true);
        $("#workingCalendarDetails").dataTable().fnDestroy();
        var fromNextDate = $("#FromNextDate").val();
        var toNextDate = $("#ToNextDate").val();
        var displayDateFrom = new Date(fromNextDate);
        var displayDateTo = new Date(toNextDate);
        $("#displayFrom").text($.datepicker.formatDate('d M, yy', displayDateFrom));
        $("#displayTo").text($.datepicker.formatDate('d M, yy', displayDateTo));
        var group = $("#group").val() != null ? $("#group").val().join() : "";
        var title = $("#title").val() != null ? $("#title").val().join() : "";
        getWorkingCalendarDetails(fromNextDate, toNextDate, group, title);

        reloadButtonText(element);
    });

    //Click previous link
    var prev_click_allowed = true;
    $('#prev').click(function (e) {
        var element = $(this);

        actingLoading(element, true);
        $("#workingCalendarDetails").dataTable().fnDestroy();
        var fromPrevDate = $("#FromPreviousDate").val();
        var toPrevDate = $("#ToPreviousDate").val();
        var displayDateFrom = new Date(fromPrevDate);
        var displayDateTo = new Date(toPrevDate);
        $("#displayFrom").text($.datepicker.formatDate('d M, yy', displayDateFrom));
        $("#displayTo").text($.datepicker.formatDate('d M, yy', displayDateTo));

        var group = $("#group").val() != null ? $("#group").val().join() : "";
        var title = $("#title").val() != null ? $("#title").val().join() : "";
        getWorkingCalendarDetails(fromPrevDate, toPrevDate, group, title);

        reloadButtonText(element);
    });

    //Click submit button
    var submit_click_allowed = true;
    $('#submit').click(function (e) {
        var element = $(this);

        actingLoading(element, true);
        $("#workingCalendarDetails").dataTable().fnDestroy();

        var fromDate = $("#OriginalFromDate").val();
        var toDate = $("#OriginalToDate").val();
        var displayDateFrom = new Date(fromDate);
        var displayDateTo = new Date(toDate);
        $("#displayFrom").text($.datepicker.formatDate('d M, yy', displayDateFrom));
        $("#displayTo").text($.datepicker.formatDate('d M, yy', displayDateTo));

        var group = $("#group").val() != null ? $("#group").val().join() : "";
        var title = $("#title").val() != null ? $("#title").val().join() : "";
        getWorkingCalendarDetails(fromDate, toDate, group, title);

        reloadButtonText(element);

    });

    $("#exportExcel").click(function () {
        exportExcel();
    });

    setTimeout(function () {
        $("#blocker").hide();
    }, 1200);

    function getWorkingCalendarDetails(fromDate, toDate, group, title) {
        var workingCalendarContainer = $("#workingCalendarDetails");
        var employeeId = $("#EmployeeId").val();
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
	            "bSort": true,
	            "bPaginate": true,
	            "sPaginationType": "full_numbers",
	            "bInfo": true,
	            "bJQueryUI": true,
	            'bLengthChange': false,
	            'iDisplayLength': 50,
	            "aoColumnDefs": [{ 'bSortable': false, 'aTargets': [2, 3, 4, 5, 6, 7, 8]}]
            });
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

function actingLoading(element, disabled) {
//    $(element).attr("origin-value", $(element).attr("value"));
//    $(element).attr("value", 'Processing');
    element.attr("disabled", "disabled");
}

function reloadButtonText(element) {
    window.setTimeout(function () {
        element.removeAttr("disabled");
//        element.attr("value", element.attr("origin-value")); 
    }, 1200);
}