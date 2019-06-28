import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-ui-multiselect-widget/src/jquery.multiselect';

import { initDatepicker } from '../../shared/common.functions';
import '../../shared/employee.search.in.team';

$(document).ready(function () {
    initDatepicker();

    $('#Section').multiselect({
        minWidth: 180,
        checkAllText: 'Check',
        uncheckAllText: 'Uncheck'
    });

    $('#Section').on('change', function () {
        const sectionIds = $(this).val() !== null ? $(this).val().join(',') : '';
        $('#SectionIds').val(sectionIds);
    });

    $('#btnExcel').on('click', function () {
        window.location.href = _global.root + 'DailyWorkingHours/ExportExcel?' + $('form.filter').serialize();
    });
});