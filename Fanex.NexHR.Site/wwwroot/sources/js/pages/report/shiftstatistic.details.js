import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';

import '../../shared/employee.search.in.team';
import '../../shared/multiselect-tree-view';
import { axiosError, blockUI, unblockUI, floatThead, initDatepicker, resolveClientUrl } from '../../shared/common.functions';

$(document).ready(function () {
    const query = function () {
        blockUI();

        axios({
            url: resolveClientUrl('ShiftStatisticReport/ShiftStatisticDetails'),
            method: 'GET',
            params: {
                workdate: $('#WorkDate').val(),
                shiftId: $('#ShiftId').val(),
                employeeId: $('#EmployeeId').val(),
                sectionIds: $('#SectionIds').val(),
                isLocal: $('#IsLocal').val(),
                IsShowWholeCompany: $('#IsShowWholeCompany').prop('checked')
            },
            responseType: 'text'
        }).then(response => {
            $('#shiftstatisticdetails').html(response.data);
            floatThead('table#data');
        }).catch(axiosError).then(() => unblockUI());
    };

    initDatepicker();

    axios(resolveClientUrl('Department/GetHierarchy'))
        .then(response => {
            $('#Section').multiSelectTreeView({
                data: response.data,
                selectedItems: $('#SectionIds').val()
            });

            $('#btnSearch').trigger('click');
        }).catch(axiosError);

    $('#btnSearch').on('click', function () {
        $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        query();
    });

    $('#btnExcel').on('click', function () {
        $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
    });
});