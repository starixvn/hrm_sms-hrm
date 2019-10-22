import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import '../../../libs/monthPicker/monPickr-0.0.3';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import { axiosError, blockUI, floatThead, unblockUI, initFromDateOnChange, confirm, alert } from '../../shared/common.functions';
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
        url: _global.root + 'report/PartialListWeekendLeaveDetail',
        data: $('form.filter').serialize(),
        success: function (data) {
            $('#weekend-leave-detail').html(data);
            floatThead("table#data");
            unblockUI();
        },
        error: function (jqXHR, textStatus, errorThrown) {
            unblockUI();
            alert(textStatus + ': ' + errorThrown);
        }
    });
}

function initDatepicker() {
        $('.datepicker').each(function () {
            let $this = $(this);

            // get minDate, maxDate
            let mindate = $this.attr('mindate'),
                maxdate = $this.attr('maxdate');

            if (mindate && mindate !== 'null') {
                mindate = mindate.split('/');
                $this.datepicker('option', 'minDate', new Date(mindate[0], mindate[1] - 1, mindate[2]));
            }

            if (maxdate && maxdate !== 'null') {
                maxdate = maxdate.split('/');
                $this.datepicker('option', 'maxDate', new Date(maxdate[0], maxdate[1] - 1, maxdate[2]));
            }

            $this.datepicker({
                dateFormat: 'dd M, yy',
                changeMonth: true,
                changeYear: true
            });

            $this.attr('readonly', 'readonly');
            $this.keydown(function (e) {
                const tab = 9, enter = 13;
                if (e.keyCode === tab || e.keyCode === enter) {
                    return true;
                }

                return false;
            });

            $this.closest('.calendar-form').find('.icon-calendar').off('click').on('click', function () {
                if ($this.datepicker('widget').is(':visible')) {
                    $this.datepicker('hide');
                } else {
                    $this.datepicker('show');
                }
            });
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

    initFromDateOnChange($('#FromDate'), $('#ToDate'));
    initDatepicker();

    $('form.filter #submit').on('click', function (event) {
        event.preventDefault();
        query();
    });

    $('form.filter #btnExcel').on('click', function (event) {
        event.preventDefault();
        window.location.href = _global.root + 'ExportWeekendLeave/ExportWeekendLeaveDetailReport?' + $('form.filter').serialize();
    });
});