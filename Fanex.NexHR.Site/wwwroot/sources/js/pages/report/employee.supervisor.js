import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { parse } from 'qs';

import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import { axiosError, blockUI, unblockUI, floatThead, resolveClientUrl, downloadFile } from '../../shared/common.functions';

(function () {
    function query() {
        $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));
        blockUI();
        axios({
            url: resolveClientUrl('report/EmployeeSupervisorDetails'),
            method: 'POST',
            params: parse($('form.filter').serialize()),
            responseType: 'text'
        }).then(response => {
            $('#EmployeeSupervisorDetails').html(response.data);
            floatThead('table#data');
        }).catch(axiosError).then(() => unblockUI());
    }

    $(document).ready(function () {
        axios(resolveClientUrl('Section/GetOrgChart'))
            .then(response => {
                $('#Team').multiSelectTreeView({
                    data: response.data,
                    isSelectedAll: true
                });

                query();
            }).catch(axiosError);

        $('#category').on('change', function () {
            $('#IsLocal').val($(this).val());
        });

        $("#btnSubmit").on('click', function () {
            query();
        });

        $('#btnExcel').on('click', function () {
            $('#TeamIds').val($('#Team').multiSelectTreeView('getSelectedItems'));

            blockUI();
            axios({
                url: resolveClientUrl('ExportSupervisor/EmployeeSupervisor'),
                method: 'GET',
                params: parse($('form.filter').serialize()),
                responseType: 'blob'
            }).then(response => {
                const fileName = response.headers['content-disposition'].match(/filename=(.+)/)[1];
                downloadFile(response.data, fileName);
            }).catch(axiosError).then(() => unblockUI());
        });
    });
})();