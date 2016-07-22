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
                $(this).closest('tr').find('td.approverSubtractScore span').text(-approverSubstractScore.toFixed(1));
                $(this).closest('tr').find('td.approverSubtractScore span').css('color', 'red');
            } else {
                $(this).closest('tr').find('td.approverSubtractScore span').text('0.0');
                $(this).closest('tr').find('td.approverSubtractScore span').css('color', 'inherit');
            }
        });

        $('#discipline-detail table.discipline-detail td input.violationRecorded:checkbox')
        .on('click', function () {
            var hasConfirmed = $(this).data('confirmed').toLowerCase() === 'true';

            if ($(this).prop('checked') !== hasConfirmed) {
                $(this).data('changed', true);
            } else {
                $(this).data('changed', false);
            }
        });
    }

    function getDataChanged() {
        var ignoredDisciplines = [];

        $('#discipline-detail table.discipline-detail').find('tbody tr').each(function () {
            var approverSubstractScore = parseFloat($(this).find('td.approverSubtractScore span').text()),
                isChanged = false;

            $(this).find('td input.ignoredDiscipline').each(function () {
                if ($(this).data('changed')) {
                    isChanged = true;
                }
            });

            if (isChanged) {
                var ignoredDiscipline = {
                    WorkDate: '',
                    EmployeeId: false,
                    IsIgnoredChangeShift: false,
                    IsIgnoredMissingCheckInOut: false,
                    IsIgnoredLateIn: false,
                    IsIgnoredEarlyOut: false,
                    IsIgnoredAbsent: false,
                    IsIgnoredUnpaidLeave: false,
                    IsIgnoredWorkingUnderTime: false,
                    IsIgnoredUnpaidLeaveAbsent: false,
                    ApproverSubtractScore: Math.abs(approverSubstractScore)
                };

                $(this).find('td input.ignoredDiscipline').each(function () {
                    var disciplineTypeId = $(this).data('discipline-type-id');
                    ignoredDiscipline.WorkDate = $(this).data('workdate');
                    ignoredDiscipline.EmployeeId = $(this).data('employee-id');

                    switch (disciplineTypeId) {
                        case 1:
                            ignoredDiscipline.IsIgnoredChangeShift = $(this).prop('checked');
                            break;
                        case 2:
                            ignoredDiscipline.IsIgnoredMissingCheckInOut = $(this).prop('checked');
                            break;
                        case 3:
                            ignoredDiscipline.IsIgnoredLateIn = $(this).prop('checked');
                            break;
                        case 4:
                            ignoredDiscipline.IsIgnoredEarlyOut = $(this).prop('checked');
                            break;
                        case 5:
                            ignoredDiscipline.IsIgnoredAbsent = $(this).prop('checked');
                            break;
                        case 6:
                            ignoredDiscipline.IsIgnoredUnpaidLeave = $(this).prop('checked');
                            break;
                        case 7:
                            ignoredDiscipline.IsIgnoredWorkingUnderTime = $(this).prop('checked');
                            break;
                        case 8:
                            ignoredDiscipline.IsIgnoredUnpaidLeaveAbsent = $(this).prop('checked');
                            break;
                    }
                });

                ignoredDisciplines.push(ignoredDiscipline);
            }
        });

        return ignoredDisciplines;
    }

    function initEventForSaveChange() {
        $('#discipline-detail table.discipline-detail input#saveChange').on('click', function () {
            var ignoredDisciplines = getDataChanged();

            if (ignoredDisciplines.length > 0) {
                ignoredDisciplines = JSON.stringify({ 'ignoredDisciplineDetails': ignoredDisciplines });

                confirm('Are you sure you want to save?',
                    function () {
                        $.ajax({
                            type: 'POST',
                            dataType: 'json',
                            url: _global.root + 'discipline/UpdateIgnoredDiscipline',
                            data: ignoredDisciplines,
                            contentType: 'application/json; charset=utf-8',
                            success: function (result) {
                                if (result.IsSuccessful) {
                                    alert('Updated Successfully!');
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
        });
    }

    function query() {
        $.ajax({
            url: _global.root + 'discipline/PartialListDisciplineDetail',
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