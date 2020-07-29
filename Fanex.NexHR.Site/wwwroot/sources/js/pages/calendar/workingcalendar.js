import 'jquery-ui-bundle/jquery-ui';
import 'bootstrap/dist/js/bootstrap';
import axios from 'axios';
import 'floatthead/dist/jquery.floatThead';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import { axiosError, blockUI, initDatepicker, initFromDateOnChange, unblockUI, resolveClientUrl, sortTable, floatThead } from '../../shared/common.functions';

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

            floatThead('table#data');

            $('.quick-search').on('keyup', function () {
                var value = $(this).val().toLowerCase();
                $("table#data tbody tr").filter(function () {
                    $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
                });
            });
            initSortTable();

        }).catch(axiosError).then(() => unblockUI());
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

        initDatepicker();
        initFromDateOnChange($('#FromDate'), $('#ToDate'));

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
            sortTable('#data', this);
        });
    }

    return {
        initModule: initModule
    };
}(jQuery));

$(document).ready(function () {
    workingCalendarModule.initModule();
});