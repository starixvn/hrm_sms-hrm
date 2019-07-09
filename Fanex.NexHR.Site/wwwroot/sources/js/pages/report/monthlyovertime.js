import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import '../../../libs/monthPicker/monPickr-0.0.3';

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
            $('#Month').val(month);
            $('#Year').val(year);
        }
    });

    $('#btnExcel').on('click', function () {
        window.location.href = _global.root + 'OvertimeReport/ExportMonthlyOvertime?' + $('form.filter').serialize();
    });
});