'use strict';
/*global _global*/

(function () {
    function query() {
        $.ajax({
            url: _global.root + 'report/TotalWorkingHourMonthlyDetails',
            type: 'POST',
            dataType: 'html',
            data: $('form.form-filter').serialize(),
            success: function (response) {
                $('#TotalWorkingHourMonthlyDetails').html(response);
                var weekendColumn = $("table.table th.weekend");
                weekendColumn.each(function (i) {
                    var index = weekendColumn[i].cellIndex;
                    var column = $("table.table td").filter(":nth-child(" + (index + 1) + ")");
                    column.css("background-color", "#bdbdbd");
                });
                floatThead('table#data');
            },
            error: function () {
                alert(_messages.errorTryAgain);
            }
        });
    }

    $(document).ready(function () {
        var dateRangePicker = $('#DateControl');
        dateRangePicker.monPickr({
            showDateRange: false,
            sortDesc: true,
            minDate: $("#MinDate").val(),
            maxDate: $("#MaxDate").val(),
            today: undefined,
            month: undefined,
            afterChangeMonth: function () {
                var month = dateRangePicker.monPickr("getMonth", "MM"),
                    year = dateRangePicker.monPickr("getMonth", "yyyy");
                $('#Month').val(month);
                $('#Year').val(year);
            }
        });

        $(".monpickr-dropdown").hide();
        $("#section").multiselect();

        $('form.form-filter #section').on('change', function () {
            var sectionIds = $(this).val() !== null ? $(this).val().join(',') : '';
            $('#SectionIds').val(sectionIds);
        });

        $("#btnSubmit").on('click', function () {
            query();
        });

        $("#btnExcel").on('click', function () {
            window.location.href = _global.root + 'Export/ExportTotalWorkingHourMonthly?' + $('form.form-filter').serialize();
        });

        query();
    });
})();