import 'jquery-ui-bundle/jquery-ui';
import 'floatthead/dist/jquery.floatThead';
import '../../shared/multiselect-tree-view';
import '../../shared/employee.search.in.team';
import { floatThead, alert, resolveClientUrl } from '../../shared/common.functions';

(function () {
    function query() {
        $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        $.ajax({
            url: resolveClientUrl('report/EmployeeSupervisorDetails'),
            type: 'POST',
            dataType: 'html',
            data: $('form.filter').serialize(),
            success: function (response) {
                $('#EmployeeSupervisorDetails').html(response);
                floatThead('table#data');
            },
            error: function(jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
            }
        });
    }

    $(document).ready(function () {
        $.ajax({
            url: resolveClientUrl('Department/GetHierarchy'),
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
            window.location.href = resolveClientUrl('ExportSupervisor/EmployeeSupervisor?' + $('form.filter').serialize());
        });
    });
})();