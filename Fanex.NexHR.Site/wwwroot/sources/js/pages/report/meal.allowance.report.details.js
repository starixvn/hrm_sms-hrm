import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import axios from 'axios';
import { parse } from 'qs';
import 'floatthead/dist/jquery.floatThead';

import '../../shared/employee.search.in.team';
import '../../shared/multiselect-tree-view';
import {
    axiosError, initDatepicker, initFromDateOnChange, floatThead,
    blockUI, unblockUI, downloadFile, resolveClientUrl, sortTable
} from '../../shared/common.functions';


$(document).on('click', '#btnSearchDetails', function () {
    $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
    query(false);
});

let query = function (IsFirstLoad) {
    blockUI();
    var params = $('form.filter-detail').serialize() + "&IsFirstLoad=" + IsFirstLoad;
    axios({
        url: resolveClientUrl('MealAllowanceReport/MealAllowanceDetailsGrid'),
        method: 'GET',
        params: parse(params),
        responseType: 'text'
    }).then(response => {
        $('#MealAllowanceDetaills').html(response.data);
        floatThead('table#data');

        $('th.col-sorter').on('click', function () {
            sortTable('table#data', this);
            $("table#data tbody tr").each(function (index) {
                var initIndex = index + 1;
                $(this).find("td:eq(0)").html(initIndex++);
            });
        });
    }).catch(axiosError).then(() => unblockUI());
};

$(document).on('click', '#btnExcelDetails', function () {
    $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
    blockUI();
    axios({
        url: resolveClientUrl('MealAllowanceReport/ExportExcel'),
        method: 'GET',
        params: parse($('form.filter-detail').serialize()),
        responseType: 'blob'
    }).then(response => {
        const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
        downloadFile(response.data, fileName);
    }).catch(axiosError).then(() => unblockUI());
});

$(document).ready(function () {
    initDatepicker();
    initFromDateOnChange($('#FromDate'), $('#ToDate'));
    
    axios(resolveClientUrl('Section/GetOrgChart'))
        .then(response => {
            $('#Team').multiSelectTreeView({
                data: response.data,
                isSelectedAll: true
            });
            query(true);
        }).catch(axiosError);
});
