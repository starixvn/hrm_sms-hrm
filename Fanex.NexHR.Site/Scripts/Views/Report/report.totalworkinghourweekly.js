'use strict';
/*global _global, _messages, floatThead*/

(function () {
    function query() {
        $.ajax({
            url: _global.root + 'report/TotalWorkingHourWeeklyDetails',
            type: 'POST',
            dataType: 'html',
            data: $('form.form-filter').serialize(),
            success: function (response) {
                $('#TotalWorkingHourWeeklyDetails').html(response);
                floatThead('table#data');
            },
            error: function () {
                alert(_messages.errorTryAgain);
            }
        });
    }

    $(document).ready(function () {
        $("#section").multiselect();
        $("#FromDate, #ToDate").attr('readonly', true);

        $('form.form-filter #section').on('change', function () {
            var sectionIds = $(this).val() !== null ? $(this).val().join(',') : '';
            $('#SectionIds').val(sectionIds);
        });

        $("#btnSubmit").on('click', function () {
            query();
        });

        $("#btnExcel").on('click', function () {
            window.location.href = _global.root + 'ExportTotalWorkingHour/ExportTotalWorkingHourWeekly?' + $('form.form-filter').serialize();
        });

        query();
    });
})();