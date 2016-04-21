'use strict';
/*global _global*/

(function () {
    function initEventForTeamDropdown() {
        $('form.form-filter #TeamId').on('change', function () {
            var teamId = $('form.form-filter #TeamId').val();

            if (teamId.length > 0) {
                $('form.form-filter #TeamName').val($('form.form-filter #TeamId option:selected').text());
            }
        });
    }

    function query() {
        $.ajax({
            url: _global.root + 'discipline/ListDisciplineDetail',
            data: $('form.form-filter').serialize(),
            success: function (data) {
                $('#discipline-detail').html(data);
            }
        });
    }

    $(document).ready(function () {
        initEventForTeamDropdown();

        $('form.form-filter #submit').on('click', function (event) {
            event.preventDefault();
            query();
        });

        $('form.form-filter #btnExcel').on('click', function (event) {
            event.preventDefault();

            window.location.href = _global.root + 'Export/ExportDisciplineDetailReport?' + $('form.form-filter').serialize();
        });

        query();
    });
})();