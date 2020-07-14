import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { parse } from 'qs';

import '../../shared/multiselect-tree-view';
import {
    axiosError, blockUI, unblockUI, floatThead, initDatepicker, initFromDateOnChange, resolveClientUrl, downloadFile
} from '../../shared/common.functions';

$(document).ready(function () {
    let query = function () {
        blockUI();
        axios({
            url: resolveClientUrl('MonthlyWorkingHours/GetMonthlyWorkingHours'),
            method: 'GET',
            params: parse($('form.filter').serialize()),
            responseType: 'text'
        }).then(response => {
            $('#MonthlyWorkingHoursDetails').html(response.data);
            floatThead('table#data');
        }).catch(axiosError).then(() => unblockUI());
    };

    initDatepicker();
    initFromDateOnChange($('#FromDate'), $('#ToDate'));

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
            url: resolveClientUrl('MonthlyWorkingHours/ExportExcel'),
            method: 'GET',
            params: parse($('form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
});