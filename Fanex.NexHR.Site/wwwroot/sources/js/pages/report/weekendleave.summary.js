import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import '../../../libs/monthPicker/monPickr-0.0.3';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import {  blockUI, floatThead, unblockUI, confirm, alert, initDatepicker } from '../../shared/common.functions';
import '../../date';

function initEventForDateRangeDropdown() {
    $('form.filter #DateRange').on('change', function () {
        var year = $('form.filter #DateRange').val();

        if (parseInt(year) !== 0) {
            var fromDate = new Date($('form.filter #FromDate').val());
            var toDate = new Date($('form.filter #ToDate').val());

            fromDate.setFullYear(parseInt(year), 0, 1);
            toDate.setFullYear(parseInt(year), 11, 31);

            $('form.filter #FromDate').val(formatDate(fromDate));
            $('form.filter #ToDate').val(formatDate(toDate));
        }
    });
}

function initEventForViewWeekendLeaveDetail() {
    $('table a.viewWeekendLeaveDetail').on('click', function () {
        var empId = $(this).data('empid'),
            employee = $(this).data('employee'),
            teamId = $('form.filter #TeamId').val(),
            isLocal = $('form.filter #IsLocal').val(),
            fromDate = $('form.filter #FromDate').datepicker({ dateFormat: 'dd M, yy' }).val(),
            toDate = $('form.filter #ToDate').datepicker({ dateFormat: 'dd M, yy' }).val();

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
    blockUI();
    $.ajax({
        url: _global.root + 'report/PartialListWeekendLeaveSummary',
        data: $('form.filter').serialize(),
        success: function (data) {
            unblockUI();
            $('#weekend-leave-summary').html(data);
            floatThead("table#data");
            initEventForViewWeekendLeaveDetail();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            unblockUI();
            alert(textStatus + ': ' + errorThrown);
        }
    });
}

$(document).ready(function () {
    initEventForDateRangeDropdown();
    $.ajax({
        url: _global.root + 'Department/GetHierarchy',
        success: function (data) {
            $('#Section').multiSelectTreeView({
                data: data,
                noneSelectedText: 'Select a section',
                selectedItems: $('#TeamId').val(),
                singleSelection: true,
                onSelect: function () {
                    $('#TeamId').val($('#Section').multiSelectTreeView('getSelectedItems'));
                }
            });

            query();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            alert(textStatus + ': ' + errorThrown);
        }
    });

    initDatepicker();

    $('form.filter #FromDate, form.filter #ToDate').on('change', function () {
        $('form.filter #DateRange').val('0');
    });

    $('form.filter #submit').on('click', function (event) {
        event.preventDefault();
        query();
    });
    $('form.filter #btnExcel').on('click', function (event) {
        event.preventDefault();
        window.location.href = _global.root + 'ExportWeekendLeave/ExportWeekendLeaveSummaryReport?' + $('form.filter').serialize();
    });
});