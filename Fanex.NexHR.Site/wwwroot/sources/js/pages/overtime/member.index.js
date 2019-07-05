import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'jquery-ui-multiselect-widget/src/jquery.multiselect';
import 'floatthead/dist/jquery.floatThead';
import '../../../libs/monthPicker/monPickr-0.0.3';
import axios from 'axios';

import { axiosError, blockUI, unblockUI, floatThead } from '../../shared/common.functions';
import '../../shared/employee.search.in.team';

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
            $('#Month').val(`01-${month}-${year}`);
        }
    });

    $('.monpickr-dropdown').hide();

    if ($('#Month').val() === '') {
        $('.monpickr-selected-month.monpickr-input.ui-widget-content').val('');
    }

    $('#Section').multiselect({
        minWidth: 180,
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

    floatThead('table#data');
});