import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-ui-multiselect-widget/src/jquery.multiselect';
import 'floatthead/dist/jquery.floatThead';
import '../../jquery-weekpicker';
import moment from 'moment';
import axios from 'axios';

import { axiosError, blockUI, unblockUI, floatThead } from '../../shared/common.functions';
import '../../shared/employee.search.in.team';

$(document).ready(function () {
    let query = function () {
        blockUI();

        axios({
            url: _global.root + 'WeeklyWorkingHours/GetWeeklyWorkingHours',
            method: 'GET',
            params: {
                fromDate: $('#FromDate').val(),
                toDate: $('#ToDate').val(),
                sectionIds: $('#SectionIds').val(),
                isLocal: $('#Category').val(),
                employeeId: $('#EmployeeId').val()
            },
            responseType: 'html'
        }).then(response => {
            $('#WeeklyWorkingHoursDetails').html(response.data);
            floatThead('table#data');
        }).catch(axiosError).then(() => unblockUI());
    };

    $(document).on('mouseup', function (e) {
        let $wrapper = $('.weekpicker-wrapper');

        if (!$wrapper.is(e.target) && $wrapper.has(e.target).length === 0 && $wrapper.is(':visible')) {
            $('#week-picker').hide();
        }
    });

    $('#week-picker').weekpicker({
        firstDay: 1,
        currentText: $('#FromDate').val(),
        onSelect: function (dateText, startDateText, startDate, endDate, inst) {
            $('#FromDate').val(moment(startDate).format('DD MMM, YYYY'));
            $('#ToDate').val(moment(endDate).format('DD MMM, YYYY'));
            $('#week-picker').hide();
        }
    });

    $('#week-picker').hide();

    $('#FromDate, #ToDate, .icon-calendar').on('click', function () {
        $('#week-picker').toggle();
    });

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
        window.location.href = _global.root + 'WeeklyWorkingHours/ExportExcel?' + $('form.filter').serialize();
    });

    query();
});