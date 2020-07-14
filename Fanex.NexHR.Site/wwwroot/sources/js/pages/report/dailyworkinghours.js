import 'jquery-ui-bundle/jquery-ui';
import axios from 'axios';
import { parse } from 'qs';

import '../../shared/multiselect-tree-view';
import { axiosError, initDatepicker, resolveClientUrl, blockUI, unblockUI, downloadFile } from '../../shared/common.functions';

$(document).ready(function () {
    let query = function () {
        blockUI();
        axios({
            url: resolveClientUrl('DailyWorkingHours/GetDailyWorkingHours'),
            method: 'GET',
            params: parse($('form.filter').serialize()),
            responseType: 'text'
        }).then(response => {
            $('#DailyWorkingHoursDetails').html(response.data);
            floatThead('table#data');
        }).catch(axiosError).then(() => unblockUI());
    };

    initDatepicker();

    axios(resolveClientUrl('Section/GetOrgChart'))
        .then(response => {
            $('#Team').multiSelectTreeView({
                data: response.data,
                isSelectedAll: true
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
            url: resolveClientUrl('DailyWorkingHours/ExportExcel'),
            method: 'GET',
            params: parse($('form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
});