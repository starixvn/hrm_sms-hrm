import 'jquery-ui-bundle/jquery-ui';
import axios from 'axios';

import '../../shared/multiselect-tree-view';
import { axiosError, initDatepicker, resolveClientUrl } from '../../shared/common.functions';

$(document).ready(function () {
    initDatepicker();

    axios(resolveClientUrl('Department/GetHierarchy'))
        .then(response => {
            $('#Section').multiSelectTreeView({
                data: response.data,
                isSelectedAll: true
            });
        }).catch(axiosError);

    $('#btnExcel').on('click', function () {
        $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        window.location.href = resolveClientUrl('DailyWorkingHours/ExportExcel?' + $('form.filter').serialize());
    });
});