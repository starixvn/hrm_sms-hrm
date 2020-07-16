import 'jquery-ui-bundle/jquery-ui';
import axios from 'axios';
import { parse } from 'qs';
import 'floatthead/dist/jquery.floatThead';

import '../../shared/multiselect-tree-view';
import {
    axiosError, initDatepicker, initFromDateOnChange, floatThead,
    blockUI, unblockUI, downloadFile, resolveClientUrl, sortTable
} from '../../shared/common.functions';



$(document).ready(function () {
    initDatepicker();
    initFromDateOnChange($('#FromDate'), $('#ToDate'));

    let query = function () {
        blockUI();
        axios({
            url: resolveClientUrl('WorkingCalendarSummaryReport/GetWorkingCalendarSumary'),
            method: 'GET',
            params: parse($('form.filter').serialize()),
            responseType: 'text'
        }).then(response => {
            $('#WorkingCalendarDetails').html(response.data);
            $('th.col-sorter').on('click', function () {
                sortTable(this);
                $("tbody tr").each(function (index) {
                    var initIndex = index + 1;
                    $(this).find("td:eq(0)").html(initIndex++);
                });
            });

            floatThead('table#data');
        }).catch(axiosError).then(() => unblockUI());
    };

    $('#btnSearch').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        console.log('sadadsadsdsa212121');
        query();
    });

    axios(resolveClientUrl('Section/GetOrgChart'))
        .then(response => {
            $('#Team').multiSelectTreeView({
                data: response.data,
                isSelectedAll: true
            });
        }).catch(axiosError);

    $('#btnExcel').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));

        blockUI();
        axios({
            url: resolveClientUrl('WorkingCalendarSummaryReport/ExportExcel'),
            method: 'GET',
            params: parse($('form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
});