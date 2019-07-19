import $ from 'jquery/dist/jquery';
import 'floatthead/dist/jquery.floatThead';

import { floatThead } from '../../shared/common.functions';

$(document).ready(function () {
    floatThead('table#data');

    $('#btnExcel').on('click', function () {
        window.location.href = _global.root + 'ManpowerReport/ExportManpower?' + $('form.filter').serialize();
    });
});