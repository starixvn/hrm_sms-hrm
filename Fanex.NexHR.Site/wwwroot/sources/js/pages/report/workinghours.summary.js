import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';

import '../../shared/multiselect-tree-view';
import { axiosError, floatThead, initDatepicker, initFromDateOnChange, setDateRange } from '../../shared/common.functions';
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

    axios(_global.root + 'Department/GetHierarchy')
        .then(response => {
            const sectionIds = $('#SectionIds').val();
            console.log(sectionIds);
            $('#Section').multiSelectTreeView({
                data: response.data,
                selectedItems: sectionIds
            });
        }).catch(axiosError);

    $('#btnSearch').on('click', function () {
        $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        $('form.filter').submit();
    });

    $('#btnExcel').on('click', function () {
        $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        window.location.href = _global.root + 'WorkingHoursSummaryReport/ExportExcel?' + $('form.filter').serialize();
    });
});