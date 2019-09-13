import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';

import { axiosError, blockUI, floatThead, unblockUI } from '../../shared/common.functions';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';

$(document).ready(function () {
    let query = function () {
        blockUI();

        axios({
            url: _global.root + 'report/GetUnpaidLeaveSummary',
            method: 'GET',
            params: {
                year: $('#Year').val(),
                sectionIds: $('#SectionIds').val(),
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

    axios(_global.root + 'Department/GetHierarchy')
        .then(response => {
            $('#Section').multiSelectTreeView({
                data: response.data,
                isSelectedAll: true
            });

            query();
        }).catch(axiosError);

    $('#btnSearch').on('click', function () {
        $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        query();
    });

    $('#btnExcel').click(function () {
        const sectionIds = $('#Section').multiSelectTreeView('getSelectedItems');
        const url = _global.root + 'ExportLeaveSummary/UnpaidLeaveSummary?year=' + $('#Year').val() +
            '&SectionIds=' + sectionIds + '&Employee=' + $('#Employee').val() +
            '&EmployeeId=' + $('#EmployeeId').val() + '&category=' + $('#Category').val() + '&leaveType=' + $('#LeaveType').val();
        location.href = url;
    });
});