import 'jquery-ui-bundle/jquery-ui';
import axios from 'axios';
import { parse } from 'qs';
import '../../shared/multiselect-tree-view';
import 'floatthead/dist/jquery.floatThead';

import { axiosError, resolveClientUrl, blockUI, unblockUI, downloadFile, initDatepicker, sortTable, floatThead } from '../../shared/common.functions';

$(document).ready(function () {

    let query = function () {
        blockUI();
        axios({
            url: resolveClientUrl('OvertimeReport/ListMonthlyOvertimeDetails'),
            method: 'GET',
            params: parse($('form.filter').serialize()),
            responseType: 'text'
        }).then(response => {
            $('#MonthlyOvertimeDetails').html(response.data);
            floatThead('table#data');
            $('th.col-sorter').on('click', function () {
                sortTable('table#data', this);
                $('table#data tbody tr').each(function (index) {
                    let rowIndex = index + 1;
                    $(this).find('td:eq(0)').html(rowIndex++);
                });
            });
        }).catch(axiosError).then(() => unblockUI());
    };

    initDatepicker();

    axios(resolveClientUrl('Section/GetOrgChart'))
        .then(response => {

            $('#Team').multiSelectTreeView({
                data: response.data,
                isSelectedAll: true,
                hasSearchBox: true
            });
        }).catch(axiosError);

    $('#btnSearch').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        query();
    });

    $('#btnExcel').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        blockUI();
        axios({
            url: resolveClientUrl('OvertimeReport/ExportMonthlyOvertime'),
            method: 'GET',
            params: parse($('form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
});