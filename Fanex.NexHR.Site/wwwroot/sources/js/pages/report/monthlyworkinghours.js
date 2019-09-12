import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';

import '../../shared/multiselect-tree-view';
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

    axios(_global.root + 'Department/GetHierarchy')
        .then(response => {
            $('#Section').multiSelectTreeView({
                data: response.data,
                isSelectedAll: true
            });

            $('#btnSearch').trigger('click');
        }).catch(axiosError);

    $('#btnSearch').on('click', function () {
        $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        query();
    });

    $('#btnExcel').on('click', function () {
        $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        window.location.href = _global.root + 'MonthlyWorkingHours/ExportExcel?' + $('form.filter').serialize();
    });
});