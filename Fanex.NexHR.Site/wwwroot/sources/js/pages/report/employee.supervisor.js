import $ from 'jquery/dist/jquery';
import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import { floatThead, alert } from '../../shared/common.functions';

(function () {
    function query() {
        $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        $.ajax({
            url: _global.root + 'report/EmployeeSupervisorDetails',
            type: 'POST',
            dataType: 'html',
            data: $('form.filter').serialize(),
            success: function (response) {
                $('#EmployeeSupervisorDetails').html(response);

                floatThead('table#data');
            },
            error: function () {
                alert(_messages.errorTryAgain);
            }
        });
    }

    $(document).ready(function () {
        $.ajax({
            url: _global.root + 'Department/GetHierarchy',
            success: function (data) {
                $('#Section').multiSelectTreeView({
                    data: data,
                    isSelectedAll: true
                });

                query();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });

        $('#category').on('change', function () {
            $('#IsLocal').val($(this).val());
        });

        $("#btnSubmit").on('click', function () {
            query();
        });

        $('#btnExcel').on('click', function () {
            $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
            window.location.href = _global.root + 'ExportSupervisor/EmployeeSupervisor?' + $('form.filter').serialize();
        });
    });
})();