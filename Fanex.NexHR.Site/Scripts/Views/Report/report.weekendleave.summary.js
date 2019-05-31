'use strict';

function initEventForDateRangeDropdown() {
    $('form.form-filter #DateRange').on('change', function () {
        var year = $('form.form-filter #DateRange').val();

        if (parseInt(year) !== 0) {
            var fromDate = new Date($('form.form-filter #FromDate').val());
            var toDate = new Date($('form.form-filter #ToDate').val());

            fromDate.setFullYear(parseInt(year), 0, 1);
            toDate.setFullYear(parseInt(year), 11, 31);

            $('form.form-filter #FromDate').val(formatDate(fromDate));
            $('form.form-filter #ToDate').val(formatDate(toDate));
        }
    });
}

function initEventForViewWeekendLeaveDetail() {
    $('table a.viewWeekendLeaveDetail').on('click', function () {
        var empId = $(this).data('empid'),
            employee = $(this).data('employee'),
            teamId = $('form.form-filter #TeamId').val(),
            isLocal = $('form.form-filter #IsLocal').val(),
            fromDate = $('form.form-filter #FromDate').datepicker({ dateFormat: 'dd M, yy' }).val(),
            toDate = $('form.form-filter #ToDate').datepicker({ dateFormat: 'dd M, yy' }).val();

        window.location.href = _global.root + 'report/WeekendLeaveDetail?fromDate=' + fromDate + '&toDate=' + toDate
            + '&employeeId=' + empId + '&employee=' + employee + '&teamId=' + teamId
            + '&isLocal=' + isLocal + '&fromSummaryPage=true&leaveTypeId=-1', '_blank';
    });
}

function formatDate(date) {
    var monthNames = [
      "Jan", "Feb", "Mar",
      "Apr", "May", "Jun", "Jul",
      "Aug", "Sep", "Oct",
      "Nov", "Dec"
    ];

    var day = date.getDate();
    var monthIndex = date.getMonth();
    var year = date.getFullYear();

    return (day < 10 ? "0" + day : day) + ' ' + monthNames[monthIndex] + ', ' + year;
}

function query() {
    $.ajax({
        url: _global.root + 'report/PartialListWeekendLeaveSummary',
        data: $('form.form-filter').serialize(),
        success: function (data) {
            $('#weekend-leave-summary').html(data);

            floatThead("table#data");

            initEventForViewWeekendLeaveDetail();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
        }
    });
}

$(document).ready(function () {
    initEventForDateRangeDropdown();
    $('form.form-filter #FromDate, form.form-filter #ToDate').on('change', function () {
        $('form.form-filter #DateRange').val('0');
    });

    $('form.form-filter #submit').on('click', function (event) {
        event.preventDefault();
        query();
    });
    $('form.form-filter #btnExcel').on('click', function (event) {
        event.preventDefault();
        window.location.href = _global.root + 'ExportWeekendLeave/ExportWeekendLeaveSummaryReport?' + $('form.form-filter').serialize();
    });

    query();
})