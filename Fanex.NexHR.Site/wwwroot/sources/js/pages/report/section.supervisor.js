import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { parse } from 'qs';

import '../../shared/multiselect-tree-view';
import { axiosError, blockUI, unblockUI, floatThead, resolveClientUrl, downloadFile } from '../../shared/common.functions';

(function () {
    function query() {
        $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        blockUI();
        axios({
            url: resolveClientUrl('report/SectionSupervisorDetails'),
            method: 'POST',
            params: parse($('form.filter').serialize()),
            responseType: 'text'
        }).then(response => {
            $('#SectionSupervisorDetails').html(response.data);
            floatThead('table#data');
        }).catch(axiosError).then(() => unblockUI());
    }

    $(document).ready(function () {
        axios(resolveClientUrl('Department/GetHierarchy'))
            .then(response => {
                $('#Section').multiSelectTreeView({
                    data: response.data,
                    isSelectedAll: true
                });

                query();
            }).catch(axiosError);

        $("#btnSubmit").on('click', function () {
            query();
        });

        $('#btnExcel').on('click', function () {
            $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));

            blockUI();
            axios({
                url: resolveClientUrl('ExportSupervisor/SectionSupervisor'),
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