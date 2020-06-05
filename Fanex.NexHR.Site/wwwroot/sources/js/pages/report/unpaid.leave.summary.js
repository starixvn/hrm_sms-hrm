import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';

import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import { axiosError, blockUI, unblockUI, floatThead, resolveClientUrl, downloadFile } from '../../shared/common.functions';

$(document).ready(function () {
    let query = function () {
        blockUI();

        axios({
            url: resolveClientUrl('report/GetUnpaidLeaveSummary'),
            method: 'GET',
            params: {
                year: $('#Year').val(),
                teamIds: $('#TeamIds').val(),
                category: $('#Category').val(),
                employee: $('#Employee').val(),
                employeeId: $('#EmployeeId').val(),
                leaveType: $('#LeaveType').val()
            },
            responseType: 'text'
        }).then(response => {
            $('#UnpaidLeaveSummaryDetails').html(response.data);
            floatThead('table#data');
        }).catch(axiosError).then(() => unblockUI());
    };

    axios(resolveClientUrl('Section/GetHierarchy'))
        .then(response => {
            $('#Team').multiSelectTreeView({
                data: response.data,
                isSelectedAll: true
            });

            query();
        }).catch(axiosError);

    $('#btnSearch').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        query();
    });

    $('#btnExcel').click(function () {
        const teamIds = $('#Team').multiSelectTreeView('getSelectedItems');

        blockUI();
        axios({
            url: resolveClientUrl('ExportLeaveSummary/UnpaidLeaveSummary'),
            method: 'GET',
            params: {
                year: $('#Year').val(),
                teamIds: $('#TeamIds').val(),
                category: $('#Category').val(),
                employee: $('#Employee').val(),
                employeeId: $('#EmployeeId').val(),
                leaveType: $('#LeaveType').val()
            },
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename=(.+)/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
});