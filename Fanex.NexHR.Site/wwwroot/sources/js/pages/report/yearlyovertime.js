import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { parse } from 'qs';

import '../../shared/multiselect-tree-view';
import { axiosError, floatThead, resolveClientUrl, blockUI, unblockUI, downloadFile } from '../../shared/common.functions';

$(document).ready(function () {
    floatThead('table#data');

    axios(resolveClientUrl('Section/GetHierarchy'))
        .then(response => {
            const teamIds = $('#TeamIds').val();

            $('#Team').multiSelectTreeView({
                data: response.data,
                selectedItems: teamIds
            });
        }).catch(axiosError);

    $('#btnSearch').on('click', function () {
        const teamIds = $('#Team').multiSelectTreeView('getSelectedItems');
        $('#TeamIds').val(teamIds === '' ? ' ' : teamIds);
        $('form.filter').submit();
    });

    $('#btnExcel').on('click', function () {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        blockUI();
        axios({
            url: resolveClientUrl('OvertimeReport/ExportYearlyOvertime'),
            method: 'GET',
            params: parse($('form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
});