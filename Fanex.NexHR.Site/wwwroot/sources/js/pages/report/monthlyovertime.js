import 'jquery-ui-bundle/jquery-ui';
import '../../../libs/monthPicker/monPickr-0.0.3';
import axios from 'axios';
import { parse } from 'qs';

import { axiosError, resolveClientUrl, blockUI, unblockUI, downloadFile } from '../../shared/common.functions';

$(document).ready(function () {
    let monthPicker = $('#MonthControl');
    monthPicker.monPickr({
        showDateRange: false,
        sortDesc: true,
        minDate: $('#MinDate').val(),
        maxDate: $('#MaxDate').val(),
        today: undefined,
        month: undefined,
        afterChangeMonth: function () {
            const month = monthPicker.monPickr('getMonth', 'MM');
            const year = monthPicker.monPickr('getMonth', 'yyyy');
            $('#Month').val(month);
            $('#Year').val(year);
        }
    });

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