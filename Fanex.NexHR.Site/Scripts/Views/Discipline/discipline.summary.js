'use strict';
/*global _global, autoIndex, noInformation, floatThead*/

(function () {
    var $disciplineTableBody, $dialogAddOtherDiscipline, $dialogViewOtherDiscipline, $btnAddOtherDiscipline;

    function showSuccessMsg($dialog, msg) {
        $dialog.find('.validation-summary-valid')
            .show().addClass('success').find('ul:first').append('<li>' + msg + '</li>');
    }

    function showErrorMsg($dialog, msg) {
        $dialog.find('.validation-summary-valid')
            .show().addClass('validation-summary-errors').find('ul:first').append(msg);
    }

    function removeMsg($dialog) {
        $dialog.find('.validation-summary-valid')
            .hide().removeClass('success').removeClass('validation-summary-errors').find('ul:first').html("");
    }

    function validateAddOtherDiscipline() {
        var isValid = true,
            errorString = '',
            empId = $dialogAddOtherDiscipline.find('#EmployeeId').val(),
            workDate = $dialogAddOtherDiscipline.find('#WorkDate').datepicker('getDate'),
            disciplineTypeId = $dialogAddOtherDiscipline.find('#DisciplineTypeId').val(),
            score = $dialogAddOtherDiscipline.find('#MinusScore').val(),
            comment = $dialogAddOtherDiscipline.find('#Comment').val();

        if (empId && parseInt(empId, 10) <= 0) {
            errorString += '<li>The Employee field is required.</li>';
        }

        if (workDate === null || workDate === '') {
            errorString += '<li>Discipline Date field is required.</li>';
        }

        if (disciplineTypeId && parseInt(disciplineTypeId, 10) === 0) {
            errorString += '<li>Discipline Type field is required.</li>';
        }

        if (score.trim().length === 0 || parseFloat(score) <= 0) {
            errorString += '<li>Score must be greater than 0.</li>';
        }

        if (comment.trim().length === 0) {
            errorString += '<li>Comment field is required.</li>';
        }

        if (errorString.length > 0) {
            showErrorMsg($dialogAddOtherDiscipline, errorString);
            isValid = false;
        }

        return isValid;
    }

    function initAutocomplete($dialog) {
        var $selectorName = $dialog.find('#Employee'),
            $selectorId = $dialog.find('#EmployeeId');
        $selectorName.autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: _global.root + 'discipline/GetMemberToAddOtherDiscipline',
                    data: {
                        keyword: request.term
                    },
                    success: function (data) {
                        response($.map(data, function (item) {
                            return {
                                label: item.NickName + ' (' + item.FullName + ')',
                                value: item.Id
                            };
                        }));
                    }
                });
            },
            minLength: 2,
            select: function (event, ui) {
                event.preventDefault();
                $selectorName.val(ui.item.label);
                $selectorId.val(ui.item.value);
            },
            focus: function (event, ui) {
                event.preventDefault();
                $selectorName.val(ui.item.label);
                $selectorId.val(ui.item.value);
            },
            change: function (event, ui) {
                event.preventDefault();
                if (ui && ui.item) {
                    $selectorName.val(ui.item.label);
                    $selectorId.val(ui.item.value);
                } else {
                    if ($selectorName.val() === '') {
                        $selectorId.val('');
                    } else {
                        $selectorId.val('0');
                    }
                }
            }
        }).autocomplete('widget').addClass('search-employee-autocomplete');
    }

    function initDatePicker($dialog) {
        var mindate = $dialog.find('.datepicker').attr('mindate');

        $dialog.find('.datepicker').datepicker({
            dateFormat: 'dd M, yy',
            showOn: 'button',
            buttonImageOnly: true,
            changeMonth: true,
            changeYear: true,
            buttonImage: _global.root + 'images/calendar.png'
        });

        if (mindate !== 'null') {
            mindate = mindate.split('/');
            $dialog.find('.datepicker').datepicker('option', 'minDate', new Date(mindate[0], mindate[1] - 1, mindate[2]));
        }
    }

    function initEventForOtherDisciplineDropdown() {
        $dialogAddOtherDiscipline.find('select#DisciplineTypeId').on('change', function () {
            var score = $('option:selected', this).data('score');
            $dialogAddOtherDiscipline.find('#MinusScore').val(score);
        });
    }

    function initAddOtherDisciplineDialog() {
        initAutocomplete($dialogAddOtherDiscipline);
        initDatePicker($dialogAddOtherDiscipline);
        initEventForOtherDisciplineDropdown();
        $dialogAddOtherDiscipline.find('select#DisciplineTypeId').change();

        $dialogAddOtherDiscipline.dialog({
            modal: true,
            title: 'Add Other Discipline',
            buttons: [
                {
                    text: 'Submit',
                    click: function () {
                        removeMsg($dialogAddOtherDiscipline);
                        if (validateAddOtherDiscipline()) {
                            $.ajax({
                                type: 'POST',
                                url: _global.root + 'discipline/AddOtherDiscipline',
                                data: $('#frmAddOtherDiscipline').serialize(),
                                success: function (result) {
                                    if (result.IsSuccessful) {
                                        showSuccessMsg($dialogAddOtherDiscipline, 'Add other discipline successfully!');
                                        $('#frmAddOtherDiscipline').find('#IsSuccessful').prop('checked', true);
                                    } else {
                                        showErrorMsg($dialogAddOtherDiscipline, '<li>' + result.Message + '</li>');
                                    }
                                }
                            });
                        }
                    }
                },
                {
                    text: 'Cancel',
                    click: function () {
                        $(this).dialog('close');
                    }
                }
            ],
            close: function () {
                if ($('#frmAddOtherDiscipline').find('#IsSuccessful').prop('checked')) {
                    query();
                }
            },
            show: {
                effect: 'fade',
                duration: 300
            },
            hide: {
                effect: 'fade',
                duration: 200
            },
            minWidth: 460,
            dialogClass: 'modal-shadow'
        });
    }

    function initEventForTeamDropdown() {
        $('form.form-filter #TeamId').on('change', function () {
            var teamId = $('form.form-filter #TeamId').val();

            if (teamId.length > 0) {
                $('form.form-filter #TeamName').val($('form.form-filter #TeamId option:selected').text());
            }
        });
    }

    function initEventViewOtherDiscipline() {
        $disciplineTableBody.find('a.viewOtherDiscipline').on('click', function () {
            var empId = parseInt($(this).data('empid'), 10),
                nickname = $(this).data('nickname');

            if (empId > 0) {
                $.ajax({
                    type: 'GET',
                    url: _global.root + 'discipline/ViewOtherDiscipline',
                    dataType: 'html',
                    data: {
                        employeeId: empId,
                        fromDate: $('form.form-filter #FromDate').datepicker({ dateFormat: 'dd M, yy' }).val(),
                        toDate: $('form.form-filter #ToDate').datepicker({ dateFormat: 'dd M, yy' }).val()
                    },
                    success: function (data) {
                        $dialogViewOtherDiscipline.html(data);
                        initEventForDeleteOtherDiscipline();
                        $dialogViewOtherDiscipline.dialog({
                            modal: true,
                            title: 'Other Discipline - ' + nickname,
                            close: function () {
                                if ($dialogViewOtherDiscipline.find('#IsSuccessful').prop('checked')) {
                                    query();
                                }
                            },
                            show: {
                                effect: 'fade',
                                duration: 300
                            },
                            hide: {
                                effect: 'fade',
                                duration: 200
                            },
                            minWidth: 500,
                            dialogClass: 'modal-shadow'
                        });
                    }
                });
            }
        });
    }

    function initEventForViewDisciplineDetail() {
        $disciplineTableBody.find('a.viewDisciplineDetail').on('click', function () {
            var empId = $(this).data('empid'),
                employee = $(this).data('employee'),
                teamId = $('form.form-filter #TeamId').val(),
                isLocal = $('form.form-filter #IsLocal').val(),
                reasonId = $('form.form-filter #ReasonId').val(),
                fromDate = $('form.form-filter #FromDate').datepicker({ dateFormat: 'dd M, yy' }).val(),
                toDate = $('form.form-filter #ToDate').datepicker({ dateFormat: 'dd M, yy' }).val();

            window.open(_global.root + 'discipline/DisciplineDetail?fromDate=' + fromDate + '&toDate=' + toDate
                + '&employeeId=' + empId + '&employee=' + employee + '&teamId=' + teamId
                + '&isLocal=' + isLocal + '&reasonId=' + reasonId + '&fromSummaryPage=true', '_blank');
        });
    }

    function initEventForDeleteOtherDiscipline() {
        $dialogViewOtherDiscipline.find('a.deleteOtherDiscipline').on('click', function () {
            var empId = $(this).data('empid'),
                workdate = $(this).data('workdate'),
                disciplineTypeId = $(this).data('disciplinetype'),
                rowIndex = $(this).data('row-index');

            confirm('Are you sure you want to delete this other discipline?', function () {
                $.post(_global.root + 'discipline/DeleteOtherDiscipline', {
                    employeeId: empId,
                    workDate: workdate,
                    disciplineTypeId: disciplineTypeId
                }, function () {
                    $dialogViewOtherDiscipline.find('#IsSuccessful').prop('checked', true);
                    $('#row_' + rowIndex).remove();
                    autoIndex();

                    if ($dialogViewOtherDiscipline.find('table.table tbody > tr').length === 0) {
                        $(noInformation(6)).appendTo('#dialogViewOtherDiscipline table.table');
                    }
                });
            });
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

    function initEventAddOtherDiscipline() {
        $btnAddOtherDiscipline.on('click', function (event) {
            event.preventDefault();

            $.ajax({
                type: 'GET',
                url: _global.root + 'discipline/PartialAddOtherDiscipline',
                dataType: 'html',
                success: function (data) {
                    $dialogAddOtherDiscipline.html(data);
                    initAddOtherDisciplineDialog();
                }
            });
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
            url: _global.root + 'discipline/PartialListDisciplineSummary',
            data: $('form.form-filter').serialize(),
            success: function (data) {
                $disciplineTableBody.html(data);

                initEventViewOtherDiscipline();
                initEventForViewDisciplineDetail();
            }
        });
    }

    $(document).ready(function () {
        $disciplineTableBody = $('#disciplineTableBody');
        $dialogAddOtherDiscipline = $('#dialogAddOtherDiscipline');
        $dialogViewOtherDiscipline = $('#dialogViewOtherDiscipline');
        $btnAddOtherDiscipline = $('#btnAddOtherDiscipline');

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
            window.location.href = _global.root + 'ExportDiscipline/ExportDisciplineSummaryReport?' + $('form.form-filter').serialize();
        });

        floatThead("table#data");
        initEventAddOtherDiscipline();
        initEventAddWeekendLeave();
        query();
    });
})();