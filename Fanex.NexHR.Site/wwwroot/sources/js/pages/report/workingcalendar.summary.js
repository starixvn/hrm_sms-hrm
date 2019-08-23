import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-ui-multiselect-widget/src/jquery.multiselect';

import { initDatepicker, initFromDateOnChange } from '../../shared/common.functions';

$(document).ready(function () {
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

    $('#btnExcel').on('click', function () {
        window.location.href = _global.root + 'WorkingCalendarSummaryReport/ExportExcel?' + $('form.filter').serialize();
    });
});