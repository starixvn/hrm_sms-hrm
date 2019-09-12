import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import axios from 'axios';

import '../../shared/multiselect-tree-view';
import { axiosError, initDatepicker, initFromDateOnChange } from '../../shared/common.functions';

$(document).ready(function () {
    initDatepicker();
    initFromDateOnChange($('#FromDate'), $('#ToDate'));

    axios(_global.root + 'Department/GetHierarchy')
        .then(response => {
            $('#Section').multiSelectTreeView({
                data: response.data,
                isSelectedAll: true
            });
        }).catch(axiosError);

    $('#btnExcel').on('click', function () {
        $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        window.location.href = _global.root + 'WorkingCalendarSummaryReport/ExportExcel?' + $('form.filter').serialize();
    });
});