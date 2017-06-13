'use strict';
/*global _global*/

(function () {
    $(document).ready(function () {
        $("#section").multiselect();

        $('form.form-filter #section').on('change', function () {
            var sectionIds = $(this).val() !== null ? $(this).val().join(',') : '';
            $('#SectionIds').val(sectionIds);
        });

        $("#btnExcel").on('click', function () {
            window.location.href = _global.root + 'ExportTotalWorkingHour/ExportTotalWorkingHourDaily?' + $('form.form-filter').serialize();
        });
    });
})();