'use strict';
/*global _global, _messages, floatThead*/

(function () {
    function query() {
        $('#SectionIds').val($('#Section').multiSelectTreeView('getSelectedItems'));
        $.ajax({
            url: _global.root + 'report/EmployeeSupervisorDetails',
            type: 'POST',
            dataType: 'html',
            data: $('form.form-filter').serialize(),
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
            window.location.href = _global.root + 'ExportSupervisor/EmployeeSupervisor?' + $('form.form-filter').serialize();
        });
    });
})();