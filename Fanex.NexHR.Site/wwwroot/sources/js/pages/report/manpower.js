import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { parse } from 'qs';

import { axiosError, floatThead, blockUI, unblockUI, downloadFile, resolveClientUrl } from '../../shared/common.functions';

$(document).ready(function () {
    floatThead('table#data');

    $('#btnExcel').on('click', function () {
        blockUI();
        axios({
            url: resolveClientUrl('ManpowerReport/ExportManpower'),
            method: 'GET',
            params: parse($('form.filter').serialize()),
            responseType: 'blob'
        }).then(response => {
            const fileName = response.headers['content-disposition'].match(/filename="(.+)"/)[1];
            downloadFile(response.data, fileName);
        }).catch(axiosError).then(() => unblockUI());
    });
});