import 'jquery-ui-bundle/jquery-ui';
import 'bootstrap/dist/js/bootstrap';
import 'datatables.net-bs/js/dataTables.bootstrap';
import '../../jquery-weekpicker';
import moment from 'moment';
import axios from 'axios';

import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import { axiosError, blockUI, unblockUI, resolveClientUrl } from '../../shared/common.functions';

const workingCalendarModule = (function ($) {
    function query() {
        blockUI();

        axios({
            url: resolveClientUrl('calendar/WorkingCalendarDetails'),
            method: 'GET',
            params: {
                fromDate: $('#FromDate').val(),
                toDate: $('#ToDate').val(),
                teamIds: $('#Team').multiSelectTreeView('getSelectedItems'),
                jobTitleIds: $('#JobTitle').multiSelectTreeView('getSelectedItems'),
                employeeId: $('#EmployeeId').val(),
                isLocal: $('#IsLocal').val()
            },
            responseType: 'text'
        }).then(response => {
            $('#workingCalendarDetails').html(response.data);
            if ($.fn.DataTable.isDataTable('#data')) {
                $('#data').DataTable().destroy();
            }
            const table = $('#data').DataTable({
                dom: 't' + "<'dataTables_footer'ip>",
                scrollY: '50vh',
                scrollCollapse: true,
                ordering: false,
                pageLength: 50,
                lengthChange: false,
                pagingType: 'full_numbers',
                language: {
                    paginate: {
                        next: '<span class="fa fa-angle-right"></span>',
                        previous: '<span class="fa fa-angle-left"></span>',
                        first: '<span class="fa fa-angle-double-left"></span>',
                        last: '<span class="fa fa-angle-double-right"></span>'
                    }
                }
            });

            $('.quick-search').on('keyup', function () {
                table.search(this.value).draw();
            });

            initSortTable();

            $('#prev-week').click(function () {
                const fromNextDate = $('#FromPreviousDate').val(),
                    toNextDate = $('#ToPreviousDate').val();
                $('#FromDate').val(fromNextDate);
                $('#ToDate').val(toNextDate);

                query();
            });

            $('#next-week').click(function () {
                const fromNextDate = $('#FromNextDate').val(),
                    toNextDate = $('#ToNextDate').val();
                $('#FromDate').val(fromNextDate);
                $('#ToDate').val(toNextDate);

                query();
            });
        }).catch(axiosError).then(() => unblockUI());
    }

    function initWeekpicker() {
        $('#week-picker').weekpicker({
            firstDay: 1,
            currentText: $('#FromDate').val(),
            onSelect: function (dateText, startDateText, startDate, endDate) {
                $('#FromDate').val(moment(startDate).format('DD MMM, YYYY'));
                $('#ToDate').val(moment(endDate).format('DD MMM, YYYY'));
                $('#week-picker').hide();
            }
        });

        $('#FromDate, #ToDate, .icon-calendar').on('click', function () {
            $('#week-picker').toggle();
        });

        $(document).on('mouseup', function (e) {
            let $wrapper = $('.weekpicker-wrapper');

            if (!$wrapper.is(e.target) && $wrapper.has(e.target).length === 0 && $wrapper.is(':visible')) {
                $('#week-picker').hide();
            }
        });
    }

    function initModule() {
        const promises = [
            axios(resolveClientUrl('Section/GetOrgChart')),
            axios(resolveClientUrl('calendar/GetJobTitle'))
        ];

        Promise.all(promises)
            .then(([teamResponse, jobResponse]) => {
                $('#Team').multiSelectTreeView({
                    data: teamResponse.data,
                    isSelectedAll: true
                });
                $('#JobTitle').multiSelectTreeView({
                    data: jobResponse.data,
                    isSelectedAll: true
                });

                query();
            }).catch(axiosError);

        initWeekpicker();

        $('#btnSearch').on('click', function () {
            query();
        });

        $('#btnExcel').on('click', function () {
            const teamIds = $('#Team').multiSelectTreeView('getSelectedItems');
            const url = 'ExportCalendar/EmployeeWorkingCalendar?fromDate=' + $('#FromDate').val()
                + '&toDate=' + $('#ToDate').val()
                + '&employeeId=' + $('#EmployeeId').val()
                + '&teamIds=' + (teamIds !== '' ? teamIds : ' ')
                + '&jobTitleIds=' + $('#JobTitle').multiSelectTreeView('getSelectedItems')
                + '&isLocal=' + $('#IsLocal').val();
            location.href = resolveClientUrl(url);
        });
    }

    function initSortTable() {
        $('th.col-sorter').on('click', function () {
            sortCustomTable('#data', this);
        });
    }

    function sortCustomTable(th) {
        let comparer = function (index) {
            return function (a, b) {
                var valA = getCellValue(a, index),
                    valB = getCellValue(b, index);
                return $.isNumeric(valA) && $.isNumeric(valB) ? valA - valB : valA.toString().localeCompare(valB);
            };
        };
        let getCellValue = function (row, index) {
            return $(row).children('td').eq(index).text();
        };
        let refreshSorting = function () {
            $('th.col-sorter').each(function () {
                $(this).find('div.fa').removeClass('fa-sort-asc').removeClass('fa-sort-desc');
            });
        };

        var $this = $(th).find('div.fa');
        if ($this.hasClass('fa-sort-desc')) {
            $this.removeClass('fa-sort-desc').addClass('fa-sort-asc');
        } else if ($this.hasClass('fa-sort-asc')) {
            $this.removeClass('fa-sort-asc').addClass('fa-sort-desc');
        } else {
            refreshSorting();
            $this.addClass('fa-sort-desc');
        }

        var table = $('#data_wrapper').find('table').eq(1);
        var rows = table.find('tbody tr').toArray().sort(comparer($(th).index()));
        th.asc = !th.asc;
        if (!th.asc) {
            rows = rows.reverse();
        }

        for (var i = 0; i < rows.length; i++) {
            table.append(rows[i]);
        }
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    workingCalendarModule.initModule();
});