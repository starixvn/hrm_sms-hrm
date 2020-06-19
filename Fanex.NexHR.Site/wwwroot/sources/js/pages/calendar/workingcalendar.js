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

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    workingCalendarModule.initModule();
});