import $ from 'jquery/dist/jquery';
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
            url: resolveClientUrl('MealAllowanceReport/GetMealAllowanceSummary'),
            method: 'GET',
            params: parse($('form.filter').serialize()),
            responseType: 'text'
        }).then(response => {
            $('#MealAllowanceSummary').html(response.data);
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

    $('#btnSearch').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
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
            url: resolveClientUrl('MealAllowanceReport/ExportExcel'),
            method: 'GET',
            params: parse($('form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });

    $('#btnExcelDetails').on('click', function () {
        var FromDate = $("#fromDate").val();
        var ToDate = $("#toDate").val();
        var TeamIds = $("#teamIds").val();
        var EmployeeId = $("#employeeId").val();
        var IsLocal = $("#isLocal").val();
        var params = { FromDate, ToDate, TeamIds, EmployeeId, IsLocal};
        blockUI();
        axios({
            url: resolveClientUrl('MealAllowanceReport/ExportExcel'),
            method: 'GET',
            params: params,
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
    
});

$(document).on('click', '.viewdetails', function () {
    var filterSerialize = $('form.filter').serialize() + "&Employee=" + $(this).attr('id');
    window.location.href = "MealAllowanceReport/MealAllowanceDetails?" + filterSerialize;
});