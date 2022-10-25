import 'jquery-ui-bundle/jquery-ui';
import 'tipsy/src/javascripts/jquery.tipsy';
import 'floatthead/dist/jquery.floatThead';
import axios from 'axios';
import { parse } from 'qs';
import moment from 'moment';

import '../../date';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import {
    axiosError, blockUI, floatThead, unblockUI, alert, initDatepicker,
    resolveClientUrl, downloadFile
} from '../../shared/common.functions';

(function () {
    var $disciplineTable;

    function initEventForViewDisciplineDetail() {
        $disciplineTable.find('a.viewDisciplineDetail').on('click', function () {
            var year = parseInt($(this).data('year'), 10),
                empId = $(this).data('empid'),
                employee = $(this).data('employee'),
                teamId = $('form.form-filter #TeamId').val(),
                isLocal = $('form.form-filter #IsLocal').val(),
                fromDate = moment(new Date(year, 0, 1)).format('DD MMM, YYYY'),
                toDate = moment(new Date(year + 1, 0, 1)).subtract(1, 'days').format('DD MMM, YYYY');

            window.open(resolveClientUrl('discipline/DisciplineDetail?fromDate=' + fromDate + '&toDate=' + toDate
                + '&employeeId=' + empId + '&employee=' + employee + '&teamId=' + teamId
                + '&isLocal=' + isLocal + '&hasIgnored=false'), '_blank');
        });
    }

    function query() {
        blockUI();
        $.ajax({
            url: resolveClientUrl('disciplineSummaryPerYear/getDisciplineSummaryGrid'),
            type: 'POST',
            data: $('form.form-filter').serialize(),
            success: function (data) {
                $disciplineTable.html(data);
                floatThead("table#data");
                initEventForViewDisciplineDetail();
                unblockUI();
            }
        });
    }

    $(document).ready(function () {
        $disciplineTable = $('#discipline-summary-table');
        initDatepicker();
        $.ajax({
            url: resolveClientUrl('Section/GetOrgChart'),
            success: function (data) {
                $('#Team').multiSelectTreeView({
                    data: data,
                    isSelectedAll: true,
                    hasSearchBox: true
                });
                $('#TeamId').val($('#Team').multiSelectTreeView('getSelectedItems'));

                query();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });

        $('form.form-filter #submit').on('click', function (event) {
            $('#TeamId').val($('#Team').multiSelectTreeView('getSelectedItems'));
            query();
        });

        $('form.form-filter #FromYear').on('change', function () {
            var fromYear = parseInt($(this).val());
            $('form.form-filter #ToYear').val(fromYear + 4);
        });

        $('form.form-filter #ToYear').on('change', function () {
            var toYear = parseInt($(this).val());
            $('form.form-filter #FromYear').val(toYear - 4);
        });

        $('form.form-filter #btnExcel').on('click', function (event) {
            event.preventDefault();

            blockUI();
            axios({
                url: resolveClientUrl('disciplineSummaryPerYear/ExportExcel'),
                method: 'GET',
                params: parse($('form.form-filter').serialize()),
                responseType: 'blob'
            }).then(response => {
                const fileName = response.headers['content-disposition'].match(/filename=(.+)/)[1];
                downloadFile(response.data, fileName);
            }).catch(axiosError).then(() => unblockUI());
        });
    });
})();