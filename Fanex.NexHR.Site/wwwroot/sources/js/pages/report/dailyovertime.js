import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { parse } from 'qs';

import '../../shared/multiselect-tree-view';
import {
    axiosError, floatThead, initDatepicker, initFromDateOnChange, resolveClientUrl,
    blockUI, unblockUI, downloadFile
} from '../../shared/common.functions';

$(document).ready(function () {
    initDatepicker();
    initFromDateOnChange($('#FromDate'), $('#ToDate'));
    floatThead('table#data');

    axios(resolveClientUrl('Section/GetOrgChart'))
        .then(response => {
            $('#Team').multiSelectTreeView({
                data: response.data,
                selectedItems: $('#TeamIds').val()
            });
        }).catch(axiosError);

    $('#btnSearch').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        $('form.filter').submit();
    });

    $('#btnExcel').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));

        blockUI();
        axios({
            url: resolveClientUrl('OvertimeReport/ExportDailyOvertime'),
            method: 'GET',
            params: parse($('form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
});