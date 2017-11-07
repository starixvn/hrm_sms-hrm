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
        $('form.form-filter #section').multiselect();

        query();

        $('form.form-filter #section').on('change', function () {
            var sectionIds = $(this).val() !== null ? $(this).val().join(',') : '';
            $('#SectionIds').val(sectionIds);
        });

        $('form.form-filter #category').on('change', function () {
            $('form.form-filter #IsLocal').val($(this).val());
        });

        $("form.form-filter #btnSubmit").on('click', function () {
            query();
        });

        $('form.form-filter #btnExcel').on('click', function () {
            window.location.href = _global.root + 'ExportSupervisor/EmployeeSupervisor?' + $('form.form-filter').serialize();
        });
    });
})();