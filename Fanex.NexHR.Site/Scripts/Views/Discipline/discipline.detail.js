'use strict';
/*global _global, floatThead*/

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

        $('#discipline-detail table.discipline-detail td input.outOfControlReason:checkbox')
        .on('click', function () {
            var changed = !$(this).data('changed');

            $(this).data('changed', changed);
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

            if (!isChanged)
            {
                if ($('td input.outOfControlReason').first().data('changed')) {
                    isChanged = true;
                }
            }

            if (isChanged) {
                var ignoredDiscipline = {
                    WorkDate: '',
                    EmployeeId: false,
                    IsIgnoredChangeShift: false,
                    IsIgnoredMissingCheckInOut: false,
                    IsIgnoredLateIn: false,
                    IsIgnoredEarlyOut: false,
                    IsIgnoredAbsent: false,
                    IsIgnoredUnplannedLeave: false,
                    IsIgnoredUnpaidLeave: false,
                    IsIgnoredWorkingUnderTime: false,
                    IsIgnoredUnpaidLeaveAbsent: false,
                    IsIgnoredWeekendLeave: false,
                    ApproverSubtractScore: Math.abs(approverSubstractScore),
                    IsOutOfControlReason: false
                };

                ignoredDiscipline.IsOutOfControlReason = $(this).find('td input.outOfControlReason').prop('checked');

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
                        case 9:
                            ignoredDiscipline.IsIgnoredUnplannedLeave = $(this).prop('checked');
                            break;
                        case 10:
                            ignoredDiscipline.IsIgnoredWeekendLeave = $(this).prop('checked');
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

    function initEventForDateRangeDropdown() {
        $('form.form-filter #DateRange').on('change', function () {
            var dateRange = $('form.form-filter #DateRange').val();

            if (parseInt(dateRange) !== 0) {
                var quarter = dateRange.charAt(0);
                var year = dateRange.substring(1, dateRange.length);

                var fromDate = new Date($('form.form-filter #FromDate').val());
                var toDate = new Date($('form.form-filter #ToDate').val());
                switch (parseInt(quarter)) {
                    case 1: // First Half Year 1H
                        fromDate.setFullYear(parseInt(year), 0, 1);
                        toDate.setFullYear(parseInt(year), 5, 30);
                        break;

                    case 2: // Second Half Year 2H
                        fromDate.setFullYear(parseInt(year), 6, 1);
                        toDate.setFullYear(parseInt(year), 11, 31);
                        break;

                    default:
                        fromDate.setFullYear(parseInt(year), 0, 1);
                        toDate.setFullYear(parseInt(year), 11, 31);
                        break;
                }

                $('form.form-filter #FromDate').val(formatDate(fromDate));
                $('form.form-filter #ToDate').val(formatDate(toDate));
            }
        });
    }

    function formatDate(date) {
        var monthNames = [
          "Jan", "Feb", "Mar",
          "Apr", "May", "Jun", "Jul",
          "Aug", "Sep", "Oct",
          "Nov", "Dec"
        ];

        var day = date.getDate();
        var monthIndex = date.getMonth();
        var year = date.getFullYear();

        return (day < 10 ? "0" + day : day) + ' ' + monthNames[monthIndex] + ', ' + year;
    }

    function query() {
        $.ajax({
            url: _global.root + 'discipline/PartialListDisciplineDetail',
            data: $('form.form-filter').serialize(),
            success: function (data) {
                $('#discipline-detail').html(data);

                floatThead("table#data");
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

        initEventForDateRangeDropdown();
        $('form.form-filter #FromDate, form.form-filter #ToDate').on('change', function () {
            $('form.form-filter #DateRange').val('0');
        });

        $('form.form-filter #submit').on('click', function (event) {
            event.preventDefault();
            query();
        });

        $('form.form-filter #btnExcel').on('click', function (event) {
            event.preventDefault();
            window.location.href = _global.root + 'ExportDiscipline/ExportDisciplineDetailReport?' + $('form.form-filter').serialize();
        });

        query();
    });
})();