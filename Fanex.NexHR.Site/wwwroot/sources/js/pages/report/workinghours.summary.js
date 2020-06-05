import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { parse } from 'qs';

import '../../shared/multiselect-tree-view';
import '../../date';
import {
    axiosError, floatThead, initDatepicker, initFromDateOnChange, setDateRange,
    blockUI, unblockUI, downloadFile, resolveClientUrl
} from '../../shared/common.functions';

$(document).ready(function () {
    floatThead('table#data');

    initDatepicker();
    initFromDateOnChange($('#FromDate'), $('#ToDate'));
    $('#Date').val($('#DateOption').val());
    $('#Date').on('change', function () {
        const value = $(this).val();
        setDateRange(value);
        $('#DateOption').val(value);
    });
    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val(0);
        $('#DateOption').val(0);
    });

    axios(resolveClientUrl('Section/GetHierarchy'))
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
            url: resolveClientUrl('WorkingHoursSummaryReport/ExportExcel'),
            method: 'GET',
            params: parse($('form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
});