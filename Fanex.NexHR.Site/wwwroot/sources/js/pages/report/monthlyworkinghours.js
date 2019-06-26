import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-ui-multiselect-widget/src/jquery.multiselect';
import 'floatthead/dist/jquery.floatThead';
import '../../../libs/monthPicker/monPickr-0.0.3';
import axios from 'axios';

import { axiosError, blockUI, unblockUI, floatThead } from '../../shared/common.functions';
import '../../shared/employee.search.in.team';

$(document).ready(function () {
    let query = function () {
        blockUI();

        axios({
            url: _global.root + 'MonthlyWorkingHours/GetMonthlyWorkingHours',
            method: 'GET',
            params: {
                month: $('#Month').val(),
                year: $('#Year').val(),
                calendar: $('#Calendar').val(),
                sectionIds: $('#SectionIds').val(),
                isLocal: $('#Category').val(),
                employeeId: $('#EmployeeId').val()
            },
            responseType: 'html'
        }).then(response => {
            $('#MonthlyWorkingHoursDetails').html(response.data);
            floatThead('table#data');
        }).catch(axiosError).then(() => unblockUI());
    };

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

    $('.monpickr-dropdown').hide();
    $('#Section').multiselect({
        minWidth: 180,
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