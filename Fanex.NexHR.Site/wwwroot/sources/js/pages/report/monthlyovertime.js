import 'jquery-ui-bundle/jquery-ui';
import axios from 'axios';
import { parse } from 'qs';

import { axiosError, resolveClientUrl, blockUI, unblockUI, downloadFile, initDatepicker, initFromDateOnChange } from '../../shared/common.functions';

$(document).ready(function () {
    initDatepicker();
    initFromDateOnChange($('#FromDate'), $('#ToDate'));

    $('#btnExcel').on('click', function () {
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