'use strict';
/* exported DatePicker */

function setDateTimeRange(selectedIndex) {
    var fromDate = new Date($('#FromDate').val()).toString('dd MMM, yyyy');
    var toDate = new Date($('#ToDate').val()).toString('dd MMM, yyyy');
    var now = $('#DateTime-Now').val();

    switch (parseInt(selectedIndex, 10)) {
        case 0: //default
            break;
        case 1: //today
            toDate = fromDate = Date.parse(now).toString('dd MMM, yyyy');
            break;
        case 2: // yesterday
            toDate = fromDate = Date.parse(now).add(-1).days().toString('dd MMM, yyyy');
            break;
        case 3: //current week
            fromDate = Date.parse(now).moveToDayOfWeek(0, -1).toString('dd MMM, yyyy');
            toDate = Date.parse(now).toString('dd MMM, yyyy');
            break;
        case 4: //last week
            fromDate = Date.parse(now).moveToDayOfWeek(0, -1).add(-7).days().toString('dd MMM, yyyy');
            toDate = Date.parse(now).moveToDayOfWeek(0, -1).add(-1).days().toString('dd MMM, yyyy');
            break;
        case 5: //current month
            fromDate = Date.parse(now).moveToFirstDayOfMonth().toString('dd MMM, yyyy');
            toDate = Date.parse(now).toString('dd MMM, yyyy');
            break;
        case 6: //last month
            fromDate = Date.parse(now).add(-1).months().moveToFirstDayOfMonth().toString('dd MMM, yyyy');
            toDate = Date.parse(now).add(-1).months().moveToLastDayOfMonth().toString('dd MMM, yyyy');
            break;
        case 7: //since last month
            fromDate = Date.parse(now).add(-1).months().moveToFirstDayOfMonth().toString('dd MMM, yyyy');
            toDate = Date.parse(now).toString('dd MMM, yyyy');
            break;
    }

    $('#FromDate').val(fromDate);
    $('#ToDate').val(toDate);
}

function initDatePicker() {
    if ($('.datepicker').length > 0) {
        // get minDate, maxDate, disabledDates
        var mindate = $('.datepicker').attr('mindate');
        var maxdate = $('.datepicker').attr('maxdate');
        var disabledDates = $('.datepicker').attr('disabledDate');

        if (disabledDates.length > 0) {
            $('.datepicker').datepicker({
                dateFormat: 'dd M, yy',
                beforeShowDay: checkBadDates,
                changeMonth: true,
                changeYear: true
            });
        }
        else {
            $('.datepicker').datepicker({
                dateFormat: 'dd M, yy',
                changeMonth: true,
                changeYear: true
            });
        }

        $('.datepicker').attr('readonly', 'readonly');
        $('.datepicker').keydown(function (e) {
            var tab = 9, enter = 13;
            if (e.keyCode === tab || e.keyCode === enter) {
                return true;
            }

            return false;
        });
    }

    $('#Date').val($('#DateOption').val());
    setDateTimeRange($('#DateOption').val());
}

function registerEventTriggerDatepicker() {
    $('.trigger-fromdate').on('click', function () {
        var fromDate = $('#FromDate');
        if (fromDate.datepicker('widget').is(':visible')) {
            fromDate.datepicker('hide');
        } else {
            fromDate.datepicker('show');
        }
    });

    $('.trigger-todate').on('click', function () {
        var toDate = $('#ToDate');
        if (toDate.datepicker('widget').is(':visible')) {
            toDate.datepicker('hide');
        } else {
            toDate.datepicker('show');
        }
    });
}

$(document).ready(function () {
    $('#Date').change(function () {
        setDateTimeRange($(this).val());
        $('#DateOption').attr('value', $(this).val());
    });

    $('#Date').val($('#DateOption').val());
    $('#FromDate, #ToDate').change(function () {
        $('#Date').val('0');
        $('#DateOption').attr('value', '0');
    });

    initDatePicker();
    registerEventTriggerDatepicker();

    var mindate = $('.datepicker').attr('mindate');
    var maxdate = $('.datepicker').attr('maxdate');
    if (mindate !== 'null') {
        mindate = mindate.split('/');
        $('.datepicker').datepicker('option', 'minDate', new Date(mindate[0], mindate[1] - 1, mindate[2]));
    }
    if (maxdate !== 'null') {
        maxdate = maxdate.split('/');
        $('.datepicker').datepicker('option', 'maxDate', new Date(maxdate[0], maxdate[1] - 1, maxdate[2]));
    }
});