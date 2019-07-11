import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-ui-multiselect-widget/src/jquery.multiselect';
import 'floatthead/dist/jquery.floatThead';

import { floatThead, initDatepicker, initFromDateOnChange } from '../../shared/common.functions';
import '../../shared/employee.search.in.team';

$(document).ready(function () {
    initDatepicker();
    initFromDateOnChange($('#FromDate'), $('#ToDate'));
    floatThead('table#data');

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
        window.location.href = _global.root + 'OvertimeReport/ExportDailyOvertime?' + $('form.filter').serialize();
    });
});