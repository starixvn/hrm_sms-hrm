'use strict';
/*global _global, _messages, floatThead*/

(function () {
    function query() {
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
        $('#group').multiselect();

        $('#group').on('change', function () {
            var groupIds = $(this).val() !== null ? $(this).val().join(',') : '';
            $('#GroupIds').val(groupIds);
        });

        $('#category').on('change', function () {
            $('#IsLocal').val($(this).val());
        });

        $("#submit").on('click', function () {
            query();
        });

        $('form.form-filter #excel').on('click', function () {
            window.location.href = _global.root + 'Export/ExportEmployeeSupervisor?' + $('form.form-filter').serialize();
        });
    });
})();