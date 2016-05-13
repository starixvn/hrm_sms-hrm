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

    function initEventForIgnoreDiscipline() {
        $('#discipline-detail table.discipline-detail td input.ignoredDiscipline:checkbox')
        .on('click', function () {
            var disciplineScore = parseFloat($(this).data('discipline-score')),
                approverSubstractScore = parseFloat($(this).closest('tr').find('td.approverSubtractScore span').text()),
                hasIgnored = $(this).data('has-ignored').toLowerCase() === 'true';

            if ($(this).prop('checked')) {
                approverSubstractScore = Math.abs(approverSubstractScore) - disciplineScore;
            } else {
                approverSubstractScore = Math.abs(approverSubstractScore) + disciplineScore;
            }

            if ($(this).prop('checked') !== hasIgnored) {
                $(this).data('changed', true);
            } else {
                $(this).data('changed', false);
            }

            if (approverSubstractScore > 0) {
                $(this).closest('tr').find('td.approverSubtractScore span').text(-approverSubstractScore);
                $(this).closest('tr').find('td.approverSubtractScore span').css('color', 'red');
            } else {
                $(this).closest('tr').find('td.approverSubtractScore span').text('0.0');
                $(this).closest('tr').find('td.approverSubtractScore span').css('color', 'inherit');
            }
        });
    }

    function getDataChanged() {
        var ignoredDisciplines = [];

        $('#discipline-detail table.discipline-detail').find('td input.ignoredDiscipline').each(function () {
            if ($(this).data('changed')) {
                var ignoredDiscipline = {
                    WorkDate: $(this).data('workdate'),
                    EmployeeId: $(this).data('employee-id'),
                    DisciplineTypeId: $(this).data('discipline-type-id'),
                    Ignored: $(this).prop('checked')
                };

                ignoredDisciplines.push(ignoredDiscipline);
            }
        });

        return ignoredDisciplines;
    }

    function initEventForSaveChange() {
        $('#discipline-detail table.discipline-detail input#saveChange').on('click', function () {
            var ignoredDisciplines = getDataChanged();
            ignoredDisciplines = JSON.stringify({ 'ignoredDisciplineDetails': ignoredDisciplines });

            $.ajax({
                type: 'POST',
                dataType: 'json',
                url: _global.root + 'discipline/UpdateIgnoredDiscipline',
                data: ignoredDisciplines,
                contentType: 'application/json; charset=utf-8',
                success: function (result) {
                    if (result.Code === 0) {
                        query();
                    } else {
                        alert(result.Message);
                    }
                },
                error: function (jqXHR, textStatus, errorThrown) {
                    alert(textStatus + ': ' + errorThrown);
                }
            });
        });
    }

    function query() {
        $.ajax({
            url: _global.root + 'discipline/ListDisciplineDetail',
            data: $('form.form-filter').serialize(),
            success: function (data) {
                $('#discipline-detail').html(data);
                initEventForIgnoreDiscipline();
                initEventForSaveChange();
            },
            error: function (jqXHR, textStatus, errorThrown) {
                alert(textStatus + ': ' + errorThrown);
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