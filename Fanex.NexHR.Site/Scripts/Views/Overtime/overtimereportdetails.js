'use strict';
/*global _global, _messages, floatThead*/

(function () {
    function query() {
        $.ajax({
            url: _global.root + 'overtime/OTExcelReportDetails',
            type: 'POST',
            dataType: 'html',
            data: $('form.form-filter').serialize(),
            success: function (response) {
                $('#OTExcelReportDetails').html(response);

                floatThead('table#data');
            },
            error: function () {
                alert(_messages.errorTryAgain);
            }
        });
    }

    $(document).ready(function () {
        $('#section').multiselect();
        $("#title").multiselect({
            selectedText: '# selected',
            selectedList: 0
        });

        $('#fromDate, #toDate').attr('readonly', 'readonly');

        $('form.form-filter #section').on('change', function () {
            var sectionIds = $(this).val() !== null ? $(this).val().join(',') : '';
            $('#SectionIds').val(sectionIds);
        });

        $('form.form-filter #title').on('change', function () {
            var jobTitleIds = $(this).val() !== null ? $(this).val().join(',') : '';
            $('#JobTitleIds').val(jobTitleIds);
        });

        $('form.form-filter #category').on('change', function () {
            $('form.form-filter #IsLocal').val($(this).val());
        });

        query();

        $("#btnSubmit").click(function () {
            query();
        });

        $("#btnExcel").click(function () {
            window.location.href = _global.root + "export/OvertimeReport?" + $('form.form-filter').serialize();
        });
    });
})();