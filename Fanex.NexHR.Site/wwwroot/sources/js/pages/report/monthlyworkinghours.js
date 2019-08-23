import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-ui-multiselect-widget/src/jquery.multiselect';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';

import { axiosError, blockUI, unblockUI, floatThead, initDatepicker, initFromDateOnChange } from '../../shared/common.functions';

$(document).ready(function () {
    let query = function () {
        blockUI();

        axios({
            url: _global.root + 'MonthlyWorkingHours/GetMonthlyWorkingHours',
            method: 'GET',
            params: {
                fromDate: $('#FromDate').val(),
                toDate: $('#ToDate').val(),
                sectionIds: $('#SectionIds').val(),
                isLocal: $('#Category').val(),
                employee: $('#Employee').val(),
                employeeStatusId: $('#EmployeeStatusId').val()
            },
            responseType: 'html'
        }).then(response => {
            $('#MonthlyWorkingHoursDetails').html(response.data);
            floatThead('table#data');
        }).catch(axiosError).then(() => unblockUI());
    };

    initDatepicker();
    initFromDateOnChange($('#FromDate'), $('#ToDate'));

    $('#Section').multiselect({
        minWidth: 170,
        checkAllText: 'Check',
        uncheckAllText: 'Uncheck'
    });

    $('#Section').on('change', function () {
        const sectionIds = $(this).val() !== null ? $(this).val().join(',') : '';
        $('#SectionIds').val(sectionIds);
    });

    $('#btnSearch').on('click', function () {
        query();
    });

    $('#btnExcel').on('click', function () {
        window.location.href = _global.root + 'MonthlyWorkingHours/ExportExcel?' + $('form.filter').serialize();
    });

    query();
});