import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import '../../jquery-weekpicker';
import moment from 'moment';
import axios from 'axios';

import '../../shared/multiselect-tree-view';
import { axiosError, blockUI, unblockUI, floatThead, resolveClientUrl } from '../../shared/common.functions';

$(document).ready(function () {
    let query = function () {
        blockUI();

        axios({
            url: resolveClientUrl('WeeklyWorkingHours/GetWeeklyWorkingHours'),
            method: 'GET',
            params: {
                fromDate: $('#FromDate').val(),
                toDate: $('#ToDate').val(),
                sectionIds: $('#SectionIds').val(),
                isLocal: $('#IsLocal').val(),
                employee: $('#Employee').val(),
                employeeStatusId: $('#EmployeeStatusId').val()
            },
            responseType: 'text'
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
        onSelect: function (dateText, startDateText, startDate, endDate) {
            $('#FromDate').val(moment(startDate).format('DD MMM, YYYY'));
            $('#ToDate').val(moment(endDate).format('DD MMM, YYYY'));
            $('#week-picker').hide();
        }
    });

    $('#week-picker').hide();

    $('#FromDate, #ToDate, .icon-calendar').on('click', function () {
        $('#week-picker').toggle();
    });

    axios(resolveClientUrl('Department/GetHierarchy'))
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
        window.location.href = resolveClientUrl('WeeklyWorkingHours/ExportExcel?' + $('form.filter').serialize());
    });
});