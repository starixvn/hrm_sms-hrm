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
        if (!next_click_allowed) return;

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

        next_click_allowed = false;
        window.setTimeout(function () { next_click_allowed = true; }, 500);
    });

    //Click previous link
    var prev_click_allowed = true;
    $('#prev').click(function (e) {
        if (!prev_click_allowed) return;

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

        prev_click_allowed = false;
        window.setTimeout(function () { prev_click_allowed = true; }, 500);

    });

    //Click submit button
    var submit_click_allowed = true;
    $('#submit').click(function (e) {
        if (!submit_click_allowed) return;

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

        submit_click_allowed = false;
        window.setTimeout(function () { submit_click_allowed = true; }, 500);

    });

    $("#exportExcel").click(function () {
        exportExcel();
    });

    function getWorkingCalendarDetails(fromDate, toDate, group, title) {
        var workingCalendarContainer = $("#workingCalendarDetails");
        var employeeId = null;
        $.get(_global.root + "calendar/WorkingCalendarDetails",
            { "empId": employeeId, "fromDate": fromDate, "toDate": toDate, "groupId": group, "jobTitleId": title })
                    .done(function (content) {
                        workingCalendarContainer.empty();
                        workingCalendarContainer.html(content).dataTable({
                            //"bProcessing": true,
                            "sScrollY": "570px",
                            "sScrollX": "100%",
                            "sScrollXInner": "100%",
                            "bScrollCollapse": true,
                            "bFilter": true,
                            "bSort": false,
                            "bPaginate": false,
                            "bInfo": true,
                            "bJQueryUI": true
                            //"bDestroy": true
                        });
                        new FixedColumns(oTable, {
                            "iLeftColumns": 2,
                            "iLeftWidth": 250
                            //                            "iRightColumns": 1,
                            //                            "iRightWidth": 180
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