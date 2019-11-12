import 'jquery-ui-bundle/jquery-ui';
import axios from 'axios';

import '../../shared/multiselect-tree-view';
import { axiosError, blockUI, unblockUI, initDatepicker, initFromDateOnChange, resolveClientUrl } from '../../shared/common.functions';

$(document).ready(function () {
    const query = function () {
        blockUI();

        axios({
            url: resolveClientUrl('ShiftStatisticReport/ShiftStatistic'),
            method: 'GET',
            params: {
                fromDate: $('#FromDate').val(),
                toDate: $('#ToDate').val(),
                sectionIds: $('#SectionIds').val(),
                isLocal: $('#Category').val(),
                IsShowWholeCompany: $('#IsShowWholeCompany').prop('checked')
            },
            responseType: 'text'
        }).then(response => {
            $('#shiftstatisticdetails').html(response.data);
        }).catch(axiosError).then(() => unblockUI());
    };

    initDatepicker();
    initFromDateOnChange($('#FromDate'), $('#ToDate'));

    axios(resolveClientUrl('Department/GetHierarchy'))
        .then(response => {
            $('#Section').multiSelectTreeView({
                data: response.data,
                isSelectedAll: true
            });

            $('#btnSearch').trigger('click');
        }).catch(axiosError);

    $('#btnSearch').on('click', function () {
        $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        query();
    });

    $('#btnExcel').on('click', function () {
        $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        window.location.href = resolveClientUrl('ShiftStatisticReport/ExportExcel?' + $('form.filter').serialize());
    });
});