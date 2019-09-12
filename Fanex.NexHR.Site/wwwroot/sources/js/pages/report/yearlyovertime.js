import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';

import '../../shared/multiselect-tree-view';
import { axiosError, floatThead } from '../../shared/common.functions';

$(document).ready(function () {
    floatThead('table#data');

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
        window.location.href = _global.root + 'OvertimeReport/ExportYearlyOvertime?' + $('form.filter').serialize();
    });
});