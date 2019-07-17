import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-ui-multiselect-widget/src/jquery.multiselect';
import 'floatthead/dist/jquery.floatThead';

import { floatThead, initDatepicker, initFromDateOnChange, setDateRange } from '../../shared/common.functions';
import '../../shared/employee.search.in.team';
import '../../date';

$(document).ready(function () {
    floatThead('table#data');

    initDatepicker();
    initFromDateOnChange($('#FromDate'), $('#ToDate'));
    $('#Date').val($('#DateOption').val());
    $('#Date').on('change', function () {
        const value = $(this).val();
        setDateRange(value);
        $('#DateOption').val(value);
    });
    $('#FromDate, #ToDate').on('change', function () {
        $('#Date').val(0);
        $('#DateOption').val(0);
    });

    $('#Section').multiselect({
        minWidth: 170,
        checkAllText: 'Check',
        uncheckAllText: 'Uncheck'
    });

    const sectionIds = $('#SectionIds').val().split(',');
    $('#Section').val(sectionIds);
    $('#Section').multiselect('refresh');

    $('#Section').on('change', function () {
        const sectionIds = $(this).val() !== null ? $(this).val().join(',') : '';
        $('#SectionIds').val(sectionIds);
    });

    $('#btnExcel').on('click', function () {
        window.location.href = _global.root + 'WorkingHoursSummaryReport/ExportExcel?' + $('form.filter').serialize();
    });
});